using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Hazel;
using HarryPotter.Classes.Items;
using HarryPotter.Classes.Roles;
using HarryPotter.Classes.WorldItems;
using HunterLib.Classes;
using Il2CppSystem.Net;

namespace HarryPotter.Classes
{
    public enum Packets
    {
        AssignRole = 70,
        FixLightsRpc = 71,
        ForceAllVotes = 72,
        CreateCurse = 73,
        DestroyCurse = 74,
        KillPlayerUnsafe = 75,
        DeactivatePlayer = 76,
        DestroyCrucio = 77,
        CreateCrucio = 78,
        StartControlling = 79,
        MoveControlledPlayer = 80,
        InvisPlayer = 81,
        DefensiveDuelist = 82,
        RevivePlayer = 83,
        TeleportPlayer = 84,
        SpawnItem = 85,
        TryPickupItem = 86,
        GiveItem = 87,
        DestroyItem = 88,
        UseItem = 89,
        UpdateSpeedMultiplier = 90,
        RevealRole = 91,
        FakeKill = 92,
        FinallyDie = 93,
        RequestRole = 94,
    }

    public class CustomRpc
    {
        public void Handle(byte packetId, MessageReader reader)
        {
            switch (packetId)
            {
                // Role Assignment
                case (byte)Packets.AssignRole:
                    HandleAssignRole(reader);
                    break;

                // Role Request
                case (byte)Packets.RequestRole:
                    HandleRequestRole(reader);
                    break;

                // Player Death
                case (byte)Packets.FinallyDie:
                    HandleFinallyDie(reader);
                    break;

                // Fake Kill
                case (byte)Packets.FakeKill:
                    HandleFakeKill(reader);
                    break;

                // Lights Fix
                case (byte)Packets.FixLightsRpc:
                    HandleFixLightsRpc();
                    break;

                // Force All Votes
                case (byte)Packets.ForceAllVotes:
                    HandleForceAllVotes(reader);
                    break;

                // Curse Creation
                case (byte)Packets.CreateCurse:
                    HandleCreateCurse(reader);
                    break;

                // Crucio Creation
                case (byte)Packets.CreateCrucio:
                    HandleCreateCrucio(reader);
                    break;

                // Curse Destruction
                case (byte)Packets.DestroyCurse:
                    HandleDestroyCurse();
                    break;

                // Crucio Destruction
                case (byte)Packets.DestroyCrucio:
                    HandleDestroyCrucio();
                    break;

                // Unsafe Kill
                case (byte)Packets.KillPlayerUnsafe:
                    HandleKillPlayerUnsafe(reader);
                    break;

                // Player Deactivation
                case (byte)Packets.DeactivatePlayer:
                    HandleDeactivatePlayer(reader);
                    break;

                // Player Control
                case (byte)Packets.StartControlling:
                    HandleStartControlling(reader);
                    break;

                // Controlled Player Movement
                case (byte)Packets.MoveControlledPlayer:
                    HandleMoveControlledPlayer(reader);
                    break;

                // Player Invisibility
                case (byte)Packets.InvisPlayer:
                    HandleInvisPlayer(reader);
                    break;

                // Defensive Duelist Activation
                case (byte)Packets.DefensiveDuelist:
                    HandleDefensiveDuelist(reader);
                    break;

                // Revive Player
                case (byte)Packets.RevivePlayer:
                    HandleRevivePlayer(reader);
                    break;

                // Player Teleport
                case (byte)Packets.TeleportPlayer:
                    HandleTeleportPlayer(reader);
                    break;

                // Item Spawn
                case (byte)Packets.SpawnItem:
                    HandleSpawnItem(reader);
                    break;

                // Item Pickup Try
                case (byte)Packets.TryPickupItem:
                    HandleTryPickupItem(reader);
                    break;

                // Item Give
                case (byte)Packets.GiveItem:
                    HandleGiveItem(reader);
                    break;

                // Item Destroy
                case (byte)Packets.DestroyItem:
                    HandleDestroyItem(reader);
                    break;

                // Use Item
                case (byte)Packets.UseItem:
                    HandleUseItem(reader);
                    break;

                // Speed Multiplier Update
                case (byte)Packets.UpdateSpeedMultiplier:
                    HandleUpdateSpeedMultiplier(reader);
                    break;

                // Reveal Role
                case (byte)Packets.RevealRole:
                    HandleRevealRole(reader);
                    break;

                default:
                    break;
            }
        }

