namespace FSM_Strategic
{
    public enum Strategy
    {
        FocusFire
    }

    public class StrategicCommand
    {
        public readonly Strategy Strategy;
        public readonly Character EnemyTarget;
    }
}