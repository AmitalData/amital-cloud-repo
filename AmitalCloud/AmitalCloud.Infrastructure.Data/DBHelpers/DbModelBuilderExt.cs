using AmitalCloud.Infrastructure.Domain.Enums;
using AmitalCloud.Infrastructure.Model.Enums;
using System;
using System.Collections;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace AmitalCloud.Infrastructure.Data.DBHelpers
{
    internal static class DbModelBuilderExt
    {
        public static void SetDefaultSchema(this ModelBuilder myDbModelBuilder,
            AmitalCloudDBSchema schema,
            string ConnSchemaUserId)
        {
            //if (schema == AmitalCloudDBSchema.none)
            //{
            //    throw new Exception("Enums.AmitalCloudDBSchema.none !!?????");
            //}
            var toSchema = ConnSchemaUserId;//schema.ToString();
            //if (schema == AmitalCloudDBSchema.AMITAL_DB)
            //{
            //    toSchema = DBHelpers.DbContextBaseUtil.GetSchemaAMITAL_DB();
            //}
            myDbModelBuilder.HasDefaultSchema(toSchema);//Not Work !!!!
            RewriteSchema(myDbModelBuilder, toSchema);//Work !!!!

        }
        const BindingFlags RewriteSchemaBindingFlags = BindingFlags.Instance | BindingFlags.NonPublic;
        static void RewriteSchema(ModelBuilder modelBuilder, string schema)
        {
            var modelBuilderType = modelBuilder.GetType();
            var modelConfiguration = modelBuilderType.GetProperty("ModelConfiguration", RewriteSchemaBindingFlags).GetValue(modelBuilder);
            var activeEntityConfigurations = (IList)modelConfiguration.GetType().GetProperty("ActiveEntityConfigurations", RewriteSchemaBindingFlags).GetValue(modelConfiguration);
            foreach (var item in activeEntityConfigurations)
            {
                RewriteSchemaForEntityTypeConfiguration(item, schema);
            }
        }
        static void RewriteSchemaForEntityTypeConfiguration(object entityTypeConfiguration, string schema)
        {
            // not bulletproof, but better than nothing
            if (entityTypeConfiguration.GetType().Name != "EntityTypeConfiguration")
                throw new ArgumentException();
            var entityTypeConfigurationType = entityTypeConfiguration.GetType();
            var entityMappingConfigurations = ((IList)entityTypeConfigurationType.GetField("_entityMappingConfigurations", RewriteSchemaBindingFlags).GetValue(entityTypeConfiguration));
            foreach (var entityMappingConfiguration in entityMappingConfigurations)
            {
                var navigationPropertyConfigurations = (IDictionary)entityTypeConfigurationType.GetField("_navigationPropertyConfigurations", RewriteSchemaBindingFlags).GetValue(entityTypeConfiguration);
                foreach (var val in navigationPropertyConfigurations.Values)
                {
                    var associationMappingConfiguration = val.GetType().GetProperty("AssociationMappingConfiguration", RewriteSchemaBindingFlags).GetValue(val);
                    if (associationMappingConfiguration == null)
                        continue;
                    var tableNameAssociation = associationMappingConfiguration.GetType().GetField("_tableName", RewriteSchemaBindingFlags).GetValue(associationMappingConfiguration);
                    var schemaAssociation = (string)tableNameAssociation.GetType().GetProperty("Schema").GetValue(tableNameAssociation);
                    var nameAssociation = (string)tableNameAssociation.GetType().GetProperty("Name").GetValue(tableNameAssociation);
                    ToTableHelper(associationMappingConfiguration, nameAssociation, schemaAssociation ?? schema);
                }
                var tableNameEntity = entityMappingConfiguration.GetType().GetProperty("TableName").GetValue(entityMappingConfiguration);
                var schemaEntity = (string)tableNameEntity.GetType().GetProperty("Schema").GetValue(tableNameEntity);
                var nameEntity = (string)tableNameEntity.GetType().GetProperty("Name").GetValue(tableNameEntity);
                ToTableHelper(entityTypeConfiguration, nameEntity,
                    //schemaEntity ?? 
                    schema);
            }
        }
        static void ToTableHelper(object configuration, string name, string schema)
        {
            configuration.GetType().GetMethod("ToTable", new[] { typeof(string), typeof(string) }).Invoke(configuration, new[] { name, schema });
        }
    }
}
