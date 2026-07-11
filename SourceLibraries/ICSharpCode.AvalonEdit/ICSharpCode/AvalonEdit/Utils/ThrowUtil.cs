using System;
using System.Globalization;

namespace ICSharpCode.AvalonEdit.Utils;

internal static class ThrowUtil
{
	public static T CheckNotNull<T>(T val, string parameterName) where T : class
	{
		if (val == null)
		{
			throw new ArgumentNullException(parameterName);
		}
		return val;
	}

	public static int CheckNotNegative(int val, string parameterName)
	{
		if (val < 0)
		{
			throw new ArgumentOutOfRangeException(parameterName, val, "value must not be negative");
		}
		return val;
	}

	public static int CheckInRangeInclusive(int val, string parameterName, int lower, int upper)
	{
		if (val < lower || val > upper)
		{
			throw new ArgumentOutOfRangeException(parameterName, val, "Expected: " + lower.ToString(CultureInfo.InvariantCulture) + " <= " + parameterName + " <= " + upper.ToString(CultureInfo.InvariantCulture));
		}
		return val;
	}

	public static InvalidOperationException NoDocumentAssigned()
	{
		return new InvalidOperationException("Document is null");
	}

	public static InvalidOperationException NoValidCaretPosition()
	{
		return new InvalidOperationException("Could not find a valid caret position in the line");
	}
}
