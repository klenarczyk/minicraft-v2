namespace Minicraft.Engine.World.Chunks;

public sealed record GenerateRequest(ChunkPosition Position) : IChunkWorkRequest;