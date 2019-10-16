using System;

public class Message {
    public string text;
    public int year;
    public int month;
    public int day;
    public int hour;
    public int minute;
    public int second;
    public int type;

    public Message (
        string m,
        int y,
        int mo, 
        int d, 
        int h,
        int mi,
        int s,
        int t
    ) {
        text = m;
        year = y;
        month = mo;
        day = d;
        hour = h;
        minute = mi;
        second = s;
        type = t;
    }

    public Message(Message m) {
        text = m.text;
        year = m.year;
        month = m.month;
        day = m.day;
        hour = m.hour;
        minute = m.minute;
        second = m.second;
        type = m.type;
    }

    public Message(string m, int t) {
        text = m;
        type = t;
        DateTime curTime = System.DateTime.Now;
        year = curTime.Year;
        month = curTime.Month;
        day = curTime.Day;
        hour = curTime.Hour;
        minute = curTime.Minute;
        second = curTime.Second;
    }
}