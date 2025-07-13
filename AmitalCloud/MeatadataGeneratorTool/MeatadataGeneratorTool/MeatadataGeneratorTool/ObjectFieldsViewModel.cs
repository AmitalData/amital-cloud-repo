using System.Collections.Generic;
using GalaSoft.MvvmLight.Command;
using System.Linq;
using System.Windows;
using System.Collections.ObjectModel;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using static System.Net.Mime.MediaTypeNames;

namespace MeatadataGeneratorTool
{
    public class ObjectFieldsViewModel : PropertyChangedImplementation
    {
        public List<string> DataTypesList { get { return new List<string>() { "Boolean", "Constant", "Date", "DateTime","DateTime2", "Decimal", "Double", "Integer", "LookUp", "nText", "PickList", "SigDouble", "Text", "UnsDecimal", "UnsInteger", "List","Emails", "Byte[]", "BigInteger", "Binary", "Time", "Raw" }; } }
        public List<string> DependencyFiltersList { get { return new List<string>() { "Constant", "Path", }; } }
        public List<string> TextCaseList { get { return new List<string>() { "Lower", "Upper", }; } }
        public List<string> OperatorsList { get; set; }

        public List<ObjectFieldsViewModel> ControlFieldsList { get; set; }
        ObjectTableViewModel viewModel;

        public ObjectFieldsViewModel(ObjectTableViewModel model, bool isNew)
        {
            this.viewModel = model;
            //DataTypesList = ;
            //DependencyFiltersList = ;
            //TextCaseList = ;
            ControlFieldsList = new List<ObjectFieldsViewModel>();

            FirePropertyChanged("DataTypesList");
            FirePropertyChanged("ControlFieldsList");
            FirePropertyChanged("DependencyFiltersList");
            FirePropertyChanged("TextCaseList");
            FirePropertyChanged("FieldDataType");
            if (isNew)
            {
                ButtonsVisibility = Visibility.Visible;
            }
            else
            {
                ButtonsVisibility = Visibility.Collapsed;
            }
        }

        public Visibility ButtonsVisibility { get; set; }
        public void SetControlFieldsList(ObjectFieldsViewModel field)
        {
            foreach (ObjectFieldsViewModel item in viewModel.ObsList.Where(d => d.FieldName != field.FieldName && d.FieldDataType == "LookUp"))
            {
                if (!ControlFieldsList.Where(f => f.Id == item.Id).Any())
                {
                    ControlFieldsList.Add(item);
                }
            }

            if (!string.IsNullOrEmpty(field.ControlField1))
            {
                this.ControlFieldViewMode1 = viewModel.ObsList.Where(f => f.FieldName == this.ControlField1).FirstOrDefault();
            }

            if (!string.IsNullOrEmpty(field.ControlField2))
            {
                this.ControlFieldViewMode2 = viewModel.ObsList.Where(f => f.FieldName == this.ControlField2).FirstOrDefault();
            }

            if (!string.IsNullOrEmpty(field.ControlField3))
            {
                this.ControlFieldViewMode3 = viewModel.ObsList.Where(f => f.FieldName == this.ControlField3).FirstOrDefault();
            }
        }

        void SetOperatorsList()
        {
            if (FieldDataType == "Text" || FieldDataType == "nText" || FieldDataType == "Emails")
            {
                OperatorsList = new List<string>() { "StartsWith", "Contains", "Equals", };
            }
            else if (FieldDataType == "Integer" || FieldDataType == "Decimal" || FieldDataType == "Double" || FieldDataType == "DateTime" || FieldDataType == "DateTime2" || FieldDataType == "SigDouble" || FieldDataType == "UnsDecimal" || FieldDataType == "UnsInteger")
            {
                OperatorsList = new List<string>() { "Equals", "LargerThan", "LessThan", "GreaterThanOrEqual", "LessThanOrEqual", "Between", };
            }
            else if (FieldDataType == "LookUp" || FieldDataType == "Boolean" || FieldDataType == "Byte[]")
            {
                OperatorsList = new List<string>() { "Equals", };
            }
            else
            {
                OperatorsList = new List<string>() { "StartsWith", "Equals", "LargerThan", "LessThan", "GreaterThanOrEqual", "LessThanOrEqual", "Between", };
            }
            FirePropertyChanged("OperatorsList");
        }


        public string Id { get; set; }

        string fieldName;
        [Required(ErrorMessage = "Field 'FieldName' is required.")]
        public string FieldName
        {
            get { return fieldName; }
            set { fieldName = value; IsChecked = true; FirePropertyChanged("FieldName"); this.SetPMandList(value); }
        }

		 string generatedComponentPath ;

		public string GeneratedComponentPath
		{
			get { return generatedComponentPath; }
			set { generatedComponentPath = value; FirePropertyChanged("GeneratedComponentPath"); }
		}

		string oldfieldName;
        //[Required(ErrorMessage = "Field 'FieldName' is required.")]
        public string OldFieldName
        {
            get { return oldfieldName; }
            set { oldfieldName = value; FirePropertyChanged("OldFieldName");}
        }

        string oldNames;
        public string OldNames
        {
            get { return oldNames; }
            set { oldNames = value; FirePropertyChanged("OldNames"); }
        }

        string shortName;
        public string ShortName
        {
            get { return shortName; }
            set { shortName = value; FirePropertyChanged("ShortName"); }
        }

