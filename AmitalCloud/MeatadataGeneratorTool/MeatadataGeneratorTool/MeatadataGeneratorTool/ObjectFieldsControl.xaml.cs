using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MeatadataGeneratorTool
{
    /// <summary>
    /// Interaction logic for ObjectFieldsControl.xaml
    /// </summary>
    public partial class ObjectFieldsControl : UserControl
    {
        public ObjectFieldsControl()
        {
            InitializeComponent();
        }

        private void cancelBtn_Click(object sender, RoutedEventArgs e)
        {

        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox txtField = sender as TextBox;
            if (!string.IsNullOrEmpty(txtField.Text) && txtField.Text.Contains(" "))
            {
                int oldsele = txtField.SelectionStart;
                int oldlength = txtField.Text.Length;
                txtField.Text = txtField.Text.Replace(" ", "");

                txtField.SelectionStart = oldsele;//Math.Abs(EmailText.txtControl.Text.Length - oldlength) + oldsele;
            }
        }
    }
}
