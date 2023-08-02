using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class stores a comprehensive set of information related to the result of the execution of an Action.
/// </summary>
public class Result
{
    private Dictionary<string, object> dict = new Dictionary<string, object>();

    /// <summary>
    /// Assigns a value to the given key in the result dictionary.
    /// </summary>
    /// <param name="key">The name of the key.</param>
    /// <param name="value">The object to associate with the key.</param>
    public void Set(string key, object value)
    {
        dict[key] = value;
    }

    /// <summary>
    /// Retrieves the value for a given key in the result dictionary.
    /// </summary>
    /// <param name="key">The name of the key.</param>
    /// <typeparam name="T">The type of value required.</typeparam>
    /// <returns></returns>
    public T Get<T>(string key)
    {
        if (dict.ContainsKey(key))
        {
            return (T)dict[key];
        }
        return default(T);  // Return default value for the type if key not found
    }

    /// <summary>
    /// Adds the given value to the value associated with the given key in the result dictionary.
    /// </summary>
    /// <param name="key">The name of the key.</param>
    /// <param name="value">The value to add.</param>
    public void Add(string key, float value)
    {
        if (!dict.ContainsKey(key))  // No existing value
        {
            dict[key] = 0f;
        }
        if (dict[key] is float)  // Value exists
        {
            dict[key] = (float)(dict[key]) + value;
        }
        else  // Value exists for this key, but it is not an addable type
        {
            throw new InvalidOperationException($"The value associated with key '{key}' is not a float.");
        }
    }

    /// <summary>
    /// Appends the given value to the list associated with the given key in the result dictionary. If the list does not yet exist, it creates one and adds the value.
    /// </summary>
    /// <param name="key">The name of the key.</param>
    /// <param name="value">The value to append.</param>
    /// <typeparam name="T">The type of value to append.</typeparam>
    public void Append<T>(string key, T value)
    {
        if (!dict.ContainsKey(key))  // List does not exist yet
        {
            dict[key] = new List<T>();
        }
        if (dict[key] is List<T> list)  // List exists
        {
            list.Add(value);
        }
        else  // Value exists for this key, but it is not a list
        {
            throw new InvalidOperationException($"The value associated with key '{key}' is not a list.");
        }
    }
}