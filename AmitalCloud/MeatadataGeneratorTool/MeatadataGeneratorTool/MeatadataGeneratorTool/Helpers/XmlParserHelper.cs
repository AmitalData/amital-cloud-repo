using MeatadataGeneratorTool.CloseTablesData;
using MeatadataGeneratorTool.DataContractsModule;
using MeatadataGeneratorTool.EventTypes;
using MeatadataGeneratorTool.Features;
using MeatadataGeneratorTool.MenuButtons;
using MeatadataGeneratorTool.QueryModule;
using MeatadataGeneratorTool.ScreensModule;
using MeatadataGeneratorTool.TabsModule;
using MeatadataGeneratorTool.TextCodes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml;
using System.Text.RegularExpressions;

namespace MeatadataGeneratorTool.Helpers
{
    public class XmlParserHelper
    {

        public ObjectTableViewModel LoadObjectTableData(XmlDocument document)
        {


            XmlElement entity = document["entity"];

            ObjectTableViewModel objectTable = BuildObjectTable(entity);

            List<ObjectFieldsViewModel> fields = new List<ObjectFieldsViewModel>();
            List<QueryViewModel> queries = new List<QueryViewModel>();
            List<QueryColumnsViewModel> QueryColumns = new List<QueryColumnsViewModel>();
            List<ScreensViewModel> screens = new List<ScreensViewModel>();
            List<TabsViewModel> tabs = new List<TabsViewModel>();
            List<MenuButtonViewModel> MenuButtons = new List<MenuButtonViewModel>();
            List<Row> Data = new List<Row>();
            List<EventTypesViewModel> EventTypes = new List<EventTypesViewModel>();
            List<DataContractViewModel> DataContracts = new List<DataContractViewModel>();
            List<TextCodesViewModel> TextCodes = new List<TextCodesViewModel>();
            List<FeaturesViewModel> Features = new List<FeaturesViewModel>();
            foreach (XmlNode fieldNode in entity.ChildNodes)
            {
                if (fieldNode.Name == "field")
                {
                    fields.Add(BuildObjectField(fieldNode, objectTable));
                }
                else if (fieldNode.Name == "Query")
                {
                    queries.Add(BuildQuery(fieldNode, objectTable));
                }
                else if (fieldNode.Name == "Screen")
                {
                    screens.Add(BuildScreen(fieldNode, objectTable));
                }
                else if (fieldNode.Name == "Tab")
                {
                    tabs.Add(BuildTab(fieldNode, objectTable));
                }
                else if (fieldNode.Name == "Records")
                {
                    foreach (XmlNode fNode in fieldNode.ChildNodes)
                    {
                        Data.Add(BuildDataRow(fNode, objectTable));
                    }
                }
                else if (fieldNode.Name == "EventTypes")
                {
                    foreach (XmlNode fNode in fieldNode.ChildNodes)
                    {
                        EventTypes.Add(BuildEventType(fNode, objectTable));
                    }
                }
                else if (fieldNode.Name == "DataContracts")
                {
                    foreach (XmlNode fNode in fieldNode.ChildNodes)
                    {
                        DataContracts.Add(BuildDataContract(fNode, objectTable));
                    }
                }
                else if (fieldNode.Name == "MenuButtons")
                {
                    objectTable.MenuButtonGroupName = GetAttributeStringValue(fieldNode.Attributes["MenuButtonGroupName"]);
                    objectTable.MenuButtonGroupType = GetAttributeStringValue(fieldNode.Attributes["MenuButtonGroupType"]);

                    foreach (XmlNode fNode in fieldNode.ChildNodes)
                    {
                        MenuButtons.Add(BuildMenuButtons(fNode, objectTable));
                    }

                }
                else if (fieldNode.Name == "AdditionalTextCodes")
                {
                    foreach (XmlNode fNode in fieldNode.ChildNodes)
                    {
                        TextCodes.Add(BuildTextCode(fNode, objectTable));
                    }
                }

                else if (fieldNode.Name == "AdditionalFeatures")
                {
                    foreach (XmlNode fNode in fieldNode.ChildNodes)
                    {
                        Features.Add(BuildFeature(fNode, objectTable));
                    }
                }


            }


            objectTable.BuildObsList(fields);
            objectTable.BuildQueriesObsList(queries);
            objectTable.BuildScreensObsList(screens);
            objectTable.BuildTabsObsList(tabs);
            objectTable.BuildRowsData(Data);
            objectTable.BuildEventTypesObsList(EventTypes);
            objectTable.BuildMenuButtonsObsList(MenuButtons);
            objectTable.BuildDataContractsObsList(DataContracts);
            objectTable.BuildAdditionalTextCodesList(TextCodes);
            objectTable.BuildAdditionalFeaturesList(Features);
            return objectTable;
        }

        private Row BuildDataRow(XmlNode fieldNode, ObjectTableViewModel objectTable)
        {
            Row Record = new Row();
            objectTable.FieldsDictionary = new Dictionary<string, string>();



            foreach (XmlAttribute item in fieldNode.Attributes)
            {
                Record[item.Name] = item.Value;
                //ViewModel.rows.Add(row);
                //objectTable.FieldsDictionary.Add(item.Name, item.Value);
            }



            return Record;
        }


