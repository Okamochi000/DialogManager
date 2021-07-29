using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �_�C�A���O�Ǘ�
/// </summary>
public class DialogManager : MonoBehaviourSingleton<DialogManager>
{
    // �_�C�A���O�̎��
    public enum DialogType
    {
        GeneralDialog
    }

    [SerializeField] private ClickBlocker clickBlocker = null;

    private Queue<DialogBase> dialogQueue_ = new Queue<DialogBase>();
    private DialogBase activeDialog_ = new DialogBase();
    private ErrorDialogInfo errorDialogInfo_ = null;

    // Update is called once per frame
    void Update()
    {
        // �_�C�A���O�������Ă��Ȃ��ꍇ�͖���
        if (activeDialog_ != null)
        {
            if (activeDialog_.State == DialogBase.OpenAndClosedState.Destory)
            {
                Destroy(activeDialog_.gameObject);
                activeDialog_ = null;
            }
            else
            {
                return;
            }
        }

        // ���̃_�C�A���O��ݒ肷��
        activeDialog_ = GetNextDialog();
        if (activeDialog_ != null)
        {
            // ���̃_�C�A���O������ꍇ�͕\��
            activeDialog_.Open();
        }
        else
        {
            // �_�C�A���O���Ȃ��ꍇ�̓t�F�[�h���\��
            if (clickBlocker != null) { clickBlocker.SetActive(false); }
        }
    }

    /// <summary>
    /// �ėp�_�C�A���O����
    /// </summary>
    /// <returns></returns>
    public GeneralDialog CreateGeneralDialog(string title, string message, Action okCallback = null, Action cancelCallback = null)
    {
        DialogBase dialogBase = CreateDialog(DialogType.GeneralDialog);
        GeneralDialog generalDialog = dialogBase.GetComponent<GeneralDialog>();
        generalDialog.SetTitle(title);
        generalDialog.SetMessage(message);
        generalDialog.SetButtonCallback(GeneralDialog.ButtonType.Ok, okCallback);
        generalDialog.SetButtonCallback(GeneralDialog.ButtonType.Cancel, cancelCallback);
        return generalDialog;
    }

    /// <summary>
    /// �ėp�_�C�A���O����
    /// </summary>
    /// <returns></returns>
    public GeneralDialog CreateGeneralDialogOkOnly(string title, string message, Action okCallback = null)
    {
        GeneralDialog generalDialog = CreateGeneralDialog(title, message, okCallback, null);
        generalDialog.SetButtonActive(GeneralDialog.ButtonType.Cancel, false);

        return generalDialog;
    }

    /// <summary>
    /// �G���[�_�C�A���O����
    /// </summary>
    /// <returns></returns>
    public GeneralDialog CreateErrorDialog(ErrorDialogInfo info)
    {
        // ���w��̏ꍇ�͕s���ȃG���[
        if (info == null) { info = new ErrorDialogInfo(ErrorDialogInfo.ErrorType.Unknow); }

        // �����̃_�C�A���O�����ׂĔj��
        ClearQueue();
        CloseActiveDialog();

        // �G���[�_�C�A���O�̐���
        errorDialogInfo_ = info;
        GeneralDialog generalDialog = CreateGeneralDialogOkOnly(info.Title, info.Message, OnErrorDialogCallback);

        return generalDialog;
    }

    /// <summary>
    /// �_�C�A���O����
    /// </summary>
    /// <param name="dialogType"></param>
    /// <returns></returns>
    public DialogBase CreateDialog(DialogType dialogType)
    {
        // �p�X�擾
        string resourcePath = "Dialogs/";
        switch (dialogType)
        {
            case DialogType.GeneralDialog: resourcePath += "GeneralDialog"; break;
            default: break;
        }

        // ���\�[�X�ǂݍ���
        GameObject resource = Resources.Load<GameObject>(resourcePath);
        if (resource == null) { return null; }

        // �_�C�A���O����
        GameObject dialogObj = GameObject.Instantiate(resource, this.transform);
        DialogBase dialogBase = dialogObj.GetComponent<DialogBase>();

        if (activeDialog_ == null)
        {
            // �_�C�A���O���\������Ă��Ȃ��ꍇ
            activeDialog_ = dialogBase;
            dialogObj.SetActive(true);
            dialogBase.Open();
            if (clickBlocker != null) { clickBlocker.SetActive(true); }
        }
        else
        {
            // ���ɕ\������Ă���_�C�A���O������ꍇ
            dialogObj.SetActive(false);
            dialogQueue_.Enqueue(dialogBase);
        }

        return dialogBase;
    }

    /// <summary>
    /// �A�N�e�B�u��Ԃ̃_�C�A���O�����
    /// </summary>
    public void CloseActiveDialog()
    {
        if (activeDialog_ != null)
        {
            activeDialog_.Close();
        }
    }

    /// <summary>
    /// �L���[�ɗ��܂��Ă���_�C�A���O���폜����
    /// </summary>
    public void ClearQueue()
    {
        while (dialogQueue_.Count > 0)
        {
            DialogBase dialog = dialogQueue_.Dequeue();
            if (dialog != null)
            {
                dialog.Close();
                Destroy(dialog.gameObject);
            }
        }
    }

    /// <summary>
    /// �G���[�_�C�A���O�̃R�[���o�b�N
    /// </summary>
    private void OnErrorDialogCallback()
    {
        if (errorDialogInfo_ != null)
        {
            Action callback = errorDialogInfo_.Callback;
            errorDialogInfo_ = null;
            ClearQueue();
            if (callback != null)
            {
                callback();
            }
        }
    }

    /// <summary>
    /// ���̃_�C�A���O���擾����
    /// </summary>
    /// <returns></returns>
    private DialogBase GetNextDialog()
    {
        while (dialogQueue_.Count > 0)
        {
            DialogBase nextDialog = dialogQueue_.Dequeue();
            if (nextDialog != null)
            {
                if (nextDialog.State == DialogBase.OpenAndClosedState.Destory)
                {
                    Destroy(nextDialog.gameObject);
                }
                else
                {
                    activeDialog_ = nextDialog;
                    return nextDialog;
                }
            }
        }

        return null;
    }
}
