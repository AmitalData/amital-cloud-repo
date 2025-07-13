using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MeatadataGeneratorTool.QueryModule
{
    public class QueryGroupViewModel : PropertyChangedImplementation
    {
        public QueryGroupViewModel()
        {

        }

        private string code = "OTQG";
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

        private string name = "Object Table Query Group";
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

        public ObservableCollection<QueryViewModel> QueriesObsList { get; set; }

        public RelayCommand AddQueryCommand
        {
            get { return new RelayCommand(() => this.AddQueryMethod()); }
        }

        private void AddQueryMethod()
        {
            ErrorMessages = "";
            if (string.IsNullOrEmpty(Code))
            {
                ErrorMessages += " Code Is Required !";
                errorsVisibility = Visibility.Visible;
            }
            else if (string.IsNullOrEmpty(Name))
            {
                ErrorMessages += " Name Is Required !";
                errorsVisibility = Visibility.Visible;
            }
            else
            {
                errorsVisibility = Visibility.Collapsed;
                //open new query window
            }
        }

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
}
