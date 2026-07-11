using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;

namespace Swordfish.NET.Collections.Auxiliary;

public class Fraction : IComparable, IComparable<Fraction>, IEquatable<Fraction>, IEqualityComparer<Fraction>
{
	[StructLayout(LayoutKind.Explicit)]
	internal struct DecimalUInt32
	{
		[FieldOffset(0)]
		public decimal dec;

		[FieldOffset(0)]
		public int flags;
	}

	private static readonly Fraction _zero = new Fraction(BigInteger.Zero, BigInteger.One);

	private static readonly Fraction _one = new Fraction(BigInteger.One, BigInteger.One);

	private static readonly Fraction _minusOne = new Fraction(BigInteger.MinusOne, BigInteger.One);

	private static readonly Fraction _oneHalf = new Fraction(new BigInteger(1), new BigInteger(2));

	private static readonly int _doubleMaxScale = 308;

	private static readonly BigInteger _doublePrecision = BigInteger.Pow(10, _doubleMaxScale);

	private static readonly BigInteger _decimalPrecision = BigInteger.Pow(10, 28);

	private static readonly BigInteger _decimalMaxValue = (BigInteger)decimal.MaxValue;

	private static readonly BigInteger _decimalMinValue = (BigInteger)decimal.MinValue;

	private const int DecimalScaleMask = 16711680;

	private const int DecimalSignMask = int.MinValue;

	private const int DecimalMaxScale = 28;

	public BigInteger Numerator { get; private set; }

	public BigInteger Denominator { get; private set; }

	public int Sign => NormalizeSign(this).Numerator.Sign;

	public bool IsZero => this == Zero;

	public bool IsOne => this == One;

	public static Fraction Zero => _zero;

	public static Fraction One => _one;

	public static Fraction MinusOne => _minusOne;

	public static Fraction OneHalf => _oneHalf;

	public Fraction()
		: this(BigInteger.Zero, BigInteger.One)
	{
	}

	public Fraction(Fraction fraction)
		: this(fraction.Numerator, fraction.Denominator)
	{
	}

	public Fraction(int value)
		: this(value, BigInteger.One)
	{
	}

	public Fraction(BigInteger value)
		: this(value, BigInteger.One)
	{
	}

	public Fraction(BigInteger numerator, BigInteger denominator)
	{
		Numerator = numerator;
		Denominator = denominator;
	}

	public Fraction(float value)
	{
		Initialize(value, 7);
	}

	public Fraction(double value)
	{
		Initialize(value, 13);
	}

	private void Initialize(double value, int precision)
	{
		if (!CheckForWholeValues(value))
		{
			int num = Math.Sign(value);
			int num2 = value.ToString(CultureInfo.InvariantCulture).TrimEnd('0').SkipWhile((char c) => c != '.')
				.Skip(1)
				.Count();
			double num3 = Math.Round(1.0 / Math.Abs(value), precision);
			bool flag;
			BigInteger denominator;
			if (precision == 7)
			{
				float num4 = (float)num3;
				flag = num4 % 1f == 0f;
				denominator = (BigInteger)num4;
			}
			else
			{
				flag = num3 % 1.0 == 0.0;
				denominator = (BigInteger)num3;
			}
			if (flag)
			{
				Numerator = num;
				Denominator = denominator;
			}
			else if (num2 > 0)
			{
				Fraction fraction = Simplify(new Fraction((BigInteger)(value * Math.Pow(10.0, num2)), BigInteger.Pow(10, num2)));
				Numerator = fraction.Numerator;
				Denominator = fraction.Denominator;
			}
			else
			{
				Numerator = new BigInteger(value);
				Denominator = BigInteger.One;
			}
		}
	}

	public Fraction(decimal value)
	{
		int[] bits = decimal.GetBits(value);
		if (bits == null || bits.Length != 4 || (bits[3] & 0x7F00FFFF) != 0 || (bits[3] & 0xFF0000) > 1835008)
		{
			throw new ArgumentException("invalid decimal", "value");
		}
		if (!CheckForWholeValues((double)value))
		{
			BigInteger bigInteger = (new BigInteger(((ulong)(uint)bits[2] << 32) | (uint)bits[1]) << 32) | (uint)bits[0];
			if ((bits[3] & int.MinValue) != 0)
			{
				bigInteger = BigInteger.Negate(bigInteger);
			}
			int exponent = (bits[3] & 0xFF0000) >> 16;
			BigInteger denominator = BigInteger.Pow(10, exponent);
			Fraction fraction = Simplify(new Fraction(bigInteger, denominator));
			Numerator = fraction.Numerator;
			Denominator = fraction.Denominator;
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
			Numerator = BigInteger.Zero;
			Denominator = BigInteger.One;
			return true;
		}
		if (value == 1.0)
		{
			Numerator = BigInteger.One;
			Denominator = BigInteger.One;
			return true;
		}
		if (value == -1.0)
		{
			Numerator = BigInteger.MinusOne;
			Denominator = BigInteger.One;
			return true;
		}
		if (value % 1.0 == 0.0)
		{
			Numerator = (BigInteger)value;
			Denominator = BigInteger.One;
			return true;
		}
		return false;
	}

