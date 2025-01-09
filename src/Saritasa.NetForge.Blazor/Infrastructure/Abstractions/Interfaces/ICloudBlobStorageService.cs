namespace Saritasa.NetForge.Blazor.Infrastructure.Abstractions.Interfaces;

/// <summary>
/// Contains methods for interacting
/// with BLOB cloud storage directly with the client.
/// </summary>
public interface ICloudBlobStorageService
{
    /// <summary>
    /// Created signed access URL to a BLOB.
    /// </summary>
    /// <param name="key">BLOB key.</param>
    /// <param name="expires">BLOB access URL expiration date.</param>
    /// <returns>Signed access URL to a BLOB.</returns>
    string GetPreSignedUrl(string key, DateTimeOffset expires);
}
