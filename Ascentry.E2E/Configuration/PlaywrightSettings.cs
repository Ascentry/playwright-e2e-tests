using System;
using System.ComponentModel;
using System.Reflection;

namespace Ascentry.E2E.Configurations
{
    public class PlaywrightSettings
    {
        [Description("PLAYWRIGHT_PRODUCT")]
        public string Product { get; set; }

        [Description("PLAYWRIGHT_CULTURE")]
        public string Culture { get; set; }

        [Description("PLAYWRIGHT_ROOTURL")]
        public string RootUrl { get; set; }

        [Description("PLAYWRIGHT_HEADLESS")]
        public string HeadLess { get; set; }

        [Description("PLAYWRIGHT_SLOWMO")]
        public string SlowMo { get; set; }

        [Description("PLAYWRIGHT_USERNAME")]
        public string UserName { get; set; }

        [Description("PLAYWRIGHT_PASSSWORD")]
        public string Password { get; set; }

        public static string GetDescription(string propertyName)
        {
            PropertyInfo property = typeof(PlaywrightSettings).GetProperty(propertyName);

            if (property == null)
            {
                throw new ArgumentException($"Attribute '{propertyName}' does not exist in {nameof(PlaywrightSettings)}.");
            }

            DescriptionAttribute attribute = property.GetCustomAttribute<DescriptionAttribute>();
            return attribute?.Description;
        }
    }
}