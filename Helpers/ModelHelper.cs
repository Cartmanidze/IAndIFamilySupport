namespace IAndIFamilySupport.API.Helpers;

public static class ModelHelper
{
    public static string GetUserFriendlyModelName(string modelCode)
    {
        return modelCode switch
        {
            "R8PLUS" => "R8 PLUS (8,32,64)",
            "R3" => "R3 (8,32,64)",
            "R8" => "R8 (8,32,64)",
            _ => modelCode
        };
    }

    public static string NormalizeModelCode(string modelCode)
    {
        return modelCode.Replace("_", "").ToUpperInvariant();
    }

    public static string? GetPdfPath(string modelCode)
    {
        return NormalizeModelCode(modelCode) switch
        {
            "R8PLUS" => "Инструкции pdf/PLUS +NEW.pdf",
            "R3" => "Инструкции pdf/R3 кулон NEW.pdf",
            "R8" => "Инструкции pdf/R8 (8,32,64гб) NEW.pdf",
            _ => null
        };
    }
}