using Discord;
using Discord.Interactions;
using System.Globalization;
using Wp.Common.Settings;

namespace Wp.Bot.Modules.ApplicationCommands.AutoCompletion
{
    public class WarDateAutocompleteHandler : AutocompleteHandler
    {
        private static readonly int MAX_SUGGESTIONS = 25;

        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                          OVERRIDE METHODS                         *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

        public override Task<AutocompletionResult> GenerateSuggestionsAsync(IInteractionContext context, IAutocompleteInteraction autocompleteInteraction, IParameterInfo parameter, IServiceProvider services)
        {
            //// Loads databases infos
            //Guild dbGuild = Database.Context
            //    .Guilds
            //    .First(g => g.Id == context.Guild.Id);

            //// Gets date and culture info
            //DateTimeOffset today = new(dbGuild.Now.Date, dbGuild.Now.Offset);
            //CultureInfo cultureInfo = dbGuild.CultureInfo;

            // Gets date and culture info
            DateTimeOffset today = DateTimeOffset.UtcNow.Date;
            CultureInfo cultureInfo = new("fr-FR");

            List<AutocompleteResult> results = new();

            Enumerable.Range(0, Settings.NB_WAR_PROPOSED_DATES)
                .ToList()
                .ForEach(n => results.Add(new AutocompleteResult(today.AddDays(n).ToString("D", cultureInfo), today.AddDays(n).ToString())));

            return Task.FromResult(
                AutocompletionResult.FromSuccess(
                        results.Take(MAX_SUGGESTIONS)
                    )
                );
        }
    }
}