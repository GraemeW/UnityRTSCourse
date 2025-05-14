using GameDevTV.RTS.EventBus;
using GameDevTV.RTS.Events;
using GameDevTV.RTS.Units;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace GameDevTV.RTS
{
    public class PlayerInput : MonoBehaviour
    {
        // Tunables
        [Header("Hookups")]
        [SerializeField] private Rigidbody cameraTarget;
        [SerializeField] private CinemachineCamera cinemachineCamera;
        [SerializeField] private new Camera camera;
        [SerializeField] private CameraConfig cameraConfig;
        [Header("Game Behaviour")]
        [SerializeField] private int maxUnitCount = 100;
        [SerializeField] private bool complexMoveBehaviour = true;
        [SerializeField] private float complexMoveRadiusExpansion = 3.5f;
        [Header("SelectionBehaviour")]
        [SerializeField] private LayerMask selectableUnitsLayers;
        [SerializeField] private LayerMask floorLayers;
        [SerializeField] private RectTransform selectionBox;
        [SerializeField] private int maxSelectionCount = 12;

        // Cached References
        private CinemachineFollow cinemachineFollow;

        // State
        private float zoomStartTime;
        private float rotationStartTime;
        private Vector3 startingFollowOffset;
        private Vector2 startingMousePosition;
        private HashSet<AbstractUnit> aliveUnits;
        private HashSet<AbstractUnit> addedUnits;
        private List<ISelectable> selectedUnits;

        #region UnitMethods
        private void Awake()
        {
            cinemachineFollow = cinemachineCamera.GetComponent<CinemachineFollow>();
            startingFollowOffset = cinemachineFollow.FollowOffset;
            selectedUnits = new List<ISelectable>(maxSelectionCount);
            addedUnits = new HashSet<AbstractUnit>(maxSelectionCount);
            aliveUnits = new HashSet<AbstractUnit>(maxUnitCount);

            Bus<UnitSelectedEvent>.OnEvent += HandleUnitSelected;
            Bus<UnitDeselectedEvent>.OnEvent += HandleUnitDeselected;
            Bus<UnitSpawnEvent>.OnEvent += HandleUnitSpawned;
            Bus<UnitDespawnEvent>.OnEvent += HandleUnitDespawned;
        }

        private void OnDestroy()
        {
            Bus<UnitSelectedEvent>.OnEvent -= HandleUnitSelected;
            Bus<UnitDeselectedEvent>.OnEvent -= HandleUnitDeselected;
            Bus<UnitSpawnEvent>.OnEvent -= HandleUnitSpawned;
            Bus<UnitDespawnEvent>.OnEvent -= HandleUnitDespawned;
        }

        private void Update()
        {
            if (cinemachineFollow == null) { return; }

            HandlePanning();
            HandleZooming();
            HandleRotation();
            HandleDragSelect();
            HandleRightClick();
        }
        #endregion

        #region EventHandlers
        private void HandleUnitSelected(UnitSelectedEvent unitSelectedEvent)
        {
            if (selectedUnits.Count < maxSelectionCount)
            { 
                selectedUnits.Add(unitSelectedEvent.unit); 
            }
            else 
            {
                unitSelectedEvent.unit.Deselect(); 
            }
        }
        private void HandleUnitDeselected(UnitDeselectedEvent unitDeselectedEvent) => selectedUnits.Remove(unitDeselectedEvent.unit);
        private void HandleUnitSpawned(UnitSpawnEvent unitSpawnEvent) => aliveUnits.Add(unitSpawnEvent.unit);
        private void HandleUnitDespawned(UnitDespawnEvent unitDespawnEvent)
        {
            addedUnits.Remove(unitDespawnEvent.unit);
            aliveUnits.Remove(unitDespawnEvent.unit);

            ISelectable selectableUnit = unitDespawnEvent.unit as ISelectable;
            selectedUnits.Remove(selectableUnit);
        }
        #endregion

        #region CameraControls
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
        #endregion

        #region SelectionControls
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
                    if (!complexMoveBehaviour)
                    {
                        MoveSelectedUnitsSimple(terrainHit);
                    }
                    else
                    {
                        MoveSelectedUnitsComplex(terrainHit);
                    }
                }
            }
        }

        private void HandleDragSelect()
        {
            if (selectionBox == null) { return; }

            if (Mouse.current.leftButton.wasPressedThisFrame)
            {
                HandleMouseDown();
            }
            else if (Mouse.current.leftButton.isPressed && !Mouse.current.leftButton.wasPressedThisFrame)
            {
                HandleMouseDrag();
            }
            else if (Mouse.current.leftButton.wasReleasedThisFrame)
            {
                HandleMouseUp();
            }
        }

        private void HandleMouseDown()
        {
            selectionBox.gameObject.SetActive(true);
            startingMousePosition = Mouse.current.position.ReadValue();
            addedUnits.Clear();
        }

        private void HandleMouseDrag()
        {
            Vector2 mousePosition = Mouse.current.position.ReadValue();
            Bounds selectionBoxBounds = ResizeSelectionBox(mousePosition);
            foreach (AbstractUnit unit in aliveUnits)
            {
                Vector2 unitPosition = camera.WorldToScreenPoint(unit.transform.position);

                if (selectionBoxBounds.Contains(unitPosition))
                {
                    addedUnits.Add(unit);
                }
                if (addedUnits.Count == maxSelectionCount) { break; }
            }
        }

        private void HandleMouseUp()
        {
            if (!Keyboard.current.shiftKey.isPressed) { ClearSelectedUnits(); }

            HandleLeftClick();

            foreach (AbstractUnit unit in addedUnits)
            {
                if (unit is not ISelectable selectableUnit) { continue; }
                selectableUnit.Select();
            }

            selectionBox.sizeDelta = Vector2.zero;
            selectionBox.gameObject.SetActive(false);
        }

        private Bounds ResizeSelectionBox(Vector2 mousePosition)
        {
            float width = mousePosition.x - startingMousePosition.x;
            float height = mousePosition.y - startingMousePosition.y;

            selectionBox.anchoredPosition = startingMousePosition + new Vector2(width / 2, height / 2);
            selectionBox.sizeDelta = new Vector2(Mathf.Abs(width), Mathf.Abs(height));

            Bounds selectionBoxBounds = new Bounds(selectionBox.anchoredPosition, selectionBox.sizeDelta);
            return selectionBoxBounds;
        }

        private void HandleLeftClick()
        {
            if (camera == null) { return; }
            Ray cameraRay = camera.ScreenPointToRay(Mouse.current.position.ReadValue());

            if (Physics.Raycast(cameraRay, out RaycastHit hit, float.MaxValue, selectableUnitsLayers)
            && hit.collider.TryGetComponent(out ISelectable selectable))
            {
                selectable.Select();
            }
        }
        #endregion

        #region HelperMethods
        private void ClearSelectedUnits()
        {
            ISelectable[] currentSelectedUnits = selectedUnits.ToArray();
            foreach (ISelectable selectedUnit in currentSelectedUnits)
            {
                selectedUnit.Deselect();
            }
        }

        private void MoveSelectedUnitsSimple(RaycastHit terrainHit)
        {
            foreach (ISelectable selectedUnit in selectedUnits)
            {
                if (selectedUnit is not IMoveable moveable) { continue; }
                moveable.MoveTo(terrainHit.point);
            }
        }

        private void MoveSelectedUnitsComplex(RaycastHit terrainHit)
        {
            List<AbstractUnit> abstractUnits = new List<AbstractUnit>(selectedUnits.Count);
            foreach (ISelectable selectedUnit in selectedUnits)
            {
                if (selectedUnit is not AbstractUnit abstractUnit) { continue; }
                abstractUnits.Add(abstractUnit);
            }

            int unitsOnLayer = 0;
            int maxUnitsOnLayer = 1;
            float circleRadius = 0;
            float radialOffset = 0;
            foreach (AbstractUnit abstractUnit in abstractUnits)
            {
                if (abstractUnit is not IMoveable moveable) { continue; }

                Vector3 targetPosition = new Vector3(
                    terrainHit.point.x + circleRadius * Mathf.Cos(radialOffset * unitsOnLayer),
                    terrainHit.point.y,
                    terrainHit.point.z + circleRadius * Mathf.Sin(radialOffset * unitsOnLayer)
                    );

                moveable.MoveTo(targetPosition);
                unitsOnLayer++;

                if (unitsOnLayer >= maxUnitsOnLayer)
                {
                    unitsOnLayer = 0;
                    circleRadius += abstractUnit.agentRadius * complexMoveRadiusExpansion;
                    maxUnitsOnLayer = Mathf.FloorToInt(2 * Mathf.PI * circleRadius / (abstractUnit.agentRadius * 2));
                    radialOffset = 2 * Mathf.PI / maxUnitsOnLayer;
                }
            }
        }
        #endregion
    }
}
