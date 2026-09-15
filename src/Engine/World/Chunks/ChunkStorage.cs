namespace Minicraft.Engine.World.Chunks;

public sealed class ChunkStorage
{
    private readonly Dictionary<ChunkPosition, Chunk> _chunks = new();
    
    public IEnumerable<Chunk> LoadedChunks => _chunks.Values;
    
    public int Count => _chunks.Count;
    
    public bool Contains(ChunkPosition position) => _chunks.ContainsKey(position);
    
    public bool TryGet(ChunkPosition position, out Chunk? chunk) => _chunks.TryGetValue(position, out chunk);

    public void Add(Chunk chunk)
    {
        _chunks.Add(chunk.Position, chunk);
    }

    public bool Remove(ChunkPosition position, out Chunk? chunk)
    {
        return _chunks.Remove(position, out chunk);
    }
}