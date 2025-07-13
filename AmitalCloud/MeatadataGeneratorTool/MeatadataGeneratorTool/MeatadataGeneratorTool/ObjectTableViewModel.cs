using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using GalaSoft.MvvmLight.Command;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System;
using MeatadataGeneratorTool.QueryModule;
using MeatadataGeneratorTool.ScreensModule;
using MeatadataGeneratorTool.TabsModule;
using System.Windows.Controls;
using System.ComponentModel;
using System.Windows.Data;
using System.Windows.Media;
using MeatadataGeneratorTool.CloseTablesData;
using MeatadataGeneratorTool.Helpers;
using System.Windows.Documents;
using MeatadataGeneratorTool.EventTypes;
using MeatadataGeneratorTool.MenuButtons;
using MeatadataGeneratorTool.DataContractsModule;
using MeatadataGeneratorTool.TextCodes;
using MeatadataGeneratorTool.Features;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;

namespace MeatadataGeneratorTool
{
    public class ObjectTableType
    {
        public string Code { get; set; }
        public string Name { get; set; }
    }

    public class DxmlDatabaseType
    {
        public string Code { get; set; }
        public string Name { get; set; }
    }

    public class DxmlDatabaseSchema
    {
        public string Code { get; set; }
        public string Name { get; set; }
    }

    public class ObjectTableViewModel : PropertyChangedImplementation
    {

        public List<ObjectTableType> ObjectTableTypes { get; set; }

        public List<DxmlDatabaseType> DxmlDatabaseTypes { get; set; }

        public List<DxmlDatabaseSchema> DxmlDatabaseSchemas { get; set; }

        public ObservableCollection<ObjectFieldsViewModel> ObsList { get; set; }

        //public ObservableCollection<DataContractFieldViewModel> DCFieldsObsList { get; set; }


        public ObservableCollection<ObjectFieldsViewModel> TempObsList { get; set; }
        public ObservableCollection<ScreensViewModel> ScreensObsList { get; set; }
        public ObservableCollection<TabsViewModel> TabsObsList { get; set; }
        public ObservableCollection<MenuButtonViewModel> MenuButtonsObsList { get; set; }
        public ObservableCollection<DataContractViewModel> DataContractsObsList { get; set; }
        //public ObservableCollection<MenuButtonViewModel> AllMenuButtonsObsList
        //{
        //    get
        //    {
        //        if (MenuButtonsObsList != null)
        //        {
        //            var temp = MenuButtonsObsList.Where(a => a.SelectedMenuButtonType != "menuitem");
        //            ObservableCollection<MenuButtonViewModel> temp1 = new ObservableCollection<MenuButtonViewModel>();
        //            foreach (var item in temp)
        //            {
        //                temp1.Add(item);
        //            }
        //            return temp1;
        //        }
        //        else
        //        {
        //            return new ObservableCollection<MenuButtonViewModel>();
        //        }

        //    }

        //}

        //private ObservableCollection<MenuButtonViewModel> subMenuButtonsObsList;
        //public ObservableCollection<MenuButtonViewModel> SubMenuButtonsObsList
        //{ 
        //    get
        //    {
        //        if (MenuButtonsObsList != null)
        //        {
        //            var temp = MenuButtonsObsList.Where(a => a.SelectedMenuButtonType == "menuitem" && a.ParentMenuButton == SelectedMenuButton);
        //            ObservableCollection<MenuButtonViewModel> temp1 = new ObservableCollection<MenuButtonViewModel>();
        //            foreach (var item in temp)
        //            {
        //                temp1.Add(item);
        //            }
        //            return temp1;
        //        }
        //        else
        //        {
        //            return new ObservableCollection<MenuButtonViewModel>(); 
        //        }
        //    }

        //}
        private ObservableCollection<EventTypesViewModel> eventTypesObsList;
        public ObservableCollection<EventTypesViewModel> EventTypesObsList
        {
            get;
            set;
            //get
            //{
            //    if (eventTypesObsList == null || eventTypesObsList.Count == 0)
            //    {
            //        eventTypesObsList = new ObservableCollection<EventTypesViewModel>();
            //        eventTypesObsList.Add(new EventTypesViewModel(this, true)
            //        {
            //            Code = "CREV",
            //            EnglishName = "Created",
            //            LocalName = "Created",
            //            ShortView = true,
            //            IsManualEntry = false
            //        });
            //        eventTypesObsList.Add(new EventTypesViewModel(this,true) {
            //        Code = "UPEV",
            //        EnglishName = "Updated",
            //        LocalName = "Updated", 
            //        ShortView = false,
            //        IsManualEntry = false 
            //        }); 
            //    }
            //    return eventTypesObsList;
            //}
            //set
            //{
            //    eventTypesObsList = value;
            //    FirePropertyChanged("EventTypesObsList");
            //}
        }


        private ObservableCollection<TextCodesViewModel> additionalTextCodesList;
        public ObservableCollection<TextCodesViewModel> AdditionalTextCodesList
        {
            get;
            set;
           
        }

        private ObservableCollection<FeaturesViewModel> additionalFeaturesList;
        public ObservableCollection<FeaturesViewModel> AdditionalFeaturesList
        {
            get;
            set;

        }

	 
		public ObservableCollection<TextCodesViewModel> AdditionalTextCodesTempList
		{
			get;
			set;

		}

	 
		public ObservableCollection<FeaturesViewModel> AdditionalFeaturesTempList
		{
			get;
			set;

		}


		public ObjectFieldsControl fieldsControl;
        public ObservableCollection<ObjectFieldsViewModel> DisplayLookUpFieldsList { get; set; }
        public ObservableCollection<ObjectFieldsViewModel> DisplayLocalLookUpFieldsList { get; set; }
        public ObservableCollection<ObjectFieldsViewModel> ToBeDisplayOnLookUpList { get; set; }
        public ObservableCollection<ObjectFieldsViewModel> ToBeDisplayOnLookUpLocalList { get; set; }
        public ObservableCollection<Row> rows { get; set; }
        public Dictionary<string, string> FieldsDictionary = new Dictionary<string, string>();
        public ObservableCollection<QueryViewModel> queriesObsList;
        public ObservableCollection<QueryViewModel> QueriesObsList
        {
            get
            {
                if (queriesObsList == null)
                {
                    queriesObsList = new ObservableCollection<QueryViewModel>();
                }
                return queriesObsList;
            }
            set
            {
                queriesObsList = value;
                FirePropertyChanged("QueriesObsList");
            }
        }

        private ObservableCollection<QueryColumnsViewModel> queryColumnObsList;
        public ObservableCollection<QueryColumnsViewModel> QueryColumnObsList
        {
            get
            {
                return queryColumnObsList;
            }
            set
            {
                queryColumnObsList = value;
                FirePropertyChanged("QueryColumnObsList");
            }
        }

        private ObservableCollection<QueryFiltersViewModel> queryFiltersObsList;
        public ObservableCollection<QueryFiltersViewModel> QueryFiltersObsList
        {
            get
            {
                return queryFiltersObsList;
            }
            set
            {
                queryFiltersObsList = value;
                FirePropertyChanged("QueryFiltersObsList");
            }
        }

        public string objectFieldsFilter;
        public string ObjectFieldsFilter
        {
            get
            {
                return objectFieldsFilter;
            }
            set
            {
                objectFieldsFilter = value;
                FirePropertyChanged("ObjectFieldsFilter");
            }
        }


		public string additionalTextCodesFilter;
		public string AdditionalTextCodesFilter
		{
			get
			{
				return additionalTextCodesFilter;
			}
			set
			{
				additionalTextCodesFilter = value;
				FirePropertyChanged("AdditionalTextCodesFilter");
			}
		}

		public string additionalFeaturesFilter;
		public string AdditionalFeaturesFilter
		{
			get
			{
				return additionalFeaturesFilter;
			}
			set
			{
				additionalFeaturesFilter = value;
				FirePropertyChanged("AdditionalFeaturesFilter");
			}
		}



		public string dCFieldsFilter;
        public string DCFieldsFilter
        {
            get
            {
                return dCFieldsFilter;
            }
            set
            {
                dCFieldsFilter = value;
                FirePropertyChanged("DCFieldsFilter");
            }
        }


		public RelayCommand<string> AdditionalTextCodesFilterTextChanged
		{
			get { return new RelayCommand<string>(i => this.AdditionalTextCodesFilterTextChangedMethod(i)); }
			set { }
		}



		private void AdditionalTextCodesFilterTextChangedMethod(string filter)
		{

			AdditionalTextCodesTempList = new ObservableCollection<TextCodesViewModel>();
			var temp = AdditionalTextCodesList.Where(a => a.Code.ToLower().Contains(filter.ToLower())).ToList();
			foreach (var item in temp)
			{
				AdditionalTextCodesTempList.Add(item);
			}

			FirePropertyChanged("AdditionalTextCodesTempList");

			this.SelectedTextCode = AdditionalTextCodesTempList.FirstOrDefault();
		}

		public RelayCommand<string> AdditionalFeaturesFilterTextChanged
		{
			get { return new RelayCommand<string>(i => this.AdditionalFeaturesFilterTextChangedMethod(i)); }
			set { }
		}



		private void AdditionalFeaturesFilterTextChangedMethod(string filter)
		{

			AdditionalFeaturesTempList = new ObservableCollection<FeaturesViewModel>();
			var temp = AdditionalFeaturesList.Where(a => a.Code.ToLower().Contains(filter.ToLower())).ToList();
			foreach (var item in temp)
			{
				AdditionalFeaturesTempList.Add(item);
			}

			FirePropertyChanged("AdditionalFeaturesTempList");
			this.SelectedFeature = AdditionalFeaturesTempList.FirstOrDefault();
		}


		private void DCFieldsFilterTextChangedMethod(string filter)
        {

            SelectedDataContract.DBFieldsTempObsList = new ObservableCollection<ObjectFieldsViewModel>();
            var temp = SelectedDataContract.DBFieldsObsList.Where(a => a.FieldName.ToLower().Contains(filter.ToLower())).ToList();
            foreach (var item in temp)
            {
                //if (SelectedObjectField != null)
                //{
                //    SelectedObjectField.ErrorsVisibility = Visibility.Collapsed;
                //}
                //item.ErrorsVisibility = Visibility.Collapsed;
                SelectedDataContract.DBFieldsTempObsList.Add(item);
            }
        }
        public RelayCommand<string> DCFieldsFilterTextChanged
        {
            get { return new RelayCommand<string>(i => this.DCFieldsFilterTextChangedMethod(i)); }
            set { }
        }

        private void ObjectFieldsFilterTextChangedMethod(string filter)
        {

            TempObsList.Clear();
            var temp = ObsList.Where(a => a.FieldName.ToLower().Contains(filter.ToLower())).ToList();
            foreach (var item in temp)
            {
                if (SelectedObjectField != null)
                {
                    SelectedObjectField.ErrorsVisibility = Visibility.Collapsed;
                }
                item.ErrorsVisibility = Visibility.Collapsed;
                TempObsList.Add(item);
            }
        }
        public RelayCommand<string> ObjectFieldsFilterTextChanged
        {
            get { return new RelayCommand<string>(i => this.ObjectFieldsFilterTextChangedMethod(i)); }
            set { }
        }

        Visibility fieldsEditControlVisibility;

        public Visibility FieldsEditControlVisibility
        {
            get { return fieldsEditControlVisibility; }
            set { fieldsEditControlVisibility = value; FirePropertyChanged("FieldsEditControlVisibility"); }
        }
        public ObjectTableViewModel()
        {
            FieldsEditControlVisibility = Visibility.Collapsed;
            ObsList = new ObservableCollection<ObjectFieldsViewModel>();
            //DBFieldsObsList = new ObservableCollection<ObjectFieldsViewModel>();
            //DCFieldsObsList = new ObservableCollection<DataContractFieldViewModel>();
            TempObsList = new ObservableCollection<ObjectFieldsViewModel>();
            DisplayLookUpFieldsList = new ObservableCollection<ObjectFieldsViewModel>();
            DisplayLocalLookUpFieldsList = new ObservableCollection<ObjectFieldsViewModel>();
            ToBeDisplayOnLookUpList = new ObservableCollection<ObjectFieldsViewModel>();
            ToBeDisplayOnLookUpLocalList = new ObservableCollection<ObjectFieldsViewModel>();
            ObjectTableTypes = new List<ObjectTableType>() { new ObjectTableType { Code = "MD", Name = "Master Data" }, new ObjectTableType() { Code = "BR", Name = "Business Record" } };
            DxmlDatabaseTypes = new List<DxmlDatabaseType>() {
                new DxmlDatabaseType { Code = "Main", Name = "Main Database" },
                new DxmlDatabaseType { Code = "Global", Name = "Global Database" },
                new DxmlDatabaseType() { Code = "SystemLogs", Name = "SystemLogs Database" },
                new DxmlDatabaseType() { Code = "CargoTracking", Name = "CargoTracking Database" } };
            DxmlDatabaseSchemas = new List<DxmlDatabaseSchema>() {
                new DxmlDatabaseSchema { Code = "dbo", Name = "Dbo Schema" },
                new DxmlDatabaseSchema() { Code = "Customs", Name = "Customs Schema" } };
            this.AdditionalTextCodesList = new ObservableCollection<TextCodesViewModel>();
            this.AdditionalFeaturesList = new ObservableCollection<FeaturesViewModel>();
			this.AdditionalFeaturesTempList = new ObservableCollection<FeaturesViewModel>();
			this.AdditionalTextCodesTempList = new ObservableCollection<TextCodesViewModel>();
        }

        public void BuildObsList(List<ObjectFieldsViewModel> fields)
        {
            ObsList.Clear();
            //DBFieldsObsList.Clear();

            foreach (ObjectFieldsViewModel item in fields)
            {
                item.Length = item.FieldName.Length;
                ObsList.Add(item);
                //if (item.IsDBField)
                //{
                //    DBFieldsObsList.Add(item);
                //}
                item.SetControlFieldsList(item);
            }

            foreach (ObjectFieldsViewModel item in ObsList)
            {
                item.SetControlFieldsList(item);
            }

            this.SetLookUpFieldsList();


            FieldsEditControlVisibility = (ObsList.Count == 0) ? Visibility.Collapsed : Visibility.Visible;

            this.SelectedObjectField = ObsList.FirstOrDefault();
            //this.SelectedDBField = DBFieldsObsList.FirstOrDefault();
            TempObsList.Clear();
            foreach (var item in ObsList)
            {
                TempObsList.Add(item);
            }

        }

        public void BuildQueriesObsList(List<QueryViewModel> Queries)
        {
            QueriesObsList.Clear();

            foreach (var item in Queries)
            {
                QueryGroupCode = item.QueryGroupCode;
                QueriesObsList.Add(item);
                //QueryColumnObsList = item.QueryColumnObsList;
            }
            if (Queries.Count == 0)
            {
                QueryGroupCode = Guid.NewGuid().ToString().Substring(0, 4);
            }



            QueryDetailsEditControlVisibility = (QueriesObsList.Count == 0) ? Visibility.Collapsed : Visibility.Visible;

            this.SelectedQuery = QueriesObsList.FirstOrDefault();

        }

        public void BuildScreensObsList(List<ScreensViewModel> Screens)
        {
            if (ScreensObsList == null)
            {
                ScreensObsList = new ObservableCollection<ScreensViewModel>();
                ScreensObsList.Add(new ScreensViewModel(this, true)
                {
                    Name = this.ObjectTableName + "HeaderScreen",
                    IsHeaderScreen = true,
                    IsReadOnly = true,
                    ObjectTableName = this.ObjectTableName
                });
            }
            if (Screens != null && Screens.Count > 0)
            {
                ScreensObsList.Clear();
            }
            foreach (var item in Screens)
            {
                item.ScreenDetailsVisibility = Visibility.Visible;
                ScreensObsList.Add(item);
            }

            ScreensEditControlVisibility = (ScreensObsList.Count == 0) ? Visibility.Collapsed : Visibility.Visible;

            this.SelectedScreen = ScreensObsList.FirstOrDefault();
            if (SelectedScreen != null)
            {
                this.SelectedScreen.ScreenDetailsVisibility = (ScreensObsList.Count == 0) ? Visibility.Collapsed : Visibility.Visible;
            }

        }

        public void BuildTabsObsList(List<TabsViewModel> Tabs)
        {
            if (TabsObsList == null)
            {
                TabsObsList = new ObservableCollection<TabsViewModel>();
            }
            TabsObsList.Clear();

            int index = 0;
            foreach (var item in Tabs.OrderBy(t => t.IndexOrder))
            {
                TabsObsList.Insert(index++, item);
            }



            TabsEditControlVisibility = (TabsObsList.Count == 0) ? Visibility.Collapsed : Visibility.Visible;

            this.SelectedTab = TabsObsList.FirstOrDefault();
            if (SelectedTab != null)
            {
                // this.SelectedTab.ScreenDetailsVisibility = (ScreensObsList.Count == 0) ? Visibility.Collapsed : Visibility.Visible;
            }

        }

