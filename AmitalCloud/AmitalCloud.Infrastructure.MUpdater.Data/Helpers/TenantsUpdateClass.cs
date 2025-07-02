

using AmitalCloud.Infrastructure.Application.EntityQueryServices;
using AmitalCloud.Infrastructure.Data.Context;
using AmitalCloud.Infrastructure.Data.Counters;
using AmitalCloud.Infrastructure.Data.Helpers;
using AmitalCloud.Infrastructure.Data.Repositories;
using AmitalCloud.Infrastructure.Domain.DataContracts;
using AmitalCloud.Infrastructure.Domain.EntityPMs;
using AmitalCloud.Infrastructure.Domain.Helpers;
using AmitalCloud.Infrastructure.Model.EntityClasses;
using AmitalCloud.Infrastructure.Model.Interfaces;
using AmitalCloud.Infrastructure.MUpdater.Data.DataUpdate;
using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.Zip;
using System.Diagnostics;
using System.Transactions;

namespace AmitalCloud.Infrastructure.MUpdater.Data.Helpers
{

    public class TenantsUpdateClass
    {

        private static PerformanceTimerLogger performanceTimerLogger = new PerformanceTimerLogger();

        public static void UpdateDataForTenant(int tenant, string message, bool runOldCode = false, bool multiDB = false)
        {
            try
            {
                if (tenant == 0 || multiDB)
                {
                    IAmitalCloudContext context = AmitalCloudContext.GetContext(tenant);

                    #region

                    Stopwatch stopWatch = new Stopwatch();
                    stopWatch.Start();

                    switch (message.ToLower())
                    {                     
                        case "shipment":
                            {
                                UpdateShipmentAndMasterModules(context, true);

                                break;
                            }
                        
                        case "invoice":
                            {
                                //UpdateInvoiceModule(context, true);
                                break;
                            }
                            
                        case "infrastructure":
                            {
                                UpdateInfrasturtureAndLogModules(context, true);
                                break;
                            }
                        
                    }

                    if (SettingUtil.DeploymentStage.IsDBStage(SettingUtil.DeploymentStage.Development) || AmitalCloudSettings.IsCostomsDeploy)
                    {
                        using (TransactionScope scope = TransactionFactory.GetNewTransaction())
                        {
                            Repository<GlobalTenant> globalTenantRepository = new Repository<GlobalTenant>(tenant);
                            GlobalTenant globaltenant = globalTenantRepository.GetSingle(x=> x.Id == tenant);
                            globaltenant.Version = globaltenant.Version + 1;
                            globaltenant.LastUpdateDate = DateTime.Now;
                            globalTenantRepository.Update(globaltenant);
                            globalTenantRepository.SubmitChanges();
                            scope.Complete();
                        }


                    }

                    stopWatch.Stop();
                    TimeSpan ts = stopWatch.Elapsed;

                    AzureLog.SaveLogsInStorage("(" + message + ")" + " Update Tenant 0 Elapsed Time : " + ts.ToString(), "P", DateTime.Now, "", "", 0, null, null, null);

                    Console.WriteLine("Updating All closed tables history ...");
                    TableLastUpdateClass.UpdateAllClosedTablesHistory(tenant);

                    Console.WriteLine("Updateing System metadata history ...");
                    TableLastUpdateClass.UpdateSystemMetaDataHistory();

                    #endregion
                }
               
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{ex.Message} - Exiting function with Exception");
                throw;
            }

        }
       
        private static void UpdateInfrasturtureAndLogModules(IAmitalCloudContext context, bool runPostDeleteProcedure)
        {
            try
            {
                InfrastructureUpdateClass modelUpdateClass = new InfrastructureUpdateClass();
                modelUpdateClass.LoadObjectTablesMetadata(context, runPostDeleteProcedure);
                performanceTimerLogger.LogMessage("Generated" + ",InfrastructureUpdateClass");
                UpdateSystemLogsModule(context, runPostDeleteProcedure);
                UpdateAutoCreatedUpdateClass(context, runPostDeleteProcedure);
                UpdateGlobalModelUpdateClass(context, runPostDeleteProcedure);

            }
            catch (Exception ex)
            {
                Console.WriteLine($"{ex.Message} - Exiting function with Exception");
                throw;
            }

        }

        private static void UpdateSystemLogsModule(IAmitalCloudContext context, bool runPostDeleteProcedure)
        {
            try
            {
                LogsModelUpdateClass systemLogsModelUpdateClass = new LogsModelUpdateClass();
                systemLogsModelUpdateClass.LoadObjectTablesMetadata(context, runPostDeleteProcedure);

                performanceTimerLogger.LogMessage("Generated" + ",SystemLogsModelUpdateClass");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{ex.Message} - Exiting function with Exception");
                throw;
            }

        }

