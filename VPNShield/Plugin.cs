﻿using EXILED;
using System.Collections.Generic;
using System;
using System.IO;

namespace VPNShield
{
    public class Plugin : EXILED.Plugin
    {
        private static string appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        private static string exiledPath = Path.Combine(appData, "Plugins");

        public EventHandlers EventHandlers;

        public static HashSet<string> vpnWhitelistedIPs;
        public static HashSet<string> vpnBlacklistedIPs;
        public static HashSet<string> accountWhitelistedUserIDs;
        public static HashSet<string> checksWhitelistedUserIDs;

        public static bool accountCheck;
        public static int minimumAccountAge;
        public static string accountCheckKickMessage;
        public static string steamAPIKey;

        public static bool vpnCheck;
        public static string ipHubAPIKey;
        public static string vpnKickMessage;

        public static bool verboseMode;
        public static bool updateChecker;

        public static string version = "1.2.1";

        public override void OnEnable()
        {
            Log.Info("VPNShield EXILED Edition v" + version + " by SomewhatSane. Last Modified: 2020/02/11 21:26 GMT.");
            Log.Info("Thanks to KarlOfDuty for the original SMod VPNShield!");
            Log.Info("Loading Configs.");


            accountCheck = Plugin.Config.GetBool("vs_accountcheck", false);
            steamAPIKey = Plugin.Config.GetString("vs_steamapikey", null);
            minimumAccountAge = Plugin.Config.GetInt("vs_accountminage", 14);
            accountCheckKickMessage = Plugin.Config.GetString("vs_accountkickmessage", "Your account must be at least " + minimumAccountAge.ToString() + " day(s) old to play on this server.");

            vpnCheck = Plugin.Config.GetBool("vs_vpncheck", true);
            ipHubAPIKey = Plugin.Config.GetString("vs_vpnapikey", null);
            vpnKickMessage = Plugin.Config.GetString("vs_vpnkickmessage", "VPNs and proxies are forbidden on this server.");

            verboseMode = Plugin.Config.GetBool("vs_verbose", false);
            updateChecker = Plugin.Config.GetBool("vs_checkforupdates", true);

            if (verboseMode) { Log.Info("Verbose mode is enabled."); }
            if (accountCheck && steamAPIKey == null) { Log.Info("This plugin requires a Steam API Key! Get one for free at https://steamcommunity.com/dev/apikey, and set it to vs_steamapikey!"); }
            if (vpnCheck && ipHubAPIKey == null) { Log.Info("This plugin requires a VPN API Key! Get one for free at https://iphub.info, and set it to vs_vpnapikey!"); }

            _ = UpdateCheck.CheckForUpdate();

            Log.Info("Checking File System.");
            Setup.CheckFileSystem();

            Log.Info("Loading data.");

            vpnWhitelistedIPs = new HashSet<string>(FileManager.ReadAllLines(exiledPath + "/VPNShield/VPNShield-WhitelistIPs.txt")); //Known IPs that are not VPNs.
            vpnBlacklistedIPs = new HashSet<string>(FileManager.ReadAllLines(exiledPath + "/VPNShield/VPNShield-BlacklistIPs.txt")); //Known IPs that ARE VPNs.
            accountWhitelistedUserIDs = new HashSet<string>(FileManager.ReadAllLines(exiledPath + "/VPNShield/VPNShield-WhitelistAccountAgeCheck.txt")); //Known UserIDs that ARE old enough.
            checksWhitelistedUserIDs = new HashSet<string>(FileManager.ReadAllLines(exiledPath + "/VPNShield/VPNShield-WhitelistUserIDs.txt")); //UserIDs that can bypass VPN AND account checks.

            Log.Info("Loading Event Handlers.");
            EventHandlers = new EventHandlers(this);
            Events.PlayerJoinEvent += EventHandlers.OnPlayerJoin;
            Events.RemoteAdminCommandEvent += EventHandlers.OnRACommand;

            Log.Info("Done.");
            
        }

        public override void OnDisable()
        {
            Events.PlayerJoinEvent -= EventHandlers.OnPlayerJoin;
            EventHandlers = null;
            Log.Info("Disabled.");
        }

        public override void OnReload() { }

        public override string getName { get; } = "VPNShield EXILED Edition";
    }
}
