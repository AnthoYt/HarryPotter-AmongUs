using Hazel;
using UnityEngine;
using UnityEngine.Events;

namespace HarryPotter.Classes.Items
{
    public class SortingHat : Item
    {
        private bool canBeUsedInMeeting;

        public SortingHat(ModdedPlayerClass owner)
        {
            this.Owner = owner;
            this.ParentInventory = owner.Inventory;
            this.Id = 8;
            this.Name = "Sorting Hat";
            this.IsSpecial = true;
            this.Tooltip = "Sorting Hat:\nReveals the role of the targeted player.\n<#FF0000FF>Can only be used in meetings!";

            // Assurez-vous que l'icône de l'objet est bien initialisée
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
                // Logique pour révéler le rôle du joueur ciblé
                RevealTargetPlayerRole();
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

        private void RevealTargetPlayerRole()
        {
            // Logique pour révéler le rôle du joueur ciblé
            // Cette fonction devra être implémentée pour afficher ou révéler le rôle du joueur ciblé
            ModdedPlayerClass targetPlayer = GetTargetPlayer(); 
            if (targetPlayer != null)
            {
                // Applique la logique pour révéler le rôle du joueur
                RevealRole(targetPlayer);
            }
        }

        private void RevealRole(ModdedPlayerClass targetPlayer)
        {
            // Implémentation de la logique pour révéler le rôle du joueur cible
            // Exemple : afficher le rôle du joueur dans un message ou une UI
            Debug.Log($"Revealing role of {targetPlayer.Name}: {targetPlayer.RoleName}");
        }

        private void ShowUsageError()
        {
            // Affiche un message d'erreur lorsque l'objet ne peut pas être utilisé
            Debug.LogError("The Sorting Hat can only be used during a meeting!");
        }
    }
}