        private void SetPMandList(string value)
        {
            PMPropertyPath = value;
            ListPropertyPath = value;
        }

        bool isPrimaryKey;
        public bool IsPrimaryKey
        {
            get { return isPrimaryKey; }
            set { isPrimaryKey = value; IsChecked = true; FirePropertyChanged("IsPrimaryKey"); }
        }

        bool oldisPrimaryKey;
        public bool OldIsPrimaryKey
        {
            get { return oldisPrimaryKey; }
            set { oldisPrimaryKey = value; FirePropertyChanged("OldIsPrimaryKey"); }
        }

        bool isNullable;
        public bool IsNullable
        {
            get { return isNullable; }
            set { isNullable = value; IsChecked = true; FirePropertyChanged("IsNullable"); }
        }

        bool oldisNullable;
        public bool OldIsNullable
        {
            get { return oldisNullable; }
            set { oldisNullable = value; FirePropertyChanged("OldIsNullable"); }
        }

        bool noObjectField;

        public bool NoObjectField
        {
            get { return noObjectField; }
            set { noObjectField = value; FirePropertyChanged("NoObjectField"); }
        }

        int? order;

        public int? Order
        {
            get { return order; }
            set { order = value; FirePropertyChanged("Order"); }
        }

        string fieldDataType;
        public string FieldDataType
        {
            get { return fieldDataType; }
            set
            {

                fieldDataType = value;
                IsChecked = true;
                if (fieldDataType == "Text" || fieldDataType == "nText")
                {
                    IsIncludeInSearchFieldsEnabled = true;
                }
                else
                {
                    IsIncludeInSearchFieldsEnabled = false;
                    IncludeInSearchField = false;

                }
                
                this.SetOperatorsList();
                FirePropertyChanged("FieldDataType");
                FirePropertyChanged("TextVisibility");
                FirePropertyChanged("PickListVisibility");
                FirePropertyChanged("DoubleVisibility");
                FirePropertyChanged("DateTimeVisibility");
                FirePropertyChanged("LookUpDataTypeEnabled");
                FirePropertyChanged("LookUpVisibility");
                FirePropertyChanged("StringVisibility");
            }
        }

        string oldfieldDataType;
        public string OldFieldDataType
        {
            get { return oldfieldDataType; }
            set { oldfieldDataType = value; FirePropertyChanged("OldFieldDataType"); }
        }

        string lookUpTableName;
        public string LookUpTableName
        {
            get { return lookUpTableName; }
            set { lookUpTableName = value; FirePropertyChanged("LookUpTableName"); }
        }

        int minLength;
        public int MinLength
        {
            get { return minLength; }
            set { minLength = value; FirePropertyChanged("MinLength"); }
        }

        int maxLength;
        public int MaxLength
        {
            get { return maxLength; }
            set { maxLength = value; IsChecked = true; FirePropertyChanged("MaxLength"); }
        }

        int systemMaxLength;
        public int SystemMaxLength
        {
            get { return systemMaxLength; }
            set { systemMaxLength = value; FirePropertyChanged("SystemMaxLength"); }
        }

        bool isMaxLength;

        public bool IsMaxLength
        {
            get { return isMaxLength; }
            set { isMaxLength = value; IsChecked = true; FirePropertyChanged("IsMaxLength"); }
        }

        bool isFixedLength;

        public bool IsFixedLength
        {
            get { return isFixedLength; }
            set { isFixedLength = value; FirePropertyChanged("IsFixedLength"); }
        }

        bool isRequired;
        public bool IsRequired
        {
            get { return isRequired; }
            set { isRequired = value; FirePropertyChanged("IsRequired"); }
        }

        bool isRestrictable;
        public bool IsRestrictable
        {
            get { return isRestrictable; }
            set { isRestrictable = value; FirePropertyChanged("IsRestrictable"); }
        }

        bool displayOnLookUp;
        public bool DisplayOnLookUp
        {
            get { return displayOnLookUp; }
            set
            {
                displayOnLookUp = value;
                FirePropertyChanged("DisplayOnLookUp");
                FirePropertyChanged("DisplayOnLookUpEnabled");
                FirePropertyChanged("DisplayOnLookUpFontWeight");
                FirePropertyChanged("DisplayOnLookUpPathVisibility");
            }
        }

        bool displayOnLookUpLocal;
        public bool DisplayOnLookUpLocal
        {
            get { return displayOnLookUpLocal; }
            set
            {
                displayOnLookUpLocal = value;
                FirePropertyChanged("DisplayOnLookUpLocal");
                FirePropertyChanged("DisplayOnLookUpLocalEnabled");
                FirePropertyChanged("DisplayOnLookUpLocalFontWeight");
                FirePropertyChanged("DisplayOnLookUpLocalPathVisibility");
            }
        }

        public bool LookUpDataTypeEnabled
        {
            get
            {
                bool result = false;
                if (FieldDataType == "LookUp")
                {
                    result = true;
                }
                return result;
            }
            set { }
        }

        public bool DisplayOnLookUpEnabled
        {
            get
            {
                bool result = false;
                if (DisplayOnLookUp)
                {
                    result = true;
                }
                return result;
            }
            set { }
        }

        public string DisplayOnLookUpFontWeight
        {
            get
            {
                string result = "Normal";
                if (DisplayOnLookUp)
                {
                    result = "Bold";
                }
                return result;
            }
            set { }
        }

