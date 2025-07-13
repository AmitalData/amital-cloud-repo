using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MeatadataGeneratorTool.DataContractsModule
{
    public class DataContractViewModel : PropertyChangedImplementation
    {
        ObjectTableViewModel viewModel;
        //ObservableCollection<DataContractFieldViewModel> DCFieldsObsList;//= new ObservableCollection<DataContractFieldViewModel>();
        //public List<Properties> Columns 
        //{
        //    get
        //    {
        //        if (ScreenViewModel.Column2Visibility == Visibility.Collapsed)
        //        {
        //            return new List<Properties>() { 
        //                new Properties() { Id = 1, Name = "Column 1" }
        //            };
        //        }
        //        else if (ScreenViewModel.Column3Visibility == Visibility.Collapsed)
        //        {
        //            return new List<Properties>() { 
        //                new Properties() { Id = 1, Name = "Column 1" },
        //                new Properties() { Id = 2, Name = "Column 2" }
        //            };
        //        }
        //        else if (ScreenViewModel.Column4Visibility == Visibility.Collapsed)
        //        {
        //            return new List<Properties>() { 
        //                new Properties() { Id = 1, Name = "Column 1" },
        //                new Properties() { Id = 2, Name = "Column 2" },
        //                new Properties() { Id = 3, Name = "Column 3" }
        //            };
        //        }
        //        else if (ScreenViewModel.Column5Visibility == Visibility.Collapsed)
        //        {
        //            return new List<Properties>() { 
        //                new Properties() { Id = 1, Name = "Column 1" },
        //                new Properties() { Id = 2, Name = "Column 2" },
        //                new Properties() { Id = 3, Name = "Column 3" },
        //                new Properties() { Id = 4, Name = "Column 4" }
        //            };
        //        }
        //        else
        //        {
        //            return new List<Properties>() { 
        //                new Properties() { Id = 1, Name = "Column 1" },
        //                new Properties() { Id = 2, Name = "Column 2" },
        //                new Properties() { Id = 3, Name = "Column 3" },
        //                new Properties() { Id = 4, Name = "Column 4" },
        //                new Properties() { Id = 5, Name = "Column 5" }
        //            };
        //        }

        //    }
        //}
        //public ObservableCollection<ObjectFieldsViewModel> DBFieldsObsList { get; set; }
        public DataContractViewModel(ObjectTableViewModel OTViewModel, bool IsNew)
        {
            viewModel = OTViewModel;
            DBFieldsObsList = new ObservableCollection<ObjectFieldsViewModel>();
            DBFieldsTempObsList = new ObservableCollection<ObjectFieldsViewModel>();
            //DBFieldsTempObsList = DBFieldsObsList;
            this.DCName = viewModel.ObjectTableName;// + "ApiV" + (OTViewModel.DataContractsObsList == null ? 1 : (OTViewModel.DataContractsObsList.Count + 1));
            this.DCVersion = "ApiV" + (OTViewModel.DataContractsObsList == null ? 1 : (OTViewModel.DataContractsObsList.Count + 1));
            //var temp = OTViewModel.ObsList.Where(a => a.IsDBField == true);
            //foreach (var item in temp)
            //{
            //    DBFieldsObsList.Add(item);
            //}
            //dataContractViewModel = DCViewModel;
        }

        //private ObservableCollection<ObjectFieldsViewModel> obsList;
        //public ObservableCollection<ObjectFieldsViewModel> ObsList
        //{
        //    get
        //    {
        //        var temp = viewModel.ObsList.Where(a => a.FieldName != "Id" && a.FieldName != "Tenant");
        //        ObservableCollection<ObjectFieldsViewModel> ObsListtemp = new ObservableCollection<ObjectFieldsViewModel>();
        //        foreach (var item in temp)
        //        {
        //            ObsListtemp.Add(item);
        //        }
        //        obsList = ObsListtemp;
        //        return obsList;
        //    }
        //    set
        //    {
        //        obsList = value;
        //        FirePropertyChanged("ObsList");
        //    }
        //}

        ObservableCollection<ObjectFieldsViewModel> dBFieldsObsList;
        public ObservableCollection<ObjectFieldsViewModel> DBFieldsObsList
        {
            get
            {
                if (dBFieldsObsList == null || dBFieldsObsList.Count == 0)
                {
                    var temp = viewModel.ObsList.Where(a => a.IsPMField == true);
                    var DefaultDCFields = new ObservableCollection<DataContractFieldViewModel>();
                    if (DCFieldsObsList == null)
                    {
                        DCFieldsObsList = new ObservableCollection<DataContractFieldViewModel>(); ;
                    }
                    foreach (var item in temp)
                    {
                        var myCheck =  DCFieldsObsList.Where(a=>a.FieldName.ToLower() == item.FieldName.ToLower()).FirstOrDefault();
                        if ((item.IsPrimaryKey) && myCheck == null)
                        {
                            var InsertItem = new DataContractFieldViewModel();
                            InsertItem.FieldName = item.FieldName;
                            InsertItem.IsMulti = item.IsMulti;
                            InsertItem.MultiTableName = item.MultiTableName;
                           
                            if (InsertItem.FieldDataType == "LookUp")
                            {
                                if (InsertItem.FieldName.Contains("Id"))
                                {
                                    InsertItem.DCFieldName = item.FieldName.Replace("Id", "");
                                }
                                else
                                {
                                    InsertItem.DCFieldName = item.FieldName;
                                }
                                InsertItem.FieldDataType = item.FieldName.Replace("Id", "");// +"ApiV1";
                                InsertItem.DCVersion = "ApiV1";
                                InsertItem.IsCustomType = true;
                            }
                            else
                            {
                                InsertItem.DCFieldName = item.FieldName;
                                InsertItem.FieldDataType = item.FieldDataType;
                                InsertItem.IsCustomType = false;
                            }
                            InsertItem.IsKey = item.IsPrimaryKey;// false;
                            InsertItem.IsSpecialField = false;
                            InsertItem.IsNullable = item.IsNullable;

                            DCFieldsObsList.Add(InsertItem);
                        }
                        else
                        {
                            dBFieldsObsList.Add(item);
                        }
                        FirePropertyChanged("DCFieldsObsList"); 
                        //if (DBFieldsTempObsList == null)
                        //{
                        //    DBFieldsTempObsList = new ObservableCollection<ObjectFieldsViewModel>(); 
                        //}
                        //DBFieldsTempObsList.Add(item);
                    }
                    if (DBFieldsTempObsList.Count == 0 && dBFieldsObsList.Count > 0)
                    {
                        foreach (var item1 in dBFieldsObsList)
                        {
                            DBFieldsTempObsList.Add(item1);
                        }
                       
                    }
                   

                }
                FirePropertyChanged("DBFieldsTempObsList");
                return dBFieldsObsList;
            }
            set
            {
                dBFieldsObsList = value;
                FirePropertyChanged("DBFieldsObsList");
            }
        }

        ObservableCollection<ObjectFieldsViewModel> dBFieldsTempObsList;
        public ObservableCollection<ObjectFieldsViewModel> DBFieldsTempObsList
        {
            get
            {
                return dBFieldsTempObsList;
            }
            set
            {
                dBFieldsTempObsList = value;
                FirePropertyChanged("DBFieldsTempObsList");
            }
        }

        ObservableCollection<DataContractFieldViewModel> dCFieldsObsList;
        public ObservableCollection<DataContractFieldViewModel> DCFieldsObsList
        {
            get
            {
                return dCFieldsObsList;
            }
            set
            {
                dCFieldsObsList = value;
                FirePropertyChanged("DCFieldsObsList");
            }
        }

        private DataContractFieldViewModel selectedDCField;
        public DataContractFieldViewModel SelectedDCField
        {
            get
            {
                return selectedDCField;
            }
            set
            {
                selectedDCField = value;
                if (selectedDCField != null)
                {
                    DCFieldEditVisibility = Visibility.Visible;
                }
                else
                {
                    DCFieldEditVisibility = Visibility.Collapsed;
                }

                FirePropertyChanged("SelectedDCField");
            }
        }

        Visibility dCFieldEditVisibility = Visibility.Collapsed;
        public Visibility DCFieldEditVisibility
        {
            get { return dCFieldEditVisibility; }
            set { dCFieldEditVisibility = value; FirePropertyChanged("DCFieldEditVisibility"); }
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

        private string dcName;
        public string DCName
        {
            get
            {
                return dcName;
            }
            set
            {
                dcName = value;
                FirePropertyChanged("DCName");
            }
        }

        private string dcVersion = "ApiV1";
        public string DCVersion
        {
            get
            {
                return dcVersion;
            }
            set
            {
                dcVersion = value;
                FirePropertyChanged("DCVersion");
            }
        }

        private string computingPartnerName;
        public string ComputingPartnerName
        {
            get
            {
                return computingPartnerName;
            }
            set
            {
                computingPartnerName = value;
                FirePropertyChanged("ComputingPartnerName");
            }
        }

        private bool isNew = true;
        public bool IsNew
        {
            get
            {
                return isNew;
            }
            set
            {
                isNew = value;
                FirePropertyChanged("IsNew");
            }
        }

        private bool includeTenant0Data =false;
        public bool IncludeTenant0Data
        {
            get
            {
                return includeTenant0Data;
            }
            set
            {
                includeTenant0Data = value;
                FirePropertyChanged("IncludeTenant0Data");
            }
        }

        public AddDataContractControl DataContractControl;
        public Window DCWindow = new Window();
        public RelayCommand EditBtnCommand
        {
            get { return new RelayCommand(() => this.EditBtnMethod()); }
        }
        private void EditBtnMethod()
        {
            this.IsNew = false;
            DataContractViewModel model = this;
            //model.ButtonsVisibility = Visibility.Visible;
            DataContractControl = new AddDataContractControl();
            DataContractControl.DataContext = model;

            DCWindow = new Window();
            DCWindow.Width = 500;
            DCWindow.Height = 235;
            DCWindow.Content = DataContractControl;
            DCWindow.Show();
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
            viewModel.DCWindow.Close();
            if (this.DCWindow != null)
            {
                this.DCWindow.Close();
            }

        }

        private void OkBtnMethod()
        {
            ErrorMessages = string.Empty;

            //this.ValidateEntries();

            //if (viewModel.SelectedScreen.obs.Contains(viewModel.SelectedScreen.QueryColumnObsList.Where(aa => aa.ObjectFieldName == ObjectFieldName).FirstOrDefault()))
            //{
            //    ErrorMessages += ObjectFieldName + " is already exists ...";
            //}

            if (viewModel.DataContractsObsList != null && viewModel.DataContractsObsList.Where(a => a.DCName == this.DCName && a.DCVersion == this.DCVersion).Count() > 0)
            {
                var temp = viewModel.DataContractsObsList.Where(a => a.DCName == this.DCName && a.DCVersion == this.DCVersion).FirstOrDefault();
                if (temp != null && temp.IsNew == false)
                {
                    viewModel.DataContractsObsList.Remove(temp);
                }
                else
                {
                    ErrorMessages = "There already DataContract With this name and Version .";
                }
                
            }
            if (string.IsNullOrEmpty(DCName))
            {
                ErrorMessages = "Data Contract Name Is Required";
            }
            if (ErrorMessages == "")
            {
                viewModel.UpdateDataContractsObsList(this);
                viewModel.DCWindow.Close();
                if (this.DCWindow != null)
                {
                     this.DCWindow.Close();
                }
            }
            else
            {
                ErrorsVisibility = Visibility.Visible;
            }

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

        Visibility errorsVisibility = Visibility.Collapsed;
        public Visibility ErrorsVisibility
        {
            get { return errorsVisibility; }
            set { errorsVisibility = value; FirePropertyChanged("ErrorsVisibility"); }
        }

        //private void ValidateEntries()
        //{
        //    StringBuilder str = new StringBuilder();

        //    if (string.IsNullOrEmpty(ObjectFieldName))
        //    {
        //        str.AppendLine("Object Field Name is Required");
        //    }

        //    if (Column == 0)
        //    {
        //        str.AppendLine("You Should Select Column ..");
        //    }

        //    ErrorMessages = str.ToString();
        //    if (ErrorMessages != "")
        //    {
        //        ErrorsVisibility = Visibility.Visible;
        //    }
        //    else
        //    {
        //        ErrorsVisibility = Visibility.Collapsed;
        //    }

        //    //FirePropertyChanged("ErrorMessages");
        //}
        //public RelayCommand<ScreenFieldViewModel> RemoveScreenFieldCol1Command
        //{
        //    get { return new RelayCommand<ScreenFieldViewModel>(Qmodel => this.RemovScreenFieldCol1Method(Qmodel)); }
        //}
        //private void RemovScreenFieldCol1Method(ScreenFieldViewModel QModel)
        //{
        //    ScreenViewModel.ScreenFieldCol1ObsList.Remove(this);
        //}

        //public RelayCommand<ScreenFieldViewModel> RemoveScreenFieldCol2Command
        //{
        //    get { return new RelayCommand<ScreenFieldViewModel>(Qmodel => this.RemovScreenFieldCol2Method(Qmodel)); }
        //}
        //private void RemovScreenFieldCol2Method(ScreenFieldViewModel QModel)
        //{
        //    ScreenViewModel.ScreenFieldCol2ObsList.Remove(this);
        //}

        //public RelayCommand<ScreenFieldViewModel> RemoveScreenFieldCol3Command
        //{
        //    get { return new RelayCommand<ScreenFieldViewModel>(Qmodel => this.RemovScreenFieldCol3Method(Qmodel)); }
        //}
        //private void RemovScreenFieldCol3Method(ScreenFieldViewModel QModel)
        //{
        //    ScreenViewModel.ScreenFieldCol3ObsList.Remove(this);
        //}

        //public RelayCommand<ScreenFieldViewModel> RemoveScreenFieldCol4Command
        //{
        //    get { return new RelayCommand<ScreenFieldViewModel>(Qmodel => this.RemovScreenFieldCol4Method(Qmodel)); }
        //}
        //private void RemovScreenFieldCol4Method(ScreenFieldViewModel QModel)
        //{
        //    ScreenViewModel.ScreenFieldCol4ObsList.Remove(this);
        //}

        //public RelayCommand<ScreenFieldViewModel> RemoveScreenFieldCol5Command
        //{
        //    get { return new RelayCommand<ScreenFieldViewModel>(Qmodel => this.RemovScreenFieldCol5Method(Qmodel)); }
        //}
        //private void RemovScreenFieldCol5Method(ScreenFieldViewModel QModel)
        //{
        //    ScreenViewModel.ScreenFieldCol5ObsList.Remove(this);
        //}


        ObjectFieldsViewModel selectedDBField;

        public ObjectFieldsViewModel SelectedDBField
        {
            get { return selectedDBField; }
            set
            {
                selectedDBField = value;
                FirePropertyChanged("SelectedDBField");
            }
        }

        public void FireDCFieldsObsList()
        {
            FirePropertyChanged("DCFieldsObsList");
        }

        internal void FireDBFieldsObsList(bool BuildFields = false)
        {
            if (DBFieldsTempObsList.Count == 0 && DBFieldsObsList.Count > 0)
            {
                DBFieldsTempObsList = DBFieldsObsList;
            }
            foreach (var item1 in DCFieldsObsList)
            {
                var temp = viewModel.ObsList.Where(a => a.FieldName == item1.FieldName).FirstOrDefault();
                DBFieldsTempObsList.Remove(temp);
            }
            //if (BuildFields == true)
            //{
            //    DBFieldsObsList = new ObservableCollection<ObjectFieldsViewModel>();
            //    var temp = viewModel.ObsList.Where(a => a.IsDBField == true);
            //    foreach (var item in temp)
            //    {
            //        DBFieldsObsList.Add(item);
            //    }
            //}
            FirePropertyChanged("DBFieldsTempObsList");
        }
    }
    public class Properties : PropertyChangedImplementation
    {
        private int id;
        public int Id
        {
            get
            {
                return id;
            }
            set
            {
                id = value;
                FirePropertyChanged("Id");
            }
        }

        private string name;
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
}
