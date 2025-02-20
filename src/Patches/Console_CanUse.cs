using HarmonyLib;
using HarryPotter.Classes;

namespace HarryPotter.Patches
{
    [HarmonyPatch(typeof(Console), nameof(Console.CanUse))]
    public class Console_CanUse
    {
        static void Postfix(GameData.PlayerInfo __0, ref bool __1, ref bool __2)
        {
            // Recherche du joueur moddé
            ModdedPlayerClass moddedPlayer = Main.Instance.ModdedPlayerById(__0.PlayerId);

            // Si aucun joueur moddé n'est trouvé, on ne fait rien
            if (moddedPlayer == null)
                return;

            // Si le joueur moddé ne peut pas utiliser la console, on l'empêche
            if (!moddedPlayer.CanUseConsoles)
            {
                __1 = false;
                __2 = false;
            }
        }
    }
}
