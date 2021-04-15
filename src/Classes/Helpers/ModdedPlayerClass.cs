﻿using System.Collections.Generic;
using System.Linq;
using HarryPotter.Classes.Items;
using HarryPotter.Classes.WorldItems;
using Hazel;
using Reactor.Extensions;
using Rewired;
using Rewired.ComponentControls;
using UnityEngine;
using UnityEngine.Purchasing.Security;

namespace HarryPotter.Classes
{
    public class ModdedPlayerClass
    {
        public ModdedPlayerClass(PlayerControl orgPlayer, Role role, List<Item> inventory)
        {
            _Object = orgPlayer;
            Role = role;
            Inventory = inventory;
        }

        public void Update()
        {
            if (ExileController.Instance != null)
                Role.ResetCooldowns();
            
            if (_Object.Data.IsDead)
                ClearItems();

            TaskInfoHandler.Instance.Update();
            HandleNameColors();
            PopulateButtons();
            Role?.Update();

            if (AmongUsClient.Instance.AmHost)
            {
                DeluminatorWorld.WorldSpawn();
                MaraudersMapWorld.WorldSpawn();
                PortKeyWorld.WorldSpawn();
                TheGoldenSnitchWorld.WorldSpawn();

                if (Main.Instance.Config.OrderOfTheImp)
                {
                    if (Main.Instance.AllPlayers.Any(x => Main.Instance.IsPlayerRole(x, "Harry") && (x._Object.Data.IsDead || x._Object.Data.Disconnected)) &&
                        Main.Instance.AllPlayers.Any(x => Main.Instance.IsPlayerRole(x, "Hermione") && (x._Object.Data.IsDead || x._Object.Data.Disconnected)) &&
                        Main.Instance.AllPlayers.Any(x => Main.Instance.IsPlayerRole(x, "Ron") && (x._Object.Data.IsDead || x._Object.Data.Disconnected)))
                    {
                        ShipStatus.RpcEndGame(GameOverReason.ImpostorByKill, false);
                    }
                }
            }
        }

        public void HandleNameColors()
        {
            if (_Object.Data.IsDead && !CanSeeAllRolesOveridden)
            {
                foreach (ModdedPlayerClass moddedPlayer in Main.Instance.AllPlayers)
                {
                    if (moddedPlayer.Role == null) continue;
                    
                    Main.Instance.SetNameColor(moddedPlayer._Object, moddedPlayer.Role.RoleColor);
                    moddedPlayer._Object.nameText.transform.position = new Vector3(
                        moddedPlayer._Object.nameText.transform.position.x,
                        moddedPlayer._Object.transform.position.y + 0.8f,
                        moddedPlayer._Object.nameText.transform.position.z);
                    moddedPlayer._Object.nameText.text =
                        moddedPlayer._Object.Data.PlayerName + "\n" + moddedPlayer.Role.RoleName;
                }
            }

            if (Role == null)
            {
                _Object.nameText.text = _Object.Data.PlayerName + "\n" + (_Object.Data.IsImpostor ? "Impostor" : "Crewmate");
                _Object.nameText.transform.position = new Vector3(
                    _Object.nameText.transform.position.x, 
                    _Object.transform.position.y + 0.8f, 
                    _Object.nameText.transform.position.z);
                return;
            }

            Main.Instance.SetNameColor(_Object, Role.RoleColor);
            _Object.nameText.text = _Object.Data.PlayerName + "\n" + Role.RoleName;
            _Object.nameText.transform.position = new Vector3(
                _Object.nameText.transform.position.x, 
                _Object.transform.position.y + 0.8f, 
                _Object.nameText.transform.position.z);

            if (_Object.Data.IsImpostor)
            {
                foreach (ModdedPlayerClass moddedPlayer in Main.Instance.AllPlayers)
                {
                    if (moddedPlayer._Object.AmOwner)
                        continue;

                    if (!moddedPlayer._Object.Data.IsImpostor)
                        continue;
                    
                    if (moddedPlayer.Role == null)
                        continue;
                    
                    moddedPlayer._Object.nameText.transform.position = new Vector3(
                        moddedPlayer._Object.nameText.transform.position.x,
                        moddedPlayer._Object.transform.position.y + 0.8f,
                        moddedPlayer._Object.nameText.transform.position.z);
                    moddedPlayer._Object.nameText.text =
                        moddedPlayer._Object.Data.PlayerName + "\n" + moddedPlayer.Role.RoleName;
                }
            }
        }

        public void PopulateButtons()
        {
            float itemCount = 0;
            foreach (var item in Inventory)
            {
                if (item.IsSpecial) continue;
                item.DrawIcon(HudManager.Instance.ReportButton.renderer.bounds.max.x - 0.375f - (itemCount * 0.6f), HudManager.Instance.ReportButton.renderer.bounds.max.y + 0.375f, HudManager.Instance.KillButton.transform.position.z);
                itemCount++;
            }
        }

        public bool HasItem(int id)
        {
            return Inventory.FindAll(x => x.Id == id).Count > 0;
        }

        public void GiveItem(int id)
        {
            switch (id)
            {
                case 0:
                    Inventory.Add(new Deluminator(this));
                    break;
                case 1: 
                    Inventory.Add(new MaraudersMap(this));
                    break;
                case 2: 
                    Inventory.Add(new PortKey(this));
                    break;
                case 3: 
                    Inventory.Add(new TheGoldenSnitch(this));
                    break;
            }
        }

        public void ClearItems()
        {
            while (Inventory.Count > 0)
                Inventory[0].Delete();
        }
        
        public PlayerControl _Object { get; set; }
        public Role Role { get; set; }
        public ModdedPlayerClass ControllerOverride { get; set; }
        public List<Item> Inventory { get; set; }
        public bool Immortal { get; set; }
        public bool KilledByCurse { get; set; }
        public bool CanUseConsoles { get; set; } = true;
        public bool CanSeeAllRolesOveridden { get; set; }
    }
}