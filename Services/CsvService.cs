using System.Globalization;

using CsvHelper;
using CsvHelper.Configuration;

using Newtonsoft.Json;

using xmle.Models;
using xmle.Utils;

namespace xmle.Services;

public interface ICsvService
{
    string ViewCsvAsJson(ViewCsvAsJsonRequest input);
}

public class CsvService : ICsvService
{
    public CsvService()
    {

    }

    public string ViewCsvAsJson(ViewCsvAsJsonRequest input)
    {
        var csvFilePath = input.csvFilePath;
        var csvPath = FileUtil.GetFullPath(csvFilePath);

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

        return JsonConvert.SerializeObject(output, Formatting.Indented);
    }
}