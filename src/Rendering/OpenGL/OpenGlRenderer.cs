using System.Numerics;
using Minicraft.Engine.Gameplay;
using Minicraft.Rendering.Geometry;
using Silk.NET.Maths;
using Silk.NET.OpenGL;

namespace Minicraft.Rendering.OpenGL;

public sealed class OpenGlRenderer(GL gl) : IRenderer
{
    private OpenGlShader? _blockShader;
    private OpenGlTexture? _dirtTexture;
    private OpenGlMesh? _cubeMesh;
    
    public void Initialize()
    {
        _blockShader = CreateBlockShader();

        _dirtTexture = new OpenGlTexture(gl,
            Path.Combine(AppContext.BaseDirectory, "assets", "textures", "blocks", "dirt.png"));

        _cubeMesh = new OpenGlMesh(gl, CubeMeshData.Vertices, CubeMeshData.Indices);
        
        gl.Enable(EnableCap.DepthTest);
        
        gl.Enable(EnableCap.CullFace);
        gl.CullFace(TriangleFace.Back);
        gl.FrontFace(FrontFaceDirection.Ccw);
    }

    public void Render(Camera camera, double deltaTime)
    {
        if (_blockShader is null || _dirtTexture is null || _cubeMesh is null)
            throw new InvalidOperationException("Renderer has not been initialized.");
        
        gl.ClearColor(0.529f, 0.808f, 0.922f, 1.0f);
        gl.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
        
        _blockShader.Use();
        
        _dirtTexture.Bind();
        
        var model = Matrix4x4.CreateTranslation(3, 0, 0);

        _blockShader.SetInt("uTexture", 0);
        _blockShader.SetMatrix4("uModel", model);
        _blockShader.SetMatrix4("uView", camera.CreateViewMatrix());
        _blockShader.SetMatrix4("uProjection", camera.CreateProjectionMatrix());

        _cubeMesh.Draw();
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
}