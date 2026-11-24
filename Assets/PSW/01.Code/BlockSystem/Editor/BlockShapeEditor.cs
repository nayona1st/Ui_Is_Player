using System.Collections.Generic;
using _LumenLib.ServiceLocator;
using UnityEditor;
using UnityEditor.SceneManagement;
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
                tooltip = "InitCell()을 1회 호출합니다.\n" +
                          "Block Data의 cells 목록대로 셀 프리팹을 자식으로 생성하고 " +
                          "BlockSnapper의 셀 크기에 맞춰 배치합니다.\n" +
                          "(Block Data가 비어 있으면 호출되지 않습니다)"
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

            Undo.SetCurrentGroupName("Init Cell");
            int undoGroup = Undo.GetCurrentGroup();

            foreach (Object t in targets)
            {
                if (t is not BlockShape shape) continue;

                // 버튼을 누른 시점에 blockData가 null이면 호출하지 않음
                var so = new SerializedObject(shape);
                if (so.FindProperty(BlockDataPropName).objectReferenceValue == null)
                {
                    Debug.LogWarning($"[BlockShapeEditor] '{shape.name}'의 Block Data가 null이라 InitCell()을 호출하지 않았습니다.", shape);
                    continue;
                }

                var before = new HashSet<Transform>();
                foreach (Transform child in shape.transform) before.Add(child);

                if (!Application.isPlaying)
                    Undo.RecordObject(shape, "Init Cell");

                shape.InitCell(cellSize);

                if (!Application.isPlaying)
                {
                    foreach (Transform child in shape.transform)
                    {
                        if (!before.Contains(child))
                            Undo.RegisterCreatedObjectUndo(child.gameObject, "Init Cell");
                    }

                    var scene = shape.gameObject.scene;
                    if (scene.IsValid() && !EditorSceneManager.IsPreviewScene(scene))
                        EditorSceneManager.MarkSceneDirty(scene);
                }
            }

            Undo.CollapseUndoOperations(undoGroup);
        }
    }
}
