namespace Elebris_WPF_Rpg.Models.Actions
{
    public class AttackWithWeapon : BaseAction, IAction
    {
        private readonly int _damage;

        public AttackWithWeapon(GameItem itemInUse, int damage)
            : base(itemInUse)
        {
            if (itemInUse.Category != GameItem.ItemCategory.Weapon)
            {
                throw new ArgumentException($"{itemInUse.Name} is not a weapon");
            }


            _damage = damage;
        }

        public void Execute(LivingEntity actor, LivingEntity target)
        {
            string actorName = actor is Player ? "You" : $"The {actor.Name.ToLower()}";
            string targetName = target is Player ? "you" : $"the {target.Name.ToLower()}";

            if (AttackSucceeded(actor, target))
            {
                //Fake Value: Not using a dice System
                int damage = _damage;

                ReportResult($"{actorName} hit {targetName} for {damage} point{(damage > 1 ? "s" : "")}.");

                target.TakeDamage(damage);
            }
            else
            {
                ReportResult($"{actorName} missed {targetName}.");
            }
        }

        private static bool AttackSucceeded(LivingEntity attacker, LivingEntity target)
        {

            Random rand = new Random();

            if(rand.Next(100) < 70)
            {
                return false;
            }
            //Fake Value: Not using a dice System and setting a static 30% hit chance
            return true;
        }
    }
}
