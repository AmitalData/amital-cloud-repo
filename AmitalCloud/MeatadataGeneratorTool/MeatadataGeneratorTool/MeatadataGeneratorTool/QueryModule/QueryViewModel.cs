using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace MeatadataGeneratorTool.QueryModule
{
    public class SortDirections : PropertyChangedImplementation
    {
        public string name;
        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                name = value;
                FirePropertyChanged("Name");
            }
        }
    }
    public class QueryViewModel : PropertyChangedImplementation
    {
        ObjectTableViewModel viewModel;
        public ObservableCollection<SortDirections> SortDirections
        {
            get
            {
                return new ObservableCollection<SortDirections>()
                {
                    new SortDirections(){Name="Desending"},
                    new SortDirections(){Name="Ascending"}
                };
            }
        }
        private ObservableCollection<ObjectFieldsViewModel> obsList;
        public ObservableCollection<ObjectFieldsViewModel> ObsList
        {
            get
            {
                var temp = viewModel.ObsList.Where(a => a.DisplayInList == true);
                ObservableCollection<ObjectFieldsViewModel> ObsListtemp = new ObservableCollection<ObjectFieldsViewModel>();
                foreach (var item in temp)
                {
                    ObsListtemp.Add(item);
                }
                obsList = ObsListtemp;
                return obsList;
            }
            set
            {
                obsList = value;
                FirePropertyChanged("ObsList");
            }
        }
        bool IsNew;
        public QueryViewModel(ObjectTableViewModel OTViewModel, bool IsNew)
        {
            this.IsNew = IsNew;
            viewModel = OTViewModel;
            //var temp = OTViewModel.ObsList.Where(a => a.DisplayInList == true);
            //ObservableCollection<ObjectFieldsViewModel> ObsListtemp = new ObservableCollection<ObjectFieldsViewModel>();
            //foreach (var item in temp)
            //{
            //    ObsListtemp.Add(item);
            //}
            //ObsList = ObsListtemp;
            if (IsNew)
            {
                ButtonsVisibility = Visibility.Visible;
            }
        }

        private string code;
        public string Code
        {
            get
            {
                return code;
            }
            set
            {
                code = value;
                FirePropertyChanged("Code");
            }
        }
        private string textCode;
        public string TextCode
        {
            get
            {
                return textCode;
            }
            set
            {
                textCode = value;
                FirePropertyChanged("TextCode");
            }
        }
        private string localTextCode;
        public string LocalTextCode
        {
            get
            {
                return localTextCode;
            }
            set
            {
                localTextCode = value;
                FirePropertyChanged("LocalTextCode");
            }
        }
        
        private string queryGroupCode;
        public string QueryGroupCode
        {
            get
            {
                return queryGroupCode;
            }
            set
            {
                queryGroupCode = value;
                FirePropertyChanged("QueryGroupCode");
            }
        }
        private int indexOrder;
        public int IndexOrder
        {
            get
            {
                return indexOrder;
            }
            set
            {
                indexOrder = value;
                FirePropertyChanged("IndexOrder");
            }
        }
        private string objectTableName;
        public string ObjectTableName
        {
            get
            {
                return objectTableName;
            }
            set
            {
                objectTableName = value;
                FirePropertyChanged("ObjectTableName");
            }
        }
        private string querySection; // Object Table Name
        public string QuerySection
        {
            get
            {
                return querySection;
            }
            set
            {
                querySection = value;
                FirePropertyChanged("QuerySection");
            }
        }

        private string perspective; // Object Table Name
        public string Perspective
        {
            get
            {
                return perspective;
            }
            set
            {
                perspective = value;
                FirePropertyChanged("Perspective");
            }
        }
        private bool systemLevel;
        public bool SystemLevel
        {
            get
            {
                return true;
            }
            set
            {
                //SystemLevel = value;
                FirePropertyChanged("SystemLevel");
            }
        }
        private bool isAddNewEntity;
        public bool IsAddNewEntity
        {
            get
            {
                return isAddNewEntity;
            }
            set
            {
                isAddNewEntity = value;
                FirePropertyChanged("IsAddNewEntity");
            }
        }
        private bool isPackagable = true;
        public bool IsPackagable
        {
            get
            {
                return isPackagable;
            }
            set
            {
                isPackagable = value;
                FirePropertyChanged("IsPackagable");
            }
        }
        private string defaultSortName;
        public string DefaultSortName
        {
            get
            {
                return defaultSortName;
            }
            set
            {
                defaultSortName = value;
                FirePropertyChanged("DefaultSortName");
            }
        }
        private string defaultSortDirection;
        public string DefaultSortDirection
        {
            get
            {
                return defaultSortDirection;
            }
            set
            {
                defaultSortDirection = value;
                FirePropertyChanged("DefaultSortDirection");
            }
        }

        private string spotlightDataTemplate;
        public string SpotlightDataTemplate
        {
            get
            {
                return spotlightDataTemplate;
            }
            set
            {
                spotlightDataTemplate = value;
                FirePropertyChanged("SpotlightDataTemplate");
            }
        }

        private string editWizardName;
        public string EditWizardName
        {
            get
            {
                return editWizardName;
            }
            set
            {
                editWizardName = value;
                FirePropertyChanged("EditWizardName");
            }
        }


        private string editWizardComponentPath;
        public string EditWizardComponentPath
        {
            get
            {
                return editWizardComponentPath;
            }
            set
            {
                editWizardComponentPath = value;
                FirePropertyChanged("EditWizardComponentPath");
            }
        }

        private ObjectFieldsViewModel objectFieldItem;
        public ObjectFieldsViewModel ObjectFieldItem
        {
            get
            {
                return objectFieldItem;
            }
            set
            {
                objectFieldItem = value;
                DefaultSortName = value.FieldName;
                FirePropertyChanged("ObjectFieldItem");
            }
        }

        private SortDirections sortDirectionItem;
        public SortDirections SortDirectionItem
        {
            get
            {
                return sortDirectionItem;
            }
            set
            {
                sortDirectionItem = value;
                DefaultSortDirection = value.Name;
                FirePropertyChanged("SortDirectionItem");
            }
        }

        Visibility errorsVisibility = Visibility.Collapsed;
        public Visibility ErrorsVisibility
        {
            get { return errorsVisibility; }
            set { errorsVisibility = value; FirePropertyChanged("ErrorsVisibility"); }
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

        public Visibility buttonsVisibility = Visibility.Collapsed;
        public Visibility ButtonsVisibility
        {
            get
            {
                //if (IsNew)
                //{
                //    buttonsVisibility = Visibility.Visible;
                //}
                return buttonsVisibility;
            }
            set
            {
                buttonsVisibility = value;
                FirePropertyChanged("ButtonsVisibility");
            }
        }

        public RelayCommand OkBtnCommand
        {
            get { return new RelayCommand(() => this.OkBtnMethod()); }
        }

        public RelayCommand CancelBtnCommand
        {
            get { return new RelayCommand(() => this.CancelBtnMethod()); }
        }

        private void CancelBtnMethod()
        {
            viewModel.QueryWindow.Close();
        }

        private void OkBtnMethod()
        {
            ErrorMessages = string.Empty;

            this.ValidateEntries();

            if (ErrorMessages == "")
            {
                ButtonsVisibility = Visibility.Collapsed;
                viewModel.UpdateQueriesList(this);
                viewModel.QueryWindow.Close();
            }

        }

        private void ValidateEntries()
        {
            StringBuilder str = new StringBuilder();
            if (string.IsNullOrEmpty(this.Code))
            {
                str.AppendLine("Code is Required");
            }
            if (string.IsNullOrEmpty(TextCode))
            {
                str.AppendLine("Default Text is Required");
            }
            else if (ContainsHebrewCharacters(TextCode))
            {
                str.AppendLine("Text cannot contain Hebrew characters");
            }
            if (string.IsNullOrEmpty(QueryGroupCode))
            {
                str.AppendLine("Query Group Code is Required");
            }
            if (string.IsNullOrEmpty(ObjectTableName))
            {
                str.AppendLine("Object Table Name is Required");
            }
            if (string.IsNullOrEmpty(QuerySection))
            {
                str.AppendLine("Query Section is Required");
            }
            if (string.IsNullOrEmpty(DefaultSortName))
            {
                str.AppendLine("Default Sort Name Id is Required");
            }
            if (string.IsNullOrEmpty(DefaultSortDirection))
            {
                str.AppendLine("Default Sort Direction is Required");
            }


            ErrorMessages = str.ToString();
            if (ErrorMessages != "")
            {
                ErrorsVisibility = Visibility.Visible;
            }
            else
            {
                ErrorsVisibility = Visibility.Collapsed;
            }

            //FirePropertyChanged("ErrorMessages");
        }

        private bool ContainsHebrewCharacters(string text)
        {
            return Regex.IsMatch(text, @"[\u0590-\u05FF]");
        }

        public ObservableCollection<QueryColumnsViewModel> queryColumnObsList;
        public ObservableCollection<QueryColumnsViewModel> QueryColumnObsList
        {
            get
            {
                if (queryColumnObsList == null)
                {
                    queryColumnObsList = new ObservableCollection<QueryColumnsViewModel>(); 
                }
                if (viewModel.GeneratedGrid.Columns.Count < queryColumnObsList.Count)
                {
                    viewModel.GeneratedGrid.Columns.Clear();
                    
                    foreach (var item in queryColumnObsList)
                    {
                        var Temp = new DataGridTextColumn()
                        {
                            Header = item.ObjectFieldName,
                            Width = new DataGridLength(item.ColumnWidth),
                            FontSize = 12,
                        };
                        viewModel.GeneratedGrid.Columns.Add(Temp);
                        viewModel.GeneratedGrid.AddWidthChangeEvent(Temp);

                    }
                }
                var temp = queryColumnObsList.OrderBy(a => a.IndexOrder).ToList();
                //queryColumnObsList.Clear(); 
                var tempo = new ObservableCollection<QueryColumnsViewModel>(); 
                foreach (var item in temp)
                {
                    tempo.Add(item);
                }
                queryColumnObsList = tempo;
                return queryColumnObsList;
            }
            set
            {
                queryColumnObsList = value;
                FirePropertyChanged("QueryColumnObsList");
            }
        }
        public void Refresh()
        {
            FirePropertyChanged("QueryColumnObsList");
        }
        public ObservableCollection<QueryFiltersViewModel> queryFiltersObsList;
        public ObservableCollection<QueryFiltersViewModel> QueryFiltersObsList
        {
            get
            {
                if (queryFiltersObsList == null)
                {
                    queryFiltersObsList = new ObservableCollection<QueryFiltersViewModel>();
                }
                return queryFiltersObsList;
            }
            set
            {
                queryFiltersObsList = value;
                FirePropertyChanged("QueryFiltersObsList");
            }
        }

        public void UpdateQueryColumnsList(QueryColumnsViewModel item)
        {
            if (QueryColumnObsList == null)
            {
                QueryColumnObsList = new ObservableCollection<QueryColumnsViewModel>();
            }
            QueryColumnObsList.Add(item);
            FirePropertyChanged("QueryColumnObsList");
            viewModel.QueryColumnObsList = QueryColumnObsList;
            //this.SetLookUpFieldsList(); 
            viewModel.QueryColumnDetailsEditControlVisibility = (QueryColumnObsList.Count == 0) ? Visibility.Collapsed : Visibility.Visible;
            viewModel.SelectedQueryColumn = item;

        }

        public void UpdateQueryFiltersList(QueryFiltersViewModel item)
        {
            if (QueryFiltersObsList == null)
            {
                QueryFiltersObsList = new ObservableCollection<QueryFiltersViewModel>();
            }
            QueryFiltersObsList.Add(item);

            FirePropertyChanged("QueryFiltersObsList");
            viewModel.QueryFiltersObsList = QueryFiltersObsList;
            //this.SetLookUpFieldsList(); 
            viewModel.QueryFilterDetailsEditControlVisibility = (QueryFiltersObsList.Count == 0) ? Visibility.Collapsed : Visibility.Visible;
            viewModel.SelectedQueryFilter = item;

        }

        public RelayCommand<QueryViewModel> RemoveQueryCommand
        {
            get { return new RelayCommand<QueryViewModel>(m => this.RemoveQueryMethod(m)); }
        }

        public string FeatureDefaultText { get;  set; }
        public string FeatureCode { get;  set; }
        public string FeatureTextCodeCode { get;  set; }
        public string TextCodeCode { get;  set; }
        public string QueryGroupName { get;  set; }
        public bool IsSpellChecked { get;  set; }

        private void RemoveQueryMethod(QueryViewModel DelQ)
        {
            viewModel.QueriesObsList.Remove(this);
            FirePropertyChanged("QueriesObsList");
        }



    }
}
