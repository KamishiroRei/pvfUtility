using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace PvfCode.Styles;

public class RelativeAnimatingContentControl : ContentControl
{
	private enum Y6SwQtE5Zb0usYTq7Oj
	{
		Width,
		Height
	}

	private abstract class uiohO7EST2NfZKN9sCx
	{
		[CompilerGenerated]
		private double HxVEGr0e10;

		[CompilerGenerated]
		private Y6SwQtE5Zb0usYTq7Oj gFbExEaNql;

		[SpecialName]
		[CompilerGenerated]
		protected double w9SEAFXVhI()
		{
			return HxVEGr0e10;
		}

		[SpecialName]
		[CompilerGenerated]
		protected void hpgE4OGpOq(double P_0)
		{
			HxVEGr0e10 = P_0;
		}

		public uiohO7EST2NfZKN9sCx(Y6SwQtE5Zb0usYTq7Oj P_0)
		{
			MDYEi7D2n6(P_0);
		}

		[SpecialName]
		[CompilerGenerated]
		public Y6SwQtE5Zb0usYTq7Oj TAcEy43XBE()
		{
			return gFbExEaNql;
		}

		[SpecialName]
		[CompilerGenerated]
		private void MDYEi7D2n6(Y6SwQtE5Zb0usYTq7Oj P_0)
		{
			gFbExEaNql = P_0;
		}

		public abstract void K0hZ5pSHqf(double P_0, double P_1);
	}

	private abstract class hVxmQDEQwjJ9BvS3Ffa<fn8rkqEaEElqJq7mHrr> : uiohO7EST2NfZKN9sCx
	{
		[CompilerGenerated]
		private fn8rkqEaEElqJq7mHrr TXpEbPljl1;

		[CompilerGenerated]
		private double PaSEItGBtm;

		private double M18EE9f1XL;

		protected fn8rkqEaEElqJq7mHrr Instance
		{
			[CompilerGenerated]
			get
			{
				return TXpEbPljl1;
			}
			[CompilerGenerated]
			set
			{
				TXpEbPljl1 = value;
			}
		}

		protected abstract double tiUZS5UHjG();

		protected abstract void om1ZApuME0(double P_0);

		[SpecialName]
		[CompilerGenerated]
		protected double h61EdLtsor()
		{
			return PaSEItGBtm;
		}

		[SpecialName]
		[CompilerGenerated]
		private void KDIEe5uOKP(double P_0)
		{
			PaSEItGBtm = P_0;
		}

		public hVxmQDEQwjJ9BvS3Ffa(Y6SwQtE5Zb0usYTq7Oj P_0, fn8rkqEaEElqJq7mHrr rGTLfAE1RjDqpGDD58Y)
			: base(P_0)
		{
			Instance = rGTLfAE1RjDqpGDD58Y;
			KDIEe5uOKP(q5XEwJPg7C(tiUZS5UHjG()));
			M18EE9f1XL = h61EdLtsor() / 100.0;
		}

		public double q5XEwJPg7C(double P_0)
		{
			if (TAcEy43XBE() != Y6SwQtE5Zb0usYTq7Oj.Width)
			{
				return P_0 - 0.2;
			}
			return P_0 - 0.1;
		}

		public static Y6SwQtE5Zb0usYTq7Oj? XcVEoxKT4n(double P_0)
		{
			double num = Math.Floor(P_0);
			double num2 = P_0 - num;
			if (num2 >= 0.09999100000000001 && num2 <= 0.100009)
			{
				return Y6SwQtE5Zb0usYTq7Oj.Width;
			}
			if (num2 >= 0.199991 && num2 <= 0.20000900000000002)
			{
				return Y6SwQtE5Zb0usYTq7Oj.Height;
			}
			return null;
		}

		public override void K0hZ5pSHqf(double P_0, double P_1)
		{
			double num = ((TAcEy43XBE() == Y6SwQtE5Zb0usYTq7Oj.Width) ? P_0 : P_1);
			SLMEsgjftD(num);
		}

		private void SLMEsgjftD(double P_0)
		{
			om1ZApuME0(P_0 * M18EE9f1XL);
		}
	}

	private class yV3Aj0EOTqgsOseBy29 : hVxmQDEQwjJ9BvS3Ffa<DoubleAnimation>
	{
		protected override double tiUZS5UHjG()
		{
			return base.Instance.To.Value;
		}

		protected override void om1ZApuME0(double P_0)
		{
			base.Instance.To = P_0;
		}

		public yV3Aj0EOTqgsOseBy29(Y6SwQtE5Zb0usYTq7Oj P_0, DoubleAnimation P_1)
			: base(P_0, P_1)
		{
		}
	}

	private class kGBJsKEKGg8PRPnN4Ih : hVxmQDEQwjJ9BvS3Ffa<DoubleAnimation>
	{
		protected override double tiUZS5UHjG()
		{
			return base.Instance.From.Value;
		}

		protected override void om1ZApuME0(double P_0)
		{
			base.Instance.From = P_0;
		}

		public kGBJsKEKGg8PRPnN4Ih(Y6SwQtE5Zb0usYTq7Oj P_0, DoubleAnimation P_1)
			: base(P_0, P_1)
		{
		}
	}

