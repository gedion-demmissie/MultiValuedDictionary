using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Security.Cryptography.X509Certificates;

namespace MultiValuedKeyDictionary
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Multi Valued Collection Test Runner");
            MultiDict<int, string> multivaluedDictionary = new MultiDict<int, string>();

            //Let's Add values;

            bool added = multivaluedDictionary.Add(1, "banana");
            if (added)
            {
                Console.WriteLine("(1, 'banana') pair  is Added.");
            }
            added = multivaluedDictionary.Add(1, "orange");
            if (added)
            {
                Console.WriteLine("(1, 'orange') pair  is Added.");
            }
            Console.WriteLine("Key 1 has the following values:");
            foreach (var item in multivaluedDictionary[1])
            {
                Console.WriteLine(item);
            }
            Console.WriteLine("remove 'banana' from  Key 1 ");
            bool removed = multivaluedDictionary.Remove(1, "banana");
            if (removed)
            {
                Console.WriteLine(" 'banana'  is removed from key 1.");
            }

            Console.WriteLine("Now Key 1 has the following values:");
            foreach (var item in multivaluedDictionary[1])
            {
                Console.WriteLine(item);
            }
            Console.ReadKey();
        }
    }

    public interface IMultiDict<K, V> : IDictionary<K, V>
    {
       
        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns>boolean</returns>
        bool Add(K key, V value);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        IEnumerable<V> Get(K key);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        IEnumerable<V> GetOrDefault(K key);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// throws exception if element doesn't exist.
        bool Remove(K key);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns>boolean</returns>
        bool Remove(K key, V value);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        IEnumerable<KeyValuePair<K, V>> Flatten();

    }
    class MultiDict<TKey, TValue> : IMultiDict<TKey, TValue>
    {
        private Dictionary<TKey, Dictionary<TValue, TValue>> _data = new Dictionary<TKey, Dictionary<TValue, TValue>>();

        public ICollection<TKey> Keys => throw new NotImplementedException();

        public ICollection<TValue> Values => throw new NotImplementedException();

        public int Count => throw new NotImplementedException();

        public bool IsReadOnly => throw new NotImplementedException();

        TValue IDictionary<TKey, TValue>.this[TKey key] { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public IEnumerable<TValue> this[TKey key] { get => this.GetOrDefault(key); }

        public bool Add(TKey k, TValue v)
        {
            var isAdded = false;
            // can be a optimized a little with TryGetValue, this is for clarity
            if (_data.ContainsKey(k))
            {
                if(!_data[k].ContainsKey(v))
                {

                    _data[k].Add(v, v);
                    isAdded = true;
                }
            }                
            else
            {
                var keyValDictionary = new Dictionary<TValue, TValue>
                {
                    { v, v }
                };
                _data.Add(k, keyValDictionary);
                isAdded = true;
            }

            return isAdded;              
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        public bool Remove(TKey k)
        {
            bool isRemoved = false;
           
            // can be a optimized a little with TryGetValue, this is for clarity
            if (_data.ContainsKey(k))
            {
                _data.Remove(k);
                isRemoved = true;
            }

            return isRemoved;
        }

      
        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool Remove(TKey k, TValue v)
        {
            var isRemoved = false;
            // can be a optimized a little with TryGetValue, this is for clarity
            if (_data.ContainsKey(k))
            {
                if (_data[k].ContainsKey(v))
                {
                    _data[k].Remove(v);
                    isRemoved = true;
                }
            }          

            return isRemoved;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public IEnumerable<TValue> Get(TKey key)
        {
            if (_data.ContainsKey(key))
            {
                return _data[key].Values;
            }
            else
            {
                throw new KeyNotFoundException();
            }
                
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public IEnumerable<TValue> GetOrDefault(TKey key)
        {
            if (_data.ContainsKey(key))
            {
                return _data[key].Values;
            }
            else
            {
               return  Enumerable.Empty<TValue>();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void Clear()
        {
            _data.Clear();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IEnumerable<KeyValuePair<TKey, TValue>> Flatten()
        {
            IEnumerable<KeyValuePair<TKey, TValue>> flattenedContent = new List<KeyValuePair<TKey, TValue>>();
            foreach (var (item, value) in from KeyValuePair<TKey, Dictionary<TValue, TValue>> item in _data
                                          let values = item.Value.Keys
                                          from TValue value in values
                                          select (item, value))
            {
                flattenedContent.Append(new KeyValuePair<TKey, TValue>(item.Key, value));
            }

            return flattenedContent;
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool TryGetValue(TKey key, [MaybeNullWhen(false)] out TValue value)
        {
            value = default;
            return false;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        public void Add(KeyValuePair<TKey, TValue> item)
        {
            throw new NotImplementedException();
        }

        void IDictionary<TKey, TValue>.Add(TKey key, TValue value)
        {
            throw new NotImplementedException();
        }

        public bool ContainsKey(TKey key)
        {
            throw new NotImplementedException();
        }

        bool IDictionary<TKey, TValue>.Remove(TKey key)
        {
            throw new NotImplementedException();
        }

        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            throw new NotImplementedException();
        }

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            throw new NotImplementedException();
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }



        // more members
    }


}
