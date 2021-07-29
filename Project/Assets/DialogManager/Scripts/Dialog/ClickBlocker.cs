using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// クリックブロッカー
/// </summary>
public class ClickBlocker : MonoBehaviour
{
    public bool IsActive { get; private set; } = false;

    [SerializeField] private Image image = null;
    [SerializeField][Min(0)] private float fadeTime = 0.1f;
    [SerializeField][Range(0, 255)] private int alpha = 100;

    private float playTime_ = 0.0f;

    // Update is called once per frame
    void Update()
    {
        if (playTime_ == fadeTime)
        {
            return;
        }

        // アルファ値変更
        playTime_ += Time.deltaTime;
        playTime_ = Mathf.Min(playTime_, fadeTime);
        Color color = image.color;
        if (IsActive) { color.a = (playTime_ / fadeTime) * GetMaxAlphafloat(); }
        else { color.a = (1.0f - (playTime_ / fadeTime)) * GetMaxAlphafloat(); }
        image.color = color;

        // 画像非表示
        if (playTime_ == fadeTime && !IsActive)
        {
            image.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// アクティブ状態切替
    /// </summary>
    /// <param name="active"></param>
    /// <param name="fade"></param>
    public void SetActive(bool active)
    {
        Color color = image.color;
        float time = (color.a / GetMaxAlphafloat()) * fadeTime;
        if (active)
        {
            playTime_ = time;
            color.a = (playTime_ / fadeTime) * GetMaxAlphafloat();
            image.gameObject.SetActive(true);
        }
        else
        {
            playTime_ = fadeTime - time;
            color.a = (1.0f - (playTime_ / fadeTime)) * GetMaxAlphafloat();
            if (playTime_ == fadeTime) { image.gameObject.SetActive(false); }
        }

        IsActive = active;
    }

    /// <summary>
    /// アルファ値
    /// </summary>
    /// <returns></returns>
    private float GetMaxAlphafloat()
    {
        return ((float)alpha / 255.0f);
    }
}
