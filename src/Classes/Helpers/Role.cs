using UnityEngine;

namespace HarryPotter.Classes
{
    public class Role
    {
        public string RoleName { get; set; } // Le nom du rôle
        public string IntroString { get; set; } // Chaîne de texte d'introduction liée au rôle
        public Color RoleColor { get; set; } // Couleur principale associée au rôle
        public Color RoleColor2 { get; set; } // Une couleur secondaire, peut-être pour un dégradé ou un effet visuel
        public ModdedPlayerClass Owner { get; set; } // Le joueur qui possède ce rôle

        // Méthode pour mettre à jour le rôle, par défaut vide à implémenter par héritage
        public virtual void Update() { }

        // Méthode qui permet au rôle d'effectuer un kill, à surcharger si nécessaire
        public virtual bool PerformKill(KillButtonManager __instance)
        {
            return false; // Par défaut, ce rôle ne peut pas tuer
        }

        // Méthode pour réinitialiser les cooldowns, peut être surchargée dans les sous-classes
        public virtual void ResetCooldowns() { }

        // Méthode pour supprimer les cooldowns, à surcharger dans les rôles spécifiques
        public virtual void RemoveCooldowns() { }

        // Méthode qui détermine si des boutons personnalisés doivent être dessinés à l'écran pour ce rôle
        public virtual bool ShouldDrawCustomButtons()
        {
            return false; // Par défaut, pas de boutons personnalisés pour ce rôle
        }
    }
}
