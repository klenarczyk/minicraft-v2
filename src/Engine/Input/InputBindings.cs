namespace Minicraft.Engine.Input;

public class InputBindings
{
    private readonly Dictionary<GameAction, PhysicalInput> _byAction = new();
    private readonly Dictionary<PhysicalInput, GameAction> _byInput = new();
    
    public IReadOnlyDictionary<GameAction, PhysicalInput> Bindings => _byAction;
    
    public void Bind(GameAction action, PhysicalInput input)
    {
        if (_byAction.TryGetValue(action, out var prevInput))
            _byInput.Remove(prevInput);
        
        if (_byInput.TryGetValue(input, out var prevAction))
            _byAction.Remove(prevAction);
        
        _byAction[action] = input;
        _byInput[input] = action;
    }

    public bool TryGetAction(PhysicalInput input, out GameAction action)
    {
        return _byInput.TryGetValue(input, out action);
    }
}