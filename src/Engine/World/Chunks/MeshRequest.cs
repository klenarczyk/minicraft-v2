namespace Minicraft.Engine.World.Chunks;

public record MeshRequest(
    ChunkPosition Position, 
    ChunkMeshInput Input) : IChunkWorkRequest;