using DG.Tweening;  // Добавляем DOTween
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    private bool _IsUIActive = false;

    [Header("Panels")]
    public GameObject resultsPanel;  // Панель с результатами
    public GameObject shopPanel;  // Панель с результатами

    [Header("Texts")]
    public TextMeshProUGUI coinsText;  // Ссылка на UI элемент для отображения количества монет
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
        Debug.Log("Level " + (levelIndex) + " selected");
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

        Transform nextActionButtonTransform = resultsPanel.transform.Find("NextActionButton").transform;
        Button nextActionButton = nextActionButtonTransform.GetComponent<Button>();
        nextActionButton.onClick.RemoveAllListeners();

        if (earnedStars <= 0)
        {
            nextActionButton.onClick.AddListener(() =>
            {
                GameManager.Instance.StartLevel(LevelManager.Instance.level);
            });
            nextActionButtonTransform.Find("ButtonText").GetComponent<TextMeshProUGUI>().text = "Try Again";
        }
        else
        {
            nextActionButton.onClick.AddListener(() =>
            {
                GameManager.Instance.StartLevel();
            });
            nextActionButtonTransform.Find("ButtonText").GetComponent<TextMeshProUGUI>().text = "Next Level";
        }

        // Плавное появление панели
        resultsPanel.transform.localScale = Vector3.zero;  // Начальное состояние (панель скрыта)
        resultsPanel.transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack);  // Анимация появления панели

        percentageText.text = $"Percentage: {progress:F0}%";
        coinsEarnedText.text = $"Coins earned: {LevelManager.Instance.coinsEarnedOnLevel}";
        attemptsUsedText.text = $"Attempts used: {LevelManager.Instance.attempts}/{LevelManager.Instance.maxAttempts}";

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

    // Метод для обновления количества монет
    public void UpdateCoins()
    {
        coinsText.text = GameManager.Instance.player.totalCoins.ToString();
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

    // Метод для обновления процента уничтоженных блоков
    public void UpdateAttempts()
    {
        attemptsText.text = $"Attempts: {LevelManager.Instance.attempts}/{LevelManager.Instance.maxAttempts}";
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
