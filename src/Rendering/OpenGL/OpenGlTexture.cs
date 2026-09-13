using Silk.NET.OpenGL;
using StbImageSharp;

namespace Minicraft.Rendering.OpenGL;

public sealed class OpenGlTexture : IDisposable
{
    private readonly GL _gl;

    public uint Handle { get; }

    public OpenGlTexture(GL gl, string path)
    {
        _gl = gl;
        
        Handle = gl.GenTexture();
        gl.BindTexture(TextureTarget.Texture2D, Handle);

        using var stream = File.OpenRead(path);
        var image = ImageResult.FromStream(stream, ColorComponents.RedGreenBlueAlpha);

        unsafe
        {
            fixed (byte* pixels = image.Data)
            {
                gl.TexImage2D(
                    TextureTarget.Texture2D,
                    0,
                    InternalFormat.Rgba,
                    (uint)image.Width,
                    (uint)image.Height,
                    0,
                    PixelFormat.Rgba,
                    PixelType.UnsignedByte,
                    pixels);
            }
        }
        
        gl.GenerateMipmap(TextureTarget.Texture2D);
        
        gl.TexParameter(
            TextureTarget.Texture2D,
            TextureParameterName.TextureWrapS,
            (int)GLEnum.Repeat);
        
        gl.TexParameter(
            TextureTarget.Texture2D,
            TextureParameterName.TextureWrapT,
            (int)GLEnum.Repeat);
        
        gl.TexParameter(
            TextureTarget.Texture2D,
            TextureParameterName.TextureMinFilter,
            (int)GLEnum.NearestMipmapNearest);
        
        gl.TexParameter(
            TextureTarget.Texture2D,
            TextureParameterName.TextureMagFilter,
            (int)GLEnum.Nearest);
        
        gl.BindTexture(TextureTarget.Texture2D, 0);
    }

    public void Bind(uint slot = 0)
    {
        _gl.ActiveTexture((TextureUnit)((uint)TextureUnit.Texture0 + slot));
        _gl.BindTexture(TextureTarget.Texture2D, Handle);
    }
    
    public void Dispose()
    {
        _gl.DeleteTexture(Handle);
    }
}