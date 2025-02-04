using HarmonyLib;
using HarryPotter.Classes;
using Il2CppSystem;
using UnityEngine;

namespace HarryPotter.Patches
{
    // Patch pour la méthode Begin d'OverlayKillAnimation
    [HarmonyPatch(typeof(OverlayKillAnimation), nameof(OverlayKillAnimation.Begin))]
    public class OverlayKillAnimation_Begin
    {
        // Méthode Prefix qui modifie le comportement avant l'exécution de Begin
        static void Prefix(OverlayKillAnimation __instance, ref GameData.PlayerInfo __0, GameData.PlayerInfo __1)
        {
            // Vérification des paramètres pour éviter les erreurs nulles
            if (__0 == null || __1 == null || __instance == null) return;
            if (__0.PlayerId != __1.PlayerId) return; // Si les joueurs n'ont pas le même ID, on quitte

            // Trouver le joueur avec le rôle "Harry"
            ModdedPlayerClass harry = Main.Instance.FindPlayerOfRole("Harry");
            if (harry == null) return; // Si Harry n'est pas trouvé, on quitte

            // Remplacer les données du joueur __0 par celles de Harry
            __0 = harry._Object.Data;

            // Vérification de la présence des parties du corps avant de modifier
            if (__instance.killerParts?.Body != null)
            {
                // Appliquer une transformation à la partie du corps de l'animation
                __instance.killerParts.Body.transform.localScale = new Vector3(0.4f, 0.4f);
                __instance.killerParts.Body.transform.position -= new Vector3(0.3f, 0f, 0f); // Déplacer légèrement le corps
            }
        }
    }
}
