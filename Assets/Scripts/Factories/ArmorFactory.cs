using Assets.Scripts.Bonuses;
using Assets.Scripts.Interfaces;

namespace Assets.Scripts.Factories
{
    public class ArmorFactory : IBonusFactory
    {
        public AbstractBonus Create()
        {
            return new Armor();
        }
    }
}
