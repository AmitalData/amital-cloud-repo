using AmitalCloud.Infrastructure.Data.Context;
using AmitalCloud.Infrastructure.Data.Helpers;
using AmitalCloud.Infrastructure.Data.Repositories;
using AmitalCloud.Infrastructure.Model.EntityClasses ;
using AmitalCloud.Infrastructure.Domain.Helpers;
using AmitalCloud.Infrastructure.Domain.Interfaces;
using System;
using System.Linq;
using System.Text;

namespace AmitalCloud.Infrastructure.Data.Services
{
    public class DatabaseBlobService : IBlobService
    {
        public byte[] Read(BlobFileInfo fileInfo)
        {
            byte[] result = null;
            if (fileInfo.UnifreightFillingDownload(out result))
            {
                return result;
            }
            IRepository<BlobFile> repo = new Repository<BlobFile>(fileInfo.Tenant);
            BlobFile file = GetBlobFile(fileInfo, repo);
            if (file != null && file.Blob != null)
            {
                if (file.IsCompressed)
                    result = ByteCompressor.Decompress(file.Blob);
                else
                    result = file.Blob;

            }
            return result;
        }
        private BlobFile GetBlobFile(BlobFileInfo fileInfo, IRepository<BlobFile> repository)
        {
            return repository.GetMulti(a => a.Id == fileInfo.FileName).FirstOrDefault();
        }
        public void Write(byte[] data, BlobFileInfo fileInfo)
        {
            if (fileInfo.UnifreightFillingUpload(data))
            {
                return;
            }
            using (var uow = new UnitOfWork<AmitalCloudContext>(fileInfo.Tenant))
            {
                IRepository<BlobFile> repo = new Repository<BlobFile>(uow);
                BlobFile file = repo.GetMulti(a => a.Id == fileInfo.FileName).FirstOrDefault();
                if (file != null)
                {
                    file.Blob = ByteCompressor.Compress(data);
                    file.IsCompressed = true;
                    repo.Update(file);
                }
                else
                {
                    file = new BlobFile()
                    {
                        Id = fileInfo.FileName,
                        Blob = ByteCompressor.Compress(data),
                        IsCompressed = true,
                    };

                    repo.Insert(file);

                }
                uow.Save();
            }
        }
        public void WriteBlock(byte[] buffer, long sentBytes, string[] blockIdsList, int bufferNumber, BlobFileInfo fileInfo)
        {
            if (fileInfo.UnifreightFillingUploadBlock(buffer))
            {
                return;
            }

            using (var uow = new UnitOfWork<AmitalCloudContext>(fileInfo.Tenant))
            {
                IRepository<BlobFile> repo = new Repository<BlobFile>(uow);
                BlobFile file = GetBlobFile(fileInfo, repo);//  blobFileRepository.GetSingleBlobFile(fileInfo.FileName);
                if (file != null)
                {
                    if (file.Blob == null || (buffer != null && fileInfo.FileSize == buffer.Length))
                    {
                        file.Blob = buffer;
                    }
                    else
                    {
                        int i = file.Blob.Length;
                        byte[] blobData = file.Blob;
                        Array.Resize<byte>(ref blobData, i + buffer.Length);
                        buffer.CopyTo(blobData, i);
                        file.Blob = blobData;
                    }
                    repo.Update(file);
                }
                else
                {
                    file = new BlobFile()
                    {
                        Id = fileInfo.FileName,
                        Blob = buffer
                    };
                    repo.Insert(file);
                }
                uow.Save();
            }
        }
        public void Delete(BlobFileInfo fileInfo)
        {
            if (fileInfo.UnifreightFillingDelete())
            {
                return;
            }
            using (var uow = new UnitOfWork<AmitalCloudContext>(fileInfo.Tenant))
            {
                IRepository<BlobFile> repo = new Repository<BlobFile>(uow);
                BlobFile file = GetBlobFile(fileInfo, repo);
                if (file != null)
                {
                    repo.Delete(file);
                }
                uow.Save();
            }
        }
        public bool FileExists(BlobFileInfo fileInfo)
        {
            // decompress
            byte[] result = null;
            if (fileInfo.UnifreightFillingDownload(out result))
            {
                return (result != null);
            }
            IRepository<BlobFile> repo = new Repository<BlobFile>(fileInfo.Tenant);
            BlobFile file = GetBlobFile(fileInfo, repo);

            return (file != null);

        }

        public void AppendText(string text, BlobFileInfo fileInfo)
        {
            using (var uow = new UnitOfWork<AmitalCloudContext>(fileInfo.Tenant))
            {
                IRepository<BlobFile> repo = new Repository<BlobFile>(uow);

                if (!string.IsNullOrEmpty(text))
                {
                    BlobFile file = repo.GetMulti(a => a.Id == fileInfo.FileName).FirstOrDefault();
                    if (file != null)
                    {
                        if (file.Blob == null)
                        {
                            var txtBytes = Encoding.UTF8.GetBytes(text);
                            file.Blob = txtBytes;
                        }
                        else
                        {
                            var fileText = Encoding.UTF8.GetString(file.Blob);
                            fileText += text;
                            var txtBytes = Encoding.UTF8.GetBytes(fileText);
                            file.Blob = txtBytes;
                            repo.Update(file);
                        }
                    }
                    else
                    {
                        var txtBytes = Encoding.UTF8.GetBytes(text);
                        file = new BlobFile()
                        {
                            Id = fileInfo.FileName,
                            Blob = txtBytes,
                        };

                        repo.Insert(file);

                    }
                    uow.Save();
                }
            }
        }
        public void Dispose()
        {
            //  throw new NotImplementedException();
        }

        public void MoveFromAnotherStorage(string containerSASURI, string fileNameSource, BlobFileInfo destinationFileInfo)
        {
            throw new NotImplementedException();
        }
    }
}
