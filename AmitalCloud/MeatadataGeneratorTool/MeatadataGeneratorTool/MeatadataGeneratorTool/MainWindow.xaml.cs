using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;

namespace MeatadataGeneratorTool
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void addBtn_Click(object sender, RoutedEventArgs e)
        {
            ObjectTableViewModel model = new ObjectTableViewModel();
            model.ErrorsVisibility = Visibility.Collapsed;

            ObjectTableControl control = new ObjectTableControl();
            control.DataContext = model;
            control.Show();            
        }

        private void editBtn_Click(object sender, RoutedEventArgs e)
        {
            ObjectTableViewModel model = new ObjectTableViewModel()
            {
                ObjectTableName = "Shipment",
                DBTableName = "Shipments",
                ObjectTableSingular = "Shipment",
                ObjectTablePlural = "Shipments",
                LookUp1 = "FieldName",
                LookUp2 = "MinLength",
                KeyPropertyPath = "Id",
                ObjectTableTypeCode = "Master Data",
                MaxNumberOfCustomFields = 15,
                SortingByObjectField = "FieldName",
                IsNewWizard = true,
                IsMain = true,
                EnableSecurity = true,
            };
            List<ObjectFieldsViewModel> ObsList = new List<ObjectFieldsViewModel>();

            ObsList.Add(new ObjectFieldsViewModel(model,false) { FieldName = "FromPortId", FieldDataType = "LookUp", MinLength = 0, MaxLength = 50, LookUpTableName="Port",});
            ObsList.Add(new ObjectFieldsViewModel(model,false) { FieldName = "IsCustomerView", FieldDataType = "Boolean", });
            ObsList.Add(new ObjectFieldsViewModel(model,false) { FieldName = "Quantity", FieldDataType = "Integer",});
            ObsList.Add(new ObjectFieldsViewModel(model,false) { FieldName = "ShipmentNumber", FieldDataType = "Text", MinLength = 0, MaxLength = 15, SystemMaxLength = 20 });
            ObsList.Add(new ObjectFieldsViewModel(model,false) { FieldName = "Arrived Date", FieldDataType = "DateTime", IsTimeFrameFilter = true,});
            ObsList.Add(new ObjectFieldsViewModel(model,false) { FieldName = "UnitPrice", FieldDataType = "Double", DigitsAfterPoint = 2, DisplayOnLookUp = true, DisplayInLookUpIndex = 1, DisplayInLookupColumnSize = "120"});

            model.BuildObsList(ObsList);

            ObjectTableControl control = new ObjectTableControl();
            control.DataContext = model;
            control.Show(); 
        }
    }
}
