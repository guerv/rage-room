using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class RadialSelect : MonoBehaviour
{
    public InputActionProperty thumbstickAction; // joystick
    public Image[] menuImages; 

    // Update is called once per frame
    void Update()
    {
        Vector2 input = thumbstickAction.action.ReadValue<Vector2>();

        if (input.magnitude > 0.2f) // joystick moved
        {
            float angle = Mathf.Atan2(input.y, input.x) * Mathf.Rad2Deg;
            if (angle < 0) angle += 360f;

            int numItems = menuImages.Length;
            float sliceSize = 360f / numItems;
            int selectedIndex = Mathf.FloorToInt(angle / sliceSize);

            for (int i = 0; i < numItems; i++)
            {
                menuImages[i].color = (i == selectedIndex) ? Color.yellow : Color.white; 
            }
        }
        else
        {
            foreach (var item in menuImages)
                item.color = Color.white;
        }
    }
}
