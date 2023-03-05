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

		private static readonly string OWNER_TAG = "#J08PCCUQ";

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
        |*                           STATIC METHODS                          *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

		/// <summary>
		/// Tries to access the api, and change the error
		/// </summary>
		/// <returns>True if the request passed; False otherwise</returns>
		public static async Task<bool> TryAccessApiAsync()
		{
			try
			{
				// Assumption : This tag (my account) exists forever
				await client.Players.GetPlayerAsync(OWNER_TAG);

				// Default message error
				Error = ClashOfClansError.NOT_FOUND;

				return true;
			}
			catch (ClashOfClansException exception)
			{
				Error = exception.Error.Reason switch
				{
					"inMaintenance" => ClashOfClansError.IN_MAINTENANCE,
					"accessDenied" => ClashOfClansError.INVALID_IP,
					_ => ClashOfClansError.UNKNOWN_ERROR,
				};

				return false;
			}
		}

		/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                            CLAN METHODS                           *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

		/// <summary>
		/// Accesses to clans methods
		/// </summary>
		public static class Clans
		{
			/// <summary>
			/// Searches a clan by its tag
			/// </summary>
			/// <param name="tag">A Clash of Clans tag</param>
			/// <returns>A Clash of Clans clan</returns>
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

			/// <summary>
			/// Searches a clan war league group from a clan
			/// </summary>
			/// <param name="tag">A Clash of Clans tag</param>
			/// <returns>A Clash of Clans clan war league group</returns>
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

			/// <summary>
			/// Gets the current war of a clan
			/// </summary>
			/// <param name="tag">A Clash of Clans tag</param>
			/// <returns>A Clash of Clans war</returns>
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

			public static async Task<ClanWarLog?> GetWarLogAsync(string tag)
			{
				ClanWarLog? warLog = null;

				if (!string.IsNullOrEmpty(tag))
				{
					try
					{
						if (!tag.StartsWith('#')) tag = $"{tag}";

						ClashOfClans.Search.QueryResult<ClanWarLog> query = await client.Clans.GetClanWarLogAsync(tag);
						warLog = query.Items;
					}
					catch (ClashOfClansException)
					{
					}
				}

				return warLog;
			}
		}

		/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                           PLAYER METHODS                          *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

		/// <summary>
		/// Accesses to players methods
		/// </summary>
		public static class Players
		{
			/// <summary>
			/// Searchss a player account by its tag
			/// </summary>
			/// <param name="tag">A Clash of Clans tag</param>
			/// <returns>A Clash of Clans account</returns>
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

			/// <summary>
			/// Verifies an account by checking if a secret token corresponds to the tag
			/// </summary>
			/// <param name="tag">A Clash of Clans tag</param>
			/// <param name="token">A Clash of Clans secret token</param>
			/// <returns>A verify response sent by the API</returns>
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