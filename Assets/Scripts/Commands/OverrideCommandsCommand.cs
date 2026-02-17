using System.Collections.Generic;
using UnityEngine;

namespace GameDevTV.RTS.Commands
{
    [CreateAssetMenu(fileName = "OverrideCommands", menuName = "Units/Commands/OverrideCommands", order = 110)]
    public class OverrideCommandsCommand : BaseCommand
    {
        [field: SerializeField] public List<BaseCommand> commandOverrides { get; private set; } = new();

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
