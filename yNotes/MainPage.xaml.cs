using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.System.Profile;
using Windows.UI;
using Windows.ApplicationModel;
using Windows.ApplicationModel.DataTransfer;
using yNotes.Dialogs;
using Windows.UI.Xaml.Input;
using Windows.System;

// Dokumentaci k šabloně položky Prázdná stránka najdete na adrese https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x405

namespace yNotes
{
    /// <summary>
    /// Prázdná stránka, která se dá použít samostatně nebo v rámci objektu Frame
    /// </summary>
    public sealed partial class MainPage : Page
    {
        PackageVersion version;

        public int updateID;
        int prevUpdateID;

        readonly ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;

        public bool[] options =
        {
            true,
            false,
            false
        };

        public static readonly VirtualKey[] gamepadKeys =
        {
            VirtualKey.GamepadA,
            VirtualKey.GamepadB,
            VirtualKey.GamepadX,
            VirtualKey.GamepadY,

            VirtualKey.GamepadDPadDown,
            VirtualKey.GamepadDPadLeft,
            VirtualKey.GamepadDPadUp,
            VirtualKey.GamepadDPadRight,

            VirtualKey.GamepadLeftShoulder,
            VirtualKey.GamepadRightShoulder,

            VirtualKey.GamepadLeftThumbstickButton,
            VirtualKey.GamepadRightThumbstickButton,

            VirtualKey.GamepadLeftThumbstickDown,
            VirtualKey.GamepadLeftThumbstickUp,
            VirtualKey.GamepadLeftThumbstickLeft,
            VirtualKey.GamepadLeftThumbstickRight,
            VirtualKey.GamepadRightThumbstickDown,
            VirtualKey.GamepadRightThumbstickUp,
            VirtualKey.GamepadRightThumbstickLeft,
            VirtualKey.GamepadRightThumbstickRight,

            VirtualKey.GamepadLeftTrigger,
            VirtualKey.GamepadRightTrigger,

            VirtualKey.GamepadMenu,
            VirtualKey.GamepadView,
        };

        /// <summary>
        /// Used for editing.
        /// </summary>
        TextBlock selectedBlock = null;

        Brush defaultPRColor;
        Brush redPRColor;

        public MainPage()
        {
            InitializeComponent();

            defaultPRColor = noteLengthPR.Foreground;
            redPRColor = new SolidColorBrush(Color.FromArgb(255, 255, 0, 0));

            LoadStuff();

            editButtonSP.Visibility = Visibility.Collapsed;

            XboxSupport();
        }

        private void saveB_Click(object sender, RoutedEventArgs e)
        {
            if (noteTB.Text.Length <= 0) return;

            /*
            if (notify)
            {
                DateTimeOffset time = (DateTimeOffset) (NotifyDate.Date + NotifyTime.Time);

                new ToastContentBuilder().AddArgument(notesLB.Items.Count.ToString()).AddText(noteTB.Text).Schedule(time);

                
                ToastNotifier notifier = ToastNotificationManager.CreateToastNotifier();

                XmlDocument document = new XmlDocument();
                document.CreateTextNode(noteTB.Text);

                ScheduledToastNotification scheduledToast = new ScheduledToastNotification(document, time);

                notifier.AddToSchedule(scheduledToast);
                
            }
            */
        
            TextBlock block = new TextBlock();
            block.TextWrapping = TextWrapping.WrapWholeWords;
            block.Text = noteTB.Text;

            notesLB.Items.Add(block);

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
                TextBlock lBI = obj as TextBlock;
                if (lBI == null) continue;
                string text = lBI.Text;
                strings.Add(text);
            }
            string[] stringArray = strings.ToArray();

            if (stringArray != null && stringArray.Length > 0)
                localSettings.Values["notes"] = stringArray;
            else
                localSettings.Values["notes"] = 1;

            localSettings.Values["options"] = options;

            localSettings.Values[nameof(updateID)] = updateID;
        }

