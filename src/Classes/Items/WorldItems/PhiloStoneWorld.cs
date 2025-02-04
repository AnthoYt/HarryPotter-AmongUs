using System;
using System.Linq;
using Hazel;
using UnityEngine;
using Random = UnityEngine.Random;
using hunterlib.Classes;

namespace HarryPotter.Classes.WorldItems
{
    public class PhiloStoneWorld : WorldItem
    {
        // Propriétés statiques pour la gestion de l'objet
        public static System.Random ItemRandom { get; set; } = new System.Random();
        public static float ItemSpawnChance { get; set; } = 30; // Chance que l'objet apparaisse (en pourcentage)
        public static bool HasSpawned { get; set; } // Indique si l'objet a déjà été spawn

        // Constructeur qui initialise les propriétés de l'objet
        public PhiloStoneWorld(Vector2 position)
        {
            this.Position = position;
            this.Id = 9;
            this.Icon = Main.Instance.Assets.WorldItemIcons[Id]; // Charge l'icône associée à l'objet
            this.Name = "Philosopher's Stone"; // Nom de l'objet
        }

        // Méthode statique pour gérer l'apparition de l'objet dans le monde
        public static void WorldSpawn()
        {
            // Si l'objet ne peut pas apparaître, on arrête l'exécution
            if (!CanSpawn())
                return;

            // Si le vaisseau n'est pas encore disponible, on arrête l'exécution
            if (!ShipStatus.Instance)
                return;

            // Choisit une position aléatoire parmi les positions valides disponibles
            Vector2 pos = Main.Instance.GetAllApplicableItemPositions().Random();
            Main.Instance.RpcSpawnItem(9, pos); // Appel pour spawn l'objet à la position choisie
            HasSpawned = true; // Marque que l'objet a été spawn
        }

        // Méthode statique pour vérifier si l'objet peut apparaître
        public static bool CanSpawn()
        {
            // Vérifie si l'objet est déjà présent dans la scène
            if (Main.Instance.AllItems.Any(x => x.Id == 9))
                return false;

            // Vérifie si une réunion est en cours
            if (MeetingHud.Instance)
                return false;

            // Vérifie si le jeu a commencé
            if (!AmongUsClient.Instance.IsGameStarted)
                return false;

            // Vérifie si l'objet a suffisamment de chances d'apparaître en fonction de la probabilité
            if (ItemRandom.Next(0, 100000) > ItemSpawnChance * 1000)
                return false;

            // Vérifie que le stage actuel est suffisant pour permettre l'apparition
            if (Main.Instance.CurrentStage < 1)
                return false;

            // Vérifie si l'objet a déjà été spawn
            if (HasSpawned)
                return false;

            return true; // L'objet peut apparaître
        }
    }
}
