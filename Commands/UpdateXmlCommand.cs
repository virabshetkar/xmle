using System.CommandLine;

using xmle.Models;
using xmle.Services;

public class UpdateXmlCommand : Command
{
    private readonly IConfigService config;
    private readonly TextWriter writer;
    private readonly IXmlService xmlService;

    private Argument<string> xmlPathArgument = new("xml-path") { };

    private Option<string> xPathOption = new("xpath", "-x", "--xpath") { Required = true };
    private Option<string> valueOption = new("value", "-v", "--value") { Required = true };

    public UpdateXmlCommand(IConfigService config, TextWriter writer, IXmlService xmlService) : base("update-xml", "Update the value at a given XPath")
    {
        this.config = config;
        this.writer = writer;
        this.xmlService = xmlService;

        Add(xmlPathArgument);
        Add(xPathOption);
        Add(valueOption);

        SetAction(ActionHandler);
    }

    private void ActionHandler(ParseResult result)
    {
        var parsedValues = GetParsedValues(result);

        writer.WriteLine(xmlService.UpdateXml(parsedValues));
    }

    private UpdateXmlRequest GetParsedValues(ParseResult result)
    {
        string xmlFilePath = result.GetRequiredValue(xmlPathArgument);
        string xPath = result.GetRequiredValue(xPathOption);
        string value = result.GetRequiredValue(valueOption);

        return new UpdateXmlRequest(xmlFilePath, xPath, value);
    }
}