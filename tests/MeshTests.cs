using Data;
using Simulation;
using System;
using System.Numerics;
using Unmanaged;
using Unmanaged.Collections;

namespace Meshes.Tests
{
    public class MeshTests
    {
        [TearDown]
        public void ClearUp()
        {
            Allocations.ThrowIfAny();
        }

        [Test]
        public void CreateQuadMesh()
        {
            using World world = new();
            Mesh mesh = new(world);
            Mesh.Collection<Vector3> positions = mesh.CreatePositions();
            Mesh.Collection<Color> colors = mesh.CreateColors();
            Mesh.Collection<Vector2> uvs = mesh.CreateUVs();
            positions.Add(new(0f, 0f, 0f));
            positions.Add(new(1f, 0f, 0f));
            positions.Add(new(1f, 1f, 0f));
            positions.Add(new(0f, 1f, 0f));
            colors.Add(new(1f, 0f, 0f, 1f));
            colors.Add(new(0f, 1f, 0f, 1f));
            colors.Add(new(0f, 0f, 1f, 1f));
            colors.Add(new(1f, 1f, 1f, 1f));
            uvs.Add(new(0f, 0f));
            uvs.Add(new(1f, 0f));
            uvs.Add(new(1f, 1f));
            uvs.Add(new(0f, 1f));
            Assert.That(mesh.HasPositions, Is.True);
            Assert.That(mesh.HasNormals, Is.False);
            Assert.That(mesh.HasUVs, Is.True);
            Assert.That(mesh.HasColors, Is.True);
            Assert.That(mesh.HasTangents, Is.False);
            Assert.That(mesh.VertexCount, Is.EqualTo(4));

            (Vector3 min, Vector3 max) bounds = mesh.GetBounds();
            Assert.That(bounds.min, Is.EqualTo(new Vector3(0, 0, 0)));
            Assert.That(bounds.max, Is.EqualTo(new Vector3(1, 1, 0)));
        }

        [Test]
        public void CheckMeshCollection()
        {
            using World world = new();
            Mesh mesh = new(world);
            Mesh.Collection<Vector3> positions = mesh.CreatePositions();
            positions.Add(new(0f, 0f, 0f));
            positions.Add(new(1f, 0f, 0f));
            positions.Add(new(1f, 1f, 0f));

            Assert.That(mesh.GetVersion(), Is.EqualTo(3));
            Assert.That(positions.Count, Is.EqualTo(3));
            mesh.AddTriangle(0, 1, 2);

            Assert.That(mesh.VertexCount, Is.EqualTo(3));
        }

        [Test]
        public void AssembleForRendering()
        {
            using World world = new();
            Mesh quadMesh = new(world);
            Mesh.Collection<Vector3> positions = quadMesh.CreatePositions();
            positions.Add(new(0f, 0f, 0f));
            positions.Add(new(1f, 0f, 0f));
            positions.Add(new(1f, 1f, 0f));
            positions.Add(new(0f, 1f, 0f));

            Mesh.Collection<Color> colors = quadMesh.CreateColors();
            colors.Add(new(1f, 0f, 0f, 1f));
            colors.Add(new(0f, 1f, 0f, 1f));
            colors.Add(new(0f, 0f, 1f, 1f));
            colors.Add(new(1f, 1f, 1f, 1f));

            Mesh.Collection<Vector2> uvs = quadMesh.CreateUVs();
            uvs.Add(new(0f, 0f));
            uvs.Add(new(1f, 0f));
            uvs.Add(new(1f, 1f));
            uvs.Add(new(0f, 1f));

            Mesh.Collection<Vector3> normals = quadMesh.CreateNormals();
            normals.Add(new(0f, 0f, 1f));
            normals.Add(new(0f, 0f, 1f));
            normals.Add(new(0f, 0f, 1f));
            normals.Add(new(0f, 0f, 1f));

            quadMesh.AddTriangle(0, 1, 2);
            quadMesh.AddTriangle(2, 3, 0);

            Span<Mesh.Channel> channels = [Mesh.Channel.Position, Mesh.Channel.Normal, Mesh.Channel.UV];
            using UnmanagedList<float> vertexData = new();
            uint vertexSize = quadMesh.Assemble(vertexData, channels);
            Assert.That(vertexSize, Is.EqualTo(3 + 3 + 2));
            Assert.That(vertexData.Count, Is.EqualTo(4 * vertexSize));
            for (uint v = 0; v < 4; v++)
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
