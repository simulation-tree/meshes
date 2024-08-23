# Meshes
Data type for storing 3D shapes/geometry.

### Usage
Below is an example of building a quad mesh from scratch:
```cs
Mesh mesh = new(world);
Mesh.Collection<Vector3> positions = mesh.CreatePositions();
Mesh.Collection<Vector2> uvs = mesh.CreateUVs();
Mesh.Collection<Vector3> normals = mesh.CreateNormals();
positions.Add(new(0, 0, 0));
positions.Add(new(1, 0, 0));
positions.Add(new(1, 1, 0));
positions.Add(new(0, 1, 0));
uvs.Add(new(0, 0));
uvs.Add(new(1, 0));
uvs.Add(new(1, 1));
uvs.Add(new(0, 1));
normals.Add(new(0, 0, 1));
normals.Add(new(0, 0, 1));
normals.Add(new(0, 0, 1));
normals.Add(new(0, 0, 1));
mesh.AddTriangle(0, 1, 2);
mesh.AddTriangle(0, 2, 3);

ReadOnlySpan<uint> vertexIndices = mesh.Indices.AsSpan();
```

### Assembling
Often mesh data is wanted in other formats by other systems. The `Assemble` method can be used to
retrieve a flat list of `float` values containing it in the expected order:
```cs
using UnmanagedList<float> vertexData = new();
Span<Mesh.Channel> channels = stackalloc Mesh.Channel[2];
channels[0] = Mesh.Channel.Position;
channels[1] = Mesh.Channel.UV;
meshEntity.Assemble(vertexData, channels);
```