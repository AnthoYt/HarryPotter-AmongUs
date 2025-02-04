using System.Collections.Generic;
using System.Linq;
using HarryPotter.Classes.Items;
using Hazel;
using Rewired;
using UnityEngine;
using hunterlib.Classes;

namespace HarryPotter.Classes
{
    public class WorldItem
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public Sprite Icon { get; set; }
        public Vector2 Position { get; set; }
        public GameObject ItemWorldObject { get; set; }
        public bool IsPickedUp { get; set; }

        // Mise à jour de l'item, pour gérer la prise en charge du pick-up
        public void Update()
        {
            // Ne rien faire si l'objet n'existe pas ou si le joueur est mort ou déconnecté
            if (ItemWorldObject == null || PlayerControl.LocalPlayer.Data.IsDead || PlayerControl.LocalPlayer.Data.Disconnected)
                return;

            // Si l'objet est déjà dans l'inventaire du joueur, ne pas essayer de le prendre
            if (Main.Instance.GetLocalModdedPlayer().HasItem(Id)) return;

            // Vérifier si l'objet est à portée du joueur pour un pick-up
            if (ItemWorldObject.GetComponent<SpriteRenderer>().bounds.Intersects(PlayerControl.LocalPlayer.myRend.bounds))
                PickUp();
        }

        // Dessiner l'icône de l'objet dans le monde
        public virtual void DrawWorldIcon()
        {
            // Créer l'objet s'il n'existe pas encore
            if (ItemWorldObject == null)
            {
                ItemWorldObject = new GameObject();
                ItemWorldObject.AddComponent<SpriteRenderer>();
                ItemWorldObject.SetActive(true);
            }

            // Configuration du SpriteRenderer pour afficher l'icône
            SpriteRenderer itemRender = ItemWorldObject.GetComponent<SpriteRenderer>();
            itemRender.enabled = true;
            itemRender.sprite = Icon;
            itemRender.transform.localScale = new Vector2(0.5f, 0.5f);  // Ajuster la taille de l'icône
            ItemWorldObject.transform.position = Position;
        }

        // Gérer le pick-up de l'objet
        public void PickUp()
        {
            // Si l'on est l'hôte, attribuer l'objet directement
            if (AmongUsClient.Instance.AmHost)
                Main.Instance.GiveGrabbedItem(this.Id);
            else
            {
                // Si ce n'est pas l'hôte, envoyer une requête RPC pour le pick-up
                Delete();  // Supprimer l'objet localement
                MessageWriter writer = AmongUsClient.Instance.StartRpc(PlayerControl.LocalPlayer.NetId, (byte)Packets.TryPickupItem, SendOption.Reliable);
                writer.Write(PlayerControl.LocalPlayer.PlayerId);
                writer.Write(this.Id);
                writer.EndMessage();
            }
        }

        // Supprimer l'objet du monde
        public void Delete()
        {
            IsPickedUp = true;
            if (ItemWorldObject != null)
                ItemWorldObject.Destroy();  // Détruire l'objet du monde

            // Si l'on est l'hôte, informer les autres joueurs que l'objet a été détruit
            if (AmongUsClient.Instance.AmHost)
            {
                MessageWriter writer = AmongUsClient.Instance.StartRpc(PlayerControl.LocalPlayer.NetId, (byte)Packets.DestroyItem, SendOption.Reliable);
                writer.Write(this.Id);
                writer.EndMessage();
            }
        }
    }
}