        private void LoadStuff()
        {
            LoadOptions();
            LoadNotes();
            LoadVersion();
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
            notesLB.Items.Clear();

            object raw = localSettings.Values["notes"];
            if (raw == null) return;

            object[] objects = raw as object[];
            if (objects == null) return;

            string[] strings = objects as string[];
            if (strings == null) return;

            foreach (string itemString in strings)
            {
                TextBlock block = new TextBlock();
                block.TextWrapping = TextWrapping.WrapWholeWords;
                block.Text = itemString;

                notesLB.Items.Add(block);
            }
        }

        private void LoadVersion()
        {
            LoadCurrentVersion();

            object raw = localSettings.Values[nameof(updateID)];

            int? prevVersion = raw as int?;
            if (prevVersion == null || prevVersion < updateID) ShowWhatsNewPopup();

            prevUpdateID = prevVersion != null ? (int)prevVersion : updateID;

            SaveStuff();
        }

        private void LoadCurrentVersion()
        {
            Package package = Package.Current;
            PackageId packageId = package.Id;
            version = packageId.Version;

            updateID = (version.Major * 1000000) + (version.Minor * 1000) + version.Build;
        }

        private void ShowWhatsNewPopup()
        {
            WhatsNewTT.IsOpen = true;
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

            List<TextBlock> items = new List<TextBlock>();
            foreach (TextBlock item in objects)
            {
                items.Add(item);
            }

            ExportDialog dialog = new ExportDialog(items.ToArray());
            _ = dialog.ShowAsync();
        }

        private async void ClearAllABB_Click(object sender, RoutedEventArgs e)
        {
            DeleteAllDialog dialog = new DeleteAllDialog();
            dialog.DeleteAllEvent += (object s, EventArgs args) =>
            {
                noteTB.Focus(FocusState.Keyboard);

                notesLB.Items.Clear();
                SaveStuff();
            };

            await dialog.ShowAsync();
        }

        #region Edit
        private void editB_Click(object sender, RoutedEventArgs e)
        {
            object selectedRaw = notesLB.SelectedItem;
            selectedBlock = selectedRaw as TextBlock;

            if (selectedBlock != null)
            {
                mainButtonSP.Visibility = Visibility.Collapsed;
                editButtonSP.Visibility = Visibility.Visible;

                noteTB.Text = selectedBlock.Text;
                noteTB.Focus(FocusState.Keyboard);
            }
            else
            {
                editNoNoteSelectedTT.IsOpen = true;
            }
        }

        private void editSaveB_Click(object sender, RoutedEventArgs e)
        {
            selectedBlock.Text = noteTB.Text;

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
        #endregion

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
                TextBlock item = new TextBlock();
                item.Text = note;
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

            // Disable overscan
            Windows.UI.ViewManagement.ApplicationView.GetForCurrentView().SetDesiredBoundsMode(Windows.UI.ViewManagement.ApplicationViewBoundsMode.UseCoreWindow);
        }

        private void noteTB_TextChanging(TextBox sender, TextBoxTextChangingEventArgs args)
        {
            double value = (double)noteTB.Text.Length / noteTB.MaxLength * 100;
            noteLengthPR.Value = value;

            noteLengthPR.Foreground = value == 100 ? redPRColor : defaultPRColor;

            _ = noteLengthPR.Width;
        }

        #region Keyboard Controls

