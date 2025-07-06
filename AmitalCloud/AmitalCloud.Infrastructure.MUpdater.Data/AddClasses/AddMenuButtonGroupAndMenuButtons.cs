using AmitalCloud.Infrastructure.Data.Counters;
using AmitalCloud.Infrastructure.Data.Repositories;
using AmitalCloud.Infrastructure.Domain.EntityPMs;
using AmitalCloud.Infrastructure.Model.EntityClasses;
using System.Text;
namespace AmitalCloud.Infrastructure.MUpdater.Data.AddClasses
{
    public class AddMenuButtonGroupAndMenuButtons
    {
        public static MenuButtonGroup AddMenuButtonGroup(MenuButtonGroupPM menuButtonGroupDetails, Repository<MenuButtonGroup> menuButtonGroupRepository, Dictionary<string, MenuButtonGroup> tenantMenuButtonGroups, int contextTenant = 0)
        {

            ObjectTableRepository Repo = new ObjectTableRepository(contextTenant);
            var table = Repo.GetSingleObjectTable(menuButtonGroupDetails.ObjectTableId, menuButtonGroupDetails.Tenant, false);
            string NewKey = "";
            if (ObjectTablesKeys.Keys.Keys.Contains(table.Name))
            {
                NewKey = ObjectTablesKeys.Keys[table.Name];
            }

            if (table.UpdateKey != NewKey || string.IsNullOrEmpty(NewKey))
            {
                if (tenantMenuButtonGroups.Keys.Contains(menuButtonGroupDetails.Name))
                {
                    MenuButtonGroup menuButtonGroup = tenantMenuButtonGroups[menuButtonGroupDetails.Name];
                    menuButtonGroup.ObjectTableId = menuButtonGroupDetails.ObjectTableId;
                    menuButtonGroup.Tenant = menuButtonGroupDetails.Tenant;
                    menuButtonGroupRepository.Update(menuButtonGroup);
                    //table.UpdateKey = NewKey;
                    //Repo.Update(table);
                    //Repo.SubmitChanges();
                    return menuButtonGroup;

                }
                else
                {
                    MenuButtonGroup newMenuButtonGroup = new MenuButtonGroup()
                    {
                        Tenant = menuButtonGroupDetails.Tenant,
                        ObjectTableId = menuButtonGroupDetails.ObjectTableId,
                        Id = IdCounter.GetNumber("MenuButtonGroup", contextTenant).ToString(),
                        MenuButtonGroupType = menuButtonGroupDetails.MenuButtonGroupType,
                        Name = menuButtonGroupDetails.Name,

                    };
                    menuButtonGroupRepository.Insert(newMenuButtonGroup);
                    return newMenuButtonGroup;
                }
            }
            else
            {
                return tenantMenuButtonGroups[menuButtonGroupDetails.Name];
            }
        }

