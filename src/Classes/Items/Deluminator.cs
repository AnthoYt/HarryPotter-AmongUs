using System;
using HarryPotter.Classes.WorldItems;
using Hazel;

namespace HarryPotter.Classes.Items
{
    public class Deluminator : Item
    {
        public Deluminator(ModdedPlayerClass owner)
        {
            this.Owner = owner;
            this.ParentInventory = owner.Inventory;
            this.Id = 0;
            this.Icon = Main.Instance.Assets.ItemIcons[Id];
            this.Name = "Deluminator";
            this.Tooltip = "Deluminator:\nToggles the status of the lights.";
        }

        public override void Use()
        {
            // Vérification si l'hôte utilise l'objet
            if (AmongUsClient.Instance.AmHost)
                DeluminatorWorld.HasSpawned = false;
            else
            {
                // Envoi du message RPC pour l'utilisation de l'item
                MessageWriter writer = AmongUsClient.Instance.StartRpc(PlayerControl.LocalPlayer.NetId, (byte)Packets.UseItem, SendOption.Reliable);
                writer.Write(Id);
                writer.EndMessage();
            }

            // Suppression de l'objet après utilisation
            this.Delete();

            // Vérification de l'état des lumières (sabotées ou non)
            if (Main.Instance.IsLightsSabotaged())
            {
                // Lumières sabotées - on les répare
                var switchSystem = ShipStatus.Instance.Systems[SystemTypes.Electrical] as SwitchSystem;

                if (switchSystem != null)
                {
                    // Réinitialisation de l'état des interrupteurs
                    switchSystem.ActualSwitches = switchSystem.ExpectedSwitches;

                    // Envoi du message RPC pour réparer les lumières
                    MessageWriter repairWriter = AmongUsClient.Instance.StartRpc(PlayerControl.LocalPlayer.NetId, (byte)Packets.FixLightsRpc, SendOption.Reliable);
                    repairWriter.EndMessage();
                }
            }
            else
            {
                // Lumières non sabotées - on déclenche un sabotage
                byte b = 4;
                for (var i = 0; i < 5; i++)
                {
                    // Créer un effet aléatoire de sabotage
                    if (new Random().Next(0, 2) == 0)
                        b |= (byte)(1 << i);
                }

                // Envoi du message RPC pour saboter les lumières
                ShipStatus.Instance.RpcRepairSystem(SystemTypes.Electrical, b | 128);
            }
        }
    }
}
