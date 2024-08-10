using Meshes;
using Meshes.Components;
using System;
using System.Numerics;
using Unmanaged.Collections;

public static class MeshFunctions
{
    public static bool HasPositions<T>(this T mesh) where T : IMesh
    {
        return mesh.ContainsList<T, MeshVertexPosition>();
    }

    public static bool HasUVs<T>(this T mesh) where T : IMesh
    {
        return mesh.ContainsList<T, MeshVertexUV>();
    }

    public static bool HasNormals<T>(this T mesh) where T : IMesh
    {
        return mesh.ContainsList<T, MeshVertexNormal>();
    }

    public static bool HasTangents<T>(this T mesh) where T : IMesh
    {
        return mesh.ContainsList<T, MeshVertexTangent>();
    }

    public static bool HasBiTangents<T>(this T mesh) where T : IMesh
    {
        return mesh.ContainsList<T, MeshVertexBitangent>();
    }

    public static bool HasColors<T>(this T mesh) where T : IMesh
    {
        return mesh.ContainsList<T, MeshVertexColor>();
    }

    public static Mesh.ChannelMask GetChannelMask<T>(this T mesh) where T : IMesh
    {
        bool hasPositions = mesh.HasPositions();
        bool hasUVs = mesh.HasUVs();
        bool hasNormals = mesh.HasNormals();
        bool hasTangents = mesh.HasTangents();
        bool hasBiTangents = mesh.HasBiTangents();
        bool hasColors = mesh.HasColors();
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

    public unsafe static Mesh.Collection<uint> GetIndices<T>(this T mesh) where T : IMesh
    {
        UnsafeList* list = (UnsafeList*)mesh.GetList<T, uint>().AsPointer();
        return new(list, mesh.AsEntity());
    }

    public unsafe static Mesh.Collection<Vector3> GetPositions<T>(this T mesh) where T : IMesh
    {
        UnsafeList* list = (UnsafeList*)mesh.GetList<T, MeshVertexPosition>().AsPointer();
        return new(list, mesh.AsEntity());
    }

    public unsafe static Mesh.Collection<Vector2> GetUVs<T>(this T mesh) where T : IMesh
    {
        UnsafeList* list = (UnsafeList*)mesh.GetList<T, MeshVertexUV>().AsPointer();
        return new(list, mesh.AsEntity());
    }

    public unsafe static Mesh.Collection<Vector3> GetNormals<T>(this T mesh) where T : IMesh
    {
        UnsafeList* list = (UnsafeList*)mesh.GetList<T, MeshVertexNormal>().AsPointer();
        return new(list, mesh.AsEntity());
    }

    public unsafe static Mesh.Collection<Vector3> GetTangents<T>(this T mesh) where T : IMesh
    {
        UnsafeList* list = (UnsafeList*)mesh.GetList<T, MeshVertexTangent>().AsPointer();
        return new(list, mesh.AsEntity());
    }

    public unsafe static Mesh.Collection<Vector3> GetBiTangents<T>(this T mesh) where T : IMesh
    {
        UnsafeList* list = (UnsafeList*)mesh.GetList<T, MeshVertexBitangent>().AsPointer();
        return new(list, mesh.AsEntity());
    }

    public unsafe static Mesh.Collection<Vector4> GetColors<T>(this T mesh) where T : IMesh
    {
        UnsafeList* list = (UnsafeList*)mesh.GetList<T, MeshVertexColor>().AsPointer();
        return new(list, mesh.AsEntity());
    }

    public static uint GetVertexCount<T>(this T mesh) where T : IMesh
    {
        bool hasPositions = HasPositions(mesh);
        if (!hasPositions)
        {
            return 0;
        }

        return mesh.GetList<T, MeshVertexPosition>().Count;
    }

    public static uint GetIndexCount<T>(this T mesh) where T : IMesh
    {
        return mesh.GetList<T, uint>().Count;
    }

    public static uint GetVersion<T>(this T mesh) where T : IMesh
    {
        IsMesh component = mesh.GetComponent<T, IsMesh>();
        return component.version;
    }

    public static (Vector3 min, Vector3 max) GetBounds<T>(this T mesh) where T : IMesh
    {
        bool hasPositions = HasPositions(mesh);
        if (hasPositions)
        {
            UnmanagedList<MeshVertexPosition> positions = mesh.GetList<T, MeshVertexPosition>();
            Vector3 min = new(float.MaxValue);
            Vector3 max = new(float.MinValue);
            for (uint i = 0; i < positions.Count; i++)
            {
                Vector3 position = positions[i].value;
                min = Vector3.Min(min, position);
                max = Vector3.Max(max, position);
            }

            return (min, max);
        }
        else return default;
    }

    public unsafe static Mesh.Collection<Vector3> CreatePositions<T>(this T mesh) where T : IMesh
    {
        if (HasPositions(mesh))
        {
            throw new InvalidOperationException("Mesh already contains positions.");
        }

        UnmanagedList<MeshVertexPosition> list = mesh.CreateList<T, MeshVertexPosition>();
        return new((UnsafeList*)list.AsPointer(), mesh.AsEntity());
    }

    public unsafe static Mesh.Collection<Vector2> CreateUVs<T>(this T mesh) where T : IMesh
    {
        if (HasUVs(mesh))
        {
            throw new InvalidOperationException("Mesh already contains uvs.");
        }

        UnmanagedList<MeshVertexUV> list = mesh.CreateList<T, MeshVertexUV>();
        return new((UnsafeList*)list.AsPointer(), mesh.AsEntity());
    }

    public unsafe static Mesh.Collection<Vector3> CreateNormals<T>(this T mesh) where T : IMesh
    {
        if (HasNormals(mesh))
        {
            throw new InvalidOperationException("Mesh already contains normals.");
        }

        UnmanagedList<MeshVertexNormal> list = mesh.CreateList<T, MeshVertexNormal>();
        return new((UnsafeList*)list.AsPointer(), mesh.AsEntity());
    }

    public unsafe static Mesh.Collection<Vector3> CreateTangents<T>(this T mesh) where T : IMesh
    {
        if (HasTangents(mesh))
        {
            throw new InvalidOperationException("Mesh already contains tangents.");
        }

        UnmanagedList<MeshVertexTangent> list = mesh.CreateList<T, MeshVertexTangent>();
        return new((UnsafeList*)list.AsPointer(), mesh.AsEntity());
    }

    public unsafe static Mesh.Collection<Vector3> CreateBiTangents<T>(this T mesh) where T : IMesh
    {
        if (HasBiTangents(mesh))
        {
            throw new InvalidOperationException("Mesh already contains bitangents.");
        }

        UnmanagedList<MeshVertexBitangent> list = mesh.CreateList<T, MeshVertexBitangent>();
        return new((UnsafeList*)list.AsPointer(), mesh.AsEntity());
    }

    public unsafe static Mesh.Collection<Vector4> CreateColors<T>(this T mesh) where T : IMesh
    {
        if (HasColors(mesh))
        {
            throw new InvalidOperationException("Mesh already contains colors.");
        }

        UnmanagedList<MeshVertexColor> list = mesh.CreateList<T, MeshVertexColor>();
        return new((UnsafeList*)list.AsPointer(), mesh.AsEntity());
    }

    public static void AddIndices<T>(this T mesh, ReadOnlySpan<uint> indices) where T : IMesh
    {
        UnmanagedList<uint> list = mesh.GetList<T, uint>();
        list.AddRange(indices);
    }

    public static void AddIndex<T>(this T mesh, uint index) where T : IMesh
    {
        UnmanagedList<uint> list = mesh.GetList<T, uint>();
        list.Add(index);
    }

    public static void AddTriangle<T>(this T mesh, uint a, uint b, uint c) where T : IMesh
    {
        UnmanagedList<uint> list = mesh.GetList<T, uint>();
        list.Add(a);
        list.Add(b);
        list.Add(c);
    }

    public static bool ContainsChannel<T>(this T mesh, Mesh.ChannelMask mask) where T : IMesh
    {
        if ((mask & Mesh.ChannelMask.Positions) != 0 && !HasPositions(mesh))
        {
            return false;
        }

        if ((mask & Mesh.ChannelMask.UVs) != 0 && !HasUVs(mesh))
        {
            return false;
        }

        if ((mask & Mesh.ChannelMask.Normals) != 0 && !HasNormals(mesh))
        {
            return false;
        }

        if ((mask & Mesh.ChannelMask.Tangents) != 0 && !HasTangents(mesh))
        {
            return false;
        }

        if ((mask & Mesh.ChannelMask.Bitangents) != 0 && !HasBiTangents(mesh))
        {
            return false;
        }

        if ((mask & Mesh.ChannelMask.Colors) != 0 && !HasColors(mesh))
        {
            return false;
        }

        return true;
    }

    public static bool ContainsChannel<T>(this T mesh, Mesh.Channel channel) where T : IMesh
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

    /// <summary>
    /// Appends all vertex data to the given list, in the order of channels.
    /// </summary>
    /// <returns>Size of a vertex in floats.</returns>
    public static uint Assemble<T>(this T mesh, UnmanagedList<float> vertexData, ReadOnlySpan<Mesh.Channel> channels) where T : IMesh
    {
        UnmanagedList<MeshVertexPosition> positions = default;
        UnmanagedList<MeshVertexUV> uvs = default;
        UnmanagedList<MeshVertexNormal> normals = default;
        UnmanagedList<MeshVertexTangent> tangents = default;
        UnmanagedList<MeshVertexBitangent> bitangents = default;
        UnmanagedList<MeshVertexColor> colors = default;

        static bool Contains(ReadOnlySpan<Mesh.Channel> channels, Mesh.Channel channel)
        {
            for (int i = 0; i < channels.Length; i++)
            {
                if (channels[i] == channel)
                {
                    return true;
                }
            }

            return false;
        }

        //throw if any channel is missing
        if (Contains(channels, Mesh.Channel.Position))
        {
            if (!HasPositions(mesh))
            {
                throw new InvalidOperationException("Mesh does not contain positions.");
            }
            else
            {
                positions = mesh.GetList<T, MeshVertexPosition>();
            }
        }

        if (Contains(channels, Mesh.Channel.UV))
        {
            if (!HasUVs(mesh))
            {
                throw new InvalidOperationException("Mesh does not contain texture coordinates.");
            }
            else
            {
                uvs = mesh.GetList<T, MeshVertexUV>();
            }
        }

        if (Contains(channels, Mesh.Channel.Normal))
        {
            if (!HasNormals(mesh))
            {
                throw new InvalidOperationException("Mesh does not contain normals.");
            }
            else
            {
                normals = mesh.GetList<T, MeshVertexNormal>();
            }
        }

        if (Contains(channels, Mesh.Channel.Tangent))
        {
            if (!HasTangents(mesh))
            {
                throw new InvalidOperationException("Mesh does not contain tangents.");
            }
            else
            {
                tangents = mesh.GetList<T, MeshVertexTangent>();
            }
        }

        if (Contains(channels, Mesh.Channel.BiTangent))
        {
            if (!HasBiTangents(mesh))
            {
                throw new InvalidOperationException("Mesh does not contain bitangents.");
            }
            else
            {
                bitangents = mesh.GetList<T, MeshVertexBitangent>();
            }
        }

        if (Contains(channels, Mesh.Channel.Color))
        {
            if (!HasColors(mesh))
            {
                throw new InvalidOperationException("Mesh does not contain colors.");
            }
            else
            {
                colors = mesh.GetList<T, MeshVertexColor>();
            }
        }

        uint vertexCount = mesh.GetVertexCount();
        for (uint i = 0; i < vertexCount; i++)
        {
            for (int c = 0; c < channels.Length; c++)
            {
                Mesh.Channel channel = channels[c];
                if (channel == Mesh.Channel.Position)
                {
                    Vector3 position = positions[i].value;
                    vertexData.Add(position.X);
                    vertexData.Add(position.Y);
                    vertexData.Add(position.Z);
                }
                else if (channel == Mesh.Channel.UV)
                {
                    Vector2 uv = uvs[i].value;
                    vertexData.Add(uv.X);
                    vertexData.Add(uv.Y);
                }
                else if (channel == Mesh.Channel.Normal)
                {
                    Vector3 normal = normals[i].value;
                    vertexData.Add(normal.X);
                    vertexData.Add(normal.Y);
                    vertexData.Add(normal.Z);
                }
                else if (channel == Mesh.Channel.Tangent)
                {
                    Vector3 tangent = tangents[i].value;
                    vertexData.Add(tangent.X);
                    vertexData.Add(tangent.Y);
                    vertexData.Add(tangent.Z);
                }
                else if (channel == Mesh.Channel.BiTangent)
                {
                    Vector3 bitangent = bitangents[i].value;
                    vertexData.Add(bitangent.X);
                    vertexData.Add(bitangent.Y);
                    vertexData.Add(bitangent.Z);
                }
                else if (channel == Mesh.Channel.Color)
                {
                    Vector4 color = colors[i].value;
                    vertexData.Add(color.X);
                    vertexData.Add(color.Y);
                    vertexData.Add(color.Z);
                    vertexData.Add(color.W);
                }
            }
        }

        uint vertexSize = 0;
        for (int c = 0; c < channels.Length; c++)
        {
            Mesh.Channel channel = channels[c];
            if (channel == Mesh.Channel.Position)
            {
                vertexSize += 3;
            }
            else if (channel == Mesh.Channel.UV)
            {
                vertexSize += 2;
            }
            else if (channel == Mesh.Channel.Normal)
            {
                vertexSize += 3;
            }
            else if (channel == Mesh.Channel.Tangent)
            {
                vertexSize += 3;
            }
            else if (channel == Mesh.Channel.BiTangent)
            {
                vertexSize += 3;
            }
            else if (channel == Mesh.Channel.Color)
            {
                vertexSize += 4;
            }
        }

        return vertexSize;
    }

    public static Mesh.ChannelMask AddChannel(ref this Mesh.ChannelMask mask, Mesh.Channel channel)
    {
        return channel switch
        {
            Mesh.Channel.Position => mask |= Mesh.ChannelMask.Positions,
            Mesh.Channel.UV => mask |= Mesh.ChannelMask.UVs,
            Mesh.Channel.Normal => mask |= Mesh.ChannelMask.Normals,
            Mesh.Channel.Tangent => mask |= Mesh.ChannelMask.Tangents,
            Mesh.Channel.BiTangent => mask |= Mesh.ChannelMask.Bitangents,
            Mesh.Channel.Color => mask |= Mesh.ChannelMask.Colors,
            _ => throw new NotSupportedException($"Unsupported channel {channel}")
        };
    }
}