        public bool DisplayOnLookUpLocalEnabled
        {
            get
            {
                bool result = false;
                if (DisplayOnLookUpLocal)
                {
                    result = true;
                }
                return result;
            }
            set { }
        }

        public string DisplayOnLookUpLocalFontWeight
        {
            get
            {
                string result = "Normal";
                if (DisplayOnLookUpLocal)
                {
                    result = "Bold";
                }
                return result;
            }
            set { }
        }

        bool canFilter;
        public bool CanFilter
        {
            get { return canFilter; }
            set { canFilter = value; FirePropertyChanged("CanFilter"); FirePropertyChanged("CanFilterFontWeight"); }
        }

        public string CanFilterFontWeight
        {
            get
            {
                string result = "Normal";
                if (CanFilter)
                {
                    result = "Bold";
                }
                return result;
            }
            set { }
        }

        bool displayOnly;
        public bool DisplayOnly
        {
            get { return displayOnly; }
            set { displayOnly = value; FirePropertyChanged("DisplayOnly"); }
        }

        bool systemRequired;
        public bool SystemRequired
        {
            get { return systemRequired; }
            set { systemRequired = value; FirePropertyChanged("SystemRequired"); }
        }

        bool objectFieldNotRequired;

        public bool ObjectFieldNotRequired
        {
            get { return objectFieldNotRequired; }
            set { objectFieldNotRequired = value; FirePropertyChanged("ObjectFieldNotRequired"); }
        }

        bool displayInList;
        public bool DisplayInList
        {
            get { return displayInList; }
            set { displayInList = value; FirePropertyChanged("DisplayInList"); FirePropertyChanged("DisplayInListFontWeight"); }
        }

        bool generateInList;

        public bool GenerateInList
        {
            get { return generateInList; }
            set { generateInList = value; FirePropertyChanged("GenerateInList"); }
        }

        public string DisplayInListFontWeight
        {
            get
            {
                string result = "Normal";
                if (DisplayInList)
                {
                    result = "Bold";
                }
                return result;
            }
            set { }
        }

        string converterName;
        public string ConverterName
        {
            get { return converterName; }
            set { converterName = value; FirePropertyChanged("ConverterName"); }
        }

        string dataTemplateName;
        public string DataTemplateName
        {
            get { return dataTemplateName; }
            set { dataTemplateName = value; FirePropertyChanged("DataTemplateName"); }
        }

        bool isCustomFilter;
        public bool IsCustomFilter
        {
            get { return isCustomFilter; }
            set { isCustomFilter = value; FirePropertyChanged("IsCustomFilter"); }
        }

        bool isListFilter;
        public bool IsListFilter
        {
            get { return isListFilter; }
            set { isListFilter = value; FirePropertyChanged("IsListFilter"); }
        }

        bool isChecked;
        public bool IsChecked
        {
            get { return isChecked; }
            set { isChecked = value; FirePropertyChanged("IsChecked"); }
        }

        bool isDeleted;
        public bool IsDeleted
        {
            get { return isDeleted; }
            set { isDeleted = value; FirePropertyChanged("IsDeleted"); }
        }

        bool isNew;
        public bool IsNew
        {
            get { return isNew; }
            set { isNew = value; FirePropertyChanged("IsNew"); }
        }

        bool isCustom;
        public bool IsCustom
        {
            get { return isCustom; }
            set { isCustom = value; FirePropertyChanged("IsCustom"); }
        }

         bool hasTemplate;
         public bool HasTemplate
         {
             get { return hasTemplate; }
             set { hasTemplate = value; FirePropertyChanged("HasTemplate"); }
         }
        

        string op;
        public string Operator
        {
            get { return op; }
            set { op = value; FirePropertyChanged("Operator"); }
        }

        bool multiLine;
        public bool MultiLine
        {
            get { return multiLine; }
            set { multiLine = value; FirePropertyChanged("MultiLine"); }
        }

        bool isTimeFrameFilter;
        public bool IsTimeFrameFilter
        {
            get { return isTimeFrameFilter; }
            set { isTimeFrameFilter = value; FirePropertyChanged("IsTimeFrameFilter"); }
        }

        bool displayInSearchWindowList;
        public bool DisplayInSearchWindowList
        {
            get { return displayInSearchWindowList; }
            set { displayInSearchWindowList = value; FirePropertyChanged("DisplayInSearchWindowList"); FirePropertyChanged("DisplayInSearchPathVisibility"); }
        }

        string pmPropertyPath;
        public string PMPropertyPath
        {
            get { return pmPropertyPath; }
            set { pmPropertyPath = value; FirePropertyChanged("PMPropertyPath"); }
        }

        string listPropertyPath;
        public string ListPropertyPath
        {
            get { return listPropertyPath; }
            set { listPropertyPath = value; FirePropertyChanged("ListPropertyPath"); }
        }

        string lookUpControlName;
        public string LookUpControlName
        {
            get { return lookUpControlName; }
            set { lookUpControlName = value; FirePropertyChanged("LookUpControlName"); }
        }

        int? displayInLookUpIndex;
        public int? DisplayInLookUpIndex
        {
            get { return displayInLookUpIndex; }
            set { displayInLookUpIndex = value; FirePropertyChanged("DisplayInLookUpIndex"); }
        }

