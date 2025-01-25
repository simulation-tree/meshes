using Types;
using Unmanaged.Tests;
using Worlds;

namespace Meshes.Tests
{
    public class MeshTests : UnmanagedTests
    {
        protected World world;

        static MeshTests()
        {
            TypeRegistry.Load<Meshes.TypeBank>();
        }

        protected override void SetUp()
        {
            base.SetUp();
            world = new(CreateSchema());
        }

        protected override void TearDown()
        {
            world.Dispose();
            base.TearDown();
        }

        protected virtual Schema CreateSchema()
        {
            Schema schema = new();
            schema.Load<Meshes.SchemaBank>();
            return schema;
        }
    }
}