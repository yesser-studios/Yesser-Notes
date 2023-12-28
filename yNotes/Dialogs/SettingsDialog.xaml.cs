using Windows.ApplicationModel;
using Windows.System.Profile;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

// Dokumentaci k šabloně položky Dialog obsahu najdete na adrese https://go.microsoft.com/fwlink/?LinkId=234238

namespace yNotes
{
    public sealed partial class SettingsDialog : ContentDialog
    {
        bool[] options =
        {
            true,
            false
        };

        MainPage main;

        string version;

        public SettingsDialog()
        {
            InitializeComponent();
        }

        public SettingsDialog(bool[] options, MainPage mainPage)
        {
            InitializeComponent();

            this.options = options;
            main = mainPage;

            SaveDStateSaving.IsOn = options[0];

            version = GetAppVersion();
            VersionL.Text += version;
        }

        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            PassOptions();
            main.SaveStuff();
        }

        private void PassOptions()
        {
            main.options = options;
        }

        private void SaveDStateSaving_Toggled(object sender, RoutedEventArgs e)
        {
            options[0] = SaveDStateSaving.IsOn;
        }

        public static string GetAppVersion()
        {
            Package package = Package.Current;
            PackageId packageId = package.Id;
            PackageVersion version = packageId.Version;

            return $"{version.Major}.{version.Minor}.{version.Build}.{version.Revision}";
        }

        #region Keyboard Controls

        private void ContentDialog_KeyUp(object sender, KeyRoutedEventArgs e)
        {
            if (AnalyticsInfo.VersionInfo.DeviceFamily == "Windows.Xbox")
            {
                e.Handled = true;
                return;
            }

            if (e.Key == Windows.System.VirtualKey.Up)
            {
                // Mimic Shift+Tab when user hits up arrow key.
                bool focused = FocusManager.TryMoveFocus(FocusNavigationDirection.Up);
            }
            else if (e.Key == Windows.System.VirtualKey.Down)
            {
                // Mimic Tab when user hits down arrow key.
                bool focused = FocusManager.TryMoveFocus(FocusNavigationDirection.Down);
            }
            else if (e.Key == Windows.System.VirtualKey.Left)
            {
                // Mimic Tab when user hits down arrow key.
                FocusManager.TryMoveFocus(FocusNavigationDirection.Left);
            }
            else if (e.Key == Windows.System.VirtualKey.Right)
            {
                // Mimic Tab when user hits down arrow key.
                FocusManager.TryMoveFocus(FocusNavigationDirection.Right);
            }

            e.Handled = true;
        }

        #endregion
    }
}