        bool automaticField;
        public bool AutomaticField
        {
            get { return automaticField; }
            set { automaticField = value; FirePropertyChanged("AutomaticField"); }
        }

        bool uniqueField;
        public bool UniqueField
        {
            get { return uniqueField; }
            set { uniqueField = value; FirePropertyChanged("UniqueField"); }
        }

        int displayInSearchWindowListIndex;
        public int DisplayInSearchWindowListIndex
        {
            get { return displayInSearchWindowListIndex; }
            set { displayInSearchWindowListIndex = value; FirePropertyChanged("DisplayInSearchWindowListIndex"); }
        }

        bool isMulti;
        public bool IsMulti
        {
            get { return isMulti; }
            set { isMulti = value; FirePropertyChanged("IsMulti"); FirePropertyChanged("IsMultiFontWeight"); }
        }

        string multiTableName;
        public string MultiTableName
        {
            get { return multiTableName; }
            set { multiTableName = value; FirePropertyChanged("MultiTableName"); }
        }

        string associationName;

        public string AssociationName
        {
            get { return associationName; }
            set { associationName = value; FirePropertyChanged("AssociationName"); }
        }

        string thisKey;

        public string ThisKey
        {
            get { return thisKey; }
            set { thisKey = value; FirePropertyChanged("ThisKey"); }
        }

        string otherKey;

        public string OtherKey
        {
            get { return otherKey; }
            set { otherKey = value; FirePropertyChanged("OtherKey"); }
        }

        bool isComposition;

        public bool IsComposition
        {
            get { return isComposition; }
            set { isComposition = value; FirePropertyChanged("IsComposition"); }
        }

        public string IsMultiFontWeight
        {
            get
            {
                string result = "Normal";
                if (IsMulti)
                {
                    result = "Bold";
                }
                return result;
            }
            set { }
        }

        string dependencyFilter1Value;
        public string DependencyFilter1Value
        {
            get { return dependencyFilter1Value; }
            set { dependencyFilter1Value = value; FirePropertyChanged("DependencyFilter1Value"); }
        }

        string dependencyFilter2Value;
        public string DependencyFilter2Value
        {
            get { return dependencyFilter2Value; }
            set { dependencyFilter2Value = value; FirePropertyChanged("DependencyFilter2Value"); }
        }

        string dependencyFilter3Value;
        public string DependencyFilter3Value
        {
            get { return dependencyFilter3Value; }
            set { dependencyFilter3Value = value; FirePropertyChanged("DependencyFilter3Value"); }
        }

        string dependencyFilter1Type;
        public string DependencyFilter1Type
        {
            get { return dependencyFilter1Type; }
            set { dependencyFilter1Type = value; FirePropertyChanged("DependencyFilter1Type"); }
        }

        string dependencyFilter2Type;
        public string DependencyFilter2Type
        {
            get { return dependencyFilter2Type; }
            set { dependencyFilter2Type = value; FirePropertyChanged("DependencyFilter2Type"); }
        }

        string dependencyFilter3Type;
        public string DependencyFilter3Type
        {
            get { return dependencyFilter3Type; }
            set { dependencyFilter3Type = value; FirePropertyChanged("DependencyFilter3Type"); }
        }

        bool dependencyFilter1IsList;
        public bool DependencyFilter1IsList
        {
            get { return dependencyFilter1IsList; }
            set { dependencyFilter1IsList = value; FirePropertyChanged("DependencyFilter1IsList"); }
        }

        bool dependencyFilter2IsList;
        public bool DependencyFilter2IsList
        {
            get { return dependencyFilter2IsList; }
            set { dependencyFilter2IsList = value; FirePropertyChanged("DependencyFilter2IsList"); }
        }

        bool dependencyFilter3IsList;
        public bool DependencyFilter3IsList
        {
            get { return dependencyFilter3IsList; }
            set { dependencyFilter3IsList = value; FirePropertyChanged("DependencyFilter3IsList"); }
        }

        bool isIncludeInSearchFieldsEnabled = false;
        public bool IsIncludeInSearchFieldsEnabled
        {
            get { return isIncludeInSearchFieldsEnabled; }
            set { isIncludeInSearchFieldsEnabled = value; FirePropertyChanged("IsIncludeInSearchFieldsEnabled"); }
        }

        string validForQuerySection1;
        public string ValidForQuerySection1
        {
            get { return validForQuerySection1; }
            set { validForQuerySection1 = value; FirePropertyChanged("ValidForQuerySection1"); }
        }

        string validForQuerySection2;
        public string ValidForQuerySection2
        {
            get { return validForQuerySection2; }
            set { validForQuerySection2 = value; FirePropertyChanged("ValidForQuerySection2"); }
        }

        bool displayInEntityVariables;
        public bool DisplayInEntityVariables
        {
            get { return displayInEntityVariables; }
            set { displayInEntityVariables = value; FirePropertyChanged("DisplayInEntityVariables"); }
        }

        string textCase;
        public string TextCase
        {
            get { return textCase; }
            set { textCase = value; FirePropertyChanged("TextCase"); }
        }

        bool copyToDW;
        public bool CopyToDW
        {
            get { return copyToDW; }
            set { copyToDW = value; FirePropertyChanged("CopyToDW"); }
        }

    

