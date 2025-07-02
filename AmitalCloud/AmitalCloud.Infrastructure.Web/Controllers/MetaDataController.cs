using AmitalCloud.Infrastructure.Application.EntityQueryServices;
using AmitalCloud.Infrastructure.Application.Helpers;
using AmitalCloud.Infrastructure.Data.Helpers;
using AmitalCloud.Infrastructure.Data.Queries;
using AmitalCloud.Infrastructure.Model.EntityClasses ;
using AmitalCloud.Infrastructure.Domain.EntityPMs;
using AmitalCloud.Infrastructure.Web.Helpers;
using Microsoft.AspNetCore.Mvc;
using AmitalCloud.Infrastructure.Data.Repositories;

namespace AmitalCloud.Infrastructure.Web.Controllers
{
    [ApiController]
    [Route("api/ngMetaData")]
    public class MetaDataController : ControllerBase
    {
        private readonly LoggedContactResolver _loggedContactResolver;
        public MetaDataController(LoggedContactResolver loggedContactResolver)
        {
            _loggedContactResolver = loggedContactResolver;
        }

        [HttpGet("GetAdvanceQueryFiltersPMs")]
        public ActionResult<List<AdvancedQueryFilterPM>> GetAdvanceQueryFiltersPMs()
        {
			int tenant = AmitalCloudSecurityUtility.AuthenticateTenant();
            var advanceQueryFilters = new AdvancedQueryFilterQueryService(tenant).GetMultiFromCache($"GetAdvanceQueryFiltersPMs{tenant}IsPredefined", a => (a.Tenant == tenant || a.Tenant == 0) && a.IsPredefined == true, "ObjectField,Query,Query.ObjectTable", a => new AdvancedQueryFilterPM(a)
            {
                DisplayInList = a.ObjectField.DisplayInList,
                IsCustomFilter = a.ObjectField.IsCustomFilter,
                ObjectFieldName = a.ObjectField.FieldName,
                DataTypeCode = a.ObjectField.DataTypeCode,
                ObjectFieldOperator = a.ObjectField.Operator,
                QueryObjectTableName = a.Query.ObjectTable.Name,
                QueryUserId = a.Query.UserId,
            });
            return advanceQueryFilters;
        }

        [HttpGet("GetTenantLanguageTranslations")]
        public ActionResult<List<Translation>> GetTenantLanguageTranslations()
        {
            int tenant = AmitalCloudSecurityUtility.AuthenticateTenant();
            List<Translation> AllTranslations = new List<Translation>();
            TenantPM tenantPM = new TenantQueryService(tenant).GetSingle(tenant, false, true);
            if (tenantPM != null && !string.IsNullOrEmpty(tenantPM.Language) && tenantPM.Language.ToLower() != "en" && tenantPM.Language != "english")
            {
				var url = AmitalCloudSecurityUtility.getLoggedDomain();
                AllTranslations = new TranslationQuery(tenant).GetTenantLanguageTranslations(tenantPM.Language, url);
            }
            return AllTranslations;
        }

        [HttpGet("GetTranslations")]
        public ActionResult<List<Translation>> GetTranslations()
        {
            int translationTenant = AmitalCloudSecurityUtility.AuthenticateTenant();
            var url = AmitalCloudSecurityUtility.getLoggedDomain();
            List<Translation> AllTranslations = new TranslationQuery(translationTenant).GetTenantTranslations(url);
            return AllTranslations;
        }

        [HttpGet("GetTenantObjectFields")]
        public ActionResult<List<ObjectFieldPM>>? GetTenantObjectFields()
        {
            int loggedTenant = AmitalCloudSecurityUtility.AuthenticateTenant();
            if (loggedTenant == 0)
            {
                return null;
            }
            var objectFields = new ObjectFieldQueryService(loggedTenant).GetMultiFromCache($"objectFields{loggedTenant}", a => a.Tenant == loggedTenant, "ObjectTable_LookUpTable,FullNameTextCode,ShortNameTextCode,ListTextCode,HelpTextCodeFK,ObjectTable,ObjectTable_MultiTable", a => new ObjectFieldPM(a)
            {
                ObjectTable_LookUpTableName = a.ObjectTable_LookUpTable != null ? a.ObjectTable_LookUpTable.Name : null,
                FullNameTextCodeDefaultText = a.FullNameTextCode != null ? a.FullNameTextCode.DefaultText : null,
                FullNameTextCodeLocalDefaultText = a.FullNameTextCode != null ? a.FullNameTextCode.LocalDefaultText : null,
                ShortNameTextCodeDefaultText = a.ShortNameTextCode != null ? a.ShortNameTextCode.DefaultText : null,
                MultiTableName = a.ObjectTable_MultiTable != null ? a.ObjectTable_MultiTable.Name : null,
                ListTextCodeDefaultText = a.ListTextCode != null ? a.ListTextCode.DefaultText : null,
                HelpTextDefaultText = a.HelpTextCodeFK != null ? a.HelpTextCodeFK.DefaultText : null,
            });
            return objectFields;
        }

