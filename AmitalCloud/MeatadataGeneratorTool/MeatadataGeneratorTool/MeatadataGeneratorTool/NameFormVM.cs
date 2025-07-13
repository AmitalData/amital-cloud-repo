using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MeatadataGeneratorTool
{
    public class NameFormVM : PropertyChangedImplementation
    {
        ObjectTableViewModel ParentViewModel;
        public ObservableCollection<ObjectFieldsViewModel> ObsList { get; set; }
        public List<string> DataTypesList { get { return new List<string>() { "Boolean", "Constant", "Date", "DateTime", "DateTime2", "Decimal", "Double", "Integer", "LookUp", "nText", "PickList", "SigDouble", "Text", "UnsDecimal", "UnsInteger", "List", "Emails", "Byte[]" }; } }
        string ActionType;
        public NameFormVM(ObjectTableViewModel ParentVM,string Action = "ReNameTable")
        {
            ErrorsVisibility = Visibility.Hidden;
            ActionType = Action;
            ParentViewModel = ParentVM;
            ObsList = new ObservableCollection<ObjectFieldsViewModel>();
            if (ActionType == "AlterField")
            {
                FieldsVisibility = Visibility.Visible;
                renameFieldsVisibility = Visibility.Visible;
            }
            if (ActionType == "ReNameField")
            {
                renameFieldsVisibility = Visibility.Visible;
            }
           
            foreach (var item in ParentViewModel.ObsList)
            {
                if (item.IsDBField)
                {
                    ObsList.Add(item);
                }
            }
        }

        ObjectFieldsViewModel selectedField;
        public ObjectFieldsViewModel SelectedField
        {
            get
            {
                return selectedField;
            }
            set
            {
                selectedField = value;
                this.IsNullable = value.IsNullable;
                FirePropertyChanged("SelectedField");
            }
        }


        string fieldDataType;
        public string FieldDataType
        {
            get
            {
                return fieldDataType;
            }
            set
            {
                fieldDataType = value;
                FirePropertyChanged("FieldDataType");
            }
        }

        string oldName;
        public string OldName
        {
            get
            {
                return oldName;
            }
            set
            {
                oldName = value;
                FirePropertyChanged("OldName");
            }
        }

        string defaultValue;
        public string DefaultValue
        {
            get
            {
                return defaultValue;
            }
            set
            {
                defaultValue = value;
                FirePropertyChanged("DefaultValue");
            }
        }

        bool isNullable;
        public bool IsNullable
        {
            get
            {
                return isNullable;
            }
            set
            {
                isNullable = value;
                if (value == true)
                {
                    DefaultValueEnabled = false;
                }
                else
                {
                    DefaultValueEnabled = true;
                }
                FirePropertyChanged("IsNullable");
                //FirePropertyChanged("DefaultValueEnabled");
            }
        }

        bool defaultValueEnabled;
        public bool DefaultValueEnabled
        {
            get
            {
                return defaultValueEnabled;
            }
            set
            {
                defaultValueEnabled = value; 
                FirePropertyChanged("DefaultValueEnabled");
            }
        }

        public string ErrorMessages { get; set; }

        Visibility fieldsVisibility = Visibility.Collapsed;
        public Visibility FieldsVisibility
        {
            get { return fieldsVisibility; }
            set { fieldsVisibility = value; FirePropertyChanged("FieldsVisibility"); }
        }

        Visibility renamefieldsVisibility = Visibility.Collapsed;
        public Visibility renameFieldsVisibility
        {
            get { return renamefieldsVisibility; }
            set { renamefieldsVisibility = value; FirePropertyChanged("renameFieldsVisibility"); }
        }

        Visibility errorsVisibility = Visibility.Collapsed;
        public Visibility ErrorsVisibility
        {
            get { return errorsVisibility; }
            set { errorsVisibility = value; FirePropertyChanged("ErrorsVisibility"); }
        }
        public RelayCommand OkBtnCommand
        {
            get { return new RelayCommand(() => this.OkBtnMethod()); }
        }
        private void OkBtnMethod()
        {
            ErrorMessages = string.Empty;
            if (ActionType == "AlterField" && SelectedField == null)
            {
                ErrorMessages = "You need to select a field for update .";
                FirePropertyChanged("ErrorMessages");
                ErrorsVisibility = Visibility.Visible;
                return;
            }
            if (string.IsNullOrEmpty(OldName))
            {
                ErrorMessages = "Old Name is required.";
                FirePropertyChanged("ErrorMessages");
                ErrorsVisibility = Visibility.Visible;
                return;
            }
            if (ActionType == "AlterField")
            {
                if (IsNullable == false && string.IsNullOrEmpty(DefaultValue))
                {
                    ErrorMessages = "Default Value is required.";
                    FirePropertyChanged("ErrorMessages");
                    ErrorsVisibility = Visibility.Visible;
                    return;
                }
                ParentViewModel.GenerateAlterField(this);
            }
            else if (ActionType == "ReNameField")
            {
                ParentViewModel.GenerateRenameField(this);
            }
            else
            {
                ParentViewModel.GenerateTableRename(OldName);
            }
            
        }

        public RelayCommand CancelBtnCommand
        {
            get { return new RelayCommand(() => this.CancelBtnMethod()); }
        }
        private void CancelBtnMethod()
        {
            ParentViewModel.MyWindow.Close();
        }
    }
}
