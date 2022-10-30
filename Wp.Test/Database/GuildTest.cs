using Microsoft.VisualStudio.TestTools.UnitTesting;
using Wp.Common.Models;
using Wp.Database.Services;
using DB = Wp.Database.Context;

namespace Wp.Test.Database
{
	[TestClass]
	public class GuildTest
	{
		[TestMethod]
		public void TestAdd()
		{
			Guild guild = new(1, Common.Models.TimeZone.GMT);

			DB.Guilds.Add(guild);

			Assert.IsTrue(DB.Guilds.Contains(guild));
		}

		[TestMethod]
		public void TestUpdate()
		{
			DbGuilds guilds = DB.Guilds;
			Guild guild = guilds.First(g => g.Id == 1);

			guild.MinimalTownHallLevel = 12;
			guild.PremiumLevel = PremiumLevel.MEDIUM;

			guilds.Update(guild);

			Guild newGuild = DB.Guilds.First(g => g.Id == guild.Id);

			Assert.AreEqual(guild.MinimalTownHallLevel, newGuild.MinimalTownHallLevel);
			Assert.AreEqual(guild.PremiumLevel, newGuild.PremiumLevel);
			Assert.IsTrue(guilds.Contains(newGuild));
		}

		[TestMethod]
		public void TestRemove()
		{
			Guild guild = new(1, Common.Models.TimeZone.GMT);

			Assert.IsTrue(DB.Guilds.Remove(g => g.Id == guild.Id));
			Assert.IsFalse(DB.Guilds.Contains(guild));
		}
	}
}
