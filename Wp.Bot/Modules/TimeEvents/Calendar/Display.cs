using Discord;
using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System.Globalization;
using Wp.Api;
using Wp.Api.Models;
using Wp.Common.Models;
using Wp.Common.Services.Extensions;
using Wp.Common.Services.NodaTime;
using Wp.Database;
using Wp.Database.Services;
using Wp.Language;
using Color = SixLabors.ImageSharp.Color;
using Image = SixLabors.ImageSharp.Image;

namespace Wp.Bot.Modules.TimeEvents.Calendar
{
	public static class Display
	{
		/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
		|*                               FIELDS                              *|
		\* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

		private static readonly int CANVAS_WIDTH = 800;
		private static readonly int CANVAS_HEIGHT = 300;

		/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
		|*                           PUBLIC METHODS                          *|
		\* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

		public static void Execute(IGuild guild)
		{
			// Loads databases infos
			DbCalendars calendars = Context.Calendars;
			DbTimes times = Context.Times;
			Guild dbGuild = Context
				.Guilds
				.First(g => g.Id == guild.Id);

			// Filters for guild
			Common.Models.Calendar? dbCalendar = calendars
				.AsParallel()
				.FirstOrDefault(c => c.Guild == dbGuild);

			Time? dbTime = times
				.AsParallel()
				.FirstOrDefault(t => t.Guild == dbGuild && t.Action == TimeAction.DISPLAY_CALENDAR);

			// Makes some verifications
			if (dbCalendar == null || dbTime == null) return;

			if (!dbTime.IsScanAllowed()) return;

			// Update time
			DateTimeOffset utcNow = DateTimeOffset.UtcNow;

			dbTime.Date = utcNow;
			times.Update(dbTime);

			// Gets display informations
			int timesPerDay = int.Parse(dbTime.Optional!);
			double hoursToAdd = 24.0 / timesPerDay;

			DateTimeOffset guildNow = dbGuild.Now.TruncSeconds();
			DateTimeOffset guildMidnight = new(guildNow.Date, guildNow.Offset);

			// Checks each interval
			Enumerable.Range(0, timesPerDay)
				.Select(k => guildMidnight.AddHours(k * hoursToAdd))
				.ToList()
				.ForEach(async date =>
				{
					DateTimeOffset limit = date.AddSeconds(1);

					if (guildNow.IsBetweenSeconds(date, limit))
					{
						await DisplayAsync(guild, dbCalendar);
					}
				});
		}

		/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
		|*                          PRIVATE METHODS                          *|
		\* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

