using System;
using System.Globalization;
using System.Numerics;

namespace Swordfish.NET.Collections.Auxiliary;

public class BigRational : IComparable, IComparable<BigRational>, IEquatable<BigRational>
{
	public BigInteger WholePart { get; private set; }

	public Fraction FractionalPart { get; private set; }

	public int Sign => NormalizeSign(this).WholePart.Sign;

	public bool IsZero
	{
		get
		{
			if (WholePart.IsZero)
			{
				return FractionalPart.IsZero;
			}
			return false;
		}
	}

	public static BigRational One => _one;

	public static BigRational Zero => _zero;

	public static BigRational MinusOne => _minusOne;

	private static BigRational _one => new BigRational(BigInteger.One);

	private static BigRational _zero => new BigRational(BigInteger.Zero);

	private static BigRational _minusOne => new BigRational(BigInteger.MinusOne);

	public BigRational(int value)
		: this(value, Fraction.Zero)
	{
	}

	public BigRational(BigInteger value)
		: this(value, Fraction.Zero)
	{
	}

	public BigRational(Fraction fraction)
		: this(BigInteger.Zero, fraction)
	{
	}

	public BigRational(BigInteger whole, Fraction fraction)
		: this(whole, fraction.Numerator, fraction.Denominator)
	{
	}

	public BigRational(BigInteger whole, BigInteger numerator, BigInteger denominator)
	{
		WholePart = whole;
		FractionalPart = new Fraction(numerator, denominator);
		NormalizeSign();
	}

	public BigRational(float value)
	{
		if (!CheckForWholeValues(value))
		{
			WholePart = (BigInteger)Math.Truncate(value);
			float num = Math.Abs(value) % 1f;
			FractionalPart = ((num == 0f) ? Fraction.Zero : new Fraction(num));
			NormalizeSign();
		}
	}

	public BigRational(double value)
	{
		if (!CheckForWholeValues(value))
		{
			WholePart = (BigInteger)Math.Truncate(value);
			double num = Math.Abs(value) % 1.0;
			FractionalPart = ((num == 0.0) ? Fraction.Zero : new Fraction(num));
			NormalizeSign();
		}
	}

	public BigRational(decimal value)
	{
		if (!CheckForWholeValues((double)value))
		{
			WholePart = (BigInteger)Math.Truncate(value);
			decimal num = Math.Abs(value) % 1m;
			FractionalPart = ((num == 0m) ? Fraction.Zero : new Fraction(num));
			NormalizeSign();
		}
	}

	private bool CheckForWholeValues(double value)
	{
		if (double.IsNaN(value))
		{
			throw new ArgumentException("Value is not a number", "value");
		}
		if (double.IsInfinity(value))
		{
			throw new ArgumentException("Cannot represent infinity", "value");
		}
		if (value == 0.0)
		{
			WholePart = BigInteger.Zero;
			FractionalPart = Fraction.Zero;
			return true;
		}
		if (value == 1.0)
		{
			WholePart = BigInteger.Zero;
			FractionalPart = Fraction.One;
			return true;
		}
		if (value == -1.0)
		{
			WholePart = BigInteger.Zero;
			FractionalPart = Fraction.MinusOne;
			return true;
		}
		return false;
	}

	public static BigRational Add(BigRational augend, BigRational addend)
	{
		Fraction improperFraction = augend.GetImproperFraction();
		Fraction improperFraction2 = addend.GetImproperFraction();
		return Reduce(Add(improperFraction, improperFraction2));
	}

	public static BigRational Subtract(BigRational minuend, BigRational subtrahend)
	{
		Fraction improperFraction = minuend.GetImproperFraction();
		Fraction improperFraction2 = subtrahend.GetImproperFraction();
		return Reduce(Subtract(improperFraction, improperFraction2));
	}

	public static BigRational Multiply(BigRational multiplicand, BigRational multiplier)
	{
		Fraction improperFraction = multiplicand.GetImproperFraction();
		Fraction improperFraction2 = multiplier.GetImproperFraction();
		return Reduce(Fraction.ReduceToProperFraction(Fraction.Multiply(improperFraction, improperFraction2)));
	}

	public static BigRational Divide(BigInteger dividend, BigInteger divisor)
	{
		BigInteger remainder = new BigInteger(-1);
		return new BigRational(BigInteger.DivRem(dividend, divisor, out remainder), new Fraction(remainder, divisor));
	}

