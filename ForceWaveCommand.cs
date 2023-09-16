using CommandSystem;
using Exiled.Permissions.Extensions;
using Exiled.API.Features;
using Exiled.API.Enums;
using PlayerRoles;
using System;

namespace UIURescueSquad
{
     [CommandHandler(typeof(RemoteAdminCommandHandler))]
     internal class ForceWaveCommand : ICommand
     {
          public string Command { get; set; } = "forceuiuwave";
          public string Description { get; set; } = "Forces UIU on next spawn wave!";
          public string[] Aliases { get; set; } = new string[] { };
          public bool Execute(ArraySegment<string> args, ICommandSender sender, out string response)
          {
               if (!sender.CheckPermission(PlayerPermissions.RoundEvents))
               {
                    response = "You do not have permission to use this command!";
                    return false;
               }

               UIURescueSquad.Instance.NextIsForced = true;
               UIURescueSquad.Instance.IsSpawnable = true;

               response = "Next MTF wave will be UIU!";
               return true;
          }
     }
}
