using Discord;
using Discord.Interactions;
using Wp.Language;
using Wp.Language.French;

namespace Wp.Bot.Modules.ApplicationCommands.AutoCompletion
{
    public class WarPreparationAutocompleteHandler : AutocompleteHandler
    {
        private static readonly int MAX_SUGGESTIONS = 25;

        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                          OVERRIDE METHODS                         *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

        public override Task<AutocompletionResult> GenerateSuggestionsAsync(IInteractionContext context, IAutocompleteInteraction autocompleteInteraction, IParameterInfo parameter, IServiceProvider services)
        {
            //// Loads databases infos
            //Guild dbGuild = Database.Context
            //	.Guilds
            //	.First(g => g.Id == context.Guild.Id);

            // Gets general responses
            //IGeneralResponse generalResponses = dbGuild.GeneralResponses;
            IGeneralResponse generalResponses = new FrenchGeneralResponse();

            IEnumerable<AutocompleteResult> results = new List<AutocompleteResult>()
            {
                new AutocompleteResult($"5 {generalResponses.Minutes}", 5),
                new AutocompleteResult($"15 {generalResponses.Minutes}", 15),
                new AutocompleteResult($"30 {generalResponses.Minutes}", 30),
                new AutocompleteResult($"1 {generalResponses.Hour}", 60),
                new AutocompleteResult($"2 {generalResponses.Hours}", 120),
                new AutocompleteResult($"4 {generalResponses.Hours}", 240),
                new AutocompleteResult($"6 {generalResponses.Hours}", 360),
                new AutocompleteResult($"8 {generalResponses.Hours}", 480),
                new AutocompleteResult($"12 {generalResponses.Hours}", 720),
                new AutocompleteResult($"16 {generalResponses.Hours}", 960),
                new AutocompleteResult($"20 {generalResponses.Hours}", 1200),
                new AutocompleteResult($"1 {generalResponses.Day}", 1440),
            };

            return Task.FromResult(
                AutocompletionResult.FromSuccess(
                        results.Take(MAX_SUGGESTIONS)
                    )
                );
        }
    }
}
