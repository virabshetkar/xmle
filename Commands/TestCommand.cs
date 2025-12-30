using System.CommandLine;
using Newtonsoft.Json;
using xmle.Services;
using xmle.Utils;

namespace xmle.Commands;

public class TestCommand : Command
{
    private readonly IConfigService config;
    private readonly ICsvParser csvParser;
    private readonly TextWriter writer;

    public Option<string> CsvPath { get; set; } = new("csvPath", "-f") { Required = true };

    public TestCommand(IConfigService config, ICsvParser csvParser, TextWriter writer) : base("test", "Temporary Command")
    {
        this.config = config;
        this.csvParser = csvParser;
        this.writer = writer;

        Add(CsvPath);

        SetAction(ActionHandler);
    }

    private void ActionHandler(ParseResult result)
    {
        var csvPath = result.GetValue(CsvPath);
        var fullPath = FileUtil.GetFullPath(csvPath);

        var data = csvParser.ReadCsv(fullPath);
        writer.WriteLine(JsonConvert.SerializeObject(data, Formatting.Indented));
    }
}