        private static void UpdateGlobalModelUpdateClass(IAmitalCloudContext context, bool runPostDeleteProcedure)
        {
            try
            {
                GlobalModelUpdateClass systemLogsModelUpdateClass = new GlobalModelUpdateClass();
                 systemLogsModelUpdateClass.LoadObjectTablesMetadata(context, runPostDeleteProcedure);

                performanceTimerLogger.LogMessage("Generated" + ",GlobalModelUpdateClass");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{ex.Message} - Exiting function with Exception");
                throw;
            }

        }

        private static void UpdateAutoCreatedUpdateClass(IAmitalCloudContext context, bool runPostDeleteProcedure)
        {
            try
            {
                AutoCreatedUpdateClass systemLogsModelUpdateClass = new AutoCreatedUpdateClass();
                systemLogsModelUpdateClass.LoadObjectTablesMetadata(context, runPostDeleteProcedure);

                performanceTimerLogger.LogMessage("Generated" + ",AutoCreatedUpdateClass");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{ex.Message} - Exiting function with Exception");
                throw;
            }

        }


        private static void UpdateShipmentAndMasterModules(IAmitalCloudContext context, bool runPostDeleteProcedure)
        {
            try
            {
                
                ShipmentsModelUpdateClass shipmentModelUpdateClass = new ShipmentsModelUpdateClass();
                shipmentModelUpdateClass.LoadObjectTablesMetadata(context, runPostDeleteProcedure);

                performanceTimerLogger.LogMessage("Generated" + ",ShipmentsModelUpdateClass");

                MasterModelUpdateClass masterModelUpdateClass = new MasterModelUpdateClass();
                masterModelUpdateClass.LoadObjectTablesMetadata(context, runPostDeleteProcedure);

                performanceTimerLogger.LogMessage("Generated" + ",MasterModelUpdateClass");
                
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{ex.Message} - Exiting function with Exception");
                throw;
            }

        }


        #region DataBackUp

