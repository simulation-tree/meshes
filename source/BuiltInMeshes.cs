using System.Numerics;

namespace Meshes
{
    public static class BuiltInMeshes
    {
        public static class Quad
        {
            public static readonly Vector3[] centeredPositions =
            [
                new(-0.5f, -0.5f, 0f),
                new(0.5f, -0.5f, 0f),
                new(0.5f, 0.5f, 0f),
                new(-0.5f, 0.5f, 0f)
            ];

            public static readonly Vector3[] bottomLeftPositions =
            [
                new(0f, 0f, 0f),
                new(1f, 0f, 0f),
                new(1f, 1f, 0f),
                new(0f, 1f, 0f)
            ];

            public static readonly Vector2[] uvs =
            [
                new(0, 0),
                new(1, 0),
                new(1, 1),
                new(0, 1)
            ];

            public static readonly Vector3[] normals =
            [
                new(0, 0, 1),
                new(0, 0, 1),
                new(0, 0, 1),
                new(0, 0, 1)
            ];

            public static readonly Vector4[] colors =
            [
                new(1, 1, 1, 1),
                new(1, 1, 1, 1),
                new(1, 1, 1, 1),
                new(1, 1, 1, 1)
            ];

            public static readonly uint[] indices =
            [
                0, 1, 2,
                2, 3, 0
            ];
        }

        public static class Cube
        {
            public static readonly Vector3[] positions =
            [
                new(-1, 1, -1),
                new(1, 1, -1),
                new(1, -1, -1),
                new(-1, -1, -1),
                new(-1, 1, 1),
                new(1, 1, 1),
                new(1, -1, 1),
                new(-1, -1, 1)
            ];

            public static readonly Vector2[] uvs =
            [
                new(0, 0),
                new(1, 0),
                new(1, 1),
                new(0, 1)
            ];

            public static readonly Vector4[] colors =
            [
                new(1, 1, 1, 1),
                new(1, 1, 1, 1),
                new(1, 1, 1, 1),
                new(1, 1, 1, 1),
                new(1, 1, 1, 1),
                new(1, 1, 1, 1),
                new(1, 1, 1, 1),
                new(1, 1, 1, 1)
            ];

            public static readonly uint[] indices =
            [
                0, 1, 2, 2, 3, 0,
                1, 5, 6, 6, 2, 1,
                5, 4, 7, 7, 6, 5,
                4, 0, 3, 3, 7, 4,
                3, 2, 6, 6, 7, 3,
                4, 5, 1, 1, 0, 4
            ];
        }
    }
}