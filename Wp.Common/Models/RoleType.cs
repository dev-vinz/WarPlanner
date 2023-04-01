namespace Wp.Common.Models
{
	public enum RoleType
	{
		/// <summary>
		/// Allows to execute owner's commands
		/// </summary>
		OWNER = -2,
		ADMINISTRATOR = -1,

		/// <summary>
		/// Allows to execute manager's commands
		/// </summary>
		MANAGER = 0,

		/// <summary>
		/// Allows to execute player's commands
		/// </summary>
		PLAYER = 1,
	}
}
