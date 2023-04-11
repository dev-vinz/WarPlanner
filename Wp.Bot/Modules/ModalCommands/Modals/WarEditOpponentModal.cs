using Discord.Interactions;

namespace Wp.Bot.Modules.ModalCommands.Modals
{
	public class WarEditOpponentModal : IModal
	{
		/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                               FIELDS                              *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

		public const string ID = "war_edit_opponent";
		public const string OPPONENT_TAG_ID = "opponent_tag";

		/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                             PROPERTIES                            *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

		public string Title => "Edit war's opponent";

		[InputLabel("Opponent Clash Of Clans tag")]
		[ModalTextInput(OPPONENT_TAG_ID)]
		public string Tag { get; set; } = null!;
	}
}
