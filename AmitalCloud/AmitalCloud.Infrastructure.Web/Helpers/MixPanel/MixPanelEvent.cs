using System.Collections.Generic;
using System.Text;

namespace AmitalCloud.Infrastructure.Web.Helpers.MixPanel
{
    public class MixPanelEvent
    {
        public MixPanelEvent()
        {
        }
        private StringBuilder eventJSON = new StringBuilder();

        public string Name { get; set; }
        public List<TrackingEventProperty> Properties = new List<TrackingEventProperty>();

        public override string ToString() => GetEventJSON();

        public void AddProperty(string name, string value)
        {
            Properties.Add(new TrackingEventProperty(name, value));
        }
        public string GetEventJSON()
        {
            eventJSON = new StringBuilder();
            eventJSON.Append("{ ");
            eventJSON.Append($" \"event\": \"{Name}\",");
            eventJSON.Append(" \"properties\": {");
            AppendPropertiesToJSON();
            eventJSON.Append(" }");
            eventJSON.Append("}");
            return eventJSON.ToString();
        }

        private void AppendPropertiesToJSON()
        {
            for (int i = 0; i < Properties.Count; i++)
            {
                AppendProperty(Properties[i]);
                if (i < Properties.Count - 1)
                    eventJSON.Append(" ,");
            }
        }

        private void AppendProperty(TrackingEventProperty property)
        {
            eventJSON.Append($" \"{property.Name}\": \"{property.Value}\"");
        }
    }
}
