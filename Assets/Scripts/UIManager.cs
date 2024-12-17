using DG.Tweening;  // Добавляем DOTween
using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    public TextMeshProUGUI coinsText;  // Ссылка на UI элемент для отображения количества монет
    public TextMeshProUGUI progressText;  // Ссылка на UI элемент для отображения процента уничтоженных блоков
    public GameObject resultsPanel;  // Панель с результатами
    public TextMeshProUGUI resultText;  // Текст с результатом

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

        resultsPanel.SetActive(false);  // Панель скрыта по умолчанию
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

        progressText.text = "Progress: " + progress.ToString("F0") + "%";
    }

    // Показываем результаты с анимацией
    public void ShowResults()
    {
        // Показываем панель результатов
        resultsPanel.SetActive(true);

        // Настроим анимацию для панели с результатами (например, плавное появление)
        resultsPanel.transform.localScale = Vector3.zero;  // Начальное состояние (панель скрыта)
        resultsPanel.transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack);  // Анимация появления

        // Устанавливаем текст результата
        resultText.text = "Level Completed!\nCoins: " + GameManager.Instance.player.totalCoins;

        // Анимируем появление текста
        resultText.DOFade(1f, 0.5f).SetEase(Ease.OutSine);  // Используем DOFade для TextMeshProUGUI
    }


    // Закрытие панели результатов с анимацией
    public void HideResults()
    {
        resultsPanel.transform.DOScale(Vector3.zero, 0.5f).SetEase(Ease.InBack).OnComplete(() => resultsPanel.SetActive(false));
    }
}
