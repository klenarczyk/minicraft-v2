using System.Numerics;
using Silk.NET.OpenGL;

namespace Minicraft.Rendering.OpenGL;

public sealed class OpenGlShader : IDisposable
{
    private readonly GL _gl;
    
    public uint Handle { get; }

    public OpenGlShader(GL gl, string vertexSource, string fragmentSource)
    {
        _gl = gl;
        
        uint vertexShader = CompileShader(ShaderType.VertexShader, vertexSource);
        uint fragmentShader = CompileShader(ShaderType.FragmentShader, fragmentSource);
        
        Handle = gl.CreateProgram();
        
        gl.AttachShader(Handle, vertexShader);
        gl.AttachShader(Handle, fragmentShader);

        gl.LinkProgram(Handle);

        gl.GetProgram(
            Handle,
            ProgramPropertyARB.LinkStatus,
            out int linkStatus);

        if (linkStatus != (int)GLEnum.True)
            throw new Exception("Program failed to link: " + gl.GetProgramInfoLog(Handle));

        gl.DetachShader(Handle, vertexShader);
        gl.DetachShader(Handle, fragmentShader);

        gl.DeleteShader(vertexShader);
        gl.DeleteShader(fragmentShader);
    }

    public void Use()
    {
        _gl.UseProgram(Handle);
    }

    public void SetInt(string name, int value)
    {
        int location = GetUniformLocation(name);
        _gl.Uniform1(location, value);
    }

    public void SetFloat(string name, float value)
    {
        int location = GetUniformLocation(name);
        _gl.Uniform1(location, value);
    }

    public unsafe void SetMatrix4(string name, Matrix4x4 matrix)
    {
        int location = GetUniformLocation(name);

        Span<float> values = stackalloc float[16];

        values[0] = matrix.M11;
        values[1] = matrix.M12;
        values[2] = matrix.M13;
        values[3] = matrix.M14;

        values[4] = matrix.M21;
        values[5] = matrix.M22;
        values[6] = matrix.M23;
        values[7] = matrix.M24;

        values[8] = matrix.M31;
        values[9] = matrix.M32;
        values[10] = matrix.M33;
        values[11] = matrix.M34;

        values[12] = matrix.M41;
        values[13] = matrix.M42;
        values[14] = matrix.M43;
        values[15] = matrix.M44;

        _gl.UniformMatrix4(location, false, values);
    }

    private int GetUniformLocation(string name)
    {
        int location = _gl.GetUniformLocation(Handle, name);

        if (location == -1)
            throw new InvalidOperationException($"Could not find uniform '{name}'.");

        return location;
    }

    private uint CompileShader(ShaderType type, string source)
    {
        uint shader = _gl.CreateShader(type);

        _gl.ShaderSource(shader, source);
        _gl.CompileShader(shader);

        _gl.GetShader(
            shader,
            ShaderParameterName.CompileStatus,
            out int status);

        if (status != (int)GLEnum.True)
            throw new InvalidOperationException($"{type} failed to compile: {_gl.GetShaderInfoLog(shader)}");

        return shader;
    }

    public void Dispose()
    {
        _gl.DeleteProgram(Handle);
    }
}