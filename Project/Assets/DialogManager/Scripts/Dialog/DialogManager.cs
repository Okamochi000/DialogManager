using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ダイアログ管理
/// </summary>
public class DialogManager : MonoBehaviourSingleton<DialogManager>
{
    // ダイアログの種類
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
        // ダイアログが閉じられていない場合は無視
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

        // 次のダイアログを設定する
        activeDialog_ = GetNextDialog();
        if (activeDialog_ != null)
        {
            // 次のダイアログがある場合は表示
            activeDialog_.Open();
        }
        else
        {
            // ダイアログがない場合はフェードを非表示
            if (clickBlocker != null) { clickBlocker.SetActive(false); }
        }
    }

    /// <summary>
    /// 汎用ダイアログ生成
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
    /// 汎用ダイアログ生成
    /// </summary>
    /// <returns></returns>
    public GeneralDialog CreateGeneralDialogOkOnly(string title, string message, Action okCallback = null)
    {
        GeneralDialog generalDialog = CreateGeneralDialog(title, message, okCallback, null);
        generalDialog.SetButtonActive(GeneralDialog.ButtonType.Cancel, false);

        return generalDialog;
    }

    /// <summary>
    /// エラーダイアログ生成
    /// </summary>
    /// <returns></returns>
    public GeneralDialog CreateErrorDialog(ErrorDialogInfo info)
    {
        // 未指定の場合は不明なエラー
        if (info == null) { info = new ErrorDialogInfo(ErrorDialogInfo.ErrorType.Unknow); }

        // 既存のダイアログをすべて破棄
        ClearQueue();
        CloseActiveDialog();

        // エラーダイアログの生成
        errorDialogInfo_ = info;
        GeneralDialog generalDialog = CreateGeneralDialogOkOnly(info.Title, info.Message, OnErrorDialogCallback);

        return generalDialog;
    }

    /// <summary>
    /// ダイアログ生成
    /// </summary>
    /// <param name="dialogType"></param>
    /// <returns></returns>
    public DialogBase CreateDialog(DialogType dialogType)
    {
        // パス取得
        string resourcePath = "Dialogs/";
        switch (dialogType)
        {
            case DialogType.GeneralDialog: resourcePath += "GeneralDialog"; break;
            default: break;
        }

        // リソース読み込み
        GameObject resource = Resources.Load<GameObject>(resourcePath);
        if (resource == null) { return null; }

        // ダイアログ生成
        GameObject dialogObj = GameObject.Instantiate(resource, this.transform);
        DialogBase dialogBase = dialogObj.GetComponent<DialogBase>();

        if (activeDialog_ == null)
        {
            // ダイアログが表示されていない場合
            activeDialog_ = dialogBase;
            dialogObj.SetActive(true);
            dialogBase.Open();
            if (clickBlocker != null) { clickBlocker.SetActive(true); }
        }
        else
        {
            // 既に表示されているダイアログがある場合
            dialogObj.SetActive(false);
            dialogQueue_.Enqueue(dialogBase);
        }

        return dialogBase;
    }

    /// <summary>
    /// アクティブ状態のダイアログを閉じる
    /// </summary>
    public void CloseActiveDialog()
    {
        if (activeDialog_ != null)
        {
            activeDialog_.Close();
        }
    }

    /// <summary>
    /// キューに溜まっているダイアログを削除する
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
    /// エラーダイアログのコールバック
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
    /// 次のダイアログを取得する
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