	public static BigRational Divide(BigRational dividend, BigRational divisor)
	{
		Fraction improperFraction = dividend.GetImproperFraction();
		Fraction improperFraction2 = divisor.GetImproperFraction();
		BigInteger numerator = BigInteger.Multiply(improperFraction.Numerator, improperFraction2.Denominator);
		BigInteger denominator = BigInteger.Multiply(improperFraction.Denominator, improperFraction2.Numerator);
		return Fraction.ReduceToProperFraction(new Fraction(numerator, denominator));
	}

	public static BigRational Remainder(BigInteger dividend, BigInteger divisor)
	{
		BigInteger numerator = dividend % divisor;
		return new BigRational(BigInteger.Zero, new Fraction(numerator, divisor));
	}

	public static BigRational Mod(BigRational number, BigRational mod)
	{
		Fraction improperFraction = number.GetImproperFraction();
		Fraction improperFraction2 = mod.GetImproperFraction();
		return new BigRational(Fraction.Remainder(improperFraction, improperFraction2));
	}

	public static BigRational Pow(BigRational baseValue, BigInteger exponent)
	{
		return new BigRational(Fraction.Pow(baseValue.GetImproperFraction(), exponent));
	}

	public static double Log(BigRational rational)
	{
		return Fraction.Log(rational.GetImproperFraction());
	}

	public static BigRational Abs(BigRational rational)
	{
		BigRational bigRational = Reduce(rational);
		return new BigRational(BigInteger.Abs(bigRational.WholePart), bigRational.FractionalPart);
	}

	public static BigRational Negate(BigRational rational)
	{
		BigRational bigRational = Reduce(rational);
		return new BigRational(BigInteger.Negate(bigRational.WholePart), bigRational.FractionalPart);
	}

	public static BigRational Add(Fraction augend, Fraction addend)
	{
		return new BigRational(BigInteger.Zero, Fraction.Add(augend, addend));
	}

	public static BigRational Subtract(Fraction minuend, Fraction subtrahend)
	{
		return new BigRational(BigInteger.Zero, Fraction.Subtract(minuend, subtrahend));
	}

	public static BigRational Multiply(Fraction multiplicand, Fraction multiplier)
	{
		return new BigRational(BigInteger.Zero, Fraction.Multiply(multiplicand, multiplier));
	}

	public static BigRational Divide(Fraction dividend, Fraction divisor)
	{
		return new BigRational(BigInteger.Zero, Fraction.Divide(dividend, divisor));
	}

	public static BigRational LeastCommonDenominator(BigRational left, BigRational right)
	{
		Fraction improperFraction = left.GetImproperFraction();
		Fraction improperFraction2 = right.GetImproperFraction();
		return Reduce(new BigRational(Fraction.LeastCommonDenominator(improperFraction, improperFraction2)));
	}

	public static BigRational GreatestCommonDivisor(BigRational left, BigRational right)
	{
		Fraction improperFraction = left.GetImproperFraction();
		Fraction improperFraction2 = right.GetImproperFraction();
		return Reduce(new BigRational(Fraction.GreatestCommonDivisor(improperFraction, improperFraction2)));
	}

	public static BigRational operator +(BigRational augend, BigRational addend)
	{
		return Add(augend, addend);
	}

	public static BigRational operator -(BigRational minuend, BigRational subtrahend)
	{
		return Subtract(minuend, subtrahend);
	}

	public static BigRational operator *(BigRational multiplicand, BigRational multiplier)
	{
		return Multiply(multiplicand, multiplier);
	}

	public static BigRational operator /(BigRational dividend, BigRational divisor)
	{
		return Divide(dividend, divisor);
	}

	public static BigRational operator +(BigRational rational)
	{
		return Abs(rational);
	}

	public static BigRational operator -(BigRational rational)
	{
		return Negate(rational);
	}

	public static BigRational operator ++(BigRational rational)
	{
		return Add(rational, One);
	}

	public static BigRational operator --(BigRational rational)
	{
		return Subtract(rational, One);
	}

	public static bool operator ==(BigRational left, BigRational right)
	{
		return Compare(left, right) == 0;
	}

	public static bool operator !=(BigRational left, BigRational right)
	{
		return Compare(left, right) != 0;
	}

	public static bool operator <(BigRational left, BigRational right)
	{
		return Compare(left, right) < 0;
	}

	public static bool operator <=(BigRational left, BigRational right)
	{
		return Compare(left, right) <= 0;
	}

	public static bool operator >(BigRational left, BigRational right)
	{
		return Compare(left, right) > 0;
	}

