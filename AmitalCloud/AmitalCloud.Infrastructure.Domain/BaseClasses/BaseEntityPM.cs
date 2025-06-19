using AmitalCloud.Infrastructure.Domain.DataContracts;
using AmitalCloud.Infrastructure.Domain.Enums;
using AmitalCloud.Infrastructure.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
namespace AmitalCloud.Infrastructure.Domain.BaseClasses
{
    [DataContract]
    public abstract class BaseEntityPM : IEntityPM
    {
        protected BaseEntityPM()
        {
            AdditionalProperties = new Dictionary<string, object>();
        }
        [DataMember]
        public ChangeSetOperation ChangeSetOp { get; set; }
        public virtual int Tenant { get; set; }
        [DataMember]
        public string EncodeBase64NVARCHARFieldsBy { get; set; }
        [ThreadStatic]
        public static bool SuppressCreateNotifyPropertyChangeValues = false;
        object _CurrentContextTag;
        public object CurrentContextTag
        {
            get { return _CurrentContextTag; }
            set { _CurrentContextTag = value; }
        }
        List<NotifyPropertyChangeValues> changedProperties;
        public List<NotifyPropertyChangeValues> ChangedProperties
        {
            get
            {
                if (changedProperties == null)
                {
                    if (SuppressCreateNotifyPropertyChangeValues) return null;
                    changedProperties = new List<NotifyPropertyChangeValues>();
                }
                return changedProperties;
            }
        }
        public void NotifyPropertyChanged(NotifyPropertyChangeValues values)
        {
            if (SuppressCreateNotifyPropertyChangeValues) return;
            values.Id = Guid.NewGuid().ToString();
            ChangedProperties.Add(values);
        }
        public Dictionary<string, object> AdditionalProperties { get; set; }
    }
}