        ObjectFieldsViewModel controlFieldViewModel1;
        public ObjectFieldsViewModel ControlFieldViewMode1
        {
            get { return controlFieldViewModel1; }
            set
            {
                ControlField1 = value != null ? value.FieldName : null;
                controlFieldViewModel1 = value; FirePropertyChanged("ControlFieldViewMode1");
            }
        }

        ObjectFieldsViewModel controlFieldViewMode2;
        public ObjectFieldsViewModel ControlFieldViewMode2
        {
            get { return controlFieldViewMode2; }
            set
            {
                controlFieldViewMode2 = value;
                ControlField2 = value != null ? value.FieldName : null;
                FirePropertyChanged("ControlFieldViewMode2");
            }
        }

        ObjectFieldsViewModel controlFieldViewMode3;
        public ObjectFieldsViewModel ControlFieldViewMode3
        {
            get { return controlFieldViewMode3; }
            set
            {
                controlFieldViewMode3 = value;
                ControlField3 = value != null ? value.FieldName : null;
                FirePropertyChanged("ControlFieldViewMode3");
            }
        }

        string controlField1;
        public string ControlField1
        {
            get { return controlField1; }
            set { controlField1 = value; FirePropertyChanged("ControlField1"); }
        }

        string controlField2;
        public string ControlField2
        {
            get { return controlField2; }
            set { controlField2 = value; FirePropertyChanged("ControlField2"); }
        }

        string controlField3;
        public string ControlField3
        {
            get { return controlField3; }
            set { controlField3 = value; FirePropertyChanged("ControlField3"); }
        }


        string code;
        public string Code
        {
            get { return code; }
            set { code = value; FirePropertyChanged("Code"); }
        }

        int? digitsAfterPoint;
        public int? DigitsAfterPoint
        {
            get { return digitsAfterPoint; }
            set { digitsAfterPoint = value; IsChecked = true; FirePropertyChanged("DigitsAfterPoint"); }
        }

        int? numberOfDigits;

        public int? NumberOfDigits
        {
            get { return numberOfDigits; }
            set { numberOfDigits = value; IsChecked = true; FirePropertyChanged("NumberOfDigits"); }
        }

        bool inActive;
        public bool InActive
        {
            get { return inActive; }
            set { inActive = value; FirePropertyChanged("InActive"); }
        }

        string columnHeaderTemplateName;
        public string ColumnHeaderTemplateName
        {
            get { return columnHeaderTemplateName; }
            set { columnHeaderTemplateName = value; FirePropertyChanged("ColumnHeaderTemplateName"); }
        }

        string displayInLookupColumnSize;
        public string DisplayInLookupColumnSize
        {
            get { return displayInLookupColumnSize; }
            set { displayInLookupColumnSize = value; FirePropertyChanged("DisplayInLookupColumnSize"); }
        }

        bool displayLongName;
        public bool DisplayLongName
        {
            get { return displayLongName; }
            set { displayLongName = value; FirePropertyChanged("DisplayLongName"); }
        }

        string customPickListCode;
        public string CustomPickListCode
        {
            get { return customPickListCode; }
            set { customPickListCode = value; FirePropertyChanged("CustomPickListCode"); }
        }

        string defaultText;
        public string DefaultText
        {
            get { return defaultText; }
            set { defaultText = value; FirePropertyChanged("DefaultText"); }
        }

        string fullLocalDefaultText;
        public string FullLocalDefaultText
        {
            get { return fullLocalDefaultText; }
            set { fullLocalDefaultText = value; FirePropertyChanged("FullLocalDefaultText"); }
        }

        string listLableDefaultText;
        public string ListLableDefaultText
        {
            get { return listLableDefaultText; }
            set { listLableDefaultText = value; FirePropertyChanged("ListLableDefaultText"); }
        }

        string listLocalDefaultText;
        public string ListLocalDefaultText
        {
            get { return listLocalDefaultText; }
            set { listLocalDefaultText = value; FirePropertyChanged("ListLocalDefaultText"); }
        }

        string helpTextDefaultText;
        public string HelpTextDefaultText
        {
            get { return helpTextDefaultText; }
            set { helpTextDefaultText = value; FirePropertyChanged("HelpTextDefaultText"); }
        }

        string helpLocalDefaultText;
        public string HelpLocalDefaultText
        {
            get { return helpLocalDefaultText; }
            set { helpLocalDefaultText = value; FirePropertyChanged("HelpLocalDefaultText"); }
        }

        string shortFieldLableDefaultText;
        public string ShortFieldLableDefaultText
        {
            get { return shortFieldLableDefaultText; }
            set { shortFieldLableDefaultText = value; FirePropertyChanged("ShortFieldLableDefaultText"); }
        }

        string shortLocalDefaultText;
        public string ShortLocalDefaultText
        {
            get { return shortLocalDefaultText; }
            set { shortLocalDefaultText = value; FirePropertyChanged("ShortLocalDefaultText"); }
        }

        bool isForeignKey;
        public bool IsForeignKey
        {
            get { return isForeignKey; }
            set { isForeignKey = value; FirePropertyChanged("IsForeignKey"); FirePropertyChanged("ForeignEntityFontWeight"); }
        }

        bool dontBuildRelationOnDB;
        public bool DontBuildRelationOnDB
        {
            get { return dontBuildRelationOnDB; }
            set { dontBuildRelationOnDB = value; FirePropertyChanged("DontBuildRelationOnDB"); FirePropertyChanged("ForeignEntityFontWeight"); }
        }

