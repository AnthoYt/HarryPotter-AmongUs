﻿using System;
using System.Collections.Generic;
using System.Linq;
using HarryPotter.Classes.WorldItems;
using Reactor;
using UnityEngine;

namespace HarryPotter.Classes.Helpers
{
    [RegisterInIl2Cpp]
    class UpdateHandler : MonoBehaviour
    {
        public UpdateHandler(IntPtr ptr) : base(ptr)
        {
        }
        
        private void LateUpdate()
        {
            Main.Instance?.Config?.ReloadSettings();
            
            if ((!AmongUsClient.Instance.IsGameStarted || PlayerControl.LocalPlayer == null) && Main.Instance != null)
            {
                foreach (WorldItem wItem in Main.Instance.AllItems) wItem.Delete();
                DeluminatorWorld.HasSpawned = false;
                MaraudersMapWorld.HasSpawned = false;
                PortKeyWorld.HasSpawned = false;
                TheGoldenSnitchWorld.HasSpawned = false;
                GhostStoneWorld.HasSpawned = false;
                ButterBeerWorld.HasSpawned = false;
                Main.Instance.CurrentStage = 0;
                Main.Instance.AllItems.Clear();
                Main.Instance.AllPlayers.Clear();
                Main.Instance.PossibleItemPositions = Main.Instance.DefaultItemPositons;
                TaskInfoHandler.Instance.AllInfo.Clear();
                return;
            }

            foreach (PlayerControl player in PlayerControl.AllPlayerControls)
                if (Main.Instance?.AllPlayers.Where(x => x?._Object == player).ToList().Count == 0)
                    Main.Instance?.AllPlayers.Add(new ModdedPlayerClass(player, null, new List<Item>()));

            foreach (ModdedPlayerClass player in Main.Instance?.AllPlayers.ToList())
                if (player == null || player._Object == null || player._Object.Data.Disconnected) Main.Instance?.AllPlayers.Remove(player);

            Main.Instance?.GetLocalModdedPlayer().Update();
        }
    }
}
