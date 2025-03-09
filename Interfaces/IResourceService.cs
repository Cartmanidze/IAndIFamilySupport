namespace IAndIFamilySupport.API.Interfaces;

public interface IResourceService
{
    byte[]? GetResourceBytes(string resourceName);
    Stream? GetResourceStream(string resourceName);
    IEnumerable<string> GetAvailableResources();
}