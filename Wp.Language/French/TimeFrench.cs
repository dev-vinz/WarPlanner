﻿namespace Wp.Language.French
{
    public class TimeFrench : ITime
    {
        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                       CALENDAR DISPLAY EVENT                      *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

        public string CalendarDisplayCannotSend => "Le salon servant à afficher le calendrier a été supprimé";

        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                          GUILD JOIN EVENT                         *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

        public string GuildJoinedTitle => "Merci de m'avoir ajouté !";
        public string GuildJoinedDescription(string username) => $"Hello **{username}**, merci de m'avoir ajouté à votre serveur";
        public string GuildJoinedFieldTimeZoneTitle => "Fuseau Horaire";
        public string GuildJoinedFieldTimeZoneDescription(string timeZone, double offset) => $"`{timeZone}` : UTC{offset:+#;-#;+0}" +
            $"\n*Pour le changer, exécutez la commande `/timezone`*";

        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                          WAR REMIND EVENT                         *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

        public string WarRemindManagerApiOffline(string competition, string remaining) => $"Je ne peux actuellement pas vérifier si les joueur–se–s sont présent–e–s dans le clan pour le match en **{competition}**" +
            $"\nPense à le vérifier manuellement quand tu en auras la possibilité" +
            $"\nLe match commence dans {remaining}";

        public string WarRemindPlayerApiOffline(string player, string competition, string remaining) => $"Hey {player} !" +
            $"\nJe ne peux actuellement pas vérifier si tu es dans le clan, mais dans le doute, n'oublie pas ton match en **{competition}**" +
            $"\nIl commence dans {remaining}";

        public string WarRemindWarnManager(string players, string clan, string competition, string remaining) => $"Les joueur–se–s suivant–e–s ne se trouvent pas dans le clan *{clan}* pour le match en **{competition}**" +
            $"\n\n{players}" +
            $"\n\nIl reste {remaining}";

        public string WarRemindWarnPlayer(string player, string clan, string competition, string opponent, string start, string remaining) => $"Hey {player}, n'oublie pas de rejoindre **{clan}** pour ton match en `{competition}` contre **{opponent}** !" +
            $"\nÇa commence à **{start}**, il ne reste plus que {remaining}";

        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                      WAR REMIND STATUS EVENT                      *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

        public string WarRemindStatusManagerApiOffline(string competition) => $"Je ne peux actuellement pas vérifier les préférences de guerre des joueur–se–s" +
            $"\nFais attention au prochain lancement de guerre de clan, vous avez un match en **{competition}** dans 2 jours";

        public string WarRemindStatusPlayerApiOffline(string player, string competition) => $"Hey {player} !" +
            $"\nJe ne peux actuellement pas vérifier ta préférence de guerre, mais n'oublie pas te mettre en rouge pour ton match en **{competition}** dans 2 jours";

        public string WarRemindStatusWarnManager(string players, string competition) => $"Il y a un match en **{competition}** dans 2 jours, alors ne remets pas ces joueurs en guerre d'ici-là" +
            $"\n{players}";

        public string WarRemindStatusWarnPlayer(string player, string competition) => $"Hey {player}, n'oublie pas que dans 2 jours tu as un match en **{competition}**, alors passe en rouge !";
    }
}
