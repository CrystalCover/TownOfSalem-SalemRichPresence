using DiscordRPC;
using HarmonyLib;
using static Scene;
using static System.DateTime;

namespace Eca.SalemRichPresence
{
    [HarmonyPatch(typeof(IApplicationService), "RaiseOnSceneLoaded")]
    internal class IApplicationServiceExtra
    {
        private static string coven => "coven";

        private static string tos => "tos";

        private static string offline => "offline";

        private static string Offline => "Offline";

        private static string online => "online";

        private static string Online => "Online";

        private static DiscordRpcClient DiscordRpcClient => Plugin.DiscordRpcClient;

        private static RichPresence CurrentPresence => DiscordRpcClient.CurrentPresence;

        internal static Timestamps Timestamps { get; set; }

        private static void Postfix(Scene scene)
        {
            string largeImageKey;
            string smallImageKey;
            string smallImageText;
            string state;
            switch (scene)
            {
                case Login:
                case BigLogin:
                    largeImageKey = GlobalServiceLocator.SteamService.IsCovenDLCOwned() ? coven : tos;
                    smallImageKey = offline;
                    smallImageText = state = Offline;
                    Timestamps = null;
                    break;
                case Home:
                case BigHome:
                    largeImageKey = GlobalServiceLocator.UserService.AccountFlags.OwnsCoven ? coven : tos;
                    smallImageKey = online;
                    smallImageText = state = Online;
                    Timestamps = Timestamps ?? new Timestamps(UtcNow);
                    break;
                default:
                    return;
            }
            RichPresence newPresence = CurrentPresence?.Clone() ?? new RichPresence() { Assets = new Assets() };
            newPresence.Assets.LargeImageKey = largeImageKey;
            newPresence.Assets.SmallImageKey = smallImageKey;
            newPresence.Assets.SmallImageText = smallImageText;
            newPresence.Timestamps = Timestamps;
            newPresence.State = state;
            newPresence.Details = string.Empty;
            DiscordRpcClient.SetPresence(newPresence);
        }
    }
}
