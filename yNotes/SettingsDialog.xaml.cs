using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

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
    }
}
