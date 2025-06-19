using AmitalCloud.Infrastructure.Data.Azure;
using AmitalCloud.Infrastructure.Domain.Helpers;
using AmitalCloud.Infrastructure.Domain.Interfaces;
using System;
using System.ServiceModel;

namespace AmitalCloud.Infrastructure.Data.Services
{
    public class FileSystemBlobService : IBlobService
    {
        //master master.PR1 changes (needed only on master branch
        //master master.PR1 hot fix (needed  on master & R5 branches)
        public byte[] Read(BlobFileInfo fileInfo)
        {
            byte[] result = null;
            string filepath = fileInfo.ContainerName + "/" + StorageAcountDetails.GetBlobNameByLocation(fileInfo.FileName + "." + fileInfo.Extension.ToLower(), fileInfo.FolderName);
            var blobService = GetService();
            var response = blobService.Read(filepath);
            if (!response.HasError)
            {
                result = response.Result as byte[];
            }
            else
            {
                throw new Exception(response.ErrorMessage, new Exception(response.InnerErrorMessage));

            }
            return result;

        }

        public void Write(byte[] data, BlobFileInfo fileInfo)
        {

            string filepath = fileInfo.ContainerName + "/" + StorageAcountDetails.GetBlobNameByLocation(fileInfo.FileName + "." + fileInfo.Extension.ToLower(), fileInfo.FolderName);
            var blobService = GetService();
            var response = blobService.Write(data, filepath);
            if (response.HasError)
            {
                throw new Exception(response.ErrorMessage, new Exception(response.InnerErrorMessage));
            }
        }




        public void WriteBlock(byte[] buffer, long sentBytes, string[] blockIdsList, int bufferNumber, BlobFileInfo fileInfo)
        {
            string filepath = fileInfo.ContainerName + "/" + StorageAcountDetails.GetBlobNameByLocation(fileInfo.FileName + "." + fileInfo.Extension.ToLower(), fileInfo.FolderName);
            var blobService = GetService();
            long filesize = fileInfo.FileSize != null ? (long)fileInfo.FileSize : 0;
            var response = blobService.WriteBlock(buffer, filesize, sentBytes, filepath, fileInfo.FileName);
            if (response.HasError)
            {
                throw new Exception(response.ErrorMessage, new Exception(response.InnerErrorMessage));
            }

        }


        public void Delete(BlobFileInfo fileInfo)
        {

            string filepath = fileInfo.ContainerName + "/" + StorageAcountDetails.GetBlobNameByLocation(fileInfo.FileName + "." + fileInfo.Extension.ToLower(), fileInfo.FolderName);
            var blobService = GetService();
            var response = blobService.Delete(filepath);
            if (response.HasError)
            {
                throw new Exception(response.ErrorMessage, new Exception(response.InnerErrorMessage));
            }

        }

        private static string GetServiceUrl()
        {
            // return @"http://192.116.221.103/AmitalStorageService2/BlobService.svc";
            //return @"http://localhost:34488/BlobService.svc";
            return System.Configuration.ConfigurationManager.AppSettings.Get("BlobServicePath");
        }
        private static BlobServiceReference.BlobServiceClient GetService()
        {
            string blobServicePath = GetServiceUrl();// System.Configuration.ConfigurationManager.AppSettings.Get("BlobServicePath");//"http://localhost:34488/BlobService.asmx";//System.Configuration.ConfigurationManager.AppSettings.Get("BlobServicePath");
            BasicHttpBinding binding = new BasicHttpBinding(BasicHttpSecurityMode.None);
            binding.MaxBufferSize = 2147483647;
            binding.MaxReceivedMessageSize = 2147483647;
            binding.ReaderQuotas.MaxStringContentLength = 2147483647;
            binding.ReaderQuotas.MaxArrayLength = 2147483647;
            binding.SendTimeout = TimeSpan.FromMinutes(5);//huge !!!
            EndpointAddress epa = new EndpointAddress(blobServicePath);


            var blobService = new BlobServiceReference.BlobServiceClient(binding, epa);
            return blobService;
        }

        public bool FileExists(BlobFileInfo fileInfo)
        {
            byte[] result = null;
            string filepath = fileInfo.ContainerName + "/" + StorageAcountDetails.GetBlobNameByLocation(fileInfo.FileName + "." + fileInfo.Extension.ToLower(), fileInfo.FolderName);
            var blobService = GetService();
            var response = blobService.Read(filepath);
            if (!response.HasError)
            {
                result = response.Result as byte[];
            }

            return (result != null);
        }

        public void AppendText(string text, BlobFileInfo fileInfo)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            //throw new NotImplementedException();
        }

        public void MoveFromAnotherStorage(string containerSASURI, string fileNameSource, BlobFileInfo destinationFileInfo)
        {
            throw new NotImplementedException();
        }
    }
}
