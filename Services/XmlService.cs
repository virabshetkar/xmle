using System.Xml;

using Newtonsoft.Json;

namespace xmle.Services;

public interface IXmlService
{
    XmlDocument GetRootXml(string filepath);
    string GetValueFromXpath(XmlDocument xml, string xpath);
    void UpdateValueForXpath(XmlDocument xml, string xpath, string value);
}

public class XmlService : IXmlService
{
    public XmlDocument GetRootXml(string filepath)
    {
        var xrs = new XmlReaderSettings()
        {
            IgnoreWhitespace = true,
        };

        var reader = XmlReader.Create(filepath, xrs);

        var doc = new XmlDocument();

        doc.Load(reader);
        return doc;
    }

    public string GetValueFromXpath(XmlDocument xml, string xpath)
    {
        var el = xml.SelectNodes(xpath);
        if (el is null) throw new Exception("No element found!");

        return JsonConvert.SerializeObject(el, Newtonsoft.Json.Formatting.Indented);
    }

    public void UpdateValueForXpath(XmlDocument xml, string xpath, string value)
    {
        var el = xml.SelectSingleNode(xpath);
        if (el is null) throw new Exception("No element found!");

        if (el is XmlAttribute)
        {
            el.Value = value;
        }
        else if (el is XmlElement)
        {
            el.InnerXml = value;
        }
    }
}