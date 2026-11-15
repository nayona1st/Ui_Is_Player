using NKY.Script.AStar.Obstacle;
using NUnit.Framework;
using UnityEngine;

namespace NKY.Script.AStar.Tests
{
    public class ObstacleBoxTests
    {
        private const float Tol = 1e-4f;

        [Test]
        public void Point_OutsideAxisAligned_ReturnsGap()
        {
            var box = new ObstacleBox(Vector2.zero, new Vector2(1f, 1f), 0f);
            Assert.AreEqual(2f, box.DistanceToPoint(new Vector2(3f, 0f)), Tol);
        }

        [Test]
        public void Point_Inside_ReturnsZero()
        {
            var box = new ObstacleBox(Vector2.zero, new Vector2(1f, 1f), 0f);
            Assert.AreEqual(0f, box.DistanceToPoint(new Vector2(0.5f, 0.5f)), Tol);
        }

        [Test]
        public void Point_NearestIsCorner_ReturnsCornerDistance()
        {
            var box = new ObstacleBox(Vector2.zero, new Vector2(1f, 1f), 0f);
            Assert.AreEqual(Mathf.Sqrt(2f), box.DistanceToPoint(new Vector2(2f, 2f)), Tol);
        }

        [Test]
        public void Square_Rotated45_PointOnAxis()
        {
            var box = new ObstacleBox(Vector2.zero, new Vector2(1f, 1f), 45f);
            Assert.AreEqual(2f - Mathf.Sqrt(2f), box.DistanceToPoint(new Vector2(2f, 0f)), Tol);
        }

        [Test]
        public void Rotated90_UsesSwappedExtents()
        {
            var box = new ObstacleBox(new Vector2(5f, 5f), new Vector2(2f, 1f), 90f);
            Assert.AreEqual(2f, box.DistanceToPoint(new Vector2(8f, 5f)), Tol);
        }

        // 회전 부호를 검증하는 테스트: 직사각형 + 45도 + 대칭이 아닌 점
        [Test]
        public void Rectangle_Rotated45_DetectsWrongRotationSign()
        {
            var box = new ObstacleBox(Vector2.zero, new Vector2(2f, 1f), 45f);
            Assert.AreEqual(0.9262f, box.DistanceToPoint(new Vector2(3f, 1f)), Tol);
        }

        [Test]
        public void Point_OnBoundary_ReturnsZero()
        {
            var box = new ObstacleBox(Vector2.zero, new Vector2(1f, 1f), 0f);
            Assert.AreEqual(0f, box.DistanceToPoint(new Vector2(1f, 0f)), Tol);
        }
        
        private static ObstacleBox Unit(float angleDeg = 0f)
            => new ObstacleBox(Vector2.zero, new Vector2(1f, 1f), angleDeg);

        [Test]
        public void Segment_ParallelAbove_ReturnsGap()
            => Assert.AreEqual(2f, Unit().DistanceToSegment(new Vector2(-3f, 3f), new Vector2(3f, 3f)), Tol);

        [Test]
        public void Segment_PassesThrough_ReturnsZero()
            => Assert.AreEqual(0f, Unit().DistanceToSegment(new Vector2(-3f, 0f), new Vector2(3f, 0f)), Tol);

        [Test]
        public void Segment_FullyInside_ReturnsZero()
            => Assert.AreEqual(0f, Unit().DistanceToSegment(new Vector2(-0.5f, 0f), new Vector2(0.5f, 0f)), Tol);

        [Test]
        public void Segment_EndpointIsNearest()
            => Assert.AreEqual(2f, Unit().DistanceToSegment(new Vector2(3f, 0f), new Vector2(5f, 0f)), Tol);

        [Test]
        public void Segment_CornerIsNearest()
            => Assert.AreEqual(Mathf.Sqrt(2f) * 0.5f, Unit().DistanceToSegment(new Vector2(3f, 0f), new Vector2(0f, 3f)), Tol);

        [Test]
        public void Segment_ZeroLength_ActsLikePoint()
            => Assert.AreEqual(Mathf.Sqrt(2f), Unit().DistanceToSegment(new Vector2(2f, 2f), new Vector2(2f, 2f)), Tol);

        [Test]
        public void Segment_Rotated45_CornerIsNearest()
            => Assert.AreEqual(3f - Mathf.Sqrt(2f), Unit(45f).DistanceToSegment(new Vector2(3f, -1f), new Vector2(3f, 1f)), Tol);

        [Test]
        public void Segment_ZeroLengthOnRectangle45_MatchesPointTest()
        {
            var box = new ObstacleBox(Vector2.zero, new Vector2(2f, 1f), 45f);
            Assert.AreEqual(0.9262f, box.DistanceToSegment(new Vector2(3f, 1f), new Vector2(3f, 1f)), Tol);
        }
    }
}