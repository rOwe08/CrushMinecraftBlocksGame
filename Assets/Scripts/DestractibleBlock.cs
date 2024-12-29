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
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
        }

        blockRenderer = GetComponent<Renderer>();

        if (blockType != null)
        {
            hp = blockType.health;
        }

        Invoke(nameof(EnableFallDamage), fallDamageActivationDelay);

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        if (destructionSound == null || hitSound == null)
        {
            Debug.LogWarning("Audio clips are missing in GameManager.");
        }
    }

    private void Update()
    {
        if (hp < 0)
        {
            HandleBlockDestruction();
        }

        if (transform.position.y < -10)
        {
            HandleBlockDestruction();
        }
    }

    private void HandleBlockDestruction()
    {
        hitSound = GameManager.Instance.hitSound;
        destructionSound = GameManager.Instance.destructionSound;

        if (destructionSound != null)
        {
            PlayDestructionSound(transform.position, destructionSound);
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
        Destroy(gameObject);
    }

    void PlayDestructionSound(Vector3 position, AudioClip clip)
    {
        GameObject tempAudioObject = new GameObject("TempAudio");
        tempAudioObject.transform.position = position;

        AudioSource tempAudioSource = tempAudioObject.AddComponent<AudioSource>();
        tempAudioSource.clip = clip;
        tempAudioSource.volume = 0.1f;

        tempAudioSource.Play();
        Destroy(tempAudioObject, clip.length);
    }

    void EnableFallDamage()
    {
        canTakeFallDamage = true;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground") && canTakeFallDamage)
        {
            TakeDamage(SpawnManager.Instance.GetSpawnedGroundIndex() * 100f);
        }
        else if (collision.gameObject.CompareTag("Projectile"))
        {
            float projectileDamage = GameManager.Instance.player.cannonPower * 10;
            TakeDamage(projectileDamage);

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

    public void TakeDamage(float damage)
    {
        hp -= damage;
        UpdateBlockColor();
    }

    private void UpdateBlockColor()
    {
        if (blockRenderer != null && blockType != null)
        {
            float healthPercentage = hp / blockType.health;
            blockRenderer.material.color = new Color(1f, healthPercentage, healthPercentage);
        }
    }
}
