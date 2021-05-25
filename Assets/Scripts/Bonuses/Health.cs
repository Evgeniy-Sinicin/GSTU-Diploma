using Assets.Scripts.Interfaces;
using UnityEngine;

namespace Assets.Scripts.Bonuses
{
    public class Health : AbstractBonus
    {
        [SerializeField]
        private GameObject prefab;

        [SerializeField]
        private int incrementHealth = 20;
        [SerializeField]
        private int incrementScore = 10;

        public override void Buff(Player _player)
        {
            _player.Heal(incrementHealth);
            _player.AddScore(incrementScore);
        }

        public override GameObject GetPrefab()
        {
            return prefab;
        }
    }
}