        string foreignEntity;
        public string ForeignEntity
        {
            get { return foreignEntity; }
            set { foreignEntity = value; FirePropertyChanged("ForeignEntity"); }
        }

        string navigationPropertyName;
        public string NavigationPropertyName
        {
            get { return navigationPropertyName; }
            set { navigationPropertyName = value; FirePropertyChanged("NavigationPropertyName"); }
        }

        public string ForeignEntityFontWeight
        {
            get
            {
                string result = "Normal";
                if (IsForeignKey)
                {
                    result = "Bold";
                }
                return result;
            }
            set { }
        }

        public bool IsDBFieldEnabled
        {
            get
            {
                bool result = false;
                if (IsDBField)
                {
                    result = true;
                }
                return result;
            }
            set { }
        }
        public string IsPMFontWeight
        {
            get
            {
                string result = "Normal";
                if (isPMField)
                {
                    result = "Bold";
                }
                return result;
            }
            set { }
        }
        public bool IsPMFieldEnabled
        {
            get
            {
                bool result = false;
                if (IsPMField)
                {
                    result = true;
                }
                return result;
            }
            set { }
        }

        bool isDBField;
        public bool IsDBField
        {
            get { return isDBField; }
            set { isDBField = value; FirePropertyChanged("IsDBField"); FirePropertyChanged("IsDBFieldEnabled"); }
        }

        bool isPMField;
        public bool IsPMField
        {
            get { return isPMField; }
            set { isPMField = value; FirePropertyChanged("IsPMField"); FirePropertyChanged("IsPMFieldEnabled"); FirePropertyChanged("IsPMFontWeight"); }
        }
		string tableRelatedPM;
		public string TableRelatedPM
		{
			get { return tableRelatedPM; }
			set { tableRelatedPM = value; FirePropertyChanged("TableRelatedPM"); }
		}
		string fieldRelatedPM;
		public string FieldRelatedPM
		{
			get { return fieldRelatedPM; }
			set { fieldRelatedPM = value; FirePropertyChanged("FieldRelatedPM"); }
		}
		private bool enableAutoFill;
        public bool EnableAutoFill
        {
            get 
            { 
                return enableAutoFill;
            }
            set 
            {
                enableAutoFill = value;
                FirePropertyChanged("EnableAutoFill");
            }
        }

        private bool includeInSearchField;
        public bool IncludeInSearchField
        {
            get 
            {
                return includeInSearchField;
            }
            set 
            {
                includeInSearchField = value;
                FirePropertyChanged("IncludeInSearchField");
            }
        }

        private bool allowedinAutomationConditions;
        public bool AllowedinAutomationConditions
        {
            get
            {
                return allowedinAutomationConditions;
            }
            set
            {
                allowedinAutomationConditions = value;
                FirePropertyChanged("AllowedinAutomationConditions");
            }
        }

        private bool automationEmailRecipient;
        public bool AutomationEmailRecipient
        {
            get
            {
                return automationEmailRecipient;
            }
            set
            {
                automationEmailRecipient = value;
                FirePropertyChanged("AutomationEmailRecipient");
            }
        }

        private bool canAutomateSetValue;
        public bool CanAutomateSetValue
        {
            get
            {
                return canAutomateSetValue;
            }
            set
            {
                canAutomateSetValue = value;
                FirePropertyChanged("CanAutomateSetValue");
            }
        }


        private bool displayInAutomationAsEnitity;
        public bool DisplayInAutomationAsEnitity
        {
            get
            {
                return displayInAutomationAsEnitity;
            }
            set
            {
                displayInAutomationAsEnitity = value;
                FirePropertyChanged("DisplayInAutomationAsEnitity");
            }
        }


        private string recordType;
        public string RecordType
        {
            get
            {
                return recordType;
            }
            set
            {
                recordType = value;
                FirePropertyChanged("RecordType");
            }
        }



        private string additionalQuerySections;
        public string AdditionalQuerySections
        {
            get
            {
                return additionalQuerySections;
            }
            set
            {
                additionalQuerySections = value;
                FirePropertyChanged("AdditionalQuerySections");
            }
        }

        
        private bool displayInRequiredFields;
        public bool DisplayInRequiredFields
        {
            get
            {
                return displayInRequiredFields;
            }
            set
            {
                displayInRequiredFields = value;
                FirePropertyChanged("DisplayInRequiredFields");
            }
        }


        string hTMLListComponentURL;
        public string HtmlListComponentUrl
        {
            get { return hTMLListComponentURL; }
            set { hTMLListComponentURL = value; FirePropertyChanged("HtmlListComponentUrl"); }
        }

        string hTMLListComponentName;
        public string HtmlListComponentName
        {
            get { return hTMLListComponentName; }
            set { hTMLListComponentName = value; FirePropertyChanged("HtmlListComponentName"); }
        }

        private bool enableFullscreenTextBox;
        public bool EnableFullscreenTextBox
        {
            get
            {
                return enableFullscreenTextBox;
            }
            set
            {
                enableFullscreenTextBox = value;
                FirePropertyChanged("EnableFullscreenTextBox");
            }
        }

        string helpTextCode;
        public string HelpTextCode
        {
            get { return helpTextCode; }
            set { helpTextCode = value; FirePropertyChanged("HelpTextCode"); }
        }

