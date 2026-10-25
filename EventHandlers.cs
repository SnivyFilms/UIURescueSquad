using System.Collections.Generic;
using System.Linq;
using Exiled.Events.EventArgs.Map;
using Exiled.Events.EventArgs.Server;
using Exiled.Loader;
using MEC;
using PlayerRoles;
using Player = Exiled.API.Features.Player;

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
          }

          public void OnRespawningTeam(RespawningTeamEventArgs ev)
          {
               if (ev.Wave.Faction == Faction.FoundationStaff)
               {

                    bool forced = UIURescueSquad.Instance.NextIsForced;
                    bool miniWaveBlocked = ev.Wave.IsMiniWave &&
                                           !UIURescueSquad.Instance.Config.SpawnManager.UiuSpawnsDuringMiniWave;
                    bool conditionsMet = !forced &
                                         Respawns >= UIURescueSquad.Instance.Config.SpawnManager.Respawns &&
                                         UIURespawns < UIURescueSquad.Instance.Config.SpawnManager.MaxSpawns &&
                                         Loader.Random.Next(101) <
                                         UIURescueSquad.Instance.Config.SpawnManager.Probability;

                    if (!forced && !conditionsMet)
                         return;

                    Respawns++;
                    UIURescueSquad.Instance.IsSpawnable = true;
                    List<Player> players = ev.Players.GetRange(0, ev.Players.Count >  UIURescueSquad.Instance.Config.SpawnManager.MaxSquad
                         ?  UIURescueSquad.Instance.Config.SpawnManager.MaxSquad
                         : ev.Players.Count);

                    int uiuSpawnCount = 0;
                    foreach (var player in players.Where(player => player is not null))
                    {
                         switch (uiuSpawnCount)
                         {
                              case 0:
                                   UIURescueSquad.Instance.Config.UiuLeader.AddRole(player);
                                   uiuSpawnCount++;
                                   break;
                              case >= 1 and <= 3:
                                   UIURescueSquad.Instance.Config.UiuAgent.AddRole(player);
                                   uiuSpawnCount++;
                                   break;
                              default:
                                   UIURescueSquad.Instance.Config.UiuSoldier.AddRole(player);
                                   break;
                         }
                    }
                    UIURespawns++;
                    UIURescueSquad.Instance.IsSpawnable = false;
                    UIURescueSquad.Instance.NextIsForced = false;
                    ev.IsAllowed = false;
               }
               Respawns++;
          }

          public void OnWaveSpawned(RespawnedTeamEventArgs ev)
          {
               if (UIURescueSquad.Instance.IsSpawnable)
                    UIURescueSquad.Instance.IsSpawnable = false;
          }
          
          public void OnAnnouncingNtfEntrance(AnnouncingNtfEntranceEventArgs ev)
          {
               if (ev.Wave == null)
                    return;
               if (!UIURescueSquad.Instance.Config.SpawnManager.UseCustomCassie)
                    return;
               ev.IsAllowed = false;
               Exiled.API.Features.Cassie.Clear();
               string cassieMessage = string.Empty;
               string cassieText = string.Empty;
               if (UIURescueSquad.Instance.IsSpawnable || UIURescueSquad.Instance.NextIsForced)
               {
                    if (ev.ScpsLeft == 0 && !string.IsNullOrEmpty(UIURescueSquad.Instance.Config.SpawnManager.UiuAnnouncmentCassieNoScp))
                    {
                         cassieMessage = UIURescueSquad.Instance.Config.SpawnManager.UiuAnnouncmentCassieNoScp;
                         cassieText = UIURescueSquad.Instance.Config.SpawnManager.CassieTextUiuNoSCPs;
                    }
                    else if (ev.ScpsLeft >= 1 && !string.IsNullOrEmpty(UIURescueSquad.Instance.Config.SpawnManager.UiuAnnouncementCassie))
                    {
                         cassieMessage = UIURescueSquad.Instance.Config.SpawnManager.UiuAnnouncementCassie;
                         cassieText = UIURescueSquad.Instance.Config.SpawnManager.CassieTextUiuSCPs;
                    }
                    UIURescueSquad.Instance.NextIsForced = false;
                    UIURescueSquad.Instance.IsSpawnable = false;
               }
               else
               {
                    if (ev.Wave.IsMiniWave)
                    {
                         switch (ev.ScpsLeft)
                         {
                              case 0 when
                                   !string.IsNullOrEmpty(
                                        UIURescueSquad.Instance.Config.SpawnManager.NtfMiniAnnouncmentCassieNoScp):
                                   cassieMessage = UIURescueSquad.Instance.Config.SpawnManager.NtfMiniAnnouncmentCassieNoScp;
                                   cassieText = UIURescueSquad.Instance.Config.SpawnManager.CassieTextMiniMtfNoSCPs;
                                   break;
                              case >= 1 when
                                   !string.IsNullOrEmpty(UIURescueSquad.Instance.Config.SpawnManager
                                        .NtfMiniAnnouncementCassie):
                                   cassieMessage = UIURescueSquad.Instance.Config.SpawnManager.NtfMiniAnnouncementCassie;
                                   cassieText = UIURescueSquad.Instance.Config.SpawnManager.CassieTextMiniMtfSCPs;
                                   break;
                         }
                    }
                    else
                    {
                         switch (ev.ScpsLeft)
                         {
                              case 0 when
                                   !string.IsNullOrEmpty(
                                        UIURescueSquad.Instance.Config.SpawnManager.NtfAnnouncmentCassieNoScp):
                                   cassieMessage = UIURescueSquad.Instance.Config.SpawnManager.NtfAnnouncmentCassieNoScp;
                                   cassieText = UIURescueSquad.Instance.Config.SpawnManager.CassieTextMtfNoSCPs;
                                   break;
                              case >= 1 when
                                   !string.IsNullOrEmpty(UIURescueSquad.Instance.Config.SpawnManager
                                        .NtfAnnouncementCassie):
                                   cassieMessage = UIURescueSquad.Instance.Config.SpawnManager.NtfAnnouncementCassie;
                                   cassieText = UIURescueSquad.Instance.Config.SpawnManager.CassieTextMtfSCPs;
                                   break;
                         }
                    }
               }

               cassieMessage = cassieMessage.Replace("{scpnum}", $"{ev.ScpsLeft} scpsubject");
               cassieText = cassieText.Replace("{scpnum}", $"{ev.ScpsLeft} SCP subject");

               if (ev.ScpsLeft > 1)
               {
                    cassieMessage = cassieMessage.Replace("scpsubject", "scpsubjects");
                    cassieText = cassieText.Replace("SCP subject", "SCP subjects");
               }

               if (ev.UnitName != null)
               {
                    cassieMessage = cassieMessage.Replace("{designation}", $"nato_{ev.UnitName[0]} {ev.UnitNumber}");
                    cassieText = cassieText.Replace("{designation}", GetNatoName(ev.UnitName) + " " + ev.UnitNumber);
               }

               if (!string.IsNullOrWhiteSpace(cassieMessage))
                    LabApi.Features.Wrappers.Announcer.Message(cassieMessage, cassieText);
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
