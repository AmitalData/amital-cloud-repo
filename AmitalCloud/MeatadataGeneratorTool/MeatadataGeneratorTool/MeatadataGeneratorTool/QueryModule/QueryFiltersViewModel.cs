using GalaSoft.MvvmLight.Command;
//using Simplog.Data.InfrastructureModel.EntityPOCOs; using Simplog.Global.Data.GlobalModel.EntityPOCOs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MeatadataGeneratorTool.QueryModule
{
    public class QueryFiltersViewModel : PropertyChangedImplementation
    {






        ObjectTableViewModel viewModel;
        QueryViewModel queryviewModel;
        bool IsNew;
        public ObservableCollection<ObjectFieldsViewModel> ObsList { get; set; }
        public List<string> OperatorsList { get { return new List<string>() { "Equal", "NotEqual", "StartsWith", "Contains", "LargerThan", "LessThan", "GreaterThanOrEqual", "Between", "Custom", }; } }
        public QueryFiltersViewModel(ObjectTableViewModel OTViewModel, QueryViewModel QViewModel, bool IsNew)
        {
            this.IsNew = IsNew;
            viewModel = OTViewModel;
            queryviewModel = QViewModel;
            ObsList = OTViewModel.ObsList;
            if (IsNew)
            {
                ButtonsVisibility = Visibility.Visible;
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

        private string predefinedValue;
        public string PredefinedValue
        {
            get
            {
                return predefinedValue;
            }
            set
            {
                predefinedValue = value;
                FirePropertyChanged("PredefinedValue");
            }
        }

        private string predefinedValue2;
        public string PredefinedValue2
        {
            get
            {
                return predefinedValue2;
            }
            set
            {
                predefinedValue2 = value;
                FirePropertyChanged("PredefinedValue2");
            }
        }

        private bool customPredefined = false;
        public bool CustomPredefined
        {
            get
            {
                return customPredefined;
            }
            set
            {
                customPredefined = value;
                FirePropertyChanged("CustomPredefined");
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

        private bool isPredefined;
        public bool IsPredefined
        {
            get
            {
                return true;
            }
            set
            {
                isPredefined = value;
                FirePropertyChanged("IsPredefined");
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
            viewModel.FilterWindow.Close();
        }

        private void OkBtnMethod()
        {
            ErrorMessages = string.Empty;

            this.ValidateEntries();

            if (ErrorMessages == "")
            {
                ButtonsVisibility = Visibility.Collapsed;
                queryviewModel.UpdateQueryFiltersList(this);
                viewModel.FilterWindow.Close();
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
            if (string.IsNullOrEmpty(PredefinedValue))
            {
                str.AppendLine("Predefined Value is Required");
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

        public RelayCommand<QueryViewModel> RemoveFilterCommand
        {
            get { return new RelayCommand<QueryViewModel>(m => this.RemoveFilterMethod(m)); }
        }

        public int IndexOrder { get; set; }
        public string Operator { get; set; }

        private void RemoveFilterMethod(QueryViewModel QModel)
        {
            QModel.QueryFiltersObsList.Remove(this);
            FirePropertyChanged("QueryFiltersObsList");
        }
    }
}
