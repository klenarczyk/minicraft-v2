using System.Numerics;

namespace Minicraft.Engine.Gameplay;

public sealed class Camera(Vector3 position)
{
    public Vector3 Position { get; private set; } = position;

    public float Yaw { get; private set; }
    public float Pitch { get; private set; }

    public float FieldOfView { get; private set; } = MathF.PI / 2f;
    public float NearPlane { get; private set; } = 0.1f;
    public float FarPlane { get; private set; } = 1000f;

    public float AspectRatio { get; private set; } = 16f / 9f;

    public Vector3 Forward
    {
        get
        {
            var direction = new Vector3(
                MathF.Cos(Pitch) * MathF.Cos(Yaw),
                MathF.Sin(Pitch),
                MathF.Cos(Pitch) * MathF.Sin(Yaw)
            );
            
            return Vector3.Normalize(direction);
        }
    }
    
    public Vector3 Right => Vector3.Normalize(Vector3.Cross(Forward, Vector3.UnitY));

    public Vector3 Up => Vector3.Normalize(Vector3.Cross(Right, Forward));
    
    public Matrix4x4 CreateViewMatrix()
        => Matrix4x4.CreateLookAt(Position, Position + Forward, Vector3.UnitY);
    
    public Matrix4x4 CreateProjectionMatrix()
        => Matrix4x4.CreatePerspectiveFieldOfView(FieldOfView, AspectRatio, NearPlane, FarPlane);

    public void Rotate(float deltaYaw, float deltaPitch)
    {
        Yaw += deltaYaw;
        Pitch += deltaPitch;
        
        const float limit = MathF.PI / 2f - 0.01f;
        
        Pitch = Math.Clamp(Pitch, -limit, limit);
    }
    
    public void Move(Vector3 offset)
    {
        Position += offset;
    }
    
    public void Resize(int width, int height)
    {
        if (height <= 0 || width <= 0) return;
        AspectRatio = (float)width / height;
    }
}