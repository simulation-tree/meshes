using Meshes.Components;
using Simulation;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using Unmanaged;
using Unmanaged.Collections;

namespace Meshes
{
    public readonly struct Mesh : IMesh, IDisposable
    {
        public readonly Entity entity;

        eint IEntity.Value => entity.value;
        World IEntity.World => entity.world;

        public Mesh(World world, eint existingEntity)
        {
            this.entity = new(world, existingEntity);
        }

        /// <summary>
        /// Creates an empty mesh entity.
        /// </summary>
        public Mesh(World world)
        {
            entity = new(world);
            entity.AddComponent(new IsMesh());
            entity.CreateList<Entity, uint>();
        }

        public readonly void Dispose()
        {
            entity.Dispose();
        }

        public readonly override string ToString()
        {
            return entity.ToString();
        }

        public static Query GetQuery(World world)
        {
            return new(world, RuntimeType.Get<IsMesh>());
        }

        public static RuntimeType GetCollectionType(Channel channel)
        {
            return channel switch
            {
                Channel.Position => RuntimeType.Get<Vector3>(),
                Channel.UV => RuntimeType.Get<Vector2>(),
                Channel.Normal => RuntimeType.Get<Vector3>(),
                Channel.Tangent => RuntimeType.Get<Vector3>(),
                Channel.Bitangent => RuntimeType.Get<Vector3>(),
                Channel.Color => RuntimeType.Get<Vector4>(),
                _ => throw new NotSupportedException($"Unsupported channel {channel}")
            };
        }
        
        public readonly struct Collection<T> : IList<T> where T : unmanaged, IEquatable<T>
        {
            private readonly UnmanagedList<T> list;
            private readonly Entity entity;

            public readonly T this[int index]
            {
                get => list[(uint)index];
                set => list[(uint)index] = value;
            }

            public readonly int Count => (int)list.Count;

            bool ICollection<T>.IsReadOnly => false;

            internal unsafe Collection(UnsafeList* list, Entity entity)
            {
                this.list = new(list);
                this.entity = entity;
            }

            private readonly void Modified()
            {
                ref IsMesh mesh = ref entity.GetComponentRef<Entity, IsMesh>();
                mesh.version++;
            }

            public readonly void Add(T item)
            {
                list.Add(item);
                Modified();
            }

            public readonly void Clear()
            {
                list.Clear();
                Modified();
            }

            public readonly bool Contains(T item)
            {
                return list.Contains(item);
            }

            void ICollection<T>.CopyTo(T[] array, int arrayIndex)
            {
                list.AsSpan().CopyTo(array.AsSpan(arrayIndex));
            }

            public readonly UnmanagedList<T>.Enumerator GetEnumerator()
            {
                return list.GetEnumerator();
            }

            IEnumerator<T> IEnumerable<T>.GetEnumerator()
            {
                return GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }

            public readonly int IndexOf(T item)
            {
                if (list.TryIndexOf(item, out uint index))
                {
                    return (int)index;
                }
                else return -1;
            }

            public readonly void Insert(int index, T item)
            {
                list.Insert((uint)index, item);
                Modified();
            }

            public readonly bool Remove(T item)
            {
                if (list.TryIndexOf(item, out uint index))
                {
                    list.RemoveAt(index);
                    Modified();
                    return true;
                }
                else return false;
            }

            public readonly void RemoveAt(int index)
            {
                list.RemoveAt((uint)index);
                Modified();
            }
        }

        [Flags]
        public enum ChannelMask : byte
        {
            Positions = 1,
            UVs = 2,
            Normals = 4,
            Tangents = 8,
            Bitangents = 16,
            Colors = 32
        }

        public enum Channel : byte
        {
            Position = 0,
            UV = 1,
            Normal = 2,
            Tangent = 3,
            Bitangent = 4,
            Color = 5
        }
    }
}
