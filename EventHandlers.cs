using Exiled.API.Features;
using Exiled.Events.EventArgs.Map;
using Exiled.Events.EventArgs.Server;
using Exiled.Loader;
using MEC;
using PlayerRoles;
using Respawning;
using System;
using System.Collections.Generic;
using Exiled.API.Enums;

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
                    
                    //if (Math.Round(Respawn.Time.TotalSeconds, 0) != UIURescueSquad.Instance.Config.SpawnWaveCalculation)
                    //    continue;
                    
                    if (Respawn.IsSpawning)
                         continue;

                    if (Respawn.NextKnownSpawnableFaction is SpawnableFaction.NtfWave)
                    {
                         UIURescueSquad.Instance.IsSpawnable = 
                              (Loader.Random.Next(100) <= UIURescueSquad.Instance.Config.SpawnManager.Probability &&
                               Respawns >= UIURescueSquad.Instance.Config.SpawnManager.Respawns &&
                               UIURespawns < UIURescueSquad.Instance.Config.SpawnManager.MaxSpawns) || UIURescueSquad.Instance.NextIsForced;
                    }
                    else if (Respawn.NextKnownSpawnableFaction == SpawnableFaction.NtfMiniWave &&
                             UIURescueSquad.Instance.Config.SpawnManager.UiuSpawnsDuringMiniWave)
                    {
                         UIURescueSquad.Instance.IsSpawnable = 
                              (Loader.Random.Next(100) <= UIURescueSquad.Instance.Config.SpawnManager.Probability && 
                               Respawns >= UIURescueSquad.Instance.Config.SpawnManager.Respawns &&
                               UIURespawns < UIURescueSquad.Instance.Config.SpawnManager.MaxSpawns) || UIURescueSquad.Instance.NextIsForced;
                    }
                         
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
               cassieText = cassieText.Replace("{scpnum}", $"{ev.ScpsLeft} SCP subject");

               if (ev.ScpsLeft > 1)
               {
                    cassieMessage = cassieMessage.Replace("scpsubject", "scpsubjects");
                    cassieText = cassieText.Replace("SCP subject", "SCP subjects");
               }
               cassieMessage = cassieMessage.Replace("{designation}", $"nato_{ev.UnitName[0]} {ev.UnitNumber}");
               cassieText = cassieText.Replace("{designation}", GetNatoName(ev.UnitName) + " " + ev.UnitNumber);

               if (!string.IsNullOrEmpty(cassieMessage))
                    Cassie.MessageTranslated(cassieMessage, cassieText, isSubtitles: UIURescueSquad.Instance.Config.SpawnManager.Subtitles);
          }
        public string GetNatoName(string unitName)
        {
            Dictionary<string, string> natoAlphabet = new Dictionary<string, string>()
            {
                {"a", "ALPHA"},
                {"b", "BRAVO"},
                {"c", "CHARLIE"},
                {"d", "DELTA"},
                {"e", "ECHO"},
                {"f", "FOXTROT"},
                {"g", "GOLF"},
                {"h", "HOTEL"},
                {"i", "INDIA"},
                {"j", "JULIET"},
                {"k", "KILO"},
                {"l", "LIMA"},
                {"m", "MIKE"},
                {"n", "NOVEMBER"},
                {"o", "OSCAR"},
                {"p", "PAPA"},
                {"q", "QUEBEC"},
                {"r", "ROMEO"},
                {"s", "SIERRA"},
                {"t", "TANGO"},
                {"u", "UNIFORM"},
                {"v", "VICTOR"},
                {"w", "WHISKEY"},
                {"x", "XRAY"},
                {"y", "YANKEE"},
                {"z", "ZULU" },
            };

            string firstLetter = unitName[0].ToString().ToLower();

            if (natoAlphabet.ContainsKey(firstLetter))
            {
                return natoAlphabet[firstLetter];
            }
            else
            {
                return $"nato_{firstLetter}";
            }
        }
    }
}