        public ObjectFieldsViewModel BuildObjectField(XmlNode fieldNode, ObjectTableViewModel table)
        {
            ObjectFieldsViewModel field = new ObjectFieldsViewModel(table, false);
            field.Id = GetAttributeStringValue(fieldNode.Attributes["Id"]);
            field.AutomaticField = GetAttributeBoolValue(fieldNode.Attributes["AutomaticField"]);
            field.CanFilter = GetAttributeBoolValue(fieldNode.Attributes["CanFilter"]);
            field.ColumnHeaderTemplateName = GetAttributeStringValue(fieldNode.Attributes["ColumnHeaderTemplateName"]);

            field.ControlField1 = GetAttributeStringValue(fieldNode.Attributes["ControlField1"]);
            field.ControlField2 = GetAttributeStringValue(fieldNode.Attributes["ControlField2"]);
            field.ControlField3 = GetAttributeStringValue(fieldNode.Attributes["ControlField3"]);

            field.ConverterName = GetAttributeStringValue(fieldNode.Attributes["ConverterName"]);
            field.CustomPickListCode = GetAttributeStringValue(fieldNode.Attributes["CustomPickListCode"]);
            field.DataTemplateName = GetAttributeStringValue(fieldNode.Attributes["DataTemplateName"]);
            field.DefaultText = GetAttributeStringValue(fieldNode.Attributes["DefaultText"]);

            field.DependencyFilter1Type = GetAttributeStringValue(fieldNode.Attributes["DependencyFilter1Type"]);
            field.DependencyFilter2Type = GetAttributeStringValue(fieldNode.Attributes["DependencyFilter2Type"]);
            field.DependencyFilter3Type = GetAttributeStringValue(fieldNode.Attributes["DependencyFilter3Type"]);

            field.DependencyFilter1Value = GetAttributeStringValue(fieldNode.Attributes["DependencyFilter1Value"]);
            field.DependencyFilter2Value = GetAttributeStringValue(fieldNode.Attributes["DependencyFilter2Value"]);
            field.DependencyFilter3Value = GetAttributeStringValue(fieldNode.Attributes["DependencyFilter3Value"]);

            field.DependencyFilter1IsList = GetAttributeBoolValue(fieldNode.Attributes["DependencyFilter1IsList"]);
            field.DependencyFilter2IsList = GetAttributeBoolValue(fieldNode.Attributes["DependencyFilter2IsList"]);
            field.DependencyFilter3IsList = GetAttributeBoolValue(fieldNode.Attributes["DependencyFilter3IsList"]);

            field.DigitsAfterPoint = GetAttributeNullableIntegerValue(fieldNode.Attributes["DigitsAfterPoint"]);
            field.NumberOfDigits = GetAttributeNullableIntegerValue(fieldNode.Attributes["NumberOfDigits"]);
            field.DisplayInEntityVariables = GetAttributeBoolValue(fieldNode.Attributes["DisplayInEntityVariables"]);
            field.DisplayInList = GetAttributeBoolValue(fieldNode.Attributes["DisplayInList"]);
            field.DisplayInLookupColumnSize = GetAttributeStringValue(fieldNode.Attributes["DisplayInLookupColumnSize"]);
            field.DisplayInLookUpIndex = GetAttributeIntegerValue(fieldNode.Attributes["DisplayInLookUpIndex"]);
            field.DisplayInSearchWindowList = GetAttributeBoolValue(fieldNode.Attributes["DisplayInSearchWindowList"]);
            field.DisplayInSearchWindowListIndex = GetAttributeIntegerValue(fieldNode.Attributes["DisplayInSearchWindowListIndex"]);
            field.DisplayLongName = GetAttributeBoolValue(fieldNode.Attributes["DisplayLongName"]);
            field.DisplayOnLookUp = GetAttributeBoolValue(fieldNode.Attributes["DisplayOnLookUp"]);
            field.DisplayOnLookUpLocal = GetAttributeBoolValue(fieldNode.Attributes["DisplayOnLookUpLocal"]);
            field.DisplayOnly = GetAttributeBoolValue(fieldNode.Attributes["DisplayOnly"]);
            field.FieldDataType = GetAttributeStringValue(fieldNode.Attributes["FieldsDataType"]);
            if (fieldNode.Attributes["OldFieldDataType"] != null)
            {
                field.OldFieldDataType = GetAttributeStringValue(fieldNode.Attributes["OldFieldDataType"]);
            }
            else
            {
                field.OldFieldDataType = GetAttributeStringValue(fieldNode.Attributes["FieldsDataType"]);
            }

            field.FieldName = GetAttributeStringValue(fieldNode.Attributes["FieldName"]);
            field.GeneratedComponentPath = GetAttributeStringValue(fieldNode.Attributes["GeneratedComponentPath"]);

            if (fieldNode.Attributes["OldFieldName"] != null)
            {
                field.OldFieldName = GetAttributeStringValue(fieldNode.Attributes["OldFieldName"]);
            }
            else
            {
                field.OldFieldName = GetAttributeStringValue(fieldNode.Attributes["FieldName"]);
            }


            if (fieldNode.Attributes["OldNames"] != null)
            {
                field.OldNames = GetAttributeStringValue(fieldNode.Attributes["OldNames"]);
            }
            else
            {
                field.OldNames = GetAttributeStringValue(fieldNode.Attributes["FieldName"]);
            }


            field.ShortName = GetAttributeStringValue(fieldNode.Attributes["ShortName"]);


            field.ForeignEntity = GetAttributeStringValue(fieldNode.Attributes["ForeignEntity"]);
            field.FullLocalDefaultText = GetAttributeStringValue(fieldNode.Attributes["FullLocalDefaultText"]);
            field.HelpLocalDefaultText = GetAttributeStringValue(fieldNode.Attributes["HelpLocalDefaultText"]);
            field.HelpTextDefaultText = GetAttributeStringValue(fieldNode.Attributes["HelpTextDefaultText"]);
            field.InActive = GetAttributeBoolValue(fieldNode.Attributes["InActive"]);
            field.IsCustomFilter = GetAttributeBoolValue(fieldNode.Attributes["IsCustomFilter"]);
            field.IsListFilter = GetAttributeBoolValue(fieldNode.Attributes["IsListFilter"]);
            field.IsDBField = GetAttributeBoolValue(fieldNode.Attributes["HasDataBaseField"]);
            field.IsForeignKey = GetAttributeBoolValue(fieldNode.Attributes["IsForeignKey"]);

            field.DontBuildRelationOnDB = GetAttributeBoolValue(fieldNode.Attributes["DontBuildRelationOnDB"]);

            field.IsMulti = GetAttributeBoolValue(fieldNode.Attributes["IsMulti"]);
            field.IsPMField = GetAttributeBoolValue(fieldNode.Attributes["HasPMField"]);
			field.TableRelatedPM = GetAttributeStringValue(fieldNode.Attributes["TableRelatedPM"]);
			field.FieldRelatedPM = GetAttributeStringValue(fieldNode.Attributes["FieldRelatedPM"]);
			field.IsPrimaryKey = GetAttributeBoolValue(fieldNode.Attributes["IsPrimaryKey"]);
            if (fieldNode.Attributes["OldIsPrimaryKey"] != null)
            {
                field.OldIsPrimaryKey = GetAttributeBoolValue(fieldNode.Attributes["OldIsPrimaryKey"]);
            }
            else
            {
                field.OldIsPrimaryKey = GetAttributeBoolValue(fieldNode.Attributes["IsPrimaryKey"]);
            }
            field.IsRequired = GetAttributeBoolValue(fieldNode.Attributes["IsRequired"]);
            field.IsTimeFrameFilter = GetAttributeBoolValue(fieldNode.Attributes["IsTimeFrameFilter"]);
            field.ListLableDefaultText = GetAttributeStringValue(fieldNode.Attributes["ListLableDefaultText"]);
            field.ListLocalDefaultText = GetAttributeStringValue(fieldNode.Attributes["ListLocalDefaultText"]);
            field.ListPropertyPath = GetAttributeStringValue(fieldNode.Attributes["ListPropertyPath"]);
            field.LookUpControlName = GetAttributeStringValue(fieldNode.Attributes["LookUpControlName"]);
            field.LookUpTableName = GetAttributeStringValue(fieldNode.Attributes["LookUpTableName"]);
            field.MaxLength = GetAttributeIntegerValue(fieldNode.Attributes["MaxLength"]);
            field.MinLength = GetAttributeIntegerValue(fieldNode.Attributes["MinLength"]);
            field.MultiLine = GetAttributeBoolValue(fieldNode.Attributes["MultiLine"]);
            field.MultiTableName = GetAttributeStringValue(fieldNode.Attributes["MultiTableName"]);
            field.NavigationPropertyName = GetAttributeStringValue(fieldNode.Attributes["NavigationPropertyName"]);
            field.Operator = GetAttributeStringValue(fieldNode.Attributes["Operator"]);
            field.PMPropertyPath = GetAttributeStringValue(fieldNode.Attributes["PMPropertyPath"]);
            field.ShortFieldLableDefaultText = GetAttributeStringValue(fieldNode.Attributes["ShortFieldLableDefaultText"]);
            field.ShortLocalDefaultText = GetAttributeStringValue(fieldNode.Attributes["ShortLocalDefaultText"]);
            field.SystemMaxLength = GetAttributeIntegerValue(fieldNode.Attributes["SystemMaxLength"]);
            field.SystemRequired = GetAttributeBoolValue(fieldNode.Attributes["SystemRequired"]);
            field.ObjectFieldNotRequired = GetAttributeBoolValue(fieldNode.Attributes["ObjectFieldNotRequired"]);
            field.TextCase = GetAttributeStringValue(fieldNode.Attributes["TextCase"]);
            field.UniqueField = GetAttributeBoolValue(fieldNode.Attributes["UniqueField"]);
            field.ValidForQuerySection1 = GetAttributeStringValue(fieldNode.Attributes["ValidForQuerySection1"]);
            field.ValidForQuerySection2 = GetAttributeStringValue(fieldNode.Attributes["ValidForQuerySection2"]);
            field.IsNullable = GetAttributeBoolValue(fieldNode.Attributes["IsNullable"]);
            if (fieldNode.Attributes["OldIsNullable"] != null)
            {
                field.OldIsNullable = GetAttributeBoolValue(fieldNode.Attributes["OldIsNullable"]);
            }
            else
            {
                field.OldIsNullable = GetAttributeBoolValue(fieldNode.Attributes["IsNullable"]);
            }
            field.IsComposition = GetAttributeBoolValue(fieldNode.Attributes["IsComposition"]);
            field.ThisKey = GetAttributeStringValue(fieldNode.Attributes["ThisKey"]);
            field.OtherKey = GetAttributeStringValue(fieldNode.Attributes["OtherKey"]);
            field.AssociationName = GetAttributeStringValue(fieldNode.Attributes["AssociationName"]);
            field.NoObjectField = GetAttributeBoolValue(fieldNode.Attributes["NoMetaDataField"]);

            field.IsMaxLength = GetAttributeBoolValue(fieldNode.Attributes["IsMaxLength"]);
            field.IsFixedLength = GetAttributeBoolValue(fieldNode.Attributes["IsFixedLength"]);

            field.GenerateInList = GetAttributeBoolValue(fieldNode.Attributes["GenerateInList"]);

            //if (GetAttributeIntegerValue(fieldNode.Attributes["Order"]) == 0)
            //{
            //    field.Order = null;
            //}
            //else
            field.Order = GetAttributeNullableIntegerValue(fieldNode.Attributes["Order"]);
            field.EnableAutoFill = GetAttributeBoolValue(fieldNode.Attributes["EnableAutoFill"]);
            field.IncludeInSearchField = GetAttributeBoolValue(fieldNode.Attributes["IncludeInSearchField"]);
            field.AllowedinAutomationConditions = GetAttributeBoolValue(fieldNode.Attributes["AllowedinAutomationConditions"]);
            field.AutomationEmailRecipient = GetAttributeBoolValue(fieldNode.Attributes["AutomationEmailRecipient"]);
            field.CanAutomateSetValue = GetAttributeBoolValue(fieldNode.Attributes["CanAutomateSetValue"]);
            field.DisplayInAutomationAsEnitity = GetAttributeBoolValue(fieldNode.Attributes["DisplayInAutomationAsEnitity"]);
            field.RecordType = GetAttributeStringValue(fieldNode.Attributes["RecordType"]);
            field.AdditionalQuerySections = GetAttributeStringValue(fieldNode.Attributes["AdditionalQuerySections"]);
            field.DisplayInRequiredFields = GetAttributeBoolValue(fieldNode.Attributes["DisplayInRequiredFields"]);



            if (fieldNode.Attributes["HtmlListComponentName"] != null)
            {
                // HtmlListComponentName
                //HtmlListComponentUrl
                field.HtmlListComponentName = GetAttributeStringValue(fieldNode.Attributes["HtmlListComponentName"]);
            }
            if (fieldNode.Attributes["HtmlListComponentUrl"] != null)
            {

                field.HtmlListComponentUrl = GetAttributeStringValue(fieldNode.Attributes["HtmlListComponentUrl"]);
            }

            if (fieldNode.Attributes["HtmlHeaderComponentName"] != null)
            {
                field.HtmlHeaderComponentName = GetAttributeStringValue(fieldNode.Attributes["HtmlHeaderComponentName"]);
            }
            if (fieldNode.Attributes["HtmlHeaderComponentUrl"] != null)
            {
                field.HtmlHeaderComponentUrl = GetAttributeStringValue(fieldNode.Attributes["HtmlHeaderComponentUrl"]);
            }
            if (fieldNode.Attributes["EnableFullscreenTextBox"] != null)
            {

                field.EnableFullscreenTextBox = GetAttributeBoolValue(fieldNode.Attributes["EnableFullscreenTextBox"]);
            }
            else
            {
                field.EnableFullscreenTextBox = false;
            }


            field.HelpTextCode = GetAttributeStringValue(fieldNode.Attributes["HelpTextCode"]);
            field.Code = GetAttributeStringValue(fieldNode.Attributes["Code"]);



            if (fieldNode.Attributes["DisplayInDocumentReferences"] != null)
            {
                field.DisplayInDocumentReferences = GetAttributeBoolValue(fieldNode.Attributes["DisplayInDocumentReferences"]);
            }
            else
            {
                field.DisplayInDocumentReferences = false;
            }
            if (fieldNode.Attributes["DisplayInSearchWindowFiltersIndex"] != null)
            {
                field.DisplayInSearchWindowFiltersIndex = GetAttributeIntegerValue(fieldNode.Attributes["DisplayInSearchWindowFiltersIndex"]);
            }




            if (fieldNode.Attributes["DisplayInSearchWindowFilters"] != null)
            {
                field.DisplayInSearchWindowFilters = GetAttributeBoolValue(fieldNode.Attributes["DisplayInSearchWindowFilters"]);
            }
            else
            {
                field.DisplayInSearchWindowFilters = false;
            }

            if (fieldNode.Attributes["AllowedInCustomerFieldsSettings"] != null)
            {
                field.AllowedInCustomerFieldsSettings = GetAttributeBoolValue(fieldNode.Attributes["AllowedInCustomerFieldsSettings"]);
            }
            else
            {
                field.AllowedInCustomerFieldsSettings = false;
            }

            if (fieldNode.Attributes["IsRestrictable"] != null)
            {
                field.IsRestrictable = GetAttributeBoolValue(fieldNode.Attributes["IsRestrictable"]);
            }
            else
            {
                field.IsRestrictable = false;
            }

            if (fieldNode.Attributes["IsCustom"] != null)
            {
                field.IsCustom = GetAttributeBoolValue(fieldNode.Attributes["IsCustom"]);
            }
            else
            {
                field.IsCustom = false;
            }
            if (fieldNode.Attributes["HasTemplate"] != null)
            {
                field.HasTemplate = GetAttributeBoolValue(fieldNode.Attributes["HasTemplate"]);
            }
            else
            {
                field.HasTemplate = false;
            }

            if (fieldNode.Attributes["IsSpellCheckedFullFieldLable"] != null)
            {
                field.IsSpellCheckedFullFieldLable = GetAttributeBoolValue(fieldNode.Attributes["IsSpellCheckedFullFieldLable"]);
            }
            else
            {
                field.IsSpellCheckedFullFieldLable = false;
            }

            if (fieldNode.Attributes["IsSpellCheckedHelpLocalDefaultText"] != null)
            {
                field.IsSpellCheckedHelpLocalDefaultText = GetAttributeBoolValue(fieldNode.Attributes["IsSpellCheckedHelpLocalDefaultText"]);
            }
            else
            {
                field.IsSpellCheckedHelpLocalDefaultText = false;
            }

            if (fieldNode.Attributes["IsSpellCheckedShortLocalDefaultText"] != null)
            {
                field.IsSpellCheckedShortLocalDefaultText = GetAttributeBoolValue(fieldNode.Attributes["IsSpellCheckedShortLocalDefaultText"]);
            }
            else
            {
                field.IsSpellCheckedShortLocalDefaultText = false;
            }

            if (fieldNode.Attributes["IsSpellCheckedListLocalDefaultText"] != null)
            {
                field.IsSpellCheckedListLocalDefaultText = GetAttributeBoolValue(fieldNode.Attributes["IsSpellCheckedListLocalDefaultText"]);
            }
            else
            {
                field.IsSpellCheckedListLocalDefaultText = false;
            }

            field.IsChecked = GetAttributeBoolValue(fieldNode.Attributes["IsChecked"]);
            field.IsDeleted = GetAttributeBoolValue(fieldNode.Attributes["IsDeleted"]);
            field.IsNew = GetAttributeBoolValue(fieldNode.Attributes["IsNew"]);

            if (fieldNode.Attributes["CopyToDW"] != null)
            {
                field.CopyToDW = GetAttributeBoolValue(fieldNode.Attributes["CopyToDW"]);
            }
            else
            {
                field.CopyToDW = false;
            }

            if (fieldNode.Attributes["ModelName"] != null)
            {
                field.ModelName = GetAttributeStringValue(fieldNode.Attributes["ModelName"]);
            }
            if (fieldNode.Attributes["ObjectFieldDataMapping"] != null)
            {
                field.ObjectFieldDataMapping = GetAttributeStringValue(fieldNode.Attributes["ObjectFieldDataMapping"]);
            }

            return field;

        }

