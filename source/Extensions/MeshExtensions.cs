using Meshes.Components;
using System;
using System.Diagnostics;
using System.Numerics;
using Unmanaged;
using Worlds;

namespace Meshes
{
    public static class MeshExtensions
    {
        public static bool HasPositions<T>(this T mesh) where T : unmanaged, IMesh
        {
            return mesh.AsEntity().ContainsArray<MeshVertexPosition>();
        }

        public static bool HasNormals<T>(this T mesh) where T : unmanaged, IMesh
        {
            return mesh.AsEntity().ContainsArray<MeshVertexNormal>();
        }

        public static bool HasUVs<T>(this T mesh) where T : unmanaged, IMesh
        {
            return mesh.AsEntity().ContainsArray<MeshVertexUV>();
        }

        public static bool HasColors<T>(this T mesh) where T : unmanaged, IMesh
        {
            return mesh.AsEntity().ContainsArray<MeshVertexColor>();
        }

        public static bool HasTangents<T>(this T mesh) where T : unmanaged, IMesh
        {
            return mesh.AsEntity().ContainsArray<MeshVertexTangent>();
        }

        public static bool HasBiTangents<T>(this T mesh) where T : unmanaged, IMesh
        {
            return mesh.AsEntity().ContainsArray<MeshVertexBiTangent>();
        }

        public static uint GetVertexCount<T>(this T mesh) where T : unmanaged, IMesh
        {
            return mesh.AsEntity().GetArrayLength<MeshVertexPosition>();
        }

        public static uint GetIndexCount<T>(this T mesh) where T : unmanaged, IMesh
        {
            return mesh.AsEntity().GetArrayLength<MeshVertexIndex>();
        }

        public static USpan<uint> GetIndices<T>(this T mesh) where T : unmanaged, IMesh
        {
            return mesh.AsEntity().GetArray<MeshVertexIndex>().As<uint>();
        }

        public static USpan<Vector3> GetVertexPositions<T>(this T mesh) where T : unmanaged, IMesh
        {
            ThrowIfMissingPositions(mesh);

            return mesh.AsEntity().GetArray<MeshVertexPosition>().As<Vector3>();
        }

        public static USpan<Vector3> GetVertexNormals<T>(this T mesh) where T : unmanaged, IMesh
        {
            ThrowIfMissingNormals(mesh);

            return mesh.AsEntity().GetArray<MeshVertexNormal>().As<Vector3>();
        }

        public static USpan<Vector2> GetVertexUVs<T>(this T mesh) where T : unmanaged, IMesh
        {
            ThrowIfMissingUVs(mesh);

            return mesh.AsEntity().GetArray<MeshVertexUV>().As<Vector2>();
        }

        public static USpan<Vector4> GetVertexColors<T>(this T mesh) where T : unmanaged, IMesh
        {
            ThrowIfMissingColors(mesh);

            return mesh.AsEntity().GetArray<MeshVertexColor>().As<Vector4>();
        }

        public static USpan<Vector3> GetVertexTangents<T>(this T mesh) where T : unmanaged, IMesh
        {
            ThrowIfMissingTangents(mesh);

            return mesh.AsEntity().GetArray<MeshVertexTangent>().As<Vector3>();
        }

        public static USpan<Vector3> GetVertexBiTangents<T>(this T mesh) where T : unmanaged, IMesh
        {
            ThrowIfMissingBiTangents(mesh);

            return mesh.AsEntity().GetArray<MeshVertexBiTangent>().As<Vector3>();
        }

        public static uint GetVertexSize<T>(this T mesh) where T : unmanaged, IMesh
        {
            return mesh.GetChannels().GetVertexSize();
        }

