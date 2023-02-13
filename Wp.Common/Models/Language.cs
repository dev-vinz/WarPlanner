using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Reflection;
using Wp.Database.Services.Attributes;

namespace Wp.Common.Models
{
    public enum Language
    {
        /// <summary>
        /// French language
        /// </summary>
        [Display(Name = "Français")]
        [Language("fr", "fr-FR", "🇫🇷")]
        FRENCH = 1,
    }

    public static class LanguageExtensions
    {
        /// <summary>
        /// Gets the culture info of this instance
        /// </summary>
        /// <param name="language"></param>
        /// <returns>The culture info of this instance</returns>
        public static CultureInfo GetCultureInfo(this Language language)
        {
            string cultureInfo = language
                .GetType()?
                .GetMember(language.ToString())?
                .First()?
                .GetCustomAttribute<LanguageAttribute>()?
                .CultureInfo ?? throw new ArgumentNullException(nameof(language));

            return new CultureInfo(cultureInfo);
        }

        /// <summary>
        /// Converts the value of this instance to its equivalent string representation
        /// </summary>
        /// <param name="language"></param>
        /// <returns>The string representation of the value of this instance</returns>
        public static string GetDisplayName(this Language language)
        {
            return language
                .GetType()?
                .GetMember(language.ToString())?
                .First()?
                .GetCustomAttribute<DisplayAttribute>()?
                .Name ?? throw new ArgumentNullException(nameof(language));
        }

        /// <summary>
        /// Gets the emoji of this instance
        /// </summary>
        /// <param name="language"></param>
        /// <returns>The emoji of this instance</returns>
        public static string GetEmoji(this Language language)
        {
            return language
                .GetType()?
                .GetMember(language.ToString())?
                .First()?
                .GetCustomAttribute<LanguageAttribute>()?
                .Emoji ?? throw new ArgumentNullException(nameof(language));
        }

        /// <summary>
        /// Gets the shortcut value of this instance
        /// </summary>
        /// <param name="language"></param>
        /// <returns>The shortcut value of this instance</returns>
        public static string GetShortcutValue(this Language language)
        {
            return language
                .GetType()?
                .GetMember(language.ToString())?
                .First()?
                .GetCustomAttribute<LanguageAttribute>()?
                .Name ?? throw new ArgumentNullException(nameof(language));
        }
    }
}
