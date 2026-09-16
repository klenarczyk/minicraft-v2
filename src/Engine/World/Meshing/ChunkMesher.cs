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
                AddFace(vertices, indices, world, x, y, z, worldX, worldZ, Direction.West);

            if (IsAir(world, worldX + 1, y, worldZ))
                AddFace(vertices, indices, world, x, y, z, worldX, worldZ, Direction.East);

            if (IsAir(world, worldX, y - 1, worldZ))
                AddFace(vertices, indices, world, x, y, z, worldX, worldZ, Direction.Down);

            if (IsAir(world, worldX, y + 1, worldZ))
                AddFace(vertices, indices, world, x, y, z, worldX, worldZ, Direction.Up);

            if (IsAir(world, worldX, y, worldZ - 1))
                AddFace(vertices, indices, world, x, y, z, worldX, worldZ, Direction.North);

            if (IsAir(world, worldX, y, worldZ + 1))
                AddFace(vertices, indices, world, x, y, z, worldX, worldZ, Direction.South);
        }

        return new MeshData(vertices.ToArray(), indices.ToArray());
    }

    private void AddFace(
        List<MeshVertex> vertices,
        List<uint> indices,
        World world,
        int x, int y, int z,
        int wx, int wz,
        Direction direction)
    {
        var start = (uint)vertices.Count;

        switch (direction)
        {
            case Direction.West:
            {
                byte ao0 = CalculateAo(world,
                    wx - 1, y - 1, wz,
                    wx - 1, y, wz - 1,
                    wx - 1, y - 1, wz - 1);

                byte ao1 = CalculateAo(world,
                    wx - 1, y - 1, wz,
                    wx - 1, y, wz + 1,
                    wx - 1, y - 1, wz + 1);

                byte ao2 = CalculateAo(world,
                    wx - 1, y + 1, wz,
                    wx - 1, y, wz + 1,
                    wx - 1, y + 1, wz + 1);

                byte ao3 = CalculateAo(world,
                    wx - 1, y + 1, wz,
                    wx - 1, y, wz - 1,
                    wx - 1, y + 1, wz - 1);
                
                AddVertex(vertices, x, y, z, -1, 0, 0, 0, 0, ao0);
                AddVertex(vertices, x, y, z + 1, -1, 0, 0, 1, 0, ao1);
                AddVertex(vertices, x, y + 1, z + 1, -1, 0, 0, 1, 1, ao2);
                AddVertex(vertices, x, y + 1, z, -1, 0, 0, 0, 1, ao3);
                
                AddQuadIndices(indices, start, ao0, ao1, ao2, ao3);
                break;
            }

            case Direction.East:
            {
                byte ao0 = CalculateAo(world,
                    wx + 1, y - 1, wz,
                    wx + 1, y, wz - 1,
                    wx + 1, y - 1, wz - 1);

                byte ao1 = CalculateAo(world,
                    wx + 1, y + 1, wz,
                    wx + 1, y, wz - 1,
                    wx + 1, y + 1, wz - 1);

                byte ao2 = CalculateAo(world,
                    wx + 1, y + 1, wz,
                    wx + 1, y, wz + 1,
                    wx + 1, y + 1, wz + 1);

                byte ao3 = CalculateAo(world,
                    wx + 1, y - 1, wz,
                    wx + 1, y, wz + 1,
                    wx + 1, y - 1, wz + 1);
                
                AddVertex(vertices, x + 1, y, z, 1, 0, 0, 0, 0, ao0);
                AddVertex(vertices, x + 1, y + 1, z, 1, 0, 0, 0, 1, ao1);
                AddVertex(vertices, x + 1, y + 1, z + 1, 1, 0, 0, 1, 1, ao2);
                AddVertex(vertices, x + 1, y, z + 1, 1, 0, 0, 1, 0, ao3);
                
                AddQuadIndices(indices, start, ao0, ao1, ao2, ao3);
                break;
            }

            case Direction.Down:
            {
                byte ao0 = CalculateAo(world,
                    wx - 1, y - 1, wz,
                    wx, y - 1, wz - 1,
                    wx - 1, y - 1, wz - 1);

                byte ao1 = CalculateAo(world,
                    wx + 1, y - 1, wz,
                    wx, y - 1, wz - 1,
                    wx + 1, y - 1, wz - 1);

                byte ao2 = CalculateAo(world,
                    wx + 1, y - 1, wz,
                    wx, y - 1, wz + 1,
                    wx + 1, y - 1, wz + 1);

                byte ao3 = CalculateAo(world,
                    wx - 1, y - 1, wz,
                    wx, y - 1, wz + 1,
                    wx - 1, y - 1, wz + 1);
                
                AddVertex(vertices, x, y, z, 0, -1, 0, 0, 0, ao0);
                AddVertex(vertices, x + 1, y, z, 0, -1, 0, 1, 0, ao1);
                AddVertex(vertices, x + 1, y, z + 1, 0, -1, 0, 1, 1, ao2);
                AddVertex(vertices, x, y, z + 1, 0, -1, 0, 0, 1, ao3);
                
                AddQuadIndices(indices, start, ao0, ao1, ao2, ao3);
                break;
            }

            case Direction.Up:
            {
                byte ao0 = CalculateAo(world,
                    wx - 1, y + 1, wz,
                    wx,     y + 1, wz - 1,
                    wx - 1, y + 1, wz - 1);
                
                byte ao1 = CalculateAo(world,
                    wx - 1, y + 1, wz,
                    wx,     y + 1, wz + 1,
                    wx - 1, y + 1, wz + 1);

                byte ao2 = CalculateAo(world,
                    wx + 1, y + 1, wz,
                    wx,     y + 1, wz + 1,
                    wx + 1, y + 1, wz + 1);
                
                byte ao3 = CalculateAo(world,
                    wx + 1, y + 1, wz,
                    wx,     y + 1, wz - 1,
                    wx + 1, y + 1, wz - 1);
                
                AddVertex(vertices, x, y + 1, z, 0, 1, 0, 0, 0, ao0);
                AddVertex(vertices, x, y + 1, z + 1, 0, 1, 0, 0, 1, ao1);
                AddVertex(vertices, x + 1, y + 1, z + 1, 0, 1, 0, 1, 1, ao2);
                AddVertex(vertices, x + 1, y + 1, z, 0, 1, 0, 1, 0, ao3);
                
                AddQuadIndices(indices, start, ao0, ao1, ao2, ao3);
                break;
            }

            case Direction.North:
            {
                byte ao0 = CalculateAo(world,
                    wx - 1, y, wz - 1,
                    wx, y - 1, wz - 1,
                    wx - 1, y - 1, wz - 1);

                byte ao1 = CalculateAo(world,
                    wx - 1, y, wz - 1,
                    wx, y + 1, wz - 1,
                    wx - 1, y + 1, wz - 1);

                byte ao2 = CalculateAo(world,
                    wx + 1, y, wz - 1,
                    wx, y + 1, wz - 1,
                    wx + 1, y + 1, wz - 1);

                byte ao3 = CalculateAo(world,
                    wx + 1, y, wz - 1,
                    wx, y - 1, wz - 1,
                    wx + 1, y - 1, wz - 1);
                
                AddVertex(vertices, x, y, z, 0, 0, -1, 0, 0, ao0);
                AddVertex(vertices, x, y + 1, z, 0, 0, -1, 0, 1, ao1);
                AddVertex(vertices, x + 1, y + 1, z, 0, 0, -1, 1, 1, ao2);
                AddVertex(vertices, x + 1, y, z, 0, 0, -1, 1, 0, ao3);
                
                AddQuadIndices(indices, start, ao0, ao1, ao2, ao3);
                break;
            }

            case Direction.South:
            {
                byte ao0 = CalculateAo(world,
                    wx + 1, y, wz + 1,
                    wx, y - 1, wz + 1,
                    wx + 1, y - 1, wz + 1);

                byte ao1 = CalculateAo(world,
                    wx + 1, y, wz + 1,
                    wx, y + 1, wz + 1,
                    wx + 1, y + 1, wz + 1);

                byte ao2 = CalculateAo(world,
                    wx - 1, y, wz + 1,
                    wx, y + 1, wz + 1,
                    wx - 1, y + 1, wz + 1);

                byte ao3 = CalculateAo(world,
                    wx - 1, y, wz + 1,
                    wx, y - 1, wz + 1,
                    wx - 1, y - 1, wz + 1);
                
                AddVertex(vertices, x + 1, y, z + 1, 0, 0, 1, 0, 0, ao0);
                AddVertex(vertices, x + 1, y + 1, z + 1, 0, 0, 1, 0, 1, ao1);
                AddVertex(vertices, x, y + 1, z + 1, 0, 0, 1, 1, 1, ao2);
                AddVertex(vertices, x, y, z + 1, 0, 0, 1, 1, 0, ao3);
                
                AddQuadIndices(indices, start, ao0, ao1, ao2, ao3);
                break;
            }
        }
    }

    private static void AddVertex(
        List<MeshVertex> vertices,
        float x, float y, float z,
        float nx, float ny, float nz,
        float u, float v,
        byte ao)
    {
        vertices.Add(new MeshVertex(x, y, z, nx, ny, nz, u, v, ao));
    }

    private static void AddQuadIndices(
        List<uint> indices, uint start,
        byte ao0, byte ao1, byte ao2, byte ao3)
    {
        indices.Add(start);
        indices.Add(start + 1);
        
        if (ao0 + ao2 > ao1 + ao3)
        {
            indices.Add(start + 2);
            indices.Add(start);
        }
        else
        {
            indices.Add(start + 3);
            indices.Add(start + 1);
        }

        indices.Add(start + 2);
        indices.Add(start + 3);
    }

    private byte CalculateAo(World world,
        int side1X, int side1Y, int side1Z,
        int side2X, int side2Y, int side2Z,
        int cornerX, int cornerY, int cornerZ)
    {
        bool side1 = IsSolid(world, side1X, side1Y, side1Z);
        bool side2 = IsSolid(world, side2X, side2Y, side2Z);
        bool corner = IsSolid(world, cornerX, cornerY, cornerZ);

        if (side1 && side2) return 0;
        
        return (byte)(3
                - (side1 ? 1 : 0)
                - (side2 ? 1 : 0)
                - (corner ? 1 : 0));
    }
    
    private bool IsSolid(World world, int x, int y, int z)
    {
        return blocks.Get(world.GetBlock(x, y, z)).IsSolid;
    }

    private bool IsAir(World world, int x, int y, int z)
        => !IsSolid(world, x, y, z);
}