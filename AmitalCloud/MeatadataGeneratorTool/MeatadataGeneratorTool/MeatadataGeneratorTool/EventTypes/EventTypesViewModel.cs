using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MeatadataGeneratorTool.EventTypes
{
    public class EventTypesViewModel : PropertyChangedImplementation
    {
        ObjectTableViewModel viewModel;
        public EventTypesViewModel(ObjectTableViewModel OTableVM, bool IsNew)
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

        private string englishName;
        public string EnglishName
        {
            get
            {
                return englishName;
            }
            set
            {
                englishName = value; 
                FirePropertyChanged("EnglishName");
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

        private bool shortView;
        public bool ShortView
        {
            get
            {
                return shortView;
            }
            set
            {
                shortView = value;
                FirePropertyChanged("ShortView");
            }
        }

        private bool isManualEntry;
        public bool IsManualEntry
        {
            get
            {
                return isManualEntry;
            }
            set
            {
                isManualEntry = value;
                FirePropertyChanged("IsManualEntry");
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
            viewModel.EventsWindow.Close();
        }

        private void OkBtnMethod()
        {
            ErrorMessages = string.Empty;

            this.ValidateEntries();

            if (ErrorMessages == "")
            {
                ButtonsVisibility = Visibility.Collapsed;
                viewModel.UpdateEventTypesObsList(this);
                EventTypesDetailsVisibility = Visibility.Visible;
                viewModel.EventsWindow.Close();
            }

        }

        private void ValidateEntries()
        {
            StringBuilder str = new StringBuilder();
            if (string.IsNullOrEmpty(this.Code))
            {
                str.AppendLine("Tab Code is Required");
            }

            if (string.IsNullOrEmpty(this.EnglishName))
            {
                str.AppendLine("Tab English Name is Required");
            }

            if (string.IsNullOrEmpty(this.LocalName))
            {
                str.AppendLine("Local Name is Required");
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

        Visibility eventTypesDetailsVisibility = Visibility.Collapsed;
        public Visibility EventTypesDetailsVisibility
        {
            get { return eventTypesDetailsVisibility; }
            set { eventTypesDetailsVisibility = value; FirePropertyChanged("EventTypesDetailsVisibility"); }
        }

         
         
        private string eventTypeCategoryCode;
        public string EventTypeCategoryCode
        {
            get
            {
                return eventTypeCategoryCode;
            }

            set
            {
                eventTypeCategoryCode = value;
                FirePropertyChanged("EventTypeCategoryCode");
            }
        }

        private bool isAgentView;
        public bool IsAgentView
        {
            get
            {
                return isAgentView;
            }

            set
            {
                isAgentView = value;
                FirePropertyChanged("IsAgentView");
            }
        }

        private bool isCustomerView;
        public bool IsCustomerView
        {
            get
            {
                return isCustomerView;
            }

            set
            {
                isCustomerView = value;
                FirePropertyChanged("IsCustomerView");
            }
        }
        private bool allowedInAutomation;
        public bool AllowedInAutomation
        {
            get
            {
                return allowedInAutomation;
            }

            set
            {
                allowedInAutomation = value;
                FirePropertyChanged("AllowedInAutomation");
            }
        }


        private bool manualActivatedFollowUp;
        public bool ManualActivatedFollowUp
        {
            get
            {
                return manualActivatedFollowUp;
            }

            set
            {
                manualActivatedFollowUp = value;
                FirePropertyChanged("ManualActivatedFollowUp");
            }
        }

        private bool isFollowUp;
        public bool IsFollowUp
        {
            get
            {
                return isFollowUp;
            }

            set
            {
                isFollowUp = value;
                FirePropertyChanged("IsFollowUp");
            }
        }

        private string followUpEnglishName;
        public string FollowUpEnglishName
        {
            get
            {
                return followUpEnglishName;
            }

            set
            {
                followUpEnglishName = value;
                FirePropertyChanged("FollowUpEnglishName");
            }
        }
        private string followUpLocalName;
        public string FollowUpLocalName
        {
            get
            {
                return followUpLocalName;
            }

            set
            {
                followUpLocalName = value;
                FirePropertyChanged("FollowUpLocalName");
            }
        }
        private string entityStatusCode;
        public string EntityStatusCode
        {
            get
            {
				if (string.IsNullOrEmpty(entityStatusCode))
					return null;

                return entityStatusCode;
            }

            set
            {
                entityStatusCode = value;
                FirePropertyChanged("EntityStatusCode");
            }
        }

        private bool isSharedLogisticsEnabled;
        public bool IsSharedLogisticsEnabled
        {
            get
            {
                return isSharedLogisticsEnabled;
            }

            set
            {
                isSharedLogisticsEnabled = value;
                FirePropertyChanged("IsSharedLogisticsEnabled");
            }
        }
        


    }
}
