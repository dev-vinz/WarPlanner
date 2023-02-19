namespace Wp.Language.French
{
	public class TimeFrench : ITime
	{
		/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                       CALENDAR DISPLAY EVENT                      *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

		public string CalendarDisplayCannotSend => "Le salon servant à afficher le calendrier a été supprimé";

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
	}
}
