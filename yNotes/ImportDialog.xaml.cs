using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.Pickers;
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
    }
}
