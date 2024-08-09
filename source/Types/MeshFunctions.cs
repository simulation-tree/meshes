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

    public static uint Assemble<T>(this T mesh, UnmanagedList<float> vertexData) where T : IMesh
    {
        Mesh.ChannelMask mask = mesh.GetChannelMask();
        return Assemble(mesh, vertexData, mask);
    }

    /// <summary>
    /// Builds a vertex buffer from the chosen data in the mesh.
    /// </summary>
    /// <returns>Amount of vertex positions.</returns>
    public static uint Assemble<T>(this T mesh, UnmanagedList<float> vertexData, Mesh.ChannelMask mask) where T : IMesh
    {
        uint vertexCount = mesh.GetVertexCount();
        UnmanagedList<MeshVertexPosition> positions = default;
        UnmanagedList<MeshVertexUV> uvs = default;
        UnmanagedList<MeshVertexNormal> normals = default;
        UnmanagedList<MeshVertexTangent> tangents = default;
        UnmanagedList<MeshVertexBitangent> bitangents = default;
        UnmanagedList<MeshVertexColor> colors = default;

        //throw if any channel is missing
        bool wantsPosition = (mask & Mesh.ChannelMask.Positions) != 0;
        if (wantsPosition)
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

        bool wantsUvs = (mask & Mesh.ChannelMask.UVs) != 0;
        if (wantsUvs)
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

        bool wantsNormals = (mask & Mesh.ChannelMask.Normals) != 0;
        if (wantsNormals)
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

        bool wantsTangents = (mask & Mesh.ChannelMask.Tangents) != 0;
        if (wantsTangents)
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

        bool wantsBitangents = (mask & Mesh.ChannelMask.Bitangents) != 0;
        if (wantsBitangents)
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

        bool wantsColors = (mask & Mesh.ChannelMask.Colors) != 0;
        if (wantsColors)
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

        for (uint i = 0; i < vertexCount; i++)
        {
            if (wantsPosition)
            {
                Vector3 position = positions[i].value;
                vertexData.Add(position.X);
                vertexData.Add(position.Y);
                vertexData.Add(position.Z);
            }

            if (wantsUvs)
            {
                Vector2 uv = uvs[i].value;
                vertexData.Add(uv.X);
                vertexData.Add(uv.Y);
            }

            if (wantsNormals)
            {
                Vector3 normal = normals[i].value;
                vertexData.Add(normal.X);
                vertexData.Add(normal.Y);
                vertexData.Add(normal.Z);
            }

            if (wantsTangents)
            {
                Vector3 tangent = tangents[i].value;
                vertexData.Add(tangent.X);
                vertexData.Add(tangent.Y);
                vertexData.Add(tangent.Z);
            }

            if (wantsBitangents)
            {
                Vector3 bitangent = bitangents[i].value;
                vertexData.Add(bitangent.X);
                vertexData.Add(bitangent.Y);
                vertexData.Add(bitangent.Z);
            }

            if (wantsColors)
            {
                Vector4 color = colors[i].value;
                vertexData.Add(color.X);
                vertexData.Add(color.Y);
                vertexData.Add(color.Z);
                vertexData.Add(color.W);
            }
        }

        return vertexCount;
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