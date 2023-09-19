using System;
using Exiled.API.Features;
using Exiled.CustomRoles.API;
using Exiled.CustomRoles.API.Features;

using MapEvent = Exiled.Events.Handlers.Map;
using ServerEvent = Exiled.Events.Handlers.Server;

namespace UIURescueSquad
{
     public class UIURescueSquad : Plugin<Configs.Config>
     {

          public override string Name { get; } = "UIURescueSquad";
          public override string Author { get; } = "JesusQC, Michal78900, Marco15453, Vicious Vikki, & Misfiy";
          public override string Prefix { get; } = "UIURescueSquad";
          public override Version Version { get; } = new Version(5, 3, 1);
          public override Version RequiredExiledVersion => new Version(8, 2, 1);
          
          public static UIURescueSquad Instance;

          public bool IsSpawnable = false;
          public bool NextIsForced = false;

          private EventHandlers eventHandlers;

          public override void OnEnabled()
          {
               Instance = this;
               Config.UiuSoldier.Register();
               Config.UiuAgent.Register();
               Config.UiuLeader.Register();

               eventHandlers = new();

               ServerEvent.RoundStarted += eventHandlers.OnRoundStarted;
               ServerEvent.RespawningTeam += eventHandlers.OnRespawningTeam;
               MapEvent.AnnouncingNtfEntrance += eventHandlers.OnAnnouncingNtfEntrance;

               base.OnEnabled();
          }

          public override void OnDisabled()
          {
               CustomRole.UnregisterRoles();

               ServerEvent.RoundStarted -= eventHandlers.OnRoundStarted;
               ServerEvent.RespawningTeam -= eventHandlers.OnRespawningTeam;
               MapEvent.AnnouncingNtfEntrance -= eventHandlers.OnAnnouncingNtfEntrance;

               eventHandlers = null;
               Instance = null!;
               base.OnDisabled();
          }
     }
}
