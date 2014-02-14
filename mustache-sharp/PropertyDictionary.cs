using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;

namespace Mustache
{
    /// <summary>
    /// Provides methods for creating instances of PropertyDictionary.
    /// </summary>
    internal sealed class PropertyDictionary : IDictionary<string, object>
    {
		class VariableInfo {
			PropertyInfo _property;
			FieldInfo _field;

			public VariableInfo(PropertyInfo property) {
				_property = property;
			}

			public VariableInfo(FieldInfo field) {
				_field = field;
			}

			public object GetValue(object instance) {
				if (_property != null) return _property.GetValue(instance, null);
				if (_field != null) return _field.GetValue(instance);
				return null;
			}
		}
		private static readonly Dictionary<Type, Dictionary<string, VariableInfo>> _cache = new Dictionary<Type, Dictionary<string, VariableInfo>>();

        private readonly object _instance;
		private readonly Dictionary<string, VariableInfo> _typeCache;

        /// <summary>
        /// Initializes a new instance of a PropertyDictionary.
        /// </summary>
        /// <param name="instance">The instance to wrap in the PropertyDictionary.</param>
        public PropertyDictionary(object instance)
        {
            _instance = instance;
            if (instance == null)
            {
				_typeCache = new Dictionary<string, VariableInfo>();
            }
            else
            {
                _typeCache = getCacheType(_instance);
            }
        }

		private static Dictionary<string, VariableInfo> getCacheType(object instance)
        {
            Type type = instance.GetType();
			Dictionary<string, VariableInfo> typeCache;
            if (!_cache.TryGetValue(type, out typeCache))
            {
				typeCache = new Dictionary<string, VariableInfo>();
                BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.FlattenHierarchy;
                foreach (PropertyInfo propertyInfo in type.GetProperties(flags))
                {
                    if (!propertyInfo.IsSpecialName)
                    {
                        if (!typeCache.ContainsKey(propertyInfo.Name)) typeCache.Add(propertyInfo.Name, new VariableInfo(propertyInfo));
                    }
                }
				foreach (FieldInfo fieldInfo in type.GetFields(flags)) {
					if (!fieldInfo.IsSpecialName) {
						if (!typeCache.ContainsKey(fieldInfo.Name)) typeCache.Add(fieldInfo.Name, new VariableInfo(fieldInfo));
					}
				}
				_cache.Add(type, typeCache);
            }
            return typeCache;
        }

        /// <summary>
        /// Gets the underlying instance.
        /// </summary>
        public object Instance
        {
            get { return _instance; }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        void IDictionary<string, object>.Add(string key, object value)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Determines whether a property with the given name exists.
        /// </summary>
        /// <param name="key">The name of the property.</param>
        /// <returns>True if the property exists; otherwise, false.</returns>
        public bool ContainsKey(string key)
        {
            return _typeCache.ContainsKey(key);
        }

        /// <summary>
        /// Gets the name of the properties in the type.
        /// </summary>
        public ICollection<string> Keys
        {
            get { return _typeCache.Keys; }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        bool IDictionary<string, object>.Remove(string key)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Tries to get the value for the given property name.
        /// </summary>
        /// <param name="key">The name of the property to get the value for.</param>
        /// <param name="value">The variable to store the value of the property or the default value if the property is not found.</param>
        /// <returns>True if a property with the given name is found; otherwise, false.</returns>
        /// <exception cref="System.ArgumentNullException">The name of the property was null.</exception>
        public bool TryGetValue(string key, out object value)
        {
            VariableInfo propertyInfo;
            if (!_typeCache.TryGetValue(key, out propertyInfo))
            {
                value = null;
                return false;
            }
            value = getValue(propertyInfo);
            return true;
        }

        /// <summary>
        /// Gets the values of all of the properties in the object.
        /// </summary>
        public ICollection<object> Values
        {
            get
            {
				ICollection<VariableInfo> propertyInfos = _typeCache.Values;
                List<object> values = new List<object>();
				foreach (VariableInfo propertyInfo in propertyInfos)
                {
                    object value = getValue(propertyInfo);
                    values.Add(value);
                }
                return values.AsReadOnly();
            }
        }

        /// <summary>
        /// Gets or sets the value of the property with the given name.
        /// </summary>
        /// <param name="key">The name of the property to get or set.</param>
        /// <returns>The value of the property with the given name.</returns>
        /// <exception cref="System.ArgumentNullException">The property name was null.</exception>
        /// <exception cref="System.Collections.Generic.KeyNotFoundException">The type does not have a property with the given name.</exception>
        /// <exception cref="System.ArgumentException">The property did not support getting or setting.</exception>
        /// <exception cref="System.ArgumentException">
        /// The object does not match the target type, or a property is a value type but the value is null.
        /// </exception>
        public object this[string key]
        {
            get
            {
				VariableInfo propertyInfo = _typeCache[key];
                return getValue(propertyInfo);
            }
            [EditorBrowsable(EditorBrowsableState.Never)]
            set
            {
                throw new NotSupportedException();
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        void ICollection<KeyValuePair<string, object>>.Add(KeyValuePair<string, object> item)
        {
            throw new NotSupportedException();
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        void ICollection<KeyValuePair<string, object>>.Clear()
        {
            throw new NotSupportedException();
        }

        bool ICollection<KeyValuePair<string, object>>.Contains(KeyValuePair<string, object> item)
        {
			VariableInfo propertyInfo;
            if (!_typeCache.TryGetValue(item.Key, out propertyInfo))
            {
                return false;
            }
            object value = getValue(propertyInfo);
            return Equals(item.Value, value);
        }

        void ICollection<KeyValuePair<string, object>>.CopyTo(KeyValuePair<string, object>[] array, int arrayIndex)
        {
            List<KeyValuePair<string, object>> pairs = new List<KeyValuePair<string, object>>();
			ICollection<KeyValuePair<string, VariableInfo>> collection = _typeCache;
			foreach (KeyValuePair<string, VariableInfo> pair in collection)
            {
				VariableInfo propertyInfo = pair.Value;
                object value = getValue(propertyInfo);
                pairs.Add(new KeyValuePair<string, object>(pair.Key, value));
            }
            pairs.CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// Gets the number of properties in the type.
        /// </summary>
        public int Count
        {
            get { return _typeCache.Count; }
        }

        /// <summary>
        /// Gets or sets whether updates will be ignored.
        /// </summary>
        bool ICollection<KeyValuePair<string, object>>.IsReadOnly
        {
            get { return true; }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        bool ICollection<KeyValuePair<string, object>>.Remove(KeyValuePair<string, object> item)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Gets the propety name/value pairs in the object.
        /// </summary>
        /// <returns></returns>
        public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
        {
			foreach (KeyValuePair<string, VariableInfo> pair in _typeCache)
            {
                object value = getValue(pair.Value);
                yield return new KeyValuePair<string, object>(pair.Key, value);
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

		private object getValue(VariableInfo propertyInfo)
        {
            return propertyInfo.GetValue(_instance);
        }
    }
}