        public void BuildMenuButtonsObsList(List<MenuButtonViewModel> MenuButtons)
        {
            if (MenuButtonsObsList == null)
            {
                MenuButtonsObsList = new ObservableCollection<MenuButtonViewModel>();
            }
            MenuButtonsObsList.Clear();

            int index = 0;
            foreach (var item in MenuButtons.OrderBy(m=>m.IndexOrder))
            {
                MenuButtonsObsList.Insert(index, item);
                index++;
            }



            MenuButtonsEditControlVisibility = (MenuButtonsObsList.Count == 0) ? Visibility.Collapsed : Visibility.Visible;

            this.SelectedMenuButton = MenuButtonsObsList.FirstOrDefault();
            if (SelectedMenuButton != null)
            {
                // this.SelectedTab.ScreenDetailsVisibility = (ScreensObsList.Count == 0) ? Visibility.Collapsed : Visibility.Visible;
            }
            //FirePropertyChanged("SubMenuButtonsObsList");
            //FirePropertyChanged("AllMenuButtonsObsList");

        }

        public void BuildEventTypesObsList(List<EventTypesViewModel> EventTypes)
        {
            if (EventTypesObsList == null)
            {
                EventTypesObsList = new ObservableCollection<EventTypesViewModel>();
                EventTypesObsList.Add(new EventTypesViewModel(this, true)
                {
                    Code = "CREV",
                    EnglishName = "Created",
                    LocalName = "Created",
                    ShortView = true,
                    IsManualEntry = false
                });
                EventTypesObsList.Add(new EventTypesViewModel(this, true)
                {
                    Code = "UPEV",
                    EnglishName = "Updated",
                    LocalName = "Updated",
                    ShortView = false,
                    IsManualEntry = false
                });
            }
            if (EventTypes != null && EventTypes.Count > 0)
            {
                EventTypesObsList.Clear();
            }
            foreach (var item in EventTypes)
            {
                EventTypesObsList.Add(item);
            }



            EventTypesEditControlVisibility = (EventTypesObsList.Count == 0) ? Visibility.Collapsed : Visibility.Visible;

            this.SelectedEventType = EventTypesObsList.FirstOrDefault();

        }

        public void BuildAdditionalTextCodesList(List<TextCodesViewModel> textCodes)
        {
            if (AdditionalTextCodesList == null)
            {
                AdditionalTextCodesList = new ObservableCollection<TextCodesViewModel>();
				AdditionalTextCodesTempList = new ObservableCollection<TextCodesViewModel>();
			}
            if (AdditionalTextCodesList != null && AdditionalTextCodesList.Count > 0)
            {
                AdditionalTextCodesList.Clear();
				AdditionalTextCodesTempList.Clear();

			}
            foreach (var item in textCodes)
            {
                AdditionalTextCodesList.Add(item);
				AdditionalTextCodesTempList.Add(item);
            }



            TextCodesEditControlVisibility = (AdditionalTextCodesTempList.Count == 0) ? Visibility.Collapsed : Visibility.Visible;

            this.SelectedTextCode = AdditionalTextCodesTempList.FirstOrDefault();

        }

        public void BuildAdditionalFeaturesList(List<FeaturesViewModel> features)
        {
            if (AdditionalFeaturesList == null)
            {
                AdditionalFeaturesList = new ObservableCollection<FeaturesViewModel>();
				AdditionalFeaturesTempList = new ObservableCollection<FeaturesViewModel>();

            }
            if (AdditionalFeaturesList != null && AdditionalFeaturesList.Count > 0)
            {
                AdditionalFeaturesList.Clear();
				AdditionalFeaturesTempList.Clear();

			}
            foreach (var item in features)
            {
                AdditionalFeaturesList.Add(item);
				AdditionalFeaturesTempList.Add(item);
			}



            FeaturesEditControlVisibility = (AdditionalFeaturesTempList.Count == 0) ? Visibility.Collapsed : Visibility.Visible;

            this.SelectedFeature = AdditionalFeaturesTempList.FirstOrDefault();

        }

        public void BuildDataContractsObsList(List<DataContractViewModel> DataContracts)
        {
            if (DataContractsObsList == null)
            {
                DataContractsObsList = new ObservableCollection<DataContractViewModel>();

            }
            if (DataContractsObsList != null && DataContractsObsList.Count > 0)
            {
                DataContractsObsList.Clear();
            }
            foreach (var item in DataContracts)
            {
                //if (item.DCFieldsObsList == null)
                //{
                //    item.DCFieldsObsList = new ObservableCollection<DataContractFieldViewModel>();
                //}
                if (item.DBFieldsTempObsList.Count == 0 && item.DBFieldsObsList.Count > 0)
                {
                    item.DBFieldsTempObsList = item.DBFieldsObsList;
                }
                foreach (var item1 in item.DCFieldsObsList)
                {
                    var temp = ObsList.Where(a => a.FieldName == item1.FieldName).FirstOrDefault();
                    item.DBFieldsTempObsList.Remove(temp);
                }
                //item.DBFieldsObsList = new ObservableCollection<ObjectFieldsViewModel>();
                //foreach (var item0 in temp)
                //{
                //    item.DBFieldsObsList.Add(item0);
                //} 
                DataContractsObsList.Add(item);
            }



            //EventTypesEditControlVisibility = (EventTypesObsList.Count == 0) ? Visibility.Collapsed : Visibility.Visible;
            if (DataContractsObsList.Count > 0)
            {
                this.SelectedDataContract = DataContractsObsList.FirstOrDefault();
            }


        }



        public void BuildRowsData(List<Row> Rows)
        {
            if (rows == null)
            {
                rows = new ObservableCollection<Row>();
            }
            rows.Clear();

            if (CLoseTableDataGrid.Columns.Count == 0)
            {

                foreach (var objectField in this.ObsList)
                {
                    var TempColumn = new DataGridTextColumn() { MinWidth = 120 };
                    TempColumn.Header = objectField.FieldName;
                    Binding bind = new Binding();
                    bind.Mode = BindingMode.OneWay;

                    bind.Converter = new RowIndexConverter();
                    bind.ConverterParameter = objectField.FieldName;
                    TempColumn.Binding = bind;
                    CLoseTableDataGrid.Columns.Add(TempColumn);
                }
               var itemWithMaxColumns = Rows.OrderByDescending(r => r._data.Count).FirstOrDefault();
                //if (itemWithMaxColumns != null)
                //{
                //    foreach (var xx in itemWithMaxColumns._data)
                //    {
                //        var TempColumn = new DataGridTextColumn() { MinWidth = 120 };
                //        TempColumn.Header = xx.Key;
                //        Binding bind = new Binding();
                //        bind.Mode = BindingMode.OneWay;
                      
                //        bind.Converter = new RowIndexConverter();
                //        bind.ConverterParameter = xx.Key;
                //        TempColumn.Binding = bind;
                //        CLoseTableDataGrid.Columns.Add(TempColumn);
                //    }
                //}

                var missingDataRows = Rows.Where(r => r._data.Count < itemWithMaxColumns._data.Count);
                foreach(var item in missingDataRows)
                {
                    var list =  itemWithMaxColumns._data.Where(d => !item._data.ContainsKey(d.Key)).ToList();
                    foreach(var missedData in list)
                    {
                        item._data.Add(missedData.Key, null);
                    }
                    //item._data.Add()
                }
            }

            foreach (var item in Rows)
            {
                rows.Add(item);
                //if (CLoseTableDataGrid.Columns.Count == 0)
                //{
                //    foreach (var xx in item._data)
                //    {
                //        var TempColumn = new DataGridTextColumn() { MinWidth = 120 };
                //        TempColumn.Header = xx.Key;
                //        Binding bind = new Binding();
                //        bind.Mode = BindingMode.OneWay;
                //        bind.Converter = new RowIndexConverter();
                //        bind.ConverterParameter = xx.Key;
                //        TempColumn.Binding = bind;
                //        CLoseTableDataGrid.Columns.Add(TempColumn);
                //    }
                //}
            }

            CLoseTableDataGrid.ItemsSource = rows;
        }

