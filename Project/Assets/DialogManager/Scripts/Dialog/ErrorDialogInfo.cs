using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// エラー情報
/// </summary>
public class ErrorDialogInfo
{
    public enum ErrorType
    {
        Unknow,     // 不明なエラー
        Network,    // ネットワークエラー
    }

    public string Title { get; set; } = "";
    public string Message { get; set; } = "";
    public Action Callback { get; private set; } = null;

    public ErrorDialogInfo(ErrorType errorType, Action callback = null)
    {
        // エラー情報設定
        switch (errorType)
        {
            case ErrorType.Unknow:
                Title = "不明なエラー";
                Message = "不明なエラーが発生しました。";
                break;
            case ErrorType.Network:
                Title = "ネットワークエラー";
                Message = "ネットワークエラーが発生しました。";
                break;
        }

        // コールバック設定(未設定の場合はデフォルトのコールバックを設定)
        Callback = callback;
        if (Callback == null)
        {
            Callback = OnTitleBack;
        }
    }

    private void OnTitleBack() {}
}
