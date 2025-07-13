using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;

namespace MeatadataGeneratorTool.MenuButtons
{
    public class MenuButtonViewModel : PropertyChangedImplementation
    {
        ObjectTableViewModel viewModel;
        string Type = "";
        public MenuButtonViewModel(ObjectTableViewModel OTableVM, bool IsNew,string type)
        {
            viewModel = OTableVM;
            Type = type;
        }

        public void RefreshMenuItems()
        {
            FirePropertyChanged("MenuButtonItems");
        }
        public List<Type> MenuButtonTypes
        {
            get
            {
                if (Type == "Item")
                {
                    //SelectedMenuButtonType = "menuitem";
                    return new List<Type>() { 
                        new Type() { Code = "menuitem", Name = "Menu Item" },
                         new Type() { Code = "separator", Name = "Separator" }
                    };  
                }
                else
                {
                    return new List<Type>() { 
                        new Type() { Code = "button", Name = "Button" },
                        //new Type() { Code = "menuitem", Name = "Menu Item" },
                        new Type() { Code = "dropdownbutton", Name = "Drop Down Button" },
                        
                    };  
                }
               
            }
        }

        private int width;
        public int Width
        {
            get
            {
                return width;
            }
            set
            {
                width = value;
                FirePropertyChanged("Width");
            }
        }


        private string htmlComponentPath;
        public string HtmlComponentPath
        {
            get
            {
                return htmlComponentPath;
            }
            set
            {
                htmlComponentPath = value;
                FirePropertyChanged("HtmlComponentPath");
            }
        }

        private string eventCode;
        public string EventCode
        {
            get
            {
                return eventCode;
            }
            set
            {
                eventCode = value;
                FirePropertyChanged("EventCode");
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
        private string defaultText;
        public string DefaultText
        {
            get
            {
                return defaultText;
            }
            set
            {
                defaultText = value;
                FirePropertyChanged("DefaultText");
            }
        }
        private string selectedMenuButtonType;
        public string SelectedMenuButtonType
        {
            get
            {
                if (Type == "Item" && selectedMenuButtonType == null)
                {
                    selectedMenuButtonType = "menuitem";
                }
                return selectedMenuButtonType;
            }
            set
            {
                selectedMenuButtonType = value;
                FirePropertyChanged("SelectedMenuButtonType");
            }
        }

        private string style;
        public string Style
        {
            get
            {
                return style;
            }
            set
            {
                style = value;
                FirePropertyChanged("Style");
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

        private ObservableCollection<MenuButtonViewModel> MenuButtonitems;
        public ObservableCollection<MenuButtonViewModel> MenuButtonItems
        {
            get
            {
                return MenuButtonitems;
            }
            set
            {
                MenuButtonitems = value;
                FirePropertyChanged("MenuButtonItems");
            }
        }

        private Visibility buttonsVisibility = Visibility.Collapsed;
        public Visibility ButtonsVisibility
        {
            get
            {
                return buttonsVisibility;
            }
            set
            {
                buttonsVisibility = value;
                FirePropertyChanged("ButtonsVisibility");
            }
        }

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


        private string localDefaultText;
        public string LocalDefaultText
        {
            get
            {
                return localDefaultText;
            }
            set
            {
                localDefaultText = value;
                FirePropertyChanged("LocalDefaultText");
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
            viewModel.tabWindow.Close();
        }

        private void OkBtnMethod()
        {
            ErrorMessages = string.Empty;

            this.ValidateEntries();

            if (ErrorMessages == "")
            {
                ButtonsVisibility = Visibility.Collapsed;
                if (Type == "Item")
                {
                    if (viewModel.SelectedMenuButton.MenuButtonItems == null)
                    {
                        viewModel.SelectedMenuButton.MenuButtonItems = new ObservableCollection<MenuButtonViewModel>();
                    }

                    if (viewModel.SelectedMenuButton.MenuButtonItems.Count > 0)
                        this.IndexOrder = viewModel.SelectedMenuButton.MenuButtonItems.OrderByDescending(f => f.IndexOrder).FirstOrDefault().IndexOrder + 1;
                    viewModel.SelectedMenuButton.MenuButtonItems.Add(this); 
                }
                else
                {
                    viewModel.UpdateMenuButtonsObsList(this);
                }
                MenuButtonsDetailsVisibility = Visibility.Visible;
                viewModel.MenuButtonWindow.Close();
            }

        }

        private void ValidateEntries()
        {
            StringBuilder str = new StringBuilder();
            if (string.IsNullOrEmpty(this.EventCode))
            {
                str.AppendLine("Event Code is Required");
            }

            if (string.IsNullOrEmpty(this.DefaultText))
            {
                str.AppendLine("Default Text is Required");
            }
            else if (ContainsHebrewCharacters(this.DefaultText))
            {
                str.AppendLine("Default Text cannot contain Hebrew characters");
            }

            if (string.IsNullOrEmpty(this.SelectedMenuButtonType))
            {
                str.AppendLine("Menu Button Type is Required");
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

                FirePropertyChanged("ErrorMessages");
        }

        Visibility errorsVisibility = Visibility.Collapsed;
        public Visibility ErrorsVisibility
        {
            get { return errorsVisibility; }
            set { errorsVisibility = value; FirePropertyChanged("ErrorsVisibility"); }
        }

        Visibility menuButtonsDetailsVisibility = Visibility.Collapsed;
        public Visibility MenuButtonsDetailsVisibility
        {
            get { return menuButtonsDetailsVisibility; }
            set { menuButtonsDetailsVisibility = value; FirePropertyChanged("MenuButtonsDetailsVisibility"); }
        }


        private bool ContainsHebrewCharacters(string text)
        {
            return Regex.IsMatch(text, @"[\u0590-\u05FF]");
        }

        public string TextCodeCode { get; internal set; }
        public string FeatureTextCodeCode { get; internal set; }
        public string FeatureDefaultText { get; internal set; }
        public string FeatureCode { get; internal set; }
        public bool IsPackagable { get; set; }
    }

    public class Type : PropertyChangedImplementation
    {
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
