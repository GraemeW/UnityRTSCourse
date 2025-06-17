using UnityEngine;

namespace GameDevTV.RTS.Utilities
{
    public static class AnimationConstants
    {
        // Static Animator References
        // Note:  These MUST match the variables in the animator variables

        public static int speedRef { get; private set; } = Animator.StringToHash("Speed");
        public static int speedMagnitudeRef { get; private set; } = Animator.StringToHash("SpeedMagnitude");
        public static int attackRef { get; private set; } = Animator.StringToHash("Attack");
        public static int isGatheringRef { get; private set; } = Animator.StringToHash("IsGathering");
        public static int isBuildingRef { get; private set; } = Animator.StringToHash("IsBuilding");

        public static void AnimateMovement(Animator animator, float speed)
        {
            if (animator == null || animator.runtimeAnimatorController == null) { return; }
            animator.SetFloat(AnimationConstants.speedRef, speed);
        }

        public static void AnimateGathering(Animator animator, bool isGathering)
        {
            if (animator == null || animator.runtimeAnimatorController == null) { return; }
            animator.SetBool(AnimationConstants.isGatheringRef, isGathering);
        }
    }
}
