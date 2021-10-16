using System;
using UnityEngine;

/// <summary>
/// ダイアログのオート生成
/// </summary>
public class AutoDialogSample : MonoBehaviour
{
    [SerializeField] private float generalInterval = 2.0f;
    [SerializeField] private float errorInterval = 10.0f;

    private float playTime_ = 0.0f;

    // Update is called once per frame
    void Update()
    {
        int num = (int)(playTime_ / generalInterval);
        playTime_ += Time.deltaTime;
        int nextNum = (int)(playTime_ / generalInterval);

        if (num != nextNum)
        {
            if (playTime_ >= errorInterval)
            {
                string message = String.Format("このメッセージは[{0}]に送信されました\n\nエラーダイアログは他のダイアログより優先表示されます\n\nエラーダイアログ以外のダイアログは削除されます", DateTime.Now);
                ErrorDialogInfo info = new ErrorDialogInfo(ErrorDialogInfo.ErrorType.Unknow);
                info.Message = message;
                DialogManager.Instance.CreateErrorDialog(info);
                playTime_ = 0.0f;
            }
            else
            {
                string message = String.Format("このメッセージは[{0}]に送信されました\n\nメッセージはキューに溜まります", DateTime.Now);
                DialogManager.Instance.CreateGeneralDialogOkOnly("テスト", message);
            }
        }
    }
}
