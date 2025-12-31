using Newtonsoft.Json;

namespace xmle.Services;

public class XmleConfiguration
{
    public string? RootPath { get; set; }
    public string? RowPath { get; set; }
    public string[]? ColumnPaths { get; set; }
    public string[]? Titles { get; set; }
}

public interface IConfigService
{
    void Initialize();
    public XmleConfiguration? GetConfig();
}

public class ConfigService : IConfigService
{
    private XmleConfiguration? config;

    public ConfigService()
    {
        config = null;
        Initialize();
    }

    public void Initialize()
    {
        var path = FindConfigFilePath(".xmle.json");
        if (path is null) return;
        config = GetConfiguration(path);
    }

    public XmleConfiguration? GetConfig()
    {
        return config;
    }

    private static string? FindConfigFilePath(string fileName)
    {
        if (string.IsNullOrEmpty(fileName)) throw new ArgumentException("File name is empty! Can't find config file");

        string cwd = Directory.GetCurrentDirectory();

        while (true)
        {
            string candidate = Path.Combine(cwd, fileName);

            if (File.Exists(candidate))
            {
                return candidate;
            }

            DirectoryInfo? parent = Directory.GetParent(cwd);

            if (parent is null) return null;

            cwd = parent.FullName;
        }
    }

    private static XmleConfiguration GetConfiguration(string filePath)
    {
        XmleConfiguration? config = JsonConvert.DeserializeObject<XmleConfiguration>(File.ReadAllText(filePath));
        return config is null ? throw new ArgumentNullException(nameof(config), "No Config Found!") : config;
    }
}