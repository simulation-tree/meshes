using System;
using System.Numerics;

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
    }
}
