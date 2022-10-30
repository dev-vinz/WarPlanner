using ClashOfClans;
using ClashOfClans.Core;
using ClashOfClans.Models;
using Wp.Api.Settings;

namespace Wp.Api
{
    public static class ClashOfClansApi
    {
        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                               FIELDS                              *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

        private static readonly ClashOfClansClient client;

        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                             PROPERTIES                            *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

        public static ClashOfClansError Error { get; set; }

        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                            CONSTRUCTORS                           *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

        static ClashOfClansApi()
        {
            client = new ClashOfClansClient(Keys.CLASH_OF_CLANS_TOKEN);

            Error = ClashOfClansError.NOT_FOUND;
        }

        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                            CLAN METHODS                           *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

        public static class Clans
        {
            public static async Task<Clan?> GetByTagAsync(string tag)
            {
                Clan? clan = null;

                if (!string.IsNullOrWhiteSpace(tag))
                {
                    try
                    {
                        if (!tag.StartsWith('#')) tag = $"#{tag}";

                        clan = await client.Clans.GetClanAsync(tag);
                    }
                    catch (ClashOfClansException)
                    {
                    }
                }

                return clan;
            }

            public static async Task<ClanWarLeagueGroup?> GetWarLeagueGroupAsync(string tag)
            {
                ClanWarLeagueGroup? group = null;

                if (!string.IsNullOrWhiteSpace(tag))
                {
                    try
                    {
                        if (!tag.StartsWith('#')) tag = $"#{tag}";

                        group = await client.Clans.GetClanWarLeagueGroupAsync(tag);
                    }
                    catch (ClashOfClansException)
                    {
                    }
                }

                return group;
            }

            public static async Task<ClanWar?> GetCurrentWarAsync(string tag)
            {
                ClanWar? war = null;

                if (!string.IsNullOrEmpty(tag))
                {
                    try
                    {
                        if (!tag.StartsWith('#')) tag = $"#{tag}";

                        war = await client.Clans.GetCurrentWarAsync(tag);
                    }
                    catch (ClashOfClansException)
                    {
                    }
                }

                return war;
            }
        }


        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                           PLAYER METHODS                          *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

        public static class Players
        {
            public static async Task<Player?> GetByTagAsync(string tag)
            {
                Player? player = null;

                if (!string.IsNullOrWhiteSpace(tag))
                {
                    try
                    {
                        if (!tag.StartsWith('#')) tag = $"#{tag}";

                        player = await client.Players.GetPlayerAsync(tag.ToUpper().Replace('O', '0'));
                    }
                    catch (ClashOfClansException)
                    {
                    }
                }

                return player;
            }

            public static async Task<VerifyTokenResponse?> VerifyTokenAsync(string tag, string token)
            {
                VerifyTokenResponse? verifyResponse = null;

                if (!string.IsNullOrWhiteSpace(tag) && !string.IsNullOrWhiteSpace(token))
                {
                    try
                    {
                        if (!tag.StartsWith('#')) tag = $"#{tag}";

                        verifyResponse = await client.Players.VerifyTokenAsync(tag, token);
                    }
                    catch (ClashOfClansException)
                    {
                    }
                }

                return verifyResponse;
            }
        }
    }
}