        void button_Click(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        public void UpdateObsList(ObjectFieldsViewModel item)
        {
            ObsList.Add(item);
            //if ( item.IsDBField)
            //{
            //     DBFieldsObsList.Add(item);
            //}
            TempObsList.Add(item);

            FirePropertyChanged("ObsList");
            //FirePropertyChanged("DBFieldsObsList");
            FirePropertyChanged("TempObsList");
            this.SetLookUpFieldsList();

            FieldsEditControlVisibility = (ObsList.Count == 0) ? Visibility.Collapsed : Visibility.Visible;

            this.SelectedObjectField = item;
        }
        Visibility screensEditControlVisibility;

        public Visibility ScreensEditControlVisibility
        {
            get { return screensEditControlVisibility; }
            set { screensEditControlVisibility = value; FirePropertyChanged("ScreensEditControlVisibility"); }
        }

        Visibility tabsEditControlVisibility;

        public Visibility TabsEditControlVisibility
        {
            get { return tabsEditControlVisibility; }
            set { tabsEditControlVisibility = value; FirePropertyChanged("TabsEditControlVisibility"); }
        }

        Visibility menubuttonsEditControlVisibility;

        public Visibility MenuButtonsEditControlVisibility
        {
            get { return menubuttonsEditControlVisibility; }
            set { menubuttonsEditControlVisibility = value; FirePropertyChanged("MenuButtonsEditControlVisibility"); }
        }

        Visibility eventTypesEditControlVisibility;
        public Visibility EventTypesEditControlVisibility
        {
            get { return eventTypesEditControlVisibility; }
            set { eventTypesEditControlVisibility = value; FirePropertyChanged("EventTypesEditControlVisibility"); }
        }

        Visibility textCodesEditControlVisibility;
        public Visibility TextCodesEditControlVisibility
        {
            get { return textCodesEditControlVisibility; }
            set { textCodesEditControlVisibility = value; FirePropertyChanged("TextCodesEditControlVisibility"); }
        }


        Visibility featuresEditControlVisibility;
        public Visibility FeaturesEditControlVisibility
        {
            get { return featuresEditControlVisibility; }
            set { featuresEditControlVisibility = value; FirePropertyChanged("FeaturesEditControlVisibility"); }
        }

        public void UpdateScreensObsList(ScreensViewModel item)
        {
            if (ScreensObsList == null)
            {
                ScreensObsList = new ObservableCollection<ScreensViewModel>();
            }
            ScreensObsList.Add(item);

            FirePropertyChanged("ScreensObsList");

            ScreensEditControlVisibility = (ScreensObsList.Count == 0) ? Visibility.Collapsed : Visibility.Visible;

            this.SelectedScreen = item;
        }
        ScreensViewModel selectedScreen;

        public ScreensViewModel SelectedScreen
        {
            get { return selectedScreen; }
            set { selectedScreen = value; FirePropertyChanged("SelectedScreen"); }
        }

        public void UpdateTabsObsList(TabsViewModel item)
        {
            if (TabsObsList == null)
            {
                TabsObsList = new ObservableCollection<TabsViewModel>();
            }
            TabsObsList.Add(item);

            FirePropertyChanged("TabsObsList");

            TabsEditControlVisibility = (TabsObsList.Count == 0) ? Visibility.Collapsed : Visibility.Visible;

            this.SelectedTab = item;
        }

        public void UpdateMenuButtonsObsList(MenuButtonViewModel item)
        {
            if (MenuButtonsObsList == null)
            {
                MenuButtonsObsList = new ObservableCollection<MenuButtonViewModel>();
            }
            MenuButtonsObsList.Add(item);

            FirePropertyChanged("MenuButtonsObsList");

            MenuButtonsEditControlVisibility = (MenuButtonsObsList.Count == 0) ? Visibility.Collapsed : Visibility.Visible;
            //if (item.ParentMenuButton == null)
            //{
            //    this.SelectedMenuButton = item;
            //}
            //else
            //{
            this.SelectedMenuItem = item;
            //    FirePropertyChanged("SelectedMenuButton");
            //}
            //FirePropertyChanged("SubMenuButtonsObsList");
            //FirePropertyChanged("AllMenuButtonsObsList");
        }

        public void UpdateEventTypesObsList(EventTypesViewModel item)
        {
            if (EventTypesObsList == null)
            {
                EventTypesObsList = new ObservableCollection<EventTypesViewModel>();
            }
            EventTypesObsList.Add(item);

            FirePropertyChanged("EventTypesObsList");

            EventTypesEditControlVisibility = (EventTypesObsList.Count == 0) ? Visibility.Collapsed : Visibility.Visible;

            this.SelectedEventType = item;
        }

        public void UpdateTextCodesList(TextCodesViewModel item)
        {
            if (AdditionalTextCodesList == null)
            {
                AdditionalTextCodesList = new ObservableCollection<TextCodesViewModel>();
				AdditionalTextCodesTempList = new ObservableCollection<TextCodesViewModel>();

			}
            AdditionalTextCodesList.Add(item);
			AdditionalTextCodesTempList.Add(item);

			FirePropertyChanged("AdditionalTextCodesList");
			FirePropertyChanged("AdditionalTextCodesTempList");

			TextCodesEditControlVisibility = (AdditionalTextCodesTempList.Count == 0) ? Visibility.Collapsed : Visibility.Visible;

            this.SelectedTextCode = item;
        }

        public void UpdateFeaturesList(FeaturesViewModel item)
        {
            if (AdditionalFeaturesList == null)
            {
                AdditionalFeaturesList = new ObservableCollection<FeaturesViewModel>();
				AdditionalFeaturesTempList = new ObservableCollection<FeaturesViewModel>();

			}
            AdditionalFeaturesList.Add(item);
			AdditionalFeaturesTempList.Add(item);

			FirePropertyChanged("AdditionalFeaturesList");
			FirePropertyChanged("AdditionalFeaturesTempList");

			FeaturesEditControlVisibility = (AdditionalFeaturesTempList.Count == 0) ? Visibility.Collapsed : Visibility.Visible;

            this.SelectedFeature = item;
        }



        public void UpdateDataContractsObsList(DataContractViewModel item)
        {
            if (DataContractsObsList == null)
            {
                DataContractsObsList = new ObservableCollection<DataContractViewModel>();
            }

            DataContractsObsList.Add(item);

            FirePropertyChanged("DataContractsObsList");

            //EventTypesEditControlVisibility = (EventTypesObsList.Count == 0) ? Visibility.Collapsed : Visibility.Visible;

            this.SelectedDataContract = item;
        }


        TabsViewModel selectedTab;

        public TabsViewModel SelectedTab
        {
            get { return selectedTab; }
            set { selectedTab = value; FirePropertyChanged("SelectedTab"); }
        }



        public bool enableSubMenu = false;
        public bool EnableSubMenu
        {
            get
            {
                return enableSubMenu;
            }
            set
            {
                enableSubMenu = value;
                FirePropertyChanged("EnableSubMenu");
            }
        }

        

          

        MenuButtonViewModel selectedMenuItem;
        public MenuButtonViewModel SelectedMenuItem
        {
            get { return selectedMenuItem; }
            set
            {
                selectedMenuItem = value;
                FirePropertyChanged("SelectedMenuItem");
            }
        }

        MenuButtonViewModel selectedMenuButton;
        public MenuButtonViewModel SelectedMenuButton
        {
            get { return selectedMenuButton; }
            set
            {
                selectedMenuButton = value;
                if (selectedMenuButton != null)
                {
                    if (selectedMenuButton.SelectedMenuButtonType == "dropdownbutton")
                    {
                        //FirePropertyChanged("SubMenuButtonsObsList");
                        EnableSubMenu = true;
                    }
                    else
                    {
                        //FirePropertyChanged("SubMenuButtonsObsList");
                        EnableSubMenu = false;
                    }
                }
                FirePropertyChanged("SelectedMenuButton");

            }
        }

        EventTypesViewModel selectedEventType;
        public EventTypesViewModel SelectedEventType
        {
            get { return selectedEventType; }
            set { selectedEventType = value; FirePropertyChanged("SelectedEventType"); }
        }

        TextCodesViewModel selectedTextCode;
        public TextCodesViewModel SelectedTextCode
        {
            get { return selectedTextCode; }
            set { selectedTextCode = value; FirePropertyChanged("SelectedTextCode"); }
        }

        FeaturesViewModel selectedFeature;
        public FeaturesViewModel SelectedFeature
        {
            get { return selectedFeature; }
            set { selectedFeature = value; FirePropertyChanged("SelectedFeature"); }
        }

        DataContractViewModel selectedDataContract;
        public DataContractViewModel SelectedDataContract
        {
            get { return selectedDataContract; }
            set
            {
                selectedDataContract = value;
                value.FireDBFieldsObsList(true);
                FirePropertyChanged("SelectedDataContract");
            }
        }

        private void SetLookUpFieldsList()
        {

            DisplayLookUpFieldsList.Clear();
            DisplayLocalLookUpFieldsList.Clear();
            ToBeDisplayOnLookUpList.Clear();
            ToBeDisplayOnLookUpLocalList.Clear();
            foreach (ObjectFieldsViewModel item in ObsList.Where(d => d.FieldDataType != "LookUp" && d.DisplayOnLookUp))
            {
                DisplayLookUpFieldsList.Add(item);
            }

            foreach (ObjectFieldsViewModel item in ObsList.Where(d => d.FieldDataType != "LookUp" && !d.DisplayOnLookUp))
            {
                ToBeDisplayOnLookUpList.Add(item);
            }

            foreach (ObjectFieldsViewModel item in ObsList.Where(d => d.FieldDataType != "LookUp" && d.DisplayOnLookUpLocal))
            {
                DisplayLocalLookUpFieldsList.Add(item);
            }

            foreach (ObjectFieldsViewModel item in ObsList.Where(d => d.FieldDataType != "LookUp" && !d.DisplayOnLookUpLocal))
            {
                ToBeDisplayOnLookUpLocalList.Add(item);
            }

            DisplayLookUpFieldsList = new ObservableCollection<ObjectFieldsViewModel>(DisplayLookUpFieldsList.OrderBy(c => c.DisplayInLookUpIndex).ToList());
            DisplayLocalLookUpFieldsList=new ObservableCollection<ObjectFieldsViewModel>(DisplayLocalLookUpFieldsList.OrderBy(c=>c.DisplayInLookUpIndex).ToList());

            FirePropertyChanged("DisplayLookUpFieldsList");
            FirePropertyChanged("DisplayLocalLookUpFieldsList");
            FirePropertyChanged("ToBeDisplayOnLookUpList");
            FirePropertyChanged("ToBeDisplayOnLookUpLocalList");
        }

        public RelayCommand<ObjectFieldsViewModel> LookUpSelectionChanged
        {
            get { return new RelayCommand<ObjectFieldsViewModel>(i => this.LookUpSelectionChangedMethod(i)); }
            set { }
        }

        public ObjectFieldsViewModel selectedField;
        private void LookUpSelectionChangedMethod(ObjectFieldsViewModel item)
        {
            selectedField = item;
        }

        public RelayCommand AddToListCommand
        {
            get { return new RelayCommand(() => this.AddToListMethod()); }
        }


        private void AddToListMethod()
        {

            if (selectedField != null)
            {
                selectedField.DisplayOnLookUp = true;
                DisplayLookUpFieldsList.Add(selectedField);
                ToBeDisplayOnLookUpList.Remove(selectedField);
            }

            FirePropertyChanged("DisplayLookUpFieldsList");
            FirePropertyChanged("ToBeDisplayOnLookUpList");
            FirePropertyChanged("ObsList");
        }

        public void RemoveFromListMethod(ObjectFieldsViewModel selected)
        {

            if (selected != null)
            {
				selected.DisplayInSearchWindowList = false;
                selected.DisplayOnLookUp = false;
                ToBeDisplayOnLookUpList.Add(selected);
                DisplayLookUpFieldsList.Remove(selected);

                FirePropertyChanged("DisplayLookUpFieldsList");
                FirePropertyChanged("ToBeDisplayOnLookUpList");
                FirePropertyChanged("ObsList");
            }
        }

        /// <summary>
        /// to add a display on lookup local 
        /// </summary>
        public RelayCommand<ObjectFieldsViewModel> LookUpLocalSelectionChanged
        {
            get { return new RelayCommand<ObjectFieldsViewModel>(i => this.LookUpLocalSelectionChangedMethod(i)); }
            set { }
        }

        public ObjectFieldsViewModel selectedLocalField;
        private void LookUpLocalSelectionChangedMethod(ObjectFieldsViewModel item)
        {
            selectedLocalField = item;
            
           // MessageBox.Show(selectedLocalField.FieldName);
        }

        public RelayCommand AddToLocalListCommand
        {
            get { return new RelayCommand(() => this.AddToLocalListMethod()); }
        }


        private void AddToLocalListMethod()
        {

            if (selectedLocalField != null)
            {
                selectedLocalField.DisplayOnLookUpLocal = true;
                DisplayLocalLookUpFieldsList.Add(selectedLocalField);
                ToBeDisplayOnLookUpLocalList.Remove(selectedLocalField);
            }

            FirePropertyChanged("DisplayLocalLookUpFieldsList");
            FirePropertyChanged("ToBeDisplayOnLookUpLocalList");
            FirePropertyChanged("ObsList");
        }

        public void RemoveFromLocalListMethod(ObjectFieldsViewModel selected)
        {

            if (selected != null)
            {
				selected.DisplayInSearchWindowList = false;
				selected.DisplayOnLookUpLocal = false;
                ToBeDisplayOnLookUpLocalList.Add(selected);
                DisplayLocalLookUpFieldsList.Remove(selected);

                FirePropertyChanged("DisplayLocalLookUpFieldsList");
                FirePropertyChanged("ToBeDisplayOnLookUpLocalList");
                FirePropertyChanged("ObsList");
            }
        }

        //public RelayCommand<object> RemoveFromLookupCommand
        //{
        //    get { return new RelayCommand<object>(m => RemoveFromLookupCommand(m)); }
        //}

        //public void RemoveFromLookupCommand(object m)
        //{

        //}

        string objectTableName;
        [Required(ErrorMessage = "Field 'ObjectTableName' is required.")]
        public string ObjectTableName
        {
            get { return objectTableName; }
            set { objectTableName = value; FirePropertyChanged("ObjectTableName"); }
        }

        string parentObjectTableName;
        public string ParentObjectTableName
        {
            get { return parentObjectTableName; }
            set { parentObjectTableName = value; FirePropertyChanged("ParentObjectTableName"); }
        }

        string defaultText;

        public string DefaultText
        {
            get { return defaultText; }
            set { defaultText = value; FirePropertyChanged("DefaultText"); }
        }

        string localDefaultText;

        public string LocalDefaultText
        {
            get { return localDefaultText; }
            set { localDefaultText = value; FirePropertyChanged("LocalDefaultText"); }
        }


        string newButtonDefaultText;

        public string NewButtonDefaultText
        {
            get { return newButtonDefaultText; }
            set { newButtonDefaultText = value; FirePropertyChanged("NewButtonDefaultText"); }
        }

        string newButtonLocalDefaultText;

        public string NewButtonLocalDefaultText
        {
            get { return newButtonLocalDefaultText; }
            set { newButtonLocalDefaultText = value; FirePropertyChanged("NewButtonLocalDefaultText"); }
        }

        string newWizardComponentPath;

        public string NewWizardComponentPath
        {
            get { return newWizardComponentPath; }
            set { newWizardComponentPath = value; FirePropertyChanged("NewWizardComponentPath"); }
        }

        bool noPMController;
        public bool NoPMController
        {
            get { return noPMController; }
            set { noPMController = value; FirePropertyChanged("NoPMController"); }
        }


        bool noTS;
        public bool NoTS
        {
            get { return noTS; }
            set { noTS = value; FirePropertyChanged("NoTS"); }
        }

		bool noDefaultFeatures;
		public bool NoDefaultFeatures
		{
			get { return noDefaultFeatures; }
			set { noDefaultFeatures = value; FirePropertyChanged("NoDefaultFeatures"); }
		}

		

		bool noViewController;
        public bool NoViewController
        {
            get { return noViewController; }
            set { noViewController = value; FirePropertyChanged("NoViewController"); }
        }


        bool hasCompactSearch;
        public bool HasCompactSearch
        {
            get { return hasCompactSearch; }
            set { hasCompactSearch = value; FirePropertyChanged("HasCompactSearch"); }
        }

        bool applyDefaultValues;
        public bool ApplyDefaultValues
        {
            get { return applyDefaultValues; }
            set { applyDefaultValues = value; FirePropertyChanged("ApplyDefaultValues"); }
        }

        bool hasMenuButtons;
        public bool HasMenuButtons
        {
            get { return hasMenuButtons; }
            set { hasMenuButtons = value; FirePropertyChanged("HasMenuButtons"); }
        }

        bool applyOnPropertyChangedCode;
        public bool ApplyOnPropertyChangedCode
        {
            get { return applyOnPropertyChangedCode; }
            set { applyOnPropertyChangedCode = value; FirePropertyChanged("ApplyOnPropertyChangedCode"); }
        }

        bool tenantZeroData;
        public bool TenantZeroData
        {
            get { return tenantZeroData; }
            set { tenantZeroData = value; FirePropertyChanged("TenantZeroData"); }
        }

        bool showFastSearch = false;
        public bool ShowFastSearch
        {
            get { return showFastSearch; }
            set { showFastSearch = value; FirePropertyChanged("ShowFastSearch"); }
        }


        bool hasApiHelper;
        public bool HasApiHelper
        {
            get { return hasApiHelper; }
            set { hasApiHelper = value; FirePropertyChanged("HasApiHelper"); }
        }

        bool allowedForComputingPartners;
        public bool AllowedForComputingPartners
        {
            get { return allowedForComputingPartners; }
            set { allowedForComputingPartners = value; FirePropertyChanged("AllowedForComputingPartners"); }
        }

        bool isMetadataOnlyTable;
        public bool IsMetadataOnlyTable
        {
            get { return isMetadataOnlyTable; }
            set { isMetadataOnlyTable = value; FirePropertyChanged("IsMetadataOnlyTable"); }
        }

        string parentTableName;

        public string ParentTableName
        {
            get { return parentTableName; }
            set { parentTableName = value; FirePropertyChanged("ParentTableName"); }
        }

        string dBTableName;
        public string DBTableName
        {
            get { return dBTableName; }
            set { dBTableName = value; IsDirty = true; FirePropertyChanged("DBTableName"); }
        }

        string dBTableOldNames;
        public string DBTableOldNames
        {
            get { return dBTableOldNames; }
            set { dBTableOldNames = value; FirePropertyChanged("DBTableOldNames"); }
        }

        string dBTableShortName;
        public string DBTableShortName
        {
            get { return dBTableShortName; }
            set { dBTableShortName = value; IsDirty = true; FirePropertyChanged("DBTableShortName"); }
        }
        
        string olddBTableName;
        public string OldDBTableName
        {
            get { return olddBTableName; }
            set { olddBTableName = value; FirePropertyChanged("OldDBTableName"); }
        }

        string objectTableSingular;
        public string ObjectTableSingular
        {
            get { return objectTableSingular; }
            set { objectTableSingular = value; FirePropertyChanged("ObjectTableSingular"); }
        }

        string objectTablePlural;
        public string ObjectTablePlural
        {
            get { return objectTablePlural; }
            set { objectTablePlural = value; FirePropertyChanged("ObjectTablePlural"); }
        }

        string descriptionDefaultText;
        public string DescriptionDefaultText
        {
            get { return descriptionDefaultText; }
            set { descriptionDefaultText = value; FirePropertyChanged("DescriptionDefaultText"); }
        }

        string descriptionLocalDefaultText;
        public string DescriptionLocalDefaultText
        {
            get { return descriptionLocalDefaultText; }
            set { descriptionLocalDefaultText = value; FirePropertyChanged("DescriptionLocalDefaultText"); }
        }

        bool isDirty;
        public bool IsDirty
        {
            get { return isDirty; }
            set { isDirty = value; FirePropertyChanged("IsDirty"); }
        }

        bool isNew;
        public bool IsNew
        {
            get { return isNew; }
            set { isNew = value; FirePropertyChanged("IsNew"); }
        }

        bool isNewWizard;
        public bool IsNewWizard
        {
            get { return isNewWizard; }
            set { isNewWizard = value; FirePropertyChanged("IsNewWizard"); }
        }

        string newWizardControlName;

        public string NewWizardControlName
        {
            get { return newWizardControlName; }
            set { newWizardControlName = value; FirePropertyChanged("NewWizardControlName"); }
        }

        string lookUp1;
        public string LookUp1
        {
            get { return lookUp1; }
            set { lookUp1 = value; FirePropertyChanged("LookUp1"); }
        }

        string lookUp2;
        public string LookUp2
        {
            get { return lookUp2; }
            set { lookUp2 = value; FirePropertyChanged("LookUp2"); }
        }

        string codeField;
        public string CodeField
        {
            get { return codeField; }
            set { codeField = value; FirePropertyChanged("CodeField"); }
        }

        string nameField;
        public string NameField
        {
            get { return nameField; }
            set { nameField = value; FirePropertyChanged("NameField"); }
        }

        string dependencyFilter1;
        public string DependencyFilter1
        {
            get { return dependencyFilter1; }
            set { dependencyFilter1 = value; FirePropertyChanged("DependencyFilter1"); }
        }

        string dependencyFilter2;
        public string DependencyFilter2
        {
            get { return dependencyFilter2; }
            set { dependencyFilter2 = value; FirePropertyChanged("DependencyFilter2"); }
        }

        string dependencyFilter3;
        public string DependencyFilter3
        {
            get { return dependencyFilter3; }
            set { dependencyFilter3 = value; FirePropertyChanged("DependencyFilter3"); }
        }

        string keyPropertyPath;
        public string KeyPropertyPath
        {
            get { return keyPropertyPath; }
            set { keyPropertyPath = value; FirePropertyChanged("KeyPropertyPath"); }
        }

        bool autoCompleteSearchWindow;
        public bool AutoCompleteSearchWindow
        {
            get { return autoCompleteSearchWindow; }
            set { autoCompleteSearchWindow = value; FirePropertyChanged("AutoCompleteSearchWindow"); }
        }

        bool isClosed;
        public bool IsClosed
        {
            get { return isClosed; }
            set
            {
                isClosed = value;
                if (isClosed)
                {
                    TablesDataVisibility = Visibility.Visible;

                }
                else
                {
                    TablesDataVisibility = Visibility.Collapsed;
                }
                FirePropertyChanged("ClosedTablesFieldsVisibility");
                FirePropertyChanged("IsClosed");
            }
        }

        bool cacheOnClient;
        public bool CacheOnClient
        {
            get { return cacheOnClient; }
            set { cacheOnClient = value; FirePropertyChanged("CacheOnClient"); }
        }

        bool editableFromAutoCompleteWindow;
        public bool EditableFromAutoCompleteWindow
        {
            get { return editableFromAutoCompleteWindow; }
            set { editableFromAutoCompleteWindow = value; FirePropertyChanged("EditableFromAutoCompleteWindow"); }
        }

        bool hasCounter;
        public bool HasCounter
        {
            get { return hasCounter; }
            set { hasCounter = value; FirePropertyChanged("HasCounter"); }
        }

        bool enableEditFromLOV;
        public bool EnableEditFromLOV
        {
            get { return enableEditFromLOV; }
            set { enableEditFromLOV = value; FirePropertyChanged("EnableEditFromLOV"); }
        }

        bool enableAddFromLOV;
        public bool EnableAddFromLOV
        {
            get { return enableAddFromLOV; }
            set { enableAddFromLOV = value; FirePropertyChanged("EnableAddFromLOV"); }
        }

        bool isRestrictable;
        public bool IsRestrictable
        {
            get { return isRestrictable; }
            set { isRestrictable = value; FirePropertyChanged("IsRestrictable"); }
        }

        bool isMain;
        public bool IsMain
        {
            get { return isMain; }
            set { isMain = value; FirePropertyChanged("IsMain"); }
        }

        bool isAutoComplete;
        public bool IsAutoComplete
        {
            get { return isAutoComplete; }
            set { isAutoComplete = value; FirePropertyChanged("IsAutoComplete"); }
        }

        string sortingByObjectField;
        public string SortingByObjectField
        {
            get { return sortingByObjectField; }
            set { sortingByObjectField = value; FirePropertyChanged("SortingByObjectField"); }
        }


        string sortingByDirection;
        public string SortingByDirection
        {
            get { return sortingByDirection; }
            set { sortingByDirection = value; FirePropertyChanged("SortingByDirection"); }
        }



        bool inActive;
        public bool InActive
        {
            get { return inActive; }
            set { inActive = value; FirePropertyChanged("InActive"); }
        }

        string shortTitleControlPath;
        public string ShortTitleControlPath
        {
            get { return shortTitleControlPath; }
            set { shortTitleControlPath = value; FirePropertyChanged("ShortTitleControlPath"); }
        }

        bool isSaveButtonVisible;
        public bool IsSaveButtonVisible
        {
            get { return isSaveButtonVisible; }
            set { isSaveButtonVisible = value; FirePropertyChanged("IsSaveButtonVisible"); }
        }

        bool isComposition;
        public bool IsComposition
        {
            get { return isComposition; }
            set { isComposition = value; FirePropertyChanged("DomainServiceVisibility"); FirePropertyChanged("IsComposition"); }
        }

        public string Id { get; set; }

        bool enableSecurity;
        public bool EnableSecurity
        {
            get { return enableSecurity; }
            set { enableSecurity = value; FirePropertyChanged("EnableSecurity"); }
        }

        bool allowCustomFields;
        public bool AllowCustomFields
        {
            get { return allowCustomFields; }
            set { allowCustomFields = value; FirePropertyChanged("AllowCustomFields"); }
        }

        bool hasDynamicHeader;
        public bool HasDynamicHeader
        {
            get { return hasDynamicHeader; }
            set { hasDynamicHeader = value; FirePropertyChanged("HasDynamicHeader"); }
        }

        string mainTipCode;
        public string MainTipCode
        {
            get { return mainTipCode; }
            set { mainTipCode = value; FirePropertyChanged("MainTipCode"); }
        }

        string objectTableTypeCode;
        public string ObjectTableTypeCode
        {
            get { return objectTableTypeCode; }
            set { objectTableTypeCode = value; FirePropertyChanged("ObjectTableTypeCode"); }
        }

        string dxmlDatabaseTypeCode;
        public string DxmlDatabaseTypeCode
        {
            get { return dxmlDatabaseTypeCode; }
            set { dxmlDatabaseTypeCode = value; FirePropertyChanged("DxmlDatabaseTypeCode"); }
        }

        string dxmlDatabaseSchemaCode;
        public string DxmlDatabaseSchemaCode
        {
            get { return dxmlDatabaseSchemaCode; }
            set { dxmlDatabaseSchemaCode = value; FirePropertyChanged("DxmlDatabaseSchemaCode"); }
        }

        int maxNumberOfCustomFields;
        public int MaxNumberOfCustomFields
        {
            get { return maxNumberOfCustomFields; }
            set { maxNumberOfCustomFields = value; FirePropertyChanged("MaxNumberOfCustomFields"); }
        }


        bool generateDomainService;

        public bool GenerateDomainService
        {
            get { return generateDomainService; }
            set { generateDomainService = value; FirePropertyChanged("GenerateDomainService"); }
        }

        string closeTableCode;
        public string CloseTableCode
        {
            get { return closeTableCode; }
            set { closeTableCode = value; FirePropertyChanged("CloseTableCode"); }
        }

        string closeTableName;
        public string CloseTableName
        {
            get { return closeTableName; }
            set { closeTableName = value; FirePropertyChanged("CloseTableName"); }
        }

        bool isEditable;
        public bool IsEditable
        {
            get { return isEditable; }
            set { isEditable = value; FirePropertyChanged("IsEditable"); }
        }

        bool hasCustomFilter;

        public bool HasCustomFilter
        {
            get { return hasCustomFilter; }
            set { hasCustomFilter = value; FirePropertyChanged("HasCustomFilter"); }
        }


        bool hasCustomFields;

        public bool HasCustomFields
        {
            get { return hasCustomFields; }
            set { hasCustomFields = value; FirePropertyChanged("HasCustomFields"); }
        }

        bool availableInCustomization;

        public bool AvailableInCustomization
        {
            get { return availableInCustomization; }
            set { availableInCustomization = value; FirePropertyChanged("AvailableInCustomization"); }
        }

        bool supportSubEntity;

        public bool SupportSubEntity
        {
            get { return supportSubEntity; }
            set { supportSubEntity = value; FirePropertyChanged("SupportSubEntity"); }
        }

        bool applyGenericCustomFields;
        public bool ApplyGenericCustomFields
        {
            get { return applyGenericCustomFields; }
            set { applyGenericCustomFields = value; FirePropertyChanged("ApplyGenericCustomFields"); }
        }

        bool availableInDocumentTypes;
        public bool AvailableInDocumentTypes
        {
            get { return availableInDocumentTypes; }
            set { availableInDocumentTypes = value; FirePropertyChanged("AvailableInDocumentTypes"); }
        }

        bool hasShortTitle;

        public bool HasShortTitle
        {
            get { return hasShortTitle; }
            set { hasShortTitle = value; FirePropertyChanged("HasShortTitle"); }
        }

        bool hasFiltersMenu;

        public bool HasFiltersMenu
        {
            get { return hasFiltersMenu; }
            set { hasFiltersMenu = value; FirePropertyChanged("HasFiltersMenu"); }
        }

        

        bool hasHelper;

        public bool HasHelper
        {
            get { return hasHelper; }
            set { hasHelper = value; FirePropertyChanged("HasHelper"); }
        }

        bool hasCustomValidator;

        public bool HasCustomValidator
        {
            get { return hasCustomValidator; }
            set { hasCustomValidator = value; FirePropertyChanged("HasCustomValidator"); }
        }

        string clientModuleName;

        public string ClientModuleName
        {
            get { return clientModuleName; }
            set { clientModuleName = value; FirePropertyChanged("ClientModuleName"); }


        }

        string serverModuleName;

        public string ServerModuleName
        {
            get { return serverModuleName; }
            set { serverModuleName = value; ; FirePropertyChanged("ServerModuleName"); }
        }

        public int fieldLength;
        public int FieldLength
        {
            get
            {
                return fieldLength;
            }
            set
            {
                fieldLength = value;
                FirePropertyChanged("FieldLength");
            }
        }


        string lovDisplayMemberPath;
        public string LovDisplayMemberPath
        {
            get { return lovDisplayMemberPath; }
            set { lovDisplayMemberPath = value; FirePropertyChanged("LovDisplayMemberPath"); }
        }

        string lovDisplayMemberPathLocal;
        public string LovDisplayMemberPathLocal
        {
            get { return lovDisplayMemberPathLocal; }
            set { lovDisplayMemberPathLocal = value; FirePropertyChanged("LovDisplayMemberPathLocal"); }
        }

        bool isTabsHidden;

        public bool IsTabsHidden
        {
            get { return isTabsHidden; }
            set { isTabsHidden = value; FirePropertyChanged("IsTabsHidden"); }
        }

        bool isBusinessUnitEnabled;
        public bool IsBusinessUnitEnabled
        {
            get { return isBusinessUnitEnabled; }
            set { isBusinessUnitEnabled = value; FirePropertyChanged("IsBusinessUnitEnabled"); }
        }

        ObjectFieldsViewModel selectedObjectField;

        public ObjectFieldsViewModel SelectedObjectField
        {
            get { return selectedObjectField; }
            set
            {
                selectedObjectField = value;
                if (value != null)
                    FieldLength = value.Length;
                FirePropertyChanged("SelectedObjectField");
            }
        }
		bool isLock;
		public bool IsLock
		{
			get { return isLock; }
			set { isLock = value; FirePropertyChanged("IsLock"); FirePropertyChanged("IsLockFontWeight"); }
		}
		string relatedEntity;
		public string RelatedEntity
		{
			get { return relatedEntity; }
			set { relatedEntity = value; FirePropertyChanged("RelatedEntity"); FirePropertyChanged("IsLockFontWeight"); }
		}
		string thisKey;
		public string ThisKey
		{
			get { return thisKey; }
			set { thisKey = value; FirePropertyChanged("ThisKey"); }
		}
		string relatedKey;
		public string RelatedKey
		{
			get { return relatedKey; }
			set { relatedKey = value; FirePropertyChanged("RelatedKey"); }
		}

		public string IsLockFontWeight
		{
			get
			{
				string result = "Normal";
				if (IsLock && !string.IsNullOrEmpty(RelatedEntity))
				{
					result = "Bold";
				}
				return result;
			}
			set { }
		}
		// commands
		public RelayCommand<ObjectFieldsViewModel> RemoveFieldCommand
        {
            get { return new RelayCommand<ObjectFieldsViewModel>(m => this.RemoveFieldMethod(m)); }
        }

        public void RemoveFieldMethod(ObjectFieldsViewModel selected)
        {
            if (selected != null)
            {
                ObsList.Where(a => a.FieldName == selected.FieldName).FirstOrDefault().IsDeleted = true;
                TempObsList.Where(a => a.FieldName == selected.FieldName).FirstOrDefault().IsDeleted = true;
                ObsList.Where(a => a.FieldName == selected.FieldName).FirstOrDefault().IsChecked = true;

                int selectedIndex = ObsList.IndexOf(selected);
                if(selectedIndex != 0)
                {
                    selectedIndex -= 1;
                }

                ObsList.Remove(selected);
                TempObsList.Remove(selected);

                if (ObsList.Count > 0)
                    this.SelectedObjectField = ObsList[selectedIndex];


                FirePropertyChanged("ObsList");
                FirePropertyChanged("TempObsList");

                this.SetLookUpFieldsList();

                FieldsEditControlVisibility = (ObsList.Count == 0) ? Visibility.Collapsed : Visibility.Visible;

                 

                //this.SelectedObjectField = ObsList.FirstOrDefault();
                //this.SelectedDBField = DBFieldsObsList.FirstOrDefault();
            }
        }

        public ScreensControl ScreensControl;
        public RelayCommand AddScreenCommand
        {
            get { return new RelayCommand(() => this.AddScreenMethod()); }
        }
        public Window screenWindow = new Window();
        private void AddScreenMethod()
        {
            ScreensViewModel model = new ScreensViewModel(this, true);
            ScreensControl = new ScreensControl();
            ScreensControl.DataContext = model;

            screenWindow = new Window();
            screenWindow.Width = 500;
            screenWindow.Height = 300;
            screenWindow.Content = ScreensControl;
            screenWindow.Show();

            //fieldsControl.Show();
        }
        public RelayCommand AddTableDataCommand
        {
            get { return new RelayCommand(() => this.AddTableDataMethod()); }
        }
        public Window TableDataWindow = new Window();
        public TablesDataUserControl TablesDataUserControl;
        private void AddTableDataMethod()
        {


            CloseTablesDataViewModel model = new CloseTablesDataViewModel(this);
            TablesDataUserControl = new TablesDataUserControl();
            TablesDataUserControl.DataContext = model;

            TableDataWindow = new Window();
            TableDataWindow.Width = 500;
            TableDataWindow.Height = 400;
            TableDataWindow.Content = TablesDataUserControl;
            TableDataWindow.Show();
        }



        public RelayCommand EditTableDataCommand
        {
            get { return new RelayCommand(() => this.EditTableDataMethod()); }
        }
        private void EditTableDataMethod()
        {

            if (CLoseTableDataGrid.SelectedItem != null)
            {
                CloseTablesDataViewModel model = new CloseTablesDataViewModel(this, true);
                var rowData =  (CLoseTableDataGrid.SelectedItem as Row)._data;

                Dictionary<string, string> FieldsDictionaryTemp = new Dictionary<string, string>();

                foreach (var k in rowData.Keys)
                {
                    if (!this.FieldsDictionary.ContainsKey(k))
                        this.FieldsDictionary.Add(k, rowData[k] != null ? rowData[k].ToString() : null);
                    else 
                        FieldsDictionaryTemp.Add(k, rowData[k] != null ? rowData[k].ToString() : null);
                }

                if(FieldsDictionaryTemp.Count > 0 )
                {
                    this.FieldsDictionary = FieldsDictionaryTemp;
                }

                TablesDataUserControl = new TablesDataUserControl();
                TablesDataUserControl.DataContext = model;

                TableDataWindow = new Window();
                TableDataWindow.Width = 500;
                TableDataWindow.Height = 400;
                TableDataWindow.Content = TablesDataUserControl;
                TableDataWindow.Show();
            }
        }


        public EventTypesControl EventsControl;
        public Window EventsWindow = new Window();
        public RelayCommand AddEventTypeCommand
        {
            get { return new RelayCommand(() => this.AddEventTypeMethod()); }
        }
        private void AddEventTypeMethod()
        {
            EventTypesViewModel model = new EventTypesViewModel(this, true);
            model.ButtonsVisibility = Visibility.Visible;
            EventsControl = new EventTypesControl();
            EventsControl.DataContext = model;

            EventsWindow = new Window();
            EventsWindow.Width = 500;
            EventsWindow.Height = 600;
            EventsWindow.Content = EventsControl;
            EventsWindow.Show();
        }


        public AddDataContractControl DataContractControl;
        public Window DCWindow = new Window();
        public RelayCommand AddDataContractCommand
        {
            get { return new RelayCommand(() => this.AddDataContractMethod()); }
        }
        private void AddDataContractMethod()
        {
            DataContractViewModel model = new DataContractViewModel(this, true);
            //model.ButtonsVisibility = Visibility.Visible;
            DataContractControl = new AddDataContractControl();
            DataContractControl.DataContext = model;

            DCWindow = new Window();
            DCWindow.Width = 500;
            DCWindow.Height = 235;
            DCWindow.Content = DataContractControl;
            DCWindow.Show();
        }

        public RelayCommand<EventTypesViewModel> RemoveEventTypeCommand
        {
            get { return new RelayCommand<EventTypesViewModel>(m => this.RemoveEventTypeMethod(m)); }
        }

        private void RemoveEventTypeMethod(EventTypesViewModel DelET)
        {
            EventTypesObsList.Remove(DelET);
            FirePropertyChanged("EventTypesObsList");
        }


        public TextCodesControl TextCodesControl;
        public Window TextCodesWindow = new Window();
        public RelayCommand AddTextCodeCommand
        {
            get { return new RelayCommand(() => this.AddTextCodeMethod()); }
        }
        private void AddTextCodeMethod()
        {
            TextCodesViewModel model = new TextCodesViewModel(this, true);
            model.ButtonsVisibility = Visibility.Visible;
            TextCodesControl = new TextCodesControl();
            TextCodesControl.DataContext = model;

            TextCodesWindow = new Window();
            TextCodesWindow.Width = 500;
            TextCodesWindow.Height = 600;
            TextCodesWindow.Content = TextCodesControl;
            TextCodesWindow.Show();
        }

        public RelayCommand<TextCodesViewModel> RemoveTextCodeCommand
        {
            get { return new RelayCommand<TextCodesViewModel>(m => this.RemoveTextCodeMethod(m)); }
        }

        private void RemoveTextCodeMethod(TextCodesViewModel DelET)
        {
            AdditionalTextCodesList.Remove(DelET);
            FirePropertyChanged("AdditionalTextCodesList");

			AdditionalTextCodesTempList.Remove(DelET);
			FirePropertyChanged("AdditionalTextCodesTempList");
		}


        public FeaturesControl FeaturesControl;
        public Window FeaturesWindow = new Window();
        public RelayCommand AddFeatureCommand
        {
            get { return new RelayCommand(() => this.AddFeatureMethod()); }
        }
        private void AddFeatureMethod()
        {
            FeaturesViewModel model = new FeaturesViewModel(this, true);
            model.ButtonsVisibility = Visibility.Visible;
            FeaturesControl = new FeaturesControl();
            FeaturesControl.DataContext = model;

            FeaturesWindow = new Window();
            FeaturesWindow.Width = 500;
            FeaturesWindow.Height = 600;
            FeaturesWindow.Content = FeaturesControl;
            FeaturesWindow.Show();
        }

        public RelayCommand<FeaturesViewModel> RemoveFeatureCommand
        {
            get { return new RelayCommand<FeaturesViewModel>(m => this.RemoveFeatureMethod(m)); }
        }

        private void RemoveFeatureMethod(FeaturesViewModel item)
        {
            AdditionalFeaturesList.Remove(item);
            FirePropertyChanged("AdditionalFeaturesList");

			AdditionalFeaturesTempList.Remove(item);
			FirePropertyChanged("AdditionalFeaturesTempList");
		}



        public RelayCommand<TabsViewModel> RemoveTabCommand
        {
            get { return new RelayCommand<TabsViewModel>(m => this.RemoveTabMethod(m)); }
        }

        private void RemoveTabMethod(TabsViewModel DelTab)
        {
            int selectedIndex = TabsObsList.IndexOf(DelTab);
            if (selectedIndex != 0)
            {
                selectedIndex -= 1;
            }

            TabsObsList.Remove(DelTab);

            if (TabsObsList.Count > 0)
                this.SelectedTab = TabsObsList[selectedIndex];

            FirePropertyChanged("TabsObsList");
        }

        public RelayCommand<ScreensViewModel> RemoveScreenCommand
        {
            get { return new RelayCommand<ScreensViewModel>(m => this.RemoveScreenMethod(m)); }
        }

        private void RemoveScreenMethod(ScreensViewModel DelScr)
        {
            int selectedIndex = ScreensObsList.IndexOf(DelScr);
            if (selectedIndex != 0)
            {
                selectedIndex -= 1;
            }

            ScreensObsList.Remove(DelScr);

            if (ScreensObsList.Count > 0)
                this.SelectedScreen = ScreensObsList[selectedIndex];

            FirePropertyChanged("ScreensObsList");
        }

        public RelayCommand<MenuButtonViewModel> RemoveMenuButtonCommand
        {
            get { return new RelayCommand<MenuButtonViewModel>(m => this.RemoveMenuButtonMethod(m)); }
        }

        private void RemoveMenuButtonMethod(MenuButtonViewModel DelMenuButton)
        {
            MenuButtonsObsList.Remove(DelMenuButton);
            FirePropertyChanged("MenuButtonsObsList");
            //FirePropertyChanged("SubMenuButtonsObsList");
            //FirePropertyChanged("AllMenuButtonsObsList");
        }

        public RelayCommand<MenuButtonViewModel> RemoveMenuItemCommand
        {
            get { return new RelayCommand<MenuButtonViewModel>(m => this.RemoveMenuItemMethod(m)); }
        }

        private void RemoveMenuItemMethod(MenuButtonViewModel DelMenuButton)
        {
            SelectedMenuButton.MenuButtonItems.Remove(DelMenuButton);
            SelectedMenuButton.RefreshMenuItems();
            FirePropertyChanged("SelectedMenuButton");
            //FirePropertyChanged("SubMenuButtonsObsList");
            //FirePropertyChanged("AllMenuButtonsObsList");
        }

        public MenuButtonControl MenuButtonControl;
        public RelayCommand<string> AddMenuButtonCommand
        {
            get { return new RelayCommand<string>(m => this.AddMenuButtonMethod(m)); }
        }
        public Window MenuButtonWindow = new Window();
        private void AddMenuButtonMethod(string Para)
        {
            MenuButtonViewModel model = new MenuButtonViewModel(this, true, Para);
            model.ButtonsVisibility = Visibility.Visible;
            MenuButtonControl = new MenuButtonControl();
            MenuButtonControl.DataContext = model;

            MenuButtonWindow = new Window();
            MenuButtonWindow.Width = 500;
            MenuButtonWindow.Height = 600;
            MenuButtonWindow.Content = MenuButtonControl;
            MenuButtonWindow.Show();

            //fieldsControl.Show();
        }

        public TabsControl TabsControl;
        public RelayCommand AddTabCommand
        {
            get { return new RelayCommand(() => this.AddTabMethod()); }
        }
        public Window tabWindow = new Window();
        private void AddTabMethod()
        {
            TabsViewModel model = new TabsViewModel(this, true);
            model.ButtonsVisibility = Visibility.Visible;
            TabsControl = new TabsControl();
            TabsControl.DataContext = model;

            tabWindow = new Window();
            tabWindow.Width = 600;
            tabWindow.Height = 450;
            tabWindow.Content = TabsControl;
            tabWindow.Show();

            //fieldsControl.Show();
        }

        public RelayCommand AddFieldCommand
        {
            get { return new RelayCommand(() => this.AddFieldMethod()); }
        }
        public Window newWindow = new Window();
        private void AddFieldMethod()
        {
            ObjectFieldsViewModel model = new ObjectFieldsViewModel(this, true);
            model.SetControlFieldsList(model);
            fieldsControl = new ObjectFieldsControl();
            fieldsControl.DataContext = model;

            newWindow = new Window();
            newWindow.Content = fieldsControl;
            newWindow.Show();

            //fieldsControl.Show();
        }

        public RelayCommand OkBtnCommand
        {
            get { return new RelayCommand(() => this.OkBtnMethod()); }
        }

        Visibility errorsVisibility = Visibility.Collapsed;
        public Visibility ErrorsVisibility
        {
            get { return errorsVisibility; }
            set { errorsVisibility = value; FirePropertyChanged("ErrorsVisibility"); }
        }




        //Visibility domainServiceVisibility = Visibility.Collapsed;
        public Visibility DomainServiceVisibility
        {
            get
            {
                if (IsComposition)
                {
                    GenerateDomainService = false;
                    return Visibility.Collapsed;
                }
                else
                {
                    return Visibility.Visible;
                }
            }
            //set { errorsVisibility = value; FirePropertyChanged("DomainServiceVisibility"); }
        }


        public string errorMessages;
        public string ErrorMessages
        {
            get
            {
                return errorMessages;
            }
            set
            {
                errorMessages = value;
                FirePropertyChanged("ErrorMessages");
            }
        }

        public RelayCommand GenerateBtnCommand
        {
            get { return new RelayCommand(() => this.GenerateBtnMethod()); }
        }
        private void GenerateBtnMethod()
        {
            SaveSQLChanges();
        }
        public bool SaveSQLChanges()
        {
            bool succeeded = false;
            if ((DBTableName.Contains("Customs") ? DBTableName.Substring(9).Length > 30 : DBTableName.Length > 30))
            {
                ErrorMessages = "Table Name Shouldn't be more than 30 char. length ..";
                ErrorsVisibility = Visibility.Visible;
                return false;
            }
            try
            {
                ErrorsVisibility = Visibility.Collapsed;
                ErrorMessages = string.Empty;

                this.ValidateDBObjectTable();
                if (ObsList != null)
                {
                    foreach (var item in ObsList)
                    {
                        this.ValidateDBObjectField(item);
                    }
                }
                else
                {
                    ObsList = new ObservableCollection<ObjectFieldsViewModel>();
                }

                if (ErrorMessages == "")
                {
                    //UpdateObsList(this);
                    succeeded = true;
                    XmlGeneratorClass.GenerateSqlXmlFileFromTool(this,true);
                    // App.CurrentControl.Close();
                    //Environment.Exit(0);
                }
                else
                {
                    ErrorsVisibility = Visibility.Visible;
                }
            }

            catch (Exception ex)
            {
                string error = ex.Message + "\n" + ex.StackTrace != null ? ex.StackTrace : "";
                if (ex.InnerException != null && !string.IsNullOrEmpty(ex.InnerException.Message))
                {
                    error += "\n" + ex.InnerException.Message;

                    if (ex.InnerException.StackTrace != null)
                    {
                        error += "\n" + ex.InnerException.StackTrace != null ? ex.InnerException.StackTrace : "";
                    }
                }


                MessageBox.Show(error);

            }

            return succeeded;
        }

        private void ValidateDBObjectField(ObjectFieldsViewModel item)
        {
            StringBuilder str = new StringBuilder();
            if (item.Length <= 30)
            {
                if (item.FieldName.Length > 30)
                {
                    str.AppendLine("FieldName Couldn't be more than 30 char. ");
                }
            }

            if (string.IsNullOrEmpty(item.FieldName))
            {
                str.AppendLine("Field Name is Required");
            }


            if (string.IsNullOrEmpty(item.FieldDataType))
            {
                str.AppendLine("Data Type is Required");
            }
            else
            {

                if (item.FieldDataType == "Text" || item.FieldDataType == "nText" || item.FieldDataType == "LookUp")
                {
                    if (string.IsNullOrEmpty(item.ToString()))
                    {
                        str.AppendLine("Max Length is Required");
                    }
                }
                else if (item.FieldDataType == "Decimal" || item.FieldDataType == "Double" || item.FieldDataType == "SigDouble" || item.FieldDataType == "UnsDecimal")
                {
                    if (item.DigitsAfterPoint == null)
                    {
                        str.AppendLine("Digits After Point is Required");
                    }
                }
            }

            if (item.IsForeignKey)
            {
                if (string.IsNullOrEmpty(item.ForeignEntity))
                {
                    str.AppendLine("Foreign Entity is Required");
                }
                if (string.IsNullOrEmpty(item.NavigationPropertyName))
                {
                    str.AppendLine("Navigation Property Name is Required");
                }
            }


            if (!string.IsNullOrEmpty(ErrorMessages))
            {
                ErrorsVisibility = Visibility.Visible;
            }

            if (!string.IsNullOrEmpty(str.ToString()))
            {
                ErrorMessages = ErrorMessages + "\n" + item.FieldName + " Field Errors:\n" + str.ToString();
            }

            FirePropertyChanged("ErrorMessages");
        }

        private void ValidateDBObjectTable()
        {
            StringBuilder str = new StringBuilder();
            if (string.IsNullOrEmpty(DBTableName))
            {
                str.AppendLine("DB Table Name is Required");
            }

            ErrorMessages = str.ToString();
            if (!string.IsNullOrEmpty(ErrorMessages))
            {
                ErrorsVisibility = Visibility.Visible;
            }

            FirePropertyChanged("ErrorMessages");
        }

        public RelayCommand GenerateRenameTableBtnCommand
        {
            get { return new RelayCommand(() => this.GenerateRenameTableBtnMethod()); }
        }
        //string oldTableName;
        //public string OldTableName
        //{
        //    get
        //    {
        //        return oldTableName;
        //    }
        //    set
        //    {
        //        oldTableName = value;
        //        FirePropertyChanged("OldTableName");
        //    }
        //}
        public Window MyWindow;
        private void GenerateRenameTableBtnMethod()
        {
            MyWindow = new Window();
            NameFormVM VM = new NameFormVM(this);
            NameForm control = new NameForm();
            control.DataContext = VM;
            MyWindow.Width = 330;
            MyWindow.Height = 170;
            MyWindow.Content = control;
            MyWindow.ShowDialog();
        }

        public void GenerateTableRename(string OldTableName)
        {
            if (string.IsNullOrEmpty(OldTableName))
            {
                ErrorMessages = "OldTableName is required ..";
                ErrorsVisibility = Visibility.Visible;
                return;
            }
            XmlGeneratorClass.GenerateSQLXmlForRenameTable(DBTableName, OldTableName);
            MyWindow.Close();
        }

        public RelayCommand GenerateDeleteFieldsBtnCommand
        {
            get { return new RelayCommand(() => this.GenerateDeleteFieldsBtnMethod()); }
        }
        private void GenerateDeleteFieldsBtnMethod()
        {

            CheckedObjectFields = new List<ObjectFieldsViewModel>();
            if (string.IsNullOrEmpty(DBTableName))
            {
                ErrorMessages = "Table Name is required ..";
                ErrorsVisibility = Visibility.Visible;
                return;
            }
            foreach (var item in ObsList)
            {
                if (item.IsChecked)
                {
                    if (!item.IsDBField)
                    {
                        ErrorMessages = item.FieldName + " is not a poco ..";
                        ErrorsVisibility = Visibility.Visible;
                        return;
                    }
                    this.ValidateDBObjectField(item);///////////////
                    CheckedObjectFields.Add(item);
                }
            }
            XmlGeneratorClass.GenerateSQLXmlForDropColumns(CheckedObjectFields, DBTableName);
        }

        public RelayCommand GenerateRenameFieldsBtnCommand
        {
            get { return new RelayCommand(() => this.GenerateRenameFieldsBtnMethod()); }
        }
        private void GenerateRenameFieldsBtnMethod()
        {
            MyWindow = new Window();
            NameFormVM VM = new NameFormVM(this, "ReNameField");
            NameForm control = new NameForm();
            control.DataContext = VM;
            MyWindow.Width = 380;
            MyWindow.Height = 280;
            MyWindow.Content = control;
            MyWindow.ShowDialog();
            //CheckedObjectFields = new List<ObjectFieldsViewModel>();
            //if (string.IsNullOrEmpty(DBTableName))
            //{
            //    ErrorMessages = "Table Name is required ..";
            //    ErrorsVisibility = Visibility.Visible;
            //    return;
            //}
            //foreach (var item in ObsList)
            //{
            //    if (item.IsChecked)
            //    {
            //        if (!item.IsDBField)
            //        {
            //            ErrorMessages = item.FieldName + " is not a poco ..";
            //            ErrorsVisibility = Visibility.Visible;
            //            return;
            //        }
            //        this.ValidateDBObjectField(item);
            //        CheckedObjectFields.Add(item);
            //    }
            //}
            //XmlGeneratorClass.GenerateSQLXmlForRenameColumns(CheckedObjectFields, DBTableName);
        }
        public void GenerateRenameField(NameFormVM nameFormVM)
        {
            XmlGeneratorClass.GenerateSQLXmlForRenameColumns(nameFormVM, DBTableName);
            MyWindow.Close();
        }

        public RelayCommand GenerateFieldsBtnCommand
        {
            get { return new RelayCommand(() => this.GenerateFieldsBtnMethod()); }
        }
        private void GenerateFieldsBtnMethod()
        {
            CheckedObjectFields = new List<ObjectFieldsViewModel>();
            if (string.IsNullOrEmpty(DBTableName))
            {
                ErrorMessages = "Table Name is required ..";
                ErrorsVisibility = Visibility.Visible;
                return;
            }
            foreach (var item in ObsList)
            {
                if (item.IsChecked)
                {
                    if (!item.IsDBField)
                    {
                        ErrorMessages = item.FieldName + " is not a poco ..";
                        ErrorsVisibility = Visibility.Visible;
                        return;
                    }
                    this.ValidateDBObjectField(item);
                    CheckedObjectFields.Add(item);
                }
            }
            XmlGeneratorClass.GenerateSQLXmlForColumns(this, DBTableName);
            //XmlGeneratorClass.GenerateSqlXmlFileFromTool(this,true);
        }

        public List<ObjectFieldsViewModel> CheckedObjectFields = new List<ObjectFieldsViewModel>();

        public RelayCommand GenerateUpdateFieldsBtnCommand
        {
            get { return new RelayCommand(() => this.GenerateUpdateFieldsBtnMethod()); }
        }
        private void GenerateUpdateFieldsBtnMethod()
        {
            MyWindow = new Window();
            NameFormVM VM = new NameFormVM(this, "AlterField");
            NameForm control = new NameForm();
            control.DataContext = VM;
            MyWindow.Width = 380;
            MyWindow.Height = 280;
            MyWindow.Content = control;
            MyWindow.ShowDialog();
            //CheckedObjectFields = new List<ObjectFieldsViewModel>();
            //if (string.IsNullOrEmpty(DBTableName))
            //{
            //    ErrorMessages = "Table Name is required ..";
            //    ErrorsVisibility = Visibility.Visible;
            //    return;
            //}
            //foreach (var item in ObsList)
            //{
            //    if (item.IsChecked)
            //    {
            //        if (!item.IsDBField)
            //        {
            //            ErrorMessages = item.FieldName + " is not a poco ..";
            //            ErrorsVisibility = Visibility.Visible;
            //            return;
            //        }
            //        if (!item.IsNullable && !string.IsNullOrEmpty(item.DefaultValue))
            //        {
            //            ErrorMessages = "This Field is not allowing null,Please Fill The Default Value ..";
            //            ErrorsVisibility = Visibility.Visible;
            //            return;
            //        }
            //        this.ValidateDBObjectField(item);
            //        CheckedObjectFields.Add(item);
            //    }
            //}
            //XmlGeneratorClass.GenerateSQLXmlForAlterColumns(CheckedObjectFields, DBTableName);
        }

        public void GenerateAlterField(NameFormVM nameFormVM)
        {
            XmlGeneratorClass.GenerateSQLXmlForAlterColumns(nameFormVM, DBTableName);
            MyWindow.Close();
        }

        private void OkBtnMethod()
        {
            SaveChanges();
        }

        public bool SaveChanges()
        {
            bool succeeded = false;

            if (this.IsComposition && string.IsNullOrEmpty(ParentTableName))
            {
                ErrorMessages = "ParentTableName Table Name is required for composition table";
                ErrorsVisibility = Visibility.Visible;
                return false;
            }

            if (string.IsNullOrEmpty(DBTableName))
            {
                ErrorMessages = "DataBase Table Name is required ..";
                ErrorsVisibility = Visibility.Visible;
                return false;
            }
            if (string.IsNullOrEmpty(ObjectTableName))
            {
                ErrorMessages = "ObjectTableName is required ..";
                ErrorsVisibility = Visibility.Visible;
                return false;
            }
            //if ((ObjectTableName.Contains("Customs") ? ObjectTableName.Substring(9).Length > 30 : ObjectTableName.Length > 30) || (DBTableName.Contains("Customs") ? DBTableName.Substring(9).Length > 30 : DBTableName.Length > 30))
            //{
            //    ErrorMessages = "ObjectTableName and DataBase Table Name Shouldn't be more than 30 char. length ..";
            //    ErrorsVisibility = Visibility.Visible;
            //    return false;
            //}

            if (string.IsNullOrEmpty(DxmlDatabaseTypeCode) && ObsList.Where(f => f.IsDBField).Any())
            {
                ErrorMessages = "Database Type is Required";
                ErrorsVisibility = Visibility.Visible;
                return false;
            }

            if (string.IsNullOrEmpty(DxmlDatabaseSchemaCode) && ObsList.Where(f => f.IsDBField).Any())
            {
                ErrorMessages = "Database Schema is Required";
                ErrorsVisibility = Visibility.Visible;
                return false;
            }
            if (IsLock && !string.IsNullOrEmpty(RelatedEntity) && (string.IsNullOrEmpty(ThisKey) || string.IsNullOrEmpty(RelatedKey)))
            {
                ErrorMessages = "Is RelatedEntity, fields is required ..";
                ErrorsVisibility = Visibility.Visible;
                return false;
            }
            if(IsLock && !string.IsNullOrEmpty(RelatedEntity) && !string.IsNullOrEmpty(ThisKey) && !string.IsNullOrEmpty(RelatedKey)) { 
                string[] thisKeys = ThisKey.Split(',');
                string[] relatedKeys = RelatedKey.Split(',');
                if(thisKeys.Length != 2 || relatedKeys.Length != 2) { 
					ErrorMessages = "This Key and Related Key should be in the format of 'Key1,Key2'";
					ErrorsVisibility = Visibility.Visible;
					return false;
				}

			}
            if (IsLock && string.IsNullOrEmpty(RelatedEntity) && string.IsNullOrEmpty(ThisKey)) 
            {
				ErrorMessages = "IsLock must parameter1";
				ErrorsVisibility = Visibility.Visible;
				return false;

			}



            if (IsClosed)
            {
                var errorMessagesBuilder = new StringBuilder();

                var closeTableCodeField = ObsList.FirstOrDefault(f => f.FieldName == CloseTableCode);
                if (closeTableCodeField == null)
                {
                    errorMessagesBuilder.AppendLine("The field 'CloseTableCode' is required.");
                }
                else if (closeTableCodeField.FieldDataType != "Text")
                {
                    errorMessagesBuilder.AppendLine("The field 'CloseTableCode' must be of type Text.");
                }

                var closeTableNameField = ObsList.FirstOrDefault(f => f.FieldName == CloseTableName);
                if (closeTableNameField == null)
                {
                    errorMessagesBuilder.AppendLine("The field 'CloseTableName' is required.");
                }
                else if (closeTableNameField.FieldDataType != "Text")
                {
                    errorMessagesBuilder.AppendLine("The field 'CloseTableName' must be of type Text.");
                }

                if (errorMessagesBuilder.Length > 0)
                {
                    ErrorMessages = errorMessagesBuilder.ToString().Trim();
                    ErrorsVisibility = Visibility.Visible;
                    return false;
                }
            }




            try
            {
                ErrorsVisibility = Visibility.Collapsed;
                ErrorMessages = string.Empty;

                this.ValidateObjectTable();
                if (ObsList != null)
                {
                    foreach (var item in ObsList)
                    {
                        this.ValidateObjectField(item);

						if (!DisplayLookUpFieldsList.Any(f => f.FieldName == item.FieldName) && !DisplayLocalLookUpFieldsList.Any(f => f.FieldName == item.FieldName))
						{
							item.DisplayInSearchWindowList = false;
						}
					}
                }
                else
                {
                    ObsList = new ObservableCollection<ObjectFieldsViewModel>();
                }
                if (!string.IsNullOrEmpty(QueryGroupCode) || !string.IsNullOrEmpty(QueryGroupName))
                {
                    if (QueriesObsList != null)
                    {
                        foreach (var item in QueriesObsList)
                        {
                            this.ValidateQueries(item);
                        }
                    }
                    else
                    {
                        QueriesObsList = new ObservableCollection<QueryViewModel>();
                    }

                    if (QueryColumnObsList != null)
                    {
                        foreach (var item in QueryColumnObsList)
                        {
                            this.ValidateQueryColumns(item);
                        }
                    }
                    else
                    {
                        QueryColumnObsList = new ObservableCollection<QueryColumnsViewModel>();
                        //ErrorMessages += "Query Columns Can't Be Empty !";
                    }


                    if (string.IsNullOrEmpty(QueryGroupCode))
                    {
                        ErrorMessages += "Query Group Code Is Required !";
                    }

                    if (string.IsNullOrEmpty(QueryGroupName))
                    {
                        ErrorMessages += "Query Group Name Is Required !";
                    }

                    if (string.IsNullOrEmpty(MenuButtonGroupType))
                    {
                        ErrorMessages += "Menu Button Group Type Is Required !";
                    }

                    if (string.IsNullOrEmpty(MenuButtonGroupName))
                    {
                        ErrorMessages += "Menu Button Group Name Is Required !";
                    }

                }

                if (ScreensObsList != null)
                {
                    foreach (var item in ScreensObsList)
                    {
                        this.ValidateScreens(item);
                    }
                }
                else
                {
                    ScreensObsList = new ObservableCollection<ScreensViewModel>();
                }

                if (TabsObsList != null)
                {
                    foreach (var item in TabsObsList)
                    {
                        this.ValidateTabs(item);
                    }
                }
                else
                {
                    TabsObsList = new ObservableCollection<TabsViewModel>();
                }
                if (MenuButtonsObsList != null)
                {
                    foreach (var item in MenuButtonsObsList)
                    {
                        this.ValidateMenuButtons(item);
                    }
                }
                else
                {
                    MenuButtonsObsList = new ObservableCollection<MenuButtonViewModel>();
                }
                if (rows == null)
                {
                    TabsObsList = new ObservableCollection<TabsViewModel>();
                }
                if (DataContractsObsList != null)
                {
                    //if (!(ErrorMessages.Contains("Cannot Find Foreign Entity") || ErrorMessages.Contains("Database Type is Required") || ErrorMessages.Contains("Database Schema is Required")))
                    //{
                    //    ErrorMessages = "";
                    //}
                    //else
                    //{
                    //    ErrorMessages = string.Join("\n\n", ErrorMessages.Split(new string[] { "\n\n" }, StringSplitOptions.None).Where(l => l.Contains("Cannot Find Foreign Entity") || l.Contains("Database Type is Required") || l.Contains("Database Schema is Required")).ToArray());
                    //    ErrorMessages = string.Join("\n", ErrorMessages.Split('\n').Where(l => l.Contains("Cannot Find Foreign Entity") || l.Contains("Database Type is Required") || l.Contains("Database Schema is Required") || l.Contains("Field Errors:") || string.IsNullOrEmpty(l)).ToArray());
                    //}

                    foreach (var item in DataContractsObsList)
                    {
                        if (item.DCFieldsObsList != null && item.DCFieldsObsList.Where(a => a.IsKey == true).Count() == 0)
                        {
                            ErrorMessages += item.DCName + " must have at least one key. ";
                        }
                        //if (item.DCFieldsObsList.Where(a => a.IsSpecialField == true).Count() > 0 && string.IsNullOrEmpty(item.ComputingPartnerName))
                        //{
                        //    ErrorMessages += item.DCName + " must have a Computing Partner For Translation. ";
                        //}
                    }
                }

                if(AdditionalTextCodesList != null)
                {
                    foreach (var item in AdditionalTextCodesList)
                    {
                        this.ValidateTextCodes(item);
                    }
                }
                if (AdditionalFeaturesList != null)
                {
                    foreach (var item in AdditionalFeaturesList)
                    {
                        this.ValidateFeatures(item);
                    }
                }

                if (ErrorMessages == "")
                {
                    //UpdateObsList(this);
                    succeeded = true;

                    XmlGeneratorClass.GenerateXmlFileFromTool(this);

                    XmlGeneratorClass.GenerateDXMLFileFromTool(this);

                    // App.CurrentControl.Close();
                    Environment.Exit(0);
                }
                else
                {
                    ErrorsVisibility = Visibility.Visible;
                }
            }
            catch (Exception ex)
            {
                string error = ex.Message + "\n" + ex.StackTrace != null ? ex.StackTrace : "";
                if (ex.InnerException != null && !string.IsNullOrEmpty(ex.InnerException.Message))
                {
                    error += "\n" + ex.InnerException.Message;

                    if (ex.InnerException.StackTrace != null)
                    {
                        error += "\n" + ex.InnerException.StackTrace != null ? ex.InnerException.StackTrace : "";
                    }
                }


                MessageBox.Show(error);

            }

            return succeeded;
        }

        private void ValidateObjectField(ObjectFieldsViewModel item)
        {
            StringBuilder str = new StringBuilder();
            if (item.Length <= 30)
            {
                if (item.FieldName.Length > 30)
                {
                    str.AppendLine("FieldName Couldn't be more than 30 char. ");
                }
            }

            if (string.IsNullOrEmpty(item.DefaultText))
            {
                str.AppendLine("Default Text is Required");
            }
            else if (ContainsHebrewCharacters(item.DefaultText))
            {
                str.AppendLine("Default Text cannot contain Hebrew characters");
            }
            if (!string.IsNullOrEmpty(item.ListLableDefaultText) && ContainsHebrewCharacters(item.ListLableDefaultText))
            {
                str.AppendLine("'List Lable Default Text' cannot contain Hebrew characters");
            }
            if (!string.IsNullOrEmpty(item.HelpTextDefaultText) && ContainsHebrewCharacters(item.HelpTextDefaultText))
            {
                str.AppendLine("'HelpText Default Text' cannot contain Hebrew characters");
            }
            if (!string.IsNullOrEmpty(item.ShortFieldLableDefaultText) && ContainsHebrewCharacters(item.ShortFieldLableDefaultText))
            {
                str.AppendLine("'Short Field Lable Default Text' cannot contain Hebrew characters");
            }
            if (string.IsNullOrEmpty(item.FieldName))
            {
                str.AppendLine("Field Name is Required");
            }
            if (string.IsNullOrEmpty(item.PMPropertyPath))
            {
                str.AppendLine("PM Property Path is Required");
            }
            if (string.IsNullOrEmpty(item.ListPropertyPath))
            {
                str.AppendLine("List Property Path is Required");
            }

            if (string.IsNullOrEmpty(item.FieldDataType))
            {
                str.AppendLine("Data Type is Required");
            }
            else
            {

                if (item.FieldDataType == "Text" || item.FieldDataType == "nText" || item.FieldDataType == "LookUp")
                {


                    if (string.IsNullOrEmpty(item.ToString()))
                    {
                        str.AppendLine("Max Length is Required");
                    }

                    if (string.IsNullOrEmpty(item.SystemMaxLength.ToString()))
                    {
                        str.AppendLine("System Max Length is Required");
                    }


                    if (item.MaxLength == 0)
                    {
                        str.AppendLine("Max Length is can't be zero!");
                    }

                    if (item.SystemMaxLength == 0)
                    {
                        str.AppendLine("Max Length is can't be zero!");
                    }


                }
                else if (item.FieldDataType == "PickList")
                {
                    if (string.IsNullOrEmpty(item.CustomPickListCode))
                    {
                        str.AppendLine("Custom Pick List Code is Required");
                    }
                }
                else if (item.FieldDataType == "Decimal" || item.FieldDataType == "Double" || item.FieldDataType == "SigDouble" || item.FieldDataType == "UnsDecimal")
                {
                    if (item.DigitsAfterPoint == null)
                    {
                        str.AppendLine("Digits After Point is Required");
                    }
                }
                else if (item.FieldDataType == "LookUp")
                {
                    if (string.IsNullOrEmpty(item.LookUpTableName))
                    {
                        str.AppendLine("Look Up Table Name is Required");
                    }
                }


                if (item.FieldDataType != "LookUp")
                {
                    item.LookUpTableName = null;
                }
            }

            if (item.DisplayInList)
            {
                if (string.IsNullOrEmpty(item.ListLableDefaultText))
                {
                    str.AppendLine("List Lable Default Text is Required");
                }
                if (string.IsNullOrEmpty(item.ValidForQuerySection1))
                {
                    str.AppendLine("Valid For Query Section 1 is Required");
                }
            }

            if (item.IsMulti)
            {
                if (string.IsNullOrEmpty(item.MultiTableName))
                {
                    str.AppendLine("Multi Table Name is Required");
                }
            }

            if (item.IsForeignKey)
            {
                if (string.IsNullOrEmpty(item.ForeignEntity))
                {
                    str.AppendLine("Foreign Entity is Required");
                }
                if (string.IsNullOrEmpty(item.NavigationPropertyName))
                {
                    str.AppendLine("Navigation Property Name is Required");
                }
            }

            if (item.DisplayOnLookUp)
            {
                if (item.DisplayInLookUpIndex == null)
                {
                    str.AppendLine("Display In Look Up Index is Required");
                }
                //if (string.IsNullOrEmpty(item.DisplayInLookupColumnSize))
                //{
                //    str.AppendLine("Display In Look Up Column Size is Required");
                //}
            }

            if (item.CanFilter)
            {
                if (string.IsNullOrEmpty(item.Operator))
                {
                    str.AppendLine("Operator is Required");
                }
            }

            if (item.IsForeignKey && !string.IsNullOrEmpty(item.ForeignEntity))
            {
                string foreignEntityFileName = App.GetForeignEntityFileName(item.ForeignEntity);

                if (!App.LXMLFilesPaths.Where(l => Path.GetFileName(l).ToLower() == foreignEntityFileName.ToLower() + ".lxml").Any() && !App.DXMLFilesPaths.Where(d => Path.GetFileName(d).ToLower() == foreignEntityFileName.ToLower() + ".dxml").Any())
                {
                    str.AppendLine("Cannot Find Foreign Entity " + item.ForeignEntity);
                }
            }

            if (!string.IsNullOrEmpty(ErrorMessages))
            {
                ErrorsVisibility = Visibility.Visible;
            }

            if (!string.IsNullOrEmpty(str.ToString()))
            {
                ErrorMessages = ErrorMessages + "\n" + item.FieldName + " Field Errors:\n" + str.ToString();
            }

            //ErrorMessages = ErrorMessages + "\n" + item.FieldName + " Field Errors:\n" + str.ToString();

            FirePropertyChanged("ErrorMessages");
        }

        private void ValidateQueries(QueryViewModel item)
        {
            StringBuilder str = new StringBuilder();
            if (string.IsNullOrEmpty(item.Code))
            {
                str.AppendLine("Code is Required");
            }
            if (string.IsNullOrEmpty(item.TextCode))
            {
                str.AppendLine("Default Text is Required");
            }
            else if (ContainsHebrewCharacters(item.TextCode))
            {
                str.AppendLine($"Queries, {item?.Code} - Text cannot contain Hebrew characters");
            }
            if (string.IsNullOrEmpty(item.QueryGroupCode))
            {
                str.AppendLine("Query Group Code is Required");
            }
            if (string.IsNullOrEmpty(item.ObjectTableName))
            {
                str.AppendLine("Object Table Name is Required");
            }
            if (string.IsNullOrEmpty(item.QuerySection))
            {
                str.AppendLine("Query Section is Required");
            }
            if (string.IsNullOrEmpty(item.DefaultSortName))
            {
                str.AppendLine("Default Sort Name Id is Required");
            }
            if (string.IsNullOrEmpty(item.DefaultSortDirection))
            {
                str.AppendLine("Default Sort Direction is Required");
            }


            if (!string.IsNullOrEmpty(ErrorMessages))
            {
                ErrorsVisibility = Visibility.Visible;
            }

            if (!string.IsNullOrEmpty(str.ToString()))
            {
                ErrorMessages = ErrorMessages + "\n" + str.ToString();
            }

            //ErrorMessages = ErrorMessages + "\n" + item.FieldName + " Field Errors:\n" + str.ToString();

            FirePropertyChanged("ErrorMessages");
        }

        private void ValidateQueryColumns(QueryColumnsViewModel item)
        {
            StringBuilder str = new StringBuilder();
            if (string.IsNullOrEmpty(item.QueryCode))
            {
                str.AppendLine("Query Code is Required");
            }
            if (string.IsNullOrEmpty(item.ObjectFieldName))
            {
                str.AppendLine("Object Field Name is Required");
            }
            if (item.ColumnWidth == 0)
            {
                item.ColumnWidth = 100;
            }
            if (!string.IsNullOrEmpty(ErrorMessages))
            {
                ErrorsVisibility = Visibility.Visible;
            }

            if (!string.IsNullOrEmpty(str.ToString()))
            {
                ErrorMessages = ErrorMessages + "\n" + str.ToString();
            }


            FirePropertyChanged("ErrorMessages");
        }

        private void ValidateScreens(ScreensViewModel item)
        {
            StringBuilder str = new StringBuilder();

            if (string.IsNullOrEmpty(item.Name))
            {
                str.AppendLine("Screen Name is Required");
            }
            if (!string.IsNullOrEmpty(ErrorMessages))
            {
                ErrorsVisibility = Visibility.Visible;
            }

            if (!string.IsNullOrEmpty(str.ToString()))
            {
                ErrorMessages = ErrorMessages + "\n" + str.ToString();
            }
            FirePropertyChanged("ErrorMessages");
        }

        private void ValidateTabs(TabsViewModel item)
        {
            StringBuilder str = new StringBuilder();

            if (string.IsNullOrEmpty(item.Name))
            {
                str.AppendLine("Tab Name is Required");
            }
            else if (ContainsHebrewCharacters(item.Name))
            {
                str.AppendLine($"Tabs, {item?.Code} - Tab Name cannot contain Hebrew characters");
            }
            if (!string.IsNullOrEmpty(ErrorMessages))
            {
                ErrorsVisibility = Visibility.Visible;
            }

            if (!string.IsNullOrEmpty(str.ToString()))
            {
                ErrorMessages = ErrorMessages + "\n" + str.ToString();
            }
            FirePropertyChanged("ErrorMessages");
        }

        private void ValidateMenuButtons(MenuButtonViewModel item)
        {
            StringBuilder str = new StringBuilder();

            if (string.IsNullOrEmpty(item.EventCode))
            {
                str.AppendLine("Event Code is Required");   
            }

            if (string.IsNullOrEmpty(item.DefaultText))
            {
                str.AppendLine("Default Text is Required");
            }
            else if (ContainsHebrewCharacters(item.DefaultText))
            {
                str.AppendLine($"MenuButton, {item?.EventCode} - Default Text cannot contain Hebrew characters");
            }

            if (string.IsNullOrEmpty(item.SelectedMenuButtonType))
            {
                str.AppendLine("Menu Button Type is Required");
            }

            if (!string.IsNullOrEmpty(ErrorMessages))
            {
                ErrorsVisibility = Visibility.Visible;
            }

            if (!string.IsNullOrEmpty(str.ToString()))
            {
                ErrorMessages = ErrorMessages + "\n" + str.ToString();
            }
            FirePropertyChanged("ErrorMessages");
        }

        private void ValidateObjectTable()
        {
            StringBuilder str = new StringBuilder();

            if (string.IsNullOrEmpty(DefaultText))
            {
                str.AppendLine("Default Text is Required");
            }
            else if (ContainsHebrewCharacters(DefaultText))
            {
                str.AppendLine("Default Text cannot contain Hebrew characters");
            }

            if (string.IsNullOrEmpty(KeyPropertyPath))
            {
                str.AppendLine("Key Property Path is Required");
            }
            if (string.IsNullOrEmpty(SortingByObjectField))
            {
                str.AppendLine("Sorting By ObjectField is Required");
            }
            if (IsComposition && string.IsNullOrEmpty(ParentTableName))
            {
                str.AppendLine("Parent Table Name is Required");
            }
            if (IsNewWizard && string.IsNullOrEmpty(NewWizardControlName))
            {
                str.AppendLine("New Wizard Control Name is Required");
            }
            if (string.IsNullOrEmpty(ObjectTableName))
            {
                str.AppendLine("Object Table Name is Required");
            }
            if (string.IsNullOrEmpty(DBTableName))
            {
                str.AppendLine("DB Table Name is Required");
            }
            if (string.IsNullOrEmpty(ObjectTableSingular))
            {
                str.AppendLine("Object Table Singular is Required");
            }
            if (string.IsNullOrEmpty(ObjectTablePlural))
            {
                str.AppendLine("Object Table Plural is Required");
            }
            if (string.IsNullOrEmpty(ObjectTableTypeCode))
            {
                str.AppendLine("Object Table Type is Required");
            }
            if (IsClosed)
            {
                if (string.IsNullOrEmpty(CloseTableCode) || string.IsNullOrEmpty(CloseTableName))
                {
                    str.AppendLine("Close Table Code and Name are Required");
                }
            }
            if (!IsComposition && !IsMain && !IsClosed)
            {
                str.AppendLine("You should select table type (IsComposition or IsMain or IsClosed)");
            }

            ErrorMessages = str.ToString();
            if (!string.IsNullOrEmpty(ErrorMessages))
            {
                ErrorsVisibility = Visibility.Visible;
            }

            FirePropertyChanged("ErrorMessages");
        }



        private void ValidateTextCodes(TextCodesViewModel item)
        {
            StringBuilder str = new StringBuilder();

            if (!string.IsNullOrEmpty(item.DefaultText) && ContainsHebrewCharacters(item.DefaultText))
            {
                str.AppendLine($"TextCodes, {item?.Code} - Default Text cannot contain Hebrew characters");
            }
            if (!string.IsNullOrEmpty(ErrorMessages))
            {
                ErrorsVisibility = Visibility.Visible;
            }

            if (!string.IsNullOrEmpty(str.ToString()))
            {
                ErrorMessages = ErrorMessages + "\n" + str.ToString();
            }
            FirePropertyChanged("ErrorMessages");
        }

        private void ValidateFeatures(FeaturesViewModel item)
        {
            StringBuilder str = new StringBuilder();

            if (!string.IsNullOrEmpty(item.FeatureDefaultText) && ContainsHebrewCharacters(item.FeatureDefaultText))
            {
                str.AppendLine($"Features, {item?.Code} - Default Text cannot contain Hebrew characters");
            }
            if (!string.IsNullOrEmpty(ErrorMessages))
            {
                ErrorsVisibility = Visibility.Visible;
            }

            if (!string.IsNullOrEmpty(str.ToString()))
            {
                ErrorMessages = ErrorMessages + "\n" + str.ToString();
            }
            FirePropertyChanged("ErrorMessages");
        }

        private bool ContainsHebrewCharacters(string text)
        {
            return Regex.IsMatch(text, @"[\u0590-\u05FF]");
        }

        private string queryGroupCode;
        public string QueryGroupCode
        {
            get
            {
                if (string.IsNullOrEmpty(queryGroupCode))
                {
                    var temp = Guid.NewGuid().ToString().Substring(0, 4);//.Replace("-", ""); 
                    return temp;// queryGroupCode;
                }
                return queryGroupCode;
            }
            set
            {
                queryGroupCode = value;
                FirePropertyChanged("QueryGroupCode");
            }
        }

        private string queryGroupName = " Query Group";
        public string QueryGroupName
        {
            get
            {
                if (string.IsNullOrEmpty(queryGroupName))
                {
                    return ObjectTableName + " Query Group";// queryGroupName;
                }
                else
                    return queryGroupName;
            }
            set
            {
                queryGroupName = value;
                FirePropertyChanged("QueryGroupName");
            }
        }

        private string queryGroupCode1;
        public string QueryGroupCode1
        {
            get
            {
                if (string.IsNullOrEmpty(queryGroupCode1))
                {
                    var temp = Guid.NewGuid().ToString().Substring(0, 4);//.Replace("-", ""); 
                    return temp;// queryGroupCode;
                }
                return queryGroupCode1;
            }
            set
            {
                queryGroupCode1 = value;
                FirePropertyChanged("QueryGroupCode1");
            }
        }

        private string queryGroupName1 = " Query Group";
        public string QueryGroupName1
        {
            get
            {
                return queryGroupName1;// queryGroupName;
            }
            set
            {
                queryGroupName1 = value;
                FirePropertyChanged("QueryGroupName1");
            }
        }

        private string menuButtonGroupName = "EditButtonsGroup";
        public string MenuButtonGroupName
        {
            get
            {
                if (menuButtonGroupName == "EditButtonsGroup")
                    return ObjectTableName + "EditButtonsGroup";
                else
                    return menuButtonGroupName;
            }
            set
            {
                menuButtonGroupName = value;
                FirePropertyChanged("MenuButtonGroupName");
            }
        }

        private string menuButtonGroupType = "Edit";
        public string MenuButtonGroupType
        {
            get
            {
                if (menuButtonGroupType == "Edit")
                    return ObjectTableName + "Edit";
                else
                    return menuButtonGroupType;
            }
            set
            {
                menuButtonGroupType = value;
                FirePropertyChanged("MenuButtonGroupType");
            }
        }
        public int CustomFieldsCount { get; set; }
        public RelayCommand AddQueryCommand
        {
            get { return new RelayCommand(() => this.AddQueryMethod()); }
        }
        public Window QueryWindow = new Window();
        QueriesUserControl QueryControl;
        private void AddQueryMethod()
        {
            ErrorMessages = "";
            if (string.IsNullOrEmpty(QueryGroupCode))
            {
                ErrorMessages += " Query Group Code Is Required !";
                ErrorsVisibility = Visibility.Visible;
            }
            else if (string.IsNullOrEmpty(QueryGroupName))
            {
                ErrorMessages += " Query Group Name Is Required !";
                ErrorsVisibility = Visibility.Visible;
            }
            else
            {
                ErrorsVisibility = Visibility.Collapsed;
                QueryViewModel model = new QueryViewModel(this, true);
                model.ObjectTableName = ObjectTableName;
                model.QuerySection = ObjectTableName;
                model.QueryGroupCode = QueryGroupCode;
                //model.SetControlFieldsList(model);
                QueryControl = new QueriesUserControl();
                QueryControl.DataContext = model;

                QueryWindow = new Window();
                QueryWindow.Title = "New Query";
                QueryWindow.Content = QueryControl;
                QueryWindow.Show();
            }

        }

        public RelayCommand<QueryViewModel> RemoveQueryCommand
        {
            get { return new RelayCommand<QueryViewModel>(m => this.RemoveQueryMethod(m)); }
        }

        private void RemoveQueryMethod(QueryViewModel DelQ)
        {
            QueriesObsList.Remove(DelQ);
            FirePropertyChanged("QueriesObsList");
        }

        private QueryViewModel selectedQuery;
        public QueryViewModel SelectedQuery
        {
            get
            {
                return selectedQuery;
            }
            set
            {
                selectedQuery = value;
                FirePropertyChanged("SelectedQuery");

                if (value != null && value.QueryFiltersObsList != null)
                {
                    this.SelectedQueryFilter = value.QueryFiltersObsList.FirstOrDefault();
                }
                else
                    this.SelectedQueryFilter = null;
            }
        }

        private Visibility queryDetailsEditControlVisibility;
        public Visibility QueryDetailsEditControlVisibility
        {
            get
            {
                return queryDetailsEditControlVisibility;
            }
            set
            {
                queryDetailsEditControlVisibility = value;
                FirePropertyChanged("QueryDetailsEditControlVisibility");
            }
        }

        private Visibility tablesDataVisibility = Visibility.Collapsed;
        public Visibility TablesDataVisibility
        {
            get
            {
                return tablesDataVisibility;
            }
            set
            {
                tablesDataVisibility = value;
                FirePropertyChanged("TablesDataVisibility");
            }
        }

        public void UpdateQueriesList(QueryViewModel item)
        {
            if (QueriesObsList == null)
            {
                QueriesObsList = new ObservableCollection<QueryViewModel>();
            }
            QueriesObsList.Add(item);

            FirePropertyChanged("QueriesObsList");
            //this.SetLookUpFieldsList(); 
            QueryDetailsEditControlVisibility = (QueriesObsList.Count == 0) ? Visibility.Collapsed : Visibility.Visible;
            this.SelectedQuery = item;

        }


        private DataGrid cLoseTableDataGrid;
        public DataGrid CLoseTableDataGrid
        {
            get
            {
                if (cLoseTableDataGrid == null)
                {
                    cLoseTableDataGrid = new MyGrid() { Width = 1875, MinRowHeight = 35 };
                    cLoseTableDataGrid.IsReadOnly = true;
                    Binding bnd = new Binding();
                    cLoseTableDataGrid.SelectionChanged += cLoseTableDataGrid_SelectionChanged;
                    cLoseTableDataGrid.AutoGenerateColumns = false;
                    cLoseTableDataGrid.CanUserResizeColumns = true;
                }
                return cLoseTableDataGrid;
            }
            set
            {
                cLoseTableDataGrid = value;
                FirePropertyChanged("CLoseTableDataGrid");
            }
        }

        private Row selectedRow;
        public Row SelectedRow
        {
            get
            {
                return selectedRow;
            }
            set
            {
                selectedRow = value;
                FirePropertyChanged("SelectedRow");
            }
        }
        void cLoseTableDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SelectedRow = CLoseTableDataGrid.SelectedItem as Row;
        }

