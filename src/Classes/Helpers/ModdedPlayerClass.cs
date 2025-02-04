using System.Collections.Generic;
using System.Linq;
using HarryPotter.Classes.Items;
using HarryPotter.Classes.UI;
using HarryPotter.Classes.WorldItems;
using UnityEngine;

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
            // Réinitialiser les cooldowns du rôle si nécessaire
            if (ExileController.Instance != null)
                Role.ResetCooldowns();
            
            // Si le joueur est mort, vider les objets
            if (_Object.Data.IsDead)
                ClearItems();

            // Montrer les joueurs morts si l'objet 4 (GhostStone) est dans l'inventaire
            if (HasItem(4))
            {
                foreach (PlayerControl player in PlayerControl.AllPlayerControls)
                    if (player.Data.IsDead)
                        player.Visible = true;
            }

            // Vérifier si le joueur doit être réanimé
            ShouldRevive = HasItem(9);

            // Mettre à jour les informations de tâche et gérer les couleurs des noms
            TaskInfoHandler.Instance.Update();
            HandleNameColors();
            Role?.Update();

            // Gestion du tir du Vigilante
            if (VigilanteShotEnabled)
            {
                HudManager.Instance.KillButton.gameObject.SetActive(HudManager.Instance.UseButton.isActiveAndEnabled);
                HudManager.Instance.KillButton.SetTarget(Main.Instance.GetClosestTarget(_Object, false));
                HudManager.Instance.KillButton.SetCoolDown(0f, 1f);
            }

            // Tirer si la touche Q est pressée
            if (Input.GetKeyDown(KeyCode.Q) && VigilanteShotEnabled) 
                HudManager.Instance.KillButton.PerformKill();

            // Si on est l'hôte, faire apparaître les objets du monde
            if (AmongUsClient.Instance.AmHost)
            {
                DeluminatorWorld.WorldSpawn();
                MaraudersMapWorld.WorldSpawn();
                PortKeyWorld.WorldSpawn();
                TheGoldenSnitchWorld.WorldSpawn();
                GhostStoneWorld.WorldSpawn();
                ButterBeerWorld.WorldSpawn();
                ElderWandWorld.WorldSpawn();
                BasWorldItem.WorldSpawn();
                SortingHatWorld.WorldSpawn();
                PhiloStoneWorld.WorldSpawn();

                // Vérification de l'Ordre des Imposteurs
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
            // Si le rôle est null, afficher le nom du joueur avec un rôle par défaut
            if (Role == null)
            {
                _Object.nameText.text = _Object.Data.PlayerName + "\n" + (_Object.Data.IsImpostor ? "Impostor" : "Muggle");
                _Object.nameText.transform.position = new Vector3(
                    _Object.nameText.transform.position.x, 
                    _Object.transform.position.y + 0.8f, 
                    _Object.nameText.transform.position.z);
                return;
            }

            // Afficher le nom du joueur et la couleur de son rôle
            Main.Instance.SetNameColor(_Object, Role.RoleColor);
            _Object.nameText.text = _Object.Data.PlayerName + "\n" + Role.RoleName;
            _Object.nameText.transform.position = new Vector3(
                _Object.nameText.transform.position.x, 
                _Object.transform.position.y + 0.8f, 
                _Object.nameText.transform.position.z);

            // Si le joueur est un imposteur, ajuster les noms des autres imposteurs
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
        
        // Vérifier si le joueur a un item avec l'ID spécifié
        public bool HasItem(int id)
        {
            return Inventory.Any(x => x.Id == id);
        }

        // Ajouter un item à l'inventaire du joueur
        public void GiveItem(int id)
        {
            Item item = id switch
            {
                0 => new Deluminator(this),
                1 => new MaraudersMap(this),
                2 => new PortKey(this),
                3 => new TheGoldenSnitch(this),
                4 => new GhostStone(this),
                5 => new ButterBeer(this),
                6 => new ElderWand(this),
                7 => new BasItem(this),
                8 => new SortingHat(this),
                9 => new PhiloStone(this),
                _ => null
            };

            if (item == null) return;
            Inventory.Add(item);

            // Si l'item est un piège, l'utiliser immédiatement
            if (item.IsTrap) item.Use();

            // Afficher un message à l'écran concernant l'item récupéré
            string trapText = "You picked up a trap item! Unpredictable effects have been activated!";
            string normalText = "You picked up an item! Press 'C' to open your Inventory.";
            
            PopupTMPHandler.Instance.CreatePopup(item.IsTrap ? trapText : normalText, Color.white, Color.black);
        }

        // Vider l'inventaire du joueur
        public void ClearItems()
        {
            foreach (var item in Inventory)
                item.Delete();
            Inventory.Clear();
        }

        public PlayerControl _Object { get; set; }
        public Role Role { get; set; }
        public ModdedPlayerClass ControllerOverride { get; set; }
        public List<Item> Inventory { get; set; }
        public bool Immortal { get; set; }
        public bool KilledByCurse { get; set; }
        public bool CanUseConsoles { get; set; } = true;
        public bool CanSeeAllRolesOveridden { get; set; }
        public bool ReverseDirectionalControls { get; set; }
        public float SpeedMultiplier { get; set; } = 1f;
        public bool VigilanteShotEnabled { get; set; }
        public bool ShouldRevive { get; set; }
    }
}
