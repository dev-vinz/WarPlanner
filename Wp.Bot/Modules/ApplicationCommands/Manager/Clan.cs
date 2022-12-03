using Discord;
using Discord.Interactions;
using Wp.Api;
using Wp.Bot.Services;
using Wp.Common.Models;
using Wp.Database.Services;
using Wp.Language;

namespace Wp.Bot.Modules.ApplicationCommands.Manager
{
	[Group("clan", "Clan commands handler")]
	public class Clan : InteractionModuleBase<SocketInteractionContext>
	{
		/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                               FIELDS                              *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

		private readonly CommandHandler handler;

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

		public Clan(CommandHandler handler)
		{
			this.handler = handler;
		}

		/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                          ABSTRACT METHODS                         *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */



		/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                           PUBLIC METHODS                          *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

		[SlashCommand("add", "Register a new clan within the guild", runMode: RunMode.Async)]
		public async Task Add([Summary("tag", "Clash Of Clans clan's tag")] string tag)
		{
			await DeferAsync(true);

			// Loads databases infos
			DbClans clans = Database.Context.Clans;
			Guild dbGuild = Database.Context
				.Guilds
				.First(g => g.Id == Context.Guild.Id);

			// Filters for guild
			Common.Models.Clan[] dbClans = clans
				.AsParallel()
				.Where(c => c.Guild == dbGuild)
				.ToArray();

			// Gets general responses
			IGeneralResponse generalResponses = dbGuild.GeneralResponses;

			ClashOfClans.Models.Clan? cClan = await ClashOfClansApi.Clans.GetByTagAsync(tag);

			if (cClan is null)
			{
				await ModifyOriginalResponseAsync(msg => msg.Content = generalResponses.ClashOfClansError);

				return;
			}

			// Gets command responses
			IManager commandText = dbGuild.ManagerText;

			if (dbClans.Any(c => c.Tag == cClan.Tag))
			{
				await ModifyOriginalResponseAsync(msg => msg.Content = commandText.ClanAddAlreadyRegistered(cClan.Name));

				return;
			}

			Common.Models.Clan dbClan = new(dbGuild, cClan.Tag);

			// Register clan within guild
			clans.Add(dbClan);

			await ModifyOriginalResponseAsync(msg => msg.Content = commandText.ClanAddRegistered(cClan.Name));
		}

		[SlashCommand("delete", "Delete a registered clan from the guild", runMode: RunMode.Async)]
		public async Task Delete()
		{
			await DeferAsync(true);

			// Loads databases infos
			DbClans clans = Database.Context.Clans;
			Guild dbGuild = Database.Context
				.Guilds
				.First(g => g.Id == Context.Guild.Id);

			// Filters for guild
			Common.Models.Clan[] dbClans = clans
				.AsParallel()
				.Where(c => c.Guild == dbGuild)
				.ToArray();

			Common.Models.Competition[] dbCompetitions = Database.Context
				.Competitions
				.AsParallel()
				.Where(c => c.Guild == dbGuild)
				.ToArray();

			// Gets command responses
			IManager commandText = dbGuild.ManagerText;

			if (!dbClans.Any())
			{
				await ModifyOriginalResponseAsync(msg => msg.Content = commandText.NoClanRegisteredToRemove);

				return;
			}

			// Build select menu
			SelectMenuBuilder menuBuilder = new SelectMenuBuilder()
				.WithCustomId(IdProvider.CLAN_REMOVE_SELECT_MENU);

			dbClans
				.AsParallel()
				.ForAll(c =>
				{
					if (!dbCompetitions.AsParallel().Any(comp => comp.MainTag == c.Tag || comp.SecondTag == c.Tag))
					{
						menuBuilder.AddOption(c.Profile.Name, c.Tag, c.Tag);
					}
				});

			if (menuBuilder.Options.Count < 1)
			{
				await ModifyOriginalResponseAsync(msg => msg.Content = commandText.ClanCannotRemovedBecauseCurrentlyUsed);

				return;
			}

			menuBuilder.Options = menuBuilder.Options
				.AsParallel()
				.OrderBy(o => o.Label)
				.ToList();

			ComponentBuilder componentBuilder = new ComponentBuilder()
				.WithSelectMenu(menuBuilder);

			IUserMessage message = await ModifyOriginalResponseAsync(msg =>
			{
				msg.Content = commandText.SelectClanToRemove;
				msg.Components = new(componentBuilder.Build());
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
