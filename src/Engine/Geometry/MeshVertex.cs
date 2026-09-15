using System.Runtime.InteropServices;

namespace Minicraft.Engine.Geometry;

[StructLayout(LayoutKind.Sequential)]
public readonly record struct MeshVertex(
    float X, float Y, float Z,      // Position
    float Nx, float Ny, float Nz,   // Normals
    float U, float V                // Texture
);