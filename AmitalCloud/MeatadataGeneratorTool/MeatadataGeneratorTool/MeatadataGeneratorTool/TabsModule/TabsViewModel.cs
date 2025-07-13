using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;

namespace MeatadataGeneratorTool.TabsModule
{
    public class TabsViewModel : PropertyChangedImplementation
    {
        ObjectTableViewModel viewModel;
        public TabsViewModel(ObjectTableViewModel OTableVM, bool IsNew)
        {
            viewModel = OTableVM;
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
                if (string.IsNullOrEmpty(TextCode))
                {
                    TextCode = ObjectTableName + ".TH." + name.Trim().Replace(" ", "");
                } 
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
        private string controlPath;
        public string ControlPath
        {
            get
            {
                return controlPath;
            }
            set
            {
                controlPath = value;
                FirePropertyChanged("ControlPath");
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
        private string localName;
        public string LocalName
        {
            get
            {
                return localName;
            }
            set
            {
                localName = value;
                FirePropertyChanged("LocalName");
            }
        }

        private string htmlComponentName;
        public string HtmlComponentName
        {
            get
            {
                return htmlComponentName;
            }
            set
            {
                htmlComponentName = value;
                FirePropertyChanged("HtmlComponentName");
            }
        }

        private string htmlComponentURL;
        public string HtmlComponentURL
        {
            get
            {
                return htmlComponentURL;
            }
            set
            {
                htmlComponentURL = value;
                FirePropertyChanged("HtmlComponentURL");
            }
        }

        private bool isPackagable;
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


        private bool hasGeneralFeature;
        public bool HasGeneralFeature
        {
            get
            {
                return hasGeneralFeature;
            }
            set
            {
                hasGeneralFeature = value;
                FirePropertyChanged("HasGeneralFeature");
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
		private bool isLocked;
		public bool IsLocked
		{
			get
			{
				return isLocked;
			}
			set
			{
				isLocked = value;
				FirePropertyChanged("IsLocked");
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
                viewModel.UpdateTabsObsList(this);
                TabDetailsVisibility = Visibility.Visible;
                viewModel.tabWindow.Close();
            }

        }

        private void ValidateEntries()
        {
            StringBuilder str = new StringBuilder();
            if (string.IsNullOrEmpty(this.Name))
            {
                str.AppendLine("Tab Name is Required");
            }
            else if (ContainsHebrewCharacters(this.Name))
            {
                str.AppendLine("Tab Name cannot contain Hebrew characters");
            }

            if (string.IsNullOrEmpty(this.Code))
            {
                str.AppendLine("Tab Code is Required");
            }

            if (string.IsNullOrEmpty(this.ControlPath))
            {
                str.AppendLine("Control Path is Required");
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

        Visibility tabDetailsVisibility = Visibility.Collapsed;
        public Visibility TabDetailsVisibility
        {
            get { return tabDetailsVisibility; }
            set { tabDetailsVisibility = value; FirePropertyChanged("TabDetailsVisibility"); }
        }

        private bool ContainsHebrewCharacters(string text)
        {
            return Regex.IsMatch(text, @"[\u0590-\u05FF]");
        }

        public string FeatureCode { get; internal set; }
        public string FeatureTextCodeCode { get; internal set; }
        public string FeatureDefaultText { get; internal set; }
        public bool IsSpellChecked { get; internal set; }
    }
}
