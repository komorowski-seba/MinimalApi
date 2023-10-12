using System.Collections;

namespace Domain.Common;

public abstract class ValueObject : Entitie, IEnumerable<object>
{
    public abstract IEnumerator<object> GetEnumerator();

    public override bool Equals(object? obj)
    {
        if (obj is null || obj.GetType() != GetType())
            return false;

        if (ReferenceEquals(this, obj))
            return true;

        using var values = GetEnumerator();
        using var objValues = ((ValueObject)obj).GetEnumerator();

        while (objValues.MoveNext() && values.MoveNext())
        {
            if (values.Current is not null && !values.Current.Equals(objValues.Current))
                return false;
        }

        return true;
    }

    public override int GetHashCode()
    {
        return this
            .Select(n => n.GetHashCode())
            .Aggregate(((n, m) => n ^ m));
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}