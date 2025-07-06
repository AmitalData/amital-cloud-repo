using AmitalCloud.Infrastructure.Data.Counters;
using AmitalCloud.Infrastructure.Data.Repositories;
using AmitalCloud.Infrastructure.Domain.EntityPMs;
using AmitalCloud.Infrastructure.Model.EntityClasses;

namespace AmitalCloud.Infrastructure.MUpdater.Data.AddClasses
{
    public class AddScreensAndScreenFields
    {
        public static Screen AddScreen(ScreenPM screenDetails, Repository<Screen> screenRepository,Dictionary<string,Screen>tenantZeroScreens,int contextTenant=0)
        {
            if (tenantZeroScreens.Keys.Contains(screenDetails.Code + screenDetails.ObjectTableId))
            {
                Screen screen = tenantZeroScreens[screenDetails.Code + screenDetails.ObjectTableId];
                screen.IsReadOnly = screenDetails.IsReadOnly;
                screen.NumberOfColumns = screenDetails.NumberOfColumns;
                screen.NumberOfRows = screenDetails.NumberOfRows;
                screen.Name = screenDetails.Name;


                screenRepository.Update(screen);
                return screen;
            }
            else
            {
                Screen newScreen = new Screen()
                {
                    NumberOfRows = screenDetails.NumberOfRows,
                    NumberOfColumns = screenDetails.NumberOfColumns,
                    IsReadOnly = screenDetails.IsReadOnly,
                    Code = screenDetails.Code,
                    Name=screenDetails.Name,
                    Id = IdCounter.GetNumber("Screen", contextTenant).ToString(),
                    ObjectTableId = screenDetails.ObjectTableId,
                    Tenant = 0,

                };
                screenRepository.Insert(newScreen);
                return newScreen;

            }
        }

        public static ScreenField AddScreenField(ScreenFieldPM screenFieldDetails, Repository<ScreenField> screenFieldsRepository, Dictionary<string, ScreenField> tenantScreenFields,int contextTenant=0)
        {
            if (tenantScreenFields.Keys.Contains(screenFieldDetails.ScreenCode + screenFieldDetails.ObjectFieldCode))
            {
                ScreenField screenfield = tenantScreenFields[screenFieldDetails.ScreenCode + screenFieldDetails.ObjectFieldCode];
                screenfield.Column = screenFieldDetails.Column;
                screenfield.Row = screenFieldDetails.Row;
                
                screenFieldsRepository.Update(screenfield);
                return screenfield;
            }
            else
            {
                ScreenField newscreenfield = new ScreenField()
                {
                    Row = screenFieldDetails.Row,
                    Column = screenFieldDetails.Column,
                    Id = IdCounter.GetNumber("ScreenField", contextTenant).ToString(),
                    ObjectFieldId = screenFieldDetails.ObjectFieldId,
                    ScreenId = screenFieldDetails.ScreenId,
                    ScreenCode = screenFieldDetails.ScreenCode,
                    Tenant = screenFieldDetails.Tenant,
                    ObjectFieldCode = screenFieldDetails.ObjectFieldCode,

                };
                screenFieldsRepository.Insert(newscreenfield);
                return newscreenfield;
            }
        }
    }
}