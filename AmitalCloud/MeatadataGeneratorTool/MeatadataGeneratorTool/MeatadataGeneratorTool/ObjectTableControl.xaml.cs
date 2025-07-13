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
using System.Windows.Shapes;

namespace MeatadataGeneratorTool
{
    /// <summary>
    /// Interaction logic for ObjectTableControl.xaml
    /// </summary>
    public partial class ObjectTableControl : Window
    {
        public ObjectTableControl()
        { 
            InitializeComponent();//

            this.Closing += ObjectTableControl_Closing;
          
           
        }

        void ObjectTableControl_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {

            System.Windows.Application.Current.Shutdown();
            Environment.Exit(0);
            //ObjectTableViewModel viewModel = this.DataContext as ObjectTableViewModel;
            //MessageBoxResult result = MessageBox.Show("Do you want to save your changes?", "Save Changes", MessageBoxButton.YesNo);
            //if (result == MessageBoxResult.Yes)
            //{
            //    if(!viewModel.SaveChanges())
            //    {
            //        e.Cancel = true;
            //    }
            //}
            //else
            //{
            //   // Environment.Exit(0);
            //  //  this.Close();
            //}
        }
        
        private void cancelBtn_Click(object sender, RoutedEventArgs e)
        {
            ObjectTableViewModel viewModel = this.DataContext as ObjectTableViewModel;
            MessageBoxResult result = MessageBox.Show("Do you want to save your changes?", "Save Changes", MessageBoxButton.YesNoCancel);
            if (result == MessageBoxResult.Yes)
            {
                viewModel.SaveChanges();
            }
            else
            {
                if (result != MessageBoxResult.Cancel)
                {
                    Environment.Exit(0);
                    this.Close();
                }
            }
        }

        private void okBtn_Click(object sender, RoutedEventArgs e)
        {
            
        }

        private void btnDeleteLook1_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            cmbLook1.SelectedItem = null;
        }

        private void btnDeleteLook2_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            cmbLook2.SelectedItem = null;
        }

        private void btnRemove_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            if (btn.DataContext != null)
            {
                ObjectFieldsViewModel selected = btn.DataContext as ObjectFieldsViewModel;
                ObjectTableViewModel viewModel = this.DataContext as ObjectTableViewModel;
                if (viewModel != null && selected != null)
                {
                    selected.ErrorsVisibility = Visibility.Collapsed;
                    viewModel.RemoveFromListMethod(selected);
                }
            }
        }

        private void btnRemove_Click_Local(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            if (btn.DataContext != null)
            {
                ObjectFieldsViewModel selected = btn.DataContext as ObjectFieldsViewModel;
                ObjectTableViewModel viewModel = this.DataContext as ObjectTableViewModel;
                if (viewModel != null && selected != null)
                {
                    selected.ErrorsVisibility = Visibility.Collapsed;
                    viewModel.RemoveFromLocalListMethod(selected);
                }
            }
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }




        //private void dgLookupFields_Sorting(object sender, DataGridSortingEventArgs e)
        //{

        //    int i = 0;
        //    foreach (ObjectFieldsViewModel field in dgLookupFields.Items)
        //    {
        //        field.DisplayInLookUpIndex = i;
        //        i++;
        //    }
        //  //  foreach( dgLookupFields.Items.

        //}
    }
}
