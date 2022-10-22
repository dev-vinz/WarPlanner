using System.ComponentModel.DataAnnotations;
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
        [Language("en")]
        ENGLISH = 0,

        /// <summary>
        /// French language
        /// </summary>
        [Display(Name = "Français")]
        [Language("fr")]
        FRENCH = 1,

        /// <summary>
        /// Spanish language
        /// </summary>
        [Display(Name = "Español")]
        [Language("es")]
        SPANISH = 2,
    }

    public static class LanguageExtensions
    {
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