        public RelayCommand RemoveTableDataCommand
        {
            get { return new RelayCommand(() => this.RemoveTableDataMethod()); }
        }

        private void RemoveTableDataMethod()
        {
            if (SelectedRow != null)
            {
                rows.Remove(SelectedRow);
            }
        }

        private MyGrid generatedGrid;
        public MyGrid GeneratedGrid
        {
            get
            {
                if (generatedGrid == null)
                {
                    generatedGrid = new MyGrid() { Width = 1875, MinRowHeight = 35 };
                    generatedGrid.CanUserResizeColumns = true;
                    generatedGrid.ColumnReordered += generatedGrid_ColumnReordered;
                    generatedGrid.ColumnWidthChanged += generatedGrid_ColumnWidthChanged;
                }
                return generatedGrid;
            }
            set
            {
                generatedGrid = value;
                FirePropertyChanged("GeneratedGrid");
            }
        }

        void generatedGrid_ColumnWidthChanged(MyArgs args)
        {
            this.SelectedQuery.QueryColumnObsList.Where(a => a.ObjectFieldName == args.Column.Header.ToString()).FirstOrDefault().ColumnWidth = (int)args.Column.ActualWidth;
        }

        void generatedGrid_ColumnReordered(object sender, DataGridColumnEventArgs e)
        {
            foreach (var item in generatedGrid.Columns)
            {
                this.SelectedQuery.QueryColumnObsList.Where(a => a.ObjectFieldName == item.Header.ToString()).FirstOrDefault().IndexOrder = item.DisplayIndex;//.IndexOf(generatedGrid.Columns.Where(a => a.Header.ToString() == item.ObjectFieldName).FirstOrDefault());
            }

            SelectedQuery.Refresh();
        }


