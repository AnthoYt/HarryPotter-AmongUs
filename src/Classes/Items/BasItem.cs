namespace HarryPotter.Classes.Items
{
    public class BasiliskItem : Item // Renommé pour plus de clarté
    {
        public BasiliskItem(ModdedPlayerClass owner)
        {
            Owner = owner;
            ParentInventory = owner.Inventory;
            Id = 7;
            Name = "Basilisk";
            Tooltip = "A dangerous item that stuns the player who uses it.";
            IsTrap = true;
        }

        public override void Use()
        {
            // On vérifie que l'objet est valide avant de tenter de l'utiliser
            if (Owner._Object != null)
            {
                // Suppression de l'item de l'inventaire
                Delete();

                // Démarre la coroutine pour étourdir le joueur
                hunterlib.Classes.Coroutines.Start(Main.Instance.CoStunPlayer(Owner._Object));
            }
            else
            {
                // Optionnel: vous pourriez vouloir loguer ou gérer l'erreur ici
                UnityEngine.Debug.LogError("L'objet du propriétaire est nul, impossible d'utiliser le Basilisk.");
            }
        }
    }
}
