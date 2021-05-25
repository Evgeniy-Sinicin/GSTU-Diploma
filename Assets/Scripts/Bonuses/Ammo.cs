using Assets.Scripts.Interfaces;
using UnityEngine;

namespace Assets.Scripts.Bonuses
{
    public class Ammo : AbstractBonus
    {
        [SerializeField]
        private GameObject prefab;

        [SerializeField]
        private int incrementAmmo = 30;
        [SerializeField]
        private int incrementScore = 30;

        public override void Buff(Player _player)
        {
            _player.AddBullets(incrementAmmo);
            _player.AddScore(incrementScore);
        }

        public override GameObject GetPrefab()
        {
            return prefab;
        }
    }
}