	public static Fraction Add(Fraction augend, Fraction addend)
	{
		return new Fraction(BigInteger.Add(BigInteger.Multiply(augend.Numerator, addend.Denominator), BigInteger.Multiply(augend.Denominator, addend.Numerator)), BigInteger.Multiply(augend.Denominator, addend.Denominator));
	}

	public static Fraction Subtract(Fraction minuend, Fraction subtrahend)
	{
		return new Fraction(BigInteger.Subtract(BigInteger.Multiply(minuend.Numerator, subtrahend.Denominator), BigInteger.Multiply(minuend.Denominator, subtrahend.Numerator)), BigInteger.Multiply(minuend.Denominator, subtrahend.Denominator));
	}

	public static Fraction Multiply(Fraction multiplicand, Fraction multiplier)
	{
		Fraction fraction = new Fraction(BigInteger.Multiply(multiplicand.Numerator, multiplier.Numerator), BigInteger.Multiply(multiplicand.Denominator, multiplier.Denominator));
		Fraction fraction2 = Simplify(fraction);
		if (fraction != fraction2)
		{
			throw new ArithmeticException("Multiply methods needs to simplify result. Please add this behavior to this method.");
		}
		return fraction;
	}

	public static Fraction Divide(Fraction dividend, Fraction divisor)
	{
		return Simplify(Multiply(dividend, Reciprocal(divisor)));
	}

	public static Fraction Remainder(BigInteger dividend, BigInteger divisor)
	{
		return new Fraction(dividend % divisor, divisor);
	}

	public static Fraction Remainder(Fraction dividend, Fraction divisor)
	{
		return new Fraction(BigInteger.Multiply(dividend.Numerator, divisor.Denominator) % BigInteger.Multiply(dividend.Denominator, divisor.Numerator), BigInteger.Multiply(dividend.Denominator, divisor.Denominator));
	}

	public static Fraction DivRem(Fraction dividend, Fraction divisor, out Fraction remainder)
	{
		BigInteger bigInteger = dividend.Numerator * divisor.Denominator;
		BigInteger bigInteger2 = dividend.Denominator * divisor.Numerator;
		BigInteger denominator = dividend.Denominator * divisor.Denominator;
		remainder = new Fraction(bigInteger % bigInteger2, denominator);
		return new Fraction(bigInteger, bigInteger2);
	}

	public static BigInteger DivRem(BigInteger dividend, BigInteger divisor, out Fraction remainder)
	{
		BigInteger remainder2 = new BigInteger(-1);
		BigInteger result = BigInteger.DivRem(dividend, divisor, out remainder2);
		remainder = new Fraction(remainder2, divisor);
		return result;
	}

	public static Fraction Pow(Fraction value, BigInteger exponent)
	{
		if (exponent.Sign == 0)
		{
			return One;
		}
		Fraction fraction;
		BigInteger bigInteger;
		if (exponent.Sign < 0)
		{
			if (value == Zero)
			{
				throw new ArgumentException("Cannot raise zero to a negative power", "value");
			}
			fraction = Reciprocal(value);
			bigInteger = BigInteger.Negate(exponent);
		}
		else
		{
			fraction = new Fraction(value);
			bigInteger = exponent;
		}
		Fraction fraction2 = fraction;
		for (; bigInteger > BigInteger.One; --bigInteger)
		{
			fraction2 = Multiply(fraction2, fraction);
		}
		return fraction2;
	}

	public static double Log(Fraction fraction)
	{
		double num = BigInteger.Log(fraction.Numerator);
		double num2 = BigInteger.Log(fraction.Denominator);
		return num - num2;
	}

	public static Fraction Reciprocal(Fraction fraction)
	{
		return Simplify(new Fraction(fraction.Denominator, fraction.Numerator));
	}

	public static Fraction Abs(Fraction fraction)
	{
		if (fraction.Numerator.Sign >= 0)
		{
			return fraction;
		}
		return new Fraction(BigInteger.Abs(fraction.Numerator), fraction.Denominator);
	}

	public static Fraction Negate(Fraction fraction)
	{
		return new Fraction(BigInteger.Negate(fraction.Numerator), fraction.Denominator);
	}

	public static Fraction LeastCommonDenominator(Fraction left, Fraction right)
	{
		return new Fraction(left.Denominator * right.Denominator, BigInteger.GreatestCommonDivisor(left.Denominator, right.Denominator));
	}

