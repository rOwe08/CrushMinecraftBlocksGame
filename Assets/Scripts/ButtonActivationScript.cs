using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class ButtonActivationSctipt : MonoBehaviour
{
    private Button button;
    private Vector3 originalScale;
    private bool previousInteractableState;

    void Start()
    {
        button = GetComponent<Button>();
        originalScale = transform.localScale;
        previousInteractableState = button.interactable; // Сохраняем начальное состояние
    }

    void Update()
    {
        // Проверяем, изменилась ли доступность кнопки
        if (button.interactable != previousInteractableState)
        {
            previousInteractableState = button.interactable;

            if (button.interactable)
            {
                AnimateButton();
            }
        }
    }

    private void AnimateButton()
    {
        // Увеличение кнопки и возврат с баунсом
        transform.DOScale(originalScale * 1.2f, 0.5f).OnComplete(() =>
        {
            transform.DOScale(originalScale, 0.2f).SetEase(Ease.OutBounce);
        });
    }
}
