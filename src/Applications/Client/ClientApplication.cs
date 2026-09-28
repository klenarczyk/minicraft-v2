using System.Numerics;
using Minicraft.Engine;
using Minicraft.Engine.Input;
using Minicraft.Rendering.Conversion;
using Minicraft.Rendering.OpenGL;
using Silk.NET.Input;
using Silk.NET.Input.Sdl;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;
using Silk.NET.Windowing.Sdl;

namespace Minicraft.Applications.Client;

public sealed class ClientApplication
{
    private readonly IWindow _window;
    
    private readonly GameEngine _engine = new();
    private OpenGlRenderer? _renderer;
    
    private IKeyboard? _keyboard;
    private IMouse? _mouse;

    private Vector2 _lastMousePosition;
    private bool _firstMouseMove = true;
    
    public ClientApplication()
    {
        SdlWindowing.RegisterPlatform();
        SdlWindowing.Use();
        SdlInput.RegisterPlatform();
        
        var options = WindowOptions.Default;
        options.Title = "Minicraft";

        _window = Window.Create(options);

        _window.Load += Load;
        _window.Update += Update;
        _window.Render += Render;
        _window.Closing += Close;
        
        _window.FramebufferResize += size =>
        {
            _renderer?.Resize(size.X, size.Y);
            _engine.Player.Camera.Resize(size.X, size.Y);
        };
        
        // _window.WindowState = WindowState.Fullscreen;
    }

    public void Run() => _window.Run();
    
    // ---

    private void Load()
    {
        ConfigureInput();
        
        var size = _window.FramebufferSize;
        var gl = _window.CreateOpenGL(); // Only OpenGL is supported
        
        _renderer = new OpenGlRenderer(gl);
        _renderer.Initialize();
        
        _engine.Initialize();
        
        _engine.Player.Camera.Resize(size.X, size.Y);
        _renderer.Resize(size.X, size.Y);
        
        ConfigureDefaultBindings();
    }

    private void Update(double deltaTime)
    {
        _engine.Update(deltaTime);
    }
    
    private void Render(double deltaTime)
    {
        if (_renderer is null) 
            throw new InvalidOperationException("Renderer has not been initialized.");

        var chunks = ChunkRenderDataConverter
            .Convert(_engine.World.Chunks.LoadedChunks)
            .ToArray();
        
        _renderer.Render(_engine.Player.Camera, chunks, deltaTime);
    }
    
    private void Close()
    {
        _engine.Shutdown();
        _renderer?.Shutdown();
    }
    
    // --- Config ---

    private void ConfigureInput()
    {
        var input = _window.CreateInput();
        _keyboard = input.Keyboards[0];
        _mouse = input.Mice[0];
        
        // Keyboard
        _keyboard.KeyDown += (_, key, _) =>
        {
            if (key == Key.Escape) _window.Close();
            _engine.Input.HandleInput(InputAdapter.ToPhysicalInput(key), true);
        };
        
        _keyboard.KeyUp += (_, key, _) =>
        {
            _engine.Input.HandleInput(InputAdapter.ToPhysicalInput(key), false);
        };
        
        // Mouse
        _mouse.Cursor.CursorMode = CursorMode.Raw;

        _mouse.MouseDown += (_, button) =>
        {
            _engine.Input.HandleInput(InputAdapter.ToPhysicalInput(button), true);
        };

        _mouse.MouseUp += (_, button) =>
        {
            _engine.Input.HandleInput(InputAdapter.ToPhysicalInput(button), false);
        };
        
        _mouse.MouseMove += (_, position) =>
        {
            if (_firstMouseMove)
            {
                _lastMousePosition = position;
                _firstMouseMove = false;
                return;
            }
            
            _engine.Input.AddMouseDelta(position - _lastMousePosition);
            _lastMousePosition = position;
        };
    }
    
    private void ConfigureDefaultBindings()
    {
        var bindings = _engine.Input.Bindings;

        Bind(GameAction.MoveForward, Key.W);
        Bind(GameAction.MoveBackward, Key.S);
        Bind(GameAction.MoveLeft, Key.A);
        Bind(GameAction.MoveRight, Key.D);
        Bind(GameAction.Jump, Key.Space);
        Bind(GameAction.Crouch, Key.ControlLeft);
        
        return;

        void Bind(GameAction action, Key key) => bindings.Bind(action, InputAdapter.ToPhysicalInput(key));
    }
}