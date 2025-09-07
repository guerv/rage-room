using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class RadialMenuController : MonoBehaviour
{
    [Header("Menu Setup")]
    public GameObject radialMenu; // assign canvas
    public Image[] menuImages;

    [Header("Spawning")]
    public GameObject[] prefabs; // one per slice
    public Transform vrControllerTransform; // reference to VR controller's transform

    [Header("XR")]
    public XRDirectInteractor interactor; // RH controller
    public InputActionProperty showMenuAction; // grip button action
    public InputActionProperty thumbstickAction; // joystick

    private int currentSelection = -1; // currently highlighted slice
    private int lastSelection = -1; // conf on release
    private bool menuActive = false; 

    private void OnEnable() // when canvas is enabled 
    {
        if (showMenuAction != null && showMenuAction.action != null) showMenuAction.action.Enable();
        if (thumbstickAction != null && thumbstickAction.action != null) thumbstickAction.action.Enable();

        if (radialMenu != null)
            radialMenu.SetActive(false);
    }
    private void OnDisable()
    {
        if (showMenuAction != null && showMenuAction.action != null) showMenuAction.action.Disable();
        if (thumbstickAction != null && thumbstickAction.action != null) thumbstickAction.action.Disable();
    }


    void Update()
    {
        bool isGrabbing = interactor.hasSelection;
        float gripValue = 0f;


        // When to show menu
        if (showMenuAction != null && showMenuAction.action != null)
        {
            gripValue = showMenuAction.action.ReadValue<float>();
        }

        bool pressed = gripValue > 0.5f;

        menuActive = pressed && !isGrabbing;
        radialMenu.SetActive(menuActive);

        if (menuActive)
        {
            HandleJoystickSelection();
        }
        else
        {
            currentSelection = -1;
            // grip released 
            if (lastSelection != -1)
            {
                SpawnSelectedPrefab(lastSelection);
                lastSelection = -1;
            }

            foreach (var img in menuImages)
                img.color = Color.white; 
        }

    }

    private void HandleJoystickSelection()
    {
        Vector2 input = thumbstickAction.action.ReadValue<Vector2>();

        // joystick movement 
        if (input.magnitude > 0.2f) // joystick moved
        {

            float angle = Mathf.Atan2(input.y, input.x) * Mathf.Rad2Deg;
            if (angle < 0) angle += 360f;

            int numItems = menuImages.Length;
            float sliceSize = 360f / numItems;
            int selectedIndex = Mathf.FloorToInt(angle / sliceSize);

            if (selectedIndex != currentSelection)
            {
                currentSelection = selectedIndex;
                lastSelection = currentSelection; 
            }

            for (int i = 0; i < numItems; i++)
            {
                menuImages[i].color = (i == currentSelection) ? Color.yellow : Color.white;
            }
        }
        else
        {
            currentSelection = -1; 
            foreach (var item in menuImages)
                item.color = Color.white;
        }
    }

    private void SpawnSelectedPrefab(int index)
    {
        if (index < 0 || index >= prefabs.Length) return; // null case 

        GameObject selectedPrefab = prefabs[index];
        GameObject spawned = Instantiate(selectedPrefab, vrControllerTransform.position, vrControllerTransform.rotation);

        //auto-attach prefab to controller
        var interactable = spawned.GetComponent<XRGrabInteractable>();
        if (interactable != null && interactor != null && interactor.interactionManager != null)
        {
            interactor.interactionManager.SelectEnter(
                (IXRSelectInteractor)interactor,
                (IXRSelectInteractable)interactable
            );
        }
    }
}
