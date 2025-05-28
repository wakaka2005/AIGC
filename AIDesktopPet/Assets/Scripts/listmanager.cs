using UnityEngine;
using UnityEngine.UI;

public class ToDoListManager : MonoBehaviour
{
    [Header("UI References")]
    public Button toggleButton;         // 控制显示/隐藏的按钮
    public GameObject toDoPanel;        // 待办事项面板物体
    public Text buttonText;             // 按钮上的文本组件

    [Header("Settings")]
    public string showText = "Show To-Do List";
    public string hideText = "Hide To-Do List";

    private void Start()
    {
        // 确保初始状态正确
        toDoPanel.SetActive(false);
        buttonText.text = showText;

        // 绑定按钮点击事件
        toggleButton.onClick.AddListener(ToggleToDoList);
    }

    // 切换待办事项的显示状态
    public void ToggleToDoList()
    {
        // 切换面板的激活状态
        bool isActive = !toDoPanel.activeSelf;
        toDoPanel.SetActive(isActive);

        // 更新按钮文本
        buttonText.text = isActive ? hideText : showText;
    }

    // 示例：添加新待办事项的方法
    public void AddNewTask(string taskName)
    {
        // 这里可以扩展为实际创建任务对象的逻辑
        Debug.Log($"Added new task: {taskName}");
    }
}