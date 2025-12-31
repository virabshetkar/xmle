namespace xmle.Models;

public record UpdateXmlRequest(string xmlFilePath, string xPath, string value);