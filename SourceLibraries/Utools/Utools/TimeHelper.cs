using System;
using System.Runtime.CompilerServices;

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
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(5, 14);
			defaultInterpolatedStringHandler.AppendFormatted(str[0]);
			defaultInterpolatedStringHandler.AppendFormatted(str[1]);
			defaultInterpolatedStringHandler.AppendFormatted(str[2]);
			defaultInterpolatedStringHandler.AppendFormatted(str[3]);
			defaultInterpolatedStringHandler.AppendLiteral("-");
			defaultInterpolatedStringHandler.AppendFormatted(str[4]);
			defaultInterpolatedStringHandler.AppendFormatted(str[5]);
			defaultInterpolatedStringHandler.AppendLiteral("-");
			defaultInterpolatedStringHandler.AppendFormatted(str[6]);
			defaultInterpolatedStringHandler.AppendFormatted(str[7]);
			defaultInterpolatedStringHandler.AppendLiteral(" ");
			defaultInterpolatedStringHandler.AppendFormatted(str[8]);
			defaultInterpolatedStringHandler.AppendFormatted(str[9]);
			defaultInterpolatedStringHandler.AppendLiteral(":");
			defaultInterpolatedStringHandler.AppendFormatted(str[10]);
			defaultInterpolatedStringHandler.AppendFormatted(str[11]);
			defaultInterpolatedStringHandler.AppendLiteral(":");
			defaultInterpolatedStringHandler.AppendFormatted(str[12]);
			defaultInterpolatedStringHandler.AppendFormatted(str[13]);
			return Convert.ToDateTime(defaultInterpolatedStringHandler.ToStringAndClear());
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
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(6, 4);
				defaultInterpolatedStringHandler.AppendFormatted(value);
				defaultInterpolatedStringHandler.AppendLiteral("天");
				defaultInterpolatedStringHandler.AppendFormatted(value2);
				defaultInterpolatedStringHandler.AppendLiteral("小时");
				defaultInterpolatedStringHandler.AppendFormatted(value3);
				defaultInterpolatedStringHandler.AppendLiteral("分钟");
				defaultInterpolatedStringHandler.AppendFormatted(value4);
				defaultInterpolatedStringHandler.AppendLiteral("秒");
				return defaultInterpolatedStringHandler.ToStringAndClear();
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
