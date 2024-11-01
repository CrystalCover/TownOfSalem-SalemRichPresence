using DiscordRPC;
using HarmonyLib;
using System.Collections.Generic;
using static System.DateTime;

namespace Eca.SalemRichPresence
{
    [HarmonyPatch(typeof(ILobbyService))]
    internal class ILobbyServiceExtra
    {
        private static DiscordRpcClient DiscordRpcClient => Plugin.DiscordRpcClient;

        private static RichPresence CurrentPresence => DiscordRpcClient.CurrentPresence;

        private static string inQueue => "inqueue";

        private static string InQueue => "In Queue";

        private static string online => "online";

        private static string Online => "Online";

        private static IUserService UserService => GlobalServiceLocator.UserService;

        private static UserSelections Selections => UserService.Selections;

        private static string lobbyicon => "lobbyicon_";

        private static string inLobby => "inlobby";

        private static string InLobby => "In Lobby";

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

        [HarmonyPatch("RaiseOnStartRankedQueue")]
        [HarmonyPostfix]
        private static void RaiseOnStartRankedQueue()
        {
            RichPresence newPresence = CurrentPresence.Clone();
            newPresence.Assets.SmallImageKey = inQueue;
            newPresence.Assets.SmallImageText = newPresence.State = InQueue;
            newPresence.Timestamps = new Timestamps(UtcNow);
            DiscordRpcClient.SetPresence(newPresence);
        }

        [HarmonyPatch("RaiseOnLeaveRankedQueue")]
        [HarmonyPostfix]
        private static void RaiseOnLeaveRankedQueue()
        {
            RichPresence newPresence = CurrentPresence.Clone();
            newPresence.Assets.SmallImageKey = online;
            newPresence.Assets.SmallImageText = newPresence.State = Online;
            newPresence.Timestamps = IApplicationServiceExtra.Timestamps;
            DiscordRpcClient.SetPresence(newPresence);
        }

        [HarmonyPatch(nameof(ILobbyService.RaiseOnLobbyCreated))]
        [HarmonyPostfix]
        private static void RaiseOnLobbyCreated(GameMode mode)
        {
            RichPresence newPresence = CurrentPresence.Clone();
            //newPresence.Assets.LargeImageKey = lobbyicon + Selections.lobbyIcon;
            newPresence.Assets.SmallImageKey = inLobby;
            newPresence.Assets.SmallImageText = newPresence.State = InLobby;
            newPresence.Timestamps = new Timestamps(UtcNow);
            newPresence.Details = GameModes[mode.Id];
            DiscordRpcClient.SetPresence(newPresence);
        }
    }
}