namespace Minicraft.Engine.World;

public sealed class BlockRegistry
{
    private readonly Dictionary<BlockId, BlockDefinition> _blocks = new();

    public void Register(BlockDefinition block)
    {
        if (!_blocks.TryAdd(block.Id, block))
            throw new InvalidOperationException($"Block with id {block.Id} is already registered.");
    }
    
    public BlockDefinition Get(BlockId id) => _blocks[id];
}