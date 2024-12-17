using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }

    public int levelsAmount = 50;
    public int maxLevelCompleted = 0;
    public int level = 0;

    public int totalBlocks = 0;
    public int remainingBlocks = 0;

    public int attempts = 0;

    public bool isLevelActive = false;

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

    public void EndLevel()
    {
        isLevelActive = false;

        if (level == maxLevelCompleted + 1)
        {
            maxLevelCompleted++;
        }
    }

    public void AddBlock()
    {
        remainingBlocks++;
        totalBlocks++;
    }

    public void RemoveBlock()
    {
        remainingBlocks--;
    }
}
