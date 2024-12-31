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

    public AudioManager audioManager;

    public int cameraPosition = 0;

    [Header("Audio Clips")]
    public AudioClip winningSound;
    public AudioClip losingSound;

    private bool isRewardedSubscribed = false;
    private bool rewardClaimed = false;

    public bool IsTutorialPassed = false;

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

        //ResetData();
        //ResetData();
        //SaveData();
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
        if (!isRewardedSubscribed)
        {
            Debug.Log("Подписка на событие RewardVideoEvent");
            YandexGame.RewardVideoEvent += Rewarded;
            isRewardedSubscribed = true;
        }
    }

    private void OnDisable()
    {
        if (isRewardedSubscribed)
        {
            Debug.Log("Отписка от события RewardVideoEvent");
            YandexGame.RewardVideoEvent -= Rewarded;
            isRewardedSubscribed = false;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        LoadData();

        UIManager.Instance.ShowLevelsPanel();
        SpawnManager.Instance.ColorPurchasedBlocksOnStart();

        if (!IsTutorialPassed)
        {
            TutorialManager.Instance.StartTutorial();
        }
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
        player.IsLevelEnding = true;

        Debug.Log("Level Ended");
        LevelManager.Instance.EndLevel();
        UIManager.Instance.ShowResults();

        GameManager.Instance.player.AmountOfRewardedHP = 0;

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
        UIManager.Instance.LoadData();
        SpawnManager.Instance.LoadData();

        IsTutorialPassed = YandexGame.savesData.IsTutorialPassed;

    }

    private void SaveData()
    {
        player.SaveData();
        LevelManager.Instance.SaveData();
        UIManager.Instance.SaveData();
        SpawnManager.Instance.SaveData();

        YandexGame.SaveProgress();
    }

    // Сохранение данных при выходе из игры
    private void OnApplicationQuit()
    {
        SaveData();
    }

    // Подписанный метод получения награды

    void Rewarded(int id)
    {
        if (rewardClaimed)
        {
            Debug.LogWarning("Reward already claimed. Ignoring duplicate call.");
            return;
        }

        rewardClaimed = true;

        Debug.Log($"Rewarded event triggered. ID: {id}");

        if (id == 1)
        {
            ResourceAnimator.Instance.AnimateResourceChange
                (player.totalCoins, player.totalCoins + rewardCoinsAd, true);
            player.AddCoins(rewardCoinsAd);
        }
        else if (id == 2)
        {
            ResourceAnimator.Instance.AnimateResourceChange
                (player.totalDiamonds, player.totalDiamonds + rewardDiamondsAd, false);
            player.AddDiamonds(rewardDiamondsAd);
        }
        else if (id == 3)
        {
            player.AmountOfRewardedHP++;
        }

        // Через какое-то время сбрасываем флаг
        StartCoroutine(ResetRewardFlag());
    }

    private IEnumerator ResetRewardFlag()
    {
        yield return new WaitForSeconds(0.1f);  // или любое другое время
        rewardClaimed = false;
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

    public void PlaySoundOnce(AudioClip clip)
    {
        audioManager.PlaySoundOnce(clip);
    }

    public void ResetData()
    {
        YandexGame.ResetSaveProgress();
        LoadData();
        //player.ResetData();
        //LevelManager.Instance.ResetData();
        //UIManager.Instance.ResetData();
        //SpawnManager.Instance.ResetData();

        //IsTutorialPassed = false;
        //YandexGame.savesData.IsTutorialPassed = false;

        //YandexGame.SaveProgress();
    }
}
