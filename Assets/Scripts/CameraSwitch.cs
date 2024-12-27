using UnityEngine;
using UnityEngine.Events;
using Cinemachine;

public class CameraSwitch : MonoBehaviour
{
    public CinemachineVirtualCamera mainCamera;
    public CinemachineVirtualCamera shopCamera;

    private bool isShopCameraActive = false;

    // UnityEvent для события переключения камеры
    public UnityEvent<bool> OnCameraSwitched;

    public void SwitchCamera()
    {
        if (isShopCameraActive)
        {
            mainCamera.Priority = 10;
            shopCamera.Priority = 5;
            GameManager.Instance.cameraPosition = 0;
        }
        else
        {
            mainCamera.Priority = 5;
            shopCamera.Priority = 10;
            GameManager.Instance.cameraPosition = 1;
        }

        // Вызываем UnityEvent и передаем флаг активности камеры
        if (OnCameraSwitched != null)
        {
            OnCameraSwitched.Invoke(isShopCameraActive);
        }

        isShopCameraActive = !isShopCameraActive;
    }
}
