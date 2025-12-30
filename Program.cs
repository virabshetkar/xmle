using System.CommandLine;

using Microsoft.Extensions.DependencyInjection;

using xmle.Commands;
using xmle.Services;


namespace xmle;

public class Program
{
    public static async Task<int> Main(string[] args)
    {
        var services = new ServiceCollection();
        ConfigureServices(services);
        var provider = services.BuildServiceProvider();

        var rootCommand = RootCommandBuilder.Build(provider);

        foreach (var commandType in RootCommandBuilder.GetAllCommands())
        {
            rootCommand.Add((Command)provider.GetRequiredService(commandType));
        }

        try
        {
            return await rootCommand.Parse(args).InvokeAsync(new() { EnableDefaultExceptionHandler = false });
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine(ex.Message);
            return ex.HResult;
        }
    }

    public static void ConfigureServices(IServiceCollection services)
    {
        services.AddSingleton<ICsvParser, CsvParser>();
        services.AddSingleton<IXmlService, XmlService>();
        services.AddSingleton<IConfigService, ConfigService>();

        services.AddSingleton<TextWriter>(Console.Out);

        foreach (var commandType in RootCommandBuilder.GetAllCommands())
        {
            services.AddTransient(commandType);
        }
    }
}