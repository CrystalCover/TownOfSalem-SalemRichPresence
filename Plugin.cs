using BepInEx;
using DiscordRPC;
using HarmonyLib;

namespace Eca.SalemRichPresence
{
    [BepInPlugin(GUID, Name, Version)]
    internal class Plugin : BaseUnityPlugin
    {
        private const string GUID = "Eca.SalemRichPresence";

        private const string Name = "SalemRichPresence";

        private const string Version = "1.0";

        internal static DiscordRpcClient DiscordRpcClient { get; } = new DiscordRpcClient("1149108941901529158");

        internal static Harmony Harmony { get; } = new Harmony(GUID);

        private void Awake()
        {
            if (DiscordRpcClient.Initialize())
                Harmony.PatchAll();
        }
    }
}