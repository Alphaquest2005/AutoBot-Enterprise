﻿using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;

namespace Core.Common.UI.FlowDocuments
{
    internal static class Helpers
    {
        /// <summary>
        ///     If you use a bindable flow document element more than once, you may encounter a "Collection was modified"
        ///     exception.
        ///     The error occurs when the binding is updated because of a change to an inherited dependency property. The most
        ///     common scenario
        ///     is when the inherited DataContext changes. It appears that an inherited properly like DataContext is propagated to
        ///     its descendants.
        ///     When the enumeration of descendants gets to a BindableXXX, the dependency properties of that element change
        ///     according to the new
        ///     DataContext, which change the (non-dependency) properties. However, for some reason, changing the flow content
        ///     invalidates the
        ///     enumeration and raises an exception.
        ///     To work around this, one can either DataContext="{Binding DataContext, RelativeSource={RelativeSource
        ///     AncestorType=FrameworkElement}}"
        ///     in code. This is clumsy, so every derived type calls this function instead (which performs the same thing).
        ///     See http://code.logos.com/blog/2008/01/data_binding_in_a_flowdocument.html
        /// </summary>
        /// <param name="element"></param>
        public static void FixupDataContext(FrameworkContentElement element)
        {
            var b = new Binding(FrameworkContentElement.DataContextProperty.Name)
            {
                // another approach (if this one has problems) is to bind to an ancestor by ElementName
                RelativeSource = new RelativeSource(RelativeSourceMode.FindAncestor, typeof(FrameworkElement), 1)
            };
            element.SetBinding(FrameworkContentElement.DataContextProperty, b);
        }


        private static bool InternalUnFixupDataContext(DependencyObject dp)
        {
            // only consider those elements for which we've called FixupDataContext(): they all belong to this namespace
            if (dp is FrameworkContentElement && dp.GetType().Namespace == typeof(Helpers).Namespace)
            {
                var binding = BindingOperations.GetBinding(dp, FrameworkContentElement.DataContextProperty);
                if (binding != null
                    && binding.Path != null && binding.Path.Path == FrameworkContentElement.DataContextProperty.Name
                    && binding.RelativeSource != null &&
                    binding.RelativeSource.Mode == RelativeSourceMode.FindAncestor &&
                    binding.RelativeSource.AncestorType == typeof(FrameworkElement) &&
                    binding.RelativeSource.AncestorLevel == 1)
                {
                    BindingOperations.ClearBinding(dp, FrameworkContentElement.DataContextProperty);
                    return true;
                }
            }

            // as soon as we have disconnected a binding, return. Don't continue the enumeration, since the collection may have changed
            foreach (var child in LogicalTreeHelper.GetChildren(dp))
                if (child is DependencyObject dependencyObject)
                    if (InternalUnFixupDataContext(dependencyObject))
                        return true;
            return false;
        }


        public static void UnFixupDataContext(DependencyObject dp)
        {
            while (InternalUnFixupDataContext(dp))
                ;
        }


        /// <summary>
        ///     Convert "data" to a flow document block object. If data is already a block, the return value is data recast.
        /// </summary>
        /// <param name="dataContext">only used when bindable content needs to be created</param>
        /// <param name="data"></param>
        /// <returns></returns>
        public static Block ConvertToBlock(object dataContext, object data)
        {
            if (data is Block block) return block;

            if (data is Inline inline) return new Paragraph(inline);

            if (data is BindingBase @base)
            {
                var run = new Run();
                if (dataContext is BindingBase context)
                    run.SetBinding(FrameworkContentElement.DataContextProperty, context);
                else
                    run.DataContext = dataContext;
                run.SetBinding(Run.TextProperty, @base);
                return new Paragraph(run);
            }
            else
            {
                var run = new Run
                {
                    Text = data == null ? string.Empty : data.ToString()
                };
                return new Paragraph(run);
            }
        }


        public static FrameworkContentElement LoadDataTemplate(DataTemplate dataTemplate)
        {
            object content = dataTemplate.LoadContent();
            if (content is Fragment fragment) return fragment.Content;

            if (content is TextBlock block)
            {
                var inlines = block.Inlines;
                if (inlines.Count == 1) return inlines.FirstInline;

                var paragraph = new Paragraph();
                // we can't use an enumerator, since adding an inline removes it from its collection
                while (inlines.FirstInline != null)
                    paragraph.Inlines.Add(inlines.FirstInline);
                return paragraph;
            }

            throw new Exception("Data template needs to contain a <Fragment> or <TextBlock>");
        }
    }
}