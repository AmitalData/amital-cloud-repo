

using AmitalCloud.Infrastructure.Data.Counters;
using AmitalCloud.Infrastructure.Data.Repositories;
using AmitalCloud.Infrastructure.Domain.EntityPMs;
using AmitalCloud.Infrastructure.Model.EntityClasses;
using System.Text;

namespace AmitalCloud.Infrastructure.MUpdater.Data.AddClasses
{
    public class AddObjectsAndObjectFields
    {
        public static Repository<ObjectTable> objecttablesRepository;
        public static ObjectTable AddObjectTable(ObjectTablePM objectTablesDetails, Repository<ObjectTable> objectTableRepository, Repository<TextCode> textCodeRepository, Dictionary<string, ObjectTable> tenantZeroObjectTables, Dictionary<string, TextCode> tenantZeroTextCodes,int contextTenant=0)
        {
            if (!tenantZeroObjectTables.Keys.Contains(objectTablesDetails.ObjectTableName))
            {
                #region Create
                ObjectTable objectTable = new ObjectTable();
                objectTable.Id = IdCounter.GetNumber("ObjectTable", contextTenant).ToString();

                objectTable.HasCustomFilter = objectTablesDetails.HasCustomFilter;                
                objectTable.Name = objectTablesDetails.ObjectTableName;
                objectTable.Tenant = 0;
                objectTable.LookUp1 = objectTablesDetails.LookUp1;
                objectTable.LookUp2 = objectTablesDetails.LookUp2;
                objectTable.DependencyFilter1 = objectTablesDetails.DependencyFilter1;
                objectTable.DependencyFilter2 = objectTablesDetails.DependencyFilter2;
                objectTable.DependencyFilter3 = objectTablesDetails.DependencyFilter3;
                objectTable.IsNewWizard = objectTablesDetails.IsNewWizard;
                objectTable.NewWizardControlName = objectTablesDetails.NewWizardControlName;
                objectTable.KeyPropertyPath = objectTablesDetails.KeyPropertyPath;
                objectTable.AutoCompleteSearchWindow = objectTablesDetails.AutoCompleteSearchWindow;
                objectTable.IsClosed = objectTablesDetails.IsClosed;
                objectTable.CacheOnClient = objectTablesDetails.CacheOnClient;
                objectTable.EditableFromAutoCompleteWindow = objectTablesDetails.EditableFromAutoCompleteWindow;
                objectTable.LastUpdateDate = DateTime.Now;
                objectTable.HasCounter = objectTablesDetails.HasCounter;
                objectTable.EnableAddFromLOV = objectTablesDetails.EnableAddFromLOV;
                objectTable.EnableEditFromLOV = objectTablesDetails.EnableEditFromLOV;
                objectTable.IsRestrictable = objectTablesDetails.IsRestrictable;
                objectTable.IsMain = objectTablesDetails.IsMain;
                objectTable.EnableEditFromLOV = objectTablesDetails.EnableEditFromLOV;
                objectTable.IsAutoComplete = objectTablesDetails.IsAutoComplete;
                objectTable.SortingByObjectField = objectTablesDetails.SortingByObjectField;
                objectTable.DBTableName = objectTablesDetails.DBTableName;
                objectTable.DBTableShortName = objectTablesDetails.DBTableShortName;
                objectTable.InActive = objectTablesDetails.InActive;
                objectTable.IsSaveButtonVisible = objectTablesDetails.IsSaveButtonVisible;
                objectTable.MainTipCode = objectTablesDetails.MainTipCode;
                objectTable.IsComposition = objectTablesDetails.IsComposition;
                objectTable.EnableSecurity = objectTablesDetails.EnableSecurity;
                objectTable.ObjectTableTypeCode = objectTablesDetails.ObjectTableTypeCode;
                objectTable.AllowCustomFields = objectTablesDetails.AllowCustomFields;
                objectTable.MaxNumberOfCustomFields = objectTablesDetails.MaxNumberOfCustomFields;
                objectTable.SearchFields = objectTablesDetails.ObjectTableName + "," + objectTablesDetails.DBTableName + "," + objectTablesDetails.NewWizardControlName + "," + objectTablesDetails.KeyPropertyPath + "," + objectTablesDetails.SortingByObjectField;
                objectTable.HasDynamicHeader = objectTablesDetails.HasDynamicHeader;
                objectTable.HasDocuments = objectTablesDetails.HasDocuments;
                objectTable.HasCustomFields = objectTablesDetails.HasCustomFields;
                objectTable.ClientModuleName = objectTablesDetails.ClientModuleName;
                objectTable.ServerModuleName = objectTablesDetails.ServerModuleName;
                objectTable.NewWizardComponentPath = objectTablesDetails.NewWizardComponentPath;
                objectTable.HasHelper = objectTablesDetails.HasHelper;
                objectTable.HasShortTitle = objectTablesDetails.HasShortTitle;
                objectTable.HasMenuButtons = objectTablesDetails.HasMenuButtons;
                objectTable.HasFiltersMenu = objectTablesDetails.HasFiltersMenu;
                objectTable.HasCustomValidator = objectTablesDetails.HasCustomValidator;
                objectTable.DisableSearchBox = objectTablesDetails.DisableSearchBox;
                objectTable.CustomFieldsCount = objectTablesDetails.CustomFieldsCount;
                objectTable.ParentObjectTableName = objectTablesDetails.ParentObjectTableName;
                objectTable.AllowedForComputingPartners = objectTablesDetails.AllowedForComputingPartners;
                objectTable.CodeField = objectTablesDetails.CodeField;
                objectTable.NameField = objectTablesDetails.NameField;
                objectTable.LovDisplayMemberPathLocal = objectTablesDetails.LovDisplayMemberPathLocal;
                objectTable.LovDisplayMemberPath = objectTablesDetails.LovDisplayMemberPath;
                objectTable.IsTabsHidden = objectTablesDetails.IsTabsHidden;
                if (!string.IsNullOrEmpty(objectTablesDetails.HashString))
                    objectTable.HashString = objectTablesDetails.HashString;
                objectTableRepository.Insert(objectTable);
                
                TextCode objectSingular = null;
                if (!tenantZeroTextCodes.Keys.Contains(objectTablesDetails.ObjectTableName))
                {
                    objectSingular = new TextCode();
                    objectSingular.Id = IdCounter.GetNumber("TextCode", contextTenant).ToString();
                    objectSingular.ObjectTableId = objectTable.Id;
                    objectSingular.Code = objectTablesDetails.ObjectTableName;
                    objectSingular.DefaultText = objectTablesDetails.DefaultText;
                    objectSingular.DefaultTextPlural = objectTablesDetails.ObjectTablePlural;
                    objectSingular.Tenant = 0;
                    objectSingular.TextCodeTypeCode = "T";
                    objectSingular.LocalDefaultText = objectTablesDetails.LocalDefaultText;
                    textCodeRepository.Insert(objectSingular);
                }
                else
                {
                    objectSingular = tenantZeroTextCodes[objectTablesDetails.ObjectTableName];
                    objectSingular.DefaultText = objectTablesDetails.DefaultText;
                    objectSingular.DefaultTextPlural = objectTablesDetails.ObjectTablePlural;
                    objectSingular.LocalDefaultText = TryConvertFromBase64(objectTablesDetails.LocalDefaultText);
                    objectSingular.InActive = objectTablesDetails.InActive;
                    textCodeRepository.Update(objectSingular);
                }
                

                if (!string.IsNullOrEmpty(objectTablesDetails.DescriptionDefaultText))
                {
                    TextCode descriptionTextCode = null;
                    if (!tenantZeroTextCodes.Keys.Contains(objectTablesDetails.ObjectTableName + "Description"))
                    {
                        descriptionTextCode = new TextCode();
                        descriptionTextCode.Id = IdCounter.GetNumber("TextCode", contextTenant).ToString();
                        descriptionTextCode.ObjectTableId = objectTable.Id;
                        descriptionTextCode.Code = objectTablesDetails.ObjectTableName + "Description";
                        descriptionTextCode.DefaultText = objectTablesDetails.DescriptionDefaultText;
                        descriptionTextCode.LocalDefaultText = TryConvertFromBase64(objectTablesDetails.DescriptionLocalDefaultText);


                        descriptionTextCode.Tenant = 0;
                        descriptionTextCode.TextCodeTypeCode = "F";
                        textCodeRepository.Insert(descriptionTextCode);
                        textCodeRepository.SubmitChanges();
                    }
                    else
                    {
                        descriptionTextCode = tenantZeroTextCodes[objectTablesDetails.ObjectTableName + "Description"];
                        descriptionTextCode.DefaultText = objectTablesDetails.DescriptionDefaultText;
                        descriptionTextCode.LocalDefaultText = TryConvertFromBase64(objectTablesDetails.DescriptionLocalDefaultText);
                        textCodeRepository.Update(descriptionTextCode);
                        textCodeRepository.SubmitChanges();
                    }
                    objectTable.DescriptionTextCodeId = descriptionTextCode.Id;
                    objectTable.DescriptionTextCodeCode = descriptionTextCode.Code;
                }

                if (!string.IsNullOrEmpty(objectTablesDetails.NewButtonDefaultText) || !string.IsNullOrEmpty(objectTablesDetails.NewButtonLocalDefaultText))
                {
                    TextCode newButtonTextCode = null;
                    if (!tenantZeroTextCodes.Keys.Contains(objectTablesDetails.ObjectTableName + ".NewButton"))
                    {
                        newButtonTextCode = new TextCode();
                        newButtonTextCode.Id = IdCounter.GetNumber("TextCode", contextTenant).ToString();
                        newButtonTextCode.ObjectTableId = objectTable.Id;
                        newButtonTextCode.Code = objectTablesDetails.ObjectTableName + ".NewButton";
                        newButtonTextCode.DefaultText = objectTablesDetails.NewButtonDefaultText;
                        newButtonTextCode.LocalDefaultText = TryConvertFromBase64(objectTablesDetails.NewButtonLocalDefaultText);


                        newButtonTextCode.Tenant = 0;
                        newButtonTextCode.TextCodeTypeCode = "B";
                        textCodeRepository.Insert(newButtonTextCode);
                        textCodeRepository.SubmitChanges();
                    }
                    else
                    {
                        newButtonTextCode = tenantZeroTextCodes[objectTablesDetails.ObjectTableName + ".NewButton"];
                        newButtonTextCode.DefaultText = objectTablesDetails.NewButtonDefaultText; 
                        textCodeRepository.Update(newButtonTextCode);
                        textCodeRepository.SubmitChanges();
                    }
                    objectTable.NewButtonTextCodeId = newButtonTextCode.Id;
                    objectTable.NewButtonTextCodeCode = newButtonTextCode.Code;
                }

                objectTable.IsLookUp = (!string.IsNullOrEmpty(objectTablesDetails.LookUp1) && !objectTablesDetails.IsComposition);
                objectTable.AvailableInCustomization = objectTablesDetails.AvailableInCustomization;
                objectTable.SupportSubEntity = objectTablesDetails.SupportSubEntity;
                objectTable.ApplyGenericCustomFields = objectTablesDetails.ApplyGenericCustomFields;
                objectTable.AvailableInDocumentTypes = objectTablesDetails.AvailableInDocumentTypes;
                objectTable.IsLock = objectTablesDetails.IsLock;
                objectTable.RelatedEntity = objectTablesDetails.RelatedEntity;
                objectTable.ThisKey = objectTablesDetails.ThisKey;
                objectTable.RelatedKey = objectTablesDetails.RelatedKey;
                objectTable.ShowFastSearch = objectTablesDetails.ShowFastSearch;

                return objectTable;
                #endregion
            }

            else
            {
                #region Update
                ObjectTable updatedObjectTable = tenantZeroObjectTables[objectTablesDetails.ObjectTableName];
                if (!string.IsNullOrEmpty(objectTablesDetails.HashString))
                    updatedObjectTable.HashString = objectTablesDetails.HashString;
                updatedObjectTable.HasCustomFilter = objectTablesDetails.HasCustomFilter;                
                updatedObjectTable.Name = objectTablesDetails.ObjectTableName;
                updatedObjectTable.Tenant = 0;
                updatedObjectTable.LookUp1 = objectTablesDetails.LookUp1;
                updatedObjectTable.LookUp2 = objectTablesDetails.LookUp2;
                updatedObjectTable.DependencyFilter1 = objectTablesDetails.DependencyFilter1;
                updatedObjectTable.DependencyFilter2 = objectTablesDetails.DependencyFilter2;
                updatedObjectTable.DependencyFilter3 = objectTablesDetails.DependencyFilter3;
                updatedObjectTable.IsNewWizard = objectTablesDetails.IsNewWizard;
                updatedObjectTable.NewWizardControlName = objectTablesDetails.NewWizardControlName;
                updatedObjectTable.KeyPropertyPath = objectTablesDetails.KeyPropertyPath;
                updatedObjectTable.AutoCompleteSearchWindow = objectTablesDetails.AutoCompleteSearchWindow;
                updatedObjectTable.IsClosed = objectTablesDetails.IsClosed;
                updatedObjectTable.CacheOnClient = objectTablesDetails.CacheOnClient;
                updatedObjectTable.EditableFromAutoCompleteWindow = objectTablesDetails.EditableFromAutoCompleteWindow;
                updatedObjectTable.LastUpdateDate = DateTime.Now;
                updatedObjectTable.HasCounter = objectTablesDetails.HasCounter;
                updatedObjectTable.EnableAddFromLOV = objectTablesDetails.EnableAddFromLOV;
                updatedObjectTable.EnableEditFromLOV = objectTablesDetails.EnableEditFromLOV;
                updatedObjectTable.IsRestrictable = objectTablesDetails.IsRestrictable;
                updatedObjectTable.IsMain = objectTablesDetails.IsMain;
                updatedObjectTable.EnableEditFromLOV = objectTablesDetails.EnableEditFromLOV;
                updatedObjectTable.IsAutoComplete = objectTablesDetails.IsAutoComplete;
                updatedObjectTable.SortingByObjectField = objectTablesDetails.SortingByObjectField;
                updatedObjectTable.DBTableName = objectTablesDetails.DBTableName;
                updatedObjectTable.DBTableShortName = objectTablesDetails.DBTableShortName;
                updatedObjectTable.InActive = objectTablesDetails.InActive;
                updatedObjectTable.IsSaveButtonVisible = objectTablesDetails.IsSaveButtonVisible;
                updatedObjectTable.IsComposition = objectTablesDetails.IsComposition;
                updatedObjectTable.MainTipCode = objectTablesDetails.MainTipCode;
                updatedObjectTable.EnableSecurity = objectTablesDetails.EnableSecurity;
                updatedObjectTable.ObjectTableTypeCode = objectTablesDetails.ObjectTableTypeCode;
                updatedObjectTable.AllowCustomFields = objectTablesDetails.AllowCustomFields;
                updatedObjectTable.MaxNumberOfCustomFields = objectTablesDetails.MaxNumberOfCustomFields;
                updatedObjectTable.SearchFields = objectTablesDetails.ObjectTableName + "," + objectTablesDetails.DBTableName + "," + objectTablesDetails.NewWizardControlName + "," + objectTablesDetails.KeyPropertyPath + "," + objectTablesDetails.SortingByObjectField;
                updatedObjectTable.HasDynamicHeader = objectTablesDetails.HasDynamicHeader;
                updatedObjectTable.HasDocuments = objectTablesDetails.HasDocuments;
                updatedObjectTable.HasCustomFields = objectTablesDetails.HasCustomFields;
                updatedObjectTable.ClientModuleName = objectTablesDetails.ClientModuleName;
                updatedObjectTable.ServerModuleName = objectTablesDetails.ServerModuleName;
                updatedObjectTable.NewWizardComponentPath = objectTablesDetails.NewWizardComponentPath;
                updatedObjectTable.HasHelper = objectTablesDetails.HasHelper;
                updatedObjectTable.HasShortTitle = objectTablesDetails.HasShortTitle;
                updatedObjectTable.HasMenuButtons = objectTablesDetails.HasMenuButtons;
                updatedObjectTable.HasFiltersMenu = objectTablesDetails.HasFiltersMenu;
                updatedObjectTable.HasCustomValidator = objectTablesDetails.HasCustomValidator;
                updatedObjectTable.DisableSearchBox = objectTablesDetails.DisableSearchBox;
                updatedObjectTable.CustomFieldsCount = objectTablesDetails.CustomFieldsCount;
                updatedObjectTable.ParentObjectTableName = objectTablesDetails.ParentObjectTableName;
                updatedObjectTable.AllowedForComputingPartners = objectTablesDetails.AllowedForComputingPartners;
                updatedObjectTable.CodeField = objectTablesDetails.CodeField;
                updatedObjectTable.NameField = objectTablesDetails.NameField;
                updatedObjectTable.LovDisplayMemberPath = objectTablesDetails.LovDisplayMemberPath;
                updatedObjectTable.LovDisplayMemberPathLocal= objectTablesDetails.LovDisplayMemberPathLocal;
                if (tenantZeroTextCodes.Keys.Contains(objectTablesDetails.ObjectTableName + updatedObjectTable.Tenant.ToString() + updatedObjectTable.Id))
                {
                    TextCode updatedTextCode = tenantZeroTextCodes[objectTablesDetails.ObjectTableName + updatedObjectTable.Tenant.ToString() + updatedObjectTable.Id];
                    if (!updatedTextCode.IsSpellChecked || (string.IsNullOrEmpty(updatedTextCode.DefaultText) || string.IsNullOrEmpty(updatedTextCode.DefaultTextPlural)))
                    {
                        updatedTextCode.DefaultText = objectTablesDetails.DefaultText;
                        updatedTextCode.DefaultTextPlural = objectTablesDetails.ObjectTablePlural;
                        updatedTextCode.LocalDefaultText = TryConvertFromBase64(objectTablesDetails.LocalDefaultText);
                        updatedTextCode.InActive = objectTablesDetails.InActive;
                        textCodeRepository.Update(updatedTextCode);
                    }
                }
                else
                {
                    TextCode textCode = new TextCode();
                    textCode.Id = IdCounter.GetNumber("TextCode", contextTenant).ToString();
                    textCode.ObjectTableId = updatedObjectTable.Id;
                    textCode.Code = objectTablesDetails.ObjectTableName;
                    textCode.DefaultText = objectTablesDetails.DefaultText;
                    textCode.DefaultTextPlural = objectTablesDetails.ObjectTablePlural;
                    textCode.Tenant = 0;
                    textCode.TextCodeTypeCode = "T";
                    textCode.LocalDefaultText = TryConvertFromBase64(objectTablesDetails.LocalDefaultText);
                    textCodeRepository.Insert(textCode);
                }
                if (objectTablesDetails.DescriptionDefaultText != null)
                {
                    TextCode descriptionTextCode = null;
                    if (tenantZeroTextCodes.Keys.Contains(objectTablesDetails.ObjectTableName + "Description" + updatedObjectTable.Tenant.ToString() + updatedObjectTable.Id))
                    {
                        descriptionTextCode = tenantZeroTextCodes[objectTablesDetails.ObjectTableName + "Description" + updatedObjectTable.Tenant.ToString() + updatedObjectTable.Id];
                        if (!descriptionTextCode.IsSpellChecked || string.IsNullOrEmpty(descriptionTextCode.DefaultText) || string.IsNullOrEmpty(descriptionTextCode.DefaultTextPlural))
                        {
                            descriptionTextCode.DefaultText = objectTablesDetails.DescriptionDefaultText;
                            descriptionTextCode.DefaultTextPlural = objectTablesDetails.ObjectTablePlural;
                            descriptionTextCode.InActive = objectTablesDetails.InActive;
                            descriptionTextCode.LocalDefaultText = TryConvertFromBase64(objectTablesDetails.DescriptionLocalDefaultText);
                            textCodeRepository.Update(descriptionTextCode);
                        }
                    }

                    else
                    {
                        descriptionTextCode = new TextCode();
                        descriptionTextCode.Id = IdCounter.GetNumber("TextCode", contextTenant).ToString();
                        descriptionTextCode.ObjectTableId = updatedObjectTable.Id;
                        descriptionTextCode.Code = objectTablesDetails.ObjectTableName + "Description";
                        descriptionTextCode.DefaultText = objectTablesDetails.DescriptionDefaultText;
                        descriptionTextCode.LocalDefaultText = TryConvertFromBase64(objectTablesDetails.DescriptionLocalDefaultText);

                        descriptionTextCode.Tenant = 0;
                        descriptionTextCode.TextCodeTypeCode = "T";
                        textCodeRepository.Insert(descriptionTextCode);
                        updatedObjectTable.DescriptionTextCodeId = descriptionTextCode.Id;
                        updatedObjectTable.DescriptionTextCodeCode = descriptionTextCode.Code;
                    }
                }

                if (objectTablesDetails.NewButtonDefaultText != null || objectTablesDetails.NewButtonLocalDefaultText != null)
                {
                    TextCode newButtonTextCode = null;
                    if (tenantZeroTextCodes.Keys.Contains(objectTablesDetails.ObjectTableName + ".NewButton" + updatedObjectTable.Tenant.ToString() + updatedObjectTable.Id))
                    {
                        newButtonTextCode = tenantZeroTextCodes[objectTablesDetails.ObjectTableName + ".NewButton" + updatedObjectTable.Tenant.ToString() + updatedObjectTable.Id];
                        if (!newButtonTextCode.IsSpellChecked || string.IsNullOrEmpty(newButtonTextCode.DefaultText))
                        {
                            newButtonTextCode.DefaultText = objectTablesDetails.NewButtonDefaultText;
                            newButtonTextCode.DefaultText = objectTablesDetails.NewButtonLocalDefaultText;
                            newButtonTextCode.InActive = objectTablesDetails.InActive;
                            textCodeRepository.Update(newButtonTextCode);
                        }
                    }

                    else
                    {
                        newButtonTextCode = new TextCode();
                        newButtonTextCode.Id = IdCounter.GetNumber("TextCode", contextTenant).ToString();
                        newButtonTextCode.ObjectTableId = updatedObjectTable.Id;
                        newButtonTextCode.Code = objectTablesDetails.ObjectTableName + ".NewButton";
                        newButtonTextCode.DefaultText = objectTablesDetails.NewButtonDefaultText;
                        newButtonTextCode.LocalDefaultText = TryConvertFromBase64(objectTablesDetails.NewButtonLocalDefaultText);
                        newButtonTextCode.Tenant = 0;
                        newButtonTextCode.TextCodeTypeCode = "B";
                        textCodeRepository.Insert(newButtonTextCode);
                        updatedObjectTable.NewButtonTextCodeId = newButtonTextCode.Id;
                        updatedObjectTable.NewButtonTextCodeCode = newButtonTextCode.Code;
                    }
                }

                updatedObjectTable.IsLookUp = (!string.IsNullOrEmpty(objectTablesDetails.LookUp1) && !objectTablesDetails.IsComposition);
                updatedObjectTable.IsTabsHidden = objectTablesDetails.IsTabsHidden;
                updatedObjectTable.AvailableInCustomization = objectTablesDetails.AvailableInCustomization;
                updatedObjectTable.SupportSubEntity = objectTablesDetails.SupportSubEntity;
                updatedObjectTable.ApplyGenericCustomFields = objectTablesDetails.ApplyGenericCustomFields;
                updatedObjectTable.AvailableInDocumentTypes = objectTablesDetails.AvailableInDocumentTypes;
				updatedObjectTable.IsLock = objectTablesDetails.IsLock;
				updatedObjectTable.RelatedEntity = objectTablesDetails.RelatedEntity;
				updatedObjectTable.ThisKey = objectTablesDetails.ThisKey;
				updatedObjectTable.RelatedKey = objectTablesDetails.RelatedKey;
                updatedObjectTable.ShowFastSearch = objectTablesDetails.ShowFastSearch;

				objectTableRepository.Update(updatedObjectTable);
                return updatedObjectTable;
                #endregion
            }
        }

