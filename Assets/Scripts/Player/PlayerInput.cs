using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Unity.Cinemachine;
using UnityEngine.InputSystem;
using GameDevTV.RTS.EventBus;
using GameDevTV.RTS.Events;
using GameDevTV.RTS.Units;
using GameDevTV.RTS.Commands;
using UnityEngine.EventSystems;

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
        [SerializeField] private RectTransform selectionBox;
        [SerializeField] [ColorUsage(showAlpha:true, hdr:true)] private Color errorTintColor = Color.red;
        [SerializeField] [ColorUsage(showAlpha:true, hdr:true)] private Color errorFresnelColor = new(4.0f, 1.7f, 0f, 2.0f);
        [SerializeField] [ColorUsage(showAlpha:true, hdr:true)] private Color availableToPlaceTintColor = new(0.2f, 0.65f, 1.0f, 2.0f);
        [SerializeField] [ColorUsage(showAlpha:true, hdr:true)] private Color availableToPlaceFresnelColor = new(4.0f, 1.7f, 0f, 2.0f);
        
        // Static and Const
        public static int maxSelectionCount { get; private set; } = 12;
        private static readonly int _tint = Shader.PropertyToID("_Tint");
        private static readonly int _fresnelColor = Shader.PropertyToID("_FresnelColor");

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

        private BaseCommand queuedCommand;
        private GameObject ghostInstance;
        private MeshRenderer ghostRenderer;

        #region StaticMethods
        private static List<BaseCommand> GetAvailableCommands(AbstractCommandable abstractUnit)
        {
            var overrideCommandsCommands = new List<OverrideCommandsCommand>();
            foreach (BaseCommand command in abstractUnit.currentCommands)
            {
                if (command is OverrideCommandsCommand commandsCommand) { overrideCommandsCommands.Add(commandsCommand); }
            }

            List<BaseCommand> allAvailableCommands = new();
            foreach (OverrideCommandsCommand overrideCommand in overrideCommandsCommands)
            {
                allAvailableCommands.AddRange(overrideCommand.commandOverrides
                    .Where(command => command is not OverrideCommandsCommand));
            }

            allAvailableCommands.AddRange(abstractUnit.currentCommands
                .Where(command => command is not OverrideCommandsCommand));
            
            return allAvailableCommands;
        }
        #endregion
        
        #region UnitMethods
        private void Awake()
        {
            cinemachineFollow = cinemachineCamera.GetComponent<CinemachineFollow>();
            startingFollowOffset = cinemachineFollow.FollowOffset;
            selectedUnits = new List<ISelectable>(maxSelectionCount);
            addedUnits = new HashSet<AbstractUnit>(maxSelectionCount);
            aliveUnits = new HashSet<AbstractUnit>(maxUnitCount);

            Debug.Log("Clearing All Event Subscriptions");
            Bus<UnitSelectedEvent>.ClearAllSubscriptions();
            Debug.Log("Event Subscriptions After Clear, re-subbing");


            Bus<UnitSelectedEvent>.SubscribeToEvent(HandleUnitSelected);
            Bus<UnitDeselectedEvent>.SubscribeToEvent(HandleUnitDeselected);
            Bus<UnitSpawnEvent>.SubscribeToEvent(HandleUnitSpawned);
            Bus<UnitDeathEvent>.SubscribeToEvent(HandleUnitDeath);
            Bus<CommandSelectedEvent>.SubscribeToEvent(HandleActionSelected);
        }

        private void OnDestroy()
        {
            Bus<UnitSelectedEvent>.UnsubscribeFromEvent(HandleUnitSelected);
            Bus<UnitDeselectedEvent>.UnsubscribeFromEvent(HandleUnitDeselected);
            Bus<UnitSpawnEvent>.UnsubscribeFromEvent(HandleUnitSpawned);
            Bus<UnitDeathEvent>.UnsubscribeFromEvent(HandleUnitDeath);
            Bus<CommandSelectedEvent>.UnsubscribeFromEvent(HandleActionSelected);
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

            // Hack code to test event unsub
            if (Keyboard.current.deleteKey.wasReleasedThisFrame)
            {
                Debug.Log("Deleting All Events");
                Debug.Log("Events Before Deletion:");
                Bus.PrintAllEvents();
                Bus.DeleteAllEvents();
                Debug.Log("Events After Deletion:");
                Bus.PrintAllEvents();
                Debug.Log("End Deletion Test");
            }
        }
        #endregion

        #region EventHandlers
        private void HandleUnitSelected(UnitSelectedEvent unitSelectedEvent)
        {
            Debug.Log("Unit Selected Event Received");

            if (selectedUnits.Count < maxSelectionCount)
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
        private void HandleUnitDeath(UnitDeathEvent unitDespawnEvent)
        {
            addedUnits.Remove(unitDespawnEvent.unit);
            aliveUnits.Remove(unitDespawnEvent.unit);

            ISelectable selectableUnit = unitDespawnEvent.unit;
            selectedUnits.Remove(selectableUnit);
        }

        private void HandleActionSelected(CommandSelectedEvent commandSelectedEvent)
        {
            queuedCommand = commandSelectedEvent.baseCommand;
            if (!commandSelectedEvent.baseCommand.requiresClickToActivate)
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
                if (addedUnits.Count == maxSelectionCount) { break; }
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
            
            foreach (CommandContext commandContext in GetCommandContexts())
            {
                var volatileCommandContext = new CommandContext(commandContext);
                if (useQueuedCommand && queuedCommand != null)
                {
                    if (queuedCommand.CanHandle(ref volatileCommandContext)) { queuedCommand.Handle(volatileCommandContext); }
                }
                else
                {
                    ExecuteFirstViableCommand(volatileCommandContext.commandable, ref volatileCommandContext);
                }
            }
        }

        private List<CommandContext> GetCommandContexts()
        {
            Ray cameraRay = camera.ScreenPointToRay(Mouse.current.position.ReadValue());
            
            var commandContexts = new List<CommandContext>();
            List<AbstractCommandable> abstractUnits = selectedUnits.Where((unit) => unit is AbstractCommandable).Cast<AbstractCommandable>().ToList();
            int unitIndex = 0;
            foreach (CommandContext commandContext in abstractUnits.Select(abstractUnit => new CommandContext(abstractUnit, cameraRay, unitIndex)))
            {
                commandContexts.Add(commandContext);
                unitIndex++;
            }
            return commandContexts;
        }

        private void ExecuteFirstViableCommand(AbstractCommandable abstractUnit, ref CommandContext commandContext)
        {
            foreach (ICommand command in GetAvailableCommands(abstractUnit))
            {
                if (!command.CanHandle(ref commandContext, true)) { continue; }
                
                command.Handle(commandContext);
                break;
            }
        }

        private void SetupGhostVisuals(bool enable)
        {
            if (enable)
            {
                ghostInstance = Instantiate(queuedCommand.ghostPrefab);
                if (ghostInstance != null) { ghostRenderer = ghostInstance.GetComponentInChildren<MeshRenderer>(); }
            }
            else
            {
                if (ghostInstance != null) { Destroy(ghostInstance); }
                ghostInstance = null;
                ghostRenderer = null;
            }
        }

        private void HandleGhost()
        {
            if (ghostInstance == null) { return; }
            if (Keyboard.current.escapeKey.wasReleasedThisFrame) { SetupGhostVisuals(false); queuedCommand = null; return; }
            
            if (queuedCommand == null) { return; }
            bool positionSet = false;
            foreach (CommandContext commandContext in GetCommandContexts())
            {
                var volatileCommandContext = new CommandContext(commandContext);
                bool isValidPlacement = queuedCommand.CanHandle(ref volatileCommandContext);
                SetGhostColor(isValidPlacement);
                if (positionSet) { continue; }
                
                ghostInstance.transform.position = volatileCommandContext.hit.point;
                positionSet = true;
            }
        }

        private void SetGhostColor(bool isValidPlacement)
        {
            if (ghostRenderer == null) { return; }
            ghostRenderer.material.SetColor(_tint, isValidPlacement ? availableToPlaceTintColor : errorTintColor);
            ghostRenderer.material.SetColor(_fresnelColor, isValidPlacement ? availableToPlaceFresnelColor : errorFresnelColor);
        }
        #endregion
    }
}