        public static MenuButton AddMenuButton(MenuButtonPM menuButtonDetails, Repository<MenuButton> menuButtonRepository, Dictionary<string, MenuButton> tenantMenuButtons, Repository<TextCode> textCodeRepository, Dictionary<string, TextCode> textCodes,int contextTenant=0)
        {
            ObjectTableRepository Repo = new ObjectTableRepository(contextTenant);
            var table = Repo.GetSingleObjectTable(menuButtonDetails.ObjectTableId, menuButtonDetails.Tenant, false);
            string NewKey = "";
            if (ObjectTablesKeys.Keys.Keys.Contains(table.Name))
            {
                NewKey = ObjectTablesKeys.Keys[table.Name];
            }

            if (table.UpdateKey != NewKey || string.IsNullOrEmpty(NewKey))
            {
                if (tenantMenuButtons.Keys.Contains(menuButtonDetails.EventCode + menuButtonDetails.MenuButtonGroupId))
                {
                    MenuButton menuButton = tenantMenuButtons[menuButtonDetails.EventCode + menuButtonDetails.MenuButtonGroupId];
                    menuButton.FeatureId = menuButtonDetails.FeatureId;
                    menuButton.Index = menuButtonDetails.Index;
                    menuButton.IsActive = menuButtonDetails.IsActive;
                    menuButton.ParentMenuButtonId = menuButtonDetails.ParentMenuButtonId;
                    menuButton.Tenant = menuButtonDetails.Tenant;
                    menuButton.MenuButtonType = menuButtonDetails.MenuButtonType;
                    menuButton.DropDownControl = menuButtonDetails.DropDownControl;
                    menuButton.Style = menuButtonDetails.Style;
                    menuButton.ControlPath = menuButtonDetails.ControlPath;
                    menuButton.Width = menuButtonDetails.Width;
                    menuButton.HtmlComponentPath = menuButtonDetails.HtmlComponentPath;
                    menuButton.FeatureUniqeCode = menuButtonDetails.FeatureUniqeCode;
                    menuButtonRepository.Update(menuButton);

                    if (textCodes.Keys.Contains(menuButtonDetails.LabelTextCodeCode))
                    {
                        TextCode textCode = textCodes[menuButtonDetails.LabelTextCodeCode];
                        if (!textCode.IsSpellChecked)
                        {
                            textCode.DefaultText = menuButtonDetails.LabelTextCodeDefaultText;
                            textCode.LocalDefaultText = TryConvertFromBase64(menuButtonDetails.LocalDefaultText);
                        }
                        textCode.InActive = false;
                        textCodeRepository.Update(textCode);
                    }
                    else if (textCodes.Keys.Contains(menuButtonDetails.LabelTextCodeCode + menuButtonDetails.Tenant + menuButtonDetails.ObjectTableId))
                    {
                        TextCode textCode = textCodes[menuButtonDetails.LabelTextCodeCode + menuButtonDetails.Tenant + menuButtonDetails.ObjectTableId];
                        if (!textCode.IsSpellChecked)
                        {
                            textCode.DefaultText = menuButtonDetails.LabelTextCodeDefaultText;
                            textCode.LocalDefaultText = TryConvertFromBase64(menuButtonDetails.LocalDefaultText);
                        }
                        textCode.InActive = false;
                        textCodeRepository.Update(textCode);

                    }
                    else
                    {
                        var textCode = new TextCode()
                        {
                            DefaultText = menuButtonDetails.LabelTextCodeDefaultText,
                            Id = IdCounter.GetNumber("TextCode", contextTenant).ToString(),
                            TextCodeTypeCode = "B",
                            Code = menuButtonDetails.LabelTextCodeCode,
                            ObjectTableId = menuButtonDetails.ObjectTableId,
                            Tenant = menuButtonDetails.Tenant,
                            LocalDefaultText = TryConvertFromBase64(menuButtonDetails.LocalDefaultText),
                            InActive = false
                        };

                        textCodeRepository.Insert(textCode);
                    }
                    return menuButton;
                }
                else
                {
                    TextCode newTextCode = null;
                    if (textCodes.Keys.Contains(menuButtonDetails.LabelTextCodeCode))
                    {
                        newTextCode = textCodes[menuButtonDetails.LabelTextCodeCode];

                    }
                    else
                    {
                        newTextCode = new TextCode()
                        {
                            DefaultText = menuButtonDetails.LabelTextCodeDefaultText,
                            Id = IdCounter.GetNumber("TextCode", contextTenant).ToString(),
                            TextCodeTypeCode = "B",
                            Code = menuButtonDetails.LabelTextCodeCode,
                            ObjectTableId = menuButtonDetails.ObjectTableId,
                            Tenant = menuButtonDetails.Tenant,
                            LocalDefaultText = TryConvertFromBase64(menuButtonDetails.LocalDefaultText),

                        };
                        textCodeRepository.Insert(newTextCode);
                    }


                    MenuButton newMenuButton = new MenuButton()
                    {
                        Tenant = menuButtonDetails.Tenant,
                        ParentMenuButtonId = menuButtonDetails.ParentMenuButtonId,
                        IsActive = menuButtonDetails.IsActive,
                        Index = menuButtonDetails.Index,
                        FeatureId = menuButtonDetails.FeatureId,
                        MenuButtonGroupId = menuButtonDetails.MenuButtonGroupId,
                        EventCode = menuButtonDetails.EventCode,
                        MenuButtonType = menuButtonDetails.MenuButtonType,
                        DropDownControl = menuButtonDetails.DropDownControl,
                        ControlPath = menuButtonDetails.ControlPath,
                        HtmlComponentPath = menuButtonDetails.HtmlComponentPath,
                        Id = IdCounter.GetNumber("MenuButton", contextTenant).ToString(),
                        LabelTextCodeId = newTextCode.Id,
                        LabelTextCodeCode = newTextCode.Code,
                        Style = menuButtonDetails.Style,
                        FeatureUniqeCode = menuButtonDetails.FeatureUniqeCode,
                        Width = menuButtonDetails.Width,
                    };
                    menuButtonRepository.Insert(newMenuButton);

                    return newMenuButton;
                }
            }
            else
            {
                return tenantMenuButtons[menuButtonDetails.EventCode + menuButtonDetails.MenuButtonGroupId];
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

    public static class ObjectTablesKeys
    {
        public static Dictionary<string, string> Keys = new Dictionary<string, string>() { };
    }
}
