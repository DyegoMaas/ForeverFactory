using ForeverFactory.Behaviors;

namespace ForeverFactory
{
    public static class ForeverFactoryGlobalSettings
    {
        internal static Behavior GlobalBehavior { get; private set; }
        
        public static void UseBehavior<TBehavior>(TBehavior behavior)
            where TBehavior : Behavior
        {
            GlobalBehavior = behavior;
        }

        internal static void ResetBehavior()
        {
            GlobalBehavior = null;
        }
    }
}