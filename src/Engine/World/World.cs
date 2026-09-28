using System.Numerics;
using Minicraft.Engine.World.Blocks;
using Minicraft.Engine.World.Chunks;
using Minicraft.Engine.World.Generation;
using Minicraft.Engine.World.Meshing;

namespace Minicraft.Engine.World;

public sealed class World : IDisposable
{
    public BlockRegistry Blocks { get; }
    
    public ChunkStorage Chunks { get; }
    public ChunkStreamer Streamer { get; }
    public ChunkScheduler Scheduler { get; }

    private readonly HashSet<ChunkPosition> _generatingChunks = [];
    private readonly HashSet<ChunkPosition> _meshingChunks = [];
    
    public World()
    {
        Blocks = new BlockRegistry();
        Chunks = new ChunkStorage();
        Streamer = new ChunkStreamer();
        
        var mesher = new ChunkMesher(Blocks);

        Scheduler = new ChunkScheduler(
            GenerateChunk,
            mesher.Build,
            Environment.ProcessorCount);
    }
    
    public void Initialize()
    {
        Blocks.Register(new BlockDefinition(new BlockId((ushort)Blocks.Count), "dirt"));
    }

    public void Update(double deltaTime, Vector3 playerPosition)
    {
        Streamer.Update(playerPosition);

        ReconcileChunks();
        ProcessCompletedWork();
        ScheduleMeshes();
    }

    public BlockId GetBlock(int worldX, int worldY, int worldZ)
    {
        if (worldY is < 0 or >= Chunk.SizeY)
            return BlockId.Air;

        var chunkX = (int)Math.Floor((double)worldX / Chunk.SizeX);
        var chunkZ = (int)Math.Floor((double)worldZ / Chunk.SizeZ);
        
        int localX = worldX - chunkX * Chunk.SizeX;
        int localZ = worldZ - chunkZ * Chunk.SizeZ;

        var position = new ChunkPosition(chunkX, chunkZ);

        if (!Chunks.TryGet(position, out var chunk))
            return BlockId.Air;
        
        return chunk!.GetBlock(localX, worldY, localZ);
    }
    
    // ---

    private void ReconcileChunks()
    {
        foreach (var chunk in Chunks.LoadedChunks.ToArray())
        {
            if (Streamer.DesiredChunks.Contains(chunk.Position)) continue;
            
            var position = chunk.Position;

            if (Chunks.Remove(position, out _))
            {
                InvalidateNeighborhood(position);
            }
        }
        
        foreach (var position in Streamer.DesiredChunks)
        {
            if (Chunks.Contains(position)) continue;
            if (!_generatingChunks.Add(position)) continue;
            
            Scheduler.RequestGenerate(position);
        }
    }

    private void ProcessCompletedWork()
    {
        while (Scheduler.TryGetCompleted(out var res))
        {
            switch (res)
            {
                case GenerateResult generateRes:
                    ApplyGeneration(generateRes);
                    break;
                
                case MeshResult meshRes:
                    ApplyMesh(meshRes);
                    break;
            }
        }
    }

    private void ApplyGeneration(GenerateResult result)
    {
        _generatingChunks.Remove(result.Position);

        // Player moved away while generating
        if (!Streamer.DesiredChunks.Contains(result.Position)) return;

        // Already loaded chunk
        if (Chunks.Contains(result.Position)) return;
        
        var chunk = new Chunk(result.Snapshot);
        Chunks.Add(chunk);

        InvalidateNeighborhood(result.Position);
    }

    private void ApplyMesh(MeshResult result)
    {
        _meshingChunks.Remove(result.Position);
        
        if (!Chunks.TryGet(result.Position, out var chunk)) return;

        // Player moved away
        if (!Streamer.DesiredChunks.Contains(result.Position)) return;

        chunk!.SetMesh(result.Mesh);
    }

    private void ScheduleMeshes()
    {
        foreach (var chunk in Chunks.LoadedChunks)
        {
            var position = chunk.Position;
            
            if (_meshingChunks.Contains(position)) continue;

            if (chunk.Mesh is not null && !chunk.IsMeshDirty) continue;

            var input = CreateMeshInput(position);

            _meshingChunks.Add(position);
            Scheduler.RequestMesh(position, input);
        }
    }

    private ChunkSnapshot GenerateChunk(ChunkPosition position)
    {
        var blocks = RippleWorldGenerator.Generate(position, new BlockId(1));
        return new ChunkSnapshot(position, blocks);
    }

    private ChunkMeshInput CreateMeshInput(ChunkPosition position)
    {
        return new ChunkMeshInput(
            GetSnapshot(position),
            GetSnapshot(position with { Z = position.Z - 1 }),
            GetSnapshot(new ChunkPosition(position.X + 1, position.Z - 1)),
            GetSnapshot(position with { X = position.X + 1 }),
            GetSnapshot(new ChunkPosition(position.X + 1, position.Z + 1)),
            GetSnapshot(position with { Z = position.Z + 1 }),
            GetSnapshot(new ChunkPosition(position.X - 1, position.Z + 1)),
            GetSnapshot(position with { X = position.X - 1 }),
            GetSnapshot(new ChunkPosition(position.X - 1, position.Z - 1)));
    }

    private ChunkSnapshot? GetSnapshot(ChunkPosition position)
    {
        if (!Chunks.TryGet(position, out var chunk)) return null;
        return chunk!.CreateSnapshot();
    }

    private void InvalidateNeighborhood(ChunkPosition center)
    {
        for (var x = -1; x <= 1; x++)
        for (var z = -1; z <= 1; z++)
        {
            var position = new ChunkPosition(center.X + x, center.Z + z);
            
            if (!Chunks.TryGet(position, out var chunk)) continue;
            
            chunk!.MarkMeshDirty();
        }
    }
    
    public void Dispose()
    {
        Scheduler.Dispose();
    }
}