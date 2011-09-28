/* 
 * Copyright (c) 2011 NOVALISTIC
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 */

using System;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Interop;
using System.Windows.Media;

namespace WPFDraggableGlass
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();
		}

		/// <summary>
		/// Replaces the introductory message in the "client area" with a greeting.
		/// </summary>
		public void SayHello()
		{
			string name;

			if (!string.IsNullOrWhiteSpace(tbxName.Text))
			{
				name = tbxName.Text;
			}
			else
			{
				name = "world";
			}

			txtMessage.FontSize = 24;
			txtMessage.FontWeight = FontWeights.DemiBold;
			txtMessage.Text = string.Format("Hello, {0}!", name);
		}

		/// <summary>
		/// Displays the introductory message in the "client area".
		/// </summary>
		public void ShowIntroMessage()
		{
			txtMessage.FontSize = 13;
			txtMessage.FontWeight = FontWeights.Normal;
			txtMessage.Text = "";

			txtMessage.Inlines.Add(new Run("Drag anywhere on the glass to move this window. Like with the title bar, you can double-click the glass to maximize or restore the window. On Windows 7, you can even perform Aero Snap or Aero Shake gestures. You can also interact with the controls as per normal."));
			txtMessage.Inlines.Add(new LineBreak());
			txtMessage.Inlines.Add(new LineBreak());
			txtMessage.Inlines.Add(new Run("This text block is in a scroll viewer with a white background. The scroll viewer, as one of the top-level controls (after the window grid), serves as the \"client area\" for the window, preventing it from being dragged when the cursor is on it."));
			txtMessage.Inlines.Add(new LineBreak());
			txtMessage.Inlines.Add(new LineBreak());
			txtMessage.Inlines.Add(new Run("Other top-level controls include the label and the text box at the top, and the command buttons at the bottom. These respond to user interaction as normal, instead of causing the window to be dragged."));
			txtMessage.Inlines.Add(new LineBreak());
			txtMessage.Inlines.Add(new LineBreak());
			txtMessage.Inlines.Add(new Run("All code in this sample project is distributed under the MIT license, which can be found in license.txt."));
		}

		#region Draggable glass functionality

		private bool IsOnGlass(int lParam)
		{
			int x = lParam << 16 >> 16, y = lParam >> 16;
			Point point = PointFromScreen(new Point(x, y));

			// In XAML: <Grid x:Name="windowGrid">...</Grid>
			return VisualTreeHelper.HitTest(windowGrid, point) == null;

			// If you'd like to ignore hits against the grid itself (if it has a background image
			// or gradient for example, the entire grid area will respond to the hit), use this
			// return statement instead:
//			return VisualTreeHelper.HitTest(windowGrid, point).VisualHit == windowGrid;
		}

		private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
		{
			switch (msg)
			{
				// Ignore clicks if desktop composition isn't enabled
				case DwmApiInterop.WM_NCHITTEST:
					if (DwmApiInterop.IsCompositionEnabled()
						&& DwmApiInterop.IsOnClientArea(hwnd, msg, wParam, lParam)
						&& IsOnGlass(lParam.ToInt32()))
					{
						handled = true;
						return new IntPtr(DwmApiInterop.HTCAPTION);
					}

					return IntPtr.Zero;

				// Also toggle glass painting on this window when desktop composition is toggled
				case DwmApiInterop.WM_DWMCOMPOSITIONCHANGED:
					try
					{
						AdjustGlassFrame();
					}
					catch (InvalidOperationException)
					{
						FallbackPaint();
					}
					return IntPtr.Zero;

				default:
					return IntPtr.Zero;
			}
		}

		#endregion

		#region Aero Glass extensions - implementation details not essential to this sample

		private IntPtr hwnd;
		private HwndSource hsource;

		private void Window_SourceInitialized(object sender, EventArgs e)
		{
			try
			{
				if ((hwnd = new WindowInteropHelper(this).Handle) == IntPtr.Zero)
				{
					throw new InvalidOperationException("Could not get window handle for the main window.");
				}

				hsource = HwndSource.FromHwnd(hwnd);
				hsource.AddHook(WndProc);

				AdjustGlassFrame();
			}
			catch (InvalidOperationException)
			{
				FallbackPaint();
			}
		}

		private void AdjustGlassFrame()
		{
			if (DwmApiInterop.IsCompositionEnabled())
			{
				ExtendGlassIntoClientArea(0, 0, 32, 35);
			}
			else
			{
				FallbackPaint();
			}
		}

		private void ExtendGlassIntoClientArea(int left, int right, int top, int bottom)
		{
			var margins = new MARGINS { cxLeftWidth = left, cxRightWidth = right, cyTopHeight = top, cyBottomHeight = bottom };
			int hresult = DwmApiInterop.ExtendFrameIntoClientArea(hwnd, ref margins);

			if (hresult == 0)
			{
				hsource.CompositionTarget.BackgroundColor = Colors.Transparent;
				Background = Brushes.Transparent;
			}
			else
			{
				throw new InvalidOperationException("Could not extend glass window frames in the main window.");
			}
		}

		private void FallbackPaint()
		{
			Background = Brushes.White;
		}

		#endregion

		#region Event handlers

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			ShowIntroMessage();
			tbxName.Focus();
		}

		private void btnSayHello_Click(object sender, RoutedEventArgs e)
		{
			SayHello();
		}

		private void btnShowIntroMessage_Click(object sender, RoutedEventArgs e)
		{
			ShowIntroMessage();
		}

		#endregion
	}
}