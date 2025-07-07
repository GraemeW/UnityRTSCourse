using UnityEngine;

namespace GameDevTV.RTS.Commands
{
    [CreateAssetMenu(fileName = "TestAction", menuName = "Units/Commands/Test")]
    public class TestCommand : ActionBase
    {
        public override bool CanHandle(ref CommandContext commandContext, bool skipCondition = false)
        {
            return true;
        }

        public override void Handle(CommandContext commandContext)
        {
        }
    }
}
