using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteManager : MonoBehaviour {

    public SpriteData[] sprites;
    public Sprite[] uiSprites;
    public Dictionary<Vector2, GameObject> overlayObjects = new Dictionary<Vector2, GameObject>();
    bool deleteMode = false;
    SpriteData activeSprite;
    float alpha = 0.5f;

    public bool DeleteMode
    {
        get
        {
            return deleteMode;
        }

        set
        {
            deleteMode = value;
        }
    }

    void Start()
    {
        SetActiveSprite(GetSpriteDataFromName("Concrete"));
    }

    public void SetActiveSprite(string spriteName)
    {
        activeSprite = GetSpriteDataFromName(spriteName);
    }

    public void SetActiveSprite(SpriteData _activeSprite)
    {
        activeSprite = _activeSprite;
    }

    public SpriteData GetActiveSprite()
    {
        return activeSprite;
    }

    public SpriteData GetSpriteDataFromName(string name)
    {
        foreach (SpriteData data in sprites)
        {
            if (data.spriteName == name)
                return data;
        }
        Debug.LogError("No sprite with name: " + name);
        return null;
    }

    public SpriteData GetSpriteDataFromSprite(Sprite sprite)
    {
        foreach (SpriteData data in sprites)
        {
            if (data.sprite == sprite)
                return data;
        }
        Debug.LogError("No sprite with sprite: " + sprite.name);
        return null;
    }

    public Sprite GetUISprite(string name)
    {
        foreach (Sprite sprite in uiSprites)
        {
            if (sprite.name == name)
                return sprite;
        }
        Debug.LogError("No sprite with name: " + name);
        return null;
    }

    public void SetOverlaySprite(Vector2 position, Sprite sprite)
    {
        GameObject overlay = new GameObject();
        overlay.transform.parent = transform;
        overlay.name = "overlay";
        overlay.transform.position = position;
        SpriteRenderer sr = overlay.AddComponent<SpriteRenderer>();
        sr.sprite = sprite;
        sr.sortingLayerName = "Overlay";
        Color temp = sr.color;
        temp.a = alpha;
        sr.color = temp;
        overlayObjects.Add(position, overlay);
    }

    public void RemoveOverlaySprite(Vector2 position)
    {
        GameObject overlay;
        if (overlayObjects.TryGetValue(position, out overlay))
        {
            Destroy(overlay);
            overlayObjects.Remove(position);
        }
    }
}

[System.Serializable]
public class SpriteData
{
    public string spriteName;
    public Sprite sprite;
    public TileManager.TileType type;
    [Range (0f, 1f)]
    public float walkPenalty;
    public int buildTime;
}
