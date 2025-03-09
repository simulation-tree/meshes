using Collections.Generic;
using System;
using System.Numerics;
using Worlds;

namespace Meshes.Tests
{
    public class MeshEntityTests : MeshTests
    {
        [Test]
        public void CreateQuadMesh()
        {
            using World world = CreateWorld();
            Mesh mesh = new(world);
            Mesh.Collection<Vector3> positions = mesh.CreatePositions(4);
            Mesh.Collection<Vector4> colors = mesh.CreateColors(4);
            Mesh.Collection<Vector2> uvs = mesh.CreateUVs(4);
            positions[0] = new(0f, 0f, 0f);
            positions[1] = new(1f, 0f, 0f);
            positions[2] = new(1f, 1f, 0f);
            positions[3] = new(0f, 1f, 0f);
            colors[0] = new(1f, 0f, 0f, 1f);
            colors[1] = new(0f, 1f, 0f, 1f);
            colors[2] = new(0f, 0f, 1f, 1f);
            colors[3] = new(1f, 1f, 1f, 1f);
            uvs[0] = new(0f, 0f);
            uvs[1] = new(1f, 0f);
            uvs[2] = new(1f, 1f);
            uvs[3] = new(0f, 1f);
            Assert.That(mesh.ContainsPositions, Is.True);
            Assert.That(mesh.VertexCount, Is.EqualTo(4));
            Assert.That(mesh.ContainsNormals, Is.False);
            Assert.That(mesh.ContainsUVs, Is.True);
            Assert.That(mesh.ContainsColors, Is.True);
            Assert.That(mesh.ContainsTangents, Is.False);

            (Vector3 min, Vector3 max) bounds = mesh.Bounds;
            Assert.That(bounds.min, Is.EqualTo(new Vector3(0, 0, 0)));
            Assert.That(bounds.max, Is.EqualTo(new Vector3(1, 1, 0)));
        }

        [Test]
        public void CheckMeshCollection()
        {
            using World world = CreateWorld();
            Mesh mesh = new(world);
            Assert.That(mesh.Version, Is.EqualTo(1));

            Mesh.Collection<Vector3> positions = mesh.CreatePositions(3);
            positions[0] = new(0f, 0f, 0f);
            positions[1] = new(1f, 0f, 0f);
            positions[2] = new(1f, 1f, 0f);

            Assert.That(mesh.Version, Is.EqualTo(3));
            Assert.That(positions.Length, Is.EqualTo(3));
            Assert.That(mesh.IndexCount, Is.EqualTo(0));
            mesh.AddTriangle(0, 1, 2);

            Assert.That(mesh.VertexCount, Is.EqualTo(3));
            Assert.That(mesh.IndexCount, Is.EqualTo(3));
        }

        [Test]
        public void AssembleForRendering()
        {
            using World world = CreateWorld();
            Mesh quadMesh = new(world);
            Mesh.Collection<Vector3> positions = quadMesh.CreatePositions(4);
            positions[0] = new(0f, 0f, 0f);
            positions[1] = new(1f, 0f, 0f);
            positions[2] = new(1f, 1f, 0f);
            positions[3] = new(0f, 1f, 0f);

            Mesh.Collection<Vector4> colors = quadMesh.CreateColors(4);
            colors[0] = new(1f, 0f, 0f, 1f);
            colors[1] = new(0f, 1f, 0f, 1f);
            colors[2] = new(0f, 0f, 1f, 1f);
            colors[3] = new(1f, 1f, 1f, 1f);

            Mesh.Collection<Vector2> uvs = quadMesh.CreateUVs(4);
            uvs[0] = new(0f, 0f);
            uvs[1] = new(1f, 0f);
            uvs[2] = new(1f, 1f);
            uvs[3] = new(0f, 1f);

            Mesh.Collection<Vector3> normals = quadMesh.CreateNormals(4);
            normals[0] = new(0f, 0f, 1f);
            normals[1] = new(0f, 0f, 1f);
            normals[2] = new(0f, 0f, 1f);
            normals[3] = new(0f, 0f, 1f);

            quadMesh.AddTriangle(0, 1, 2);
            quadMesh.AddTriangle(2, 3, 0);

            Span<MeshChannel> channels = [MeshChannel.Position, MeshChannel.Normal, MeshChannel.UV];
            int vertexSize = channels.GetVertexSize();
            int vertexCount = quadMesh.VertexCount;
            Assert.That(vertexSize, Is.EqualTo(3 + 3 + 2));
            Assert.That(quadMesh.VertexSize, Is.EqualTo(3 + 4 + 2 + 3));
            Assert.That(vertexCount, Is.EqualTo(4));

            using Array<float> vertexData = new(vertexSize * vertexCount);
            int added = quadMesh.Assemble(vertexData.AsSpan(), channels);
            Assert.That(added, Is.EqualTo(4 * vertexSize));
            for (int v = 0; v < 4; v++)
            {
                float x = vertexData[v * vertexSize];
                float y = vertexData[v * vertexSize + 1];
                float z = vertexData[v * vertexSize + 2];
                Assert.That(x, Is.EqualTo(positions[v].X));
                Assert.That(y, Is.EqualTo(positions[v].Y));
                Assert.That(z, Is.EqualTo(positions[v].Z));

                float nx = vertexData[v * vertexSize + 3];
                float ny = vertexData[v * vertexSize + 4];
                float nz = vertexData[v * vertexSize + 5];

                Assert.That(nx, Is.EqualTo(normals[v].X));
                Assert.That(ny, Is.EqualTo(normals[v].Y));

                float cu = vertexData[v * vertexSize + 6];
                float cv = vertexData[v * vertexSize + 7];
                Assert.That(cu, Is.EqualTo(uvs[v].X));
                Assert.That(cv, Is.EqualTo(uvs[v].Y));
            }
        }
    }
}
