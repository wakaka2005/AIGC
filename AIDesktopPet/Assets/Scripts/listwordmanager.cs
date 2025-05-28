using UnityEngine;
using UnityEngine.UI;

public class ResettableTextField : MonoBehaviour
{
    [Header("UI References")]
    public InputField textInputField;  // ���������
    public Button resetButton;         // ���ð�ť

    [Header("Settings")]
    public string placeholderText = "����������...";  // �հ���ʾ����

    void Start()
    {
        // ȷ������ѷ���
        if (textInputField == null || resetButton == null)
        {
            Debug.LogError("�����UI���!");
            return;
        }

        // ���ó�ʼ״̬
        textInputField.text = "";
        textInputField.placeholder.GetComponent<Text>().text = placeholderText;

        // �󶨰�ť�¼�
        resetButton.onClick.AddListener(ResetTextField);
    }

    // �����ı��ֶ�
    void ResetTextField()
    {
        // �������
        textInputField.text = "";

        // ���������
        textInputField.ActivateInputField();

        // ���ƶ��豸����ʾ����
#if UNITY_IOS || UNITY_ANDROID
        TouchScreenKeyboard.Open("", TouchScreenKeyboardType.Default);
#endif
    }

    // ��ȡ��ǰ��������
    public string GetText()
    {
        return textInputField.text;
    }

    // ������������
    public void SetText(string newText)
    {
        textInputField.text = newText;
    }
}
