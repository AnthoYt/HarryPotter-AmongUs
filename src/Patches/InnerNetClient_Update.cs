using System.Collections;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using HarryPotter.Classes;
using HarryPotter.Classes.WorldItems;
using InnerNet;
using TMPro;
using UnhollowerBaseLib;
using UnityEngine;

namespace HarryPotter.Patches
{
    [HarmonyPatch(typeof(InnerNetClient), nameof(InnerNetClient.Update))]
    class InnerNetClient_Update
    {
        static void Postfix(InnerNetClient __instance)
        {
            hunterlib.Classes.Coroutines.Start(LateUpdate());
        }

        static IEnumerator LateUpdate()
        {
            yield return new WaitForEndOfFrame();
            RunUpdate();
        }

        static void RunUpdate()
        {
            if (AmongUsClient.Instance.GameState == InnerNetClient.GameStates.Joined)
            {
                if (Main.Instance?.Config?.SelectRoles == true) // Ajout de la vérification explicite
                {
                    if (Input.GetKeyDown(KeyCode.Alpha1))
                        Main.Instance.RpcRequestRole("Harry");
                    if (Input.GetKeyDown(KeyCode.Alpha2))
                        Main.Instance.RpcRequestRole("Hermione");
                    if (Input.GetKeyDown(KeyCode.Alpha3))
                        Main.Instance.RpcRequestRole("Ron");
                    if (Input.GetKeyDown(KeyCode.Alpha4))
                        Main.Instance.RpcRequestRole("Voldemort");
                    if (Input.GetKeyDown(KeyCode.Alpha5))
                        Main.Instance.RpcRequestRole("Bellatrix");
                }
            }

            // Vérification de Main.Instance avant d'appeler ReloadSettings
            Main.Instance?.Config?.ReloadSettings();

            // Vérification de HudManager.Instance avant de manipuler l'UI
            if (HudManager.InstanceExists && HudManager.Instance.GameSettings != null)
            {
                foreach (TextMeshPro lobbySettingTMP in Main.Instance.CustomOptions)
                {
                    Vector2 pos = HudManager.Instance.GameSettings.transform.position;

                    pos.x += lobbySettingTMP.renderedWidth / 2;
                    pos.y -= lobbySettingTMP.renderedHeight / 2;
                    pos.y -= HudManager.Instance.GameSettings.renderedHeight;
                    pos.y -= lobbySettingTMP.renderedHeight * Main.Instance.CustomOptions.IndexOf(lobbySettingTMP);

                    lobbySettingTMP.gameObject.transform.position = pos;
                    lobbySettingTMP.gameObject.SetActive(HudManager.Instance.GameSettings.isActiveAndEnabled);

                    string optionText = Main.Instance.GetOptionTextByName(lobbySettingTMP.gameObject.name);

                    RectTransform lobbyTextTrans = lobbySettingTMP.gameObject.GetComponent<RectTransform>();
                    lobbyTextTrans.sizeDelta = lobbySettingTMP.GetPreferredValues(optionText);

                    lobbySettingTMP.text = optionText;
                }
            }

            // Réinitialisation des WorldItems
            if (!AmongUsClient.Instance.IsGameStarted && Main.Instance != null)
            {
                ResetWorldItems(); // Extraction de la logique dans une méthode séparée
                return;
            }

            // Mise à jour de la liste des joueurs
            foreach (PlayerControl player in PlayerControl.AllPlayerControls)
            {
                if (Main.Instance?.AllPlayers?.Any(x => x?._Object == player) != true)
                {
                    Main.Instance?.AllPlayers.Add(new ModdedPlayerClass(player, null, new List<Item>()));
                }
            }

            // Nettoyage des joueurs déconnectés
            Main.Instance?.AllPlayers.RemoveAll(player => player == null || player._Object == null || player._Object.Data.Disconnected);

            // Mise à jour du joueur local modifié
            Main.Instance?.GetLocalModdedPlayer()?.Update();
        }

        // Méthode pour réinitialiser les objets WorldItem
        static void ResetWorldItems()
        {
            foreach (WorldItem wItem in Main.Instance.AllItems)
                wItem.Delete();

            DeluminatorWorld.HasSpawned = false;
            MaraudersMapWorld.HasSpawned = false;
            PortKeyWorld.HasSpawned = false;
            TheGoldenSnitchWorld.HasSpawned = false;
            GhostStoneWorld.HasSpawned = false;
            ButterBeerWorld.HasSpawned = false;
            ElderWandWorld.HasSpawned = false;
            BasWorldItem.HasSpawned = false;
            SortingHatWorld.HasSpawned = false;
            PhiloStoneWorld.HasSpawned = false;

            Main.Instance.CurrentStage = 0;
            Main.Instance.AllItems.Clear();
            Main.Instance.AllPlayers.Clear();
            Main.Instance.PossibleItemPositions = Main.Instance.DefaultItemPositons;
            TaskInfoHandler.Instance.AllInfo.Clear();
        }
    }
}
