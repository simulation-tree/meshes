using System;
using System.Numerics;
using Unmanaged;

namespace Meshes
{
    public static class ChannelExtensions
    {
        public static Type GetCollectionType(this Mesh.Channel channel)
        {
            return channel switch
            {
                Mesh.Channel.Position => typeof(Vector3),
                Mesh.Channel.UV => typeof(Vector2),
                Mesh.Channel.Normal => typeof(Vector3),
                Mesh.Channel.Tangent => typeof(Vector3),
                Mesh.Channel.BiTangent => typeof(Vector3),
                Mesh.Channel.Color => typeof(Vector4),
                _ => throw new NotSupportedException($"Unsupported channel `{channel}`")
            };
        }

        public static Mesh.ChannelMask AddChannel(ref Mesh.ChannelMask mask, Mesh.Channel channel)
        {
            return channel switch
            {
                Mesh.Channel.Position => mask |= Mesh.ChannelMask.Positions,
                Mesh.Channel.UV => mask |= Mesh.ChannelMask.UVs,
                Mesh.Channel.Normal => mask |= Mesh.ChannelMask.Normals,
                Mesh.Channel.Tangent => mask |= Mesh.ChannelMask.Tangents,
                Mesh.Channel.BiTangent => mask |= Mesh.ChannelMask.BiTangents,
                Mesh.Channel.Color => mask |= Mesh.ChannelMask.Colors,
                _ => throw new NotSupportedException($"Unsupported channel `{channel}`")
            };
        }

        public static Mesh.ChannelMask GetChannelMask(this USpan<Mesh.Channel> channels)
        {
            Mesh.ChannelMask mask = 0;
            for (byte i = 0; i < channels.Length; i++)
            {
                Mesh.Channel channel = channels[i];
                mask = channel switch
                {
                    Mesh.Channel.Position => mask |= Mesh.ChannelMask.Positions,
                    Mesh.Channel.UV => mask |= Mesh.ChannelMask.UVs,
                    Mesh.Channel.Normal => mask |= Mesh.ChannelMask.Normals,
                    Mesh.Channel.Tangent => mask |= Mesh.ChannelMask.Tangents,
                    Mesh.Channel.BiTangent => mask |= Mesh.ChannelMask.BiTangents,
                    Mesh.Channel.Color => mask |= Mesh.ChannelMask.Colors,
                    _ => throw new NotSupportedException($"Unsupported channel `{channel}`")
                };
            }

            return mask;
        }

        public static uint GetVertexSize(this Mesh.ChannelMask mask)
        {
            uint size = 0;
            if ((mask & Mesh.ChannelMask.Positions) != 0)
            {
                size += 3;
            }

            if ((mask & Mesh.ChannelMask.UVs) != 0)
            {
                size += 2;
            }

            if ((mask & Mesh.ChannelMask.Normals) != 0)
            {
                size += 3;
            }

            if ((mask & Mesh.ChannelMask.Tangents) != 0)
            {
                size += 3;
            }

            if ((mask & Mesh.ChannelMask.BiTangents) != 0)
            {
                size += 3;
            }

            if ((mask & Mesh.ChannelMask.Colors) != 0)
            {
                size += 4;
            }

            return size;
        }

        public static uint GetVertexSize(this USpan<Mesh.Channel> channels)
        {
            uint size = 0;
            for (byte i = 0; i < channels.Length; i++)
            {
                Mesh.Channel channel = channels[i];
                size += channel switch
                {
                    Mesh.Channel.Position => 3,
                    Mesh.Channel.UV => 2,
                    Mesh.Channel.Normal => 3,
                    Mesh.Channel.Tangent => 3,
                    Mesh.Channel.BiTangent => 3,
                    Mesh.Channel.Color => 4,
                    _ => throw new NotSupportedException($"Unsupported channel `{channel}`")
                };
            }

            return size;
        }
    }
}
