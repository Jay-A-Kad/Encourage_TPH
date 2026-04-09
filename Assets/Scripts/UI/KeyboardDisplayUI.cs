using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class KeyboardDisplayUI : MonoBehaviour
{
    [Header("Layout")]
    public float keySize = 36f;
    [Tooltip("Gap between keys horizontally")]
    public float keySpacing = 5f;
    [Tooltip("Gap between keyboard rows vertically")]
    public float rowSpacing = 5f;
    public float rowStagger = 12f;

    [Header("Visuals")]
    public Color defaultBackground = new Color(0.94f, 0.94f, 0.92f);  // off-white
    public Color defaultText = Color.black;
    public Color pressedBackground = Color.black;
    public Color pressedText = Color.white;

    [Header("Flash Timing")]
    [Range(0.05f, 0.5f)]
    public float pressDuration = 0.15f;

    [Header("Font")]
    public TMP_FontAsset font;


    private static readonly string[] Rows = { "QWERTYUIOP", "ASDFGHJKL", "ZXCVBNM" };

    private readonly Dictionary<char, KeyVisual> _keys = new();
    private Sprite _keySprite;

    private class KeyVisual
    {
        public Image background;
        public TextMeshProUGUI label;
        public Coroutine resetRoutine;
    }


    private void Start()
    {
        _keySprite = BuildRoundedSprite(64, 64, cornerRadius: 12);
        SpawnKeys();
    }

    private void Update()
    {
        // Input.inputString captures 
        foreach (char c in Input.inputString)
        {
            char upper = char.ToUpper(c);
            if (_keys.TryGetValue(upper, out KeyVisual kv))
                FlashKey(kv);
        }
    }


    private void SpawnKeys()
    {
        float step = keySize + keySpacing;

        for (int row = 0; row < Rows.Length; row++)
        {
            string letters = Rows[row];

            // Center each row horizontally, then apply stagger offset
            float rowWidth = letters.Length * keySize + (letters.Length - 1) * keySpacing;
            float startX = -rowWidth * 0.5f + keySize * 0.5f + row * rowStagger;

            // Row 0  that is (Q-P) is at the top & row 2 is (Z-M) is at the bottom
            float posY = (Rows.Length - 1 - row) * (keySize + rowSpacing) + keySize * 0.5f;

            for (int col = 0; col < letters.Length; col++)
            {
                char letter = letters[col];
                float posX = startX + col * step;
                _keys[letter] = CreateKey(letter, new Vector2(posX, posY));
            }
        }


        var rt = GetComponent<RectTransform>();
        if (rt != null)
        {
            float totalW = Rows[0].Length * keySize + (Rows[0].Length - 1) * keySpacing + 24f;
            float totalH = Rows.Length * keySize + (Rows.Length - 1) * rowSpacing + 16f;
            rt.sizeDelta = new Vector2(totalW, totalH);
        }
    }

    private KeyVisual CreateKey(char letter, Vector2 anchoredPos)
    {

        var root = new GameObject(letter.ToString(), typeof(RectTransform));
        root.transform.SetParent(transform, false);

        var rt = root.GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(keySize, keySize);
        rt.anchoredPosition = anchoredPos;


        var img = root.AddComponent<Image>();
        img.sprite = _keySprite;
        img.type = Image.Type.Simple;
        img.color = defaultBackground;


        var labelGo = new GameObject("Label", typeof(RectTransform));
        labelGo.transform.SetParent(root.transform, false);

        var labelRt = labelGo.GetComponent<RectTransform>();
        labelRt.anchorMin = Vector2.zero;
        labelRt.anchorMax = Vector2.one;
        labelRt.offsetMin = Vector2.zero;
        labelRt.offsetMax = Vector2.zero;

        var tmp = labelGo.AddComponent<TextMeshProUGUI>();
        tmp.text = letter.ToString();
        tmp.color = defaultText;
        tmp.fontSize = keySize * 0.42f;
        tmp.fontStyle = FontStyles.Bold;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.enableAutoSizing = false;
        if (font != null) tmp.font = font;

        return new KeyVisual { background = img, label = tmp };
    }

    // ── Key flash ─────────────────────────────────────────────────────

    private void FlashKey(KeyVisual kv)
    {
        // Cancel any in-progress reset so rapid presses don't glitch
        if (kv.resetRoutine != null)
            StopCoroutine(kv.resetRoutine);

        kv.background.color = pressedBackground;
        kv.label.color = pressedText;
        kv.resetRoutine = StartCoroutine(ResetAfterDelay(kv));
    }

    private IEnumerator ResetAfterDelay(KeyVisual kv)
    {
        yield return new WaitForSeconds(pressDuration);
        kv.background.color = defaultBackground;
        kv.label.color = defaultText;
        kv.resetRoutine = null;
    }

    private static Sprite BuildRoundedSprite(int width, int height, int cornerRadius)
    {
        var tex = new Texture2D(width, height, TextureFormat.RGBA32, mipChain: false);
        tex.filterMode = FilterMode.Bilinear;
        tex.wrapMode = TextureWrapMode.Clamp;

        var pixels = new Color[width * height];
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                float alpha = CornerAlpha(x, y, width, height, cornerRadius);
                pixels[y * width + x] = new Color(1f, 1f, 1f, alpha);
            }
        }

        tex.SetPixels(pixels);
        tex.Apply();


        int b = cornerRadius;
        return Sprite.Create(
            tex,
            new Rect(0, 0, width, height),
            new Vector2(0.5f, 0.5f),
            pixelsPerUnit: 100f,
            extrude: 0,
            meshType: SpriteMeshType.FullRect,
            border: new Vector4(b, b, b, b));
    }


    private static float CornerAlpha(int px, int py, int w, int h, int r)
    {
        bool inCornerX = px < r || px >= w - r;
        bool inCornerY = py < r || py >= h - r;


        if (!inCornerX || !inCornerY) return 1f;

        // Corner circle center
        int cx = px < r ? r : w - r - 1;
        int cy = py < r ? r : h - r - 1;

        float dist = Mathf.Sqrt((px - cx) * (px - cx) + (py - cy) * (py - cy));


        return Mathf.Clamp01((float)r + 0.5f - dist);
    }
}
