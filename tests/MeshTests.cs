using Simulation;
using System.Numerics;
using Unmanaged;

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
            Mesh.Collection<Vector4> colors = mesh.CreateColors();
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
            Assert.That(mesh.HasPositions(), Is.True);
            Assert.That(mesh.HasNormals(), Is.False);
            Assert.That(mesh.HasUVs(), Is.True);
            Assert.That(mesh.HasColors(), Is.True);
            Assert.That(mesh.HasTangents(), Is.False);
            Assert.That(mesh.GetVertexCount(), Is.EqualTo(4));

            (Vector3 min, Vector3 max) bounds = mesh.GetBounds();
            Assert.That(bounds.min, Is.EqualTo(new Vector3(0, 0, 0)));
            Assert.That(bounds.max, Is.EqualTo(new Vector3(1, 1, 0)));
        }
    }
}
