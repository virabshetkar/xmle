using System.CommandLine;

using xmle.Models;
using xmle.Services;

public class ViewCsvAsJsonCommand : Command
{
    private readonly IConfigService config;
    private readonly TextWriter writer;
    private readonly ICsvService csvService;

    public Argument<string> CsvPathArgument { get; set; } = new("csv-path") { };

    public ViewCsvAsJsonCommand(IConfigService config, TextWriter writer, xmle.Services.ICsvService csvService) : base("view-csv", "View Csv as Json")
    {
        this.config = config;
        this.writer = writer;
        this.csvService = csvService;
        Add(CsvPathArgument);

        SetAction(ActionHandler);
    }

    private void ActionHandler(ParseResult result)
    {
        var parsedValues = GetParsedValues(result);

        writer.WriteLine(csvService.ViewCsvAsJson(parsedValues));
    }

    private ViewCsvAsJsonRequest GetParsedValues(ParseResult result)
    {
        string csvPath = result.GetRequiredValue(CsvPathArgument);
        return new ViewCsvAsJsonRequest(csvPath);
    }
}