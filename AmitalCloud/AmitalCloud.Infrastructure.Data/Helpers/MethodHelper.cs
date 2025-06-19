using AmitalCloud.Infrastructure.Data.Repositories;
using AmitalCloud.Infrastructure.Model.EntityClasses ;
using AmitalCloud.Infrastructure.Domain.EntityKeys;
using System;

namespace AmitalCloud.Infrastructure.Data.Helpers
{
    public class MethodHelper
    {
        public static bool IsLCLEntity(string myTransportModeId, string myShipmentTypeId)
        {
            bool myResult = false;

            if (myTransportModeId != null)
            {
                myTransportModeId = myTransportModeId.ToUpper();
            }

            if (myShipmentTypeId != null)
            {
                myShipmentTypeId = myShipmentTypeId.ToUpper();
            }

            if (myTransportModeId == "A")
            {
                myResult = true;
            }

            else if (myTransportModeId == "O" && myShipmentTypeId == "LCLD")
            {
                myResult = true;
            }

            else if (myTransportModeId == "I" && myShipmentTypeId == "LTL")
            {
                myResult = true;
            }

            return myResult;
        }
        public static bool IsFCLEntity(string myTransportModeId, string myShipmentTypeId)
        {
            bool output = false;

            if (myTransportModeId != null)
            {
                myTransportModeId = myTransportModeId.ToUpper();
            }

            if (myShipmentTypeId != null)
            {
                myShipmentTypeId = myShipmentTypeId.ToUpper();
            }

            if (myTransportModeId == "O" && (myShipmentTypeId == "FCLD" || myShipmentTypeId == "MYGO"))
            {
                output = true;
            }

            if (myTransportModeId == "I" && (myShipmentTypeId == "FTL" || myShipmentTypeId == "MYGI"))
            {
                output = true;
            }

            return output;
        }

        public static double Roundd(double? value, int digits)
        {
            double? myValue = null;

            if (value != null)
            {
                myValue = Convert.ToDouble(value);
            }

            double? myResult = myValue;

            if (myValue != null && digits >= 1 && digits <= 15)
            {
                string mySTR = String.Format("{0:N" + digits + "}", myValue);

                myResult = Convert.ToDouble(mySTR);
            }

            return myResult == null ? 0 : myResult.Value;
        }
        public static double? Round(double? value, int digits)
        {
            double? myValue = null;

            if (value != null)
            {
                myValue = Convert.ToDouble(value);
            }

            double? myResult = myValue;

            if (myValue != null && digits >= 1 && digits <= 15)
            {
                string mySTR = String.Format("{0:N" + digits + "}", myValue);

                myResult = Convert.ToDouble(mySTR);
            }

            return myResult;
        }
        public static decimal? Round(object value, int digits)
        {
            double? myValue = null;
            if (value != null)
            {
                myValue = Convert.ToDouble(value);
            }

            double? myResult = myValue;

            if (myValue != null && digits >= 1 && digits <= 15)
            {
                string mySTR = String.Format("{0:N" + digits + "}", myValue);

                myResult = Convert.ToDouble(mySTR);
            }

            if (myResult == null)
            {
                return null;
            }

            else
            {
                return (decimal)myResult;
            }
        }

        public static void AddToSearchFields(ref string mySearchFields, string myField)
        {
            if (!string.IsNullOrEmpty(myField))
            {
                mySearchFields = string.IsNullOrEmpty(mySearchFields) ? myField : mySearchFields + "," + myField;
            }
        }

