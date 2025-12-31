
namespace xmle.Utils;

public static class FileUtil
{
    public static string GetFullPath(string? path)
    {
        if (string.IsNullOrEmpty(path)) throw new ArgumentNullException("Path is not given");

        var fullPath = Path.GetFullPath(path);
        if (!Path.Exists(fullPath)) throw new FileNotFoundException($"File: \"{path}\" not found!");

        return fullPath;
    }

    public static string GetFullPath2(string? path)
    {
        if (string.IsNullOrEmpty(path)) throw new ArgumentNullException("Path is not given");

        var fullPath = Path.GetFullPath(path);

        return fullPath;
    }
}