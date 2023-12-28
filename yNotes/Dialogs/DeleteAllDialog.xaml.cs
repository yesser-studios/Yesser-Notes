using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.System.Profile;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// Dokumentaci k šabloně položky Dialog obsahu najdete na adrese https://go.microsoft.com/fwlink/?LinkId=234238

namespace yNotes.Dialogs
{
    public sealed partial class DeleteAllDialog : ContentDialog
    {
        public delegate void DeleteAllEventHandler(object sender, EventArgs args);
        public event DeleteAllEventHandler DeleteAllEvent;

        public DeleteAllDialog()
        {
            this.InitializeComponent();
        }

        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            DeleteAllEvent.Invoke(this, EventArgs.Empty);
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
