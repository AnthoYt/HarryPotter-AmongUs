using HarryPotter.Classes.WorldItems;
using Hazel;

namespace HarryPotter.Classes.Items
{
    public class GhostStone : Item
    {
        private bool isActive;

        public GhostStone(ModdedPlayerClass owner)
        {
            this.Owner = owner;
            this.ParentInventory = owner.Inventory;
            this.Id = 4;
            this.Name = "Resurrection Stone";
            this.Tooltip = "Resurrection Stone:\nAllows you to see ghosts.\n<#FF0000FF>This item cannot be consumed.";
            this.Icon = Main.Instance.Assets.WorldItemIcons[Id];
            this.IsSpecial = true;
            
            isActive = false; // Initialisation de l'état de l'item
        }

        public override void Use()
        {
            // Vérifie si l'item est déjà actif
            if (isActive)
            {
                ShowAlreadyActiveMessage();
                return;
            }

            // Active la capacité de voir les fantômes
            ActivateGhostVision();
        }

        private void ActivateGhostVision()
        {
            // Active la capacité de voir les fantômes
            isActive = true;
            Main.Instance.RpcActivateGhostVision(Owner._Object);

            // Affiche un message pour indiquer que l'item a été utilisé
            ShowActivationMessage();
        }

        private void ShowAlreadyActiveMessage()
        {
            // Affiche un message indiquant que l'item est déjà actif
            Debug.LogWarning("The Resurrection Stone is already active and you are currently seeing ghosts.");
        }

        private void ShowActivationMessage()
        {
            // Affiche un message pour confirmer que l'item a été activé
            Debug.Log("The Resurrection Stone has been activated! You can now see ghosts.");
        }

        // Méthode pour désactiver la capacité de voir les fantômes si nécessaire
        public void DeactivateGhostVision()
        {
            if (!isActive) return;

            isActive = false;
            Main.Instance.RpcDeactivateGhostVision(Owner._Object);

            // Affiche un message pour indiquer que l'activation a été annulée
            Debug.Log("The Resurrection Stone has been deactivated.");
        }
    }
}
