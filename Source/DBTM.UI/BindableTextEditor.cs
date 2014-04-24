using System.Windows;
using ICSharpCode.AvalonEdit;

namespace DBTM.UI
{
    public class BindableTextEditor : TextEditor
    {
        static BindableTextEditor()
        {
            var propertyMetadata = new UIPropertyMetadata(string.Empty, textChangedCallBack);

            BindableTextProperty = DependencyProperty.Register("BindableText", typeof(string), typeof(BindableTextEditor),propertyMetadata);
        }

        static void textChangedCallBack(DependencyObject property, DependencyPropertyChangedEventArgs args)
        {
            BindableTextEditor searchTextBox = (BindableTextEditor)property;

            var newValue = (string)args.NewValue;

            if (searchTextBox.Text!=newValue)
            {
                searchTextBox.Text = newValue;
            }
        }

        public BindableTextEditor()
        {
            TextChanged += (o, args) =>
                                    {
                                        BindableText = Text;
                                    };
        }


        public static readonly DependencyProperty BindableTextProperty;

        public static string GetBindableTextProperty(DependencyObject target)
        {
            return (string)target.GetValue(BindableTextProperty);
        }

        public static void SetBindableTextProperty(DependencyObject target, string value)
        {
            target.SetValue(BindableTextProperty, value);
        }

        public string BindableText
        {
            get { return (string)GetValue(BindableTextProperty); }
            set { SetValue(BindableTextProperty, value); }
        }
    }
}