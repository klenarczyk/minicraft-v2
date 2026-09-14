namespace Minicraft.Engine.World;

public sealed class BlockDefinition(BlockId id, string name, bool isSolid = true)
{
    public BlockId Id { get; } = id;
    public string Name { get; } = name;

    public bool IsSolid { get; } = isSolid;
}