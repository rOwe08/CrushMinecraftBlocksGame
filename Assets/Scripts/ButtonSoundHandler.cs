using UnityEngine;
using UnityEngine.UI;

public class ButtonSoundHandler : MonoBehaviour
{
    public AudioClip buttonClickSound;  // Звук для воспроизведения при нажатии
    private AudioSource audioSource;    // Источник звука

    void Start()
    {
        // Получаем компонент AudioSource (или добавляем его, если его нет)
        audioSource = gameObject.AddComponent<AudioSource>();

        // Если указан звук, присваиваем его AudioSource
        if (buttonClickSound != null)
        {
            audioSource.clip = buttonClickSound;
        }

        // Добавляем обработчик нажатия на кнопку
        Button button = GetComponent<Button>();
        if (button != null)
        {
            button.onClick.AddListener(PlayClickSound);
        }
    }

    // Метод для воспроизведения звука
    void PlayClickSound()
    {
        if (audioSource != null && buttonClickSound != null)
        {
            audioSource.PlayOneShot(buttonClickSound);  // Проигрываем звук
        }
    }
}