        /// <summary>
        /// Appends vertex data into the given span, in the order of the channels given.
        /// <para>Missing channels on the mesh are skipped.</para>
        /// </summary>
        /// <returns>How many <c>float</c> were added to <paramref name="vertexData"/>.</returns>
        public static uint Assemble<T>(this T mesh, USpan<float> vertexData, USpan<Mesh.Channel> channels) where T : unmanaged, IMesh
        {
            USpan<Vector3> positions = default;
            USpan<Vector2> uvs = default;
            USpan<Vector3> normals = default;
            USpan<Vector3> tangents = default;
            USpan<Vector3> biTangents = default;
            USpan<Vector4> colors = default;

            for (uint i = 0; i < channels.Length; i++)
            {
                Mesh.Channel channel = channels[i];
                if (channel == Mesh.Channel.Position)
                {
                    positions = GetVertexPositions(mesh);
                }
                else if (channel == Mesh.Channel.UV)
                {
                    uvs = GetVertexUVs(mesh);
                }
                else if (channel == Mesh.Channel.Normal)
                {
                    normals = GetVertexNormals(mesh);
                }
                else if (channel == Mesh.Channel.Tangent)
                {
                    tangents = GetVertexTangents(mesh);
                }
                else if (channel == Mesh.Channel.BiTangent)
                {
                    biTangents = GetVertexBiTangents(mesh);
                }
                else if (channel == Mesh.Channel.Color)
                {
                    colors = GetVertexColors(mesh);
                }
            }

            uint vertexCount = GetVertexCount(mesh);
            uint index = 0;
            for (uint i = 0; i < vertexCount; i++)
            {
                for (uint c = 0; c < channels.Length; c++)
                {
                    Mesh.Channel channel = channels[c];
                    if (channel == Mesh.Channel.Position)
                    {
                        Vector3 position = positions[i];
                        vertexData[index++] = position.X;
                        vertexData[index++] = position.Y;
                        vertexData[index++] = position.Z;
                    }
                    else if (channel == Mesh.Channel.UV)
                    {
                        Vector2 uv = uvs[i];
                        vertexData[index++] = uv.X;
                        vertexData[index++] = uv.Y;
                    }
                    else if (channel == Mesh.Channel.Normal)
                    {
                        Vector3 normal = normals[i];
                        vertexData[index++] = normal.X;
                        vertexData[index++] = normal.Y;
                        vertexData[index++] = normal.Z;
                    }
                    else if (channel == Mesh.Channel.Tangent)
                    {
                        Vector3 tangent = tangents[i];
                        vertexData[index++] = tangent.X;
                        vertexData[index++] = tangent.Y;
                        vertexData[index++] = tangent.Z;
                    }
                    else if (channel == Mesh.Channel.BiTangent)
                    {
                        Vector3 bitangent = biTangents[i];
                        vertexData[index++] = bitangent.X;
                        vertexData[index++] = bitangent.Y;
                        vertexData[index++] = bitangent.Z;
                    }
                    else if (channel == Mesh.Channel.Color)
                    {
                        Vector4 color = colors[i];
                        vertexData[index++] = color.X;
                        vertexData[index++] = color.Y;
                        vertexData[index++] = color.Z;
                        vertexData[index++] = color.W;
                    }
                }
            }

            return index;
        }

        public static bool ContainsChannel<T>(this T mesh, Mesh.Channel channel) where T : unmanaged, IMesh
        {
            if (channel == Mesh.Channel.Position && !HasPositions(mesh))
            {
                return false;
            }
            else if (channel == Mesh.Channel.UV && !HasUVs(mesh))
            {
                return false;
            }
            else if (channel == Mesh.Channel.Normal && !HasNormals(mesh))
            {
                return false;
            }
            else if (channel == Mesh.Channel.Tangent && !HasTangents(mesh))
            {
                return false;
            }
            else if (channel == Mesh.Channel.BiTangent && !HasBiTangents(mesh))
            {
                return false;
            }
            else if (channel == Mesh.Channel.Color && !HasColors(mesh))
            {
                return false;
            }

            return true;
        }

        public static void IncrementVersion<T>(this T mesh) where T : unmanaged, IMesh
        {
            ref IsMesh component = ref mesh.AsEntity().GetComponent<IsMesh>();
            component.version++;
        }

