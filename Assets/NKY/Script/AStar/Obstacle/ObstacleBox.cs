using UnityEngine;

namespace NKY.Script.AStar.Obstacle
{
    public readonly struct ObstacleBox
    {
        private const float Eps = 1e-8f;

        public readonly Vector2 Center;
        public readonly Vector2 HalfSize;
        public readonly float Cos;
        public readonly float Sin;

        public ObstacleBox(Vector2 center, Vector2 halfSize, float angleDeg)
        {
            Center = center;
            HalfSize = halfSize;
            float rad = angleDeg * Mathf.Deg2Rad;
            Cos = Mathf.Cos(rad);
            Sin = Mathf.Sin(rad);
        }

        public Vector2 ToLocal(Vector2 worldPoint)
        {
            Vector2 d = worldPoint - Center;
            return new Vector2(
                d.x * Cos + d.y * Sin,
                -d.x * Sin + d.y * Cos);
        }

        public float DistanceToPoint(Vector2 worldPoint)
        {
            return LocalPointToRect(ToLocal(worldPoint));
        }

        // 선분 A-B(월드 좌표)와 박스 사이의 최단 거리. 겹치면 0
        public float DistanceToSegment(Vector2 worldA, Vector2 worldB)
        {
            Vector2 a = ToLocal(worldA);
            Vector2 b = ToLocal(worldB);

            // 1. 겹치면 0 (끝점이 둘 다 바깥이어도 관통할 수 있음)
            if (LocalSegmentIntersectsRect(a, b))
                return 0f;

            // 2. 안 겹치면 최단 거리는 끝점 -> 직사각형, 꼭짓점 -> 선분 중 하나에서 나옴
            float min = Mathf.Min(LocalPointToRect(a), LocalPointToRect(b));

            float hx = HalfSize.x;
            float hy = HalfSize.y;
            min = Mathf.Min(min, PointToSegment(new Vector2(-hx, -hy), a, b));
            min = Mathf.Min(min, PointToSegment(new Vector2(hx, -hy), a, b));
            min = Mathf.Min(min, PointToSegment(new Vector2(-hx, hy), a, b));
            min = Mathf.Min(min, PointToSegment(new Vector2(hx, hy), a, b));
            return min;
        }

        // 로컬 좌표의 점에서 축정렬 직사각형까지 거리 (안쪽이면 0)
        private float LocalPointToRect(Vector2 local)
        {
            Vector2 clamped = new Vector2(
                Mathf.Clamp(local.x, -HalfSize.x, HalfSize.x),
                Mathf.Clamp(local.y, -HalfSize.y, HalfSize.y));
            return Vector2.Distance(local, clamped);
        }

        // slab 방식: x 구간, y 구간에서 선분이 직사각형 안에 있는 t 범위를 차례로 좁힘
        private bool LocalSegmentIntersectsRect(Vector2 a, Vector2 b)
        {
            Vector2 d = b - a;
            float tMin = 0f;
            float tMax = 1f;

            if (!ClipAxis(a.x, d.x, HalfSize.x, ref tMin, ref tMax)) return false;
            if (!ClipAxis(a.y, d.y, HalfSize.y, ref tMin, ref tMax)) return false;
            return true;
        }

        private static bool ClipAxis(float a, float d, float half, ref float tMin, ref float tMax)
        {
            if (Mathf.Abs(d) < Eps)
                return a >= -half && a <= half; // 축과 나란한 선분: 범위 안에 있는지만 확인

            float inv = 1f / d;
            float t1 = (-half - a) * inv;
            float t2 = (half - a) * inv;
            if (t1 > t2) (t1, t2) = (t2, t1);

            tMin = Mathf.Max(tMin, t1);
            tMax = Mathf.Min(tMax, t2);
            return tMin <= tMax;
        }

        private static float PointToSegment(Vector2 p, Vector2 a, Vector2 b)
        {
            Vector2 ab = b - a;
            float len2 = ab.sqrMagnitude;
            if (len2 < Eps)
                return Vector2.Distance(p, a); // 길이 0인 선분

            float t = Mathf.Clamp01(Vector2.Dot(p - a, ab) / len2);
            return Vector2.Distance(p, a + ab * t);
        }
    }
}