using System.Numerics;

namespace Meshes
{
    /// <summary>
    /// Contains pre-defined mesh data.
    /// </summary>
    public static class BuiltInMeshes
    {
        /// <summary>
        /// Data for a simple quad.
        /// </summary>
        public static class Quad
        {
            /// <summary>
            /// Vertex positions starting at -0.5 on the XY plane.
            /// </summary>
            public static readonly Vector3[] centeredPositions =
            [
                new(-0.5f, -0.5f, 0f),
                new(0.5f, -0.5f, 0f),
                new(0.5f, 0.5f, 0f),
                new(-0.5f, 0.5f, 0f)
            ];

            /// <summary>
            /// Vertex positions starting at 0 on the XY plane.
            /// </summary>
            public static readonly Vector3[] bottomLeftPositions =
            [
                new(0f, 0f, 0f),
                new(1f, 0f, 0f),
                new(1f, 1f, 0f),
                new(0f, 1f, 0f)
            ];

            /// <summary>
            /// UVs.
            /// </summary>
            public static readonly Vector2[] uvs =
            [
                new(0, 0),
                new(1, 0),
                new(1, 1),
                new(0, 1)
            ];

            /// <summary>
            /// Normals.
            /// </summary>
            public static readonly Vector3[] normals =
            [
                new(0, 0, 1),
                new(0, 0, 1),
                new(0, 0, 1),
                new(0, 0, 1)
            ];

            /// <summary>
            /// Colors.
            /// </summary>
            public static readonly Vector4[] colors =
            [
                new(1, 1, 1, 1),
                new(1, 1, 1, 1),
                new(1, 1, 1, 1),
                new(1, 1, 1, 1)
            ];

            /// <summary>
            /// Indices.
            /// </summary>
            public static readonly uint[] indices =
            [
                0, 1, 2,
                2, 3, 0
            ];
        }

        /// <summary>
        /// Data for a 3D cube.
        /// </summary>
        public static class Cube
        {
            /// <summary>
            /// Vertex positions starting at -0.5 and ending at 0.5.
            /// </summary>
            public static readonly Vector3[] positions =
            [
                new(-0.5f, 0.5f, -0.5f),
                new(0.5f, 0.5f, -0.5f),
                new(0.5f, -0.5f, -0.5f),
                new(-0.5f, -0.5f, -0.5f),
                new(-0.5f, 0.5f, 0.5f),
                new(0.5f, 0.5f, 0.5f),
                new(0.5f, -0.5f, 0.5f),
                new(-0.5f, -0.5f, 0.5f)
            ];

            /// <summary>
            /// UVs.
            /// </summary>
            public static readonly Vector2[] uvs =
            [
                new(0, 0),
                new(1, 0),
                new(1, 1),
                new(0, 1)
            ];

            /// <summary>
            /// Colors.
            /// </summary>
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

            /// <summary>
            /// Indices.
            /// </summary>
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