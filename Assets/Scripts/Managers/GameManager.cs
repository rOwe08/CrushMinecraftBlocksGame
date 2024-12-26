using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using YG;

public class GameManager : MonoBehaviour
{
    public int CurrentLevel = 1;

    public Player player;

    public static GameManager Instance { get; private set; }
    public int rewardCoinsAd;
    public int rewardDiamondsAd;

    public GameObject chosenProjectileInShop;

    public AudioClip destructionSound; // Звук разрушения блока
    public AudioClip hitSound; // Звук разрушения блока

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
        if(level == LevelManager.Instance.levelsAmount)
        {
            level--;
        }

        LevelManager.Instance.StartLevel(level);
        UIManager.Instance.SetUpUI();
        UIManager.Instance.UpdateUI();
    }

    [ContextMenu("EndLevel")]
    public void EndLevel()
    {
        player.IsLevelEnding = true;

        Debug.Log("Level Ended");
        LevelManager.Instance.EndLevel();
        UIManager.Instance.ShowResults();

        SaveData();
        AddNewLeaderboard();
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
        player.LoadData();
        LevelManager.Instance.LoadData();
    }

    private void SaveData()
    {
        player.SaveData();
        LevelManager.Instance.SaveData();
    }

    // Сохранение данных при выходе из игры
    private void OnApplicationQuit()
    {
        SaveData();
    }

    // Сохранение данных при паузе (сворачивании приложения или на мобильных устройствах)
    private void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus)
        {
            SaveData();
        }
    }

    private void OnEnable()
    {
        YandexGame.RewardVideoEvent += Rewarded;
    }
    // Отписываемся от события открытия рекламы в OnDisable
    private void OnDisable()
    {
        YandexGame.RewardVideoEvent -= Rewarded;
    }

    // Подписанный метод получения награды
    void Rewarded(int id)
    {
        // Если ID = 1, то выдаём "+100 монет"
        if (id == 1)
            player.AddCoins(rewardCoinsAd);
        // Если ID = 2, то выдаём "+оружие".
        else if (id == 2)
            player.AddDiamonds(rewardDiamondsAd);
        // Если ID = 2, то выдаём "+оружие".
        //else if (id == 2)
        //    AddWeapon();
    }

    // Метод для вызова видео рекламы
    public void WatchRewardedAd(int id)
    {
        // Вызываем метод открытия видео рекламы
        YandexGame.RewVideoShow(id);
    }

    void AddNewLeaderboard()
    {
        int count = 0;

        foreach (float progress in LevelManager.Instance.levelList) 
        {
            if(progress >= 100)
            {
                count++;
            }
        }

        YandexGame.NewLeaderboardScores("mainLeaderboard", count);
    }
}
