using System.Numerics;
using Minicraft.Engine.Input;

namespace Minicraft.Engine.Gameplay;

public sealed class Player
{
    public Camera Camera { get; } = new(Vector3.Zero);

    public void Update(double deltaTime, InputState input)
    {
        const float mouseSensitivity = 0.002f;
        const float speed = 5f;
        
        Camera.Rotate(
            input.MouseDelta.X * mouseSensitivity, 
            -input.MouseDelta.Y * mouseSensitivity
        );
        
        var movement = Vector3.Zero;

        var forward = Camera.Forward;
        forward.Y = 0;
        
        if (forward.LengthSquared() > 0)
            forward = Vector3.Normalize(forward);
        
        var right = Camera.Right;
        var up = Vector3.UnitY;

        if (input.IsActive(GameAction.MoveForward))
            movement += forward;

        if (input.IsActive(GameAction.MoveBackward))
            movement -= forward;

        if (input.IsActive(GameAction.MoveRight))
            movement += right;

        if (input.IsActive(GameAction.MoveLeft))
            movement -= right;
        
        if (input.IsActive(GameAction.Jump))
            movement += up;

        if (input.IsActive(GameAction.Crouch))
            movement -= up;

        if (movement.LengthSquared() > 0)
            movement = Vector3.Normalize(movement);

        Camera.Move(movement * speed * (float)deltaTime);
    }
}