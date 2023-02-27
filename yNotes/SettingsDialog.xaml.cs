using Windows.ApplicationModel;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

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

            return string.Format("{0}.{1}.{2}.{3}", version.Major, version.Minor, version.Build, version.Revision);
        }
    }
}