        private bool allowedInCustomerFieldsSettings;
        public bool AllowedInCustomerFieldsSettings
        {
            get
            {
                return allowedInCustomerFieldsSettings;
            }
            set
            {
                allowedInCustomerFieldsSettings = value;
                FirePropertyChanged("AllowedInCustomerFieldsSettings");
            }
        }


        private bool displayInSearchWindowFilters;
        public bool DisplayInSearchWindowFilters
        {
            get
            {
                return displayInSearchWindowFilters;
            }
            set
            {
                displayInSearchWindowFilters = value;
                FirePropertyChanged("DisplayInSearchWindowFilters");
            }
        }

        private int displayInSearchWindowFiltersIndex;
        public int DisplayInSearchWindowFiltersIndex
        {
            get
            {
                return displayInSearchWindowFiltersIndex;
            }
            set
            {
                displayInSearchWindowFiltersIndex = value;
                FirePropertyChanged("DisplayInSearchWindowFiltersIndex");
            }
        }

        private bool displayInDocumentReferences;
        public bool DisplayInDocumentReferences
        {
            get
            {
                return displayInDocumentReferences;
            }
            set
            {
                displayInDocumentReferences = value;
                FirePropertyChanged("DisplayInDocumentReferences");
            }
        }

        public Visibility TextVisibility
        {
            get
            {
                Visibility result = Visibility.Collapsed;
                if (FieldDataType == "Text" || FieldDataType == "nText")
                {
                    result = Visibility.Visible;
                }
                return result;
            }
            set { }
        }

        public Visibility StringVisibility
        {
            get
            {
                Visibility result = Visibility.Collapsed;
                if (FieldDataType == "Text" || FieldDataType == "nText" || FieldDataType == "LookUp" || FieldDataType == "Emails")
                {
                    result = Visibility.Visible;
                }
                return result;
            }
            set { }
        }

        public Visibility PickListVisibility
        {
            get
            {
                Visibility result = Visibility.Collapsed;
                if (FieldDataType == "PickList")
                {
                    result = Visibility.Visible;
                }
                return result;
            }
            set { }
        }

        public Visibility DoubleVisibility
        {
            get
            {
                Visibility result = Visibility.Collapsed;
                if (FieldDataType == "Decimal" || FieldDataType == "Double" || FieldDataType == "SigDouble" || FieldDataType == "UnsDecimal")
                {
                    result = Visibility.Visible;
                }
                return result;
            }
            set { }
        }

        public Visibility DateTimeVisibility
        {
            get
            {
                Visibility result = Visibility.Collapsed;
                if (FieldDataType == "Date" || FieldDataType == "DateTime" || FieldDataType == "DateTime2")
                {
                    result = Visibility.Visible;
                }
                return result;
            }
            set { }
        }

        public Visibility LookUpVisibility
        {
            get
            {
                Visibility result = Visibility.Collapsed;
                if (FieldDataType == "LookUp")
                {
                    result = Visibility.Visible;
                }
                return result;
            }
            set { }
        }

        public Visibility DisplayOnLookUpPathVisibility
        {
            get
            {
                Visibility result = Visibility.Collapsed;
                if (DisplayOnLookUp)
                {
                    result = Visibility.Visible;
                }
                return result;
            }
            set { }
        }

        public Visibility DisplayOnLookUpLocalPathVisibility
        {
            get
            {
                Visibility result = Visibility.Collapsed;
                if (DisplayOnLookUpLocal)
                {
                    result = Visibility.Visible;
                }
                return result;
            }
            set { }
        }

        public int Length
        {
            get;
            set;
        }

        public string ErrorMessages { get; set; }

        Visibility errorsVisibility = Visibility.Collapsed;
        public Visibility ErrorsVisibility
        {
            get { return errorsVisibility; }
            set { errorsVisibility = value; FirePropertyChanged("ErrorsVisibility"); }
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
            viewModel.newWindow.Close();
        }

        private void OkBtnMethod()
        {
            ErrorsVisibility = Visibility.Collapsed;
            ErrorMessages = string.Empty;
            if (FieldName.Length > 30)
            {
                ErrorMessages = "FieldName Shouldn't be more than 30 char. ..";
                FirePropertyChanged("ErrorMessages");
                ErrorsVisibility = Visibility.Visible; 
            }
            else
            {
                this.ValidateEntries();

                if (ErrorMessages == "")
                {
                    this.IsNew = true;
                    this.IsChecked = true;
                    ButtonsVisibility = Visibility.Collapsed;
                    viewModel.UpdateObsList(this);
                    viewModel.newWindow.Close();
                }
            } 
        }

