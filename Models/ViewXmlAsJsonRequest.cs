namespace xmle.Models;

public record ViewXmlAsJsonRequest
(
    string xmlFilePath,
    string rootPath,
    string rowPath,
    string[] columnPaths,
    string[]? titles
);