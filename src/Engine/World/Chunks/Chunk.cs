using Minicraft.Engine.Geometry;
using Minicraft.Engine.World.Blocks;

namespace Minicraft.Engine.World.Chunks;

public sealed class Chunk
{
    public const int SizeX = 16;
    public const int SizeY = 256;
    public const int SizeZ = 16;
    
    private readonly BlockId[] _blocks = new BlockId[SizeX * SizeY * SizeZ];
    
    public ChunkPosition Position { get; }
    
    public MeshData? Mesh { get; private set; }
    public int MeshVersion { get; private set; }
    public bool IsMeshDirty { get; private set; }

    public Chunk(ChunkPosition position)
    {
        Position = position;
    }
    
    public Chunk(ChunkSnapshot snapshot)
    {
        Position = snapshot.Position;
        _blocks = snapshot.Blocks.ToArray();
    }
    
    public BlockId GetBlock(int x, int y, int z)
    {
        return _blocks[GetBlockIndex(x, y, z)];
    }

    public void SetBlock(int x, int y, int z, BlockId block)
    {
        _blocks[GetBlockIndex(x, y, z)] = block;
    }

    public void SetMesh(MeshData mesh)
    {
        Mesh = mesh;
        MeshVersion++;
        IsMeshDirty = false;
    }

    public void MarkMeshDirty()
    {
        IsMeshDirty = true;
    }

    public ChunkSnapshot CreateSnapshot()
    {
        return new ChunkSnapshot(Position, _blocks);
    }

    public static int GetBlockIndex(int x, int y, int z)
    {
        if (x < 0 || x >= SizeX ||
            y < 0 || y >= SizeY ||
            z < 0 || z >= SizeZ)
        {
            throw new ArgumentOutOfRangeException($"Block {x},{y},{z} is out of range.");
        }
        
        return x + SizeX * (z + SizeZ * y);
    }
}