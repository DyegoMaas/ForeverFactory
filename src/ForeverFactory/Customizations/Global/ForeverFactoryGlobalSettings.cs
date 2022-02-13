using ForeverFactory.Behaviors;

namespace ForeverFactory.Customizations.Global
{
    public static class ForeverFactoryGlobalSettings
    {
        internal static Behavior GlobalBehavior { get; set; }
        
        public static void UseBehavior<TBehavior>(TBehavior behavior)
            where TBehavior : Behavior, new()
        {
            GlobalBehavior = behavior;
        }

        internal static void ResetBehavior()
        {
            GlobalBehavior = null;
        }
    }
}