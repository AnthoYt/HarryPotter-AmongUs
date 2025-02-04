using HarmonyLib;
using HarryPotter.Classes;
using InnerNet;
using System.Linq;
using HarryPotter.Classes.Helpers;
using HarryPotter.Classes.Helpers.UI;
using UnityEngine;

namespace HarryPotter.Patches
{
    [HarmonyPatch(typeof(InnerNetClient), nameof(InnerNetClient.Start))]
    public static class InnerNetClient_Start
    { 
        static void Postfix()
        {
            // Vérification que Main.Instance existe avant de l'utiliser
            if (Main.Instance == null) return;

            // Décommenter la ligne si tu souhaites jouer un son de thème au démarrage
            // SoundManager.Instance.PlaySound(Main.Instance.Assets.HPTheme, false, 1f);

            // Vérification de HatManager.Instance pour éviter les erreurs
            if (HatManager.Instance != null)
            {
                // Ajout des chapeaux personnalisés
                foreach (Hat customHat in Hat.AllHats)
                {
                    // Instancier un chapeau de base
                    HatBehaviour newHat = Object.Instantiate(HatManager.Instance.AllHats.FirstOrDefault());
                    if (newHat != null) // Vérification pour éviter des objets nuls
                    {
                        newHat.MainImage = customHat.MainSprite;
                        newHat.NoBounce = !customHat.Bounce;
                        newHat.ChipOffset = customHat.ChipOffset;
                        HatManager.Instance.AllHats.Insert(1, newHat);
                    }
                }
            }

            // Ajout des UI components si nécessaire
            if (GameObject.FindObjectOfType<InventoryUI>() == null)
                new GameObject().AddComponent<InventoryUI>();

            if (GameObject.FindObjectOfType<MindControlMenu>() == null)
                new GameObject().AddComponent<MindControlMenu>();

            if (GameObject.FindObjectOfType<HotbarUI>() == null)
                new GameObject().AddComponent<HotbarUI>();

            // Réinitialisation des options personnalisées
            Main.Instance.ResetCustomOptions();
        }
    }
}
