using AmitalCloud.Infrastructure.Data.Context;
using AmitalCloud.Infrastructure.Data.Repositories;
using AmitalCloud.Infrastructure.Data.Services;
using AmitalCloud.Infrastructure.Domain.DataContracts;
using AmitalCloud.Infrastructure.Model.EntityClasses ;
using AmitalCloud.Infrastructure.Domain.Helpers;
using AmitalCloud.Infrastructure.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using AmitalCloud.Infrastructure.Model.Interfaces;

namespace AmitalCloud.Infrastructure.Data.Helpers
{
    public static class BlobFileInfoExt
    {

        const bool isFeatureOn = true;

        public static bool UnifreightFillingDelete(this BlobFileInfo fileInfo)
        {
            if (!fileInfo.IsUnifreightFillingMode())
            {
                return false;
            }
            NetCommonHelper.Logger.DevLog.Instance.WriteDebug("UnifreightFillingDelete:Done (meanwhile nothing to do the filling will remain on Unifreight )");
            return true;
        }
        public static bool UnifreightFillingUpload(this BlobFileInfo fileInfo, byte[] data)
        {
            //string documentId; DocumentsFiling poco = null;
            if (!fileInfo.IsUnifreightFillingMode())
            {
                return false;
            }

            //CustomsSettingQueryService.GetSettingByTenant(tenant)
             return true;
        }
        public static bool IsUnifreightFillingModeBase(int tenant, string FolderName)
        {
            if (!isFeatureOn)
            {
                return isFeatureOn;
            }
            if (AmitalCloudSettings.StorageServiceMode != "db")
            {
                return false;
            }
            if (!AmitalCloudSettings.IsCostomsDeploy)
            {
                return false;
            }
            if (!AmitalCloudSettings.GetAmitalCustomsSettingsMInject(tenant).IsConnectedToUniFreight)
            {
                return false;
            }
            if (FolderName != "docsin")
            {
                return false;
            }


            if (tenant == 0)
            {
                return false;
            }


            if (String.IsNullOrWhiteSpace(AmitalCloudSettings.GetAmitalCustomsSettingsMInject(tenant).OnPremiseFillingService))
            {
                return false;
            }
            return true;
        }
        public static bool IsUnifreightFillingMode(this BlobFileInfo fileInfo, bool isnew = false)
        {
            var documentId = "";
            if (!IsUnifreightFillingModeBase(fileInfo.Tenant, fileInfo.FolderName))
            {
                return false;
            }
            documentId = fileInfo.FileName;
            if (String.IsNullOrWhiteSpace(documentId))
            {
                return false;
            }

            var maybe = ("documentId.extension" == "documentId.extension");
            if (maybe)
            {
                documentId = documentId.Split("."[0])[0];
            }
            if (String.IsNullOrWhiteSpace(documentId))
            {
                return false;
            }
            if (isnew)
            {
                return true;
            }
            if (!string.IsNullOrWhiteSpace(fileInfo.UDocumentsFilingId) && fileInfo.UCreateDate.HasValue)
            {
                fileInfo.UMode = true;
                fileInfo.UDocumentsId = documentId;

            }
            else
            {
                IAmitalCloudContext context = AmitalCloudContext.GetContext(fileInfo.Tenant);
                var poco = new Repository<DocumentsFiling>(context).GetMulti(a => a.DocumentId == documentId && a.Tenant == fileInfo.Tenant).FirstOrDefault();//.GetSingleDocumentsFilingByDocumentId(documentId, fileInfo.Tenant);
                if (poco == null)
                {
                    return false;
                }
                fileInfo.UMode = true;
                fileInfo.UDocumentsId = documentId;
                fileInfo.UDocumentsFilingId = poco.Id;
                fileInfo.UCreateDate = poco.CreateDate;
            }

            return true;
        }
        public static bool UnifreightFillingDownload(this BlobFileInfo fileInfo, out byte[] data)
        {
            data = null;
            //string documentId; DocumentsFiling poco = null;
            if (!fileInfo.IsUnifreightFillingMode())
            {
                return false;
            }
            if (fileInfo.USuppressWriteDueSameMD5Hash)
            {
                NetCommonHelper.Logger.DevLog.Instance.WriteDebug("fileInfo.USuppressWriteDueSameMD5Hash");
                return true;
            }

            //CustomsSettingQueryService.GetSettingByTenant(tenant)
             return true;
        }

        public static bool UnifreightFillingUploadBlock(this BlobFileInfo fileInfo, byte[] data)
        {
            //string documentId; DocumentsFiling poco = null;
            //if (!IsUnifreightFillingMode(fileInfo, out documentId, out poco))
            if (!fileInfo.IsUnifreightFillingMode())
            {
                return false;
            }
            var myBlobFile = MyBlobFile.AddBlock(fileInfo.UDocumentsId, data, fileInfo.FileSize.Value);
            if (myBlobFile != null)
            {
                UnifreightFillingUpload(fileInfo, myBlobFile.Blob);
            }
            return true;

        }

        class MyBlobFile
        {


            readonly string _Id;
            readonly DateTime _CreateAt;


            private MyBlobFile()
            {
            }
            private MyBlobFile(string Id)
            {
                _CreateAt = DateTime.Now;
                this._Id = Id;
            }
            public byte[] Blob { get; set; }
            public string Id
            {
                get { return _Id; }
            }
            public DateTime CreateAt
            {
                get { return _CreateAt; }
            }


            private static List<MyBlobFile> _List = new List<MyBlobFile>();
            internal static MyBlobFile AddBlock(string blobId, byte[] buffer, double FileSize)
            {
                var created = DateTime.Now;
                lock (_List)
                {
                    var have2del = new List<MyBlobFile>(_List
                        .Where(rec => DateTime.Now.Subtract(rec.CreateAt) > TimeSpan.FromMinutes(10)));
                    foreach (var curr in have2del)
                    {
                        _List.Remove(curr);
                    }
                    var file = _List.FirstOrDefault(r => r.Id == blobId);
                    if (file == null)
                    {
                        file = new MyBlobFile(blobId);
                        _List.Add(file);
                    }
                    if (file.Blob == null)
                    {
                        file.Blob = buffer;

                    }
                    else
                    {
                        int i = file.Blob.Length;
                        byte[] blobData = file.Blob;
                        Array.Resize<byte>(ref blobData, i + buffer.Length);
                        buffer.CopyTo(blobData, i);

                        //if (sentBytes == fileSize)
                        //{
                        //    blobData = ByteCompressor.Compress(blobData);
                        //}

                        file.Blob = blobData;

                    }
                    if (file.Blob.Length == FileSize)
                    {
                        _List.Remove(file);
                        return file;
                    }
                    return null;

                }
            }
        }
    }

}
