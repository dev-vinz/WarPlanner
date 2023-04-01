namespace Wp.Common.Models
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
		RUNNING_OVER = 1,
	}
}