        public static DatesHelper GetDates(string code, int tenant)
        {
            DatesHelper helper = new DatesHelper();

            DateTime? todayDate = TenantServerConfigration.GetCurrentDateTime(tenant).Date;
            DateTime? date1 = null;
            DateTime? date2 = null;
            int days = 0;

            if (code.Contains("_1"))
            {
                string str = code.Replace("_1", "");

                days = Convert.ToInt32(str);
            }
            else
            {
                days = Convert.ToInt32(code);
            }

            if (code == "-365_1")
            {
                date1 = new DateTime(todayDate.Value.Year - 1, 1, 1);
                date2 = new DateTime(todayDate.Value.Year - 1, 12, 31);
            }

            else if (code == "-30_1")
            {
                if (todayDate.Value.Month == 1)
                {
                    date1 = new DateTime(todayDate.Value.Year - 1, 12, 1);
                }
                else
                {
                    date1 = new DateTime(todayDate.Value.Year, todayDate.Value.Month - 1, 1);
                }

                date2 = new DateTime(date1.Value.Year, date1.Value.Month, DateTime.DaysInMonth(date1.Value.Year, date1.Value.Month));
            }

            else if (code == "-90_1")
            {
                switch (todayDate.Value.Month)
                {
                    case 1:
                    case 2:
                    case 3:
                        {
                            date1 = new DateTime(todayDate.Value.Year - 1, 10, 1);
                            date2 = new DateTime(todayDate.Value.Year - 1, 12, 31);
                            break;
                        }

                    case 4:
                    case 5:
                    case 6:
                        {
                            date1 = new DateTime(todayDate.Value.Year, 1, 1);
                            date2 = new DateTime(todayDate.Value.Year, 3, 31);
                            break;
                        }

                    case 7:
                    case 8:
                    case 9:
                        {
                            date1 = new DateTime(todayDate.Value.Year, 4, 1);
                            date2 = new DateTime(todayDate.Value.Year, 6, 30);
                            break;
                        }

                    case 10:
                    case 11:
                    case 12:
                        {
                            date1 = new DateTime(todayDate.Value.Year, 7, 1);
                            date2 = new DateTime(todayDate.Value.Year, 9, 30);
                            break;
                        }
                }
            }

            else if (code == "0_1")
            {
                date1 = new DateTime(todayDate.Value.Year, todayDate.Value.Month, 1);
                date2 = new DateTime(todayDate.Value.Year, todayDate.Value.Month, DateTime.DaysInMonth(todayDate.Value.Year, todayDate.Value.Month));
                days = Convert.ToInt32("-30");
            }

            else
            {
                date1 = todayDate.Value.AddDays(days).Date;
                date2 = todayDate.Value.AddHours(23).AddMinutes(59).AddSeconds(59);
            }

            helper.Date1 = date1;
            helper.Date2 = date2;
            helper.Days = days;

            return helper;
        }

        //public static DatesHelper GetTicketsChartsDates(string code, int tenant)
        //{
        //    DateTime? todayDate = TenantServerConfigration.GetCurrentDateTime(tenant).Date;
        //    DateTime? date1 = null;
        //    DateTime? date2 = null;

        //    switch (code)
        //    {
        //        case "0":
        //            {
        //                date1 = todayDate;
        //                date2 = todayDate;
        //                break;
        //            }

        //        case "-1":
        //            {
        //                date1 = todayDate.Value.AddDays(-1);
        //                date2 = todayDate.Value.AddDays(-1);
        //                break;
        //            }

        //        case "-7":
        //            {
        //                date1 = todayDate.Value.AddDays(-7);
        //                date2 = todayDate.Value.AddDays(-1);
        //                break;
        //            }

        //        case "0_1":// current Month
        //            {
        //                date1 = new DateTime(todayDate.Value.Year, todayDate.Value.Month, 1);
        //                date2 = new DateTime(todayDate.Value.Year, todayDate.Value.Month, DateTime.DaysInMonth(todayDate.Value.Year, todayDate.Value.Month));
        //                break;
        //            }

        //        case "-30_1": // Last Month
        //            {
        //                if (todayDate.Value.Month == 1)
        //                {
        //                    date1 = new DateTime(todayDate.Value.Year - 1, 12, 1);
        //                }
        //                else
        //                {
        //                    date1 = new DateTime(todayDate.Value.Year, todayDate.Value.Month - 1, 1);
        //                }

        //                date2 = new DateTime(date1.Value.Year, date1.Value.Month, DateTime.DaysInMonth(date1.Value.Year, date1.Value.Month));
        //                break;
        //            }

        //        case "-30": // Last 30 days  
        //            {
        //                if (todayDate.Value.Month == 1)
        //                {
        //                    date1 = new DateTime(todayDate.Value.Year - 1, 12, 1);
        //                }
        //                else
        //                {
        //                    date1 = new DateTime(todayDate.Value.Year, todayDate.Value.Month - 1, 1);
        //                }

        //                date2 = new DateTime(date1.Value.Year, date1.Value.Month, DateTime.DaysInMonth(date1.Value.Year, date1.Value.Month));
        //                break;
        //            }

        //        case "-90": // Last 90 days  
        //            {
        //                if (todayDate.Value.Month == 1)
        //                {
        //                    date1 = new DateTime(todayDate.Value.Year - 1, 12, 1);
        //                }
        //                else
        //                {
        //                    date1 = new DateTime(todayDate.Value.Year, todayDate.Value.Month - 1, 1);
        //                }

        //                date2 = new DateTime(date1.Value.Year, date1.Value.Month, DateTime.DaysInMonth(date1.Value.Year, date1.Value.Month));
        //                break;
        //            }


        //        case "-90_1": // Last quarter 
        //            {
        //                switch (todayDate.Value.Month)
        //                {
        //                    case 1:
        //                    case 2:
        //                    case 3:
        //                        {
        //                            date1 = new DateTime(todayDate.Value.Year - 1, 10, 1);
        //                            date2 = new DateTime(todayDate.Value.Year - 1, 12, 31);
        //                            break;
        //                        }

