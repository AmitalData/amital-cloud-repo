using AmitalCloud.Infrastructure.Data.Context;
using AmitalCloud.Infrastructure.Data.Repositories;
using AmitalCloud.Infrastructure.Model.EntityClasses ;
using AmitalCloud.Infrastructure.Domain.EntityKeys;
using AmitalCloud.Infrastructure.Domain.Interfaces;
using System.Collections.Generic;
using System.Linq;
using AmitalCloud.Infrastructure.Model.Interfaces;

namespace AmitalCloud.Infrastructure.Data.Validators
{
    public class InfrastructureTranslateTextsClass
    {
        public static string GetTranslation(string textCodeCode, string var1, string var2, string var3, int tenant)
        {
            string result = string.Empty;
            IAmitalCloudContext context = AmitalCloudContext.GetContext(tenant);
            IRepository<Translation> translationRepository = new Repository<Translation>(context);
            IRepository<TextCode> textCodeRepository = new Repository<TextCode>(context);
            Tenant myTenant = new Repository<Tenant>(context).GetSingle(new TenantKeys<string> { Id = tenant });
            Translation translation = null;
            TextCode textCode = null;
            string textCodeCodeTranslated = "";
            string variable1 = "";
            string variable2 = "";
            string variable3 = "";

            if (myTenant != null)
            {
                translation = GetSingleTranslation(textCodeCode, tenant, translationRepository, myTenant);
                if (translation != null)
                {
                    textCodeCodeTranslated = translation.TranslatedText;
                }

                else if (translation == null)
                {
                    textCode = textCodeRepository.GetMulti(a => a.Code == textCodeCode && a.Tenant == tenant).FirstOrDefault();                   //.GetTextCodeByTenantAndCode(textCodeCode, tenant);
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
                    translation = GetSingleTranslation(textCodeCode, tenant, translationRepository, myTenant);
                    if (translation != null)
                    {
                        var1Translated = translation.TranslatedText;
                    }

                    else
                    {
                        textCode = textCodeRepository.GetMulti(a => a.Code == variable && a.Tenant == tenant).FirstOrDefault();  //.GetTextCodeByTenantAndCode(variable, tenant);
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
                translation = GetSingleTranslation(var2, tenant, translationRepository, myTenant);// translationRepository.GetSingleTranslation(tenant, var2, myTenant.Language);
                if (translation != null)
                {
                    var2Translated = translation.TranslatedText;
                }

                else
                {
                    textCode = textCodeRepository.GetMulti(a => a.Code == var2 && a.Tenant == tenant).FirstOrDefault(); //.GetTextCodeByTenantAndCode(var2, tenant);
                    if (textCode != null)
                    {
                        var2Translated = textCode.DefaultText;
                    }
                }

                variable2 = var2Translated;

                /* Translated Var3 */
                string var3Translated = var3;
                translation = GetSingleTranslation(var3, tenant, translationRepository, myTenant);
                if (translation != null)
                {
                    var3Translated = translation.TranslatedText;
                }

                else
                {
                    textCode = textCodeRepository.GetMulti(a => a.Code == var3 && a.Tenant == tenant).FirstOrDefault(); //.GetTextCodeByTenantAndCode(var3, tenant);
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

        private static Translation GetSingleTranslation(string textCodeCode, int tenant, IRepository<Translation> translationRepository, Tenant myTenant)
        {
            return translationRepository.GetMulti(d => d.Tenant == tenant && d.TextCodeCode == textCodeCode && (d.TranslationHeader.Description == myTenant.Language || d.TranslationHeaderCode == myTenant.Language)).FirstOrDefault();
            //       GetSingleTranslation(tenant, textCodeCode, myTenant.Language);
        }

        public static string Translate(string textCodeCode, int tenant)
        {
            string result = string.Empty;

            TranslationRepository translationRepository = new TranslationRepository(tenant);
            Tenant myTenant = new Repository<Tenant>(translationRepository.context).GetSingle(a => a.Id == tenant);

            if (myTenant != null)
            {
                Translation translation = translationRepository.GetSingleTranslation(tenant, textCodeCode, myTenant.Language);
                if (translation != null)
                {
                    result = translation.TranslatedText;
                }

                else if (translation == null)
                {
                    TextCode textCode = new Repository<TextCode>(translationRepository.context).GetMulti(a => a.Code == textCodeCode && a.Tenant == tenant).FirstOrDefault(); //   GetTextCodeByTenantAndCode(textCodeCode, tenant);
                    if (textCode != null)
                    {
                        result = textCode.DefaultText;
                    }
                }
            }

            return result;
            //return "! " + Result;
        }
    }
}
