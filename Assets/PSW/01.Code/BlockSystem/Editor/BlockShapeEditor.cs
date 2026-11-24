using _LumenLib.ServiceLocator;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace PSW._01.Code.BlockSystem.Editor
{
    [CustomEditor(typeof(BlockShape))]
    [CanEditMultipleObjects]
    public class BlockShapeEditor : UnityEditor.Editor
    {
        private const string BlockDataPropName = "blockData";
        private const string WhatIsBlockPropName = "whatIsBlock";
        private const string InitCellUndoName = "Init Cell";

        private SerializedProperty _blockDataProp;
        private Button _initCellButton;
        private HelpBox _missingDataHelpBox;

        public override VisualElement CreateInspectorGUI()
        {
            _blockDataProp = serializedObject.FindProperty(BlockDataPropName);

            var root = new VisualElement();

            // Script field (read-only)
            var scriptField = new PropertyField(serializedObject.FindProperty("m_Script"));
            scriptField.SetEnabled(false);
            root.Add(scriptField);

            // ── Data ──
            root.Add(CreateHeader("Block Data"));
            var blockDataField = new PropertyField(_blockDataProp, "Block Data")
            {
                tooltip = "이 블록 모양을 구성할 셀 정보(BlockDataSO)입니다."
            };
            root.Add(blockDataField);

            // ── Collision ──
            root.Add(CreateHeader("Collision"));
            root.Add(new PropertyField(serializedObject.FindProperty(WhatIsBlockPropName), "What Is Block")
            {
                tooltip = "다른 블록과의 겹침을 검사할 때 사용하는 레이어입니다."
            });

            // ── Editor Tools ──
            root.Add(CreateHeader("Editor Tools"));

            _missingDataHelpBox = new HelpBox(
                "Block Data가 비어 있어 셀을 생성할 수 없습니다.", HelpBoxMessageType.Warning);
            root.Add(_missingDataHelpBox);

            _initCellButton = new Button(OnClickInitCell)
            {
                text = "Init Cell",
                tooltip = "기존 블록 셀과 목록을 정리하고 Block Data에 맞춰 다시 생성합니다.\n" +
                          "새 셀은 BlockSnapper의 셀 크기에 맞게 배치합니다."
            };
            _initCellButton.style.height = 26;
            _initCellButton.style.marginTop = 4;
            root.Add(_initCellButton);

            root.TrackPropertyValue(_blockDataProp, _ => RefreshToolState());
            RefreshToolState();

            return root;
        }

        private static Label CreateHeader(string text)
        {
            var header = new Label(text);
            header.style.unityFontStyleAndWeight = FontStyle.Bold;
            header.style.marginTop = 10;
            header.style.marginBottom = 2;
            header.style.paddingLeft = 3;
            header.style.paddingBottom = 2;
            header.style.borderBottomWidth = 1;
            header.style.borderBottomColor = new Color(0.5f, 0.5f, 0.5f, 0.4f);
            return header;
        }

        private void RefreshToolState()
        {
            if (_initCellButton == null) return;

            serializedObject.UpdateIfRequiredOrScript();
            bool hasData = _blockDataProp.hasMultipleDifferentValues || _blockDataProp.objectReferenceValue != null;

            _initCellButton.SetEnabled(hasData);
            _missingDataHelpBox.style.display = hasData ? DisplayStyle.None : DisplayStyle.Flex;
        }

        private void OnClickInitCell()
        {
            serializedObject.ApplyModifiedProperties();

            // Play 모드: ServiceLocator에 등록된 BlockSnapper / Edit 모드: 씬에서 직접 탐색
            if (!ServiceLocator.TryGet(out BlockSnapper snapper))
                snapper = Object.FindFirstObjectByType<BlockSnapper>(FindObjectsInactive.Include);

            if (snapper == null || snapper.GetComponent<Grid>() == null)
            {
                Debug.LogWarning("[BlockShapeEditor] 열린 씬에서 Grid가 붙은 BlockSnapper를 찾지 못해 InitCell()을 호출할 수 없습니다. " +
                                 "(셀 크기를 BlockSnapper의 Grid에서 가져옵니다)");
                return;
            }

            float cellSize = snapper.GetCellSize().x;
            bool recordUndo = !Application.isPlaying;
            int undoGroup = 0;

            if (recordUndo)
            {
                Undo.IncrementCurrentGroup();
                Undo.SetCurrentGroupName(InitCellUndoName);
                undoGroup = Undo.GetCurrentGroup();
            }

            try
            {
                foreach (Object t in targets)
                {
                    if (t is BlockShape shape)
                        TryRebuildCells(shape, cellSize, recordUndo);
                }
            }
            finally
            {
                if (recordUndo) Undo.CollapseUndoOperations(undoGroup);
            }
        }

        private static void TryRebuildCells(BlockShape shape, float cellSize, bool recordUndo)
        {
            if (EditorUtility.IsPersistent(shape))
            {
                Debug.LogWarning("Open the prefab in Prefab Mode to initialize its cells.", shape);
                return;
            }

            var data = shape.BlockData;
            if (data == null || data.cells == null) return;
            foreach (var cell in data.cells)
            {
                if (cell.cellPrefab == null) return;
            }

            var oldCells = new List<GameObject>();
            foreach (Transform child in shape.transform)
            {
                if (child.TryGetComponent<BlockCell>(out _) && !child.TryGetComponent<BlockShape>(out _))
                    oldCells.Add(child.gameObject);
            }

            if (recordUndo)
            {
                if (!RemoveCellsWithUndo(shape, oldCells)) return;
            }
            else
            {
                foreach (var oldCell in oldCells)
                {
                    oldCell.SetActive(false);
                    Object.Destroy(oldCell);
                }
            }

            var newCells = new List<BlockCell>(data.cells.Length + 1);
            if (shape.TryGetComponent<BlockCell>(out var rootCell))
                newCells.Add(rootCell);

            foreach (var cell in data.cells)
            {
                var newCell = Object.Instantiate(cell.cellPrefab, shape.transform);
                newCell.transform.localPosition = cell.cellPos * cellSize;
                if (recordUndo) Undo.RegisterCreatedObjectUndo(newCell.gameObject, InitCellUndoName);
                newCells.Add(newCell);
            }

            shape.SetCells(newCells);
            if (recordUndo && PrefabUtility.IsPartOfPrefabInstance(shape))
                PrefabUtility.RecordPrefabInstancePropertyModifications(shape);
        }

        private static bool RemoveCellsWithUndo(BlockShape shape, IReadOnlyList<GameObject> oldCells)
        {
            var prefabRoot = PrefabUtility.GetOutermostPrefabInstanceRoot(shape.gameObject);
            foreach (var oldCell in oldCells)
            {
                if (prefabRoot == null ||
                    PrefabUtility.GetOutermostPrefabInstanceRoot(oldCell) != prefabRoot ||
                    PrefabUtility.IsAddedGameObjectOverride(oldCell)) continue;

                Debug.LogWarning("Rebuild inherited cells on their source prefab in Prefab Mode.", shape);
                return false;
            }

            Undo.RegisterCompleteObjectUndo(shape, InitCellUndoName);
            foreach (var oldCell in oldCells)
            {
                Undo.DestroyObjectImmediate(oldCell);
            }

            return true;
        }
    }
}
