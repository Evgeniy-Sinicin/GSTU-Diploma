using Assets.Scripts.Bonuses;
using Assets.Scripts.Interfaces;

namespace Assets.Scripts.Factories
{
    public class HealthFactory : IBonusFactory
    {
        public AbstractBonus Create()
        {
            return new Health();
        }
    }
}
