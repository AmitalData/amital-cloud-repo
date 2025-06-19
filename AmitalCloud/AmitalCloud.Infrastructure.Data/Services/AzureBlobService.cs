using AmitalCloud.Infrastructure.Data.Azure;
using AmitalCloud.Infrastructure.Data.Context;
using AmitalCloud.Infrastructure.Data.Helpers;
using AmitalCloud.Infrastructure.Data.Repositories;
using AmitalCloud.Infrastructure.Domain.DataContracts;
using AmitalCloud.Infrastructure.Model.EntityClasses ;
using AmitalCloud.Infrastructure.Domain.Helpers;
using AmitalCloud.Infrastructure.Domain.Interfaces;
using AmitalCloud.Infrastructure.Model.Interfaces;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Specialized;
using System.Text;
using Azure.Storage.Blobs.Models;

namespace AmitalCloud.Infrastructure.Data.Services
{
    public class AzureBlobService : IBlobService, IDisposable
    {
        public byte[] Read(BlobFileInfo fileInfo)
        {
            //tenant1/docsout/1-379.pdf
            //string containername = filepath.Split('/')[0];
            //string localPath = filepath.Replace(containername + "/", "");
            byte[] result = null;
            if (fileInfo.FolderName != "SchedularLogs")
            {
                BlobClient blobfile = GetCloudBlockBlob(fileInfo);

                result = DownloadCloudBlob(blobfile);
                if (fileInfo.FolderName != "logos" && !fileInfo.IsDecrypted)
                {
                    Document document = GetDocument(fileInfo, AmitalCloudContext.GetContext(fileInfo.Tenant));
                    if (document != null && document.IsEncrypted)
                    {
                        AesFunction aesFunction = new AesFunction();
                        result = aesFunction.DecryptData(result, fileInfo.Tenant);
                    }
                }
            }
            else
            {
                BlobClient blobfile = GetCloudAppendBlob(fileInfo);
                result = DownloadCloudBlob(blobfile);
            }

            return result;
        }

        private byte[] DownloadCloudBlob(BlobClient blobfile)
        {
            byte[] result = null;
            if (blobfile.Exists())
            {
                using (MemoryStream memstream = new MemoryStream())
                {
                    blobfile.DownloadToAsync(memstream);
                    result = memstream.ToArray();
                }
            }
            return result;
        }
        private BlobClient GetCloudBlockBlob(BlobFileInfo fileInfo)
        {
            string localPath = null;
            BlobContainerClient blobContainer = null;
            GetFileBlobContainerInfo(fileInfo, out localPath, out blobContainer);
            BlobClient blobfile = blobContainer.GetBlobClient(localPath);
            if (!blobfile.Exists())
            {
                if (!string.IsNullOrEmpty(AmitalCloudSettings.AzureFolderName) && IsEnableAzureRootFolder(fileInfo.Tenant))
                {
                    GetFileBlobContainerWithoutAzureFolder(fileInfo, out localPath, out blobContainer);
                    blobfile = blobContainer.GetBlobClient(localPath);
                    if (!blobfile.Exists())
                    {
                        blobfile = blobContainer.GetBlobClient(localPath.ToLower());
                    }
                }
                else
                    blobfile = blobContainer.GetBlobClient(localPath.ToLower());
            }
            return blobfile;
        }

        private BlobClient GetCloudAppendBlob(BlobFileInfo fileInfo)
        {
            string localPath = null;
            BlobContainerClient blobContainer = null;
            GetFileBlobContainerInfo(fileInfo, out localPath, out blobContainer);
            BlobClient blobfile = blobContainer.GetBlobClient(localPath);

            if (!blobfile.Exists())
            {
                if (!string.IsNullOrEmpty(AmitalCloudSettings.AzureFolderName) && IsEnableAzureRootFolder(fileInfo.Tenant))
                {
                    GetFileBlobContainerWithoutAzureFolder(fileInfo, out localPath, out blobContainer);
                    blobfile = blobContainer.GetBlobClient(localPath);
                    if (!blobfile.Exists())
                    {
                        blobfile = blobContainer.GetBlobClient(localPath.ToLower());
                    }
                }
                else
                    blobfile = blobContainer.GetBlobClient(localPath.ToLower());
            }

            return blobfile;
        }


        private void GetFileBlobContainerWithoutAzureFolder(BlobFileInfo fileInfo, out string localPath, out BlobContainerClient blobContainer)
        {
            if (fileInfo.HasExternalContainer)
            {
                string extension = !string.IsNullOrEmpty(fileInfo.Extension) ? fileInfo.Extension.ToLower() : "";
                localPath = fileInfo.FileName + "." + extension;
                blobContainer = StorageAcountDetails.GetCurrentContainer(fileInfo.ExternalContainerName);

            }
            else
            {
                string extension = !string.IsNullOrEmpty(fileInfo.Extension) ? fileInfo.Extension.ToLower() : "";
                localPath = StorageAcountDetails.GetBlobNameByLocation(fileInfo.FileName + "." + extension, fileInfo.FolderName);
                blobContainer = StorageAcountDetails.GetCurrentContainer(fileInfo.ContainerName);
            }
        }

