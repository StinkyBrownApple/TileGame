using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuilderManager : MonoBehaviour
{

    [SerializeField]
    AIManager aiManager;
    [SerializeField]
    GameObject builderGO;

    Rect restArea = new Rect(new Vector2(2, 2), new Vector2(5, 5));
    Queue<Builder> availableBuilders = new Queue<Builder>();
    List<Builder> workingBuilders = new List<Builder>();
    Queue<JobData> jobQueue = new Queue<JobData>();

    void Start()
    {
        for (int i = 0; i < 1; i++)
        {
            AddBuilder(new Vector2(UnityEngine.Random.Range(restArea.x, restArea.x + restArea.width), UnityEngine.Random.Range(restArea.y, restArea.y + restArea.height)));
        }
    }

    public void AddBuilder(Vector2 spawnPoint)
    {
        GameObject newBuilder = Instantiate(builderGO, spawnPoint, Quaternion.identity, transform);
        Builder builder = newBuilder.GetComponent<Builder>();
        availableBuilders.Enqueue(builder);
    }

    public void CreateJob(JobData job)
    {
        jobQueue.Enqueue(job);
        DoNextJob();
    }

    private void DoNextJob()
    {
        if(availableBuilders.Count > 0 && jobQueue.Count > 0)
        {
            Builder currentBuilder = availableBuilders.Dequeue();
            workingBuilders.Add(currentBuilder);
            if (currentBuilder.ReturningToBase)
                currentBuilder.ReturningToBase = false;
            currentBuilder.Build(aiManager, jobQueue.Dequeue(), JobFinished);
        }
    }

    private void JobFinished(Builder availableBuilder, JobData jobData)
    {
        availableBuilders.Enqueue(availableBuilder);
        workingBuilders.Remove(availableBuilder);
        jobData.callback(jobData.buildLocation, jobData.buildSprite);
        if (jobQueue.Count > 0)
            DoNextJob();
        else
            ReturnToRestArea(availableBuilder);
    }

    private void ReturnToRestArea(Builder builder)
    {
        builder.ReturnToBase(restArea);
    }
}

public struct JobData
{
    public Vector2 buildLocation;
    public Sprite buildSprite;
    public int buildTime;
    public Action<Vector2, Sprite> callback;

    public JobData(Vector2 _buildLocation, Sprite _buildSprite, int _buildTime, Action<Vector2, Sprite> _callback)
    {
        buildLocation = _buildLocation;
        buildSprite = _buildSprite;
        buildTime = _buildTime;
        callback = _callback;
    }
}

