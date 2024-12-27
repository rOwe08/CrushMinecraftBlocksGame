using UnityEngine;
using Cinemachine;

public class CameraSwitch : MonoBehaviour
{
    public CinemachineVirtualCamera mainCamera;  // Камера для основной локации
    public CinemachineVirtualCamera shopCamera;  // Камера для магазина

    private bool isShopCameraActive = false;

    // Метод для переключения камеры
    public void SwitchCamera()
    {
        if (isShopCameraActive)
        {
            // Включаем основную камеру и выключаем магазинную
            mainCamera.Priority = 10;
            shopCamera.Priority = 5;

            GameManager.Instance.cameraPosition = 0;
        }
        else
        {
            // Включаем магазинную камеру и выключаем основную
            mainCamera.Priority = 5;
            shopCamera.Priority = 10;

            GameManager.Instance.cameraPosition = 1;
        }

        // Переключаем флаг
        isShopCameraActive = !isShopCameraActive;
    }
}
