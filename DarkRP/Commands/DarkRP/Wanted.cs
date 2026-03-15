using System;
using System.Linq;
using CommandSystem;
using LabApi.Features.Wrappers;


namespace DarkRP.Commands.DarkRP
{
    [CommandHandler(typeof(ClientCommandHandler))]
    public class Wanted  : ParentCommand, ICommand
    {
        public override string Command { get; } = "want";
        public override string Description { get; } = ".want name reason";

        public override string[] Aliases { get; } = new string[] { "wanted" };
        public override void LoadGeneratedCommands() { }

        protected override bool ExecuteParent(ArraySegment<string> args, ICommandSender sender, out string response)
        {
            var p = Player.Get(sender);

            if (!Modules.Players.Jobs.Government.IsGovernment(p))
            {
                response = $"Only government roles can set people as wanted!";
                return false;
            }

            if (args.Count < 2)
            {
                response = "Missing arguments! Correct Usage: .want playername reason";
                return false;
            }

            var target = Player.GetByDisplayName(args.ElementAt(0));

            if (target == null)
            {
                response = $"Couldn't find player \"{args.ElementAt(0)}\"";
                return false;
            }


            string reason = string.Join(" ", args.Segment(1).ToArray());
            if (reason.Trim() == "")
                reason = "Illegal Activities";


            reason = reason.Replace("</color>", "").Replace("<color", "").Replace("<size", "").Replace("</size>", "");
            if (reason.Length > 32)
            {
                response = "Wanted reason too long! 32 character limit!";
                return false;
            }
       
            response = $"{target.DisplayName} is now wanted! Reason: {reason}";
            Modules.Players.Jobs.Government.SetWanted(target, reason, p);
            return true;
        }
    }
}
