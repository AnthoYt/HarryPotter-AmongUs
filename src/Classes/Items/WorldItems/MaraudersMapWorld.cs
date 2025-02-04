using System.Linq;
using UnityEngine;
using hunterlib.Classes;

namespace HarryPotter.Classes.WorldItems
{
    public class MaraudersMapWorld : WorldItem
    {
        public static System.Random ItemRandom { get; set; } = new System.Random();
        public static float ItemSpawnChance { get; set; } = 30;
        public static bool HasSpawned { get; set; }

        public MaraudersMapWorld(Vector2 position)
        {
            this.Position = position;
            this.Id = 1;
            this.Icon = Main.Instance.Assets.WorldItemIcons[Id];
            this.Name = "Marauder's Map";
        }

        // Méthode pour spawn l'objet dans le monde
        public static void WorldSpawn()
        {
            if (!CanSpawn())
                return;

            if (!ShipStatus.Instance)
                return;

            // Choisir une position valide pour le spawn
            Vector2 pos = Main.Instance.GetAllApplicableItemPositions().Random();
            Main.Instance.RpcSpawnItem(1, pos);  // Demander au serveur de spawner l'objet
            HasSpawned = true;
        }

        // Vérification des conditions pour spawner l'objet
        public static bool CanSpawn()
        {
            // Vérifier si un Marauder's Map est déjà dans le monde
            if (Main.Instance.AllItems.Any(x => x.Id == 1)) return false;

            // Ne peut pas spawn si une réunion est en cours
            if (MeetingHud.Instance) return false;

            // Vérifie si le jeu a commencé
            if (!AmongUsClient.Instance.IsGameStarted) return false;

            // Vérification de la probabilité d'apparition
            if (ItemRandom.Next(0, 100000) > ItemSpawnChance) return false;

            // Vérifie si l'objet a déjà été spawné
            if (HasSpawned) return false;

            return true;  // L'objet peut apparaître si toutes les conditions sont validées
        }
    }
}
