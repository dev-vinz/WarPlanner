using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Reflection;
using Wp.Database.Services.Attributes;

namespace Wp.Common.Models
{
    public enum Language
    {
        /// <summary>
        /// English language
        /// </summary>
        [Display(Name = "English")]
        [Language("en", "en-US")]
        ENGLISH = 0,

        /// <summary>
        /// French language
        /// </summary>
        [Display(Name = "Français")]
        [Language("fr", "fr-FR")]
        FRENCH = 1,

        /// <summary>
        /// Spanish language
        /// </summary>
        [Display(Name = "Español")]
        [Language("es", "es-ES")]
        SPANISH = 2,
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
