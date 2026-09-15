namespace Minicraft.Rendering.OpenGL;

public sealed class ChunkMeshEntry
{
    public required OpenGlMesh Mesh { get; set; }
    public required int MeshVersion { get; set; }
}