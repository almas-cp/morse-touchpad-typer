using System;
using System.Collections.Generic;
using System.ComponentModel;
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

		public string CurrentMorseSequence
		{
			get { return (string)GetValue(CurrentMorseSequenceProperty); }
			set { SetValue(CurrentMorseSequenceProperty, value); }
		}
		public static readonly DependencyProperty CurrentMorseSequenceProperty =
			DependencyProperty.Register("CurrentMorseSequence", typeof(string), typeof(MainWindow), new PropertyMetadata(""));

		public string LastTypedCharacter
		{
			get { return (string)GetValue(LastTypedCharacterProperty); }
			set { SetValue(LastTypedCharacterProperty, value); }
		}
		public static readonly DependencyProperty LastTypedCharacterProperty =
			DependencyProperty.Register("LastTypedCharacter", typeof(string), typeof(MainWindow), new PropertyMetadata(""));

		public MainWindow()
		{
			InitializeComponent();
			
			// Timer to detect when all contacts are lifted
			_noContactTimer = new DispatcherTimer
			{
				Interval = TimeSpan.FromMilliseconds(100) // Check every 100ms
			};
			_noContactTimer.Tick += OnNoContactTimer;

			// Initialize morse code typer
			_morseTyper = new MorseCodeTyper();
			_morseTyper.MorseSequenceUpdated += OnMorseSequenceUpdated;
			_morseTyper.CharacterTyped += OnCharacterTyped;

			// Start minimized to tray
			this.WindowState = WindowState.Minimized;
			this.ShowInTaskbar = false;
			
			// Show startup notification
			LastTypedCharacter = "Morse Touchpad Typer started - Enable to begin typing globally";
		}

		private HwndSource _targetSource;
		private readonly List<string> _log = new();
		private DispatcherTimer _noContactTimer;
		private MorseCodeTyper _morseTyper;
		private bool _isClosing = false;

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
						
						// Process contacts for morse code
						_morseTyper.ProcessContacts(contacts);
						
						// Reset timer since we have active contacts
						_noContactTimer.Stop();
						_noContactTimer.Start();
					}
					else
					{
						// No contacts in this message, process for morse code
						_morseTyper.ProcessContacts(new TouchpadContact[0]);
						
						// Start timer to clear display
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

		private void MorseEnabled_Checked(object sender, RoutedEventArgs e)
		{
			_morseTyper.IsEnabled = true;
			EnableMorseMenuItem.IsChecked = true;
			LastTypedCharacter = "Morse typer enabled - Text selection gestures suppressed";
		}

		private void MorseEnabled_Unchecked(object sender, RoutedEventArgs e)
		{
			_morseTyper.IsEnabled = false;
			EnableMorseMenuItem.IsChecked = false;
			CurrentMorseSequence = "";
			LastTypedCharacter = "Morse typer disabled";
		}

		private void OnMorseSequenceUpdated(string sequence)
		{
			Dispatcher.Invoke(() =>
			{
				CurrentMorseSequence = string.IsNullOrEmpty(sequence) ? "" : $"Current: {sequence}";
			});
		}

		private void OnCharacterTyped(char character)
		{
			Dispatcher.Invoke(() =>
			{
				LastTypedCharacter = $"Typed: '{character}'";
				_log.Add($"Morse typed: {character}");
			});
		}

		private void Window_StateChanged(object sender, EventArgs e)
		{
			if (WindowState == WindowState.Minimized)
			{
				this.ShowInTaskbar = false;
				TrayIcon.Visibility = Visibility.Visible;
			}
		}

		private void Window_Closing(object sender, CancelEventArgs e)
		{
			if (!_isClosing)
			{
				e.Cancel = true;
				this.WindowState = WindowState.Minimized;
			}
		}

		private void ShowWindow_Click(object sender, RoutedEventArgs e)
		{
			this.Show();
			this.WindowState = WindowState.Normal;
			this.ShowInTaskbar = true;
			this.Activate();
		}

		private void ToggleMorse_Click(object sender, RoutedEventArgs e)
		{
			var menuItem = sender as MenuItem;
			if (menuItem.IsChecked)
			{
				_morseTyper.IsEnabled = true;
				MorseEnabledCheckBox.IsChecked = true;
				LastTypedCharacter = "Morse typer enabled - Text selection gestures suppressed";
			}
			else
			{
				_morseTyper.IsEnabled = false;
				MorseEnabledCheckBox.IsChecked = false;
				CurrentMorseSequence = "";
				LastTypedCharacter = "Morse typer disabled - Text selection restored";
			}
		}

		private void Exit_Click(object sender, RoutedEventArgs e)
		{
			_isClosing = true;
			_morseTyper?.Dispose(); // Clean up gesture suppressor
			TrayIcon.Dispose();
			Application.Current.Shutdown();
		}
	}
}