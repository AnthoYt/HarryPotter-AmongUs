using hunterlib.Classes;

namespace HarryPotter.Classes
{
    class Config
    {
        // Déclaration des options personnalisées pour la configuration du jeu
        private CustomToggleOption Option1 = CustomToggleOption.Create("Order of the Impostors", false);
        private CustomToggleOption Option3 = CustomToggleOption.Create("Can Spells be Used In Vents", false);
        private CustomToggleOption Option4 = CustomToggleOption.Create("Show Info Popups/Tooltips");
        private CustomToggleOption Option5 = CustomToggleOption.Create("Shared Voldemort Cooldowns");

        private CustomNumberOption Option9 = CustomNumberOption.Create("Defensive Duelist Cooldown", 20f, 40f, 10, 2.5f);
        private CustomNumberOption Option10 = CustomNumberOption.Create("Invisibility Cloak Cooldown", 20f, 40f, 10, 2.5f);
        private CustomNumberOption Option11 = CustomNumberOption.Create("Time Turner Cooldown", 20f, 40f, 10f, 2.5f);
        private CustomNumberOption Option12 = CustomNumberOption.Create("Crucio Cooldown", 20f, 40f, 10f, 2.5f);

        // Propriétés publiques en lecture seule qui accèdent aux options
        public bool OrderOfTheImp { get; private set; }
        public float MapDuration { get { return 10; } } // Durée fixe pour la carte
        public float DefensiveDuelistDuration { get { return 10; } } // Durée fixe pour le duelliste défensif
        public float InvisCloakDuration { get { return 10; } } // Durée fixe pour le manteau d'invisibilité
        public float HourglassTimer { get { return 10; } } // Durée fixe pour le sablier
        public float BeerDuration { get { return 10; } } // Durée fixe pour la bière
        public float CrucioDuration { get { return 10; } } // Durée fixe pour Crucio

        // Propriétés publiques avec les valeurs des cooldowns
        public float DefensiveDuelistCooldown { get; private set; }
        public float InvisCloakCooldown { get; private set; }
        public float HourglassCooldown { get; private set; }
        public float CrucioCooldown { get; private set; }

        // Propriétés pour les options de configuration
        public bool SpellsInVents { get; private set; }
        public float ImperioDuration { get { return 10; } } // Durée fixe pour Imperio
        public bool ShowPopups { get; private set; }
        public bool SeparateCooldowns { get; private set; }
        public bool SimplerWatermark { get { return false; } } // Valeur par défaut pour la marque d'eau simplifiée
        public bool SelectRoles { get { return false; } } // Valeur par défaut pour la sélection des rôles
        public bool UseCustomRegion { get { return false; } } // Valeur par défaut pour l'utilisation de régions personnalisées

        // Méthode pour recharger les paramètres et mettre à jour les valeurs des options
        public void ReloadSettings()
        {
            // Mettre à jour les valeurs des options à partir des paramètres actuels
            OrderOfTheImp = Option1.Value;
            SpellsInVents = Option3.Value;
            DefensiveDuelistCooldown = Option9.Value;
            InvisCloakCooldown = Option10.Value;
            HourglassCooldown = Option11.Value;
            CrucioCooldown = Option12.Value;
            ShowPopups = Option4.Value;
            SeparateCooldowns = !Option5.Value; // Inverser la valeur de l'option "Shared Voldemort Cooldowns"
        }
    }
}
