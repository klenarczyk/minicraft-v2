namespace Minicraft.Engine.Geometry;

public static class CubeMeshData
{
    public static readonly MeshData Data = new(
        [
            // Front (+Z)
            new MeshVertex(-0.5f, 0.5f, 0.5f, 0.0f, 1.0f),
            new MeshVertex(-0.5f, 0.5f, 0.5f, 0.0f, 1.0f),
            new MeshVertex(-0.5f, 0.5f, 0.5f, 0.0f, 1.0f),
            new MeshVertex(-0.5f, 0.5f, 0.5f, 0.0f, 1.0f),

            // Back (-Z)
            new MeshVertex(0.5f, 0.5f, -0.5f, 0.0f, 1.0f),
            new MeshVertex(0.5f, -0.5f, -0.5f, 0.0f, 0.0f),
            new MeshVertex(-0.5f, -0.5f, -0.5f, 1.0f, 0.0f),
            new MeshVertex(-0.5f, 0.5f, -0.5f, 1.0f, 1.0f),

            // Left (-X)
            new MeshVertex(-0.5f, 0.5f, -0.5f, 0.0f, 1.0f),
            new MeshVertex(-0.5f, -0.5f, -0.5f,  0.0f, 0.0f),
            new MeshVertex(-0.5f, -0.5f, 0.5f, 1.0f, 0.0f),
            new MeshVertex(-0.5f, 0.5f, 0.5f, 1.0f, 1.0f),

            // Right (+X)
            new MeshVertex(0.5f, 0.5f, 0.5f, 0.0f, 1.0f),
            new MeshVertex(0.5f, -0.5f, 0.5f, 0.0f, 0.0f),
            new MeshVertex(0.5f, -0.5f, -0.5f, 1.0f, 0.0f),
            new MeshVertex(0.5f, 0.5f, -0.5f, 1.0f, 1.0f),

            // Top (+Y)
            new MeshVertex(-0.5f, 0.5f, -0.5f, 0.0f, 1.0f),
            new MeshVertex(-0.5f, 0.5f, 0.5f, 0.0f, 0.0f),
            new MeshVertex(0.5f, 0.5f, 0.5f, 1.0f, 0.0f),
            new MeshVertex(0.5f, 0.5f, -0.5f, 1.0f, 1.0f),

            // Bottom (-Y)
            new MeshVertex(-0.5f, -0.5f, 0.5f, 0.0f, 1.0f),
            new MeshVertex(-0.5f, -0.5f, -0.5f,  0.0f, 0.0f),
            new MeshVertex(0.5f, -0.5f, -0.5f, 1.0f, 0.0f),
            new MeshVertex(0.5f, -0.5f, 0.5f, 1.0f, 1.0f)
        ],

        [
            // Front
            0, 1, 2,
            2, 3, 0,

            // Back
            4, 5, 6,
            6, 7, 4,

            // Left
            8, 9, 10,
            10, 11, 8,

            // Right
            12, 13, 14,
            14, 15, 12,

            // Top
            16, 17, 18,
            18, 19, 16,

            // Bottom
            20, 21, 22,
            22, 23, 20
        ]
    );
}