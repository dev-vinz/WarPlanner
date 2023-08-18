using Discord;

namespace Wp.Discord.Extensions
{
    public static class EmbedBuilderExtensions
    {
        public static EmbedBuilder WithRandomColor(this EmbedBuilder builder)
        {
            return builder.WithColor(new Color((uint)new Random().Next(0x1000000)));
        }
    }
}
