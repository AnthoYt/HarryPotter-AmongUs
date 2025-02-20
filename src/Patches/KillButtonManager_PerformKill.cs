using HarmonyLib;
using HarryPotter.Classes;
using HarryPotter.Classes.Roles;
using HarryPotter.Classes.UI;
using UnityEngine;

namespace HarryPotter.Patches
{
    [HarmonyPatch(typeof(KillButtonManager), nameof(KillButtonManager.PerformKill))]
    class KillButtonManager_PerformKill
    {
        static bool Prefix(KillButtonManager __instance)
        {
            // Vérification si le bouton de tuerie est celui de l'interface et si le joueur moddé est Bellatrix et sous contrôle mental
            var localPlayer = Main.Instance.GetLocalModdedPlayer();
            if (__instance == HudManager.Instance.KillButton && localPlayer?.Role?.RoleName == "Bellatrix")
            {
                var bellatrixRole = (Bellatrix)localPlayer.Role;
                var mindControlledPlayer = bellatrixRole.MindControlledPlayer;

                // Si un joueur est sous contrôle mental et la cible n'est pas déjà définie, on procède à l'exécution
                if (mindControlledPlayer != null && HudManager.Instance.KillButton.CurrentTarget != null && !Main.Instance.ControlKillUsed)
                {
                    Main.Instance.ControlKillUsed = true;
                    Main.Instance.RpcKillPlayer(mindControlledPlayer._Object, HudManager.Instance.KillButton.CurrentTarget, true);
                }
                return false; // On empêche l'exécution de la méthode originale
            }

            // Si le joueur ne peut pas bouger, on ne fait rien
            if (!PlayerControl.LocalPlayer.CanMove) return false;

            // Si le joueur a un rôle, on appelle la méthode PerformKill spécifique à ce rôle
            if (localPlayer?.Role != null)
            {
                return localPlayer.Role.PerformKill(__instance);
            }

            return true; // Si aucune des conditions précédentes n'est remplie, on laisse l'exécution continuer
        }
    }
}
