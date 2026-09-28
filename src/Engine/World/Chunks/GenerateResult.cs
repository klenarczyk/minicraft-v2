namespace Minicraft.Engine.World.Chunks;

public sealed record GenerateResult(
    ChunkPosition Position, 
    ChunkSnapshot Snapshot) : IChunkWorkResult;