        public RelayCommand<QueryViewModel> AddQueryColumnCommand
        {
            get { return new RelayCommand<QueryViewModel>(Qmodel => this.AddQueryColumnMethod(Qmodel)); }
        }
        public Window QueryColumnWindow = new Window();
        QueryColumnsUserControl QueryColumnControl;
        private void AddQueryColumnMethod(QueryViewModel QModel)
        {
            SelectedQuery = QModel;
            ErrorMessages = "";
            if (SelectedQuery == null)
            {
                ErrorMessages += " You should select query !";
                ErrorsVisibility = Visibility.Visible;
            }
            else
            {
                ErrorsVisibility = Visibility.Collapsed;
                QueryColumnsViewModel model = new QueryColumnsViewModel(this, SelectedQuery, true);
                //model.ObjectTableName = ObjectTableName;
                //model.SetControlFieldsList(model);
                QueryColumnControl = new QueryColumnsUserControl();
                QueryColumnControl.DataContext = model;

                QueryColumnWindow = new Window();
                QueryColumnWindow.Title = "New Query Column";
                QueryColumnWindow.Content = QueryColumnControl;
                QueryColumnWindow.Show();
            }

        }

        public RelayCommand<QueryViewModel> RemoveQueryColumnCommand
        {
            get { return new RelayCommand<QueryViewModel>(Qmodel => this.RemoveQueryColumnMethod(Qmodel)); }
        }

