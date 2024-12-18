using UnityEngine;

public class DestructibleBlock : MonoBehaviour
{
    public int rewardCoins = 10;  // Награда за разрушение блока
    public float hp;  // Здоровье блока
    public float fallDamageThreshold = 1f;  // Порог скорости падения, при котором наносится урон
    public float fallDamageMultiplier = 10f;  // Множитель урона при падении
    public bool isFalling = false;  // Флаг, активен ли блок в свободном падении
    public float fallDamageActivationDelay = 1f;  // Время задержки перед активацией урона от падения

    private Rigidbody rb;  // Храним ссылку на Rigidbody
    private Renderer blockRenderer;  // Рендер для изменения цвета
    public BlockType blockType;  // Тип блока, включая максимальное здоровье
    private bool canTakeFallDamage = false;  // Может ли блок получать урон от падения

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
        if (blockRenderer == null)
        {
            Debug.LogWarning("No Renderer found on the block, it will not change color based on health.");
        }

        // Проверяем, установлен ли BlockType, и устанавливаем здоровье
        if (blockType != null)
        {
            hp = blockType.health;
        }

        ActivatePhysics();
        Invoke(nameof(EnableFallDamage), fallDamageActivationDelay);  // Активируем урон от падений через заданную задержку
    }

    // Метод, активирующий возможность получения урона от падений
    void EnableFallDamage()
    {
        canTakeFallDamage = true;
    }

    void OnCollisionEnter(Collision collision)
    {
        // 1. Проверяем попадание снарядом
        if (collision.gameObject.CompareTag("Projectile"))
        {
            float impactSpeed = collision.relativeVelocity.magnitude;  // Скорость столкновения
            float projectileDamage = impactSpeed * 10;  // Пример урона от скорости
            TakeDamage(projectileDamage);  // Наносим урон блоку
        }

        // 2. Проверяем урон от падений, только если разрешено брать урон от падений
        if (canTakeFallDamage && hp > 0 && !isFalling && rb.velocity.magnitude > fallDamageThreshold)
        {
            float fallDamage = rb.velocity.magnitude * fallDamageMultiplier;
            TakeDamage(fallDamage);

            // Выводим в логи процент урона от падения
            float damagePercentage = (fallDamage / blockType.health) * 100;  // Используем maxHealth из BlockType
            Debug.Log($"Block took {damagePercentage:F2}% damage from falling.");
        }
    }

    // Метод для получения урона
    public void TakeDamage(float damage)
    {
        float initialHealth = hp;
        hp -= damage;

        // Вычисляем процент урона
        float damagePercentage = ((initialHealth - hp) / initialHealth) * 100;

        if (damagePercentage > 0)
        {
            Debug.Log($"Block took {damagePercentage:F2}% damage.");
        }

        // Изменяем цвет в зависимости от оставшегося здоровья
        UpdateBlockColor();

        if (hp <= 0)
        {
            GameManager.Instance.AddCoins(rewardCoins);
            LevelManager.Instance.RemoveBlock();
            Destroy(gameObject);  // Уничтожаем блок
        }
    }

    // Метод для активации физики при падении блока
    public void ActivatePhysics()
    {
        rb.isKinematic = false;  // Включаем физику
        isFalling = true;  // Устанавливаем флаг, что блок находится в свободном падении
    }

    // Метод для того, чтобы сбросить флаг после падения
    private void OnCollisionExit(Collision collision)
    {
        if (isFalling && collision.relativeVelocity.magnitude < fallDamageThreshold)
        {
            isFalling = false;  // Отключаем флаг падения после столкновения
        }
    }

    // Метод для изменения цвета блока в зависимости от его здоровья
    private void UpdateBlockColor()
    {
        if (blockRenderer != null && blockType != null)
        {
            // Вычисляем процент оставшегося здоровья
            float healthPercentage = hp / blockType.health;  // Используем maxHealth из BlockType

            // Изменяем только красный цвет
            blockRenderer.material.color = new Color(1f, 1f - healthPercentage, 1f - healthPercentage);
            // 1f - здоровье влияет на интенсивность красного
        }
    }
}
