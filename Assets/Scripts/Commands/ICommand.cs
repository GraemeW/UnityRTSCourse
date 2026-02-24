namespace GameDevTV.RTS.Commands
{
    public interface ICommand
    {
        public bool isSingleUnitCommand { get; }
        public bool requiresClickToActivate { get; }
        public bool allowRightClick { get; }
        
        bool CanHandle(ref CommandContext commandContext, bool skipCondition = false);
        void Handle(CommandContext commandContext);
    }
}
