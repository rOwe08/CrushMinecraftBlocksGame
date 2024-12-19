using UnityEngine;

public class DestructibleBlock : MonoBehaviour
{
    public int rewardCoins = 10;  // Награда за разрушение блока
    public float hp;  // Здоровье блока
    public float fallDamageThreshold = 3f;  // Минимальная высота падения для получения урона
    public float fallDamageMultiplier = 5f;  // Множитель урона при падении
    public BlockType blockType;  // Тип блока, включая максимальное здоровье
    public float fallDamageActivationDelay = 1f;  // Задержка перед активацией урона от падений

    private float startY;  // Высота, с которой начинается падение
    private Rigidbody rb;  // Храним ссылку на Rigidbody
    private Renderer blockRenderer;  // Рендер для изменения цвета
    private bool canTakeFallDamage = false;  // Флаг, можно ли получать урон от падений

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
    }

    private void Update()
    {
        if (hp < 0) 
        {
            GameManager.Instance.AddCoins(rewardCoins);
            LevelManager.Instance.RemoveBlock();
            Destroy(gameObject);  // Уничтожаем блок   
        }
    }
    // Активируем возможность получать урон от падения через задержку
    void EnableFallDamage()
    {
        canTakeFallDamage = true;
    }

    void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Block collided with " + collision.gameObject.name);  // Логируй имя объекта, с которым произошло столкновение

        // Проверка на урон от падений только при столкновении с землей
        if (collision.gameObject.CompareTag("Ground") && canTakeFallDamage)
        {
            TakeDamage(10f);  // Фиксированный урон от падения
        }

        // Проверка на урон от снарядов
        else if (collision.gameObject.CompareTag("Projectile"))
        {
            float impactSpeed = collision.relativeVelocity.magnitude;
            float projectileDamage = impactSpeed * 10;
            TakeDamage(projectileDamage);
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
            // Вычисляем процент оставшегося здоровья
            float healthPercentage = hp / blockType.health;

            // Изменяем только красный цвет
            blockRenderer.material.color = new Color(1f, 1f - healthPercentage, 1f - healthPercentage);
        }
    }
}