        public static USpan<Vector3> CreatePositions<T>(this T mesh, uint length) where T : unmanaged, IMesh
        {
            ThrowIfAlreadyContainsPositions(mesh);
            IncrementVersion(mesh);

            USpan<MeshVertexPosition> array = mesh.AsEntity().CreateArray<MeshVertexPosition>(length);
            return array.As<Vector3>();
        }

        public static USpan<Vector3> ResizePositions<T>(this T mesh, uint length) where T : unmanaged, IMesh
        {
            ThrowIfMissingPositions(mesh);
            IncrementVersion(mesh);

            USpan<MeshVertexPosition> array = mesh.AsEntity().ResizeArray<MeshVertexPosition>(length);
            return array.As<Vector3>();
        }

        public static USpan<Vector2> CreateUVs<T>(this T mesh, uint length) where T : unmanaged, IMesh
        {
            ThrowIfAlreadyContainsUVs(mesh);
            IncrementVersion(mesh);

            USpan<MeshVertexUV> array = mesh.AsEntity().CreateArray<MeshVertexUV>(length);
            return array.As<Vector2>();
        }

        public static USpan<Vector2> ResizeUVs<T>(this T mesh, uint length) where T : unmanaged, IMesh
        {
            ThrowIfMissingUVs(mesh);
            IncrementVersion(mesh);

            USpan<MeshVertexUV> array = mesh.AsEntity().ResizeArray<MeshVertexUV>(length);
            return array.As<Vector2>();
        }

        public static USpan<Vector3> CreateNormals<T>(this T mesh, uint length) where T : unmanaged, IMesh
        {
            ThrowIfAlreadyContainsNormals(mesh);
            IncrementVersion(mesh);

            USpan<MeshVertexNormal> array = mesh.AsEntity().CreateArray<MeshVertexNormal>(length);
            return array.As<Vector3>();
        }

        public static USpan<Vector3> ResizeNormals<T>(this T mesh, uint length) where T : unmanaged, IMesh
        {
            ThrowIfMissingNormals(mesh);
            IncrementVersion(mesh);

            USpan<MeshVertexNormal> array = mesh.AsEntity().ResizeArray<MeshVertexNormal>(length);
            return array.As<Vector3>();
        }

        public static USpan<Vector3> CreateTangents<T>(this T mesh, uint length) where T : unmanaged, IMesh
        {
            ThrowIfAlreadyContainsTangents(mesh);
            IncrementVersion(mesh);

            USpan<MeshVertexTangent> array = mesh.AsEntity().CreateArray<MeshVertexTangent>(length);
            return array.As<Vector3>();
        }

        public static USpan<Vector3> ResizeTangents<T>(this T mesh, uint length) where T : unmanaged, IMesh
        {
            ThrowIfMissingTangents(mesh);
            IncrementVersion(mesh);

            USpan<MeshVertexTangent> array = mesh.AsEntity().ResizeArray<MeshVertexTangent>(length);
            return array.As<Vector3>();
        }

        public static USpan<Vector3> CreateBiTangents<T>(this T mesh, uint length) where T : unmanaged, IMesh
        {
            ThrowIfAlreadyContainsBiTangents(mesh);
            IncrementVersion(mesh);

            USpan<MeshVertexBiTangent> array = mesh.AsEntity().CreateArray<MeshVertexBiTangent>(length);
            return array.As<Vector3>();
        }

        public static USpan<Vector3> ResizeBiTangents<T>(this T mesh, uint length) where T : unmanaged, IMesh
        {
            ThrowIfMissingBiTangents(mesh);
            IncrementVersion(mesh);

            USpan<MeshVertexBiTangent> array = mesh.AsEntity().ResizeArray<MeshVertexBiTangent>(length);
            return array.As<Vector3>();
        }

