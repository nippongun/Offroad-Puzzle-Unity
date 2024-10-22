using UnityEngine;

namespace Weapons
{
    public class Weapon : MonoBehaviour, IFireable
    {
        [Header("Stats")]
        [SerializeField] int damage = 10;
        [SerializeField] float range = 100f;
        [SerializeField] float fireRate = 600f;
        [SerializeField] float impactForce = 30f;
        [SerializeField] int maxAmmo = 30;
        [SerializeField] int currentAmmo;
        [Header("References")]
        [SerializeField] ParticleSystem muzzleFlash;
        AudioSource shootingSound;

        private float lastFireTime = 0f;
        public int MaxAmmo => maxAmmo;
        public int CurrentAmmo { get => currentAmmo; set => currentAmmo = value; }

        void Awake()
        {

            shootingSound = GetComponent<AudioSource>();
            if (muzzleFlash == null)
            {
                muzzleFlash = GetComponentInChildren<ParticleSystem>();
            }
        }

        void Start()
        {
            currentAmmo = maxAmmo;
        }

        public void Fire(Transform hitTransform)
        {
            if (!CanFire()) return;

            PlayMuzzleFlash();
            PlayShootingSound();
            lastFireTime = Time.time;

            if (currentAmmo <= 0)
            {
                Debug.Log("OUT OF AMMO");
                return;
            }

            currentAmmo -= 1;
            Debug.Log($"AMMO: {currentAmmo}");

            if (hitTransform != null)
            {
                Debug.Log(" HIT");
                if (hitTransform.TryGetComponent<IDamageable>(out IDamageable damageable))
                {
                    Debug.Log($"Damageable found: {damageable}");
                    damageable.TakeDamage(damage);
                }
            }
        }

        public void Reload()
        {
            currentAmmo = maxAmmo;
            Debug.Log("RELOADED");
        }

        private void PlayMuzzleFlash()
        {
            if (muzzleFlash != null)
            {
                muzzleFlash.Play();
            }
            else
            {
                Debug.LogError("Attempted to play muzzle flash, but it is null. Check inspector assignment and script execution order.");
            }
        }

        private void PlayShootingSound()
        {
            if (shootingSound != null)
            {
                shootingSound.Play();
                Debug.Log("SHOOTING SOUND");
            }
            else
            {
                Debug.LogError("Attempted to play shooting sound, but it is null. Check inspector assignment and script execution order.");
            }
        }

        public bool CanFire()
        {
            Debug.Log($"{(float)1 / (fireRate / 60f)}");
            return Time.time - lastFireTime >= (1 / (fireRate / 60f));
        }
    }
}