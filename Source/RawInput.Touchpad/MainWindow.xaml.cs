using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace RawInput.Touchpad
{
	public partial class MainWindow : Window
	{
		public bool TouchpadExists
		{
			get { return (bool)GetValue(TouchpadExistsProperty); }
			set { SetValue(TouchpadExistsProperty, value); }
		}
		public static readonly DependencyProperty TouchpadExistsProperty =
			DependencyProperty.Register("TouchpadExists", typeof(bool), typeof(MainWindow), new PropertyMetadata(false));

		public string TouchpadContacts
		{
			get { return (string)GetValue(TouchpadContactsProperty); }
			set { SetValue(TouchpadContactsProperty, value); }
		}
		public static readonly DependencyProperty TouchpadContactsProperty =
			DependencyProperty.Register("TouchpadContacts", typeof(string), typeof(MainWindow), new PropertyMetadata(null));

		public MainWindow()
		{
			InitializeComponent();
			
			// Timer to detect when all contacts are lifted
			_noContactTimer = new DispatcherTimer
			{
				Interval = TimeSpan.FromMilliseconds(100) // Check every 100ms
			};
			_noContactTimer.Tick += OnNoContactTimer;
		}

		private HwndSource _targetSource;
		private readonly List<string> _log = new();
		private DispatcherTimer _noContactTimer;

		protected override void OnSourceInitialized(EventArgs e)
		{
			base.OnSourceInitialized(e);

			_targetSource = PresentationSource.FromVisual(this) as HwndSource;
			_targetSource?.AddHook(WndProc);

			TouchpadExists = TouchpadHelper.Exists();

			_log.Add($"Precision touchpad exists: {TouchpadExists}");

			if (TouchpadExists)
			{
				var success = TouchpadHelper.RegisterInput(_targetSource.Handle);

				_log.Add($"Precision touchpad registered: {success}");
			}
		}

		private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
		{
			switch (msg)
			{
				case TouchpadHelper.WM_INPUT:
					var contacts = TouchpadHelper.ParseInput(lParam);
					if (contacts != null && contacts.Length > 0)
					{
						TouchpadContacts = string.Join(Environment.NewLine, contacts.Select(x => x.ToString()));
						_log.Add("---");
						_log.Add($"Contacts: {contacts.Length}");
						_log.Add(TouchpadContacts);
						
						// Reset timer since we have active contacts
						_noContactTimer.Stop();
						_noContactTimer.Start();
					}
					else
					{
						// No contacts in this message, start timer to clear display
						_noContactTimer.Start();
					}
					break;
			}
			return IntPtr.Zero;
		}

		private void OnNoContactTimer(object sender, EventArgs e)
		{
			// Timer elapsed without new contacts, assume all fingers lifted
			_noContactTimer.Stop();
			TouchpadContacts = "No contacts (0)";
			_log.Add("---");
			_log.Add("No contacts detected");
		}

		private void Copy_Click(object sender, RoutedEventArgs e)
		{
			Clipboard.SetText(string.Join(Environment.NewLine, _log));
		}
	}
}