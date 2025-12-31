using System.CommandLine;

using xmle.Models;
using xmle.Services;

public class UpdateXmlCommand : Command
{
    private readonly IConfigService config;
    private readonly TextWriter writer;
    private readonly IXmlService xmlService;

    public Argument<string> XmlPathArgument { get; set; } = new("xml-path") { };

    public Option<string> XPathOption { get; set; } = new("xpath", "-x", "--xpath") { Required = true };
    public Option<string> ValueOption { get; set; } = new("value", "-v", "--value") { Required = true };

    public UpdateXmlCommand(IConfigService config, TextWriter writer, IXmlService xmlService) : base("update-xml", "Update the value at a given XPath")
    {
        this.config = config;
        this.writer = writer;
        this.xmlService = xmlService;

        Add(XmlPathArgument);
        Add(XPathOption);
        Add(ValueOption);

        SetAction(ActionHandler);
    }

    private void ActionHandler(ParseResult result)
    {
        var parsedValues = GetParsedValues(result);

        writer.WriteLine(xmlService.UpdateXml(parsedValues));
    }

    private UpdateXmlRequest GetParsedValues(ParseResult result)
    {
        string xmlFilePath = result.GetRequiredValue(XmlPathArgument);
        string xPath = result.GetRequiredValue(XPathOption);
        string value = result.GetRequiredValue(ValueOption);

        return new UpdateXmlRequest(xmlFilePath, xPath, value);
    }
}