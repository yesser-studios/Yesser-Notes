using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using System.Threading.Tasks;
using Windows.Devices.Enumeration.Pnp;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Automation.Peers;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.System.Profile;

// Dokumentaci k šabloně položky Prázdná stránka najdete na adrese https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x405

namespace yNotes
{
    /// <summary>
    /// Prázdná stránka, která se dá použít samostatně nebo v rámci objektu Frame
    /// </summary>
    public sealed partial class MainPage : Page
    {
        readonly ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;

        public bool[] options =
        {
            true,
            false
        };

        /// <summary>
        /// Used for editing.
        /// </summary>
        ListBoxItem selectedItem = null;

        public MainPage()
        {
            InitializeComponent();

            LoadStuff();

            editButtonSP.Visibility = Visibility.Collapsed;
        }

        private void saveB_Click(object sender, RoutedEventArgs e)
        {
            if (noteTB.Text.Length <= 0) return;

            ListBoxItem item = new ListBoxItem();
            item.Content = noteTB.Text;

            notesLB.Items.Add(item);

            if (options[0])
            {
                noteTB.Text = "";
            }

            SaveStuff();
        }

        private void deleteB_Click(object sender, RoutedEventArgs e)
        {
            object[] itemsToDelete = notesLB.SelectedItems.ToArray();

            foreach (object item in itemsToDelete)
            {
                notesLB.Items.Remove(item);
            }

            
            SaveStuff();
        }

        public void SaveStuff()
        {
            ItemCollection itemCollection = notesLB.Items;

            object[] objects = itemCollection.ToArray();

            List<string> strings = new List<string>();
            foreach (object obj in objects)
            {
                ListBoxItem lBI = obj as ListBoxItem;
                if (lBI == null) continue;
                string text = lBI.Content as string;
                strings.Add(text);
            }
            string[] stringArray = strings.ToArray();

            if (stringArray != null && stringArray.Length > 0)
                localSettings.Values["notes"] = stringArray;
            else
                localSettings.Values["notes"] = 1;

            localSettings.Values["options"] = options;
        }

        private void LoadStuff()
        {
            LoadOptions();
            LoadNotes();
        }

        private void LoadOptions()
        {
            object optionsRaw = localSettings.Values["options"];
            if (optionsRaw == null) return;

            bool[] optionsArray = optionsRaw as bool[];
            if (optionsArray == null || optionsArray.Length <= 0) return;

            options = optionsArray;
        }

        private void LoadNotes()
        {
            object raw = localSettings.Values["notes"];
            if (raw == null) return;

            object[] objects = raw as object[];
            if (objects == null) return;

            string[] strings = objects as string[];
            if (strings == null) return;

            foreach (string itemString in strings)
            {
                ListBoxItem item = new ListBoxItem();
                item.Content = itemString;

                notesLB.Items.Add(item);
            }
        }

        private void SettingsABB_Click(object sender, RoutedEventArgs e)
        {
            SettingsDialog dialog = new SettingsDialog(options, this);
            _ = dialog.ShowAsync();
        }

        private void ExportABB_Click(object sender, RoutedEventArgs e)
        {
            ItemCollection collection = notesLB.Items;
            object[] objects = collection.ToArray();

            if (objects == null || objects.Length <= 0)
                return;

            List<ListBoxItem> items = new List<ListBoxItem>();
            foreach (ListBoxItem item in objects)
            {
                items.Add(item);
            }

            ExportDialog dialog = new ExportDialog(items.ToArray());
            _ = dialog.ShowAsync();
        }

        private void ClearAllABB_Click(object sender, RoutedEventArgs e)
        {
            ClearFO.Hide();

            noteTB.Focus(FocusState.Keyboard);

            notesLB.Items.Clear();
            SaveStuff();
        }

        private void editB_Click(object sender, RoutedEventArgs e)
        {
            object selectedRaw = notesLB.SelectedItem;
            selectedItem = selectedRaw as ListBoxItem;

            if (selectedItem != null)
            {
                mainButtonSP.Visibility = Visibility.Collapsed;
                editButtonSP.Visibility = Visibility.Visible;

                noteTB.Text = selectedItem.Content as string;
                noteTB.Focus(FocusState.Keyboard);
            }
            else
            {
                editNoNoteSelectedTT.IsOpen = true;
            }

        }

        private void editSaveB_Click(object sender, RoutedEventArgs e)
        {
            selectedItem.Content = noteTB.Text;

            editButtonSP.Visibility = Visibility.Collapsed;
            mainButtonSP.Visibility = Visibility.Visible;

            noteTB.Text = "";

            SaveStuff();
        }

        private void editCancelB_Click(object sender, RoutedEventArgs e)
        {
            editButtonSP.Visibility = Visibility.Collapsed;
            mainButtonSP.Visibility = Visibility.Visible;

            noteTB.Text = "";
        }

        private void ImportABB_Click(object sender, RoutedEventArgs e)
        {
            ImportDialog dialog = new ImportDialog(this);
            _ = dialog.ShowAsync();
        }

        public void ImportNotes(IList<string> notes)
        {
            notesLB.Items.Clear();
            foreach (string note in notes)
            {
                ListBoxItem item = new ListBoxItem();
                item.Content = note;
                notesLB.Items.Add(item);
            }
        }

        private void XboxSupport()
        {
            if (AnalyticsInfo.VersionInfo.DeviceFamily == "Windows.Xbox")
            {
                ExportABB.Visibility = Visibility.Collapsed;
                ImportABB.Visibility = Visibility.Collapsed;
            }
        }
    }
}
