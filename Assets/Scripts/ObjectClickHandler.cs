using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class ObjectClickHandler : MonoBehaviour
{
    private GameObject uiWindow;    // Ссылка на UI-окно, которое будет всплывать
    private Vector3 initialPosition;  // Исходная позиция объекта
    private float liftHeight = 0.5f;  // Высота подъема объекта
    private float liftDuration = 0.3f; // Длительность анимации подъема
    private Rigidbody rb;  // Rigidbody для отключения гравитации

    private bool isLifted = false; // Флаг для отслеживания состояния объекта (поднят или нет)

    void Start()
    {
        // Сохраняем исходную позицию объекта
        initialPosition = transform.position;

        // Получаем Rigidbody компонента, чтобы управлять гравитацией
        rb = GetComponent<Rigidbody>();

        uiWindow = UIManager.Instance.buyProjectilePanel;
        // Окно UI должно быть изначально скрыто
        uiWindow.SetActive(false);
    }

    void OnMouseDown()
    {
        // Поднимаем объект только, если он не поднят
        if (!isLifted)
        {
            if (GameManager.Instance.chosenProjectileInShop != null)
            {
                GameManager.Instance.chosenProjectileInShop.GetComponent<ObjectClickHandler>().LowerObject();
            }

            GameManager.Instance.chosenProjectileInShop = this.gameObject;
            // Отключаем гравитацию и замораживаем движение по оси Y
            rb.useGravity = false;
            rb.constraints = RigidbodyConstraints.FreezePositionY;

            // Поднимаем объект
            transform.DOMoveY(initialPosition.y + liftHeight, liftDuration).OnComplete(() =>
            {
                isLifted = true; // Устанавливаем флаг, что объект поднят
            });

            // Включаем UI окно
            UIManager.Instance.PrepareProjectileShopPanel(this.gameObject);
            uiWindow.SetActive(true);
        }
        else
        {
            LowerObject();
        }

    }

    // Метод для опускания объекта (будет вызван при нажатии на кнопку в UI)
    public void LowerObject()
    {
        // Опускаем объект только, если он поднят
        if (isLifted)
        {
            rb.useGravity = true;
            rb.constraints = RigidbodyConstraints.None;
            uiWindow.SetActive(false);
            isLifted = false;

        }
    }
}
