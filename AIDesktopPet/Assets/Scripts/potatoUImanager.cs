using UnityEngine;
using UnityEngine.UI;

public class PomodoroUIController : MonoBehaviour
{
    [Header("按钮引用")]
    public Button showPomodoroButton; // 显示番茄钟的按钮

    [Header("番茄钟UI")]
    public GameObject pomodoroPanel; // 番茄钟UI面板
    public Button closePomodoroButton; // 关闭番茄钟的按钮（可选）

    void Start()
    {
        // 绑定按钮点击事件
        showPomodoroButton.onClick.AddListener(ShowPomodoroUI);

        // 如果有关闭按钮，也绑定事件
        if (closePomodoroButton != null)
        {
            closePomodoroButton.onClick.AddListener(HidePomodoroUI);
        }

        // 初始时隐藏番茄钟UI
        HidePomodoroUI();
    }

    /// <summary>
    /// 显示番茄钟UI
    /// </summary>
    public void ShowPomodoroUI()
    {
        pomodoroPanel.SetActive(true);
    }

    /// <summary>
    /// 隐藏番茄钟UI
    /// </summary>
    public void HidePomodoroUI()
    {
        pomodoroPanel.SetActive(false);
    }

    /// <summary>
    /// 切换番茄钟UI显示状态
    /// </summary>
    public void TogglePomodoroUI()
    {
        pomodoroPanel.SetActive(!pomodoroPanel.activeSelf);
    }
}