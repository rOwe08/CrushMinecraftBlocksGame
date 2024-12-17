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
            LevelManager.Instance.RemoveBlock();

            // Разрушаем блок
            Destroy(gameObject, 0.01f);
        }
    }
}
