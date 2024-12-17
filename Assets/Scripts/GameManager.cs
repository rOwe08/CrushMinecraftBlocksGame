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

        StartLevel();
    }

    public void StartLevel()
    {
        LevelManager.Instance.StartLevel();
        UIManager.Instance.UpdateUI();
    }

    public void AddCoins(int coins)
    {
        player.AddCoins(coins);
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
