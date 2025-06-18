using System.Collections.Generic;
using UnityEngine;
using Unity.Cinemachine;
using UnityEngine.InputSystem;
using GameDevTV.RTS.EventBus;
using GameDevTV.RTS.Events;
using GameDevTV.RTS.Units;
using GameDevTV.RTS.Commands;
using UnityEngine.EventSystems;
using System.Linq;
using UnityEngine.AI;

namespace GameDevTV.RTS.Player
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
        [SerializeField] private bool enableEdgePan = true;
        [SerializeField] private int maxUnitCount = 100;
        [Header("SelectionBehaviour")]
        [SerializeField] private LayerMask selectableLayers;
        [SerializeField] private LayerMask floorLayers;
        [SerializeField] private RectTransform selectionBox;
        [field: SerializeField] public static int MAX_SELECTION_COUNT { get; private set; } = 12;

        // Cached References
        private CinemachineFollow cinemachineFollow;

        // State
        private float zoomStartTime;
        private float rotationStartTime;
        private Vector3 startingFollowOffset;
        private Vector2 startingMousePosition;
        [SerializeField] private bool wasMouseDownOnUI;

        private HashSet<AbstractUnit> aliveUnits;
        private HashSet<AbstractUnit> addedUnits;
        private List<ISelectable> selectedUnits;

        private ActionBase queuedCommand;
        private GameObject ghostInstance;

        #region UnitMethods
        private void Awake()
        {
            cinemachineFollow = cinemachineCamera.GetComponent<CinemachineFollow>();
            startingFollowOffset = cinemachineFollow.FollowOffset;
            selectedUnits = new List<ISelectable>(MAX_SELECTION_COUNT);
            addedUnits = new HashSet<AbstractUnit>(MAX_SELECTION_COUNT);
            aliveUnits = new HashSet<AbstractUnit>(maxUnitCount);

            Bus<UnitSelectedEvent>.OnEvent += HandleUnitSelected;
            Bus<UnitDeselectedEvent>.OnEvent += HandleUnitDeselected;
            Bus<UnitSpawnEvent>.OnEvent += HandleUnitSpawned;
            Bus<UnitDespawnEvent>.OnEvent += HandleUnitDespawned;
            Bus<ActionSelectedEvent>.OnEvent += HandleActionSelected;
        }

        private void OnDestroy()
        {
            Bus<UnitSelectedEvent>.OnEvent -= HandleUnitSelected;
            Bus<UnitDeselectedEvent>.OnEvent -= HandleUnitDeselected;
            Bus<UnitSpawnEvent>.OnEvent -= HandleUnitSpawned;
            Bus<UnitDespawnEvent>.OnEvent -= HandleUnitDespawned;
            Bus<ActionSelectedEvent>.OnEvent -= HandleActionSelected;
        }

        private void Update()
        {
            if (cinemachineFollow == null) { return; }

            HandlePanning();
            HandleZooming();
            HandleRotation();
            HandleLeftClick();
            HandleRightClick();
            HandleGhost();
        }
        #endregion

        #region EventHandlers
        private void HandleUnitSelected(UnitSelectedEvent unitSelectedEvent)
        {
            if (selectedUnits.Count < MAX_SELECTION_COUNT)
            {
                if (!selectedUnits.Contains(unitSelectedEvent.unit)) // Explicitly separate logic, do not enter else/deselect
                {
                    selectedUnits.Add(unitSelectedEvent.unit);
                }
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

        private void HandleActionSelected(ActionSelectedEvent actionSelectedEvent)
        {
            queuedCommand = actionSelectedEvent.action;
            if (!actionSelectedEvent.action.requiresClickToActivate)
            {
                ActivateCommand(true);
                queuedCommand = null;
            }
            else if (queuedCommand.ghostPrefab != null)
            {
                SetupGhostVisuals(true);
            }
        }
        #endregion

        #region CameraControls
        private void HandlePanning()
        {
            Vector3 moveAmount = GetKeyboardMoveAmount();
            if (enableEdgePan) { moveAmount += GetMouseMoveAmount(); }

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
            if (Mouse.current.rightButton.wasReleasedThisFrame)
            {
                ActivateCommand();
            }
        }

        private void HandleLeftClick()
        {
            if (queuedCommand == null)
            {
                HandleDragSelect();
            }
            else
            {
                HandleQueuedCommandExecution();
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

            wasMouseDownOnUI = EventSystem.current.IsPointerOverGameObject();
            if (wasMouseDownOnUI)
            {
                selectionBox.sizeDelta = Vector2.zero;
                selectionBox.gameObject.SetActive(false);
            }
        }

        private void HandleMouseDrag()
        {
            if (queuedCommand != null || wasMouseDownOnUI) { return; }

            Vector2 mousePosition = Mouse.current.position.ReadValue();
            Bounds selectionBoxBounds = ResizeSelectionBox(mousePosition);
            foreach (AbstractUnit unit in aliveUnits)
            {
                Vector2 unitPosition = camera.WorldToScreenPoint(unit.transform.position);

                if (selectionBoxBounds.Contains(unitPosition))
                {
                    addedUnits.Add(unit);
                }
                if (addedUnits.Count == MAX_SELECTION_COUNT) { break; }
            }
        }

        private void HandleMouseUp()
        {
            if (wasMouseDownOnUI) { return; }

            if (queuedCommand == null && !Keyboard.current.shiftKey.isPressed) { ClearSelectedUnits(); }

            HandlePointSelect();
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

        private void HandlePointSelect()
        {
            if (camera == null) { return; }
            Ray cameraRay = camera.ScreenPointToRay(Mouse.current.position.ReadValue());

            if (Physics.Raycast(cameraRay, out RaycastHit hit, float.MaxValue, selectableLayers)
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

        private void HandleQueuedCommandExecution()
        {
            if (Mouse.current.leftButton.wasReleasedThisFrame)
            {
                if (EventSystem.current.IsPointerOverGameObject()) { return; }

                ActivateCommand(true);
                queuedCommand = null;
            }
        }

        private void ActivateCommand(bool useQueuedCommand = false)
        {
            if (camera == null) { return; }
            if (selectedUnits.Count == 0) { return; }
            SetupGhostVisuals(false);

            Ray cameraRay = camera.ScreenPointToRay(Mouse.current.position.ReadValue());

            List<AbstractCommandable> abstractUnits = selectedUnits.Where((unit) => unit is AbstractCommandable).Cast<AbstractCommandable>().ToList();
            int unitIndex = 0;
            foreach (AbstractCommandable abstractUnit in abstractUnits)
            {
                CommandContext commandContext = new CommandContext(abstractUnit, cameraRay, unitIndex);

                if (useQueuedCommand && queuedCommand != null)
                {
                    if (queuedCommand.CanHandle(ref commandContext)) { queuedCommand.Handle(commandContext); }
                }
                else
                {
                    ExecuteFirstViableCommand(abstractUnit, ref commandContext);
                }
                unitIndex++;
            }
        }

        private void ExecuteFirstViableCommand(AbstractCommandable abstractUnit, ref CommandContext commandContext)
        {
            foreach (ICommand command in abstractUnit.currentCommands)
            {
                if (command.CanHandle(ref commandContext))
                {
                    command.Handle(commandContext);
                    break;
                }
            }
        }

        private void SetupGhostVisuals(bool enable)
        {
            if (enable)
            {
                ghostInstance = Instantiate(queuedCommand.ghostPrefab);
            }
            else
            {
                if (ghostInstance != null) { Destroy(ghostInstance); }
                ghostInstance = null;
            }
        }

        private void HandleGhost()
        {
            if (ghostInstance == null) { return; }
            if (Keyboard.current.escapeKey.wasReleasedThisFrame) { SetupGhostVisuals(false); queuedCommand = null; return; }

            Ray cameraRay = camera.ScreenPointToRay(Mouse.current.position.ReadValue());

            if (Physics.Raycast(cameraRay, out RaycastHit hit, float.MaxValue, floorLayers))
            {
                ghostInstance.transform.position = hit.point;
            }
        }
        #endregion
    }
}
