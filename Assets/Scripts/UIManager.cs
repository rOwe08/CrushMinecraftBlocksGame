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
    public TextMeshProUGUI levelText;  // Текст с уровнем

    [Header("Levels")]
    public GameObject levelsPanel;  // Панель для уровней
    public GameObject levelButtonPrefab;  // Префаб кнопки уровня
    public Transform levelsContainer;  // Контейнер, куда будут добавляться кнопки уровней

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

        levelsPanel.SetActive(false);  // Панель скрыта по умолчанию
        resultsPanel.SetActive(false);  // Панель скрыта по умолчанию
    }

    private void Update()
    {
        UpdateUI();
    }

    // Функция для появления панели уровней
    public void ShowLevelsPanel()
    {
        levelsPanel.SetActive(true);  // Делаем панель видимой

        // Очищаем контейнер от предыдущих кнопок уровней (если они были)
        foreach (Transform child in levelsContainer)
        {
            Destroy(child.gameObject);
        }

        // Создаем кнопки для каждого уровня
        for (int i = 0; i < LevelManager.Instance.levelsAmount; i++)
        {
            GameObject levelButton = Instantiate(levelButtonPrefab, levelsContainer);
            TextMeshProUGUI levelText = levelButton.GetComponentInChildren<TMPro.TextMeshProUGUI>();
            UnityEngine.UI.Button buttonComponent = levelButton.GetComponentInChildren<UnityEngine.UI.Button>();

            // Устанавливаем текст на кнопке
            levelText.text = "Level " + (i + 1);

            // Присваиваем номер уровня для каждой кнопки
            int levelIndex = i;  // Локальная копия индекса для использования в замыкании

            // Устанавливаем состояние кнопки (нажимаема или нет)
            bool isUnlocked = (i + 1) <= LevelManager.Instance.maxLevelCompleted + 1;  // maxLevelCompleted — максимальный пройденный уровень
            buttonComponent.interactable = isUnlocked;  // Делаем кнопку неактивной, если уровень заблокирован

            // Управляем отображением CloseImage (активен, если уровень заблокирован)
            Transform closeImage = levelButton.transform.Find("CloseImage");
            if (closeImage != null)
            {
                closeImage.gameObject.SetActive(!isUnlocked);  // Активируем картинку, если уровень заблокирован
            }

            // Добавляем слушатель события нажатия кнопки, если уровень разблокирован
            if (isUnlocked)
            {
                buttonComponent.onClick.AddListener(() => OnLevelSelected(levelIndex + 1));
            }
        }

        // Анимация появления панели (если нужно)
        levelsPanel.transform.localScale = Vector3.zero;  // Начальное состояние (панель скрыта)
        levelsPanel.transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack);  // Анимация появления
    }


    // Закрытие панели уровней
    public void HideLevelsPanel()
    {
        levelsPanel.transform.DOScale(Vector3.zero, 0.5f).SetEase(Ease.InBack).OnComplete(() => levelsPanel.SetActive(false));
    }

    // Функция, вызываемая при выборе уровня
    private void OnLevelSelected(int levelIndex)
    {
        Debug.Log("Level " + (levelIndex + 1) + " selected");
        // Здесь можно вызывать функцию для начала выбранного уровня
        LevelManager.Instance.StartLevel(levelIndex);
        HideLevelsPanel();  // Закрываем панель после выбора уровня
    }

    public void UpdateUI()
    {
        UpdateCoins();
        UpdateProgress();
        UpdateLevel();
    }

    // Метод для обновления количества монет
    public void UpdateCoins()
    {
        coinsText.text = "Coins: " + GameManager.Instance.player.totalCoins;
    }

    // Метод для обновления уровня
    public void UpdateLevel()
    {
        levelText.text = "Level: " + LevelManager.Instance.level;
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
    }


    // Закрытие панели результатов с анимацией
    public void HideResults()
    {
        resultsPanel.transform.DOScale(Vector3.zero, 0.5f).SetEase(Ease.InBack).OnComplete(() => resultsPanel.SetActive(false));
    }
}
