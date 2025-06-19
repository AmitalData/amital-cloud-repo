using AmitalCloud.Infrastructure.Domain.DataContracts;
using AmitalCloud.Infrastructure.Domain.Enums;
using System.Collections.Generic;

namespace AmitalCloud.Infrastructure.Domain.Interfaces
{
    public interface IEntityPM
    {
        List<NotifyPropertyChangeValues> ChangedProperties { get; }
        ChangeSetOperation ChangeSetOp { get; set; }
        object CurrentContextTag { get; set; }
        string EncodeBase64NVARCHARFieldsBy { get; set; }
        int Tenant { get; set; }
        void NotifyPropertyChanged(NotifyPropertyChangeValues values);
    }
}