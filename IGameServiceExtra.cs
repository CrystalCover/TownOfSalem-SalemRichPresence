using DiscordRPC;
using HarmonyLib;
using System.Collections.Generic;
using static System.DateTime;
using static System.String;

namespace Eca.SalemRichPresence
{
    [HarmonyPatch(typeof(IGameService))]
    internal class IGameServiceExtra
    {
        private static DiscordRpcClient DiscordRpcClient => Plugin.DiscordRpcClient;

        private static RichPresence CurrentPresence => DiscordRpcClient.CurrentPresence;

        private static IUserService UserService => GlobalServiceLocator.UserService;

        private static UserSelections Selections => UserService.Selections;

        private static string background => "background_";

        private static string inGame => "ingame";

        private static string InGame => "In Game";

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

        private static string Day => "Day {0}";

        private static string Night => "Night {0}";

        private static Dictionary<int, string> Winners { get; } = new Dictionary<int, string>()
        {
            { 1, "Town Wins" },
            { 2, "The Mafia Wins" },
            { 3, "Serial Killer Wins" },
            { 4, "Arsonist Wins" },
            { 5, "Witch Wins" },
            { 6, "Amneisac Wins" },
            { 7, "Executioner Wins" },
            { 8, "Survivor Wins" },
            { 9, "Neutrals Win" },
            { 10, "Jester Wins" },
            { 11, "Draw" },
            { 12, "Werewolf Wins" },
            { 13, "Vampires Win" },
            { 14, "The Coven Wins" },
            { 15, "Pestilence, Horseman of the Apocalypse Wins" },
            { 16, "Juggernaut Wins" },
            { 17, "Guardian Angel Wins" },
            { 18, "Pirate Wins" },
            { 19, "Lovers Win" },
            { 20, "Plaguebearer Wins" }
        };

        [HarmonyPatch("RaiseOnGameStarted")]
        [HarmonyPostfix]
        private static void RaiseOnGameStarted(int gameMode)
        {
            RichPresence newPresence = CurrentPresence.Clone();
            //newPresence.Assets.LargeImageKey = background + Selections.background;
            newPresence.Assets.SmallImageKey = inGame;
            newPresence.Assets.SmallImageText = newPresence.State = InGame;
            newPresence.Timestamps = new Timestamps(UtcNow);
            newPresence.Details = GameModes[gameMode];
            DiscordRpcClient.SetPresence(newPresence);
        }

        [HarmonyPatch("RaiseOnFirstDayTransition")]
        [HarmonyPostfix]
        private static void RaiseOnFirstDayTransition(IGameService __instance)
        {
            CurrentPresence.State = GameModes[__instance.ActiveGameState.GameMode.Id];
            RaiseOnDayTransition(1);
        }

        [HarmonyPatch("RaiseOnDayTransition")]
        [HarmonyPostfix]
        private static void RaiseOnDayTransition(int dayNumber) => DiscordRpcClient.UpdateDetails(Format(Day, dayNumber));

        [HarmonyPatch("RaiseOnNight")]
        [HarmonyPostfix]
        private static void RaiseOnNight(int nightNumber) => DiscordRpcClient.UpdateDetails(Format(Night, nightNumber));

        [HarmonyPatch("RaiseOnGameOver")]
        [HarmonyPostfix]
        private static void RaiseOnGameOver(int winningFaction) => DiscordRpcClient.UpdateDetails(Winners[winningFaction]);
    }
}
