namespace Minicraft.Engine.World.Blocks;

public sealed class BlockRegistry
{
    private readonly Dictionary<BlockId, BlockDefinition> _blocks = new()
    {
        { BlockId.Air, new BlockDefinition(BlockId.Air, "air", false) }
    };

    public int Count => _blocks.Count;
    
    public void Register(BlockDefinition block)
    {
        if (!_blocks.TryAdd(block.Id, block))
            throw new InvalidOperationException($"Block with id {block.Id} is already registered.");
    }
    
    public BlockDefinition Get(BlockId id) => _blocks[id];
}