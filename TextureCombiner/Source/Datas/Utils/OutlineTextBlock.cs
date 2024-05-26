using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Markup;
using System.Windows.Media;

namespace TextureCombiner.Datas.Utils
{
    [ContentProperty("Text")]
    public class OutlinedTextBlock : FrameworkElement
    {
#region DependencyProperties
        public static readonly DependencyProperty ForegroundColorProperty = DependencyProperty.Register(
          "ForegroundColor",
          typeof(Brush),
          typeof(OutlinedTextBlock),
          new FrameworkPropertyMetadata(Brushes.Black, FrameworkPropertyMetadataOptions.AffectsRender));

        #region Stroke
        public static readonly DependencyProperty StrokeProperty = DependencyProperty.Register(
          "Stroke",
          typeof(Brush),
          typeof(OutlinedTextBlock),
          new FrameworkPropertyMetadata(Brushes.Black, FrameworkPropertyMetadataOptions.AffectsRender, StrokePropertyChangedCallback));

        public static readonly DependencyProperty StrokeThicknessProperty = DependencyProperty.Register(
          "StrokeThickness",
          typeof(double),
          typeof(OutlinedTextBlock),
          new FrameworkPropertyMetadata(1d, FrameworkPropertyMetadataOptions.AffectsRender, StrokePropertyChangedCallback));
        #endregion

        #region Font
        public static readonly DependencyProperty FontFamilyProperty = TextElement.FontFamilyProperty.AddOwner(
          typeof(OutlinedTextBlock),
          new FrameworkPropertyMetadata(OnFormattedTextUpdated));

        public static readonly DependencyProperty FontSizeProperty = TextElement.FontSizeProperty.AddOwner(
          typeof(OutlinedTextBlock),
          new FrameworkPropertyMetadata(OnFormattedTextUpdated));

        public static readonly DependencyProperty FontStretchProperty = TextElement.FontStretchProperty.AddOwner(
          typeof(OutlinedTextBlock),
          new FrameworkPropertyMetadata(OnFormattedTextUpdated));

        public static readonly DependencyProperty FontStyleProperty = TextElement.FontStyleProperty.AddOwner(
          typeof(OutlinedTextBlock),
          new FrameworkPropertyMetadata(OnFormattedTextUpdated));

        public static readonly DependencyProperty FontWeightProperty = TextElement.FontWeightProperty.AddOwner(
          typeof(OutlinedTextBlock),
          new FrameworkPropertyMetadata(OnFormattedTextUpdated));
        #endregion

        #region Text
        public static readonly DependencyProperty TextProperty = DependencyProperty.Register(
          "Text",
          typeof(string),
          typeof(OutlinedTextBlock),
          new FrameworkPropertyMetadata(OnFormattedTextInvalidated));

        public static readonly DependencyProperty TextAlignmentProperty = DependencyProperty.Register(
          "TextAlignment",
          typeof(TextAlignment),
          typeof(OutlinedTextBlock),
          new FrameworkPropertyMetadata(OnFormattedTextUpdated));

        public static readonly DependencyProperty TextDecorationsProperty = DependencyProperty.Register(
          "TextDecorations",
          typeof(TextDecorationCollection),
          typeof(OutlinedTextBlock),
          new FrameworkPropertyMetadata(OnFormattedTextUpdated));

        public static readonly DependencyProperty TextTrimmingProperty = DependencyProperty.Register(
          "TextTrimming",
          typeof(TextTrimming),
          typeof(OutlinedTextBlock),
          new FrameworkPropertyMetadata(OnFormattedTextUpdated));

        public static readonly DependencyProperty TextWrappingProperty = DependencyProperty.Register(
          "TextWrapping",
          typeof(TextWrapping),
          typeof(OutlinedTextBlock),
          new FrameworkPropertyMetadata(TextWrapping.NoWrap, OnFormattedTextUpdated));
        #endregion
        #endregion

        #region Fields
        FormattedText formattedText;
        Geometry textGeometry;
        Pen pen;
        #endregion

        #region Properties
        public Brush ForegroundColor
        {
            get { return (Brush)GetValue(ForegroundColorProperty); }
            set { SetValue(ForegroundColorProperty, value); }
        }

        #region Font
        public FontFamily FontFamily
        {
            get { return (FontFamily)GetValue(FontFamilyProperty); }
            set { SetValue(FontFamilyProperty, value); }
        }

        [TypeConverter(typeof(FontSizeConverter))]
        public double FontSize
        {
            get { return (double)GetValue(FontSizeProperty); }
            set { SetValue(FontSizeProperty, value); }
        }

        public FontStretch FontStretch
        {
            get { return (FontStretch)GetValue(FontStretchProperty); }
            set { SetValue(FontStretchProperty, value); }
        }

        public FontStyle FontStyle
        {
            get { return (FontStyle)GetValue(FontStyleProperty); }
            set { SetValue(FontStyleProperty, value); }
        }

        public FontWeight FontWeight
        {
            get { return (FontWeight)GetValue(FontWeightProperty); }
            set { SetValue(FontWeightProperty, value); }
        }
        #endregion

        #region Stroke
        public Brush Stroke
        {
            get { return (Brush)GetValue(StrokeProperty); }
            set { SetValue(StrokeProperty, value); }
        }

        public double StrokeThickness
        {
            get { return (double)GetValue(StrokeThicknessProperty); }
            set { SetValue(StrokeThicknessProperty, value); }
        }
        #endregion

