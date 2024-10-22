using UnityEngine;
namespace Weapons
{
    interface IFireable
    {
        int MaxAmmo { get; }
        int CurrentAmmo { get; set; }
        void Fire(Transform hitTransform);
        void Reload();
    }
}