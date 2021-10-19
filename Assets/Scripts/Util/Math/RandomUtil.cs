using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Helper class for methods related to random.
/// </summary>
public static class RandomUtil
{
    /// <summary>
    /// Returns a random element of a given array.
    /// </summary>
    /// <typeparam name="T">The type of the array.</typeparam>
    /// <param name="array">The array to take the random element from.</param>
    /// <returns>The random element.</returns>
    public static T Element<T>(T[] array) => array[Random.Range(0, array.Length)];

    /// <summary>
    /// Returns a random element of a given list.
    /// </summary>
    /// <typeparam name="T">The type of the list.</typeparam>
    /// <param name="array">The list to take the random element from.</param>
    /// <returns>The random element.</returns>
    public static T Element<T>(List<T> array) => array[Random.Range(0, array.Count)];

    /// <summary>
    /// Returns an array with random elements of a given array with no duplicates.
    /// </summary>
    /// <typeparam name="T">The type of the array.</typeparam>
    /// <param name="array">The array to take random elements from.</param>
    /// <param name="amount">How many elements are taken from the array.</param>
    /// <returns>The random elements of the given array.</returns>
    public static T[] ElementsNoDuplicates<T>(T[] array, int amount)
    {
        if (amount > array.Length)
        {
            Debug.LogError("Can not generate that many elements without duplicates!");
            return new T[0];
        }
        T[] retArr = new T[amount];
        if (amount == array.Length)
        {
            System.Array.Copy(array, retArr, amount);
            for (int i = 0; i < retArr.Length; i++)
            {
                int index = Random.Range(0, retArr.Length);
                T temp = retArr[i];
                retArr[i] = retArr[index];
                retArr[index] = temp;
            }
            return retArr;
        }

        HashSet<int> alreadyGeneratedIndexes = new HashSet<int>();
        for (int i = 0; i < retArr.Length; i++)
        {
            int toGenerate;
            do
            {
                toGenerate = Random.Range(0, array.Length);
            } while (alreadyGeneratedIndexes.Contains(toGenerate));
            alreadyGeneratedIndexes.Add(toGenerate);
            retArr[i] = array[toGenerate];
        }
        return retArr;
    }
}
