using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : IHeapItem<Node>{

    public Node parent;

    public bool walkable;
    public Vector2 position;
    public int walkPenalty;

    public int gCost;
    public int hCost;

    public int fCost
    {
        get
        {
            return gCost + hCost;
        }
    }

    int heapIndex;


    public Node(Vector2 _position, float _walkPenalty)
    {
        position = _position;
        walkPenalty = (int)(_walkPenalty * 10);
        walkable = _walkPenalty == 1 ? false : true;
    }

    public void UpdateNode(float _walkPenalty)
    {
        walkPenalty = (int)(_walkPenalty * 10);
        walkable = _walkPenalty == 1 ? false : true;
    }

    public int HeapIndex
    {
        get
        {
            return heapIndex;
        }

        set
        {
            heapIndex = value;
        }
    }
    public int CompareTo(Node other)
    {
        int compare = fCost.CompareTo(other.fCost);
        if(compare == 0)
        {
            compare = hCost.CompareTo(other.hCost);
        }
        return -compare;
    }
}
