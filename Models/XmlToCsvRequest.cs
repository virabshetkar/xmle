namespace xmle.Models;

public record XmlToCsvRequest(string xmlFilePath, string csvFilePath, string rootPath, string rowPath, string[] columnPaths, string[]? titles);