using UnityEngine;

public class ProjectileScript : MonoBehaviour
{
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
            Destroy(gameObject, 1);
        }
        else if (collision.gameObject.CompareTag("Ground"))
        {
            Destroy(gameObject, 3);
        }
    }

    private void OnDestroy()
    {
        LevelManager.Instance.IsProjectileInScene = false;
    }
}
