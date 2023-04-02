using System;
using System.Windows.Controls;
using System.Windows.Documents;

namespace SupportClasses
{
    public static class RichTextBoxSupport
    {
        public static TextRange FindTextInRange(RichTextBox rtb, string searchText)
        {
            TextRange searchRange = new TextRange(rtb.Document.ContentStart, rtb.Document.ContentEnd);

            int offset = searchRange.Text.IndexOf(searchText, StringComparison.OrdinalIgnoreCase);
            if (offset < 0)
                return null;  // Not found

            var start = GetTextPositionAtOffset(searchRange.Start, offset);
            TextRange result = new TextRange(start, GetTextPositionAtOffset(start, searchText.Length));

            return result;
        }

        public static TextPointer GetTextPositionAtOffset(TextPointer position, int offset)
        {
            for (TextPointer current = position; current != null; current = position.GetNextContextPosition(LogicalDirection.Forward))
            {
                position = current;
                var adjacent = position.GetAdjacentElement(LogicalDirection.Forward);
                var context = position.GetPointerContext(LogicalDirection.Forward);
                switch (context)
                {
                    case TextPointerContext.Text:
                        int count = position.GetTextRunLength(LogicalDirection.Forward);
                        if (offset <= count)
                        {
                            return position.GetPositionAtOffset(offset);
                        }
                        offset -= count;
                        break;
                    case TextPointerContext.ElementStart:
                        if (adjacent is InlineUIContainer)
                        {
                            offset--;
                        }
                        else if (adjacent is ListItem lsItem)
                        {
                            var trange = new TextRange(lsItem.ElementStart, lsItem.ElementEnd);
                            var index = trange.Text.IndexOf('\t');
                            if (index >= 0)
                            {
                                offset -= index + 1;
                            }
                        }
                        break;
                    case TextPointerContext.ElementEnd:
                        if (adjacent is Paragraph)
                            offset -= 2;
                        break;
                }
            }
            return position;
        }
    }
}
