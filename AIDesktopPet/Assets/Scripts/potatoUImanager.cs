using UnityEngine;
using UnityEngine.UI;

public class PomodoroUIController : MonoBehaviour
{
    [Header("��ť����")]
    public Button showPomodoroButton; // ��ʾ�����ӵİ�ť

    [Header("������UI")]
    public GameObject pomodoroPanel; // ������UI���
    public Button closePomodoroButton; // �رշ����ӵİ�ť����ѡ��

    void Start()
    {
        // �󶨰�ť����¼�
        showPomodoroButton.onClick.AddListener(ShowPomodoroUI);

        // ����йرհ�ť��Ҳ���¼�
        if (closePomodoroButton != null)
        {
            closePomodoroButton.onClick.AddListener(HidePomodoroUI);
        }

        // ��ʼʱ���ط�����UI
        HidePomodoroUI();
    }

    /// <summary>
    /// ��ʾ������UI
    /// </summary>
    public void ShowPomodoroUI()
    {
        pomodoroPanel.SetActive(true);
    }

    /// <summary>
    /// ���ط�����UI
    /// </summary>
    public void HidePomodoroUI()
    {
        pomodoroPanel.SetActive(false);
    }

    /// <summary>
    /// �л�������UI��ʾ״̬
    /// </summary>
    public void TogglePomodoroUI()
    {
        pomodoroPanel.SetActive(!pomodoroPanel.activeSelf);
    }
}