using UnityEngine;

public class DestructibleBlock : MonoBehaviour
{
    public int rewardCoins = 10;  // Награда за разрушение блока

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Projectile"))  // Снаряд помечен тегом "Projectile"
        {
            // Начисляем награду за разрушение блока
            GameManager.Instance.AddCoins(rewardCoins);
            UIManager.Instance.UpdateUI();

            // Разрушаем блок
            Destroy(gameObject);
        }
    }
}
