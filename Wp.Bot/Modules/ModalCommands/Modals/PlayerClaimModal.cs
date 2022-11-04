using Discord.Interactions;

namespace Wp.Bot.Modules.ModalCommands.Modals
{
    public class PlayerClaimModal : IModal
    {
        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                               FIELDS                              *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

        public const string ID = "claim_player_account";

        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                             PROPERTIES                            *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

        public string Title => "Link your account";

        [InputLabel("Clash Of Clans account's tag")]
        [ModalTextInput("claim_tag", placeholder: "#ABCD1234")]
        public string Tag { get; set; } = null!;

        [InputLabel("Clash Of Clans account's token")]
        [ModalTextInput("claim_token", placeholder: "xh35nbdrj", minLength: 8, maxLength: 8)]
        public string Token { get; set; } = null!;
    }
}
