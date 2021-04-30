﻿using HarmonyLib;
using HarryPotter.Classes;
using HarryPotter.Classes.Roles;
using HarryPotter.Classes.UI;
using UnityEngine;

namespace HarryPotter.Patches
{
    [HarmonyPatch(typeof(KillButtonManager), nameof(KillButtonManager.PerformKill))]
    class KillButtonManager_PerformKill
    {
        static bool Prefix(KillButtonManager __instance)
        {
            if (!PlayerControl.LocalPlayer.CanMove) return true;
            
            if (PlayerControl.LocalPlayer.FindClosestTarget() != null)
            {
                if (Main.Instance.ModdedPlayerById(PlayerControl.LocalPlayer.FindClosestTarget().PlayerId).Immortal)
                {
                    if (HudManager.Instance.KillButton == __instance)
                    {
                        PopupTMPHandler.Instance.CreatePopup("When using his ability, Ron cannot be killed.\nYour cooldown was reset.", Color.white, Color.black);
                        PlayerControl.LocalPlayer.SetKillTimer(PlayerControl.GameOptions.KillCooldown);
                        return false;
                    }
                }
            }
            
            if (__instance == HudManager.Instance.KillButton && Main.Instance.GetLocalModdedPlayer()?.Role?.RoleName == "Bellatrix" && ((Bellatrix)Main.Instance.GetLocalModdedPlayer().Role).MindControlledPlayer != null)
            {
                PlayerControl killer = ((Bellatrix)Main.Instance.GetLocalModdedPlayer().Role).MindControlledPlayer._Object;
                if (killer.FindClosestTarget() != null && !Main.Instance.ControlKillUsed)
                {
                    Main.Instance.ControlKillUsed = true;
                    Main.Instance.RpcKillPlayer(killer, killer.FindClosestTarget(), false);
                }
                return false;
            }

            if (Main.Instance.GetLocalModdedPlayer()?.Role != null)
                return Main.Instance.GetLocalModdedPlayer().Role.PerformKill(__instance);
            return true;
        }
    }
}