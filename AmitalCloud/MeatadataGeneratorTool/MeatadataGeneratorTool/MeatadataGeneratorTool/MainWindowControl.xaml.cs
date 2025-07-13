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
    /// Interaction logic for MainWindowControl.xaml
    /// </summary>
    public partial class MainWindowControl : Window
    {
        public MainWindowControl()
        {
            InitializeComponent();
            this.Closing += MainWindowControl_Closing;
        }

        void MainWindowControl_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Environment.Exit(0);
        }
        private void cancelBtn_Click(object sender, RoutedEventArgs e)
        {
            //ObjectTableViewModel viewModel = this.DataContext as ObjectTableViewModel;
            //MessageBoxResult result = MessageBox.Show("Do you want to save your changes?", "Save Changes", MessageBoxButton.YesNoCancel);
            //if (result == MessageBoxResult.Yes)
            //{
            //    viewModel.SaveChanges();
            //}
            //else
            //{
            //    if (result != MessageBoxResult.Cancel)
            //    {
                    Environment.Exit(0);
            //        this.Close();
            //    }
            //}
        }
    }
}
