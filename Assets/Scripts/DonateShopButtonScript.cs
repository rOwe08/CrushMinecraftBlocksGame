using UnityEngine;
using UnityEngine.UI;

public class DonateShopButtonScripts : MonoBehaviour
{
    public Sprite backSprite; // Спрайт для возвращения
    public Sprite shopSprite; // Спрайт для магазина

    private Image buttonImage; // Компонент Image на кнопке

    void Start()
    {
        buttonImage = transform.Find("Image").GetComponent<Image>(); // Получаем ссылку на компонент Image

        CameraSwitch cameraSwitch = FindObjectOfType<CameraSwitch>();

        if (cameraSwitch != null)
        {
            // Подписываемся на UnityEvent
            cameraSwitch.OnCameraSwitched.AddListener(OnCameraSwitched);
        }

        LevelManager.Instance.OnLevelStarted.AddListener(OnLevelStarted);
    }

    // Метод, который будет вызываться при переключении камеры
    private void OnLevelStarted(bool isLevelStarted)
    {
        if (isLevelStarted)
        {
            transform.GetComponent<Button>().interactable = false;    
        }
        else
        {
            transform.GetComponent<Button>().interactable = true;
        }
    }

    // Метод, который будет вызываться при переключении камеры
    private void OnCameraSwitched(bool isShopCameraActive)
    {
        if (isShopCameraActive)
        {
            // Меняем спрайт кнопки на магазинный
            buttonImage.sprite = shopSprite;
            UIManager.Instance.HideProjectileBuyPanel();
            UIManager.Instance.ShowLevelsPanel();
        }
        else
        {
            // Меняем спрайт кнопки на основной
            buttonImage.sprite = backSprite;
            UIManager.Instance.HideLevelsPanel();
            UIManager.Instance.HideShopPanel();
            UIManager.Instance.HideResults();
        }
    }
}
