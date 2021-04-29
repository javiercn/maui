using System;
using Microsoft.Maui;
using Microsoft.Maui.Controls.Compatibility;
using Microsoft.Maui.Hosting;
using Microsoft.AspNetCore.Components.WebView.Maui;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Maui.Controls.Sample.SingleProject
{
	public class Startup : IStartup
	{
		internal static bool UseBlazor = false;

		public void Configure(IAppHostBuilder appBuilder)
		{
			appBuilder
				.UseFormsCompatibility()
				.RegisterBlazorMauiWebView()
				.UseMauiApp<MyApp>();

			if (UseBlazor)
            {
				appBuilder.UseServiceProviderFactory(new DIExtensionsServiceProviderFactory()); // Blazor requires service scopes, which are supported only with Microsoft.Extensions.DependencyInjection
				appBuilder
					.ConfigureServices(services =>
					{
						services.AddBlazorWebView();
					});
			}
		}

		// To use the Microsoft.Extensions.DependencyInjection ServiceCollection and not the MAUI one
		class DIExtensionsServiceProviderFactory : IServiceProviderFactory<ServiceCollection>
		{
			public ServiceCollection CreateBuilder(IServiceCollection services)
				=> new ServiceCollection { services };

			public IServiceProvider CreateServiceProvider(ServiceCollection containerBuilder)
				=> containerBuilder.BuildServiceProvider();
		}
	}
}