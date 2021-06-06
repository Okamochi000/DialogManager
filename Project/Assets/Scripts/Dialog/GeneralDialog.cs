using System;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// �ėp�_�C�A���O
/// </summary>
public class GeneralDialog : DialogBase
{
    public enum ButtonType
    {
        Ok,
        Cancel
    }

    [SerializeField] private Text titleText = null;
    [SerializeField] private Text bodyText = null;
    [SerializeField] private Button okButton = null;
    [SerializeField] private Button cancelButton = null;

    private Action[] buttonCallbacks_ = new Action[Enum.GetValues(typeof(ButtonType)).Length];
    private Action selectedCallback_ = null;

    // Start is called before the first frame update
    void Start()
    {
        // �{�^���ݒ�
        foreach (ButtonType buttonType in Enum.GetValues(typeof(ButtonType)))
        {
            string identifier = ((int)buttonType).ToString();
            Button button = GetButton(buttonType);
            ButtonIdentifier.AddButtonCallback(button, identifier, OnButtonCallback);
            UpdateLayout(button.transform);
        }
    }

    /// <summary>
    /// �^�C�g����ݒ肷��
    /// </summary>
    /// <param name="title"></param>
    public void SetTitle(string title)
    {
        titleText.text = title;
    }

    /// <summary>
    /// ���b�Z�[�W��ݒ肷��
    /// </summary>
    /// <param name="message"></param>
    public void SetMessage(string message)
    {
        bodyText.text = message;
    }

    /// <summary>
    /// �{�^���̖��O��ݒ肷��
    /// </summary>
    /// <param name="buttonType"></param>
    /// <param name="name"></param>
    public void SetButtonName(ButtonType buttonType, string name)
    {
        Button button = GetButton(buttonType);
        Text buttonText = button.GetComponentInChildren<Text>(true);
        buttonText.text = name;
    }

    /// <summary>
    /// �{�^���̃R�[���o�b�N��ݒ肷��
    /// </summary>
    /// <param name="buttonType"></param>
    /// <param name="callback"></param>
    public void SetButtonCallback(ButtonType buttonType, Action callback)
    {
        buttonCallbacks_[(int)buttonType] = callback;
    }

    /// <summary>
    /// �{�^���̕\����Ԃ�ݒ肷��
    /// </summary>
    /// <param name="buttonType"></param>
    /// <param name="active"></param>
    public void SetButtonActive(ButtonType buttonType, bool active)
    {
        Button button = GetButton(buttonType);
        button.gameObject.SetActive(active);
    }

    /// <summary>
    /// ����ꂽ���̃R�[���o�b�N
    /// </summary>
    protected override void OnClosed()
    {
        base.OnClosed();

        // �{�^�����I������Ă�����R�[���o�b�N���Ăяo��
        if (selectedCallback_ != null)
        {
            selectedCallback_();
        }
    }

    /// <summary>
    /// �{�^�����擾����
    /// </summary>
    /// <param name="buttonType"></param>
    /// <returns></returns>
    private Button GetButton(ButtonType buttonType)
    {
        switch (buttonType)
        {
            case ButtonType.Ok: return okButton;
            case ButtonType.Cancel: return cancelButton;
            default: break;
        }

        return null;
    }

    /// <summary>
    /// �{�^���R�[���o�b�N
    /// </summary>
    /// <param name="identifier"></param>
    private void OnButtonCallback(string identifier)
    {
        if (State != OpenAndClosedState.Playing) { return; }

        int typeNum = 0;

        // �����Ȓl�̏ꍇ�̓L�����Z�������ɂ���
        if (!int.TryParse(identifier, out typeNum)) { typeNum = (int)ButtonType.Cancel; }
        else if (typeNum < 0 || typeNum >= Enum.GetValues(typeof(ButtonType)).Length) { typeNum = (int)ButtonType.Cancel; }
        selectedCallback_ = buttonCallbacks_[typeNum];

        // ����
        Close();
    }

    /// <summary>
    /// ���C�A�E�g�X�V
    /// </summary>
    private void UpdateLayout(Transform target)
    {
        HorizontalOrVerticalLayoutGroup layoutGroup = target.GetComponent<HorizontalOrVerticalLayoutGroup>();
        if (layoutGroup != null)
        {
            layoutGroup.CalculateLayoutInputHorizontal();
            layoutGroup.CalculateLayoutInputVertical();
            layoutGroup.SetLayoutHorizontal();
            layoutGroup.SetLayoutVertical();
        }

        ContentSizeFitter sizeFitter = target.GetComponent<ContentSizeFitter>();
        if (sizeFitter)
        {
            sizeFitter.SetLayoutHorizontal();
            sizeFitter.SetLayoutVertical();
        }
    }
}
