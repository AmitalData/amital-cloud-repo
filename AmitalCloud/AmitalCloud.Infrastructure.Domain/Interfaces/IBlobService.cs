using AmitalCloud.Infrastructure.Domain.Helpers;

namespace AmitalCloud.Infrastructure.Domain.Interfaces
{
    public interface IBlobService
    {
        byte[] Read(BlobFileInfo fileInfo);
        void Write(byte[] data, BlobFileInfo fileInfo);
        void WriteBlock(byte[] buffer, long sentBytes, string[] blockIdsList, int bufferNumber, BlobFileInfo fileInfo);
        void Delete(BlobFileInfo fileInfo);
        bool FileExists(BlobFileInfo fileInfo);
        void AppendText(string text, BlobFileInfo fileInfo);
        void Dispose();
        void MoveFromAnotherStorage(string containerSASURI, string fileNameSource, BlobFileInfo destinationFileInfo);
    }
}