        public static void AddObjectField(ObjectFieldPM objectFieldDetails, Repository<TextCode> textCodeRepository, ObjectFieldRepository objectFieldsRepository, Dictionary<string, ObjectField> tenantZeroObjectFields, Dictionary<string, TextCode> tenantZeroTextCodes, Dictionary<string, ObjectTable> tenantZeroObjectTables,int contextTenant = 0)
        {
           

            if (!String.IsNullOrEmpty(objectFieldDetails.ObjectTableName) && String.IsNullOrEmpty(objectFieldDetails.ObjectTableId))
            {
                ObjectTable table = GetObjectTable(objectFieldDetails.ObjectTableName, tenantZeroObjectTables, contextTenant);
              
                objectFieldDetails.ObjectTableId = table.Id;

            }
            if (!String.IsNullOrEmpty(objectFieldDetails.LookUpTableName) && String.IsNullOrEmpty(objectFieldDetails.LookUpTableId))
            {
                ObjectTable table = GetObjectTable(objectFieldDetails.LookUpTableName, tenantZeroObjectTables, contextTenant);
                if (table != null)
                {
                    objectFieldDetails.LookUpTableId = table.Id;
                }

            }

            if (!String.IsNullOrEmpty(objectFieldDetails.MultiTableName) && String.IsNullOrEmpty(objectFieldDetails.MultiTableId))
            {
                ObjectTable table = GetObjectTable(objectFieldDetails.MultiTableName, tenantZeroObjectTables, contextTenant);
                if (table != null)
                {
                    objectFieldDetails.MultiTableId = table.Id;
                }

            }
            if (!tenantZeroObjectFields.Keys.Contains(objectFieldDetails.FieldName + objectFieldDetails.ObjectTableId))
            {

                TextCode objectFieldTextCode = null;
                if (!tenantZeroTextCodes.Keys.Contains(objectFieldDetails.ObjectTableName + ".F." + objectFieldDetails.FullFieldLable + objectFieldDetails.Tenant + objectFieldDetails.ObjectTableId))
                {
                    objectFieldTextCode = new TextCode();
                    objectFieldTextCode.Code = objectFieldDetails.ObjectTableName + ".F." + objectFieldDetails.FullFieldLable;
                    objectFieldTextCode.DefaultText = objectFieldDetails.DefaultText;
                    objectFieldTextCode.Id = IdCounter.GetNumber("TextCode", objectFieldDetails.Tenant).ToString();
                    objectFieldTextCode.ObjectTableId = objectFieldDetails.ObjectTableId;
                    objectFieldTextCode.Tenant = 0;
                    objectFieldTextCode.TextCodeTypeCode = "F";
                    objectFieldTextCode.InActive = objectFieldDetails.InActive;
                    objectFieldTextCode.LocalDefaultText = TryConvertFromBase64(objectFieldDetails.FullLocalDefaultText);
                    textCodeRepository.Insert(objectFieldTextCode);
                }
                else
                {
                    objectFieldTextCode = tenantZeroTextCodes[objectFieldDetails.ObjectTableName + ".F." + objectFieldDetails.FullFieldLable + objectFieldDetails.Tenant + objectFieldDetails.ObjectTableId];
                    objectFieldTextCode.DefaultText = objectFieldDetails.DefaultText;
                    textCodeRepository.Update(objectFieldTextCode);
                }

                TextCode helpTextTextCode = null;
                TextCode listFieldLableTextCode = null;
                TextCode fullFieldTextCode = null;
                if (!string.IsNullOrEmpty(objectFieldDetails.HelpTextCode))
                {
                    if (!tenantZeroTextCodes.Keys.Contains(objectFieldDetails.ObjectTableName + ".F." + objectFieldDetails.HelpTextCode + "HelpText"))
                    {
                        helpTextTextCode = new TextCode();
                        helpTextTextCode.Code = objectFieldDetails.ObjectTableName + "." + objectFieldDetails.HelpTextCode + "HelpText";
                        helpTextTextCode.DefaultText = objectFieldDetails.HelpTextDefaultText;
                        helpTextTextCode.Id = IdCounter.GetNumber("TextCode", objectFieldDetails.Tenant).ToString();
                        helpTextTextCode.ObjectTableId = objectFieldDetails.ObjectTableId;
                        helpTextTextCode.Tenant = 0;
                        helpTextTextCode.TextCodeTypeCode = "H";
                        helpTextTextCode.InActive = objectFieldDetails.InActive;
                        helpTextTextCode.LocalDefaultText = TryConvertFromBase64(objectFieldDetails.HelpLocalDefaultText);
                        textCodeRepository.Insert(helpTextTextCode);
                    }
                    else
                    {
                        helpTextTextCode = tenantZeroTextCodes[objectFieldDetails.ObjectTableName + ".F." + objectFieldDetails.HelpTextCode + "HelpText"];
                        helpTextTextCode.DefaultText = objectFieldDetails.HelpTextDefaultText;
                        helpTextTextCode.LocalDefaultText = TryConvertFromBase64(objectFieldDetails.HelpLocalDefaultText);
                        textCodeRepository.Update(helpTextTextCode);
                    }
                }

                else
                {
                    if (!tenantZeroTextCodes.Keys.Contains((!string.IsNullOrEmpty(objectFieldDetails.ObjectTableName) ? objectFieldDetails.ObjectTableName : objectFieldDetails.ValidForQuerySection1) + "." + objectFieldDetails.FullFieldLable + "HelpText"))
                    {
                        helpTextTextCode = new TextCode();
                        helpTextTextCode.Code = (!string.IsNullOrEmpty(objectFieldDetails.ObjectTableName) ? objectFieldDetails.ObjectTableName : objectFieldDetails.ValidForQuerySection1) + "." + objectFieldDetails.FullFieldLable + "HelpText";
                        helpTextTextCode.DefaultText = null;
                        helpTextTextCode.Id = IdCounter.GetNumber("TextCode", objectFieldDetails.Tenant).ToString();
                        helpTextTextCode.ObjectTableId = objectFieldDetails.ObjectTableId;
                        helpTextTextCode.Tenant = 0;
                        helpTextTextCode.TextCodeTypeCode = "H";
                        helpTextTextCode.InActive = objectFieldDetails.InActive;
                        helpTextTextCode.LocalDefaultText = TryConvertFromBase64(objectFieldDetails.HelpLocalDefaultText);
                        textCodeRepository.Insert(helpTextTextCode);
                    }
                    else
                    {
                        helpTextTextCode = tenantZeroTextCodes[(!string.IsNullOrEmpty(objectFieldDetails.ObjectTableName) ? objectFieldDetails.ObjectTableName : objectFieldDetails.ValidForQuerySection1) + "." + objectFieldDetails.FullFieldLable + "HelpText"];
                        helpTextTextCode.DefaultText = objectFieldDetails.HelpTextDefaultText;
                        helpTextTextCode.LocalDefaultText = TryConvertFromBase64(objectFieldDetails.HelpLocalDefaultText);
                        helpTextTextCode.InActive = objectFieldDetails.InActive;
                        textCodeRepository.Update(helpTextTextCode);
                    }
                }

                if (!string.IsNullOrEmpty(objectFieldDetails.ShortFieldLable))
                {
                    if (!tenantZeroTextCodes.Keys.Contains(objectFieldDetails.ObjectTableName + ".F." + objectFieldDetails.ShortFieldLable + ".Short"))
                    {
                        fullFieldTextCode = new TextCode();
                        fullFieldTextCode.Code = objectFieldDetails.ObjectTableName + ".F." + objectFieldDetails.ShortFieldLable + ".Short";
                        fullFieldTextCode.DefaultText = objectFieldDetails.ShortFieldLableDefaultText;
                        fullFieldTextCode.Id = IdCounter.GetNumber("TextCode", objectFieldDetails.Tenant).ToString();
                        fullFieldTextCode.ObjectTableId = objectFieldDetails.ObjectTableId;
                        fullFieldTextCode.Tenant = 0;
                        fullFieldTextCode.TextCodeTypeCode = "F";
                        fullFieldTextCode.InActive = objectFieldDetails.InActive;
                        fullFieldTextCode.LocalDefaultText = TryConvertFromBase64(objectFieldDetails.ShortLocalDefaultText);
                        textCodeRepository.Insert(fullFieldTextCode);
                    }
                    else
                    {
                        fullFieldTextCode = tenantZeroTextCodes[objectFieldDetails.ObjectTableName + ".F." + objectFieldDetails.ShortFieldLable + ".Short"];
                        fullFieldTextCode.DefaultText = objectFieldDetails.ShortFieldLableDefaultText;
                        fullFieldTextCode.LocalDefaultText = TryConvertFromBase64(objectFieldDetails.ShortLocalDefaultText);
                        fullFieldTextCode.InActive = objectFieldDetails.InActive;
                        textCodeRepository.Update(fullFieldTextCode);
                    }
                }

                if (!string.IsNullOrEmpty(objectFieldDetails.ListFieldLable) && (objectFieldDetails.DisplayInList || objectFieldDetails.DisplayInSearchWindowList || objectFieldDetails.DisplayOnLookUp || objectFieldDetails.DisplayOnLookUpLocal))
                {
                    if (!tenantZeroTextCodes.Keys.Contains(objectFieldDetails.ObjectTableName + ".CH." + objectFieldDetails.ListFieldLable))
                    {
                        listFieldLableTextCode = new TextCode();
                        listFieldLableTextCode.Code = objectFieldDetails.ObjectTableName + ".CH." + objectFieldDetails.ListFieldLable;
                        listFieldLableTextCode.DefaultText = objectFieldDetails.ListLableDefaultText;
                        listFieldLableTextCode.Id = IdCounter.GetNumber("TextCode", objectFieldDetails.Tenant).ToString();
                        listFieldLableTextCode.ObjectTableId = objectFieldDetails.ObjectTableId;
                        listFieldLableTextCode.Tenant = 0;
                        listFieldLableTextCode.TextCodeTypeCode = "CH";
                        listFieldLableTextCode.InActive = objectFieldDetails.InActive;
                        listFieldLableTextCode.LocalDefaultText = TryConvertFromBase64(objectFieldDetails.ListLocalDefaultText);
                        textCodeRepository.Insert(listFieldLableTextCode);
                    }
                    else
                    {
                        listFieldLableTextCode = tenantZeroTextCodes[objectFieldDetails.ObjectTableName + ".CH." + objectFieldDetails.ListFieldLable];
                        listFieldLableTextCode.DefaultText = objectFieldDetails.ListLableDefaultText;
                        listFieldLableTextCode.LocalDefaultText = TryConvertFromBase64(objectFieldDetails.ListLocalDefaultText);
                        listFieldLableTextCode.InActive = objectFieldDetails.InActive;
                        textCodeRepository.Update(listFieldLableTextCode);
                    }

                }


                ObjectField newObjectField = new ObjectField();
                newObjectField.ControlField1 = objectFieldDetails.ControlField1;
                newObjectField.ControlField2 = objectFieldDetails.ControlField2;
                newObjectField.ControlField3 = objectFieldDetails.ControlField3;
                newObjectField.DataTypeCode = objectFieldDetails.FieldsDataType;
                newObjectField.DisplayOnLookUp = objectFieldDetails.DisplayOnLookUp;
                newObjectField.DisplayOnLookUpLocal = objectFieldDetails.DisplayOnLookUpLocal;
                newObjectField.FullNameTextCodeId = objectFieldTextCode.Id;
                newObjectField.FullNameTextCodeCode = objectFieldTextCode.Code;
                newObjectField.FieldName = objectFieldDetails.FieldName;
                newObjectField.ShortName = objectFieldDetails.ShortName;
                newObjectField.Code = objectFieldDetails.Code;
                if (string.IsNullOrEmpty(newObjectField.FieldCode))
                {
                    newObjectField.FieldCode = objectFieldDetails.ObjectTableName + "." + objectFieldDetails.FieldName; ;
                }
                if (string.IsNullOrEmpty(objectFieldDetails.Code))
                {
                    newObjectField.Code = objectFieldDetails.FieldName;
                }
                if (helpTextTextCode != null)
                {
                    newObjectField.HelpTextCodeId = helpTextTextCode.Id;
                    newObjectField.HelpTextCodeCode = helpTextTextCode.Code;
                }
                if (listFieldLableTextCode != null)
                {
                    newObjectField.ListTextCodeId = listFieldLableTextCode.Id;
                    newObjectField.ListTextCodeCode = listFieldLableTextCode.Code;
                }
                newObjectField.Id = IdCounter.GetNumber("ObjectField", objectFieldDetails.Tenant).ToString();
                newObjectField.IsCustom = objectFieldDetails.IsCustom;
                newObjectField.LookUpTableId = objectFieldDetails.LookUpTableId;
                newObjectField.MaxLength = objectFieldDetails.MaxLength;
                newObjectField.MinLength = objectFieldDetails.MinLength;
                newObjectField.IsMaxLength = objectFieldDetails.IsMaxLength;
                newObjectField.IsRequired = objectFieldDetails.IsRequired;
                newObjectField.ObjectTableId = objectFieldDetails.ObjectTableId;
                newObjectField.Tenant = 0;
                newObjectField.CanFilter = objectFieldDetails.CanFilter;
                newObjectField.DisplayOnly = objectFieldDetails.DisplayOnly;
                if (objectFieldDetails.IsRequired)
                {
                    newObjectField.SystemRequired = objectFieldDetails.SystemRequired;
                }
                newObjectField.SystemMaxLength = newObjectField.MaxLength;
                newObjectField.DisplayInList = objectFieldDetails.DisplayInList;
                newObjectField.ConverterName = objectFieldDetails.ConverterName;
                newObjectField.DataTemplateName = objectFieldDetails.DataTemplateName;
                newObjectField.MultiLine = objectFieldDetails.MultiLine;
                newObjectField.IsCustomFilter = objectFieldDetails.IsCustomFilter;
                newObjectField.Operator = objectFieldDetails.Operator;
                newObjectField.IsTimeFrameFilter = objectFieldDetails.IsTimeFrameFilter;
                newObjectField.DisplayInSearchWindowFilters = objectFieldDetails.DisplayInSearchWindowFilters;
                newObjectField.DisplayInSearchWindowList = objectFieldDetails.DisplayInSearchWindowList;
                newObjectField.PMPropertyPath = objectFieldDetails.PMPropertyPath;
                newObjectField.ListPropertyPath = objectFieldDetails.ListPropertyPath;
                newObjectField.DisplayInLookUpIndex = objectFieldDetails.DisplayInLookUpIndex;
                newObjectField.AutomaticField = objectFieldDetails.AutomaticField;
                newObjectField.UniqueField = objectFieldDetails.UniqueField;
                newObjectField.ShortNameTextCodeId = fullFieldTextCode != null ? fullFieldTextCode.Id : null;
                newObjectField.ShortNameTextCodeCode = fullFieldTextCode != null ? fullFieldTextCode.Code : null;
                newObjectField.DisplayInSearchWindowFiltersIndex = objectFieldDetails.DisplayInSearchWindowFiltersIndex;
                newObjectField.DisplayInSearchWindowListIndex = objectFieldDetails.DisplayInSearchWindowListIndex;
                newObjectField.IsMulti = objectFieldDetails.IsMulti;
                newObjectField.MultiTableId = objectFieldDetails.MultiTableId;
                newObjectField.DependencyFilter1Value = objectFieldDetails.DependencyFilter1Value;
                newObjectField.DependencyFilter2Value = objectFieldDetails.DependencyFilter2Value;
                newObjectField.DependencyFilter3Value = objectFieldDetails.DependencyFilter3Value;
                newObjectField.DependencyFilter1Type = objectFieldDetails.DependencyFilter1Type;
                newObjectField.DependencyFilter2Type = objectFieldDetails.DependencyFilter2Type;
                newObjectField.DependencyFilter3Type = objectFieldDetails.DependencyFilter3Type;
                newObjectField.ValidForQuerySection1 = objectFieldDetails.ValidForQuerySection1;
                newObjectField.ValidForQuerySection2 = objectFieldDetails.ValidForQuerySection2;
                newObjectField.IsRestrictable = objectFieldDetails.IsRestrictable;
                newObjectField.DisplayInEntityVariables = objectFieldDetails.DisplayInEntityVariables;
                newObjectField.InActive = objectFieldDetails.InActive;
                newObjectField.DisplayInLookupColumnSize = objectFieldDetails.DisplayInLookupColumnSize;
                newObjectField.ColumnHeaderTemplateName = objectFieldDetails.ColumnHeaderTemplateName;
                newObjectField.CustomerPermissionTypeCode = objectFieldDetails.CustomerPermissionTypeCode;
                newObjectField.AgentPermissionTypeCode = objectFieldDetails.AgentPermissionTypeCode;
                newObjectField.CustomPickListCode = objectFieldDetails.CustomPickListCode;
                newObjectField.NumberOfDigits = objectFieldDetails.NumberOfDigits;
                newObjectField.DigitsAfterPoint = objectFieldDetails.DigitsAfterPoint;
                newObjectField.DependencyFilter1IsList = objectFieldDetails.DependencyFilter1IsList;
                newObjectField.DependencyFilter2IsList = objectFieldDetails.DependencyFilter2IsList;
                newObjectField.DependencyFilter3IsList = objectFieldDetails.DependencyFilter3IsList;
                newObjectField.IsMaxLength = objectFieldDetails.IsMaxLength;
                newObjectField.HasTemplate = objectFieldDetails.HasTemplate;
                newObjectField.HtmlHeaderComponentName = objectFieldDetails.HtmlHeaderComponentName;
                newObjectField.HtmlHeaderComponentUrl = objectFieldDetails.HtmlHeaderComponentUrl;
                newObjectField.HtmlListComponentName = objectFieldDetails.HtmlListComponentName;
                newObjectField.HtmlListComponentUrl = objectFieldDetails.HtmlListComponentUrl;
                newObjectField.AllowedinAutomationConditions = objectFieldDetails.AllowedinAutomationConditions;
                newObjectField.AutomationEmailRecipient = objectFieldDetails.AutomationEmailRecipient;
                newObjectField.CanAutomateSetValue = objectFieldDetails.CanAutomateSetValue;
                newObjectField.CopyToDW = objectFieldDetails.CopyToDW;
                newObjectField.AllowedInCustomerFieldsSettings = objectFieldDetails.AllowedInCustomerFieldsSettings;
                newObjectField.DisplayInDocumentReferences = objectFieldDetails.DisplayInDocumentReferences;
                newObjectField.AllowedInAirlineMessaging = objectFieldDetails.AllowedInAirlineMessaging;
                newObjectField.EnableFullscreenTextBox = objectFieldDetails.EnableFullscreenTextBox;
                newObjectField.DisplayInAutomationAsEnitity = objectFieldDetails.DisplayInAutomationAsEnitity;
                newObjectField.RecordType = objectFieldDetails.RecordType;
                newObjectField.AdditionalQuerySections = objectFieldDetails.AdditionalQuerySections;
                newObjectField.DisplayInRequiredFields = objectFieldDetails.DisplayInRequiredFields;
                newObjectField.ObjectFieldDataMapping = objectFieldDetails.ObjectFieldDataMapping;
                newObjectField.IsListFilter = objectFieldDetails.IsListFilter;
                newObjectField.LeftKey = objectFieldDetails.ThisKey;
                newObjectField.RightKey = objectFieldDetails.OtherKey;
                newObjectField.IsForeignKey = objectFieldDetails.IsForeignKey;
                newObjectField.ForeignEntity = objectFieldDetails.IsForeignKey ? objectFieldDetails.ForeignEntity : objectFieldDetails.IsMulti ? objectFieldDetails.MultiTableName : null;
                newObjectField.NavigationPropertyName = objectFieldDetails.NavigationPropertyName;
                newObjectField.ForMetaDataOnly = objectFieldDetails.NoMetaDataField;
                newObjectField.GeneratedComponentPath = objectFieldDetails.GeneratedComponentPath;
                objectFieldsRepository.Insert(newObjectField);
            }
            else
            {
                ObjectField updatedObjectField = tenantZeroObjectFields[objectFieldDetails.FieldName + objectFieldDetails.ObjectTableId];
                updatedObjectField.IsCustom = objectFieldDetails.IsCustom;
                updatedObjectField.ControlField1 = objectFieldDetails.ControlField1;
                updatedObjectField.ControlField2 = objectFieldDetails.ControlField2;
                updatedObjectField.ControlField3 = objectFieldDetails.ControlField3;
                updatedObjectField.IsRequired = objectFieldDetails.IsRequired;
                updatedObjectField.LookUpTableId = objectFieldDetails.LookUpTableId;
                updatedObjectField.MaxLength = objectFieldDetails.MaxLength;
                updatedObjectField.MinLength = objectFieldDetails.MinLength;
                updatedObjectField.ObjectTableId = objectFieldDetails.ObjectTableId;
                updatedObjectField.Tenant = 0;
                updatedObjectField.CanFilter = objectFieldDetails.CanFilter;
                updatedObjectField.DisplayOnly = objectFieldDetails.DisplayOnly;
                updatedObjectField.SystemRequired = objectFieldDetails.SystemRequired;
                updatedObjectField.SystemMaxLength = objectFieldDetails.SystemMaxLength;
                updatedObjectField.DisplayInList = objectFieldDetails.DisplayInList;
                updatedObjectField.ConverterName = objectFieldDetails.ConverterName;
                updatedObjectField.DataTemplateName = objectFieldDetails.DataTemplateName;
                updatedObjectField.MultiLine = objectFieldDetails.MultiLine;
                updatedObjectField.IsCustomFilter = objectFieldDetails.IsCustomFilter;
                updatedObjectField.Operator = objectFieldDetails.Operator;
                updatedObjectField.IsTimeFrameFilter = objectFieldDetails.IsTimeFrameFilter;
                updatedObjectField.DisplayInSearchWindowFilters = objectFieldDetails.DisplayInSearchWindowFilters;
                updatedObjectField.DisplayInSearchWindowList = objectFieldDetails.DisplayInSearchWindowList;
                updatedObjectField.PMPropertyPath = objectFieldDetails.PMPropertyPath;
                updatedObjectField.ListPropertyPath = objectFieldDetails.ListPropertyPath;
                updatedObjectField.DisplayInLookUpIndex = objectFieldDetails.DisplayInLookUpIndex;
                updatedObjectField.AutomaticField = objectFieldDetails.AutomaticField;
                updatedObjectField.UniqueField = objectFieldDetails.UniqueField;
                updatedObjectField.DisplayOnLookUp = objectFieldDetails.DisplayOnLookUp;
                updatedObjectField.DisplayOnLookUpLocal = objectFieldDetails.DisplayOnLookUpLocal;
                updatedObjectField.DisplayInSearchWindowFiltersIndex = objectFieldDetails.DisplayInSearchWindowFiltersIndex;
                updatedObjectField.DisplayInSearchWindowListIndex = objectFieldDetails.DisplayInSearchWindowListIndex;
                updatedObjectField.IsMulti = objectFieldDetails.IsMulti;
                updatedObjectField.MultiTableId = objectFieldDetails.MultiTableId;
                updatedObjectField.DependencyFilter1Value = objectFieldDetails.DependencyFilter1Value;
                updatedObjectField.DependencyFilter2Value = objectFieldDetails.DependencyFilter2Value;
                updatedObjectField.DependencyFilter3Value = objectFieldDetails.DependencyFilter3Value;
                updatedObjectField.DependencyFilter1Type = objectFieldDetails.DependencyFilter1Type;
                updatedObjectField.DependencyFilter2Type = objectFieldDetails.DependencyFilter2Type;
                updatedObjectField.DependencyFilter3Type = objectFieldDetails.DependencyFilter3Type;
                updatedObjectField.ValidForQuerySection1 = objectFieldDetails.ValidForQuerySection1;
                updatedObjectField.ValidForQuerySection2 = objectFieldDetails.ValidForQuerySection2;
                updatedObjectField.IsRestrictable = objectFieldDetails.IsRestrictable;
                updatedObjectField.DisplayInEntityVariables = objectFieldDetails.DisplayInEntityVariables;
                updatedObjectField.InActive = objectFieldDetails.InActive;
                updatedObjectField.DisplayInLookupColumnSize = objectFieldDetails.DisplayInLookupColumnSize;
                updatedObjectField.ColumnHeaderTemplateName = objectFieldDetails.ColumnHeaderTemplateName;
                updatedObjectField.CustomerPermissionTypeCode = objectFieldDetails.CustomerPermissionTypeCode;
                updatedObjectField.AgentPermissionTypeCode = objectFieldDetails.AgentPermissionTypeCode;
                updatedObjectField.CustomPickListCode = objectFieldDetails.CustomPickListCode;
                updatedObjectField.DataTypeCode = objectFieldDetails.FieldsDataType;
                updatedObjectField.NumberOfDigits = objectFieldDetails.NumberOfDigits;
                updatedObjectField.DigitsAfterPoint = objectFieldDetails.DigitsAfterPoint;
                updatedObjectField.DependencyFilter1IsList = objectFieldDetails.DependencyFilter1IsList;
                updatedObjectField.DependencyFilter2IsList = objectFieldDetails.DependencyFilter2IsList;
                updatedObjectField.DependencyFilter3IsList = objectFieldDetails.DependencyFilter3IsList;
                updatedObjectField.IsMaxLength = objectFieldDetails.IsMaxLength;
                updatedObjectField.HasTemplate = objectFieldDetails.HasTemplate;
                updatedObjectField.HtmlHeaderComponentName = objectFieldDetails.HtmlHeaderComponentName;
                updatedObjectField.HtmlHeaderComponentUrl = objectFieldDetails.HtmlHeaderComponentUrl;
                updatedObjectField.HtmlListComponentName = objectFieldDetails.HtmlListComponentName;
                updatedObjectField.HtmlListComponentUrl = objectFieldDetails.HtmlListComponentUrl;
                updatedObjectField.AllowedinAutomationConditions = objectFieldDetails.AllowedinAutomationConditions;
                updatedObjectField.AutomationEmailRecipient = objectFieldDetails.AutomationEmailRecipient;
                updatedObjectField.CanAutomateSetValue = objectFieldDetails.CanAutomateSetValue;
                updatedObjectField.CopyToDW = objectFieldDetails.CopyToDW;
                updatedObjectField.AllowedInCustomerFieldsSettings = objectFieldDetails.AllowedInCustomerFieldsSettings;
                updatedObjectField.DisplayInDocumentReferences = objectFieldDetails.DisplayInDocumentReferences;
                updatedObjectField.Code = objectFieldDetails.Code;
                updatedObjectField.AllowedInAirlineMessaging = objectFieldDetails.AllowedInAirlineMessaging;
                updatedObjectField.EnableFullscreenTextBox = objectFieldDetails.EnableFullscreenTextBox;
                updatedObjectField.DisplayInAutomationAsEnitity = objectFieldDetails.DisplayInAutomationAsEnitity;
                updatedObjectField.RecordType = objectFieldDetails.RecordType;
                updatedObjectField.AdditionalQuerySections = objectFieldDetails.AdditionalQuerySections;
                updatedObjectField.DisplayInRequiredFields = objectFieldDetails.DisplayInRequiredFields;
                updatedObjectField.ObjectFieldDataMapping = objectFieldDetails.ObjectFieldDataMapping;
                updatedObjectField.IsListFilter = objectFieldDetails.IsListFilter;
                updatedObjectField.LeftKey = objectFieldDetails.ThisKey;
                updatedObjectField.RightKey = objectFieldDetails.OtherKey;
                updatedObjectField.IsForeignKey = objectFieldDetails.IsForeignKey;
                updatedObjectField.ForeignEntity = objectFieldDetails.IsForeignKey ? objectFieldDetails.ForeignEntity : objectFieldDetails.IsMulti ? objectFieldDetails.MultiTableName : null;
                updatedObjectField.NavigationPropertyName = objectFieldDetails.NavigationPropertyName;
                updatedObjectField.ForMetaDataOnly = objectFieldDetails.NoMetaDataField;

                if (string.IsNullOrEmpty(objectFieldDetails.Code))
                {
                    updatedObjectField.Code = objectFieldDetails.FieldName;
                }
                if (!string.IsNullOrEmpty(objectFieldDetails.OldFieldName))
                {
                    updatedObjectField.FieldName = objectFieldDetails.FieldName;
                }

                if (string.IsNullOrEmpty(updatedObjectField.FieldCode))
                {
                    updatedObjectField.FieldCode = objectFieldDetails.ObjectTableName + "." + objectFieldDetails.FieldName;
                }

                if (objectFieldDetails.ObjectTableName == "Address")
                    return;

                if (!tenantZeroTextCodes.ContainsKey(objectFieldDetails.ObjectTableName + ".F." + objectFieldDetails.FullFieldLable + objectFieldDetails.Tenant + objectFieldDetails.ObjectTableId))
                {
                    TextCode objectFieldTextCode = new TextCode();
                    objectFieldTextCode.Code = objectFieldDetails.ObjectTableName + ".F." + objectFieldDetails.FullFieldLable;
                    objectFieldTextCode.DefaultText = objectFieldDetails.DefaultText;
                    objectFieldTextCode.Id = IdCounter.GetNumber("TextCode", objectFieldDetails.Tenant).ToString();
                    objectFieldTextCode.ObjectTableId = objectFieldDetails.ObjectTableId;
                    objectFieldTextCode.Tenant = 0;
                    objectFieldTextCode.TextCodeTypeCode = "F";
                    objectFieldTextCode.InActive = objectFieldDetails.InActive;
                    objectFieldTextCode.LocalDefaultText = TryConvertFromBase64(objectFieldDetails.FullLocalDefaultText);
                    textCodeRepository.Insert(objectFieldTextCode);
                }
                else
                {
                    TextCode updatedFullNameTextCode = tenantZeroTextCodes[objectFieldDetails.ObjectTableName + ".F." + objectFieldDetails.FullFieldLable + objectFieldDetails.Tenant + objectFieldDetails.ObjectTableId];
                    if (!updatedFullNameTextCode.IsSpellChecked)
                    {
                        updatedFullNameTextCode.DefaultText = objectFieldDetails.DefaultText;
                        updatedFullNameTextCode.InActive = objectFieldDetails.InActive;
                        updatedFullNameTextCode.LocalDefaultText = TryConvertFromBase64(objectFieldDetails.FullLocalDefaultText);
                        textCodeRepository.Update(updatedFullNameTextCode);
                    }
                }
                if (objectFieldDetails.ShortFieldLable != null)
                {

                    if (tenantZeroTextCodes.Keys.Contains(objectFieldDetails.ObjectTableName + ".F." + objectFieldDetails.ShortFieldLable + ".Short" + objectFieldDetails.Tenant + objectFieldDetails.ObjectTableId))
                    {

                        TextCode updatedShortNameTextCode = tenantZeroTextCodes[objectFieldDetails.ObjectTableName + ".F." + objectFieldDetails.ShortFieldLable + ".Short" + objectFieldDetails.Tenant + objectFieldDetails.ObjectTableId];
                        if (!updatedShortNameTextCode.IsSpellChecked)
                        {
                            updatedShortNameTextCode.DefaultText = objectFieldDetails.ShortFieldLableDefaultText;
                            updatedShortNameTextCode.InActive = objectFieldDetails.InActive;
                            updatedShortNameTextCode.LocalDefaultText = TryConvertFromBase64(objectFieldDetails.ShortLocalDefaultText);
                            if (updatedObjectField.ShortNameTextCodeId == null)
                            {
                                updatedObjectField.ShortNameTextCodeId = updatedShortNameTextCode.Id;
                            }
                            if (updatedObjectField.ShortNameTextCodeCode == null)
                            {
                                updatedObjectField.ShortNameTextCodeCode = updatedShortNameTextCode.Code;
                            }
                            textCodeRepository.Update(updatedShortNameTextCode);
                        }
                    }
                    else
                    {
                        TextCode updatedShortNameTextCode = new TextCode();
                        updatedShortNameTextCode.Code = objectFieldDetails.ObjectTableName + ".F." + objectFieldDetails.ShortFieldLable + ".Short";
                        updatedShortNameTextCode.DefaultText = objectFieldDetails.ShortFieldLableDefaultText;
                        updatedShortNameTextCode.Id = IdCounter.GetNumber("TextCode", objectFieldDetails.Tenant).ToString();
                        updatedShortNameTextCode.ObjectTableId = objectFieldDetails.ObjectTableId;
                        updatedShortNameTextCode.Tenant = 0;
                        updatedShortNameTextCode.TextCodeTypeCode = "F";
                        updatedShortNameTextCode.InActive = objectFieldDetails.InActive;
                        updatedShortNameTextCode.LocalDefaultText = TryConvertFromBase64(objectFieldDetails.ShortLocalDefaultText);
                        updatedObjectField.ShortNameTextCodeId = updatedShortNameTextCode.Id;
                        updatedObjectField.ShortNameTextCodeCode = updatedShortNameTextCode.Code;
                        textCodeRepository.Insert(updatedShortNameTextCode);
                    }
                }

                if (updatedObjectField.HelpTextCodeCode != null)
                {
                    if (tenantZeroTextCodes.ContainsKey((objectFieldDetails.ObjectTableName != null ? objectFieldDetails.ObjectTableName : objectFieldDetails.ValidForQuerySection1) + "." + (objectFieldDetails.HelpTextCode != null ? objectFieldDetails.HelpTextCode : objectFieldDetails.FullFieldLable) + "HelpText" + objectFieldDetails.Tenant + objectFieldDetails.ObjectTableId))
                    {
                        TextCode updatedHelpTextCode = tenantZeroTextCodes[(objectFieldDetails.ObjectTableName != null ? objectFieldDetails.ObjectTableName : objectFieldDetails.ValidForQuerySection1) + "." + (objectFieldDetails.HelpTextCode != null ? objectFieldDetails.HelpTextCode : objectFieldDetails.FullFieldLable) + "HelpText" + objectFieldDetails.Tenant + objectFieldDetails.ObjectTableId];

                        if (!updatedHelpTextCode.IsSpellChecked)
                        {
                            updatedHelpTextCode.DefaultText = (objectFieldDetails.HelpTextDefaultText != null ? objectFieldDetails.HelpTextDefaultText : string.Empty);
                            updatedHelpTextCode.InActive = objectFieldDetails.InActive;
                            updatedHelpTextCode.LocalDefaultText = TryConvertFromBase64(objectFieldDetails.HelpLocalDefaultText);
                            textCodeRepository.Update(updatedHelpTextCode);
                        }
                    }
                    else
                    {
                        TextCode helpTextTextCode = null;

                        if (!string.IsNullOrEmpty(objectFieldDetails.HelpTextCode))
                        {
                            helpTextTextCode = new TextCode();
                            helpTextTextCode.Code = objectFieldDetails.ObjectTableName + "." + objectFieldDetails.HelpTextCode + "HelpText";
                            helpTextTextCode.DefaultText = objectFieldDetails.HelpTextDefaultText;
                            helpTextTextCode.Id = IdCounter.GetNumber("TextCode", objectFieldDetails.Tenant).ToString();
                            helpTextTextCode.ObjectTableId = objectFieldDetails.ObjectTableId;
                            helpTextTextCode.Tenant = 0;
                            helpTextTextCode.TextCodeTypeCode = "H";
                            helpTextTextCode.InActive = objectFieldDetails.InActive;
                            helpTextTextCode.LocalDefaultText = TryConvertFromBase64(objectFieldDetails.HelpLocalDefaultText);
                            textCodeRepository.Insert(helpTextTextCode);
                            updatedObjectField.HelpTextCodeId = helpTextTextCode.Id;
                            updatedObjectField.HelpTextCodeCode = helpTextTextCode.Code;
                        }

                        else
                        {
                            helpTextTextCode = new TextCode();
                            helpTextTextCode.Code = (!string.IsNullOrEmpty(objectFieldDetails.ObjectTableName) ? objectFieldDetails.ObjectTableName : objectFieldDetails.ValidForQuerySection1) + "." + objectFieldDetails.FullFieldLable + "HelpText";
                            helpTextTextCode.DefaultText = objectFieldDetails.HelpTextDefaultText;
                            helpTextTextCode.Id = IdCounter.GetNumber("TextCode", objectFieldDetails.Tenant).ToString();
                            helpTextTextCode.ObjectTableId = objectFieldDetails.ObjectTableId;
                            helpTextTextCode.Tenant = 0;
                            helpTextTextCode.TextCodeTypeCode = "H";
                            helpTextTextCode.InActive = objectFieldDetails.InActive;
                            helpTextTextCode.LocalDefaultText = TryConvertFromBase64(objectFieldDetails.HelpLocalDefaultText);
                            textCodeRepository.Insert(helpTextTextCode);
                            updatedObjectField.HelpTextCodeId = helpTextTextCode.Id;
                            updatedObjectField.HelpTextCodeCode = helpTextTextCode.Code;
                        }
                    }
                }

                else
                {
                    TextCode helpTextTextCode = null;

                    if (!string.IsNullOrEmpty(objectFieldDetails.HelpTextCode))
                    {
                        helpTextTextCode = new TextCode();
                        helpTextTextCode.Code = objectFieldDetails.ObjectTableName + "." + objectFieldDetails.HelpTextCode + "HelpText";
                        helpTextTextCode.DefaultText = objectFieldDetails.HelpTextDefaultText;
                        helpTextTextCode.Id = IdCounter.GetNumber("TextCode", objectFieldDetails.Tenant).ToString();
                        helpTextTextCode.ObjectTableId = objectFieldDetails.ObjectTableId;
                        helpTextTextCode.Tenant = 0;
                        helpTextTextCode.TextCodeTypeCode = "H";
                        helpTextTextCode.InActive = objectFieldDetails.InActive;
                        helpTextTextCode.LocalDefaultText = TryConvertFromBase64(objectFieldDetails.HelpLocalDefaultText);
                        textCodeRepository.Insert(helpTextTextCode);
                        updatedObjectField.HelpTextCodeId = helpTextTextCode.Id;
                        updatedObjectField.HelpTextCodeCode = helpTextTextCode.Code;
                    }

                    else
                    {
                        helpTextTextCode = new TextCode();
                        helpTextTextCode.Code = (!string.IsNullOrEmpty(objectFieldDetails.ObjectTableName) ? objectFieldDetails.ObjectTableName : objectFieldDetails.ValidForQuerySection1) + "." + objectFieldDetails.FullFieldLable + "HelpText";
                        helpTextTextCode.DefaultText = objectFieldDetails.HelpTextDefaultText;
                        helpTextTextCode.Id = IdCounter.GetNumber("TextCode", objectFieldDetails.Tenant).ToString();
                        helpTextTextCode.ObjectTableId = objectFieldDetails.ObjectTableId;
                        helpTextTextCode.Tenant = 0;
                        helpTextTextCode.TextCodeTypeCode = "H";
                        helpTextTextCode.InActive = objectFieldDetails.InActive;
                        helpTextTextCode.LocalDefaultText = TryConvertFromBase64(objectFieldDetails.HelpLocalDefaultText);
                        textCodeRepository.Insert(helpTextTextCode);
                        updatedObjectField.HelpTextCodeId = helpTextTextCode.Id;
                        updatedObjectField.HelpTextCodeCode = helpTextTextCode.Code;
                    }
                }

                if (objectFieldDetails.ListFieldLable != null && (objectFieldDetails.DisplayInList || objectFieldDetails.DisplayOnLookUp || objectFieldDetails.DisplayOnLookUpLocal || objectFieldDetails.DisplayInSearchWindowList))
                {

                    if (tenantZeroTextCodes.Keys.Contains(objectFieldDetails.ObjectTableName + ".CH." + objectFieldDetails.ListFieldLable + objectFieldDetails.Tenant + objectFieldDetails.ObjectTableId))
                    {
                        TextCode updatedlistTextCode = tenantZeroTextCodes[objectFieldDetails.ObjectTableName + ".CH." + objectFieldDetails.ListFieldLable + objectFieldDetails.Tenant + objectFieldDetails.ObjectTableId];
                        if (!updatedlistTextCode.IsSpellChecked)
                        {
                            updatedlistTextCode.DefaultText = objectFieldDetails.ListLableDefaultText;
                            updatedlistTextCode.InActive = objectFieldDetails.InActive;
                            updatedlistTextCode.LocalDefaultText = TryConvertFromBase64(objectFieldDetails.ListLocalDefaultText);

                            if (updatedObjectField.ListTextCodeId == null)
                            {
                                updatedObjectField.ListTextCodeId = updatedlistTextCode.Id;
                            }
                            if (updatedObjectField.ListTextCodeCode == null)
                            {
                                updatedObjectField.ListTextCodeCode = updatedlistTextCode.Code;
                            }

                            textCodeRepository.Update(updatedlistTextCode);
                        }
                    }

                    else
                    {
                        TextCode updatedlistTextCode = new TextCode();
                        updatedlistTextCode.Code = objectFieldDetails.ObjectTableName + ".CH." + objectFieldDetails.ListFieldLable;
                        updatedlistTextCode.DefaultText = objectFieldDetails.ListLableDefaultText;
                        updatedlistTextCode.Id = IdCounter.GetNumber("TextCode", objectFieldDetails.Tenant).ToString();
                        updatedlistTextCode.ObjectTableId = objectFieldDetails.ObjectTableId;
                        updatedlistTextCode.Tenant = 0;
                        updatedlistTextCode.TextCodeTypeCode = "CH";
                        updatedlistTextCode.InActive = objectFieldDetails.InActive;
                        updatedObjectField.ListTextCodeId = updatedlistTextCode.Id;
                        updatedObjectField.ListTextCodeCode = updatedlistTextCode.Code;
                        updatedlistTextCode.LocalDefaultText = TryConvertFromBase64(objectFieldDetails.ListLocalDefaultText);
                        textCodeRepository.Insert(updatedlistTextCode);
                    }
                }


                if (updatedObjectField.IsCustomFilter)
                {
                    updatedObjectField.CanFilter = true;
                }

                updatedObjectField.GeneratedComponentPath = objectFieldDetails.GeneratedComponentPath;

                objectFieldsRepository.Update(updatedObjectField);

            }


        }
     
