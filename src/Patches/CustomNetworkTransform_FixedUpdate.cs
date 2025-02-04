using HarmonyLib;
using HarryPotter.Classes;
using System.Linq;
using UnityEngine;

namespace HarryPotter.Patches
{
    [HarmonyPatch(typeof(CustomNetworkTransform), nameof(CustomNetworkTransform.FixedUpdate))]
    class CustomNetworkTransform_FixedUpdate
    {
        static bool Prefix(CustomNetworkTransform __instance)
        {
            // Vérification que Main.Instance et PlayerControl.LocalPlayer ne sont pas null
            if (Main.Instance == null || PlayerControl.LocalPlayer == null)
                return true; // On permet au code original de continuer s'il y a un problème d'initialisation

            // Si ce n'est pas le propriétaire, on continue avec les actions suivantes
            if (!__instance.AmOwner)
            {
                // Vérification que le joueur local est bien le contrôleur du transform
                if (Main.Instance.AllPlayers.Any(x => x._Object.NetTransform == __instance && x.ControllerOverride == Main.Instance.GetLocalModdedPlayer()))
                    return false; // Empêcher l'exécution du code de mise à jour pour ce cas particulier

                // Vérification et interpolation du mouvement
                if (__instance.interpolateMovement != 0f)
                {
                    Vector2 vector = __instance.targetSyncPosition - __instance.body.position;

                    // Si le déplacement est suffisamment grand, on le traite
                    if (vector.sqrMagnitude >= 0.0001f)
                    {
                        float num = __instance.interpolateMovement / __instance.sendInterval;
                        vector.x *= num;
                        vector.y *= num;

                        // Calcul de la vitesse pour le joueur local
                        if (PlayerControl.LocalPlayer != null)
                        {
                            // On suppose un multiplicateur de vitesse basé sur le premier joueur trouvé avec ce transform
                            var foundPlayer = Main.Instance.AllPlayers.FirstOrDefault(x => x._Object.NetTransform == __instance);
                            if (foundPlayer != null)
                            {
                                float multiplier = foundPlayer.SpeedMultiplier;
                                vector = Vector2.ClampMagnitude(vector, PlayerControl.LocalPlayer.MyPhysics.TrueSpeed * multiplier);
                            }
                        }

                        __instance.body.velocity = vector;
                    }
                    else
                    {
                        __instance.body.velocity = Vector2.zero; // Aucune vitesse si trop petit mouvement
                    }
                }

                // Mise à jour de la position cible avec la vélocité
                __instance.targetSyncPosition += __instance.targetSyncVelocity * Time.fixedDeltaTime * 0.1f;
                return false; // Bloque l'exécution du code original
            }

            return true; // Permet à l'exécution du code original de continuer si c'est le propriétaire
        }
    }
}
