using Minicraft.Engine.Geometry;
using Minicraft.Engine.World.Blocks;
using Minicraft.Engine.World.Chunks;

namespace Minicraft.Engine.World.Meshing;

public sealed class ChunkMesher(BlockRegistry blocks)
{
    public MeshData Build(World world, Chunk chunk)
    {
        var vertices = new List<MeshVertex>();
        var indices = new List<uint>();

        for (var y = 0; y < Chunk.SizeY; y++)
        for (var z = 0; z < Chunk.SizeZ; z++)
        for (var x = 0; x < Chunk.SizeX; x++)
        {
            var blockId = chunk.GetBlock(x, y, z);
            var block = blocks.Get(blockId);

            if (!block.IsSolid) continue;

            int worldX = chunk.Position.X * Chunk.SizeX + x;
            int worldZ = chunk.Position.Z * Chunk.SizeZ + z;
            
            if (IsAir(world, worldX - 1, y, worldZ))
                AddFace(vertices, indices, x, y, z, Direction.West);

            if (IsAir(world, worldX + 1, y, worldZ))
                AddFace(vertices, indices, x, y, z, Direction.East);

            if (IsAir(world, worldX, y - 1, worldZ))
                AddFace(vertices, indices, x, y, z, Direction.Down);

            if (IsAir(world, worldX, y + 1, worldZ))
                AddFace(vertices, indices, x, y, z, Direction.Up);

            if (IsAir(world, worldX, y, worldZ - 1))
                AddFace(vertices, indices, x, y, z, Direction.North);

            if (IsAir(world, worldX, y, worldZ + 1))
                AddFace(vertices, indices, x, y, z, Direction.South);
        }

        return new MeshData(vertices.ToArray(), indices.ToArray());
    }

    private bool IsAir(World world, int x, int y, int z)
    {
        return !blocks.Get(world.GetBlock(x, y, z)).IsSolid;
    }

    private static void AddFace(
        List<MeshVertex> vertices,
        List<uint> indices,
        int x, int y, int z,
        Direction direction)
    {
        var start = (uint)vertices.Count;

        switch (direction)
        {
            case Direction.West:
                AddVertex(vertices, x, y, z, 0, 0);
                AddVertex(vertices, x, y, z + 1, 1, 0);
                AddVertex(vertices, x, y + 1, z + 1, 1, 1);
                AddVertex(vertices, x, y + 1, z, 0, 1);
                break;

            case Direction.East:
                AddVertex(vertices, x + 1, y, z, 0, 0);
                AddVertex(vertices, x + 1, y + 1, z, 0, 1);
                AddVertex(vertices, x + 1, y + 1, z + 1, 1, 1);
                AddVertex(vertices, x + 1, y, z + 1, 1, 0);
                break;

            case Direction.Down:
                AddVertex(vertices, x, y, z, 0, 0);
                AddVertex(vertices, x + 1, y, z, 1, 0);
                AddVertex(vertices, x + 1, y, z + 1, 1, 1);
                AddVertex(vertices, x, y, z + 1, 0, 1);
                break;

            case Direction.Up:
                AddVertex(vertices, x, y + 1, z, 0, 0);
                AddVertex(vertices, x, y + 1, z + 1, 0, 1);
                AddVertex(vertices, x + 1, y + 1, z + 1, 1, 1);
                AddVertex(vertices, x + 1, y + 1, z, 1, 0);
                break;

            case Direction.North:
                AddVertex(vertices, x, y, z, 0, 0);
                AddVertex(vertices, x, y + 1, z, 0, 1);
                AddVertex(vertices, x + 1, y + 1, z, 1, 1);
                AddVertex(vertices, x + 1, y, z, 1, 0);
                break;

            case Direction.South:
                AddVertex(vertices, x + 1, y, z + 1, 0, 0);
                AddVertex(vertices, x + 1, y + 1, z + 1, 0, 1);
                AddVertex(vertices, x, y + 1, z + 1, 1, 1);
                AddVertex(vertices, x, y, z + 1, 1, 0);
                break;
        }

        indices.Add(start);
        indices.Add(start + 1);
        indices.Add(start + 2);

        indices.Add(start);
        indices.Add(start + 2);
        indices.Add(start + 3);
    }

    private static void AddVertex(
        List<MeshVertex> vertices,
        float x, float y, float z,
        float u, float v)
    {
        vertices.Add(new MeshVertex(x, y, z, u, v));
    }
}