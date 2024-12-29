using DG.Tweening;  // Добавляем DOTween
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using YG;
using Unity.VisualScripting;
using System;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    private bool _IsUIActive = false;

    [Header("Panels")]
    public GameObject resultsPanel;  // Панель с результатами
    public GameObject shopPanel;  // Панель с результатами
    public GameObject buyProjectilePanel;

    [Header("Texts")]
    public TextMeshProUGUI coinsText;  // Ссылка на UI элемент для отображения количества монет
    public TextMeshProUGUI diamondsText;  // Ссылка на UI элемент для отображения количества монет

    public TextMeshProUGUI progressText;  // Ссылка на UI элемент для отображения процента уничтоженных блоков
    public TextMeshProUGUI levelText;  // Текст с уровнем
    public TextMeshProUGUI attemptsText; // Текст с попытками
    public TextMeshProUGUI percentageText; // Текст с процентами
    public TextMeshProUGUI coinsEarnedText; // Текст с собранными монетами
    public TextMeshProUGUI attemptsUsedText; // Текст с использованными попытками

    [Header("Shop Buttons")]
    public Button powerCannonButton;
    public Button sizeCannonButton;
    public Button massCannonButton;
    public Button floorButton;
    public Button attemptsButton;
    public Button explosiveButton;
    public Button projectilesAmountButton;
    public Button armageddonButton;

    public Button rewardedCoinsButton;
    public Button rewardedAttemptButton;
    public Button rewardedDiamondsButton;

    private GameObject[] upgradeButtons;

    [Header("Prices")]
    public int[] upgradeCostsCoins; // Цены на апгрейды в монетах для всех кнопок
    public int[] upgradeCostsDiamonds; // Цены на апгрейды в кристаллах для всех кнопок

    public int[] upgradeLevels; // Уровни апгрейда для всех кнопок
    public int[] upgradeMaxLevels; // Уровни апгрейда для всех кнопок

    [Header("Main Canvas Buttons")]
    public Button resetButton;
    public Button armageddonUseButton;

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

    private int rewardForRewardCoins = 100;
    private int rewardForRewardDiamonds = 15;

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

        upgradeButtons = new GameObject[8];

        // Привязываем функции к кнопкам
        powerCannonButton.onClick.AddListener(() => Upgrade(0));
        sizeCannonButton.onClick.AddListener(() => Upgrade(1));
        massCannonButton.onClick.AddListener(() => Upgrade(2));
        floorButton.onClick.AddListener(() => Upgrade(3));
        attemptsButton.onClick.AddListener(() => Upgrade(4));
        explosiveButton.onClick.AddListener(() => Upgrade(5));
        projectilesAmountButton.onClick.AddListener(() => Upgrade(6));
        armageddonButton.onClick.AddListener(() => Upgrade(7));

        upgradeButtons[0] = powerCannonButton.gameObject;
        upgradeButtons[1] = sizeCannonButton.gameObject;
        upgradeButtons[2] = massCannonButton.gameObject;
        upgradeButtons[3] = floorButton.gameObject;
        upgradeButtons[4] = attemptsButton.gameObject;
        upgradeButtons[5] = explosiveButton.gameObject;
        upgradeButtons[6] = projectilesAmountButton.gameObject;
        upgradeButtons[7] = armageddonButton.gameObject;

        for (int i = 0; i < upgradeButtons.Length; i++)
        {
            UpdateUpgradeButton(i);
        }
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
        UpdateArmageddonButton();
    }

    private void UpdateArmageddonButton()
    {
        if (!GameManager.Instance.player.IsArmageddonActivated)
        {
            armageddonUseButton.interactable = true;
        }
        else
        {
            armageddonUseButton.interactable = false;
        }
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

    private void UpdateUpgradeButton(int index)
    {
        // Находим нужную кнопку по индексу (например, через массив кнопок)
        GameObject upgradeButton = upgradeButtons[index];

        // Находим текстовые компоненты внутри кнопки
        Transform parentTransform = upgradeButton.transform.parent;

        TextMeshProUGUI priceText = upgradeButton.transform.Find("Text/PriceText").GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI levelText = parentTransform.Find("LevelText").GetComponent<TextMeshProUGUI>();

        // Получаем текущие значения уровня и стоимости
        int currentLevel = upgradeLevels[index];
        int currentCoinsCost = upgradeCostsCoins[index];
        int currentDiamondsCost = upgradeCostsDiamonds[index];

        if (currentCoinsCost > currentDiamondsCost)
        {
            priceText.text = $"{FormatNumber(currentCoinsCost)}";
        }
        else
        {
            priceText.text = $"{FormatNumber(currentDiamondsCost)}";
        }

        // Обновляем текст уровня
        levelText.text = currentLevel.ToString();
    }

    private void Upgrade(int index)
    {
        int currentLevel = upgradeLevels[index]; // Текущий уровень апгрейда
        int coinsCost = upgradeCostsCoins[index]; // Текущая цена в монетах
        int diamondsCost = upgradeCostsDiamonds[index]; // Текущая цена в кристаллах

        // Проверка, хватает ли ресурсов для апгрейда
        if (GameManager.Instance.player.totalCoins >= coinsCost && GameManager.Instance.player.totalDiamonds >= diamondsCost)
        {
            if (upgradeMaxLevels[index] > upgradeLevels[index])
            {
                ResourceAnimator.Instance.AnimateResourceChange(GameManager.Instance.player.totalCoins, GameManager.Instance.player.totalCoins - coinsCost, true);
                ResourceAnimator.Instance.AnimateResourceChange(GameManager.Instance.player.totalDiamonds, GameManager.Instance.player.totalDiamonds - diamondsCost, false);

                // Уменьшаем количество ресурсов
                GameManager.Instance.player.totalCoins -= coinsCost;
                GameManager.Instance.player.totalDiamonds -= diamondsCost;

                // Повышаем уровень апгрейда
                upgradeLevels[index]++;

                // Повышаем цену для следующего уровня
                upgradeCostsCoins[index] += upgradeCostsCoins[index] / 10;
                upgradeCostsDiamonds[index] += upgradeCostsDiamonds[index] / 10;

                if (upgradeLevels[index] == upgradeMaxLevels[index])
                {
                    // Находим нужную кнопку по индексу (например, через массив кнопок)
                    GameObject upgradeButton = upgradeButtons[index];

                    // Находим текстовые компоненты внутри кнопки
                    Transform parentTransform = upgradeButton.transform.parent;

                    GameObject priceObj = upgradeButton.transform.Find("Text").gameObject;
                    TextMeshProUGUI levelText = parentTransform.Find("LevelText").GetComponent<TextMeshProUGUI>();

                    priceObj.SetActive(false);

                    if (YandexGame.EnvironmentData.language == "ru")
                    {
                        // Обновляем текст уровня
                        levelText.text = "MAКС.";
                    }
                    else if (YandexGame.EnvironmentData.language == "en")
                    {
                        // Обновляем текст уровня
                        levelText.text = "MAX.";
                    }
                    else if (YandexGame.EnvironmentData.language == "tr")
                    {
                        // Обновляем текст уровня
                        levelText.text = "MAX.";
                    }

                    upgradeButton.GetComponent<Button>().interactable = false;
                }
                else
                {
                    UpdateUpgradeButton(index); // Обновляем текст кнопки после апгрейда
                }

                // Обновляем UI
                UpdateCoins();
                UpdateDiamonds();

                // Применяем изменения (например, апгрейд статов)
                ApplyUpgrade(index);

                SaveData();
            }
        }
        else
        {
            Debug.Log("Не хватает ресурсов для апгрейда!");
        }
    }

    private void ApplyUpgrade(int index)
    {
        switch (index)
        {
            case 0:
            // Применяем изменения для Power Cannon
            GameManager.Instance.player.cannonPower += 1f;
            break;
            case 1:
            // Применяем изменения для Size Cannon
            GameManager.Instance.player.cannonSize += 0.05f;
            break;
            case 2:
            // Применяем изменения для Mass Cannon
            GameManager.Instance.player.cannonMass += 50f;
            break;
            case 3:
            // Применяем изменения для Floor
            SpawnManager.Instance.ChangeGround();
            break;
            case 4:
            // Применяем изменения для Attempts
            LevelManager.Instance.AddAttempts(1);
            break;
            case 5:
            // Применяем изменения для Explosive
            GameManager.Instance.player.explosiveDamage += 10;
            break;
            case 6:
            // Применяем изменения для Projectiles Amount
            GameManager.Instance.player.projectileAmount += 1;
            break;
            case 7:
            // Применяем изменения для Armageddon
            armageddonUseButton.interactable = true;
            SpawnManager.Instance.armageddonInterval -= SpawnManager.Instance.armageddonInterval * 0.2f;
            GameManager.Instance.player.armageddonLevel++;
            break;
            default:
            Debug.Log("Неверный индекс апгрейда!");
            break;
        }
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
            coinsEarnedText.text = $"Монет получено: {FormatNumber(LevelManager.Instance.coinsEarnedOnLevel)}";
            attemptsUsedText.text = $"Попыток использовано: {LevelManager.Instance.attempts}/{LevelManager.Instance.maxAttempts}";
        }
        else if (YandexGame.EnvironmentData.language == "en")
        {
            percentageText.text = $"Progress: {progress:F0}%";
            coinsEarnedText.text = $"Coins earned: {FormatNumber(LevelManager.Instance.coinsEarnedOnLevel)}";
            attemptsUsedText.text = $"Attempts used: {LevelManager.Instance.attempts}/{LevelManager.Instance.maxAttempts}";
        }
        else if (YandexGame.EnvironmentData.language == "tr")
        {
            percentageText.text = $"Yüzde: {progress:F0}%";
            coinsEarnedText.text = $"Kazanılan paralar: {FormatNumber(LevelManager.Instance.coinsEarnedOnLevel)}";
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

        for (int i = 0; i < upgradeButtons.Length; i++)
        {
            UpdateUpgradeButton(i);
        }

        UpdateRewardedButtons();
    }

    private void UpdateRewardedButtons()
    {
        int rewardCoinsAd = (GameManager.Instance.player.totalCoins / 10 > 100) ? (int)(GameManager.Instance.player.totalCoins / 10) : 100;
        int rewardDiamondsAd = (GameManager.Instance.player.totalDiamonds / 5 > 15) ? (int)(GameManager.Instance.player.totalDiamonds / 5) : 15;

        GameManager.Instance.rewardCoinsAd = rewardCoinsAd;
        GameManager.Instance.rewardDiamondsAd = rewardDiamondsAd;

        rewardedCoinsButton.transform.Find("Text/AmountText").GetComponent<TextMeshProUGUI>().text = FormatNumber(rewardCoinsAd);
        rewardedDiamondsButton.transform.Find("Text/AmountText").GetComponent<TextMeshProUGUI>().text = FormatNumber(rewardDiamondsAd);

        rewardedCoinsButton.onClick.RemoveAllListeners();
        rewardedDiamondsButton.onClick.RemoveAllListeners();

        rewardedCoinsButton.onClick.AddListener(() => {
            GameManager.Instance.WatchRewardedAd(1);
        }
        );
        rewardedDiamondsButton.onClick.AddListener(() => {
            GameManager.Instance.WatchRewardedAd(2);
        }
        );
        //rewardedAttemptButton;
        //private int rewardForRewardCoins = 100;
        //private int rewardForRewardDiamonds = 15;
    }

    // Закрытие панели уровней
    public void HideShopPanel()
    {
        shopPanel.transform.DOScale(Vector3.zero, 0.5f).SetEase(Ease.InBack).OnComplete(() => shopPanel.SetActive(false));
    }

    // Метод для обновления количества монет
    public void UpdateCoins()
    {
        coinsText.text = FormatNumber(GameManager.Instance.player.totalCoins);
    }

    // Метод для обновления количества кристаллов
    public void UpdateDiamonds()
    {
        diamondsText.text = FormatNumber(GameManager.Instance.player.totalDiamonds);
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
        float progress;

        // Обновляем прогресс в UI
        if (LevelManager.Instance.totalBlocks == 0)
        {
            progress = 0f;
        }
        else
        {
            progress = (float)(LevelManager.Instance.totalBlocks - LevelManager.Instance.remainingBlocks) / LevelManager.Instance.totalBlocks * 100;
        }

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
    string FormatNumber(int number)
    {
        if (number >= 1000000)
        {
            return $"{(number / 1000000f):0.00}m";
        }
        else if (number >= 1000)
        {
            return $"{(number / 1000f):0.00}k";
        }
        else
        {
            return number.ToString();
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

    internal void PrepareProjectileShopPanel(GameObject projectile)
    {
        int j = GetShopMaterialIndex(projectile);
        if (j == -1)
        {
            Debug.Log("Material isn't here");
            return;
        }

        Button useButton = buyProjectilePanel.transform.Find("UseButton").GetComponent<Button>();
        Button buyButton = buyProjectilePanel.transform.Find("BuyButton").GetComponent<Button>();

        UpdateShopPanelUI(j, useButton, buyButton);

        useButton.onClick.RemoveAllListeners();
        buyButton.onClick.RemoveAllListeners();

        useButton.onClick.AddListener(() =>
        {
            UseShopItem(j, useButton, buyButton);
        });

        buyButton.onClick.AddListener(() =>
        {
            PurchaseShopItem(j, projectile, useButton, buyButton);
        });
    }

    private int GetShopMaterialIndex(GameObject projectile)
    {
        for (int i = 0; i < SpawnManager.Instance.shopMaterials.Length; i++)
        {
            if (projectile.GetComponent<MeshRenderer>().material.name.Contains(SpawnManager.Instance.shopMaterials[i].name))
            {
                return i;
            }
        }
        return -1;
    }

    private void UpdateShopPanelUI(int j, Button useButton, Button buyButton)
    {
        if (GameManager.Instance.player.shopMaterialsPurchased[j])
        {
            buyProjectilePanel.transform.Find("ResourcePanel").gameObject.SetActive(false);
            SetUseButtonState(useButton, j);
            buyButton.interactable = false;
        }
        else
        {
            buyProjectilePanel.transform.Find("ResourcePanel").gameObject.SetActive(true);
            useButton.interactable = false;
            buyButton.interactable = true;
        }
    }

    private void SetUseButtonState(Button useButton, int j)
    {
        if (SpawnManager.Instance.projectilePrefab.GetComponent<MeshRenderer>().sharedMaterial.name == SpawnManager.Instance.shopMaterials[j].name)
        {
            SetUseButtonText(useButton, "Используется", "Is Used", "Kullanılır");
            useButton.interactable = false;
        }
        else
        {
            SetUseButtonText(useButton, "Использовать", "Use", "Kullanmak için");
            useButton.interactable = true;
        }
    }

    private void SetUseButtonText(Button button, string ruText, string enText, string trText)
    {
        TextMeshProUGUI buttonText = button.transform.GetComponentInChildren<TextMeshProUGUI>();
        string language = YandexGame.EnvironmentData.language;

        if (language == "ru")
        {
            buttonText.text = ruText;
        }
        else if (language == "en")
        {
            buttonText.text = enText;
        }
        else if (language == "tr")
        {
            buttonText.text = trText;
        }
    }

    private void UseShopItem(int j, Button useButton, Button buyButton)
    {
        SpawnManager.Instance.projectilePrefab.GetComponent<MeshRenderer>().sharedMaterial = SpawnManager.Instance.shopMaterials[j];
        if (GameManager.Instance.player.shopMaterialsPurchased[j])
        {
            UpdateShopPanelUI(j, useButton, buyButton);
        }
    }

    private void PurchaseShopItem(int j, GameObject projectile, Button useButton, Button buyButton)
    {
        if (GameManager.Instance.player.totalDiamonds >= 50)
        {
            ResourceAnimator.Instance.AnimateResourceChange(GameManager.Instance.player.totalDiamonds, GameManager.Instance.player.totalDiamonds - 50, true);
            GameManager.Instance.AddDiamonds(-50);
            GameManager.Instance.player.shopMaterialsPurchased[j] = true;

            // Обновляем цвет блока после покупки
            SpawnManager.Instance.UpdateShopBlockColor(j);
            Debug.Log("Updating color for block " + j);


            PrepareProjectileShopPanel(projectile);
        }
    }


    public void SaveData()
    {
        YandexGame.savesData.upgradeLevels = upgradeLevels;
        YandexGame.savesData.upgradeCostsCoins = upgradeCostsCoins; // Цены на апгрейды в монетах для всех кнопок
        YandexGame.savesData.upgradeCostsDiamonds = upgradeCostsDiamonds; // Цены на апгрейды в кристаллах для всех кнопок
}

    public void LoadData()
    {
        if (YandexGame.savesData.upgradeLevels == null || YandexGame.savesData.upgradeLevels.Length <= 0)
        {
            SaveData();
        }
        upgradeLevels = YandexGame.savesData.upgradeLevels;

        if(YandexGame.savesData.upgradeCostsCoins == null || YandexGame.savesData.upgradeCostsCoins.Length <= 0)
        {
            SaveData();
        }
        upgradeCostsCoins = YandexGame.savesData.upgradeCostsCoins; // Цены на апгрейды в монетах для всех кнопок

        if (YandexGame.savesData.upgradeCostsDiamonds == null || YandexGame.savesData.upgradeCostsDiamonds.Length <= 0)
        {
            SaveData();
        }
        upgradeCostsDiamonds = YandexGame.savesData.upgradeCostsDiamonds; // Цены на апгрейды в кристаллах для всех кнопок
    }

    public void HideProjectileBuyPanel()
    {
        buyProjectilePanel.SetActive(false);
    }
}
