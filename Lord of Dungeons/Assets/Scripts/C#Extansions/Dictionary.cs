using System.Collections.Generic;

namespace System.Collections.Generic
{
    public class CustomDictionary<TKey, TValue> : Dictionary<TKey, TValue>
    {
        public T GetKey<T, W>(Dictionary<T, W> dict)
        {
            T key = default;
            foreach (KeyValuePair<T, W> pair in dict)
            {
                key = pair.Key;
                break;

            }
            return key;
        }
        public T KeyByValue<T, W>(Dictionary<T, W> dict, W val)
        {
            T key = default;
            foreach (KeyValuePair<T, W> pair in dict)
            {
                if (EqualityComparer<W>.Default.Equals(pair.Value, val))
                {
                    key = pair.Key;
                    break;
                }
            }
            return key;
        }
    }
}