        // Helper Methods for Each Packet Type
        private void HandleAssignRole(MessageReader reader)
        {
            byte playerId = reader.ReadByte();
            string roleName = reader.ReadString();
            ModdedPlayerClass rolePlayer = Main.Instance.ModdedPlayerById(playerId);
            switch (roleName)
            {
                case "Voldemort": rolePlayer.Role = new Voldemort(rolePlayer); break;
                case "Bellatrix": rolePlayer.Role = new Bellatrix(rolePlayer); break;
                case "Harry": rolePlayer.Role = new Harry(rolePlayer); break;
                case "Hermione": rolePlayer.Role = new Hermione(rolePlayer); break;
                case "Ron": rolePlayer.Role = new Ron(rolePlayer); break;
            }
        }

        private void HandleRequestRole(MessageReader reader)
        {
            if (AmongUsClient.Instance.AmHost)
            {
                byte requesterId = reader.ReadByte();
                string requestedRole = reader.ReadString();

                if (Main.Instance.PlayersWithRequestedRoles.All(x => x.Item1.PlayerId != requesterId))
                    Main.Instance.PlayersWithRequestedRoles.Add(new Pair<PlayerControl, string>(GameData.Instance.GetPlayerById(requesterId).Object, requestedRole));
            }
        }

        private void HandleFinallyDie(MessageReader reader)
        {
            byte finallyDeadId = reader.ReadByte();
            Main.Instance.PlayerDie(Main.Instance.ModdedPlayerById(finallyDeadId)._Object);
        }

        private void HandleFakeKill(MessageReader reader)
        {
            byte fakeKilledId = reader.ReadByte();
            Coroutines.Start(Main.Instance.CoFakeKill(Main.Instance.ModdedPlayerById(fakeKilledId)._Object));
        }

        private void HandleFixLightsRpc()
        {
            var switchSystem = ShipStatus.Instance.Systems[SystemTypes.Electrical].Cast<SwitchSystem>();
            switchSystem.ActualSwitches = switchSystem.ExpectedSwitches;
        }

        private void HandleForceAllVotes(MessageReader reader)
        {
            byte forcePlayer = reader.ReadByte();
            Main.Instance.ForceAllVotes((sbyte)forcePlayer);
        }

        private void HandleCreateCurse(MessageReader reader)
        {
            byte casterId = reader.ReadByte();
            Vector2 direction = new Vector2(reader.ReadSingle(), reader.ReadSingle());
            Main.Instance.CreateCurse(direction, Main.Instance.ModdedPlayerById(casterId));
        }

        private void HandleCreateCrucio(MessageReader reader)
        {
            byte blinderId = reader.ReadByte();
            Vector2 crucioDirection = new Vector2(reader.ReadSingle(), reader.ReadSingle());
            Main.Instance.CreateCrucio(crucioDirection, Main.Instance.ModdedPlayerById(blinderId));
        }

        private void HandleDestroyCurse()
        {
            Main.Instance.DestroySpell("_curse");
        }

        private void HandleDestroyCrucio()
        {
            Main.Instance.DestroySpell("_crucio");
        }

        private void HandleKillPlayerUnsafe(MessageReader reader)
        {
            byte killerId = reader.ReadByte();
            byte targetId = reader.ReadByte();
            bool isCurseKill = reader.ReadBoolean();
            bool forceAnim = reader.ReadBoolean();
            ModdedPlayerClass target = Main.Instance.ModdedPlayerById(targetId);
            ModdedPlayerClass killer = Main.Instance.ModdedPlayerById(killerId);
            Main.Instance.KillPlayer(killer._Object, target._Object, isCurseKill, forceAnim);
        }

        private void HandleDeactivatePlayer(MessageReader reader)
        {
            byte blindId = reader.ReadByte();
            ModdedPlayerClass blind = Main.Instance.ModdedPlayerById(blindId);
            Main.Instance.CrucioBlind(blind._Object);
        }

        private void HandleStartControlling(MessageReader reader)
        {
            byte controllerId = reader.ReadByte();
            byte controlledId = reader.ReadByte();
            ModdedPlayerClass controller = Main.Instance.ModdedPlayerById(controllerId);
            ModdedPlayerClass controlled = Main.Instance.ModdedPlayerById(controlledId);
            Main.Instance.ControlPlayer(controller._Object, controlled._Object);
        }

        private void HandleMoveControlledPlayer(MessageReader reader)
        {
            byte moveId = reader.ReadByte();
            Vector3 newVel = new Vector3(reader.ReadSingle(), reader.ReadSingle());
            Vector3 newPos = new Vector3(reader.ReadSingle(), reader.ReadSingle());
            PlayerControl movePlayer = Main.Instance.ModdedPlayerById(moveId)._Object;
            if (movePlayer.AmOwner)
            {
                movePlayer.transform.position = newPos;
                movePlayer.MyPhysics.body.position = newPos;
                movePlayer.MyPhysics.body.velocity = newVel;
            }
        }

