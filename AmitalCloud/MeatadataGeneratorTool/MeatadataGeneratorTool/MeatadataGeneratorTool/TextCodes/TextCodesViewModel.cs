using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;

namespace MeatadataGeneratorTool.TextCodes
{
    public class TextCodesViewModel : PropertyChangedImplementation
    {
        ObjectTableViewModel viewModel;
        public TextCodesViewModel(ObjectTableViewModel OTableVM, bool IsNew)
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

        private string textCodeTypeCode;
        public string TextCodeTypeCode
        {
            get
            {
                return textCodeTypeCode;
            }
            set
            {
                textCodeTypeCode = value;
                FirePropertyChanged("TextCodeTypeCode");
            }
        }

    
        private bool isSpellChecked;
        public bool IsSpellChecked
        {
            get
            {
                return isSpellChecked;
            }
            set
            {
                isSpellChecked = value;
                FirePropertyChanged("IsSpellChecked");
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
            viewModel.TextCodesWindow.Close();
        }

        private void OkBtnMethod()
        {
            ErrorMessages = string.Empty;

            this.ValidateEntries();

            if (ErrorMessages == "")
            {
                ButtonsVisibility = Visibility.Collapsed;
                viewModel.UpdateTextCodesList(this);
                 
                viewModel.TextCodesWindow.Close();
            }

        }

        private void ValidateEntries()
        {
            StringBuilder str = new StringBuilder();
            if (string.IsNullOrEmpty(this.Code))
            {
                str.AppendLine("Code is Required");
            }

            if (string.IsNullOrEmpty(this.DefaultText))
            {
                str.AppendLine("Default Text is Required");
            }
            else if (ContainsHebrewCharacters(this.DefaultText))
            {
                str.AppendLine("Default Text cannot contain Hebrew characters");
            }

            if (string.IsNullOrEmpty(this.TextCodeTypeCode))
            {
                str.AppendLine("Type is Required");
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




        private bool ContainsHebrewCharacters(string text)
        {
            return Regex.IsMatch(text, @"[\u0590-\u05FF]");
        }


    }
}
