using Minicraft.Engine.Geometry;
using Minicraft.Engine.World.Blocks;
using Minicraft.Engine.World.Chunks;

namespace Minicraft.Engine.World.Meshing;

public sealed class ChunkMesher(BlockRegistry blocks)
{
    public MeshData Build(ChunkMeshInput input)
    {
        var vertices = new List<MeshVertex>();
        var indices = new List<uint>();

        for (var y = 0; y < Chunk.SizeY; y++)
        for (var z = 0; z < Chunk.SizeZ; z++)
        for (var x = 0; x < Chunk.SizeX; x++)
        {
            var blockId = input.GetBlock(x, y, z);
            if (!blocks.Get(blockId).IsSolid) continue;
            
            if (IsAir(input, x - 1, y, z))
                AddFace(vertices, indices, input, x, y, z, Direction.West);

            if (IsAir(input, x + 1, y, z))
                AddFace(vertices, indices, input, x, y, z, Direction.East);

            if (IsAir(input, x, y - 1, z))
                AddFace(vertices, indices, input, x, y, z, Direction.Down);

            if (IsAir(input, x, y + 1, z))
                AddFace(vertices, indices, input, x, y, z, Direction.Up);

            if (IsAir(input, x, y, z - 1))
                AddFace(vertices, indices, input, x, y, z, Direction.North);

            if (IsAir(input, x, y, z + 1))
                AddFace(vertices, indices, input, x, y, z, Direction.South);
        }

        return new MeshData(vertices.ToArray(), indices.ToArray());
    }

