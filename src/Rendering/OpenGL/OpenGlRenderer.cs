using System.Numerics;
using Minicraft.Engine.Gameplay;
using Minicraft.Engine.World.Chunks;
using Minicraft.Rendering.Abstractions;
using Silk.NET.Maths;
using Silk.NET.OpenGL;

namespace Minicraft.Rendering.OpenGL;

public sealed class OpenGlRenderer(GL gl) : IRenderer
{
    private OpenGlShader? _blockShader;
    private OpenGlTexture? _dirtTexture;
    
    private readonly Dictionary<ChunkPosition, ChunkMeshEntry> _chunkEntries = new();
    
    public void Initialize()
    {
        _blockShader = CreateBlockShader();

        _dirtTexture = new OpenGlTexture(gl,
            Path.Combine(AppContext.BaseDirectory, "assets", "textures", "blocks", "dirt.png"));
        
        gl.Enable(EnableCap.DepthTest);
        
        gl.Enable(EnableCap.CullFace);
        gl.CullFace(TriangleFace.Back);
        gl.FrontFace(FrontFaceDirection.Ccw);
        
        // gl.PolygonMode(TriangleFace.FrontAndBack, PolygonMode.Line);
    }

    public void Render(Camera camera, IEnumerable<ChunkRenderData> chunks, double deltaTime)
    {
        if (_blockShader is null || _dirtTexture is null)
            throw new InvalidOperationException("Renderer has not been initialized.");
        
        gl.ClearColor(0.529f, 0.808f, 0.922f, 1.0f);
        gl.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
        
        _blockShader.Use();
        _dirtTexture.Bind();

        _blockShader.SetInt("uTexture", 0);
        _blockShader.SetMatrix4("uView", camera.CreateViewMatrix());
        _blockShader.SetMatrix4("uProjection", camera.CreateProjectionMatrix());
        
        var renderedPositions = new HashSet<ChunkPosition>();
        
        foreach (var chunk in chunks)
        {
            renderedPositions.Add(chunk.Position);
            
            var mesh = GetOrCreateMesh(chunk);

            var position = new Vector3(
                chunk.Position.X * Chunk.SizeX,
                0,
                chunk.Position.Z * Chunk.SizeZ);

            var model = Matrix4x4.CreateTranslation(position);
            _blockShader.SetMatrix4("uModel", model);

            mesh.Draw();
        }
        
        foreach (var position in _chunkEntries.Keys.ToArray())
        {
            if (renderedPositions.Contains(position)) continue;

            _chunkEntries[position].Mesh.Dispose();
            _chunkEntries.Remove(position);
        }
    }
    
    public void Resize(int width, int height)
    {
        if (width <= 0 || height <= 0) return;
        gl.Viewport(new Vector2D<int>(width, height));
    }
    
    public void Shutdown()
    {
        // OpenGL resources are automatically released when the context is destroyed.
        // Explicit cleanup can slow down shutdown.
    }
    
    // ---

    private OpenGlShader CreateBlockShader()
    {
        string shaderDirectory = Path.Combine(AppContext.BaseDirectory, "assets", "shaders");

        string vertexSource = File.ReadAllText(Path.Combine(shaderDirectory, "block.vert"));
        string fragmentSource = File.ReadAllText(Path.Combine(shaderDirectory, "block.frag"));
        
        return new OpenGlShader(gl, vertexSource, fragmentSource);
    }

    private OpenGlMesh GetOrCreateMesh(ChunkRenderData chunk)
    {
        if (_chunkEntries.TryGetValue(chunk.Position, out var entry))
        {
            if (entry.MeshVersion == chunk.MeshVersion) return entry.Mesh;
            
            entry.Mesh.Dispose();
            
            var newMesh = new OpenGlMesh(gl, chunk.Mesh.Vertices, chunk.Mesh.Indices);
            
            entry.Mesh = newMesh;
            entry.MeshVersion = chunk.MeshVersion;
            
            return newMesh;
        }

        var mesh = new OpenGlMesh(gl, chunk.Mesh.Vertices, chunk.Mesh.Indices);
        _chunkEntries.Add(chunk.Position, new ChunkMeshEntry
        {
            Mesh = mesh,
            MeshVersion = chunk.MeshVersion
        });

        return mesh;
    }
}