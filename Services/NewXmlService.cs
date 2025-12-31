using System.Xml;
using Newtonsoft.Json;
using xmle.Models;
using xmle.Utils;

namespace xmle.Services;

public interface INewXmlService
{
    string ViewXmlAsJson(ViewXmlAsJsonDto input);
}

public class NewXmlService : INewXmlService
{
    public NewXmlService()
    {

    }

    public string ViewXmlAsJson(ViewXmlAsJsonDto input)
    {
        (var xmlFilePath, var rootPath, var rowPath, var columnPaths, var titles) = input;

        var xml = GetXmlDocument(xmlFilePath);
        var rootNode = xml.SelectNodes(rootPath);
        if (rootNode is null) throw new ArgumentNullException(nameof(rootNode));
        if (rootNode.Count != 1) throw new Exception("Multiple Root Nodes found");

        if (titles is not null && titles.Length > 0 && titles.Length != columnPaths.Length) throw new Exception("Titles and columns don't match");

        XmlNode rn = rootNode[0]!;

        var rows = rn.SelectNodes(rowPath);
        if (rows is null) return "[]";

        IList<IDictionary<string, string>> res = new List<IDictionary<string, string>>();

        foreach (var row in rows)
        {
            if (row is XmlElement)
            {
                var dict = new Dictionary<string, string>();

                for (int i = 0; i < columnPaths.Length; i++)
                {
                    var colPath = columnPaths[i];
                    var field = ((XmlElement)row).SelectSingleNode(colPath);
                    if (field is not null && field.HasChildNodes && (field.FirstChild is XmlCDataSection || field.FirstChild is XmlText))
                    {
                        dict[titles is null || titles.Length == 0 ? colPath : titles[i]] = field.FirstChild.InnerText;
                        continue;
                    }
                    dict[titles is null || titles.Length == 0 ? colPath : titles[i]] = field?.InnerText ?? "";
                }

                res.Add(dict);
            }
            else throw new ArgumentException("Row is not an XmlElement");
        }

        return JsonConvert.SerializeObject(res, Newtonsoft.Json.Formatting.Indented);
    }

    private XmlDocument GetXmlDocument(string xmlFilePath)
    {
        var filePath = FileUtil.GetFullPath(xmlFilePath);

        var xrs = new XmlReaderSettings()
        {
            IgnoreWhitespace = true,
        };

        var reader = XmlReader.Create(filePath, xrs);

        var doc = new XmlDocument();

        doc.Load(reader);
        return doc;
    }
}