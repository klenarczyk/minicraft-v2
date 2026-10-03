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
    private OpenGlShader? _skyShader;
    
    private OpenGlTexture? _dirtTexture;
    
    private readonly Dictionary<ChunkPosition, ChunkMeshEntry> _chunkEntries = new();
    
    private uint _skyVao;
    
    private static readonly Vector3 SkyHorizonColor = new(0.65f, 0.80f, 1.0f);
    private static readonly Vector3 SkyZenithColor = new(0.15f, 0.35f, 0.75f);
    
    public void Initialize()
    {
        _blockShader = CreateBlockShader();
        _skyShader = CreateSkyShader();

        _dirtTexture = new OpenGlTexture(gl,
            Path.Combine(AppContext.BaseDirectory, "assets", "textures", "blocks", "dirt.png"));

        _skyVao = gl.GenVertexArray();
        
        gl.Enable(EnableCap.DepthTest);
        
        gl.Enable(EnableCap.CullFace);
        gl.CullFace(TriangleFace.Back);
        gl.FrontFace(FrontFaceDirection.Ccw);
        
        // gl.PolygonMode(TriangleFace.FrontAndBack, PolygonMode.Line);
    }

    public void Render(Camera camera, IEnumerable<ChunkRenderData> chunks, double deltaTime)
    {
        if (_blockShader is null || _skyShader is null || _dirtTexture is null)
            throw new InvalidOperationException("Renderer has not been initialized.");
        
        gl.ClearColor(0, 0, 0, 1);
        gl.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
        
        var view = camera.CreateViewMatrix();
        var projection = camera.CreateProjectionMatrix();
        
        // --- Sky ---
        
        _skyShader.Use();
        
        var inverseView = Matrix4x4.Invert(view, out var invView)
            ? invView
            :throw new InvalidOperationException("Could not invert view matrix.");
        
        var inverseProjection = Matrix4x4.Invert(projection, out var invProjection)
            ? invProjection
            : throw new InvalidOperationException("Could not invert projection matrix.");
        
        _skyShader.SetMatrix4("uInvView", inverseView);
        _skyShader.SetMatrix4("uInvProjection", inverseProjection);
        
        _skyShader.SetVector3("uHorizonColor", SkyHorizonColor);
        _skyShader.SetVector3("uZenithColor", SkyZenithColor);
        
        gl.Disable(EnableCap.DepthTest);
        gl.BindVertexArray(_skyVao);
        
        gl.DrawArrays(PrimitiveType.Triangles, 0, 3);
        
        gl.BindVertexArray(0);
        gl.Enable(EnableCap.DepthTest);
        
        // --- Blocks ---
        
        _blockShader.Use();
        _dirtTexture.Bind();

        _blockShader.SetInt("uTexture", 0);
        _blockShader.SetVector3("uLightDirection", new Vector3(0.5f, 1.0f, 0.3f)); // TODO: Turn into an actual sun
        
        _blockShader.SetMatrix4("uView", view);
        _blockShader.SetMatrix4("uProjection", projection);
        
        _blockShader.SetVector3("uCameraPosition", camera.Position);
        
        _blockShader.SetVector3("uFogColor", SkyHorizonColor);
        _blockShader.SetFloat("uFogStart", 102.4f); // TODO: Make render distance dependent
        _blockShader.SetFloat("uFogEnd", 121.6f);
        
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

    private OpenGlShader CreateSkyShader()
    {
        string shaderDirectory = Path.Combine(AppContext.BaseDirectory, "assets", "shaders");
        
        string vertexSource = File.ReadAllText(Path.Combine(shaderDirectory, "sky.vert"));
        string fragmentSource = File.ReadAllText(Path.Combine(shaderDirectory, "sky.frag"));
        
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