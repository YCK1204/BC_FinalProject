using System;
using System.Globalization;
using UnityEngine;
using UnityEngine.UI;

public class RealTime : MonoBehaviour
{
    public Text timeText;
    public Text dateText;

    private int lastMinute = -1;

    void Start()
    {
        UpdateTimeText();
    }

    void Update()
    {
        int currentMinute = DateTime.Now.Minute;
        if (currentMinute != lastMinute)
        {
            UpdateTimeText();
        }
    }
    void UpdateTimeText()
    {
        DateTime now = DateTime.Now;

        timeText.text = now.ToString("h:mmtt", CultureInfo.InvariantCulture).ToLower();

        string[] days = { "일요일", "월요일", "화요일", "수요일", "목요일", "금요일", "토요일" };
        string dayOfWeek = days[(int)now.DayOfWeek];
        dateText.text = $"{dayOfWeek} {now.ToString("MM월dd일")}";

        lastMinute = now.Minute;
    }
}