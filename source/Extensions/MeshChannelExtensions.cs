using System;
using System.Numerics;
using Types;

namespace Meshes
{
    /// <summary>
    /// Extension class mesh channels.
    /// </summary>
    public static class MeshChannelExtensions
    {
        /// <summary>
        /// Retrieves the intended data type for the given <paramref name="channel"/>.
        /// </summary>
        public static TypeMetadata GetCollectionType(this MeshChannel channel)
        {
            return channel switch
            {
                MeshChannel.Position => TypeMetadata.Get<Vector3>(),
                MeshChannel.UV => TypeMetadata.Get<Vector2>(),
                MeshChannel.Normal => TypeMetadata.Get<Vector3>(),
                MeshChannel.Tangent => TypeMetadata.Get<Vector3>(),
                MeshChannel.BiTangent => TypeMetadata.Get<Vector3>(),
                MeshChannel.Color => TypeMetadata.Get<Vector4>(),
                _ => throw new NotSupportedException($"Unsupported channel `{channel}`")
            };
        }

        /// <summary>
        /// Modifies the <paramref name="mask"/> to include the given <paramref name="channel"/>.
        /// </summary>
        public static MeshChannelMask AddChannel(this ref MeshChannelMask mask, MeshChannel channel)
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

        /// <summary>
        /// Retrieves the channel mask for the given span of <paramref name="channels"/>.
        /// </summary>
        public static MeshChannelMask GetChannelMask(this ReadOnlySpan<MeshChannel> channels)
        {
            MeshChannelMask mask = 0;
            for (int i = 0; i < channels.Length; i++)
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

        /// <summary>
        /// Retrieves the size in <see cref="float"/>s for a single vertex based on the given <paramref name="mask"/>.
        /// </summary>
        public static int GetVertexSize(this MeshChannelMask mask)
        {
            int size = 0;
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

        /// <summary>
        /// Retrieves the size in <see cref="float"/>s for a single vertex based on the given span of <paramref name="channels"/>.
        /// </summary>
        public static int GetVertexSize(this ReadOnlySpan<MeshChannel> channels)
        {
            int size = 0;
            for (int i = 0; i < channels.Length; i++)
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

        /// <summary>
        /// Retrieves the size in <see cref="float"/>s for a single vertex based on the given span of <paramref name="channels"/>.
        /// </summary>
        public static int GetVertexSize(this Span<MeshChannel> channels)
        {
            int size = 0;
            for (int i = 0; i < channels.Length; i++)
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