	private class jcPABtE9OOyMjFvkxdc : hVxmQDEQwjJ9BvS3Ffa<DoubleKeyFrame>
	{
		protected override double tiUZS5UHjG()
		{
			return base.Instance.Value;
		}

		protected override void om1ZApuME0(double P_0)
		{
			base.Instance.Value = P_0;
		}

		public jcPABtE9OOyMjFvkxdc(Y6SwQtE5Zb0usYTq7Oj P_0, DoubleKeyFrame P_1)
			: base(P_0, P_1)
		{
		}
	}

	private double KNTQfbQkp5;

	private double WhEQ5OB2LG;

	private List<uiohO7EST2NfZKN9sCx> TI6QS53gmb;

	public RelativeAnimatingContentControl()
	{
		base.SizeChanged += FHsQFVQHxB;
	}

	private void FHsQFVQHxB(object P_0, SizeChangedEventArgs P_1)
	{
		if (P_1 == null)
		{
			return;
		}
		Size newSize = P_1.NewSize;
		if (newSize.Height > 0.0)
		{
			newSize = P_1.NewSize;
			if (newSize.Width > 0.0)
			{
				newSize = P_1.NewSize;
				KNTQfbQkp5 = newSize.Width;
				newSize = P_1.NewSize;
				WhEQ5OB2LG = newSize.Height;
				WsBQrKdTeS();
			}
		}
	}

	private void WsBQrKdTeS()
	{
		if (!(WhEQ5OB2LG > 0.0) || !(KNTQfbQkp5 > 0.0))
		{
			return;
		}
		if (TI6QS53gmb == null)
		{
			TI6QS53gmb = new List<uiohO7EST2NfZKN9sCx>();
			foreach (VisualStateGroup visualStateGroup3 in VisualStateManager.GetVisualStateGroups(this))
			{
				if (visualStateGroup3 == null)
				{
					continue;
				}
				foreach (VisualState state in visualStateGroup3.States)
				{
					if (state == null)
					{
						continue;
					}
					Storyboard storyboard = state.Storyboard;
					if (storyboard == null)
					{
						continue;
					}
					foreach (Timeline child in storyboard.Children)
					{
						DoubleAnimation doubleAnimation = child as DoubleAnimation;
						DoubleAnimationUsingKeyFrames doubleAnimationUsingKeyFrames = child as DoubleAnimationUsingKeyFrames;
						if (doubleAnimation != null)
						{
							BGaQ2xRXb4(doubleAnimation);
						}
						else if (doubleAnimationUsingKeyFrames != null)
						{
							vGhQmFqSkS(doubleAnimationUsingKeyFrames);
						}
					}
				}
			}
		}
		LYEQWVPu1J();
		foreach (VisualStateGroup visualStateGroup4 in VisualStateManager.GetVisualStateGroups(this))
		{
			if (visualStateGroup4 == null)
			{
				continue;
			}
			foreach (VisualState state2 in visualStateGroup4.States)
			{
				state2?.Storyboard?.Begin(this);
			}
		}
	}

	private void LYEQWVPu1J()
	{
		foreach (uiohO7EST2NfZKN9sCx item in TI6QS53gmb)
		{
			item.K0hZ5pSHqf(KNTQfbQkp5, WhEQ5OB2LG);
		}
	}

	private void vGhQmFqSkS(DoubleAnimationUsingKeyFrames P_0)
	{
		foreach (DoubleKeyFrame keyFrame in P_0.KeyFrames)
		{
			Y6SwQtE5Zb0usYTq7Oj? y6SwQtE5Zb0usYTq7Oj = hVxmQDEQwjJ9BvS3Ffa<DoubleKeyFrame>.XcVEoxKT4n(keyFrame.Value);
			if (y6SwQtE5Zb0usYTq7Oj.HasValue)
			{
				TI6QS53gmb.Add(new jcPABtE9OOyMjFvkxdc(y6SwQtE5Zb0usYTq7Oj.Value, keyFrame));
			}
		}
	}

	private void BGaQ2xRXb4(DoubleAnimation P_0)
	{
		if (P_0.To.HasValue)
		{
			Y6SwQtE5Zb0usYTq7Oj? y6SwQtE5Zb0usYTq7Oj = hVxmQDEQwjJ9BvS3Ffa<DoubleAnimation>.XcVEoxKT4n(P_0.To.Value);
			if (y6SwQtE5Zb0usYTq7Oj.HasValue)
			{
				TI6QS53gmb.Add(new yV3Aj0EOTqgsOseBy29(y6SwQtE5Zb0usYTq7Oj.Value, P_0));
			}
		}
		if (P_0.From.HasValue)
		{
			Y6SwQtE5Zb0usYTq7Oj? y6SwQtE5Zb0usYTq7Oj2 = hVxmQDEQwjJ9BvS3Ffa<DoubleAnimation>.XcVEoxKT4n(P_0.To.Value);
			if (y6SwQtE5Zb0usYTq7Oj2.HasValue)
			{
				TI6QS53gmb.Add(new kGBJsKEKGg8PRPnN4Ih(y6SwQtE5Zb0usYTq7Oj2.Value, P_0));
			}
		}
	}
}