	public static Fraction GreatestCommonDivisor(Fraction left, Fraction right)
	{
		Simplify(left);
		Simplify(right);
		BigInteger numerator = BigInteger.GreatestCommonDivisor(left.Numerator, right.Numerator);
		BigInteger denominator = LCM(left.Denominator, right.Denominator);
		return new Fraction(numerator, denominator);
	}

	private static BigInteger LCM(BigInteger value1, BigInteger value2)
	{
		BigInteger bigInteger = BigInteger.Abs(value1);
		BigInteger bigInteger2 = BigInteger.Abs(value2);
		return bigInteger * bigInteger2 / BigInteger.GreatestCommonDivisor(bigInteger, bigInteger2);
	}

	public static Fraction operator +(Fraction left, Fraction right)
	{
		return Add(left, right);
	}

	public static Fraction operator -(Fraction left, Fraction right)
	{
		return Subtract(left, right);
	}

	public static Fraction operator *(Fraction left, Fraction right)
	{
		return Multiply(left, right);
	}

	public static Fraction operator /(Fraction left, Fraction right)
	{
		return Divide(left, right);
	}

	public static Fraction operator %(Fraction left, Fraction right)
	{
		return Remainder(left, right);
	}

	public static Fraction operator +(Fraction fraction)
	{
		return Abs(fraction);
	}

	public static Fraction operator -(Fraction fraction)
	{
		return Negate(fraction);
	}

	public static Fraction operator ++(Fraction fraction)
	{
		return Add(fraction, One);
	}

	public static Fraction operator --(Fraction fraction)
	{
		return Subtract(fraction, One);
	}

	public static bool operator ==(Fraction left, Fraction right)
	{
		return Compare(left, right) == 0;
	}

	public static bool operator !=(Fraction left, Fraction right)
	{
		return Compare(left, right) != 0;
	}

	public static bool operator <(Fraction left, Fraction right)
	{
		return Compare(left, right) < 0;
	}

	public static bool operator <=(Fraction left, Fraction right)
	{
		return Compare(left, right) <= 0;
	}

	public static bool operator >(Fraction left, Fraction right)
	{
		return Compare(left, right) > 0;
	}

	public static bool operator >=(Fraction left, Fraction right)
	{
		return Compare(left, right) >= 0;
	}

	public static int Compare(Fraction left, Fraction right)
	{
		BigInteger left2 = BigInteger.Multiply(left.Numerator, right.Denominator);
		BigInteger right2 = BigInteger.Multiply(right.Numerator, left.Denominator);
		return BigInteger.Compare(left2, right2);
	}

	int IComparable.CompareTo(object obj)
	{
		if (obj == null)
		{
			return 1;
		}
		if (!(obj is Fraction))
		{
			throw new ArgumentException("Argument must be of type Fraction", "obj");
		}
		return Compare(this, (Fraction)obj);
	}

	public int CompareTo(Fraction other)
	{
		return Compare(this, other);
	}

	public static explicit operator BigRational(Fraction value)
	{
		return new BigRational(BigInteger.Zero, value);
	}

	public static explicit operator Fraction(float value)
	{
		return new Fraction(value);
	}

	public static explicit operator Fraction(double value)
	{
		return new Fraction(value);
	}

	public static explicit operator Fraction(decimal value)
	{
		return new Fraction(value);
	}

	public static explicit operator double(Fraction value)
	{
		if (IsInRangeDouble(value.Numerator) && IsInRangeDouble(value.Denominator))
		{
			return (double)value.Numerator / (double)value.Denominator;
		}
		BigInteger bigInteger = BigInteger.Multiply(value.Numerator, _doublePrecision) / value.Denominator;
		if (bigInteger.IsZero)
		{
			return 0.0;
		}
		bool flag = false;
		double num = 0.0;
		for (int num2 = _doubleMaxScale; num2 > 0; num2--)
		{
			if (!flag)
			{
				if (IsInRangeDouble(bigInteger))
				{
					num = (double)bigInteger;
					flag = true;
				}
				else
				{
					bigInteger /= (BigInteger)10;
				}
			}
			num /= 10.0;
		}
		if (flag)
		{
			return num;
		}
		if (value.Sign >= 0)
		{
			return double.PositiveInfinity;
		}
		return double.NegativeInfinity;
	}

	public static explicit operator decimal(Fraction value)
	{
		if (IsInRangeDecimal(value.Numerator) && IsInRangeDecimal(value.Denominator))
		{
			return (decimal)value.Numerator / (decimal)value.Denominator;
		}
		BigInteger bigInteger = value.Numerator * _decimalPrecision / value.Denominator;
		if (bigInteger.IsZero)
		{
			return 0m;
		}
		int num = 28;
		while (num >= 0)
		{
			if (!IsInRangeDecimal(bigInteger))
			{
				bigInteger /= (BigInteger)10;
				num--;
				continue;
			}
			DecimalUInt32 decimalUInt = default(DecimalUInt32);
			decimalUInt.dec = (decimal)bigInteger;
			decimalUInt.flags = (decimalUInt.flags & -16711681) | (num << 16);
			return decimalUInt.dec;
		}
		throw new OverflowException("Value was either too large or too small for a decimal.");
	}

