using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;


namespace MeatadataGeneratorTool.Features
{
    public class FeaturesViewModel : PropertyChangedImplementation
    {
        ObjectTableViewModel viewModel;
        public FeaturesViewModel(ObjectTableViewModel OTableVM, bool IsNew)
        {
            viewModel = OTableVM;
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

        private string featureTypeCode;
        public string FeatureTypeCode
        {
            get
            {
                return featureTypeCode;
            }
            set
            {
                featureTypeCode = value;
                FirePropertyChanged("FeatureTypeCode");
            }
        }

        private string featureTextCodeCode;
        public string FeatureTextCodeCode
        {
            get
            {
                return featureTextCodeCode;
            }
            set
            {
                featureTextCodeCode = value;
                FirePropertyChanged("FeatureTextCodeCode");
            }
        }

        private string featureDefaultText;
        public string FeatureDefaultText
        {
            get
            {
                return featureDefaultText;
            }
            set
            {
                featureDefaultText = value;
                FirePropertyChanged("FeatureDefaultText");
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


        private bool isBusinessUnitEnabled;
        public bool IsBusinessUnitEnabled
        {
            get
            {
                return isBusinessUnitEnabled;
            }
            set
            {
                isBusinessUnitEnabled = value;
                FirePropertyChanged("IsBusinessUnitEnabled");
            }
        }


        private bool isOld;
        public bool IsOld
        {
            get
            {
                return isOld;
            }
            set
            {
                isOld = value;
                FirePropertyChanged("IsOld");
            }
        }
        private bool isCoreFeature;
        public bool IsCoreFeature
        {
            get
            {
                return isCoreFeature;
            }
            set
            {
                isCoreFeature = value;
                FirePropertyChanged("IsCoreFeature");
            }
        }


        private string toggleCode;
        public string ToggleCode
        {
            get
            {
                return toggleCode;
            }
            set
            {
                toggleCode = value;
                FirePropertyChanged("ToggleCode");
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
            viewModel.FeaturesWindow.Close();
        }

        private void OkBtnMethod()
        {
            ErrorMessages = string.Empty;

            this.ValidateEntries();

            if (ErrorMessages == "")
            {
                ButtonsVisibility = Visibility.Collapsed;
                viewModel.UpdateFeaturesList(this);

                viewModel.FeaturesWindow.Close();
            }

        }

        private void ValidateEntries()
        {
            StringBuilder str = new StringBuilder();
            if (string.IsNullOrEmpty(this.Code))
            {
                str.AppendLine("Code is Required");
            }

            if (string.IsNullOrEmpty(this.FeatureDefaultText))
            {
                str.AppendLine("Default Text is Required");
            }
            else if (ContainsHebrewCharacters(this.FeatureDefaultText))
            {
                str.AppendLine("Default Text cannot contain Hebrew characters");
            }

            if (string.IsNullOrEmpty(this.FeatureTypeCode))
            {
                str.AppendLine("Type is Required");
            }

            if (string.IsNullOrEmpty(this.FeatureTextCodeCode))
            {
                this.FeatureTextCodeCode = this.viewModel.ObjectTableName + ".Features." + this.Code;
            }

            
            //if (string.IsNullOrEmpty(this.LocalName))
            //{
            //    str.AppendLine("Local Name is Required");
            //}


            ErrorMessages = str.ToString();
            if (ErrorMessages != "")
            {
                ErrorsVisibility = Visibility.Visible;
            }
            else
            { 
                this.ErrorsVisibility = Visibility.Collapsed; 
            }

                FirePropertyChanged("ErrorMessages");
        }

        Visibility errorsVisibility = Visibility.Collapsed;
        public Visibility ErrorsVisibility
        {
            get { return errorsVisibility; }
            set { errorsVisibility = value; FirePropertyChanged("ErrorsVisibility"); }
        }

        private bool ContainsHebrewCharacters(string text)
        {
            return Regex.IsMatch(text, @"[\u0590-\u05FF]");
        }




    }
}