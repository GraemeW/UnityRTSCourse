using GameDevTV.RTS.Units;
using UnityEngine;

namespace GameDevTV.RTS.Commands
{
    public abstract class ActionBase : ScriptableObject, ICommand
    {
        public abstract bool CanHandle(AbstractCommandable commandable, Ray cameraRay, out RaycastHit hit);
        public abstract void Handle(AbstractCommandable commandable, RaycastHit hit);
    }
}
