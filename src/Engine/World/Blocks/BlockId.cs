namespace Minicraft.Engine.World.Blocks;

public readonly record struct BlockId(ushort Value)
{
    public static readonly BlockId Air = new(0);
}