	public static bool operator >=(BigRational left, BigRational right)
	{
		return Compare(left, right) >= 0;
	}

	public static int Compare(BigRational left, BigRational right)
	{
		BigRational bigRational = Reduce(left);
		BigRational bigRational2 = Reduce(right);
		if (bigRational.WholePart == bigRational2.WholePart)
		{
			Fraction left2 = ((bigRational.Sign == -1) ? Fraction.Negate(bigRational.FractionalPart) : bigRational.FractionalPart);
			Fraction right2 = ((bigRational2.Sign == -1) ? Fraction.Negate(bigRational2.FractionalPart) : bigRational2.FractionalPart);
			return Fraction.Compare(left2, right2);
		}
		return BigInteger.Compare(bigRational.WholePart, bigRational2.WholePart);
	}

	int IComparable.CompareTo(object obj)
	{
		if (obj == null)
		{
			return 1;
		}
		if (!(obj is BigRational))
		{
			throw new ArgumentException("Argument must be of type BigRational", "obj");
		}
		return Compare(this, (BigRational)obj);
	}

	public int CompareTo(BigRational other)
	{
		return Compare(this, other);
	}

	public static explicit operator BigRational(byte value)
	{
		return new BigRational((BigInteger)value);
	}

	public static explicit operator BigRational(sbyte value)
	{
		return new BigRational((BigInteger)value);
	}

	public static explicit operator BigRational(short value)
	{
		return new BigRational((BigInteger)value);
	}

	public static explicit operator BigRational(ushort value)
	{
		return new BigRational((BigInteger)value);
	}

	public static explicit operator BigRational(int value)
	{
		return new BigRational((BigInteger)value);
	}

	public static explicit operator BigRational(uint value)
	{
		return new BigRational((BigInteger)value);
	}

	public static explicit operator BigRational(long value)
	{
		return new BigRational((BigInteger)value);
	}

	public static explicit operator BigRational(ulong value)
	{
		return new BigRational((BigInteger)value);
	}

	public static explicit operator BigRational(BigInteger value)
	{
		return new BigRational(value);
	}

	public static explicit operator BigRational(float value)
	{
		return new BigRational(value);
	}

	public static explicit operator BigRational(double value)
	{
		return new BigRational(value);
	}

	public static explicit operator BigRational(decimal value)
	{
		return new BigRational(value);
	}

	public static explicit operator double(BigRational value)
	{
		double num = (double)value.FractionalPart;
		return (double)value.WholePart + num * (double)value.Sign;
	}

	public static explicit operator decimal(BigRational value)
	{
		decimal num = (decimal)value.FractionalPart;
		return (decimal)value.WholePart + num * (decimal)value.Sign;
	}

	public static explicit operator Fraction(BigRational value)
	{
		return Fraction.Simplify(new Fraction(BigInteger.Add(value.FractionalPart.Numerator, BigInteger.Multiply(value.WholePart, value.FractionalPart.Denominator)), value.FractionalPart.Denominator));
	}

	public static BigRational Parse(string value)
	{
		if (string.IsNullOrWhiteSpace(value))
		{
			throw new ArgumentException("Argument cannot be null, empty or whitespace.");
		}
		string[] array = value.Trim().Split('/');
		if (array.Length == 1)
		{
			if (!BigInteger.TryParse(array[0], out var result))
			{
				throw new ArgumentException("Invalid string given for number.");
			}
			return new BigRational(result);
		}
		if (array.Length == 2)
		{
			BigInteger result2 = BigInteger.Zero;
			string[] array2 = array[0].Trim().Split(new char[2] { '+', ' ' }, StringSplitOptions.RemoveEmptyEntries);
			BigInteger result3;
			if (array2.Length == 1)
			{
				if (!BigInteger.TryParse(array[0].Trim(), out result3))
				{
					throw new ArgumentException("Invalid string given for numerator.");
				}
			}
			else
			{
				if (array2.Length != 2)
				{
					throw new ArgumentException("Invalid fraction given as string to parse.");
				}
				if (!BigInteger.TryParse(array2[0].Trim(), out result2))
				{
					throw new ArgumentException("Invalid string given for whole number.");
				}
				if (!BigInteger.TryParse(array2[1].Trim(), out result3))
				{
					throw new ArgumentException("Invalid string given for numerator.");
				}
			}
			if (!BigInteger.TryParse(array[1].Trim(), out var result4))
			{
				throw new ArgumentException("Invalid string given for denominator.");
			}
			return new BigRational(result2, result3, result4);
		}
		throw new ArgumentException("Invalid fraction given as string to parse.");
	}