		private static Dictionary<string, Stream> CreateCanvas(string calendarId, Guild guild)
		{
			// Initializes settings
			Dictionary<string, Stream> dictionary = new();
			NodaConverter converter = new();

			// Gets all events during the week
			List<CalendarEvent>[] currentWeek = GetCalendarInformations(calendarId, guild.TimeZone, out int nbImages);

			int totalEvents = currentWeek
				.Select(events => events.Count == 0 ? 1 : events.Count)
				.Sum();

			// Other settings
			int lineWidth = 1;

			int intervalWidth = (CANVAS_WIDTH - 2 * lineWidth) / totalEvents;
			int intervalHeight = (CANVAS_HEIGHT - 2 * lineWidth) / 10;

			IPen blackPen = new Pen(Color.Black, lineWidth);
			Color darkOrange = Color.FromRgb(246, 178, 107);
			Color lightOrange = Color.FromRgb(252, 229, 205);

			FontFamily fontFamily = SystemFonts.Collection.Get("Quicksand SemiBold");

			CultureInfo cultureInfo = guild.Language.GetCultureInfo();

			// Gets general response
			IGeneralResponse generalResponses = guild.GeneralResponses;
			string[] tabDays =
			{
				generalResponses.Sunday,
				generalResponses.Monday,
				generalResponses.Tuesday,
				generalResponses.Wednesday,
				generalResponses.Thursday,
				generalResponses.Friday,
				generalResponses.Saturday
			};

			#region Drawing
			for (int k = 0; k < nbImages; k++)
			{
				using Image img = new Image<Rgba32>(CANVAS_WIDTH, CANVAS_HEIGHT);

				img.Mutate(ctx =>
				{
					int count = 0;
					int color = 0;

					bool firstImage = k == 0;

					#region Draw Background
					for (int i = lineWidth; i < CANVAS_WIDTH - intervalWidth; i += intervalWidth)
					{
						int multiplicator = currentWeek.ElementAt(count).Count == 0 ? 1 : currentWeek.ElementAt(count).Count;

						Color fillColor = color++ % 2 == 0 ? darkOrange : lightOrange;
						RectangleF rectBackground = new(i, lineWidth, (i + intervalWidth) * multiplicator, CANVAS_HEIGHT - lineWidth);

						ctx.Fill(new SolidBrush(fillColor), rectBackground);

						if (currentWeek.ElementAt(count).Count > 1)
							i += intervalWidth * (currentWeek.ElementAt(count).Count - 1);

						count++;
					}

					if (firstImage)
					{
						Color greyColor = Color.FromRgb(217, 217, 217);
						RectangleF rectFirst = new(lineWidth, 2 * intervalHeight, CANVAS_WIDTH - lineWidth, 3 * intervalHeight);

						ctx.Fill(new SolidBrush(greyColor), rectFirst);
					}
					#endregion

					RectangleF rect = new(lineWidth, lineWidth, CANVAS_WIDTH - 2 * lineWidth, CANVAS_HEIGHT - 2 * lineWidth);
					ctx.Draw(blackPen, rect);

					count = 0;

					int fontSize = 50;
					Font quicksand = fontFamily.CreateFont(fontSize);

					do
					{
						quicksand = fontFamily.CreateFont(fontSize--);
					} while (TextMeasurer.Measure(tabDays[0], new TextOptions(quicksand)).Width > intervalWidth - 20);

					Font font = fontFamily.CreateFont(fontSize, FontStyle.Bold);

					#region Draw Titles And Main Vertical Lines
					for (var i = 0; i < CANVAS_WIDTH - intervalWidth; i += intervalWidth)
					{
						#region Only For The First Image
						if (firstImage)
						{
							float multiplicator = currentWeek.ElementAt(count).Count == 0 ? 1 : currentWeek.ElementAt(count).Count;
							float posX = i + (multiplicator / 2) * intervalWidth;
							float posY = lineWidth + (intervalHeight / 2f);

							DateTimeOffset gToday = converter.ConvertNowTo(guild.TimeZone).Date;

							string today = tabDays[(int)gToday.AddDays(count).DayOfWeek];

							PointF point = new(posX, posY);

							TextOptions option = new(font)
							{
								Origin = point,
								HorizontalAlignment = HorizontalAlignment.Center,
								VerticalAlignment = VerticalAlignment.Center,
							};

							ctx.DrawText(option, today, new SolidBrush(Color.Black));
						}
						#endregion

						PointF pStart = new(i, lineWidth);
						PointF pEnd = new(i, CANVAS_HEIGHT - lineWidth);

						ctx.DrawLines(blackPen, pStart, pEnd);

						if (currentWeek.ElementAt(count).Count > 1)
							i += intervalWidth * (currentWeek.ElementAt(count).Count - 1);

						count++;
					}
					#endregion

					#region Draw Horizontal Lines
					for (int i = 0; i < CANVAS_HEIGHT - intervalHeight; i += intervalHeight)
					{
						PointF pStart = new(lineWidth, i);
						PointF pEnd = new(CANVAS_WIDTH - lineWidth, i);

						ctx.DrawLines(blackPen, pStart, pEnd);
					}
					#endregion

					#region Draw Informations And Additionals Vertical Lines
					int nbColonne = 0;
					font = fontFamily.CreateFont(13, FontStyle.Bold);
					quicksand = fontFamily.CreateFont(11, FontStyle.Bold);
					for (int i = 0; i < currentWeek.Length; i++)
					{
						#region Only For The First Image
						if (firstImage)
						{
							DateTimeOffset gToday = converter.ConvertNowTo(guild.TimeZone).Date;
							int add = currentWeek.ElementAt(i).Count > 0 ? currentWeek.ElementAt(i).Count : 1;

							PointF position = new((float)(lineWidth + nbColonne * intervalWidth + add / 2.0 * intervalWidth), (float)(lineWidth + 3.0 / 2.0 * intervalHeight));

							TextOptions options = new(font)
							{
								Origin = position,
								HorizontalAlignment = HorizontalAlignment.Center,
								VerticalAlignment = VerticalAlignment.Center,
							};

							ctx.DrawText(options, gToday.AddDays(i).ToString("dd/MM", cultureInfo), new SolidBrush(Color.Black));
						}
						#endregion

						for (int j = 0; j < currentWeek[i].Count; j++)
						{
							CalendarEvent @event = currentWeek[i][j];

							float posX = lineWidth + (j + nbColonne) * intervalWidth;
							float posY = firstImage ? lineWidth + 2 * intervalHeight : lineWidth;

							float textPosX = (float)(posX + intervalWidth / 2.0);
							float textPosY = (float)(posY + intervalHeight / 2.0);

							if (j != 0) ctx.DrawLines(blackPen, new PointF(posX, posY), new PointF(posX, CANVAS_HEIGHT - lineWidth));

							IBrush brushColor = new SolidBrush(Color.Black);

							TextOptions options = new(quicksand)
							{
								HorizontalAlignment = HorizontalAlignment.Center,
								VerticalAlignment = VerticalAlignment.Center,
							};

							#region Only For The First Image
							if (firstImage)
							{
								options.Origin = new PointF(textPosX, textPosY);
								ctx.DrawText(options, @event.CompetitionName, brushColor);

								options.Origin = new PointF(textPosX, textPosY + intervalHeight);
								ctx.DrawText(options, @event.OpponentClan?.Name, brushColor);

								options.Origin = new PointF(textPosX, textPosY + 2 * intervalHeight);
								ctx.DrawText(options, @event.Start.ToString("t", cultureInfo), brushColor);
							}
							#endregion

							int startRow = firstImage ? 3 : 0;
							int playersToTake = firstImage ? 5 : 10;
							int playersToSkip = firstImage ? 0 : k * 10 - 5;

							string[] playersDisplay = @event.Players?.Skip(playersToSkip)?.Take(playersToTake).ToArray() ?? Array.Empty<string>();

							PointF point2 = new(textPosX, textPosY + (startRow + 1) * intervalHeight);

							for (int l = 0; l < playersDisplay.Length; l++)
							{
								string tag = playersDisplay[l];
								var cPlayer = ClashOfClansApi.Players.GetByTagAsync(tag);
								cPlayer.Wait();

								PointF point = new(textPosX, textPosY + (startRow + l) * intervalHeight);

								options.Origin = point;

								ctx.DrawText(options, cPlayer?.Result?.Name, brushColor);
							}
						}

						nbColonne += currentWeek[i].Count == 0 ? 1 : currentWeek[i].Count;
					}
					#endregion
				});

				MemoryStream ms = new();
				img.SaveAsPng(ms);
				ms.Seek(0, SeekOrigin.Begin);

				dictionary.Add($"calendar_{k}.png", ms);
			}
			#endregion

			return dictionary;
		}

