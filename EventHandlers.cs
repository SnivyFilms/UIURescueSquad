using Exiled.API.Features;
using Exiled.Events.EventArgs.Map;
using Exiled.Events.EventArgs.Server;
using Exiled.Loader;
using MEC;
using PlayerRoles;
using Respawning;
using System;
using System.Collections.Generic;

namespace UIURescueSquad
{
     internal sealed class EventHandlers
     {
          private int Respawns = 0;
          private int UIURespawns = 0;
          private CoroutineHandle calcuationCoroutine;

          public void OnRoundStarted()
          {
               UIURescueSquad.Instance.IsSpawnable = false;
               UIURescueSquad.Instance.NextIsForced = false;
               Respawns = 0;
               UIURespawns = 0;

               if (calcuationCoroutine.IsRunning)
                    Timing.KillCoroutines(calcuationCoroutine);

               calcuationCoroutine = Timing.RunCoroutine(spawnCalculation());
          }

          private IEnumerator<float> spawnCalculation()
          {
               while (true)
               {
                    yield return Timing.WaitForSeconds(1f);

                    if (Round.IsEnded)
                         break;

                    if (Math.Round(Respawn.TimeUntilSpawnWave.TotalSeconds, 0) != UIURescueSquad.Instance.Config.SpawnWaveCalculation)
                         continue;

                    if (Respawn.NextKnownTeam == SpawnableTeamType.NineTailedFox)
                         UIURescueSquad.Instance.IsSpawnable = (Loader.Random.Next(100) <= UIURescueSquad.Instance.Config.SpawnManager.Probability &&
                             Respawns >= UIURescueSquad.Instance.Config.SpawnManager.Respawns &&
                             UIURespawns < UIURescueSquad.Instance.Config.SpawnManager.MaxSpawns) || UIURescueSquad.Instance.NextIsForced;
               }
          }

          public void OnRespawningTeam(RespawningTeamEventArgs ev)
          {
               if (UIURescueSquad.Instance.IsSpawnable || UIURescueSquad.Instance.NextIsForced)
               {
                    List<Player> players = new List<Player>();
                    if (ev.Players.Count > UIURescueSquad.Instance.Config.SpawnManager.MaxSquad)
                         players = ev.Players.GetRange(0, UIURescueSquad.Instance.Config.SpawnManager.MaxSquad);
                    else
                         players = ev.Players.GetRange(0, ev.Players.Count);

                    Queue<RoleTypeId> queue = ev.SpawnQueue;
                    foreach (RoleTypeId role in queue)
                    {
                         if (players.Count <= 0)
                              break;
                         Player player = players.RandomItem();
                         players.Remove(player);
                         switch (role)
                         {
                              case RoleTypeId.NtfCaptain:
                                   UIURescueSquad.Instance.Config.UiuLeader.AddRole(player);
                                   break;
                              case RoleTypeId.NtfSergeant:
                                   UIURescueSquad.Instance.Config.UiuAgent.AddRole(player);
                                   break;
                              case RoleTypeId.NtfPrivate:
                                   UIURescueSquad.Instance.Config.UiuSoldier.AddRole(player);
                                   break;
                         }
                    }
                    UIURespawns++;
                    
                    ev.NextKnownTeam = SpawnableTeamType.None;
               }
               Respawns++;
          }

          public void OnAnnouncingNtfEntrance(AnnouncingNtfEntranceEventArgs ev)
          {
               string cassieMessage = string.Empty;
               string cassieText = string.Empty;
               if (UIURescueSquad.Instance.IsSpawnable || UIURescueSquad.Instance.NextIsForced)
               {
                    if (ev.ScpsLeft == 0 && !string.IsNullOrEmpty(UIURescueSquad.Instance.Config.SpawnManager.UiuAnnouncmentCassieNoScp))
                    {
                         ev.IsAllowed = false;
                         cassieMessage = UIURescueSquad.Instance.Config.SpawnManager.UiuAnnouncmentCassieNoScp;
                         cassieText = UIURescueSquad.Instance.Config.SpawnManager.CassieTextUiuNoSCPs;
                    }
                    else if (ev.ScpsLeft >= 1 && !string.IsNullOrEmpty(UIURescueSquad.Instance.Config.SpawnManager.UiuAnnouncementCassie))
                    {
                         ev.IsAllowed = false;
                         cassieMessage = UIURescueSquad.Instance.Config.SpawnManager.UiuAnnouncementCassie;
                         cassieText = UIURescueSquad.Instance.Config.SpawnManager.CassieTextUiuSCPs;
                    }
                    UIURescueSquad.Instance.NextIsForced = false;
                    UIURescueSquad.Instance.IsSpawnable = false;
               }
               else
               {
                    if (ev.ScpsLeft == 0 && !string.IsNullOrEmpty(UIURescueSquad.Instance.Config.SpawnManager.NtfAnnouncmentCassieNoScp))
                    {
                         ev.IsAllowed = false;
                         cassieMessage = UIURescueSquad.Instance.Config.SpawnManager.NtfAnnouncmentCassieNoScp;
                         cassieText = UIURescueSquad.Instance.Config.SpawnManager.CassieTextMtfNoSCPs;
                    }
                    else if (ev.ScpsLeft >= 1 && !string.IsNullOrEmpty(UIURescueSquad.Instance.Config.SpawnManager.NtfAnnouncementCassie))
                    {
                         ev.IsAllowed = false;
                         cassieMessage = UIURescueSquad.Instance.Config.SpawnManager.NtfAnnouncementCassie;
                         cassieText = UIURescueSquad.Instance.Config.SpawnManager.CassieTextMtfSCPs;
                    }
               }

               cassieMessage = cassieMessage.Replace("{scpnum}", $"{ev.ScpsLeft} scpsubject");
               cassieText = cassieText.Replace("{scpnum}", $"{ev.ScpsLeft} scpsubject");

            if (ev.ScpsLeft > 1)
            {
                cassieMessage = cassieMessage.Replace("scpsubject", "scpsubjects");
                cassieText = cassieText.Replace("scpsubject", "scpsubjects");
            }
               cassieMessage = cassieMessage.Replace("{designation}", $"nato_{ev.UnitName[0]} {ev.UnitNumber}");
               cassieText = cassieText.Replace("{designation}", $"nato_{ev.UnitName[0]} {ev.UnitNumber}");

               if (!string.IsNullOrEmpty(cassieMessage))
                    Cassie.MessageTranslated(cassieMessage, cassieText, isSubtitles: UIURescueSquad.Instance.Config.SpawnManager.Subtitles);
          }
     }
}
