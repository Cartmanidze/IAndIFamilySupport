using System.Reflection;
using IAndIFamilySupport.API.Interfaces;

namespace IAndIFamilySupport.API.Services;

internal class EmbeddedResourceService : IResourceService
{
    private readonly Assembly _assembly;
    private readonly ILogger<EmbeddedResourceService> _logger;

    public EmbeddedResourceService(ILogger<EmbeddedResourceService> logger)
    {
        _logger = logger;
        _assembly = Assembly.GetExecutingAssembly();
    }

    public byte[]? GetResourceBytes(string resourceName)
    {
        try
        {
            var fullResourceName = GetFullResourceName(resourceName);
            if (fullResourceName == null)
            {
                _logger.LogWarning("Resource {ResourceName} not found", resourceName);
                return null;
            }

            using var stream = _assembly.GetManifestResourceStream(fullResourceName);
            if (stream == null)
            {
                _logger.LogWarning("Failed to get stream for resource {FullResourceName}", fullResourceName);
                return null;
            }

            using var memoryStream = new MemoryStream();
            stream.CopyTo(memoryStream);
            return memoryStream.ToArray();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting resource bytes for {ResourceName}", resourceName);
            return null;
        }
    }

    public Stream? GetResourceStream(string resourceName)
    {
        try
        {
            var fullResourceName = GetFullResourceName(resourceName);
            if (fullResourceName == null)
            {
                _logger.LogWarning("Resource {ResourceName} not found", resourceName);
                return null;
            }

            return _assembly.GetManifestResourceStream(fullResourceName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting resource stream for {ResourceName}", resourceName);
            return null;
        }
    }

    public IEnumerable<string> GetAvailableResources()
    {
        return _assembly.GetManifestResourceNames();
    }

    private string? GetFullResourceName(string resourceName)
    {
        var allResources = _assembly.GetManifestResourceNames();
        var matchingResources = allResources
            .Where(r => r.EndsWith(resourceName, StringComparison.OrdinalIgnoreCase))
            .ToList();

        if (matchingResources.Count == 0)
        {
            _logger.LogWarning("No matching resources found for {ResourceName}", resourceName);
            return null;
        }

        if (matchingResources.Count > 1)
            _logger.LogWarning("Multiple matching resources found for {ResourceName}: {MatchingResources}",
                resourceName, string.Join(", ", matchingResources));

        return matchingResources.First();
    }
}