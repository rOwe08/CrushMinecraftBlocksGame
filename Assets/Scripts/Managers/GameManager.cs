using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    public int CurrentLevel = 1;

    public Player player;

    public static GameManager Instance { get; private set; }

    void Awake()
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

    // Start is called before the first frame update
    void Start()
    {
        LoadData();

        UIManager.Instance.ShowLevelsPanel();
    }

    public void StartLevel(int level = 0)
    {
        LevelManager.Instance.StartLevel(level);
        UIManager.Instance.SetUpUI();
        UIManager.Instance.UpdateUI();
    }

    [ContextMenu("EndLevel")]
    public void EndLevel()
    {
        LevelManager.Instance.EndLevel();
        UIManager.Instance.ShowResults();
    }

    public void AddCoins(int coins)
    {
        player.AddCoins(coins);
        LevelManager.Instance.coinsEarnedOnLevel += coins;
    }

    public void AddDiamonds(int amount)
    {
        player.AddDiamonds(amount);
    }
    private void LoadData()
    {

    }

    private void SaveData()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
