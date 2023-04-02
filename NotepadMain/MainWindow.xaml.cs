using Microsoft.Win32;
using System;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Printing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Xps;
using System.Windows.Xps.Packaging;
// To get preview printing with XPS
using Media = System.Windows.Media;
using WForms = System.Windows.Forms;

namespace NotepadMain
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string _previewWindowXaml =
                            @"<Window
                                xmlns ='http://schemas.microsoft.com/netfx/2007/xaml/presentation'
                                xmlns:x ='http://schemas.microsoft.com/winfx/2006/xaml'
                                Title ='Print Preview - @@TITLE'
                                Height ='500' Width ='500'
                                WindowStartupLocation ='CenterOwner'>
                                              <DocumentViewer Name='dv1'/>
                             </Window>";

        // Create Save and Open File Dialog
        SaveFileDialog saveFileDialog = new SaveFileDialog()
        {
            Filter = "RTF Files|*.rtf",
            Title = "Save .RTF (Rich Text Format) File"
        };

        OpenFileDialog openFileDialog = new OpenFileDialog()
        {
            Filter = "RTF Files|*.rtf",
            Title = "Open .RTF (Rich Text Format) File"
        };

        public string CurrentFile { set; get; }

        public MainWindow()
        {
            InitializeComponent();
        }

        private void saveAsMenus_Click(object sender, RoutedEventArgs e)
        {
            // If OK Has been Clicked
            if (saveFileDialog.ShowDialog() == true)
            {
                // Get filename Selected
                string fileNameToSave = saveFileDialog.FileName;

                // Use Custom Class to Save File
                SupportClasses.RTFIO.SaveRTF(fileNameToSave, richTextBox1);

                // Save Current File
                this.CurrentFile = fileNameToSave;

            }
        }

        private void openMenus_Click(object sender, RoutedEventArgs e)
        {

            // If OK Has been Clicked
            if (openFileDialog.ShowDialog() == true)
            {
                // Get filename Selected
                string fileNameToOpen = openFileDialog.FileName;

                // Use Custom Class to Save File
                try
                {
                    SupportClasses.RTFIO.LoadRTF(fileNameToOpen, richTextBox1);

                    // Save Current File
                    this.CurrentFile = fileNameToOpen;

                }
                catch (Exception ex)
                {
                    System.Windows.Forms.MessageBox.Show($"Erro While Opening the File:\n{ex.Message}");
                }

            }

        }

        private void saveMenus_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentFile != null)
            {
                SupportClasses.RTFIO.SaveRTF(CurrentFile, richTextBox1);
            }
            else
            {
                // Call SaveAs Event Listener
                saveAsMenus_Click(sender, e);
            }
        }

        private void findMenus_Click(object sender, RoutedEventArgs e)
        {
            // Create Find And Replace window Object
            FindReplace findReplace = new FindReplace(richTextBox1);

            findReplace.Show();
        }

        private void fontMenus_Click(object sender, RoutedEventArgs e)
        {
            // Create Font Dialog
            WForms.FontDialog fontDialog = new WForms.FontDialog();

            // Show Dialog
            if (fontDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                // Change Font with output
                richTextBox1.FontFamily = new Media.FontFamily(fontDialog.Font.Name);
                //
                richTextBox1.FontSize = fontDialog.Font.Size; //fontDialog.Font.Size * 96.0 / 72.0;
                richTextBox1.FontWeight = fontDialog.Font.Bold ? FontWeights.Bold : FontWeights.Regular;
                richTextBox1.FontStyle = fontDialog.Font.Italic ? FontStyles.Italic : FontStyles.Normal;

                // Text Decoration
                TextDecorationCollection tdc = new TextDecorationCollection();
                if (fontDialog.Font.Underline) tdc.Add(TextDecorations.Underline);
                if (fontDialog.Font.Strikeout) tdc.Add(TextDecorations.Strikethrough);

                // TODO: Change Style for each part of the text and add or remove with .Contains(TextDecorations.Strikethrough[0]);
                TextRange allTextRange = new TextRange(richTextBox1.Document.ContentStart, richTextBox1.Document.ContentEnd);
                TextDecorationCollection rangeDecoration = (TextDecorationCollection)allTextRange.GetPropertyValue(Inline.TextDecorationsProperty);
                rangeDecoration.Add(tdc);

            }

        }

        private void PrintDocument_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {
            // Get RTF Font Info
            TextRange allTextRange = new TextRange(richTextBox1.Document.ContentStart, richTextBox1.Document.ContentEnd);
            string current_font = this.richTextBox1.FontFamily.ToString();
            double fontSize = this.richTextBox1.FontSize;

            // Creating font from RTF
            Font printingFont = new Font(new System.Drawing.FontFamily(current_font), (int)fontSize);

            //Font printingFont = new Font(current_font, fontSize)

            e.Graphics.DrawString(allTextRange.Text, printingFont, System.Drawing.Brushes.Black, 66, 50);
        }

        private void printMenus_Click(object sender, RoutedEventArgs e)
        {
            int option = 3;
            // --------------------------------------------------------
            //              OPTION 1 (USING WINDOWS FORMS)
            // --------------------------------------------------------
            if (option == 0)
            {

                // Create Windows Objects
                PrintDocument printDocument = new PrintDocument();
                printDocument.DefaultPageSettings.PaperSize = new PaperSize("Custum", 500, 500); // set Paper Size of the Document
                printDocument.PrintPage += new PrintPageEventHandler(this.PrintDocument_PrintPage); // Event Handler

                // Add Current Documennt to the Dialog
                WForms.PrintPreviewDialog printPreviewDialog = new WForms.PrintPreviewDialog();
                printPreviewDialog.Document = printDocument;

                if (printPreviewDialog.ShowDialog() == WForms.DialogResult.OK)
                {
                    printDocument.Print();
                }
            }
            else if (option == 1)
            {


                // --------------------------------------------------------
                //              OPTION 2 (USING WPF XPS Preview)
                // --------------------------------------------------------

                PrintDialog pd = new PrintDialog();

                // calculate page size  
                var sz = new System.Windows.Size(pd.PrintableAreaWidth, pd.PrintableAreaHeight);

                string tempFileName = System.IO.Path.GetTempFileName();

                File.Delete(tempFileName);

                using (XpsDocument xpsDocument = new XpsDocument(tempFileName, FileAccess.ReadWrite))
                {

                    XpsDocumentWriter writer = XpsDocument.CreateXpsDocumentWriter(xpsDocument);
                    var paginator = (((IDocumentPaginatorSource)richTextBox1.Document).DocumentPaginator);
                    writer.Write(paginator);
                    DocumentViewer previewWindow = new DocumentViewer
                    {
                        Document = xpsDocument.GetFixedDocumentSequence()
                    };

                    Window printpriview = new Window();
                    printpriview.Content = previewWindow;
                    printpriview.Title = "C1FlexGrid: Print Preview";
                    printpriview.Show();
                }
            }
            else if (option == 2)
            {
                // --------------------------------------------------------
                //              OPTION 3 (USING WPF)
                // --------------------------------------------------------

                PrintDialog pd = new PrintDialog();
                if (pd.ShowDialog() == true)
                {
                    pd.PrintVisual(richTextBox1 as Media.Visual, "printing as visual");
                    //pd.PrintDocument((((IDocumentPaginatorSource)richTextBox1.Document).DocumentPaginator), "printing as paginator");
                }
            }
            else if (option == 3)
            {
                // --------------------------------------------------------
                //              OPTION 4 (USING WPG + XPS doesn't freeze app)
                // --------------------------------------------------------

                TextRange range = new TextRange(richTextBox1.Document.ContentStart, richTextBox1.Document.ContentEnd);

                MemoryStream ms = new MemoryStream();
                range.Save(ms, DataFormats.Rtf);

                FlowDocument flowDocumentCopy = new FlowDocument();
                TextRange copyDocumentRange = new TextRange(flowDocumentCopy.ContentStart, flowDocumentCopy.ContentEnd);
                copyDocumentRange.Load(ms, DataFormats.Rtf);

                // Create a XpsDocumentWriter object, open a Windows common print dialog.
                // This methods returns a ref parameter that represents information about the dimensions of the printer media.
                PrintDocumentImageableArea ia = null;
                XpsDocumentWriter docWriter = PrintQueue.CreateXpsDocumentWriter(ref ia);

                if (docWriter != null && ia != null)
                {
                    DocumentPaginator paginator = ((IDocumentPaginatorSource)flowDocumentCopy).DocumentPaginator;

                    // Change the PageSize and PagePadding for the document to match the CanvasSize for the printer device.
                    paginator.PageSize = new System.Windows.Size(ia.MediaSizeWidth, ia.MediaSizeHeight);
                    Thickness pagePadding = flowDocumentCopy.PagePadding;
                    flowDocumentCopy.PagePadding = new Thickness(
                            Math.Max(ia.OriginWidth, pagePadding.Left),
                            Math.Max(ia.OriginHeight, pagePadding.Top),
                            Math.Max(ia.MediaSizeWidth - (ia.OriginWidth + ia.ExtentWidth), pagePadding.Right),
                            Math.Max(ia.MediaSizeHeight - (ia.OriginHeight + ia.ExtentHeight), pagePadding.Bottom));
                    flowDocumentCopy.ColumnWidth = double.PositiveInfinity;

                    // Send DocumentPaginator to the printer.
                    docWriter.Write(paginator);

                }
            }

        }
    }
}
