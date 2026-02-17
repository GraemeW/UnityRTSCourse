namespace GameDevTV.RTS.Commands
{
    public interface ICommand
    {
        bool CanHandle(ref CommandContext commandContext, bool skipCondition = false);
        void Handle(CommandContext commandContext);
    }
}
