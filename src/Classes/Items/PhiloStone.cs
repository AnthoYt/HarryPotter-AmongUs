using HarryPotter.Classes.WorldItems;
using Hazel;

namespace HarryPotter.Classes.Items
{
    public class PhiloStone : Item
    {
        private bool hasBeenUsed;

        public PhiloStone(ModdedPlayerClass owner)
        {
            this.Owner = owner;
            this.ParentInventory = owner.Inventory;
            this.Id = 9;
            this.Name = "Philosopher's Stone";
            this.Tooltip = "Philosopher's Stone:\nThis item will revive you when you die.\n<#FF0000FF>This item will be automatically consumed.";
            this.Icon = Main.Instance.Assets.WorldItemIcons[Id];
            this.IsSpecial = true;

            // Initialisation pour savoir si l'item a déjà été utilisé
            hasBeenUsed = false;
        }

        public override void Use()
        {
            // Vérification si l'item a déjà été utilisé
            if (hasBeenUsed)
            {
                ShowItemAlreadyUsedMessage();
                return;
            }

            // Vérifie si le joueur est mort et peut être réanimé
            if (Owner._Object.Data.IsDead)
            {
                RevivePlayer();
            }
            else
            {
                ShowErrorMessage("You are not dead, you cannot use the Philosopher's Stone!");
            }
        }

        private void RevivePlayer()
        {
            // Réanime le joueur
            Main.Instance.RpcRevivePlayer(Owner._Object);
            hasBeenUsed = true;

            // Supprime l'objet après utilisation
            this.Delete();
        }

        private void ShowItemAlreadyUsedMessage()
        {
            // Affiche un message si l'item a déjà été utilisé
            Debug.LogWarning("The Philosopher's Stone has already been used and consumed.");
        }

        private void ShowErrorMessage(string message)
        {
            // Affiche un message d'erreur si la condition pour utiliser l'item n'est pas remplie
            Debug.LogError(message);
        }
    }
}
