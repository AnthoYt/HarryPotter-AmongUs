using System;
using System.Collections.Generic;
using HarmonyLib;
using HarryPotter.Classes;
using HarryPotter.Classes.Roles;
using Hazel;
using UnityEngine;

namespace HarryPotter.Patches
{
    [HarmonyPatch(typeof(PlayerPhysics), nameof(PlayerPhysics.FixedUpdate))]
    class PlayerPhysics_FixedUpdate
    {
        static bool Prefix(PlayerPhysics __instance)
        {
            // Récupère le joueur moddé par son ID
            ModdedPlayerClass moddedController = Main.Instance.ModdedPlayerById(__instance.myPlayer.PlayerId);

            // Vérifie si ce n'est pas le joueur local ou si ce n'est pas Bellatrix avec un joueur contrôlé
            if (__instance.myPlayer != PlayerControl.LocalPlayer || moddedController?.Role?.RoleName != "Bellatrix" || ((Bellatrix)moddedController.Role).MindControlledPlayer == null)
                return true;

            // Récupère le PlayerPhysics du joueur contrôlé mentalement
            PlayerPhysics controlledPlayer = ((Bellatrix)moddedController.Role).MindControlledPlayer._Object.MyPhysics;
            
            // Définit la vitesse en fonction de l'entrée du joystick
            Vector2 vel = HudManager.Instance.joystick.Delta * __instance.TrueSpeed;
            
            // Applique la vitesse au joueur contrôlé
            controlledPlayer.body.velocity = vel;

            // Crée un message RPC pour synchroniser la position et la vitesse du joueur contrôlé
            MessageWriter writer = AmongUsClient.Instance.StartRpc(PlayerControl.LocalPlayer.NetId, (byte)Packets.MoveControlledPlayer, SendOption.Reliable);
            writer.Write(controlledPlayer.myPlayer.PlayerId);
            writer.Write(vel.x);
            writer.Write(vel.y);
            writer.Write(controlledPlayer.body.position.x);
            writer.Write(controlledPlayer.body.position.y);
            writer.EndMessage();

            // Empêche l'exécution de la méthode originale (FixedUpdate) dans ce cas
            return false;
        }

        static void Postfix(PlayerPhysics __instance)
        {
            // Si le joueur local est le propriétaire et que le joueur moddé existe
            if (__instance.AmOwner && Main.Instance.GetLocalModdedPlayer() != null)
            {
                // Applique un éventuel renversement des contrôles directionnels
                __instance.body.velocity *= Main.Instance.GetLocalModdedPlayer().ReverseDirectionalControls ? -1f : 1f;

                // Applique un multiplicateur de vitesse (par exemple, si le modded player a une vitesse boostée)
                __instance.body.velocity *= Main.Instance.GetLocalModdedPlayer().SpeedMultiplier;
            }
        }
    }
}
