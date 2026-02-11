using UnityEngine;

namespace GameDevTV.RTS.Units
{
    public interface IBuildingBuilder
    {
        public GameObject Build(BuildingSO buildingSO, Vector3 targetLocation);
        public bool IsBuilding { get; }
        public void ResumeBuilding(BaseBuilding building);
        public void CancelBuilding();
        public void ResetCommandList();
    }
}
