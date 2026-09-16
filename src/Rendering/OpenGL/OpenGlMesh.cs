using Minicraft.Engine.Geometry;
using Minicraft.Engine.World.Meshing;
using Silk.NET.OpenGL;

namespace Minicraft.Rendering.OpenGL;

public sealed class OpenGlMesh : IDisposable
{
    private readonly GL _gl;

    private readonly uint _vao;
    private readonly uint _vbo;
    private readonly uint _ebo;

    private readonly uint _indexCount;

    public unsafe OpenGlMesh(GL gl, MeshVertex[] vertices, uint[] indices)
    {
        _gl = gl;

        _indexCount = (uint)indices.Length;

        _vao = gl.GenVertexArray();
        _vbo = gl.GenBuffer();
        _ebo = gl.GenBuffer();

        gl.BindVertexArray(_vao);

        // Vertex buf
        gl.BindBuffer(BufferTargetARB.ArrayBuffer, _vbo);

        fixed (MeshVertex* buffer = vertices)
        {
            gl.BufferData(
                BufferTargetARB.ArrayBuffer,
                (nuint)(vertices.Length * sizeof(MeshVertex)),
                buffer,
                BufferUsageARB.StaticDraw);
        }

        // Index buf
        gl.BindBuffer(BufferTargetARB.ElementArrayBuffer, _ebo);

        fixed (uint* buffer = indices)
        {
            gl.BufferData(
                BufferTargetARB.ElementArrayBuffer,
                (nuint)(indices.Length * sizeof(uint)),
                buffer,
                BufferUsageARB.StaticDraw);
        }

        // Position
        const uint positionLocation = 0;

        gl.EnableVertexAttribArray(positionLocation);

        gl.VertexAttribPointer(
            positionLocation,
            3,
            VertexAttribPointerType.Float,
            false,
            (uint)sizeof(MeshVertex),
            (void*)0);

        // Normals
        const uint normalLocation = 1;
        
        gl.EnableVertexAttribArray(normalLocation);
        
        gl.VertexAttribPointer(
            normalLocation,
            3,
            VertexAttribPointerType.Float,
            false,
            (uint)sizeof(MeshVertex),
            (void*)(3 * sizeof(float)));
        
        // Texture coords
        const uint texCoordLocation = 2;

        gl.EnableVertexAttribArray(texCoordLocation);

        gl.VertexAttribPointer(
            texCoordLocation,
            2,
            VertexAttribPointerType.Float,
            false,
            (uint)sizeof(MeshVertex),
            (void*)(6 * sizeof(float)));
        
        // Ambient occlusion
        // Texture coords
        const uint aoLocation = 3;

        gl.EnableVertexAttribArray(aoLocation);

        gl.VertexAttribPointer(
            aoLocation,
            1,
            VertexAttribPointerType.UnsignedByte,
            false,
            (uint)sizeof(MeshVertex),
            (void*)(8 * sizeof(float)));
        
        // ---
        
        gl.BindVertexArray(0);
        gl.BindBuffer(BufferTargetARB.ArrayBuffer, 0);
    }

    public unsafe void Draw()
    {
        _gl.BindVertexArray(_vao);

        _gl.DrawElements(
            PrimitiveType.Triangles,
            _indexCount,
            DrawElementsType.UnsignedInt,
            null);

        _gl.BindVertexArray(0);
    }

    public void Dispose()
    {
        _gl.DeleteVertexArray(_vao);
        _gl.DeleteBuffer(_vbo);
        _gl.DeleteBuffer(_ebo);
    }
}