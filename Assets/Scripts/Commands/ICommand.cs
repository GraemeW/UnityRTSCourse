namespace GameDevTV.RTS.Commands
{
    public interface ICommand
    {
        public bool IsSingleUnitCommand { get; }
        bool CanHandle(ref CommandContext commandContext, bool skipCondition = false);
        void Handle(CommandContext commandContext);
    }
}
