using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Wp.Common.Models;
using Wp.Database;
using Wp.Discord.Extensions;

namespace Wp.Bot.Modules.ApplicationCommands
{
	public class RequireUserRoleAttribute : PreconditionAttribute
	{
		/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                               FIELDS                              *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

		private readonly RoleType role;

		/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                             PROPERTIES                            *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */



		/* * * * * * * * * * * * * * * * * *\
        |*            SHORTCUTS            *|
        \* * * * * * * * * * * * * * * * * */



		/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                            CONSTRUCTORS                           *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

		public RequireUserRoleAttribute(RoleType role)
		{
			this.role = role;
		}

		/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                          ABSTRACT METHODS                         *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */



		/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                           PUBLIC METHODS                          *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */



		/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                         PROTECTED METHODS                         *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */



		/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                          PRIVATE METHODS                          *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */



		/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                          OVERRIDE METHODS                         *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

		public override Task<PreconditionResult> CheckRequirementsAsync(IInteractionContext context, ICommandInfo commandInfo, IServiceProvider services)
		{
			// Loads databases infos
			Guild dbGuild = Context
				.Guilds
				.First(g => g.Id == context.Guild.Id);

			// Checks if this user is a Guild User, which is the only context where roles exist
			if (context.User is SocketGuildUser gUser)
			{
				bool isOk = role switch
				{
					RoleType.OWNER => gUser.IsTheOwner(),
					RoleType.ADMINISTRATOR => gUser.IsAnAdmin(),
					RoleType.MANAGER => gUser.IsAManager(),
					RoleType.PLAYER => gUser.IsAPlayer(),
					_ => throw new ArgumentOutOfRangeException(nameof(role), role, null),
				};

				if (isOk)
				{
					return Task.FromResult(PreconditionResult.FromSuccess());
				}
				else
				{
					return Task.FromResult(PreconditionResult.FromError(dbGuild.GeneralResponses.RequireUserAttribute));
				}
			}
			else
			{
				return Task.FromResult(PreconditionResult.FromError(dbGuild.GeneralResponses.RequireUserNotInGuild));
			}
		}

		/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                           STATIC METHODS                          *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */



		/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                              INDEXERS                             *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */



		/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                         OPERATORS OVERLOAD                        *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */
	}
}
