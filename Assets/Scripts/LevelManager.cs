using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }

    public int totalBlocks = 0;
    public int remainingBlocks = 0;

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

    public void StartLevel()
    {
        SpawnManager.Instance.SpawnBuilding(new Vector3(10, 0, 10), 5, 5);
    }

    public void AddBlock()
    {
        remainingBlocks++;
        totalBlocks++;
    }

    public void RemoveBlock()
    {
        remainingBlocks--;
        if (remainingBlocks <= 0)
        {
            Debug.Log("Level completed!");
        }
    }
}
