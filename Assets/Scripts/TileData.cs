using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileData : MonoBehaviour{

    TileManager.TileType type;
    float walkPenalty;

    public TileData(float _walkPenalty, TileManager.TileType _type)
    {
        type = _type;
        walkPenalty = _walkPenalty;
    }

    public TileManager.TileType Type
    {
        get
        {
            return type;
        }

        set
        {
            type = value;
        }
    }

    public float WalkPenalty
    {
        get
        {
            return walkPenalty;
        }

        set
        {
            walkPenalty = value;
        }
    }
}