        private void GetFileBlobContainerInfo(BlobFileInfo fileInfo, out string localPath, out BlobContainerClient blobContainer)
        {
            if (fileInfo.HasExternalContainer)
            {
                if (string.IsNullOrEmpty(fileInfo.ExternalContainerName))
                {
                    fileInfo.ExternalContainerName = "tenant" + fileInfo.Tenant;
                }
                string extension = !string.IsNullOrEmpty(fileInfo.Extension) ? fileInfo.Extension.ToLower() : "";

                localPath = fileInfo.FileName + "." + extension;
                if (!string.IsNullOrEmpty(AmitalCloudSettings.AzureFolderName) && IsEnableAzureRootFolder(fileInfo.Tenant))
                {
                    localPath = fileInfo.ExternalContainerName + "/" + localPath;
                    blobContainer = StorageAcountDetails.GetCurrentContainer(AmitalCloudSettings.AzureFolderName.ToLower());
                }
                else
                {
                    blobContainer = StorageAcountDetails.GetCurrentContainer(fileInfo.ExternalContainerName);
                }
            }
            else
            {
                string extension = !string.IsNullOrEmpty(fileInfo.Extension) ? fileInfo.Extension.ToLower() : "";

                localPath = StorageAcountDetails.GetBlobNameByLocation(fileInfo.FileName + "." + extension, fileInfo.FolderName);
                if (!string.IsNullOrEmpty(AmitalCloudSettings.AzureFolderName) && IsEnableAzureRootFolder(fileInfo.Tenant))
                {
                    localPath = fileInfo.ContainerName + "/" + localPath;
                    blobContainer = StorageAcountDetails.GetCurrentContainer(AmitalCloudSettings.AzureFolderName.ToLower());
                }
                else
                {
                    if (fileInfo.FolderName == "how-to")
                    {
                        blobContainer = StorageAcountDetails.GetCurrentContainer(fileInfo.FolderName);
                    }

                    else
                    {
                        blobContainer = StorageAcountDetails.GetCurrentContainer(fileInfo.ContainerName);
                    }
                }
            }
        }

        public void MoveFromAnotherStorage(string containerSASURI, string fileNameSource, BlobFileInfo destinationFileInfo)
        {
            string cacheKey = $"BlobContainer_({containerSASURI.Substring(containerSASURI.Length - 30, 29)})";

            BlobContainerClient blobContainerSource = CacheManager.GetOrInsertNewObject(cacheKey, () =>
                new BlobContainerClient(new Uri(containerSASURI)));
            BlobClient blobSource = blobContainerSource.GetBlobClient(fileNameSource);
            GetFileBlobContainerInfo(destinationFileInfo, out string localPath, out BlobContainerClient blobContainerDest);
            BlobClient blobDestination = blobContainerDest.GetBlobClient(localPath);

            blobDestination.StartCopyFromUri(blobSource.Uri);

            bool isCopyComplete = false;
            while (!isCopyComplete)
            {
                var propertiesResponse = blobDestination.GetProperties();
                var properties = propertiesResponse.Value;

                if (properties.CopyStatus == CopyStatus.Success)
                {
                    isCopyComplete = true;
                }
                else if (properties.CopyStatus == CopyStatus.Failed)
                {
                    throw new InvalidOperationException("Blob copy failed.");
                }
                else
                {
                    Thread.Sleep(50);
                }
            }

            blobSource.Delete();
        }

        public void Write(byte[] data, BlobFileInfo fileInfo)
        {
            string localPath = null;
            BlobContainerClient blobContainer = null;
            GetFileBlobContainerInfo(fileInfo, out localPath, out blobContainer);


            var blobfile = blobContainer.GetBlobClient(localPath);
            using (Stream blobstream = blobfile.OpenWrite(overwrite: true))
            {
                if (fileInfo.FolderName != "logos")
                {
                    IAmitalCloudContext context = AmitalCloudContext.GetContext(fileInfo.Tenant);
                    Document document = GetDocument(fileInfo, context);
                    if ((document != null && document.IsEncrypted) || fileInfo.IsEncrypted)
                    {
                        DocumentsFiling documentsFiling = GetDocumentInfo(fileInfo, context);
                        if (documentsFiling != null)
                        {
                            blobfile.SetMetadata(new Dictionary<string, string>
                            {
                                { "Code", documentsFiling.Code }
                            });
                        }
                        AesFunction aesFunction = new AesFunction();
                        data = aesFunction.EncryptData(data, fileInfo.Tenant, fileInfo.AesKey);
                    }
                }
                blobstream.Write(data, 0, (int)data.Length);
            }
        }

        private DocumentsFiling GetDocumentInfo(BlobFileInfo fileInfo, IAmitalCloudContext context)
        {
            return new Repository<DocumentsFiling>(context).GetMulti(a => a.Document.FileName == fileInfo.FileName && a.Tenant == fileInfo.Tenant).FirstOrDefault();
            //.GetSingleDocumentsFilingByDocumentId(fileInfo.FileName, fileInfo.Tenant);
        }

