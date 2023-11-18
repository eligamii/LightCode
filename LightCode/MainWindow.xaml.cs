using Microsoft.UI;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;
using Monaco;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Windows.Storage;
using WinRT.Interop;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace LightCode
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        [DllImport("Shcore.dll", SetLastError = true)]
        internal static extern int GetDpiForMonitor(IntPtr hmonitor, Monitor_DPI_Type dpiType, out uint dpiX, out uint dpiY);

        internal enum Monitor_DPI_Type : int
        {
            MDT_Effective_DPI = 0,
            MDT_Angular_DPI = 1,
            MDT_Raw_DPI = 2,
            MDT_Default = MDT_Effective_DPI
        }

        private double GetScaleAdjustment()
        {
            IntPtr hWnd = WindowNative.GetWindowHandle(this);
            WindowId wndId = Win32Interop.GetWindowIdFromWindow(hWnd);
            DisplayArea displayArea = DisplayArea.GetFromWindowId(wndId, DisplayAreaFallback.Primary);
            IntPtr hMonitor = Win32Interop.GetMonitorFromDisplayId(displayArea.DisplayId);

            // Get DPI.
            int result = GetDpiForMonitor(hMonitor, Monitor_DPI_Type.MDT_Default, out uint dpiX, out uint _);
            if (result != 0)
            {
                throw new System.Exception("Could not get DPI for monitor.");
            }

            uint scaleFactorPercent = (uint)(((long)dpiX * 100 + (96 >> 1)) / 96);
            return scaleFactorPercent / 100.0;
        }

        private void SetDragRegionForCustomTitleBar(AppWindow appWindow)
        {
            try
            {
                // Check to see if customization is supported.
                // The method returns true on Windows 10 since Windows App SDK 1.2, and on all versions of
                // Windows App SDK on Windows 11.
                if (AppWindowTitleBar.IsCustomizationSupported()
                    && appWindow.TitleBar.ExtendsContentIntoTitleBar)
                {
                    double scaleAdjustment = GetScaleAdjustment();
                    List<Windows.Graphics.RectInt32> dragRectsList = new();

                    Windows.Graphics.RectInt32 dragRect;
                    dragRect.X = (int)(controlGrid.ActualWidth * scaleAdjustment);
                    dragRect.Y = 0;
                    dragRect.Height = (int)(appTitleBarGrid.ActualHeight * scaleAdjustment);
                    dragRect.Width = (int)((appTitleBarGrid.ActualWidth - controlGrid.ActualWidth) * scaleAdjustment);
                    dragRectsList.Add(dragRect);

                    Windows.Graphics.RectInt32[] dragRects = dragRectsList.ToArray();

                    appWindow.TitleBar.SetDragRectangles(dragRects);
                }
            }
            catch { }
        }
        private void AppTitleBarGrid_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            SetDragRegionForCustomTitleBar(AppWindow);
        }

        private void AppTitleBarGrid_Loaded(object sender, RoutedEventArgs e)
        {
            SetDragRegionForCustomTitleBar(AppWindow);
        }

        public MainWindow()
        {
            this.InitializeComponent();
            this.ExtendsContentIntoTitleBar = true;
            this.SetTitleBar(appTitleBarGrid);
            this.SystemBackdrop = new MicaBackdrop();

            appTitleBarGrid.Loaded += AppTitleBarGrid_Loaded;
            appTitleBarGrid.SizeChanged += AppTitleBarGrid_SizeChanged;
        }

        private StorageFile _file;
        public MainWindow(StorageFile file)
        {
            this.InitializeComponent();
            this.ExtendsContentIntoTitleBar = true;
            this.SetTitleBar(appTitleBarGrid);
            this.SystemBackdrop = new MicaBackdrop();
            _file = file;

            appTitleBarGrid.Loaded += AppTitleBarGrid_Loaded;
            appTitleBarGrid.SizeChanged += AppTitleBarGrid_SizeChanged;
        }


        private async void Editor_Loaded(object sender, object args)
        {
            Editor editor = sender as Editor;
            if (_file is null)
            {
                editor.SetLanguage(Language.CSharp);
            }
            else
            {
                await editor.OpenFileAsync(_file.Path);
            }
        }

        private async void SaveMenuFlyoutItem_Click(object sender, RoutedEventArgs e)
        {
            await editor.SaveAsync();
        }

        private async void SaveAsMenuFlyoutItem_Click(object sender, RoutedEventArgs e)
        {
            //await editor.SaveAsAsync(this);
        }
    }
}