        private void HandleInvisPlayer(MessageReader reader)
        {
            byte invisId = reader.ReadByte();
            PlayerControl invisPlayer = Main.Instance.ModdedPlayerById(invisId)._Object;
            Main.Instance.InvisPlayer(invisPlayer);
        }

        private void HandleDefensiveDuelist(MessageReader reader)
        {
            byte ddId = reader.ReadByte();
            PlayerControl ddPlayer = Main.Instance.ModdedPlayerById(ddId)._Object;
            Main.Instance.DefensiveDuelist(ddPlayer);
        }

        private void HandleRevivePlayer(MessageReader reader)
        {
            byte reviveId = reader.ReadByte();
            foreach (PlayerControl player in PlayerControl.AllPlayerControls)
            {
                if (player.PlayerId != reviveId)
                    continue;
                if (!player.Data.IsDead)
                    continue;
                
                player.Revive();
                foreach (DeadBody body in UnityEngine.Object.FindObjectsOfType<DeadBody>())
                    if (body.ParentId == reviveId)
                        UnityEngine.Object.Destroy(body.gameObject);
            }
        }

        private void HandleTeleportPlayer(MessageReader reader)
        {
            byte teleportId = reader.ReadByte();
            Vector2 teleportPos = new Vector2(reader.ReadSingle(), reader.ReadSingle());
            foreach (PlayerControl player in PlayerControl.AllPlayerControls)
            {
                if (teleportId == player.PlayerId)
                    player.NetTransform.SnapTo(teleportPos);
            }
        }

        private void HandleSpawnItem(MessageReader reader)
        {
            int itemId = reader.ReadInt32();
            Vector2 itemPosition = new Vector2(reader.ReadSingle(), reader.ReadSingle());
            Vector2 velocity = new Vector2(reader.ReadSingle(), reader.ReadSingle());
            Main.Instance.SpawnItem(itemId, itemPosition, velocity);
        }

        private void HandleTryPickupItem(MessageReader reader)
        {
            if (!AmongUsClient.Instance.AmHost)
                return;
            
            byte targetPlayer = reader.ReadByte();
            int pickupId = reader.ReadInt32();
            if (Main.Instance.AllItems.Any(x => x.Id == pickupId))
            {
                List<WorldItem> allMatches = Main.Instance.AllItems.FindAll(x => x.Id == pickupId);
                foreach (WorldItem item in allMatches) item.Delete();
                Main.Instance.AllItems.RemoveAll(x => x.IsPickedUp);
                
                MessageWriter writer = AmongUsClient.Instance.StartRpc(PlayerControl.LocalPlayer.NetId, (byte)Packets.GiveItem, SendOption.Reliable);
                writer.Write(targetPlayer);
                writer.Write(pickupId);
                writer.EndMessage();
            }
        }

        private void HandleGiveItem(MessageReader reader)
        {
            byte targetPlayer2 = reader.ReadByte();
            int pickupId2 = reader.ReadInt32();
            
            if (targetPlayer2 != PlayerControl.LocalPlayer.PlayerId)
                return;

            if (Main.Instance.GetLocalModdedPlayer().HasItem(pickupId2))
                return;

            Main.Instance.GiveGrabbedItem(pickupId2);
            Main.Instance.AllItems.RemoveAll(x => x.IsPickedUp);
        }

        private void HandleDestroyItem(MessageReader reader)
        {
            if (!AmongUsClient.Instance.AmHost)
            {
                int targetItemId = reader.ReadInt32();
                List<WorldItem> allMatches = Main.Instance.AllItems.FindAll(x => x.Id == targetItemId);
                foreach (WorldItem item in allMatches)
                    item.Delete();
                Main.Instance.AllItems.RemoveAll(x => x.IsPickedUp);
            }
        }

        private void HandleUseItem(MessageReader reader)
        {
            if (!AmongUsClient.Instance.AmHost) return;
            
            int usedItemId = reader.ReadInt32();
            switch (usedItemId)
            {
                case 0: DeluminatorWorld.HasSpawned = false; break;
                case 1: MaraudersMapWorld.HasSpawned = false; break;
                case 2: PortKeyWorld.HasSpawned = false; break;
                case 5: ButterBeerWorld.HasSpawned = false; break;
            }
        }

        private void HandleUpdateSpeedMultiplier(MessageReader reader)
        {
            byte readerId = reader.ReadByte();
            float newSpeed = reader.ReadSingle();
            Main.Instance.ModdedPlayerById(readerId).SpeedMultiplier = newSpeed;
        }

        private void HandleRevealRole(MessageReader reader)
        {
            byte revealId = reader.ReadByte();
            Main.Instance.RevealRole(revealId);
        }
    }
}
