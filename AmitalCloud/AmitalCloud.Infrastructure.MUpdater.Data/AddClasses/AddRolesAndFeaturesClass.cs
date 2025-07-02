using AmitalCloud.Infrastructure.Data.Counters;
using AmitalCloud.Infrastructure.Data.Repositories;
using AmitalCloud.Infrastructure.Domain.EntityPMs;
using AmitalCloud.Infrastructure.Model.EntityClasses;

namespace AmitalCloud.Infrastructure.MUpdater.Data.AddClasses
{
    public class AddRolesAndFeaturesClass
    {
        public static Role AddRole(RolePM roleDetails, Repository<Role> roleRepository, Dictionary<string, Role> tenantRoles,int contextTenant=0)
        {
            if (tenantRoles.Keys.Contains(roleDetails.Code))
            {
                Role updatedRole = tenantRoles[roleDetails.Code];
                updatedRole.Name = roleDetails.Name;
                updatedRole.Tenant = roleDetails.Tenant;
                updatedRole.RoleTypeCode = roleDetails.RoleTypeCode;
                updatedRole.Description = roleDetails.Description;
                roleRepository.Update(updatedRole);
                return updatedRole;
            }

            else
            {
                Role newRole = new Role()
                {
                    Tenant = roleDetails.Tenant,
                    Name = roleDetails.Name,
                    Code = roleDetails.Code,
                    RoleTypeCode = roleDetails.RoleTypeCode,
                    Description = roleDetails.Description,

                    Id = IdCounter.GetNumber("Role", contextTenant).ToString(),
                };

                roleRepository.Insert(newRole);
                return newRole;
            }
        }

