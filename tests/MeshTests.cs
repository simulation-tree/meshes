using Types;
using Worlds;
using Worlds.Tests;

namespace Meshes.Tests
{
    public class MeshTests : WorldTests
    {
        static MeshTests()
        {
            TypeRegistry.Load<Meshes.TypeBank>();
        }

        protected override Schema CreateSchema()
        {
            Schema schema = base.CreateSchema();
            schema.Load<Meshes.SchemaBank>();
            return schema;
        }
    }
}