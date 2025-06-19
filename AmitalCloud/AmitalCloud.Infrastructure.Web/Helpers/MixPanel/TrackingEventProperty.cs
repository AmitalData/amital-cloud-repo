
namespace AmitalCloud.Infrastructure.Web.Helpers.MixPanel
{
    public class TrackingEventProperty
    {
        public TrackingEventProperty(string name, string value)
        {
            Name = name;
            Value = value;
        }
        public string Name { get; set; }
        public string Value { get; set; }
    }

}
