﻿using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects.DataClasses;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace InsightPresentationLayer
{
       public static partial class Common
        {
            /// <summary>
            /// Finds a Child of a given item in the visual tree. 
            /// </summary>
            /// <param name="parent">A direct parent of the queried item.</param>
            /// <typeparam name="T">The type of the queried item.</typeparam>
            /// <param name="childName">x:Name or Name of child. </param>
            /// <returns>The first parent item that matches the submitted type parameter. 
            /// If not matching item can be found, 
            /// a null parent is being returned.</returns>
            /// 

            static public T GetVisualChild<T>(Visual parent) where T : Visual
            {
                T child = default(T);
                int numVisuals = VisualTreeHelper.GetChildrenCount(parent);
                for (int i = 0; i < numVisuals; i++)
                {
                    Visual v = (Visual)VisualTreeHelper.GetChild(parent, i);
                    child = v as T;
                    if (child == null) child = GetVisualChild<T>(v);
                    if (child != null) break;
                }
                return child;
            }


            public static T FindChild<T>(DependencyObject parent, string childName)
               where T : DependencyObject
            {
                // Confirm parent and childName are valid. 
                if (parent == null) return null;

                T foundChild = null;

                int childrenCount = VisualTreeHelper.GetChildrenCount(parent);
                for (int i = 0; i < childrenCount; i++)
                {
                    var child = VisualTreeHelper.GetChild(parent, i);
                    // If the child is not of the request child type child
                    T childType = child as T;
                    if (childType == null)
                    {
                        // recursively drill down the tree
                        foundChild = FindChild<T>(child, childName);

                        // If the child is found, break so we do not overwrite the found child. 
                        if (foundChild != null) break;
                    }
                    else if (!string.IsNullOrEmpty(childName))
                    {
                        var frameworkElement = child as FrameworkElement;
                        // If the child's name is set for search
                        if (frameworkElement != null && frameworkElement.Name == childName)
                        {
                            // if the child's name is of the request name
                            foundChild = (T)child;
                            break;
                        }
                    }
                    else
                    {
                        // child element found.
                        foundChild = (T)child;
                        break;
                    }
                }

                return foundChild;
            }

            public static T Clone<T>(T source)
            {
                if (!typeof(T).IsSerializable)
                {
                    throw new ArgumentException("The type must be serializable.", "source");
                }

                // Don't serialize a null object, simply return the default for that object
                if (Object.ReferenceEquals(source, null))
                {
                    return default(T);
                }

                IFormatter formatter = new BinaryFormatter();
                Stream stream = new MemoryStream();
                using (stream)
                {
                    formatter.Serialize(stream, source);
                    stream.Seek(0, SeekOrigin.Begin);
                    return (T)formatter.Deserialize(stream);
                }
            }


      

        }
    
}