        private async void Page_KeyUp(object sender, KeyRoutedEventArgs e)
        {
            if (gamepadKeys.Contains(e.OriginalKey))
            {
                e.Handled = true;
                return;
            }

            if (e.Key == Windows.System.VirtualKey.Up)
            {
                // Mimic Shift+Tab when user hits up arrow key.
                bool focused = FocusManager.TryMoveFocus(FocusNavigationDirection.Up);
                if (!focused)
                    await FocusManager.TryFocusAsync(ImportABB, FocusState.Keyboard);
            }
            else if (e.Key == Windows.System.VirtualKey.Down)
            {
                // Mimic Tab when user hits down arrow key.
                bool focused = FocusManager.TryMoveFocus(FocusNavigationDirection.Down);
                if (!focused)
                    await FocusManager.TryFocusAsync(ImportABB, FocusState.Keyboard);
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

        private async void mainCB_KeyUp(object sender, KeyRoutedEventArgs e)
        {
            if (gamepadKeys.Contains(e.OriginalKey))
            {
                e.Handled = true;
                return;
            }

            var selected = FocusManager.GetFocusedElement();

            if (e.Key == Windows.System.VirtualKey.Down)
            {
                if (selected != ClearAllABB && selected != ViewFullChangelogABB)
                    await FocusManager.TryFocusAsync(notesLB, FocusState.Keyboard);
            }
            else if (e.Key == Windows.System.VirtualKey.Up)
            {
                await FocusManager.TryFocusAsync(deleteB, FocusState.Keyboard);
            }

            e.Handled = true;
        }

        #endregion

        /* Notification
        private void NotificationB_Click(object sender, RoutedEventArgs e)
        {
            
            if (!NotifyFlyout.IsOpen) NotifyFlyout.Hide();
            
        }

        private void NotifyOKB_Click(object sender, RoutedEventArgs e)
        {
            
            if (NotifyDate != null && NotifyTime != null)
            {
                notify = true;
                NotifyFlyout.Hide();
                NotificationB.Style = NotifyOKB.Style;
            }
            else
            {
                notifyNoDateTimeTT.IsOpen = true;
            }
            
        }

        private void NotifyCancelB_Click(object sender, RoutedEventArgs e)
        {
            notify = false;
            NotifyFlyout.Hide();
            NotificationB.Style = NotifyCancelB.Style;
        }
        */

        #region Copy, Cut and Paste

        private void copyB_Click(object sender, RoutedEventArgs e)
        {
            if (FocusManager.GetFocusedElement() == noteTB)
                return;

            object selectedRaw = notesLB.SelectedItem;
            selectedBlock = selectedRaw as TextBlock;

            if (selectedBlock != null)
            {
                try
                {
                    var dataPackage = new DataPackage();
                    dataPackage.SetText(selectedBlock.Text);
                    Clipboard.SetContent(dataPackage);
                    copyBFlyout.ShowAt(copyB);
                }
                catch { return; }
            }
        }

        private void cutB_Click(object sender, RoutedEventArgs e)
        {
            if (FocusManager.GetFocusedElement() == noteTB)
                return;

            object selectedRaw = notesLB.SelectedItem;
            selectedBlock = selectedRaw as TextBlock;

            if (selectedBlock != null)
            {
                try
                {
                    var dataPackage = new DataPackage();
                    dataPackage.SetText(selectedBlock.Text);
                    Clipboard.SetContent(dataPackage);
                    deleteB_Click(sender, e);
                    copyBFlyout.ShowAt(copyB);
                }
                catch { return; }
            }
        }

        private async void pasteB_Click(object sender, RoutedEventArgs e)
        {
            if (FocusManager.GetFocusedElement() == noteTB)
                return;

            var dataPackageView = Clipboard.GetContent();
            if (!dataPackageView.Contains("text")) return;
            string text = await dataPackageView.GetTextAsync();

            if (text == null) return;

            TextBlock block = new TextBlock();
            block.TextWrapping = TextWrapping.WrapWholeWords;
            block.Text = text;

            if (block == null) return;

            notesLB.Items.Add(block);

            SaveStuff();
        }

        #endregion

        private async void FullChangelogB_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new ChangelogDialog();
            await dialog.ShowAsync();
        }

        /*
        #region Register for printing

        private PrintManager printMan;
        private PrintDocument printDoc;
        private IPrintDocumentSource printDocSource;
        private TextBlock textBlock;
        private Canvas canvas;

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            // Register for PrintTaskRequested event
            printMan = PrintManager.GetForCurrentView();
            printMan.PrintTaskRequested += PrintTaskRequested;

            // Build a PrintDocument and register for callbacks
            printDoc = new PrintDocument();
            printDocSource = printDoc.DocumentSource;
            printDoc.Paginate += Paginate;
            printDoc.GetPreviewPage += GetPreviewPage;
            printDoc.AddPages += AddPages;
        }

        #endregion

        #region Showing the print dialog

        private async void PrintButtonClick(object sender, RoutedEventArgs e)
        {
            if (PrintManager.IsSupported())
            {
                try
                {
                    // Show print UI
                    await PrintManager.ShowPrintUIAsync();
                }
                catch
                {
                    // Printing cannot proceed at this time
                    ContentDialog noPrintingDialog = new ContentDialog()
                    {
                        Title = "Printing error",
                        Content = "\nSorry, printing can' t proceed at this time.",
                        PrimaryButtonText = "OK"
                    };
                    await noPrintingDialog.ShowAsync();
                }
            }
            else
            {
                // Printing is not supported on this device
                ContentDialog noPrintingDialog = new ContentDialog()
                {
                    Title = "Printing not supported",
                    Content = "\nSorry, printing is not supported on this device.",
                    PrimaryButtonText = "OK"
                };
                await noPrintingDialog.ShowAsync();
            }
        }

        private void PrintTaskRequested(PrintManager sender, PrintTaskRequestedEventArgs args)
        {
            // Create the PrintTask.
            // Defines the title and delegate for PrintTaskSourceRequested
            var printTask = args.Request.CreatePrintTask("Print", PrintTaskSourceRequrested);

            // Handle PrintTask.Completed to catch failed print jobs
            printTask.Completed += PrintTaskCompleted;
        }

        private void PrintTaskSourceRequrested(PrintTaskSourceRequestedArgs args)
        {
            // Set the document source.
            args.SetSource(printDocSource);
        }

        #endregion

        #region Print preview

        private void Paginate(object sender, PaginateEventArgs e)
        {
            // As I only want to print one Rectangle, so I set the count to 1
            printDoc.SetPreviewPageCount(1, PreviewPageCountType.Final);
        }

        private void GetPreviewPage(object sender, GetPreviewPageEventArgs e)
        {
            textBlock = new TextBlock
            {
                Foreground = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0)),
                FontFamily = FontFamily.XamlAutoFontFamily,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };
            string text = "";

            foreach (object item in notesLB.Items)
            {
                TextBlock itemAsTB = item as TextBlock;
                if (itemAsTB != null)
                {
                    text += itemAsTB.Text + "\n";
                }
            }

            textBlock.Text = text;

            canvas = new Canvas();
            canvas.HorizontalAlignment = HorizontalAlignment.Stretch;
            canvas.VerticalAlignment = VerticalAlignment.Stretch;
            canvas.Children.Add(textBlock);

            // Provide a UIElement as the print preview.
            printDoc.SetPreviewPage(e.PageNumber, canvas);
        }

        #endregion

        #region Add pages to send to the printer

        private void AddPages(object sender, AddPagesEventArgs e)
        {
            printDoc.AddPage(textBlock);

            // Indicate that all of the print pages have been provided
            printDoc.AddPagesComplete();
        }

        #endregion

        #region Print task completed

        private async void PrintTaskCompleted(PrintTask sender, PrintTaskCompletedEventArgs args)
        {
            // Notify the user when the print operation fails.
            if (args.Completion == PrintTaskCompletion.Failed)
            {
                await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
                {
                    ContentDialog noPrintingDialog = new ContentDialog()
                    {
                        Title = "Printing error",
                        Content = "\nSorry, failed to print.",
                        PrimaryButtonText = "OK"
                    };
                    await noPrintingDialog.ShowAsync();
                });
            }
        }

        #endregion
        */
    }
}