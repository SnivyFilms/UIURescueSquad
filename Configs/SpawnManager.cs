using System;
using System.ComponentModel;

namespace UIURescueSquad.Configs
{ 
    public class SpawnManager
    {

        [Description("How many respawn waves must occur before considering UIU to spawn.")]
        public int Respawns { get; private set; } = 1;

        [Description("The maximum number of times UIU Rescue Squad can spawn per game.")]
        public int MaxSpawns { get; set; } = 3;

        [Description("Probability of a UIU Squad replacing a MTF spawn")]
        public int Probability { get; private set; } = 50;

        [Description("The maximum size of a UIU squad")]
        public int MaxSquad { get; private set; } = 8;

        [Description("Set this to false if you dont want to have any custom Cassie messages")]
        public bool UseCustomCassie { get; set; } = true;

        [Description("UIU entrance Cassie Message")]
        public string UiuAnnouncementCassie { get; private set; } = "The U I U Squad designated {designation} HasEntered AllRemaining AwaitingRecontainment {scpnum}";
        public string UiuAnnouncmentCassieNoScp { get; private set; } = "The U I U Squad designated {designation} HasEntered AllRemaining NoSCPsLeft";
        public string NtfAnnouncementCassie { get; private set; } = "MTFUnit epsilon 11 designated {designation} hasentered AllRemaining AwaitingRecontainment {scpnum}";
        public string NtfAnnouncmentCassieNoScp { get; private set; } = "MTFUnit epsilon 11 designated {designation} hasentered AllRemaining NoSCPsLeft";
        public string NtfMiniAnnouncementCassie { get; private set; } = "MTFUnit epsilon 11 designated {designation} hasentered AllRemaining AwaitingRecontainment {scpnum}";
        public string NtfMiniAnnouncmentCassieNoScp { get; private set; } = "MTFUnit epsilon 11 designated {designation} hasentered AllRemaining NoSCPsLeft";

        [Description("Cassie Text MTF SCPs")]
        public string CassieTextMtfSCPs { get; private set; } = "Mobile Task Force Unit Epsilon 11, designated {designation} has entered the facility. All remaining personnel are advised to proceed with standard evaction protocols until a MTF squad reaches your destination. awaiting recontainment of {scpnum}.";

        [Description("Cassie Text MTF No SCPs")]
        public string CassieTextMtfNoSCPs { get; private set; } = "Mobile Task Force Unit Epsilon 11, designated {designation} has entered the facility. All remaining personnel are advised to proceed with standard evaction protocols until a MTF squad reaches your destination. Substantial threat remains within the facility - exercise caution.";
        [Description("Cassie Text MTF SCPs")]
        public string CassieTextMiniMtfSCPs { get; private set; } = "Mobile Task Force Unit Epsilon 11, designated {designation} has entered the facility. All remaining personnel are advised to proceed with standard evaction protocols until a MTF squad reaches your destination. awaiting recontainment of {scpnum}.";

        [Description("Cassie Text MTF No SCPs")]
        public string CassieTextMiniMtfNoSCPs { get; private set; } = "Mobile Task Force Unit Epsilon 11, designated {designation} has entered the facility. All remaining personnel are advised to proceed with standard evaction protocols until a MTF squad reaches your destination. Substantial threat remains within the facility - exercise caution.";

        [Description("Cassie Text UIU SCPs")]
        public string CassieTextUiuSCPs { get; private set; } = "The UIU Squad, designated {designation} has entered the facility. Awaiting recontainment of {scpnum}";

        [Description("Cassie Text UIU No SCPs")]
        public string CassieTextUiuNoSCPs { get; private set; } = "The UIU Squad, designated {designation} has entered the facility. Substantial threat remains within the facility - exercise caution.";

        [Description("Can UIU Spawn during a Mini-MTF Wave?")]
        public bool UiuSpawnsDuringMiniWave { get; set; } = false;
    }
}
