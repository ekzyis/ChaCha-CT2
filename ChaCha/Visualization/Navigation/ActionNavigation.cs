using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Cryptool.Plugins.ChaCha
{
    public class ActionNavigation : IActionNavigation<TextBlock, Border, Shape, RichTextBox, TextBox>
    {
        private readonly Brush _copyBrush = Brushes.AliceBlue;
        private readonly Brush _markBrush = Brushes.Purple;

        #region Interface implementation
        [Obsolete("This method has been deprecated because it was incompatible with caching.", true)]
        public bool SaveStateHasBeenCalled { get; private set; } = false;
        // Stack with actions where the last dictionary contains undo actions which reverts changes from the last applied page action of an UI Element.
        [Obsolete("This method has been deprecated because it was incompatible with caching.", true)]
        private readonly Stack<Dictionary<int, Action>> _undoState = new Stack<Dictionary<int, Action>>();
        // temporary variable to collect undo actions before pushing into stack.
        [Obsolete("This method has been deprecated because it was incompatible with caching.", true)]
        private readonly Dictionary<int, Action> _undoActions = new Dictionary<int, Action>();
        [Obsolete("This method has been deprecated because it was incompatible with caching.", true)]
        public void SaveState(params TextBlock[] textblocks)
        {
            SaveStateHasBeenCalled = true;
            foreach (TextBlock tb in textblocks)
            {
                int hash = tb.GetHashCode();
                // do not overwrite states since first added state was the "most original one"
                if (!_undoActions.ContainsKey(hash))
                {
                    // copy inline elements
                    Inline[] state = new Inline[tb.Inlines.Count];
                    tb.Inlines.CopyTo(state, 0);
                    _undoActions[hash] = () =>
                    {
                        tb.Inlines.Clear();
                        foreach (Inline i in state)
                        {
                            tb.Inlines.Add(i);
                        }
                    };
                }
            }
        }

        [Obsolete("This method has been deprecated because it was incompatible with caching.", true)]
        public void SaveState(params Border[] borders)
        {
            SaveStateHasBeenCalled = true;
            foreach (Border b in borders)
            {
                int hash = b.GetHashCode();
                if (!_undoActions.ContainsKey(hash))
                {
                    Brush background;
                    Brush borderBrush;
                    Thickness borderThickness;
                    if (b.Background != null)
                    {
                        background = b.Background.Clone();
                    }
                    else
                    {
                        background = Brushes.White;
                    }
                    if (b.BorderBrush != null)
                    {
                        borderBrush = b.BorderBrush.Clone();
                    }
                    else
                    {
                        borderBrush = Brushes.Black;
                    }
                    if (b.BorderThickness != null)
                    {
                        borderThickness = b.BorderThickness;
                    }
                    else
                    {
                        borderThickness = new Thickness(1);
                    }
                    _undoActions[hash] = () =>
                    {
                        b.Background = background;
                        b.BorderBrush = borderBrush;
                        b.BorderThickness = borderThickness;
                    };
                }
            }
        }

        [Obsolete("This method has been deprecated because it was incompatible with caching.", true)]
        public void SaveState(params Shape[] shapes)
        {
            SaveStateHasBeenCalled = true;
            foreach (Shape s in shapes)
            {
                int hash = s.GetHashCode();
                if (!_undoActions.ContainsKey(hash))
                {
                    Brush strokeBrush;
                    double strokeThickness = s.StrokeThickness;
                    if (s.Stroke != null)
                    {
                        strokeBrush = s.Stroke.Clone();
                    }
                    else
                    {
                        strokeBrush = Brushes.Black;
                    }
                    _undoActions[hash] = () =>
                    {
                        s.Stroke = strokeBrush;
                        s.StrokeThickness = strokeThickness;
                    };
                }
            }
        }

        [Obsolete("This method has been deprecated because it was incompatible with caching.", true)]
        public void SaveState(params RichTextBox[] textboxes)
        {
            SaveStateHasBeenCalled = true;
            foreach (RichTextBox rtb in textboxes)
            {
                int hash = rtb.GetHashCode();
                // copy block element
                Block[] state = new Block[rtb.Document.Blocks.Count];
                rtb.Document.Blocks.CopyTo(state, 0);
                _undoActions[hash] = () =>
                {
                    rtb.Document.Blocks.Clear();
                    foreach (Block b in state)
                    {
                        rtb.Document.Blocks.Add(b);
                    }
                };
            }
        }

        [Obsolete("This method has been deprecated because it was incompatible with caching.", true)]
        public void SaveState(params TextBox[] textboxes)
        {
            SaveStateHasBeenCalled = true;
            foreach (TextBox tb in textboxes)
            {
                int hash = tb.GetHashCode();
                // copy text element
                string state = tb.Text;
                _undoActions[hash] = () =>
                {
                    tb.Text = state;
                };
            }
        }

        [Obsolete("This method has been deprecated because it was incompatible with caching.", true)]
        public void FinishPageAction()
        {
            // copy dictionary using new
            _undoState.Push(new Dictionary<int, Action>(_undoActions));
            _undoActions.Clear();
            SaveStateHasBeenCalled = false;
        }

        [Obsolete("This method has been deprecated because it was incompatible with caching.", true)]
        public Dictionary<int, Action> GetUndoActions()
        {
            return _undoState.Pop();
        }

        [Obsolete("This method has been deprecated because it was incompatible with caching.", true)]
        public void Undo()
        {
            Dictionary<int, Action> undoActions = GetUndoActions();
            foreach (Action undo in undoActions.Values)
            {
                undo();
            }
        }
        #endregion

        #region Private and helper methods (methods which do not call SaveState)
#pragma warning disable IDE1006
        private static void _RemoveLast(InlineCollection list)
        {
            list.Remove(list.LastInline);
        }
        private static void _ReplaceLast(InlineCollection list, Inline element)
        {
            _RemoveLast(list);
            list.Add(element);
        }
        private static void _MakeBoldLast(InlineCollection list)
        {
            _ReplaceLast(list, MakeBold((Run)(list.LastInline)));
        }
        private static void _UnboldLast(InlineCollection list)
        {
            _ReplaceLast(list, new Run { Text = ((Run)(list.LastInline)).Text });
        }
        private static void _Add(InlineCollection list, Inline element)
        {
            list.Add(element);
        }
        private static void _Clear(InlineCollection list)
        {
            list.Clear();
        }
        private static void _RemoveLast(BlockCollection list)
        {
            list.Remove(list.LastBlock);
        }
        private static void _ReplaceLast(BlockCollection list, Inline element)
        {
            _RemoveLast(list);
            list.Add(new Paragraph(element));
        }
        private static void _UnboldLast(BlockCollection list)
        {
            if (list.LastBlock != null) _ReplaceLast(list, new Run { Text = (((Run)((Paragraph)(list.LastBlock)).Inlines.LastInline).Text) });
        }
        private static void _MakeBoldLast(BlockCollection list)
        {
            if (list.LastBlock != null) _ReplaceLast(list, MakeBold(new Run { Text = (((Run)((Paragraph)(list.LastBlock)).Inlines.LastInline).Text) }));
        }
        private static void _Add(BlockCollection list, Inline element)
        {
            list.Add(new Paragraph(element));
        }
        private static void _Clear(BlockCollection list)
        {
            list.Clear();
        }
#pragma warning restore IDE1006

        private static Run MakeBold(Run r)
        {
            return new Run { Text = r.Text, FontWeight = FontWeights.Bold };
        }
        #endregion

        #region Generic API (FrameworkElement, Control, ...)
        public void Show(params FrameworkElement[] elements)
        {
            foreach (FrameworkElement element in elements) element.Visibility = Visibility.Visible;
        }
        public void Collapse(params FrameworkElement[] elements)
        {
            foreach(FrameworkElement element in elements) element.Visibility = Visibility.Collapsed;
        }
        public void Hide(FrameworkElement element)
        {
            element.Visibility = Visibility.Hidden;
        }
        public void ResetMargin(params FrameworkElement[] tbs)
        {
            tbs[0].Margin = new Thickness(0);
        }
        public void SetFontSize(Control c, double size)
        {
            c.FontSize = size;
        }

        public void SetStyle(FrameworkElement element, Style s)
        {
            element.Style = s;
        }
        #endregion

        #region TextBlock API
        public void RemoveLast(TextBlock tb)
        {
            _RemoveLast(tb.Inlines);
        }
        public void ReplaceLast(TextBlock tb, Inline element)
        {
            _ReplaceLast(tb.Inlines, element);
        }
        public void ReplaceLast(TextBlock tb, string text)
        {
            _ReplaceLast(tb.Inlines, new Run(text));
        }
        public void MakeBoldLast(TextBlock tb)
        {
            _MakeBoldLast(tb.Inlines);
        }
        public void UnboldLast(params TextBlock[] tbs)
        {
            foreach (TextBlock tb in tbs)
            {
                _UnboldLast(tb.Inlines);
            }
        }
        public void Add(TextBlock tb, Inline element)
        {
            _Add(tb.Inlines, element);
        }
        public void Add(TextBlock tb, string element)
        {
            Add(tb, new Run(element));
        }
        public void AddBold(TextBlock tb, string element)
        {
            Add(tb, MakeBold(new Run(element)));
        }
        public void Clear(params TextBlock[] textblocks)
        {
            foreach (TextBlock tb in textblocks)
            {
                _Clear(tb.Inlines);
            }
        }
        #endregion

        #region Border API
        public void SetBackground(Border b, Brush background)
        {
            b.Background = background;
        }
        public void SetCopyBackground(params Border[] borders)
        {
            foreach (Border b in borders)
            {
                SetBackground(b, _copyBrush);
            }
        }
        public void UnsetBackground(params Border[] borders)
        {
            foreach (Border b in borders)
            {
                SetBackground(b, Brushes.White);
            }
        }
        public void SetBorderColor(Border b, Brush borderBrush)
        {
            b.BorderBrush = borderBrush;
        }
        public void SetBorderStroke(Border b, double stroke)
        {
            b.BorderThickness = new Thickness(stroke);
        }
        public void MarkBorder(Border b)
        {
            SetBorderColor(b, _markBrush);
            SetBorderStroke(b, 2);
        }
        public void UnmarkBorder(Border b)
        {
            SetBorderColor(b, Brushes.Black);
            SetBorderStroke(b, 1);
        }

        public PageAction MarkCopyFromAction(Border[] borders)
        {
            return new PageAction(() =>
            {
                foreach (Border b in borders)
                {
                    SetCopyBackground(b);
                }
            }, () =>
            {
                foreach (Border b in borders)
                {
                    UnsetBackground(b);
                }
            });
        }

        public PageAction CopyAction(Border[] copyFrom, Border[] copyTo, string[] previousToValue, bool replace = false)
        {
            Debug.Assert(copyTo.Length == copyFrom.Length, $"Amount of copyTo ({copyTo.Length}) and copyFrom elements ({copyFrom.Length}) not equal");
            Debug.Assert(copyTo.Length == previousToValue.Length, $"Amount of copyTo ({copyTo.Length}) and previousToValue elements ({previousToValue.Length}) not equal");
            return new PageAction(() =>
            {
                for (int i = 0; i < copyTo.Length; ++i)
                {
                    Border copyFromBorder = copyFrom[i];
                    Border copyToBorder = copyTo[i];
                    SetCopyBackground(copyToBorder);
                    string text = "";
                    if (copyFromBorder.Child is TextBlock copyFromTextBlock)
                    {
                        text = ((Run)copyFromTextBlock.Inlines.LastInline).Text;
                    }
                    else if (copyFromBorder.Child is RichTextBox copyFromRichTextBox)
                    {
                        TextRange textRange = new TextRange(copyFromRichTextBox.Document.ContentStart, copyFromRichTextBox.Document.ContentEnd);
                        text = textRange.Text;
                    }
                    else if (copyFromBorder.Child is TextBox copyFromTextBox)
                    {
                        text = copyFromTextBox.Text;
                    }
                    else if (copyFromBorder.Child is StackPanel copyFromStackPanel)
                    {
                        text = copyFromStackPanel.Children.OfType<TextBox>().First().Text;
                    }
                    else
                    {
                        Debug.Assert(false, "Input type for CopyAction not supported.");
                    }
                    if (copyToBorder.Child is TextBlock copyToTextBlock)
                    {
                        if (replace)
                        {
                            ReplaceLast(copyToTextBlock, text);
                        }
                        else
                        {
                            Add(copyToTextBlock, text);
                        }
                    }
                    else if (copyToBorder.Child is RichTextBox copyToRichTextBox)
                    {
                        if (replace)
                        {
                            ReplaceLast(copyToRichTextBox, text);
                        }
                        else
                        {
                            Add(copyToRichTextBox, text);
                        }
                    }
                    else if (copyToBorder.Child is TextBox copyToTextBox)
                    {
                        Replace(copyToTextBox, text);
                    }
                    else if (copyToBorder.Child is StackPanel copyToStackPanel)
                    {
                        Replace(copyToStackPanel.Children.OfType<TextBox>().First(), text);
                    }
                    else
                    {
                        Debug.Assert(false, "Output type for CopyAction not supported.");
                    }
                }
            }, () =>
            {
                for (int i = 0; i < copyTo.Length; ++i)
                {
                    Border copyFromBorder = copyFrom[i];
                    Border copyToBorder = copyTo[i];
                    UnsetBackground(copyToBorder);
                    string text = previousToValue[i];
                    if (copyToBorder.Child is TextBlock copyToTextBlock)
                    {
                        ReplaceLast(copyToTextBlock, text);
                    }
                    else if (copyToBorder.Child is RichTextBox copyFromRichTextBox)
                    {
                        ReplaceLast(copyFromRichTextBox, text);
                    }
                    else if (copyToBorder.Child is TextBox copyFromTextBox)
                    {
                        Replace(copyFromTextBox, text);
                    }
                    else if (copyToBorder.Child is StackPanel copyFromStackPanel)
                    {
                        Replace(copyFromStackPanel.Children.OfType<TextBox>().First(), text);
                    }
                    else
                    {
                        Debug.Assert(false, "Output type for CopyAction not supported.");
                    }
                }
            });
        }

        public PageAction UnmarkCopyAction(Border[] copyFrom, Border[] copyTo)
        {
            return new PageAction(() =>
            {
                foreach (Border b in copyFrom)
                {
                    UnsetBackground(b);
                }
                foreach (Border b in copyTo)
                {
                    UnsetBackground(b);
                }
            }, () =>
            {
                foreach (Border b in copyFrom)
                {
                    SetCopyBackground(b);
                }
                foreach (Border b in copyTo)
                {
                    SetCopyBackground(b);
                }
            });
        }

        public PageAction[] CopyActions(Border[] copyFrom, Border[] copyTo, string[] previousToValue, bool replace = false)
        {
            Debug.Assert(copyFrom.Length == copyTo.Length);
            PageAction mark = MarkCopyFromAction(copyFrom);
            PageAction copy = CopyAction(copyFrom, copyTo, previousToValue, replace);
            PageAction unmark = UnmarkCopyAction(copyFrom, copyTo);
            return new PageAction[] { mark, copy, unmark };
        }

        public PageAction[] CopyActions(Border[] copyFrom, Shape[] paths, Border[] copyTo, string[] previousToValue, bool replace = false)
        {
            PageAction[] copyActions = CopyActions(copyFrom, copyTo, previousToValue, replace);

            void MarkPaths()
            {
                foreach (Shape s in paths)
                {
                    MarkShape(s);
                }
            }

            void UndoMarkPaths()
            {
                foreach (Shape s in paths)
                {
                    UnmarkShape(s);

                }
            }

            PageAction mark = copyActions[0];
            mark.AddToExec(MarkPaths);
            mark.AddToUndo(UndoMarkPaths);
            PageAction unmark = copyActions[2];
            unmark.AddToExec(UndoMarkPaths);
            unmark.AddToUndo(MarkPaths);
            return copyActions;
        }

        public void Clear(params Border[] borders)
        {
            foreach (Border b in borders)
            {
                UnsetBackground(b);
                UnmarkBorder(b);
                if (b.Child is TextBlock tblock)
                {
                    Clear(tblock);
                }
                else if (b.Child is RichTextBox rtb)
                {
                    Clear(rtb);
                }
                else if (b.Child is TextBox tbox)
                {
                    Clear(tbox);
                }
                else if (b.Child is StackPanel sp)
                {
                    Clear(sp);
                }
                else
                {
                    Debug.Assert(false, "Child Clear Border not supported.");
                }
            }
        }

        #endregion

        #region Shape API
        public void SetShapeStrokeColor(Shape s, Brush brush)
        {
            s.Stroke = brush;
        }
        public void SetShapeStroke(Shape s, double stroke)
        {
            s.StrokeThickness = stroke;
        }
        public void SetShapeStroke(double stroke, params Shape[] shapes)
        {
            foreach (Shape s in shapes)
            {
                SetShapeStroke(s, stroke);
            }
        }
        public void MarkShape(params Shape[] shapes)
        {
            foreach (Shape s in shapes)
            {
                SetShapeStrokeColor(s, _markBrush);
                SetShapeStroke(s, 2);
            }
        }
        public void UnmarkShape(params Shape[] shapes)
        {
            foreach (Shape s in shapes)
            {
                SetShapeStrokeColor(s, Brushes.Black);
                SetShapeStroke(s, 1);
            }
        }

        public void Clear(params Shape[] shapes)
        {
            UnmarkShape(shapes);
        }
        #endregion

        #region RichTextBox API
        public void UnboldLast(params RichTextBox[] rtbs)
        {
            foreach (RichTextBox rtb in rtbs)
            {
                _UnboldLast(rtb.Document.Blocks);
            }
        }
        public void MakeBoldLast(RichTextBox tb)
        {
            _MakeBoldLast(tb.Document.Blocks);
        }
        public void RemoveLast(RichTextBox tb)
        {
            _RemoveLast(tb.Document.Blocks);
        }
        public void ReplaceLast(RichTextBox tb, string text)
        {
            _ReplaceLast(tb.Document.Blocks, new Run(text));
        }
        public void Add(RichTextBox tb, Inline element)
        {
            _Add(tb.Document.Blocks, element);
        }
        public void Add(RichTextBox tb, string text)
        {
            Add(tb, new Run(text));
        }
        public void AddBold(RichTextBox tb, string text)
        {
            Add(tb, MakeBold(new Run(text)));
        }
        public void Clear(params RichTextBox[] tbs)
        {
            foreach (RichTextBox tb in tbs)
            {
                _Clear(tb.Document.Blocks);
            }
        }
        public void Clear(RichTextBox rtb, string text)
        {
            _Clear(rtb.Document.Blocks);
            Add(rtb, text);
        }

        public void SetDocument(RichTextBox rtb, FlowDocument doc)
        {
            rtb.Document = doc;
        }

        public void SetDocumentAndShow(RichTextBox rtb, FlowDocument doc)
        {
            SetDocument(rtb, doc);
            Show(rtb);
        }
        public void ClearAndCollapse(params RichTextBox[] rtbs)
        {
            Clear(rtbs);
            Collapse(rtbs);
        }
        #endregion

        #region TextBox API
        public void Replace(TextBox tb, string text)
        {
            tb.Text = text;
        }

        public void Add(TextBox tb, string text)
        {
            tb.Text += text;
        }

        public void Clear(params TextBox[] tbs)
        {
            foreach (TextBox tb in tbs)
            {
                tb.Text = "";
            }
        }

        public void ClearAndCollapse(params TextBox[] tbs)
        {
            Clear(tbs);
            Collapse(tbs);
        }

        public void ReplaceAndShow(TextBox tb, string text)
        {
            Replace(tb, text);
            Show(tb);
        }
        #endregion

        #region StackPanel API
        public void AddTextBox(StackPanel sp, string text, Style style = null)
        {
            TextBox tb = new TextBox() { Text = text };
            if(style != null)
            {
                tb.Style = style;
            }
            Add(sp, tb);
        }
        public void Add(StackPanel sp, TextBox tb)
        {
            sp.Children.Add(tb);
        }
        public void RemoveLast(StackPanel sp)
        {
            sp.Children.RemoveAt(sp.Children.Count - 1);
        }
        public void Clear(StackPanel sp)
        {
            TextBox[] tbChilds = sp.Children.OfType<TextBox>().ToArray();
            Clear(tbChilds);
            RichTextBox[] rtbChilds = sp.Children.OfType<RichTextBox>().ToArray();
            Clear(rtbChilds);
        }
        #endregion
    }
}