        //                    case 4:
        //                    case 5:
        //                    case 6:
        //                        {
        //                            date1 = new DateTime(todayDate.Value.Year, 1, 1);
        //                            date2 = new DateTime(todayDate.Value.Year, 3, 31);
        //                            break;
        //                        }

        //                    case 7:
        //                    case 8:
        //                    case 9:
        //                        {
        //                            date1 = new DateTime(todayDate.Value.Year, 4, 1);
        //                            date2 = new DateTime(todayDate.Value.Year, 6, 30);
        //                            break;
        //                        }

        //                    case 10:
        //                    case 11:
        //                    case 12:
        //                        {
        //                            date1 = new DateTime(todayDate.Value.Year, 7, 1);
        //                            date2 = new DateTime(todayDate.Value.Year, 9, 30);
        //                            break;
        //                        }
        //                }
        //                break;
        //            }


        //        case "-365": // Last year  
        //            {

        //                break;
        //            }


        //        case "-365_1": // Last year 
        //            {

        //                break;
        //            }

        //    }

        //    DatesHelper myResult = new DatesHelper()
        //    {
        //        Date1 = date1,
        //        Date2 = date2,
        //    };

        //    return myResult;  
        //}

        public static bool IsAirlineRestricted(string myAirlineId, Repository<Airline> myRepository, int tenant)
        {
            bool isRestricted = false;

            if (!string.IsNullOrEmpty(myAirlineId))
            {
                var key = new AirlineKeys<string>() { Id = myAirlineId };
                Airline myAirline = myRepository.GetSingle(key);
                if (myAirline != null)
                {
                    if (!myAirline.IsAllowedInAirlinesRestriction)
                    {
                        isRestricted = true;
                    }
                }
            }

            return isRestricted;
        }

        public static double? CalculateChargeableWeight(object grossWeight, object volumetricWeight, string grossWeightUnitCode, string chargeableWeightUnitCode, string directionId, string transportModeId)
        {
            double? myGrossWeight = null;
            double? myVolumetricWeight = null;

            if (grossWeight != null)
            {
                myGrossWeight = Convert.ToDouble(grossWeight);
            }

            if (volumetricWeight != null)
            {
                myVolumetricWeight = Convert.ToDouble(volumetricWeight);
            }

            double? myResult = null;

            if (myGrossWeight != null || myVolumetricWeight != null)
            {

                double? grossWeightInVolumetricUnit = GetWeightFromWeight(grossWeightUnitCode, chargeableWeightUnitCode, myGrossWeight);

                if (grossWeightInVolumetricUnit != null && myVolumetricWeight == null)
                {
                    myResult = grossWeightInVolumetricUnit;
                }

                if (grossWeightInVolumetricUnit == null && myVolumetricWeight != null)
                {
                    myResult = myVolumetricWeight;
                }

                if (grossWeightInVolumetricUnit != null && myVolumetricWeight != null)
                {
                    myResult = grossWeightInVolumetricUnit > myVolumetricWeight ? grossWeightInVolumetricUnit : myVolumetricWeight;
                }
            }

            if (myResult != null)
            {
                myResult = Round(myResult, chargeableWeightUnitCode, directionId, transportModeId);
            }

            return myResult;
        }
        public static double? GetWeightFromWeight(string fromWeightCode, string toWeightCode, object weight)
        {
            double? myWeight = null;

            if (weight != null)
            {
                myWeight = Convert.ToDouble(weight);
            }

            double? myResult = null;
            double? factorOfConvert = 1;

            if (weight != null)
            {
                if (string.IsNullOrEmpty(fromWeightCode))
                {
                    factorOfConvert = 1;
                }

                else
                {
                    switch (fromWeightCode.ToUpper())
                    {
                        case "KG": { factorOfConvert = 1; break; }
                        case "LB": { factorOfConvert = 0.45359237; break; }     // 1 LB = 0.45359237 KG
                        case "MT": { factorOfConvert = 1000; break; }           // 1 mt = 1000 KG
                    }
                }

                double? weightInKilograms = myWeight * factorOfConvert;

                if (string.IsNullOrEmpty(toWeightCode))
                {
                    factorOfConvert = 1;
                }

                else
                {
                    switch (toWeightCode.ToUpper())
                    {
                        case "KG": { factorOfConvert = 1; break; }
                        case "LB": { factorOfConvert = 0.45359237; break; }     // 1 LB = 0.45359237 KG
                        case "MT": { factorOfConvert = 1000; break; }           // 1 mt = 1000 KG
                    }
                }

                myResult = weightInKilograms / factorOfConvert;
            }

            if (myResult != null)
            {
                myResult = Round(myResult.Value, 3);
            }

            return myResult;
        }
        public static double? Round(object args, string chargeableWeightUnitCode, string directionId, string transportModeId)
        {
            double? myArgs = null;

            if (args != null)
            {
                myArgs = Convert.ToDouble(args);
            }

            double? result = myArgs;

            if (chargeableWeightUnitCode != "MT")
            {
                if (directionId == "E" && transportModeId == "A")
                {
                    if (result != null)
                    {
                        string toString = result.ToString();
                        string[] r = toString.Split('.');

                        if (r.Length > 1)
                        {
                            string strDigits = "0." + r[1];
                            double? digits = Convert.ToDouble(strDigits);
                            double? integer = Convert.ToDouble(r[0]);

                            if (digits < 0.5)
                            {
                                result = integer + 0.5;
                            }

                            else
                            {
                                result = integer + 1;
                            }
                        }
                    }
                }
            }

            return result;
        }

