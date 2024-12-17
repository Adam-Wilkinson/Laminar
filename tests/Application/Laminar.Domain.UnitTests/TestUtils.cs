using System.Collections;

namespace Laminar.Domain.UnitTests;

public static class TestUtils
{
    public static bool SequenceEquals(IEnumerable? firstList, IEnumerable? secondList)
    {
        if (firstList is null)
        {
            return secondList is null;
        }

        if (secondList is null)
        {
            return false;
        }

        var first = firstList.GetEnumerator();
        var second = secondList.GetEnumerator();
        using var firstDisposable = first as IDisposable;
        using var secondDisposable = second as IDisposable;

        while (first.MoveNext())
        {
            if (!second.MoveNext())
            {
                return false;
            }

            if (first.Current is null && second.Current is not null || (first.Current is not null && !first.Current.Equals(second.Current)))
            {
                return false;
            }
        }

        return !second.MoveNext();
    }
    
    public static bool SequenceReferenceEquals(IEnumerable? firstList, IEnumerable? secondList)
    {
        if (firstList is null)
        {
            return secondList is null;
        }

        if (secondList is null)
        {
            return false;
        }

        var first = firstList.GetEnumerator();
        var second = secondList.GetEnumerator();
        using var firstDisposable = first as IDisposable;
        using var secondDisposable = second as IDisposable;

        while (first.MoveNext())
        {
            if (!second.MoveNext())
            {
                return false;
            }

            if (!ReferenceEquals(first.Current, second.Current))
            {
                return false;
            }
        }

        return !second.MoveNext();
    }
}