        [HttpGet("GetTenantTextCodes")]
        public ActionResult<List<TextCodePM>> GetTenantTextCodes()
        {
			int tenant = AmitalCloudSecurityUtility.AuthenticateTenant();

			var textCodes = new TextCodeQueryService(tenant).GetMultiFromCache($"textCode{tenant}", a => a.Tenant == tenant, "ObjectTable,SpellCheckedByUser.Contact", a => new TextCodePM(a)
            {
                ObjectTableName = a.ObjectTable.Name,
                SpellCheckedByUserName = a.SpellCheckedByUser == null ? null : a.SpellCheckedByUser.Contact.EnglishName,
            });
            return textCodes;
        }

        [HttpGet("GetLoggedUserPM/{useremail?}/{getloggeduser?}")]
        public ActionResult<UserPM?>? GetLoggedUserPM([FromQuery] string useremail, bool getloggeduser)
        {
            int tenant = AmitalCloudSecurityUtility.AuthenticateTenant();
            ContactPM contact = _loggedContactResolver.GetLoggedContact(tenant);
            if (contact == null)
            {
                return null;
            }
            if (!String.IsNullOrWhiteSpace(useremail) && useremail == contact.Email)
            {
                var service = new UserQueryService(tenant);
                return service.GetMulti(a => a.Contact.Email == contact.Email).FirstOrDefault();
            }
            return null;
        }

        [HttpGet("GetAllScreenFieldsByTenant")]
        public ActionResult<List<ScreenFieldPM>> GetAllScreenFieldsByTenant(string screenfields)
        {
            int tenant = AmitalCloudSecurityUtility.AuthenticateTenant();
            var screenFields = new ScreenFieldQuery(tenant).GetScreenFieldPMsByTenant();
            return screenFields;
        }

        [HttpGet("GetAllScreensByTenant")]
        public ActionResult<List<ScreenPM>> GetAllScreensByTenant(string screens)
        {
            int tenant = AmitalCloudSecurityUtility.AuthenticateTenant();
            var allScreens = new ScreenQuery(tenant).GetScreenPMsByTenant();
            return allScreens;
        }

        [HttpGet("GetAllObjectTableTabsByTenant")]
        public ActionResult<List<ObjectTableTabPM>> GetAllObjectTableTabsByTenant([FromQuery] string objecttabletabs)
        {
            int tenant = AmitalCloudSecurityUtility.AuthenticateTenant();
            var objectTableTabs = new ObjectTableTabQuery(tenant).GetObjectTableTabPMsByTenant();
            return objectTableTabs;
        }

        [HttpGet("GetAllObjectTables")]
        public ActionResult<List<ObjectTablePM>> GetAllObjectTables([FromQuery] string objecttables)
        {
            int tenant = AmitalCloudSecurityUtility.AuthenticateTenant();
            ObjectTableQuery query = new ObjectTableQuery(tenant);
            var objectTables = query.GetObjectPMsByTenant(tenant).ToList();
            return objectTables;
        }

        [HttpGet("GetAllMenusTablesByTenant"), HttpGet("menustables")]
        public ActionResult<List<MenusTablePM>> GetAllMenusTablesByTenant([FromQuery] string menustables)
        {
            int tenant = AmitalCloudSecurityUtility.AuthenticateTenant();
            var menuTables = new MenusTableQueryService(tenant).GetMultiFromCache($"GetAllMenusTablesByTenant{tenant}", a => a.Tenant == tenant || a.Tenant == 0, "ObjectTable,Feature", a => new MenusTablePM(a)
            {
                ObjectTableName = a.ObjectTable.Name,
                FeatureCode = a.Feature.Code,
            });
            return menuTables;
        }

        [HttpGet("GetAllStatusesByTenant"), HttpGet("getallstatuses/{inActive?}/{dumb2?}")]
        public IActionResult GetAllStatusesByTenant(bool inActive, [FromQuery] string dumb2)
        {
            try
            {
                int tenant = AmitalCloudSecurityUtility.AuthenticateTenant();
                var statuses = new EntityStatusQueryService(tenant).GetMultiFromCache($"entityStatus{tenant}-{inActive}", a => a.Tenant == tenant && a.InActive == inActive, "ObjectTable", a => new EntityStatusPM(a)
                {
                    ObjectTableName = a.ObjectTable.Name,
                });
                return Ok(statuses);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, AmitalCloudApiExceptionBuilder.BuildException(ex));
            }
        }

        [HttpGet("GetAllDirections/{dummy2?}")]
        public ActionResult<List<DirectionPM>> GetAllDirections([FromQuery] string dummy2)
        {
            var directions = new DirectionQueryService(AmitalCloudSecurityUtility.AuthenticateTenant()).GetMulti(a => true).ToList();
            return directions;
        }

