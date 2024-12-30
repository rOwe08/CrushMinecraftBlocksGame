using UnityEngine;

public class ProjectileScript : MonoBehaviour
{
    public GameObject explosionEffectPrefab;  // Префаб взрыва
    public float explosionRadius = 1f;        // Радиус взрыва
    public float explosionForce = 1f;      // Сила взрыва
    public float upwardsModifier = 0.5f;      // Вертикальная сила взрыва

    public void Awake()
    {
        LevelManager.Instance.IsProjectileInScene = true;

        GetComponent<Rigidbody>().mass = GameManager.Instance.player.cannonMass;
        transform.localScale = GameManager.Instance.player.cannonSize * Vector3.one;
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Проверяем, что объект, с которым столкнулся снаряд, имеет компонент DestructibleBlock
        if (collision.gameObject.GetComponent<DestructibleBlock>() != null)
        {
            // Запускаем анимацию взрыва

            if (UIManager.Instance.isExplosionPurchased) 
            {
                Explode(collision.contacts[0].point);
            }

            // Уничтожаем снаряд через 1 секунду
            Destroy(gameObject, 1);
        }
        else if (collision.gameObject.CompareTag("Ground"))
        {
            // Уничтожаем снаряд через 3 секунды
            Destroy(gameObject, 3);
        }
    }

    private void Explode(Vector3 explosionPosition)
    {
        // Создаем взрывной эффект
        Instantiate(explosionEffectPrefab, explosionPosition, Quaternion.identity);

        // Применяем взрывную силу к окружающим объектам
        Collider[] colliders = Physics.OverlapSphere(explosionPosition, explosionRadius);
        foreach (Collider hit in colliders)
        {
            Rigidbody rb = hit.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.AddExplosionForce(explosionForce, explosionPosition, explosionRadius, upwardsModifier, ForceMode.Impulse);
            }

            // Проверяем, является ли объект разрушаемым блоком и наносим урон
            DestructibleBlock destructibleBlock = hit.GetComponent<DestructibleBlock>();
            if (destructibleBlock != null)
            {
                float explosionDamage = GameManager.Instance.player.explosiveDamage; // Урон от взрыва
                destructibleBlock.TakeDamage(explosionDamage);
            }
        }
    }

    private void OnDestroy()
    {
        LevelManager.Instance.IsProjectileInScene = false;
    }
}