        public static Feature AddFeature(FeaturePM featureDetails, Repository<Feature> featuresRepository, Repository<TextCode> textCodeReposit, Dictionary<string, Feature> tenantFearures, Dictionary<string, TextCode> textCodes, int contextTenant = 0)
        {

            ObjectTableRepository Repo = new ObjectTableRepository(contextTenant);
            var table = Repo.GetSingleObjectTable(featureDetails.ObjectTableId, featureDetails.Tenant, false);

            featureDetails.Code = featureDetails.Code.Trim();
            featureDetails.FeatureUniqeCode = table.Name + "." + featureDetails.Code;

            if (featureDetails.FeatureTypeCode == "MODL")
            {
                featureDetails.Packagable = true;
            }

            if (featureDetails.Code == "UPDATE" || featureDetails.Code == "READ" || featureDetails.Code == "NEW")
            {
                featureDetails.Packagable = false;
            }

            if (tenantFearures.Keys.Contains(featureDetails.Code + featureDetails.ObjectTableId))
            {
                Feature updatedFeature = tenantFearures[featureDetails.Code + featureDetails.ObjectTableId];
                updatedFeature.ObjectTableId = featureDetails.ObjectTableId;
                updatedFeature.Tenant = featureDetails.Tenant;
                updatedFeature.FeatureTypeCode = featureDetails.FeatureTypeCode;
                updatedFeature.Code = featureDetails.Code;
                updatedFeature.Packagable = featureDetails.Packagable;
                updatedFeature.IsBusinessUnitEnabled = featureDetails.IsBusinessUnitEnabled;
                updatedFeature.FeatureUniqeCode = featureDetails.FeatureUniqeCode;

                TextCode updatedTextCode = null;
                if (textCodes.Keys.Contains(featureDetails.NameTextCodeCode + featureDetails.Tenant + featureDetails.ObjectTableId))
                {
                    updatedTextCode = textCodes[featureDetails.NameTextCodeCode + featureDetails.Tenant + featureDetails.ObjectTableId];
                }

                if (updatedTextCode == null)
                {
                    updatedTextCode = new TextCode()
                    {
                        Id = IdCounter.GetNumber("TextCode", contextTenant).ToString(),
                        Tenant = featureDetails.Tenant,
                        ObjectTableId = featureDetails.ObjectTableId,
                        DefaultText = featureDetails.NameTextCodeDefaultText,
                        Code = featureDetails.NameTextCodeCode,
                        TextCodeTypeCode = "O",
                    };

                    updatedFeature.NameTextCodeId = updatedTextCode.Id;
                    updatedFeature.NameTextCodeCode = updatedTextCode.Code;
                    textCodeReposit.Insert(updatedTextCode);
                }

                else
                {
                    updatedTextCode = textCodes[featureDetails.NameTextCodeCode + featureDetails.Tenant + featureDetails.ObjectTableId];
                    updatedTextCode.DefaultText = featureDetails.NameTextCodeDefaultText;
                    updatedTextCode.ObjectTableId = featureDetails.ObjectTableId;
                    updatedTextCode.Tenant = featureDetails.Tenant;
                    updatedTextCode.InActive = false;

                    textCodeReposit.Update(updatedTextCode);


                }
                featuresRepository.Update(updatedFeature);

                return updatedFeature;
            }

            else
            {

                    TextCode newTextCode = null;
                    if (textCodes.Keys.Contains(featureDetails.NameTextCodeCode + featureDetails.Tenant + featureDetails.ObjectTableId))
                    {
                        newTextCode = textCodes[featureDetails.NameTextCodeCode + featureDetails.Tenant + featureDetails.ObjectTableId];
                    }

                if (newTextCode == null)
                {
                    newTextCode = new TextCode()
                    {
                        Id = IdCounter.GetIdWithIdsRange("TextCode", 100, contextTenant).ToString(),
                        Tenant = featureDetails.Tenant,
                        ObjectTableId = featureDetails.ObjectTableId,
                        DefaultText = featureDetails.NameTextCodeDefaultText,
                        Code = featureDetails.NameTextCodeCode.Trim(),
                        TextCodeTypeCode = "O",
                    };

                            textCodeReposit.Insert(newTextCode);
                            textCodes.Add(featureDetails.NameTextCodeCode + featureDetails.Tenant + featureDetails.ObjectTableId, newTextCode);

                    }

                Feature newFeature = new Feature()
                {
                    Id = IdCounter.GetIdWithIdsRange("Feature", 100, contextTenant).ToString(),
                    Tenant = featureDetails.Tenant,
                    ObjectTableId = featureDetails.ObjectTableId,
                    Code = featureDetails.Code.Trim(),
                    NameTextCodeId = newTextCode.Id,
                    NameTextCodeCode = newTextCode.Code,
                    FeatureTypeCode = featureDetails.FeatureTypeCode,
                    Packagable = featureDetails.Packagable,
                    IsBusinessUnitEnabled = featureDetails.IsBusinessUnitEnabled,
                    IsOld = false,
                    IsCoreFeature = featureDetails.IsCoreFeature,
                    FeatureUniqeCode = featureDetails.FeatureUniqeCode
                };

                featuresRepository.Insert(newFeature);
                tenantFearures.Add(newFeature.Code + newFeature.ObjectTableId, newFeature);

                return newFeature;
            }


        }

        
        public static Feature AddFeature(FeaturePM featureDetails, Repository<Feature> featuresRepository, Repository<TextCode> textCodeReposit, Dictionary<string, Feature> tenantFearures, Dictionary<string, TextCode> textCodes,ObjectTable table,int contextTenant=0)
        {

            
            featureDetails.Code = featureDetails.Code.Trim();
            featureDetails.FeatureUniqeCode = table.Name + "." + featureDetails.Code;

            if (featureDetails.FeatureTypeCode == "MODL")
            {
                featureDetails.Packagable = true;
            }

            if (featureDetails.Code == "UPDATE" || featureDetails.Code == "READ" || featureDetails.Code == "NEW")
            {
                featureDetails.Packagable = false;
            }

            if (tenantFearures.Keys.Contains(featureDetails.Code + featureDetails.ObjectTableId))
            {
                Feature updatedFeature = tenantFearures[featureDetails.Code + featureDetails.ObjectTableId];
                updatedFeature.ObjectTableId = featureDetails.ObjectTableId;
                updatedFeature.Tenant = featureDetails.Tenant;
                updatedFeature.FeatureTypeCode = featureDetails.FeatureTypeCode;
                updatedFeature.Code = featureDetails.Code;
                updatedFeature.Packagable = featureDetails.Packagable;
                updatedFeature.IsBusinessUnitEnabled = featureDetails.IsBusinessUnitEnabled;
                updatedFeature.FeatureUniqeCode = featureDetails.FeatureUniqeCode;
                updatedFeature.ToggleCode = featureDetails.ToggleCode;

                TextCode updatedTextCode = null;
                if (textCodes.Keys.Contains(featureDetails.NameTextCodeCode + featureDetails.Tenant + featureDetails.ObjectTableId))
                {
                    updatedTextCode = textCodes[featureDetails.NameTextCodeCode + featureDetails.Tenant + featureDetails.ObjectTableId];
                }

                if (updatedTextCode == null)
                {
                    updatedTextCode = new TextCode()
                    {
                        Id = IdCounter.GetNumber("TextCode", contextTenant).ToString(),
                        Tenant = featureDetails.Tenant,
                        ObjectTableId = featureDetails.ObjectTableId,
                        DefaultText = featureDetails.NameTextCodeDefaultText,
                        Code = featureDetails.NameTextCodeCode,
                        TextCodeTypeCode = "O",
                    };

                    updatedFeature.NameTextCodeId = updatedTextCode.Id;
                    updatedFeature.NameTextCodeCode = updatedTextCode.Code;
                    textCodeReposit.Insert(updatedTextCode);
                }

                else
                {
                    updatedTextCode = textCodes[featureDetails.NameTextCodeCode + featureDetails.Tenant + featureDetails.ObjectTableId];
                    updatedTextCode.DefaultText = featureDetails.NameTextCodeDefaultText;
                    updatedTextCode.ObjectTableId = featureDetails.ObjectTableId;
                    updatedTextCode.Tenant = featureDetails.Tenant;
                    updatedTextCode.InActive = false;

                    textCodeReposit.Update(updatedTextCode);


                }
                featuresRepository.Update(updatedFeature);

                return updatedFeature;
            }

            else
            {
                TextCode newTextCode = null;
                if (textCodes.Keys.Contains(featureDetails.NameTextCodeCode + featureDetails.Tenant + featureDetails.ObjectTableId))
                {
                    newTextCode = textCodes[featureDetails.NameTextCodeCode + featureDetails.Tenant + featureDetails.ObjectTableId];
                }

                if (newTextCode == null)
                {
                    newTextCode = new TextCode()
                    {
                        Id = IdCounter.GetIdWithIdsRange("TextCode", 100, contextTenant).ToString(),
                        Tenant = featureDetails.Tenant,
                        ObjectTableId = featureDetails.ObjectTableId,
                        DefaultText = featureDetails.NameTextCodeDefaultText,
                        Code = featureDetails.NameTextCodeCode.Trim(),
                        TextCodeTypeCode = "O",
                    };

                    textCodeReposit.Insert(newTextCode);
                    textCodes.Add(featureDetails.NameTextCodeCode + featureDetails.Tenant + featureDetails.ObjectTableId, newTextCode);
                }

                Feature newFeature = new Feature()
                {
                    Id = IdCounter.GetIdWithIdsRange("Feature", 100, contextTenant).ToString(),
                    Tenant = featureDetails.Tenant,
                    ObjectTableId = featureDetails.ObjectTableId,
                    Code = featureDetails.Code.Trim(),
                    NameTextCodeId = newTextCode.Id,
                    NameTextCodeCode = newTextCode.Code,
                    FeatureTypeCode = featureDetails.FeatureTypeCode,
                    Packagable = featureDetails.Packagable,
                    IsBusinessUnitEnabled = featureDetails.IsBusinessUnitEnabled,
                    IsOld = false,
                    IsCoreFeature = featureDetails.IsCoreFeature,
                    FeatureUniqeCode = featureDetails.FeatureUniqeCode,
                    ToggleCode = featureDetails.ToggleCode

                };

                featuresRepository.Insert(newFeature);
                tenantFearures.Add(newFeature.Code + newFeature.ObjectTableId, newFeature);
                
                return newFeature;
            }


        }

