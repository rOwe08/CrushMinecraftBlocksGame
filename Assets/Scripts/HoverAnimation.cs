using UnityEngine;
using DG.Tweening;

public class HoverAnimation : MonoBehaviour
{
    private Vector3 initialPosition;    // Исходная позиция объекта
    private bool isHovered = false;     // Флаг, показывающий, навели ли мышку на объект
    private float hoverHeight = 0.5f;   // Высота, на которую поднимаем объект
    private float hoverDuration = 0.3f; // Длительность анимации подъема
    private float rotateSpeed = 90f;    // Скорость вращения объекта при наведении

    private Tween moveTween;            // Сохраняем ссылку на tween движения

    void Start()
    {
        // Сохраняем исходную позицию объекта
        initialPosition = transform.position;
    }

    void OnMouseEnter()
    {
        if (!isHovered)
        {
            isHovered = true;

            // Поднимаем объект на hoverHeight
            moveTween?.Kill(); // Останавливаем предыдущую анимацию, если есть

            // Начинаем вращение объекта вокруг оси Y
            transform.DORotate(new Vector3(0, 360, 0), 2f, RotateMode.FastBeyond360)
                .SetLoops(-1, LoopType.Restart)
                .SetEase(Ease.Linear);
        }
    }

    void OnMouseExit()
    {
        if (isHovered)
        {
            isHovered = false;

            // Возвращаем объект в исходную позицию
            moveTween?.Kill(); // Останавливаем предыдущую анимацию, если есть

            // Останавливаем вращение
            transform.DOKill();  // Останавливает все анимации на объекте
            transform.rotation = Quaternion.identity;  // Возвращаем начальный угол
        }
    }
}