		private static async Task DisplayAsync(IGuild guild, Common.Models.Calendar dbCalendar)
		{
			// Just to be sure
			if (dbCalendar.ChannelId == null) return;

			// Gets text channel
			ITextChannel? channel = await guild.GetTextChannelAsync((ulong)dbCalendar.ChannelId);

			if (channel == null)
			{
				IUser owner = await guild.GetOwnerAsync();
				await owner.SendMessageAsync(dbCalendar.Guild.AdminText.CalendarDisplayCannotSend);

				return;
			}

			// Gets all the canvas
			Dictionary<string, Stream> canvas = CreateCanvas(dbCalendar.Id, dbCalendar.Guild);

			// Tries to remove old message
			try
			{
				IMessage? oldMsg = await channel.GetMessageAsync(dbCalendar.MessageId ?? 0);
				await oldMsg?.DeleteAsync()!;
			}
			catch (Exception)
			{
			}

			// Transforms canvas into FileAttachment
			FileAttachment[] files = canvas
				.Select(c => new FileAttachment(c.Value, c.Key))
				.ToArray();

			// Sends new message with files
			IMessage msg = await channel.SendFilesAsync(files);

			// Update in database
			dbCalendar.MessageId = msg.Id;
			Context.Calendars.Update(dbCalendar);
		}

		private static List<CalendarEvent>[] GetCalendarInformations(string calendarId, Common.Models.TimeZone tZone, out int nbImages)
		{
			nbImages = 1;

			// Gets calendar events
			Task<CalendarEvent[]> taskEvent = GoogleCalendarApi.Events.ListAsync(calendarId);
			taskEvent.Wait();

			IEnumerable<CalendarEvent> events = taskEvent.Result;
			NodaConverter converter = new();

			// Initializes start and end dates displayed
			DateTimeOffset startDate = converter.ConvertNowTo(tZone).Date;
			DateTimeOffset endDate = startDate.AddDays(6);

			// Filters events
			int maxPlayers = 0;
			IEnumerable<CalendarEvent> currentWeek = events.Where(m => m.IsBetweenDate(startDate, endDate));

			List<CalendarEvent>[] tabEvents =
			{
				new List<CalendarEvent>(),
				new List<CalendarEvent>(),
				new List<CalendarEvent>(),
				new List<CalendarEvent>(),
				new List<CalendarEvent>(),
				new List<CalendarEvent>(),
				new List<CalendarEvent>()
			};

			// Adds events to a two dimensional array
			foreach (CalendarEvent match in currentWeek)
			{
				int index = (int)match.Start.DayOfWeek;

				if (match.Players?.Count > maxPlayers)
					maxPlayers = match.Players.Count;

				tabEvents[index].Add(match);
			}

			// Makes the array starting from today
			int today = (int)startDate.DayOfWeek;
			List<CalendarEvent>[] finalTab = tabEvents
				.Skip(today)
				.Concat(tabEvents.Take(today))
				.ToArray();

			// Adjusts the number of image required
			maxPlayers -= 5;
			nbImages += (int)Math.Ceiling(maxPlayers / 10.0);

			return finalTab;
		}
	}
}