        public RelayCommand<QueryViewModel> GenerateQueryColumnCommand
        {
            get { return new RelayCommand<QueryViewModel>(Qmodel => this.GenerateQueryColumnMethod(Qmodel)); }
        }

        private void GenerateQueryColumnMethod(QueryViewModel QModel)
        {
            SelectedQuery = QModel;
            ErrorMessages = "";
            if (SelectedQuery == null)
            {
                ErrorMessages += " You should select query !";
                ErrorsVisibility = Visibility.Visible;
            }
            else
            {
                var temp = ObsList.Where(a => a.DisplayInList == true);
                SelectedQuery.QueryColumnObsList = new ObservableCollection<QueryColumnsViewModel>();
                foreach (var item in temp)
                {
                    QueryColumnsViewModel model = new QueryColumnsViewModel(this, SelectedQuery, true);
                    model.ColumnWidth = 100;
                    model.ObjectFieldName = item.FieldName;
                    SelectedQuery.UpdateQueryColumnsList(model);
                }
                ErrorsVisibility = Visibility.Collapsed;
                //model.ObjectTableName = ObjectTableName;
                //model.SetControlFieldsList(model);

            }

        }


        private void RemoveQueryColumnMethod(QueryViewModel QModel)
        {
            QModel.QueryColumnObsList.Remove(SelectedQueryColumn);
            FirePropertyChanged("QueryColumnObsList");
        }