        public static void BuildObjectTablesZipFilesData(bool includeCustoms = false, int tenant = 0)
        {
            try
            {
                Repository<ObjectField> objectFieldsRepository = new Repository<ObjectField>(tenant);
                ObjectTableQueryService objectTabelQuery = new ObjectTableQueryService(tenant);
                Repository<ObjectTable> objectTabelRepository = new ObjectTableRepository(tenant);
                Repository<TextCode> textCodeRepository = new Repository<TextCode>(tenant);
                List<ObjectTable> ObjectTableList = null;
                ObjectTableList = objectTabelRepository.GetAll(0).Where(t => !t.Name.Contains("Customs.")).ToList();
                ObjectTableList = ObjectTableList.Where(t => t.HashString != null).ToList();
                List<TextCode> textCodePMLists = textCodeRepository.GetMulti(x=> x.Tenant ==0 && !x.InActive);
               
                var objectFieldLists = new ObjectFieldQueryService(0).GetMultiFromCache($"objectFields{0}", a => a.Tenant == 0 && !a.InActive, "ObjectTable_LookUpTable,FullNameTextCode,ShortNameTextCode,ListTextCode,HelpTextCodeFK,ObjectTable_MultiTable", a => new ObjectFieldPM(a)
                {
                    IsMaxLength = a.IsMaxLength,
                    AutomaticField = a.AutomaticField,
                    CanFilter = a.CanFilter,
                    ConverterName = a.ConverterName,
                    DataTemplateName = a.DataTemplateName,
                    DataTypeCode = a.DataTypeCode,
                    DependencyFilter1Type = a.DependencyFilter1Type,
                    DependencyFilter1Value = a.DependencyFilter1Value,
                    DependencyFilter2Type = a.DependencyFilter2Type,
                    DependencyFilter2Value = a.DependencyFilter2Value,
                    DisplayInList = a.DisplayInList,
                    DisplayInLookUpIndex = a.DisplayInLookUpIndex,
                    DisplayInSearchWindowFilters = a.DisplayInSearchWindowFilters,
                    DisplayInSearchWindowFiltersIndex = a.DisplayInSearchWindowFiltersIndex,
                    DisplayInSearchWindowList = a.DisplayInSearchWindowList,
                    DisplayInSearchWindowListIndex = a.DisplayInSearchWindowListIndex,
                    DisplayOnLookUp = a.DisplayOnLookUp,
                    DisplayOnly = a.DisplayOnly,
                    FullNameTextCodeId = a.FullNameTextCodeId,
                    FieldName = a.FieldName,
                    ShortName = a.ShortName,
                    ShortNameTextCodeId = a.ShortNameTextCodeId,
                    HelpTextCodeId = a.HelpTextCodeId,
                    Id = a.Id,
                    IsCustom = a.IsCustom,
                    IsCustomFilter = a.IsCustomFilter,
                    IsMulti = a.IsMulti,
                    IsRequired = a.IsRequired,
                    IsTimeFrameFilter = a.IsTimeFrameFilter,
                    ListTextCodeId = a.ListTextCodeId,
                    ListPropertyPath = a.ListPropertyPath,
                    LookUpControlName = a.LookUpControlName,
                    LookUpTableId = a.LookUpTableId,
                    MaxLength = a.MaxLength,
                    MinLength = a.MinLength,
                    MultiLine = a.MultiLine,
                    MultiTableId = a.MultiTableId,
                    ObjectTableId = a.ObjectTableId,
                    ObjectTableName = a.ObjectTable.Name,
                    Operator = a.Operator,
                    PMPropertyPath = a.PMPropertyPath,
                    SystemMaxLength = a.SystemMaxLength,
                    SystemRequired = a.SystemRequired,
                    Tenant = a.Tenant,
                    UniqueField = a.UniqueField,
                    ObjectTable_LookUpTableName = a.ObjectTable_LookUpTable != null ? a.ObjectTable_LookUpTable.Name : null,
                    FullNameTextCodeDefaultText = a.FullNameTextCode != null ? a.FullNameTextCode.DefaultText : null,
                    ShortNameTextCodeDefaultText = a.ShortNameTextCode != null ? a.ShortNameTextCode.DefaultText : null,
                    FullNameTextCodeCode = a.FullNameTextCodeCode,
                    ShortNameTextCodeCode = a.ShortNameTextCodeCode,
                    HelpTextCodeCode = a.HelpTextCodeCode,
                    ListTextCodeCode = a.ListTextCodeCode,
                    MultiTableName = a.ObjectTable_MultiTable != null ? a.ObjectTable_MultiTable.Name : null,
                    ListTextCodeDefaultText = a.ListTextCode != null ? a.ListTextCode.DefaultText : null,
                    HelpTextDefaultText = a.HelpTextCodeFK != null ? a.HelpTextCodeFK.DefaultText : null,
                    ValidForQuerySection2 = a.ValidForQuerySection2,
                    ValidForQuerySection1 = a.ValidForQuerySection1,
                    IsRestrictable = a.IsRestrictable,
                    DisplayInEntityVariables = a.DisplayInEntityVariables,
                    DigitsAfterPoint = a.DigitsAfterPoint,
                    TextCase = a.TextCase,
                    SearchFields = a.SearchFields,
                    DisplayInLookupColumnSize = a.DisplayInLookupColumnSize,
                    ColumnHeaderTemplateName = a.ColumnHeaderTemplateName,
                    TenantZeroIsRequired = a.IsRequired,
                    TenantZeroMaxLength = a.MaxLength,
                    TenantZeroMinLength = a.MinLength,
                    DisplayLongName = a.DisplayLongName,
                    AgentPermissionTypeCode = a.AgentPermissionTypeCode,
                    CustomerPermissionTypeCode = a.CustomerPermissionTypeCode,
                    ControlField1 = a.ControlField1,
                    ControlField2 = a.ControlField2,
                    CustomPickListCode = a.CustomPickListCode,
                    NumberOfDigits = a.NumberOfDigits,
                    DependencyFilter1IsList = a.DependencyFilter1IsList,
                    DependencyFilter2IsList = a.DependencyFilter2IsList,
                    FullNameTextCodeLocalDefaultText = a.FullNameTextCode != null ? a.FullNameTextCode.LocalDefaultText : null,
                    AllowedinAutomationConditions = a.AllowedinAutomationConditions,
                    AutomationEmailRecipient = a.AutomationEmailRecipient,
                    AllowedInAirlineMessaging = a.AllowedInAirlineMessaging,
                    CanAutomateSetValue = a.CanAutomateSetValue,
                    HtmlHeaderComponentUrl = a.HtmlHeaderComponentUrl,
                    HtmlListComponentUrl = a.HtmlListComponentUrl,
                    HtmlHeaderComponentName = a.HtmlHeaderComponentName,
                    HtmlListComponentName = a.HtmlListComponentName,
                    HasTemplate = a.HasTemplate,
                    GeneratedComponentPath = a.GeneratedComponentPath,
                    AllowedInCustomerFieldsSettings = a.AllowedInCustomerFieldsSettings,
                    DisplayInDocumentReferences = a.DisplayInDocumentReferences,
                    Code = a.Code,
                    ControlField3 = a.ControlField3,
                    DependencyFilter3Value = a.DependencyFilter3Value,
                    DependencyFilter3Type = a.DependencyFilter3Type,
                    DependencyFilter3IsList = a.DependencyFilter3IsList,
                    CopyToDW = a.CopyToDW,
                    DisplayOnLookUpLocal = a.DisplayOnLookUpLocal,
                    EnableFullscreenTextBox = a.EnableFullscreenTextBox,
                    RecordType = a.RecordType,
                    DisplayInAutomationAsEnitity = a.DisplayInAutomationAsEnitity,
                    FieldCode = a.FieldCode,
                    AdditionalQuerySections = a.AdditionalQuerySections,
                    DisplayInRequiredFields = a.DisplayInRequiredFields,
                    LeftKey = a.LeftKey,
                    RightKey = a.RightKey,
                    IsForeignKey = a.IsForeignKey,
                    ForeignEntity = a.ForeignEntity,
                    NavigationPropertyName = a.NavigationPropertyName,
                }).ToList();
                Dictionary<string, byte[]> cachedObjectFieldsJosnByte = new Dictionary<string, byte[]>();
                Dictionary<string, byte[]> cachedTextCodesJosnByte = new Dictionary<string, byte[]>();
                Dictionary<string, byte[]> cachedCloseTableJosnByte = new Dictionary<string, byte[]>();


                foreach (ObjectTable objectTable in ObjectTableList)
                {
                    List<ObjectFieldPM> fieldsList = objectFieldLists.Where(d => d.ObjectTableId == objectTable.Id).ToList();
                    if (fieldsList != null)
                    {
                        var josn = AmitalCloudXmlSerializer.SerializeObjectToJosnStringMax(fieldsList);
                        var buffer = System.Text.Encoding.UTF8.GetBytes(josn);
                        cachedObjectFieldsJosnByte.Add(objectTable.Name, buffer);
                    }

                    List<TextCode> textcodes = new List<TextCode>();
                    if (objectTable.Name == "General")
                        textcodes = textCodePMLists.Where(d => d.ObjectTableId == objectTable.Id || ((d.TextCodeTypeCode == "T" || d.Code.Contains(".O.TableDescription") || d.Code.Contains(".F.SearchFields") ) && d.ObjectTableId != objectTable.Id)).ToList();
                    else
                        textcodes = textCodePMLists.Where(d => d.ObjectTableId == objectTable.Id).ToList();


                    if (textcodes != null)
                    {
                        var josn = AmitalCloudXmlSerializer.SerializeObjectToJosnString(textcodes);
                        var buffer = System.Text.Encoding.UTF8.GetBytes(josn);
                        cachedTextCodesJosnByte.Add(objectTable.Name, buffer);
                    }

                    if (objectTable.IsClosed && objectTable.CacheOnClient)
                    {
                        //TODO:  Convert close table data to JSON format and insert to the cache                      
                    }



                }

                foreach (ObjectTable objectTable in ObjectTableList)
                {
                    if (objectTable.DBTableName.Contains("LeadDocumentType"))
                    {
                    }
                    List<string> tableNames = new List<string>();
                    Dictionary<string, byte[]> dataList = new Dictionary<string, byte[]>();
                    //Object Field
                    byte[] bytejosn = GetByteDataByKey(cachedObjectFieldsJosnByte, objectTable.Name);
                    if (bytejosn != null) dataList.Add(objectTable.Name + "_" + "ObjectFields", CompressionFileData(objectTable.Name + "_" + "Fields", bytejosn));

                    //Text Code
                    bytejosn = GetByteDataByKey(cachedTextCodesJosnByte, objectTable.Name);
                    if (bytejosn != null) dataList.Add(objectTable.Name + "_" + "TextCodes", CompressionFileData(objectTable.Name + "_" + "Codes", bytejosn));

                    tableNames.Add(objectTable.Name);
                    List<ObjectFieldPM> fieldsList = objectFieldLists.Where(d => d.ObjectTableId == objectTable.Id && (d.DataTypeCode == "LookUp" || d.IsMulti)).ToList();

                    foreach (ObjectFieldPM field in fieldsList)
                    {
                        string tablename = field.IsMulti ? field.MultiTableName : field.ObjectTable_LookUpTableName;
                        if (!tableNames.Contains(tablename))
                        {
                            //Object Field
                            bytejosn = GetByteDataByKey(cachedObjectFieldsJosnByte, tablename);
                            if (bytejosn != null) dataList.Add(tablename + "_" + "ObjectFields", CompressionFileData(tablename + "_" + "Fields", bytejosn));

                            //Text Code
                            bytejosn = GetByteDataByKey(cachedTextCodesJosnByte, tablename);
                            if (bytejosn != null) dataList.Add(tablename + "_" + "TextCodes", CompressionFileData(tablename + "_" + "Codes", bytejosn));

                            //CloseTable
                            if (field.DataTypeCode == "LookUp")
                            {
                                ObjectTable table = ObjectTableList.Where(d => d.Id == field.LookUpTableId).FirstOrDefault();
                                if (table != null && table.IsClosed)
                                {
                                    bytejosn = GetByteDataByKey(cachedCloseTableJosnByte, table.Name);
                                    if (bytejosn != null) dataList.Add(table.Name + "_" + "ClosedData", CompressionFileData(table.Name + "_" + "Closed", bytejosn));//+ "_" + "Closed"
                                }
                            }
                            tableNames.Add(tablename);
                        }
                    }
                    if (objectTable.IsClosed)
                    {
                        bytejosn = GetByteDataByKey(cachedCloseTableJosnByte, objectTable.Name);

                        if (bytejosn != null) dataList.Add(objectTable.Name + "_" + "ClosedData", CompressionFileData(objectTable.Name + "_" + "Closed", bytejosn));
                    }
                    objectTable.EntityResource = CompressionData(objectTable.Name, dataList);
                    objectTable.EntityResourceLastUpdate = DateTime.UtcNow;
                    objectTabelRepository.Update(objectTable);
                }

                objectTabelRepository.SubmitChanges();
               TableLastUpdateClass.UpdateSystemMetaDataHistory();

            }
           
            catch (Exception ex)
            {
                Console.WriteLine($"{ex.Message} Exiting function with Exception");
                throw;
            }

        }

