using Discord;

namespace Wp.Discord
{
    public static class CustomEmojis
    {
        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                               FIELDS                              *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

        private static readonly string clanLegend = "<:clan_legend:1075493296249507890>";
        private static readonly string clanTitan = "<:clan_titan:1075493284576759808>";
        private static readonly string clanChampion = "<:clan_champion:1075493269271756900>";
        private static readonly string clanMaster = "<:clan_master:1075493257724825631>";
        private static readonly string clanCrystal = "<:clan_crystal:1075493231539781783>";
        private static readonly string clanGold = "<:clan_gold:1075493216884895784>";
        private static readonly string clanSilver = "<:clan_silver:1075493204360712333>";
        private static readonly string clanBronze = "<:clan_bronze:1075493185184346203>";

        private static readonly string townHall16 = "<:th16:1220669377028821116>";
        private static readonly string townHall15 = "<:th15:1075491829102952490>";
        private static readonly string townHall14 = "<:th14:1075491818462007387>";
        private static readonly string townHall13 = "<:th13:1075491806541791272>";
        private static readonly string townHall12 = "<:th12:1075491795791781938>";
        private static readonly string townHall11 = "<:th11:1075491784240668782>";
        private static readonly string townHall10 = "<:th10:1075491772412739604>";
        private static readonly string townHall09 = "<:th9:1075491760450568282>";
        private static readonly string townHall08 = "<:th8:1075491749901897738>";

        private static readonly string cocShield = "<:coc_shield:1075503671544123462>";
        private static readonly string cocStar = "<:coc_star:1079127614586900630>";
        private static readonly string profile = "<:profile:1075707302251860018>";
        private static readonly string warPlanner = "<:warplanner:1075503261907419187>";

        private static readonly string cocClock = "<a:coc_clock:1076472253962784769>";
        private static readonly string cocTrophy = "<a:coc_trophy:1075712817619009598>";
        private static readonly string warSwords = "<a:war_swords:1075707589247123487>";

        private static readonly string inStatus = "<a:out:1078231420109082634>";
        private static readonly string outStatus = "<a:in:1078231422516613131>";

        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                             PROPERTIES                            *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

        public static readonly Emote ClanLegend = Emote.Parse(clanLegend);
        public static readonly Emote ClanTitan = Emote.Parse(clanTitan);
        public static readonly Emote ClanMaster = Emote.Parse(clanMaster);
        public static readonly Emote ClanChampion = Emote.Parse(clanChampion);
        public static readonly Emote ClanCrystal = Emote.Parse(clanCrystal);
        public static readonly Emote ClanGold = Emote.Parse(clanGold);
        public static readonly Emote ClanSilver = Emote.Parse(clanSilver);
        public static readonly Emote ClanBronze = Emote.Parse(clanBronze);

        public static readonly Emote TownHall16 = Emote.Parse(townHall16);
        public static readonly Emote TownHall15 = Emote.Parse(townHall15);
        public static readonly Emote TownHall14 = Emote.Parse(townHall14);
        public static readonly Emote TownHall13 = Emote.Parse(townHall13);
        public static readonly Emote TownHall12 = Emote.Parse(townHall12);
        public static readonly Emote TownHall11 = Emote.Parse(townHall11);
        public static readonly Emote TownHall10 = Emote.Parse(townHall10);
        public static readonly Emote TownHall09 = Emote.Parse(townHall09);
        public static readonly Emote TownHall08 = Emote.Parse(townHall08);

        public static readonly Emote CocShield = Emote.Parse(cocShield);
        public static readonly Emote CocStar = Emote.Parse(cocStar);
        public static readonly Emote Profile = Emote.Parse(profile);
        public static readonly Emote WarPlanner = Emote.Parse(warPlanner);

        public static readonly Emote CocClock = Emote.Parse(cocClock);
        public static readonly Emote CocTrophy = Emote.Parse(cocTrophy);
        public static readonly Emote WarSwords = Emote.Parse(warSwords);

        public static readonly Emote InStatus = Emote.Parse(inStatus);
        public static readonly Emote OutStatus = Emote.Parse(outStatus);

        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                           PUBLIC METHODS                          *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

        /// <summary>
        /// Parses an Emote from it's clan level
        /// </summary>
        /// <param name="clanLevel">A Clash of Clans clan's level</param>
        /// <returns>An emote</returns>
        public static Emote ParseClanLevel(int clanLevel)
        {
            return clanLevel switch
            {
                1 or 2 or 3 or 4 => ClanBronze,
                5 or 6 or 7 or 8 or 9 => ClanSilver,
                10 or 11 => ClanGold,
                12 or 13 => ClanCrystal,
                14 or 15 => ClanMaster,
                16 or 17 => ClanChampion,
                18 or 19 => ClanTitan,
                _ => ClanLegend,
            };
        }

        /// <summary>
        /// Parses an Emote from it's town hall level
        /// </summary>
        /// <param name="townHallLevel">A Clash of Clans town hall's level</param>
        /// <returns>An emote</returns>
        public static Emote ParseTownHallLevel(int townHallLevel)
        {
            return townHallLevel switch
            {
                16 => TownHall16,
                15 => TownHall15,
                14 => TownHall14,
                13 => TownHall13,
                12 => TownHall12,
                11 => TownHall11,
                10 => TownHall10,
                9 => TownHall09,
                8 => TownHall08,
                _ => CocShield
            };
        }
    }
}
