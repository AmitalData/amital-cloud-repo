using System;

namespace AmitalCloud.Infrastructure.Domain.Helpers
{
    public partial class BlobFileInfo
    {
        public string FileName { get; set; }
        public string Extension { get; set; }
        public double? FileSize { get; set; }
        public string FolderName { get; set; }

        int tenant;

        public int Tenant
        {
            get { return tenant; }
            set
            {
                tenant = value;
                //if (string.IsNullOrEmpty(ContainerName))
                // {
                this.ContainerName = "tenant" + value;
                // }
            }
        }

        public string ContainerName { get; private set; }
        public bool HasExternalContainer { get; set; }
        public string ExternalContainerName { get; set; }
        public bool IsDecrypted { get; set; }
        public string AesKey { get; set; }
        public bool IsEncrypted { get; set; }

        public bool UMode { get; set; }
        public string UDocumentsFilingId { get; set; }
        public string UDocumentsId { get; set; }
        public DateTime? UCreateDate { get; set; }
        public int? UFileVer { get; set; }
        public bool USuppressWriteDueSameMD5Hash { get; set; }
    }

}
