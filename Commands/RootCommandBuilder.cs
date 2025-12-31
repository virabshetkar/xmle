using System.CommandLine;
using System.CommandLine.Help;

namespace xmle.Commands;

public class RootCommandBuilder
{
    public static RootCommand Build(IServiceProvider provider)
    {
        var rootCommand = new RootCommand("A tool to edit XML files")
        {
            Action = new HelpAction()
        };

        return rootCommand;
    }

    public static IEnumerable<Type> GetAllCommands()
    {
        var commandTypes = typeof(Program).Assembly.ExportedTypes.Where(t => !t.IsAbstract).Where(t => typeof(Command).IsAssignableFrom(t));
        return commandTypes;
    }
}