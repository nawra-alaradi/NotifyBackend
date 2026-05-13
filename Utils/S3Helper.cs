using Amazon.S3.Model;
using Amazon.S3;
using Amazon.Lambda.Core;

namespace NotifyBackend.Utils
{
    public class S3Helper
    {
        private static IAmazonS3 _s3Client;
        private readonly string awsBucketName;

        public S3Helper(string bucketName, IAmazonS3 s3Client)
        {
            //awsBucketName = configuration["AwsS3:BucketName"];
            awsBucketName = bucketName;

            _s3Client = s3Client;
        }

        public async Task<List<string>> GenerateDownloadURLs(long noteID, int expires = 30)
        {
            List<string> downloadUrls = new List<string>();
            // List all objects
            ListObjectsV2Request listRequest = new ListObjectsV2Request
            {
                BucketName = awsBucketName,
                Prefix = $"note{noteID}/"

            };
            ListObjectsV2Response listResponse = await _s3Client.ListObjectsV2Async(listRequest);
            foreach (S3Object obj in listResponse.S3Objects)
            {
                LambdaLogger.Log("Object - " + obj.Key);
                LambdaLogger.Log(" Size - " + obj.Size);
                LambdaLogger.Log(" LastModified - " + obj.LastModified);
                LambdaLogger.Log(" Storage class - " + obj.StorageClass);
                var request = new GetPreSignedUrlRequest
                {
                    Verb = HttpVerb.GET,
                    BucketName = awsBucketName,
                    Key = obj.Key,
                    Expires = DateTime.UtcNow.AddMinutes(expires),

                };
                downloadUrls.Add(_s3Client.GetPreSignedURL(request));

            }
            return downloadUrls;
        }

        public async Task<string> GenerateDownloadURLAsync(long noteID, string fileName )
        {
            string downloadURL = "";
            string key = $"note{noteID}/{fileName}";
            try
            {
                var metadata = await _s3Client.GetObjectMetadataAsync(awsBucketName, key);
                var request = new GetPreSignedUrlRequest
                {
                    BucketName = awsBucketName,
                    Key = key,

                    //Key = $"signatures/note{noteID}/{fileName}",
                    Expires = DateTime.UtcNow.AddDays(7),
                    Verb = HttpVerb.GET // Specify GET verb for download
                };

                downloadURL = _s3Client.GetPreSignedURL(request);

            }

            catch (AmazonS3Exception s3Ex)
            {

                if (s3Ex.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    LambdaLogger.Log($"File not found in S3: {key}");
                    
                }
                LambdaLogger.Log($"Error generating presigned URL: {s3Ex.Message}");

            }
            catch (Exception ex)
            {
                LambdaLogger.Log($"An unexpected error occurred: {ex.Message}");

            }

            return downloadURL;
        }

        public string GenerateUploadURL(string filename, long noteID)
        {
            if (filename.StartsWith("/"))
            {
                filename = filename[1..];
                LambdaLogger.Log("File Name: " + filename);
            }
            //var path = $"signatures/note{noteID}/" + filename;
            var path = $"note{noteID}/" + filename;

            var request = new GetPreSignedUrlRequest
            {
                Verb = HttpVerb.PUT,
                BucketName = awsBucketName,
                Key = path,
                Expires = DateTime.UtcNow.AddMinutes(5)
            };
            var url = _s3Client.GetPreSignedURL(request);
            return url;
        }


        public async void S3MoveTempFile(string key, string newPath)
        {
            var copyObjectRequest = new CopyObjectRequest
            {
                SourceBucket = awsBucketName,
                DestinationBucket = awsBucketName,
                SourceKey = key,
                DestinationKey = newPath,
            };

            await _s3Client.CopyObjectAsync(copyObjectRequest);

            var deleteObjectRequest = new DeleteObjectRequest
            {
                BucketName = awsBucketName,
                Key = key
            };

            await _s3Client.DeleteObjectAsync(deleteObjectRequest);
        }

        public bool DeleteFile(string key)
        {
            var deleteObjectRequest = new DeleteObjectRequest
            {
                BucketName = awsBucketName,
                Key = key
            };
            var res = _s3Client.DeleteObjectAsync(deleteObjectRequest).Result;
            return res.HttpStatusCode == System.Net.HttpStatusCode.OK;
        }
    }
}