        public static USpan<Vector4> CreateColors<T>(this T mesh, uint length) where T : unmanaged, IMesh
        {
            ThrowIfAlreadyContainsColors(mesh);
            IncrementVersion(mesh);

            USpan<MeshVertexColor> array = mesh.AsEntity().CreateArray<MeshVertexColor>(length);
            return array.As<Vector4>();
        }

        public static USpan<Vector4> ResizeColors<T>(this T mesh, uint length) where T : unmanaged, IMesh
        {
            ThrowIfMissingColors(mesh);
            IncrementVersion(mesh);

            USpan<MeshVertexColor> array = mesh.AsEntity().ResizeArray<MeshVertexColor>(length);
            return array.As<Vector4>();
        }

        public static void AddIndices<T>(this T mesh, USpan<uint> indices) where T : unmanaged, IMesh
        {
            uint count = mesh.AsEntity().GetArrayLength<MeshVertexIndex>();
            USpan<MeshVertexIndex> span = mesh.AsEntity().ResizeArray<MeshVertexIndex>(count + indices.Length);
            indices.As<MeshVertexIndex>().CopyTo(span.Slice(count));
        }

        public static void AddIndex<T>(this T mesh, uint index) where T : unmanaged, IMesh
        {
            uint count = mesh.AsEntity().GetArrayLength<MeshVertexIndex>();
            USpan<MeshVertexIndex> span = mesh.AsEntity().ResizeArray<MeshVertexIndex>(count + 1);
            span[count] = index;
        }

        public static void AddTriangle<T>(this T mesh, uint a, uint b, uint c) where T : unmanaged, IMesh
        {
            uint count = mesh.AsEntity().GetArrayLength<MeshVertexIndex>();
            USpan<MeshVertexIndex> span = mesh.AsEntity().ResizeArray<MeshVertexIndex>(count + 3);
            span[count] = a;
            span[count + 1] = b;
            span[count + 2] = c;
        }

        public static (Vector3 min, Vector3 max) GetBounds<T>(this T mesh) where T : unmanaged, IMesh
        {
            ThrowIfMissingPositions(mesh);

            USpan<MeshVertexPosition> positions = mesh.AsEntity().GetArray<MeshVertexPosition>();
            Vector3 min = new(float.MaxValue);
            Vector3 max = new(float.MinValue);
            for (uint i = 0; i < positions.Length; i++)
            {
                Vector3 position = positions[i].value;
                min = Vector3.Min(min, position);
                max = Vector3.Max(max, position);
            }

            return (min, max);
        }

        public static Mesh.ChannelMask GetChannels<T>(this T mesh) where T : unmanaged, IMesh
        {
            bool hasPositions = HasPositions(mesh);
            bool hasUVs = HasUVs(mesh);
            bool hasNormals = HasNormals(mesh);
            bool hasTangents = HasTangents(mesh);
            bool hasBiTangents = HasBiTangents(mesh);
            bool hasColors = HasColors(mesh);
            Mesh.ChannelMask mask = default;
            if (hasPositions)
            {
                mask |= Mesh.ChannelMask.Positions;
            }

            if (hasUVs)
            {
                mask |= Mesh.ChannelMask.UVs;
            }

            if (hasNormals)
            {
                mask |= Mesh.ChannelMask.Normals;
            }

            if (hasTangents)
            {
                mask |= Mesh.ChannelMask.Tangents;
            }

            if (hasBiTangents)
            {
                mask |= Mesh.ChannelMask.Tangents;
            }

            if (hasColors)
            {
                mask |= Mesh.ChannelMask.Colors;
            }

            return mask;
        }

        public static uint GetVersion<T>(this T mesh) where T : unmanaged, IMesh
        {
            IsMesh component = mesh.AsEntity().GetComponent<IsMesh>();
            return component.version;
        }

        public static USpan<uint> ResizeIndices<T>(this T mesh, uint length) where T : unmanaged, IMesh
        {
            return mesh.AsEntity().ResizeArray<MeshVertexIndex>(length).As<uint>();
        }

