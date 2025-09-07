using System;
using Meta.XR.ImmersiveDebugger.UserInterface.Generic;
using UnityEngine;

public class RadialMenuLayout : MonoBehaviour
{
    public float radius = 150f; // ditance from center
    public RectTransform[] menuPics; 
    
    void Start()
    {
        PositionPics(); 
    }

    void PositionPics()
    {
        int count = menuPics.Length;
        for (int i = 0; i < count; i++)
        {
            float angle = i * 2f * Mathf.PI / count; // i * 2pi / count 
            float x = Mathf.Cos(angle) * radius;
            float y = Mathf.Sin(angle) * radius;

            RectTransform rt = menuPics[i].GetComponent<RectTransform>();
            rt.anchoredPosition = new Vector2(x, y); 
        }
    }
}