        public QueryViewModel BuildQuery(XmlNode fieldNode, ObjectTableViewModel table)
        {
            QueryViewModel Query = new QueryViewModel(table, false);
            Query.Code = GetAttributeStringValue(fieldNode.Attributes["Code"]);
            Query.TextCode = GetAttributeStringValue(fieldNode.Attributes["TextCode"]);
            Query.LocalTextCode = GetAttributeStringValue(fieldNode.Attributes["LocalTextCode"]);
            Query.QueryGroupCode = GetAttributeStringValue(fieldNode.Attributes["QueryGroupCode"]);
            Query.IndexOrder = GetAttributeIntegerValue(fieldNode.Attributes["IndexOrder"]);
            Query.ObjectTableName = GetAttributeStringValue(fieldNode.Attributes["ObjectTableName"]);
            Query.QuerySection = GetAttributeStringValue(fieldNode.Attributes["QuerySection"]);
            Query.SystemLevel = GetAttributeBoolValue(fieldNode.Attributes["SystemLevel"]);
            Query.IsAddNewEntity = GetAttributeBoolValue(fieldNode.Attributes["IsAddNewEntity"]);
            Query.DefaultSortName = GetAttributeStringValue(fieldNode.Attributes["DefaultSortName"]);
            Query.DefaultSortDirection = GetAttributeStringValue(fieldNode.Attributes["DefaultSortDirection"]);
            Query.IsPackagable = GetAttributeBoolValue(fieldNode.Attributes["IsPackagable"]);
            Query.SpotlightDataTemplate = GetAttributeStringValue(fieldNode.Attributes["SpotlightDataTemplate"]);
            Query.EditWizardName = GetAttributeStringValue(fieldNode.Attributes["EditWizardName"]);

            if (fieldNode.Attributes["EditWizardComponentPath"] != null)
            {
                Query.EditWizardComponentPath = GetAttributeStringValue(fieldNode.Attributes["EditWizardComponentPath"]);
            }

            if (fieldNode.Attributes["QueryGroupName"] != null)
            {
                Query.QueryGroupName = GetAttributeStringValue(fieldNode.Attributes["QueryGroupName"]);
            }
            if (fieldNode.Attributes["TextCodeCode"] != null)
            {
                Query.TextCodeCode = GetAttributeStringValue(fieldNode.Attributes["TextCodeCode"]);
            }
            if (fieldNode.Attributes["FeatureCode"] != null)
            {
                Query.FeatureCode = GetAttributeStringValue(fieldNode.Attributes["FeatureCode"]);
            }
            if (fieldNode.Attributes["FeatureTextCodeCode"] != null)
            {
                Query.FeatureTextCodeCode = GetAttributeStringValue(fieldNode.Attributes["FeatureTextCodeCode"]);
            }
            if (fieldNode.Attributes["FeatureDefaultText"] != null)
            {
                Query.FeatureDefaultText = GetAttributeStringValue(fieldNode.Attributes["FeatureDefaultText"]);
            }

            if (fieldNode.Attributes["IsSpellChecked"] != null)
            {
                Query.IsSpellChecked = GetAttributeBoolValue(fieldNode.Attributes["IsSpellChecked"]);
            }

            if (fieldNode.Attributes["Perspective"] != null)
            {
                Query.Perspective = GetAttributeStringValue(fieldNode.Attributes["Perspective"]);
            }



            foreach (XmlNode fNode in fieldNode.ChildNodes)
            {
                if (fNode.Name == "QueryColumns")
                {
                    foreach (XmlNode item in fNode.ChildNodes)
                    {
                        Query.QueryColumnObsList.Add(BuildQueryColumns(item, table, Query));
                    }
                }
                else if (fNode.Name == "QueryFilters")
                {
                    foreach (XmlNode item in fNode.ChildNodes)
                    {
                        Query.QueryFiltersObsList.Add(BuildQueryFilters(item, table, Query));
                    }
                }
            }
            return Query;
        }

