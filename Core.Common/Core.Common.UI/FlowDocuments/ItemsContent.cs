﻿using System;
using System.Collections;
using System.Windows;
using System.Windows.Documents;

namespace Core.Common.UI.FlowDocuments
{
    public class ItemsContent : Section
    {
        private static readonly DependencyProperty ItemsSourceProperty = DependencyProperty.Register("ItemsSource",
            typeof(IEnumerable), typeof(ItemsContent), new PropertyMetadata(OnItemsSourceChanged));

        private static readonly DependencyProperty ItemTemplateProperty = DependencyProperty.Register("ItemTemplate",
            typeof(DataTemplate), typeof(ItemsContent), new PropertyMetadata(OnItemTemplateChanged));

        private static readonly DependencyProperty ItemsPanelProperty = DependencyProperty.Register("ItemsPanel",
            typeof(DataTemplate), typeof(ItemsContent), new PropertyMetadata(OnItemsPanelChanged));

        public ItemsContent()
        {
            Helpers.FixupDataContext(this);
            Loaded += ItemsContent_Loaded;
        }

        public IEnumerable ItemsSource
        {
            get => (IEnumerable) GetValue(ItemsSourceProperty);
            set => SetValue(ItemsSourceProperty, value);
        }

        public DataTemplate ItemTemplate
        {
            get => (DataTemplate) GetValue(ItemTemplateProperty);
            set => SetValue(ItemTemplateProperty, value);
        }

        public DataTemplate ItemsPanel
        {
            get => (DataTemplate) GetValue(ItemsPanelProperty);
            set => SetValue(ItemsPanelProperty, value);
        }

        private void ItemsContent_Loaded(object sender, RoutedEventArgs e)
        {
            GenerateContent(ItemsPanel, ItemTemplate, ItemsSource);
        }

        private void GenerateContent(DataTemplate itemsPanel, DataTemplate itemTemplate, IEnumerable itemsSource)
        {
            Blocks.Clear();
            if (itemTemplate != null && itemsSource != null)
            {
                FrameworkContentElement panel = null;

                foreach (var data in itemsSource)
                {
                    if (panel == null)
                        if (itemsPanel == null)
                        {
                            panel = this;
                        }
                        else
                        {
                            var p = Helpers.LoadDataTemplate(itemsPanel);
                            if (!(p is Block))
                                throw new Exception("ItemsPanel must be a block element");
                            Blocks.Add((Block) p);
                            panel = Attached.GetItemsHost(p);
                            if (panel == null)
                                throw new Exception(
                                    "ItemsHost not found. Did you forget to specify Attached.IsItemsHost?");
                        }

                    var element = Helpers.LoadDataTemplate(itemTemplate);
                    element.DataContext = data;
                    Helpers.UnFixupDataContext(element);
                    if (panel is Section)
                        ((Section) panel).Blocks.Add(Helpers.ConvertToBlock(data, element));
                    else if (panel is TableRowGroup)
                        ((TableRowGroup) panel).Rows.Add((TableRow) element);
                    else
                        throw new Exception(
                            $"Don't know how to add an instance of {element.GetType()} to an instance of {panel.GetType()}");
                }
            }
        }

        private void GenerateContent()
        {
            GenerateContent(ItemsPanel, ItemTemplate, ItemsSource);
        }

        private void OnItemsSourceChanged(IEnumerable newValue)
        {
            if (IsLoaded)
                GenerateContent(ItemsPanel, ItemTemplate, newValue);
        }

        private void OnItemTemplateChanged(DataTemplate newValue)
        {
            if (IsLoaded)
                GenerateContent(ItemsPanel, newValue, ItemsSource);
        }

        private void OnItemsPanelChanged(DataTemplate newValue)
        {
            if (IsLoaded)
                GenerateContent(newValue, ItemTemplate, ItemsSource);
        }

        private static void OnItemsSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((ItemsContent) d).OnItemsSourceChanged((IEnumerable) e.NewValue);
        }

        private static void OnItemTemplateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((ItemsContent) d).OnItemTemplateChanged((DataTemplate) e.NewValue);
        }

        private static void OnItemsPanelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((ItemsContent) d).OnItemsPanelChanged((DataTemplate) e.NewValue);
        }
    }
}