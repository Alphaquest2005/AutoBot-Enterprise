using System.Windows;
using System.Windows.Documents;

namespace Core.Common.UI.FlowDocuments
{
    public class BlockTemplateContent : Section
    {
        private static readonly DependencyProperty TemplateProperty = DependencyProperty.Register("Template",
            typeof(DataTemplate), typeof(BlockTemplateContent), new PropertyMetadata(OnTemplateChanged));


        public BlockTemplateContent()
        {
            Helpers.FixupDataContext(this);
            Loaded += BlockTemplateContent_Loaded;
        }

        public DataTemplate Template
        {
            get => (DataTemplate) GetValue(TemplateProperty);
            set => SetValue(TemplateProperty, value);
        }


        private void BlockTemplateContent_Loaded(object sender, RoutedEventArgs e)
        {
            GenerateContent(Template);
        }


        private void GenerateContent(DataTemplate template)
        {
            Blocks.Clear();
            if (template != null)
            {
                var element = Helpers.LoadDataTemplate(template);
                Blocks.Add((Block) element);
            }
        }

        private void OnTemplateChanged(DataTemplate dataTemplate)
        {
            GenerateContent(dataTemplate);
        }


        private static void OnTemplateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((BlockTemplateContent) d).OnTemplateChanged((DataTemplate) e.NewValue);
        }
    }
}