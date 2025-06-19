using AmitalCloud.Infrastructure.Application.EntityQueryServices;
using AmitalCloud.Infrastructure.Application.EntityUpdateServices;
using AmitalCloud.Infrastructure.Data.Helpers;
using AmitalCloud.Infrastructure.Domain.EntityPMs;
using AmitalCloud.Infrastructure.Domain.Enums;
using AmitalCloud.Infrastructure.Web.DataContracts;
using System;
using SkiaSharp;

namespace AmitalCloud.Infrastructure.Application.Helpers
{
    public class CaptchaHelper
    {
        public string GenerateCaptchaImage(string code)
        {
            int width = 120;
            int height = 40;

            using var bitmap = new SKBitmap(width, height);
            using var canvas = new SKCanvas(bitmap);
            canvas.Clear(SKColors.White);

            using var paint = new SKPaint
            {
                Color = SKColors.Green,
                IsAntialias = true,
                TextSize = 24,
                Typeface = SKTypeface.FromFamilyName("Times New Roman", SKFontStyle.BoldItalic)
            };

            canvas.DrawText(code, 10, 30, paint);

            using var image = SKImage.FromBitmap(bitmap);
            using var data = image.Encode(SKEncodedImageFormat.Jpeg, 90);

            var base64 = Convert.ToBase64String(data.ToArray());
            return $"data:image/jpeg;base64,{base64}";
        }

        public bool CheckCaptchaCodeValidated(string code, string Key, string? userCaptchaKey = null, bool withoutCheckUsed = false)
        {
            bool result = false;
            if (!string.IsNullOrEmpty(Key))
            {
                CaptchaKeyQueryService captchaKeyQueryService = new CaptchaKeyQueryService(0);

                CaptchaKeyPM? captchaKey = !withoutCheckUsed ? captchaKeyQueryService.GetMulti(d => d.Id == Key && !d.IsUsed).FirstOrDefault() : captchaKeyQueryService.GetSingle(Key, false, false);
                if (captchaKey != null)
                {
                    if (captchaKey.Id == userCaptchaKey || string.IsNullOrEmpty(userCaptchaKey))
                    {
                        if (!string.IsNullOrEmpty(code))
                        {
                            if (captchaKey.Code.ToUpper() == code.ToUpper()) result = true;
                        }
                    }

                    captchaKey.IsUsed = true;
                    CaptchaKeyUpdateService captchaKeyUpdateService = new CaptchaKeyUpdateService(0);
                    captchaKey.ChangeSetOp = ChangeSetOperation.Update;
                    captchaKeyUpdateService.Update(captchaKey, true);
                }
            }
            return result;
        }

        public void AddCaptchaKey(string email, UserData data, string activity)
        {
            CaptchaKeyPM captchaKey = new CaptchaKeyPM()
            {
                Id = Guid.NewGuid().ToString(),
                IP = AuthenticationUtil.GetIP4Address(),
                Code = RandomString(6),
                CreateDate = DateTime.Now,
                Email = email,
                Activity = activity,
                IsUsed = false,
                ChangeSetOp = ChangeSetOperation.Insert
            };
            CaptchaKeyUpdateService captchaKeyUpdateService = new CaptchaKeyUpdateService(data.Tenant);
            captchaKeyUpdateService.Update(captchaKey, true);

            data.HasError = true;
            data.InValidCaptcha = true;
            data.CaptchaImage = GenerateCaptchaImage(captchaKey.Code);
            data.CaptchaKey = captchaKey.Id;
        }

        public static Random random = new Random();
        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNPQRSTUVWXYZ123456789";
            return new string(Enumerable.Repeat(chars, length).Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}