using Hazel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using HarryPotter.Classes.Roles;
using HarryPotter.Classes.UI;
using HarryPotter.Classes.WorldItems;
using hunterlib.Classes;
using InnerNet;
using TMPro;
using UnityEngine;

namespace HarryPotter.Classes
{
    public class Pair<T1, T2>
    {
        public Pair(T1 first, T2 second)
        {
            Item1 = first;
            Item2 = second;
        }

        public T1 Item1 { get; set; }
        public T2 Item2 { get; set; }
    }
    
    class Main
    {
        public static Main Instance { get; set; }
        public List<ModdedPlayerClass> AllPlayers { get; set; }
        public List<WorldItem> AllItems { get; set; }
        public Config Config { get; set; }
        public CustomRpc Rpc { get; set; }
        public Asset Assets { get; set; }
        public int CurrentStage { get; set; }
        public List<Pair<PlayerControl, string>> PlayersWithRequestedRoles { get; set; }
        public List<Tuple<byte, Vector2>> PossibleItemPositions { get; set; }
        public List<Tuple<byte, Vector2>> DefaultItemPositions { get; } = new List<Tuple<byte, Vector2>>
        {
            new Tuple<byte, Vector2>(2, new Vector2(18.58625f, -21.96028f)),
            // Ajout d'autres positions par défaut ici ...
        };

        public Main()
        {
            Config = new Config();
            Rpc = new CustomRpc();
            Assets = new Asset();
            AllPlayers = new List<ModdedPlayerClass>();
            AllItems = new List<WorldItem>();
            PlayersWithRequestedRoles = new List<Pair<PlayerControl, string>>();
        }

        public void RpcRequestRole(string roleName)
        {
            if (AmongUsClient.Instance.AmHost)
            {
                if (PlayersWithRequestedRoles.All(x => x.Item1 != PlayerControl.LocalPlayer))
                    PlayersWithRequestedRoles.Add(new Pair<PlayerControl, string>(PlayerControl.LocalPlayer, roleName));
            }
            else
            {
                MessageWriter writer = AmongUsClient.Instance.StartRpc(PlayerControl.LocalPlayer.NetId, (byte)Packets.RequestRole, SendOption.Reliable);
                writer.Write(PlayerControl.LocalPlayer.PlayerId);
                writer.Write(roleName);
                writer.EndMessage();
            }
        }

        public List<Vector2> GetAllApplicableItemPositions()
        {
            return PossibleItemPositions
                .Where(position => ShipStatus.Instance != null && position.Item1 == PlayerControl.GameOptions.MapId)
                .Select(position => position.Item2)
                .ToList();
        }

        public ModdedPlayerClass ModdedPlayerById(byte id)
        {
            return AllPlayers.FirstOrDefault(player => player._Object.PlayerId == id);
        }

        public IEnumerator CoStunPlayer(PlayerControl player)
        {
            ImportantTextTask durationText = TaskInfoHandler.Instance.AddNewItem(1, "");

            while (true)
            {
                bool isMeeting = MeetingHud.Instance;
                bool isGameEnded = !AmongUsClient.Instance.IsGameStarted;

                if (player.AmOwner)
                {
                    string roleColorHex = TaskInfoHandler.Instance.GetRoleHexColor(player);
                    durationText.Text = $"{roleColorHex}You are stunned until the next meeting.</color></color>";
                    player.myLight.LightRadius = Mathf.Lerp(ShipStatus.Instance.MinLightRadius, ShipStatus.Instance.MaxLightRadius, 0) * PlayerControl.GameOptions.CrewLightMod;
                    player.moveable = false;
                    player.MyPhysics.body.velocity = Vector2.zero;
                }

                if (isMeeting || isGameEnded)
                {
                    TaskInfoHandler.Instance.RemoveItem(durationText);
                    player.moveable = true;
                    yield break;
                }

                yield return null;
            }
        }

        public IEnumerator CoActivateButterBeer(PlayerControl player)
        {
            DateTime now = DateTime.UtcNow;
            ImportantTextTask durationText = TaskInfoHandler.Instance.AddNewItem(1, "");
            SetSpeedMultiplier(player.PlayerId, 2f);
            ModdedPlayerById(player.PlayerId).ReverseDirectionalControls = true;

            while (true)
            {
                bool isMeeting = MeetingHud.Instance;
                bool isGameEnded = !AmongUsClient.Instance.IsGameStarted;
                bool hasTimeExpired = now.AddSeconds(Config.BeerDuration) < DateTime.UtcNow;

                if (player.AmOwner)
                {
                    double remainingTime = Math.Ceiling(Config.HourglassTimer - (DateTime.UtcNow - now).TotalSeconds);
                    string roleColorHex = TaskInfoHandler.Instance.GetRoleHexColor(player);
                    durationText.Text = $"{roleColorHex}You are drunk on Butter Beer! {remainingTime}s remaining</color></color>";
                }

                if (isMeeting || isGameEnded || hasTimeExpired)
                {
                    TaskInfoHandler.Instance.RemoveItem(durationText);
                    SetSpeedMultiplier(player.PlayerId, 1f);
                    ModdedPlayerById(player.PlayerId).ReverseDirectionalControls = false;
                    yield break;
                }

                yield return null;
            }
        }

        // Ajout d'autres méthodes mises à jour ici ...

        public void SetSpeedMultiplier(byte playerId, float newSpeed)
        {
            ModdedPlayerById(playerId).SpeedMultiplier = newSpeed;

            MessageWriter writer = AmongUsClient.Instance.StartRpc(PlayerControl.LocalPlayer.NetId, (byte)Packets.UpdateSpeedMultiplier, SendOption.Reliable);
            writer.Write(playerId);
            writer.Write(newSpeed);
            writer.EndMessage();
        }

        public List<TextMeshPro> CustomOptions { get; set; }

        public string GetOptionTextByName(string name)
        {
            foreach (CustomNumberOption numberOption in CustomNumberOption.AllNumberOptions)
                if (numberOption.Name == name) return $"{numberOption.Name}: {numberOption.Value}";

            foreach (CustomToggleOption toggleOption in CustomToggleOption.AllToggleOptions)
                if (toggleOption.Name == name) return $"{toggleOption.Name}: {(toggleOption.Value ? "On" : "Off")}";

            return "no option text found (ERR)";
        }

        // Autres méthodes...
    }
}
