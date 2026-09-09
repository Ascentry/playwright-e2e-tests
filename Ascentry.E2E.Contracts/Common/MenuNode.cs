using Ascentry.E2E.Contracts.Enums;

namespace Ascentry.E2E.Contracts.Common
{
    public class MenuNode
    {
        public TranslationEnum Key { get; }
        public MenuLevelEnum Level { get; }

        public MenuNode(MenuLevelEnum level, TranslationEnum key)
        {
            Level = level;
            Key = key;
        }
    }
}
