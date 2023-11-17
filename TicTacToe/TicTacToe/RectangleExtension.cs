using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace TicTacToe
{
    /// <summary>
    /// Class creating an attached property `FieldValue` to automate showing crosses and circles in the UI.
    /// </summary>
    public class RectangleExtension
    {
        public static DependencyProperty FieldValueProperty = DependencyProperty.RegisterAttached(
            "FieldValue",
            typeof(TicTacToeGame.Field),
            typeof(RectangleExtension),
            new FrameworkPropertyMetadata(TicTacToeGame.Field.Empty, flags: FrameworkPropertyMetadataOptions.AffectsRender)
            );
        public static TicTacToeGame.Field GetFieldValue(UIElement target) => (TicTacToeGame.Field)target.GetValue(FieldValueProperty);
        public static void SetFieldValue(UIElement target, TicTacToeGame.Field value) => target.SetValue(FieldValueProperty, value);
    }
}