        public RelayCommand<QueryViewModel> MoveUpCommand
        {
            get { return new RelayCommand<QueryViewModel>(Qmodel => this.MoveUpMethod(Qmodel)); }
        }

        private void MoveUpMethod(QueryViewModel Qmodel)
        {
            int index = QueriesObsList.IndexOf(Qmodel);
            if (index > 0)
            {
                QueriesObsList.Remove(Qmodel);
                QueriesObsList.Insert(index - 1, Qmodel);
                SelectedQuery = Qmodel;
            }
        }

        public RelayCommand<QueryViewModel> MoveDownCommand
        {
            get { return new RelayCommand<QueryViewModel>(Qmodel => this.MoveDownMethod(Qmodel)); }
        }

        private void MoveDownMethod(QueryViewModel Qmodel)
        {
            int index = QueriesObsList.IndexOf(Qmodel);
            if (index < QueriesObsList.Count - 1)
            {
                QueriesObsList.Remove(Qmodel);
                QueriesObsList.Insert(index + 1, Qmodel);
                SelectedQuery = Qmodel;
            }
        }


        public RelayCommand<TabsViewModel> MoveTabUpCommand
        {
            get { return new RelayCommand<TabsViewModel>(Tmodel => this.MoveTabUpMethod(Tmodel)); }
        }

        private void MoveTabUpMethod(TabsViewModel Tmodel)
        {
            int index = TabsObsList.IndexOf(Tmodel);

            if (index > 0)
            {
                int selectedIndexOrder = Tmodel.IndexOrder;
                int prevIndexOrder = TabsObsList[index - 1].IndexOrder;


                TabsObsList.Remove(Tmodel);
                TabsObsList.Insert(index - 1, Tmodel);

                Tmodel.IndexOrder = prevIndexOrder;
                TabsObsList[index].IndexOrder = selectedIndexOrder;

                SelectedTab = Tmodel;
            }
        }

        public RelayCommand<TabsViewModel> MoveTabDownCommand
        {
            get { return new RelayCommand<TabsViewModel>(Tmodel => this.MoveTabDownMethod(Tmodel)); }
        }

