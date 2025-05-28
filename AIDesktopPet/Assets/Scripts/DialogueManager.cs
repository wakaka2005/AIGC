using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;


public class DialogueManager : MonoBehaviour
{
    public GameObject dialoguePanel;
    public Text petReplyText;
    public InputField inputField;
    public Button sendButton;

    public float idleTimeout = 10f; // 超时时间（秒）
    private Coroutine autoCloseCoroutine;

    public static DialogueManager Instance;
    public bool isWaitingForAI = false;

    private void Awake()
    {
        Instance = this;
        dialoguePanel.SetActive(false); // 默认隐藏
    }

    private void Start()
    {
        sendButton.onClick.AddListener(OnSendClicked);
        // 👇 新增：监听输入变化，重置倒计时
        inputField.onValueChanged.AddListener((text) => RestartIdleTimer());
    }

    public void ShowDialogue()
    {
        dialoguePanel.SetActive(true);
        petReplyText.text = "嗨！今天想聊点什么呢？";

        RestartIdleTimer(); // 显示时启动倒计时
    }

    void OnSendClicked()
    {
        string userInput = inputField.text;
        inputField.text = "";

        if (!string.IsNullOrEmpty(userInput))
        {
            petReplyText.text = "🤖 AI 正在思考中...";
            isWaitingForAI = true;
            RestartIdleTimer();

            // ✅ 获取当前完整时间信息
            string fullDate = DateTime.Now.ToString("yyyy年M月d日");
            string weekDay = DateTime.Now.ToString("dddd");
            string currentTime = DateTime.Now.ToString("HH:mm");

            // ✅ 请求 MCP 获取节日信息
            MCPController.Instance.CheckTodayFestival((festivalMessage) =>
            {
                // 拼接完整的 prompt 给 DeepSeek
                string prompt =
                    $"{festivalMessage} 今天是 {fullDate}，{weekDay}，现在是 {currentTime}。" +
                    $"用户说：{userInput}";

                Debug.Log("📤 发送给 AI 的完整内容：" + prompt);

                DeepSeekChatManager.Instance.SendMessageToAI(prompt, (reply) =>
                {
                    petReplyText.text = reply;
                    isWaitingForAI = false;
                    RestartIdleTimer();
                });
            });
        }
    }


    public void OnScrollViewChanged(Vector2 position)
    {
        // 用户滑动时 → 重置计时
        RestartIdleTimer();
    }
    public void ToggleDialogue()
    {
        if (dialoguePanel.activeSelf)
        {
            dialoguePanel.SetActive(false);
            Debug.Log("🔽 点击按钮关闭对话面板");
        }
        else
        {
            ShowDialogue(); // 原有逻辑
            Debug.Log("🔼 点击按钮打开对话面板");
        }
    }


    public void RestartIdleTimer()
    {
        if (autoCloseCoroutine != null)
        {
            StopCoroutine(autoCloseCoroutine);
        }
        autoCloseCoroutine = StartCoroutine(AutoCloseCoroutine());
    }

    IEnumerator AutoCloseCoroutine()
    {
        float elapsed = 0f;

        while (elapsed < idleTimeout)
        {
            if (!isWaitingForAI)
            {
                elapsed += Time.deltaTime;
            }
            yield return null;
        }

        dialoguePanel.SetActive(false);
        Debug.Log("🕓 长时间未操作，自动关闭对话面板");
    }

}