	private static bool IsInRangeDouble(BigInteger number)
	{
		if ((BigInteger)double.MinValue < number)
		{
			return number < (BigInteger)double.MaxValue;
		}
		return false;
	}

	private static bool IsInRangeDecimal(BigInteger number)
	{
		if (_decimalMinValue <= number)
		{
			return number <= _decimalMaxValue;
		}
		return false;
	}

	public override bool Equals(object obj)
	{
		if (obj == null)
		{
			return false;
		}
		if (!(obj is Fraction))
		{
			return false;
		}
		return Equals(this, (Fraction)obj);
	}

	public bool Equals(Fraction other)
	{
		return Equals(this, other);
	}

	public bool Equals(Fraction left, Fraction right)
	{
		if (left.Denominator == right.Denominator)
		{
			return left.Numerator == right.Numerator;
		}
		return left.Numerator * right.Denominator == left.Denominator * right.Numerator;
	}

	public override int GetHashCode()
	{
		return GetHashCode(this);
	}

	public int GetHashCode(Fraction fraction)
	{
		return BigRational.CombineHashCodes(fraction.Numerator.GetHashCode(), fraction.Denominator.GetHashCode());
	}

	public static BigRational ReduceToProperFraction(Fraction value)
	{
		Fraction fraction = Simplify(value);
		if (fraction.Numerator.IsZero)
		{
			return new BigRational(BigInteger.Zero, fraction);
		}
		if (fraction.Denominator.IsOne)
		{
			return new BigRational(fraction.Numerator, Zero);
		}
		if (BigInteger.Abs(fraction.Numerator) > BigInteger.Abs(fraction.Denominator))
		{
			int sign = fraction.Numerator.Sign;
			BigInteger remainder = new BigInteger(-1);
			BigInteger bigInteger = BigInteger.DivRem(BigInteger.Abs(fraction.Numerator), fraction.Denominator, out remainder);
			if (sign == -1)
			{
				bigInteger = BigInteger.Negate(bigInteger);
			}
			return new BigRational(bigInteger, new Fraction(remainder, fraction.Denominator));
		}
		return new BigRational(BigInteger.Zero, fraction.Numerator, fraction.Denominator);
	}

	public static Fraction Simplify(Fraction value)
	{
		Fraction fraction = NormalizeSign(value);
		if (fraction.Numerator.IsZero || fraction.Numerator.IsOne || fraction.Numerator == BigInteger.MinusOne)
		{
			return new Fraction(fraction);
		}
		BigInteger numerator = fraction.Numerator;
		BigInteger denominator = fraction.Denominator;
		BigInteger bigInteger = BigInteger.GreatestCommonDivisor(numerator, denominator);
		if (bigInteger > BigInteger.One)
		{
			return new Fraction(numerator / bigInteger, denominator / bigInteger);
		}
		return new Fraction(fraction);
	}

	internal static Fraction NormalizeSign(Fraction value)
	{
		BigInteger bigInteger = value.Numerator;
		BigInteger bigInteger2 = value.Denominator;
		if (bigInteger.Sign == 1 && bigInteger2.Sign == 1)
		{
			return value;
		}
		if (bigInteger.Sign == -1 && bigInteger2.Sign == 1)
		{
			return value;
		}
		if (bigInteger.Sign == 1 && bigInteger2.Sign == -1)
		{
			bigInteger = BigInteger.Negate(bigInteger);
			bigInteger2 = BigInteger.Negate(bigInteger2);
		}
		else if (bigInteger.Sign == -1 && bigInteger2.Sign == -1)
		{
			bigInteger = BigInteger.Negate(bigInteger);
			bigInteger2 = BigInteger.Negate(bigInteger2);
		}
		return new Fraction(bigInteger, bigInteger2);
	}

	public override string ToString()
	{
		return ToString(CultureInfo.CurrentCulture);
	}

	public string ToString(string format)
	{
		return ToString(format, CultureInfo.CurrentCulture);
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
		string text = numberFormatInfo.NativeDigits[0];
		text.First();
		if (Numerator.IsZero)
		{
			return text;
		}
		if (Denominator.IsOne)
		{
			return string.Format(provider, "{0}", Numerator.ToString(format, provider));
		}
		return string.Format(provider, "{0}/{1}", Numerator.ToString(format, provider), Denominator.ToString(format, provider));
	}
}
