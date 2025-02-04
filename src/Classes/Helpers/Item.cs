using System.Collections.Generic;
using UnityEngine;

namespace HarryPotter.Classes
{
    public class Item
    {
        public int Id { get; set; } // L'identifiant unique de l'objet
        public string Name { get; set; } // Le nom de l'objet
        public Sprite Icon { get; set; } // L'icône associée à l'objet
        public List<Item> ParentInventory { get; set; } // Inventaire parent de cet objet
        public ModdedPlayerClass Owner { get; set; } // Le joueur qui possède l'objet
        public bool IsSpecial { get; set; } // Indique si l'objet est spécial (fonctionnalité spéciale)
        public bool IsTrap { get; set; } // Indique si l'objet est une piège
        public string Tooltip { get; set; } // Description de l'objet à afficher dans l'interface

        // Méthode virtuelle à surcharger dans des classes dérivées pour définir l'action de l'objet lorsqu'il est utilisé
        public virtual void Use() { }

        // Méthode pour supprimer l'objet de l'inventaire du propriétaire
        public void Delete()
        {
            // Si l'inventaire parent existe, on enlève l'objet
            if (ParentInventory != null)
            {
                ParentInventory.Remove(this);
                ParentInventory = null; // Dissocie l'inventaire
            }
        }
    }
}
