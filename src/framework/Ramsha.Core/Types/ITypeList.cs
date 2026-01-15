

namespace Ramsha;

public interface ITypeList : ITypeList<object>
{

}


public interface ITypeList<in TBaseType> : IList<Type>
{

    void Add<T>() where T : TBaseType;

    bool TryAdd<T>() where T : TBaseType;
    bool Contains<T>() where T : TBaseType;

    void Remove<T>() where T : TBaseType;
    void AddAfter<TExisting, TNew>()
      where TExisting : TBaseType
    where TNew : TBaseType;

    public void AddBefore<TExisting, TNew>()
      where TExisting : TBaseType
      where TNew : TBaseType;
}

