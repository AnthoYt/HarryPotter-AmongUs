using System.Collections.Generic;
using UnityEngine;

namespace HarryPotter.Classes
{
    public class Hat
    {
        // Liste statique pour gérer tous les chapeaux
        public static List<Hat> AllHats { get; set; } = new List<Hat>();

        // Indicateur si le chapeau doit rebondir
        public bool Bounce { get; set; }

        // Décalage du chip du chapeau (utilisé pour l'animation par exemple)
        public Vector2 ChipOffset { get; set; }

        // Sprite principal du chapeau
        public Sprite MainSprite { get; set; }

        // Constructeur pour initialiser un chapeau
        public Hat(Sprite mainSprite, Vector2 chipOffset, bool bounce)
        {
            MainSprite = mainSprite;
            ChipOffset = chipOffset;
            Bounce = bounce;

            // Ajouter le chapeau à la liste des chapeaux
            AllHats.Add(this);
        }

        // Méthode pour supprimer un chapeau de la liste
        public void RemoveHat()
        {
            if (AllHats.Contains(this))
                AllHats.Remove(this);
        }

        // Exemple d'une méthode pour animer le chapeau si 'Bounce' est vrai
        public void AnimateBounce()
        {
            if (Bounce)
            {
                // Implémenter la logique d'animation du rebond ici (par exemple, un mouvement vertical)
                // Exemple simple : déplacer le chapeau selon une animation de rebond
                // Cela dépend de votre logique de jeu, voici une base simple :
                Vector2 position = new Vector2(0, Mathf.Sin(Time.time) * 0.1f); // Oscille de haut en bas
                // Appliquer ce mouvement à la position du chapeau
                // Ceci peut être adapté selon le système de votre jeu
            }
        }
    }
}
