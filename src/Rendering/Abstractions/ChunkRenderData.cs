using Minicraft.Engine.Geometry;
using Minicraft.Engine.World.Chunks;

namespace Minicraft.Rendering.Abstractions;

public readonly record struct ChunkRenderData(
    ChunkPosition Position,
    MeshData Mesh
);