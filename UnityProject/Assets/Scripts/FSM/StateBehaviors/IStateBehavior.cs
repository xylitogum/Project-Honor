namespace FSM.StateBehaviors
{
    /// <summary>
    /// An interface for defining state behaviors. All classes that
    /// represent the behavior of a state must implement this interface.
    /// </summary>
    public interface IStateBehavior
    {
        Command GetCommand(Character agent, GameManager gameManager);
    }
}