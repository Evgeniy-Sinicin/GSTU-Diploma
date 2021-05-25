using Assets.Scripts.Bonuses;
using Assets.Scripts.Interfaces;
using System;

namespace Assets.Scripts.Factories
{
    public class AmmoFactory : IBonusFactory
    {
        public AbstractBonus Create()
        {
            return new Ammo();
        }
    }
}
