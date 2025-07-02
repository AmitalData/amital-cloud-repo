
using AmitalCloud.Infrastructure.Data.Counters;
using AmitalCloud.Infrastructure.Data.Repositories;
using AmitalCloud.Infrastructure.Domain.EntityPMs;
using AmitalCloud.Infrastructure.Model.EntityClasses;
using System.Text;

namespace AmitalCloud.Infrastructure.MUpdater.Data.AddClasses
{
    public class AddEventTypes
    {
        public static void AddEventType(EventTypePM eventTypeDetails, Repository<EventType> eventTypeRepository, Dictionary<string, EventType> tenantEventTypes, int contextTenant=0)
        {
            if (tenantEventTypes.Keys.Contains(eventTypeDetails.Code + eventTypeDetails.ObjectTableId))
            {
                EventType eventType = tenantEventTypes[eventTypeDetails.Code + eventTypeDetails.ObjectTableId];
                eventType.EnglishName = eventTypeDetails.EnglishName;
                eventType.AddedManually = eventTypeDetails.AddedManually;
                eventType.EntityStatusId = eventTypeDetails.EntityStatusId;
                eventType.FollowUpEnglishName = eventTypeDetails.FollowUpEnglishName;
                eventType.FollowUpLocalName = TryConvertFromBase64(eventTypeDetails.FollowUpLocalName);
                eventType.InActive = eventTypeDetails.InActive;
                eventType.IsFollowUp = eventTypeDetails.IsFollowUp;
                eventType.IsManualEntry = eventTypeDetails.IsManualEntry;
                eventType.LocalName = TryConvertFromBase64(eventTypeDetails.LocalName);
                eventType.ManualActivatedFollowUp = eventTypeDetails.ManualActivatedFollowUp;
                eventType.ObjectTableId = eventTypeDetails.ObjectTableId;
                eventType.ShortView = eventTypeDetails.ShortView;
                eventType.Tenant = eventTypeDetails.Tenant;
                eventType.SearchFields = eventTypeDetails.Code + "," + eventTypeDetails.EnglishName + "," + eventTypeDetails.LocalName;
                eventType.EventTypeCategoryCode = string.IsNullOrEmpty(eventTypeDetails.EventTypeCategoryCode) ? "OPE" : eventTypeDetails.EventTypeCategoryCode;
                eventType.IsCustomerView = eventTypeDetails.IsCustomerView;
                eventType.IsAgentView = eventTypeDetails.IsAgentView;
                eventType.IsSharedLogisticsEnabled = eventTypeDetails.IsSharedLogisticsEnabled;
                eventType.AllowedInAutomation = eventTypeDetails.AllowedInAutomation;
                eventType.UpdateDate = DateTime.Now;

                eventTypeRepository.Update(eventType);
            }

            else
            {
                EventType newEventType = new EventType()
                {
                    Id = IdCounter.GetNumber("EventType", contextTenant).ToString(),
                    Tenant = eventTypeDetails.Tenant,
                    ShortView = eventTypeDetails.ShortView,
                    ObjectTableId = eventTypeDetails.ObjectTableId,
                    ManualActivatedFollowUp = eventTypeDetails.ManualActivatedFollowUp,
                    LocalName = TryConvertFromBase64(eventTypeDetails.LocalName),
                    IsManualEntry = eventTypeDetails.IsManualEntry,
                    IsFollowUp = eventTypeDetails.IsFollowUp,
                    InActive = eventTypeDetails.InActive,
                    FollowUpLocalName = TryConvertFromBase64(eventTypeDetails.FollowUpLocalName),
                    FollowUpEnglishName = eventTypeDetails.FollowUpEnglishName,
                    EntityStatusId = eventTypeDetails.EntityStatusId,
                    AddedManually = eventTypeDetails.AddedManually,
                    Code = eventTypeDetails.Code,
                    EnglishName = eventTypeDetails.EnglishName,
                    SearchFields = eventTypeDetails.Code + "," + eventTypeDetails.EnglishName + "," + eventTypeDetails.LocalName,
                    EventTypeCategoryCode = string.IsNullOrEmpty(eventTypeDetails.EventTypeCategoryCode) ? "OPE" : eventTypeDetails.EventTypeCategoryCode,
                    IsCustomerView = eventTypeDetails.IsCustomerView,
                    IsAgentView = eventTypeDetails.IsAgentView,
                    IsSharedLogisticsEnabled = eventTypeDetails.IsSharedLogisticsEnabled,
                    AllowedInAutomation = eventTypeDetails.AllowedInAutomation,
                    UpdateDate = DateTime.Now,

                };

                eventTypeRepository.Insert(newEventType);
            }

        }

        public static string TryConvertFromBase64(string input)
        {
            try
            {
                if (input == null)
                {
                    return null;
                }
                if (input.StartsWith("BS64:") || input.StartsWith("\"BS64:"))
                {

                    return ConvertFromBase64(input);


                }
                return input;

            }
            catch (FormatException)
            {
                return input;
            }
        }

        private static string ConvertFromBase64(string input)
        {
            string backUp = input;
            try
            {
                input = input.Trim('\"');
                input = input.Substring(5);//REMOVE BS64:
                byte[] data = Convert.FromBase64String(input);
                string decodedString = Encoding.UTF8.GetString(data);
                decodedString = decodedString.Trim('\"');
                return decodedString;

            }
            catch (FormatException)
            {
                return backUp;
            }

        }

    }
}