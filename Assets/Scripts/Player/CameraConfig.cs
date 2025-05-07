using UnityEngine;

namespace GameDevTV.RTS
{
    [System.Serializable]
    public class CameraConfig
    {
        [field: SerializeField] public bool enableEdgePan { get; private set; } = true;
        [field: SerializeField] public float mousePanSpeed { get; private set; } = 25;
        [field: SerializeField] public float edgePanSize { get; private set; } = 50;
        [field: SerializeField] public float keyboardPanSpeed { get; private set; } = 25;
        [field: SerializeField] public float minZoomDistance { get; private set; } = 5;
        [field: SerializeField] public float zoomSpeed { get; private set; } = 1;
        [field: SerializeField] public float rotationSpeed { get; private set; } = 1;
    }
}
