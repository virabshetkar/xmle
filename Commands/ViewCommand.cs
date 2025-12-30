using System.CommandLine;
using System.CommandLine.Parsing;

using xmle.Services;
using xmle.Utils;

namespace xmle.Commands;

public class ViewCommand : Command
{
    private readonly IXmlService xmlService;
    private readonly TextWriter writer;
    private readonly Option<string> xpathOption;
    private readonly Argument<string> xmlPathArgument;

    public ViewCommand(IXmlService xmlService, TextWriter writer) : base("view", "View data in JSON format")
    {
        xpathOption = new Option<string>("xpath", "-x")
        {
            Description = "XPath to find the first XML Element",
            DefaultValueFactory = (ArgumentResult res) => { return "/"; }
        };

        xmlPathArgument = new Argument<string>("xmlPath")
        {
            Description = "Path to XML file",
        };

        Add(xmlPathArgument);
        Add(xpathOption);

        SetAction(ActionHandler);
        this.xmlService = xmlService;
        this.writer = writer;
    }

    private async Task ActionHandler(ParseResult result)
    {
        var xmlPath = result.GetValue(xmlPathArgument);
        xmlPath = FileUtil.GetFullPath(xmlPath);

        var xpath = result.GetValue(xpathOption);
        if (xpath is null)
        {
            writer.WriteLine("XPath is not given");
            return;
        }

        var xml = xmlService.GetRootXml(xmlPath);

        var data = xmlService.GetValueFromXpath(xml, xpath);
        writer.WriteLine(data);
    }
}