using Discord.Interactions;

namespace Wp.Bot.Modules.ModalCommands.Modals
{
    public class PlayerClaimModal : IModal
    {
        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                               FIELDS                              *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

        public const string ID = "claim_player_account";
        public const string TAG_ID = "claim_tag";
        public const string TOKEN_ID = "claim_token";

        public const int MIN_TOKEN_LENGTH = 8;
        public const int MAX_TOKEN_LENGTH = 8;

        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                             PROPERTIES                            *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

        public string Title => "Link your account";

        [InputLabel("Clash Of Clans account's tag")]
        [ModalTextInput(TAG_ID, placeholder: "#ABCD1234")]
        public string Tag { get; set; } = null!;

        [InputLabel("Clash Of Clans account's token")]
        [ModalTextInput(TOKEN_ID, placeholder: "xh35nbdrj", minLength: MIN_TOKEN_LENGTH, maxLength: MAX_TOKEN_LENGTH)]
        public string Token { get; set; } = null!;
    }
}
