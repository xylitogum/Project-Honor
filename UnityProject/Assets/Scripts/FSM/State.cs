namespace FSM
{
    /// <summary>
    /// Valid states for the finite-state machine. Each of these
    /// should represent a tactical or strategic behavior pattern.
    /// </summary>
    public enum State
    {
        Tactical_Aiming,
        Tactical_Resting,
        Tactical_Moving,
        Tactical_Retreating,
        Strategic_FanOut,
        Strategic_FocusFire,
        Strategic_Regroup,
        Strategic_MoveToFiringPosition,
        Strategic_HoldPosition,
        Strategic_Aim,
        Strategic_Move,
        Strategic_TakeCover,
        Strategic_Null
        
    }
}