using Discord;
using Discord.Interactions;
using System.Globalization;
using Wp.Api;
using Wp.Common.Models;
using Wp.Common.Services.NodaTime;
using Calendar = Wp.Common.Models.Calendar;

namespace Wp.Bot.Modules.ApplicationCommands.AutoCompletion
{
	public class WarAutocompleteHandler : AutocompleteHandler
	{
		private static readonly int MAX_SUGGESTIONS = 25;

		/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                          OVERRIDE METHODS                         *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

		public override async Task<AutocompletionResult> GenerateSuggestionsAsync(IInteractionContext context, IAutocompleteInteraction autocompleteInteraction, IParameterInfo parameter, IServiceProvider services)
		{
			string predicate = autocompleteInteraction.Data.Current.Value as string ?? string.Empty;

			// Loads databases infos
			Guild dbGuild = Database.Context
				.Guilds
				.First(g => g.Id == context.Guild.Id);

			Calendar? dbCalendar = Database.Context
				.Calendars
				.FirstOrDefault(c => c.Guild == dbGuild);

			if (dbCalendar == null) return AutocompletionResult.FromError(InteractionCommandError.Exception, "TODO: No calendar registered");

			// Loads calendar event
			Api.Models.CalendarEvent[] allEvents = await GoogleCalendarApi.Events.ListAsync(dbCalendar.Id);

			if (!allEvents.Any()) return AutocompletionResult.FromSuccess();

			// Instances our time converter
			NodaConverter nodaConverter = new();
			CultureInfo cultureInfo = dbGuild.CultureInfo;

			IEnumerable<AutocompleteResult> results = allEvents
				.OrderBy(e => e.Start)
				.Select(e => new AutocompleteResult(dbGuild.ManagerText.WarEditAutocompletion(e.Start.ToString("dd/MM, HH:mm", cultureInfo), DateTimeOffset.FromUnixTimeSeconds((long)(e.End - e.Start).TotalSeconds).ToString("HH:mm", cultureInfo), e.OpponentClan?.Name ?? e.OpponentTag), e.Id));

			if (predicate != string.Empty)
			{
				results = results.Where(r => r.Name.Contains(predicate, StringComparison.InvariantCultureIgnoreCase));
			}

			return AutocompletionResult.FromSuccess(
						results.Take(MAX_SUGGESTIONS)
					);
		}
	}
}
