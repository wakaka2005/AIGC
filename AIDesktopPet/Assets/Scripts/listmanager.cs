using UnityEngine;
using UnityEngine.UI;

public class ToDoListManager : MonoBehaviour
{
    [Header("UI References")]
    public Button toggleButton;         // ������ʾ/���صİ�ť
    public GameObject toDoPanel;        // ���������������
    public Text buttonText;             // ��ť�ϵ��ı����

    [Header("Settings")]
    public string showText = "Show To-Do List";
    public string hideText = "Hide To-Do List";

    private void Start()
    {
        // ȷ����ʼ״̬��ȷ
        toDoPanel.SetActive(false);
        buttonText.text = showText;

        // �󶨰�ť����¼�
        toggleButton.onClick.AddListener(ToggleToDoList);
    }

    // �л������������ʾ״̬
    public void ToggleToDoList()
    {
        // �л����ļ���״̬
        bool isActive = !toDoPanel.activeSelf;
        toDoPanel.SetActive(isActive);

        // ���°�ť�ı�
        buttonText.text = isActive ? hideText : showText;
    }

    // ʾ��������´�������ķ���
    public void AddNewTask(string taskName)
    {
        // ���������չΪʵ�ʴ������������߼�
        Debug.Log($"Added new task: {taskName}");
    }
}