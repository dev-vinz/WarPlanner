using Discord;
using Discord.Interactions;
using Wp.Common.Models;
using Wp.Database.Services;

namespace Wp.Bot.Modules.ApplicationCommands.AutoCompletion
{
    public class CompetitionAutocompleteHandler : AutocompleteHandler
    {
        private static readonly int MAX_SUGGESTIONS = 25;

        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                          OVERRIDE METHODS                         *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

        public override Task<AutocompletionResult> GenerateSuggestionsAsync(IInteractionContext context, IAutocompleteInteraction autocompleteInteraction, IParameterInfo parameter, IServiceProvider services)
        {
            string predicate = autocompleteInteraction.Data.Current.Value as string ?? string.Empty;

            // Loads databases infos
            DbCompetitions competitions = Database.Context.Competitions;
            Guild dbGuild = Database.Context
                .Guilds
                .First(g => g.Id == context.Guild.Id);

            // Sorted competitions
            IEnumerable<Competition> dbCompetitions = competitions
                .Where(c => c.Guild == dbGuild);

            if (predicate != string.Empty)
            {
                dbCompetitions = dbCompetitions.Where(c => c.Name.Contains(predicate, StringComparison.InvariantCultureIgnoreCase));
            }

            IEnumerable<AutocompleteResult> results = dbCompetitions
                .AsParallel()
                .Select(c => new AutocompleteResult(c.Name, c.Id.ToString()));

            return Task.FromResult(
                AutocompletionResult.FromSuccess(
                        results
                            .OrderBy(r => r.Name)
                            .Take(MAX_SUGGESTIONS)
                    )
                );
        }
    }
}
