using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MouseManager : MonoBehaviour{

    public bool mouseOnUI = false;
    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;
    }

	public Vector2 GetTileCoordsFromMouse()
    {
        Vector2 worldPos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        if (worldPos.x < 0)
            worldPos.x--;
        if (worldPos.y < 0)
            worldPos.y--;
        worldPos.x = (int)worldPos.x;
        worldPos.y = (int)worldPos.y;
        return worldPos;
    }

    public void MouseOnGUI()
    {
        mouseOnUI = true;
    }

    public void MouseOffGUI()
    {
        mouseOnUI = false;
    }
}
