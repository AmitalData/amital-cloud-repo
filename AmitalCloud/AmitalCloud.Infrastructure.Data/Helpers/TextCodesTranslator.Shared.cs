namespace AmitalCloud.Infrastructure.Data.Helpers
{
    public partial class TextCodesTranslator : ITextCodeTranslator
    {
        public static string TranslateText(string textCodeCode, int tenant)
        {
            Helpers.TextCodesTranslator translator = new Helpers.TextCodesTranslator();
            var TranslateText = translator.Translate(textCodeCode, tenant);
            if (string.IsNullOrWhiteSpace(TranslateText))
            {
                TranslateText = "$Text(" + textCodeCode + ")";//Our Version Of Uniface Convention   
            }
            return TranslateText;
        }
        public static string TranslateText(string textCodeCode, int tenant, bool showLocal)
        {
            TextCodesTranslator translator = new TextCodesTranslator();
            var TranslateText = translator.Translate(textCodeCode, tenant, showLocal);

            return TranslateText;
        }
    }

    public interface ITextCodeTranslator
    {
        string Translate(string textCodeCode, int tenant);
        string Translate(string textCodeCode, int tenant, bool showLocal);
    }
}