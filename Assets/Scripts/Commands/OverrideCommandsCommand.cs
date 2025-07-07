using UnityEngine;

namespace GameDevTV.RTS.Commands
{
    [CreateAssetMenu(fileName = "OverrideCommands", menuName = "Units/Commands/OverrideCommands", order = 110)]
    public class OverrideCommandsCommand : ActionBase
    {
        [field: SerializeField] public ActionBase[] commandOverrides { get; private set; }

        public override bool CanHandle(ref CommandContext commandContext, bool skipCondition = false)
        {
            return commandContext.commandable != null;
        }

        public override void Handle(CommandContext commandContext)
        {
            commandContext.commandable.SetCommandOverrides(commandOverrides);
        }
    }
}
