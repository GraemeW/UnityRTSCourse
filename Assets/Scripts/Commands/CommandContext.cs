using UnityEngine;
using GameDevTV.RTS.Units;

namespace GameDevTV.RTS.Commands
{
    public struct CommandContext
    {
        // Properties
        public AbstractCommandable commandable {  get; private set; }
        public Ray cameraRay { get; private set; }
        public RaycastHit hit { get; set; }
        public int unitIndex { get; set; }

        // Constructors
        // Default -- have ray, need to derive hit
        public CommandContext(AbstractCommandable commandable, Ray cameraRay, int unitIndex = 0)
        {
            this.commandable = commandable;
            this.cameraRay = cameraRay;
            this.unitIndex = unitIndex;

            hit = new RaycastHit();
        }
        
        // Derivative
        public CommandContext(CommandContext commandContext)
        {
            commandable = commandContext.commandable;
            unitIndex = commandContext.unitIndex;
            hit = commandContext.hit;
            cameraRay = commandContext.cameraRay;
        }
    }
}
