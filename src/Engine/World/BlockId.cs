namespace Minicraft.Engine.World;

public readonly record struct BlockId(ushort Value)
{
    public static readonly BlockId Air = new(0);
}