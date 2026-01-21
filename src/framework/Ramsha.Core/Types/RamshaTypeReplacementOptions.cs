namespace Ramsha;

public sealed class RamshaTypeReplacementOptions
{
    private readonly Dictionary<Type, Type> _replacements = new();

    public IReadOnlyDictionary<Type, Type> Replacements => _replacements;

    public RamshaTypeReplacementOptions ReplaceBase<TBase, TImplementation>()
        where TImplementation : TBase
    {

        ReplaceBase(typeof(TBase), typeof(TImplementation));
        return this;
    }

    public RamshaTypeReplacementOptions Replace<TKey, TTarget>()
    where TKey : IRamshaTypedKey
    {
        Replace(typeof(TKey), typeof(TTarget));
        return this;
    }

    public RamshaTypeReplacementOptions Replace(Type key, Type targetType)
    {
        if (!typeof(IRamshaTypedKey).IsAssignableFrom(key))
        {
            throw new ArgumentException(
                $"The Type {key.FullName} is not a typed key.");
        }

        _replacements[key] = targetType;
        return this;
    }


    public RamshaTypeReplacementOptions ReplaceBase(Type baseType, Type implementationType)
    {
        if (!baseType.IsAssignableFrom(implementationType))
        {
            throw new ArgumentException(
                $"{implementationType.FullName} must inherit from {baseType.FullName}");
        }

        _replacements[baseType] = implementationType;
        return this;
    }

    public Type GetOrSelf(Type keyType)
    {
        return _replacements.TryGetValue(keyType, out var impl)
            ? impl
            : keyType;
    }
    public Type? GetOrNull(Type keyType)
    {
        return _replacements.TryGetValue(keyType, out var impl)
            ? impl
            : null;
    }

    public bool TryGet(Type keyType, out Type targetType)
    => _replacements.TryGetValue(keyType, out targetType);

    public Type? GetOrNull<T>()
    {
        return GetOrNull(typeof(T));
    }

    public Type GetOrSelf<TBase>()
        => GetOrSelf(typeof(TBase));
}