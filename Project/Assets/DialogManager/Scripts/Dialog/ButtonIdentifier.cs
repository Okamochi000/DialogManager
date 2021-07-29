using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ボタン識別子
/// </summary>
public class ButtonIdentifier : MonoBehaviour
{
    public string Identifier { get; set; } = "";

    private Action<string> callback_ = null;

    /// <summary>
    /// ボタン識別子の追加
    /// </summary>
    /// <param name="button"></param>
    /// <param name="identifier"></param>
    /// <param name="callback"></param>
    /// <returns></returns>
    public static ButtonIdentifier AddButtonCallback(Button button, string identifier, Action<string> callback)
    {
        if (button == null) { return null; }

        ButtonIdentifier buttonIdentifier = button.gameObject.GetComponent<ButtonIdentifier>();
        if (buttonIdentifier == null) { buttonIdentifier = button.gameObject.AddComponent<ButtonIdentifier>(); }
        buttonIdentifier.Identifier = identifier;
        button.onClick.AddListener(buttonIdentifier.OnButtonCallback);
        buttonIdentifier.callback_ = callback;

        return buttonIdentifier;
    }

    /// <summary>
    /// ボタンコールバック
    /// </summary>
    /// <param name="buttonAction"></param>
    /// <returns></returns>
    private void OnButtonCallback()
    {
        if (callback_ != null)
        {
            callback_(Identifier);
        }
    }
}
