namespace Wp.Database.Models
{
    public enum PlayerStatisticAction
    {
        /// <summary>
        /// Attack or defense when the base hasn't been attacked yet
        /// </summary>
        OPENING = 0,

        /// <summary>
        /// Attack or defense when the base has already been attacked
        /// </summary>
        REUNNING_OVER = 1,
    }
}
