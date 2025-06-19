using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace AmitalCloud.Infrastructure.Data.Azure
{
    public static class BlobExtensions
    {
        public static bool Exists(this BlobClient blob)
        {
            try
            {
                BlobProperties properties = blob.GetProperties();
                return true;
            }
            catch (RequestFailedException e) when (e.Status == 404)
            {
                return false;
            }
            catch (RequestFailedException e)
            {
                throw new Exception($"An error occurred while checking blob existence: {e.Message}", e);
            }
        }
    }
}