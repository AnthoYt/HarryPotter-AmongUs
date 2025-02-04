using System;
using System.Linq;
using Hazel;
using UnityEngine;
using hunterlib.Classes;

namespace HarryPotter.Classes.WorldItems
{
    public class TheGoldenSnitchWorld : WorldItem
    {
        public static System.Random ItemRandom { get; set; } = new System.Random();
        public static float ItemSpawnChance { get; set; } = 30;
        public static bool HasSpawned { get; set; }
        public Vector2 Velocity { get; set; }

        public TheGoldenSnitchWorld(Vector2 position, Vector2 velocity)
        {
            this.Position = position;
            this.Velocity = velocity;
            this.Id = 3;
            this.Icon = Main.Instance.Assets.WorldItemIcons[Id];
            this.Name = "The Golden Snitch";
        }

        private double _time;

        public override void DrawWorldIcon()
        {
            // Vérifie si l'objet du monde n'a pas été créé
            if (ItemWorldObject == null)
            {
                // Création de l'objet du monde
                System.Console.WriteLine("Creating new Item: " + Name);
                ItemWorldObject = new GameObject(Name); // Ajout d'un nom à l'objet pour faciliter le débogage
                var spriteRenderer = ItemWorldObject.AddComponent<SpriteRenderer>();
                spriteRenderer.sprite = Icon;
                spriteRenderer.enabled = true;
                spriteRenderer.transform.localScale = new Vector2(0.5f, 0.5f);
                ItemWorldObject.transform.position = Position;

                // Ajout du Rigidbody2D et Collider2D
                var itemRigid = ItemWorldObject.AddComponent<Rigidbody2D>();
                var itemCollider = ItemWorldObject.AddComponent<BoxCollider2D>();

                // Paramétrage du collider et des propriétés physiques
                itemCollider.autoTiling = false;
                itemCollider.edgeRadius = 0;
                itemCollider.size = Icon.bounds.size * 0.5f;
                itemCollider.sharedMaterial = Main.Instance.Assets.SnitchMaterial;
                ItemWorldObject.layer = 8;

                itemRigid.velocity = Velocity; // Définir la vélocité initiale
            }

            // Configuration du Rigidbody2D pour un mouvement lisse
            var itemRigid2 = ItemWorldObject.GetComponent<Rigidbody2D>();
            itemRigid2.fixedAngle = true;
            itemRigid2.drag = 0;
            itemRigid2.angularDrag = 0;
            itemRigid2.inertia = 0;
            itemRigid2.gravityScale = 0;

            // Animation de rotation basée sur une sinusoïde
            _time += Time.deltaTime;
            var angle = (float)(25 + (25 * Math.Sin(_time)));
            itemRigid2.rotation = angle - 32.5f;
        }

        // Méthode statique pour faire apparaître l'objet dans le monde
        public static void WorldSpawn()
        {
            // Vérifie les conditions de spawn avant de créer l'objet
            if (!CanSpawn())
                return;

            if (!ShipStatus.Instance)
                return;

            Vector2 pos = Main.Instance.GetAllApplicableItemPositions().Random(); // Sélectionne une position aléatoire
            Main.Instance.RpcSpawnItem(3, pos); // Demande au serveur de faire apparaître l'objet
            HasSpawned = true; // Marque l'objet comme ayant été spawné
        }

        // Conditions de spawn de l'objet
        public static bool CanSpawn()
        {
            // Vérifie si l'objet est déjà présent dans le monde
            if (Main.Instance.AllItems.Where(x => x.Id == 3).Any()) return false;

            // Ne peut pas apparaître en réunion
            if (MeetingHud.Instance) return false;

            // Vérifie si le jeu a commencé
            if (!AmongUsClient.Instance.IsGameStarted) return false;

            // Vérifie la probabilité d'apparition de l'objet
            if (ItemRandom.Next(0, 100000) > ItemSpawnChance) return false;

            // Vérifie le stade du jeu (par exemple, ne spawn pas avant un certain stade)
            if (Main.Instance.CurrentStage < 2) return false;

            // Vérifie si l'objet a déjà été spawné
            if (HasSpawned) return false;

            return true; // Si toutes les conditions sont validées, on autorise le spawn
        }
    }
}
