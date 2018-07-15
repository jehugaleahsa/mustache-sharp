using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Mustache.Test
{
    [TestClass]
    public class UpcastDictionaryTester
    {
        [TestMethod]
        public void ShouldReturnNullForNull()
        {
            IDictionary<string, object> result = UpcastDictionary.Create(null);
            Assert.IsNull(result, "Null should be returned for null.");
        }

        [TestMethod]
        public void ShouldReturnArgumentIfIDictionary_string_object()
        {
            object source = new Dictionary<string, object>();
            IDictionary<string, object> result = UpcastDictionary.Create(source);
            Assert.AreSame(source, result, "The up-cast wrapper should not be applied if already a IDictionary<string, object>.");
        }

        [TestMethod]
        public void ShouldReturnNullIfNotGenericType()
        {
            object source = String.Empty;
            IDictionary<string, object> result = UpcastDictionary.Create(source);
            Assert.IsNull(result, "Null should be returned for non-generic types.");
        }

        [TestMethod]
        public void ShouldReturnNullIfWrongNumberOfGenericArguments()
        {
            object source = new List<string>();
            IDictionary<string, object> result = UpcastDictionary.Create(source);
            Assert.IsNull(result, "Null should be returned for generic types with the wrong number of type arguments.");
        }

        [TestMethod]
        public void ShouldReturnNullIfFirstGenericTypeArgumentIsNotAString()
        {
            object source = new Dictionary<object, object>();
            IDictionary<string, object> result = UpcastDictionary.Create(source);
            Assert.IsNull(result, "Null should be returned if the first generic type argument is not a string.");
        }

        [TestMethod]
        public void ShouldReturnNullIfNotDictionaryType()
        {
            object source = (Converter<string, object>)(s => (object)s);
            IDictionary<string, object> result = UpcastDictionary.Create(source);
            Assert.IsNull(result, "Null should be returned for non-dictionary types.");
        }

        [TestMethod]
        public void ShouldReturnUpcastWrapperForDictionary_string_TValue()
        {
            object source = new Dictionary<string, string>();
            IDictionary<string, object> result = UpcastDictionary.Create(source);
            Assert.IsInstanceOfType(result, typeof(UpcastDictionary<string>), "The source was not wrapped.");
        }

        [TestMethod]
        public void ShouldFindKeyIfInWrappedDictionary()
        {
            object source = new Dictionary<string, string>() { { "Name", "Bob" } };
            IDictionary<string, object> result = UpcastDictionary.Create(source);
            bool containsKey = result.ContainsKey("Name");
            Assert.IsTrue(containsKey, "The key Name should have been found.");
        }

        [TestMethod]
        public void ShouldNotFindKeyIfNotInWrappedDictionary()
        {
            object source = new Dictionary<string, string>() { { "Name", "Bob" } };
            IDictionary<string, object> result = UpcastDictionary.Create(source);
            bool containsKey = result.ContainsKey("Age");
            Assert.IsFalse(containsKey, "The key Age should not have been found.");
        }

        [TestMethod]
        public void ShouldFindKeysInWrappedDictionary()
        {
            var source = new Dictionary<string, string>() { { "Name", "Bob" }, { "Age", "100" } };
            IDictionary<string, object> result = UpcastDictionary.Create(source);
            ICollection sourceKeys = source.Keys;
            ICollection wrappedKeys = result.Keys.ToArray();
            CollectionAssert.AreEquivalent(sourceKeys, wrappedKeys, "The same keys should have been found in both collections.");
        }

        [TestMethod]
        public void ShouldFindKeyIfInWrappedDictionary_TryGetValue()
        {
            var source = new Dictionary<string, string>() { { "Name", "Bob" } };
            IDictionary<string, object> result = UpcastDictionary.Create(source);
            bool found = result.TryGetValue("Name", out object value);
            Assert.IsTrue(found, "The key should have been found.");
            Assert.AreSame(source["Name"], value, "The value in the underlying dictionary should have been returned.");
        }

        [TestMethod]
        public void ShouldNotFindKeyIfNotInWrappedDictionary_TryGetValue()
        {
            var source = new Dictionary<string, int>() { { "Age", 100 } };
            IDictionary<string, object> result = UpcastDictionary.Create(source);
            bool found = result.TryGetValue("Name", out object value);
            Assert.IsFalse(found, "The key should not have been found.");
            Assert.IsNull(value, "The value should be null even if the actual type is a struct.");
        
        }

        [TestMethod]
        public void ShouldReturnValuesAsObjects()
        {
            var source = new Dictionary<string, int>() { { "Age", 100 }, { "Weight", 500 } };
            IDictionary<string, object> result = UpcastDictionary.Create(source);
            ICollection sourceValues = source.Values;
            ICollection wrappedValues = result.Values.ToArray();
            CollectionAssert.AreEquivalent(sourceValues, wrappedValues, "The underlying values were not returned.");
        }

        [TestMethod]
        public void ShouldFindKeyIfInWrappedDictionary_Indexer()
        {
            var source = new Dictionary<string, string>() { { "Name", "Bob" } };
            IDictionary<string, object> result = UpcastDictionary.Create(source);
            object value = result["Name"];
            Assert.AreSame(source["Name"], value, "The value in the underlying dictionary should have been returned.");
        }

        [TestMethod]
        [ExpectedException(typeof(KeyNotFoundException))]
        public void ShouldNotFindKeyIfNotInWrappedDictionary_Indexer()
        {
            var source = new Dictionary<string, int>() { { "Age", 100 } };
            IDictionary<string, object> result = UpcastDictionary.Create(source);
            object value = result["Name"];
        }

        [TestMethod]
        public void ShouldNotFindPairIfValueWrongType()
        {
            var source = new Dictionary<string, int>() { { "Age", 100 } };
            IDictionary<string, object> result = UpcastDictionary.Create(source);
            bool contains = result.Contains(new KeyValuePair<string, object>("Age", "Blah"));
            Assert.IsFalse(contains, "The pair should not have been found.");
        }

        [TestMethod]
        public void ShouldFindPairInWrappedDictionary()
        {
            var source = new Dictionary<string, int>() { { "Age", 100 } };
            IDictionary<string, object> result = UpcastDictionary.Create(source);
            bool contains = result.Contains(new KeyValuePair<string, object>("Age", 100));
            Assert.IsTrue(contains, "The pair should have been found.");
        }

        [TestMethod]
        public void ShouldCopyPairsToArray()
        {
            var source = new Dictionary<string, int>() { { "Age", 100 }, { "Weight", 45 } };
            IDictionary<string, object> result = UpcastDictionary.Create(source);
            var array = new KeyValuePair<string, object>[2];
            result.CopyTo(array, 0);
            var expected = new KeyValuePair<string, object>[]
            {
                new KeyValuePair<string, object>("Age", 100),
                new KeyValuePair<string, object>("Weight", 45)
            };
            CollectionAssert.AreEqual(expected, array, "The pairs were not copied.");
        }

        [TestMethod]
        public void ShouldGetCount()
        {
            var source = new Dictionary<string, int>() { { "Age", 100 }, { "Weight", 45 } };
            IDictionary<string, object> result = UpcastDictionary.Create(source);
            Assert.AreEqual(source.Count, result.Count, "The source and Upcast dictionary should have the same count.");
        }

        [TestMethod]
        public void ShouldGetEnumerator()
        {
            var source = new Dictionary<string, int>() { { "Age", 100 }, { "Weight", 45 } };
            IDictionary<string, object> result = UpcastDictionary.Create(source);
            IEnumerator<KeyValuePair<string, object>> enumerator = result.GetEnumerator();
            var values = new List<KeyValuePair<string, object>>();
            while (enumerator.MoveNext())
            {
                values.Add(enumerator.Current);
            }
            var expected = new KeyValuePair<string, object>[]
            {
                new KeyValuePair<string, object>("Age", 100),
                new KeyValuePair<string, object>("Weight", 45)
            };
            CollectionAssert.AreEqual(expected, values, "The enumerator did not return the correct pairs.");
        }

        /// <summary>
        /// Newtonsoft's JSON.NET has an object called JObject. This is a concrete class
        /// that inherits from IDictionary&lt;string, JToken&gt;. The UpcastDictionary
        /// should be able to handle this type.
        /// </summary>
        [TestMethod]
        public void ShouldHandleConcreteClassInheritingFromDictionary()
        {
            var dictionary = new ConcreteDictionary() { { "Name", "Bob" } };
            var result = UpcastDictionary.Create(dictionary);
            Assert.AreEqual(dictionary["Name"], result["Name"]);
        }

        public class ConcreteDictionary : Dictionary<string, string>
        {
        }
    }
}
