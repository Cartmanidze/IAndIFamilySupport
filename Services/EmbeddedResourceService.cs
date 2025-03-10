using System.Reflection;
using IAndIFamilySupport.API.Interfaces;

namespace IAndIFamilySupport.API.Services;

internal class EmbeddedResourceService : IResourceService
{
    private readonly Assembly _assembly;
    private readonly ILogger<EmbeddedResourceService> _logger;

    private readonly string[] _resources;

    public EmbeddedResourceService(ILogger<EmbeddedResourceService> logger)
    {
        _logger = logger;
        _assembly = Assembly.GetExecutingAssembly();

        _resources = _assembly.GetManifestResourceNames();
        _logger.LogInformation("Found {Count} embedded resources", _resources.Length);
        foreach (var resource in _resources) _logger.LogDebug("Embedded resource: {Resource}", resource);
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
        return _resources;
    }

    private string? GetFullResourceName(string resourcePath)
    {
        _logger.LogDebug("All available resources: {Resources}",
            string.Join(", ", _resources));

        var matchingResources = _resources
            .Where(r => r.Contains(resourcePath, StringComparison.OrdinalIgnoreCase))
            .ToList();

        if (matchingResources.Count == 0)
        {
            _logger.LogWarning("No matching resources found for path {ResourcePath}", resourcePath);
            return null;
        }

        if (matchingResources.Count > 1)
            _logger.LogWarning("Multiple matching resources found for {ResourcePath}: {MatchingResources}",
                resourcePath, string.Join(", ", matchingResources));

        return matchingResources.First();
    }
}