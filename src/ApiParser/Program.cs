using System;
using System.Threading.Tasks;
using ApiParser.Data;
using ApiParser.Inputs;
using ApiParser.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;

namespace ApiParser
{
    class Program
    {
        public static IConfiguration Configuration;

        static async Task<int> Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .Filter.With<HttpClientLoggingFilter>()
                .WriteTo.Console(Serilog.Events.LogEventLevel.Information)
                .MinimumLevel.Information()
                .Enrich.FromLogContext()
                .CreateLogger();

            if (args.Length < 1)
            {
                Log.Information("No input file specified");
                return 1;
            }


            try
            {
                // Create service collection
                Log.Information("Loading up application");
                ServiceCollection serviceCollection = new ServiceCollection();
                ConfigureServices(serviceCollection);
                IServiceProvider serviceProvider = serviceCollection.BuildServiceProvider();

                await serviceProvider.GetService<App>().Run(args[0]);

                return 0;
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, ex.Message);
                return 1;
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        private static void ConfigureServices(IServiceCollection services)
        {
            // Add logging
            services.AddSingleton(LoggerFactory.Create(builder =>
            {
                builder.AddSerilog(dispose: true);
            }));

            services.AddLogging();
            
            // Build configuration
            Configuration = new ConfigurationBuilder()
                .SetBasePath(Environment.CurrentDirectory)
                .AddJsonFile("appsettings.json", false)
                .Build();

            services.AddSingleton(Configuration);

            services.AddHttpClient();

            // Add app

            services.AddSingleton<IInputParser, CsvParser>();
            services.AddSingleton<IDataGetter, HttpDataGetter>();

            services.AddSingleton<AppSettings>();
            services.AddTransient<App>();
        }
    }
}
