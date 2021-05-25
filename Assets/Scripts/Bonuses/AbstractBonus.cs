using Mirror;
using UnityEngine;

namespace Assets.Scripts.Bonuses
{
    public abstract class AbstractBonus : NetworkBehaviour
    {
        public abstract void Buff(Player _player);
        public abstract GameObject GetPrefab();
    }
}
