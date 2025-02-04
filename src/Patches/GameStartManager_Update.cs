using HarmonyLib;
using HarryPotter.Classes;
using InnerNet;
using System.Linq;
using HarryPotter.Classes.Helpers;
using HarryPotter.Classes.Helpers.UI;
using UnityEngine;

namespace HarryPotter.Patches
{
    [HarmonyPatch(typeof(GameStartManager), nameof(GameStartManager.Update))]
    public static class GameStartManager_Update
    {
        static void Postfix(GameStartManager __instance)
        {
            // Vérification de l'existence de MakePublicButton avant de modifier sa position
            if (__instance.MakePublicButton != null)
            {
                __instance.MakePublicButton.transform.position = new Vector3(
                    __instance.MakePublicButton.transform.position.x, 
                    __instance.MakePublicButton.transform.position.y, 
                    0);
            }

            // Si tu souhaites décommenter d'autres parties, tu peux le faire en vérifiant si les objets existent
            // par exemple :
            // if (__instance.GameRoomName != null)
            // {
            //     __instance.GameRoomName.transform.localPosition = new Vector3(0.75f, 4.15f);
            // }

            // if (__instance.PlayerCounter != null)
            // {
            //     __instance.PlayerCounter.transform.localPosition = new Vector3(0.3f, -1.1f);
            // }

            // if (__instance.StartButton != null)
            // {
            //     __instance.StartButton.transform.localPosition = new Vector3(0f, -0.4f);
            // }
        }
    }
}
