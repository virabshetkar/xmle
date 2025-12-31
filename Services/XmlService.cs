using System.Xml;

using Newtonsoft.Json;

using xmle.Models;
using xmle.Utils;

namespace xmle.Services;

public interface IXmlService
{
    public IList<IDictionary<string, string>> GetDataFromXml(ViewXmlAsJsonRequest input);
    public string ViewXmlAsJson(ViewXmlAsJsonRequest input);
    public bool UpdateXml(UpdateXmlRequest input);
}

public class XmlService : IXmlService
{
    public XmlService()
    {

    }

    public bool UpdateXml(UpdateXmlRequest input)
    {
        var (xmlFilePath, xPath, value) = input;
        var xml = GetXmlDocument(xmlFilePath);

        var nodes = xml.SelectNodes(xPath);

        if (nodes is null || nodes.Count == 0) throw new Exception($"Could not find element at path: \"{xPath}\"");

        if (nodes.Count > 1) throw new Exception($"Found multiple values at path: \"{xPath}\"");

        var node = nodes[0];

        if (node is XmlElement)
        {
            ValidateXmlElement(node);
            ((XmlElement)node).InnerText = value;
        }
        else if (node is XmlAttribute)
        {
            ((XmlAttribute)node).Value = value;
        }
        else
        {
            throw new Exception("Could not update the XML Node");
        }

        xml.Save(FileUtil.GetFullPath(xmlFilePath));
        return true;
    }

    private static void ValidateXmlElement(XmlNode? node)
    {
        if (node is null) throw new Exception("Could not find Node");
        if (node.HasChildNodes)
        {
            if (node.ChildNodes.Count > 1 || !(node.FirstChild is XmlText || node.FirstChild is XmlCDataSection))
                throw new Exception("You are rewritting a parent node. We do not allow this currently");
        }
    }

    public string ViewXmlAsJson(ViewXmlAsJsonRequest input)
    {
        var res = GetDataFromXml(input);

        return JsonConvert.SerializeObject(res, Newtonsoft.Json.Formatting.Indented);
    }

    public IList<IDictionary<string, string>> GetDataFromXml(ViewXmlAsJsonRequest input)
    {
        var (xmlFilePath, rootPath, rowPath, columnPaths, titles) = input;

        var xml = GetXmlDocument(xmlFilePath);
        var rootNode = xml.SelectNodes(rootPath);
        if (rootNode is null || rootNode.Count == 0) throw new Exception($"Could not find root node with XPath: \"{rootPath}\"");
        if (rootNode.Count != 1) throw new Exception("Multiple Root Nodes found");

        if (titles is not null && titles.Length > 0 && titles.Length != columnPaths.Length) throw new Exception("Titles and columns don't match");

        XmlNode rn = rootNode[0]!;

        var rows = rn.SelectNodes(rowPath);
        if (rows is null) return new List<IDictionary<string, string>>();

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
        return res;
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