        [Conditional("DEBUG")]
        public static void ThrowIfMissingPositions<T>(this T mesh) where T : unmanaged, IMesh
        {
            if (!mesh.HasPositions())
            {
                throw new InvalidOperationException($"Mesh `{mesh}` is missing `{typeof(MeshVertexPosition)}` elements");
            }
        }

        [Conditional("DEBUG")]
        public static void ThrowIfMissingNormals<T>(this T mesh) where T : unmanaged, IMesh
        {
            if (!mesh.HasNormals())
            {
                throw new InvalidOperationException($"Mesh `{mesh}` is missing `{typeof(MeshVertexNormal)}` elements");
            }
        }

        [Conditional("DEBUG")]
        public static void ThrowIfMissingUVs<T>(this T mesh) where T : unmanaged, IMesh
        {
            if (!mesh.HasUVs())
            {
                throw new InvalidOperationException($"Mesh `{mesh}` is missing `{typeof(MeshVertexUV)}` elements");
            }
        }

        [Conditional("DEBUG")]
        public static void ThrowIfMissingColors<T>(this T mesh) where T : unmanaged, IMesh
        {
            if (!mesh.HasColors())
            {
                throw new InvalidOperationException($"Mesh `{mesh}` is missing `{typeof(MeshVertexColor)}` elements");
            }
        }

        [Conditional("DEBUG")]
        public static void ThrowIfMissingTangents<T>(this T mesh) where T : unmanaged, IMesh
        {
            if (!mesh.HasTangents())
            {
                throw new InvalidOperationException($"Mesh `{mesh}` is missing `{typeof(MeshVertexTangent)}` elements");
            }
        }

        [Conditional("DEBUG")]
        public static void ThrowIfMissingBiTangents<T>(this T mesh) where T : unmanaged, IMesh
        {
            if (!mesh.HasBiTangents())
            {
                throw new InvalidOperationException($"Mesh `{mesh}` is missing `{typeof(MeshVertexBiTangent)}` elements");
            }
        }

        [Conditional("DEBUG")]
        public static void ThrowIfAlreadyContainsPositions<T>(this T mesh) where T : unmanaged, IMesh
        {
            if (mesh.HasPositions())
            {
                throw new InvalidOperationException($"Mesh `{mesh}` already contains `{typeof(MeshVertexPosition)}` elements");
            }
        }

        [Conditional("DEBUG")]
        public static void ThrowIfAlreadyContainsNormals<T>(this T mesh) where T : unmanaged, IMesh
        {
            if (mesh.HasNormals())
            {
                throw new InvalidOperationException($"Mesh `{mesh}` already contains `{typeof(MeshVertexNormal)}` elements");
            }
        }

        [Conditional("DEBUG")]
        public static void ThrowIfAlreadyContainsUVs<T>(this T mesh) where T : unmanaged, IMesh
        {
            if (mesh.HasUVs())
            {
                throw new InvalidOperationException($"Mesh `{mesh}` already contains `{typeof(MeshVertexUV)}` elements");
            }
        }

        [Conditional("DEBUG")]
        public static void ThrowIfAlreadyContainsColors<T>(this T mesh) where T : unmanaged, IMesh
        {
            if (mesh.HasColors())
            {
                throw new InvalidOperationException($"Mesh `{mesh}` already contains `{typeof(MeshVertexColor)}` elements");
            }
        }

        [Conditional("DEBUG")]
        public static void ThrowIfAlreadyContainsTangents<T>(this T mesh) where T : unmanaged, IMesh
        {
            if (mesh.HasTangents())
            {
                throw new InvalidOperationException($"Mesh `{mesh}` already contains `{typeof(MeshVertexTangent)}` elements");
            }
        }

        [Conditional("DEBUG")]
        public static void ThrowIfAlreadyContainsBiTangents<T>(this T mesh) where T : unmanaged, IMesh
        {
            if (mesh.HasBiTangents())
            {
                throw new InvalidOperationException($"Mesh `{mesh}` already contains `{typeof(MeshVertexBiTangent)}` elements");
            }
        }
    }
}
