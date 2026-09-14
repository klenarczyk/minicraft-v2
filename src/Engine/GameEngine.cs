using Minicraft.Engine.Gameplay;
using Minicraft.Engine.Geometry;
using Minicraft.Engine.Input;
using Minicraft.Engine.World;
using Minicraft.Engine.World.Generation;
using Minicraft.Engine.World.Meshing;

namespace Minicraft.Engine;

public sealed class GameEngine
{
    public InputSystem Input { get; } = new();
    
    public BlockRegistry Blocks { get; } = new();
    public Chunk World { get; } = new();
    
    public Player Player { get; } = new();

    private ChunkMesher? _mesher;
    private MeshData? _worldMesh;
    
    public void Initialize()
    {
        Blocks.Register(new BlockDefinition(BlockId.Air, "air", false));
        Blocks.Register(new BlockDefinition(new BlockId(1), "dirt"));

        FlatWorldGenerator.Generate(World, new BlockId(1));
        
        _mesher = new ChunkMesher(Blocks);
        _worldMesh = _mesher.Build(World);
    }

    public void Update(double deltaTime)
    {
        Player.Update(deltaTime, Input.State);
        
        Input.EndFrame();
    }
    
    public void Shutdown() { }

    public MeshData GetWorldMesh()
    {
        return _worldMesh ?? throw new InvalidOperationException("Engine has not been initialized.");
    }
}