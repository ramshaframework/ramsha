using System.Collections;
using System.Reflection;

namespace Ramsha;

public class TypeList : TypeList<object>, ITypeList
{
}

public class TypeList<TBaseType> : ITypeList<TBaseType>
{

    public int Count => _typeList.Count;

    public bool IsReadOnly => false;

    public Type this[int index]
    {
        get { return _typeList[index]; }
        set
        {
            CheckType(value);
            _typeList[index] = value;
        }
    }

    private readonly List<Type> _typeList;


    public TypeList()
    {
        _typeList = new List<Type>();
    }

    public void Add<T>() where T : TBaseType
    {
        _typeList.Add(typeof(T));
    }

    public bool TryAdd<T>() where T : TBaseType
    {
        if (Contains<T>())
        {
            return false;
        }

        Add<T>();
        return true;
    }

    public void Add(Type item)
    {
        CheckType(item);
        _typeList.Add(item);
    }

    public void Insert(int index, Type item)
    {
        CheckType(item);
        _typeList.Insert(index, item);
    }

    public int IndexOf(Type item)
    {
        return _typeList.IndexOf(item);
    }

    public bool Contains<T>() where T : TBaseType
    {
        return Contains(typeof(T));
    }

    public bool Contains(Type item)
    {
        return _typeList.Contains(item);
    }

    public void Remove<T>() where T : TBaseType
    {
        _typeList.Remove(typeof(T));
    }

    public bool Remove(Type item)
    {
        return _typeList.Remove(item);
    }

    public void RemoveAt(int index)
    {
        _typeList.RemoveAt(index);
    }

    public void Clear()
    {
        _typeList.Clear();
    }

    public void CopyTo(Type[] array, int arrayIndex)
    {
        _typeList.CopyTo(array, arrayIndex);
    }

    public IEnumerator<Type> GetEnumerator()
    {
        return _typeList.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return _typeList.GetEnumerator();
    }

    private static void CheckType(Type item)
    {
        if (!typeof(TBaseType).GetTypeInfo().IsAssignableFrom(item))
        {
            throw new ArgumentException($"Given type ({item.AssemblyQualifiedName}) should be instance of {typeof(TBaseType).AssemblyQualifiedName} ", nameof(item));
        }
    }

    public void AddAfter<TExisting, TNew>()
    where TExisting : TBaseType
    where TNew : TBaseType
    {
        AddAfter(typeof(TExisting), typeof(TNew));
    }

    public void AddAfter(Type existingType, Type newType)
    {
        CheckType(newType);

        var index = _typeList.IndexOf(existingType);
        if (index < 0)
        {
            throw new ArgumentException($"Existing type {existingType.FullName} not found in the list.", nameof(existingType));
        }

        _typeList.Insert(index + 1, newType);
    }

    public void AddBefore<TExisting, TNew>()
        where TExisting : TBaseType
        where TNew : TBaseType
    {
        AddBefore(typeof(TExisting), typeof(TNew));
    }

    public void AddBefore(Type existingType, Type newType)
    {
        CheckType(newType);

        var index = _typeList.IndexOf(existingType);
        if (index < 0)
        {
            throw new ArgumentException($"Existing type {existingType.FullName} not found in the list.", nameof(existingType));
        }

        _typeList.Insert(index, newType);
    }

}