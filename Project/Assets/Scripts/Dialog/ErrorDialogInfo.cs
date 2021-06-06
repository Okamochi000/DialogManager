using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �G���[���
/// </summary>
public class ErrorDialogInfo
{
    public enum ErrorType
    {
        Unknow,     // �s���ȃG���[
        Network,    // �l�b�g���[�N�G���[
    }

    public string Title { get; set; } = "";
    public string Message { get; set; } = "";
    public Action Callback { get; private set; } = null;

    public ErrorDialogInfo(ErrorType errorType, Action callback = null)
    {
        // �G���[���ݒ�
        switch (errorType)
        {
            case ErrorType.Unknow:
                Title = "�s���ȃG���[";
                Message = "�s���ȃG���[���������܂����B";
                break;
            case ErrorType.Network:
                Title = "�l�b�g���[�N�G���[";
                Message = "�l�b�g���[�N�G���[���������܂����B";
                break;
        }

        // �R�[���o�b�N�ݒ�(���ݒ�̏ꍇ�̓f�t�H���g�̃R�[���o�b�N��ݒ�)
        Callback = callback;
        if (Callback == null)
        {
            Callback = OnTitleBack;
        }
    }

    private void OnTitleBack() {}
}
