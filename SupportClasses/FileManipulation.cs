using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;


namespace SupportClasses
{
    public static class RTFIO
    {
        public static void SaveRTF(string filename, RichTextBox rtb)
        {
            // Extract Text Range From RichTextBox
            TextRange rtbRange = new TextRange(rtb.Document.ContentStart, rtb.Document.ContentEnd);

            // File Stream
            FileStream fStream;
            using (fStream = new(filename, FileMode.Create))
            {
                // Save RichTextBox to a file specified by _fileName
                rtbRange.Save(fStream, DataFormats.Rtf);
            }

        }

        public static void LoadRTF(string filename, RichTextBox rtb)
        {
            TextRange range;
            FileStream fStream;
            if (File.Exists(filename))
            {
                range = new TextRange(rtb.Document.ContentStart, rtb.Document.ContentEnd);
                fStream = new FileStream(filename, FileMode.OpenOrCreate);
                range.Load(fStream, DataFormats.Rtf);
                fStream.Close();
            }
        }

    }
}