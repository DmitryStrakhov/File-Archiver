using System;
using System.Windows.Media;
using FileArchiver.Core.Converters;

namespace FileArchiver.Core.Controls {
    public class ArrowPenBrushConverter : SimpleValueConverterBase<bool, Brush> {
        public ArrowPenBrushConverter() {
        }
        protected override Brush ConvertCore(bool value) {
            return value ? Brushes.Black : Brushes.LightGray;
        }
    }
}