        private void MoveTabDownMethod(TabsViewModel Tmodel)
        {
            int index = TabsObsList.IndexOf(Tmodel);
            if (index < TabsObsList.Count - 1)
            {
                int selectedIndexOrder = Tmodel.IndexOrder;
                int nextIndexOrder = TabsObsList[index + 1].IndexOrder;


                TabsObsList.Remove(Tmodel);
                TabsObsList.Insert(index + 1, Tmodel);

                Tmodel.IndexOrder = nextIndexOrder;
                TabsObsList[index].IndexOrder = selectedIndexOrder;

                SelectedTab = Tmodel;
            }
        }

        public RelayCommand<QueryViewModel> MoveColumnUpCommand
        {
            get { return new RelayCommand<QueryViewModel>(Qmodel => this.MoveColumnUpMethod(Qmodel)); }
        }

        private void MoveColumnUpMethod(QueryViewModel Qmodel)
        {
            var tempSelected = SelectedQueryColumn;
            int index = Qmodel.QueryColumnObsList.IndexOf(tempSelected);
            if (index > 0)
            {
                Qmodel.QueryColumnObsList.Remove(tempSelected);
                Qmodel.QueryColumnObsList.Insert(index - 1, tempSelected);
                SelectedQueryColumn = tempSelected;
            }
        }

        public RelayCommand<QueryViewModel> MoveColumnDownCommand
        {
            get { return new RelayCommand<QueryViewModel>(Qmodel => this.MoveColumnDownMethod(Qmodel)); }
        }

        private void MoveColumnDownMethod(QueryViewModel Qmodel)
        {
            var tempSelected = SelectedQueryColumn;
            int index = Qmodel.QueryColumnObsList.IndexOf(tempSelected);
            if (index < Qmodel.QueryColumnObsList.Count - 1)
            {
                Qmodel.QueryColumnObsList.Remove(tempSelected);
                Qmodel.QueryColumnObsList.Insert(index + 1, tempSelected);
                SelectedQueryColumn = tempSelected;
            }
        }


        private Visibility queryColumnDetailsEditControlVisibility;
        public Visibility QueryColumnDetailsEditControlVisibility
        {
            get
            {
                return queryColumnDetailsEditControlVisibility;
            }
            set
            {
                queryColumnDetailsEditControlVisibility = value;
                FirePropertyChanged("QueryColumnDetailsEditControlVisibility");
            }
        }

        public Visibility ClosedTablesFieldsVisibility
        {
            get
            {
                if (IsClosed)
                {
                    return Visibility.Visible;
                }
                else
                {
                    //CloseTableCode = "";
                    //CloseTableName = "";
                    return Visibility.Collapsed;
                }

            }
        }

        private Visibility queryFilterDetailsEditControlVisibility;
        public Visibility QueryFilterDetailsEditControlVisibility
        {
            get
            {
                return queryFilterDetailsEditControlVisibility;
            }
            set
            {
                queryFilterDetailsEditControlVisibility = value;
                FirePropertyChanged("QueryFilterDetailsEditControlVisibility");
            }
        }

        private QueryColumnsViewModel selectedQueryColumn;
        public QueryColumnsViewModel SelectedQueryColumn
        {
            get
            {
                return selectedQueryColumn;
            }
            set
            {
                selectedQueryColumn = value;
                FirePropertyChanged("SelectedQueryColumn");

             
            }
        }

        private QueryFiltersViewModel selectedQueryFilter;
        public QueryFiltersViewModel SelectedQueryFilter
        {
            get
            {
                return selectedQueryFilter;
            }
            set
            {
                selectedQueryFilter = value;
                FirePropertyChanged("SelectedQueryFilter");
            }
        }

        public RelayCommand AddFilterCommand
        {
            get { return new RelayCommand(() => this.AddFilterMethod()); }
        }
        public Window FilterWindow = new Window();
        QueryFiltersUserControl QueryFilterControl;
        private void AddFilterMethod()
        {
            ErrorMessages = "";
            if (SelectedQuery == null)
            {
                ErrorMessages += " A Query Should Be Selected !";
                ErrorsVisibility = Visibility.Visible;
            }
            else
            {
                ErrorsVisibility = Visibility.Collapsed;
                QueryFiltersViewModel model = new QueryFiltersViewModel(this, SelectedQuery, true);
                model.QueryCode = SelectedQuery.Code;
                //model.SetControlFieldsList(model);
                QueryFilterControl = new QueryFiltersUserControl();
                QueryFilterControl.DataContext = model;

                FilterWindow = new Window();
                FilterWindow.Title = "New Query Filter";
                FilterWindow.Content = QueryFilterControl;
                FilterWindow.Show();
            }

        }

        public RelayCommand<QueryViewModel> RemoveFilterCommand
        {
            get { return new RelayCommand<QueryViewModel>(m => this.RemoveFilterMethod(m)); }
        }
        private void RemoveFilterMethod(QueryViewModel QModel)
        {
            QModel.QueryFiltersObsList.Remove(SelectedQueryFilter);
            FirePropertyChanged("QueryFiltersObsList");
        }

        public RelayCommand<DataContractFieldViewModel> DCFieldMoveLeftCommand
        {
            get { return new RelayCommand<DataContractFieldViewModel>(Qmodel => this.DCFieldMoveLeftMethod(Qmodel)); }
        }

        private void DCFieldMoveLeftMethod(DataContractFieldViewModel Qmodel)
        {
            if (SelectedDataContract.SelectedDCField != null && !SelectedDataContract.SelectedDCField.IsKey && SelectedDataContract.SelectedDCField.FieldName != "Tenant")
            {
                //int index = DBFieldsObsList.IndexOf(Qmodel);
                //if (index > 0)
                //{
                var RemovedItem = SelectedDataContract.DCFieldsObsList.Where(a => a.FieldName == SelectedDataContract.SelectedDCField.FieldName).FirstOrDefault();
                var InsertItem = ObsList.Where(a => a.FieldName == SelectedDataContract.SelectedDCField.FieldName).FirstOrDefault();
                SelectedDataContract.DCFieldsObsList.Remove(RemovedItem);
                SelectedDataContract.DBFieldsObsList.Add(InsertItem);
                SelectedDataContract.SelectedDBField = InsertItem;
                SelectedDataContract.FireDBFieldsObsList();
                SelectedDataContract.FireDCFieldsObsList();
                //FirePropertyChanged("AllMenuButtonsObsList");
                //}
            }
        }

        public RelayCommand<ObjectFieldsViewModel> DCFieldMoveRightCommand
        {
            get { return new RelayCommand<ObjectFieldsViewModel>(Qmodel => this.DCFieldMoveRightMethod(Qmodel)); }
        }

        private void DCFieldMoveRightMethod(ObjectFieldsViewModel Qmodel)
        {
            if (SelectedDataContract.SelectedDBField != null)
            {
                var InsertItem = new DataContractFieldViewModel();
                InsertItem.FieldName = SelectedDataContract.SelectedDBField.FieldName;
                InsertItem.IsMulti = SelectedDataContract.SelectedDBField.IsMulti;
                InsertItem.MultiTableName = SelectedDataContract.SelectedDBField.MultiTableName;
                if (SelectedDataContract.SelectedDBField.IsMulti == true)
                {
                    InsertItem.DCFieldName = SelectedDataContract.SelectedDBField.FieldName;
                    InsertItem.FieldDataType = "List<" + SelectedDataContract.SelectedDBField.MultiTableName + ">";
                    InsertItem.DCVersion = "ApiV1";
                    InsertItem.IsCustomType = true;
                }
                else if (SelectedDataContract.SelectedDBField.FieldDataType == "LookUp")
                {
                    if (SelectedDataContract.SelectedDBField.FieldName.Contains("Id"))
                    {
                        InsertItem.DCFieldName = SelectedDataContract.SelectedDBField.FieldName.Replace("Id", "");
                    }
                    else
                    {
                        InsertItem.DCFieldName = SelectedDataContract.SelectedDBField.FieldName;
                    }
                    InsertItem.FieldDataType = SelectedDataContract.SelectedDBField.FieldName.Replace("Id", "");// +"ApiV1";
                    InsertItem.DCVersion = "ApiV1";
                    InsertItem.IsCustomType = true;
                }
                else
                {
                    if (SelectedDataContract.SelectedDBField.FieldName == "Code")
                    {
                        InsertItem.DCFieldName = "LogitudeCode";
                    }
                    else
                    {
                        InsertItem.DCFieldName = SelectedDataContract.SelectedDBField.FieldName;
                    }
                    InsertItem.FieldDataType = SelectedDataContract.SelectedDBField.FieldDataType;
                    InsertItem.IsCustomType = false;
                }
                InsertItem.IsKey = SelectedDataContract.SelectedDBField.IsPrimaryKey;// false;
                InsertItem.IsSpecialField = false;
                InsertItem.DCVersion = null;
                InsertItem.IsNullable = SelectedDataContract.SelectedDBField.IsNullable;

                if (SelectedDataContract.DCFieldsObsList == null)
                {
                    SelectedDataContract.DCFieldsObsList = new ObservableCollection<DataContractFieldViewModel>();
                }
                SelectedDataContract.DCFieldsObsList.Add(InsertItem);
                var RemovedItem = SelectedDataContract.DBFieldsObsList.Where(a => a.FieldName == SelectedDataContract.SelectedDBField.FieldName).FirstOrDefault();
                SelectedDataContract.DBFieldsObsList.Remove(RemovedItem);
                //DBFieldsObsList.Insert(DBFieldsObsList.Count - 1, InsertItem);
                //SelectedDBField = InsertItem;
                SelectedDataContract.FireDBFieldsObsList();
                SelectedDataContract.FireDCFieldsObsList();
                //int index = DBFieldsObsList.IndexOf(Qmodel);
                //if (index > 0)
                //{
                //    var RemovedItem = SelectedDataContract.DCFieldsObsList.Where(a => a.FieldName == Qmodel.FieldName).FirstOrDefault();
                //    SelectedDataContract.DCFieldsObsList.Remove(RemovedItem);
                //    DBFieldsObsList.Insert(index - 1, Qmodel);
                //    SelectedDBField = Qmodel;
                //    FirePropertyChanged("DBFieldsObsList");
                //    //FirePropertyChanged("AllMenuButtonsObsList");
                //}
            }
        }

        public RelayCommand<MenuButtonViewModel> Col1MoveLeftCommand
        {
            get { return new RelayCommand<MenuButtonViewModel>(Qmodel => this.Col1MoveLeftMethod(Qmodel)); }
        }

        private void Col1MoveLeftMethod(MenuButtonViewModel Qmodel)
        {
            if (Qmodel != null)
            {
                int index = MenuButtonsObsList.IndexOf(Qmodel);
                if (index > 0)
                {
                    MenuButtonsObsList.Remove(Qmodel);
                    MenuButtonsObsList.Insert(index - 1, Qmodel);
                    SelectedMenuButton = Qmodel;
                    FirePropertyChanged("MenuButtonsObsList");
                    //FirePropertyChanged("AllMenuButtonsObsList");
                }
            }
        }

        public RelayCommand<MenuButtonViewModel> Col1MoveRightCommand
        {
            get { return new RelayCommand<MenuButtonViewModel>(Qmodel => this.Col1MoveRightMethod(Qmodel)); }
        }

        private void Col1MoveRightMethod(MenuButtonViewModel Qmodel)
        {
            if (Qmodel != null)
            {
                int index = MenuButtonsObsList.IndexOf(Qmodel);
                if (index < MenuButtonsObsList.Count - 1)
                {
                    MenuButtonsObsList.Remove(Qmodel);
                    MenuButtonsObsList.Insert(index + 1, Qmodel);
                    SelectedMenuButton = Qmodel;
                    FirePropertyChanged("MenuButtonsObsList");
                    //FirePropertyChanged("AllMenuButtonsObsList");
                }
            }
        }

        public RelayCommand<MenuButtonViewModel> Col1MoveUpCommand
        {
            get { return new RelayCommand<MenuButtonViewModel>(Qmodel => this.Col1MoveUpMethod(Qmodel)); }
        }

        private void Col1MoveUpMethod(MenuButtonViewModel Qmodel)
        {
            if (Qmodel != null)
            {
                int index = SelectedMenuButton.MenuButtonItems.IndexOf(Qmodel);
                if (index > 0)
                {
                    int selectedIndexOrder = Qmodel.IndexOrder;
                    int prevIndexOrder = SelectedMenuButton.MenuButtonItems[index - 1].IndexOrder;

                    SelectedMenuButton.MenuButtonItems.Remove(Qmodel);
                    SelectedMenuButton.MenuButtonItems.Insert(index - 1, Qmodel);

                    Qmodel.IndexOrder = prevIndexOrder;
                    SelectedMenuButton.MenuButtonItems[index].IndexOrder = selectedIndexOrder;

                    SelectedMenuItem = Qmodel;
                    //FirePropertyChanged("MenuButtonsObsList");
                    //FirePropertyChanged("SubMenuButtonsObsList");
                }
            }
        }

        public RelayCommand<MenuButtonViewModel> Col1MoveDownCommand
        {
            get { return new RelayCommand<MenuButtonViewModel>(Qmodel => this.Col1MoveDownMethod(Qmodel)); }
        }

        public bool DisableSearchBox { get;  set; }
        public bool HasDocuments { get;  set; }
        public bool IsLookUp { get; set; }
        public string SearchFields { get; set; }

        private void Col1MoveDownMethod(MenuButtonViewModel Qmodel)
        {
            if (Qmodel != null)
            {
                int index = SelectedMenuButton.MenuButtonItems.IndexOf(Qmodel);
                if (index < SelectedMenuButton.MenuButtonItems.Count - 1)
                {
                    int selectedIndexOrder = Qmodel.IndexOrder;
                    int nextIndexOrder = SelectedMenuButton.MenuButtonItems[index + 1].IndexOrder;

                    SelectedMenuButton.MenuButtonItems.Remove(Qmodel);
                    SelectedMenuButton.MenuButtonItems.Insert(index + 1, Qmodel);

                    Qmodel.IndexOrder = nextIndexOrder;
                    SelectedMenuButton.MenuButtonItems[index].IndexOrder = selectedIndexOrder;

                    SelectedMenuItem = Qmodel;
                    //FirePropertyChanged("MenuButtonsObsList");
                    //FirePropertyChanged("SubMenuButtonsObsList");
                }
            }
        }

    }

    public class MyGrid : DataGrid
    {
        public delegate void ColumnWidthPropertyChangedHandler(MyArgs args);
        public event ColumnWidthPropertyChangedHandler ColumnWidthChanged;
        protected override void OnInitialized(EventArgs e)
        {

            EventHandler widthPropertyChangedHandler = (sender, x) =>
            {
                if (ColumnWidthChanged != null)
                    ColumnWidthChanged(new MyArgs() { Column = (DataGridTextColumn)sender });
            };
            var sortDirectionPropertyDescriptor = DependencyPropertyDescriptor.FromProperty(DataGridTextColumn.SortDirectionProperty, typeof(DataGridTextColumn));
            var widthPropertyDescriptor = DependencyPropertyDescriptor.FromProperty(DataGridTextColumn.WidthProperty, typeof(DataGridTextColumn));

            Loaded += (sender, x) =>
            {
                foreach (var column in Columns)
                {
                    widthPropertyDescriptor.AddValueChanged(column, widthPropertyChangedHandler);
                }
            };
            Unloaded += (sender, x) =>
            {
                foreach (var column in Columns)
                {
                    widthPropertyDescriptor.RemoveValueChanged(column, widthPropertyChangedHandler);
                }
            };

            base.OnInitialized(e);
        }

        public void AddWidthChangeEvent(DataGridColumn column)
        {
            EventHandler widthPropertyChangedHandler = (sender, x) =>
            {
                if (ColumnWidthChanged != null)
                    ColumnWidthChanged(new MyArgs() { Column = (DataGridColumn)sender });
            };

            var sortDirectionPropertyDescriptor = DependencyPropertyDescriptor.FromProperty(DataGridColumn.SortDirectionProperty, typeof(DataGridColumn));
            var widthPropertyDescriptor = DependencyPropertyDescriptor.FromProperty(DataGridColumn.WidthProperty, typeof(DataGridColumn));


            widthPropertyDescriptor.AddValueChanged(column, widthPropertyChangedHandler);
        }



    }

    public class MyArgs : EventArgs
    {
        public DataGridColumn Column { get; set; }
    }

    public class FieldsValues
    {
        public Dictionary<string, object> FieldsDictionary = new Dictionary<string, object>();

        public object GetFieldValue(string key)
        {
            if (FieldsDictionary.Keys.Contains(key))
            {
                return FieldsDictionary[key];
            }
            else
            {
                return null;
            }
        }

        public void SetFieldValue(string key, object value)
        {
            if (FieldsDictionary.Keys.Contains(key))
            {
                FieldsDictionary[key] = value;
            }
            else
            {
                FieldsDictionary.Add(key, value);
            }
        }





    }

}



