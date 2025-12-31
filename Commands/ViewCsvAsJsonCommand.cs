using System.CommandLine;

using xmle.Models;
using xmle.Services;

public class ViewCsvAsJsonCommand : Command
{
    private readonly TextWriter writer;
    private readonly ICsvService csvService;

    private Argument<string> csvPathArgument = new("csv-path") { };

    public ViewCsvAsJsonCommand(TextWriter writer, xmle.Services.ICsvService csvService) : base("view-csv", "View Csv as Json")
    {
        this.writer = writer;
        this.csvService = csvService;

        Add(csvPathArgument);

        SetAction(ActionHandler);
    }

    private void ActionHandler(ParseResult result)
    {
        var parsedValues = GetParsedValues(result);

        writer.WriteLine(csvService.ViewCsvAsJson(parsedValues));
    }

    private ViewCsvAsJsonRequest GetParsedValues(ParseResult result)
    {
        string csvPath = result.GetRequiredValue(csvPathArgument);
        return new ViewCsvAsJsonRequest(csvPath);
    }
}