        #region Text
        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        public TextAlignment TextAlignment
        {
            get { return (TextAlignment)GetValue(TextAlignmentProperty); }
            set { SetValue(TextAlignmentProperty, value); }
        }

        public TextDecorationCollection TextDecorations
        {
            get { return (TextDecorationCollection)GetValue(TextDecorationsProperty); }
            set { SetValue(TextDecorationsProperty, value); }
        }

        public TextTrimming TextTrimming
        {
            get { return (TextTrimming)GetValue(TextTrimmingProperty); }
            set { SetValue(TextTrimmingProperty, value); }
        }

        public TextWrapping TextWrapping
        {
            get { return (TextWrapping)GetValue(TextWrappingProperty); }
            set { SetValue(TextWrappingProperty, value); }
        }
        #endregion
        #endregion

        #region Constructor
        public OutlinedTextBlock()
        {
            TextDecorations = new TextDecorationCollection();
            RefreshPen();
        }
        #endregion

        #region Methods
        void RefreshPen()
        {
            pen = new Pen(Stroke, StrokeThickness)
            {
                DashCap = PenLineCap.Round,
                EndLineCap = PenLineCap.Round,
                LineJoin = PenLineJoin.Round,
                StartLineCap = PenLineCap.Round
            };

            InvalidateVisual();
        }

        protected override void OnRender(DrawingContext _drawingContext)
        {
            PrepareGeometry();

            _drawingContext.DrawGeometry(null, pen, textGeometry);
            _drawingContext.DrawGeometry(ForegroundColor, null, textGeometry);
        }

        void PrepareGeometry()
        {
            if (textGeometry != null)
                return;

            PrepareFormattedText();
            textGeometry = formattedText.BuildGeometry(new Point(0, 0));
        }

        void PrepareFormattedText()
        {
            if (formattedText != null)
                return;

            string _textToDisplay = Text != null ? Text : "";

            formattedText = new FormattedText(
              _textToDisplay,
              CultureInfo.CurrentUICulture,
              FlowDirection,
              new Typeface(FontFamily, FontStyle, FontWeight, FontStretch),
              FontSize,
              Brushes.Black);

            UpdateFormattedText();
        }

        void UpdateFormattedText()
        {
            if (formattedText == null)
                return;

            formattedText.MaxLineCount = TextWrapping == TextWrapping.NoWrap ? 1 : int.MaxValue;
            formattedText.TextAlignment = TextAlignment;
            formattedText.Trimming = TextTrimming;

            formattedText.SetFontSize(FontSize);
            formattedText.SetFontStyle(FontStyle);
            formattedText.SetFontWeight(FontWeight);
            formattedText.SetFontFamily(FontFamily);
            formattedText.SetFontStretch(FontStretch);
            formattedText.SetTextDecorations(TextDecorations);
        }

        static void StrokePropertyChangedCallback(DependencyObject _dependencyObject, 
            DependencyPropertyChangedEventArgs _dependencyPropertyChangedEventArgs)
        {
            OutlinedTextBlock _outlinedTextBlock = (OutlinedTextBlock)_dependencyObject;
            _outlinedTextBlock?.RefreshPen();
        }

        protected override Size MeasureOverride(Size _availableSize)
        {
            PrepareFormattedText();

            // constrain the formatted text according to the available size

            double _width = _availableSize.Width;
            double _height = _availableSize.Height;

            // the Math.Min call is important - without this constraint (which seems arbitrary, but is the maximum allowable text width), things blow up when availableSize is infinite in both directions
            // the Math.Max call is to ensure we don't hit zero, which will cause MaxTextHeight to throw
            formattedText.MaxTextWidth = Math.Min(double.MaxValue, _width);
            formattedText.MaxTextHeight = Math.Max(double.MinValue, _height);

            // return the desired size
            return new Size(Math.Ceiling(formattedText.Width), Math.Ceiling(formattedText.Height));
        }

        protected override Size ArrangeOverride(Size _finalSize)
        {
            PrepareFormattedText();

            // update the formatted text with the final size
            formattedText.MaxTextWidth = _finalSize.Width;
            formattedText.MaxTextHeight = Math.Max(double.MinValue, _finalSize.Height);

            // need to re-generate the geometry now that the dimensions have changed
            textGeometry = null;

            return _finalSize;
        }

        static void OnFormattedTextInvalidated(DependencyObject _dependencyObject,
          DependencyPropertyChangedEventArgs _eventArgs)
        {
            OutlinedTextBlock _outlinedTextBlock = (OutlinedTextBlock)_dependencyObject;
            _outlinedTextBlock.formattedText = null;
            _outlinedTextBlock.textGeometry = null;

            _outlinedTextBlock.InvalidateMeasure();
            _outlinedTextBlock.InvalidateVisual();
        }

        static void OnFormattedTextUpdated(DependencyObject _dependencyObject, 
            DependencyPropertyChangedEventArgs _eventArgs)
        {
            OutlinedTextBlock _outlinedTextBlock = (OutlinedTextBlock)_dependencyObject;
            _outlinedTextBlock.UpdateFormattedText();
            _outlinedTextBlock.textGeometry = null;

            _outlinedTextBlock.InvalidateMeasure();
            _outlinedTextBlock.InvalidateVisual();
        }
        #endregion
    }
}