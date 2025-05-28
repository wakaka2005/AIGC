using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Linq;

[System.Serializable]
public class ChatMessage
{
    public string role;
    public string content;
}

public class DeepSeekChatManager : MonoBehaviour
{
    public static DeepSeekChatManager Instance;

    [Header("DeepSeek API Key")]
    public string apiKey = "dsk-你的真实Key";

    private string apiUrl = "https://api.deepseek.com/v1/chat/completions";
    private List<ChatMessage> conversationHistory = new List<ChatMessage>();

    // 初始化时可加入系统设定（可选）
    private void Start()
    {
        conversationHistory.Clear();
        conversationHistory.Add(new ChatMessage { role = "system", content = "你是一只可爱的猫咪助手，用亲切温柔的语气回答玩家问题。" });
    }

    void Awake()
    {
        Instance = this;
    }

    public void SendMessageToAI(string userInput, System.Action<string> onReply)
    {
        Debug.Log($"📤 正在发送用户内容到 DeepSeek: {userInput}");

        // 添加用户发言到历史
        conversationHistory.Add(new ChatMessage { role = "user", content = userInput });

        StartCoroutine(SendRequest(conversationHistory, (aiReply) =>
        {
            // 添加 AI 回复到历史
            conversationHistory.Add(new ChatMessage { role = "assistant", content = aiReply });
            onReply?.Invoke(aiReply);
        }));
        // 在 conversationHistory 中，只保留最近 4 条（2轮）对话：
        if (conversationHistory.Count > 6)
        {
            conversationHistory = conversationHistory.Skip(conversationHistory.Count - 6).ToList();
        }

        if (string.IsNullOrEmpty(apiKey))
        {
            Debug.LogError("🚫 未设置 DeepSeek API Key！");
            onReply?.Invoke("请设置 API Key");
            return;
        }
    }


    IEnumerator SendRequest(List<ChatMessage> messages, System.Action<string> onReply)
    {
        var messageListJson = new StringBuilder();
        messageListJson.Append("\"messages\":[");

        for (int i = 0; i < messages.Count; i++)
        {
            var msg = messages[i];
            messageListJson.Append("{\"role\":\"" + msg.role + "\",\"content\":\"" + EscapeJson(msg.content) + "\"}");
            if (i < messages.Count - 1)
                messageListJson.Append(",");
        }

        messageListJson.Append("]");

        string jsonBody = "{ \"model\": \"deepseek-chat\"," + messageListJson.ToString() + " }";

        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonBody);

        UnityWebRequest request = new UnityWebRequest(apiUrl, "POST");
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("Authorization", $"Bearer {apiKey}");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            string response = request.downloadHandler.text;
            Debug.Log("✅ DeepSeek 返回成功：\n" + response);

            string content = ExtractReply(response);
            Debug.Log("🧠 AI 回复内容为：\n" + content);

            onReply?.Invoke(content);
        }
        else
        {
            Debug.LogError("❌ 请求失败：" + request.error);
            onReply?.Invoke("（AI 无法连接，请稍后再试）");
        }
    }

    private string EscapeJson(string s)
    {
        return s.Replace("\\", "\\\\").Replace("\"", "\\\"").Replace("\n", "\\n");
    }


    private string ExtractReply(string json)
{
    try
    {
        var match = Regex.Match(json, "\"content\":\"([^\"]*)\"");
        if (match.Success)
        {
            string content = match.Groups[1].Value;
            content = content.Replace("\\n", "\n").Replace("\\\"", "\"");
            return content;
        }
        else
        {
            Debug.LogWarning("⚠️ 正则提取失败，未找到 content 字段");
            return "⚠ AI 回复解析失败（content字段未找到）";
        }
    }
    catch (System.Exception e)
    {
        Debug.LogError("❌ 解析 AI 回复出错：" + e.Message);
        return "⚠ AI 回复解析失败（异常）";
    }
}

}
