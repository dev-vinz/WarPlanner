using Wp.Bot.Services.Languages.French;
using Wp.Common.Models;

namespace Wp.Bot.Services.Languages
{
    public static class LanguageExtensions
    {
        public static IGlobalResponse GetGlobalResponse(this Language language)
        {
            return language switch
            {
                Language.FRENCH => new FrenchGlobalResponse(),
                _ => throw new ArgumentNullException(nameof(language)),
            };
        }
    }
}