        public QueryColumnsViewModel BuildQueryColumns(XmlNode fieldNode, ObjectTableViewModel table, QueryViewModel QViewModel)
        {
            QueryColumnsViewModel QueryColumn = new QueryColumnsViewModel(table, QViewModel, false);
            QueryColumn.QueryCode = GetAttributeStringValue(fieldNode.Attributes["QueryCode"]);
            QueryColumn.IndexOrder = GetAttributeIntegerValue(fieldNode.Attributes["IndexOrder"]);
            QueryColumn.ObjectFieldName = GetAttributeStringValue(fieldNode.Attributes["ObjectFieldName"]);
            QueryColumn.ColumnWidth = GetAttributeIntegerValue(fieldNode.Attributes["ColumnWidth"]);
            return QueryColumn;
        }

        public QueryFiltersViewModel BuildQueryFilters(XmlNode fieldNode, ObjectTableViewModel table, QueryViewModel QViewModel)
        {
            QueryFiltersViewModel QueryFilter = new QueryFiltersViewModel(table, QViewModel, false);
            QueryFilter.QueryCode = GetAttributeStringValue(fieldNode.Attributes["QueryCode"]);
            QueryFilter.PredefinedValue = GetAttributeStringValue(fieldNode.Attributes["PredefinedValue"]);
            QueryFilter.ObjectFieldName = GetAttributeStringValue(fieldNode.Attributes["ObjectFieldName"]);
            QueryFilter.IsPredefined = GetAttributeBoolValue(fieldNode.Attributes["IsPredefined"]);
            if (fieldNode.Attributes["PredefinedValue2"] != null)
            {
                QueryFilter.PredefinedValue2 = GetAttributeStringValue(fieldNode.Attributes["PredefinedValue2"]);
            }
            if (fieldNode.Attributes["Operator"] != null)
            {
                QueryFilter.Operator = GetAttributeStringValue(fieldNode.Attributes["Operator"]);
            }
            if (fieldNode.Attributes["IndexOrder"] != null)
            {
                QueryFilter.IndexOrder = GetAttributeIntegerValue(fieldNode.Attributes["IndexOrder"]);
            }
            QueryFilter.CustomPredefined = GetAttributeBoolValue(fieldNode.Attributes["CustomPredefined"]);

            return QueryFilter;
        }

