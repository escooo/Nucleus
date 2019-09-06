﻿// Copyright (c) 2016 Paul Jeffries
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using Nucleus.Base;
using Nucleus.Extensions;
using Nucleus.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Nucleus.Extensions
{
    /// <summary>
    /// Extension methods on types and collections of types
    /// </summary>
    public static class TypeExtensions
    {
        /// <summary>
        /// The number of levels of inheritance between this type and a type that
        /// is somewhere in its inheritance chain.
        /// </summary>
        /// <param name="type">This type</param>
        /// <param name="ancestorType">A type which is an ancestor of this one</param>
        /// <param name="interfaceProxy">The value to be returned in the case that the 
        /// specified type is not a direct ancestor but is still assignable</param>
        /// <returns>If the specified type is an ancestor of this one, the number of
        /// inheritance levels between the two types.  If the specified type is this 
        /// type, 0.  If the specified type cannot be found in the inheritance chain,
        /// -1.</returns>
        public static int InheritanceLevelsTo(this Type type, Type ancestorType, int interfaceProxy = -1)
        {
            int count = 0;
            while (type != null && type != ancestorType)
            {
                count++;
                type = type.BaseType;
            }
            if (type == ancestorType) return count;
            else if (interfaceProxy >= 0 && ancestorType.IsAssignableFrom(type))
                return interfaceProxy;
            else return -1;
        }

        /// <summary>
        /// Find the type in this set of types which is the least number of
        /// inheritance levels above the specified type.
        /// </summary>
        /// <param name="forType">The type to seach for</param>
        /// <param name="inTypes">The collection of types to look within</param>
        /// <param name="includeSelf">If true (default) the type itself may be returned if found.
        /// Otherwise it will be excluded from the search and only its ancestors may be returned.</param>
        /// <param name="interfaceProxy">The number of inheritance levels to be assumed in the case of
        /// compatible interfaces</param>
        /// <returns>The type in this collection that is closest in the inheritance
        /// hierarchy to the specified type.  Or, null if the type does not have an
        /// ancestor in the collection.</returns>
        public static Type ClosestAncestor(this IEnumerable<Type> inTypes, Type forType, bool includeSelf = true, int interfaceProxy = 100000)
        {
            int minDist = -1;
            Type closest = null;
            int distLimit = 0;
            if (!includeSelf) distLimit = 1;
            foreach (Type ancestorType in inTypes)
            {
                int dist = forType.InheritanceLevelsTo(ancestorType, interfaceProxy);
                if (dist >= distLimit && (minDist < 0 || dist < minDist))
                {
                    minDist = dist;
                    closest = ancestorType;
                }
            }
            return closest;
        }

        /// <summary>
        /// Find the type in this set of types which is the least number of
        /// inheritance levels below the specified type.
        /// </summary>
        /// <param name="forType">The type to seach for</param>
        /// <param name="inTypes">The collection of types to look within</param>
        /// <returns>The type in this collection that is closest in the inheritance
        /// hierarchy to the specified type.  Or, null if the type does not have a
        /// descendent in the collection.</returns>
        public static Type ClosestDescendent(this IEnumerable<Type> inTypes, Type forType)
        {
            int minDist = -1;
            Type closest = null;
            foreach (Type descendentType in inTypes)
            {
                int dist = descendentType.InheritanceLevelsTo(forType);
                if (dist >= 0 && (minDist < 0 || dist < minDist))
                {
                    minDist = dist;
                    closest = descendentType;
                }
            }
            return closest;
        }

        /// <summary>
        /// Is this a collection type? i.e. does it implement ICollection?
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsCollection(this Type type)
        {
            return typeof(ICollection).IsAssignableFrom(type)
                   || typeof(ICollection<>).IsAssignableFrom(type);
        }

        /// <summary>
        /// Is this an enumerable type?  i.e. does it implement IEnumerable?
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsEnumerable(this Type type)
        {
            return typeof(IEnumerable).IsAssignableFrom(type)
                   || typeof(IEnumerable<>).IsAssignableFrom(type);
        }

        /// <summary>
        /// Is this a List type?  i.e. does it implement IList?
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsList(this Type type)
        {
            return typeof(IList).IsAssignableFrom(type)
                   || typeof(IList<>).IsAssignableFrom(type);
        }

        /// <summary>
        /// Is this a dictionary type?
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsDictionary(this Type type)
        {
            return typeof(IDictionary).IsAssignableFrom(type)
                || typeof(IDictionary<,>).IsAssignableFrom(type);
        }

        /// <summary>
        /// Is this the standard CLR Dictionary type or a subclass of it?
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsStandardDictionary(this Type type)
        {
            return (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Dictionary<,>));
        }

        /// <summary>
        /// Extract all members from this type that have been annotated with an AutoUIAttribute,
        /// sorted by their order.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static IList<MemberInfo> GetAutoUIMembers(this Type type)
        {
            SortedList<double, MemberInfo> result = new SortedList<double, MemberInfo>();
            MemberInfo[] mInfos = type.GetMembers();
            foreach (MemberInfo mInfo in mInfos)
            {
                object[] attributes = mInfo.GetCustomAttributes(typeof(AutoUIAttribute), true);
                if (attributes.Count() > 0)
                {
                    AutoUIAttribute aInput = (AutoUIAttribute)attributes[0];
                    double keyValue = aInput.Order;
                    while (result.ContainsKey(keyValue)) keyValue = keyValue.NextValidValue();
                    result.Add(keyValue, mInfo);
                }
            }
            return result.Values.ToList();
        }

        /// <summary>
        /// Extract all properties from this type that have been annotated with an AutoUIAttribute,
        /// sorted by their order.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static IList<PropertyInfo> GetAutoUIProperties(this Type type)
        {
            SortedList<double, PropertyInfo> result = new SortedList<double, PropertyInfo>();
            PropertyInfo[] pInfos = type.GetProperties();
            foreach (PropertyInfo pInfo in pInfos)
            {
                object[] attributes = pInfo.GetCustomAttributes(typeof(AutoUIAttribute), true);
                if (attributes.Count() > 0)
                {
                    AutoUIAttribute aInput = (AutoUIAttribute)attributes[0];
                    double keyValue = aInput.Order;
                    while (result.ContainsKey(keyValue)) keyValue = keyValue.NextValidValue();
                    result.Add(keyValue, pInfo);
                }
            }
            return result.Values.ToList();
        }

        /// <summary>
        /// Extract all properties from this set of types that have been annotated with an
        /// AutoUIAttribute, sorted by their order.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static IList<PropertyInfo> GetAutoUIProperties(this IEnumerable<Type> types, bool allowDuplicateNames = false)
        {
            HashSet<string> names = new HashSet<string>();
            SortedList<double, PropertyInfo> result = new SortedList<double, PropertyInfo>();
            foreach (Type type in types)
            {
                PropertyInfo[] pInfos = type.GetProperties();
                foreach (PropertyInfo pInfo in pInfos)
                {
                    object[] attributes = pInfo.GetCustomAttributes(typeof(AutoUIAttribute), true);
                    if (attributes.Count() > 0 && (allowDuplicateNames || !names.Contains(pInfo.Name)))
                    {
                        AutoUIAttribute aInput = (AutoUIAttribute)attributes[0];
                        double keyValue = aInput.Order;
                        while (result.ContainsKey(keyValue)) keyValue = keyValue.NextValidValue();
                        result.Add(keyValue, pInfo);
                        names.Add(pInfo.Name);
                    }
                }
            }
            return result.Values.ToList();
        }

        /// <summary>
        /// Extract all methods from this type that have been annotated with an AutoUIAttribute,
        /// sorted by their order.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static IList<MethodInfo> GetAutoUIMethods(this Type type)
        {
            SortedList<double, MethodInfo> result = new SortedList<double, MethodInfo>();
            MethodInfo[] mInfos = type.GetMethods();
            foreach (MethodInfo mInfo in mInfos)
            {
                object[] attributes = mInfo.GetCustomAttributes(typeof(AutoUIAttribute), true);
                if (attributes.Count() > 0)
                {
                    AutoUIAttribute aInput = (AutoUIAttribute)attributes[0];
                    double keyValue = aInput.Order;
                    while (result.ContainsKey(keyValue)) keyValue = keyValue.NextValidValue();
                    result.Add(keyValue, mInfo);
                }
            }
            return result.Values.ToList();
        }

        /// <summary>
        /// Get the method (if any) on this object tagged with the OnDeserializedAttribute
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static MethodInfo GetOnDeserializedMethod(this Type type)
        {
#if !JS
            MethodInfo[] mInfos = type.GetMethods();
            foreach (MethodInfo mInfo in mInfos)
            {
                if (mInfo.GetCustomAttribute<OnDeserializedAttribute>() != null)
                {
                    return mInfo;
                }
            }
#endif
            return null;
        }

        /// <summary>
        /// Get a list of all the non-abstract types that derive from this type
        /// </summary>
        /// <param name="type"></param>
        /// <param name="allAssemblies">If true, all loaded assembles will be checked, else only the assembly the 
        /// base type is defined in.</param>
        /// <returns></returns>
        public static IList<Type> GetSubTypes(this Type type, bool allAssemblies = false, bool includeSelf = false)
        {
            IList<Type> result = new List<Type>();
            if (includeSelf) result.Add(type);
            if (allAssemblies)
            {
                foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
                {
                    try
                    {
                        foreach (Type subType in assembly.GetTypes())
                        {
                            if (subType.IsSubclassOf(type) && !subType.IsAbstract) result.Add(subType);
                        }
                    }
                    catch
                    {

                    }
                }
            }
            else
            {
                Assembly assembly = type.Assembly;
                foreach (Type subType in assembly.GetTypes())
                {
                    if (subType.IsSubclassOf(type) && !subType.IsAbstract) result.Add(subType);
                }
            }
            return result;
        }

        /// <summary>
        /// Get all fields of this type, including private ones inherited from base classes
        /// </summary>
        /// <param name="type"></param>
        /// <param name="outFields">The collection of field infos to be populated</param>
        /// <param name="flags">A bitmask composed of one or more BindingFlags which specify 
        /// how the search is conduted</param>
        public static void GetAllFields(this Type type, ICollection<FieldInfo> outFields, BindingFlags flags, bool ignoreNonSerialised = false, Func<FieldInfo, bool> filter = null)
        {
            foreach (var field in type.GetFields(flags))
            {
                // Ignore inherited fields.
                if (field.DeclaringType == type && //Necessary?
#if !JS
                    (!ignoreNonSerialised || !field.IsNotSerialized) &&
#endif
                    (filter == null || filter(field)))
                {
                    outFields.Add(field);
                }
            }

            var baseType = type.BaseType;
            if (baseType != null)
                baseType.GetAllFields(outFields, flags, ignoreNonSerialised, filter);
        }

        /// <summary>
        /// Get all fields of this type, including private ones inherited from base classes
        /// </summary>
        /// <param name="type"></param>
        /// <param name="flags">A bitmask composed of one or more BindingFlags which specify 
        /// how the search is conduted</param>
        /// <param name="ignoreNonSerialised"></param>
        public static IList<FieldInfo> GetAllFields(this Type type, BindingFlags flags, bool ignoreNonSerialised = false, Func<FieldInfo, bool> filter = null)
        {
            var result = new List<FieldInfo>();
            type.GetAllFields(result, flags, ignoreNonSerialised, filter);
            return result;
        }

        /// <summary>
        /// Get all fields of this type, including private ones inherited from base classes
        /// </summary>
        /// <param name="type"></param>
        public static IList<FieldInfo> GetAllFields(this Type type, bool ignoreNonSerialised = false, Func<FieldInfo, bool> filter = null)
        {
            BindingFlags flags = BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy;
            return type.GetAllFields(flags, ignoreNonSerialised, filter);
        }

        /// <summary>
        /// Searches for the specified field recursively.  If it cannot be found within this type,
        /// the base class hierarchy will be searched also.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="name">The name of the field to find</param>
        /// <param name="flags">>A bitmask composed of one or more BindingFlags which specify 
        /// how the search is conduted</param>
        /// <returns>The FieldInfo if found, else null</returns>
        public static FieldInfo GetBaseField(this Type type, string name, BindingFlags flags)
        {
            FieldInfo result = type.GetField(name, flags);
            if (result == null)
            {
                var baseType = type.BaseType;
                if (baseType != null)
                    result = baseType.GetBaseField(name, flags);
            }
            return result;
        }

        /// <summary>
        /// Searches for the specified public or private field recursively.  
        /// If it cannot be found within this type, the base class hierarchy 
        /// will be searched also.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="name">The name of the field to find</param>
        /// <param name="flags">>A bitmask composed of one or more BindingFlags which specify 
        /// how the search is conduted</param>
        /// <returns>The FieldInfo if found, else null</returns>
        public static FieldInfo GetBaseField(this Type type, string name)
        {
            BindingFlags flags = BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy;
            return GetBaseField(type, name, flags);
        }

        /// <summary>
        /// Does this type posess a parameterless constructor
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool HasParameterlessConstructor(this Type type)
        {
#if !JS
            return type.GetConstructor(BindingFlags.Instance | BindingFlags.Public, //| BindingFlags.NonPublic,
                                        null, Type.EmptyTypes, null) != null;
#else
            return type.GetConstructor(new Type[0]) != null;
#endif
        }


#if !JS

        /// <summary>
        /// Get a collection of all types which this type relies on for its definition
        /// </summary>
        /// <param name="type"></param>
        /// <param name="ignoreNonSerialised"></param>
        /// <returns></returns>
        public static ICollection<Type> GetDependencies(this Type type, bool ignoreNonSerialised = false)
        {
            HashSet<Type> result = new HashSet<Type>();
            GetDependencies(type, result, ignoreNonSerialised);
            return result;
        }

        /// <summary>
        /// Get a collection of all types which this type relies on for its definition
        /// </summary>
        /// <param name="type"></param>
        /// <param name="ignoreNonSerialised"></param>
        /// <returns></returns>
        public static void GetDependencies(this Type type, ICollection<Type> output, bool ignoreNonSerialised = false)
        {
            if (!output.Contains(type))
            {
                output.Add(type);
                if (type.BaseType != null) type.BaseType.GetDependencies(output, ignoreNonSerialised);
                foreach (FieldInfo fI in type.GetAllFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
                {
                    if (!ignoreNonSerialised || fI.GetCustomAttribute(typeof(NonSerializedAttribute)) == null)
                    if (!ignoreNonSerialised || fI.GetAttribute<NonSerializedAttribute>() == null)
                    fI.FieldType.GetDependencies(output, ignoreNonSerialised);
                }
            }
        }
#endif

        /// <summary>
        /// Get the type of the items stored within this collection type
        /// </summary>
        /// <param name="collectionType"></param>
        /// <returns></returns>
        public static Type GetItemType(this Type collectionType)
        {
            Type iEnum = FindIEnumerable(collectionType);
            if (iEnum == null) return collectionType;
            else return iEnum.GetGenericArguments()[0];
        }

        /// <summary>
        /// Get the generic IEnumerable type of the specified collection type
        /// </summary>
        /// <param name="collectionType"></param>
        /// <returns></returns>
        private static Type FindIEnumerable(Type collectionType)
        {
            if (collectionType != null)
            {
                if (collectionType.IsGenericType)
                {
                    foreach (Type arg in collectionType.GetGenericArguments())
                    {
#if !JS
                        Type iEnum = typeof(IEnumerable<>).MakeGenericType(arg);
#else
                        Type iEnum = typeof(IEnumerable<>).MakeGenericType(new Type[] { arg });
#endif
                        if (iEnum.IsAssignableFrom(collectionType)) return iEnum;
                    }
                }
                Type[] interfaces = collectionType.GetInterfaces();
                if (interfaces != null && interfaces.Length > 0)
                {
                    foreach (Type iface in interfaces)
                    {
                        Type iEnum = FindIEnumerable(iface);
                        if (iEnum != null) return iEnum;
                    }
                }
                if (collectionType.BaseType != null && collectionType.BaseType != typeof(object))
                    return FindIEnumerable(collectionType.BaseType);
            }
            return null;
        }

        /// <summary>
        /// Retrieves a custom attribute applied to this type
        /// </summary>
        /// <typeparam name="TAttribute"></typeparam>
        /// <param name="type"></param>
        /// <returns></returns>
        public static TAttribute GetCustomAttribute<TAttribute>(this Type type)
            where TAttribute : Attribute
        {
#if !JS
            return Attribute.GetCustomAttribute(type, typeof(TAttribute)) as TAttribute;
#else
            return null; //TODO!
#endif
        }

        /// Create an instance of this type.
        /// Will use Activator.CreateInstance if a parameterless constructor exists, else
        /// will fall back to FormatterServices.GetUninitializedObject
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static object Instantiate(this Type type)
        {
#if !JS
            if (type.HasParameterlessConstructor()) return Activator.CreateInstance(type);
            else return FormatterServices.GetUninitializedObject(type);
#else
            return Activator.CreateInstance(type);
#endif
        }

        /// <summary>
        /// Get the default copying behaviour for this type during duplication,
        /// as (possibly) specified by the CopyAttribute on the type itself.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static CopyBehaviour DefaultCopyBehaviour(this Type type)
        {
            var cAtt = type.GetCustomAttribute<CopyAttribute>();
            if (cAtt != null) return cAtt.Behaviour;
            else return CopyBehaviour.COPY;
        }

        /// <summary>
        /// Get the default equivalent type into which the specified type
        /// will be converted via the 'Convert' method on this converter type.
        /// </summary>
        /// <param name="converter"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static Type DefaultConversionType(this Type converter, Type type)
        {
            MethodInfo mInfo = converter.GetMethod("Convert", new Type[] { type });
            if (mInfo != null) return mInfo.ReturnType;
            else return null;
        }

        /// <summary>
        /// Get the name of this type including any generic parameters
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public static string NameWithGenericParameters(this Type t)
        {
            if (!t.IsGenericType)
                return t.Name;

            StringBuilder sb = new StringBuilder();
            sb.Append(t.Name.Substring(0, t.Name.IndexOf('`')));
            sb.Append('<');
            bool appendComma = false;
            foreach (Type arg in t.GetGenericArguments())
            {
                if (appendComma) sb.Append(',');
                sb.Append(NameWithGenericParameters(arg));
                appendComma = true;
            }
            sb.Append('>');
            return sb.ToString();
        }
    }
}
