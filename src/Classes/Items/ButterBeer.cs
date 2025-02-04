using HarryPotter.Classes.WorldItems;
using Hazel;

namespace HarryPotter.Classes.Items
{
    public class ButterBeer : Item
    {
        public ButterBeer(ModdedPlayerClass owner)
        {
            this.Owner = owner;
            this.ParentInventory = owner.Inventory;
            this.Id = 5;
            this.Name = "Butter Beer";
            this.Tooltip = "A magical drink with unexpected effects."; // Ajout d'un texte de tooltip descriptif
            this.IsTrap = true; // Indique que cet objet est un piège
        }

        public override void Use()
        {
            Delete(); // Supprime l'objet après son utilisation

            // Lance la coroutine pour activer l'effet du Butter Beer
            hunterlib.Classes.Coroutines.Start(Main.Instance.CoActivateButterBeer(Owner._Object));
        }
    }
}
