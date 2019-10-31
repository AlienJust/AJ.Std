using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;

namespace AlienJust.Support.Wpf.Behaviours {
	public class WindowClosingBehavior {
		public static ICommand GetClosed(DependencyObject obj) {
			return (ICommand) obj.GetValue(ClosedProperty);
		}

		public static void SetClosed(DependencyObject obj, ICommand value) {
			obj.SetValue(ClosedProperty, value);
		}

		public static readonly DependencyProperty ClosedProperty = DependencyProperty.RegisterAttached("Closed", typeof (ICommand), typeof (WindowClosingBehavior), new UIPropertyMetadata(ClosedChanged));

		private static void ClosedChanged(DependencyObject target, DependencyPropertyChangedEventArgs e) {
			var window = target as Window;

			if (window != null) {
				if (e.NewValue != null) {
					window.Closed += WindowClosed;
				}
				else {
					window.Closed -= WindowClosed;
				}
			}
		}

		public static ICommand GetClosing(DependencyObject obj) {
			return (ICommand) obj.GetValue(ClosingProperty);
		}

		public static void SetClosing(DependencyObject obj, ICommand value) {
			obj.SetValue(ClosingProperty, value);
		}

		public static readonly DependencyProperty ClosingProperty = DependencyProperty.RegisterAttached("Closing", typeof (ICommand), typeof (WindowClosingBehavior), new UIPropertyMetadata(ClosingChanged));

		private static void ClosingChanged(DependencyObject target, DependencyPropertyChangedEventArgs e) {
			var window = target as Window;

			if (window != null) {
				if (e.NewValue != null) {
					window.Closing += WindowClosing;
				}
				else {
					window.Closing -= WindowClosing;
				}
			}
		}

		public static ICommand GetCancelClosing(DependencyObject obj) {
			return (ICommand) obj.GetValue(CancelClosingProperty);
		}

		public static void SetCancelClosing(DependencyObject obj, ICommand value) {
			obj.SetValue(CancelClosingProperty, value);
		}

		public static readonly DependencyProperty CancelClosingProperty = DependencyProperty.RegisterAttached("CancelClosing", typeof (ICommand), typeof (WindowClosingBehavior));

		private static void WindowClosed(object sender, EventArgs e) {
			ICommand closed = GetClosed(sender as Window);
			closed?.Execute(null);
		}

		private static void WindowClosing(object sender, CancelEventArgs e) {
			ICommand closing = GetClosing(sender as Window);
			if (closing != null) {
				if (closing.CanExecute(null)) {
					closing.Execute(null);
				}
				else {
					ICommand cancelClosing = GetCancelClosing(sender as Window);
					cancelClosing?.Execute(null);

					e.Cancel = true;
				}
			}
		}
	}
}