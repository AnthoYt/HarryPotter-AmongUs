using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using HarryPotter.Classes;
using HarryPotter.Classes.Items;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace HarryPotter.Patches
{
    [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Start))]  
    public class MeetingHud_Start
    {
        static void Prefix(MeetingHud __instance)
        {
            // Fonction pour créer un bouton spécifique
            void CreateButton(PlayerVoteArea voteArea, GameObject originalButton, string buttonName, Sprite buttonSprite, Color buttonColor, float xOffset)
            {
                GameObject newButton = Object.Instantiate(originalButton);
                newButton.name = buttonName;
                newButton.transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>().color = buttonColor;
                newButton.GetComponent<SpriteRenderer>().sprite = buttonSprite;
                newButton.transform.SetParent(voteArea.Buttons.transform);
                newButton.transform.localPosition = new Vector3(
                    originalButton.transform.localPosition.x - (originalButton.transform.localPosition.x - voteArea.Buttons.transform.GetChild(1).transform.localPosition.x) * xOffset,
                    originalButton.transform.localPosition.y, originalButton.transform.localPosition.z);
                newButton.transform.localScale = originalButton.transform.localScale;
                newButton.SetActive(true);
            }

            // Ajout du bouton "Snitch" si le joueur possède l'objet 3
            if (Main.Instance.GetLocalModdedPlayer().HasItem(3))
            {
                foreach (PlayerVoteArea voteArea in __instance.playerStates)
                {
                    GameObject confirmButton = voteArea.Buttons.transform.GetChild(0).gameObject;
                    GameObject cancelButton = voteArea.Buttons.transform.GetChild(1).gameObject;
                    CreateButton(voteArea, confirmButton, "SnitchButton", Main.Instance.Assets.SmallSnitchSprite, Color.yellow, 1f);
                }
            }

            // Ajout du bouton "Sort" si le joueur possède l'objet 8
            if (Main.Instance.GetLocalModdedPlayer().HasItem(8))
            {
                foreach (PlayerVoteArea voteArea in __instance.playerStates)
                {
                    GameObject confirmButton = voteArea.Buttons.transform.GetChild(0).gameObject;
                    GameObject cancelButton = voteArea.Buttons.transform.GetChild(1).gameObject;
                    CreateButton(voteArea, cancelButton, "SortButton", Main.Instance.Assets.SmallSortSprite, new Color(195f / 255f, 0f, 1f), 0.12f);
                }
            }
        }
    }
}
