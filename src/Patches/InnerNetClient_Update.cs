﻿﻿﻿using System;
  using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using HarryPotter.Classes;
  using HarryPotter.Classes.Items;
  using HarryPotter.Classes.WorldItems;
  using InnerNet;
  using UnityEngine;

  namespace HarryPotter.Patches
{
    [HarmonyPatch(typeof(InnerNetClient), nameof(InnerNetClient.Update))]
    class InnerNetClient_Update
    {
        static void Postfix(InnerNetClient __instance)
        {
            Main.Instance?.Config?.ReloadSettings();
            
            if ((__instance.GameState != InnerNetClient.GameStates.Started || PlayerControl.LocalPlayer == null) && Main.Instance != null)
            {
                foreach (WorldItem wItem in Main.Instance.AllItems)
                    wItem.Delete();
                DeluminatorWorld.HasSpawned = false;
                MaraudersMapWorld.HasSpawned = false;
                PortKeyWorld.HasSpawned = false;
                TheGoldenSnitchWorld.HasSpawned = false;
                Main.Instance.CurrentStage = 0;
                Main.Instance.AllItems.Clear();
                Main.Instance.AllPlayers.Clear();
                Main.Instance.PossibleItemPositions = Main.Instance.DefaultItemPositons;
                TaskInfoHandler.Instance.AllInfo.Clear();
                return;
            }

            foreach (PlayerControl player in PlayerControl.AllPlayerControls)
                if (Main.Instance?.AllPlayers.Where(x => x._Object == player).ToList().Count == 0)
                    Main.Instance.AllPlayers.Add(new ModdedPlayerClass(player, null, new List<Item>()));

            Main.Instance?.GetLocalModdedPlayer().Update();
        }
    }
}
