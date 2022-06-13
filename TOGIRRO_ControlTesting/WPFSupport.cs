using System;
using System.ComponentModel;
using System.Globalization;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Markup;

namespace TOGIRRO_ControlTesting
{
    //========================================================================================================================================
    //===Вспомогательный класс-конвертер описаний перечислений================================================================================
    //========================================================================================================================================
    #region
    public class DescriptionConverter : EnumConverter
	{
		public DescriptionConverter(Type type) : base(type) { }
		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
		{
			if (destinationType == typeof(string))
			{
				if (value != null)
				{
					FieldInfo fieldInfo = value.GetType().GetField(value.ToString());
					if (fieldInfo != null)
					{
						var attributes = (DescriptionAttribute[])fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), false);

						return ((attributes.Length > 0) && (!string.IsNullOrEmpty(attributes[0].Description))) ? attributes[0].Description : value.ToString();
					}
				}
				return string.Empty;
			}
			return base.ConvertTo(context, culture, value, destinationType);
		}
	}
    #endregion
    //========================================================================================================================================



    //========================================================================================================================================
    //===Вспомогательный класс-конвертер для автоматической нумерации строк в DataGrid========================================================
    //========================================================================================================================================
    #region
    public class RowToIndexConverter : MarkupExtension, IValueConverter
	{
		static RowToIndexConverter converter;

		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			if (value is DataGridRow row) return row.GetIndex() + 1;
			else return -1;
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}

		public override object ProvideValue(IServiceProvider serviceProvider)
		{
			if (converter == null) converter = new RowToIndexConverter();
			return converter;
		}

		public RowToIndexConverter()
		{
		}
	}
    #endregion
    //========================================================================================================================================



    //========================================================================================================================================
    //===Вспомогательный класс-элемент для числового стобца в DataGrid========================================================================
    //========================================================================================================================================
    #region
    public class DataGridNumericColumn : DataGridTextColumn
	{
		protected override object PrepareCellForEdit(System.Windows.FrameworkElement editingElement, System.Windows.RoutedEventArgs editingEventArgs)
		{
			TextBox edit = editingElement as TextBox;
			edit.PreviewTextInput += OnPreviewTextInput;
			edit.PreviewKeyDown += new KeyEventHandler(OnKeyDown);

			return base.PrepareCellForEdit(editingElement, editingEventArgs);
		}

		private void OnPreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
		{
			try
			{
				Convert.ToUInt32(e.Text);
			}
			catch
			{
				e.Handled = true;
			}
		}

		private void OnKeyDown(object sender, KeyEventArgs e)
        {
			if ((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
            {
				if (e.Key == Key.V)
				{
					e.Handled = true;
				}
            }
		}
	}
    #endregion
    //========================================================================================================================================
}
