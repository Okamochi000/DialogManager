using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// �_�C�A���O�x�[�X
/// </summary>
public class DialogBase : MonoBehaviour
{
    /// <summary>
    /// �J���
    /// </summary>
    public enum OpenAndClosedState
    {
        None,       // �J���Ă��Ȃ�
        Open,       // �J���Ă���r��
        Playing,    // �J���I���Ă���
        Close,      // ���Ă���r��
        Destory     // ���I����
    }

    public OpenAndClosedState State { get; private set; } = OpenAndClosedState.None;

    private bool isCloseAnim = false;

    /// <summary>
    /// �J��
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
    /// ����
    /// </summary>
    public void Close()
    {
        if (State == OpenAndClosedState.None)
        {
            // �J����Ă��Ȃ��ꍇ�͂��̂܂ܕ���
            State = OpenAndClosedState.Destory;
            this.gameObject.SetActive(false);
        }
        else if (State == OpenAndClosedState.Open || State == OpenAndClosedState.Playing)
        {
            // ����A�j���[�V�������J�n����
            StartCoroutine(WaitCloseAnimation());
        }
    }

    /// <summary>
    /// ����A�j���[�V�������I�������Ƃ��̃R�[���o�b�N
    /// </summary>
    protected virtual void OnClosed() { }

    /// <summary>
    /// �J���A�j���[�V�����҂�
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
    /// ����A�j���[�V�����I���҂�
    /// </summary>
    /// <returns></returns>
    private IEnumerator WaitCloseAnimation()
    {
        if (isCloseAnim) { yield break; }

        isCloseAnim = true;

        // �J�����܂ő҂�
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

            // �A�j���[�V�����I���҂�
            while (animInfo.normalizedTime < 1.0f)
            {
                yield return null;
                animInfo = animator.GetCurrentAnimatorStateInfo(0);
            }
        }

        isCloseAnim = false;
        State = OpenAndClosedState.Destory;
        this.gameObject.SetActive(false);

        // ����ꂽ���̃R�[���o�b�N
        OnClosed();
    }
}
