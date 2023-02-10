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
    public sealed partial class ExportDialog : ContentDialog
    {
        ListBoxItem[] lBItems;
        List<string> lines = new List<string>();

        public ExportDialog(ListBoxItem[] lBItems)
        {
            this.InitializeComponent();
            this.lBItems = lBItems;

            GetLines();
            UpdateContent();
        }

        private async void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            FileSavePicker picker = new FileSavePicker();
            picker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
            picker.FileTypeChoices.Add("Plain Text", new List<string>() { ".txt" });
            picker.SuggestedFileName = "Exported";
            MessageDialog dialog;

            try
            {
                Windows.Storage.StorageFile file = await picker.PickSaveFileAsync();
                

                if (file != null)
                {
                    await FileIO.WriteLinesAsync(file, lines.ToArray());
                    Windows.Storage.Provider.FileUpdateStatus status = await CachedFileManager.CompleteUpdatesAsync(file);
                    if (status == Windows.Storage.Provider.FileUpdateStatus.Complete)
                    {
                        dialog = new MessageDialog("File saved successfully!", "File Saved");
                        await dialog.ShowAsync();
                    }
                    else
                    {
                        dialog = new MessageDialog("File could not save.", "Error While Saving File");
                        await dialog.ShowAsync();
                    }
                }
            }
            catch
            {
                dialog = new MessageDialog("File could not save. Check if the folder or file is editable by you!", "Error While Saving File");
            }
            
        }

        private void UpdateContent()
        {
            string text = "";

            foreach (var item in lines)
            {
                text += item.ToString() + "\n";
            }

            previewL.Text = text;
        }

        private void GetLines()
        {
            foreach (var item in lBItems)
            {
                lines.Add((string)item.Content);
            }
        }
    }
}
