using Microsoft.UI.Xaml;
using System.Linq;
using Windows.ApplicationModel.Activation;
using Windows.Storage;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace LightCode
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    public partial class App : Application
    {
        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Invoked when the application is launched.
        /// </summary>
        /// <param name="args">Details about the launch request and process.</param>
        protected override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
        {
            var appArgs = Microsoft.Windows.AppLifecycle.AppInstance.GetCurrent().GetActivatedEventArgs();

            if (appArgs.Kind is Microsoft.Windows.AppLifecycle.ExtendedActivationKind.File &&
               appArgs.Data is IFileActivatedEventArgs fileActivatedEventArgs &&
               fileActivatedEventArgs.Files.FirstOrDefault() is StorageFile file)
            {
                m_window = new MainWindow(file);
                m_window.Activate();
            }
            else
            {
                m_window = new MainWindow();
                m_window.Activate();
            }
        }

        private Window m_window;
    }
}
