using System.Net;
using Amazon;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using Amazon.S3.Util;
using Microsoft.Extensions.Options;
using Saritasa.NetForge.Blazor.Infrastructure.Abstractions.Interfaces;
using Saritasa.Tools.Domain.Exceptions;
using NotFoundException = Saritasa.NetForge.Blazor.Domain.Exceptions.NotFoundException;

namespace Saritasa.NetForge.Demo.Infrastructure.UploadFiles.S3Storage;

/// <summary>
/// Contains methods for interacting with S3 storage.
/// </summary>
public class S3StorageService : IBlobStorageService, ICloudBlobStorageService, IDisposable
{
    private bool disposedValue;
    private readonly AmazonS3Client s3Client;
    private readonly S3Settings settings;
    private readonly ILogger<S3StorageService> logger;
    private readonly string bucketName;

    /// <summary>
    /// Constructor.
    /// </summary>
    public S3StorageService(IOptions<S3Settings> options, ILogger<S3StorageService> logger)
    {
        this.logger = logger;
        settings = options.Value;

        s3Client = ConfigureAmazonS3Client();
        bucketName = settings.BucketName;
    }

    /// <inheritdoc />
    public async Task UploadAsync(string key, Stream stream, CancellationToken cancellationToken)
    {
        try
        {
            await CreateBucketIfNotExistsAsync(cancellationToken);
            using var transferUtility = new TransferUtility(s3Client);

            var uploadRequest = new TransferUtilityUploadRequest()
            {
                CannedACL = S3CannedACL.PublicRead,
                InputStream = stream,
                BucketName = bucketName,
                Key = key,
            };

            await transferUtility.UploadAsync(uploadRequest, cancellationToken);
            logger.LogInformation("File: '{key}' is uploaded successfully.", key);
        }
        catch (Exception ex)
        {
            HandleException(ex);
        }
    }

    /// <inheritdoc />
    public async Task<Stream> GetAsync(string key, CancellationToken cancellationToken)
    {
        await ValidateExistsObjectAsync(key, cancellationToken);
        try
        {
            var response = await s3Client.GetObjectAsync(bucketName, key, cancellationToken);
            logger.LogInformation("File: '{key}' is received successfully.", key);
            return response.ResponseStream;
        }
        catch (Exception ex)
        {
            HandleException(ex);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<IEnumerable<string>> GetListKeysAsync(string pattern, CancellationToken cancellationToken)
    {
        try
        {
            await CreateBucketIfNotExistsAsync(cancellationToken);

            var listObjectsRequest = new ListObjectsV2Request
            {
                BucketName = bucketName,
                Prefix = pattern,
            };
            var listObjectsResponse = await s3Client.ListObjectsV2Async(listObjectsRequest, cancellationToken);

            logger.LogInformation("Successfully got list of objects.");

            return listObjectsResponse.S3Objects
                .Select(s => s.Key)
                .ToList();
        }
        catch (Exception ex)
        {
            HandleException(ex);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task RemoveAsync(string key, CancellationToken cancellationToken)
    {
        await ValidateExistsObjectAsync(key, cancellationToken);

        try
        {
            await s3Client.DeleteObjectAsync(bucketName, key, cancellationToken);
            logger.LogInformation("File: '{key}' is removed successfully.", key);
        }
        catch (Exception ex)
        {
            HandleException(ex);
        }
    }

    /// <inheritdoc />
    public string GetPreSignedUrl(string key, DateTimeOffset expires)
    {
        var preSignedUrlRequest = new GetPreSignedUrlRequest()
        {
            BucketName = bucketName,
            Key = key,
            Expires = expires.UtcDateTime,
            Protocol = GetServiceUrlProtocol(),
        };
        var preSignedUrl = s3Client.GetPreSignedURL(preSignedUrlRequest);

        logger.LogInformation("Pre-signed URL for the file '{key}' was received successfully.", key);
        return preSignedUrl;
    }

    private AmazonS3Client ConfigureAmazonS3Client()
    {
        var config = new AmazonS3Config
        {
            RegionEndpoint = RegionEndpoint.GetBySystemName(settings.RegionName),
        };

        if (!string.IsNullOrEmpty(settings.ServiceUrl))
        {
            config.ServiceURL = settings.ServiceUrl;
            config.ForcePathStyle = settings.ForcePathStyle;
        }

        if (!string.IsNullOrEmpty(settings.AccessKey))
        {
            var credentials = new BasicAWSCredentials(settings.AccessKey, settings.SecretKey);
            logger.LogInformation("S3 client has been configured with access key.");
            return new AmazonS3Client(credentials, config);
        }

        logger.LogInformation("S3 client has been configured with local credentials.");
        return new AmazonS3Client(config);
    }

    private async Task ValidateExistsObjectAsync(string key, CancellationToken cancellationToken)
    {
        try
        {
            await s3Client.GetObjectMetadataAsync(bucketName, key, cancellationToken);
        }
        catch (AmazonS3Exception ex)
        {
            string message;
            if (ex.StatusCode == HttpStatusCode.NotFound)
            {
                message = $"File: '{key}' is not exist.";
                logger.LogError("File: '{key}' is not exist.", key);
                throw new NotFoundException(message, ex);
            }

            message = $"Cannot get file metadata with key {key}.";
            logger.LogError(ex, "Cannot get file metadata with key {key}.", key);
            throw new InfrastructureException(message, ex);
        }
        catch (Exception ex)
        {
            HandleException(ex);
        }
    }

    private async Task CreateBucketIfNotExistsAsync(CancellationToken cancellationToken)
    {
        try
        {
            if (!await AmazonS3Util.DoesS3BucketExistV2Async(s3Client, bucketName))
            {
                var putBucketRequest = new PutBucketRequest
                {
                    BucketName = bucketName,
                    UseClientRegion = true,
                };

                await s3Client.PutBucketAsync(putBucketRequest, cancellationToken);
            }
        }
        catch (Exception ex)
        {
            HandleException(ex);
        }
    }

    private void HandleException(Exception exception)
    {
        if (exception is HttpRequestException)
        {
            logger.LogWarning(exception, "S3 is not available.");
        }

        throw new InfrastructureException(exception);
    }

    private Protocol GetServiceUrlProtocol()
    {
        if (Uri.TryCreate(settings.ServiceUrl, UriKind.Absolute, out var uri))
        {
            return uri.Scheme == "http" ? Protocol.HTTP : Protocol.HTTPS;
        }

        return Protocol.HTTPS;
    }

    /// <inheritdoc />
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Dispose pattern implementation.
    /// </summary>
    /// <param name="disposing">Dispose managed resources.</param>
    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                s3Client.Dispose();
            }

            disposedValue = true;
        }
    }
}
