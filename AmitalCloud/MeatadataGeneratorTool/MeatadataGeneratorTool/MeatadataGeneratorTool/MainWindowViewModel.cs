using GalaSoft.MvvmLight.Command;
using MeatadataGeneratorTool.EventTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MeatadataGeneratorTool
{
    public class MainWindowViewModel : PropertyChangedImplementation
    {
        public List<TableType> TableTypesList { get { return new List<TableType>() { new TableType() { TableTypeName = "Main Table" }, new TableType() { TableTypeName = "CLose Table" }, new TableType() { TableTypeName = "Composition Table" } }; } }
        public List<ObjectFieldsViewModel> FieldsList { get; set; }
        public ObjectTableViewModel NewObjectTable = new ObjectTableViewModel();
        public static ObjectTableControl CurrentControl { get; set; }
        public MainWindowViewModel()
        {

        }

        #region Fields Properties

        public bool isIdChecked;
        public bool IsIdChecked
        {
            get
            {
                return isIdChecked;
            }
            set
            {
                isIdChecked = value;
                FirePropertyChanged("IsIdChecked");
            }
        }
        public bool isTenantChecked;
        public bool IsTenantChecked
        {
            get
            {
                return isTenantChecked;
            }
            set
            {
                isTenantChecked = value;
                FirePropertyChanged("IsTenantChecked");
            }
        }
        public bool isCreateDateChecked;
        public bool IsCreateDateChecked
        {
            get
            {
                return isCreateDateChecked;
            }
            set
            {
                isCreateDateChecked = value;
                FirePropertyChanged("IsCreateDateChecked");
            }
        }
        public bool isCreatedByUserIdChecked;
        public bool IsCreatedByUserIdChecked
        {
            get
            {
                return isCreatedByUserIdChecked;
            }
            set
            {
                isCreatedByUserIdChecked = value;
                FirePropertyChanged("IsCreatedByUserIdChecked");
            }
        }
        public bool isUpdateDateChecked;
        public bool IsUpdateDateChecked
        {
            get
            {
                return isUpdateDateChecked;
            }
            set
            {
                isUpdateDateChecked = value;
                FirePropertyChanged("IsUpdateDateChecked");
            }
        }
        public bool isUpdatedByUserIdChecked;
        public bool IsUpdatedByUserIdChecked
        {
            get
            {
                return isUpdatedByUserIdChecked;
            }
            set
            {
                isUpdatedByUserIdChecked = value;
                FirePropertyChanged("IsUpdatedByUserIdChecked");
            }
        }
        public bool isSearchFieldsChecked;
        public bool IsSearchFieldsChecked
        {
            get
            {
                return isSearchFieldsChecked;
            }
            set
            {
                isSearchFieldsChecked = value;
                FirePropertyChanged("IsSearchFieldsChecked");
            }
        }
        public bool isCodeChecked;
        public bool IsCodeChecked
        {
            get
            {
                return isCodeChecked;
            }
            set
            {
                isCodeChecked = value;
                FirePropertyChanged("IsCodeChecked");
            }
        }
        public bool isNameChecked;
        public bool IsNameChecked
        {
            get
            {
                return isNameChecked;
            }
            set
            {
                isNameChecked = value;
                FirePropertyChanged("IsNameChecked");
            }
        }
        public string objectTableName;
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
        #endregion

        #region Visibility Properties

        public Visibility idVisibility = Visibility.Collapsed;
        public Visibility IdVisibility
        {
            get
            {
                return idVisibility;
            }
            set
            {
                idVisibility = value;
                FirePropertyChanged("IdVisibility");
            }
        }
        public Visibility tenantVisibility = Visibility.Collapsed;
        public Visibility TenantVisibility
        {
            get
            {
                return tenantVisibility;
            }
            set
            {
                tenantVisibility = value;
                FirePropertyChanged("TenantVisibility");
            }
        }
        public Visibility createDateVisibility = Visibility.Collapsed;
        public Visibility CreateDateVisibility
        {
            get
            {
                return createDateVisibility;
            }
            set
            {
                createDateVisibility = value;
                FirePropertyChanged("CreateDateVisibility");
            }
        }
        public Visibility createdByUserIdVisibility = Visibility.Collapsed;
        public Visibility CreatedByUserIdVisibility
        {
            get
            {
                return createdByUserIdVisibility;
            }
            set
            {
                createdByUserIdVisibility = value;
                FirePropertyChanged("CreatedByUserIdVisibility");
            }
        }
        public Visibility updateDateVisibility = Visibility.Collapsed;
        public Visibility UpdateDateVisibility
        {
            get
            {
                return updateDateVisibility;
            }
            set
            {
                updateDateVisibility = value;
                FirePropertyChanged("UpdateDateVisibility");
            }
        }
        public Visibility updateByUserIdVisibility = Visibility.Collapsed;
        public Visibility UpdateByUserIdVisibility
        {
            get
            {
                return updateByUserIdVisibility;
            }
            set
            {
                updateByUserIdVisibility = value;
                FirePropertyChanged("UpdateByUserIdVisibility");
            }
        }
        public Visibility searchFieldsStringVisibility = Visibility.Collapsed;
        public Visibility SearchFieldsStringVisibility
        {
            get
            {
                return searchFieldsStringVisibility;
            }
            set
            {
                searchFieldsStringVisibility = value;
                FirePropertyChanged("SearchFieldsStringVisibility");
            }
        }
        public Visibility codeVisibility = Visibility.Collapsed;
        public Visibility CodeVisibility
        {
            get
            {
                return codeVisibility;
            }
            set
            {
                codeVisibility = value;
                FirePropertyChanged("CodeVisibility");
            }
        }
        public Visibility nameVisibility = Visibility.Collapsed;
        public Visibility NameVisibility
        {
            get
            {
                return nameVisibility;
            }
            set
            {
                nameVisibility = value;
                FirePropertyChanged("NameVisibility");
            }
        }

        public bool isSearchFieldsEnabled = true;
        public bool IsSearchFieldsEnabled
        {
            get
            {
                return isSearchFieldsEnabled;
            }
            set
            {
                isSearchFieldsEnabled = value;
                FirePropertyChanged("IsSearchFieldsEnabled");
            }
        }
        #endregion

        public TableType SelectedType { get; set; }

        #region Commands

        public RelayCommand<TableType> TableTypesSelectionChanged
        {
            get { return new RelayCommand<TableType>(i => this.TableTypesSelectionChangedSelectionChangedMethod(i)); }
            set { }
        }
        private void TableTypesSelectionChangedSelectionChangedMethod(TableType item)
        {
            SelectedType = item;
            switch (item.TableTypeName)
            {
                case "Main Table":
                    IsIdChecked = true;
                    IsTenantChecked = true;
                    IsCreateDateChecked = true;
                    IsCreatedByUserIdChecked = true;
                    IsUpdateDateChecked = true;
                    IsUpdatedByUserIdChecked = true;
                    IsSearchFieldsChecked = true;
                    IsSearchFieldsEnabled = true;
                    IsCodeChecked = false;
                    IsNameChecked = false;
                    IdVisibility = Visibility.Visible;
                    TenantVisibility = Visibility.Visible;
                    CreateDateVisibility = Visibility.Visible;
                    CreatedByUserIdVisibility = Visibility.Visible;
                    UpdateByUserIdVisibility = Visibility.Visible;
                    UpdateDateVisibility = Visibility.Visible;
                    SearchFieldsStringVisibility = Visibility.Visible;
                    CodeVisibility = Visibility.Collapsed;
                    NameVisibility = Visibility.Collapsed;
                    
                    break;
                case "CLose Table":
                    IsIdChecked = false;
                    IsTenantChecked = false;
                    IsCreateDateChecked = false;
                    IsCreatedByUserIdChecked = false;
                    IsUpdateDateChecked = false;
                    IsUpdatedByUserIdChecked = false;
                    IsSearchFieldsChecked = true;
                    IsSearchFieldsEnabled = false;
                    IsCodeChecked = true;
                    IsNameChecked = true;
                    IdVisibility = Visibility.Collapsed;
                    TenantVisibility = Visibility.Collapsed;
                    CreateDateVisibility = Visibility.Collapsed;
                    CreatedByUserIdVisibility = Visibility.Collapsed;
                    UpdateByUserIdVisibility = Visibility.Collapsed;
                    UpdateDateVisibility = Visibility.Collapsed;
                    SearchFieldsStringVisibility = Visibility.Visible;
                    CodeVisibility = Visibility.Visible;
                    NameVisibility = Visibility.Visible;
                    break;
                case "Composition Table":
                    IsIdChecked = false;
                    IsTenantChecked = true;
                    IsCreateDateChecked = false;
                    IsCreatedByUserIdChecked = false;
                    IsUpdateDateChecked = false;
                    IsUpdatedByUserIdChecked = false;
                    IsSearchFieldsChecked = false;
                    IsSearchFieldsEnabled = true;
                    IsCodeChecked = false;
                    IsNameChecked = false;
                    IdVisibility = Visibility.Visible;
                    TenantVisibility = Visibility.Visible;
                    CreateDateVisibility = Visibility.Collapsed;
                    CreatedByUserIdVisibility = Visibility.Collapsed;
                    UpdateByUserIdVisibility = Visibility.Collapsed;
                    UpdateDateVisibility = Visibility.Collapsed;
                    SearchFieldsStringVisibility = Visibility.Collapsed;
                    CodeVisibility = Visibility.Collapsed;
                    NameVisibility = Visibility.Collapsed;
                    break;
                default:
                    break;

            }
        }

        public RelayCommand OkBtnCommand
        {
            get { return new RelayCommand(() => this.OkBtnMethod()); }
        }

        private void OkBtnMethod()
        {
            ErrorMessages = "";
            if (!string.IsNullOrEmpty(ObjectTableName) && SelectedType != null && ObjectTableName.Length <= 30)
            {
                NewObjectTable.ObjectTableName = ObjectTableName;
                NewObjectTable.IsNew = true;
                if (SelectedType.TableTypeName == "Main Table")
                {
                    if (IsIdChecked)
                    {
                        NewObjectTable.IsMain = true;
                        var propObjectField = new ObjectFieldsViewModel(NewObjectTable, true);
                        propObjectField.FieldName = "Id";
                        propObjectField.PMPropertyPath = "Id";
                        propObjectField.ListPropertyPath = "Id";
                        propObjectField.FieldDataType = "Text";
                        propObjectField.MaxLength = 15;
                        propObjectField.IsMaxLength = false;
                        propObjectField.SystemMaxLength = 15;
                        propObjectField.IsPrimaryKey = true;
                        propObjectField.IsRequired = true;
                        propObjectField.IsDBField = true;
                        propObjectField.IsPMField = true;
                        propObjectField.GenerateInList = true;
                        propObjectField.EnableAutoFill = true;
                        propObjectField.ListLableDefaultText = "Id";
                        propObjectField.ValidForQuerySection1 = NewObjectTable.ObjectTableName;
                        propObjectField.DefaultText = "Id";
                        propObjectField.NoObjectField = true;
                       
                        NewObjectTable.UpdateObsList(propObjectField);
                    }
                    if (IsTenantChecked)
                    {
                        var propObjectField = new ObjectFieldsViewModel(NewObjectTable, true);
                        propObjectField.FieldName = "Tenant";
                        propObjectField.PMPropertyPath = "Tenant";
                        propObjectField.ListPropertyPath = "Tenant";
                        propObjectField.FieldDataType = "Integer";
                        propObjectField.IsRequired = true;
                        propObjectField.IsMaxLength = false;
                        propObjectField.IsDBField = true;
                        propObjectField.IsPMField = true;
                        propObjectField.GenerateInList = true;
                        propObjectField.ListLableDefaultText = "Tenant";
                        propObjectField.ValidForQuerySection1 = NewObjectTable.ObjectTableName;
                        propObjectField.DefaultText = "Tenant";
                        propObjectField.EnableAutoFill = true;
                        propObjectField.NoObjectField = true;
                        NewObjectTable.UpdateObsList(propObjectField);
                    }
                    if (IsCreateDateChecked)
                    {
                        var propObjectField = new ObjectFieldsViewModel(NewObjectTable, true);
                        propObjectField.FieldName = "CreateDate";
                        propObjectField.PMPropertyPath = "CreateDate";
                        propObjectField.ListPropertyPath = "CreateDate";
                        propObjectField.FieldDataType = "DateTime";
                        propObjectField.IsRequired = true;
                        propObjectField.IsMaxLength = false;
                        propObjectField.IsDBField = true;
                        propObjectField.IsPMField = true;
                        propObjectField.GenerateInList = true;
                        propObjectField.DisplayInList = true;
                        propObjectField.ListLableDefaultText = "Create Date";
                        propObjectField.ValidForQuerySection1 = NewObjectTable.ObjectTableName;
                        propObjectField.DefaultText = "Create Date";
                        propObjectField.EnableAutoFill = true;
                        NewObjectTable.UpdateObsList(propObjectField);
                    }
                    if (IsCreatedByUserIdChecked)
                    {
                        var propObjectField = new ObjectFieldsViewModel(NewObjectTable, true);
                        propObjectField.FieldName = "CreatedByUserId";
                        propObjectField.PMPropertyPath = "CreatedByUserId";
                        propObjectField.ListPropertyPath = "CreatedByUserId";
                        propObjectField.FieldDataType = "LookUp";
                        propObjectField.LookUpTableName = "User";
                        propObjectField.IsRequired = true;
                        propObjectField.IsDBField = true;
                        propObjectField.IsMaxLength = false;
                        propObjectField.IsPMField = true;
                        propObjectField.GenerateInList = true;
                        propObjectField.MaxLength = 15;
                        propObjectField.SystemMaxLength = 15;
                        propObjectField.IsForeignKey = true;
                        propObjectField.ForeignEntity = "User";
                        propObjectField.NavigationPropertyName = "CreatedByUser";
                        propObjectField.DefaultText = "Created By";
                        propObjectField.EnableAutoFill = true;
                        NewObjectTable.UpdateObsList(propObjectField);
                    }
                    if (IsUpdateDateChecked)
                    {
                        var propObjectField = new ObjectFieldsViewModel(NewObjectTable, true);
                        propObjectField.FieldName = "UpdateDate";
                        propObjectField.PMPropertyPath = "UpdateDate";
                        propObjectField.ListPropertyPath = "UpdateDate";
                        propObjectField.FieldDataType = "DateTime";
                        propObjectField.IsRequired = true;
                        propObjectField.IsMaxLength = false;
                        propObjectField.IsDBField = true;
                        propObjectField.IsPMField = true;
                        propObjectField.GenerateInList = true;
                        propObjectField.DisplayInList = true;
                        propObjectField.ListLableDefaultText = "Update Date";
                        propObjectField.ValidForQuerySection1 = NewObjectTable.ObjectTableName;
                        propObjectField.DefaultText = "Update Date";
                        propObjectField.EnableAutoFill = true;
                        NewObjectTable.UpdateObsList(propObjectField);
                    }
                    if (IsUpdatedByUserIdChecked)
                    {
                        var propObjectField = new ObjectFieldsViewModel(NewObjectTable, true);
                        propObjectField.FieldName = "UpdatedByUserId";
                        propObjectField.PMPropertyPath = "UpdatedByUserId";
                        propObjectField.ListPropertyPath = "UpdatedByUserId";
                        propObjectField.FieldDataType = "LookUp";
                        propObjectField.LookUpTableName = "User";
                        propObjectField.IsRequired = true;
                        propObjectField.IsMaxLength = false;
                        propObjectField.IsDBField = true;
                        propObjectField.IsPMField = true;
                        propObjectField.GenerateInList = true;
                        propObjectField.MaxLength = 15;
                        propObjectField.SystemMaxLength = 15;
                        propObjectField.IsForeignKey = true;
                        propObjectField.ForeignEntity = "User";
                        propObjectField.NavigationPropertyName = "UpdatedByUser";
                        propObjectField.DefaultText = "Updated By";
                        propObjectField.EnableAutoFill = true;
                        NewObjectTable.UpdateObsList(propObjectField);
                    }
                    if (IsSearchFieldsChecked)
                    {
                        var propObjectField = new ObjectFieldsViewModel(NewObjectTable, true);
                        propObjectField.FieldName = "SearchFields";
                        propObjectField.PMPropertyPath = "SearchFields";
                        propObjectField.ListPropertyPath = "SearchFields";
                        propObjectField.FieldDataType = "nText";
                        propObjectField.IsMaxLength = true;
                        propObjectField.MaxLength = 1000;
                        propObjectField.SystemMaxLength = 1000;
                        propObjectField.IsNullable = true;
                        propObjectField.IsDBField = true;
                        propObjectField.IsPMField = true;
                        propObjectField.GenerateInList = true;
                        propObjectField.ListLableDefaultText = "Search ...";
                        propObjectField.ValidForQuerySection1 = NewObjectTable.ObjectTableName;
                        propObjectField.DefaultText = "Search ...";
                        NewObjectTable.UpdateObsList(propObjectField);
                    }

                }
                else if (SelectedType.TableTypeName == "CLose Table")
                {
                    NewObjectTable.IsClosed = true;


                    var propObjectField = new ObjectFieldsViewModel(NewObjectTable, true);
                    propObjectField.FieldName = "Code";
                    propObjectField.PMPropertyPath = "Code";
                    propObjectField.ListPropertyPath = "Code";
                    propObjectField.FieldDataType = "Text";
                    propObjectField.IsMaxLength = false;
                    propObjectField.MaxLength = 3;
                    propObjectField.IsPrimaryKey = true;
                    propObjectField.SystemMaxLength = 3;
                    propObjectField.IsNullable = false;
                    propObjectField.IsDBField = true;
                    propObjectField.IsPMField = true;
                    propObjectField.GenerateInList = true;
                    propObjectField.ListLableDefaultText = "Code";
                    propObjectField.ValidForQuerySection1 = NewObjectTable.ObjectTableName;
                    propObjectField.DefaultText = "Code";
                    NewObjectTable.UpdateObsList(propObjectField);


                    propObjectField = new ObjectFieldsViewModel(NewObjectTable, true);
                    propObjectField.FieldName = "Name";
                    propObjectField.PMPropertyPath = "Name";
                    propObjectField.ListPropertyPath = "Name";
                    propObjectField.FieldDataType = "Text";
                    propObjectField.IsMaxLength = false;
                    propObjectField.MaxLength = 3;
                    propObjectField.SystemMaxLength = 3;
                    propObjectField.IsNullable = false;
                    propObjectField.IsDBField = true;
                    propObjectField.IsPMField = true;
                    propObjectField.GenerateInList = true;
                    propObjectField.ListLableDefaultText = "Name";
                    propObjectField.ValidForQuerySection1 = NewObjectTable.ObjectTableName;
                    propObjectField.DefaultText = "Name";
                    NewObjectTable.UpdateObsList(propObjectField);

                    propObjectField = new ObjectFieldsViewModel(NewObjectTable, true);
                    propObjectField.FieldName = "SearchFields";
                    propObjectField.PMPropertyPath = "SearchFields";
                    propObjectField.ListPropertyPath = "SearchFields";
                    propObjectField.FieldDataType = "nText";
                    propObjectField.IsMaxLength = true;
                    propObjectField.MaxLength = 1000;
                    propObjectField.SystemMaxLength = 1000;
                    propObjectField.IsNullable = true;
                    propObjectField.IsDBField = true;
                    propObjectField.IsPMField = true;
                    propObjectField.GenerateInList = true;
                    propObjectField.ListLableDefaultText = "Search ...";
                    propObjectField.ValidForQuerySection1 = NewObjectTable.ObjectTableName;
                    propObjectField.DefaultText = "Search ...";
                    NewObjectTable.UpdateObsList(propObjectField);



                }
                else if (SelectedType.TableTypeName == "Composition Table")
                {
                    NewObjectTable.IsComposition = true;
                    if (IsIdChecked)
                    {
                        var propObjectField = new ObjectFieldsViewModel(NewObjectTable, true);
                        propObjectField.FieldName = "Id";
                        propObjectField.PMPropertyPath = "Id";
                        propObjectField.ListPropertyPath = "Id";
                        propObjectField.FieldDataType = "Text";
                        propObjectField.MaxLength = 15;
                        propObjectField.IsMaxLength = false;
                        propObjectField.SystemMaxLength = 15;
                        propObjectField.IsPrimaryKey = true;
                        propObjectField.IsRequired = true;
                        propObjectField.IsDBField = true;
                        propObjectField.IsPMField = true;
                        propObjectField.GenerateInList = true;
                        propObjectField.ListLableDefaultText = "Id";
                        propObjectField.ValidForQuerySection1 = NewObjectTable.ObjectTableName;
                        propObjectField.DefaultText = "Id";
                        propObjectField.EnableAutoFill = true;
                        propObjectField.NoObjectField = true;
                        NewObjectTable.UpdateObsList(propObjectField);
                    }
                    if (IsTenantChecked)
                    {
                        var propObjectField = new ObjectFieldsViewModel(NewObjectTable, true);
                        propObjectField.FieldName = "Tenant";
                        propObjectField.PMPropertyPath = "Tenant";
                        propObjectField.ListPropertyPath = "Tenant";
                        propObjectField.FieldDataType = "Integer";
                        propObjectField.IsRequired = true;
                        propObjectField.IsMaxLength = false;
                        propObjectField.IsDBField = true;
                        propObjectField.IsPMField = true;
                        propObjectField.GenerateInList = true;
                        propObjectField.ListLableDefaultText = "Id";
                        propObjectField.ValidForQuerySection1 = NewObjectTable.ObjectTableName;
                        propObjectField.DefaultText = "Tenant";
                        propObjectField.EnableAutoFill = true;
                        propObjectField.NoObjectField = true;
                        NewObjectTable.UpdateObsList(propObjectField);
                    }
                }

                var tempEventType = new EventTypesViewModel(NewObjectTable, true)
                {
                    Code = "CREV",
                    EnglishName = "Created",
                    LocalName = "Created",
                    ShortView = true,
                    IsManualEntry = false
                };
                NewObjectTable.UpdateEventTypesObsList(tempEventType);
                tempEventType = new EventTypesViewModel(NewObjectTable, true)
                {
                    Code = "UPEV",
                    EnglishName = "Updated",
                    LocalName = "Updated",
                    ShortView = false,
                    IsManualEntry = false
                };
                NewObjectTable.UpdateEventTypesObsList(tempEventType);

                CurrentControl = new ObjectTableControl();
                CurrentControl.DataContext = NewObjectTable;

                //CurrentControl.WindowStyle = WindowStyle.None;
                CurrentControl.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                CurrentControl.WindowState = WindowState.Maximized;
                CurrentControl.Show();
                CurrentControl.Closed += CurrentControl_Closed;
                App.MainControl.Hide();

            }
            else
            {
                if (ObjectTableName.Contains("Customs") ? ObjectTableName.Substring(9).Length > 30 : ObjectTableName.Length > 30)
                {
                    ErrorMessages = "Object Table Name Should Be Less Than Or Equal 30 !!";
                    ErrorsVisibility = Visibility.Visible;
                }
                else
                {
                    ErrorMessages = "Object Table Name And Type Are Required !!";
                    ErrorsVisibility = Visibility.Visible;
                }

            }

        }

        void CurrentControl_Closed(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }

        #endregion

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
    }

    public class TableType
    {
        public string TableTypeName { get; set; }
    }
}
