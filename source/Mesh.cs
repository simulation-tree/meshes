using Meshes.Components;
using System;
using System.Diagnostics;
using System.Numerics;
using Worlds;

namespace Meshes
{
    /// <summary>
    /// An entity that stores mesh data.
    /// </summary>
    public readonly partial struct Mesh : IEntity
    {
        /// <summary>
        /// Checks if the mesh is loaded.
        /// </summary>
        public readonly bool IsLoaded
        {
            get
            {
                if (TryGetComponent(out IsMeshRequest request))
                {
                    return request.loaded;
                }

                return IsCompliant;
            }
        }

        /// <summary>
        /// Checks if the mesh contains vertex positions.
        /// </summary>
        public readonly bool ContainsPositions => (GetComponent<IsMesh>().channels & MeshChannelMask.Positions) != 0;

        /// <summary>
        /// Checks if the mesh contains vertex UVs.
        /// </summary>
        public readonly bool ContainsUVs => (GetComponent<IsMesh>().channels & MeshChannelMask.UVs) != 0;

        /// <summary>
        /// Checks if the mesh contains vertex normals.
        /// </summary>
        public readonly bool ContainsNormals => (GetComponent<IsMesh>().channels & MeshChannelMask.Normals) != 0;

        /// <summary>
        /// Checks if the mesh contains vertex tangents.
        /// </summary>
        public readonly bool ContainsTangents => (GetComponent<IsMesh>().channels & MeshChannelMask.Tangents) != 0;

        /// <summary>
        /// Checks if the mesh contains vertex bi-tangents.
        /// </summary>
        public readonly bool ContainsBiTangents => (GetComponent<IsMesh>().channels & MeshChannelMask.BiTangents) != 0;

        /// <summary>
        /// Checks if the mesh contains vertex colors.
        /// </summary>
        public readonly bool ContainsColors => (GetComponent<IsMesh>().channels & MeshChannelMask.Colors) != 0;

        /// <summary>
        /// Amount of vertices.
        /// </summary>
        public readonly int VertexCount
        {
            get
            {
                ThrowIfNotLoaded();

                return GetComponent<IsMesh>().vertexCount;
            }
        }

        /// <summary>
        /// Amount of indices.
        /// </summary>
        public readonly int IndexCount
        {
            get
            {
                ThrowIfNotLoaded();

                return GetComponent<IsMesh>().indexCount;
            }
        }

        /// <summary>
        /// Vertex position channel data.
        /// </summary>
        public readonly Span<Vector3> Positions
        {
            get
            {
                ThrowIfNotLoaded();
                ThrowIfPositionsMissing();

                return GetArray<MeshVertexPosition>().As<Vector3>();
            }
        }

        /// <summary>
        /// UVs channel data.
        /// </summary>
        public readonly Span<Vector2> UVs
        {
            get
            {
                ThrowIfNotLoaded();
                ThrowIfUVsMissing();

                return GetArray<MeshVertexUV>().As<Vector2>();
            }
        }

        /// <summary>
        /// Normals channel data.
        /// </summary>
        public readonly Span<Vector3> Normals
        {
            get
            {
                ThrowIfNotLoaded();
                ThrowIfNormalsMissing();

                return GetArray<MeshVertexNormal>().As<Vector3>();
            }
        }

        /// <summary>
        /// Tangents channel data.
        /// </summary>
        public readonly Span<Vector3> Tangents
        {
            get
            {
                ThrowIfNotLoaded();
                ThrowIfTangentsMissing();

                return GetArray<MeshVertexTangent>().As<Vector3>();
            }
        }

        /// <summary>
        /// Bi-tangents channel data.
        /// </summary>
        public readonly Span<Vector3> BiTangents
        {
            get
            {
                ThrowIfNotLoaded();
                ThrowIfBiTangentsMissing();

                return GetArray<MeshVertexBiTangent>().As<Vector3>();
            }
        }

        /// <summary>
        /// Vertex color channel data.
        /// </summary>
        public readonly Span<Vector4> Colors
        {
            get
            {
                ThrowIfNotLoaded();
                ThrowIfColorsMissing();

                return GetArray<MeshVertexColor>().As<Vector4>();
            }
        }

        /// <summary>
        /// Indices data.
        /// </summary>
        public readonly Span<uint> Indices
        {
            get
            {
                ThrowIfNotLoaded();

                return GetArray<MeshVertexIndex>().As<uint>();
            }
        }

        /// <summary>
        /// Mask of channels contained in the mesh.
        /// </summary>
        public readonly MeshChannelMask Channels
        {
            get
            {
                ThrowIfNotLoaded();

                return GetComponent<IsMesh>().channels;
            }
        }

        /// <summary>
        /// Size of a single vertex in <see cref="float"/>s.
        /// </summary>
        public readonly int VertexSize
        {
            get
            {
                ThrowIfNotLoaded();

                return GetComponent<IsMesh>().channels.GetVertexSize();
            }
        }

        /// <summary>
        /// The bounding box of the mesh.
        /// </summary>
        public readonly (Vector3 min, Vector3 max) Bounds
        {
            get
            {
                ThrowIfNotLoaded();
                ThrowIfPositionsMissing();

                Span<Vector3> positions = GetArray<MeshVertexPosition>().AsSpan<Vector3>();
                Vector3 min = new(float.MaxValue);
                Vector3 max = new(float.MinValue);
                for (int i = 0; i < positions.Length; i++)
                {
                    Vector3 position = positions[i];
                    min = Vector3.Min(min, position);
                    max = Vector3.Max(max, position);
                }

                return (min, max);
            }
        }

        /// <summary>
        /// Version of the mesh.
        /// </summary>
        public readonly ushort Version => GetComponent<IsMesh>().version;

        /// <summary>
        /// Creates a blank mesh with no channel data or vertices.
        /// </summary>
        public Mesh(World world)
        {
            this.world = world;
            value = world.CreateEntity(new IsMesh(0, default, 0, 0));
            CreateArray<MeshVertexIndex>();
        }

        /// <summary>
        /// Creates a blank mesh with no channel data assigned.
        /// </summary>
        public Mesh(World world, MeshChannelMask meshChannels, int vertexCount, int indexCount)
        {
            this.world = world;
            value = world.CreateEntity(new IsMesh(0, meshChannels, vertexCount, indexCount));
            CreateArray<MeshVertexIndex>(indexCount);
            if ((meshChannels & MeshChannelMask.Positions) != 0)
            {
                CreateArray<MeshVertexPosition>(vertexCount);
            }

            if ((meshChannels & MeshChannelMask.UVs) != 0)
            {
                CreateArray<MeshVertexUV>(vertexCount);
            }

            if ((meshChannels & MeshChannelMask.Normals) != 0)
            {
                CreateArray<MeshVertexNormal>(vertexCount);
            }

            if ((meshChannels & MeshChannelMask.Tangents) != 0)
            {
                CreateArray<MeshVertexTangent>(vertexCount);
            }

            if ((meshChannels & MeshChannelMask.BiTangents) != 0)
            {
                CreateArray<MeshVertexBiTangent>(vertexCount);
            }

            if ((meshChannels & MeshChannelMask.Colors) != 0)
            {
                CreateArray<MeshVertexColor>(vertexCount);
            }
        }

        /// <summary>
        /// Creates a mesh+request from an existing model and a sub mesh index.
        /// </summary>
        public Mesh(World world, Entity modelEntity, int meshIndex = 0)
        {
            this.world = world;
            value = world.CreateEntity(new IsMeshRequest((rint)1, meshIndex));
            AddReference(modelEntity);
        }

        /// <summary>
        /// Creates a mesh with the given <paramref name="positions"/> and <paramref name="indices"/>.
        /// </summary>
        public Mesh(World world, ReadOnlySpan<Vector3> positions, ReadOnlySpan<uint> indices)
        {
            this.world = world;
            value = world.CreateEntity(new IsMesh(0, MeshChannelMask.Positions, positions.Length, indices.Length));
            CreateArray(positions.As<Vector3, MeshVertexPosition>());
            CreateArray(indices.As<uint, MeshVertexIndex>());
        }

        /// <summary>
        /// Creates a mesh with the given <paramref name="positions"/>, <paramref name="uvs"/> and <paramref name="indices"/>.
        /// </summary>
        public Mesh(World world, ReadOnlySpan<Vector3> positions, ReadOnlySpan<Vector2> uvs, ReadOnlySpan<uint> indices)
        {
            this.world = world;
            value = world.CreateEntity(new IsMesh(0, MeshChannelMask.Positions | MeshChannelMask.UVs, positions.Length, indices.Length));
            CreateArray(positions.As<Vector3, MeshVertexPosition>());
            CreateArray(uvs.As<Vector2, MeshVertexUV>());
            CreateArray(indices.As<uint, MeshVertexIndex>());
        }

        /// <summary>
        /// Creates a mesh with the given <paramref name="positions"/>, <paramref name="uvs"/>, <paramref name="normals"/> and <paramref name="indices"/>.
        /// </summary>
        public Mesh(World world, ReadOnlySpan<Vector3> positions, ReadOnlySpan<Vector2> uvs, ReadOnlySpan<Vector3> normals, ReadOnlySpan<uint> indices)
        {
            this.world = world;
            value = world.CreateEntity(new IsMesh(0, MeshChannelMask.Positions | MeshChannelMask.UVs | MeshChannelMask.Normals, positions.Length, indices.Length));
            CreateArray(positions.As<Vector3, MeshVertexPosition>());
            CreateArray(uvs.As<Vector2, MeshVertexUV>());
            CreateArray(normals.As<Vector3, MeshVertexNormal>());
            CreateArray(indices.As<uint, MeshVertexIndex>());
        }

        /// <summary>
        /// Creates a mesh with the given <paramref name="positions"/>, <paramref name="uvs"/>, <paramref name="colors"/> and <paramref name="indices"/>.
        /// </summary>
        public Mesh(World world, ReadOnlySpan<Vector3> positions, ReadOnlySpan<Vector2> uvs, ReadOnlySpan<Vector4> colors, ReadOnlySpan<uint> indices)
        {
            this.world = world;
            value = world.CreateEntity(new IsMesh(0, MeshChannelMask.Positions | MeshChannelMask.UVs | MeshChannelMask.Colors, positions.Length, indices.Length));
            CreateArray(positions.As<Vector3, MeshVertexPosition>());
            CreateArray(uvs.As<Vector2, MeshVertexUV>());
            CreateArray(colors.As<Vector4, MeshVertexColor>());
            CreateArray(indices.As<uint, MeshVertexIndex>());
        }

        /// <summary>
        /// Creates a mesh with the given <paramref name="positions"/>, <paramref name="uvs"/>, <paramref name="normals"/>, <paramref name="colors"/> and <paramref name="indices"/>.
        /// </summary>
        public Mesh(World world, ReadOnlySpan<Vector3> positions, ReadOnlySpan<Vector2> uvs, ReadOnlySpan<Vector3> normals, ReadOnlySpan<Vector4> colors, ReadOnlySpan<uint> indices)
        {
            this.world = world;
            value = world.CreateEntity(new IsMesh(0, MeshChannelMask.Positions | MeshChannelMask.UVs | MeshChannelMask.Normals | MeshChannelMask.Colors, positions.Length, indices.Length));
            CreateArray(positions.As<Vector3, MeshVertexPosition>());
            CreateArray(uvs.As<Vector2, MeshVertexUV>());
            CreateArray(normals.As<Vector3, MeshVertexNormal>());
            CreateArray(colors.As<Vector4, MeshVertexColor>());
            CreateArray(indices.As<uint, MeshVertexIndex>());
        }

        readonly void IEntity.Describe(ref Archetype archetype)
        {
            archetype.AddComponentType<IsMesh>();
            archetype.AddArrayType<MeshVertexIndex>();
        }

        /// <inheritdoc/>
        public readonly override string ToString()
        {
            return value.ToString();
        }

        /// <summary>
        /// Modifies the amount of vertices in the mesh, and resizes the internal arrays.
        /// </summary>
        public readonly void SetVertexCount(int newVertexCount)
        {
            ThrowIfNotLoaded();

            ref IsMesh mesh = ref GetComponent<IsMesh>();
            mesh.version++;
            mesh.vertexCount = newVertexCount;

            if ((mesh.channels & MeshChannelMask.Positions) != 0)
            {
                GetArray<MeshVertexPosition>().Resize(newVertexCount);
            }

            if ((mesh.channels & MeshChannelMask.UVs) != 0)
            {
                GetArray<MeshVertexUV>().Resize(newVertexCount);
            }

            if ((mesh.channels & MeshChannelMask.Normals) != 0)
            {
                GetArray<MeshVertexNormal>().Resize(newVertexCount);
            }

            if ((mesh.channels & MeshChannelMask.Tangents) != 0)
            {
                GetArray<MeshVertexTangent>().Resize(newVertexCount);
            }

            if ((mesh.channels & MeshChannelMask.BiTangents) != 0)
            {
                GetArray<MeshVertexBiTangent>().Resize(newVertexCount);
            }

            if ((mesh.channels & MeshChannelMask.Colors) != 0)
            {
                GetArray<MeshVertexColor>().Resize(newVertexCount);
            }
        }

        /// <summary>
        /// Resizes the amount of indices this mesh contains.
        /// </summary>
        /// <returns>Span of indices.</returns>
        public readonly Span<uint> SetIndexCount(int newIndexCount)
        {
            ThrowIfNotLoaded();

            ref IsMesh mesh = ref GetComponent<IsMesh>();
            mesh.version++;
            mesh.indexCount = newIndexCount;

            Values<MeshVertexIndex> array = GetArray<MeshVertexIndex>();
            array.Resize(newIndexCount);
            return array.AsSpan<uint>();
        }

        /// <summary>
        /// Modifies the amount of vertices in the mesh, and resizes the internal arrays.
        /// </summary>
        /// <returns>Span of indices.</returns>
        public readonly Span<uint> SetVertexAndIndexCount(int newVertexCount, int newIndexCount)
        {
            ThrowIfNotLoaded();

            ref IsMesh mesh = ref GetComponent<IsMesh>();
            mesh.version++;
            mesh.vertexCount = newVertexCount;
            mesh.indexCount = newIndexCount;

            if ((mesh.channels & MeshChannelMask.Positions) != 0)
            {
                GetArray<MeshVertexPosition>().Resize(newVertexCount);
            }

            if ((mesh.channels & MeshChannelMask.UVs) != 0)
            {
                GetArray<MeshVertexUV>().Resize(newVertexCount);
            }

            if ((mesh.channels & MeshChannelMask.Normals) != 0)
            {
                GetArray<MeshVertexNormal>().Resize(newVertexCount);
            }

            if ((mesh.channels & MeshChannelMask.Tangents) != 0)
            {
                GetArray<MeshVertexTangent>().Resize(newVertexCount);
            }

            if ((mesh.channels & MeshChannelMask.BiTangents) != 0)
            {
                GetArray<MeshVertexBiTangent>().Resize(newVertexCount);
            }

            if ((mesh.channels & MeshChannelMask.Colors) != 0)
            {
                GetArray<MeshVertexColor>().Resize(newVertexCount);
            }

            Values<MeshVertexIndex> array = GetArray<MeshVertexIndex>();
            array.Resize(newIndexCount);
            return array.AsSpan<uint>();
        }

        /// <summary>
        /// Checks if the mesh contains the given <paramref name="channel"/> of data.
        /// </summary>
        public readonly bool ContainsChannel(MeshChannel channel)
        {
            return GetComponent<IsMesh>().channels.Contains(channel);
        }

        /// <summary>
        /// Appends vertex data into the given <paramref name="destination"/>, in the order of the <paramref name="channels"/> given.
        /// <para>
        /// Missing channels on the mesh are skipped.
        /// </para>
        /// </summary>
        /// <returns>Amount of <see cref="float"/>s written to the <paramref name="destination"/>.</returns>
        public readonly int Assemble(Span<float> destination, ReadOnlySpan<MeshChannel> channels)
        {
            return Assemble(world, value, destination, channels);
        }

        /// <summary>
        /// Appends vertex data into the given <paramref name="destination"/>, in the order of the <paramref name="channels"/> given.
        /// <para>
        /// Missing channels on the mesh are skipped.
        /// </para>
        /// </summary>
        /// <returns>Amount of <see cref="float"/>s written to the <paramref name="destination"/>.</returns>
        public static int Assemble(World world, uint entity, Span<float> destination, ReadOnlySpan<MeshChannel> channels)
        {
            Span<Vector3> positions = default;
            Span<Vector2> uvs = default;
            Span<Vector3> normals = default;
            Span<Vector3> tangents = default;
            Span<Vector3> biTangents = default;
            Span<Vector4> colors = default;

            ReadOnlySpan<byte> channelsAsByte = channels.As<MeshChannel, byte>();
            if (channelsAsByte.Contains((byte)MeshChannel.Position))
            {
                positions = world.GetArray<MeshVertexPosition>(entity).AsSpan<Vector3>();
            }

            if (channelsAsByte.Contains((byte)MeshChannel.UV))
            {
                uvs = world.GetArray<MeshVertexUV>(entity).AsSpan<Vector2>();
            }

            if (channelsAsByte.Contains((byte)MeshChannel.Normal))
            {
                normals = world.GetArray<MeshVertexNormal>(entity).AsSpan<Vector3>();
            }

            if (channelsAsByte.Contains((byte)MeshChannel.Tangent))
            {
                tangents = world.GetArray<MeshVertexTangent>(entity).AsSpan<Vector3>();
            }

            if (channelsAsByte.Contains((byte)MeshChannel.BiTangent))
            {
                biTangents = world.GetArray<MeshVertexBiTangent>(entity).AsSpan<Vector3>();
            }

            if (channelsAsByte.Contains((byte)MeshChannel.Color))
            {
                colors = world.GetArray<MeshVertexColor>(entity).AsSpan<Vector4>();
            }

            int vertexCount = positions.Length;
            int index = 0;
            for (int i = 0; i < vertexCount; i++)
            {
                for (int c = 0; c < channels.Length; c++)
                {
                    MeshChannel channel = channels[c];
                    if (channel == MeshChannel.Position)
                    {
                        Vector3 position = positions[i];
                        destination[index++] = position.X;
                        destination[index++] = position.Y;
                        destination[index++] = position.Z;
                    }
                    else if (channel == MeshChannel.UV)
                    {
                        Vector2 uv = uvs[i];
                        destination[index++] = uv.X;
                        destination[index++] = uv.Y;
                    }
                    else if (channel == MeshChannel.Normal)
                    {
                        Vector3 normal = normals[i];
                        destination[index++] = normal.X;
                        destination[index++] = normal.Y;
                        destination[index++] = normal.Z;
                    }
                    else if (channel == MeshChannel.Tangent)
                    {
                        Vector3 tangent = tangents[i];
                        destination[index++] = tangent.X;
                        destination[index++] = tangent.Y;
                        destination[index++] = tangent.Z;
                    }
                    else if (channel == MeshChannel.BiTangent)
                    {
                        Vector3 bitangent = biTangents[i];
                        destination[index++] = bitangent.X;
                        destination[index++] = bitangent.Y;
                        destination[index++] = bitangent.Z;
                    }
                    else if (channel == MeshChannel.Color)
                    {
                        Vector4 color = colors[i];
                        destination[index++] = color.X;
                        destination[index++] = color.Y;
                        destination[index++] = color.Z;
                        destination[index++] = color.W;
                    }
                }
            }

            return index;
        }

        /// <summary>
        /// Creates a new cube mesh.
        /// </summary>
        public static Mesh CreateCube(World world)
        {
            return new(world, BuiltInMeshes.Cube.positions, BuiltInMeshes.Cube.uvs, BuiltInMeshes.Cube.colors, BuiltInMeshes.Cube.indices);
        }

        /// <summary>
        /// Creates a new quad starting at -0.5 on the XY plane.
        /// </summary>
        public static Mesh CreateCenteredQuad(World world)
        {
            return new(world, BuiltInMeshes.Quad.centeredPositions, BuiltInMeshes.Quad.uvs, BuiltInMeshes.Quad.normals, BuiltInMeshes.Quad.colors, BuiltInMeshes.Quad.indices);
        }

        /// <summary>
        /// Creates a new quad starting at 0 on the XY plane.
        /// </summary>
        public static Mesh CreateBottomLeftQuad(World world)
        {
            return new(world, BuiltInMeshes.Quad.bottomLeftPositions, BuiltInMeshes.Quad.uvs, BuiltInMeshes.Quad.normals, BuiltInMeshes.Quad.colors, BuiltInMeshes.Quad.indices);
        }

        [Conditional("DEBUG")]
        private readonly void ThrowIfNotLoaded()
        {
            if (!IsLoaded)
            {
                throw new InvalidOperationException($"Mesh `{value}` is not loaded");
            }
        }

        [Conditional("DEBUG")]
        private readonly void ThrowIfPositionsMissing()
        {
            if (!ContainsPositions)
            {
                throw new InvalidOperationException($"Mesh `{value}` does not contain positions");
            }
        }

        [Conditional("DEBUG")]
        private readonly void ThrowIfUVsMissing()
        {
            if (!ContainsUVs)
            {
                throw new InvalidOperationException($"Mesh `{value}` does not contain UVs");
            }
        }

        [Conditional("DEBUG")]
        private readonly void ThrowIfNormalsMissing()
        {
            if (!ContainsNormals)
            {
                throw new InvalidOperationException($"Mesh `{value}` does not contain normals");
            }
        }

        [Conditional("DEBUG")]
        private readonly void ThrowIfTangentsMissing()
        {
            if (!ContainsTangents)
            {
                throw new InvalidOperationException($"Mesh `{value}` does not contain tangents");
            }
        }

        [Conditional("DEBUG")]
        private readonly void ThrowIfBiTangentsMissing()
        {
            if (!ContainsBiTangents)
            {
                throw new InvalidOperationException($"Mesh `{value}` does not contain bi-tangents");
            }
        }

        [Conditional("DEBUG")]
        private readonly void ThrowIfColorsMissing()
        {
            if (!ContainsColors)
            {
                throw new InvalidOperationException($"Mesh `{value}` does not contain colors");
            }
        }

        /// <summary>
        /// Increments the version of the mesh.
        /// </summary>
        public readonly void IncrementVersion()
        {
            ref IsMesh mesh = ref GetComponent<IsMesh>();
            mesh.version++;
        }
    }
}