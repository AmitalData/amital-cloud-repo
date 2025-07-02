using System.Text;
using AmitalCloud.Infrastructure.Model.EntityClasses;
using AmitalCloud.Infrastructure.Data.Repositories;
using AmitalCloud.Infrastructure.Data.Counters;
using AmitalCloud.Infrastructure.Domain.EntityPMs;
namespace AmitalCloud.Infrastructure.MUpdater.Data.AddClasses
{
    public class AddTextCodes
    {
        
        private static Dictionary<string, TextCode> AddedTextCodes = new Dictionary<string, TextCode>();
        public static TextCode AddTextCode(TextCodePM textCodeDetails, Repository<TextCode> textCodeRepository, Dictionary<string, TextCode> textCodes,int contextTenant = 0)
        {
            if (textCodes.Keys.Contains(textCodeDetails.Code + textCodeDetails.Tenant + textCodeDetails.ObjectTableId))
            {
                TextCode textCode = textCodes[textCodeDetails.Code + textCodeDetails.Tenant + textCodeDetails.ObjectTableId];
                if (!textCode.IsSpellChecked)
                {
                    textCode.DefaultText = textCodeDetails.DefaultText;
                    textCode.LocalDefaultText = textCodeDetails.LocalDefaultText;
                }
                textCode.DefaultTextPlural = textCodeDetails.DefaultTextPlural;
                textCode.TextCodeTypeCode = textCodeDetails.TextCodeTypeCode;
                textCode.InActive = textCodeDetails.InActive;
                textCode.IsSpellChecked = textCodeDetails.IsSpellChecked;
                textCode.LocalDefaultText = TryConvertFromBase64(textCode.LocalDefaultText);

                textCodeRepository.Update(textCode);
                return textCode;
            }
            else
            {
                if (!AddedTextCodes.ContainsKey(textCodeDetails.Code))
                {
                    TextCode newTextCode = new TextCode()
                    {
                        TextCodeTypeCode = textCodeDetails.TextCodeTypeCode,
                        DefaultTextPlural = textCodeDetails.DefaultTextPlural,
                        DefaultText = textCodeDetails.DefaultText,
                        Code = textCodeDetails.Code,
                        Id = IdCounter.GetNumber("TextCode", contextTenant).ToString(),
                        ObjectTableId = textCodeDetails.ObjectTableId,
                        Tenant = textCodeDetails.Tenant,
                        LocalDefaultText = TryConvertFromBase64(textCodeDetails.LocalDefaultText),
                        IsSpellChecked = textCodeDetails.IsSpellChecked,
                    };

                    textCodeRepository.Insert(newTextCode);
                    AddedTextCodes.Add(textCodeDetails.Code, newTextCode);
                    return newTextCode;
                }
                else
                    return AddedTextCodes[textCodeDetails.Code];
            }
        }


        public static TextCode AddTextCode(TextCodePM textCodeDetails, Dictionary<string, TextCode> textCodes, List<TextCode> addedTextCodes,int contextTenant = 0)
        {
            if (textCodes.Keys.Contains(textCodeDetails.Code + textCodeDetails.Tenant + textCodeDetails.ObjectTableId))
            {
                TextCode textCode = textCodes[textCodeDetails.Code + textCodeDetails.Tenant + textCodeDetails.ObjectTableId];
                return textCode;
            }
            else
            {
                TextCode newTextCode = new TextCode()
                {
                    TextCodeTypeCode = textCodeDetails.TextCodeTypeCode,
                    DefaultTextPlural = textCodeDetails.DefaultTextPlural,
                    DefaultText = textCodeDetails.DefaultText,
                    Code = textCodeDetails.Code,
                    Id = IdCounter.GetIdWithIdsRange("TextCode", 100, contextTenant).ToString(),
                    ObjectTableId = textCodeDetails.ObjectTableId,
                    Tenant = textCodeDetails.Tenant,
                    LocalDefaultText = TryConvertFromBase64(textCodeDetails.LocalDefaultText),
                    IsSpellChecked = textCodeDetails.IsSpellChecked,
                };
              
                addedTextCodes.Add(newTextCode);
                return newTextCode;
            }
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
            string substringToRemove = "\"";
            string backUp = input;
            try
            {
                input = input.Trim('\"');
                input = input.Substring(5);//REMOVE BS64:
                byte[] data = Convert.FromBase64String(input);
                string decodedString = Encoding.UTF8.GetString(data);
               
                return decodedString;

            }
            catch (FormatException)
            {
                return backUp;
            }

        }
   
       
     

    }
}