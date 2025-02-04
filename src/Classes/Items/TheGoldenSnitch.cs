using Hazel;
using UnityEngine;
using UnityEngine.Events;

namespace HarryPotter.Classes.Items
{
    public class TheGoldenSnitch : Item
    {
        private bool canBeUsedInMeeting;

        public TheGoldenSnitch(ModdedPlayerClass owner)
        {
            this.Owner = owner;
            this.ParentInventory = owner.Inventory;
            this.Id = 3;
            this.Name = "The Golden Snitch";
            this.IsSpecial = true;
            this.Tooltip = "The Golden Snitch:\nForces all votes onto a targeted player.\n<#FF0000FF>Can only be used in meetings!";

            // Assure que l'icône de l'objet est bien initialisée
            SetItemIcon();

            // Vérification de si l'objet peut être utilisé dans une réunion
            canBeUsedInMeeting = true; // À adapter selon la logique de ton jeu (par exemple vérifier si le joueur est en réunion)
        }

        private void SetItemIcon()
        {
            // Assurez-vous que l'icône est bien définie
            this.Icon = Main.Instance.Assets.WorldItemIcons[Id];
        }

        public override void UseItem()
        {
            // Vérifie que l'item ne peut être utilisé que pendant une réunion
            if (canBeUsedInMeeting && IsInMeeting())
            {
                // Logique pour forcer les votes sur un joueur ciblé
                ForceVotesOnTargetPlayer();
            }
            else
            {
                // Logique pour informer l'utilisateur si l'item ne peut pas être utilisé
                ShowUsageError();
            }
        }

        private bool IsInMeeting()
        {
            // Implémente la logique pour vérifier si le joueur est actuellement dans une réunion.
            // Par exemple, vérifier si une session de réunion est en cours dans le jeu.
            return Main.Instance.IsMeetingActive; // Exemple à adapter selon ton propre jeu
        }

        private void ForceVotesOnTargetPlayer()
        {
            // Logique pour forcer les votes d'un joueur ciblé
            // Cette fonction devra être implémentée pour appliquer les votes
            // Exemple de code :
            ModdedPlayerClass targetPlayer = GetTargetPlayer(); 
            if (targetPlayer != null)
            {
                // Applique la logique pour forcer les votes
                ApplyForcedVotes(targetPlayer);
            }
        }

        private void ApplyForcedVotes(ModdedPlayerClass targetPlayer)
        {
            // Implémentation de la logique pour appliquer les votes forcés sur le joueur cible
            // Exemple : ciblez tous les joueurs et redirigez leurs votes vers le joueur ciblé
            // (Cela dépendra des règles spécifiques de ton jeu)
            Debug.Log($"Forcing votes on {targetPlayer.Name}");
        }

        private void ShowUsageError()
        {
            // Affiche un message d'erreur lorsque l'objet ne peut pas être utilisé
            Debug.LogError("The Golden Snitch can only be used during a meeting!");
        }
    }
}
