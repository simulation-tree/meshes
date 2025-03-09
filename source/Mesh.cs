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

        public readonly bool ContainsPositions => ContainsArray<MeshVertexPosition>();
        public readonly bool ContainsUVs => ContainsArray<MeshVertexUV>();
        public readonly bool ContainsNormals => ContainsArray<MeshVertexNormal>();
        public readonly bool ContainsTangents => ContainsArray<MeshVertexTangent>();
        public readonly bool ContainsBiTangents => ContainsArray<MeshVertexBiTangent>();
        public readonly bool ContainsColors => ContainsArray<MeshVertexColor>();

        public readonly int VertexCount
        {
            get
            {
                ThrowIfNotLoaded();
                ThrowIfPositionsMissing();

                return GetArrayLength<MeshVertexPosition>();
            }
        }

        public readonly int IndexCount
        {
            get
            {
                ThrowIfNotLoaded();

                return GetArrayLength<MeshVertexIndex>();
            }
        }

        public readonly Collection<Vector3> Positions
        {
            get
            {
                ThrowIfNotLoaded();
                ThrowIfPositionsMissing();

                return new(this, GetArray<MeshVertexPosition>().As<Vector3>());
            }
        }

        public readonly Collection<Vector2> UVs
        {
            get
            {
                ThrowIfNotLoaded();
                ThrowIfUVsMissing();

                return new(this, GetArray<MeshVertexUV>().As<Vector2>());
            }
        }

        public readonly Collection<Vector3> Normals
        {
            get
            {
                ThrowIfNotLoaded();
                ThrowIfNormalsMissing();

                return new(this, GetArray<MeshVertexNormal>().As<Vector3>());
            }
        }

        public readonly Collection<Vector3> Tangents
        {
            get
            {
                ThrowIfNotLoaded();
                ThrowIfTangentsMissing();

                return new(this, GetArray<MeshVertexTangent>().As<Vector3>());
            }
        }

        public readonly Collection<Vector3> BiTangents
        {
            get
            {
                ThrowIfNotLoaded();
                ThrowIfBiTangentsMissing();

                return new(this, GetArray<MeshVertexBiTangent>().As<Vector3>());
            }
        }

        public readonly Collection<Vector4> Colors
        {
            get
            {
                ThrowIfNotLoaded();
                ThrowIfColorsMissing();

                return new(this, GetArray<MeshVertexColor>().As<Vector4>());
            }
        }

        public readonly Collection<uint> Indices
        {
            get
            {
                ThrowIfNotLoaded();

                return new(this, GetArray<MeshVertexIndex>().As<uint>());
            }
        }

        public readonly MeshChannelMask Channels
        {
            get
            {
                ThrowIfNotLoaded();

                MeshChannelMask mask = 0;
                if (ContainsPositions) mask |= MeshChannelMask.Positions;
                if (ContainsUVs) mask |= MeshChannelMask.UVs;
                if (ContainsNormals) mask |= MeshChannelMask.Normals;
                if (ContainsTangents) mask |= MeshChannelMask.Tangents;
                if (ContainsBiTangents) mask |= MeshChannelMask.BiTangents;
                if (ContainsColors) mask |= MeshChannelMask.Colors;
                return mask;
            }
        }

        public readonly uint VertexSize
        {
            get
            {
                ThrowIfNotLoaded();

                return Channels.GetVertexSize();
            }
        }

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

        public readonly uint Version => GetComponent<IsMesh>().version;

        /// <summary>
        /// Creates a blank mesh with no data/channels.
        /// </summary>
        public Mesh(World world)
        {
            this.world = world;
            value = world.CreateEntity(new IsMesh(1));
            CreateArray<MeshVertexIndex>();
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

        public Mesh(World world, ReadOnlySpan<Vector3> positions, ReadOnlySpan<uint> indices)
        {
            this.world = world;
            value = world.CreateEntity(new IsMesh(1));
            CreateArray(positions.As<Vector3, MeshVertexPosition>());
            CreateArray(indices.As<uint, MeshVertexIndex>());
        }

        public Mesh(World world, ReadOnlySpan<Vector3> positions, ReadOnlySpan<Vector2> uvs, ReadOnlySpan<uint> indices)
        {
            this.world = world;
            value = world.CreateEntity(new IsMesh(1));
            CreateArray(positions.As<Vector3, MeshVertexPosition>());
            CreateArray(uvs.As<Vector2, MeshVertexUV>());
            CreateArray(indices.As<uint, MeshVertexIndex>());
        }

        public Mesh(World world, ReadOnlySpan<Vector3> positions, ReadOnlySpan<Vector2> uvs, ReadOnlySpan<Vector3> normals, ReadOnlySpan<uint> indices)
        {
            this.world = world;
            value = world.CreateEntity(new IsMesh(1));
            CreateArray(positions.As<Vector3, MeshVertexPosition>());
            CreateArray(uvs.As<Vector2, MeshVertexUV>());
            CreateArray(normals.As<Vector3, MeshVertexNormal>());
            CreateArray(indices.As<uint, MeshVertexIndex>());
        }

        public Mesh(World world, ReadOnlySpan<Vector3> positions, ReadOnlySpan<Vector2> uvs, ReadOnlySpan<Vector4> colors, ReadOnlySpan<uint> indices)
        {
            this.world = world;
            value = world.CreateEntity(new IsMesh(1));
            CreateArray(positions.As<Vector3, MeshVertexPosition>());
            CreateArray(uvs.As<Vector2, MeshVertexUV>());
            CreateArray(colors.As<Vector4, MeshVertexColor>());
            CreateArray(indices.As<uint, MeshVertexIndex>());
        }

        public Mesh(World world, ReadOnlySpan<Vector3> positions, ReadOnlySpan<Vector2> uvs, ReadOnlySpan<Vector3> normals, ReadOnlySpan<Vector4> colors, ReadOnlySpan<uint> indices)
        {
            this.world = world;
            value = world.CreateEntity(new IsMesh(1));
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

        public readonly override string ToString()
        {
            return value.ToString();
        }

        public readonly bool ContainsChannel(MeshChannel channel)
        {
            return channel switch
            {
                MeshChannel.Position => ContainsPositions,
                MeshChannel.UV => ContainsUVs,
                MeshChannel.Normal => ContainsNormals,
                MeshChannel.Tangent => ContainsTangents,
                MeshChannel.BiTangent => ContainsBiTangents,
                MeshChannel.Color => ContainsColors,
                _ => false,
            };
        }

        /// <summary>
        /// Appends vertex data into the given span, in the order of the channels given.
        /// <para>Missing channels on the mesh are skipped.</para>
        /// </summary>
        /// <returns>How many <see cref="float"/>s were added to <paramref name="vertexData"/>.</returns>
        public readonly int Assemble(Span<float> vertexData, ReadOnlySpan<MeshChannel> channels)
        {
            Span<Vector3> positions = default;
            Span<Vector2> uvs = default;
            Span<Vector3> normals = default;
            Span<Vector3> tangents = default;
            Span<Vector3> biTangents = default;
            Span<Vector4> colors = default;

            for (int i = 0; i < channels.Length; i++)
            {
                MeshChannel channel = channels[i];
                if (channel == MeshChannel.Position)
                {
                    positions = GetArray<MeshVertexPosition>().AsSpan<Vector3>();
                }
                else if (channel == MeshChannel.UV)
                {
                    uvs = GetArray<MeshVertexUV>().AsSpan<Vector2>();
                }
                else if (channel == MeshChannel.Normal)
                {
                    normals = GetArray<MeshVertexNormal>().AsSpan<Vector3>();
                }
                else if (channel == MeshChannel.Tangent)
                {
                    tangents = GetArray<MeshVertexTangent>().AsSpan<Vector3>();
                }
                else if (channel == MeshChannel.BiTangent)
                {
                    biTangents = GetArray<MeshVertexBiTangent>().AsSpan<Vector3>();
                }
                else if (channel == MeshChannel.Color)
                {
                    colors = GetArray<MeshVertexColor>().AsSpan<Vector4>();
                }
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
                        vertexData[index++] = position.X;
                        vertexData[index++] = position.Y;
                        vertexData[index++] = position.Z;
                    }
                    else if (channel == MeshChannel.UV)
                    {
                        Vector2 uv = uvs[i];
                        vertexData[index++] = uv.X;
                        vertexData[index++] = uv.Y;
                    }
                    else if (channel == MeshChannel.Normal)
                    {
                        Vector3 normal = normals[i];
                        vertexData[index++] = normal.X;
                        vertexData[index++] = normal.Y;
                        vertexData[index++] = normal.Z;
                    }
                    else if (channel == MeshChannel.Tangent)
                    {
                        Vector3 tangent = tangents[i];
                        vertexData[index++] = tangent.X;
                        vertexData[index++] = tangent.Y;
                        vertexData[index++] = tangent.Z;
                    }
                    else if (channel == MeshChannel.BiTangent)
                    {
                        Vector3 bitangent = biTangents[i];
                        vertexData[index++] = bitangent.X;
                        vertexData[index++] = bitangent.Y;
                        vertexData[index++] = bitangent.Z;
                    }
                    else if (channel == MeshChannel.Color)
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

        public readonly void IncrementVersion()
        {
            ThrowIfNotLoaded();

            ref IsMesh mesh = ref GetComponent<IsMesh>();
            mesh = mesh.IncrementVersion();
        }

        public readonly Collection<Vector3> CreatePositions(int length)
        {
            ThrowIfPositionsPresent();

            IncrementVersion();
            return new(this, CreateArray<MeshVertexPosition>(length).As<Vector3>());
        }

        public readonly Collection<Vector2> CreateUVs(int length)
        {
            ThrowIfUVsPresent();

            IncrementVersion();
            return new(this, CreateArray<MeshVertexUV>(length).As<Vector2>());
        }

        public readonly Collection<Vector3> CreateNormals(int length)
        {
            ThrowIfNormalsPresent();

            IncrementVersion();
            return new(this, CreateArray<MeshVertexNormal>(length).As<Vector3>());
        }

        public readonly Collection<Vector3> CreateTangents(int length)
        {
            ThrowIfTangentsPresent();

            IncrementVersion();
            return new(this, CreateArray<MeshVertexTangent>(length).As<Vector3>());
        }

        public readonly Collection<Vector3> CreateBiTangents(int length)
        {
            ThrowIfBiTangentsPresent();

            IncrementVersion();
            return new(this, CreateArray<MeshVertexBiTangent>(length).As<Vector3>());
        }

        public readonly Collection<Vector4> CreateColors(int length)
        {
            ThrowIfColorsPresent();

            IncrementVersion();
            return new(this, CreateArray<MeshVertexColor>(length).As<Vector4>());
        }

        public readonly void AddIndices(ReadOnlySpan<uint> indices)
        {
            IncrementVersion();

            Values<MeshVertexIndex> array = GetArray<MeshVertexIndex>();
            int length = array.Length;
            array.Length += indices.Length;
            indices.CopyTo(array.AsSpan<uint>(length));
        }

        public readonly void AddIndex(uint index)
        {
            IncrementVersion();

            Values<MeshVertexIndex> array = GetArray<MeshVertexIndex>();
            array.Length++;
            array[array.Length - 1] = index;
        }

        public readonly void AddTriangle(uint a, uint b, uint c)
        {
            IncrementVersion();

            Values<MeshVertexIndex> array = GetArray<MeshVertexIndex>();
            int length = array.Length;
            array.Length += 3;
            array[length] = a;
            array[length + 1] = b;
            array[length + 2] = c;
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
        private readonly void ThrowIfPositionsPresent()
        {
            if (ContainsPositions)
            {
                throw new InvalidOperationException($"Mesh `{value}` already contains positions");
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
        private readonly void ThrowIfUVsPresent()
        {
            if (ContainsUVs)
            {
                throw new InvalidOperationException($"Mesh `{value}` already contains UVs");
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
        private readonly void ThrowIfNormalsPresent()
        {
            if (ContainsNormals)
            {
                throw new InvalidOperationException($"Mesh `{value}` already contains normals");
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
        private readonly void ThrowIfTangentsPresent()
        {
            if (ContainsTangents)
            {
                throw new InvalidOperationException($"Mesh `{value}` already contains tangents");
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
        private readonly void ThrowIfBiTangentsPresent()
        {
            if (ContainsBiTangents)
            {
                throw new InvalidOperationException($"Mesh `{value}` already contains bi-tangents");
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

        [Conditional("DEBUG")]
        private readonly void ThrowIfColorsPresent()
        {
            if (ContainsColors)
            {
                throw new InvalidOperationException($"Mesh `{value}` already contains colors");
            }
        }

        public ref struct Collection<T> where T : unmanaged
        {
            private readonly Mesh mesh;
            private readonly Values<T> array;
            private bool changed;

            public T this[int index]
            {
                readonly get => array[index];
                set
                {
                    array[index] = value;
                    TryIncrementVersion();
                }
            }

            public int Length
            {
                readonly get => array.Length;
                set
                {
                    array.Length = value;
                    TryIncrementVersion();
                }
            }

            internal Collection(Mesh mesh, Values<T> array)
            {
                this.mesh = mesh;
                this.array = array;
                changed = false;
            }

            private void TryIncrementVersion()
            {
                if (!changed)
                {
                    mesh.IncrementVersion();
                    changed = true;
                }
            }

            /// <summary>
            /// Resizes the array to fit <paramref name="span"/> and copies from it.
            /// </summary>
            public void CopyFrom(System.Span<T> span)
            {
                array.Length = span.Length;
                array.CopyFrom(span);
                TryIncrementVersion();
            }

            public readonly System.Span<T> AsSpan()
            {
                return array.AsSpan();
            }
        }
    }
}