using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    public TextMeshProUGUI coinsText;  // Ссылка на UI элемент для отображения количества монет
    public TextMeshProUGUI progressText;  // Ссылка на UI элемент для отображения процента уничтоженных блоков

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

    public void UpdateUI()
    {
        UpdateCoins();
        UpdateProgress();
    }

    // Метод для обновления количества монет
    public void UpdateCoins()
    {
        coinsText.text = "Coins: " + GameManager.Instance.player.totalCoins;
    }

    // Метод для обновления процента уничтоженных блоков
    public void UpdateProgress()
    {
        // Обновляем прогресс в UI
        float progress = (float)(LevelManager.Instance.totalBlocks - LevelManager.Instance.remainingBlocks) / LevelManager.Instance.totalBlocks * 100;

        progressText.text = "Progress: " + progress.ToString("F2") + "%";
    }
}
