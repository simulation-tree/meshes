using Types;
using Worlds;
using Worlds.Tests;

namespace Meshes.Tests
{
    public class MeshTests : WorldTests
    {
        static MeshTests()
        {
            MetadataRegistry.Load<MeshesTypeBank>();
        }

        protected override Schema CreateSchema()
        {
            Schema schema = base.CreateSchema();
            schema.Load<MeshesSchemaBank>();
            return schema;
        }
    }
}