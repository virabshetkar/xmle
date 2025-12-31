namespace xmle.Models;

public record ViewXmlAsJsonDto
(
    string xmlFilePath,
    string rootPath,
    string rowPath,
    string[] columnPaths,
    string[]? titles
);