using System;
using System.Numerics;
using Unmanaged;

namespace Meshes
{
    public static class MeshChannelExtensions
    {
        public static Type GetCollectionType(this MeshChannel channel)
        {
            return channel switch
            {
                MeshChannel.Position => typeof(Vector3),
                MeshChannel.UV => typeof(Vector2),
                MeshChannel.Normal => typeof(Vector3),
                MeshChannel.Tangent => typeof(Vector3),
                MeshChannel.BiTangent => typeof(Vector3),
                MeshChannel.Color => typeof(Vector4),
                _ => throw new NotSupportedException($"Unsupported channel `{channel}`")
            };
        }

        public static MeshChannelMask AddChannel(ref MeshChannelMask mask, MeshChannel channel)
        {
            return channel switch
            {
                MeshChannel.Position => mask |= MeshChannelMask.Positions,
                MeshChannel.UV => mask |= MeshChannelMask.UVs,
                MeshChannel.Normal => mask |= MeshChannelMask.Normals,
                MeshChannel.Tangent => mask |= MeshChannelMask.Tangents,
                MeshChannel.BiTangent => mask |= MeshChannelMask.BiTangents,
                MeshChannel.Color => mask |= MeshChannelMask.Colors,
                _ => throw new NotSupportedException($"Unsupported channel `{channel}`")
            };
        }

        public static MeshChannelMask GetChannelMask(this USpan<MeshChannel> channels)
        {
            MeshChannelMask mask = 0;
            for (uint i = 0; i < channels.Length; i++)
            {
                MeshChannel channel = channels[i];
                mask = channel switch
                {
                    MeshChannel.Position => mask |= MeshChannelMask.Positions,
                    MeshChannel.UV => mask |= MeshChannelMask.UVs,
                    MeshChannel.Normal => mask |= MeshChannelMask.Normals,
                    MeshChannel.Tangent => mask |= MeshChannelMask.Tangents,
                    MeshChannel.BiTangent => mask |= MeshChannelMask.BiTangents,
                    MeshChannel.Color => mask |= MeshChannelMask.Colors,
                    _ => throw new NotSupportedException($"Unsupported channel `{channel}`")
                };
            }

            return mask;
        }

        public static uint GetVertexSize(this MeshChannelMask mask)
        {
            uint size = 0;
            if ((mask & MeshChannelMask.Positions) != 0)
            {
                size += 3;
            }

            if ((mask & MeshChannelMask.UVs) != 0)
            {
                size += 2;
            }

            if ((mask & MeshChannelMask.Normals) != 0)
            {
                size += 3;
            }

            if ((mask & MeshChannelMask.Tangents) != 0)
            {
                size += 3;
            }

            if ((mask & MeshChannelMask.BiTangents) != 0)
            {
                size += 3;
            }

            if ((mask & MeshChannelMask.Colors) != 0)
            {
                size += 4;
            }

            return size;
        }

        public static uint GetVertexSize(this USpan<MeshChannel> channels)
        {
            uint size = 0;
            for (uint i = 0; i < channels.Length; i++)
            {
                MeshChannel channel = channels[i];
                size += channel switch
                {
                    MeshChannel.Position => 3,
                    MeshChannel.UV => 2,
                    MeshChannel.Normal => 3,
                    MeshChannel.Tangent => 3,
                    MeshChannel.BiTangent => 3,
                    MeshChannel.Color => 4,
                    _ => throw new NotSupportedException($"Unsupported channel `{channel}`")
                };
            }

            return size;
        }
    }
}
