using System;
using hunterlib.Classes;
using InnerNet;
using UnityEngine;

namespace HarryPotter.Classes
{
    public delegate void HitEvent(Spell spell, PlayerControl player);
    
    [RegisterInIl2Cpp]
    public class Spell : MonoBehaviour
    {
        public ModdedPlayerClass Owner { get; set; }
        public Vector3 MousePosition { get; set; } // Correction du nom de la propriété
        public SpriteRenderer SpellRender { get; set; }
        public Rigidbody2D SpellRigid { get; set; }
        public CircleCollider2D SpellCollider { get; set; }
        public DateTime ShootTime { get; set; }
        public Sprite[] SpellSprites { get; set; }
        
        public event HitEvent OnHit;
        private int _spriteIndex;
        
        public Spell(IntPtr ptr) : base(ptr)
        {
        }

        public void Start()
        {
            ShootTime = DateTime.UtcNow;
            
            // Ajout des composants nécessaires au sort
            SpellRender = gameObject.AddComponent<SpriteRenderer>();
            SpellRigid = gameObject.AddComponent<Rigidbody2D>();
            SpellCollider = gameObject.AddComponent<CircleCollider2D>();
            
            gameObject.SetActive(true);
            SpellRender.enabled = true;
            
            // Initialisation de la position du sort
            SpellRigid.transform.position = Owner._Object.myRend.bounds.center;
            SpellRender.transform.localScale = new Vector2(1f, 1f);

            // Calcul de la direction du tir
            Vector3 v = MousePosition - Owner._Object.myRend.bounds.center;
            float dist = Vector2.Distance(MousePosition, Owner._Object.myRend.bounds.center);
            Vector3 d = v * 3f * (2f / dist); // Normalisation du vecteur directionnel
            float AngleRad = Mathf.Atan2(MousePosition.y - Owner._Object.myRend.bounds.center.y, MousePosition.x - Owner._Object.myRend.bounds.center.x);
            float shootDeg = (180 / (float)Math.PI) * AngleRad;

            // Configuration du collider et de la vitesse du sort
            SpellCollider.isTrigger = true;
            SpellCollider.radius = 0.2f;
            SpellRigid.velocity = new Vector2(d.x, d.y);
            gameObject.layer = 31;

            // Paramètres de la physique du sort
            SpellRigid.rotation = shootDeg;
            SpellRigid.drag = 0;
            SpellRigid.angularDrag = 0;
            SpellRigid.inertia = 0;
            SpellRigid.gravityScale = 0;
        }

        public void Update()
        {
            // Changement de sprite pour l'animation
            if (_spriteIndex <= 5)
                SpellRender.sprite = SpellSprites[0];
            else
                SpellRender.sprite = SpellSprites[1];

            // Réinitialisation de l'index du sprite
            if (_spriteIndex >= 10)
                _spriteIndex = 0;

            _spriteIndex++;

            // Détection des collisions avec les joueurs
            if (Owner._Object.AmOwner)
            {
                foreach (PlayerControl player in PlayerControl.AllPlayerControls)
                {
                    // Ignore les joueurs morts, déconnectés, ou les imposteurs
                    if (player.Data.IsDead || player.Data.Disconnected || Owner._Object == player || player.Data.IsImpostor)
                        continue;

                    // Vérification si le sort touche un joueur
                    if (!player.myRend.bounds.Intersects(SpellRender.bounds))
                        continue;

                    // Vérification de l'activation du collider du joueur
                    if (!player.Collider.enabled)
                        continue;

                    // Déclenche l'événement OnHit
                    OnHit?.Invoke(this, player);
                    return; // Sortie après avoir touché un joueur
                }
            }

            // Destruction du sort après 5 secondes ou si le jeu est terminé
            if (ShootTime.AddSeconds(5) < DateTime.UtcNow ||
                MeetingHud.Instance ||
                !AmongUsClient.Instance.IsGameStarted)
            {
                Destroy(gameObject); // Utilisation de Destroy pour une meilleure gestion de la mémoire
            }
        }

        public void OnTriggerEnter2D(Collider2D other)
        {
            if (other.isTrigger) return; // Ignore les objets trigger

            // Détection de collision avec certains layers et déclenchement de l'événement
            switch (other.gameObject.layer)
            {
                case 10:
                case 11:
                    OnHit?.Invoke(this, null);
                    break;
            }
        }
    }
}
