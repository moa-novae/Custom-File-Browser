using System;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using Q1.State;
using Q1.Services;

namespace Q1
{
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            IServiceProvider serviceProvider = CreateServiceProvider();
            Window window = new MainWindow();
            serviceProvider.GetRequiredService<UserStateServices>();
            window.DataContext = serviceProvider.GetRequiredService<MainViewModel>();
            window.Show();

        }
        // The dependency injection probably could've done in a better way
        // Needs more work
        private IServiceProvider CreateServiceProvider()
        {
            IServiceCollection services = new ServiceCollection();

            services.AddScoped<UserStateServices>();
            services.AddSingleton<UserState>();
            services.AddScoped<DirectoryItemStateServices>();
            // The folder which you monitor can be changed in app.config
            services.AddSingleton<DirectoryItemState>(state =>
            new DirectoryItemState(Q1.Properties.Settings.Default.DirectoryPath));
            services.AddScoped<MainViewModel>();

            return services.BuildServiceProvider();
        }



    }
}