        [HttpGet("GetAllTransportModes/{dummy?}")]
        public ActionResult<List<TransportModePM>> GetAllTransportModes([FromQuery] string dummy)
        {
            var transportModes = new TransportModeQueryService(AmitalCloudSecurityUtility.AuthenticateTenant()).GetMulti(a => true).ToList();
            return transportModes;
        }

        [HttpGet("GetAccountingSettingPM"), HttpGet("accountingsettingpm")]
        public ActionResult<AccountingSettingPM?> GetAccountingSettingPM()
        {
            int id = AmitalCloudSecurityUtility.AuthenticateTenant();
            return new AccountingSettingQueryService(id).GetMulti(a => a.Id == id, a => new AccountingSettingPM(a)
            {
                TransferFTPDetailHost = a.TransferFTPDetail == null ? null : a.TransferFTPDetail.Host,
            }, "TransferFTPDetail").FirstOrDefault();
        }

        [HttpGet("GetCustomsInterfaceSettingPM"), HttpGet("customsinterfacesettingpm")]
        public ActionResult<CustomsInterfaceSettingPM?> GetCustomsInterfaceSettingPM()
        {
            int InterfaceId = AmitalCloudSecurityUtility.AuthenticateTenant();
            return new CustomsInterfaceSettingQueryService(InterfaceId).GetMulti(a => a.Tenant == InterfaceId, a => new CustomsInterfaceSettingPM(a)
            {
                ArtemusOutSettingsHost = a.ArtemusOutSettings == null ? null : a.ArtemusOutSettings.Host,
                ArtemusInSettingsHost = a.ArtemusInSettings == null ? null : a.ArtemusInSettings.Host,
                LocalCustomsInterfaceName = a.LocalCustomsInterface == null ? null : a.LocalCustomsInterface.Name,
            }, "ArtemusOutSettings,ArtemusInSettings,LocalCustomsInterface").FirstOrDefault();
        }

        [HttpGet("GetSharedLogisticsSettingM"), HttpGet("shaerdlogisticssettingpm")]
        public ActionResult<SharedLogisticsSettingPM> GetSharedLogisticsSettingM()
        {
            int settingId = AmitalCloudSecurityUtility.AuthenticateTenant();
            var sharedLogisticsSetting = new SharedLogisticsSettingQueryService(settingId).GetSingle(settingId.ToString(), false, true);
            return sharedLogisticsSetting;
        }
        //ךא בשימוש באנגולר
        [HttpGet("GetAllSpecialServicesTypesByTenant"), HttpGet("getallmenubuttonssbyobjecttable/{objecttableid}/{menubuttons}")]
        public ActionResult<List<MenuButtonPM>> GetAllSpecialServicesTypesByTenant(string objecttableid, bool menubuttons)
        {
            int tenant = AmitalCloudSecurityUtility.AuthenticateTenant();
            var service = new MenuButtonQueryService(tenant);
            return service.GetMulti(a => a.MenuButtonGroupId == objecttableid);
        }

        [HttpGet("GetQueryPMs/{userid?}/{objecttableid?}")]
        public ActionResult<List<QueryPM>> GetQueryPMs( [FromQuery] string UserId, [FromQuery] string objecttableid)
        {
            int tenant = AmitalCloudSecurityUtility.AuthenticateTenant();

            QueryQueryService queryQueryService = new QueryQueryService(tenant);
            List<QueryPM> queries = queryQueryService.GetMultiFromCache($"GetQueryPMs{tenant}-{UserId}", a => (a.Tenant == tenant && (a.UserId == UserId || a.UserId == null)) || a.Tenant == 0 || a.SharedWithAll || a.SharedWithSpecificUsers, "ObjectTable,QueryGroup,NameTextCode", a => new QueryPM(a) 
            {
                ObjectTableName = a.ObjectTable.Name,
                ObjectTableIsNewWizard = a.ObjectTable.IsNewWizard,
                ObjectTableNewWizardControlName = a.ObjectTable.NewWizardControlName,
                QueryGroupIndexOrder = a.QueryGroup != null ? a.QueryGroup.IndexOrder : 0,
                NewViewName = a.NameTextCode == null ? null : a.NameTextCode.DefaultText,
            });

            List<QueryPM> myResult = new List<QueryPM>();
            SharedUserQueryQueryService sharedUserQueryQueryService = new SharedUserQueryQueryService(tenant);
            List<SharedUserQueryPM> sharedUserQueries = sharedUserQueryQueryService.GetMulti(a => a.Tenant == tenant);
            foreach (QueryPM item in queries)
            {
                if (!item.SharedWithSpecificUsers || (item.SharedWithSpecificUsers && (sharedUserQueries.Where(d => d.QueryCode == item.UniqueCode && d.UserId == UserId).Any() || item.SharedByUserId == UserId)))
                {
                    myResult.Add(item);
                }
            }
            myResult = myResult.OrderBy(d => d.IndexOrder).ToList();
            return myResult;
        }
    }
}