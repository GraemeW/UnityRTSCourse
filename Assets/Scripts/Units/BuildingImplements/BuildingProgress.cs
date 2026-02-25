using UnityEngine;

namespace GameDevTV.RTS.Units
{
    [System.Serializable]
    public struct BuildingProgress
    {
        [System.Serializable]
        public enum BuildingState
        {
            Building,
            Paused,
            Completed,
            Destroyed
        }
        [field: SerializeField] public float startTime { get; private set; }
        [field: SerializeField] public float progress { get; private set; }
        [field: SerializeField] public BuildingState state { get; private set; }

        public BuildingProgress(BuildingState state, float startTime, float progress)
        {
            this.state = state;
            this.startTime = startTime;
            this.progress = progress;
        }
    }
}
