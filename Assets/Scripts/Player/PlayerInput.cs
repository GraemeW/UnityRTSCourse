using GameDevTV.RTS.EventBus;
using GameDevTV.RTS.Events;
using GameDevTV.RTS.Units;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace GameDevTV.RTS
{
    public class PlayerInput : MonoBehaviour
    {
        // Tunables
        [SerializeField] private Rigidbody cameraTarget;
        [SerializeField] private CinemachineCamera cinemachineCamera;
        [SerializeField] private new Camera camera;
        [SerializeField] private CameraConfig cameraConfig;
        [SerializeField] private LayerMask selectableUnitsLayers;
        [SerializeField] private LayerMask floorLayers;
        [SerializeField] private RectTransform selectionBox;

        // Cached References
        private CinemachineFollow cinemachineFollow;

        // State
        private float zoomStartTime;
        private float rotationStartTime;
        private Vector3 startingFollowOffset;
        private Vector2 startingMousePosition;
        private List<ISelectable> selectedUnits;

        private void Awake()
        {
            cinemachineFollow = cinemachineCamera.GetComponent<CinemachineFollow>();
            startingFollowOffset = cinemachineFollow.FollowOffset;
            selectedUnits = new List<ISelectable>();

            Bus<UnitSelectedEvent>.OnEvent += HandleUnitSelected;
        }

        private void OnDestroy()
        {
            Bus<UnitSelectedEvent>.OnEvent -= HandleUnitSelected;
        }

        private void HandleUnitSelected(UnitSelectedEvent unitSelectedEvent)
        {
            selectedUnits.Add(unitSelectedEvent.unit);
        }

        private void Update()
        {
            if (cinemachineFollow == null) { return; }

            HandlePanning();
            HandleZooming();
            HandleRotation();
            HandleLeftClick();
            HandleRightClick();
            HandleDragSelect();
        }

        private void HandlePanning()
        {
            Vector3 moveAmount = GetKeyboardMoveAmount();
            moveAmount += GetMouseMoveAmount();

            cameraTarget.linearVelocity = moveAmount;
        }

        private Vector3 GetKeyboardMoveAmount()
        {
            Vector3 moveAmount = Vector2.zero;

            if (Keyboard.current.upArrowKey.isPressed) { moveAmount.z += cameraConfig.keyboardPanSpeed; }
            if (Keyboard.current.downArrowKey.isPressed) { moveAmount.z -= cameraConfig.keyboardPanSpeed; }
            if (Keyboard.current.leftArrowKey.isPressed) { moveAmount.x -= cameraConfig.keyboardPanSpeed; }
            if (Keyboard.current.rightArrowKey.isPressed) { moveAmount.x += cameraConfig.keyboardPanSpeed; }

            return moveAmount;
        }

        private Vector3 GetMouseMoveAmount()
        {
            Vector3 moveAmount = Vector3.zero;
            if (!cameraConfig.enableEdgePan) { return moveAmount; }

            Vector2 mousePosition = Mouse.current.position.ReadValue();
            int screenWidth = Screen.width;
            int screenHeight = Screen.height;
            if (mousePosition.x < cameraConfig.edgePanSize) { moveAmount.x -= cameraConfig.mousePanSpeed; }
            if (mousePosition.x > screenWidth - cameraConfig.edgePanSize) { moveAmount.x += cameraConfig.mousePanSpeed; }
            if (mousePosition.y < cameraConfig.edgePanSize) { moveAmount.z -= cameraConfig.mousePanSpeed; }
            if (mousePosition.y > screenHeight - cameraConfig.edgePanSize) { moveAmount.z += cameraConfig.mousePanSpeed; }

            return moveAmount;
        }

        private void HandleZooming()
        {
            if (ShouldSetZoomStartTime())
            {
                zoomStartTime = Time.time;
            }

            Vector3 targetFollowOffset;
            float zoomTime = Mathf.Clamp01((Time.time - zoomStartTime) * cameraConfig.zoomSpeed);

            if (Keyboard.current.endKey.isPressed)
            {
                targetFollowOffset = new Vector3(
                    cinemachineFollow.FollowOffset.x,
                    cameraConfig.minZoomDistance,
                    cinemachineFollow.FollowOffset.z
                );
            }
            else
            {
                targetFollowOffset = new Vector3(
                    cinemachineFollow.FollowOffset.x,
                    startingFollowOffset.y,
                    cinemachineFollow.FollowOffset.z
                );
            }

            cinemachineFollow.FollowOffset = Vector3.Slerp(
                cinemachineFollow.FollowOffset,
                targetFollowOffset,
                zoomTime
            );
        }

        private void HandleRotation()
        {
            if (ShouldSetRotationStartTime())
            {
                rotationStartTime = Time.time;
            }

            Vector3 targetFollowOffset;
            float rotationTime = Mathf.Clamp01((Time.time - rotationStartTime) * cameraConfig.rotationSpeed);

            if (Keyboard.current.pageUpKey.isPressed && Keyboard.current.pageDownKey.isPressed)
            {
                targetFollowOffset = cinemachineFollow.FollowOffset;
            }
            else if (Keyboard.current.pageUpKey.isPressed)
            {
                targetFollowOffset = new Vector3(
                    startingFollowOffset.z,
                    cinemachineFollow.FollowOffset.y,
                    startingFollowOffset.x
                );
            }
            else if (Keyboard.current.pageDownKey.isPressed)
            {
                targetFollowOffset = new Vector3(
                    -startingFollowOffset.z,
                    cinemachineFollow.FollowOffset.y,
                    startingFollowOffset.x
                );
            }
            else
            {
                targetFollowOffset = new Vector3(
                    startingFollowOffset.x,
                    cinemachineFollow.FollowOffset.y,
                    startingFollowOffset.z
                );
            }

            cinemachineFollow.FollowOffset = Vector3.Slerp(
                cinemachineFollow.FollowOffset,
                targetFollowOffset,
                rotationTime
            );
        }

        private static bool ShouldSetZoomStartTime()
        {
            return Keyboard.current.endKey.wasPressedThisFrame || Keyboard.current.endKey.wasReleasedThisFrame;
        }

        private static bool ShouldSetRotationStartTime()
        {
            return Keyboard.current.pageUpKey.wasPressedThisFrame || Keyboard.current.pageUpKey.wasReleasedThisFrame || Keyboard.current.pageDownKey.wasPressedThisFrame || Keyboard.current.pageDownKey.wasReleasedThisFrame;
        }

        private void HandleLeftClick()
        {
            if (camera == null) { return; }

            Ray cameraRay = camera.ScreenPointToRay(Mouse.current.position.ReadValue());

            if (Mouse.current.leftButton.wasReleasedThisFrame)
            {
                ClearSelectedUnits();

                if (Physics.Raycast(cameraRay, out RaycastHit hit, float.MaxValue, selectableUnitsLayers)
                && hit.collider.TryGetComponent(out ISelectable selectable))
                {
                    selectable.Select();
                }
            }
        }

        private void HandleRightClick()
        {
            if (camera == null) { return; }
            if (selectedUnits == null || selectedUnits.Count == 0) { return; }

            Ray cameraRay = camera.ScreenPointToRay(Mouse.current.position.ReadValue());

            if (Mouse.current.rightButton.wasReleasedThisFrame)
            {
                if (Physics.Raycast(cameraRay, out RaycastHit targetHit, float.MaxValue, selectableUnitsLayers) 
                    && targetHit.collider.TryGetComponent(out ISelectable selectable))
                {
                    foreach (ISelectable selectedUnit in selectedUnits)
                    {
                        if (selectedUnit is not IMoveable moveable) { continue; }
                        moveable.SetMoveTarget(targetHit.transform);
                    }
                }
                else if (Physics.Raycast(cameraRay, out RaycastHit terrainHit, float.MaxValue, floorLayers))
                {
                    foreach (ISelectable selectedUnit in selectedUnits)
                    {
                        if (selectedUnit is not IMoveable moveable) { continue; }
                        moveable.MoveTo(terrainHit.point);
                    }
                }
            }
        }

        private void HandleDragSelect()
        {
            if (selectionBox == null) { return; }

            if (Mouse.current.leftButton.wasPressedThisFrame)
            {
                selectionBox.gameObject.SetActive(true);
                startingMousePosition = Mouse.current.position.ReadValue();
            }
            else if (Mouse.current.leftButton.isPressed && !Mouse.current.leftButton.wasPressedThisFrame)
            {
                Vector2 mousePosition = Mouse.current.position.ReadValue();
                ResizeSelectionBox(mousePosition);
            }
            else if (Mouse.current.leftButton.wasReleasedThisFrame)
            {
                selectionBox.sizeDelta = Vector2.zero;
                selectionBox.gameObject.SetActive(false);

                //ClearSelectedUnits();
            }
        }

        private void ResizeSelectionBox(Vector2 mousePosition)
        {
            float width = mousePosition.x - startingMousePosition.x;
            float height = mousePosition.y - startingMousePosition.y;

            selectionBox.anchoredPosition = startingMousePosition + new Vector2(width / 2, height / 2);
            selectionBox.sizeDelta = new Vector2(Mathf.Abs(width), Mathf.Abs(height));
        }

        private void ClearSelectedUnits()
        {
            foreach (ISelectable selectedUnit in selectedUnits)
            {
                selectedUnit.Deselect();
            }
            selectedUnits.Clear();
        }
    }
}