        private Document GetDocument(BlobFileInfo fileInfo, IAmitalCloudContext context)
        {
            return new Repository<Document>(context).GetMulti(a => a.FileName == fileInfo.FileName && a.Tenant == fileInfo.Tenant).FirstOrDefault();
            //.GetSingleDocument(fileInfo.Tenant, fileInfo.FileName);
        }

        public void WriteBlock(byte[] buffer, long sentBytes, string[] blockIdsList, int bufferNumber, BlobFileInfo fileInfo)
        {
            string localPath = null;
            BlobContainerClient blobContainer = null;
            GetFileBlobContainerInfo(fileInfo, out localPath, out blobContainer);
            var tempcloudBlockBlob = blobContainer.GetBlockBlobClient(StorageAcountDetails.GetBlobNameByLocation(fileInfo.FileName, ""));
            if (sentBytes < fileInfo.FileSize)
            {
                using (MemoryStream memorystream = new MemoryStream(buffer))
                {
                    tempcloudBlockBlob.StageBlock(blockIdsList[bufferNumber], memorystream);
                }
            }
            else
            {
                var finalcloudBlockBlob = blobContainer.GetBlobClient(localPath);
                using (MemoryStream memorystream = new MemoryStream(buffer))
                {
                    tempcloudBlockBlob.StageBlock(blockIdsList[bufferNumber], memorystream);
                }
                tempcloudBlockBlob.CommitBlockList(blockIdsList);

                using (MemoryStream memstream = new MemoryStream())
                {
                    tempcloudBlockBlob.DownloadToAsync(memstream);
                    var result = memstream.ToArray();
                    using (Stream blobstream = finalcloudBlockBlob.OpenWrite(overwrite: true))
                    {
                        bool skippfileTest = false;
                        if (fileInfo.FolderName != "logos")
                        {
                            IAmitalCloudContext context = AmitalCloudContext.GetContext(fileInfo.Tenant);
                            Document document = GetDocument(fileInfo, context);
                            if ((document != null && document.IsEncrypted) || fileInfo.IsEncrypted)
                            {
                                DocumentsFiling documentsFiling = GetDocumentInfo(fileInfo, context);

                                if (documentsFiling != null)
                                {
                                    finalcloudBlockBlob.SetMetadata(new Dictionary<string, string>
                                    {
                                        { "Code", documentsFiling.Code }
                                    });
                                }

                                AesFunction aesFunction = new AesFunction();
                                result = aesFunction.EncryptData(result, fileInfo.Tenant);

                                skippfileTest = (document != null && document.FileName == "test update failed");
                            }
                        }

                        if (!skippfileTest)
                            blobstream.Write(result, 0, (int)result.Length);

                    }


                }

                tempcloudBlockBlob.DeleteIfExists();
            }
        }


        public void Delete(BlobFileInfo fileInfo)
        {
            string localPath = null;
            BlobContainerClient blobContainer = null;
            GetFileBlobContainerInfo(fileInfo, out localPath, out blobContainer);

            var blobfile = blobContainer.GetBlobClient(localPath);
            if (!blobfile.Exists())
            {
                if (!string.IsNullOrEmpty(AmitalCloudSettings.AzureFolderName) && IsEnableAzureRootFolder(fileInfo.Tenant))
                {
                    GetFileBlobContainerWithoutAzureFolder(fileInfo, out localPath, out blobContainer);
                    blobfile = blobContainer.GetBlobClient(localPath);
                }
            }
            if (blobfile.Exists())
            {
                blobfile.Delete();
            }


        }

        public bool FileExists(BlobFileInfo fileInfo)
        {
            string localPath = null;
            BlobContainerClient blobContainer = null;
            GetFileBlobContainerInfo(fileInfo, out localPath, out blobContainer);

            var blobfile = blobContainer.GetBlobClient(localPath);
            if (!blobfile.Exists())
            {
                if (!string.IsNullOrEmpty(AmitalCloudSettings.AzureFolderName) && IsEnableAzureRootFolder(fileInfo.Tenant))
                {
                    GetFileBlobContainerWithoutAzureFolder(fileInfo, out localPath, out blobContainer);
                    blobfile = blobContainer.GetBlobClient(localPath);
                }
            }

            return blobfile.Exists();
        }

        private bool IsEnableAzureRootFolder(int tenant)
        {
            bool result = FeatureToggleHelper.HasFeatureToggle("EZR", tenant);
            return result;
        }

        public void AppendText(string text, BlobFileInfo fileInfo)
        {
            string localPath = null;
            BlobContainerClient blobContainer = null;
            GetFileBlobContainerInfo(fileInfo, out localPath, out blobContainer);

            var blobfile = blobContainer.GetAppendBlobClient(localPath);

            if (!blobfile.Exists())
            {
                blobfile.Create();
            }

            byte[] textBytes = Encoding.UTF8.GetBytes(text);
            using (var stream = new MemoryStream(textBytes))
            {
                blobfile.AppendBlock(stream);
            }
        }
        public void Dispose()
        {
        }
    }
}

