using UnityEngine;
using DG.Tweening;

public class HoverAnimation : MonoBehaviour
{
    private Vector3 initialPosition;
    private bool isHovered = false;
    private float rotateSpeed = 90f;

    private Tween moveTween;

    void Start()
    {
        initialPosition = transform.position;
    }

    void OnMouseEnter()
    {
        if (!isHovered)
        {
            isHovered = true;

            moveTween = transform.DORotate(new Vector3(0, 360, 0), 2f, RotateMode.FastBeyond360)
                .SetLoops(-1, LoopType.Restart)
                .SetEase(Ease.Linear);
        }
    }

    void OnMouseExit()
    {
        if (isHovered)
        {
            isHovered = false;
            moveTween?.Kill();
            transform.rotation = Quaternion.identity;
        }
    }
}
