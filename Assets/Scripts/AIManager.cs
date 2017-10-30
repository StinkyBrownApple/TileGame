using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class AIManager : MonoBehaviour
{

    Node[,] grid;
    Vector2 gridSize;
    Queue<PathRequest> pathRequestQueue = new Queue<PathRequest>();
    PathRequest currentPathRequest;
    bool processingPath;

    [SerializeField]
    TileManager tileManager;

    void Start()
    {
        gridSize = Settings.mapSize;
        GenerateNodeGrid(new Vector2(200, 200));
    }

    public void RequestPath(Vector2 start, Vector2 end, Action<Vector2[], bool> callback)
    {
        PathRequest newRequest = new PathRequest(start, end, callback);
        pathRequestQueue.Enqueue(newRequest);
        ProcessNextRequest();
    }

    struct PathRequest
    {
        public Vector2 start;
        public Vector2 end;
        public Action<Vector2[], bool> callback;

        public PathRequest(Vector2 _start, Vector2 _end, Action<Vector2[], bool> _callback)
        {
            start = _start;
            end = _end;
            callback = _callback;
        }
    }

    private void ProcessNextRequest()
    {
        if (!processingPath && pathRequestQueue.Count > 0)
        {
            currentPathRequest = pathRequestQueue.Dequeue();
            processingPath = true;
            BeginPathFind();
        }
    }

    private void BeginPathFind()
    {
        StartCoroutine(FindPath(currentPathRequest.start, currentPathRequest.end));
    }

    private void EndPathFind(Vector2[] route, bool success)
    {
        processingPath = false;
        currentPathRequest.callback(route, success);
        ProcessNextRequest();
    }


    IEnumerator FindPath(Vector2 _start, Vector2 _end)
    {
        Vector2[] route = new Vector2[0];
        bool success = false;

        Node start = GetNodeFromPos(_start);
        Node end = GetNodeFromPos(_end);

        
        
            Heap<Node> openSet = new Heap<Node>((int)gridSize.x * (int)gridSize.y);
            HashSet<Node> closedSet = new HashSet<Node>();
            openSet.Add(start);

            while (openSet.Count > 0)
            {
                Node currentNode = openSet.RemoveFirst();
                closedSet.Add(currentNode);
                List<Node> neighbours = GetNeighours(currentNode);
                if (currentNode == end || (!end.walkable && neighbours.Contains(end)))
                {
                    end = currentNode;
                    success = true;
                    break;
                }

                foreach (Node neighbour in neighbours)
                {
                    if (!neighbour.walkable || closedSet.Contains(neighbour))
                        continue;

                    int tempGCost = currentNode.gCost + GetDistance(currentNode, neighbour) + neighbour.walkPenalty;
                    if (tempGCost < neighbour.gCost || !openSet.Contains(neighbour))
                    {
                        neighbour.gCost = tempGCost;
                        neighbour.hCost = GetDistance(neighbour, end);
                        neighbour.parent = currentNode;

                        if (!openSet.Contains(neighbour))
                            openSet.Add(neighbour);
                        else
                            openSet.UpdateItem(neighbour);
                    }
                }
            }
        
        yield return null;
        if (success)
            route = GetPath(start, end).ToArray();
        EndPathFind(route, success);
    }



    private List<Node> GetNeighours(Node home)
    {
        List<Node> neighbours = new List<Node>();
        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if ((x == 0 && y == 0))
                    continue;
                Vector2 neighborPos = home.position + new Vector2(x, y);
                if (neighborPos.x < 0 || neighborPos.y < 0 || neighborPos.x > gridSize.x - 1 || neighborPos.y > gridSize.y - 1)
                    continue;
                neighbours.Add(grid[(int)neighborPos.x, (int)neighborPos.y]);
            }
        }
        return neighbours;
    }

    private int GetDistance(Node nodeA, Node nodeB)
    {
        int distanceX = Mathf.Abs((int)nodeA.position.x - (int)nodeB.position.x);
        int distanceY = Mathf.Abs((int)nodeA.position.y - (int)nodeB.position.y);

        if (distanceX > distanceY)
            return (14 * distanceY + 10 * (distanceX - distanceY));
        else
            return (14 * distanceX + 10 * (distanceY - distanceX));
    }

    private List<Vector2> GetPath(Node startNode, Node lastNode)
    {
        List<Vector2> pathPoints = new List<Vector2>();
        Node currentNode = lastNode;
        pathPoints.Add(currentNode.position);
        while (currentNode != startNode)
        {
            pathPoints.Add(currentNode.parent.position);
            currentNode = currentNode.parent;
        }
        pathPoints = SimplifyPath(pathPoints);
        pathPoints.Reverse();
        return pathPoints;
    }

    private List<Vector2> SimplifyPath(List<Vector2> path)
    {
        List<Vector2> waypoints = new List<Vector2>();
        Vector2 directionOld = Vector2.zero;
        waypoints.Add(path[0]);
        for (int i = 1; i < path.Count; i++)
        {
            Vector2 directionNew = new Vector2(path[i - 1].x - path[i].x, path[i - 1].y - path[i].y);
            if (directionOld != directionNew)
                waypoints.Add(path[i]);
            directionOld = directionNew;
        }
        return waypoints;
    }

    public void GenerateNodeGrid(Vector2 size)
    {
        grid = new Node[(int)size.x, (int)size.y];
        gridSize = size;
        for (int x = 0; x < size.x; x++)
        {
            for (int y = 0; y < size.y; y++)
            {
                grid[x, y] = new Node(new Vector2(x, y), tileManager.GetTileWalkPenalty(new Vector2(x, y)));
            }
        }
    }

    private Node GetNodeFromPos(Vector2 position)
    {
        return grid[(int)position.x, (int)position.y];
    }

    public void UpdateNode(Vector2 position)
    {
        grid[(int)position.x, (int)position.y].UpdateNode(tileManager.GetTileWalkPenalty(position));
    }
}
