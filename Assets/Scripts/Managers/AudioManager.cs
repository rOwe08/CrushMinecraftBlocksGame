using UnityEngine;

public class AudioManager : MonoBehaviour
{
    private AudioSource audioSource;

    void Awake()
    {
        // Проверяем, есть ли на объекте AudioSource, и добавляем, если его нет
        if (gameObject.GetComponent<AudioSource>() == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        else
        {
            audioSource = gameObject.GetComponent<AudioSource>();
        }

        // Настройки для 2D звука
        audioSource.spatialBlend = 0f;  // Устанавливаем 2D режим
        audioSource.volume = 1.0f;      // Убедитесь, что громкость установлена на 100%

        // Дополнительные настройки, если нужно
        audioSource.loop = false;       // Чтобы не зацикливать звук
    }

    // Функция для проигрывания звука один раз
    public void PlaySoundOnce(AudioClip clip)
    {
        if (clip != null && audioSource != null)
        {
            audioSource.PlayOneShot(clip);  // Проигрываем звук
        }
    }
}
