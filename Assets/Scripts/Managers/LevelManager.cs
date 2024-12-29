using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using YG;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }

    public List<GameObject> spawnedBlocks = new List<GameObject>();

    public int levelsAmount = 50;
    public int level = 0;

    public int coinsEarnedOnLevel = 0;

    public int totalBlocks = 0;
    public int remainingBlocks = 0;

    public int attempts = 0;
    public int maxAttempts = 3;

    public bool isLevelActive = false;

    private bool allProjectilesShot = false; // Флаг для проверки, что все ядра выпущены
    public bool ArmageddonEnded = true;
    public bool AreMovingBlocks = false;
    public bool IsProjectileInScene = false; // Флаг для проверки, что все ядра выпущены

    public int maxLevelCompleted = 0;
    public List<float> levelList = new List<float>();

    public UnityEvent<bool> OnLevelStarted;
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

        //ResetData();
        //SaveData();
    }

    private void Update()
    {
        foreach (GameObject block in spawnedBlocks)
        {
            if (block != null)
            {
                if (block.GetComponent<Rigidbody>().velocity.magnitude > 0f)
                {
                    AreMovingBlocks = true;
                    break;
                }
                else
                {
                    AreMovingBlocks = false;
                }
            }
            else
            {
                AreMovingBlocks = false;
            }
        }

        // Проверяем, если уровень активен и все блоки разрушены
        if (isLevelActive && (remainingBlocks <= 0 || (attempts == maxAttempts && !IsProjectileInScene && !AreMovingBlocks)))
        {
            // Если попытки использованы и все снаряды выпущены
            if (ArmageddonEnded && allProjectilesShot && !GameManager.Instance.player.IsLevelEnding)
            {
                GameManager.Instance.EndLevel();
                Debug.Log("Level completed!"); 
            } 
        }
    }

    public void OnAllProjectilesShot()
    {
        allProjectilesShot = true;
    }

    public void StartLevel(int levelToStart = 0)
    {
        DespawnAllBuildings();

        attempts = 0;
        coinsEarnedOnLevel = 0;
        totalBlocks = 0;
        remainingBlocks = 0;
        ArmageddonEnded = true;

        if (GameManager.Instance.player.armageddonLevel > 0)
        {
            GameManager.Instance.player.IsArmageddonActivated = false;
        }
        else
        {
            GameManager.Instance.player.IsArmageddonActivated = true;
        }

        GameManager.Instance.player.IsLevelEnding = false;
        allProjectilesShot = false; // Сбрасываем флаг

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

        // Вычисляем центр карты
        float centerX = SpawnManager.Instance.groundSizeX / 2f;
        float centerZ = SpawnManager.Instance.groundSizeY / 2f;
        Vector3 startPosition = new Vector3(centerX, 1, centerZ);  // Центр карты


        // В зависимости от уровня вызываем разные функции для спавна
        if (level <= 10)
        {
            // Простой домик для первых 10 уровней
            SpawnManager.Instance.SpawnWall(startPosition);  
        }
        else if (level <= 20)
        {
            // Спавним башню для уровней с 11 по 20
            SpawnManager.Instance.SpawnHouse(startPosition);  // Спавн башни
        }
        else if (level <= 30)
        {
            // Спавним что-то более сложное, например, крепость для уровней с 21 по 30
            SpawnManager.Instance.SpawnFortifiedBuilding(startPosition);  // Спавн крепости
        }
        else if (level <= 35)
        {
            // Спавним замок для уровней с 31 по 40
            SpawnManager.Instance.SpawnFortress(startPosition, false);  // Спавн замка
        }
        else if (level<= 40)
        {
            // Спавним замок для уровней с 40 по 45
            SpawnManager.Instance.SpawnFortress(startPosition, true);  // Спавн замка
        }
        else if(level <= 45)
        {
            SpawnManager.Instance.SpawnCastle(startPosition, false);  // Спавн замка
        }
        else if (level <= 50)
        {
            SpawnManager.Instance.SpawnCastle(startPosition, true);  // Спавн замка
        }

        isLevelActive = true;
        UIManager.Instance.HideResults();
        OnLevelStarted.Invoke(isLevelActive);
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

        OnLevelStarted.Invoke(isLevelActive);

        if (progress >= 50f)
        {
            GameManager.Instance.PlaySoundOnce(GameManager.Instance.winningSound);
        }
        else
        {
            GameManager.Instance.PlaySoundOnce(GameManager.Instance.losingSound);
        }

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

    public void AddAttempts(int amount)
    {
        maxAttempts += amount;
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

    public void SaveData()
    {
        YandexGame.savesData.maxLevelCompleted = maxLevelCompleted;
        YandexGame.savesData.levelList = levelList; // Преобразуем List в массив для сохранения
        YandexGame.savesData.maxAttempts = maxAttempts;
    }

    public void LoadData()
    {
        maxLevelCompleted = YandexGame.savesData.maxLevelCompleted;

        if (YandexGame.savesData.maxAttempts < 3)
        {
            YandexGame.savesData.maxAttempts = 3;
            maxAttempts = 3;

            YandexGame.SaveProgress();
        }
        else
        {
            maxAttempts = YandexGame.savesData.maxAttempts;
        }

        if (YandexGame.savesData.levelList.Count <= 0)
        {
            for(int i = 0; i < 50; i++)
            {
                levelList.Add(0);
            }
        }
        else
        {
            // Загружаем массив уровней обратно в List
            levelList = new List<float>(YandexGame.savesData.levelList);
        }
    }

    public void ResetData()
    {
        maxLevelCompleted = 0;
        levelList = new List<float>();
        maxAttempts = 3;
    }
}
