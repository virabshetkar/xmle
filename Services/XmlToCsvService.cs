using xmle.Models;
using xmle.Utils;

namespace xmle.Services;

public interface IXmlToCsvService
{
    void CovertXmlToCsv(XmlToCsvRequest input);
}

public class XmlToCsvService : IXmlToCsvService
{
    private readonly IXmlService xmlService;

    public XmlToCsvService(IXmlService xmlService)
    {
        this.xmlService = xmlService;
    }


    public void CovertXmlToCsv(XmlToCsvRequest input)
    {
        var (xmlFilePath, csvFilePath, rootPath, rowPath, columnPaths, titles) = input;
        var xmlPath = FileUtil.GetFullPath(xmlFilePath);
        var csvPath = FileUtil.GetFullPath2(csvFilePath);

        var data = xmlService.GetDataFromXml(new ViewXmlAsJsonRequest(xmlFilePath, rootPath, rowPath, columnPaths, titles));

        if (titles is null) throw new Exception("titles aren't given");

        using var writer = new StreamWriter(csvPath);

        writer.WriteLine(string.Join(',', titles));

        foreach (var d in data)
        {
            writer.WriteLine(string.Join(',', d.Values));
        }
    }
}