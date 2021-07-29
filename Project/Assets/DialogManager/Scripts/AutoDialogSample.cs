using System;
using UnityEngine;

/// <summary>
/// �_�C�A���O�̃I�[�g����
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
                string message = String.Format("���̃��b�Z�[�W��[{0}]�ɑ��M����܂���\n\n�G���[�_�C�A���O�͑��̃_�C�A���O���D��\������܂�\n\n�G���[�_�C�A���O�ȊO�̃_�C�A���O�͍폜����܂�", DateTime.Now);
                ErrorDialogInfo info = new ErrorDialogInfo(ErrorDialogInfo.ErrorType.Unknow);
                info.Message = message;
                DialogManager.Instance.CreateErrorDialog(info);
                playTime_ = 0.0f;
            }
            else
            {
                string message = String.Format("���̃��b�Z�[�W��[{0}]�ɑ��M����܂���\n\n���b�Z�[�W�̓L���[�ɗ��܂�܂�", DateTime.Now);
                DialogManager.Instance.CreateGeneralDialogOkOnly("�e�X�g", message);
            }
        }
    }
}
