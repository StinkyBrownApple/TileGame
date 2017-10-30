using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Builder : MonoBehaviour {

    AIManager aiManager;
    Action<Builder, JobData> callback;
    JobData jobData;
    bool returningToBase;

    public bool ReturningToBase
    {
        get
        {
            return returningToBase;
        }
        set
        {
            returningToBase = value;
        }
    }

    public void Build(AIManager _aiManager, JobData _jobData, Action<Builder, JobData> _callback)
    {
        aiManager = _aiManager;
        callback = _callback;
        jobData = _jobData;
        aiManager.RequestPath(transform.position, jobData.buildLocation, FinishedBuildPathfind);
    }

    private void FinishedBuildPathfind(Vector2[] waypoints, bool success)
    {
        if (success)
        {
            StartCoroutine(MoveToJob(waypoints));
        }
    }

    private void FinishedMovePathfind(Vector2[] waypoints, bool success)
    {
        if (success)
        {
            StartCoroutine(MoveToBase(waypoints));
        }
    }

    private Vector2[] OffsetWaypoints(Vector2[] walkTargets)
    {
        for (int i = 0; i < walkTargets.Length; i++)
        {
            walkTargets[i] += new Vector2(0.5f, 0.5f);
        }
        return walkTargets;
    }

    private IEnumerator MoveToJob(Vector2[] waypoints)
    {
        waypoints = OffsetWaypoints(waypoints);
        int currentIndex = 0;
        while (currentIndex < waypoints.Length)
        {
            transform.position = Vector2.MoveTowards(transform.position, waypoints[currentIndex], Time.deltaTime * 20);
            if ((Vector2)transform.position == waypoints[currentIndex])
            {
                currentIndex++;
            }
            yield return null;
        }


        /*
        if(waypoints.Length > 2)
        {
            while (true)
            {
                Vector2 incomingVector = waypoints[currentIndex];
                Vector2 outgoingVector = waypoints[currentIndex + 1] - waypoints[currentIndex];
                Vector2 nodeA = waypoints[currentIndex] - incomingVector.normalized * 2f;
                Vector2 nodeB = waypoints[currentIndex];
                Vector2 nodeC = waypoints[currentIndex] + outgoingVector.normalized * 2f;

                while((Vector2)transform.position != nodeA)
                {
                    transform.position = Vector2.MoveTowards(transform.position, nodeA, Time.deltaTime * 20);
                    yield return null;
                }
                float t = 0;
                while(t < 1)
                {
                    transform.position = Vector2.Lerp(nodeA, Vector2.Lerp(Vector2.Lerp(nodeA, nodeB, t), Vector2.Lerp(nodeB, nodeC, t), t), t);
                    t += Time.deltaTime * 10;
                    yield return null;
                }
                if(waypoints.Length - 1 > currentIndex + 2)
                {
                    currentIndex++;
                    continue;
                }
                else
                {
                    while (currentIndex + 1 < waypoints.Length)
                    {
                        while((Vector2)transform.position != waypoints[currentIndex + 1])
                        {
                            transform.position = Vector2.MoveTowards(transform.position, waypoints[currentIndex + 1], Time.deltaTime * 20);
                            yield return null;
                        }
                        currentIndex++;
                    }
                    break;
                }
            }
        }
        else
        {
            while ((Vector2)transform.position != waypoints[currentIndex])
            {
                transform.position = Vector2.MoveTowards(transform.position, waypoints[currentIndex], Time.deltaTime * 20);
                yield return null;
            }
        }
        */
        StartCoroutine(Build());
    }

    private IEnumerator MoveToBase(Vector2[] waypoints)
    {
        waypoints = OffsetWaypoints(waypoints);
        int currentIndex = 0;
        while (currentIndex < waypoints.Length && returningToBase)
        {
            transform.position = Vector2.MoveTowards(transform.position, waypoints[currentIndex], Time.deltaTime * 20);
            if ((Vector2)transform.position == waypoints[currentIndex])
            {
                currentIndex++;
            }
            yield return null;
        }
        returningToBase = false;
    }

    private IEnumerator Build()
    {
        float jobTime = UnityEngine.Random.Range(jobData.buildTime, jobData.buildTime * 0.75f);
        yield return new WaitForSeconds(jobTime);
        FinishedJob();
    }

    private void FinishedJob()
    {
        callback(this, jobData);
    }

    public void ReturnToBase(Rect baseArea)
    {
        returningToBase = true;
        aiManager.RequestPath(transform.position, new Vector2(UnityEngine.Random.Range(baseArea.x, baseArea.x + baseArea.width), UnityEngine.Random.Range(baseArea.y, baseArea.y + baseArea.height)), FinishedMovePathfind);
    }

}
