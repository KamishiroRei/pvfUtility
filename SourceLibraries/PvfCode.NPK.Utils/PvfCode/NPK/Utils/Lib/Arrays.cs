using System;
using System.Collections.Generic;

namespace PvfCode.NPK.Utils.Lib;

public static class Arrays
{
	public static T Find<T>(this T[] array, Predicate<T> match)
	{
		return Array.Find(array, match);
	}

	public static bool Compare<T>(this T[] arr1, T[] arr2)
	{
		if (arr1.Length != arr2.Length)
		{
			return false;
		}
		for (int i = 0; i < arr1.Length && i < arr2.Length; i++)
		{
			if (!object.Equals(arr1[i], arr2[i]))
			{
				return false;
			}
		}
		return true;
	}

	public static T[] Concat<T>(this T[] arr1, T[] arr2)
	{
		T[] array = new T[arr1.Length + arr2.Length];
		Buffer.BlockCopy(arr1, 0, array, 0, arr1.Length);
		Buffer.BlockCopy(arr2, 0, array, arr1.Length, arr2.Length);
		return array;
	}

	public static T[][] Split<T>(this T[] data, T[] pattern)
	{
		int num = 0;
		List<T[]> list = new List<T[]>();
		for (int i = 0; i < data.Length; i++)
		{
			int j;
			for (j = i; j < data.Length && j - i < pattern.Length && object.Equals(data[j], pattern[j - i]); j++)
			{
			}
			if (j - i == pattern.Length)
			{
				T[] array = new T[j - num];
				Buffer.BlockCopy(data, num, array, 0, array.Length);
				list.Add(array);
				num = j;
			}
			else
			{
				i = j;
			}
		}
		T[] array2 = new T[data.Length - num];
		Buffer.BlockCopy(data, num, array2, 0, array2.Length);
		list.Add(array2);
		return list.ToArray();
	}

	public static void InsertAt<T>(this List<T> list, int index, IEnumerable<T> t)
	{
		if (index > list.Count)
		{
			list.AddRange(t);
		}
		else if (index < 0)
		{
			list.InsertRange(0, t);
		}
		else
		{
			list.InsertRange(index, t);
		}
	}
}