    private void AddFace(
        List<MeshVertex> vertices,
        List<uint> indices,
        ChunkMeshInput input,
        int x, int y, int z,
        Direction direction)
    {
        var start = (uint)vertices.Count;

        switch (direction)
        {
            case Direction.West:
            {
                byte ao0 = CalculateAo(input,
                    x - 1, y - 1, z,
                    x - 1, y, z - 1,
                    x - 1, y - 1, z - 1);

                byte ao1 = CalculateAo(input,
                    x - 1, y - 1, z,
                    x - 1, y, z + 1,
                    x - 1, y - 1, z + 1);

                byte ao2 = CalculateAo(input,
                    x - 1, y + 1, z,
                    x - 1, y, z + 1,
                    x - 1, y + 1, z + 1);

                byte ao3 = CalculateAo(input,
                    x - 1, y + 1, z,
                    x - 1, y, z - 1,
                    x - 1, y + 1, z - 1);
                
                AddVertex(vertices, x, y, z, -1, 0, 0, 0, 0, ao0);
                AddVertex(vertices, x, y, z + 1, -1, 0, 0, 1, 0, ao1);
                AddVertex(vertices, x, y + 1, z + 1, -1, 0, 0, 1, 1, ao2);
                AddVertex(vertices, x, y + 1, z, -1, 0, 0, 0, 1, ao3);
                
                AddQuadIndices(indices, start, ao0, ao1, ao2, ao3);
                break;
            }

            case Direction.East:
            {
                byte ao0 = CalculateAo(input,
                    x + 1, y - 1, z,
                    x + 1, y, z - 1,
                    x + 1, y - 1, z - 1);

                byte ao1 = CalculateAo(input,
                    x + 1, y + 1, z,
                    x + 1, y, z - 1,
                    x + 1, y + 1, z - 1);

                byte ao2 = CalculateAo(input,
                    x + 1, y + 1, z,
                    x + 1, y, z + 1,
                    x + 1, y + 1, z + 1);

                byte ao3 = CalculateAo(input,
                    x + 1, y - 1, z,
                    x + 1, y, z + 1,
                    x + 1, y - 1, z + 1);
                
                AddVertex(vertices, x + 1, y, z, 1, 0, 0, 0, 0, ao0);
                AddVertex(vertices, x + 1, y + 1, z, 1, 0, 0, 0, 1, ao1);
                AddVertex(vertices, x + 1, y + 1, z + 1, 1, 0, 0, 1, 1, ao2);
                AddVertex(vertices, x + 1, y, z + 1, 1, 0, 0, 1, 0, ao3);
                
                AddQuadIndices(indices, start, ao0, ao1, ao2, ao3);
                break;
            }

            case Direction.Down:
            {
                byte ao0 = CalculateAo(input,
                    x - 1, y - 1, z,
                    x, y - 1, z - 1,
                    x - 1, y - 1, z - 1);

                byte ao1 = CalculateAo(input,
                    x + 1, y - 1, z,
                    x, y - 1, z - 1,
                    x + 1, y - 1, z - 1);

                byte ao2 = CalculateAo(input,
                    x + 1, y - 1, z,
                    x, y - 1, z + 1,
                    x + 1, y - 1, z + 1);

                byte ao3 = CalculateAo(input,
                    x - 1, y - 1, z,
                    x, y - 1, z + 1,
                    x - 1, y - 1, z + 1);
                
                AddVertex(vertices, x, y, z, 0, -1, 0, 0, 0, ao0);
                AddVertex(vertices, x + 1, y, z, 0, -1, 0, 1, 0, ao1);
                AddVertex(vertices, x + 1, y, z + 1, 0, -1, 0, 1, 1, ao2);
                AddVertex(vertices, x, y, z + 1, 0, -1, 0, 0, 1, ao3);
                
                AddQuadIndices(indices, start, ao0, ao1, ao2, ao3);
                break;
            }

            case Direction.Up:
            {
                byte ao0 = CalculateAo(input,
                    x - 1, y + 1, z,
                    x, y + 1, z - 1,
                    x - 1, y + 1, z - 1);
                
                byte ao1 = CalculateAo(input,
                    x - 1, y + 1, z,
                    x, y + 1, z + 1,
                    x - 1, y + 1, z + 1);

                byte ao2 = CalculateAo(input,
                    x + 1, y + 1, z,
                    x, y + 1, z + 1,
                    x + 1, y + 1, z + 1);
                
                byte ao3 = CalculateAo(input,
                    x + 1, y + 1, z,
                    x, y + 1, z - 1,
                    x + 1, y + 1, z - 1);
                
                AddVertex(vertices, x, y + 1, z, 0, 1, 0, 0, 0, ao0);
                AddVertex(vertices, x, y + 1, z + 1, 0, 1, 0, 0, 1, ao1);
                AddVertex(vertices, x + 1, y + 1, z + 1, 0, 1, 0, 1, 1, ao2);
                AddVertex(vertices, x + 1, y + 1, z, 0, 1, 0, 1, 0, ao3);
                
                AddQuadIndices(indices, start, ao0, ao1, ao2, ao3);
                break;
            }

            case Direction.North:
            {
                byte ao0 = CalculateAo(input,
                    x - 1, y, z - 1,
                    x, y - 1, z - 1,
                    x - 1, y - 1, z - 1);

                byte ao1 = CalculateAo(input,
                    x - 1, y, z - 1,
                    x, y + 1, z - 1,
                    x - 1, y + 1, z - 1);

                byte ao2 = CalculateAo(input,
                    x + 1, y, z - 1,
                    x, y + 1, z - 1,
                    x + 1, y + 1, z - 1);

                byte ao3 = CalculateAo(input,
                    x + 1, y, z - 1,
                    x, y - 1, z - 1,
                    x + 1, y - 1, z - 1);
                
                AddVertex(vertices, x, y, z, 0, 0, -1, 0, 0, ao0);
                AddVertex(vertices, x, y + 1, z, 0, 0, -1, 0, 1, ao1);
                AddVertex(vertices, x + 1, y + 1, z, 0, 0, -1, 1, 1, ao2);
                AddVertex(vertices, x + 1, y, z, 0, 0, -1, 1, 0, ao3);
                
                AddQuadIndices(indices, start, ao0, ao1, ao2, ao3);
                break;
            }

            case Direction.South:
            {
                byte ao0 = CalculateAo(input,
                    x + 1, y, z + 1,
                    x, y - 1, z + 1,
                    x + 1, y - 1, z + 1);

                byte ao1 = CalculateAo(input,
                    x + 1, y, z + 1,
                    x, y + 1, z + 1,
                    x + 1, y + 1, z + 1);

                byte ao2 = CalculateAo(input,
                    x - 1, y, z + 1,
                    x, y + 1, z + 1,
                    x - 1, y + 1, z + 1);

                byte ao3 = CalculateAo(input,
                    x - 1, y, z + 1,
                    x, y - 1, z + 1,
                    x - 1, y - 1, z + 1);
                
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

    private byte CalculateAo(ChunkMeshInput input,
        int side1X, int side1Y, int side1Z,
        int side2X, int side2Y, int side2Z,
        int cornerX, int cornerY, int cornerZ)
    {
        bool side1 = IsSolid(input, side1X, side1Y, side1Z);
        bool side2 = IsSolid(input, side2X, side2Y, side2Z);
        bool corner = IsSolid(input, cornerX, cornerY, cornerZ);

        if (side1 && side2) return 0;

        var occlusion = 3;
        if (side1) occlusion--;
        if (side2) occlusion--;
        if (corner) occlusion--;

        return (byte)occlusion;
    }
    
    private bool IsSolid(ChunkMeshInput input, int x, int y, int z)
        => blocks.Get(input.GetBlock(x, y, z)).IsSolid;

    private bool IsAir(ChunkMeshInput input, int x, int y, int z)
        => !IsSolid(input, x, y, z);
}