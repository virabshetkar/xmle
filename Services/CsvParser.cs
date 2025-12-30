using System.Globalization;
using System.Xml;
using CsvHelper;
using CsvHelper.Configuration;

namespace xmle.Services;

public interface ICsvParser
{
    void ToCsv(string xmlPath, string toPath);
    public IList<Dictionary<string, string>> ReadCsv(string csvPath);
}

public class CsvParser : ICsvParser
{
    private readonly TextWriter writer;

    public CsvParser(TextWriter writer)
    {
        this.writer = writer;
    }

    public void ToCsv(string xmlPath, string toPath)
    {
        var xmlDoc = new XmlDocument();
        xmlDoc.Load(xmlPath);

        var dataXmlList = xmlDoc.SelectNodes("//data");
        if (dataXmlList is null) throw new Exception("No data");

        string csvOutput = "id,en-US,de-AT\n";

        foreach (XmlNode dataXml in dataXmlList)
        {
            if (dataXml is null) continue;

            var value = dataXml.SelectSingleNode("value")?.InnerText;
            string? name = dataXml.Attributes?["name"]?.Value;
            var comment = dataXml.SelectSingleNode("comment")?.InnerText;

            csvOutput += $"{name},{comment},{value}\n";
        }

        File.WriteAllText(toPath, csvOutput);
    }

    public IList<Dictionary<string, string>> ReadCsv(string csvPath)
    {
        var output = new List<Dictionary<string, string>>();
        var config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = true,
        };

        using (var reader = new StreamReader(csvPath))
        using (var csv = new CsvReader(reader, config))
        {
            // Read first line and assume the header.
            csv.Read();
            csv.ReadHeader();

            if (csv.HeaderRecord is null) throw new Exception("No csv header found");

            while (csv.Read())
            {
                var dict = new Dictionary<string, string>();
                foreach (var header in csv.HeaderRecord)
                {
                    dict[header] = csv.GetField(header) ?? "";
                }
                output.Add(dict);
            }
        }

        return output;
    }
}