        public ScreensViewModel BuildScreen(XmlNode fieldNode, ObjectTableViewModel table)
        {
            ScreensViewModel Screen = new ScreensViewModel(table, false);
            Screen.Name = GetAttributeStringValue(fieldNode.Attributes["Name"]);
            Screen.ObjectTableName = GetAttributeStringValue(fieldNode.Attributes["ObjectTableName"]);
            Screen.IsReadOnly = GetAttributeBoolValue(fieldNode.Attributes["IsReadOnly"]);
            Screen.IsHeaderScreen = GetAttributeBoolValue(fieldNode.Attributes["IsHeaderScreen"]);
            Screen.Code = GetAttributeStringValue(fieldNode.Attributes["Code"]);

            if (fieldNode.Attributes["Code"] != null)
            {
                Screen.Code = GetAttributeStringValue(fieldNode.Attributes["Code"]);
            }
            foreach (XmlNode fNode in fieldNode.ChildNodes)
            {
                if (fNode.Name == "ScreenFields")
                {
                    if (Screen.ScreenFieldCol1ObsList == null)
                    {
                        Screen.ScreenFieldCol1ObsList = new System.Collections.ObjectModel.ObservableCollection<ScreenFieldViewModel>();
                    }
                    if (Screen.ScreenFieldCol2ObsList == null)
                    {
                        Screen.ScreenFieldCol2ObsList = new System.Collections.ObjectModel.ObservableCollection<ScreenFieldViewModel>();
                    }
                    if (Screen.ScreenFieldCol3ObsList == null)
                    {
                        Screen.ScreenFieldCol3ObsList = new System.Collections.ObjectModel.ObservableCollection<ScreenFieldViewModel>();
                    }
                    if (Screen.ScreenFieldCol4ObsList == null)
                    {
                        Screen.ScreenFieldCol4ObsList = new System.Collections.ObjectModel.ObservableCollection<ScreenFieldViewModel>();
                    }
                    if (Screen.ScreenFieldCol5ObsList == null)
                    {
                        Screen.ScreenFieldCol5ObsList = new System.Collections.ObjectModel.ObservableCollection<ScreenFieldViewModel>();
                    }
                    try
                    {
                        foreach (XmlNode item in fNode.ChildNodes)
                        {
                            var temp = BuildScreenFields(item, table, Screen);

                            if (temp.Column == 0)
                            {
                                AddScreenFieldToIndex(Screen.ScreenFieldCol1ObsList, temp);
                                //Screen.ScreenFieldCol1ObsList.Insert(temp.Row, temp);
                                Screen.Column1Visibility = Visibility.Visible;
                            }
                            else if (temp.Column == 1)
                            {
                                AddScreenFieldToIndex(Screen.ScreenFieldCol2ObsList, temp);
                                //Screen.ScreenFieldCol2ObsList.Insert(temp.Row, temp);
                                Screen.Column2Visibility = Visibility.Visible;
                            }
                            else if (temp.Column == 2)
                            {
                                AddScreenFieldToIndex(Screen.ScreenFieldCol3ObsList, temp);
                                //Screen.ScreenFieldCol3ObsList.Insert(temp.Row, temp);
                                Screen.Column3Visibility = Visibility.Visible;
                            }
                            else if (temp.Column == 3)
                            {
                                AddScreenFieldToIndex(Screen.ScreenFieldCol4ObsList, temp);
                                //Screen.ScreenFieldCol4ObsList.Insert(temp.Row, temp);
                                Screen.Column4Visibility = Visibility.Visible;
                            }
                            else if (temp.Column == 4)
                            {
                                AddScreenFieldToIndex(Screen.ScreenFieldCol5ObsList, temp);
                                //Screen.ScreenFieldCol5ObsList.Insert(temp.Row, temp);
                                Screen.Column5Visibility = Visibility.Visible;
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        throw new Exception(Screen.Name + " " + e.Message + Environment.NewLine + e.StackTrace);
                    }
                }
            }
            return Screen;
        }

        void AddScreenFieldToIndex(System.Collections.ObjectModel.ObservableCollection<ScreenFieldViewModel> ScreenFieldColObsList, ScreenFieldViewModel field)
        {
            int index = field.Row;
            try
            {
                ScreenFieldColObsList.Insert(index, field);
            }
            catch (Exception ex)
            {
                index = index - 1;
                ScreenFieldColObsList.Insert(index, field);
            }
        }

        public ScreenFieldViewModel BuildScreenFields(XmlNode fieldNode, ObjectTableViewModel table, ScreensViewModel SViewModel)
        {
            ScreenFieldViewModel ScreenField = new ScreenFieldViewModel(table, SViewModel, false);
            ScreenField.Column = GetAttributeIntegerValue(fieldNode.Attributes["Column"]);
            ScreenField.Row = GetAttributeIntegerValue(fieldNode.Attributes["Row"]);
            ScreenField.ObjectFieldName = GetAttributeStringValue(fieldNode.Attributes["ObjectFieldName"]);
            ScreenField.ScreenName = GetAttributeStringValue(fieldNode.Attributes["ScreenName"]);
            return ScreenField;
        }

        public TabsViewModel BuildTab(XmlNode fieldNode, ObjectTableViewModel table)
        {
            TabsViewModel tab = new TabsViewModel(table, false);
            tab.Name = GetAttributeStringValue(fieldNode.Attributes["TabName"]);
            tab.Code = GetAttributeStringValue(fieldNode.Attributes["Code"]);
            tab.ObjectTableName = GetAttributeStringValue(fieldNode.Attributes["ObjectTableName"]);
            tab.ControlPath = GetAttributeStringValue(fieldNode.Attributes["ControlPath"]);
            tab.TextCode = GetAttributeStringValue(fieldNode.Attributes["TextCode"]);
            tab.LocalName = GetAttributeStringValue(fieldNode.Attributes["TabLocalName"]);
            tab.IsPackagable = GetAttributeBoolValue(fieldNode.Attributes["IsPackagable"]);
            tab.IndexOrder = GetAttributeIntegerValue(fieldNode.Attributes["IndexOrder"]);
            tab.HtmlComponentName = GetAttributeStringValue(fieldNode.Attributes["HtmlComponentName"]);
            tab.HtmlComponentURL = GetAttributeStringValue(fieldNode.Attributes["HtmlComponentURL"]);
			tab.IsLocked = GetAttributeBoolValue(fieldNode.Attributes["IsLocked"]);

			if (fieldNode.Attributes["FeatureCode"] != null)
            {
                tab.FeatureCode = GetAttributeStringValue(fieldNode.Attributes["FeatureCode"]);
            }
            if (fieldNode.Attributes["FeatureTextCodeCode"] != null)
            {
                tab.FeatureTextCodeCode = GetAttributeStringValue(fieldNode.Attributes["FeatureTextCodeCode"]);
                tab.HasGeneralFeature = tab.FeatureTextCodeCode.Split('.')[0] == "General" ? true : false;
            }
            if (fieldNode.Attributes["FeatureDefaultText"] != null)
            {
                tab.FeatureDefaultText = GetAttributeStringValue(fieldNode.Attributes["FeatureDefaultText"]);
            }

            if (fieldNode.Attributes["IsPackagable"] != null)
            {
                tab.IsPackagable = GetAttributeBoolValue(fieldNode.Attributes["IsPackagable"]);
            }

            if (fieldNode.Attributes["IsSpellChecked"] != null)
            {
                tab.IsSpellChecked = GetAttributeBoolValue(fieldNode.Attributes["IsSpellChecked"]);
            }

            return tab;
        }

        public MenuButtonViewModel BuildMenuButtons(XmlNode fieldNode, ObjectTableViewModel table)
        {
            MenuButtonViewModel MenuButton = new MenuButtonViewModel(table, false, "Button");
            MenuButton.EventCode = GetAttributeStringValue(fieldNode.Attributes["EventCode"]);
            MenuButton.DefaultText = GetAttributeStringValue(fieldNode.Attributes["DefaultText"]);
            MenuButton.SelectedMenuButtonType = GetAttributeStringValue(fieldNode.Attributes["MenuButtonType"]);
            MenuButton.Style = GetAttributeStringValue(fieldNode.Attributes["Style"]);
            MenuButton.IndexOrder = GetAttributeIntegerValue(fieldNode.Attributes["IndexOrder"]);

            if (fieldNode.Attributes["LocalDefaultText"] != null)
            {
                MenuButton.LocalDefaultText = GetAttributeStringValue(fieldNode.Attributes["LocalDefaultText"]);
            }

            if (fieldNode.Attributes["TextCodeCode"] != null)
            {
                MenuButton.TextCodeCode = GetAttributeStringValue(fieldNode.Attributes["TextCodeCode"]);
            }
            if (fieldNode.Attributes["FeatureTextCodeCode"] != null)
            {
                MenuButton.FeatureTextCodeCode = GetAttributeStringValue(fieldNode.Attributes["FeatureTextCodeCode"]);
            }
            if (fieldNode.Attributes["FeatureDefaultText"] != null)
            {
                MenuButton.FeatureDefaultText = GetAttributeStringValue(fieldNode.Attributes["FeatureDefaultText"]);
            }
            if (fieldNode.Attributes["FeatureCode"] != null)
            {
                MenuButton.FeatureCode = GetAttributeStringValue(fieldNode.Attributes["FeatureCode"]);
            }
            if (fieldNode.Attributes["IsPackagable"] != null)
            {
                MenuButton.IsPackagable = GetAttributeBoolValue(fieldNode.Attributes["IsPackagable"]);
            }
            if (fieldNode.Attributes["HtmlComponentPath"] != null)
            {
                MenuButton.HtmlComponentPath = GetAttributeStringValue(fieldNode.Attributes["HtmlComponentPath"]);
            }
            if (fieldNode.Attributes["Width"] != null)
            {
                MenuButton.Width = GetAttributeIntegerValue(fieldNode.Attributes["Width"]);
            }
            if (fieldNode.ChildNodes != null)
            {
                foreach (XmlNode item in fieldNode.ChildNodes)
                {
                    MenuButtonViewModel MenuItem = new MenuButtonViewModel(table, false, "Item");
                    MenuItem.EventCode = GetAttributeStringValue(item.Attributes["EventCode"]);
                    MenuItem.DefaultText = GetAttributeStringValue(item.Attributes["DefaultText"]);
                    MenuItem.SelectedMenuButtonType = GetAttributeStringValue(item.Attributes["MenuButtonType"]);
                    MenuItem.Style = GetAttributeStringValue(item.Attributes["Style"]);
                    MenuItem.IndexOrder = GetAttributeIntegerValue(item.Attributes["IndexOrder"]);
                    if (item.Attributes["LocalDefaultText"] != null)
                    {
                        MenuItem.LocalDefaultText = GetAttributeStringValue(item.Attributes["LocalDefaultText"]);
                    }
                    if (item.Attributes["TextCodeCode"] != null)
                    {
                        MenuItem.TextCodeCode = GetAttributeStringValue(item.Attributes["TextCodeCode"]);
                    }
                    if (item.Attributes["FeatureTextCodeCode"] != null)
                    {
                        MenuItem.FeatureTextCodeCode = GetAttributeStringValue(item.Attributes["FeatureTextCodeCode"]);
                    }
                    if (item.Attributes["FeatureDefaultText"] != null)
                    {
                        MenuItem.FeatureDefaultText = GetAttributeStringValue(item.Attributes["FeatureDefaultText"]);
                    }
                    if (item.Attributes["FeatureCode"] != null)
                    {
                        MenuItem.FeatureCode = GetAttributeStringValue(item.Attributes["FeatureCode"]);
                    }
                    if (item.Attributes["IsPackagable"] != null)
                    {
                        MenuItem.IsPackagable = GetAttributeBoolValue(item.Attributes["IsPackagable"]);
                    }
                    if (item.Attributes["HtmlComponentPath"] != null)
                    {
                        MenuItem.HtmlComponentPath = GetAttributeStringValue(item.Attributes["HtmlComponentPath"]);
                    }
                    if (item.Attributes["Width"] != null)
                    {
                        MenuItem.Width = GetAttributeIntegerValue(item.Attributes["Width"]);
                    }
                    if (MenuButton.MenuButtonItems == null)
                    {
                        MenuButton.MenuButtonItems = new ObservableCollection<MenuButtonViewModel>();
                    }
                    MenuButton.MenuButtonItems.Add(MenuItem);
                }
            }
            return MenuButton;
        }

        public EventTypesViewModel BuildEventType(XmlNode fieldNode, ObjectTableViewModel table)
        {
            EventTypesViewModel EventType = new EventTypesViewModel(table, false);
            EventType.Code = GetAttributeStringValue(fieldNode.Attributes["Code"]);
            EventType.EnglishName = GetAttributeStringValue(fieldNode.Attributes["EnglishName"]);
            EventType.ObjectTableName = GetAttributeStringValue(fieldNode.Attributes["ObjectTableName"]);
            EventType.LocalName = GetAttributeStringValue(fieldNode.Attributes["LocalName"]);
            EventType.IsManualEntry = GetAttributeBoolValue(fieldNode.Attributes["IsManualEntry"]);
            EventType.ShortView = GetAttributeBoolValue(fieldNode.Attributes["ShortView"]);

            EventType.EventTypeCategoryCode = GetAttributeStringValue(fieldNode.Attributes["EventTypeCategoryCode"]);
            EventType.IsAgentView = GetAttributeBoolValue(fieldNode.Attributes["IsAgentView"]);
            EventType.IsCustomerView = GetAttributeBoolValue(fieldNode.Attributes["IsCustomerView"]);
            EventType.IsSharedLogisticsEnabled = GetAttributeBoolValue(fieldNode.Attributes["IsSharedLogisticsEnabled"]);
            EventType.AllowedInAutomation = GetAttributeBoolValue(fieldNode.Attributes["AllowedInAutomation"]);
            EventType.ManualActivatedFollowUp = GetAttributeBoolValue(fieldNode.Attributes["ManualActivatedFollowUp"]);
            EventType.IsFollowUp = GetAttributeBoolValue(fieldNode.Attributes["IsFollowUp"]);
            EventType.FollowUpEnglishName = GetAttributeStringValue(fieldNode.Attributes["FollowUpEnglishName"]);
            EventType.FollowUpLocalName = GetAttributeStringValue(fieldNode.Attributes["FollowUpLocalName"]);
            EventType.EntityStatusCode = GetAttributeStringValue(fieldNode.Attributes["EntityStatusCode"]);

            return EventType;
        }

        public TextCodesViewModel BuildTextCode(XmlNode fieldNode, ObjectTableViewModel table)
        {
            TextCodesViewModel textCode = new TextCodesViewModel(table, false);
            textCode.Code = GetAttributeStringValue(fieldNode.Attributes["Code"]);
            textCode.DefaultText = GetAttributeStringValue(fieldNode.Attributes["DefaultText"]);
            textCode.LocalDefaultText = GetAttributeStringValue(fieldNode.Attributes["LocalDefaultText"]);
            textCode.TextCodeTypeCode = GetAttributeStringValue(fieldNode.Attributes["TextCodeTypeCode"]);
            textCode.IsSpellChecked = GetAttributeBoolValue(fieldNode.Attributes["IsSpellChecked"]);



            return textCode;
        }

        public FeaturesViewModel BuildFeature(XmlNode fieldNode, ObjectTableViewModel table)
        {
            FeaturesViewModel feature = new FeaturesViewModel(table, false);
            feature.Code = GetAttributeStringValue(fieldNode.Attributes["Code"]);
            feature.FeatureTypeCode = GetAttributeStringValue(fieldNode.Attributes["FeatureTypeCode"]);
            feature.FeatureTextCodeCode = GetAttributeStringValue(fieldNode.Attributes["FeatureTextCodeCode"]);
            feature.FeatureDefaultText = GetAttributeStringValue(fieldNode.Attributes["FeatureDefaultText"]);
            feature.IsPackagable = GetAttributeBoolValue(fieldNode.Attributes["IsPackagable"]);
            feature.IsOld = GetAttributeBoolValue(fieldNode.Attributes["IsOld"]);
            feature.IsCoreFeature = GetAttributeBoolValue(fieldNode.Attributes["IsCoreFeature"]);
            feature.IsBusinessUnitEnabled = GetAttributeBoolValue(fieldNode.Attributes["IsBusinessUnitEnabled"]);
            feature.ToggleCode = GetAttributeStringValue(fieldNode.Attributes["ToggleCode"]);

            return feature;
        }

        public DataContractViewModel BuildDataContract(XmlNode fieldNode, ObjectTableViewModel table)
        {
            DataContractViewModel DataContract = new DataContractViewModel(table, false);
            DataContract.DCName = GetAttributeStringValue(fieldNode.Attributes["Name"]);
            DataContract.DCVersion = GetAttributeStringValue(fieldNode.Attributes["Version"]);
            DataContract.IncludeTenant0Data = GetAttributeBoolValue(fieldNode.Attributes["IncludeTenant0Data"]);
            DataContract.ComputingPartnerName = GetAttributeStringValue(fieldNode.Attributes["ComputingPartnerName"]);
            foreach (XmlNode fNode in fieldNode.ChildNodes)
            {
                if (fNode.Name == "DCField")
                {
                    if (DataContract.DCFieldsObsList == null)
                    {
                        DataContract.DCFieldsObsList = new ObservableCollection<DataContractFieldViewModel>();
                    }
                    //foreach (XmlNode item in fNode.ChildNodes)
                    //{
                    DataContract.DCFieldsObsList.Add(BuildDCField(fNode));
                    //}
                }
            }
            return DataContract;
        }

        public DataContractFieldViewModel BuildDCField(XmlNode fieldNode)
        {
            DataContractFieldViewModel DCField = new DataContractFieldViewModel();
            DCField.FieldName = GetAttributeStringValue(fieldNode.Attributes["FieldName"]);
            DCField.IsKey = GetAttributeBoolValue(fieldNode.Attributes["IsKey"]);
            DCField.IsManualMapping = GetAttributeBoolValue(fieldNode.Attributes["IsManualMapping"]);
            DCField.IsMulti = GetAttributeBoolValue(fieldNode.Attributes["IsMulti"]);
            DCField.MultiTableName = GetAttributeStringValue(fieldNode.Attributes["MultiTableName"]);
            DCField.DCFieldName = GetAttributeStringValue(fieldNode.Attributes["DCFieldName"]);
            DCField.FieldDataType = GetAttributeStringValue(fieldNode.Attributes["FieldsDataType"]);
            DCField.DCVersion = GetAttributeStringValue(fieldNode.Attributes["DCVersion"]);
            DCField.IsNullable = GetAttributeBoolValue(fieldNode.Attributes["IsNullable"]);
            DCField.IsSpecialField = GetAttributeBoolValue(fieldNode.Attributes["IsSpecialField"]);
            DCField.IsCustomType = GetAttributeBoolValue(fieldNode.Attributes["IsCustomType"]);
            DCField.IsAttribute = GetAttributeBoolValue(fieldNode.Attributes["IsAttribute"]);
            DCField.IgnoreCustomTypeCheck = GetAttributeBoolValue(fieldNode.Attributes["IgnoreCustomTypeCheck"]);
            try
            {
                DCField.IsCloseField = GetAttributeBoolValue(fieldNode.Attributes["IsCloseField"]);

            }
            catch (Exception)
            {
                DCField.IsCloseField = false;

            }
            try
            {
                DCField.IsCompositKey = GetAttributeBoolValue(fieldNode.Attributes["IsCompositKey"]);

            }
            catch (Exception)
            {
                DCField.IsCompositKey = false;

            }
            try
            {
                DCField.IsUpdateAllowed = GetAttributeBoolValue(fieldNode.Attributes["IsUpdateAllowed"]);

            }
            catch (Exception)
            {
                DCField.IsUpdateAllowed = false;

            }
            try
            {
                DCField.CloseTableCode = GetAttributeStringValue(fieldNode.Attributes["CloseTableCode"]);

            }
            catch (Exception)
            {
                //DCField.CloseTableCode = false;

            }


            return DCField;
        }

        public ObjectTableViewModel BuildObjectTable(XmlElement entity)
        {
            ObjectTableViewModel objectTable = new ObjectTableViewModel();
            if (entity != null)
            {
                objectTable.Id = GetAttributeStringValue(entity.Attributes["Id"]);
                objectTable.ObjectTableName = GetAttributeStringValue(entity.Attributes["ObjectTableName"]);
                objectTable.ParentObjectTableName = GetAttributeStringValue(entity.Attributes["ParentObjectTableName"]);
                objectTable.DBTableName = GetAttributeStringValue(entity.Attributes["DBTableName"]);

                objectTable.OldDBTableName = GetAttributeStringValue(entity.Attributes["OldDBTableName"]);

                objectTable.DBTableShortName = GetAttributeStringValue(entity.Attributes["DBTableShortName"]);

                if (entity.Attributes["DBTableOldNames"] != null)
                {
                    objectTable.DBTableOldNames = GetAttributeStringValue(entity.Attributes["DBTableOldNames"]);
                }
                else
                {
                    objectTable.DBTableOldNames = GetAttributeStringValue(entity.Attributes["DBTableName"]);
                }

                objectTable.DependencyFilter1 = GetAttributeStringValue(entity.Attributes["DependencyFilter1"]);
                objectTable.DependencyFilter2 = GetAttributeStringValue(entity.Attributes["DependencyFilter2"]);
                objectTable.DependencyFilter3 = GetAttributeStringValue(entity.Attributes["DependencyFilter3"]);
                objectTable.DescriptionDefaultText = GetAttributeStringValue(entity.Attributes["DescriptionDefaultText"]);
                objectTable.DescriptionLocalDefaultText = GetAttributeStringValue(entity.Attributes["DescriptionLocalDefaultText"]);
                objectTable.EditableFromAutoCompleteWindow = GetAttributeBoolValue(entity.Attributes["EditableFromAutoCompleteWindow"]);
                objectTable.EnableAddFromLOV = GetAttributeBoolValue(entity.Attributes["EnableAddFromLOV"]);
                objectTable.EnableEditFromLOV = GetAttributeBoolValue(entity.Attributes["EnableEditFromLOV"]);
                objectTable.EnableSecurity = GetAttributeBoolValue(entity.Attributes["EnableSecurity"]);
                objectTable.HasCounter = GetAttributeBoolValue(entity.Attributes["HasCounter"]);
                objectTable.HasDynamicHeader = GetAttributeBoolValue(entity.Attributes["HasDynamicHeader"]);
                objectTable.InActive = GetAttributeBoolValue(entity.Attributes["InActive"]);
                objectTable.IsAutoComplete = GetAttributeBoolValue(entity.Attributes["IsAutoComplete"]);
                objectTable.IsClosed = GetAttributeBoolValue(entity.Attributes["IsClosed"]);
                objectTable.IsNew = GetAttributeBoolValue(entity.Attributes["IsNew"]);
                objectTable.IsDirty = GetAttributeBoolValue(entity.Attributes["IsDirty"]);
                if (string.IsNullOrEmpty(objectTable.Id))
                {
                    if (objectTable.IsNew == false)
                    {
                        objectTable.IsNew = true;
                    }
                }
                //else
                //{
                //    objectTable.IsNew = false;
                //}

                objectTable.IsComposition = GetAttributeBoolValue(entity.Attributes["IsComposition"]);
                objectTable.IsMain = GetAttributeBoolValue(entity.Attributes["IsMain"]);
                objectTable.IsNewWizard = GetAttributeBoolValue(entity.Attributes["IsNewWizard"]);
                objectTable.IsEditable = GetAttributeBoolValue(entity.Attributes["IsEditable"]);
                objectTable.HasCustomFilter = GetAttributeBoolValue(entity.Attributes["HasCustomFilter"]);
                objectTable.HasCustomFields = GetAttributeBoolValue(entity.Attributes["HasCustomFields"]);
                objectTable.AvailableInCustomization = GetAttributeBoolValue(entity.Attributes["AvailableInCustomization"]);
                objectTable.SupportSubEntity = GetAttributeBoolValue(entity.Attributes["SupportSubEntity"]);
                objectTable.ApplyGenericCustomFields = GetAttributeBoolValue(entity.Attributes["ApplyGenericCustomFields"]);
                objectTable.AvailableInDocumentTypes = GetAttributeBoolValue(entity.Attributes["AvailableInDocumentTypes"]);
                objectTable.HasCustomValidator = GetAttributeBoolValue(entity.Attributes["HasCustomValidator"]);
                objectTable.HasHelper = GetAttributeBoolValue(entity.Attributes["HasHelper"]);
                objectTable.HasShortTitle = GetAttributeBoolValue(entity.Attributes["HasShortTitle"]);
                objectTable.HasFiltersMenu = GetAttributeBoolValue(entity.Attributes["HasFiltersMenu"]);
                objectTable.IsRestrictable = GetAttributeBoolValue(entity.Attributes["IsRestrictable"]);
                objectTable.IsSaveButtonVisible = GetAttributeBoolValue(entity.Attributes["IsSaveButtonVisible"]);
                objectTable.KeyPropertyPath = GetAttributeStringValue(entity.Attributes["KeyPropertyPath"]);
                objectTable.LookUp1 = GetAttributeStringValue(entity.Attributes["LookUp1"]);
                objectTable.LookUp2 = GetAttributeStringValue(entity.Attributes["LookUp2"]);
                objectTable.CodeField = GetAttributeStringValue(entity.Attributes["CodeField"]);
                objectTable.NameField = GetAttributeStringValue(entity.Attributes["NameField"]);
                objectTable.MainTipCode = GetAttributeStringValue(entity.Attributes["MainTipCode"]);
                objectTable.MaxNumberOfCustomFields = GetAttributeIntegerValue(entity.Attributes["MaxNumberOfCustomFields"]);
                objectTable.ObjectTablePlural = GetAttributeStringValue(entity.Attributes["ObjectTablePlural"]);
                objectTable.ObjectTableSingular = GetAttributeStringValue(entity.Attributes["ObjectTableSingular"]);
                objectTable.ObjectTableTypeCode = GetAttributeStringValue(entity.Attributes["ObjectTableTypeCode"]);
                objectTable.DxmlDatabaseTypeCode = GetAttributeStringValue(entity.Attributes["DxmlDatabaseTypeCode"]);
                objectTable.DxmlDatabaseSchemaCode = GetAttributeStringValue(entity.Attributes["DxmlDatabaseSchemaCode"]);
                objectTable.ShortTitleControlPath = GetAttributeStringValue(entity.Attributes["ShortTitleControlPath"]);
                objectTable.SortingByObjectField = GetAttributeStringValue(entity.Attributes["SortingByObjectField"]);
                objectTable.SortingByDirection = GetAttributeStringValue(entity.Attributes["SortingByDirection"]);
                objectTable.ParentTableName = GetAttributeStringValue(entity.Attributes["ParentTableName"]);
                objectTable.CacheOnClient = GetAttributeBoolValue(entity.Attributes["CacheOnClient"]);
                objectTable.AutoCompleteSearchWindow = GetAttributeBoolValue(entity.Attributes["AutoCompleteSearchWindow"]);
                objectTable.AllowCustomFields = GetAttributeBoolValue(entity.Attributes["AllowCustomFields"]);
                objectTable.NewWizardControlName = GetAttributeStringValue(entity.Attributes["NewWizardControlName"]);

                objectTable.DefaultText = GetAttributeStringValue(entity.Attributes["DefaultText"]);
                objectTable.LocalDefaultText = GetAttributeStringValue(entity.Attributes["LocalDefaultText"]);
                objectTable.NewButtonDefaultText = GetAttributeStringValue(entity.Attributes["NewButtonDefaultText"]);
                objectTable.NewButtonLocalDefaultText = GetAttributeStringValue(entity.Attributes["NewButtonLocalDefaultText"]);
                objectTable.QueryGroupCode = GetAttributeStringValue(entity.Attributes["Code"]);
                objectTable.QueryGroupName = GetAttributeStringValue(entity.Attributes["Name"]);
                objectTable.CloseTableCode = GetAttributeStringValue(entity.Attributes["CloseTableCode"]);
                objectTable.CloseTableName = GetAttributeStringValue(entity.Attributes["CloseTableName"]);
                objectTable.GenerateDomainService = GetAttributeBoolValue(entity.Attributes["GenerateDomainService"]);
                objectTable.ClientModuleName = GetAttributeStringValue(entity.Attributes["ClientModuleName"]);
                objectTable.ServerModuleName = GetAttributeStringValue(entity.Attributes["ServerModuleName"]);
                objectTable.NewWizardComponentPath = GetAttributeStringValue(entity.Attributes["NewWizardComponentPath"]);
                objectTable.HasMenuButtons = GetAttributeBoolValue(entity.Attributes["HasMenuButtons"]);
                objectTable.ApplyOnPropertyChangedCode = GetAttributeBoolValue(entity.Attributes["ApplyOnPropertyChangedCode"]);
                objectTable.HasApiHelper = GetAttributeBoolValue(entity.Attributes["HasApiHelper"]);
                objectTable.QueryGroupCode = GetAttributeStringValue(entity.Attributes["Code"]);
                objectTable.QueryGroupName = GetAttributeStringValue(entity.Attributes["Name"]);
                objectTable.LovDisplayMemberPath = GetAttributeStringValue(entity.Attributes["LovDisplayMemberPath"]);
                objectTable.LovDisplayMemberPathLocal = GetAttributeStringValue(entity.Attributes["LovDisplayMemberPathLocal"]);
                objectTable.TenantZeroData = GetAttributeBoolValue(entity.Attributes["TenantZeroData"]);
				objectTable.RelatedEntity = GetAttributeStringValue(entity.Attributes["RelatedEntity"]);
				objectTable.ThisKey = GetAttributeStringValue(entity.Attributes["ThisKey"]);
				objectTable.RelatedKey = GetAttributeStringValue(entity.Attributes["RelatedKey"]);

				if (entity.Attributes["IsLock"] != null)
				{
					objectTable.IsLock = GetAttributeBoolValue(entity.Attributes["IsLock"]);
				}
				else
				{
					objectTable.IsLock = false;
				}
                objectTable.ShowFastSearch = GetAttributeBoolValue(entity.Attributes["ShowFastSearch"]);

				if (entity.Attributes["NoViewController"] != null)
                {
                    objectTable.NoViewController = GetAttributeBoolValue(entity.Attributes["NoViewController"]);
                }
                else
                {
                    objectTable.NoViewController = false;
                }

                if (entity.Attributes["NoPMController"] != null)
                {
                    objectTable.NoPMController = GetAttributeBoolValue(entity.Attributes["NoPMController"]);
                }
                else
                {
                    objectTable.NoPMController = false;
                }

                if (entity.Attributes["NoTS"] != null)
                {
                    objectTable.NoTS = GetAttributeBoolValue(entity.Attributes["NoTS"]);
                }
                else
                {
                    objectTable.NoTS = false;
                }
                if (entity.Attributes["NoDefaultFeatures"] != null)
                {
                    objectTable.NoDefaultFeatures = GetAttributeBoolValue(entity.Attributes["NoDefaultFeatures"]);
                }
                else
                {
                    objectTable.NoDefaultFeatures = false;
                }
                if (entity.Attributes["IsBusinessUnitEnabled"] != null)
                {
                    objectTable.IsBusinessUnitEnabled = GetAttributeBoolValue(entity.Attributes["IsBusinessUnitEnabled"]);
                }
                else
                {
                    objectTable.IsBusinessUnitEnabled = false;
                }


                if (entity.Attributes["HasCompactSearch"] != null)
                {
                    objectTable.HasCompactSearch = GetAttributeBoolValue(entity.Attributes["HasCompactSearch"]);
                }
                else
                {
                    objectTable.HasCompactSearch = false;
                }

                if (entity.Attributes["ApplyDefaultValues"] != null)
                {
                    objectTable.ApplyDefaultValues = GetAttributeBoolValue(entity.Attributes["ApplyDefaultValues"]);
                }
                else
                {
                    objectTable.ApplyDefaultValues = false;
                }

                if (entity.Attributes["AllowedForComputingPartners"] != null)
                {
                    objectTable.AllowedForComputingPartners = GetAttributeBoolValue(entity.Attributes["AllowedForComputingPartners"]);
                }
                else
                {
                    objectTable.AllowedForComputingPartners = false;
                }

                if (entity.Attributes["IsMetadataOnlyTable"] != null)
                {
                    objectTable.IsMetadataOnlyTable = GetAttributeBoolValue(entity.Attributes["IsMetadataOnlyTable"]);
                }
                else
                {
                    objectTable.IsMetadataOnlyTable = false;
                }
                if (entity.Attributes["Code1"] != null && entity.Attributes["Name1"] != null)
                {
                    objectTable.QueryGroupCode1 = GetAttributeStringValue(entity.Attributes["Code1"]);
                    objectTable.QueryGroupName1 = GetAttributeStringValue(entity.Attributes["Name1"]);
                }

                if (entity.Attributes["CustomFieldsCount"] != null)
                {
                    objectTable.CustomFieldsCount = GetAttributeIntegerValue(entity.Attributes["CustomFieldsCount"]);
                }

                if (entity.Attributes["DisableSearchBox"] != null)
                {
                    objectTable.DisableSearchBox = GetAttributeBoolValue(entity.Attributes["DisableSearchBox"]);
                }
                else
                {
                    objectTable.DisableSearchBox = false;
                }

                if (entity.Attributes["HasDocuments"] != null)
                {
                    objectTable.HasDocuments = GetAttributeBoolValue(entity.Attributes["HasDocuments"]);
                }
                else
                {
                    objectTable.HasDocuments = false;
                }

                if (entity.Attributes["IsLookUp"] != null)
                {
                    objectTable.IsLookUp = GetAttributeBoolValue(entity.Attributes["IsLookUp"]);
                }
                else
                {
                    objectTable.IsLookUp = false;
                }

                if (entity.Attributes["SearchFields"] != null)
                {
                    objectTable.SearchFields = GetAttributeStringValue(entity.Attributes["SearchFields"]);

                }

                if (entity.Attributes["IsTabsHidden"] != null)
                {
                    objectTable.IsTabsHidden = GetAttributeBoolValue(entity.Attributes["IsTabsHidden"]);
                }
                else
                {
                    objectTable.IsTabsHidden = false;
                }


            }
            return objectTable;

        }

        public bool GetAttributeBoolValue(XmlAttribute att)
        {
            bool result = false;
            if (att != null)
            {
                if (!string.IsNullOrEmpty(att.Value))
                {
                    result = bool.Parse(att.Value);
                }
            }

            return result;
        }

        public int GetAttributeIntegerValue(XmlAttribute att)
        {
            int result = 0;
            if (att != null)
            {
                if (!string.IsNullOrEmpty(att.Value))
                {
                    result = int.Parse(att.Value);
                }
            }

            return result;
        }

        public int? GetAttributeNullableIntegerValue(XmlAttribute att)
        {
            int? result = null;
            if (att != null)
            {
                if (!string.IsNullOrEmpty(att.Value))
                {
                    result = int.Parse(att.Value);
                }
            }

            return result;
        }


        public double GetAttributeDoubleValue(XmlAttribute att)
        {
            double result = 0.0;
            if (att != null)
            {
                if (!string.IsNullOrEmpty(att.Value))
                {
                    result = double.Parse(att.Value);
                }
            }

            return result;
        }

        public string GetAttributeStringValue(XmlAttribute att)
        {
            string result = null;
            if (att != null)
            {
                if (!string.IsNullOrEmpty(att.Value))
                {
                    if (att.Value != null)
                    {
                        if (att.Value.StartsWith("BS64:") || att.Value.StartsWith("\"BS64:"))
                            att.Value = TryConvertFromBase64(att.Value);

                        result = att.Value.Trim('"');
                    }
                }
            }
            return result;
        }
        private string TryConvertFromBase64(string input)
        {
            try
            {
                return ConvertFromBase64(input);
            }
            catch (FormatException)
            {
                return input;
            }
        }
      
        private string ConvertFromBase64(string input)
        {
            string substringToRemove = "\"";
            string backUp = input;
            try
            {
                input = input.Trim('\"');
                input = input.Substring(5);//REMOVE BS64:
                byte[] data = Convert.FromBase64String(input);
                string decodedString = Encoding.UTF8.GetString(data);
                if (backUp.StartsWith(substringToRemove))
                {
                    decodedString = substringToRemove + decodedString;
                }

                if (backUp.EndsWith(substringToRemove))
                {
                    decodedString = decodedString + substringToRemove;
                }
                return decodedString;

            }
            catch (FormatException)
            {
                return backUp;
            }
        }

      


    }
}
