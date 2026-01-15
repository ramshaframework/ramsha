namespace Ramsha;

public sealed class RamshaTypeReplacementOptions
{
    private readonly Dictionary<Type, Type> _replacements = new();

    public IReadOnlyDictionary<Type, Type> Replacements => _replacements;

    public RamshaTypeReplacementOptions Replace<TBase, TImplementation>()
        where TImplementation : TBase
    {
        Replace(typeof(TBase), typeof(TImplementation));
        return this;
    }

    public RamshaTypeReplacementOptions Replace(Type baseType, Type implementationType)
    {
        if (!baseType.IsAssignableFrom(implementationType))
        {
            throw new ArgumentException(
                $"{implementationType.FullName} must inherit from {baseType.FullName}");
        }

        _replacements[baseType] = implementationType;
        return this;
    }

    public Type GetOrBase(Type baseType)
    {
        return _replacements.TryGetValue(baseType, out var impl)
            ? impl
            : baseType;
    }

    public Type GetOrBase<TBase>()
        => GetOrBase(typeof(TBase));
}