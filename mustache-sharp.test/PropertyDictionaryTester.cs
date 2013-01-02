using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace mustache.test
{
    /// <summary>
    /// Tests the PropertyDictionary class.
    /// </summary>
    [TestClass]
    public class PropertyDictionaryTester
    {
        #region Real World Example

        /// <summary>
        /// The purpose of the PropertyDictionary class is to allow an object to be inspected, 
        /// as if it were a dictionary. This means we can get and set properties by their names.
        /// </summary>
        [TestMethod]
        public void TestPropertyDictionary_AccessPropertiesViaIndexer()
        {
            var person = new
            {
                Name = "Bob",
                Age = 23,
                Birthday = new DateTime(2012, 03, 12)
            };
            PropertyDictionary wrapper = new PropertyDictionary(person);

            Assert.AreEqual(3, wrapper.Count, "The wrong number of properties were created.");

            Assert.IsTrue(wrapper.ContainsKey("Name"));
            Assert.IsTrue(wrapper.ContainsKey("Age"));
            Assert.IsTrue(wrapper.ContainsKey("Birthday"));

            Assert.AreEqual(person.Name, wrapper["Name"], "The name was not wrapped.");
            Assert.AreEqual(person.Age, wrapper["Age"], "The age was not wrapped.");
            Assert.AreEqual(person.Birthday, wrapper["Birthday"], "The birthday was not wrapped.");
        }

        #endregion

        #region Ctor & Instance & IsReadOnly

        /// <summary>
        /// If we try to wrap null, an exception should be thrown.
        /// </summary>
        [TestMethod]
        public void TestCtor_NullInstance_ThrowsException()
        {
            PropertyDictionary dictionary = new PropertyDictionary(null);
            Assert.AreEqual(0, dictionary.Count);
        }

        /// <summary>
        /// We should be able to access the underlying object.
        /// </summary>
        [TestMethod]
        public void TestCtor_SetsInstance()
        {
            object instance = new object();
            PropertyDictionary dictionary = new PropertyDictionary(instance);
            Assert.AreSame(instance, dictionary.Instance, "The instance was not set.");
            ICollection<KeyValuePair<string, object>> collection = dictionary;
            Assert.IsTrue(collection.IsReadOnly, "The collection should not have been read-only.");
        }

        #endregion

        #region Add

        /// <summary>
        /// Since the dictionary is a simple wrapper around an object, we cannot add new properties.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(NotSupportedException))]
        public void TestAdd_IDictionary_ThrowsException()
        {
            IDictionary<string, object> dictionary = new PropertyDictionary(new object());
            dictionary.Add("Name", "Bob");
        }

        /// <summary>
        /// Since the dictionary is a simple wrapper around an object, we cannot add new properties.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(NotSupportedException))]
        public void TestAdd_ICollection_ThrowsException()
        {
            ICollection<KeyValuePair<string, object>> collection = new PropertyDictionary(new object());
            collection.Add(new KeyValuePair<string, object>("Name", "Bob"));
        }

        #endregion

        #region ContainsKey

        /// <summary>
        /// If the wrapped object has a property, the key should be found.
        /// </summary>
        [TestMethod]
        public void TestContainsKey_PropertyExists_ReturnsTrue()
        {
            var person = new
            {
                Name = "Bob",
            };
            PropertyDictionary dictionary = new PropertyDictionary(person);
            bool result = dictionary.ContainsKey("Name");
            Assert.IsTrue(result, "The property name was not found.");
        }

        /// <summary>
        /// If the wrapped object does not have a property, the key should not be found.
        /// </summary>
        [TestMethod]
        public void TestContainsKey_PropertyMissing_ReturnsFalse()
        {
            var person = new { };
            PropertyDictionary dictionary = new PropertyDictionary(person);
            bool result = dictionary.ContainsKey("Name");
            Assert.IsFalse(result, "The property name was found.");
        }

        private class BaseType
        {
            public string Inherited { get; set; }
        }

        private class DerivedType : BaseType
        {
            public string Local { get; set; }
        }

        /// <summary>
        /// We should be able to see properties defined in the base type.
        /// </summary>
        [TestMethod]
        public void TestContainsKey_PropertyInherited_ReturnsTrue()
        {
            BaseType b = new DerivedType();
            PropertyDictionary dictionary = new PropertyDictionary(b);
            bool result = dictionary.ContainsKey("Inherited");
            Assert.IsTrue(result, "The property name was not found.");
        }

        private class PrivateType
        {
            private string Hidden { get; set; }
        }

        /// <summary>
        /// We should not be able to see private properties.
        /// </summary>
        [TestMethod]
        public void TestContainsKey_PropertyPrivate_ReturnsFalse()
        {
            PrivateType t = new PrivateType();
            PropertyDictionary dictionary = new PropertyDictionary(t);
            bool result = dictionary.ContainsKey("Hidden");
            Assert.IsFalse(result, "The property name was found.");
        }

        private class StaticType
        {
            public static string Static { get; set; }
        }

        /// <summary>
        /// We should not be able to see static properties.
        /// </summary>
        [TestMethod]
        public void TestContainsKey_PropertyStatic_ReturnsFalse()
        {
            StaticType t = new StaticType();
            PropertyDictionary dictionary = new PropertyDictionary(t);
            bool result = dictionary.ContainsKey("Static");
            Assert.IsFalse(result, "The property name was found.");
        }

        #endregion

        #region Keys

        /// <summary>
        /// Keys should return the name of all of the property names in the object.
        /// </summary>
        [TestMethod]
        public void TestKeys_GetsAllPropertyNames()
        {
            var person = new
            {
                Name = "Bob",
                Age = 23
            };
            PropertyDictionary dictionary = new PropertyDictionary(person);
            ICollection<string> keys = dictionary.Keys;
            Assert.AreEqual(2, keys.Count, "The wrong number of keys were returned.");
            Assert.IsTrue(keys.Contains("Name"), "The Name property was not found.");
            Assert.IsTrue(keys.Contains("Age"), "The Age property was not found.");
        }

        #endregion

        #region Remove

        /// <summary>
        /// Since a property dictionary is just a wrapper around an object, we cannot remove properties from it.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(NotSupportedException))]
        public void TestRemove_IDictionary_ThrowsException()
        {
            object instance = new object();
            IDictionary<string, object> dictionary = new PropertyDictionary(instance);
            dictionary.Remove("Name");
        }

        /// <summary>
        /// Since a property dictionary is just a wrapper around an object, we cannot remove properties from it.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(NotSupportedException))]
        public void TestRemove_ICollection_ThrowsException()
        {
            object instance = new object();
            ICollection<KeyValuePair<string, object>> collection = new PropertyDictionary(instance);
            collection.Remove(new KeyValuePair<string, object>("Name", "Whatever"));
        }

        #endregion

        #region TryGetValue

        /// <summary>
        /// If we try to get the value for a property that doesn't exist, false should returned and object set to null.
        /// </summary>
        [TestMethod]
        public void TestTryGetValue_NoSuchProperty_ReturnsFalse()
        {
            var instance = new { };
            PropertyDictionary dictionary = new PropertyDictionary(instance);
            object value;
            bool result = dictionary.TryGetValue("Name", out value);
            Assert.IsFalse(result, "The property should not have been found.");
            Assert.IsNull(value, "The value should have been null.");
        }

        /// <summary>
        /// If we try to get the value for a property that doesn't exist, false should returned and object set to null.
        /// </summary>
        [TestMethod]
        public void TestTryGetValue_PropertyExists_ReturnsTrue()
        {
            var instance = new
            {
                Name = "Test"
            };
            PropertyDictionary dictionary = new PropertyDictionary(instance);
            object value;
            bool result = dictionary.TryGetValue("Name", out value);
            Assert.IsTrue(result, "The property should have been found.");
            Assert.AreEqual(instance.Name, value, "The value should have equaled the wrapped property value.");
        }

        #endregion

        #region Values

        /// <summary>
        /// We should be able to get the value of all of the properties.
        /// </summary>
        [TestMethod]
        public void TestValues_GetsValues()
        {
            var instance = new
            {
                Name = "Bob",
                Age = 23
            };
            PropertyDictionary dictionary = new PropertyDictionary(instance);
            ICollection<object> values = dictionary.Values;
            Assert.AreEqual(2, values.Count, "The wrong number of values were returned.");
            Assert.IsTrue(values.Contains("Bob"), "The value for Name was not found.");
            Assert.IsTrue(values.Contains(23), "The value for Age was not found.");
        }

        #endregion

        #region Indexer

        /// <summary>
        /// If we try to retrieve the value for a property that does not exist, an exception
        /// should be thrown.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(KeyNotFoundException))]
        public void TestIndexer_Getter_NoSuchProperty_ThrowsException()
        {
            object instance = new object();
            PropertyDictionary dictionary = new PropertyDictionary(instance);
            object value = dictionary["Name"];
        }

        /// <summary>
        /// If we try to get a value for a property that exists, the value should
        /// be returned.
        /// </summary>
        [TestMethod]
        public void TestIndexer_Getter_PropertyExists_ReturnsValue()
        {
            var instance = new
            {
                Name = "Bob"
            };
            PropertyDictionary dictionary = new PropertyDictionary(instance);
            object value = dictionary["Name"];
            Assert.AreSame(instance.Name, value, "The wrong value was returned.");
        }

        /// <summary>
        /// If we try to set the value for a property, an exception should be thrown.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(NotSupportedException))]
        public void TestIndexer_Setter_ThrowsException()
        {
            PropertyDictionary dictionary = new PropertyDictionary(new { Name = 123 });
            dictionary["Name"] = 123;
        }

        #endregion

        #region Clear

        /// <summary>
        /// Since the dictionary is just a wrapper, Clear will simply throw an exception.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(NotSupportedException))]
        public void TestClear_ThrowsException()
        {
            object instance = new object();
            ICollection<KeyValuePair<string, object>> dictionary = new PropertyDictionary(instance);
            dictionary.Clear();
        }

        #endregion

        #region Contains

        /// <summary>
        /// Contains should find the key/value pair if both the key and value are equal.
        /// </summary>
        [TestMethod]
        public void TestContains_Explicit_PairExists_ReturnsTrue()
        {
            var person = new
            {
                Name = "Bob"
            };
            ICollection<KeyValuePair<string, object>> collection = new PropertyDictionary(person);
            bool contains = collection.Contains(new KeyValuePair<string, object>("Name", "Bob"));
            Assert.IsTrue(contains, "Did not find the pair.");
        }

        /// <summary>
        /// Contains should not find the key/value pair if the keys are not equal.
        /// </summary>
        [TestMethod]
        public void TestContains_Explicit_KeyDoesNotMatch_ReturnsFalse()
        {
            var person = new
            {
                Name = "Bob"
            };
            ICollection<KeyValuePair<string, object>> collection = new PropertyDictionary(person);
            bool contains = collection.Contains(new KeyValuePair<string, object>("Age", "Bob"));
            Assert.IsFalse(contains, "The pair should not have been found.");
        }

        /// <summary>
        /// Contains should not find the key/value pair if the values are not equal.
        /// </summary>
        [TestMethod]
        public void TestContains_Explicit_ValueDoesNotMatch_ReturnsFalse()
        {
            var person = new
            {
                Name = "Bob"
            };
            ICollection<KeyValuePair<string, object>> collection = new PropertyDictionary(person);
            bool contains = collection.Contains(new KeyValuePair<string, object>("Name", "Sally"));
            Assert.IsFalse(contains, "The pair should not have been found.");
        }

        #endregion

        #region CopyTo

        /// <summary>
        /// CopyTo should copy the key/value pairs to an array, assuming there is enough room.
        /// </summary>
        [TestMethod]
        public void TestCopyTo_Explicit()
        {
            var instance = new
            {
                Name = "Bob",
                Age = 23
            };
            ICollection<KeyValuePair<string, object>> collection = new PropertyDictionary(instance);

            KeyValuePair<string, object>[] array = new KeyValuePair<string, object>[collection.Count];
            int arrayIndex = 0;
            collection.CopyTo(array, arrayIndex);

            Assert.IsTrue(array.Contains(new KeyValuePair<string, object>("Name", "Bob")), "The name property was not found.");
            Assert.IsTrue(array.Contains(new KeyValuePair<string, object>("Age", 23)), "The age property was not found.");
        }

        #endregion

        #region GetEnumerator

        /// <summary>
        /// All the items should be enumerated in the dictionary.
        /// </summary>
        [TestMethod]
        public void TestGetEnumerator_EnumeratesAllItems()
        {
            var instance = new
            {
                Name = "Bob",
                Age = 23
            };
            IEnumerable<KeyValuePair<string, object>> dictionary = new PropertyDictionary(instance);

            Assert.IsTrue(enumerate(dictionary).Contains(new KeyValuePair<string, object>("Name", "Bob")), "The first pair was not present.");
            Assert.IsTrue(enumerate(dictionary).Contains(new KeyValuePair<string, object>("Age", 23)), "The second pair was not present.");
        }

        private static IEnumerable<T> enumerate<T>(IEnumerable<T> enumerable)
        {
            List<T> items = new List<T>();
            foreach (T item in enumerable)
            {
                items.Add(item);
            }
            return items;
        }

        /// <summary>
        /// All the items should be enumerated in the dictionary.
        /// </summary>
        [TestMethod]
        public void TestGetEnumerator_Explicit_EnumeratesAllItems()
        {
            var instance = new
            {
                Name = "Bob",
                Age = 23
            };
            IEnumerable dictionary = new PropertyDictionary(instance);

            Assert.IsTrue(enumerate(dictionary).Cast<KeyValuePair<string, object>>().Contains(new KeyValuePair<string, object>("Name", "Bob")), "The first pair was not present.");
            Assert.IsTrue(enumerate(dictionary).Cast<KeyValuePair<string, object>>().Contains(new KeyValuePair<string, object>("Age", 23)), "The second pair was not present.");
        }

        private static IEnumerable enumerate(IEnumerable enumerable)
        {
            ArrayList items = new ArrayList();
            foreach (object item in enumerable)
            {
                items.Add(item);
            }
            return items;
        }

        #endregion
    }
}
