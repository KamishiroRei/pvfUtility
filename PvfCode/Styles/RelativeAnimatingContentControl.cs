using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace PvfCode.Styles;

public class RelativeAnimatingContentControl : ContentControl
{
	private enum RelativeDimension
	{
		Width,
		Height
	}

	private abstract class RelativeValueUpdater
	{
		protected RelativeValueUpdater(RelativeDimension dimension)
		{
			Dimension = dimension;
		}

		protected RelativeDimension Dimension { get; }

		public abstract void Update(double width, double height);
	}

	private abstract class RelativeAnimationValue<TAnimation> : RelativeValueUpdater
	{
		private const double WidthMarker = 0.1;

		private const double HeightMarker = 0.2;

		private readonly double _scaleFactor;

		protected RelativeAnimationValue(RelativeDimension dimension, TAnimation animation)
			: base(dimension)
		{
			Animation = animation;
			_scaleFactor = DecodeRelativeValue(ReadValue()) / 100.0;
		}

		protected TAnimation Animation { get; }

		protected abstract double ReadValue();

		protected abstract void WriteValue(double value);

		private double DecodeRelativeValue(double encodedValue)
		{
			return encodedValue - (Dimension == RelativeDimension.Width ? WidthMarker : HeightMarker);
		}

		public static RelativeDimension? GetRelativeDimension(double encodedValue)
		{
			double marker = encodedValue - Math.Floor(encodedValue);
			if (marker >= 0.099991 && marker <= 0.100009)
			{
				return RelativeDimension.Width;
			}
			if (marker >= 0.199991 && marker <= 0.200009)
			{
				return RelativeDimension.Height;
			}
			return null;
		}

		public override void Update(double width, double height)
		{
			double dimensionSize = Dimension == RelativeDimension.Width ? width : height;
			WriteValue(dimensionSize * _scaleFactor);
		}
	}

	private sealed class DoubleAnimationToValue : RelativeAnimationValue<DoubleAnimation>
	{
		public DoubleAnimationToValue(RelativeDimension dimension, DoubleAnimation animation)
			: base(dimension, animation)
		{
		}

		protected override double ReadValue()
		{
			return Animation.To.Value;
		}

		protected override void WriteValue(double value)
		{
			Animation.To = value;
		}
	}

	private sealed class DoubleAnimationFromValue : RelativeAnimationValue<DoubleAnimation>
	{
		public DoubleAnimationFromValue(RelativeDimension dimension, DoubleAnimation animation)
			: base(dimension, animation)
		{
		}

		protected override double ReadValue()
		{
			return Animation.From.Value;
		}

		protected override void WriteValue(double value)
		{
			Animation.From = value;
		}
	}

	private sealed class DoubleKeyFrameValue : RelativeAnimationValue<DoubleKeyFrame>
	{
		public DoubleKeyFrameValue(RelativeDimension dimension, DoubleKeyFrame keyFrame)
			: base(dimension, keyFrame)
		{
		}

		protected override double ReadValue()
		{
			return Animation.Value;
		}

		protected override void WriteValue(double value)
		{
			Animation.Value = value;
		}
	}

	private double _currentWidth;

	private double _currentHeight;

	private List<RelativeValueUpdater> _relativeValues;

	public RelativeAnimatingContentControl()
	{
		base.SizeChanged += OnSizeChanged;
	}

	private void OnSizeChanged(object sender, SizeChangedEventArgs e)
	{
		if (e?.NewSize.Height > 0.0 && e.NewSize.Width > 0.0)
		{
			_currentWidth = e.NewSize.Width;
			_currentHeight = e.NewSize.Height;
			UpdateRelativeAnimations();
		}
	}

	private void UpdateRelativeAnimations()
	{
		if (_currentHeight <= 0.0 || _currentWidth <= 0.0)
		{
			return;
		}
		if (_relativeValues == null)
		{
			_relativeValues = new List<RelativeValueUpdater>();
			foreach (VisualStateGroup visualStateGroup in VisualStateManager.GetVisualStateGroups(this))
			{
				if (visualStateGroup == null)
				{
					continue;
				}
				foreach (VisualState state in visualStateGroup.States)
				{
					if (state?.Storyboard == null)
					{
						continue;
					}
					foreach (Timeline child in state.Storyboard.Children)
					{
						if (child is DoubleAnimation doubleAnimation)
						{
							TrackDoubleAnimation(doubleAnimation);
						}
						else if (child is DoubleAnimationUsingKeyFrames keyFrameAnimation)
						{
							TrackKeyFrameAnimation(keyFrameAnimation);
						}
					}
				}
			}
		}
		ApplyRelativeValues();
		foreach (VisualStateGroup visualStateGroup in VisualStateManager.GetVisualStateGroups(this))
		{
			if (visualStateGroup == null)
			{
				continue;
			}
			foreach (VisualState state in visualStateGroup.States)
			{
				state?.Storyboard?.Begin(this);
			}
		}
	}

	private void ApplyRelativeValues()
	{
		foreach (RelativeValueUpdater relativeValue in _relativeValues)
		{
			relativeValue.Update(_currentWidth, _currentHeight);
		}
	}

	private void TrackKeyFrameAnimation(DoubleAnimationUsingKeyFrames animation)
	{
		foreach (DoubleKeyFrame keyFrame in animation.KeyFrames)
		{
			RelativeDimension? dimension = RelativeAnimationValue<DoubleKeyFrame>.GetRelativeDimension(keyFrame.Value);
			if (dimension.HasValue)
			{
				_relativeValues.Add(new DoubleKeyFrameValue(dimension.Value, keyFrame));
			}
		}
	}

	private void TrackDoubleAnimation(DoubleAnimation animation)
	{
		if (animation.To.HasValue)
		{
			RelativeDimension? dimension = RelativeAnimationValue<DoubleAnimation>.GetRelativeDimension(animation.To.Value);
			if (dimension.HasValue)
			{
				_relativeValues.Add(new DoubleAnimationToValue(dimension.Value, animation));
			}
		}
		if (animation.From.HasValue)
		{
			RelativeDimension? dimension = RelativeAnimationValue<DoubleAnimation>.GetRelativeDimension(animation.From.Value);
			if (dimension.HasValue)
			{
				_relativeValues.Add(new DoubleAnimationFromValue(dimension.Value, animation));
			}
		}
	}
}
