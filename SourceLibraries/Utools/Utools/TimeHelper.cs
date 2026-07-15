using System;

namespace Utools;

public static class TimeHelper
{
	public static string DayNow => DateTime.Now.ToString("yyyy-MM-dd");

	public static string HourNow => DateTime.Now.ToString("HH:mm:ss");

	public static string GetTimeStamp()
	{
		return Convert.ToInt64((DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0)).TotalMilliseconds).ToString();
	}

	public static string DateStringFromNow(this DateTime dt)
	{
		TimeSpan timeSpan = DateTime.Now - dt;
		if (timeSpan.TotalDays > 60.0)
		{
			return dt.ToShortDateString();
		}
		if (timeSpan.TotalDays > 30.0)
		{
			return "1个月前";
		}
		if (timeSpan.TotalDays > 14.0)
		{
			return "2周前";
		}
		if (timeSpan.TotalDays > 7.0)
		{
			return "1周前";
		}
		if (timeSpan.TotalDays > 1.0)
		{
			return string.Format("{0}天前", (int)Math.Floor(timeSpan.TotalDays));
		}
		if (timeSpan.TotalHours > 1.0)
		{
			return string.Format("{0}小时前", (int)Math.Floor(timeSpan.TotalHours));
		}
		if (timeSpan.TotalMinutes > 1.0)
		{
			return string.Format("{0}分钟前", (int)Math.Floor(timeSpan.TotalMinutes));
		}
		if (timeSpan.TotalSeconds >= 1.0)
		{
			return string.Format("{0}秒前", (int)Math.Floor(timeSpan.TotalSeconds));
		}
		return "1秒前";
	}

	public static DateTime NumTimeToTime(this string str)
	{
		try
		{
			return Convert.ToDateTime($"{str[0]}{str[1]}{str[2]}{str[3]}-{str[4]}{str[5]}-{str[6]}{str[7]} {str[8]}{str[9]}:{str[10]}{str[11]}:{str[12]}{str[13]}");
		}
		catch (Exception)
		{
			return DateTime.Now.AddDays(-10000.0);
		}
	}

	public static string ToNumTime(this DateTime date)
	{
		return date.ToString("yyyMMddHHmmss");
	}

	public static DateTime ConvertStringToDateTime2(this string timeStamp)
	{
		long num = Convert.ToInt64(timeStamp) * 10000000;
		long ticks = new DateTime(1970, 1, 1, 8, 0, 0).Ticks + num;
		return new DateTime(ticks);
	}

	public static string TimeSStoDay(this string SS)
	{
		if (SS == "")
		{
			return "0";
		}
		try
		{
			int num = Convert.ToInt32(SS);
			TimeSpan timeSpan = new TimeSpan(0, 0, num);
			if (timeSpan.TotalHours > 24.0)
			{
				int value = num / 86400;
				int value2 = num % 86400 / 3600;
				int value3 = num % 3600 / 60;
				int value4 = num % 60;
				return $"{value}天{value2}小时{value3}分钟{value4}秒";
			}
			return (int)timeSpan.TotalHours + "小时" + timeSpan.Minutes + "分钟" + timeSpan.Seconds + "秒";
		}
		catch (Exception)
		{
			return "计算出错";
		}
	}

	public static string ToHoursStr(DateTime time)
	{
		return time.ToString("[HH:mm:ss]");
	}

	public static string ToLogTime(this DateTime time)
	{
		return time.ToString("yyy_MM_dd_HH_mm_ss");
	}

	public static string ToTimeStr(this DateTime time)
	{
		return time.ToString("yyy-MM-dd HH:mm:ss");
	}

	public static string ToDayStr(this DateTime time)
	{
		return time.ToString("yyyy-MM-dd");
	}

	public static string AddDayNumber(this DateTime time, int X)
	{
		return time.AddDays(X).ToString("yyyy-MM-dd HH:mm:ss");
	}
}
