using System.ComponentModel;

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
        [Description("PLAYWRIGHT_TARGET")]
        public string Target { get; set; }
        [Description("PLAYWRIGHT_USERNAME")]
        public string UserName { get; set; }
        [Description("PLAYWRIGHT_PASSSWORD")]
        public string Password { get; set; }
    }
}