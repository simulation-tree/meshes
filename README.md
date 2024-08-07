# Meshes
Data type for storing 3D shapes/geometry.

### Quad mesh example
```csharp
Mesh mesh = new(world);
Mesh.Collection<Vector3> positions = mesh.CreatePositions();
Mesh.Collection<Vector2> uvs = mesh.CreateUVs();
positions.Add(new(0, 0, 0));
positions.Add(new(1, 0, 0));
positions.Add(new(1, 1, 0));
positions.Add(new(0, 1, 0));
uvs.Add(new(0, 0));
uvs.Add(new(1, 0));
uvs.Add(new(1, 1));
uvs.Add(new(0, 1));
mesh.AddTriangle(0, 1, 2);
mesh.AddTriangle(0, 2, 3);

using UnmanagedList<float> vertexBuffer = new();
mesh.ReadVertexData(vertexBuffer);

ReadOnlySpan<uint> vertexIndices = mesh.GetVertexIndices();
```