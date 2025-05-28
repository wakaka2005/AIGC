using UnityEngine;
using System;
using System.Collections;
using UnityEngine.Networking;

public class MCPController : MonoBehaviour
{
    public static MCPController Instance;

    private void Awake()
    {
        Instance = this;
    }
    public void CheckTodayFestival(Action<string> onResult)
    {
        StartCoroutine(FetchFestivalInfo(onResult));
    }
    public void CheckTodayWeather(Action<string> onResult)
    {
        StartCoroutine(FetchWeatherInfo(onResult));
    }

    IEnumerator FetchWeatherInfo(Action<string> onResult)
    {
        string city = "Beijing"; // ✅ 默认城市（你可以换成自动定位或设置项）
        string url = $"https://api.vvhan.com/api/weather?city={city}&type=week"; // 示例免费接口

        UnityWebRequest req = UnityWebRequest.Get(url);
        yield return req.SendWebRequest();

        if (req.result == UnityWebRequest.Result.Success)
        {
            string json = req.downloadHandler.text;
            Debug.Log("🌤 天气信息返回：" + json);

            if (json.Contains("weather"))
            {
                int weatherIndex = json.IndexOf("\"weather\":\"");
                int tempIndex = json.IndexOf("\"temNight\":\"");
                int tempDayIndex = json.IndexOf("\"temDay\":\"");

                string weather = ExtractValue(json, weatherIndex + 10);
                string tempNight = ExtractValue(json, tempIndex + 12);
                string tempDay = ExtractValue(json, tempDayIndex + 10);

                string weatherMsg = $"今天天气是 {weather}，白天 {tempDay}°C，晚上 {tempNight}°C。";
                onResult?.Invoke(weatherMsg);
                yield break;
            }
            onResult?.Invoke("");
        }
        else
        {
            Debug.LogWarning("🌤 获取天气失败：" + req.error);
            onResult?.Invoke("");
        }
    }

    private string ExtractValue(string json, int start)
    {
        int end = json.IndexOf("\"", start);
        return json.Substring(start, end - start);
    }

    IEnumerator FetchFestivalInfo(Action<string> onResult)
    {
        string url = "https://timor.tech/api/holiday/info";

        UnityWebRequest req = UnityWebRequest.Get(url);
        yield return req.SendWebRequest();

        if (req.result == UnityWebRequest.Result.Success)
        {
            string json = req.downloadHandler.text;
            Debug.Log("📅 节日信息返回：" + json);

            if (json.Contains("\"holiday\""))
            {
                int nameIndex = json.IndexOf("\"name\":\"");
                if (nameIndex != -1)
                {
                    int start = nameIndex + 8;
                    int end = json.IndexOf("\"", start);
                    string festivalName = json.Substring(start, end - start);
                    onResult?.Invoke($"今天是 {festivalName} 🎉 记得祝福一下喔！");
                    yield break;
                }
            }

            onResult?.Invoke(""); // 没有节日
        }
        else
        {
            Debug.LogWarning("📅 获取节日失败：" + req.error);
            onResult?.Invoke("");
        }
    }

    void Start()
    {
       
    }

   public void SayTimeStatus()
    {
        DateTime now = DateTime.Now;
        string hourStr = now.ToString("HH:mm");
        string weekday = now.ToString("dddd");
        int hour = now.Hour;

        string hint = "";
        if (hour >= 6 && hour < 9)
            hint = "现在是早上，可能是早餐时间。";
        else if (hour >= 12 && hour < 14)
            hint = "这个时间点是中午饭时间。";
        else if (hour >= 18 && hour < 20)
            hint = "晚上吃饭的时候到了~";
        else if (hour >= 22 || hour < 6)
            hint = "夜深了，记得早点休息。";

        // 将当前时间与提示传给 AI，生成更有趣的回复
        string prompt = $"现在是 {hourStr}，{weekday}，{hint}你是一只猫咪助手，用亲切风格说一句提醒的话。";

        DialogueManager.Instance.ShowDialogue();
        DialogueManager.Instance.petReplyText.text = "⏰ MCP 正在生成提示...";
        DialogueManager.Instance.isWaitingForAI = true; // ⛔ 暂停关闭计时

        DeepSeekChatManager.Instance.SendMessageToAI(prompt, (reply) =>
        {
            DialogueManager.Instance.petReplyText.text = reply;
            DialogueManager.Instance.isWaitingForAI = false;
            DialogueManager.Instance.RestartIdleTimer(); // ✅ 正式启动倒计时
        });

        Debug.Log("🧠 MCP触发：" + prompt);
    }
}
