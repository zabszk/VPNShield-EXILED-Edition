﻿using System.Threading.Tasks;
using System.Net.Http;
using EXILED;

namespace VPNShield
{
    internal static class UpdateCheck
    {
        internal static async Task CheckForUpdate()
        {
            if (!Plugin.updateChecker)
                return;

            Log.Info("Checking for updates.");
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("User-Agent", "VPNShield Update Checker - Running VPNShield v" + Plugin.version);
                HttpResponseMessage response = await client.GetAsync("https://scpsl.somewhatsane.co.uk/plugins/vpnshield/latest.html");

                if (!response.IsSuccessStatusCode)
                {
                    Log.Error("An error occurred when trying to check for updates. Response from server: " + response.StatusCode);
                    return;
                }

                string data = await response.Content.ReadAsStringAsync();
                string[] dataarray = data.Split(';');
                    
                if (dataarray[0] == Plugin.version)
                    Log.Info("You are running the latest version of VPNShield.");

                else if (dataarray[0] != Plugin.version)
                    Log.Info("A new version of VPNShield (v" + dataarray[0] + ") is available. Download it at: " + dataarray[1] + " .");
                else
                    Log.Error("Unexpected reply from server when trying to check for updates. Response from server: " + data);
            }
        }
    }
}