        public static ObjectTable GetObjectTable(string objectTableName, Dictionary<string, ObjectTable> tenantZeroObjectTables,int contextTenant)
        {
            if (objecttablesRepository == null)
            {
                objecttablesRepository = new Repository<ObjectTable>(contextTenant);
            }

            ObjectTable table = tenantZeroObjectTables.ContainsKey(objectTableName) ? tenantZeroObjectTables[objectTableName] : null;
            if (table == null)
            {
                 table = objecttablesRepository.GetMultiFromCache<ObjectTable>(
                cacheKey: $"GetObjectTable{objectTableName}-{0}", a => a.Name == objectTableName && a.Tenant == 0).FirstOrDefault();
            }

            return table;
        }
        public static void AddObjectField(ObjectFieldPM objectFieldDetails, Repository<TextCode> textCodeRepository, ObjectFieldRepository objectFieldsRepository, Dictionary<string, ObjectField> tenantZeroObjectFields, Dictionary<string, TextCode> tenantZeroTextCodes,int contextTenant=0)
        {
            var objecttablesRepository = new Repository<ObjectTable>(contextTenant);


            if (!String.IsNullOrEmpty(objectFieldDetails.ObjectTableName) && String.IsNullOrEmpty(objectFieldDetails.ObjectTableId))
            {
                var table = objecttablesRepository.GetMultiFromCache<ObjectTablePM>(
                    cacheKey: $"GetObjectTable{objectFieldDetails.ObjectTableName}-{0}", a => a.Name == objectFieldDetails.ObjectTableName && a.Tenant == 0).FirstOrDefault();
               objectFieldDetails.ObjectTableId = table?.Id;

            }
            if (!String.IsNullOrEmpty(objectFieldDetails.LookUpTableName) && String.IsNullOrEmpty(objectFieldDetails.LookUpTableId))
            {
                var table = objecttablesRepository.GetMultiFromCache<ObjectTablePM>(
                    cacheKey: $"GetObjectTable{objectFieldDetails.LookUpTableName}-{0}", a => a.Name == objectFieldDetails.LookUpTableName && a.Tenant == 0 ).FirstOrDefault();
                objectFieldDetails.LookUpTableId = table?.Id;

            }

            if (!String.IsNullOrEmpty(objectFieldDetails.MultiTableName) && String.IsNullOrEmpty(objectFieldDetails.MultiTableId))
            {
                var table = objecttablesRepository.GetMultiFromCache<ObjectTablePM>(
                                    cacheKey: $"GetObjectTable{objectFieldDetails.MultiTableName}", a => a.Name == objectFieldDetails.MultiTableName).FirstOrDefault(); 
                if (table != null)
                {
                    objectFieldDetails.MultiTableId = table.Id;
                }

            }
            if (!tenantZeroObjectFields.Keys.Contains(objectFieldDetails.FieldName + objectFieldDetails.ObjectTableId))
            {

                TextCode objectFieldTextCode = null;
                if (!tenantZeroTextCodes.Keys.Contains(objectFieldDetails.ObjectTableName + ".F." + objectFieldDetails.FullFieldLable + objectFieldDetails.Tenant + objectFieldDetails.ObjectTableId))
                {
                    objectFieldTextCode = new TextCode();
                    objectFieldTextCode.Code = objectFieldDetails.ObjectTableName + ".F." + objectFieldDetails.FullFieldLable;
                    objectFieldTextCode.DefaultText = objectFieldDetails.DefaultText;
                    objectFieldTextCode.Id = IdCounter.GetNumber("TextCode", objectFieldDetails.Tenant).ToString();
                    objectFieldTextCode.ObjectTableId = objectFieldDetails.ObjectTableId;
                    objectFieldTextCode.Tenant = 0;
                    objectFieldTextCode.TextCodeTypeCode = "F";
                    objectFieldTextCode.InActive = objectFieldDetails.InActive;
                    objectFieldTextCode.LocalDefaultText = TryConvertFromBase64(objectFieldDetails.FullLocalDefaultText);
                    textCodeRepository.Insert(objectFieldTextCode);
                }
                else
                {
                    objectFieldTextCode = tenantZeroTextCodes[objectFieldDetails.ObjectTableName + ".F." + objectFieldDetails.FullFieldLable + objectFieldDetails.Tenant + objectFieldDetails.ObjectTableId];
                    objectFieldTextCode.DefaultText = objectFieldDetails.DefaultText;
                    textCodeRepository.Update(objectFieldTextCode);
                }

                TextCode helpTextTextCode = null;
                TextCode listFieldLableTextCode = null;
                TextCode fullFieldTextCode = null;
                if (!string.IsNullOrEmpty(objectFieldDetails.HelpTextCode))
                {
                    if (!tenantZeroTextCodes.Keys.Contains(objectFieldDetails.ObjectTableName + ".F." + objectFieldDetails.HelpTextCode + "HelpText"))
                    {
                        helpTextTextCode = new TextCode();
                        helpTextTextCode.Code = objectFieldDetails.ObjectTableName + "." + objectFieldDetails.HelpTextCode + "HelpText";
                        helpTextTextCode.DefaultText = objectFieldDetails.HelpTextDefaultText;
                        helpTextTextCode.Id = IdCounter.GetNumber("TextCode", objectFieldDetails.Tenant).ToString();
                        helpTextTextCode.ObjectTableId = objectFieldDetails.ObjectTableId;
                        helpTextTextCode.Tenant = 0;
                        helpTextTextCode.TextCodeTypeCode = "H";
                        helpTextTextCode.InActive = objectFieldDetails.InActive;
                        helpTextTextCode.LocalDefaultText = TryConvertFromBase64(objectFieldDetails.HelpLocalDefaultText);
                        textCodeRepository.Insert(helpTextTextCode);
                    }
                    else
                    {
                        helpTextTextCode = tenantZeroTextCodes[objectFieldDetails.ObjectTableName + ".F." + objectFieldDetails.HelpTextCode + "HelpText"];
                        helpTextTextCode.DefaultText = objectFieldDetails.HelpTextDefaultText;
                        helpTextTextCode.LocalDefaultText = TryConvertFromBase64(objectFieldDetails.HelpLocalDefaultText);
                        textCodeRepository.Update(helpTextTextCode);
                    }
                }

                else
                {
                    if (!tenantZeroTextCodes.Keys.Contains((!string.IsNullOrEmpty(objectFieldDetails.ObjectTableName) ? objectFieldDetails.ObjectTableName : objectFieldDetails.ValidForQuerySection1) + "." + objectFieldDetails.FullFieldLable + "HelpText"))
                    {
                        helpTextTextCode = new TextCode();
                        helpTextTextCode.Code = (!string.IsNullOrEmpty(objectFieldDetails.ObjectTableName) ? objectFieldDetails.ObjectTableName : objectFieldDetails.ValidForQuerySection1) + "." + objectFieldDetails.FullFieldLable + "HelpText";
                        helpTextTextCode.DefaultText = null;
                        helpTextTextCode.Id = IdCounter.GetNumber("TextCode", objectFieldDetails.Tenant).ToString();
                        helpTextTextCode.ObjectTableId = objectFieldDetails.ObjectTableId;
                        helpTextTextCode.Tenant = 0;
                        helpTextTextCode.TextCodeTypeCode = "H";
                        helpTextTextCode.InActive = objectFieldDetails.InActive;
                        helpTextTextCode.LocalDefaultText = TryConvertFromBase64(objectFieldDetails.HelpLocalDefaultText);
                        textCodeRepository.Insert(helpTextTextCode);
                    }
                    else
                    {
                        helpTextTextCode = tenantZeroTextCodes[(!string.IsNullOrEmpty(objectFieldDetails.ObjectTableName) ? objectFieldDetails.ObjectTableName : objectFieldDetails.ValidForQuerySection1) + "." + objectFieldDetails.FullFieldLable + "HelpText"];
                        helpTextTextCode.DefaultText = objectFieldDetails.HelpTextDefaultText;
                        helpTextTextCode.LocalDefaultText = TryConvertFromBase64(objectFieldDetails.HelpLocalDefaultText);
                        helpTextTextCode.InActive = objectFieldDetails.InActive;
                        textCodeRepository.Update(helpTextTextCode);
                    }
                }

                if (!string.IsNullOrEmpty(objectFieldDetails.ShortFieldLable))
                {
                    if (!tenantZeroTextCodes.Keys.Contains(objectFieldDetails.ObjectTableName + ".F." + objectFieldDetails.ShortFieldLable + ".Short"))
                    {
                        fullFieldTextCode = new TextCode();
                        fullFieldTextCode.Code = objectFieldDetails.ObjectTableName + ".F." + objectFieldDetails.ShortFieldLable + ".Short";
                        fullFieldTextCode.DefaultText = objectFieldDetails.ShortFieldLableDefaultText;
                        fullFieldTextCode.Id = IdCounter.GetNumber("TextCode", objectFieldDetails.Tenant).ToString();
                        fullFieldTextCode.ObjectTableId = objectFieldDetails.ObjectTableId;
                        fullFieldTextCode.Tenant = 0;
                        fullFieldTextCode.TextCodeTypeCode = "F";
                        fullFieldTextCode.InActive = objectFieldDetails.InActive;
                        fullFieldTextCode.LocalDefaultText = TryConvertFromBase64(objectFieldDetails.ShortLocalDefaultText);
                        textCodeRepository.Insert(fullFieldTextCode);
                    }
                    else
                    {
                        fullFieldTextCode = tenantZeroTextCodes[objectFieldDetails.ObjectTableName + ".F." + objectFieldDetails.ShortFieldLable + ".Short"];
                        fullFieldTextCode.DefaultText = objectFieldDetails.ShortFieldLableDefaultText;
                        fullFieldTextCode.LocalDefaultText = TryConvertFromBase64(objectFieldDetails.ShortLocalDefaultText);
                        fullFieldTextCode.InActive = objectFieldDetails.InActive;
                        textCodeRepository.Update(fullFieldTextCode);
                    }
                }

                if (!string.IsNullOrEmpty(objectFieldDetails.ListFieldLable) && (objectFieldDetails.DisplayInList || objectFieldDetails.DisplayInSearchWindowList || objectFieldDetails.DisplayOnLookUp || objectFieldDetails.DisplayOnLookUpLocal))
                {
                    if (!tenantZeroTextCodes.Keys.Contains(objectFieldDetails.ObjectTableName + ".CH." + objectFieldDetails.ListFieldLable))
                    {
                        listFieldLableTextCode = new TextCode();
                        listFieldLableTextCode.Code = objectFieldDetails.ObjectTableName + ".CH." + objectFieldDetails.ListFieldLable;
                        listFieldLableTextCode.DefaultText = objectFieldDetails.ListLableDefaultText;
                        listFieldLableTextCode.Id = IdCounter.GetNumber("TextCode", objectFieldDetails.Tenant).ToString();
                        listFieldLableTextCode.ObjectTableId = objectFieldDetails.ObjectTableId;
                        listFieldLableTextCode.Tenant = 0;
                        listFieldLableTextCode.TextCodeTypeCode = "CH";
                        listFieldLableTextCode.InActive = objectFieldDetails.InActive;
                        listFieldLableTextCode.LocalDefaultText = TryConvertFromBase64(objectFieldDetails.ListLocalDefaultText);
                        textCodeRepository.Insert(listFieldLableTextCode);
                    }
                    else
                    {
                        listFieldLableTextCode = tenantZeroTextCodes[objectFieldDetails.ObjectTableName + ".CH." + objectFieldDetails.ListFieldLable];
                        listFieldLableTextCode.DefaultText = objectFieldDetails.ListLableDefaultText;
                        listFieldLableTextCode.LocalDefaultText = TryConvertFromBase64(objectFieldDetails.ListLocalDefaultText);
                        listFieldLableTextCode.InActive = objectFieldDetails.InActive;
                        textCodeRepository.Update(listFieldLableTextCode);
                    }

                }


                ObjectField newObjectField = new ObjectField();
                newObjectField.ControlField1 = objectFieldDetails.ControlField1;
                newObjectField.ControlField2 = objectFieldDetails.ControlField2;
                newObjectField.ControlField3 = objectFieldDetails.ControlField3;
                newObjectField.DataTypeCode = objectFieldDetails.FieldsDataType;
                newObjectField.DisplayOnLookUp = objectFieldDetails.DisplayOnLookUp;
                newObjectField.DisplayOnLookUpLocal = objectFieldDetails.DisplayOnLookUpLocal;
                newObjectField.FullNameTextCodeId = objectFieldTextCode.Id;
                newObjectField.FullNameTextCodeCode = objectFieldTextCode.Code;
                newObjectField.FieldName = objectFieldDetails.FieldName;
                newObjectField.ShortName = objectFieldDetails.ShortName;
                newObjectField.Code = objectFieldDetails.Code;
                if (string.IsNullOrEmpty(newObjectField.FieldCode))
                {
                    newObjectField.FieldCode = objectFieldDetails.ObjectTableName + "." + objectFieldDetails.FieldName; ;
                }
                    if (string.IsNullOrEmpty(objectFieldDetails.Code))
                {
                    newObjectField.Code = objectFieldDetails.FieldName;
                }
                if (helpTextTextCode != null)
                {
                    newObjectField.HelpTextCodeId = helpTextTextCode.Id;
                    newObjectField.HelpTextCodeCode = helpTextTextCode.Code;
                }
                if (listFieldLableTextCode != null)
                {
                    newObjectField.ListTextCodeId = listFieldLableTextCode.Id;
                    newObjectField.ListTextCodeCode = listFieldLableTextCode.Code;
                }
                newObjectField.Id = IdCounter.GetNumber("ObjectField", objectFieldDetails.Tenant).ToString();
                newObjectField.IsCustom = objectFieldDetails.IsCustom;
                newObjectField.LookUpTableId = objectFieldDetails.LookUpTableId;
                newObjectField.MaxLength = objectFieldDetails.MaxLength;
                newObjectField.MinLength = objectFieldDetails.MinLength;
                newObjectField.IsMaxLength = objectFieldDetails.IsMaxLength;
                newObjectField.IsRequired = objectFieldDetails.IsRequired;
                newObjectField.ObjectTableId = objectFieldDetails.ObjectTableId;
                newObjectField.Tenant = 0;
                newObjectField.CanFilter = objectFieldDetails.CanFilter;
                newObjectField.DisplayOnly = objectFieldDetails.DisplayOnly;
                if (objectFieldDetails.IsRequired)
                {
                    newObjectField.SystemRequired = objectFieldDetails.SystemRequired;
                }
                newObjectField.SystemMaxLength = newObjectField.MaxLength;
                newObjectField.DisplayInList = objectFieldDetails.DisplayInList;
                newObjectField.ConverterName = objectFieldDetails.ConverterName;
                newObjectField.DataTemplateName = objectFieldDetails.DataTemplateName;
                newObjectField.MultiLine = objectFieldDetails.MultiLine;
                newObjectField.IsCustomFilter = objectFieldDetails.IsCustomFilter;
                newObjectField.Operator = objectFieldDetails.Operator;
                newObjectField.IsTimeFrameFilter = objectFieldDetails.IsTimeFrameFilter;
                newObjectField.DisplayInSearchWindowFilters = objectFieldDetails.DisplayInSearchWindowFilters;
                newObjectField.DisplayInSearchWindowList = objectFieldDetails.DisplayInSearchWindowList;
                newObjectField.PMPropertyPath = objectFieldDetails.PMPropertyPath;
                newObjectField.ListPropertyPath = objectFieldDetails.ListPropertyPath;
                newObjectField.DisplayInLookUpIndex = objectFieldDetails.DisplayInLookUpIndex;
                newObjectField.AutomaticField = objectFieldDetails.AutomaticField;
                newObjectField.UniqueField = objectFieldDetails.UniqueField;
                newObjectField.ShortNameTextCodeId = fullFieldTextCode != null ? fullFieldTextCode.Id : null;
                newObjectField.ShortNameTextCodeCode = fullFieldTextCode != null ? fullFieldTextCode.Code : null;
                newObjectField.DisplayInSearchWindowFiltersIndex = objectFieldDetails.DisplayInSearchWindowFiltersIndex;
                newObjectField.DisplayInSearchWindowListIndex = objectFieldDetails.DisplayInSearchWindowListIndex;
                newObjectField.IsMulti = objectFieldDetails.IsMulti;
                newObjectField.MultiTableId = objectFieldDetails.MultiTableId;
                newObjectField.DependencyFilter1Value = objectFieldDetails.DependencyFilter1Value;
                newObjectField.DependencyFilter2Value = objectFieldDetails.DependencyFilter2Value;
                newObjectField.DependencyFilter3Value = objectFieldDetails.DependencyFilter3Value;
                newObjectField.DependencyFilter1Type = objectFieldDetails.DependencyFilter1Type;
                newObjectField.DependencyFilter2Type = objectFieldDetails.DependencyFilter2Type;
                newObjectField.DependencyFilter3Type = objectFieldDetails.DependencyFilter3Type;
                newObjectField.ValidForQuerySection1 = objectFieldDetails.ValidForQuerySection1;
                newObjectField.ValidForQuerySection2 = objectFieldDetails.ValidForQuerySection2;
                newObjectField.IsRestrictable = objectFieldDetails.IsRestrictable;
                newObjectField.DisplayInEntityVariables = objectFieldDetails.DisplayInEntityVariables;
                newObjectField.InActive = objectFieldDetails.InActive;
                newObjectField.DisplayInLookupColumnSize = objectFieldDetails.DisplayInLookupColumnSize;
                newObjectField.ColumnHeaderTemplateName = objectFieldDetails.ColumnHeaderTemplateName;
                newObjectField.CustomerPermissionTypeCode = objectFieldDetails.CustomerPermissionTypeCode;
                newObjectField.AgentPermissionTypeCode = objectFieldDetails.AgentPermissionTypeCode;
                newObjectField.CustomPickListCode = objectFieldDetails.CustomPickListCode;
                newObjectField.NumberOfDigits = objectFieldDetails.NumberOfDigits;
                newObjectField.DigitsAfterPoint = objectFieldDetails.DigitsAfterPoint;
                newObjectField.DependencyFilter1IsList = objectFieldDetails.DependencyFilter1IsList;
                newObjectField.DependencyFilter2IsList = objectFieldDetails.DependencyFilter2IsList;
                newObjectField.DependencyFilter3IsList = objectFieldDetails.DependencyFilter3IsList;
                newObjectField.IsMaxLength = objectFieldDetails.IsMaxLength;
                newObjectField.HasTemplate = objectFieldDetails.HasTemplate;
                newObjectField.HtmlHeaderComponentName = objectFieldDetails.HtmlHeaderComponentName;
                newObjectField.HtmlHeaderComponentUrl = objectFieldDetails.HtmlHeaderComponentUrl;
                newObjectField.HtmlListComponentName = objectFieldDetails.HtmlListComponentName;
                newObjectField.HtmlListComponentUrl = objectFieldDetails.HtmlListComponentUrl;
                newObjectField.AllowedinAutomationConditions = objectFieldDetails.AllowedinAutomationConditions;
                newObjectField.AutomationEmailRecipient = objectFieldDetails.AutomationEmailRecipient;
                newObjectField.CanAutomateSetValue = objectFieldDetails.CanAutomateSetValue;
                newObjectField.CopyToDW = objectFieldDetails.CopyToDW;

                newObjectField.AllowedInCustomerFieldsSettings = objectFieldDetails.AllowedInCustomerFieldsSettings;
                newObjectField.DisplayInDocumentReferences = objectFieldDetails.DisplayInDocumentReferences;
                newObjectField.AllowedInAirlineMessaging = objectFieldDetails.AllowedInAirlineMessaging;
                newObjectField.EnableFullscreenTextBox = objectFieldDetails.EnableFullscreenTextBox;
                newObjectField.DisplayInAutomationAsEnitity = objectFieldDetails.DisplayInAutomationAsEnitity;
                newObjectField.RecordType = objectFieldDetails.RecordType;
                newObjectField.AdditionalQuerySections = objectFieldDetails.AdditionalQuerySections;
                newObjectField.DisplayInRequiredFields = objectFieldDetails.DisplayInRequiredFields;
                newObjectField.ObjectFieldDataMapping = objectFieldDetails.ObjectFieldDataMapping;
                newObjectField.IsListFilter = objectFieldDetails.IsListFilter;

                newObjectField.LeftKey = objectFieldDetails.ThisKey;
                newObjectField.RightKey = objectFieldDetails.OtherKey;
                newObjectField.IsForeignKey = objectFieldDetails.IsForeignKey;
                newObjectField.ForeignEntity = objectFieldDetails.IsForeignKey ? objectFieldDetails.ForeignEntity : objectFieldDetails.IsMulti ? objectFieldDetails.MultiTableName : null;
                newObjectField.NavigationPropertyName = objectFieldDetails.NavigationPropertyName;
                newObjectField.ForMetaDataOnly = objectFieldDetails.NoMetaDataField;

                newObjectField.GeneratedComponentPath = objectFieldDetails.GeneratedComponentPath;
                objectFieldsRepository.Insert(newObjectField);
            }
            else
            {

                ObjectField updatedObjectField = tenantZeroObjectFields[objectFieldDetails.FieldName + objectFieldDetails.ObjectTableId];


                updatedObjectField.IsCustom = objectFieldDetails.IsCustom;
                updatedObjectField.ControlField1 = objectFieldDetails.ControlField1;
                updatedObjectField.ControlField2 = objectFieldDetails.ControlField2;
                updatedObjectField.ControlField3 = objectFieldDetails.ControlField3;
                updatedObjectField.IsRequired = objectFieldDetails.IsRequired;
                updatedObjectField.LookUpTableId = objectFieldDetails.LookUpTableId;
                updatedObjectField.MaxLength = objectFieldDetails.MaxLength;
                updatedObjectField.MinLength = objectFieldDetails.MinLength;
                updatedObjectField.ObjectTableId = objectFieldDetails.ObjectTableId;
                updatedObjectField.Tenant = 0;
                updatedObjectField.CanFilter = objectFieldDetails.CanFilter;
                updatedObjectField.DisplayOnly = objectFieldDetails.DisplayOnly;
                updatedObjectField.SystemRequired = objectFieldDetails.SystemRequired;
                updatedObjectField.SystemMaxLength = objectFieldDetails.SystemMaxLength;
                updatedObjectField.DisplayInList = objectFieldDetails.DisplayInList;
                updatedObjectField.ConverterName = objectFieldDetails.ConverterName;
                updatedObjectField.DataTemplateName = objectFieldDetails.DataTemplateName;
                updatedObjectField.MultiLine = objectFieldDetails.MultiLine;
                updatedObjectField.IsCustomFilter = objectFieldDetails.IsCustomFilter;
                updatedObjectField.Operator = objectFieldDetails.Operator;
                updatedObjectField.IsTimeFrameFilter = objectFieldDetails.IsTimeFrameFilter;
                updatedObjectField.DisplayInSearchWindowFilters = objectFieldDetails.DisplayInSearchWindowFilters;
                updatedObjectField.DisplayInSearchWindowList = objectFieldDetails.DisplayInSearchWindowList;
                updatedObjectField.PMPropertyPath = objectFieldDetails.PMPropertyPath;
                updatedObjectField.ListPropertyPath = objectFieldDetails.ListPropertyPath;
                updatedObjectField.DisplayInLookUpIndex = objectFieldDetails.DisplayInLookUpIndex;
                updatedObjectField.AutomaticField = objectFieldDetails.AutomaticField;
                updatedObjectField.UniqueField = objectFieldDetails.UniqueField;
                updatedObjectField.DisplayOnLookUp = objectFieldDetails.DisplayOnLookUp;
                updatedObjectField.DisplayOnLookUpLocal = objectFieldDetails.DisplayOnLookUpLocal;
                updatedObjectField.DisplayInSearchWindowFiltersIndex = objectFieldDetails.DisplayInSearchWindowFiltersIndex;
                updatedObjectField.DisplayInSearchWindowListIndex = objectFieldDetails.DisplayInSearchWindowListIndex;
                updatedObjectField.IsMulti = objectFieldDetails.IsMulti;
                updatedObjectField.MultiTableId = objectFieldDetails.MultiTableId;
                updatedObjectField.DependencyFilter1Value = objectFieldDetails.DependencyFilter1Value;
                updatedObjectField.DependencyFilter2Value = objectFieldDetails.DependencyFilter2Value;
                updatedObjectField.DependencyFilter3Value = objectFieldDetails.DependencyFilter3Value;
                updatedObjectField.DependencyFilter1Type = objectFieldDetails.DependencyFilter1Type;
                updatedObjectField.DependencyFilter2Type = objectFieldDetails.DependencyFilter2Type;
                updatedObjectField.DependencyFilter3Type = objectFieldDetails.DependencyFilter3Type;
                updatedObjectField.ValidForQuerySection1 = objectFieldDetails.ValidForQuerySection1;
                updatedObjectField.ValidForQuerySection2 = objectFieldDetails.ValidForQuerySection2;
                updatedObjectField.IsRestrictable = objectFieldDetails.IsRestrictable;
                updatedObjectField.DisplayInEntityVariables = objectFieldDetails.DisplayInEntityVariables;
                updatedObjectField.InActive = objectFieldDetails.InActive;
                updatedObjectField.DisplayInLookupColumnSize = objectFieldDetails.DisplayInLookupColumnSize;
                updatedObjectField.ColumnHeaderTemplateName = objectFieldDetails.ColumnHeaderTemplateName;
                updatedObjectField.CustomerPermissionTypeCode = objectFieldDetails.CustomerPermissionTypeCode;
                updatedObjectField.AgentPermissionTypeCode = objectFieldDetails.AgentPermissionTypeCode;
                updatedObjectField.CustomPickListCode = objectFieldDetails.CustomPickListCode;

                updatedObjectField.DataTypeCode = objectFieldDetails.FieldsDataType;
                updatedObjectField.NumberOfDigits = objectFieldDetails.NumberOfDigits;
                updatedObjectField.DigitsAfterPoint = objectFieldDetails.DigitsAfterPoint;

                updatedObjectField.DependencyFilter1IsList = objectFieldDetails.DependencyFilter1IsList;
                updatedObjectField.DependencyFilter2IsList = objectFieldDetails.DependencyFilter2IsList;
                updatedObjectField.DependencyFilter3IsList = objectFieldDetails.DependencyFilter3IsList;
                updatedObjectField.IsMaxLength = objectFieldDetails.IsMaxLength;

                updatedObjectField.HasTemplate = objectFieldDetails.HasTemplate;
                updatedObjectField.HtmlHeaderComponentName = objectFieldDetails.HtmlHeaderComponentName;
                updatedObjectField.HtmlHeaderComponentUrl = objectFieldDetails.HtmlHeaderComponentUrl;
                updatedObjectField.HtmlListComponentName = objectFieldDetails.HtmlListComponentName;
                updatedObjectField.HtmlListComponentUrl = objectFieldDetails.HtmlListComponentUrl;
                updatedObjectField.AllowedinAutomationConditions = objectFieldDetails.AllowedinAutomationConditions;
                updatedObjectField.AutomationEmailRecipient = objectFieldDetails.AutomationEmailRecipient;
                updatedObjectField.CanAutomateSetValue = objectFieldDetails.CanAutomateSetValue;
                updatedObjectField.CopyToDW = objectFieldDetails.CopyToDW;

                updatedObjectField.AllowedInCustomerFieldsSettings = objectFieldDetails.AllowedInCustomerFieldsSettings;
                updatedObjectField.DisplayInDocumentReferences = objectFieldDetails.DisplayInDocumentReferences;
                updatedObjectField.Code = objectFieldDetails.Code;
                updatedObjectField.AllowedInAirlineMessaging = objectFieldDetails.AllowedInAirlineMessaging;
                updatedObjectField.EnableFullscreenTextBox = objectFieldDetails.EnableFullscreenTextBox;
                updatedObjectField.DisplayInAutomationAsEnitity = objectFieldDetails.DisplayInAutomationAsEnitity;
                updatedObjectField.RecordType = objectFieldDetails.RecordType;
                updatedObjectField.AdditionalQuerySections = objectFieldDetails.AdditionalQuerySections;
                updatedObjectField.DisplayInRequiredFields = objectFieldDetails.DisplayInRequiredFields;
                updatedObjectField.ObjectFieldDataMapping = objectFieldDetails.ObjectFieldDataMapping;
                updatedObjectField.IsListFilter = objectFieldDetails.IsListFilter;

                updatedObjectField.LeftKey = objectFieldDetails.ThisKey;
                updatedObjectField.RightKey = objectFieldDetails.OtherKey;
                updatedObjectField.IsForeignKey = objectFieldDetails.IsForeignKey;
                updatedObjectField.ForeignEntity = objectFieldDetails.IsForeignKey ? objectFieldDetails.ForeignEntity : objectFieldDetails.IsMulti ? objectFieldDetails.MultiTableName : null;
                updatedObjectField.NavigationPropertyName = objectFieldDetails.NavigationPropertyName;
                updatedObjectField.ForMetaDataOnly = objectFieldDetails.NoMetaDataField;

                if (string.IsNullOrEmpty(objectFieldDetails.Code))
                {
                    updatedObjectField.Code = objectFieldDetails.FieldName;
                }
                if (!string.IsNullOrEmpty(objectFieldDetails.OldFieldName))
                {
                    updatedObjectField.FieldName = objectFieldDetails.FieldName;
                }

                if (string.IsNullOrEmpty(updatedObjectField.FieldCode))
                {
                    updatedObjectField.FieldCode = objectFieldDetails.ObjectTableName + "." + objectFieldDetails.FieldName;
                }

                if (objectFieldDetails.ObjectTableName == "Address")
                    return;

				if (!tenantZeroTextCodes.ContainsKey(objectFieldDetails.ObjectTableName + ".F." + objectFieldDetails.FullFieldLable + objectFieldDetails.Tenant + objectFieldDetails.ObjectTableId))
				{
					TextCode objectFieldTextCode = new TextCode();
					objectFieldTextCode.Code = objectFieldDetails.ObjectTableName + ".F." + objectFieldDetails.FullFieldLable;
					objectFieldTextCode.DefaultText = objectFieldDetails.DefaultText;
					objectFieldTextCode.Id = IdCounter.GetNumber("TextCode", objectFieldDetails.Tenant).ToString();
					objectFieldTextCode.ObjectTableId = objectFieldDetails.ObjectTableId;
					objectFieldTextCode.Tenant = 0;
					objectFieldTextCode.TextCodeTypeCode = "F";
					objectFieldTextCode.InActive = objectFieldDetails.InActive;
					objectFieldTextCode.LocalDefaultText = TryConvertFromBase64(objectFieldDetails.FullLocalDefaultText);
					textCodeRepository.Insert(objectFieldTextCode);
				}
				else
				{
					TextCode updatedFullNameTextCode = tenantZeroTextCodes[objectFieldDetails.ObjectTableName + ".F." + objectFieldDetails.FullFieldLable + objectFieldDetails.Tenant + objectFieldDetails.ObjectTableId];
					if (!updatedFullNameTextCode.IsSpellChecked)
					{
						updatedFullNameTextCode.DefaultText = objectFieldDetails.DefaultText;
						updatedFullNameTextCode.InActive = objectFieldDetails.InActive;
						updatedFullNameTextCode.LocalDefaultText = TryConvertFromBase64(objectFieldDetails.FullLocalDefaultText);
						textCodeRepository.Update(updatedFullNameTextCode);
					}
				}
                if (objectFieldDetails.ShortFieldLable != null)
                {

                    if (tenantZeroTextCodes.Keys.Contains(objectFieldDetails.ObjectTableName + ".F." + objectFieldDetails.ShortFieldLable + ".Short" + objectFieldDetails.Tenant + objectFieldDetails.ObjectTableId))
                    {

                        TextCode updatedShortNameTextCode = tenantZeroTextCodes[objectFieldDetails.ObjectTableName + ".F." + objectFieldDetails.ShortFieldLable + ".Short" + objectFieldDetails.Tenant + objectFieldDetails.ObjectTableId];
                        if (!updatedShortNameTextCode.IsSpellChecked)
                        {
                            updatedShortNameTextCode.DefaultText = objectFieldDetails.ShortFieldLableDefaultText;
                            updatedShortNameTextCode.InActive = objectFieldDetails.InActive;
                            updatedShortNameTextCode.LocalDefaultText = objectFieldDetails.ShortLocalDefaultText;
                            if (updatedObjectField.ShortNameTextCodeId == null)
                            {
                                updatedObjectField.ShortNameTextCodeId = updatedShortNameTextCode.Id;
                            }
                            if (updatedObjectField.ShortNameTextCodeCode == null)
                            {
                                updatedObjectField.ShortNameTextCodeCode = updatedShortNameTextCode.Code;
                            }
                            textCodeRepository.Update(updatedShortNameTextCode);
                        }
                    }
                    else
                    {
                        TextCode updatedShortNameTextCode = new TextCode();
                        updatedShortNameTextCode.Code = objectFieldDetails.ObjectTableName + ".F." + objectFieldDetails.ShortFieldLable + ".Short";
                        updatedShortNameTextCode.DefaultText = objectFieldDetails.ShortFieldLableDefaultText;
                        updatedShortNameTextCode.Id = IdCounter.GetNumber("TextCode", objectFieldDetails.Tenant).ToString();
                        updatedShortNameTextCode.ObjectTableId = objectFieldDetails.ObjectTableId;
                        updatedShortNameTextCode.Tenant = 0;
                        updatedShortNameTextCode.TextCodeTypeCode = "F";
                        updatedShortNameTextCode.InActive = objectFieldDetails.InActive;
                        updatedShortNameTextCode.LocalDefaultText = TryConvertFromBase64(objectFieldDetails.ShortLocalDefaultText);
                        updatedObjectField.ShortNameTextCodeId = updatedShortNameTextCode.Id;
                        updatedObjectField.ShortNameTextCodeCode = updatedShortNameTextCode.Code;
                        textCodeRepository.Insert(updatedShortNameTextCode);
                    }
                }

                if (updatedObjectField.HelpTextCodeCode != null)
                {
                    if (tenantZeroTextCodes.ContainsKey((objectFieldDetails.ObjectTableName != null ? objectFieldDetails.ObjectTableName : objectFieldDetails.ValidForQuerySection1) + "." + (objectFieldDetails.HelpTextCode != null ? objectFieldDetails.HelpTextCode : objectFieldDetails.FullFieldLable) + "HelpText" + objectFieldDetails.Tenant + objectFieldDetails.ObjectTableId))
                    {
                        TextCode updatedHelpTextCode = tenantZeroTextCodes[(objectFieldDetails.ObjectTableName != null ? objectFieldDetails.ObjectTableName : objectFieldDetails.ValidForQuerySection1) + "." + (objectFieldDetails.HelpTextCode != null ? objectFieldDetails.HelpTextCode : objectFieldDetails.FullFieldLable) + "HelpText" + objectFieldDetails.Tenant + objectFieldDetails.ObjectTableId];

                        if (!updatedHelpTextCode.IsSpellChecked)
                        {
                            updatedHelpTextCode.DefaultText = (objectFieldDetails.HelpTextDefaultText != null ? objectFieldDetails.HelpTextDefaultText : string.Empty);
                            updatedHelpTextCode.InActive = objectFieldDetails.InActive;
                            updatedHelpTextCode.LocalDefaultText = TryConvertFromBase64(objectFieldDetails.HelpLocalDefaultText);
                            textCodeRepository.Update(updatedHelpTextCode);
                        }
                    }
                    else
                    {
                        TextCode helpTextTextCode = null;

                        if (!string.IsNullOrEmpty(objectFieldDetails.HelpTextCode))
                        {
                            helpTextTextCode = new TextCode();
                            helpTextTextCode.Code = objectFieldDetails.ObjectTableName + "." + objectFieldDetails.HelpTextCode + "HelpText";
                            helpTextTextCode.DefaultText = objectFieldDetails.HelpTextDefaultText;
                            helpTextTextCode.Id = IdCounter.GetNumber("TextCode", objectFieldDetails.Tenant).ToString();
                            helpTextTextCode.ObjectTableId = objectFieldDetails.ObjectTableId;
                            helpTextTextCode.Tenant = 0;
                            helpTextTextCode.TextCodeTypeCode = "H";
                            helpTextTextCode.InActive = objectFieldDetails.InActive;
                            helpTextTextCode.LocalDefaultText = TryConvertFromBase64(objectFieldDetails.HelpLocalDefaultText);
                            textCodeRepository.Insert(helpTextTextCode);
                            updatedObjectField.HelpTextCodeId = helpTextTextCode.Id;
                            updatedObjectField.HelpTextCodeCode = helpTextTextCode.Code;
                        }

                        else
                        {
                            helpTextTextCode = new TextCode();
                            helpTextTextCode.Code = (!string.IsNullOrEmpty(objectFieldDetails.ObjectTableName) ? objectFieldDetails.ObjectTableName : objectFieldDetails.ValidForQuerySection1) + "." + objectFieldDetails.FullFieldLable + "HelpText";
                            helpTextTextCode.DefaultText = objectFieldDetails.HelpTextDefaultText;
                            helpTextTextCode.Id = IdCounter.GetNumber("TextCode", objectFieldDetails.Tenant).ToString();
                            helpTextTextCode.ObjectTableId = objectFieldDetails.ObjectTableId;
                            helpTextTextCode.Tenant = 0;
                            helpTextTextCode.TextCodeTypeCode = "H";
                            helpTextTextCode.InActive = objectFieldDetails.InActive;
                            helpTextTextCode.LocalDefaultText = TryConvertFromBase64(objectFieldDetails.HelpLocalDefaultText);
                            textCodeRepository.Insert(helpTextTextCode);
                            updatedObjectField.HelpTextCodeId = helpTextTextCode.Id;
                            updatedObjectField.HelpTextCodeCode = helpTextTextCode.Code;
                        }
                    }
                }

                else
                {
                    TextCode helpTextTextCode = null;

                    if (!string.IsNullOrEmpty(objectFieldDetails.HelpTextCode))
                    {
                        helpTextTextCode = new TextCode();
                        helpTextTextCode.Code = objectFieldDetails.ObjectTableName + "." + objectFieldDetails.HelpTextCode + "HelpText";
                        helpTextTextCode.DefaultText = objectFieldDetails.HelpTextDefaultText;
                        helpTextTextCode.Id = IdCounter.GetNumber("TextCode", objectFieldDetails.Tenant).ToString();
                        helpTextTextCode.ObjectTableId = objectFieldDetails.ObjectTableId;
                        helpTextTextCode.Tenant = 0;
                        helpTextTextCode.TextCodeTypeCode = "H";
                        helpTextTextCode.InActive = objectFieldDetails.InActive;
                        helpTextTextCode.LocalDefaultText = TryConvertFromBase64(objectFieldDetails.HelpLocalDefaultText);
                        textCodeRepository.Insert(helpTextTextCode);
                        updatedObjectField.HelpTextCodeId = helpTextTextCode.Id;
                        updatedObjectField.HelpTextCodeCode = helpTextTextCode.Code;
                    }

                    else
                    {
                        helpTextTextCode = new TextCode();
                        helpTextTextCode.Code = (!string.IsNullOrEmpty(objectFieldDetails.ObjectTableName) ? objectFieldDetails.ObjectTableName : objectFieldDetails.ValidForQuerySection1) + "." + objectFieldDetails.FullFieldLable + "HelpText";
                        helpTextTextCode.DefaultText = objectFieldDetails.HelpTextDefaultText;
                        helpTextTextCode.Id = IdCounter.GetNumber("TextCode", objectFieldDetails.Tenant).ToString();
                        helpTextTextCode.ObjectTableId = objectFieldDetails.ObjectTableId;
                        helpTextTextCode.Tenant = 0;
                        helpTextTextCode.TextCodeTypeCode = "H";
                        helpTextTextCode.InActive = objectFieldDetails.InActive;
                        helpTextTextCode.LocalDefaultText = TryConvertFromBase64(objectFieldDetails.HelpLocalDefaultText);
                        textCodeRepository.Insert(helpTextTextCode);
                        updatedObjectField.HelpTextCodeId = helpTextTextCode.Id;
                        updatedObjectField.HelpTextCodeCode = helpTextTextCode.Code;
                    }
                }

                if (objectFieldDetails.ListFieldLable != null && (objectFieldDetails.DisplayInList || objectFieldDetails.DisplayOnLookUp || objectFieldDetails.DisplayOnLookUpLocal || objectFieldDetails.DisplayInSearchWindowList))
                {

                    if (tenantZeroTextCodes.Keys.Contains(objectFieldDetails.ObjectTableName + ".CH." + objectFieldDetails.ListFieldLable + objectFieldDetails.Tenant + objectFieldDetails.ObjectTableId))
                    {
                        TextCode updatedlistTextCode = tenantZeroTextCodes[objectFieldDetails.ObjectTableName + ".CH." + objectFieldDetails.ListFieldLable + objectFieldDetails.Tenant + objectFieldDetails.ObjectTableId];
                        if (!updatedlistTextCode.IsSpellChecked)
                        {
                            updatedlistTextCode.DefaultText = objectFieldDetails.ListLableDefaultText;
                            updatedlistTextCode.InActive = objectFieldDetails.InActive;
                            updatedlistTextCode.LocalDefaultText = TryConvertFromBase64(objectFieldDetails.ListLocalDefaultText);

                            if (updatedObjectField.ListTextCodeId == null)
                            {
                                updatedObjectField.ListTextCodeId = updatedlistTextCode.Id;
                            }
                            if (updatedObjectField.ListTextCodeCode == null)
                            {
                                updatedObjectField.ListTextCodeCode = updatedlistTextCode.Code;
                            }

                            textCodeRepository.Update(updatedlistTextCode);
                        }
                    }

                    else
                    {
                        TextCode updatedlistTextCode = new TextCode();
                        updatedlistTextCode.Code = objectFieldDetails.ObjectTableName + ".CH." + objectFieldDetails.ListFieldLable;
                        updatedlistTextCode.DefaultText = objectFieldDetails.ListLableDefaultText;
                        updatedlistTextCode.Id = IdCounter.GetNumber("TextCode", objectFieldDetails.Tenant).ToString();
                        updatedlistTextCode.ObjectTableId = objectFieldDetails.ObjectTableId;
                        updatedlistTextCode.Tenant = 0;
                        updatedlistTextCode.TextCodeTypeCode = "CH";
                        updatedlistTextCode.InActive = objectFieldDetails.InActive;
                        updatedObjectField.ListTextCodeId = updatedlistTextCode.Id;
                        updatedObjectField.ListTextCodeCode = updatedlistTextCode.Code;
                        updatedlistTextCode.LocalDefaultText = TryConvertFromBase64(objectFieldDetails.ListLocalDefaultText);
                        textCodeRepository.Insert(updatedlistTextCode);
                    }
                }


                if (updatedObjectField.IsCustomFilter)
                {
                    updatedObjectField.CanFilter = true;
                }

                updatedObjectField.GeneratedComponentPath = objectFieldDetails.GeneratedComponentPath;

                objectFieldsRepository.Update(updatedObjectField);

            }

        }

