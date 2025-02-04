using HarryPotter.Classes.WorldItems;
using Hazel;
using UnityEngine;

namespace HarryPotter.Classes.Items
{
    public class PortKey : Item
    {
        private bool canTeleport;

        public PortKey(ModdedPlayerClass owner)
        {
            this.Owner = owner;
            this.ParentInventory = owner.Inventory;
            this.Id = 2;
            this.Name = "Port Key";
            this.Tooltip = "Port Key:\nTeleports you to\nthe emergency button.";

            // Initialisation de l'icône
            SetItemIcon();

            // Définition de la logique de téléportation
            canTeleport = true; // Cela peut être configuré selon la logique du jeu (ex: vérifier si la téléportation est autorisée)
        }

        private void SetItemIcon()
        {
            // Assurez-vous que l'icône de l'objet est bien initialisée
            this.Icon = Main.Instance.Assets.ItemIcons[Id];
        }

        public override void Use()
        {
            // Vérification si la téléportation est autorisée (basée sur des conditions spécifiques)
            if (!canTeleport)
            {
                ShowTeleportError();
                return;
            }

            // Gérer l'action du PortKey en fonction de si le joueur est hôte ou non
            if (AmongUsClient.Instance.AmHost)
            {
                HandleHostTeleportation();
            }
            else
            {
                // Si le joueur n'est pas l'hôte, envoyer une demande de téléportation via RPC
                SendTeleportationRequest();
            }

            // Supprimer l'item après utilisation
            this.Delete();

            // Effectuer la téléportation effective du joueur
            PerformTeleportation();
        }

        private void HandleHostTeleportation()
        {
            // Si l'hôte utilise le PortKey, il empêche que l'objet soit à nouveau utilisé
            PortKeyWorld.HasSpawned = false;
        }

        private void SendTeleportationRequest()
        {
            // Crée une demande RPC pour les autres joueurs (lorsque ce n'est pas l'hôte)
            MessageWriter writer = AmongUsClient.Instance.StartRpc(PlayerControl.LocalPlayer.NetId, (byte)Packets.UseItem, SendOption.Reliable);
            writer.Write(Id);
            writer.EndMessage();
        }

        private void PerformTeleportation()
        {
            // Logique pour effectuer la téléportation, en fonction du MapId
            Vector2 teleportDestination = GetTeleportDestination();

            // Effectuer la téléportation avec RPC
            Main.Instance.RpcTeleportPlayer(Owner._Object, teleportDestination);
        }

        private Vector2 GetTeleportDestination()
        {
            // Détermine la destination de téléportation en fonction du MapId
            if (PlayerControl.GameOptions.MapId == 4)
            {
                return new Vector2(7.620923f, 15.0479f); // Exemple d'une position d'urgence pour une carte spécifique
            }
            else
            {
                return ShipStatus.Instance.MeetingSpawnCenter; // Destination par défaut
            }
        }

        private void ShowTeleportError()
        {
            // Affiche un message d'erreur si la téléportation n'est pas autorisée
            Debug.LogError("Teleportation with Port Key is not allowed right now!");
        }
    }
}
