using Discord;
using Discord.Interactions;
using Discord.Rest;
using Wp.Common.Models;
using Wp.Common.Settings;
using Wp.Discord.Extensions;
using Wp.Language;
using EventHandler = Wp.Bot.Services.EventHandler;

namespace Wp.Bot.Modules.ApplicationCommands.Global
{
    [RequireContext(ContextType.Guild)]
    public class Global : InteractionModuleBase<SocketInteractionContext>
    {
        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                               FIELDS                              *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

        private readonly EventHandler handler;

        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                             PROPERTIES                            *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

        public InteractionService? Commands { get; set; }

        /* * * * * * * * * * * * * * * * * *\
        |*            SHORTCUTS            *|
        \* * * * * * * * * * * * * * * * * */



        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                            CONSTRUCTORS                           *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

        public Global(EventHandler handler)
        {
            this.handler = handler;
        }

        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                          ABSTRACT METHODS                         *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */



        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                           PUBLIC METHODS                          *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

        [SlashCommand("infos", "Get some informations about the bot, and the links")]
        public async Task Informations()
        {
            await DeferAsync(true);

            // Loads databases infos
            Guild dbGuild = Database.Context
                .Guilds
                .First(g => g.Id == Context.Guild.Id);

            // Gets responses
            IGeneralResponse generalResponses = dbGuild.GeneralResponses;
            IGlobal commandText = dbGuild.GlobalText;

            // Documentation button
            ButtonBuilder docButtonBuilder = new ButtonBuilder()
                .WithLabel(generalResponses.Documentation)
                .WithStyle(ButtonStyle.Link)
                .WithUrl(Utilities.GITBOOK_DOCUMENTATION)
                .WithEmote(new Emoji("📚"));

            // Support button
            ButtonBuilder supportButtonBuilder = new ButtonBuilder()
                .WithLabel(generalResponses.SupportServer)
                .WithStyle(ButtonStyle.Link)
                .WithUrl(Utilities.SUPPORT_GUILD_INVITATION)
                .WithEmote(new Emoji("⚙️"));

            // Link button
            ButtonBuilder linkButtonBuilder = new ButtonBuilder()
                .WithLabel(generalResponses.LinkInvitation)
                .WithStyle(ButtonStyle.Link)
                .WithUrl(Utilities.BOT_LINK_INVITATION)
                .WithEmote(new Emoji("🔗"));

            // Build component
            ComponentBuilder componentBuilder = new ComponentBuilder()
                .WithButton(supportButtonBuilder)
                .WithButton(docButtonBuilder)
                .WithButton(linkButtonBuilder);

            RestApplication application = await Context.Client.GetApplicationInfoAsync();
            TimeSpan uptime = handler.Uptime;

            string fullUptime = $"{uptime.Days:00}.{uptime.Hours:00}:{uptime.Minutes:00}:{uptime.Seconds:00}";

            // Embed
            EmbedBuilder embedBuilder = new EmbedBuilder()
                .WithTitle(commandText.EmbedInformations)
                .WithThumbnailUrl(application.IconUrl)
                .WithDescription(commandText.EmbedDescription)
                .AddField(commandText.EmbedFieldLastConnection, fullUptime, false)
                .AddField(commandText.EmbedFieldAuthor, application.Owner.Mention, true)
                .WithRandomColor()
                .WithFooter($"{dbGuild.Now.Year} © {Context.Client.CurrentUser.Username}");

            await ModifyOriginalResponseAsync(msg =>
            {
                msg.Components = new(componentBuilder.Build());
                msg.Embed = embedBuilder.Build();
            });
        }

        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                         PROTECTED METHODS                         *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */



        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                          PRIVATE METHODS                          *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */



        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                          OVERRIDE METHODS                         *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */



        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                           STATIC METHODS                          *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */



        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                              INDEXERS                             *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */



        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                         OPERATORS OVERLOAD                        *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */



    }
}
