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
    public class DataContractFieldViewModel : PropertyChangedImplementation
    {

        public DataContractFieldViewModel()
        { 
            
        }

        private string dcVersion;
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

        private string dCFieldName;
        public string DCFieldName
        {
            get
            {
                return dCFieldName;
            }
            set
            {
                dCFieldName = value;
                FirePropertyChanged("DCFieldName");
            }
        }

        private string fieldname;
        public string FieldName
        {
            get
            {
                return fieldname;
            }
            set
            {
                fieldname = value;
                FirePropertyChanged("FieldName");
            }
        }

        private bool isKey;
        public bool IsKey
        {
            get
            {
                return isKey;
            }
            set
            {
                isKey = value;
                FirePropertyChanged("IsKey");
            }
        }

        private bool isManualMapping;
        public bool IsManualMapping
        {
            get
            {
                return isManualMapping;
            }
            set
            {
                isManualMapping = value;
                FirePropertyChanged("IsManualMapping");
            }
        }
         

        private bool isSpecialField;
        public bool IsSpecialField
        {
            get
            {
                return isSpecialField;
            }
            set
            {
                isSpecialField = value;
                FirePropertyChanged("IsSpecialField");
            }
        }

        private bool isNullable;
        public bool IsNullable
        {
            get
            {
                return isNullable;
            }
            set
            {
                isNullable = value;
                FirePropertyChanged("IsNullable");
            }
        }

        private bool isMulti;
        public bool IsMulti
        {
            get
            {
                return isMulti;
            }
            set
            {
                isMulti = value;
                FirePropertyChanged("IsMulti");
            }
        }

        string fieldDataType;
        public string FieldDataType
        {
            get { return fieldDataType; }
            set
            {

                fieldDataType = value;
                FirePropertyChanged(FieldDataType);
            }
        }

        string multiTableName;
        public string MultiTableName
        {
            get { return multiTableName; }
            set
            {

                multiTableName = value;
                FirePropertyChanged("MultiTableName");
            }
        }

        private bool isCustomType;
        public bool IsCustomType
        {
            get
            {
                return isCustomType;
            }
            set
            {
                isCustomType = value;
                FirePropertyChanged("IsCustomType");
            }
        }

        private bool isAttribute;
        public bool IsAttribute
        {
            get
            {
                return isAttribute;
            }
            set
            {
                isAttribute = value;
                FirePropertyChanged("IsAttribute");
            }
        }

        private bool ignoreCustomTypeCheck;
        public bool IgnoreCustomTypeCheck
        {
            get
            {
                return ignoreCustomTypeCheck;
            }
            set
            {
                ignoreCustomTypeCheck = value;
                FirePropertyChanged("IgnoreCustomTypeCheck");
            }
        }

        private bool isCloseField;
        public bool IsCloseField
        {
            get
            {
                return isCloseField;
            }
            set
            {
                isCloseField = value;
                FirePropertyChanged("IsCloseField");
            }
        }

        private bool isCompositKey;
        public bool IsCompositKey
        {
            get
            {
                return isCompositKey;
            }
            set
            {
                isCompositKey = value;
                FirePropertyChanged("IsCompositKey");
            }
        }

        private bool isUpdateAllowed;
        public bool IsUpdateAllowed
        {
            get
            {
                return isUpdateAllowed;
            }
            set
            {
                isUpdateAllowed = value;
                FirePropertyChanged("IsUpdateAllowed");
            }
        }
        

        string closeTableCode;
        public string CloseTableCode
        {
            get { return closeTableCode; }
            set
            {

                closeTableCode = value;
                FirePropertyChanged(CloseTableCode);
            }
        }





    }
}
