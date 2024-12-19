using DG.Tweening;  // Добавляем DOTween
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using YG;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    private bool _IsUIActive = false;

    [Header("Panels")]
    public GameObject resultsPanel;  // Панель с результатами
    public GameObject shopPanel;  // Панель с результатами

    [Header("Texts")]
    public TextMeshProUGUI coinsText;  // Ссылка на UI элемент для отображения количества монет
    public TextMeshProUGUI diamondsText;  // Ссылка на UI элемент для отображения количества монет

    public TextMeshProUGUI progressText;  // Ссылка на UI элемент для отображения процента уничтоженных блоков
    public TextMeshProUGUI levelText;  // Текст с уровнем
    public TextMeshProUGUI attemptsText; // Текст с попытками
    public TextMeshProUGUI percentageText; // Текст с процентами
    public TextMeshProUGUI coinsEarnedText; // Текст с собранными монетами
    public TextMeshProUGUI attemptsUsedText; // Текст с использованными попытками

    [Header("Buttons")]
    public Button resetButton;

    [Header("Stars")]
    public Image[] stars;  // Массив звёзд
    public Sprite earnedStarSprite;  // Спрайт для полученной звезды
    public Sprite unearnedStarSprite;  // Спрайт для неполученной звезды

    [Header("Badges")]
    public Image badge;
    public Sprite bronzeBadge;
    public Sprite silverBadge;
    public Sprite goldBadge;

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

    public void UpdateUI()
    {
        UpdateCoins();
        UpdateDiamonds();
        UpdateProgress();
        UpdateLevel();
        UpdateAttempts();
    }

    // Функция для появления панели уровней
    public void ShowLevelsPanel()
    {
        levelsPanel.SetActive(true);  // Делаем панель видимой
        _IsUIActive = true;

        // Очищаем контейнер от предыдущих кнопок уровней (если они были)
        foreach (Transform child in levelsContainer)
        {
            Destroy(child.gameObject);
        }

        // Создаем кнопки для каждого уровня
        for (int i = 0; i < LevelManager.Instance.levelsAmount; i++)
        {
            GameObject levelButton = Instantiate(levelButtonPrefab, levelsContainer);
            TextMeshProUGUI levelText = levelButton.GetComponentInChildren<TextMeshProUGUI>();
            Button buttonComponent = levelButton.GetComponentInChildren<Button>();

            // Устанавливаем текст на кнопке
            levelText.text = (i + 1).ToString();

            // Присваиваем номер уровня для каждой кнопки
            int levelIndex = i;  // Локальная копия индекса для использования в замыкании

            // Устанавливаем состояние кнопки (нажимаема или нет)
            bool isUnlocked = (i + 1) <= LevelManager.Instance.maxLevelCompleted + 1;  // maxLevelCompleted — максимальный пройденный уровень
            buttonComponent.interactable = isUnlocked;  // Делаем кнопку неактивной, если уровень заблокирован

            // Проверка на каждую 5-ю и 10-ю кнопку
            if ((i + 1) % 10 == 0)
            {
                buttonComponent.gameObject.GetComponent<UnityEngine.UI.Image>().color = new Color(0f, 0.878f, 1f);  // Цвет для каждой 10-й кнопки (00E0FF)
                buttonComponent.gameObject.transform.localScale = 1.2f * Vector3.one;
            }
            else if ((i + 1) % 5 == 0)
            {
                buttonComponent.gameObject.GetComponent<UnityEngine.UI.Image>().color = new Color(1f, 0.506f, 0f);  // Цвет для каждой 5-й кнопки (FF8100)
                buttonComponent.gameObject.transform.localScale = 1.1f * Vector3.one;
            }

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

            // Получаем текущий прогресс
            float progress = LevelManager.Instance.levelList[i];

            Transform badgeTransform = levelButton.transform.Find("BadgeImage");

            Image badgeImage = badgeTransform.GetComponent<Image>();
            if (progress >= 100)
            {
                badgeImage.sprite = goldBadge;
                badgeTransform.gameObject.SetActive(true);
            }
            else if (progress >= 75)
            {
                badgeImage.sprite = silverBadge;
                badgeTransform.gameObject.SetActive(true);
            }
            else if (progress >= 50)
            {
                badgeImage.sprite = bronzeBadge;
                badgeTransform.gameObject.SetActive(true);
            }

        }

        // Анимация появления панели (если нужно)
        levelsPanel.transform.localScale = Vector3.zero;  // Начальное состояние (панель скрыта)
        levelsPanel.transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack);  // Анимация появления
    }

    public void SetUpUI()
    {
        // reset button 
        resetButton.onClick.RemoveAllListeners();
        resetButton.onClick.AddListener(() =>
        {
            GameManager.Instance.StartLevel(LevelManager.Instance.level);
        });
    }

    // Закрытие панели уровней
    public void HideLevelsPanel()
    {
        levelsPanel.transform.DOScale(Vector3.zero, 0.5f).SetEase(Ease.InBack).OnComplete(() => levelsPanel.SetActive(false));
        _IsUIActive = false;
    }

    // Функция, вызываемая при выборе уровня
    private void OnLevelSelected(int levelIndex)
    {
        // Здесь можно вызывать функцию для начала выбранного уровня
        LevelManager.Instance.StartLevel(levelIndex);
        HideLevelsPanel();  // Закрываем панель после выбора уровня
    }

    public void ShowResults()
    {
        badge.gameObject.SetActive(false);
        _IsUIActive = true;

        // Показываем панель результатов
        resultsPanel.SetActive(true);

        // Настраиваем звёзды в исходное состояние (все звёзды тёмные)
        foreach (Image star in stars)
        {
            star.sprite = unearnedStarSprite;
            star.transform.localScale = Vector3.one;  // Убираем возможные анимационные изменения
        }

        // Получаем текущий прогресс
        float progress = (float)(LevelManager.Instance.totalBlocks - LevelManager.Instance.remainingBlocks) / LevelManager.Instance.totalBlocks * 100;

        // Определяем количество звёзд на основе прогресса
        int earnedStars = 0;
        if (progress >= 100)
        {
            earnedStars = 3;
        }
        else if (progress >= 75)
        {
            earnedStars = 2;
        }
        else if (progress >= 50)
        {
            earnedStars = 1;
        }

        // Запускаем анимацию появления звёзд с задержкой
        for (int i = 0; i < earnedStars; i++)
        {
            int starIndex = i;
            stars[starIndex].transform.localScale = Vector3.zero;  // Начальное состояние для Transform
            stars[starIndex].transform.DOScale(Vector3.one, 0.5f)
                .SetEase(Ease.OutBack)
                .SetDelay(starIndex * 0.5f)
                .OnComplete(() =>
                {
                    stars[starIndex].sprite = earnedStarSprite;  // Меняем спрайт на полученную звезду
                });
        }

        Transform nextLevelButtonTransform = resultsPanel.transform.Find("HorizontalLayout").Find("NextLevelButton").transform;
        Transform restartButtonTransform = resultsPanel.transform.Find("HorizontalLayout").Find("RestartButton").transform;

        Button nextLevelButton = nextLevelButtonTransform.GetComponent<Button>();
        Button restartButton = restartButtonTransform.GetComponent<Button>();

        nextLevelButton.onClick.RemoveAllListeners();
        restartButton.onClick.RemoveAllListeners();

        restartButton.onClick.AddListener(() =>
        {
            GameManager.Instance.StartLevel(LevelManager.Instance.level);
        });

        if (earnedStars <= 0)
        {
            if (LevelManager.Instance.level >= LevelManager.Instance.maxLevelCompleted)
            {
                nextLevelButton.interactable = false;
            }
            else
            {
                nextLevelButton.interactable = true;
                nextLevelButton.onClick.AddListener(() =>
                {
                    GameManager.Instance.StartLevel();
                });
            }
        }
        else
        {
            nextLevelButton.interactable = true;

            nextLevelButton.onClick.AddListener(() =>
            {
                GameManager.Instance.StartLevel();
            });
        }

        // Плавное появление панели
        resultsPanel.transform.localScale = Vector3.zero;  // Начальное состояние (панель скрыта)
        resultsPanel.transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack);  // Анимация появления панели

        if (YandexGame.EnvironmentData.language == "ru")
        {
            percentageText.text = $"Прогресс: {progress:F0}%";
            coinsEarnedText.text = $"Монет получено: {LevelManager.Instance.coinsEarnedOnLevel}";
            attemptsUsedText.text = $"Попыток использовано: {LevelManager.Instance.attempts}/{LevelManager.Instance.maxAttempts}";
        }
        else if (YandexGame.EnvironmentData.language == "en")
        {
            percentageText.text = $"Progress: {progress:F0}%";
            coinsEarnedText.text = $"Coins earned: {LevelManager.Instance.coinsEarnedOnLevel}";
            attemptsUsedText.text = $"Attempts used: {LevelManager.Instance.attempts}/{LevelManager.Instance.maxAttempts}";
        }
        else if (YandexGame.EnvironmentData.language == "tr")
        {
            percentageText.text = $"Yüzde: {progress:F0}%";
            coinsEarnedText.text = $"Kazanılan paralar: {LevelManager.Instance.coinsEarnedOnLevel}";
            attemptsUsedText.text = $"Kullanılan denemeler: {LevelManager.Instance.attempts}/{LevelManager.Instance.maxAttempts}";
        }

        // Определяем медаль на основе количества звезд
        if (earnedStars == 3)
        {
            badge.sprite = goldBadge;
            badge.gameObject.SetActive(true);
        }
        else if (earnedStars == 2)
        {
            badge.sprite = silverBadge;
            badge.gameObject.SetActive(true);
        }
        else if (earnedStars == 1)
        {
            badge.sprite = bronzeBadge;
            badge.gameObject.SetActive(true);
        }
        else
        {
            badge.gameObject.SetActive(false);
        }

        // Анимация появления медали
        badge.transform.localScale = Vector3.zero;  // Начальное состояние для Transform медали
        badge.transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack).SetDelay(earnedStars * 0.5f);  // Анимация появления медали после звезд
    }

    // Закрытие панели результатов с анимацией
    public void HideResults()
    {
        resultsPanel.transform.DOScale(Vector3.zero, 0.5f).SetEase(Ease.InBack).OnComplete(() => resultsPanel.SetActive(false));
        _IsUIActive = false;
    }


    public void ShowShopPanel()
    {
        shopPanel.SetActive(true);

        // Анимация появления панели (если нужно)
        shopPanel.transform.localScale = Vector3.zero;  // Начальное состояние (панель скрыта)
        shopPanel.transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack);  // Анимация появления
    }

    // Закрытие панели уровней
    public void HideShopPanel()
    {
        shopPanel.transform.DOScale(Vector3.zero, 0.5f).SetEase(Ease.InBack).OnComplete(() => shopPanel.SetActive(false));
    }

    // Метод для обновления количества монет
    public void UpdateCoins()
    {
        coinsText.text = GameManager.Instance.player.totalCoins.ToString();
    }

    // Метод для обновления количества кристаллов
    public void UpdateDiamonds()
    {
        diamondsText.text = GameManager.Instance.player.totalDiamonds.ToString();
    }

    // Метод для обновления уровня
    public void UpdateLevel()
    {
        if (YandexGame.EnvironmentData.language == "ru")
        {
            levelText.text = "Уровень: " + LevelManager.Instance.level;
        }
        else if (YandexGame.EnvironmentData.language == "en")
        {
            levelText.text = "Level: " + LevelManager.Instance.level;
        }
        else if (YandexGame.EnvironmentData.language == "tr")
        {
            levelText.text = "Seviye: " + LevelManager.Instance.level;
        }
    }

    // Метод для обновления процента уничтоженных блоков
    public void UpdateProgress()
    {
        // Обновляем прогресс в UI
        float progress = (float)(LevelManager.Instance.totalBlocks - LevelManager.Instance.remainingBlocks) / LevelManager.Instance.totalBlocks * 100;


        if (YandexGame.EnvironmentData.language == "ru")
        {
            progressText.text = "Прогресс: " + progress.ToString("F0") + "%";
        }
        else if (YandexGame.EnvironmentData.language == "en")
        {
            progressText.text = "Progress: " + progress.ToString("F0") + "%";
        }
        else if (YandexGame.EnvironmentData.language == "tr")
        {
            progressText.text = "İlerleme: " + progress.ToString("F0") + "%";
        }
    }

    // Метод для обновления процента уничтоженных блоков
    public void UpdateAttempts()
    {
        if (YandexGame.EnvironmentData.language == "ru")
        {
            attemptsText.text = $"Попытки: {LevelManager.Instance.attempts}/{LevelManager.Instance.maxAttempts}";
        }
        else if (YandexGame.EnvironmentData.language == "en")
        {
            attemptsText.text = $"Attempts: {LevelManager.Instance.attempts}/{LevelManager.Instance.maxAttempts}";
        }
        else if (YandexGame.EnvironmentData.language == "tr")
        {
            attemptsText.text = $"Denemeler: {LevelManager.Instance.attempts}/{LevelManager.Instance.maxAttempts}";
        }
    }

    public void ActivateUI()
    {
        _IsUIActive = true;
    }

    public bool IsUIActive()
    {
        return _IsUIActive;
    }
}
