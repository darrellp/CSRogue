using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;

namespace RogueWPF.Utilities
{
	static class PixelConversions
	{
		// ReSharper disable PossibleInvalidOperationException
		private static Matrix TransformToDevice(FrameworkElement element = null)
		{
			Matrix transformToDevice;

			var source = element == null ? null : PresentationSource.FromVisual(element);
			if (source != null)
			{
				transformToDevice = source.CompositionTarget.TransformToDevice;
			}
			else
			{
				using (var sourceTemp = new HwndSource(new HwndSourceParameters()))
					transformToDevice = sourceTemp.CompositionTarget.TransformToDevice;
			}
			return transformToDevice;
		}

		private static Matrix TransformFromDevice(FrameworkElement element = null)
		{
			Matrix transformFromDevice = TransformToDevice(element);
			transformFromDevice.Invert();
			return transformFromDevice;
		}
		// ReSharper restore PossibleInvalidOperationException

		static public Size GetElementPixelSize(FrameworkElement element)
		{
			return (Size)TransformToDevice(element).Transform(new Vector(element.ActualWidth, element.ActualHeight));
		}

		static public Size ConvertFromPixels(Vector vec)
		{
			return (Size) TransformFromDevice().Transform(vec);
		}
	}
}