	public bool Equals(BigRational other)
	{
		BigRational bigRational = Reduce(this);
		BigRational bigRational2 = Reduce(other);
		return (byte)(1u & (bigRational.WholePart.Equals(bigRational2.WholePart) ? 1u : 0u) & (bigRational.FractionalPart.Numerator.Equals(bigRational2.FractionalPart.Numerator) ? 1u : 0u) & (bigRational.FractionalPart.Denominator.Equals(bigRational2.FractionalPart.Denominator) ? 1u : 0u)) != 0;
	}

	public override bool Equals(object obj)
	{
		if (obj == null)
		{
			return false;
		}
		if (!(obj is BigRational))
		{
			return false;
		}
		return Equals((BigRational)obj);
	}

	public override int GetHashCode()
	{
		return CombineHashCodes(WholePart.GetHashCode(), FractionalPart.GetHashCode());
	}

	internal static int CombineHashCodes(int h1, int h2)
	{
		return ((h1 << 5) + h1) ^ h2;
	}

	public Fraction GetImproperFraction()
	{
		BigRational bigRational = NormalizeSign(this);
		if (bigRational.WholePart == 0L && bigRational.FractionalPart.Sign == 0)
		{
			return Fraction.Zero;
		}
		if (bigRational.FractionalPart.Sign != 0 || bigRational.FractionalPart.Denominator > 1L)
		{
			if (bigRational.WholePart.Sign != 0)
			{
				BigInteger left = BigInteger.Multiply(bigRational.WholePart, bigRational.FractionalPart.Denominator);
				BigInteger bigInteger = bigRational.FractionalPart.Numerator;
				if (bigRational.WholePart.Sign == -1)
				{
					bigInteger = BigInteger.Negate(bigInteger);
				}
				return new Fraction(BigInteger.Add(left, bigInteger), bigRational.FractionalPart.Denominator);
			}
			return bigRational.FractionalPart;
		}
		return new Fraction(bigRational.WholePart, BigInteger.One);
	}

	public static BigRational Reduce(BigRational value)
	{
		BigRational bigRational = Fraction.ReduceToProperFraction(NormalizeSign(value).FractionalPart);
		return new BigRational(value.WholePart + bigRational.WholePart, bigRational.FractionalPart);
	}

	public static BigRational NormalizeSign(BigRational value)
	{
		return value.NormalizeSign();
	}

	internal BigRational NormalizeSign()
	{
		FractionalPart = Fraction.NormalizeSign(FractionalPart);
		if (WholePart > 0L && WholePart.Sign == 1 && FractionalPart.Sign == -1)
		{
			WholePart = BigInteger.Negate(WholePart);
			FractionalPart = Fraction.Negate(FractionalPart);
		}
		return this;
	}

	public override string ToString()
	{
		return ToString(CultureInfo.CurrentCulture);
	}

	public string ToString(string format)
	{
		return ToString(CultureInfo.CurrentCulture);
	}

	public string ToString(IFormatProvider provider)
	{
		return ToString("R", provider);
	}

	public string ToString(string format, IFormatProvider provider)
	{
		NumberFormatInfo numberFormatInfo = (NumberFormatInfo)provider.GetFormat(typeof(NumberFormatInfo));
		if (numberFormatInfo == null)
		{
			numberFormatInfo = CultureInfo.CurrentCulture.NumberFormat;
		}
		string result = numberFormatInfo.NativeDigits[0];
		BigRational bigRational = Reduce(this);
		string text = ((bigRational.WholePart != 0L) ? string.Format(provider, "{0}", bigRational.WholePart.ToString(format, provider)) : string.Empty);
		string text2 = ((bigRational.FractionalPart.Numerator != 0L) ? string.Format(provider, "{0}", bigRational.FractionalPart.ToString(format, provider)) : string.Empty);
		string text3 = string.Empty;
		if (!string.IsNullOrWhiteSpace(text) && !string.IsNullOrWhiteSpace(text2))
		{
			text3 = ((bigRational.WholePart.Sign >= 0) ? (" " + numberFormatInfo.PositiveSign + " ") : (" " + numberFormatInfo.NegativeSign + " "));
		}
		if (string.IsNullOrWhiteSpace(text) && string.IsNullOrWhiteSpace(text3) && string.IsNullOrWhiteSpace(text2))
		{
			return result;
		}
		return text + text3 + text2;
	}
}
