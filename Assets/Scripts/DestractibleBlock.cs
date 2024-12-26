using UnityEngine;

public class DestructibleBlock : MonoBehaviour
{
    public float hp;  // Здоровье блока
    public float fallDamageThreshold = 3f;  // Минимальная высота падения для получения урона
    public float fallDamageMultiplier = 5f;  // Множитель урона при падении
    public BlockType blockType;  // Тип блока, включая максимальное здоровье
    public float fallDamageActivationDelay = 1f;  // Задержка перед активацией урона от падений

    private float startY;  // Высота, с которой начинается падение
    private Rigidbody rb;  // Храним ссылку на Rigidbody
    private Renderer blockRenderer;  // Рендер для изменения цвета
    private bool canTakeFallDamage = false;  // Флаг, можно ли получать урон от падений

    [Header("Audio Clips")]
    private AudioClip destructionSound; // Звук разрушения блока
    private AudioClip hitSound;         // Звук попадания снаряда
    private AudioSource audioSource;   // Источник звука

    void Awake()
    {
        // Попытка найти Rigidbody
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
        }

        // Получаем компонент Renderer
        blockRenderer = GetComponent<Renderer>();

        // Проверяем, установлен ли BlockType, и устанавливаем здоровье
        if (blockType != null)
        {
            hp = blockType.health;
        }

        Invoke(nameof(EnableFallDamage), fallDamageActivationDelay);  // Задержка перед активацией урона от падений

        // Инициализация компонента AudioSource
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        // Проверяем, если звуковые клипы загружены
        if (destructionSound == null || hitSound == null)
        {
            Debug.LogWarning("Audio clips are missing in GameManager.");
        }
    }

    private void Update()
    {
        // Проверяем здоровье блока
        if (hp < 0)
        {
            HandleBlockDestruction();
        }

        // Проверяем, если блок вылетел за пределы карты (по оси Y)
        if (transform.position.y < -10)
        {
            HandleBlockDestruction();
        }
    }

    private void HandleBlockDestruction()
    {
        destructionSound = GameManager.Instance.destructionSound;
        hitSound = GameManager.Instance.hitSound;

        // Проигрываем звук разрушения
        if (destructionSound != null)
        {
            audioSource.PlayOneShot(destructionSound);
        }
        else
        {
            Debug.LogWarning("Destruction sound not set.");
        }

        if (blockType.IsCollectable)
        {
            if (blockType == BlockManager.Instance.rewardBlockTypes[0])
            {
                ResourceAnimator.Instance.AnimateResourceChange(GameManager.Instance.player.totalDiamonds, GameManager.Instance.player.totalDiamonds + blockType.reward, false);
                GameManager.Instance.AddDiamonds(blockType.reward);
            }
            else if (blockType == BlockManager.Instance.rewardBlockTypes[1])
            {
                if (blockType.reward > GameManager.Instance.player.totalCoins / 10)
                {
                    ResourceAnimator.Instance.AnimateResourceChange(GameManager.Instance.player.totalCoins, GameManager.Instance.player.totalCoins + blockType.reward, true);
                    GameManager.Instance.AddCoins(blockType.reward);
                }
                else
                {
                    int coinsToAdd = (GameManager.Instance.player.totalCoins / 10 > 1000) ? 1000 : GameManager.Instance.player.totalCoins / 10;
                    ResourceAnimator.Instance.AnimateResourceChange(GameManager.Instance.player.totalCoins, GameManager.Instance.player.totalCoins + coinsToAdd, true);
                    GameManager.Instance.AddCoins(coinsToAdd);
                }
            }
        }

        LevelManager.Instance.RemoveBlock();
        Destroy(gameObject);  // Уничтожаем блок
    }
    // Активируем возможность получать урон от падения через задержку
    void EnableFallDamage()
    {
        canTakeFallDamage = true;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground") && canTakeFallDamage)
        {
            TakeDamage(SpawnManager.Instance.GetSpawnedGroundIndex() * 100f);
            Debug.Log($"{(SpawnManager.Instance.GetSpawnedGroundIndex() * 100f / hp) * 100} % от падения");
        }
        else if (collision.gameObject.CompareTag("Projectile"))
        {
            float projectileDamage = GameManager.Instance.player.cannonPower * 10;
            Debug.Log($"{(projectileDamage / hp) * 100} %");

            TakeDamage(projectileDamage);

            // Проигрываем звук попадания снаряда
            if (hitSound != null)
            {
                audioSource.PlayOneShot(hitSound);
            }
            else
            {
                Debug.LogWarning("Hit sound not set.");
            }
        }
    }

    // Метод для получения урона
    public void TakeDamage(float damage)
    {
        float initialHealth = hp;
        hp -= damage;

        // Изменяем цвет в зависимости от оставшегося здоровья
        UpdateBlockColor();
    }

    // Метод для изменения цвета блока в зависимости от его здоровья
    private void UpdateBlockColor()
    {
        if (blockRenderer != null && blockType != null)
        {
            // Вычисляем процент оставшегося здоровья (от 0 до 1)
            float healthPercentage = hp / blockType.health;

            // Чем меньше здоровье, тем больше красный, уменьшаем синий и зеленый
            blockRenderer.material.color = new Color(1f, healthPercentage, healthPercentage);
        }
    }

}
