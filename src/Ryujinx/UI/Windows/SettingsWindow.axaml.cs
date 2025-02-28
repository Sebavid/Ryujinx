using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using FluentAvalonia.UI.Controls;
using Ryujinx.Ava.Common.Locale;
using Ryujinx.Ava.UI.ViewModels;
using Ryujinx.HLE.FileSystem;
using Ryujinx.Input;
using System;
using System.Linq;
using Key = Avalonia.Input.Key;

namespace Ryujinx.Ava.UI.Windows
{
    public partial class SettingsWindow : StyleableAppWindow
    {
        internal readonly SettingsViewModel ViewModel;

        public SettingsWindow(VirtualFileSystem virtualFileSystem, ContentManager contentManager)
        {
            Title = RyujinxApp.FormatTitle(LocaleKeys.Settings);

            DataContext = ViewModel = new SettingsViewModel(virtualFileSystem, contentManager);

            ViewModel.CloseWindow += Close;
            ViewModel.SaveSettingsEvent += SaveSettings;

            InitializeComponent();
            Load();

#if DEBUG
            this.AttachDevTools(new KeyGesture(Key.F12, KeyModifiers.Alt));
#endif
        }

        public SettingsWindow()
        {
            DataContext = ViewModel = new SettingsViewModel();

            InitializeComponent();
            Load();
        }

        public void SaveSettings()
        {
            InputPage.InputView?.SaveCurrentProfile();

            if (Owner is MainWindow window && ViewModel.GameListNeedsRefresh)
            {
                window.LoadApplications();
            }
        }

        private void Load()
        {
            Pages.Children.Clear();
            NavPanel.SelectionChanged += NavPanelOnSelectionChanged;
            NavPanel.SelectedItem = NavPanel.MenuItems.ElementAt(0);
        }

        private void NavPanelOnSelectionChanged(object sender, NavigationViewSelectionChangedEventArgs e)
        {
            if (e.SelectedItem is NavigationViewItem navItem && navItem.Tag is not null)
            {
                switch (navItem.Tag.ToString())
                {
                    case nameof(UiPage):
                        UiPage.ViewModel = ViewModel;
                        NavPanel.Content = UiPage;
                        break;
                    case nameof(InputPage):
                        NavPanel.Content = InputPage;
                        break;
                    case nameof(HotkeysPage):
                        NavPanel.Content = HotkeysPage;
                        break;
                    case nameof(SystemPage):
                        SystemPage.ViewModel = ViewModel;
                        NavPanel.Content = SystemPage;
                        break;
                    case nameof(CpuPage):
                        NavPanel.Content = CpuPage;
                        break;
                    case nameof(GraphicsPage):
                        NavPanel.Content = GraphicsPage;
                        break;
                    case nameof(AudioPage):
                        NavPanel.Content = AudioPage;
                        break;
                    case nameof(NetworkPage):
                        NetworkPage.ViewModel = ViewModel;
                        NavPanel.Content = NetworkPage;
                        break;
                    case nameof(LoggingPage):
                        NavPanel.Content = LoggingPage;
                        break;
                    case nameof(HacksPage):
                        HacksPage.DataContext = ViewModel;
                        NavPanel.Content = HacksPage;
                        break;
                    default:
                        throw new NotImplementedException();
                }
            }
        }

        protected override void OnClosing(WindowClosingEventArgs e)
        {
            HotkeysPage.Dispose();
            
            foreach (IGamepad gamepad in RyujinxApp.MainWindow.InputManager.GamepadDriver.GetGamepads())
            {
                gamepad?.ClearLed();
            }
            
            InputPage.Dispose();
            base.OnClosing(e);
        }
    }
}
