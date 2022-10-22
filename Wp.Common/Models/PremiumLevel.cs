using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace Wp.Database.Models
{
    public enum PremiumLevel
    {
        /// <summary>
        /// No advantages
        /// </summary>
        [Display(Name = "None")]
        NONE = 0,

        /// <summary>
        /// Advantages for Low level
        /// </summary>
        [Display(Name = "Low")]
        LOW = 1,

        /// <summary>
        /// Advantages for Low and Medium levels
        /// </summary>
        [Display(Name = "Medium")]
        MEDIUM = 2,

        /// <summary>
        /// Advantages for Low, Medium and High levels
        /// </summary>
        [Display(Name = "High")]
        HIGH = 3,
    }

    public static class PremiumLevelExtensions
    {
        /// <summary>
        /// Converts the value of this instance to its equivalent string representation
        /// </summary>
        /// <param name="premiumLevel"></param>
        /// <returns>The string representation of the value of this instance</returns>
        public static string GetDisplayName(this PremiumLevel premiumLevel)
        {
            return premiumLevel
                .GetType()?
                .GetMember(premiumLevel.ToString())?
                .First()?
                .GetCustomAttribute<DisplayAttribute>()?
                .Name ?? throw new ArgumentNullException(nameof(premiumLevel));
        }
    }
}
