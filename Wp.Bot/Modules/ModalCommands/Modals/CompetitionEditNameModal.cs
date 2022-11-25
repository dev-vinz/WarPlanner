using Discord.Interactions;

namespace Wp.Bot.Modules.ModalCommands.Modals
{
    public class CompetitionEditNameModal : IModal
    {
        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                               FIELDS                              *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

        public const string ID = "competition_edit_name";
        public const string NAME_ID = "competition_name";

        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                             PROPERTIES                            *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

        public string Title => "Edit competition's name";

        [InputLabel("Competition's name")]
        [ModalTextInput(NAME_ID)]
        public string Name { get; set; } = null!;
    }
}