        public static string ConvertPartnerTypeToTableName(string myPartnerTypeId)
        {
            string myResult = null;

            switch (myPartnerTypeId)
            {
                case "AG":
                    {
                        myResult = "Agent";
                        break;
                    }
                case "AL":
                    {
                        myResult = "AirLine";
                        break;
                    }
                case "CG":
                    {
                        myResult = "Custom Agent";
                        break;
                    }
                case "CS":
                    {
                        myResult = "Customer";
                        break;
                    }

                case "PO":
                    {
                        myResult = "Potential Customer";
                        break;
                    }

                case "SG":
                    {
                        myResult = "Shipping Agent";
                        break;
                    }

                case "SL":
                    {
                        myResult = "Shipping Line";
                        break;
                    }

                case "VD":
                    {
                        myResult = "Vendor";
                        break;
                    }

                case "WH":
                    {
                        myResult = "Warehouse";
                        break;
                    }
            }

            return myResult;
        }

        public static decimal Normalize(decimal value)
        {
            return value / 1.000000000000000000000000000000000m;
        }

        public static decimal? Normalize(decimal? value)
        {
            return value / 1.000000000000000000000000000000000m;
        }

        public static int CalculateLuhnAlgorithm(string number)
        {
            int sum = 0, d;
            for (int i = 0; i < number.Length; i++)
            {
                d = Convert.ToInt32(number.Substring((number.Length - 1) - i, 1));
                if ((i + 1) % 2 != 0)
                    d = d * 2;
                if (d > 9)
                    d -= 9;
                sum += d;
            }

            if (sum % 10 == 0)
            {
                return 0;
            }
            else
            {
                return 10 - (sum % 10);
            }
        }
        public static string Trim(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return null;
            }

            else
            {
                return value.Trim();
            }
        }
        public static double GetValue(double? value)
        {
            if (value == null)
            {
                return 0;
            }

            else
            {
                return value.Value;
            }
        }

        public static double? ComputeRatio(string directionId, string transportModeId, string shipmentTypeId, Tenant tenant)
        {
            double? ratio = null;
            if (IsTenantFromUS(tenant))
            {
                ratio = GetRatioForUSTenant(directionId, transportModeId, shipmentTypeId);
            }

            if (ratio == null)
            {
                switch (transportModeId)
                {
                    case "A":
                        {
                            ratio = tenant.AirRatio;
                            break;
                        }

                    case "O":
                        {
                            if (shipmentTypeId == "FCL" || shipmentTypeId == "FCLD")
                            {
                                ratio = tenant.FCLRatio;
                            }

                            else
                            {
                                ratio = tenant.LCLRatio;
                            }

                            break;
                        }

                    case "I":
                        {
                            if (shipmentTypeId == "FTL")
                            {
                                ratio = tenant.FTLRatio;
                            }

                            else
                            {
                                ratio = tenant.LTLRatio;
                            }

                            break;
                        }
                }
            }

            return ratio;
        }
        private static bool IsTenantFromUS(Tenant tenant)
        {
            bool isFromUS = false;
            Address address = null;
            if (!string.IsNullOrEmpty(tenant.AddressId))
            {
                address = new Repository<Address>(tenant.Id).GetSingle(new AddressKeys<string>() { Id = tenant.AddressId }); //(tenant.AddressId, tenant.Id);                
            }

            if (address == null)
            {
                isFromUS = false;
            }

            if (address.Country != null && address.Country.Code.ToUpper() == "US")
            {
                isFromUS = true;
            }

            return isFromUS;
        }
        private static double? GetRatioForUSTenant(string directionId, string transportModeId, string shipmentTypeId)
        {
            double? ratio = null;

            if (directionId == "D")
            {
                if (transportModeId == "A")
                {
                    ratio = 7;
                }

                else if (transportModeId == "I")
                {
                    if (shipmentTypeId == "LTL")
                    {
                        ratio = 9;
                    }
                }
            }

            return ratio;
        }
    }

    public class DatesHelper
    {
        public DateTime? Date1 { get; set; }
        public DateTime? Date2 { get; set; }
        public int Days { get; set; }

        public DatesHelper()
        {

        }
    }
}
