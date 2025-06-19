namespace AmitalCloud.Infrastructure.Domain.Interfaces
{
    public interface IHtmlEditorHelper
    {
        //string SendEmailOutActivityForEntity(byte[] htmlData, byte[] textData, int tenant, string toEmail, string subject, string cc, string bcc, string userId, string myEntityId, string customerId, string objectTableId, string attachments, string entityReference, string documentTypeCode, string eventTypeCode);
        string SendHtmlDocument(byte[] htmlData, string internalDocumentId, string externalDocumentId, int tenant, string toEmail, string subject, string cc, string bcc, string userId, string entityId, string objectTableId, string attachments, string entityReference, string from, string replyTo);
        object GetEntity(string entityName, string entityId, int tenant);

    }
}