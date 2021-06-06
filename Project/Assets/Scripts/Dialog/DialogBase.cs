using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// ダイアログベース
/// </summary>
public class DialogBase : MonoBehaviour
{
    /// <summary>
    /// 開閉状態
    /// </summary>
    public enum OpenAndClosedState
    {
        None,       // 開いていない
        Open,       // 開いている途中
        Playing,    // 開き終えている
        Close,      // 閉じている途中
        Destory     // 閉じ終えた
    }

    public OpenAndClosedState State { get; private set; } = OpenAndClosedState.None;

    private bool isCloseAnim = false;

    /// <summary>
    /// 開く
    /// </summary>
    public void Open()
    {
        if (State == OpenAndClosedState.None)
        {
            if (EventSystem.current != null) { EventSystem.current.SetSelectedGameObject(null); }
            this.gameObject.SetActive(true);
            StartCoroutine(WaitOpenAnimation());
        }
    }

    /// <summary>
    /// 閉じる
    /// </summary>
    public void Close()
    {
        if (State == OpenAndClosedState.None)
        {
            // 開かれていない場合はそのまま閉じる
            State = OpenAndClosedState.Destory;
            this.gameObject.SetActive(false);
        }
        else if (State == OpenAndClosedState.Open || State == OpenAndClosedState.Playing)
        {
            // 閉じるアニメーションを開始する
            StartCoroutine(WaitCloseAnimation());
        }
    }

    /// <summary>
    /// 閉じるアニメーションが終了したときのコールバック
    /// </summary>
    protected virtual void OnClosed() { }

    /// <summary>
    /// 開くアニメーション待ち
    /// </summary>
    /// <returns></returns>
    private IEnumerator WaitOpenAnimation()
    {
        State = OpenAndClosedState.Open;
        Animator animator = this.GetComponent<Animator>();
        if (animator != null && animator.runtimeAnimatorController != null)
        {
            animator.SetBool("IsOpen", true);
            AnimatorStateInfo animInfo = animator.GetCurrentAnimatorStateInfo(0);
            while (animInfo.normalizedTime < 1.0f)
            {
                yield return null;
                animInfo = animator.GetCurrentAnimatorStateInfo(0);
            }
        }

        State = OpenAndClosedState.Playing;
    }

    /// <summary>
    /// 閉じるアニメーション終了待ち
    /// </summary>
    /// <returns></returns>
    private IEnumerator WaitCloseAnimation()
    {
        if (isCloseAnim) { yield break; }

        isCloseAnim = true;

        // 開かれるまで待つ
        while (State != OpenAndClosedState.Playing)
        {
            yield return null;
        }

        State = OpenAndClosedState.Close;
        Animator animator = this.GetComponent<Animator>();
        if (animator != null && animator.runtimeAnimatorController != null)
        {
            animator.SetBool("IsOpen", false);
            AnimatorStateInfo animInfo = animator.GetCurrentAnimatorStateInfo(0);

            // アニメーション終了待ち
            while (animInfo.normalizedTime < 1.0f)
            {
                yield return null;
                animInfo = animator.GetCurrentAnimatorStateInfo(0);
            }
        }

        isCloseAnim = false;
        State = OpenAndClosedState.Destory;
        this.gameObject.SetActive(false);

        // 閉じられた時のコールバック
        OnClosed();
    }
}
