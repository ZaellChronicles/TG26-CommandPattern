namespace Player.Runtime
{
    public interface ICommand
    {
        string Label { get; }

        void Execute();
    }
}