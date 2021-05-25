using Assets.Scripts.Interfaces;
using UnityEngine;

namespace Assets.Scripts.Bonuses
{
    public class Armor : AbstractBonus
    {
        [SerializeField]
        private GameObject prefab;

        [SerializeField]
        private int incrementArmor = 10;
        [SerializeField]
        private int incrementScore = 20;

        public override void Buff(Player _player)
        {
            _player.AddArmor(incrementArmor);
            _player.AddScore(incrementScore);
        }

        public override GameObject GetPrefab()
        {
            return prefab;
        }
    }
}
