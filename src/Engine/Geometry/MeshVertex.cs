using System.Runtime.InteropServices;

namespace Minicraft.Engine.Geometry;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public readonly record struct MeshVertex(
    float X, float Y, float Z,      // Position
    float Nx, float Ny, float Nz,   // Normal
    float U, float V,               // Texture
    byte Ao                         // Ambient Occlusion
);