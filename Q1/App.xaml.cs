using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using Q1.State;

namespace Q1
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            IServiceProvider serviceProvider = CreateServiceProvider();
            Window window = new MainWindow();
            serviceProvider.GetRequiredService<UserServices>();
            window.DataContext = serviceProvider.GetRequiredService<MainViewModel>();
            window.Show();

        }

        private IServiceProvider CreateServiceProvider()
        {
            IServiceCollection services = new ServiceCollection();

            services.AddSingleton<UserServices>();
            services.AddSingleton<UsersStore>();
            services.AddScoped<MainViewModel>();

            return services.BuildServiceProvider();
        }


        
    }
}
