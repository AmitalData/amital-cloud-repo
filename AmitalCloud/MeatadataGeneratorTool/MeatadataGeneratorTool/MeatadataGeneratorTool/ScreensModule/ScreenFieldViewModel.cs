using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MeatadataGeneratorTool.ScreensModule
{
    public class ScreenFieldViewModel : PropertyChangedImplementation
    {
        ObjectTableViewModel viewModel;
        ScreensViewModel ScreenViewModel;
        public List<Properties> Columns 
        {
            get
            {
                if (ScreenViewModel.Column2Visibility == Visibility.Collapsed)
                {
                    return new List<Properties>() { 
                        new Properties() { Id = 1, Name = "Column 1" }
                    };
                }
                else if (ScreenViewModel.Column3Visibility == Visibility.Collapsed)
                {
                    return new List<Properties>() { 
                        new Properties() { Id = 1, Name = "Column 1" },
                        new Properties() { Id = 2, Name = "Column 2" }
                    };
                }
                else if (ScreenViewModel.Column4Visibility == Visibility.Collapsed)
                {
                    return new List<Properties>() { 
                        new Properties() { Id = 1, Name = "Column 1" },
                        new Properties() { Id = 2, Name = "Column 2" },
                        new Properties() { Id = 3, Name = "Column 3" }
                    };
                }
                else if (ScreenViewModel.Column5Visibility == Visibility.Collapsed)
                {
                    return new List<Properties>() { 
                        new Properties() { Id = 1, Name = "Column 1" },
                        new Properties() { Id = 2, Name = "Column 2" },
                        new Properties() { Id = 3, Name = "Column 3" },
                        new Properties() { Id = 4, Name = "Column 4" }
                    };
                }
                else
                {
                    return new List<Properties>() { 
                        new Properties() { Id = 1, Name = "Column 1" },
                        new Properties() { Id = 2, Name = "Column 2" },
                        new Properties() { Id = 3, Name = "Column 3" },
                        new Properties() { Id = 4, Name = "Column 4" },
                        new Properties() { Id = 5, Name = "Column 5" }
                    };
                }
                
            }
        }

        public List<Properties> rows;
        private void SetRowsComboBoxList(int SelectedCoulmn)
        {

            int CoulmnCount = 0;
            if (Column == 1)
            {
                CoulmnCount = ScreenViewModel.ScreenFieldCol1ObsList.Count;

            }
            else if (Column == 2)
            {

                CoulmnCount = ScreenViewModel.ScreenFieldCol2ObsList.Count;
            }
            else if (Column == 3)
            {
                CoulmnCount = ScreenViewModel.ScreenFieldCol3ObsList.Count;
            }
            else if (Column == 4)
            {
                CoulmnCount = ScreenViewModel.ScreenFieldCol4ObsList.Count;
            }
            else if (Column == 5)
            {
                CoulmnCount = ScreenViewModel.ScreenFieldCol5ObsList.Count;
            }
            else
            {
                CoulmnCount = 5;
            }
            List<Properties> _Properties = new List<Properties>();

            for (int i = CoulmnCount; i < 5; i++)
            {
                _Properties.Add(new Properties() { Id = (i+1), Name = "Row " + (i + 1) });
            }

            Rows = _Properties;

        }
   
        public List<Properties> Rows
        {
            get
            {
                return rows;

            }

            set
            {
                rows = value;
                FirePropertyChanged("Rows");
            }
        }
        public ScreenFieldViewModel(ObjectTableViewModel OTViewModel, ScreensViewModel SViewModel, bool IsNew)
        {
            viewModel = OTViewModel;
            ScreenViewModel = SViewModel;
        }

        private ObservableCollection<ObjectFieldsViewModel> obsList;
        public ObservableCollection<ObjectFieldsViewModel> ObsList
        {
            get
            {
                var temp = viewModel.ObsList.Where(a => a.FieldName != "Id" && a.FieldName != "Tenant");
                ObservableCollection<ObjectFieldsViewModel> ObsListtemp = new ObservableCollection<ObjectFieldsViewModel>();
                ObsListtemp.Add(new ObjectFieldsViewModel(null, true));
                ObsListtemp[0].FieldName = "Empty_Field";
                ObsListtemp[0].DefaultText = "Empty Field (NULL For Spaces)";
     
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

        private int row;
        public int Row
        {
            get
            {
                return row;
            }
            set
            {
                row = value;
                FirePropertyChanged("Row");
            }
        }

        private int column;
        public int Column
        {
            get
            {
                return column;
            }
            set
            {
                column = value;
                SetRowsComboBoxList(value);
                FirePropertyChanged("Column");
            }
        }
         
        public string ScreenName
        {
            get
            {
                return ScreenViewModel.Name;
            }
            set
            {
                if (string.IsNullOrEmpty(ScreenViewModel.Code))
                {
                    ScreenViewModel.Name = value;
                    FirePropertyChanged("ScreenViewModel");
                }
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
            ScreenViewModel.ScreenFieldWindow.Close();
        }

        private void OkBtnMethod()
        {
            ErrorMessages = string.Empty;

            this.ValidateEntries();

            //if (viewModel.SelectedScreen.obs.Contains(viewModel.SelectedScreen.QueryColumnObsList.Where(aa => aa.ObjectFieldName == ObjectFieldName).FirstOrDefault()))
            //{
            //    ErrorMessages += ObjectFieldName + " is already exists ...";
            //}

      
            if (ErrorMessages == "")
            {
                ScreenViewModel.UpdateScreenFieldsList(this);
                ScreenViewModel.ScreenFieldWindow.Close();
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

        private void ValidateEntries()
        {
            StringBuilder str = new StringBuilder();
            
            if (string.IsNullOrEmpty(ObjectFieldName))
            {
                str.AppendLine("Object Field Name is Required");
            }

            if (Column == 0)
            {
                str.AppendLine("You Should Select Column ..");
            }

            //if (Row == 0)
            //{
            //    str.AppendLine("You Should Select Row ..");
            //}

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
        public RelayCommand<ScreenFieldViewModel> RemoveScreenFieldCol1Command
        {
            get { return new RelayCommand<ScreenFieldViewModel>(Qmodel => this.RemovScreenFieldCol1Method(Qmodel)); }
        }
        private void RemovScreenFieldCol1Method(ScreenFieldViewModel QModel)
        {
            ScreenViewModel.ScreenFieldCol1ObsList.Remove(this);
        }

        public RelayCommand<ScreenFieldViewModel> RemoveScreenFieldCol2Command
        {
            get { return new RelayCommand<ScreenFieldViewModel>(Qmodel => this.RemovScreenFieldCol2Method(Qmodel)); }
        }
        private void RemovScreenFieldCol2Method(ScreenFieldViewModel QModel)
        {
            ScreenViewModel.ScreenFieldCol2ObsList.Remove(this);
        }

        public RelayCommand<ScreenFieldViewModel> RemoveScreenFieldCol3Command
        {
            get { return new RelayCommand<ScreenFieldViewModel>(Qmodel => this.RemovScreenFieldCol3Method(Qmodel)); }
        }
        private void RemovScreenFieldCol3Method(ScreenFieldViewModel QModel)
        {
            ScreenViewModel.ScreenFieldCol3ObsList.Remove(this);
        }

        public RelayCommand<ScreenFieldViewModel> RemoveScreenFieldCol4Command
        {
            get { return new RelayCommand<ScreenFieldViewModel>(Qmodel => this.RemovScreenFieldCol4Method(Qmodel)); }
        }
        private void RemovScreenFieldCol4Method(ScreenFieldViewModel QModel)
        {
            ScreenViewModel.ScreenFieldCol4ObsList.Remove(this);
        }

        public RelayCommand<ScreenFieldViewModel> RemoveScreenFieldCol5Command
        {
            get { return new RelayCommand<ScreenFieldViewModel>(Qmodel => this.RemovScreenFieldCol5Method(Qmodel)); }
        }
        private void RemovScreenFieldCol5Method(ScreenFieldViewModel QModel)
        {
            ScreenViewModel.ScreenFieldCol5ObsList.Remove(this);
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
