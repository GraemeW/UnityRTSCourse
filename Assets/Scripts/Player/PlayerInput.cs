using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

namespace GameDevTV.RTS
{
    public class PlayerInput : MonoBehaviour
    {
        [SerializeField] private Transform cameraTarget;
        [SerializeField] private float keyboardPanSpeed = 5;

        private void Update()
        {
            Vector3 moveAmount = Vector2.zero;

            if (Keyboard.current.upArrowKey.isPressed) { moveAmount.z += keyboardPanSpeed; }
            if (Keyboard.current.downArrowKey.isPressed) { moveAmount.z -= keyboardPanSpeed; }
            if (Keyboard.current.leftArrowKey.isPressed) { moveAmount.x -= keyboardPanSpeed; }
            if (Keyboard.current.rightArrowKey.isPressed) { moveAmount.x += keyboardPanSpeed; }

            moveAmount *= Time.deltaTime;

            cameraTarget.position = cameraTarget.position + moveAmount;
        }
    }
}