        public static byte[] CompressionData(string listKey, Dictionary<string, byte[]> dataBackList)
        {
            MemoryStream outputMemStream = new MemoryStream();
            ZipOutputStream zipStream = new ZipOutputStream(outputMemStream);

            zipStream.SetLevel(3);
            byte[] bytes = null;
            foreach (string key in dataBackList.Keys)
            {
                var newEntry = new ZipEntry(key + ".zip");
                newEntry.DateTime = DateTime.Now;

                zipStream.PutNextEntry(newEntry);

                bytes = dataBackList[key];

                MemoryStream inStream = new MemoryStream(bytes);
                long inStreamLength = inStream.Length;
                if (inStreamLength < 200)
                {
                    inStreamLength = 200;
                }

                StreamUtils.Copy(inStream, zipStream, new byte[inStreamLength]);
                inStream.Close();
                zipStream.CloseEntry();

            }

            zipStream.IsStreamOwner = false;
            zipStream.Close();
            outputMemStream.Position = 0;
            return outputMemStream.ToArray();

        }

        public static byte[] CompressionFileData(string fileName, byte[] fileData)
        {
            if (ICSharpCode.SharpZipLib.Zip.ZipStrings.CodePage == 1)
                ICSharpCode.SharpZipLib.Zip.ZipStrings.CodePage = 437;
            MemoryStream outputMemStream = new MemoryStream();
            ZipOutputStream zipStream = new ZipOutputStream(outputMemStream);

            zipStream.SetLevel(3);


            var newEntry = new ZipEntry(fileName + ".json");
            newEntry.DateTime = DateTime.Now;
            ICSharpCode.SharpZipLib.Zip.ZipStrings.CodePage = 437;
            zipStream.PutNextEntry(newEntry);

            MemoryStream inStream = new MemoryStream(fileData);
            long inStreamLength = inStream.Length;
            if (inStreamLength < 200)
            {
                inStreamLength = 200;
            }

            StreamUtils.Copy(inStream, zipStream, new byte[inStreamLength]);
            inStream.Close();
            zipStream.CloseEntry();



            zipStream.IsStreamOwner = false;
            zipStream.Close();
            outputMemStream.Position = 0;

            return outputMemStream.ToArray();

        }

        private static byte[] GetByteDataByKey(Dictionary<string, byte[]> list, string key)
        {
            byte[] bytetable = null;
            if (list != null)
            {
                bytetable = list.Where(d => d.Key == key).Select(d => d.Value).FirstOrDefault();
            }
            return bytetable;
        }
        #endregion       
    }
}
