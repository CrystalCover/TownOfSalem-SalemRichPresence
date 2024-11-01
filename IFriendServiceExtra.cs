using DiscordRPC;
using HarmonyLib;
using System.Collections.Generic;
using static System.DateTime;

namespace Eca.SalemRichPresence
{
    [HarmonyPatch(typeof(IFriendService))]
    internal class IFriendServiceExtra
    {
        private static DiscordRpcClient DiscordRpcClient => Plugin.DiscordRpcClient;

        private static RichPresence CurrentPresence => DiscordRpcClient.CurrentPresence;

        private static string inParty => "inparty";

        private static string online => "online";

        private static string InParty => "In Party";

        private static string Online => "Online";

        private static Dictionary<int, string> GameModes { get; } = new Dictionary<int, string>()
        {
            { 1, "Classic" },
            { 2, "Custom" },
            { 3, "All Any" },
            { 4, "Rapid Mode" },
            { 5, "Vigilantics" },
            { 6, "Ranked" },
            { 7, "Rainbow" },
            { 8, "Ranked Practice" },
            { 9, "Custom (Coven)" },
            { 10, "Classic (Coven)" },
            { 11, "Ranked Practice (Coven)" },
            { 12, "Ranked (Coven)" },
            { 13, "All Any (Coven)" },
            { 14, "VIP Mode (Coven)" },
            { 15, "Devs Only (Coven)" },
            { 16, "Lovers Mode (Coven)" },
            { 17, "Rivals Mode (Coven)" },
            { 18, "Mafia Returns (Coven)" },
            { 19, "Dracula's Palace" },
            { 20, "Town Traitor (Coven)" },
            { 21, "Town Traitor" },
            { 22, "Devs Only" }
        };

        [HarmonyPatch("RaiseOnPartyStatusChanged")]
        [HarmonyPostfix]
        private static void RaiseOnPartyStatusChanged(IFriendService __instance, bool inParty)
        {
            RichPresence newPresence = CurrentPresence.Clone();
            newPresence.Assets.SmallImageKey = inParty ? IFriendServiceExtra.inParty : online;
            newPresence.Assets.SmallImageText = newPresence.State = inParty ? InParty : Online;
            newPresence.Timestamps = inParty ? new Timestamps(UtcNow) : IApplicationServiceExtra.Timestamps;
            newPresence.Details = inParty ? GameModes[__instance.GameMode.Id] : string.Empty;
            DiscordRpcClient.SetPresence(newPresence);
        }

        [HarmonyPatch(nameof(IFriendService.RaiseOnHostSetPartyConfigs))]
        [HarmonyPostfix]
        private static void RaiseOnHostSetPartyConfigs(int gameMode) => DiscordRpcClient.UpdateDetails(GameModes[gameMode]);
    }
}