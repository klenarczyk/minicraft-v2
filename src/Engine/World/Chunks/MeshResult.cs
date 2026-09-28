using Minicraft.Engine.Geometry;

namespace Minicraft.Engine.World.Chunks;

public sealed record MeshResult(
    ChunkPosition Position,
    MeshData Mesh) : IChunkWorkResult;