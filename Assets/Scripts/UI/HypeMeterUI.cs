using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class HypeMeterUI : MonoBehaviour
{
    [Header("Source")]
    public HypeMeterController hypeMeter;

    [Header("UI References")]
    public Slider hypeSlider;
    public Image fillImage;       // Slider fill rect Image
    public TextMeshProUGUI stateLabel;

    [Header("State Colors")]
    public Color weakColor = Color.red;
    public Color strugglingColor = Color.yellow;
    public Color strongColor = new Color(0.1f, 0.8f, 0.1f);
    public Color invincibleColor = Color.cyan;

    private static readonly string[] StateLabels =
    {
        "TOO WEAK!",
        "STRUGGLING...",
        "STRONG!",
        "INVINCIBLE!"
    };


    private void Awake()
    {
        if (hypeMeter == null) return;
        hypeMeter.onNormalizedHypeChanged.AddListener(OnNormalizedHypeChanged);
        hypeMeter.onStateChanged.AddListener(OnHypeStateChanged);
    }

    private void OnDestroy()
    {
        if (hypeMeter == null) return;
        hypeMeter.onNormalizedHypeChanged.RemoveListener(OnNormalizedHypeChanged);
        hypeMeter.onStateChanged.RemoveListener(OnHypeStateChanged);
    }


    private void OnNormalizedHypeChanged(float normalized)
    {
        if (hypeSlider != null)
            hypeSlider.value = normalized;
    }

    private void OnHypeStateChanged(HypeState state)
    {
        if (fillImage != null)
            fillImage.color = StateColor(state);

        if (stateLabel != null)
            stateLabel.text = StateLabels[(int)state];
    }


    private Color StateColor(HypeState state) => state switch
    {
        HypeState.Weak => weakColor,
        HypeState.Struggling => strugglingColor,
        HypeState.Strong => strongColor,
        HypeState.Invincible => invincibleColor,
        _ => Color.white
    };
}
