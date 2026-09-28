using Minicraft.Engine.World.Blocks;

namespace Minicraft.Engine.World.Chunks;

public sealed class ChunkMeshInput(
    ChunkSnapshot center, 
    ChunkSnapshot? north, 
    ChunkSnapshot? northEast,
    ChunkSnapshot? east, 
    ChunkSnapshot? southEast,
    ChunkSnapshot? south, 
    ChunkSnapshot? southWest,
    ChunkSnapshot? west,
    ChunkSnapshot? northWest)
{
    public ChunkSnapshot Center { get; } = center;

    public ChunkSnapshot? North { get; } = north;
    public ChunkSnapshot? NorthEast { get; } = northEast;
    public ChunkSnapshot? East { get; } = east;
    public ChunkSnapshot? SouthEast { get; } = southEast;
    public ChunkSnapshot? South { get; } = south;
    public ChunkSnapshot? SouthWest { get; } = southWest;
    public ChunkSnapshot? West { get; } = west;
    public ChunkSnapshot? NorthWest { get; } = northWest;

    public BlockId GetBlock(int localX, int y, int localZ)
    {
        if (y is < 0 or >= Chunk.SizeY)
            return BlockId.Air;

        // NW
        if (localX < 0 && localZ < 0)
            return NorthWest?.GetBlock(Chunk.SizeX - 1, y, Chunk.SizeZ - 1) ?? BlockId.Air;

        // NE
        if (localX >= Chunk.SizeX && localZ < 0)
            return NorthEast?.GetBlock(0, y, Chunk.SizeZ - 1) ?? BlockId.Air;

        // SW
        if (localX < 0 && localZ >= Chunk.SizeZ)
            return SouthWest?.GetBlock(Chunk.SizeX - 1, y, 0) ?? BlockId.Air;

        // SE
        if (localX >= Chunk.SizeX && localZ >= Chunk.SizeZ)
            return SouthEast?.GetBlock(0, y, 0) ?? BlockId.Air;

        // W
        if (localX < 0)
            return West?.GetBlock(Chunk.SizeX - 1, y, localZ) ?? BlockId.Air;

        // E
        if (localX >= Chunk.SizeX)
            return East?.GetBlock(0, y, localZ) ?? BlockId.Air;

        // N
        if (localZ < 0)
            return North?.GetBlock(localX, y, Chunk.SizeZ - 1) ?? BlockId.Air;

        // S
        if (localZ >= Chunk.SizeZ)
            return South?.GetBlock(localX, y, 0) ?? BlockId.Air;

        return Center.GetBlock(localX, y, localZ);
    }
}