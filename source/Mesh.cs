using Data.Components;
using Data.Events;
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
        private readonly Entity entity;

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

        /// <summary>
        /// Creates a mesh entity from a model address.
        /// </summary>
        public Mesh(World world, FixedString modelAddress, uint meshIndex = 0)
        {
            entity = new(world);
            entity.AddComponent(new IsMesh());
            entity.AddComponent(new IsDataRequest(modelAddress));
            entity.AddComponent(new IsMeshRequest(meshIndex));
            entity.CreateList<Entity, uint>();

            world.Submit(new DataUpdate());
            world.Poll();
        }

        public readonly void Dispose()
        {
            entity.Dispose();
        }

        public readonly override string ToString()
        {
            return entity.ToString();
        }

        Query IEntity.GetQuery(World world)
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
                Channel.BiTangent => RuntimeType.Get<Vector3>(),
                Channel.Color => RuntimeType.Get<Vector4>(),
                _ => throw new NotSupportedException($"Unsupported channel {channel}")
            };
        }
        
        //todo: efficiency: this can be better optimized by batching modifications, then incrementing version when changes are submitted
        //rather than on every individual operation
        public readonly struct Collection<T> : IList<T> where T : unmanaged, IEquatable<T>
        {
            private readonly UnmanagedList<T> list;
            private readonly Entity entity;

            public readonly T this[uint index]
            {
                get => list[index];
                set
                {
                    list[index] = value;
                    Modified();
                }
            }

            public readonly int Count => (int)list.Count;

            bool ICollection<T>.IsReadOnly => false;

            T IList<T>.this[int index]
            {
                get => list[(uint)index];
                set => list[(uint)index] = value;
            }

            internal unsafe Collection(UnsafeList* list, Entity entity)
            {
                this.list = new(list);
                this.entity = entity;
            }

            public readonly ReadOnlySpan<T> AsSpan()
            {
                return list.AsSpan();
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
            BiTangent = 4,
            Color = 5
        }
    }
}
