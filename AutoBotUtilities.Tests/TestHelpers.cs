using System;
using System.Reflection;

namespace AutoBotUtilities.Tests
{
    public static class TestHelpers
    {
        /// <summary>
        /// Invokes a private or internal method on an object using reflection.
        /// </summary>
        /// <typeparam name="T">The expected return type of the method.</typeparam>
        /// <param name="obj">The object instance to invoke the method on.</param>
        /// <param name="methodName">The name of the private/internal method.</param>
        /// <param name="parameters">The parameters to pass to the method.</param>
        /// <returns>The result of the method invocation, cast to type T.</returns>
        public static T InvokePrivateMethod<T>(object obj, string methodName, params object[] parameters)
        {
            if (obj == null) throw new ArgumentNullException(nameof(obj));
            
            Type type = obj.GetType();
            MethodInfo method = null;
            
            // Try to find the method with the exact parameter types first
            if (parameters != null && parameters.Length > 0)
            {
                Type[] parameterTypes = Array.ConvertAll(parameters, p => p?.GetType() ?? typeof(object));
                method = type.GetMethod(methodName, 
                    BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.FlattenHierarchy,
                    null, parameterTypes, null);
            }
            
            // If not found with exact types, try to find by name only
            if (method == null)
            {
                method = type.GetMethod(methodName, 
                    BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.FlattenHierarchy);
            }
            
            if (method == null)
            {
                throw new ArgumentException($"Method '{methodName}' not found on type '{type.FullName}'");
            }
            
            object result = method.Invoke(obj, parameters);
            
            if (result == null && typeof(T).IsValueType)
            {
                return default;
            }
            
            if (result == null)
            {
                return default;
            }
            
            return (T)result;
        }
        
        /// <summary>
        /// Invokes a private or internal static method using reflection.
        /// </summary>
        public static T InvokePrivateStaticMethod<T>(Type type, string methodName, params object[] parameters)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));
            
            MethodInfo method = type.GetMethod(methodName, 
                BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.FlattenHierarchy);
                
            if (method == null)
            {
                throw new ArgumentException($"Static method '{methodName}' not found on type '{type.FullName}'");
            }
            
            object result = method.Invoke(null, parameters);
            
            if (result == null && typeof(T).IsValueType)
            {
                return default;
            }
            
            return (T)result;
        }
        
        /// <summary>
        /// Gets the value of a private or internal field from an object using reflection.
        /// </summary>
        public static T GetPrivateField<T>(object obj, string fieldName)
        {
            if (obj == null) throw new ArgumentNullException(nameof(obj));
            
            Type type = obj.GetType();
            FieldInfo field = type.GetField(fieldName, 
                BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.FlattenHierarchy);
                
            if (field == null)
            {
                throw new ArgumentException($"Field '{fieldName}' not found on type '{type.FullName}'");
            }
            
            object value = field.GetValue(obj);
            return (T)value;
        }
        
        /// <summary>
        /// Sets the value of a private or internal field on an object using reflection.
        /// </summary>
        public static void SetPrivateField(object obj, string fieldName, object value)
        {
            if (obj == null) throw new ArgumentNullException(nameof(obj));
            
            Type type = obj.GetType();
            FieldInfo field = type.GetField(fieldName, 
                BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.FlattenHierarchy);
                
            if (field == null)
            {
                throw new ArgumentException($"Field '{fieldName}' not found on type '{type.FullName}'");
            }
            
            field.SetValue(obj, value);
        }
        
        /// <summary>
        /// Gets the value of a private or internal property from an object using reflection.
        /// </summary>
        public static T GetPrivateProperty<T>(object obj, string propertyName)
        {
            if (obj == null) throw new ArgumentNullException(nameof(obj));
            
            Type type = obj.GetType();
            PropertyInfo property = type.GetProperty(propertyName, 
                BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.FlattenHierarchy);
                
            if (property == null)
            {
                throw new ArgumentException($"Property '{propertyName}' not found on type '{type.FullName}'");
            }
            
            object value = property.GetValue(obj);
            return (T)value;
        }
        
        /// <summary>
        /// Sets the value of a private or internal property on an object using reflection.
        /// </summary>
        public static void SetPrivateProperty(object obj, string propertyName, object value)
        {
            if (obj == null) throw new ArgumentNullException(nameof(obj));
            
            Type type = obj.GetType();
            PropertyInfo property = type.GetProperty(propertyName, 
                BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.FlattenHierarchy);
                
            if (property == null)
            {
                throw new ArgumentException($"Property '{propertyName}' not found on type '{type.FullName}'");
            }
            
            property.SetValue(obj, value);
        }
    }
}