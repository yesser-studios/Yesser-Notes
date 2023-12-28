using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.System.Profile;
using Windows.UI.Popups;
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
    public sealed partial class ImportDialog : ContentDialog
    {
        IList<string> lines = new List<string>();
        MainPage main;

        public ImportDialog(MainPage main)
        {
            InitializeComponent();
            this.main = main;
        }

        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            main.ImportNotes(lines);
        }

        private async void SelectFileB_Click(object sender, RoutedEventArgs e)
        {
            FileOpenPicker picker = new FileOpenPicker();
            picker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
            picker.FileTypeFilter.Add(".txt");

            StorageFile file = await picker.PickSingleFileAsync();
            if (file == null) return;
            lines = await FileIO.ReadLinesAsync(file);

            UpdateUI(file);
            IsPrimaryButtonEnabled = true;
        }

        private void UpdateUI(StorageFile file)
        {
            NoSelectedFileL.Visibility = Visibility.Collapsed;

            SelectedFileL.Text += file.DisplayName;

            SelectedFileL.Visibility = Visibility.Visible;
            previewHeaderL.Visibility = Visibility.Visible;

            previewL.Text = string.Empty;
            foreach (string line in lines)
            {
                previewL.Text += line + "\n";
            }

            previewL.Visibility = Visibility.Visible;
        }

        #region Keyboard Controls

        private void ContentDialog_KeyUp(object sender, KeyRoutedEventArgs e)
        {
            if (MainPage.gamepadKeys.Contains(e.OriginalKey))
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
