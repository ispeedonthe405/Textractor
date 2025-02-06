using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using sbavalonia.symbols;
using System.Linq;
using Textractor.ViewModels;
using Textractor.Views;

namespace Textractor
{
    public partial class App : Application
    {
        public override void Initialize()
        {
            ActualThemeVariantChanged += App_ActualThemeVariantChanged;
            AvaloniaXamlLoader.Load(this);
        }

        private void App_ActualThemeVariantChanged(object? sender, System.EventArgs e)
        {
            if (((string)ActualThemeVariant.Key).Equals("Light"))
            {
                SymbolManager.SymbolColor = Colors.Black;
            }
            else
            {
                SymbolManager.SymbolColor = Colors.Ivory;
            }
        }

        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                // Avoid duplicate validations from both Avalonia and the CommunityToolkit. 
                // More info: https://docs.avaloniaui.net/docs/guides/development-guides/data-validation#manage-validationplugins
                DisableAvaloniaDataAnnotationValidation();
                desktop.MainWindow = new MainWindow
                {
                    DataContext = new VM_MainWindow(),
                };
            }

            base.OnFrameworkInitializationCompleted();
        }

        private void DisableAvaloniaDataAnnotationValidation()
        {
            // Get an array of plugins to remove
            var dataValidationPluginsToRemove =
                BindingPlugins.DataValidators.OfType<DataAnnotationsValidationPlugin>().ToArray();

            // remove each entry found
            foreach (var plugin in dataValidationPluginsToRemove)
            {
                BindingPlugins.DataValidators.Remove(plugin);
            }
        }
    }
}