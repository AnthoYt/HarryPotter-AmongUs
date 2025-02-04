using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InnerNet;
using Hazel;
using HarryPotter.Classes.WorldItems;

namespace HarryPotter.Classes.Items
{
    public class MaraudersMap : Item
    {
        private float _zoomFactor = 4f; // Facteur de zoom pour l'effet de la carte

        public MaraudersMap(ModdedPlayerClass owner)
        {
            this.Owner = owner;
            this.ParentInventory = owner.Inventory;
            this.Id = 1;
            this.Icon = Main.Instance.Assets.ItemIcons[Id];
            this.Name = "Marauder's Map";
            this.Tooltip = $"Marauder's Map:\nTemporarily zooms out\nthe camera. {Main.Instance.Config.MapDuration}s duration.";
        }

        public override void Use()
        {
            this.Delete(); // Supprime l'objet une fois utilisé
            hunterlib.Classes.Coroutines.Start(ZoomOut());
        }

        public IEnumerator ZoomOut()
        {
            DateTime endTime = DateTime.UtcNow.AddSeconds(Main.Instance.Config.MapDuration); // Détermine la durée de l'effet

            Camera.main.orthographicSize *= _zoomFactor; // Zoom avant

            // Sauvegarde de l'état actuel de l'UI et de la possibilité d'utiliser les consoles
            bool oldActiveShadowQuad = HudManager.Instance.ShadowQuad.gameObject.activeSelf;
            bool oldActiveKillButton = HudManager.Instance.KillButton.gameObject.activeSelf;
            bool oldActiveUseButton = HudManager.Instance.UseButton.gameObject.activeSelf;
            bool oldActiveReportButton = HudManager.Instance.ReportButton.gameObject.activeSelf;
            bool oldCanUseConsoles = Owner.CanUseConsoles;

            // Désactivation de certains éléments de l'UI pendant l'effet
            HudManager.Instance.ShadowQuad.gameObject.SetActive(false);
            HudManager.Instance.KillButton.gameObject.SetActive(false);
            HudManager.Instance.UseButton.gameObject.SetActive(false);
            HudManager.Instance.ReportButton.gameObject.SetActive(false);
            Owner.CanUseConsoles = false;

            // Boucle jusqu'à la fin de la durée ou jusqu'à ce qu'une condition d'arrêt se produise
            while (DateTime.UtcNow < endTime)
            {
                if (Minigame.Instance)
                    Minigame.Instance.Close(); // Ferme le mini-jeu si ouvert

                if (MeetingHud.Instance) // Si une réunion est en cours, arrête l'effet
                {
                    break;
                }

                // Si le jeu n'a pas commencé, arrête l'effet
                if (AmongUsClient.Instance.GameState != InnerNetClient.GameStates.Started)
                    break;

                yield return null; // Attend la prochaine frame
            }

            // Restauration de la caméra et de l'UI après l'effet
            Camera.main.orthographicSize /= _zoomFactor;

            // Restauration de l'UI
            HudManager.Instance.ShadowQuad.gameObject.SetActive(oldActiveShadowQuad);
            HudManager.Instance.KillButton.gameObject.SetActive(oldActiveKillButton);
            HudManager.Instance.UseButton.gameObject.SetActive(oldActiveUseButton);
            HudManager.Instance.ReportButton.gameObject.SetActive(oldActiveReportButton);
            Owner.CanUseConsoles = oldCanUseConsoles;

            yield break; // Fin de la coroutine
        }
    }
}