        private void ValidateEntries()
        {
            StringBuilder str = new StringBuilder();
            if (string.IsNullOrEmpty(this.DefaultText))
            {
                str.AppendLine("Default Text is Required");
            }
            else if (ContainsHebrewCharacters(this.DefaultText))
            {
                str.AppendLine("Default Text cannot contain Hebrew characters");
            }
            if (string.IsNullOrEmpty(FieldName))
            {
                str.AppendLine("Field Name is Required");
            }
            if (string.IsNullOrEmpty(PMPropertyPath))
            {
                str.AppendLine("PM Property Path is Required");
            }
            if (string.IsNullOrEmpty(ListPropertyPath))
            {
                str.AppendLine("List Property Path is Required");
            }

            if (string.IsNullOrEmpty(FieldDataType))
            {
                str.AppendLine("Data Type is Required");
            }
            else
            {
                if (FieldDataType == "Text" || FieldDataType == "nText" || FieldDataType == "LookUp")
                {

                    if (string.IsNullOrEmpty(MaxLength.ToString()))
                    {
                        str.AppendLine("Max Length is Required");
                    }


                    if (MaxLength == 0)
                    {
                        str.AppendLine("Max Length is can't be zero!");
                    }

                    if (string.IsNullOrEmpty(SystemMaxLength.ToString()))
                    {
                        str.AppendLine("System Max Length is Required");
                    }

                }
                else if (FieldDataType == "PickList")
                {
                    if (string.IsNullOrEmpty(CustomPickListCode))
                    {
                        str.AppendLine("Custom Pick List Code is Required");
                    }
                }
                else if (FieldDataType == "Decimal" || FieldDataType == "Double" || FieldDataType == "SigDouble" || FieldDataType == "UnsDecimal")
                {
                    if (string.IsNullOrEmpty(DigitsAfterPoint.ToString()))
                    {
                        str.AppendLine("Digits After Point is Required");
                    }
                }
                else if (FieldDataType == "LookUp")
                {
                    if (string.IsNullOrEmpty(LookUpTableName))
                    {
                        str.AppendLine("Look Up Table Name is Required");
                    }
                }

                if (FieldDataType != "LookUp")
                {
                    LookUpTableName = null;
                }
            }

            if (DisplayInList)
            {
                if (string.IsNullOrEmpty(ListLableDefaultText))
                {
                    str.AppendLine("List Lable Default Text is Required");
                }
                if (string.IsNullOrEmpty(ValidForQuerySection1))
                {
                    str.AppendLine("Valid For Query Section 1 is Required");
                }
            }

            if (IsMulti)
            {
                if (string.IsNullOrEmpty(MultiTableName))
                {
                    str.AppendLine("Multi Table Name is Required");
                }

                if (string.IsNullOrEmpty(AssociationName))
                {
                    str.AppendLine("Association Name is Required");
                }

                if (string.IsNullOrEmpty(ThisKey))
                {
                    str.AppendLine("This Key is Required");
                }

                if (string.IsNullOrEmpty(OtherKey))
                {
                    str.AppendLine("Other Key is Required");
                }
            }

            if (IsForeignKey)
            {
                if (string.IsNullOrEmpty(ForeignEntity))
                {
                    str.AppendLine("Foreign Entity is Required");
                }
                if (string.IsNullOrEmpty(NavigationPropertyName))
                {
                    str.AppendLine("Navigation Property Name is Required");
                }
            }

            if (DisplayOnLookUp)
            {
                if (DisplayInLookUpIndex == null)
                {
                    str.AppendLine("Display In Look Up Index is Required");
                }
                //if (string.IsNullOrEmpty(DisplayInLookupColumnSize))
                //{
                //    str.AppendLine("Display In Look Up Column Size is Required");
                //}
            }

            if (CanFilter)
            {
                if (string.IsNullOrEmpty(Operator))
                {
                    str.AppendLine("Operator is Required");
                }
            }
            if(!string.IsNullOrEmpty(ListLableDefaultText) && ContainsHebrewCharacters(ListLableDefaultText))
            {
                str.AppendLine("'List Lable Default Text' cannot contain Hebrew characters");
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
        private bool ContainsHebrewCharacters(string text)
        {
            return Regex.IsMatch(text, @"[\u0590-\u05FF]");
        }
        public RelayCommand AdvanceSettingsBtnCommand
        {
            get { return new RelayCommand(() => this.AdvanceSettingsMethod()); }
        }

        public string HtmlHeaderComponentUrl { get;  set; }
        public string HtmlHeaderComponentName { get;  set; }
        public bool IsSpellCheckedFullFieldLable { get;  set; }
        public bool IsSpellCheckedHelpLocalDefaultText { get;  set; }
        public bool IsSpellCheckedShortLocalDefaultText { get;  set; }
        public bool IsSpellCheckedListLocalDefaultText { get; internal set; }

        string objectFieldDataMapping;
       
        string modelName;
        public string ModelName
        {
            get { return modelName; }
            set { modelName = value; FirePropertyChanged("ModelName"); }
        }
        public string ObjectFieldDataMapping
        {
            get { return objectFieldDataMapping; }
            set
            {
                objectFieldDataMapping = value; FirePropertyChanged("ObjectFieldDataMapping");
            }
        }

        //public Window AdvanceSettingsWindow = new Window();
        ObjectFieldsAdvanceSettings AdvanceSettingsControl;
        private void AdvanceSettingsMethod()
        {
           
                ErrorsVisibility = Visibility.Collapsed;
                
                //model.ObjectTableName = ObjectTableName;
                //model.SetControlFieldsList(model);
                AdvanceSettingsControl = new ObjectFieldsAdvanceSettings();
                AdvanceSettingsControl.DataContext = this;
             
                AdvanceSettingsControl.Width = 400;
                AdvanceSettingsControl.Height = 400;
                AdvanceSettingsControl.Title = "Advance Settings";
                AdvanceSettingsControl.Show();
            

        }
        


    }
}
