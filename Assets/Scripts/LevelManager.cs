using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{

    [SerializeField]
    GameObject spriteGO;
    [SerializeField]
    SpriteManager spriteManager;
    [SerializeField]
    MouseManager mouseManager;
    [SerializeField]
    TileManager tileManager;

    void Start()
    {
        InitialiseHighlight();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateHighlight();
        if (Input.GetMouseButtonDown(0) && !mouseManager.mouseOnUI)
        {
            StartDraw();
        }
        if (Input.GetMouseButton(0) && drawing)
            UpdateDraw();
        if (Input.GetMouseButtonUp(0) && drawing)
        {
            EndDraw();
        }
    }

    //Highlight Code
    #region
    GameObject highlightGO;
    bool highlightEnabled = true;
    private void InitialiseHighlight()
    {
        highlightGO = Instantiate(spriteGO, Vector2.zero, Quaternion.identity);
        SpriteRenderer sr = highlightGO.GetComponent<SpriteRenderer>();
        sr.sprite = spriteManager.GetUISprite("highlight");
        sr.sortingLayerName = "Overlay";
    }
    private void UpdateHighlight()
    {
        if(!highlightEnabled)
        {
            highlightGO.SetActive(false);
            return;
        }
        if (mouseManager.mouseOnUI == false)
        {
            if (highlightGO.activeSelf == false)
                highlightGO.SetActive(true);
            highlightGO.transform.position = mouseManager.GetTileCoordsFromMouse();
        }
        else
            highlightGO.SetActive(false);
    }
    #endregion

    //Draw Sprite Code
    #region
    bool drawing = false;
    bool wallMode = false;
    bool changeDirection = false;
    GameObject highlightArea;
    Vector2 mouseStartPos;
    Vector2 mouseEndPos;
    Vector2 direction;
    private void StartDraw()
    {
        drawing = true;
        highlightEnabled = false;
        mouseStartPos = mouseManager.GetTileCoordsFromMouse();
        highlightArea = new GameObject("Highlight Area");
        SpriteRenderer spriteRend = highlightArea.AddComponent<SpriteRenderer>();
        spriteRend.sprite = spriteManager.GetUISprite("highlightArea");
        highlightArea.transform.position = mouseStartPos;
        spriteRend.sortingLayerName = "Overlay";

        if (spriteManager.GetActiveSprite().type == TileManager.TileType.Wall && !spriteManager.DeleteMode)
        {
            wallMode = true;
        }
        else
            wallMode = false;
    }
    private void UpdateDraw()
    {
        if(Input.GetKeyDown("escape"))
            EndDraw(false);
        if (mouseManager.GetTileCoordsFromMouse() == mouseEndPos && !wallMode)
            return;
        mouseEndPos = mouseManager.GetTileCoordsFromMouse();

        if(wallMode)
        {
            if(changeDirection)
            {
                if(mouseEndPos.x != mouseStartPos.x)
                {
                    changeDirection = false;
                    direction = Vector2.right;
                }
                else
                {
                    changeDirection = false;
                    direction = Vector2.up;
                }
            }

            if (mouseStartPos == mouseEndPos)
            {
                changeDirection = true;
            }

            if(direction == Vector2.right)
            {
                mouseEndPos.y = mouseStartPos.y;
            }
            else if (direction == Vector2.up)
            {
                mouseEndPos.x = mouseStartPos.x;
            }
        }

        float highlightWidth = mouseEndPos.x - mouseStartPos.x;
        float highlightHeight = mouseEndPos.y - mouseStartPos.y;

        if (highlightWidth < 0 && highlightHeight >= 0) //Top left of mouseStart
        {
            highlightArea.transform.position = new Vector2(mouseEndPos.x, mouseStartPos.y);
            highlightArea.transform.localScale = new Vector2(Mathf.Abs(highlightWidth) + 1, highlightHeight + 1);
        }
        else if(highlightWidth >= 0 && highlightHeight < 0) //Bottom right of mouseStart
        {
            highlightArea.transform.position = new Vector2(mouseStartPos.x, mouseEndPos.y);
            highlightArea.transform.localScale = new Vector2(highlightWidth + 1, Mathf.Abs(highlightHeight) + 1);
        }
        else if (highlightWidth < 0 && highlightHeight < 0) //Bottom left of mouseStart
        {
            highlightArea.transform.position = new Vector2(mouseEndPos.x, mouseEndPos.y);
            highlightArea.transform.localScale = new Vector2(Mathf.Abs(highlightWidth) + 1, Mathf.Abs(highlightHeight) + 1);
        }
        else
        {
            highlightArea.transform.position = mouseStartPos; //Top right of mouseStart
            highlightArea.transform.localScale = new Vector2(highlightWidth + 1, highlightHeight + 1);
        }

        if (highlightArea.transform.localScale.x == 0)
            highlightArea.transform.localScale += Vector3.right;
        if (highlightArea.transform.localScale.y == 0)
            highlightArea.transform.localScale += Vector3.up;
        
    }
    private void EndDraw(bool succesful = true)
    {
        drawing = false;
        highlightEnabled = true;
        Destroy(highlightArea);
        if (succesful)
        {
            if (!spriteManager.DeleteMode)
            {
                if(spriteManager.GetActiveSprite().type != TileManager.TileType.Wall)
                    tileManager.SetTileSpriteArea(mouseStartPos, mouseEndPos, spriteManager.GetActiveSprite());
                else
                    tileManager.SetTileSpriteArea(mouseStartPos, mouseEndPos, spriteManager.GetActiveSprite());
            }
            else
                tileManager.DeleteTileArea(mouseStartPos, mouseEndPos);
        }
    }
    #endregion
}
