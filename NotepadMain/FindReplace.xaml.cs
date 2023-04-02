using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace NotepadMain
{
    /// <summary>
    /// Interaction logic for FindReplace.xaml
    /// </summary>
    public partial class FindReplace : Window
    {
        #region Properties
        private TextRange SelectedWordRange { set; get; }
        private RichTextBox RtbMainWindow { set; get; }
        #endregion

        public FindReplace(RichTextBox rtbMainWindow)
        {
            RtbMainWindow = rtbMainWindow;
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {


        }

        private void Window_ContentRendered(object sender, System.EventArgs e)
        {
            this.MinHeight = this.ActualHeight;
            this.MaxHeight = this.ActualHeight;
        }

        private void findButton_Click(object sender, RoutedEventArgs e)
        {
            // check if Find not empty
            if (findTextBox.Text.Length > 0)
            {


                // Find Range of Word
                TextRange findResultRange = SupportClasses.RichTextBoxSupport.FindTextInRange(RtbMainWindow, findTextBox.Text);

                // If word found
                if (findResultRange != null)
                {
                    // If word already selected, remove color
                    if (SelectedWordRange != null)
                    {
                        SelectedWordRange.ApplyPropertyValue(TextElement.BackgroundProperty, null);
                    }

                    SelectedWordRange = findResultRange;
                    SelectedWordRange.ApplyPropertyValue(TextElement.BackgroundProperty, new SolidColorBrush(Colors.Yellow));
                }
            }
        }

        private void Window_Unloaded(object sender, RoutedEventArgs e)
        {
            if (SelectedWordRange != null)
            {
                SelectedWordRange.ApplyPropertyValue(TextElement.BackgroundProperty, null);
            }

        }

        private void replaceButton_Click(object sender, RoutedEventArgs e)
        {
            if (findTextBox.Text.Length > 0 && replaceWithTextBox.Text.Length > 0)
            {
                // Find and Replace Word
                //SupportClasses.RichTextBoxSupport.ReplaceTextInRange(RtbMainWindow, findTextBox.Text, replaceWithTextBox.Text);
                SelectedWordRange = SupportClasses.RichTextBoxSupport.FindTextInRange(RtbMainWindow, findTextBox.Text);
                SelectedWordRange.Text = replaceWithTextBox.Text;
                RtbMainWindow.Focus();

                // Remove Highlight
                SelectedWordRange.ApplyPropertyValue(TextElement.BackgroundProperty, new SolidColorBrush(Colors.Yellow));

            }
        }
    }
}
