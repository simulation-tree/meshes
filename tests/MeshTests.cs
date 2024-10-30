using Collections;
using Data;
using Simulation.Tests;
using System.Numerics;
using Unmanaged;

namespace Meshes.Tests
{
    public class MeshTests : SimulationTests
    {
        [Test]
        public void CreateQuadMesh()
        {
            Mesh mesh = new(World);
            USpan<Vector3> positions = mesh.CreatePositions(4);
            USpan<Color> colors = mesh.CreateColors(4);
            USpan<Vector2> uvs = mesh.CreateUVs(4);
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
            Assert.That(mesh.VertexCount, Is.EqualTo(4));
            Assert.That(mesh.HasPositions, Is.True);
            Assert.That(mesh.HasNormals, Is.False);
            Assert.That(mesh.HasUVs, Is.True);
            Assert.That(mesh.HasColors, Is.True);
            Assert.That(mesh.HasTangents, Is.False);

            (Vector3 min, Vector3 max) bounds = mesh.Bounds;
            Assert.That(bounds.min, Is.EqualTo(new Vector3(0, 0, 0)));
            Assert.That(bounds.max, Is.EqualTo(new Vector3(1, 1, 0)));
        }

        [Test]
        public void CheckMeshCollection()
        {
            Mesh mesh = new(World);
            USpan<Vector3> positions = mesh.CreatePositions(3);
            positions[0] = new(0f, 0f, 0f);
            positions[1] = new(1f, 0f, 0f);
            positions[2] = new(1f, 1f, 0f);

            Assert.That(mesh.Version, Is.EqualTo(1));
            Assert.That(positions.Length, Is.EqualTo(3));
            mesh.AddTriangle(0, 1, 2);

            Assert.That(mesh.VertexCount, Is.EqualTo(3));
        }

        [Test]
        public void AssembleForRendering()
        {
            Mesh quadMesh = new(World);
            USpan<Vector3> positions = quadMesh.CreatePositions(4);
            positions[0] = new(0f, 0f, 0f);
            positions[1] = new(1f, 0f, 0f);
            positions[2] = new(1f, 1f, 0f);
            positions[3] = new(0f, 1f, 0f);

            USpan<Color> colors = quadMesh.CreateColors(4);
            colors[0] = new(1f, 0f, 0f, 1f);
            colors[1] = new(0f, 1f, 0f, 1f);
            colors[2] = new(0f, 0f, 1f, 1f);
            colors[3] = new(1f, 1f, 1f, 1f);

            USpan<Vector2> uvs = quadMesh.CreateUVs(4);
            uvs[0] = new(0f, 0f);
            uvs[1] = new(1f, 0f);
            uvs[2] = new(1f, 1f);
            uvs[3] = new(0f, 1f);

            USpan<Vector3> normals = quadMesh.CreateNormals(4);
            normals[0] = new(0f, 0f, 1f);
            normals[1] = new(0f, 0f, 1f);
            normals[2] = new(0f, 0f, 1f);
            normals[3] = new(0f, 0f, 1f);

            quadMesh.AddTriangle(0, 1, 2);
            quadMesh.AddTriangle(2, 3, 0);

            USpan<Mesh.Channel> channels = [Mesh.Channel.Position, Mesh.Channel.Normal, Mesh.Channel.UV];
            using List<float> vertexData = new();
            uint vertexSize = quadMesh.Assemble(vertexData, channels);
            Assert.That(vertexSize, Is.EqualTo(3 + 3 + 2));
            Assert.That(vertexData, Has.Count.EqualTo(4 * vertexSize));
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
