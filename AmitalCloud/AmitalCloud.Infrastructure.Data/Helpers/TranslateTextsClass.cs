using AmitalCloud.Infrastructure.Data.Repositories;
using AmitalCloud.Infrastructure.Model.EntityClasses ;
using AmitalCloud.Infrastructure.Domain.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AmitalCloud.Infrastructure.Data.Helpers
{
    public class TranslateTextsClass
    {
        public static string GetTranslation(string textCodeCode, string var1, string var2, string var3, int tenant)
        {
            string result = string.Empty;
            TranslationRepository translationRepository = new TranslationRepository(tenant);
            IRepository<TextCode> textCodeRepository = new Repository<TextCode>(translationRepository.context);
            Tenant myTenant = new Repository<Tenant>(translationRepository.context).GetMulti(a => a.Id == tenant).FirstOrDefault();//GetSingleTenant(tenant, true);
            Translation translation = null;
            TextCode textCode = null;
            string textCodeCodeTranslated = "";
            string variable1 = "";
            string variable2 = "";
            string variable3 = "";

            if (myTenant != null)
            {
                translation = translationRepository.GetSingleTranslation(tenant, textCodeCode, myTenant.Language);
                if (translation != null)
                {
                    textCodeCodeTranslated = translation.TranslatedText;
                }

                else if (translation == null)
                {
                    textCode = GetTextCode(textCodeCode, tenant, textCodeRepository);
                    if (textCode != null)
                    {
                        textCodeCodeTranslated = textCode.DefaultText;
                    }
                }

                /* Translated Var1 */
                string var1Translated = "";
                string[] var1Array = var1.Split(',');
                List<string> var1TranslatedList = new List<string>();

                foreach (string variable in var1Array)
                {
                    translation = translationRepository.GetSingleTranslation(tenant, variable, myTenant.Language);
                    if (translation != null)
                    {
                        var1Translated = translation.TranslatedText;
                    }

                    else
                    {
                        textCode = GetTextCode(variable, tenant, textCodeRepository); //textCodeRepository.GetTextCodeByTenantAndCode(variable, tenant);
                        if (textCode != null)
                        {
                            var1Translated = textCode.DefaultText;
                        }
                    }

                    if (!string.IsNullOrEmpty(var1Translated))
                    {
                        var1TranslatedList.Add(var1Translated);
                    }
                }

                if (var1TranslatedList.Count >= 0)
                {
                    foreach (string s in var1TranslatedList)
                    {
                        variable1 = variable1 + "," + s;

                    }
                    variable1 = variable1.TrimStart(',');
                }

                else
                {
                    variable1 = var1TranslatedList[0];
                }


                /* Translated Var2 */
                string var2Translated = var2;
                translation = translationRepository.GetSingleTranslation(tenant, var2, myTenant.Language);
                if (translation != null)
                {
                    var2Translated = translation.TranslatedText;
                }

                else
                {
                    textCode = GetTextCode(var2, tenant, textCodeRepository); //textCodeRepository.GetTextCodeByTenantAndCode(var2, tenant);
                    if (textCode != null)
                    {
                        var2Translated = textCode.DefaultText;
                    }
                }

                variable2 = var2Translated;

                /* Translated Var3 */
                string var3Translated = var3;
                translation = translationRepository.GetSingleTranslation(tenant, var3, myTenant.Language);
                if (translation != null)
                {
                    var3Translated = translation.TranslatedText;
                }

                else
                {
                    textCode = GetTextCode(var3, tenant, textCodeRepository); //textCodeRepository.GetTextCodeByTenantAndCode(var3, tenant);
                    if (textCode != null)
                    {
                        var3Translated = textCode.DefaultText;
                    }
                }

                variable3 = var3Translated;
            }

            if (textCodeCodeTranslated.Contains("%FieldName"))
            {
                result = textCodeCodeTranslated.Replace("%FieldName", variable1);
            }
            if (result.Contains("%Minlength"))
            {
                result = result.Replace("%Minlength", variable2);
            }
            if (result.Contains("%Maxlength"))
            {
                result = result.Replace("%Maxlength", variable3);
            }

            return result;

        }

        private static TextCode GetTextCode(string textCodeCode, int tenant, IRepository<TextCode> textCodeRepository)
        {
            return textCodeRepository.GetMulti(a => a.Code == textCodeCode && a.Tenant == tenant).FirstOrDefault();
            //GetTextCodeByTenantAndCode(textCodeCode, tenant);
        }

        public static string Translate(string textCodeCode, int tenant)
        {

#if true//itzik- better performance + unit test inject
            string key = "TranslateTextsClass/Translate," + textCodeCode + "," + tenant.ToString();
            var val = CacheManager.GetOrInsertNewObject<string>(key, () =>
            {
#endif
                string result = string.Empty;

                //TenantRepository tenantRepository = new TenantRepository(tenant);
                TranslationRepository translationRepository = new TranslationRepository(tenant);
                IRepository<TextCode> textCodeRepository = new Repository<TextCode>(translationRepository.context);
                Tenant myTenant = new Repository<Tenant>(translationRepository.context).GetMulti(a => a.Id == tenant).FirstOrDefault(); //  GetSingleTenant(tenant, true);

                if (myTenant != null)
                {
                    Translation translation = translationRepository.GetSingleTranslation(tenant, textCodeCode, myTenant.Language);
                    if (translation != null)
                    {
                        result = translation.TranslatedText;
                    }

                    else if (translation == null)
                    {
                        TextCode textCode = GetTextCode(textCodeCode, tenant, textCodeRepository);  //textCodeRepository.GetTextCodeByTenantAndCode(textCodeCode, tenant);

                        if (textCode != null)
                        {
                            result = textCode.DefaultText;
                        }
                    }
                }

                return result;
                //return "! " + Result;
#if true//itzik- better performance + unit test inject

            });
            return val;
#endif
        }

        //<--- Yuval Chalup 14.04.2015
        public static string Translate(string textCodeCode, int tenant, bool getLocalDefaultText, bool getTextByLanguage = false)
        {

            string key = "TranslateTextsClass/Translate1," + textCodeCode + "," + tenant.ToString() + "," + getLocalDefaultText + "," + getTextByLanguage;
            var val = CacheManager.GetOrInsertNewObject<string>(key, () =>
            {
                string result = string.Empty;

                //TenantRepository tenantRepository = new TenantRepository(tenant);
                TranslationRepository translationRepository = new TranslationRepository(tenant);
                IRepository<TextCode> textCodeRepository = new Repository<TextCode>(translationRepository.context);
                Tenant myTenant = new Repository<Tenant>(translationRepository.context).GetMulti(a => a.Id == tenant).FirstOrDefault();   //GetSingleTenant(tenant, true);

                if (myTenant != null)
                {
                    Translation translation = translationRepository.GetSingleTranslation(tenant, textCodeCode, myTenant.Language);
                    if (translation != null)
                    {
                        result = translation.TranslatedText;
                    }

                    else if (translation == null)
                    {
                        TextCode textCode = GetTextCode(textCodeCode, tenant, textCodeRepository); //textCodeRepository.GetTextCodeByTenantAndCode(textCodeCode, tenant);
                        if (textCode != null)
                        {
                            if (!getTextByLanguage)
                            {
                                if (getLocalDefaultText)
                                {
                                    result = textCode.LocalDefaultText;
                                }
                                else
                                {
                                    result = textCode.DefaultText;
                                }

                            }
                            else
                            {
                                if (myTenant.Language == "HB")
                                {
                                    result = !string.IsNullOrEmpty(textCode.LocalDefaultText) ? textCode.LocalDefaultText : textCode.DefaultText;
                                }
                                else
                                {
                                    result = textCode.DefaultText;
                                }
                            }

                        }
                    }
                }
                return result;
            });
            return val;

            //return "! " + Result;
        }

        // moran 7.4.16 - AMI-55700 -->
        public static string GetRequiredFieldForTableMessageTranslation(string requiredTextCodeCode, string fieldName, string tableName, string entityReference, int tenant)
        {
            string[] var1Array1 = fieldName.Split(',');
            List<string> translatedList1 = new List<string>();
            string variable1 = "";
            string variable2 = "";
            string variable3 = "";
            string originalTranslation = "";

            if (!string.IsNullOrWhiteSpace(requiredTextCodeCode))
            {
                originalTranslation = TranslateTextsClass.Translate(requiredTextCodeCode, tenant, true);// translation.TranslatedText; // TextCode's default text
            }

            if (!string.IsNullOrWhiteSpace(tableName) && !string.IsNullOrWhiteSpace(fieldName))
            {
                string fieldID = tableName + ".F." + fieldName;
                variable1 = TranslateTextsClass.Translate(fieldID, tenant); //FieldName
            }

            if (!string.IsNullOrWhiteSpace(tableName))
            {
                variable2 = TranslateTextsClass.Translate(tableName, tenant); //FieldName
                if (string.IsNullOrWhiteSpace(variable2)) variable2 = tableName;
            }

            variable3 = entityReference;

            string translatedText = "";

            if (originalTranslation.Contains("%FieldName"))
            {
                translatedText = originalTranslation.Replace("%FieldName", variable1);
            }
            if (originalTranslation.Contains("%TableName"))
            {
                translatedText = translatedText.Replace("%TableName", variable2);
            }

            if (originalTranslation.Contains("%EntityReference"))
            {
                translatedText = translatedText.Replace("%EntityReference", variable3);
            }

            return translatedText;
        }
        // moran 7.4.16 - AMI-55700 <--


        // Abdullah
        public static Contact GetLoggedContact(int tenant)
        {
            //email
            string email = "";
            if (HttpContextHelper.User != null)
                email = HttpContextHelper.User.Identity.Name;
            else
                email = "system@tenant" + tenant.ToString() + ".com";
            Contact contactPM = new Repository<Contact>(tenant).GetMulti(a => a.Email == email && a.Tenant == tenant).FirstOrDefault();    //GetSingleContactByEmailAndTenant(email, tenant);
            return contactPM;
        }


    }

}
