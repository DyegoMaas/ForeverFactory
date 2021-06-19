namespace ForeverFactory.Core.Transforms
{
    internal delegate TAffectedProperty SetMember<in T, out TAffectedProperty>(T arg);
}