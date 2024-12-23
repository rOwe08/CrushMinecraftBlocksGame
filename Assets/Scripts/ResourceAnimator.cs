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
    }

    public void AnimateResourceChange(int startValue, int endValue, bool isCoins)
    {
        TextMeshProUGUI text = isCoins ? coinsText : diamondText;

        // Создаем новую последовательность каждый раз перед добавлением анимации
        Sequence sequence = DOTween.Sequence();

        sequence.Append(DOTween.To(() => startValue, x => text.text = x.ToString(), endValue, 0.5f)
            .SetEase(Ease.OutQuad)); // 0.5f — время анимации
    }
}
