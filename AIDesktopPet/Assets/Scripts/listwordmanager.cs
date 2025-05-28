using UnityEngine;
using UnityEngine.UI;

public class ResettableTextField : MonoBehaviour
{
    [Header("UI References")]
    public InputField textInputField;  // 文字输入框
    public Button resetButton;         // 重置按钮

    [Header("Settings")]
    public string placeholderText = "请输入内容...";  // 空白提示文字

    void Start()
    {
        // 确保组件已分配
        if (textInputField == null || resetButton == null)
        {
            Debug.LogError("请分配UI组件!");
            return;
        }

        // 设置初始状态
        textInputField.text = "";
        textInputField.placeholder.GetComponent<Text>().text = placeholderText;

        // 绑定按钮事件
        resetButton.onClick.AddListener(ResetTextField);
    }

    // 重置文本字段
    void ResetTextField()
    {
        // 清空文字
        textInputField.text = "";

        // 激活输入框
        textInputField.ActivateInputField();

        // 在移动设备上显示键盘
#if UNITY_IOS || UNITY_ANDROID
        TouchScreenKeyboard.Open("", TouchScreenKeyboardType.Default);
#endif
    }

    // 获取当前文字内容
    public string GetText()
    {
        return textInputField.text;
    }

    // 设置文字内容
    public void SetText(string newText)
    {
        textInputField.text = newText;
    }
}
