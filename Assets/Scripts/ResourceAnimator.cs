using DG.Tweening;
using TMPro;
using UnityEngine;

public class ResourceAnimator : MonoBehaviour
{
    public static ResourceAnimator Instance { get; private set; }

    private Sequence coinsSequence;
    private Sequence diamondsSequence;
    public TextMeshProUGUI diamondText;
    public TextMeshProUGUI coinsText;

    private Color originalColor;
    private Vector3 originalScale;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Если нужно сохранить объект между сценами
        }

        // Сохраняем оригинальные значения для восстановления
        originalColor = diamondText.color;
        originalScale = diamondText.transform.localScale;
    }

    public void AnimateResourceChange(int startValue, int endValue, bool isCoins)
    {
        // Если значение не изменилось, просто выходим из метода
        if (startValue == endValue)
        {
            return;
        }

        TextMeshProUGUI text = isCoins ? coinsText : diamondText;

        // Определяем увеличение или уменьшение ресурса
        bool isIncreasing = endValue > startValue;
        Color targetColor = isIncreasing ? Color.green : Color.red;
        Vector3 targetScale = isIncreasing ? originalScale * 1.2f : originalScale * 0.8f;

        // Создаем новую последовательность
        Sequence sequence = DOTween.Sequence();

        // Первая половина — изменение текста и увеличение/уменьшение с изменением цвета
        sequence.Append(DOTween.To(() => startValue, x => text.text = x.ToString(), endValue, 0.5f)
            .SetEase(Ease.OutQuad)
        );

        // Параллельно изменяем масштаб и цвет текста
        sequence.Join(text.transform.DOScale(targetScale, 0.25f).SetEase(Ease.OutQuad));
        sequence.Join(text.DOColor(targetColor, 0.25f).SetEase(Ease.OutQuad));

        // Вторая половина — возвращаем масштаб и цвет в исходное состояние
        sequence.Append(text.transform.DOScale(originalScale, 0.25f).SetEase(Ease.InQuad));
        sequence.Join(text.DOColor(originalColor, 0.25f).SetEase(Ease.InQuad));
    }

}
