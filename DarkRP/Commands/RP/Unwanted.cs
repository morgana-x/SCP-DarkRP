using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Instrumentation;
using System.Reflection;
using CommandSystem;
using LabApi.Features.Wrappers;
using DarkRP.Extensions;
using DarkRP.Modules.Entities;


namespace DarkRP.Commands.RP
{
    [CommandHandler(typeof(ClientCommandHandler))]
    public class Unwanted : ParentCommand, ICommand
    {
        public override string Command { get; } = "unwant";
        public override string Description { get; } = ".unwant name";

        public override string[] Aliases { get; } = new string[] { "unwanted" };
        public override void LoadGeneratedCommands() { }

        protected override bool ExecuteParent(ArraySegment<string> args, ICommandSender sender, out string response)
        {
            var p = Player.Get(sender);

            if (!Modules.Players.Jobs.Government.IsGovernment(p))
            {
                response = $"Only government roles can un set people as wanted!";
                return false;
            }

            if (args.Count < 1)
            {
                response = "Missing arguments! Correct Usage: .want playername";
                return false;
            }

            var target = Player.GetByDisplayName(args.ElementAt(0));

            if (target == null)
            {
                response = $"Couldn't find player \"{args.ElementAt(0)}\"";
                return false;
            }

            if (!Modules.Players.Jobs.Government.IsWanted(target))
            {
                response = $"{target.DisplayName} is not currently wanted!";
                return false;
            }
            Modules.Players.Jobs.Government.Unwant(target);
            response = $"{target.DisplayName} is no longer wanted!";
            return true;
        }
    }
}