        public static void AddObjectField(ObjectFieldPM objectFieldDetails, Repository<TextCode> textCodeRepository, Repository<ObjectField> objectFieldsRepository, Dictionary<string, ObjectField> tenantZeroObjectFields, Dictionary<string, TextCode> tenantZeroTextCodes, Dictionary<string, ObjectTable> tenantZeroObjectTables, List<ObjectField> addedFields, List<TextCode> addedTextCodes,int contextTenant = 0)
        {


            if (!String.IsNullOrEmpty(objectFieldDetails.ObjectTableName) && String.IsNullOrEmpty(objectFieldDetails.ObjectTableId))
            {
                ObjectTable table = GetObjectTable(objectFieldDetails.ObjectTableName, tenantZeroObjectTables,contextTenant);

                objectFieldDetails.ObjectTableId = table.Id;

            }
            if (!String.IsNullOrEmpty(objectFieldDetails.LookUpTableName) && String.IsNullOrEmpty(objectFieldDetails.LookUpTableId))
            {
                ObjectTable table = GetObjectTable(objectFieldDetails.LookUpTableName, tenantZeroObjectTables, contextTenant);
                if (table != null)
                {
                    objectFieldDetails.LookUpTableId = table.Id;
                }

            }

            if (!String.IsNullOrEmpty(objectFieldDetails.MultiTableName) && String.IsNullOrEmpty(objectFieldDetails.MultiTableId))
            {
                ObjectTable table = GetObjectTable(objectFieldDetails.MultiTableName, tenantZeroObjectTables, contextTenant);
                if (table != null)
                {
                    objectFieldDetails.MultiTableId = table.Id;
                }

            }


            TextCode objectFieldTextCode = null;

            objectFieldTextCode = new TextCode();
            objectFieldTextCode.Code = objectFieldDetails.ObjectTableName + ".F." + objectFieldDetails.FullFieldLable;
            objectFieldTextCode.DefaultText = objectFieldDetails.DefaultText;
            objectFieldTextCode.Id =IdCounter.GetIdWithIdsRange("TextCode",100, contextTenant).ToString();
            objectFieldTextCode.ObjectTableId = objectFieldDetails.ObjectTableId;
            objectFieldTextCode.Tenant = 0;
            objectFieldTextCode.TextCodeTypeCode = "F";
            objectFieldTextCode.InActive = objectFieldDetails.InActive;
            objectFieldTextCode.LocalDefaultText = objectFieldDetails.FullLocalDefaultText;
            addedTextCodes.Add(objectFieldTextCode);

            TextCode helpTextTextCode = null;
            TextCode listFieldLableTextCode = null;
            TextCode fullFieldTextCode = null;
            if (!string.IsNullOrEmpty(objectFieldDetails.HelpTextCode))
            {

                helpTextTextCode = new TextCode();
                helpTextTextCode.Code = objectFieldDetails.ObjectTableName + "." + objectFieldDetails.HelpTextCode + "HelpText";
                helpTextTextCode.DefaultText = objectFieldDetails.HelpTextDefaultText;
                helpTextTextCode.Id = IdCounter.GetIdWithIdsRange("TextCode",100, contextTenant).ToString();
                helpTextTextCode.ObjectTableId = objectFieldDetails.ObjectTableId;
                helpTextTextCode.Tenant = 0;
                helpTextTextCode.TextCodeTypeCode = "H";
                helpTextTextCode.InActive = objectFieldDetails.InActive;
                helpTextTextCode.LocalDefaultText = objectFieldDetails.HelpLocalDefaultText;
                addedTextCodes.Add(helpTextTextCode);


            }

            else
            {

                helpTextTextCode = new TextCode();
                helpTextTextCode.Code = (!string.IsNullOrEmpty(objectFieldDetails.ObjectTableName) ? objectFieldDetails.ObjectTableName : objectFieldDetails.ValidForQuerySection1) + "." + objectFieldDetails.FullFieldLable + "HelpText";
                helpTextTextCode.DefaultText = null;
                helpTextTextCode.Id = IdCounter.GetIdWithIdsRange("TextCode",100, contextTenant).ToString();
                helpTextTextCode.ObjectTableId = objectFieldDetails.ObjectTableId;
                helpTextTextCode.Tenant = 0;
                helpTextTextCode.TextCodeTypeCode = "H";
                helpTextTextCode.InActive = objectFieldDetails.InActive;
                helpTextTextCode.LocalDefaultText = objectFieldDetails.HelpLocalDefaultText;
                addedTextCodes.Add(helpTextTextCode);


            }

            if (!string.IsNullOrEmpty(objectFieldDetails.ShortFieldLable))
            {

                fullFieldTextCode = new TextCode();
                fullFieldTextCode.Code = objectFieldDetails.ObjectTableName + ".F." + objectFieldDetails.ShortFieldLable + ".Short";
                fullFieldTextCode.DefaultText = objectFieldDetails.ShortFieldLableDefaultText;
                fullFieldTextCode.Id = IdCounter.GetIdWithIdsRange("TextCode",100, contextTenant).ToString();
                fullFieldTextCode.ObjectTableId = objectFieldDetails.ObjectTableId;
                fullFieldTextCode.Tenant = 0;
                fullFieldTextCode.TextCodeTypeCode = "F";
                fullFieldTextCode.InActive = objectFieldDetails.InActive;
                fullFieldTextCode.LocalDefaultText = objectFieldDetails.ShortLocalDefaultText;
                addedTextCodes.Add(fullFieldTextCode);

            }

            if (!string.IsNullOrEmpty(objectFieldDetails.ListFieldLable) && (objectFieldDetails.DisplayInList || objectFieldDetails.DisplayInSearchWindowList || objectFieldDetails.DisplayOnLookUp || objectFieldDetails.DisplayOnLookUpLocal))
            {

                listFieldLableTextCode = new TextCode();
                listFieldLableTextCode.Code = objectFieldDetails.ObjectTableName + ".CH." + objectFieldDetails.ListFieldLable;
                listFieldLableTextCode.DefaultText = objectFieldDetails.ListLableDefaultText;
                listFieldLableTextCode.Id = IdCounter.GetIdWithIdsRange("TextCode",100, objectFieldDetails.Tenant).ToString();
                listFieldLableTextCode.ObjectTableId = objectFieldDetails.ObjectTableId;
                listFieldLableTextCode.Tenant = 0;
                listFieldLableTextCode.TextCodeTypeCode = "CH";
                listFieldLableTextCode.InActive = objectFieldDetails.InActive;
                listFieldLableTextCode.LocalDefaultText = objectFieldDetails.ListLocalDefaultText;
                addedTextCodes.Add(listFieldLableTextCode);


            }


            ObjectField newObjectField = new ObjectField();
            newObjectField.ControlField1 = objectFieldDetails.ControlField1;
            newObjectField.ControlField2 = objectFieldDetails.ControlField2;
            newObjectField.ControlField3 = objectFieldDetails.ControlField3;
            newObjectField.DataTypeCode = objectFieldDetails.FieldsDataType;
            newObjectField.DisplayOnLookUp = objectFieldDetails.DisplayOnLookUp;
            newObjectField.DisplayOnLookUpLocal = objectFieldDetails.DisplayOnLookUpLocal;
            newObjectField.FullNameTextCodeId = objectFieldTextCode.Id;
            newObjectField.FullNameTextCodeCode = objectFieldTextCode.Code;
            newObjectField.FieldName = objectFieldDetails.FieldName;
            newObjectField.ShortName = objectFieldDetails.ShortName;
            newObjectField.Code = objectFieldDetails.Code;
            if (string.IsNullOrEmpty(newObjectField.FieldCode))
            {
                newObjectField.FieldCode = objectFieldDetails.ObjectTableName + "." + objectFieldDetails.FieldName; ;
            }
            if (string.IsNullOrEmpty(objectFieldDetails.Code))
            {
                newObjectField.Code = objectFieldDetails.FieldName;
            }
            if (helpTextTextCode != null)
            {
                newObjectField.HelpTextCodeId = helpTextTextCode.Id;
                newObjectField.HelpTextCodeCode = helpTextTextCode.Code;
            }
            if (listFieldLableTextCode != null)
            {
                newObjectField.ListTextCodeId = listFieldLableTextCode.Id;
                newObjectField.ListTextCodeCode = listFieldLableTextCode.Code;
            }
            newObjectField.Id = IdCounter.GetIdWithIdsRange("ObjectField",100, contextTenant).ToString();
            newObjectField.IsCustom = objectFieldDetails.IsCustom;
            newObjectField.LookUpTableId = objectFieldDetails.LookUpTableId;

            newObjectField.MaxLength = objectFieldDetails.MaxLength;
            newObjectField.MinLength = objectFieldDetails.MinLength;
            newObjectField.IsMaxLength = objectFieldDetails.IsMaxLength;
            newObjectField.IsRequired = objectFieldDetails.IsRequired;
            newObjectField.ObjectTableId = objectFieldDetails.ObjectTableId;
            newObjectField.Tenant = 0;
            newObjectField.CanFilter = objectFieldDetails.CanFilter;
            newObjectField.DisplayOnly = objectFieldDetails.DisplayOnly;
            if (objectFieldDetails.IsRequired)
            {
                newObjectField.SystemRequired = objectFieldDetails.SystemRequired;
            }
            newObjectField.SystemMaxLength = newObjectField.MaxLength;
            newObjectField.DisplayInList = objectFieldDetails.DisplayInList;
            newObjectField.ConverterName = objectFieldDetails.ConverterName;
            newObjectField.DataTemplateName = objectFieldDetails.DataTemplateName;
            newObjectField.MultiLine = objectFieldDetails.MultiLine;
            newObjectField.IsCustomFilter = objectFieldDetails.IsCustomFilter;
            newObjectField.Operator = objectFieldDetails.Operator;
            newObjectField.IsTimeFrameFilter = objectFieldDetails.IsTimeFrameFilter;
            newObjectField.DisplayInSearchWindowFilters = objectFieldDetails.DisplayInSearchWindowFilters;
            newObjectField.DisplayInSearchWindowList = objectFieldDetails.DisplayInSearchWindowList;
            newObjectField.PMPropertyPath = objectFieldDetails.PMPropertyPath;
            newObjectField.ListPropertyPath = objectFieldDetails.ListPropertyPath;
            newObjectField.DisplayInLookUpIndex = objectFieldDetails.DisplayInLookUpIndex;
            newObjectField.AutomaticField = objectFieldDetails.AutomaticField;
            newObjectField.UniqueField = objectFieldDetails.UniqueField;
            newObjectField.ShortNameTextCodeId = fullFieldTextCode != null ? fullFieldTextCode.Id : null;
            newObjectField.ShortNameTextCodeCode = fullFieldTextCode != null ? fullFieldTextCode.Code : null;
            newObjectField.DisplayInSearchWindowFiltersIndex = objectFieldDetails.DisplayInSearchWindowFiltersIndex;
            newObjectField.DisplayInSearchWindowListIndex = objectFieldDetails.DisplayInSearchWindowListIndex;
            newObjectField.IsMulti = objectFieldDetails.IsMulti;
            newObjectField.MultiTableId = objectFieldDetails.MultiTableId;
            newObjectField.DependencyFilter1Value = objectFieldDetails.DependencyFilter1Value;
            newObjectField.DependencyFilter2Value = objectFieldDetails.DependencyFilter2Value;
            newObjectField.DependencyFilter3Value = objectFieldDetails.DependencyFilter3Value;
            newObjectField.DependencyFilter1Type = objectFieldDetails.DependencyFilter1Type;
            newObjectField.DependencyFilter2Type = objectFieldDetails.DependencyFilter2Type;
            newObjectField.DependencyFilter3Type = objectFieldDetails.DependencyFilter3Type;
            newObjectField.ValidForQuerySection1 = objectFieldDetails.ValidForQuerySection1;
            newObjectField.ValidForQuerySection2 = objectFieldDetails.ValidForQuerySection2;
            newObjectField.IsRestrictable = objectFieldDetails.IsRestrictable;
            newObjectField.DisplayInEntityVariables = objectFieldDetails.DisplayInEntityVariables;
            newObjectField.InActive = objectFieldDetails.InActive;
            newObjectField.DisplayInLookupColumnSize = objectFieldDetails.DisplayInLookupColumnSize;
            newObjectField.ColumnHeaderTemplateName = objectFieldDetails.ColumnHeaderTemplateName;
            newObjectField.CustomerPermissionTypeCode = objectFieldDetails.CustomerPermissionTypeCode;
            newObjectField.AgentPermissionTypeCode = objectFieldDetails.AgentPermissionTypeCode;
            newObjectField.CustomPickListCode = objectFieldDetails.CustomPickListCode;
            newObjectField.NumberOfDigits = objectFieldDetails.NumberOfDigits;
            newObjectField.DigitsAfterPoint = objectFieldDetails.DigitsAfterPoint;
            newObjectField.DependencyFilter1IsList = objectFieldDetails.DependencyFilter1IsList;
            newObjectField.DependencyFilter2IsList = objectFieldDetails.DependencyFilter2IsList;
            newObjectField.DependencyFilter3IsList = objectFieldDetails.DependencyFilter3IsList;
            newObjectField.IsMaxLength = objectFieldDetails.IsMaxLength;
            newObjectField.HasTemplate = objectFieldDetails.HasTemplate;
            newObjectField.HtmlHeaderComponentName = objectFieldDetails.HtmlHeaderComponentName;
            newObjectField.HtmlHeaderComponentUrl = objectFieldDetails.HtmlHeaderComponentUrl;
            newObjectField.HtmlListComponentName = objectFieldDetails.HtmlListComponentName;
            newObjectField.HtmlListComponentUrl = objectFieldDetails.HtmlListComponentUrl;
            newObjectField.AllowedinAutomationConditions = objectFieldDetails.AllowedinAutomationConditions;
            newObjectField.AutomationEmailRecipient = objectFieldDetails.AutomationEmailRecipient;
            newObjectField.CanAutomateSetValue = objectFieldDetails.CanAutomateSetValue;
            newObjectField.CopyToDW = objectFieldDetails.CopyToDW;
            newObjectField.AllowedInCustomerFieldsSettings = objectFieldDetails.AllowedInCustomerFieldsSettings;
            newObjectField.DisplayInDocumentReferences = objectFieldDetails.DisplayInDocumentReferences;
            newObjectField.AllowedInAirlineMessaging = objectFieldDetails.AllowedInAirlineMessaging;
            newObjectField.EnableFullscreenTextBox = objectFieldDetails.EnableFullscreenTextBox;
            newObjectField.DisplayInAutomationAsEnitity = objectFieldDetails.DisplayInAutomationAsEnitity;
            newObjectField.RecordType = objectFieldDetails.RecordType;
            newObjectField.AdditionalQuerySections = objectFieldDetails.AdditionalQuerySections;
            newObjectField.DisplayInRequiredFields = objectFieldDetails.DisplayInRequiredFields;
            newObjectField.ObjectFieldDataMapping = objectFieldDetails.ObjectFieldDataMapping;
            newObjectField.IsListFilter = objectFieldDetails.IsListFilter;
            newObjectField.LeftKey = objectFieldDetails.ThisKey;
            newObjectField.RightKey = objectFieldDetails.OtherKey;
            newObjectField.IsForeignKey = objectFieldDetails.IsForeignKey;
            newObjectField.ForeignEntity = objectFieldDetails.IsForeignKey ? objectFieldDetails.ForeignEntity : objectFieldDetails.IsMulti ? objectFieldDetails.MultiTableName : null;
            newObjectField.NavigationPropertyName = objectFieldDetails.NavigationPropertyName;
            newObjectField.ForMetaDataOnly = objectFieldDetails.NoMetaDataField;
            newObjectField.GeneratedComponentPath = objectFieldDetails.GeneratedComponentPath;
            addedFields.Add(newObjectField);

        }


        public static string TryConvertFromBase64(string input)
        {
            try
            {
                if (input == null)
                {
                    return null;
                }
                if (input.StartsWith("BS64:") || input.StartsWith("\"BS64:"))
                {

                    return ConvertFromBase64(input);


                }
                return input;

            }
            catch (FormatException)
            {
                return input;
            }
        }

        private static string ConvertFromBase64(string input)
        {
            string backUp = input;
            try
            {
                input = input.Trim('\"');
                input = input.Substring(5);//REMOVE BS64:
                byte[] data = Convert.FromBase64String(input);
                string decodedString = Encoding.UTF8.GetString(data);
                decodedString = decodedString.Trim('\"');
                return decodedString;

            }
            catch (FormatException)
            {
                return backUp;
            }

        }

    }
    
}
