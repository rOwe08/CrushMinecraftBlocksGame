using UnityEngine;

public class ProjectileScript : MonoBehaviour
{
    public float damage = 10f;  // Урон, который наносится при столкновении

    private void OnCollisionEnter(Collision collision)
    {
        // Проверяем, что объект, с которым столкнулся снаряд, имеет компонент DestructibleBlock
        if (collision.gameObject.GetComponent<DestructibleBlock>() != null)
        {
            Destroy(gameObject, 1);
        }
    }
}
