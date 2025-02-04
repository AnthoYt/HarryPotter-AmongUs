using System.Linq;
using UnityEngine;
using hunterlib.Classes;

namespace HarryPotter.Classes.WorldItems
{
    public class PortKeyWorld : WorldItem
    {
        public static System.Random ItemRandom { get; set; } = new System.Random();
        public static float ItemSpawnChance { get; set; } = 30;
        public static bool HasSpawned { get; set; }

        public PortKeyWorld(Vector2 position)
        {
            this.Position = position;
            this.Id = 2;
            this.Icon = Main.Instance.Assets.WorldItemIcons[Id];
            this.Name = "Port Key";
        }

        // Méthode de spawn de l'objet dans le monde
        public static void WorldSpawn()
        {
            if (!CanSpawn()) return;

            if (!ShipStatus.Instance)
                return;

            Vector2 pos = Main.Instance.GetAllApplicableItemPositions().Random();
            Main.Instance.RpcSpawnItem(2, pos); // Demander au serveur de faire apparaître l'objet
            HasSpawned = true;
        }

        // Vérification des conditions de spawn
        public static bool CanSpawn()
        {
            // Vérifie si un Port Key est déjà dans le monde
            if (Main.Instance.AllItems.Any(x => x.Id == 2)) return false;

            // Ne peut pas apparaître en réunion
            if (MeetingHud.Instance) return false;

            // Vérifie si le jeu a commencé
            if (!AmongUsClient.Instance.IsGameStarted) return false;

            // Vérifie la probabilité d'apparition de l'objet
            if (ItemRandom.Next(0, 100000) > ItemSpawnChance) return false;

            // Vérifie si l'objet a déjà été spawné
            if (HasSpawned) return false;

            return true; // Si toutes les conditions sont remplies, l'objet peut apparaître
        }
    }
}
