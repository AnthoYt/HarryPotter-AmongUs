using HarmonyLib;
using HarryPotter.Classes;
using HarryPotter.Classes.Roles;
using UnityEngine;

namespace HarryPotter.Patches
{
    // Patch pour la méthode MoveNext de IntroCutscene._CoBegin_d__11
    [HarmonyPatch(typeof(IntroCutscene._CoBegin_d__11), nameof(IntroCutscene._CoBegin_d__11.MoveNext))]
    class IntroCutscene_CoBegin__d_MoveNext
    {
        // Préfixe pour exécuter avant la méthode MoveNext
        static void Prefix(IntroCutscene._CoBegin_d__11 __instance)
        {
            // Commenté : Si tu veux ajouter une musique ou un thème pour l'intro
            // __instance.__4__this.IntroStinger = Main.Instance.Assets.HPTheme;
        }
        
        // Postfix pour exécuter après la méthode MoveNext
        static void Postfix(IntroCutscene._CoBegin_d__11 __instance)
        {
            // Récupère le joueur local modifié
            ModdedPlayerClass localPlayer = Main.Instance.GetLocalModdedPlayer();
            if (localPlayer == null) return; // Vérification que le joueur existe

            // Si le joueur n'est pas un imposteur, affiche "Muggle"
            if (!localPlayer._Object.Data.IsImpostor)
            {
                __instance.__4__this.Title.text = "Muggle";
            }

            // Vérifie si le joueur a un rôle valide avant de procéder
            if (localPlayer.Role == null) return;

            // Réinitialise les cooldowns du rôle
            localPlayer.Role.ResetCooldowns();

            // Active le texte de l'imposteur
            __instance.__4__this.ImpostorText.gameObject.SetActive(true);
            __instance.__4__this.ImpostorText.transform.localScale = new Vector3(0.7f, 0.7f); // Ajuste la taille du texte

            // Modifie le texte et les couleurs en fonction du rôle du joueur
            __instance.__4__this.Title.text = localPlayer.Role.RoleName;  // Affiche le nom du rôle
            __instance.__4__this.Title.color = localPlayer.Role.RoleColor; // Change la couleur du titre
            __instance.__4__this.ImpostorText.text = localPlayer.Role.IntroString; // Affiche l'intro du rôle
            __instance.__4__this.BackgroundBar.material.color = localPlayer.Role.RoleColor2; // Change la couleur de fond
        }
    }
}
