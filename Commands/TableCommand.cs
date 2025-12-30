using System.CommandLine;
using System.Xml;

using xmle.Services;

namespace xmle.Commands;


public class TableCommand : Command
{
    private readonly TextWriter writer;

    private readonly Option<string[]> columnNamesOption;
    private readonly Option<string[]> headingsOption;
    private readonly Option<string> rootXPathOption;
    private readonly Argument<string> xmlPathArgument = new("xmlPath")
    {
        Description = "Path to XML file",
    };

    public TableCommand(IConfigService config, TextWriter writer) : base("table", "Creates a table")
    {
        this.writer = writer;

        columnNamesOption = new Option<string[]>("colNames", "-c", "--cols")
        {
            DefaultValueFactory = res => { return config.GetConfig()?.Columns ?? throw new ArgumentNullException(nameof(columnNamesOption)); },
        };

        rootXPathOption = new Option<string>("rootXpath", "-r", "--root")
        {
            DefaultValueFactory = res => { return config.GetConfig()?.Table ?? throw new ArgumentNullException(nameof(rootXPathOption)); },
        };

        headingsOption = new Option<string[]>("headings", "-h", "--headings")
        {
            DefaultValueFactory = res => { return config.GetConfig()?.Headings ?? throw new ArgumentNullException(nameof(headingsOption)); }
        };

        Add(columnNamesOption);
        Add(rootXPathOption);
        Add(headingsOption);
        Add(xmlPathArgument);

        SetAction(ActionHandler);
    }

    private async Task ActionHandler(ParseResult result)
    {
        var columnNames = result.GetValue(columnNamesOption);
        var headings = result.GetValue(headingsOption);
        var rootXPath = result.GetValue(rootXPathOption);

        if (columnNames is null)
        {
            writer.WriteLine("Columns are not given!");
            return;
        }

        if (rootXPath is null)
        {
            writer.WriteLine("Root Xpath is not given!");
            return;
        }

        var xmlPath = result.GetValue<string>("xmlPath");

        if (xmlPath == null)
        {
            return;
        }

        xmlPath = Path.GetFullPath(xmlPath);
        if (!Path.Exists(xmlPath))
        {
            writer.WriteLine("XML path does not exist");
            return;
        }

        var xml = new XmlDocument();
        xml.Load(xmlPath);

        var nodes = xml.SelectNodes(rootXPath);

        if (nodes is null)
        {
            return;
        }

        if (headings is not null)
        {
            if (headings.Length != columnNames.Length) { throw new Exception("Headings and Columns don't match!"); }
            writer.WriteLine(string.Join(",", headings));
        }

        foreach (object? node in nodes)
        {
            if (node is XmlElement el)
            {

                for (int i = 0; i < columnNames.Length - 1; i++)
                {
                    writer.Write($"{GetValue(el.SelectSingleNode(columnNames[i]))},");
                }

                writer.WriteLine(GetValue(el.SelectSingleNode(columnNames[^1])));
            }
        }
    }

    private static string GetValue(XmlNode? element)
    {
        if (element is null)
        {
            return "";
        }
        else if (element is XmlAttribute)
        {
            return element.Value ?? "";
        }
        else if (element is XmlElement)
        {
            if (element.HasChildNodes && (element.FirstChild is XmlCDataSection || element.FirstChild is XmlText))
                return element.FirstChild.InnerText;

            return element.InnerXml;
        }

        return "";
    }
}