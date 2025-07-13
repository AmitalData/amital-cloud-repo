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
    public class ScreensViewModel : PropertyChangedImplementation
    {
        ObjectTableViewModel viewModel;
       // public ObservableCollection<ScreenFieldViewModel> ScreenFieldObsList { get; set; }
        public ObservableCollection<ScreenFieldViewModel> ScreenFieldCol1ObsList { get; set; }
        public ObservableCollection<ScreenFieldViewModel> ScreenFieldCol2ObsList { get; set; }
        public ObservableCollection<ScreenFieldViewModel> ScreenFieldCol3ObsList { get; set; }
        public ObservableCollection<ScreenFieldViewModel> ScreenFieldCol4ObsList { get; set; }
        public ObservableCollection<ScreenFieldViewModel> ScreenFieldCol5ObsList { get; set; }
        public bool IsNew { get; set; }
        public ScreensViewModel(ObjectTableViewModel OTableVM, bool isNew)
        {
            this.IsNew = isNew;
            viewModel = OTableVM;
            if (IsHeaderScreen)
            {
                column1Visibility = Visibility.Visible;
                column2Visibility = Visibility.Collapsed;
                column3Visibility = Visibility.Collapsed;
                column4Visibility = Visibility.Collapsed;
                column5Visibility = Visibility.Collapsed;
                AddColumnBtnVisibility = Visibility.Visible;
            }
            else
            {
                column1Visibility = Visibility.Visible;
                column2Visibility = Visibility.Visible;
                column3Visibility = Visibility.Collapsed;
                column4Visibility = Visibility.Collapsed;
                column5Visibility = Visibility.Collapsed;
                AddColumnBtnVisibility = Visibility.Collapsed;
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
        private string objectTableName;
        public string ObjectTableName
        {
            get
            {
                return viewModel.ObjectTableName;
            }
            set
            {
                objectTableName = value;
                FirePropertyChanged("ObjectTableName");
            }
        }
        private bool isReadOnly;
        public bool IsReadOnly
        {
            get
            {
                return isReadOnly;
            }
            set
            {
                isReadOnly = value;
                FirePropertyChanged("IsReadOnly");
            }
        }
        private bool isHeaderScreen;
        public bool IsHeaderScreen
        {
            get
            {
                return isHeaderScreen;
            }
            set
            {
                isHeaderScreen = value;
                if (value)
                {
                    column1Visibility = Visibility.Visible;
                    column2Visibility = Visibility.Collapsed;
                    column3Visibility = Visibility.Collapsed;
                    column4Visibility = Visibility.Collapsed;
                    column5Visibility = Visibility.Collapsed;
                    AddColumnBtnVisibility = Visibility.Visible;
                }
                else
                {
                    column1Visibility = Visibility.Visible;
                    column2Visibility = Visibility.Visible;
                    column3Visibility = Visibility.Collapsed;
                    column4Visibility = Visibility.Collapsed;
                    column5Visibility = Visibility.Collapsed;
                    AddColumnBtnVisibility = Visibility.Collapsed;
                }
                FirePropertyChanged("IsHeaderScreen");
            }
        }


        public Visibility ButtonsVisibility { get; set; }

        private string errorMessages;
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

        // commands  
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
            viewModel.screenWindow.Close();
        }

        private void OkBtnMethod()
        {
            ErrorMessages = string.Empty;

            this.ValidateEntries();

            if (ErrorMessages == "")
            {
                ButtonsVisibility = Visibility.Collapsed;
                viewModel.UpdateScreensObsList(this);
                ScreenDetailsVisibility = Visibility.Visible;
                viewModel.screenWindow.Close();
            }

        }

        private void ValidateEntries()
        {
            StringBuilder str = new StringBuilder();
            if (string.IsNullOrEmpty(this.Name))
            {
                str.AppendLine("Screen Name is Required");
            }
           

            ErrorMessages = str.ToString();
            if (ErrorMessages != "")
            {
                ErrorsVisibility = Visibility.Visible;
            }

            FirePropertyChanged("ErrorMessages");
        }

        Visibility errorsVisibility = Visibility.Collapsed;
        public Visibility ErrorsVisibility
        {
            get { return errorsVisibility; }
            set { errorsVisibility = value; FirePropertyChanged("ErrorsVisibility"); }
        }

        Visibility screenDetailsVisibility = Visibility.Collapsed;
        public Visibility ScreenDetailsVisibility
        {
            get { return screenDetailsVisibility; }
            set { screenDetailsVisibility = value; FirePropertyChanged("ScreenDetailsVisibility"); }
        }

        
        public Window ScreenFieldWindow = new Window();
        AddScreenFieldControl ScreenFieldControl;

        public RelayCommand AddScreenCol1FieldCommand
        {
            get { return new RelayCommand(() => this.AddScreenCol1FieldMethod()); }
        }
        private void AddScreenCol1FieldMethod()
        {
            ErrorMessages = "";
            if (viewModel.SelectedScreen == null)
            {
                ErrorMessages += " You should select Screen !";
                ErrorsVisibility = Visibility.Visible;
            }
            else
            {
                ErrorsVisibility = Visibility.Collapsed;
                ScreenFieldViewModel model = new ScreenFieldViewModel(viewModel, viewModel.SelectedScreen, true);
                //model.ObjectTableName = ObjectTableName;
                //model.SetControlFieldsList(model);
                ScreenFieldControl = new AddScreenFieldControl();
                ScreenFieldControl.DataContext = model;

                ScreenFieldWindow = new Window();
                ScreenFieldWindow.Width = 500;
                ScreenFieldWindow.Height = 300;
                ScreenFieldWindow.Title = "New Screen Field";
                ScreenFieldWindow.Content = ScreenFieldControl;
                ScreenFieldWindow.Show();
            }

        }

        public RelayCommand AddScreenCol2FieldCommand
        {
            get { return new RelayCommand(() => this.AddScreenCol2FieldMethod()); }
        }
        private void AddScreenCol2FieldMethod()
        {
            ErrorMessages = "";
            if (viewModel.SelectedScreen == null)
            {
                ErrorMessages += " You should select Screen !";
                ErrorsVisibility = Visibility.Visible;
            }
            else
            {
                ErrorsVisibility = Visibility.Collapsed;
                ScreenFieldViewModel model = new ScreenFieldViewModel(viewModel, viewModel.SelectedScreen, true);
                //model.ObjectTableName = ObjectTableName;
                //model.SetControlFieldsList(model);
                ScreenFieldControl = new AddScreenFieldControl();
                model.Column = 2;
                ScreenFieldControl.DataContext = model;

                ScreenFieldWindow = new Window();
                ScreenFieldWindow.Width = 500;
                ScreenFieldWindow.Height = 300;
                ScreenFieldWindow.Title = "New Screen Field";
                ScreenFieldWindow.Content = ScreenFieldControl;
                ScreenFieldWindow.Show();
            }

        }

        public RelayCommand AddScreenCol3FieldCommand
        {
            get { return new RelayCommand(() => this.AddScreenCol3FieldMethod()); }
        }
        private void AddScreenCol3FieldMethod()
        {
            ErrorMessages = "";
            if (viewModel.SelectedScreen == null)
            {
                ErrorMessages += " You should select Screen !";
                ErrorsVisibility = Visibility.Visible;
            }
            else
            {
                ErrorsVisibility = Visibility.Collapsed;
                ScreenFieldViewModel model = new ScreenFieldViewModel(viewModel, viewModel.SelectedScreen, true);
                //model.ObjectTableName = ObjectTableName;
                //model.SetControlFieldsList(model);
                ScreenFieldControl = new AddScreenFieldControl();
                model.Column = 3;
                ScreenFieldControl.DataContext = model;

                ScreenFieldWindow = new Window();
                ScreenFieldWindow.Width = 500;
                ScreenFieldWindow.Height = 300;
                ScreenFieldWindow.Title = "New Screen Field";
                ScreenFieldWindow.Content = ScreenFieldControl;
                ScreenFieldWindow.Show();
            }

        }

        public RelayCommand AddScreenCol4FieldCommand
        {
            get { return new RelayCommand(() => this.AddScreenCol4FieldMethod()); }
        }
        private void AddScreenCol4FieldMethod()
        {
            ErrorMessages = "";
            if (viewModel.SelectedScreen == null)
            {
                ErrorMessages += " You should select Screen !";
                ErrorsVisibility = Visibility.Visible;
            }
            else
            {
                ErrorsVisibility = Visibility.Collapsed;
                ScreenFieldViewModel model = new ScreenFieldViewModel(viewModel, viewModel.SelectedScreen, true);
                //model.ObjectTableName = ObjectTableName;
                //model.SetControlFieldsList(model);
                ScreenFieldControl = new AddScreenFieldControl();
                model.Column = 4;
                ScreenFieldControl.DataContext = model;

                ScreenFieldWindow = new Window();
                ScreenFieldWindow.Width = 500;
                ScreenFieldWindow.Height = 300;
                ScreenFieldWindow.Title = "New Screen Field";
                ScreenFieldWindow.Content = ScreenFieldControl;
                ScreenFieldWindow.Show();
            }

        }

        public RelayCommand AddScreenCol5FieldCommand
        {
            get { return new RelayCommand(() => this.AddScreenCol5FieldMethod()); }
        }
        private void AddScreenCol5FieldMethod()
        {
            ErrorMessages = "";
            if (viewModel.SelectedScreen == null)
            {
                ErrorMessages += " You should select Screen !";
                ErrorsVisibility = Visibility.Visible;
            }
            else
            {
                ErrorsVisibility = Visibility.Collapsed;
                ScreenFieldViewModel model = new ScreenFieldViewModel(viewModel, viewModel.SelectedScreen, true);
                //model.ObjectTableName = ObjectTableName;
                //model.SetControlFieldsList(model);
                ScreenFieldControl = new AddScreenFieldControl();
                model.Column = 5;
                ScreenFieldControl.DataContext = model;

                ScreenFieldWindow = new Window();
                ScreenFieldWindow.Width = 500;
                ScreenFieldWindow.Height = 300;
                ScreenFieldWindow.Title = "New Screen Field";
                ScreenFieldWindow.Content = ScreenFieldControl;
                ScreenFieldWindow.Show();
            }

        }

        public void UpdateScreenFieldsList(ScreenFieldViewModel item)
        {
            if (item.Column == 1)
            {
                if (ScreenFieldCol1ObsList == null)
                {
                    ScreenFieldCol1ObsList = new ObservableCollection<ScreenFieldViewModel>();
                }
                if (ScreenFieldCol1ObsList.Where(a => a.ObjectFieldName == item.ObjectFieldName).FirstOrDefault() == null)
                {
                    while ((item.Row - 1) > ScreenFieldCol1ObsList.Count)
                    {
                        var Empty_Field = new ScreenFieldViewModel(viewModel, viewModel.SelectedScreen, true);
                        Empty_Field.ObjectFieldName = "Empty_Field";
                        ScreenFieldCol1ObsList.Add(Empty_Field);
                    }
                    ScreenFieldCol1ObsList.Add(item);
                    FirePropertyChanged("ScreenFieldCol1ObsList");
                } 
            }
            else if (item.Column == 2)
            {
                if (ScreenFieldCol2ObsList == null)
                {
                    ScreenFieldCol2ObsList = new ObservableCollection<ScreenFieldViewModel>();
                }
                if (ScreenFieldCol2ObsList.Where(a => a.ObjectFieldName == item.ObjectFieldName).FirstOrDefault() == null)
                {
                    while ((item.Row -1 ) > ScreenFieldCol2ObsList.Count)
                    {
                        var Empty_Field = new ScreenFieldViewModel(viewModel, viewModel.SelectedScreen, true);
                        Empty_Field.ObjectFieldName = "Empty_Field";
                        ScreenFieldCol2ObsList.Add(Empty_Field);
                    }
                    ScreenFieldCol2ObsList.Add(item);
                    FirePropertyChanged("ScreenFieldCol2ObsList");
                }
            }
            else if (item.Column == 3)
            {
                if (ScreenFieldCol3ObsList == null)
                {
                    ScreenFieldCol3ObsList = new ObservableCollection<ScreenFieldViewModel>();
                }
                if (ScreenFieldCol3ObsList.Where(a => a.ObjectFieldName == item.ObjectFieldName).FirstOrDefault() == null)
                {
                    while ((item.Row - 1) > ScreenFieldCol3ObsList.Count)
                    {
                        var Empty_Field = new ScreenFieldViewModel(viewModel, viewModel.SelectedScreen, true);
                        Empty_Field.ObjectFieldName = "Empty_Field";
                        ScreenFieldCol3ObsList.Add(Empty_Field);
                    }
                    ScreenFieldCol3ObsList.Add(item);
                    FirePropertyChanged("ScreenFieldCol3ObsList");
                }
            }
            else if (item.Column == 4)
            {
                if (ScreenFieldCol4ObsList == null)
                {
                    ScreenFieldCol4ObsList = new ObservableCollection<ScreenFieldViewModel>();
                }
                if (ScreenFieldCol4ObsList.Where(a => a.ObjectFieldName == item.ObjectFieldName).FirstOrDefault() == null)
                {
                    while ((item.Row - 1) > ScreenFieldCol4ObsList.Count)
                    {
                        var Empty_Field = new ScreenFieldViewModel(viewModel, viewModel.SelectedScreen, true);
                        Empty_Field.ObjectFieldName = "Empty_Field";
                        ScreenFieldCol4ObsList.Add(Empty_Field);
                    }
                    ScreenFieldCol4ObsList.Add(item);
                    FirePropertyChanged("ScreenFieldCol4ObsList");
                }
            }
            else if (item.Column == 5)
            {
                if (ScreenFieldCol5ObsList == null)
                {
                    ScreenFieldCol5ObsList = new ObservableCollection<ScreenFieldViewModel>();
                }
                if (ScreenFieldCol5ObsList.Where(a => a.ObjectFieldName == item.ObjectFieldName).FirstOrDefault() == null)
                {
                    while ((item.Row - 1) > ScreenFieldCol5ObsList.Count)
                    {
                        var Empty_Field = new ScreenFieldViewModel(viewModel, viewModel.SelectedScreen, true);
                        Empty_Field.ObjectFieldName = "Empty_Field";
                        ScreenFieldCol5ObsList.Add(Empty_Field);
                    }
                    ScreenFieldCol5ObsList.Add(item);
                    FirePropertyChanged("ScreenFieldCol5ObsList");
                }
            }  
        }

        public ScreenFieldViewModel selectedFieldCol1;
        public ScreenFieldViewModel SelectedFieldCol1
        { 
            get
            {
                return selectedFieldCol1;
            }
            set
            {
                selectedFieldCol1 = value;
                FirePropertyChanged("SelectedFieldCol1");
            }
        }

        public ScreenFieldViewModel selectedFieldCol2;
        public ScreenFieldViewModel SelectedFieldCol2
        {
            get
            {
                return selectedFieldCol2;
            }
            set
            {
                selectedFieldCol2 = value;
                FirePropertyChanged("SelectedFieldCol2");
            }
        }

        public ScreenFieldViewModel selectedFieldCol3;
        public ScreenFieldViewModel SelectedFieldCol3
        {
            get
            {
                return selectedFieldCol3;
            }
            set
            {
                selectedFieldCol3 = value;
                FirePropertyChanged("SelectedFieldCol3");
            }
        }

        public ScreenFieldViewModel selectedFieldCol4;
        public ScreenFieldViewModel SelectedFieldCol4
        {
            get
            {
                return selectedFieldCol4;
            }
            set
            {
                selectedFieldCol4 = value;
                FirePropertyChanged("SelectedFieldCol4");
            }
        }

        public ScreenFieldViewModel selectedFieldCol5;
        public ScreenFieldViewModel SelectedFieldCol5
        {
            get
            {
                return selectedFieldCol5;
            }
            set
            {
                selectedFieldCol5 = value;
                FirePropertyChanged("SelectedFieldCol5");
            }
        }

        public RelayCommand<ScreenFieldViewModel> Col1MoveUpCommand
        {
            get { return new RelayCommand<ScreenFieldViewModel>(Qmodel => this.Col1MoveUpMethod(Qmodel)); }
        }

        private void Col1MoveUpMethod(ScreenFieldViewModel Qmodel)
        {
            if (Qmodel != null)
            {
                int index = ScreenFieldCol1ObsList.IndexOf(Qmodel);
                if (index > 0)
                {
                    ScreenFieldCol1ObsList.Remove(Qmodel);
                    ScreenFieldCol1ObsList.Insert(index - 1, Qmodel);
                    SelectedFieldCol1 = Qmodel;
                }
            }
        }

        public RelayCommand<ScreenFieldViewModel> Col1MoveDownCommand
        {
            get { return new RelayCommand<ScreenFieldViewModel>(Qmodel => this.Col1MoveDownMethod(Qmodel)); }
        }

        private void Col1MoveDownMethod(ScreenFieldViewModel Qmodel)
        {
            if (Qmodel != null)
            {
                int index = ScreenFieldCol1ObsList.IndexOf(Qmodel);
                if (index < ScreenFieldCol1ObsList.Count - 1)
                {
                    ScreenFieldCol1ObsList.Remove(Qmodel);
                    ScreenFieldCol1ObsList.Insert(index + 1, Qmodel);
                    SelectedFieldCol1 = Qmodel;
                }
            }
        }

        public RelayCommand<ScreenFieldViewModel> Col1MoveRightCommand
        {
            get { return new RelayCommand<ScreenFieldViewModel>(Qmodel => this.Col1MoveRightMethod(Qmodel)); }
        }

        private void Col1MoveRightMethod(ScreenFieldViewModel Qmodel)
        {
            if (Qmodel != null)
            {
                if (ScreenFieldCol2ObsList == null)
                {
                    ScreenFieldCol2ObsList = new ObservableCollection<ScreenFieldViewModel>();
                }
                ScreenFieldCol1ObsList.Remove(Qmodel);
                Qmodel.Column = 2;
                ScreenFieldCol2ObsList.Add(Qmodel);
                SelectedFieldCol2 = Qmodel;
                FirePropertyChanged("ScreenFieldCol2ObsList");
            }
        }

        public RelayCommand<ScreenFieldViewModel> Col1MoveLeftCommand
        {
            get { return new RelayCommand<ScreenFieldViewModel>(Qmodel => this.Col1MoveLeftMethod(Qmodel)); }
        }

        private void Col1MoveLeftMethod(ScreenFieldViewModel Qmodel)
        {
            if (Qmodel != null)
            {
                if (ScreenFieldCol1ObsList == null)
                {
                    ScreenFieldCol1ObsList = new ObservableCollection<ScreenFieldViewModel>();
                }
                ScreenFieldCol2ObsList.Remove(Qmodel);
                Qmodel.Column = 1;
                ScreenFieldCol1ObsList.Add(Qmodel);
                SelectedFieldCol1 = Qmodel;
                FirePropertyChanged("ScreenFieldCol1ObsList");
            }
        }

        public RelayCommand<ScreenFieldViewModel> Col2MoveUpCommand
        {
            get { return new RelayCommand<ScreenFieldViewModel>(Qmodel => this.Col2MoveUpMethod(Qmodel)); }
        }

        private void Col2MoveUpMethod(ScreenFieldViewModel Qmodel)
        {
            if (Qmodel != null)
            {
                int index = ScreenFieldCol2ObsList.IndexOf(Qmodel);
                if (index > 0)
                {
                    ScreenFieldCol2ObsList.Remove(Qmodel);
                    ScreenFieldCol2ObsList.Insert(index - 1, Qmodel);
                    SelectedFieldCol2 = Qmodel;
                }
            }
        }

        public RelayCommand<ScreenFieldViewModel> Col2MoveDownCommand
        {
            get { return new RelayCommand<ScreenFieldViewModel>(Qmodel => this.Col2MoveDownMethod(Qmodel)); }
        }

        private void Col2MoveDownMethod(ScreenFieldViewModel Qmodel)
        {
            if (Qmodel != null)
            {
                int index = ScreenFieldCol2ObsList.IndexOf(Qmodel);
                if (index < ScreenFieldCol2ObsList.Count - 1)
                {
                    ScreenFieldCol2ObsList.Remove(Qmodel);
                    ScreenFieldCol2ObsList.Insert(index + 1, Qmodel);
                    SelectedFieldCol2 = Qmodel;
                }
            }
        }

        public RelayCommand<ScreenFieldViewModel> Col2MoveRightCommand
        {
            get { return new RelayCommand<ScreenFieldViewModel>(Qmodel => this.Col2MoveRightMethod(Qmodel)); }
        }

        private void Col2MoveRightMethod(ScreenFieldViewModel Qmodel)
        {
            if (Qmodel != null)
            {
                if (ScreenFieldCol3ObsList == null)
                {
                    ScreenFieldCol3ObsList = new ObservableCollection<ScreenFieldViewModel>();
                }
                ScreenFieldCol2ObsList.Remove(Qmodel);
                Qmodel.Column = 3;
                ScreenFieldCol3ObsList.Add(Qmodel);
                SelectedFieldCol3 = Qmodel;
                FirePropertyChanged("ScreenFieldCol3ObsList");
            }
        }

        public RelayCommand<ScreenFieldViewModel> Col2MoveLeftCommand
        {
            get { return new RelayCommand<ScreenFieldViewModel>(Qmodel => this.Col2MoveLeftMethod(Qmodel)); }
        }

        private void Col2MoveLeftMethod(ScreenFieldViewModel Qmodel)
        {
            if (Qmodel != null)
            {
                if (ScreenFieldCol2ObsList == null)
                {
                    ScreenFieldCol2ObsList = new ObservableCollection<ScreenFieldViewModel>();
                }
                ScreenFieldCol3ObsList.Remove(Qmodel);
                Qmodel.Column = 2;
                ScreenFieldCol2ObsList.Add(Qmodel);
                SelectedFieldCol2 = Qmodel;
                FirePropertyChanged("ScreenFieldCol2ObsList");
            }
        }

        public RelayCommand<ScreenFieldViewModel> Col3MoveUpCommand
        {
            get { return new RelayCommand<ScreenFieldViewModel>(Qmodel => this.Col3MoveUpMethod(Qmodel)); }
        }

        private void Col3MoveUpMethod(ScreenFieldViewModel Qmodel)
        {
            if (Qmodel != null)
            {
                int index = ScreenFieldCol3ObsList.IndexOf(Qmodel);
                if (index > 0)
                {
                    ScreenFieldCol3ObsList.Remove(Qmodel);
                    ScreenFieldCol3ObsList.Insert(index - 1, Qmodel);
                    SelectedFieldCol3 = Qmodel;
                }
            }
        }

        public RelayCommand<ScreenFieldViewModel> Col3MoveDownCommand
        {
            get { return new RelayCommand<ScreenFieldViewModel>(Qmodel => this.Col3MoveDownMethod(Qmodel)); }
        }

        private void Col3MoveDownMethod(ScreenFieldViewModel Qmodel)
        {
            if (Qmodel != null)
            {
                int index = ScreenFieldCol3ObsList.IndexOf(Qmodel);
                if (index < ScreenFieldCol3ObsList.Count - 1)
                {
                    ScreenFieldCol3ObsList.Remove(Qmodel);
                    ScreenFieldCol3ObsList.Insert(index + 1, Qmodel);
                    SelectedFieldCol3 = Qmodel;
                }
            }
        }

        public RelayCommand<ScreenFieldViewModel> Col3MoveRightCommand
        {
            get { return new RelayCommand<ScreenFieldViewModel>(Qmodel => this.Col3MoveRightMethod(Qmodel)); }
        }

        private void Col3MoveRightMethod(ScreenFieldViewModel Qmodel)
        {
            if (Qmodel != null)
            {
                if (ScreenFieldCol4ObsList == null)
                {
                    ScreenFieldCol4ObsList = new ObservableCollection<ScreenFieldViewModel>();
                }
                ScreenFieldCol3ObsList.Remove(Qmodel);
                Qmodel.Column = 4;
                ScreenFieldCol4ObsList.Add(Qmodel);
                SelectedFieldCol4 = Qmodel;
                FirePropertyChanged("ScreenFieldCol4ObsList");
            }
        }

        public RelayCommand<ScreenFieldViewModel> Col3MoveLeftCommand
        {
            get { return new RelayCommand<ScreenFieldViewModel>(Qmodel => this.Col3MoveLeftMethod(Qmodel)); }
        }

        private void Col3MoveLeftMethod(ScreenFieldViewModel Qmodel)
        {
            if (Qmodel != null)
            {
                if (ScreenFieldCol3ObsList == null)
                {
                    ScreenFieldCol3ObsList = new ObservableCollection<ScreenFieldViewModel>();
                }
                ScreenFieldCol4ObsList.Remove(Qmodel);
                Qmodel.Column = 3;
                ScreenFieldCol3ObsList.Add(Qmodel);
                SelectedFieldCol3 = Qmodel;
                FirePropertyChanged("ScreenFieldCol3ObsList");
            }
        }

        public RelayCommand<ScreenFieldViewModel> Col4MoveUpCommand
        {
            get { return new RelayCommand<ScreenFieldViewModel>(Qmodel => this.Col4MoveUpMethod(Qmodel)); }
        }

        private void Col4MoveUpMethod(ScreenFieldViewModel Qmodel)
        {
            if (Qmodel != null)
            {
                int index = ScreenFieldCol4ObsList.IndexOf(Qmodel);
                if (index > 0)
                {
                    ScreenFieldCol4ObsList.Remove(Qmodel);
                    ScreenFieldCol4ObsList.Insert(index - 1, Qmodel);
                    SelectedFieldCol4 = Qmodel;
                }
            }
        }

        public RelayCommand<ScreenFieldViewModel> Col4MoveDownCommand
        {
            get { return new RelayCommand<ScreenFieldViewModel>(Qmodel => this.Col4MoveDownMethod(Qmodel)); }
        }

        private void Col4MoveDownMethod(ScreenFieldViewModel Qmodel)
        {
            if (Qmodel != null)
            {
                int index = ScreenFieldCol4ObsList.IndexOf(Qmodel);
                if (index < ScreenFieldCol4ObsList.Count - 1)
                {
                    ScreenFieldCol4ObsList.Remove(Qmodel);
                    ScreenFieldCol4ObsList.Insert(index + 1, Qmodel);
                    SelectedFieldCol4 = Qmodel;
                }
            }
        }

        public RelayCommand<ScreenFieldViewModel> Col4MoveRightCommand
        {
            get { return new RelayCommand<ScreenFieldViewModel>(Qmodel => this.Col4MoveRightMethod(Qmodel)); }
        }

        private void Col4MoveRightMethod(ScreenFieldViewModel Qmodel)
        {
            if (Qmodel != null)
            {
                if (ScreenFieldCol5ObsList == null)
                {
                    ScreenFieldCol5ObsList = new ObservableCollection<ScreenFieldViewModel>();
                }
                ScreenFieldCol4ObsList.Remove(Qmodel);
                Qmodel.Column = 5;
                ScreenFieldCol5ObsList.Add(Qmodel);
                SelectedFieldCol5 = Qmodel;
                FirePropertyChanged("ScreenFieldCol5ObsList");
            }
        }

        public RelayCommand<ScreenFieldViewModel> Col4MoveLeftCommand
        {
            get { return new RelayCommand<ScreenFieldViewModel>(Qmodel => this.Col4MoveLeftMethod(Qmodel)); }
        }

        private void Col4MoveLeftMethod(ScreenFieldViewModel Qmodel)
        {
            if (Qmodel != null)
            {
                if (ScreenFieldCol4ObsList == null)
                {
                    ScreenFieldCol4ObsList = new ObservableCollection<ScreenFieldViewModel>();
                }
                ScreenFieldCol5ObsList.Remove(Qmodel);
                Qmodel.Column = 4;
                ScreenFieldCol4ObsList.Add(Qmodel);
                SelectedFieldCol4 = Qmodel;
                FirePropertyChanged("ScreenFieldCol4ObsList");
            }
        }

        public RelayCommand<ScreenFieldViewModel> Col5MoveUpCommand
        {
            get { return new RelayCommand<ScreenFieldViewModel>(Qmodel => this.Col5MoveUpMethod(Qmodel)); }
        }

        private void Col5MoveUpMethod(ScreenFieldViewModel Qmodel)
        {
            if (Qmodel != null)
            {
                int index = ScreenFieldCol5ObsList.IndexOf(Qmodel);
                if (index > 0)
                {
                    ScreenFieldCol5ObsList.Remove(Qmodel);
                    ScreenFieldCol5ObsList.Insert(index - 1, Qmodel);
                    SelectedFieldCol5 = Qmodel;
                }
            }
        }

        public RelayCommand<ScreenFieldViewModel> Col5MoveDownCommand
        {
            get { return new RelayCommand<ScreenFieldViewModel>(Qmodel => this.Col5MoveDownMethod(Qmodel)); }
        }

        private void Col5MoveDownMethod(ScreenFieldViewModel Qmodel)
        {
            if (Qmodel != null)
            {
                int index = ScreenFieldCol5ObsList.IndexOf(Qmodel);
                if (index < ScreenFieldCol5ObsList.Count - 1)
                {
                    ScreenFieldCol5ObsList.Remove(Qmodel);
                    ScreenFieldCol5ObsList.Insert(index + 1, Qmodel);
                    SelectedFieldCol5 = Qmodel;
                }
            }
           
        }

        Visibility column1Visibility = Visibility.Collapsed;
        public Visibility Column1Visibility
        {
            get { return column1Visibility; }
            set { column1Visibility = value; FirePropertyChanged("Column1Visibility"); }
        }

        Visibility column2Visibility = Visibility.Collapsed;
        public Visibility Column2Visibility
        {
            get { return column2Visibility; }
            set { column2Visibility = value; FirePropertyChanged("Column2Visibility"); }
        }

        Visibility column3Visibility = Visibility.Collapsed;
        public Visibility Column3Visibility
        {
            get { return column3Visibility; }
            set { column3Visibility = value; FirePropertyChanged("Column3Visibility"); }
        }

        Visibility column4Visibility = Visibility.Collapsed;
        public Visibility Column4Visibility
        {
            get { return column4Visibility; }
            set { column4Visibility = value; FirePropertyChanged("Column4Visibility"); }
        }

        Visibility column5Visibility = Visibility.Collapsed;
        public Visibility Column5Visibility
        {
            get { return column5Visibility; }
            set { column5Visibility = value; FirePropertyChanged("Column5Visibility"); }
        }

        Visibility addColumnBtnVisibility = Visibility.Collapsed;
        public Visibility AddColumnBtnVisibility
        {
            get { return addColumnBtnVisibility; }
            set { addColumnBtnVisibility = value; FirePropertyChanged("AddColumnBtnVisibility"); }
        }

        public RelayCommand AddColumnBtnCommand
        {
            get { return new RelayCommand(() => this.AddColumnBtnMethod()); }
        }

        public string Code { get;  set; }

        private void AddColumnBtnMethod()
        {
            if (Column2Visibility == Visibility.Collapsed)
            {
                Column2Visibility = Visibility.Visible;
            }
            else if (Column3Visibility == Visibility.Collapsed)
            {
                Column3Visibility = Visibility.Visible;
            }
            else if (Column4Visibility == Visibility.Collapsed)
            {
                Column4Visibility = Visibility.Visible;
            }
            else if (Column5Visibility == Visibility.Collapsed)
            {
                Column5Visibility = Visibility.Visible;
            }
            else
            {
                viewModel.ErrorMessages = "You Reached the maximum number of columns ...";
                viewModel.ErrorsVisibility = Visibility.Visible; 
            }
        }
    }
}