        public static Feature AddFeature(FeaturePM featureDetails, Dictionary<string, Feature> tenantFearures, Dictionary<string, TextCode> textCodes, ObjectTable table,
            List<Feature> addedFeatures,List<TextCode> addedTextCodes,int contextTenant=0)
        {

            

            featureDetails.Code = featureDetails.Code.Trim();
            featureDetails.FeatureUniqeCode = table.Name + "." + featureDetails.Code;

            if (featureDetails.FeatureTypeCode == "MODL")
            {
                featureDetails.Packagable = true;
            }

            if (featureDetails.Code == "UPDATE" || featureDetails.Code == "READ" || featureDetails.Code == "NEW")
            {
                featureDetails.Packagable = false;
            }

            if (tenantFearures.Keys.Contains(featureDetails.Code + featureDetails.ObjectTableId))
            {
                Feature updatedFeature = tenantFearures[featureDetails.Code + featureDetails.ObjectTableId];
                
                return updatedFeature;
            }

            else
            {
                TextCode newTextCode = null;
                if (textCodes.Keys.Contains(featureDetails.NameTextCodeCode + featureDetails.Tenant + featureDetails.ObjectTableId))
                {
                    newTextCode = textCodes[featureDetails.NameTextCodeCode + featureDetails.Tenant + featureDetails.ObjectTableId];
                }

                if (newTextCode == null)
                {
                    newTextCode = new TextCode()
                    {
                        Id = IdCounter.GetIdWithIdsRange("TextCode", 100, contextTenant).ToString(),
                        Tenant = featureDetails.Tenant,
                        ObjectTableId = featureDetails.ObjectTableId,
                        DefaultText = featureDetails.NameTextCodeDefaultText,
                        Code = featureDetails.NameTextCodeCode.Trim(),
                        TextCodeTypeCode = "O",
                    };

                    addedTextCodes.Add(newTextCode);
                    textCodes.Add(featureDetails.NameTextCodeCode + featureDetails.Tenant + featureDetails.ObjectTableId, newTextCode);
                }

                Feature newFeature = new Feature()
                {
                    Id = IdCounter.GetIdWithIdsRange("Feature", 100, contextTenant).ToString(),
                    Tenant = featureDetails.Tenant,
                    ObjectTableId = featureDetails.ObjectTableId,
                    Code = featureDetails.Code.Trim(),
                    NameTextCodeId = newTextCode.Id,
                    NameTextCodeCode = newTextCode.Code,
                    FeatureTypeCode = featureDetails.FeatureTypeCode,
                    Packagable = featureDetails.Packagable,
                    IsBusinessUnitEnabled = featureDetails.IsBusinessUnitEnabled,
                    IsOld = false,
                    IsCoreFeature = featureDetails.IsCoreFeature,
                    FeatureUniqeCode = featureDetails.FeatureUniqeCode
                };

                addedFeatures.Add(newFeature);
                tenantFearures.Add(newFeature.Code + newFeature.ObjectTableId, newFeature);

                return newFeature;
            }
            

        }
        
       

    }
}