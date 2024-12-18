using UnityEngine;

public class DestructibleBlock : MonoBehaviour
{
    public int rewardCoins = 10;  // Награда за разрушение блока
    public float hp = 1000f;  // Здоровье блока
    public float fallDamageThreshold = 5f;  // Порог скорости падения, при котором наносится урон
    public float fallDamageMultiplier = 10f;  // Множитель урона при падении
    public bool isFalling = false;  // Флаг, активен ли блок в свободном падении

    private Rigidbody rb;  // Храним ссылку на Rigidbody
    private Renderer blockRenderer;  // Рендер для изменения цвета

    void Awake()
    {
        // Попытка найти Rigidbody
        rb = GetComponent<Rigidbody>();

        // Если Rigidbody не найден, добавляем его
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
            rb.isKinematic = true;  // Отключаем физику, если не нужно
            rb.mass = 10f;  // Устанавливаем массу
        }

        // Получаем компонент Renderer
        blockRenderer = GetComponent<Renderer>();

        // Если рендерера нет, выводим предупреждение
        if (blockRenderer == null)
        {
            Debug.LogWarning("No Renderer found on the block, it will not change color based on health.");
        }

        ActivatePhysics();
    }

    void OnCollisionEnter(Collision collision)
    {
        // 1. Проверяем попадание снарядом
        if (collision.gameObject.CompareTag("Projectile"))  // Снаряд помечен тегом "Projectile"
        {
            float impactSpeed = collision.relativeVelocity.magnitude;  // Скорость столкновения
            float projectileDamage = impactSpeed * 10;  // Пример урона от скорости
            TakeDamage(projectileDamage);  // Наносим урон блоку

            // Если блок разрушен, начисляем награду и убираем его
            if (hp <= 0)
            {
                GameManager.Instance.AddCoins(rewardCoins);
                LevelManager.Instance.RemoveBlock();
                Destroy(gameObject, 0.01f);
            }
        }

        // 2. Проверяем урон от падений
        if (!isFalling && rb.velocity.magnitude > fallDamageThreshold)
        {
            float fallDamage = rb.velocity.magnitude * fallDamageMultiplier;
            TakeDamage(fallDamage);

            // Выводим в логи процент урона от падения
            float damagePercentage = (fallDamage / 1000f) * 100;  // Примерно считаем процент урона от здоровья
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

        // Выводим в логи
        if (damagePercentage > 0)
        {
            Debug.Log($"Block took {damagePercentage:F2}% damage.");
        }

        // Изменяем цвет в зависимости от оставшегося здоровья
        UpdateBlockColor();

        if (hp <= 0)
        {
            // Начисляем награду за разрушение блока, если урон привел к уничтожению
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
        if (blockRenderer != null)
        {
            // Вычисляем процент оставшегося здоровья
            float healthPercentage = hp / 1000f;  // 1000 - максимальное здоровье, можно изменить

            // Изменяем только красный цвет
            blockRenderer.material.color = new Color(1f, 1f - healthPercentage, 1f - healthPercentage);
            // 1f - здоровье влияет на интенсивность красного
        }
    }
}
