# Meshes

Data type for storing 3D shapes/geometry.

### Usage

Below is an example of building a quad mesh from scratch:
```cs
Mesh mesh = new(world);
USpan<Vector3> positions = mesh.CreatePositions(4);
USpan<Vector2> uvs = mesh.CreateUVs(4);
USpan<Vector3> normals = mesh.CreateNormals(4);
positions[0] = new(0, 0, 0);
positions[1] = new(1, 0, 0);
positions[2] = new(1, 1, 0);
positions[3] = new(0, 1, 0);
uvs[0] = new(0, 0);
uvs[1] = new(1, 0);
uvs[2] = new(1, 1);
uvs[3] = new(0, 1);
normals[0] = new(0, 0, 1);
normals[1] = new(0, 0, 1);
normals[2] = new(0, 0, 1);
normals[3] = new(0, 0, 1);
mesh.AddTriangle(0, 1, 2);
mesh.AddTriangle(2, 3, 0);

USpan<uint> indices = quadMesh.GetIndices();
```

### Updating after creation

Whenever arrays of a mesh are modified in length, the version of the mesh increments.
This gives feedback for systems to update when necessary. But for the cases where the
array lengths remain the same and only contents change, the `IncrementVersion` function
should be used:
```cs
USpan<Vector3> positions = mesh.GetPositions();
positions[0] *= 2;
positions[1] *= 2;
positions[2] *= 2;
positions[3] *= 2;
mesh.IncrementVersion();

positions = mesh.ResizePositions(8);
positions[4] = new(0, 0, 0);
positions[5] = new(1, 0, 0);
positions[6] = new(1, 1, 0);
positions[7] = new(0, 1, 0);
//incrementing version is already done
```

### Assembling

Often mesh data is wanted in a specific formats . The `Assemble` function can be used to
retrieve a flat list of `float` values containing vertex data in the specified order:
```cs
USpan<Mesh.Channel> channels = [Mesh.Channel.Position, Mesh.Channel.UV];
USpan<float> vertexData = stackalloc float[(int)channels.GetVertexSize() * 4];
quadMesh.Assemble(vertexData, channels);
```