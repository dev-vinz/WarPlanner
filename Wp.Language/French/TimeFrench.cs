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

		public string WarRemindApiOffline(string player, string competition, string remaining) => $"Hey {player} !" +
			$"\nJe ne peux actuellement pas vérifier si tu es dans le clan, mais dans le doute, n'oublie pas ton match en **{competition}**" +
			$"\nIl commence dans {remaining}";

		public string WarRemindWarnPlayer(string player, string clan, string competition, string opponent, string start, string remaining) => $"Hey {player}, n'oublie pas de rejoindre **{clan}** pour ton match en `{competition}` contre **{opponent}** !" +
			$"\nÇa commence à **{start}**, il ne reste plus que {remaining}";
	}
}
