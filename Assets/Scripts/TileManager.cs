using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileManager : MonoBehaviour
{

    private Dictionary<Vector2, TileData> tileDictionary = new Dictionary<Vector2, TileData>();
    private float defaultWalkPenalty = 0.8f;
    [SerializeField]
    GameObject tileDefault;
    [SerializeField]
    GameObject parent;
    [SerializeField]
    AIManager aiManager;
    [SerializeField]
    SpriteManager spriteManager;
    [SerializeField]
    BuilderManager builderManager;

    public enum TileType
    {
        Empty, Foundation, Object, Wall
    }

    public void SetTileSprite(Vector2 coords, SpriteData sprite)
    {
        TileData tile;
        if (tileDictionary.TryGetValue(coords, out tile))
        {
            SpriteRenderer sr = tile.GetComponent<SpriteRenderer>();
            sr.sprite = sprite.sprite;
            UpdateTile(tile, sprite);
        }
        else
        {
            tile = CreateTile(coords, sprite.walkPenalty, sprite.type);
            SpriteRenderer sr = tile.GetComponent<SpriteRenderer>();
            sr.sprite = sprite.sprite;
            UpdateTile(tile, sprite);
        }
            aiManager.UpdateNode(coords);
    }

    public void SetTileSpriteArea(Vector2 startPos, Vector2 endPos, SpriteData sprite)
    {
        Vector2 start;
        Vector2 end;

        if (startPos.x - endPos.x > 0)
        {
            start.x = endPos.x;
            end.x = startPos.x;
        }
        else
        {
            start.x = startPos.x;
            end.x = endPos.x;
        }

        if (startPos.y - endPos.y > 0)
        {
            start.y = endPos.y;
            end.y = startPos.y;
        }
        else
        {
            start.y = startPos.y;
            end.y = endPos.y;
        }

        for (int x = (int)start.x; x <= end.x; x++)
        {
            for (int y = (int)start.y; y <= end.y; y++)
            {
                TileData tile;
                if (tileDictionary.TryGetValue(new Vector2(x, y), out tile))
                {
                    if (TileTypeParent(sprite.type) == tile.Type)
                    {
                        spriteManager.SetOverlaySprite(new Vector2(x, y), sprite.sprite);
                        JobData jobData = new JobData(new Vector2(x, y), sprite.sprite, sprite.buildTime, SpriteJobComplete);
                        builderManager.CreateJob(jobData);
                    }
                }
                else
                {
                    if (TileTypeParent(sprite.type) == TileType.Empty)
                    {
                        spriteManager.SetOverlaySprite(new Vector2(x, y), sprite.sprite);
                        JobData jobData = new JobData(new Vector2(x, y), sprite.sprite, sprite.buildTime, SpriteJobComplete);
                        builderManager.CreateJob(jobData);
                    }
                }

            }
        }
    }

    private void SpriteJobComplete(Vector2 position, Sprite sprite)
    {
        spriteManager.RemoveOverlaySprite(position);
        SetTileSprite(position, spriteManager.GetSpriteDataFromSprite(sprite));
    }

    private void DeleteJobComplete(Vector2 position, Sprite sprite)
    {
        DeleteTile(position);
    }

    private TileData CreateTile(Vector2 coords, float walkPenalty, TileType type)
    {
        GameObject tileGO = Instantiate(tileDefault, coords, Quaternion.identity, parent.transform);
        TileData tile = tileGO.GetComponent<TileData>();
        tile.WalkPenalty = walkPenalty;
        tile.Type = type;
        tileDictionary.Add(coords, tile);
        return tile;
    }

    public void DeleteTile(Vector2 coords)
    {
        TileData tile;
        if (tileDictionary.TryGetValue(coords, out tile))
        {
            if (TileTypeParent(tile.GetComponent<TileData>().Type) == TileType.Empty)
            {
                Destroy(tile.gameObject);
                tileDictionary.Remove(coords);
            }
            else
            {
                SetTileSprite(tile.transform.position, spriteManager.GetSpriteDataFromName("Concrete"));
            }
        }

        aiManager.UpdateNode(coords);
    }

    public void DeleteTileArea(Vector2 startPos, Vector2 endPos)
    {
        Vector2 start;
        Vector2 end;

        if (startPos.x - endPos.x > 0)
        {
            start.x = endPos.x;
            end.x = startPos.x;
        }
        else
        {
            start.x = startPos.x;
            end.x = endPos.x;
        }

        if (startPos.y - endPos.y > 0)
        {
            start.y = endPos.y;
            end.y = startPos.y;
        }
        else
        {
            start.y = startPos.y;
            end.y = endPos.y;
        }

        for (int x = (int)start.x; x <= end.x; x++)
        {
            for (int y = (int)start.y; y <= end.y; y++)
            {
                TileData temp;
                if (tileDictionary.TryGetValue(new Vector2(x, y), out temp))
                {
                    JobData jobData = new JobData(new Vector2(x, y), null, 1, DeleteJobComplete);
                    builderManager.CreateJob(jobData);
                }
            }
        }
    }

    public float GetTileWalkPenalty(Vector2 coords)
    {
        TileData tile;
        if (tileDictionary.TryGetValue(coords, out tile))
        {
            return tile.WalkPenalty;
        }
        return defaultWalkPenalty;
    }

    private TileType TileTypeParent(TileType type)
    {
        switch (type)
        {
            case TileType.Foundation:
                return TileType.Empty;
            case TileType.Object:
                return TileType.Foundation;
            case TileType.Wall:
                return TileType.Foundation;
            default:
                return TileType.Empty;
        }

    }

    private void UpdateTile(TileData tile, SpriteData data)
    {
        tile.WalkPenalty = data.walkPenalty;
        tile.Type = data.type;
    }


}
