using GalaSoft.MvvmLight.Command;
//using Simplog.Data.InfrastructureModel.EntityPOCOs; using Simplog.Global.Data.GlobalModel.EntityPOCOs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace MeatadataGeneratorTool.QueryModule
{
    public class QueryColumnsViewModel : PropertyChangedImplementation
    {
        ObjectTableViewModel viewModel;
        QueryViewModel queryviewModel;
        bool IsNew;
        private ObservableCollection<ObjectFieldsViewModel> obsList;
        public ObservableCollection<ObjectFieldsViewModel> ObsList
        {
            get
            {
                var temp = viewModel.ObsList.Where(a => a.DisplayInList == true && a.FieldName != "Id" && a.FieldName != "Tenant");//&& !viewModel.SelectedQuery.QueryColumnObsList.Contains(viewModel.SelectedQuery.QueryColumnObsList.Where(aa => aa.ObjectFieldName != a.FieldName).FirstOrDefault()));
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
        public QueryColumnsViewModel(ObjectTableViewModel OTViewModel, QueryViewModel QViewModel, bool IsNew)
        {
            this.IsNew = IsNew;
            viewModel = OTViewModel;
            queryviewModel = QViewModel;
            //var temp = OTViewModel.ObsList.Where(a=>a.DisplayInList == true);
            //ObservableCollection<ObjectFieldsViewModel> ObsListtemp = new ObservableCollection<ObjectFieldsViewModel>();
            //foreach (var item in temp)
            //{
            //    ObsListtemp.Add(item);
            //}
            //ObsList = ObsListtemp;
            if (IsNew)
            {
                ButtonsVisibility = Visibility.Visible;
                ColumnsFieldsEnabled = true;
            }
        }
         
        private string queryCode;
        public string QueryCode
        {
            get
            {
                return queryviewModel.Code;
            }
            set
            {
                queryviewModel.Code = value;
                FirePropertyChanged("QueryCode");
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
        private string objectFieldName;
        public string ObjectFieldName
        {
            get
            {
                return objectFieldName;
            }
            set
            {
                objectFieldName = value;
                FirePropertyChanged("ObjectFieldName");
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
                ObjectFieldName = value.FieldName;
                FirePropertyChanged("ObjectFieldItem");
            }
        }


        private int columnWidth;
        public int ColumnWidth
        {
            get
            {
                return columnWidth;
            }
            set
            {
                columnWidth = value;
                FirePropertyChanged("ColumnWidth");
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

        public bool columnsFieldsEnabled = false;
        public bool ColumnsFieldsEnabled
        {
            get
            { 
                return columnsFieldsEnabled;
            }
            set
            {
                columnsFieldsEnabled = value;
                FirePropertyChanged("ColumnsFieldsEnabled");
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
            viewModel.QueryColumnWindow.Close();
        }

        private void OkBtnMethod()
        {
            ErrorMessages = string.Empty;

            this.ValidateEntries();
            if (viewModel.SelectedQuery.QueryColumnObsList.Contains(viewModel.SelectedQuery.QueryColumnObsList.Where(aa => aa.ObjectFieldName == ObjectFieldName).FirstOrDefault()))
            {
                ErrorMessages += ObjectFieldName + " is already exists ...";
            }
            if (ErrorMessages == "")
            {
                ButtonsVisibility = Visibility.Collapsed;
                ColumnsFieldsEnabled = false;
                queryviewModel.UpdateQueryColumnsList(this);
                //var TempColumn = new DataGridTextColumn()
                //{
                //    Header = this.ObjectFieldName,
                //    Width = new DataGridLength(ColumnWidth),
                //    FontSize = 12,

                //}; 
                //viewModel.GeneratedGrid.Columns.Add(TempColumn);
                //viewModel.GeneratedGrid.Width = viewModel.GeneratedGrid.Columns.Sum(a=>a.Width.Value);
                viewModel.QueryColumnWindow.Close();
            }
            else
            {
                ErrorsVisibility = Visibility.Visible;
            }

        }

        private void ValidateEntries()
        {
            StringBuilder str = new StringBuilder();
            if (string.IsNullOrEmpty(this.QueryCode))
            {
                str.AppendLine("Query Code is Required");
            }
            if (string.IsNullOrEmpty(ObjectFieldName))
            {
                str.AppendLine("Object Field Name is Required");
            }
            if (ColumnWidth == 0)
            {
                ColumnWidth = 100;
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

        public RelayCommand<QueryViewModel> RemoveQueryColumnCommand
        {
            get { return new RelayCommand<QueryViewModel>(Qmodel => this.RemoveQueryColumnMethod(Qmodel)); }
        }
        private void RemoveQueryColumnMethod(QueryViewModel QModel)
        {
            QModel.QueryColumnObsList.Remove(this);
            viewModel.GeneratedGrid.Columns.Remove(viewModel.GeneratedGrid.Columns.Where(a => a.Header == this.ObjectFieldName).FirstOrDefault());
            QModel.Refresh();
        }
    }
}
