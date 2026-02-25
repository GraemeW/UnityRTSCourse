using UnityEngine;
using GameDevTV.RTS.Units;

namespace GameDevTV.RTS.Commands
{
    [CreateAssetMenu(fileName = "Attack", menuName = "Units/Commands/Attack")]
    public class AttackCommand : BaseCommand
    {
        [SerializeField] private LayerMask damageableLayers;
        
        public override bool CanHandle(ref CommandContext commandContext, bool skipCondition = false)
        {
            bool isAttacker = commandContext.commandable is IAttacker;
            bool isValidHit = Physics.Raycast(commandContext.cameraRay, out RaycastHit unitHit, float.MaxValue, damageableLayers);
            commandContext.hit = unitHit;
            bool isHitIDamageable = unitHit.transform.TryGetComponent(out IDamageable _);
            
            return isAttacker && isValidHit && isHitIDamageable;
        }

        public override void Handle(CommandContext commandContext)
        {
            IAttacker attacker = (IAttacker)commandContext.commandable;
            if (commandContext.hit.collider.TryGetComponent(out IDamageable damageable))
            {
                attacker.Attack(damageable);
            }
        }

        public override bool IsLocked(CommandContext commandContext) => false;
    }
}