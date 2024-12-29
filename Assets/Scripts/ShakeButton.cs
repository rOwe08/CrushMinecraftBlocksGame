using UnityEngine;
using UnityEngine.UI;
using DG.Tweening; // Не забудьте добавить DoTween

public class ShakeButton : MonoBehaviour
{
    public float shakeDuration = 1f; // Длительность тряски
    public float shakeStrength = 30f; // Сила тряски (амплитуда)
    public int shakeVibrato = 10; // Количество колебаний
    public float shakeRandomness = 90f; // Степень случайности движения
    public float delayBetweenShakes = 3f; // Задержка между повторами

    private RectTransform buttonRectTransform;

    void Start()
    {
        buttonRectTransform = GetComponent<RectTransform>();

        // Запускаем бесконечную анимацию тряски с задержкой
        StartShakeAnimation();
    }

    private void StartShakeAnimation()
    {
        // Анимация тряски
        buttonRectTransform.DOShakePosition(shakeDuration, shakeStrength, shakeVibrato, shakeRandomness, false, true)
            .SetLoops(-1, LoopType.Yoyo) // Бесконечный цикл с эффектом йо-йо (движение туда-обратно)
            .SetDelay(delayBetweenShakes); // Задержка между циклами
    }
}
