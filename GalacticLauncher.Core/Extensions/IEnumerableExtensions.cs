namespace GalacticLauncher.Core.Extensions;

public static class IEnumerableExtensions
{
    public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> source, Random random)
    {
        T[] array = [.. source];

        for (int i = 0; i < array.Length; i++)
        {
            int j = random.Next(i, array.Length);
            (array[i], array[j]) = (array[j], array[i]);
        }

        return array;
    }

    public static IEnumerable<T> Limit<T>(this IEnumerable<T> source, int count)
    {
        foreach (T item in source)
        {
            if (count-- <= 0) break;
            yield return item;
        }
    }
}
