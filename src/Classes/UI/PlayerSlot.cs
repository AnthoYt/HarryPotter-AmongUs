using System;
using System.Linq;
using HarryPotter.Classes.Roles;
using HarryPotter.Classes.UI;
using UnityEngine;
using hunterlib.Classes;

namespace HarryPotter.Classes.Helpers.UI
{
    [RegisterInIl2Cpp]
    class PlayerSlot : MonoBehaviour
    {
        public PlayerSlot(IntPtr ptr) : base(ptr) { }

        private void Awake()
        {
            // Récupère le bouton et ajoute des composants nécessaires
            GameObject itemButtonObj = gameObject.transform.GetChild(0).gameObject;
            PlayerButton = itemButtonObj.gameObject.AddComponent<CustomButton>();
            PlayerButton.OnClick += TryControlTargetedPlayer;
            PlayerTooltip = itemButtonObj.gameObject.AddComponent<Tooltip>();
        }

        public void TryControlTargetedPlayer()
        {
            ModdedPlayerClass localModdedPlayer = Main.Instance?.GetLocalModdedPlayer();
            
            // Vérifications des conditions nécessaires avant d'essayer de contrôler un joueur
            if (localModdedPlayer == null || ((Bellatrix)localModdedPlayer.Role).MindControlledPlayer != null) return;
            if (((Bellatrix)localModdedPlayer.Role).MindControlButton.isCoolingDown) return;
            if (Main.Instance.ModdedPlayerById(TargetedPlayer.PlayerId).Immortal) return;
            if (PlayerControl.LocalPlayer.Data.IsDead || TargetedPlayer.Data.IsDead) return;
            if (TargetedPlayer.Data.Disconnected) return;
            
            // Vérifie si le joueur cible est marqué par Bellatrix
            if (Main.Instance.GetPlayerRoleName(localModdedPlayer) == "Bellatrix" && 
                !((Bellatrix)localModdedPlayer.Role).MarkedPlayers.Any(x => x.PlayerId == TargetedPlayer.PlayerId)) return;
            
            // Autres vérifications d'état
            if (PlayerControl.LocalPlayer.inVent || MeetingHud.Instance || ExileController.Instance) return;
            if (!PlayerButton.Enabled || PlayerButton.HoverColor != Color.yellow) return;

            // Fermeture du menu de contrôle et demande de contrôle sur le joueur ciblé
            MindControlMenu.Instance.CloseMenu();
            Main.Instance.RpcControlPlayer(PlayerControl.LocalPlayer, TargetedPlayer);
        }

        public void ResetIcon()
        {
            PlayerButton.Enabled = false;
            PlayerTooltip.Enabled = false;
            
            ModdedPlayerClass localModdedPlayer = Main.Instance?.GetLocalModdedPlayer();
            if (localModdedPlayer == null || Main.Instance.GetPlayerRoleName(localModdedPlayer) != "Bellatrix")
            {
                // Si ce n'est pas Bellatrix ou qu'il n'y a pas de joueur local, on réinitialise l'icône
                DestroyIcon();
                return;
            }
            
            if (((Bellatrix)localModdedPlayer.Role).MarkedPlayers.Count < PlayerIndex + 1)
            {
                // Si le joueur local n'a pas assez de joueurs marqués, on réinitialise l'icône
                DestroyIcon();
                return;
            }

            // Cibler le joueur marqué par Bellatrix
            TargetedPlayer = ((Bellatrix)localModdedPlayer.Role).MarkedPlayers[PlayerIndex];
            GameData.PlayerInfo data = TargetedPlayer.Data;

            // Activer et configurer le bouton et l'icône
            PlayerButton.Enabled = true;
            PlayerButton.SetColor(Color.yellow);
            PlayerTooltip.Enabled = true;
            PlayerTooltip.TooltipText = data.PlayerName;

            // Créer l'icône si nécessaire
            if (Icon == null)
            {
                Icon = Instantiate(HudManager.Instance.IntroPrefab.PlayerPrefab, gameObject.transform).DontDestroy();
                Icon.gameObject.layer = 5;
                Icon.Body.sortingOrder = 5;
                Icon.SkinSlot.sortingOrder = 6;
                Icon.HatSlot.BackLayer.sortingOrder = 4;
                Icon.HatSlot.FrontLayer.sortingOrder = 6;
                Icon.name = data.PlayerName;
                Icon.SetFlipX(true);
                Icon.transform.localScale = Vector3.one * 2f;
            }

            // Configurer les matériaux et autres attributs visuels de l'icône
            PlayerControl.SetPlayerMaterialColors(data.ColorId, Icon.Body);
            DestroyableSingleton<HatManager>.Instance.SetSkin(Icon.SkinSlot, data.SkinId);
            Icon.HatSlot.SetHat(data.HatId, data.ColorId);
            PlayerControl.SetPetImage(data.PetId, data.ColorId, Icon.PetSlot);
            Icon.NameText.gameObject.SetActive(false);
        }

        private void DestroyIcon()
        {
            // Méthode pour détruire proprement l'icône
            if (Icon != null)
            {
                Icon.HatSlot.Destroy();
                Icon.SkinSlot.Destroy();
                Icon.Body.Destroy();
                Icon.gameObject.Destroy();
                Icon.Destroy();
                Icon = null;
            }
        }

        public void LateUpdate()
        {
            if (MindControlMenu.Instance.IsOpen) ResetIcon();
        }

        // Déclaration des propriétés
        public Tooltip PlayerTooltip { get; set; }
        public CustomButton PlayerButton { get; set; }
        public int PlayerIndex { get; set; }
        public PlayerControl TargetedPlayer { get; set; }
        public PoolablePlayer Icon { get; set; }
    }
}
