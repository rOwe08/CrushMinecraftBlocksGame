using System;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }

    public List<GameObject> spawnedBlocks = new List<GameObject>();

    public int levelsAmount = 50;
    public int maxLevelCompleted = 0;
    public int level = 0;

    public int coinsEarnedOnLevel = 0;

    public int totalBlocks = 0;
    public int remainingBlocks = 0;

    public int attempts = 0;
    public int maxAttempts = 3;

    public bool isLevelActive = false;

    public List<float> levelList = new List<float>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }

        for (int i = 0; i < levelsAmount; i++) 
        {
            levelList.Add(0);
        }
    }

    private void Update()
    {
        if (isLevelActive && remainingBlocks <= 0)
        {
            GameManager.Instance.EndLevel();
            Debug.Log("Level completed!");
        }
    }

    public void StartLevel(int levelToStart = 0)
    {
        DespawnAllBuildings();

        attempts = 0;
        coinsEarnedOnLevel = 0;
        totalBlocks = 0;
        remainingBlocks = 0;

        if (levelToStart == 0)
        {
            if (level < maxLevelCompleted + 2)
            {
                level++;
            }
        }
        else
        {
            level = levelToStart;
        }

        SpawnManager.Instance.SpawnBuilding(5, 5);
        isLevelActive = true;
        UIManager.Instance.HideResults();
    }

    private void DespawnAllBuildings()
    {
        foreach(GameObject block in spawnedBlocks)
        {
            Destroy(block);
        }

        spawnedBlocks.Clear();
    }

    public void EndLevel()
    {
        isLevelActive = false;
        float progress = (float)(totalBlocks - remainingBlocks) / totalBlocks * 100;

        if (level == maxLevelCompleted + 1 && progress >= 50f)
        {
            maxLevelCompleted++;
        }

        float prevResult = levelList[level - 1];

        if (prevResult < progress)
        {
            levelList[level - 1] = progress;
        }
    }

    public void AddBlock(GameObject block)
    {
        remainingBlocks++;
        totalBlocks++;
        spawnedBlocks.Add(block);
    }

    public void RemoveBlock()
    {
        remainingBlocks--;
    }

    internal void AddAttempt()
    {
        attempts++;
    }
}
