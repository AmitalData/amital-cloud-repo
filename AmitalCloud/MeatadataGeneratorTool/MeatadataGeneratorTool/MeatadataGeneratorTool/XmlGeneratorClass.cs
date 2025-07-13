using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml;
using MeatadataGeneratorTool.QueryModule;
using MeatadataGeneratorTool.ScreensModule;
using MeatadataGeneratorTool.TabsModule;
using MeatadataGeneratorTool.CloseTablesData;
using MeatadataGeneratorTool.EventTypes;
using MeatadataGeneratorTool.MenuButtons;
using MeatadataGeneratorTool.DataContractsModule;
using System.Configuration;
using System.Security.Cryptography;
using System.IO;
using MeatadataGeneratorTool.Helpers;
using MeatadataGeneratorTool.TextCodes;
using MeatadataGeneratorTool.Features;
using System.Xml.Linq;
using System.Text.RegularExpressions;

namespace MeatadataGeneratorTool
{
    public class XmlGeneratorClass
    {
        #region GenerateSQL

        private static string GeneratedSqlPath = ConfigurationManager.AppSettings["GeneratedSQLPath"];
        private static string GeneratedFullSqlPath = ConfigurationManager.AppSettings["GeneratedFullSQLPath"];
        private static string SolutionDirPath = ConfigurationManager.AppSettings["SolutionDirPath"];

        public static string CalculateMD5Hash(string input)
        {
            MD5 md = MD5.Create();
            byte[] bytes = Encoding.ASCII.GetBytes(input);
            byte[] buffer2 = md.ComputeHash(bytes);
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < buffer2.Length; i++)
            {
                builder.Append(buffer2[i].ToString("X2"));
            }
            return builder.ToString();
        }

        public static bool GenerateSqlXmlFileFromTool(ObjectTableViewModel table, bool FromOut = false)//execute when click Generate Create Table Sql
        {
            string[] strArray = new string[8];
            strArray[0] = DateTime.Now.Year.ToString();
            /*
               DateTime.Now.Year.ToString(), 
                DateTime.Now.Month.ToString().Length == 1 ? "0" + DateTime.Now.Month.ToString() : DateTime.Now.Month.ToString(),
                DateTime.Now.Day.ToString().Length == 1 ? "0" + DateTime.Now.Day.ToString() : DateTime.Now.Day.ToString(),
                DateTime.Now.Hour.ToString().Length == 1 ? "0" + DateTime.Now.Hour.ToString() : DateTime.Now.Hour.ToString(),
                DateTime.Now.Minute.ToString().Length == 1 ? "0" + DateTime.Now.Minute.ToString() : DateTime.Now.Minute.ToString(),
                DateTime.Now.Second.ToString().Length == 1 ? "0" + DateTime.Now.Second.ToString() : DateTime.Now.Second.ToString(),
             */
            strArray[1] = DateTime.Now.Month.ToString().Length == 1 ? "0" + DateTime.Now.Month.ToString() : DateTime.Now.Month.ToString();
            strArray[2] = DateTime.Now.Day.ToString().Length == 1 ? "0" + DateTime.Now.Day.ToString() : DateTime.Now.Day.ToString();
            strArray[3] = DateTime.Now.Hour.ToString().Length == 1 ? "0" + DateTime.Now.Hour.ToString() : DateTime.Now.Hour.ToString();
            strArray[4] = DateTime.Now.Minute.ToString().Length == 1 ? "0" + DateTime.Now.Minute.ToString() : DateTime.Now.Minute.ToString();
            int second = DateTime.Now.Second;
            strArray[5] = DateTime.Now.Second.ToString().Length == 1 ? "0" + DateTime.Now.Second.ToString() : DateTime.Now.Second.ToString();
            strArray[6] = "_CreateTable_";
            strArray[7] = table.DBTableName;
            string str = string.Concat(strArray);
            XmlDocument document = new XmlDocument();
            XmlDeclaration newChild = document.CreateXmlDeclaration("1.0", "UTF-8", null);
            document.AppendChild(newChild);
            XmlElement fieldElement = (XmlElement)document.AppendChild(document.CreateElement("databaseChangeLog"));
            SetAttribute("xmlns", "http://www.liquibase.org/xml/ns/dbchangelog", fieldElement);
            XmlAttribute node = document.CreateAttribute("xsi", "schemaLocation", "http://www.w3.org/2001/XMLSchema-instance");
            node.Value = "http://www.liquibase.org/xml/ns/dbchangelog http://www.liquibase.org/xml/ns/dbchangelog/dbchangelog-3.1.xsd";
            fieldElement.Attributes.Append(node);
            XmlElement element2 = document.CreateElement("changeSet");
            fieldElement.AppendChild(element2);
            SetAttribute("id", Guid.NewGuid().ToString(), element2);
            SetAttribute("author", "Logitude", element2);
            XmlElement element3 = document.CreateElement("createTable");
            element2.AppendChild(element3);
            SetAttribute("tableName", table.DBTableName, element3);


            foreach (ObjectFieldsViewModel model in table.ObsList.Where(a => a.IsDeleted == false))
            {
                if (!model.IsDBField)
                {
                    continue;
                }
                XmlElement element4 = document.CreateElement("column");
                element3.AppendChild(element4);
                string attrValue = "";
                switch (model.FieldDataType)
                {
                    case "Decimal":
                    case "UnsDecimal":
                        attrValue = string.Concat(new object[] { "decimal(", model.NumberOfDigits, ",", model.DigitsAfterPoint, ")" });
                        break;

                    case "UnsInteger":
                    case "Integer":
                    case "Constant":
                        attrValue = "int";
                        break;

                    case "SigDouble":
                    case "Double":
                        attrValue = "float";
                        break;

                    case "nText":
                        attrValue = "nvarchar(" + (model.IsMaxLength ? "2000" : (second = model.MaxLength).ToString()) + ")";
                        break;

                    case "Text":
                    case "LookUp":
                        attrValue = "varchar(" + (model.IsMaxLength ? "2000" : (second = model.MaxLength).ToString()) + ")";
                        break;

                    case "Boolean":
                        attrValue = "bit";
                        break;

                    default:
                        attrValue = model.FieldDataType.ToLower();
                        break;
                }
                SetAttribute("name", model.FieldName, element4);
                SetAttribute("type", attrValue, element4);
                XmlElement element5 = document.CreateElement("constraints");
                element4.AppendChild(element5);
                if (model.IsNullable)
                {
                    SetAttribute("nullable", "true", element5);
                }
                else
                {
                    SetAttribute("nullable", "false", element5);
                }
                if (model.IsPrimaryKey)
                {
                    SetAttribute("primaryKey", "true", element5);
                }
                if (model.IsForeignKey)
                {
                    if (!IsTheForeignEntityCreated(model.ForeignEntity))
                    {
                        MessageBox.Show("The Script for the " + model.ForeignEntity + " table must be created First .");
                        return false;
                    }
                    string str3 = HashString(table.DBTableName + model.ForeignEntity + model.NavigationPropertyName + model.FieldName) + (((table.DBTableName + model.ForeignEntity + model.NavigationPropertyName + model.FieldName).Length > 14) ? (table.DBTableName + model.ForeignEntity + model.NavigationPropertyName + model.FieldName).Substring(0, 14) : (table.DBTableName + model.ForeignEntity + model.NavigationPropertyName + model.FieldName));
                    XmlElement element6 = document.CreateElement("changeSet");
                    fieldElement.AppendChild(element6);
                    SetAttribute("id", Guid.NewGuid().ToString(), element6);
                    SetAttribute("author", "Logitude", element6);
                    /***********************************************************/

                    var test = Directory.GetFiles(SolutionDirPath, model.ForeignEntity + ".lxml", SearchOption.AllDirectories);

                    if (!string.IsNullOrEmpty(test[0]))
                    {
                        //MessageBox.Show(App.DirectOpenPath);


                        // byte[] byteArray = new byte[stream.Length];
                        //stream.Read(byteArray, 0, byteArray.Length);
                        FileStream stream = null;
                        try
                        {
                            stream = new FileStream(test[0], FileMode.Open);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);

                        }

                        try
                        {

                            XmlDocument mydocument = new XmlDocument();
                            mydocument.Load(stream);
                            XmlParserHelper ParserHelper = new XmlParserHelper();
                            ObjectTableViewModel mymodel = ParserHelper.LoadObjectTableData(mydocument);
                            var ForeignEntity = mymodel.DBTableName;
                            var referencedColumn = mymodel.ObsList.Where(a => a.IsPrimaryKey).FirstOrDefault().FieldName;
                            XmlElement element7 = document.CreateElement("addForeignKeyConstraint");
                            element6.AppendChild(element7);
                            SetAttribute("baseColumnNames", model.FieldName, element7);
                            SetAttribute("baseTableName", table.DBTableName, element7);
                            SetAttribute("constraintName", HashString(table.DBTableName + model.ForeignEntity + model.NavigationPropertyName + model.FieldName) + (((table.DBTableName + model.ForeignEntity + model.NavigationPropertyName + model.FieldName).Length > 14) ? (table.DBTableName + model.ForeignEntity + model.NavigationPropertyName + model.FieldName).Substring(0, 14) : (table.DBTableName + model.ForeignEntity + model.NavigationPropertyName + model.FieldName)), element7);
                            //SetAttribute("onDelete", "CASCADE", element7);
                            //SetAttribute("onUpdate", "RESTRICT", element7);
                            SetAttribute("referencedColumnNames", referencedColumn, element7);
                            SetAttribute("referencedTableName", ForeignEntity, element7);

                            stream.Close();
                        }

                        catch (Exception err)
                        {

                        };

                    }
                    /***********************************************************/

                }
            }
            if (!FromOut)
            {
                CreatexmlScript(GeneratedSqlPath, GeneratedSqlPath + ((str.Length > 200) ? str.Substring(0, 200) : str) + ".xml", document);

            }
            if (table.IsNew || FromOut)
            {
                var FullTables = Directory.GetFiles(GeneratedFullSqlPath);
                var MyFile = FullTables.Where(a => a.Contains("CreateTable_" + table.DBTableName)).FirstOrDefault();
                if (!string.IsNullOrEmpty(MyFile))
                {
                    CreatexmlScript(GeneratedFullSqlPath, MyFile, document);
                }
                else
                {
                    CreatexmlScript(GeneratedFullSqlPath, GeneratedFullSqlPath + ((str.Length > 200) ? str.Substring(0, 200) : str) + ".xml", document);
                }
            }

            MessageBox.Show("Script Generated For this table");
            return true;
        }

        private static bool IsTheForeignEntityCreated(string ForeignEntity)
        {
            if (!Directory.Exists(GeneratedFullSqlPath))
            {
                return false;
            }
            var Files = Directory.GetFiles(GeneratedFullSqlPath).ToList();
            if (Files.Where(a => a.Contains(ForeignEntity)).Count() > 0)
            {
                return true;
            }
            return false;
        }

        // Ignore
        public static void GenerateSQLXmlForAlterColumns(NameFormVM ALterFieldData, string DBTableName)
        {
            string[] strArray = new string[]
            {
                DateTime.Now.Year.ToString(),
                DateTime.Now.Month.ToString().Length == 1 ? "0" + DateTime.Now.Month.ToString() : DateTime.Now.Month.ToString(),
                DateTime.Now.Day.ToString().Length == 1 ? "0" + DateTime.Now.Day.ToString() : DateTime.Now.Day.ToString(),
                DateTime.Now.Hour.ToString().Length == 1 ? "0" + DateTime.Now.Hour.ToString() : DateTime.Now.Hour.ToString(),
                DateTime.Now.Minute.ToString().Length == 1 ? "0" + DateTime.Now.Minute.ToString() : DateTime.Now.Minute.ToString(),
                DateTime.Now.Second.ToString().Length == 1 ? "0" + DateTime.Now.Second.ToString() : DateTime.Now.Second.ToString(),
                "AlterColumn",
                ALterFieldData.SelectedField.FieldName,
                "_InTable_",
                DBTableName
            };
            string str = string.Concat(strArray);
            XmlDocument document = new XmlDocument();
            XmlDeclaration newChild = document.CreateXmlDeclaration("1.0", "UTF-8", null);
            document.AppendChild(newChild);
            XmlElement fieldElement = (XmlElement)document.AppendChild(document.CreateElement("databaseChangeLog"));
            SetAttribute("xmlns", "http://www.liquibase.org/xml/ns/dbchangelog", fieldElement);
            XmlAttribute node = document.CreateAttribute("xsi", "schemaLocation", "http://www.w3.org/2001/XMLSchema-instance");
            node.Value = "http://www.liquibase.org/xml/ns/dbchangelog http://www.liquibase.org/xml/ns/dbchangelog/dbchangelog-3.1.xsd";
            fieldElement.Attributes.Append(node);
            XmlElement element2 = document.CreateElement("changeSet");
            fieldElement.AppendChild(element2);
            SetAttribute("id", Guid.NewGuid().ToString(), element2);
            SetAttribute("author", "Logitude", element2);
            if (ALterFieldData.SelectedField.IsDBField)
            {
                XmlElement element3 = document.CreateElement("renameColumn");
                element2.AppendChild(element3);
                string attrValue = "";
                switch (ALterFieldData.SelectedField.FieldDataType)
                {
                    case "Decimal":
                    case "UnsDecimal":
                        attrValue = string.Concat(new object[] { "decimal(", ALterFieldData.SelectedField.DigitsAfterPoint, ",", ALterFieldData.SelectedField.NumberOfDigits, ")" });
                        break;

                    case "UnsInteger":
                    case "Integer":
                    case "Constant":
                        attrValue = "int";
                        break;

                    case "SigDouble":
                    case "Double":
                        attrValue = "float";
                        break;

                    case "nText":
                        attrValue = "nvarchar(" + (ALterFieldData.SelectedField.IsMaxLength ? "2000" : ALterFieldData.SelectedField.MaxLength.ToString()) + ")";
                        break;

                    case "Text":
                    case "LookUp":
                        attrValue = "varchar(" + (ALterFieldData.SelectedField.IsMaxLength ? "2000" : ALterFieldData.SelectedField.MaxLength.ToString()) + ")";
                        break;

                    case "Boolean":
                        attrValue = "bit";
                        break;

                    default:
                        attrValue = ALterFieldData.SelectedField.FieldDataType.ToLower();
                        break;
                }
                SetAttribute("columnDataType", attrValue, element3);
                SetAttribute("newColumnName", ALterFieldData.SelectedField.FieldName, element3);
                SetAttribute("oldColumnName", ALterFieldData.OldName, element3);
                SetAttribute("tableName", DBTableName, element3);
                if (!ALterFieldData.IsNullable)
                {
                    element2 = document.CreateElement("changeSet");
                    fieldElement.AppendChild(element2);
                    SetAttribute("id", Guid.NewGuid().ToString(), element2);
                    SetAttribute("author", "Logitude", element2);
                    XmlElement element4 = document.CreateElement("addNotNullConstraint");
                    element2.AppendChild(element4);
                    SetAttribute("columnDataType", attrValue, element4);
                    SetAttribute("columnName", ALterFieldData.SelectedField.FieldName, element4);
                    SetAttribute("defaultNullValue", ALterFieldData.DefaultValue, element4);
                    SetAttribute("tableName", DBTableName, element4);
                }
                else if (ALterFieldData.IsNullable)
                {
                    element2 = document.CreateElement("changeSet");
                    fieldElement.AppendChild(element2);
                    SetAttribute("id", Guid.NewGuid().ToString(), element2);
                    SetAttribute("author", "Logitude", element2);
                    XmlElement element5 = document.CreateElement("dropNotNullConstraint");
                    element2.AppendChild(element5);
                    SetAttribute("columnDataType", attrValue, element5);
                    SetAttribute("columnName", ALterFieldData.SelectedField.FieldName, element5);
                    SetAttribute("tableName", DBTableName, element5);
                }
            }
            CreatexmlScript(GeneratedSqlPath, GeneratedSqlPath + ((str.Length > 200) ? str.Substring(0, 200) : str) + ".xml", document);
            MessageBox.Show("Script Generated For Alter Column");
        }
        private static void CreatexmlScript(string FolderPath, string FilePath, XmlDocument document)
        {
            if (!Directory.Exists(FolderPath))
            {
                Directory.CreateDirectory(FolderPath);
            }
            document.Save(FilePath);
        }
        public static void GenerateSQLXmlForColumns(ObjectTableViewModel Table, string dbTableName)
        {
            if (Table.IsNew)
            {
                var Continue = GenerateSqlXmlFileFromTool(Table);
                if (Continue)
                {
                    Table.IsNew = false;
                    GenerateXmlFileFromTool(Table);
                }

            }
            else
            {
                string[] strArray = new string[7];
                strArray[1] = DateTime.Now.Month.ToString().Length == 1 ? "0" + DateTime.Now.Month.ToString() : DateTime.Now.Month.ToString();
                strArray[2] = DateTime.Now.Day.ToString().Length == 1 ? "0" + DateTime.Now.Day.ToString() : DateTime.Now.Day.ToString();
                strArray[3] = DateTime.Now.Hour.ToString().Length == 1 ? "0" + DateTime.Now.Hour.ToString() : DateTime.Now.Hour.ToString();
                strArray[4] = DateTime.Now.Minute.ToString().Length == 1 ? "0" + DateTime.Now.Minute.ToString() : DateTime.Now.Minute.ToString();
                strArray[5] = DateTime.Now.Second.ToString().Length == 1 ? "0" + DateTime.Now.Second.ToString() : DateTime.Now.Second.ToString();
                int second = DateTime.Now.Second;
                //strArray[5] = second.ToString();
                //strArray[6] = "_Add_Alter_Fields_";
                string str = string.Concat(strArray);
                XmlDocument document = new XmlDocument();
                XmlDeclaration newChild = document.CreateXmlDeclaration("1.0", "UTF-8", null);
                document.AppendChild(newChild);
                XmlElement fieldElement = (XmlElement)document.AppendChild(document.CreateElement("databaseChangeLog"));
                SetAttribute("xmlns", "http://www.liquibase.org/xml/ns/dbchangelog", fieldElement);
                XmlAttribute node = document.CreateAttribute("xsi", "schemaLocation", "http://www.w3.org/2001/XMLSchema-instance");
                node.Value = "http://www.liquibase.org/xml/ns/dbchangelog http://www.liquibase.org/xml/ns/dbchangelog/dbchangelog-3.1.xsd";
                fieldElement.Attributes.Append(node);
                bool CreateScript = false;
                if (!string.IsNullOrEmpty(Table.OldDBTableName) && Table.OldDBTableName != Table.DBTableName)
                {
                    XmlElement element2 = document.CreateElement("changeSet");
                    fieldElement.AppendChild(element2);
                    SetAttribute("id", Guid.NewGuid().ToString(), element2);
                    SetAttribute("author", "Logitude", element2);
                    XmlElement element3 = document.CreateElement("renameTable");
                    element2.AppendChild(element3);
                    SetAttribute("newTableName", Table.DBTableName, element3);
                    SetAttribute("oldTableName", Table.OldDBTableName, element3);
                    Table.IsDirty = false;
                    Table.OldDBTableName = Table.DBTableName;
                    str = str + "AlterTable_" + Table.OldDBTableName + "RenameTo_" + Table.DBTableName;
                    CreateScript = true;
                }
                List<ObjectFieldsViewModel> Fields = Table.CheckedObjectFields;
                if (Fields.Count > 0)
                {
                    str = str + "_AddAlterColumns";
                    //XmlElement elementrenameColumn = document.CreateElement("renameColumn");

                    foreach (ObjectFieldsViewModel model in Fields)
                    {
                        if (!model.IsDBField)
                        {
                            continue;
                        }
                        if (model.IsNew && !model.IsDeleted)
                        {
                            CreateScript = true;
                            XmlElement element2 = document.CreateElement("changeSet");
                            fieldElement.AppendChild(element2);
                            SetAttribute("id", Guid.NewGuid().ToString(), element2);
                            SetAttribute("author", "Logitude", element2);
                            XmlElement element3 = document.CreateElement("addColumn");
                            element2.AppendChild(element3);
                            SetAttribute("tableName", dbTableName, element3);
                            str = str + "_" + model.FieldName;
                            XmlElement element4 = document.CreateElement("column");
                            element3.AppendChild(element4);
                            SetAttribute("name", model.FieldName, element4);
                            string attrValue = "";
                            switch (model.FieldDataType)
                            {
                                case "Decimal":
                                case "UnsDecimal":
                                    attrValue = string.Concat(new object[] { "decimal(", model.DigitsAfterPoint, ",", model.NumberOfDigits, ")" });
                                    break;

                                case "UnsInteger":
                                case "Integer":
                                case "Constant":
                                    attrValue = "int";
                                    break;

                                case "SigDouble":
                                case "Double":
                                    attrValue = "float";
                                    break;

                                case "nText":
                                    attrValue = "nvarchar(" + (model.IsMaxLength ? "2000" : (second = model.MaxLength).ToString()) + ")";
                                    break;

                                case "Text":
                                case "LookUp":
                                    attrValue = "varchar(" + (model.IsMaxLength ? "2000" : (second = model.MaxLength).ToString()) + ")";
                                    break;

                                case "Boolean":
                                    attrValue = "bit";
                                    break;

                                default:
                                    attrValue = model.FieldDataType.ToLower();
                                    break;
                            }
                            SetAttribute("type", attrValue, element4);
                            XmlElement element5 = document.CreateElement("constraints");
                            element4.AppendChild(element5);
                            if (model.IsNullable)
                            {
                                SetAttribute("nullable", "true", element5);
                            }
                            else
                            {
                                SetAttribute("nullable", "false", element5);
                            }
                            if (model.IsPrimaryKey)
                            {
                                SetAttribute("primaryKey", "true", element5);
                            }

                            //if (model.IsPrimaryKey)
                            //{
                            //    if (!IsTheForeignEntityCreated(model.ForeignEntity))
                            //    {
                            //        MessageBox.Show("The Script for the " + model.ForeignEntity + " table must be created First .");
                            //        return;
                            //    }
                            //    string str3 = HashString(Table.DBTableName + model.ForeignEntity + model.NavigationPropertyName + model.FieldName) + (((Table.DBTableName + model.ForeignEntity + model.NavigationPropertyName + model.FieldName).Length > 14) ? (Table.DBTableName + model.ForeignEntity + model.NavigationPropertyName + model.FieldName).Substring(0, 14) : (Table.DBTableName + model.ForeignEntity + model.NavigationPropertyName + model.FieldName));
                            //    XmlElement element6 = document.CreateElement("changeSet");
                            //    fieldElement.AppendChild(element6);
                            //    SetAttribute("id", Guid.NewGuid().ToString(), element6);
                            //    SetAttribute("author", "Logitude", element6);
                            //    XmlElement element7 = document.CreateElement("addForeignKeyConstraint");
                            //    element6.AppendChild(element7);
                            //    SetAttribute("baseColumnNames", model.FieldName, element7);
                            //    SetAttribute("baseTableName", Table.DBTableName, element7);
                            //    SetAttribute("constraintName", HashString(Table.DBTableName + model.ForeignEntity + model.NavigationPropertyName + model.FieldName) + (((Table.DBTableName + model.ForeignEntity + model.NavigationPropertyName + model.FieldName).Length > 14) ? (Table.DBTableName + model.ForeignEntity + model.NavigationPropertyName + model.FieldName).Substring(0, 14) : (Table.DBTableName + model.ForeignEntity + model.NavigationPropertyName + model.FieldName)), element7);
                            //    //SetAttribute("onDelete", "CASCADE", element7);
                            //    //SetAttribute("onUpdate", "RESTRICT", element7);
                            //    SetAttribute("referencedColumnNames", model.NavigationPropertyName, element7);
                            //    SetAttribute("referencedTableName", model.ForeignEntity, element7);
                            //}
                            if (model.IsForeignKey)
                            {
                                if (!IsTheForeignEntityCreated(model.ForeignEntity))
                                {
                                    MessageBox.Show("The Script for the " + model.ForeignEntity + " table must be created First .");
                                    return;
                                }
                                string str3 = HashString(Table.DBTableName + model.ForeignEntity + model.NavigationPropertyName + model.FieldName) + (((Table.DBTableName + model.ForeignEntity + model.NavigationPropertyName + model.FieldName).Length > 14) ? (Table.DBTableName + model.ForeignEntity + model.NavigationPropertyName + model.FieldName).Substring(0, 14) : (Table.DBTableName + model.ForeignEntity + model.NavigationPropertyName + model.FieldName));
                                XmlElement element6 = document.CreateElement("changeSet");
                                fieldElement.AppendChild(element6);
                                SetAttribute("id", Guid.NewGuid().ToString(), element6);
                                SetAttribute("author", "Logitude", element6);
                                /***********************************************************/

                                var test = Directory.GetFiles(SolutionDirPath, model.ForeignEntity + ".lxml", SearchOption.AllDirectories);

                                if (!string.IsNullOrEmpty(test[0]))
                                {
                                    //MessageBox.Show(App.DirectOpenPath);


                                    // byte[] byteArray = new byte[stream.Length];
                                    //stream.Read(byteArray, 0, byteArray.Length);
                                    FileStream stream = null;
                                    try
                                    {
                                        stream = new FileStream(test[0], FileMode.Open);
                                    }
                                    catch (Exception ex)
                                    {
                                        MessageBox.Show(ex.Message);

                                    }

                                    try
                                    {

                                        XmlDocument mydocument = new XmlDocument();
                                        mydocument.Load(stream);
                                        XmlParserHelper ParserHelper = new XmlParserHelper();
                                        ObjectTableViewModel mymodel = ParserHelper.LoadObjectTableData(mydocument);
                                        var ForeignEntity = mymodel.DBTableName;
                                        var referencedColumn = mymodel.ObsList.Where(a => a.IsPrimaryKey).FirstOrDefault().FieldName; ;
                                        XmlElement element7 = document.CreateElement("addForeignKeyConstraint");
                                        element6.AppendChild(element7);
                                        SetAttribute("baseColumnNames", model.FieldName, element7);
                                        SetAttribute("baseTableName", Table.DBTableName, element7);
                                        SetAttribute("constraintName", HashString(Table.DBTableName + model.ForeignEntity + model.NavigationPropertyName + model.FieldName) + (((Table.DBTableName + model.ForeignEntity + model.NavigationPropertyName + model.FieldName).Length > 14) ? (Table.DBTableName + model.ForeignEntity + model.NavigationPropertyName + model.FieldName).Substring(0, 14) : (Table.DBTableName + model.ForeignEntity + model.NavigationPropertyName + model.FieldName)), element7);
                                        //SetAttribute("onDelete", "CASCADE", element7);
                                        //SetAttribute("onUpdate", "RESTRICT", element7);
                                        SetAttribute("referencedColumnNames", referencedColumn, element7);
                                        SetAttribute("referencedTableName", ForeignEntity, element7);

                                        stream.Close();
                                    }

                                    catch (Exception err)
                                    {

                                    };

                                }
                                /***********************************************************/

                            }
                            Table.ObsList.Where(a => a.FieldName == model.FieldName).FirstOrDefault().IsNew = false;
                            Table.ObsList.Where(a => a.FieldName == model.FieldName).FirstOrDefault().IsChecked = false;
                        }
                        else if (model.IsChecked && model.IsDBField && !model.IsDeleted)
                        {
                            CreateScript = true;
                            str = str + "_" + model.FieldName;
                            XmlElement element2 = document.CreateElement("changeSet");
                            fieldElement.AppendChild(element2);
                            SetAttribute("id", Guid.NewGuid().ToString(), element2);
                            SetAttribute("author", "Logitude", element2);


                            //XmlElement elementmodifyDataType = document.CreateElement("modifyDataType");
                            //element2.AppendChild(elementmodifyDataType);

                            string attrValue = "";
                            switch (model.FieldDataType)
                            {
                                case "Decimal":
                                case "UnsDecimal":
                                    attrValue = string.Concat(new object[] { "decimal(", model.DigitsAfterPoint, ",", model.NumberOfDigits, ")" });
                                    break;

                                case "UnsInteger":
                                case "Integer":
                                case "Constant":
                                    attrValue = "int";
                                    break;

                                case "SigDouble":
                                case "Double":
                                    attrValue = "float";
                                    break;

                                case "nText":
                                    attrValue = "nvarchar(" + (model.IsMaxLength ? "2000" : model.MaxLength.ToString()) + ")";
                                    break;

                                case "Text":
                                case "LookUp":
                                    attrValue = "varchar(" + (model.IsMaxLength ? "2000" : model.MaxLength.ToString()) + ")";
                                    break;

                                case "Boolean":
                                    attrValue = "bit";
                                    break;

                                default:
                                    attrValue = model.FieldDataType.ToLower();
                                    break;
                            }
                            if (model.FieldName != model.OldFieldName)
                            {
                                XmlElement element3 = document.CreateElement("renameColumn");
                                element2.AppendChild(element3);
                                SetAttribute("columnDataType", attrValue, element3);
                                SetAttribute("newColumnName", model.FieldName, element3);
                                SetAttribute("oldColumnName", model.OldFieldName, element3);
                                SetAttribute("tableName", dbTableName, element3);
                            }
                            //else if (model.IsPrimaryKey != model.OldIsPrimaryKey)
                            //{
                            //    SetAttribute("primaryKey", "true", element5);
                            //}
                            else if (model.FieldDataType != model.OldFieldDataType)
                            {
                                XmlElement element3 = document.CreateElement("modifyDataType");
                                element2.AppendChild(element3);
                                SetAttribute("columnName", model.FieldName, element3);
                                SetAttribute("newDataType", attrValue, element3);
                                SetAttribute("tableName", dbTableName, element3);
                            }

                            //SetAttribute("columnDataType", attrValue, elementmodifyDataType);
                            //SetAttribute("newColumnName", model.FieldName, elementmodifyDataType);
                            //SetAttribute("oldColumnName", model.OldFieldName, elementmodifyDataType);
                            //SetAttribute("tableName", dbTableName, elementmodifyDataType);
                            /*
                             <dropPrimaryKey catalogName="cat"
            constraintName="const_name"
            schemaName="public"
            tableName="person"/>
                             */
                            /*
                             <addPrimaryKey catalogName="cat"
            columnNames="id, name"
            constraintName="pk_person"
            schemaName="public"
            tableName="person"
            tablespace="A String"/>
                             */
                            if (model.IsPrimaryKey != model.OldIsPrimaryKey)
                            {
                                //if (!model.IsPrimaryKey)
                                //{
                                element2 = document.CreateElement("changeSet");
                                fieldElement.AppendChild(element2);
                                SetAttribute("id", Guid.NewGuid().ToString(), element2);
                                SetAttribute("author", "Logitude", element2);
                                XmlElement element4 = document.CreateElement("dropPrimaryKey");
                                element2.AppendChild(element4);
                                SetAttribute("constraintName", "PK_" + Table.DBTableName.ToUpper(), element4);
                                //SetAttribute("columnName", model.FieldName, element4);
                                //SetAttribute("defaultNullValue", model.DefaultValue, element4);
                                SetAttribute("tableName", dbTableName, element4);
                                var PrimaryKeyColumns = "";
                                foreach (var item in Table.ObsList.Where(a => a.IsPrimaryKey == true))
                                {
                                    PrimaryKeyColumns += item.FieldName + ",";
                                }
                                PrimaryKeyColumns = PrimaryKeyColumns.TrimEnd(',');
                                element2 = document.CreateElement("changeSet");
                                fieldElement.AppendChild(element2);
                                SetAttribute("id", Guid.NewGuid().ToString(), element2);
                                SetAttribute("author", "Logitude", element2);
                                XmlElement addPrimaryKeyelement = document.CreateElement("addPrimaryKey");
                                element2.AppendChild(addPrimaryKeyelement);
                                SetAttribute("columnNames", PrimaryKeyColumns, addPrimaryKeyelement);
                                SetAttribute("constraintName", "PK_" + Table.DBTableName.ToUpper(), addPrimaryKeyelement);
                                //SetAttribute("columnName", model.FieldName, element4);
                                //SetAttribute("defaultNullValue", model.DefaultValue, element4);
                                SetAttribute("tableName", dbTableName, addPrimaryKeyelement);
                                //}
                                //else if (model.IsNullable)
                                //{
                                //    element2 = document.CreateElement("changeSet");
                                //    fieldElement.AppendChild(element2);
                                //    SetAttribute("id", Guid.NewGuid().ToString(), element2);
                                //    SetAttribute("author", "Logitude", element2);
                                //    XmlElement element5 = document.CreateElement("dropNotNullConstraint");
                                //    element2.AppendChild(element5);
                                //    SetAttribute("columnDataType", attrValue, element5);
                                //    SetAttribute("columnName", model.FieldName, element5);
                                //    SetAttribute("tableName", dbTableName, element5);
                                //}
                            }
                            if (model.IsNullable != model.OldIsNullable)
                            {
                                if (!model.IsNullable)
                                {
                                    element2 = document.CreateElement("changeSet");
                                    fieldElement.AppendChild(element2);
                                    SetAttribute("id", Guid.NewGuid().ToString(), element2);
                                    SetAttribute("author", "Logitude", element2);
                                    XmlElement element4 = document.CreateElement("addNotNullConstraint");
                                    element2.AppendChild(element4);
                                    SetAttribute("columnDataType", attrValue, element4);
                                    SetAttribute("columnName", model.FieldName, element4);
                                    //SetAttribute("defaultNullValue", model.DefaultValue, element4);
                                    SetAttribute("tableName", dbTableName, element4);
                                }
                                else if (model.IsNullable)
                                {
                                    element2 = document.CreateElement("changeSet");
                                    fieldElement.AppendChild(element2);
                                    SetAttribute("id", Guid.NewGuid().ToString(), element2);
                                    SetAttribute("author", "Logitude", element2);
                                    XmlElement element5 = document.CreateElement("dropNotNullConstraint");
                                    element2.AppendChild(element5);
                                    SetAttribute("columnDataType", attrValue, element5);
                                    SetAttribute("columnName", model.FieldName, element5);
                                    SetAttribute("tableName", dbTableName, element5);
                                }
                            }
                            Table.ObsList.Where(a => a.FieldName == model.FieldName).FirstOrDefault().IsChecked = false;
                            Table.ObsList.Where(a => a.FieldName == model.FieldName).FirstOrDefault().OldFieldName = Table.ObsList.Where(a => a.FieldName == model.FieldName).FirstOrDefault().FieldName;
                        }
                        else if (model.IsDeleted)
                        {
                            CreateScript = true;
                            XmlElement DeletedElement;
                            if (model.IsForeignKey && model.IsDBField)
                            {
                                DeletedElement = document.CreateElement("changeSet");
                                fieldElement.AppendChild(DeletedElement);
                                SetAttribute("id", Guid.NewGuid().ToString(), DeletedElement);
                                SetAttribute("author", "Logitude", DeletedElement);
                                XmlElement element3 = document.CreateElement("dropForeignKeyConstraint");
                                DeletedElement.AppendChild(element3);
                                SetAttribute("baseTableName", Table.DBTableName, element3);
                                SetAttribute("constraintName", HashString(Table.DBTableName + model.ForeignEntity + model.NavigationPropertyName + model.FieldName) + (((Table.DBTableName + model.ForeignEntity + model.NavigationPropertyName + model.FieldName).Length > 14) ? (Table.DBTableName + model.ForeignEntity + model.NavigationPropertyName + model.FieldName).Substring(0, 14) : (Table.DBTableName + model.ForeignEntity + model.NavigationPropertyName + model.FieldName)), element3);
                            }
                            if (model.IsDBField)
                            {
                                str = str + "_" + model.FieldName;
                                DeletedElement = document.CreateElement("changeSet");
                                fieldElement.AppendChild(DeletedElement);
                                SetAttribute("id", Guid.NewGuid().ToString(), DeletedElement);
                                SetAttribute("author", "Logitude", DeletedElement);
                                XmlElement element4 = document.CreateElement("dropColumn");
                                DeletedElement.AppendChild(element4);
                                SetAttribute("columnName", model.FieldName, element4);
                                SetAttribute("tableName", Table.DBTableName, element4);
                            }
                            Table.ObsList.Remove(model);
                        }

                    }
                }
                if (CreateScript)
                {
                    GenerateXmlFileFromTool(Table);
                    CreatexmlScript(GeneratedSqlPath, GeneratedSqlPath + ((str.Length > 200) ? str.Substring(0, 200) : str) + ".xml", document);
                    MessageBox.Show("Script Generated For Selected Columns");
                    GenerateSqlXmlFileFromTool(Table, true);
                }
                else
                {
                    MessageBox.Show("There are no changes on this table");
                }

            }
        }

        public static void GenerateSQLXmlForDropColumns(List<ObjectFieldsViewModel> Fields, string DBTableName)
        {
            //string str = DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Day.ToString() + DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString() + DateTime.Now.Second.ToString() + "DropColumns_InTable_" + DBTableName;
            //XmlDocument document = new XmlDocument();
            //XmlDeclaration newChild = document.CreateXmlDeclaration("1.0", "UTF-8", null);
            //document.AppendChild(newChild);
            //XmlElement fieldElement = (XmlElement)document.AppendChild(document.CreateElement("databaseChangeLog"));
            //SetAttribute("xmlns", "http://www.liquibase.org/xml/ns/dbchangelog", fieldElement);
            //XmlAttribute node = document.CreateAttribute("xsi", "schemaLocation", "http://www.w3.org/2001/XMLSchema-instance");
            //node.Value = "http://www.liquibase.org/xml/ns/dbchangelog http://www.liquibase.org/xml/ns/dbchangelog/dbchangelog-3.1.xsd";
            //fieldElement.Attributes.Append(node);
            //foreach (ObjectFieldsViewModel model in Fields)
            //{
            //    XmlElement element2;
            //    if (model.IsForeignKey && model.IsDBField)
            //    {
            //        element2 = document.CreateElement("changeSet");
            //        fieldElement.AppendChild(element2);
            //        SetAttribute("id", Guid.NewGuid().ToString(), element2);
            //        SetAttribute("author", "Logitude", element2);
            //        XmlElement element3 = document.CreateElement("dropForeignKeyConstraint");
            //        element2.AppendChild(element3);
            //        SetAttribute("baseTableName", DBTableName, element3);
            //        SetAttribute("constraintName", HashString(DBTableName + model.ForeignEntity + model.NavigationPropertyName + model.FieldName) + (((DBTableName + model.ForeignEntity + model.NavigationPropertyName + model.FieldName).Length > 14) ? (DBTableName + model.ForeignEntity + model.NavigationPropertyName + model.FieldName).Substring(0, 14) : (DBTableName + model.ForeignEntity + model.NavigationPropertyName + model.FieldName)), element3);
            //    }
            //    if (model.IsDBField)
            //    {
            //        str = str + "_" + model.FieldName;
            //        element2 = document.CreateElement("changeSet");
            //        fieldElement.AppendChild(element2);
            //        SetAttribute("id", Guid.NewGuid().ToString(), element2);
            //        SetAttribute("author", "Logitude", element2);
            //        XmlElement element4 = document.CreateElement("dropColumn");
            //        element2.AppendChild(element4);
            //        SetAttribute("columnName", model.FieldName, element4);
            //        SetAttribute("tableName", DBTableName, element4);
            //    }
            //}
            //document.Save(GeneratedSqlPath + ((str.Length > 200) ? str.Substring(0, 200) : str) + ".xml");
            //MessageBox.Show("Script Generated For Rename Selected Columns");
        }

        public static void GenerateSQLXmlForRenameColumns(NameFormVM nameFormVM, string DBTableName)
        {
            //    string[] strArray = new string[] { DateTime.Now.Year.ToString(), DateTime.Now.Month.ToString(), DateTime.Now.Day.ToString(), DateTime.Now.Hour.ToString(), DateTime.Now.Minute.ToString(), DateTime.Now.Second.ToString(), "_RenameColumnFrom_", nameFormVM.OldName, "_To_", nameFormVM.SelectedField.FieldName, "_InTable_", DBTableName };
            //    string str = string.Concat(strArray);
            //    XmlDocument document = new XmlDocument();
            //    XmlDeclaration newChild = document.CreateXmlDeclaration("1.0", "UTF-8", null);
            //    document.AppendChild(newChild);
            //    XmlElement fieldElement = (XmlElement)document.AppendChild(document.CreateElement("databaseChangeLog"));
            //    SetAttribute("xmlns", "http://www.liquibase.org/xml/ns/dbchangelog", fieldElement);
            //    XmlAttribute node = document.CreateAttribute("xsi", "schemaLocation", "http://www.w3.org/2001/XMLSchema-instance");
            //    node.Value = "http://www.liquibase.org/xml/ns/dbchangelog http://www.liquibase.org/xml/ns/dbchangelog/dbchangelog-3.1.xsd";
            //    fieldElement.Attributes.Append(node);
            //    XmlElement element2 = document.CreateElement("changeSet");
            //    fieldElement.AppendChild(element2);
            //    SetAttribute("id", Guid.NewGuid().ToString(), element2);
            //    SetAttribute("author", "Logitude", element2);
            //    if (nameFormVM.SelectedField.IsDBField)
            //    {
            //        XmlElement element3 = document.CreateElement("renameColumn");
            //        element2.AppendChild(element3);
            //        string attrValue = "";
            //        switch (nameFormVM.SelectedField.FieldDataType)
            //        {
            //            case "Decimal":
            //            case "UnsDecimal":
            //                attrValue = string.Concat(new object[] { "decimal(", nameFormVM.SelectedField.DigitsAfterPoint, ",", nameFormVM.SelectedField.NumberOfDigits, ")" });
            //                break;

            //            case "UnsInteger":
            //            case "Integer":
            //            case "Constant":
            //                attrValue = "int";
            //                break;

            //            case "SigDouble":
            //            case "Double":
            //                attrValue = "float";
            //                break;

            //            case "nText":
            //                attrValue = "nvarchar(" + (nameFormVM.SelectedField.IsMaxLength ? "2000" : nameFormVM.SelectedField.MaxLength.ToString()) + ")";
            //                break;

            //            case "Text":
            //            case "LookUp":
            //                attrValue = "varchar(" + (nameFormVM.SelectedField.IsMaxLength ? "2000" : nameFormVM.SelectedField.MaxLength.ToString()) + ")";
            //                break;

            //            case "Boolean":
            //                attrValue = "bit";
            //                break;

            //            default:
            //                attrValue = nameFormVM.SelectedField.FieldDataType.ToLower();
            //                break;
            //        }
            //        SetAttribute("columnDataType", attrValue, element3);
            //        SetAttribute("newColumnName", nameFormVM.SelectedField.FieldName, element3);
            //        SetAttribute("oldColumnName", nameFormVM.OldName, element3);
            //        SetAttribute("tableName", DBTableName, element3);
            //    }
            //    document.Save(GeneratedSqlPath + str.Substring(0, 200) + ".xml");
            //    MessageBox.Show("Script Generated For Rename Selected Columns");
        }

        public static void GenerateSQLXmlForRenameTable(string DBTableName, string OldTableName)
        {
            //string str = DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Day.ToString() + DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString() + DateTime.Now.Second.ToString() + "_RenameTableFrom_" + OldTableName + "_To_" + DBTableName;
            //XmlDocument document = new XmlDocument();
            //XmlDeclaration newChild = document.CreateXmlDeclaration("1.0", "UTF-8", null);
            //document.AppendChild(newChild);
            //XmlElement fieldElement = (XmlElement)document.AppendChild(document.CreateElement("databaseChangeLog"));
            //SetAttribute("xmlns", "http://www.liquibase.org/xml/ns/dbchangelog", fieldElement);
            //XmlAttribute node = document.CreateAttribute("xsi", "schemaLocation", "http://www.w3.org/2001/XMLSchema-instance");
            //node.Value = "http://www.liquibase.org/xml/ns/dbchangelog http://www.liquibase.org/xml/ns/dbchangelog/dbchangelog-3.1.xsd";
            //fieldElement.Attributes.Append(node);
            //XmlElement element2 = document.CreateElement("changeSet");
            //fieldElement.AppendChild(element2);
            //SetAttribute("id", Guid.NewGuid().ToString(), element2);
            //SetAttribute("author", "Logitude", element2);
            //XmlElement element3 = document.CreateElement("renameTable");
            //element2.AppendChild(element3);
            //SetAttribute("newTableName", DBTableName, element3);
            //SetAttribute("oldTableName", OldTableName, element3);
            //document.Save(GeneratedSqlPath + ((str.Length > 200) ? str.Substring(0, 200) : str) + ".xml");
            //MessageBox.Show("Script Generated For Table Rename");
        }

        #endregion

        #region GenerateXmlFileFromTool

        public static void GenerateXmlFileFromTool(ObjectTableViewModel table)//execute when click ok button
        {
            XmlDocument doc = new XmlDocument();
            XmlDeclaration xmlDeclaration = doc.CreateXmlDeclaration("1.0", "UTF-8", null);
            doc.AppendChild(xmlDeclaration);
            XmlElement entityElement = (XmlElement)doc.AppendChild(doc.CreateElement("entity"));


            #region ObjectTable Properties

            if (string.IsNullOrEmpty(table.Id))
            {
                SetAttribute("Id", GetStringValue(Guid.NewGuid()), entityElement, null);
            }
            else
            {
                SetAttribute("Id", GetStringValue(table.Id), entityElement, null);
            }
            SetAttribute("ObjectTableName", GetStringValue(table.ObjectTableName), entityElement);
            SetAttribute("IsNew", table.IsNew.ToString().ToLower(), entityElement);

            SetAttribute("DBTableName", GetStringValue(table.DBTableName), entityElement);

            if (!string.IsNullOrEmpty(table.DBTableShortName))
            {
                SetAttribute("DBTableShortName", GetStringValue(table.DBTableShortName), entityElement);
            }


            string dbTableOldNames = null;

            if (string.IsNullOrEmpty(table.DBTableOldNames))
            {
                string shortName = string.IsNullOrEmpty(table.DBTableShortName) ? null : "," + table.DBTableShortName;
                dbTableOldNames = table.DBTableName + shortName;
            }
            else
            {
                dbTableOldNames = table.DBTableOldNames;

                if (table.DBTableOldNames.Contains(","))
                {
                    if (!table.DBTableOldNames.Split(',').Contains(table.DBTableName))
                    {
                        dbTableOldNames = dbTableOldNames + "," + table.DBTableName;
                    }

                    if (!string.IsNullOrEmpty(table.DBTableShortName))
                    {
                        if (!table.DBTableOldNames.Split(',').Contains(table.DBTableShortName))
                        {
                            dbTableOldNames = dbTableOldNames + "," + table.DBTableShortName;
                        }
                    }
                }
                else
                {
                    string shortName = string.IsNullOrEmpty(table.DBTableShortName) ? null : "," + table.DBTableShortName;

                    if (table.DBTableOldNames != table.DBTableName)
                    {
                        dbTableOldNames = dbTableOldNames + "," + table.DBTableName + shortName;
                    }
                    else
                    {
                        dbTableOldNames = dbTableOldNames + shortName;
                    }
                }
            }

            SetAttribute("DBTableOldNames", dbTableOldNames.Contains(",") ? string.Join(",", dbTableOldNames.Split(',').Distinct().ToArray()) : dbTableOldNames, entityElement);

            SetAttribute("ObjectTableSingular", GetStringValue(table.ObjectTableSingular), entityElement);
            SetAttribute("ObjectTablePlural", GetStringValue(table.ObjectTablePlural), entityElement);

            if (!string.IsNullOrEmpty(table.DescriptionDefaultText))
            {
                SetAttribute("DescriptionDefaultText", GetStringValue(table.DescriptionDefaultText), entityElement);
            }
            else
            {
                RemoveAttribute("DescriptionDefaultText", entityElement);
            }

            if (!string.IsNullOrEmpty(table.DescriptionLocalDefaultText))
            {
                SetAttribute("DescriptionLocalDefaultText", GetStringValue(table.DescriptionLocalDefaultText), entityElement);
            }
            else
            {
                RemoveAttribute("DescriptionLocalDefaultText", entityElement);
            }


            SetAttribute("HasCustomFilter", table.HasCustomFilter.ToString().ToLower(), entityElement);
            SetAttribute("HasCustomFields", table.HasCustomFields.ToString().ToLower(), entityElement);
            SetAttribute("AvailableInCustomization", table.AvailableInCustomization.ToString().ToLower(), entityElement);
            SetAttribute("SupportSubEntity", table.SupportSubEntity.ToString().ToLower(), entityElement);
            SetAttribute("ApplyGenericCustomFields", table.ApplyGenericCustomFields.ToString().ToLower(), entityElement);
            SetAttribute("AvailableInDocumentTypes", table.AvailableInDocumentTypes.ToString().ToLower(), entityElement);
			SetAttribute("IsLock", table.IsLock.ToString().ToLower(), entityElement);
			SetAttribute("RelatedEntity", GetStringValue(table.RelatedEntity), entityElement);
			SetAttribute("ThisKey", GetStringValue(table.ThisKey), entityElement);
			SetAttribute("RelatedKey", GetStringValue(table.RelatedKey), entityElement);

			SetAttribute("HasHelper", table.HasHelper.ToString().ToLower(), entityElement);
            SetAttribute("HasShortTitle", table.HasShortTitle.ToString().ToLower(), entityElement);
            SetAttribute("HasFiltersMenu", table.HasFiltersMenu.ToString().ToLower(), entityElement);

            SetAttribute("HasCustomValidator", table.HasCustomValidator.ToString().ToLower(), entityElement);
            SetAttribute("IsEditable", table.IsEditable.ToString().ToLower(), entityElement);
            SetAttribute("IsNewWizard", table.IsNewWizard.ToString().ToLower(), entityElement);
            SetAttribute("LookUp1", GetStringValue(table.LookUp1), entityElement);
            SetAttribute("LookUp2", GetStringValue(table.LookUp2), entityElement);
            SetAttribute("CodeField", GetStringValue(table.CodeField), entityElement);
            SetAttribute("NameField", GetStringValue(table.NameField), entityElement);
            SetAttribute("LovDisplayMemberPath", GetStringValue(table.LovDisplayMemberPath), entityElement);
            SetAttribute("LovDisplayMemberPathLocal", GetStringValue(table.LovDisplayMemberPathLocal), entityElement);
            SetAttribute("DependencyFilter1", GetStringValue(table.DependencyFilter1), entityElement);
            SetAttribute("DependencyFilter2", GetStringValue(table.DependencyFilter2), entityElement);
            SetAttribute("DependencyFilter3", GetStringValue(table.DependencyFilter3), entityElement);

            SetAttribute("KeyPropertyPath", GetStringValue(table.KeyPropertyPath), entityElement);
            SetAttribute("AutoCompleteSearchWindow", table.AutoCompleteSearchWindow.ToString().ToLower(), entityElement);
            SetAttribute("IsClosed", table.IsClosed.ToString().ToLower(), entityElement);
            SetAttribute("CacheOnClient", table.CacheOnClient.ToString().ToLower(), entityElement);
            SetAttribute("EditableFromAutoCompleteWindow", table.EditableFromAutoCompleteWindow.ToString().ToLower(), entityElement);
            SetAttribute("HasCounter", table.HasCounter.ToString().ToLower(), entityElement);
            // SetAttribute("EnableEditFromLOV", table.EnableEditFromLOV.ToString().ToLower(), entityElement);

            SetAttribute("EnableAddFromLOV", table.EnableEditFromLOV.ToString().ToLower(), entityElement);
            SetAttribute("IsRestrictable", table.IsRestrictable.ToString().ToLower(), entityElement);
            SetAttribute("IsMain", table.IsMain.ToString().ToLower(), entityElement);
            SetAttribute("IsAutoComplete", table.IsAutoComplete.ToString().ToLower(), entityElement);
            SetAttribute("EnableEditFromLOV", table.EnableEditFromLOV.ToString().ToLower(), entityElement);
            SetAttribute("EnableAddFromLOV", table.EnableAddFromLOV.ToString().ToLower(), entityElement);

            SetAttribute("SortingByObjectField", GetStringValue(table.SortingByObjectField), entityElement);
            SetAttribute("SortingByDirection", GetStringValue(table.SortingByDirection), entityElement);
            SetAttribute("InActive", table.InActive.ToString().ToLower(), entityElement);
            //SetAttribute("SearchFields", GetStringValue(table.SearchFields), entityElement);
            SetAttribute("ShortTitleControlPath", GetStringValue(table.ShortTitleControlPath), entityElement);

            SetAttribute("IsSaveButtonVisible", table.IsSaveButtonVisible.ToString().ToLower(), entityElement);
            SetAttribute("IsComposition", table.IsComposition.ToString().ToLower(), entityElement);
            SetAttribute("EnableSecurity", table.EnableSecurity.ToString().ToLower(), entityElement);
            SetAttribute("AllowCustomFields", table.AllowCustomFields.ToString().ToLower(), entityElement);
            SetAttribute("HasDynamicHeader", table.HasDynamicHeader.ToString().ToLower(), entityElement);

            SetAttribute("MainTipCode", GetStringValue(table.MainTipCode), entityElement);
            SetAttribute("ObjectTableTypeCode", GetStringValue(table.ObjectTableTypeCode), entityElement);
            SetAttribute("MaxNumberOfCustomFields", table.MaxNumberOfCustomFields.ToString(), entityElement);
            SetAttribute("ParentTableName", table.ParentTableName, entityElement);
            SetAttribute("NewWizardControlName", GetStringValue(table.NewWizardControlName), entityElement);
            SetAttribute("LocalDefaultText", GetStringValue(table.LocalDefaultText), entityElement);
            SetAttribute("DefaultText", GetStringValue(table.DefaultText), entityElement);
            SetAttribute("NewButtonLocalDefaultText", GetStringValue(table.NewButtonLocalDefaultText), entityElement);
            SetAttribute("NewButtonDefaultText", GetStringValue(table.NewButtonDefaultText), entityElement);
            SetAttribute("Code", GetStringValue(table.QueryGroupCode), entityElement, null);
            SetAttribute("Name", GetStringValue(table.QueryGroupName), entityElement, null);
            SetAttribute("CloseTableCode", GetStringValue(table.CloseTableCode), entityElement, null);
            SetAttribute("CloseTableName", GetStringValue(table.CloseTableName), entityElement, null);
            SetAttribute("GenerateDomainService", table.GenerateDomainService.ToString().ToLower(), entityElement);
            SetAttribute("ClientModuleName", GetStringValue(table.ClientModuleName), entityElement);
            SetAttribute("ServerModuleName", GetStringValue(table.ServerModuleName), entityElement);
            SetAttribute("NewWizardComponentPath", GetStringValue(table.NewWizardComponentPath), entityElement);
            SetAttribute("NoViewController", table.NoViewController.ToString().ToLower(), entityElement);
            SetAttribute("NoPMController", table.NoPMController.ToString().ToLower(), entityElement);
            SetAttribute("NoTS", table.NoTS.ToString().ToLower(), entityElement);
            SetAttribute("NoDefaultFeatures", table.NoDefaultFeatures.ToString().ToLower(), entityElement);
            SetAttribute("HasCompactSearch", table.HasCompactSearch.ToString().ToLower(), entityElement);
            SetAttribute("ApplyDefaultValues", table.ApplyDefaultValues.ToString().ToLower(), entityElement);
            SetAttribute("HasMenuButtons", table.HasMenuButtons.ToString().ToLower(), entityElement);
            SetAttribute("ApplyOnPropertyChangedCode", table.ApplyOnPropertyChangedCode.ToString().ToLower(), entityElement);
            SetAttribute("HasApiHelper", table.HasApiHelper.ToString().ToLower(), entityElement);
            SetAttribute("AllowedForComputingPartners", table.AllowedForComputingPartners.ToString().ToLower(), entityElement);
            SetAttribute("IsBusinessUnitEnabled", table.IsBusinessUnitEnabled.ToString().ToLower(), entityElement);
            SetAttribute("ParentObjectTableName", GetStringValue(table.ParentObjectTableName), entityElement);
            SetAttribute("TenantZeroData", table.TenantZeroData.ToString().ToLower(), entityElement);
            SetAttribute("ShowFastSearch", table.ShowFastSearch.ToString().ToLower(), entityElement);

            if (table.IsMetadataOnlyTable != false)
            {
                SetAttribute("IsMetadataOnlyTable", table.IsMetadataOnlyTable.ToString().ToLower(), entityElement);

            }
            if (!string.IsNullOrEmpty(table.QueryGroupCode1) && !string.IsNullOrEmpty(table.QueryGroupName1))
            {
                SetAttribute("Code1", GetStringValue(table.QueryGroupCode1), entityElement, null);
                SetAttribute("Name1", GetStringValue(table.QueryGroupName1), entityElement, null);
            }
            SetAttribute("CustomFieldsCount", table.CustomFieldsCount.ToString().ToLower(), entityElement);
            SetAttribute("DisableSearchBox", table.DisableSearchBox.ToString().ToLower(), entityElement);
            SetAttribute("HasDocuments", table.HasDocuments.ToString().ToLower(), entityElement);
            SetAttribute("IsLookUp", table.IsLookUp.ToString().ToLower(), entityElement);
            SetAttribute("IsTabsHidden", table.IsTabsHidden.ToString().ToLower(), entityElement);

            if (!string.IsNullOrEmpty(table.SearchFields))
            {
                SetAttribute("SearchFields", GetStringValue(table.SearchFields), entityElement, null);
            }


            SetAttribute("DxmlDatabaseTypeCode", table.DxmlDatabaseTypeCode, entityElement);
            SetAttribute("DxmlDatabaseSchemaCode", table.DxmlDatabaseSchemaCode, entityElement);

            #endregion

            #region ObjectFields Properties

            foreach (ObjectFieldsViewModel f in table.ObsList)
            {
                XmlElement fieldElement = doc.CreateElement("field");
                entityElement.AppendChild(fieldElement);
                if (string.IsNullOrEmpty(f.Id))
                {
                    SetAttribute("Id", GetStringValue(Guid.NewGuid()), fieldElement, null);
                }
                else
                {
                    SetAttribute("Id", GetStringValue(f.Id), fieldElement, null);
                }

                SetAttribute("FieldName", GetStringValue(f.FieldName), fieldElement, null);

                if (!string.IsNullOrEmpty(f.ShortName))
                {
                    SetAttribute("ShortName", f.ShortName, fieldElement, null);
                }


                SetAttribute("GeneratedComponentPath", GetStringValue(f.GeneratedComponentPath), fieldElement, null);


                string oldNames = null;

                if (string.IsNullOrEmpty(f.OldNames))
                {
                    string shortName = string.IsNullOrEmpty(f.ShortName) ? null : "," + f.ShortName;
                    oldNames = f.FieldName + shortName;
                }
                else
                {
                    oldNames = f.OldNames;

                    if (f.OldNames.Contains(","))
                    {
                        if (!f.OldNames.Split(',').Contains(f.FieldName))
                        {
                            oldNames = oldNames + "," + f.FieldName;
                        }

                        if (!string.IsNullOrEmpty(f.ShortName))
                        {
                            if (!f.OldNames.Split(',').Contains(f.ShortName))
                            {
                                oldNames = oldNames + "," + f.ShortName;
                            }
                        }
                    }
                    else
                    {
                        string shortName = string.IsNullOrEmpty(f.ShortName) ? null : "," + f.ShortName;

                        if (f.OldNames != f.FieldName)
                        {
                            oldNames = oldNames + "," + f.FieldName + shortName;
                        }
                        else
                        {
                            oldNames = oldNames + shortName;
                        }
                    }
                }

                SetAttribute("OldNames", oldNames.Contains(",") ? string.Join(",", oldNames.Split(',').Distinct().ToArray()) : oldNames, fieldElement, null);

                //SetAttribute("OldFieldName", GetStringValue(f.OldFieldName), fieldElement, null);
                SetAttribute("IsNew", f.IsNew.ToString().ToLower(), fieldElement, null);
                SetAttribute("IsChecked", f.IsChecked.ToString().ToLower(), fieldElement, null);
                SetAttribute("IsDeleted", f.IsDeleted.ToString().ToLower(), fieldElement, null);
                SetAttribute("ObjectTableName", GetStringValue(table.ObjectTableName), fieldElement, null);
                SetAttribute("FieldsDataType", GetStringValue(f.FieldDataType), fieldElement, null);

                SetAttribute("LookUpTableName", GetStringValue(f.LookUpTableName), fieldElement, null);

                SetAttribute("MinLength", f.MinLength.ToString(), fieldElement, null);
                SetAttribute("MaxLength", f.MaxLength.ToString(), fieldElement, null);
                SetAttribute("IsRequired", f.IsRequired.ToString().ToLower(), fieldElement, null);
                SetAttribute("CopyToDW", f.CopyToDW.ToString().ToLower(), fieldElement, null);
                
                SetAttribute("DisplayOnLookUp", f.DisplayOnLookUp.ToString().ToLower(), fieldElement, null);
                SetAttribute("DisplayOnLookUpLocal", f.DisplayOnLookUpLocal.ToString().ToLower(), fieldElement, null);
                SetAttribute("CanFilter", f.CanFilter.ToString().ToLower(), fieldElement, null);
                SetAttribute("DisplayOnly", f.DisplayOnly.ToString().ToLower(), fieldElement, null);
                SetAttribute("SystemRequired", f.SystemRequired.ToString().ToLower(), fieldElement, null);
                SetAttribute("ObjectFieldNotRequired", f.ObjectFieldNotRequired.ToString().ToLower(), fieldElement, null);

                SetAttribute("SystemMaxLength", f.SystemMaxLength.ToString(), fieldElement, null);
                SetAttribute("DisplayInList", f.DisplayInList.ToString().ToLower(), fieldElement, null);
                SetAttribute("ConverterName", GetStringValue(f.ConverterName), fieldElement, null);
                SetAttribute("DataTemplateName", GetStringValue(f.DataTemplateName), fieldElement, null);
                SetAttribute("IsCustomFilter", f.IsCustomFilter.ToString().ToLower(), fieldElement, null);
                SetAttribute("IsListFilter", f.IsListFilter.ToString().ToLower(), fieldElement, null);
                SetAttribute("Operator", GetStringValue(f.Operator), fieldElement, null);
                SetAttribute("MultiLine", f.MultiLine.ToString().ToLower(), fieldElement, null);
                SetAttribute("IsTimeFrameFilter", f.IsTimeFrameFilter.ToString().ToLower(), fieldElement, null);
                SetAttribute("DisplayInSearchWindowList", f.DisplayInSearchWindowList.ToString().ToLower(), fieldElement, null);
                //SetAttribute("DisplayInSearchWindowFilters", f.DisplayInSearchWindowFilters.ToString().ToLower(), fieldElement, null);
                SetAttribute("PMPropertyPath", GetStringValue(f.PMPropertyPath), fieldElement, null);
                SetAttribute("ListPropertyPath", GetStringValue(f.ListPropertyPath), fieldElement, null);
                SetAttribute("LookUpControlName", GetStringValue(f.LookUpControlName), fieldElement, null);
                SetAttribute("DisplayInLookUpIndex", f.DisplayInLookUpIndex != null ? f.DisplayInLookUpIndex.ToString() : null, fieldElement, null);
                SetAttribute("AutomaticField", f.AutomaticField.ToString().ToLower(), fieldElement, null);
                SetAttribute("UniqueField", f.UniqueField.ToString().ToLower(), fieldElement, null);
                SetAttribute("DisplayInSearchWindowListIndex", f.DisplayInSearchWindowListIndex.ToString(), fieldElement, null);
                //SetAttribute("DisplayInSearchWindowFiltersIndex", f.DisplayInSearchWindowFiltersIndex.ToString(), fieldElement, null);
                SetAttribute("IsMulti", f.IsMulti.ToString().ToLower(), fieldElement, null);
                SetAttribute("MultiTableName", GetStringValue(f.MultiTableName), fieldElement, null);

                SetAttribute("DependencyFilter1Value", GetStringValue(f.DependencyFilter1Value), fieldElement, null);
                SetAttribute("DependencyFilter2Value", GetStringValue(f.DependencyFilter2Value), fieldElement, null);
                SetAttribute("DependencyFilter3Value", GetStringValue(f.DependencyFilter3Value), fieldElement, null);
                SetAttribute("DependencyFilter1Type", GetStringValue(f.DependencyFilter1Type), fieldElement, null);
                SetAttribute("DependencyFilter2Type", GetStringValue(f.DependencyFilter2Type), fieldElement, null);
                SetAttribute("DependencyFilter3Type", GetStringValue(f.DependencyFilter3Type), fieldElement, null);
                SetAttribute("DependencyFilter1IsList", f.DependencyFilter1IsList.ToString().ToLower(), fieldElement, null);
                SetAttribute("DependencyFilter2IsList", f.DependencyFilter2IsList.ToString().ToLower(), fieldElement, null);
                SetAttribute("DependencyFilter3IsList", f.DependencyFilter3IsList.ToString().ToLower(), fieldElement, null);

                SetAttribute("ValidForQuerySection1", GetStringValue(f.ValidForQuerySection1), fieldElement, null);
                SetAttribute("ValidForQuerySection2", GetStringValue(f.ValidForQuerySection2), fieldElement, null);
                SetAttribute("IsRestrictable", f.IsRestrictable.ToString().ToLower(), fieldElement, null);
                SetAttribute("DisplayInEntityVariables", f.DisplayInEntityVariables.ToString().ToLower(), fieldElement, null);
                SetAttribute("TextCase", GetStringValue(f.TextCase), fieldElement, null);
                SetAttribute("ControlField1", GetStringValue(f.ControlField1), fieldElement, null);
                SetAttribute("ControlField2", GetStringValue(f.ControlField2), fieldElement, null);
                SetAttribute("ControlField3", GetStringValue(f.ControlField3), fieldElement, null);
                SetAttribute("Code", GetStringValue(f.Code), fieldElement, null);
                SetAttribute("AllowedInCustomerFieldsSettings", f.AllowedInCustomerFieldsSettings.ToString().ToLower(), fieldElement, null);
                SetAttribute("DisplayInSearchWindowFilters", f.DisplayInSearchWindowFilters.ToString().ToLower(), fieldElement, null);
                SetAttribute("DisplayInSearchWindowFiltersIndex", f.DisplayInSearchWindowFiltersIndex.ToString().ToLower(), fieldElement, null);
                SetAttribute("DisplayInDocumentReferences", f.DisplayInDocumentReferences.ToString().ToLower(), fieldElement, null);

                if (f.NumberOfDigits != null)
                {
                    SetAttribute("NumberOfDigits", f.NumberOfDigits.ToString(), fieldElement, null);
                }
                if (f.DigitsAfterPoint != null)
                {
                    SetAttribute("DigitsAfterPoint", f.DigitsAfterPoint.ToString(), fieldElement, null);
                }
                SetAttribute("InActive", f.InActive.ToString().ToLower(), fieldElement, null);
                //SetAttribute("SearchFields", GetStringValue(f.SearchFields), fieldElement, null);
                SetAttribute("DisplayInLookupColumnSize", GetStringValue(f.DisplayInLookupColumnSize), fieldElement, null);
                SetAttribute("ColumnHeaderTemplateName", GetStringValue(f.ColumnHeaderTemplateName), fieldElement, null);
                SetAttribute("DisplayLongName", f.DisplayLongName.ToString().ToLower(), fieldElement, null);
                //SetAttribute("CustomerPermissionTypeCode", GetStringValue(f.CustomerPermissionTypeCode), fieldElement, null);
                //SetAttribute("AgentPermissionTypeCode", GetStringValue(f.AgentPermissionTypeCode), fieldElement, null);
                SetAttribute("CustomPickListCode", GetStringValue(f.CustomPickListCode), fieldElement, null);



                SetAttribute("FullFieldLable", GetStringValue(f.FieldName), fieldElement, null);
                SetAttribute("DefaultText", GetStringValue(f.DefaultText), fieldElement, null);
                SetAttribute("FullLocalDefaultText", GetStringValue(f.FullLocalDefaultText), fieldElement, null);


                if (!string.IsNullOrEmpty(f.ListLableDefaultText))
                {

                    SetAttribute("ListFieldLable", GetStringValue(f.FieldName + "ListLable"), fieldElement, null);
                    SetAttribute("ListLableDefaultText", GetStringValue(f.ListLableDefaultText), fieldElement, null);
                    SetAttribute("ListLocalDefaultText", GetStringValue(f.ListLocalDefaultText), fieldElement, null);
                }

                if (!string.IsNullOrEmpty(f.HelpTextDefaultText))
                {
                    if (!string.IsNullOrEmpty(f.HelpTextCode))
                    {
                        SetAttribute("HelpTextCode", GetStringValue(f.HelpTextCode), fieldElement, null);
                    }
                    else
                    {
                        SetAttribute("HelpTextCode", GetStringValue(f.FieldName), fieldElement, null);
                    }
                    SetAttribute("HelpTextDefaultText", GetStringValue(f.HelpTextDefaultText), fieldElement, null);
                    SetAttribute("HelpLocalDefaultText", GetStringValue(f.HelpLocalDefaultText), fieldElement, null);
                }

                if (!string.IsNullOrEmpty(f.ShortFieldLableDefaultText))
                {
                    SetAttribute("ShortFieldLable", GetStringValue(f.FieldName), fieldElement, null);
                    SetAttribute("ShortFieldLableDefaultText", GetStringValue(f.ShortFieldLableDefaultText), fieldElement, null);
                    SetAttribute("ShortLocalDefaultText", GetStringValue(f.ShortLocalDefaultText), fieldElement, null);
                }

                SetAttribute("IsNullable", f.IsNullable.ToString().ToLower(), fieldElement, null);
                SetAttribute("IsForeignKey", f.IsForeignKey.ToString().ToLower(), fieldElement, null);
                SetAttribute("ForeignEntity", GetStringValue(f.ForeignEntity), fieldElement, null);

                SetAttribute("DontBuildRelationOnDB", f.DontBuildRelationOnDB.ToString().ToLower(), fieldElement, null);

                SetAttribute("NavigationPropertyName", GetStringValue(f.NavigationPropertyName), fieldElement, null);

                SetAttribute("IsPrimaryKey", f.IsPrimaryKey.ToString().ToLower(), fieldElement, null);
                SetAttribute("DisplayInList", f.DisplayInList.ToString().ToLower(), fieldElement, null);

                if (f.Order != null)
                {
                    SetAttribute("Order", f.Order.Value.ToString(), fieldElement, null);
                }

                SetAttribute("HasDataBaseField", f.IsDBField.ToString().ToLower(), fieldElement, null);
                SetAttribute("HasPMField", f.IsPMField.ToString().ToLower(), fieldElement, null);
                if (f.IsPMField && !string.IsNullOrEmpty(f.TableRelatedPM) && !string.IsNullOrEmpty(f.FieldRelatedPM))
                {
                    SetAttribute("TableRelatedPM", f.TableRelatedPM.ToString(), fieldElement, null);
                    SetAttribute("FieldRelatedPM", f.FieldRelatedPM.ToString(), fieldElement, null);
                }
				SetAttribute("ThisKey", GetStringValue(f.ThisKey), fieldElement, null);
                SetAttribute("OtherKey", GetStringValue(f.OtherKey), fieldElement, null);
                SetAttribute("AssociationName", GetStringValue(f.AssociationName), fieldElement, null);
                SetAttribute("IsComposition", f.IsComposition.ToString().ToLower(), fieldElement, null);



                SetAttribute("ControlField1", GetStringValue(f.ControlField1), fieldElement, null);
                SetAttribute("ControlField2", GetStringValue(f.ControlField2), fieldElement, null);

                SetAttribute("IsMaxLength", f.IsMaxLength.ToString().ToLower(), fieldElement, null);

                SetAttribute("NoMetaDataField", f.NoObjectField.ToString().ToLower(), fieldElement, null);
                SetAttribute("GenerateInList", f.GenerateInList.ToString().ToLower(), fieldElement, null);
                SetAttribute("IsFixedLength", f.IsFixedLength.ToString().ToLower(), fieldElement, null);
                SetAttribute("EnableAutoFill", f.EnableAutoFill.ToString().ToLower(), fieldElement, null);
                SetAttribute("IncludeInSearchField", f.IncludeInSearchField.ToString().ToLower(), fieldElement, null);
                SetAttribute("AllowedinAutomationConditions", f.AllowedinAutomationConditions.ToString().ToLower(), fieldElement, null);
                SetAttribute("AutomationEmailRecipient", f.AutomationEmailRecipient.ToString().ToLower(), fieldElement, null);
                SetAttribute("CanAutomateSetValue", f.CanAutomateSetValue.ToString().ToLower(), fieldElement, null);

                SetAttribute("DisplayInAutomationAsEnitity", f.DisplayInAutomationAsEnitity.ToString().ToLower(), fieldElement, null);

                if (!string.IsNullOrEmpty(f.RecordType))
                {
                    SetAttribute("RecordType", GetStringValue(f.RecordType), fieldElement, null);
                }

                if (!string.IsNullOrEmpty(f.AdditionalQuerySections))
                {
                    SetAttribute("AdditionalQuerySections", GetStringValue(f.AdditionalQuerySections), fieldElement, null);
                }
                SetAttribute("DisplayInRequiredFields", f.DisplayInRequiredFields.ToString().ToLower(), fieldElement, null);

           


                if (!string.IsNullOrEmpty(f.HtmlListComponentUrl))
                {
                    SetAttribute("HtmlListComponentUrl", GetStringValue(f.HtmlListComponentUrl), fieldElement, null);
                }
                if (!string.IsNullOrEmpty(f.HtmlListComponentName))
                {
                    SetAttribute("HtmlListComponentName", GetStringValue(f.HtmlListComponentName), fieldElement, null);
                }
                SetAttribute("HasTemplate", f.HasTemplate.ToString().ToLower(), fieldElement, null);
                SetAttribute("IsCustom", f.IsCustom.ToString().ToLower(), fieldElement, null);
                SetAttribute("HelpTextCode", GetStringValue(f.HelpTextCode), fieldElement, null);

                if (!string.IsNullOrEmpty(f.HtmlHeaderComponentUrl))
                {
                    SetAttribute("HtmlHeaderComponentUrl", GetStringValue(f.HtmlHeaderComponentUrl), fieldElement, null);
                }
                if (!string.IsNullOrEmpty(f.HtmlHeaderComponentName))
                {
                    SetAttribute("HtmlHeaderComponentName", GetStringValue(f.HtmlHeaderComponentName), fieldElement, null);
                }
                SetAttribute("IsSpellCheckedFullFieldLable", f.IsSpellCheckedFullFieldLable.ToString().ToLower(), fieldElement, null);
                SetAttribute("IsSpellCheckedHelpLocalDefaultText", f.IsSpellCheckedHelpLocalDefaultText.ToString().ToLower(), fieldElement, null);
                SetAttribute("IsSpellCheckedShortLocalDefaultText", f.IsSpellCheckedShortLocalDefaultText.ToString().ToLower(), fieldElement, null);
                SetAttribute("IsSpellCheckedListLocalDefaultText", f.IsSpellCheckedListLocalDefaultText.ToString().ToLower(), fieldElement, null);
                SetAttribute("EnableFullscreenTextBox", f.EnableFullscreenTextBox.ToString().ToLower(), fieldElement, null);

                if (!string.IsNullOrEmpty(f.ModelName))
                {
                    SetAttribute("ModelName", GetStringValue(f.ModelName), fieldElement, null);
                }
                if (!string.IsNullOrEmpty(f.ObjectFieldDataMapping))
                {
                    SetAttribute("ObjectFieldDataMapping", GetStringValue(f.ObjectFieldDataMapping), fieldElement, null);
                }

            }

            #endregion

            //#region Query Group Properties

            //XmlElement QueryGroupElement = doc.CreateElement("QueryGroup");
            //entityElement.AppendChild(QueryGroupElement);

            //SetAttribute("Code", GetStringValue(table.QueryGroupCode), QueryGroupElement, null);
            //SetAttribute("Name", GetStringValue(table.QueryGroupName), QueryGroupElement, null);

            //#endregion

            #region Queries Properties and Query Columns Properties


            foreach (QueryViewModel f in table.QueriesObsList)
            {
                //XmlElement QueryElement = (XmlElement)doc.AppendChild(doc.CreateElement("Query"));
                XmlElement QueryElement = doc.CreateElement("Query");
                entityElement.AppendChild(QueryElement);

                SetAttribute("Code", GetStringValue(f.Code), QueryElement, null);
                SetAttribute("TextCode", GetStringValue(f.TextCode), QueryElement, null);
                SetAttribute("LocalTextCode", GetStringValue(f.LocalTextCode), QueryElement, null);
                SetAttribute("QueryGroupCode", GetStringValue(f.QueryGroupCode), QueryElement, null);
                SetAttribute("IndexOrder", table.QueriesObsList.IndexOf(f).ToString(), QueryElement, null);
                SetAttribute("ObjectTableName", GetStringValue(f.ObjectTableName), QueryElement, null);
                SetAttribute("QuerySection", GetStringValue(f.QuerySection), QueryElement, null);

                SetAttribute("SystemLevel", f.SystemLevel.ToString().ToLower(), QueryElement, null);
                SetAttribute("IsAddNewEntity", f.IsAddNewEntity.ToString().ToLower(), QueryElement, null);
                SetAttribute("IsPackagable", f.IsPackagable.ToString().ToLower(), QueryElement, null);
                SetAttribute("DefaultSortName", GetStringValue(f.DefaultSortName), QueryElement, null);
                SetAttribute("DefaultSortDirection", GetStringValue(f.DefaultSortDirection), QueryElement, null);
                SetAttribute("EditWizardName", GetStringValue(f.EditWizardName), QueryElement, null);
                SetAttribute("EditWizardComponentPath", GetStringValue(f.EditWizardComponentPath), QueryElement, null);
                SetAttribute("SpotlightDataTemplate", GetStringValue(f.SpotlightDataTemplate), QueryElement, null);

                if (!string.IsNullOrEmpty(f.QueryGroupName))
                {
                    SetAttribute("QueryGroupName", GetStringValue(f.QueryGroupName), QueryElement, null);
                }
                if (!string.IsNullOrEmpty(f.TextCodeCode))
                {
                    SetAttribute("TextCodeCode", GetStringValue(f.TextCodeCode), QueryElement, null);
                }
                if (!string.IsNullOrEmpty(f.FeatureCode))
                {
                    SetAttribute("FeatureCode", GetStringValue(f.FeatureCode), QueryElement, null);
                }
                if (!string.IsNullOrEmpty(f.FeatureTextCodeCode))
                {
                    SetAttribute("FeatureTextCodeCode", GetStringValue(f.FeatureTextCodeCode), QueryElement, null);
                }
                if (!string.IsNullOrEmpty(f.FeatureDefaultText))
                {
                    SetAttribute("FeatureDefaultText", GetStringValue(f.FeatureDefaultText), QueryElement, null);
                }


                SetAttribute("IsSpellChecked", f.IsSpellChecked.ToString().ToLower(), QueryElement, null);
                if (!string.IsNullOrEmpty(f.Perspective))
                {
                    SetAttribute("Perspective", GetStringValue(f.Perspective), QueryElement, null);

                }
                // Query Columns Properties
                XmlElement QueryColumnsElement = doc.CreateElement("QueryColumns");
                QueryElement.AppendChild(QueryColumnsElement);
                foreach (QueryColumnsViewModel q in f.QueryColumnObsList)
                {
                    XmlElement QueryColumnElement = doc.CreateElement("QueryColumn");
                    QueryColumnsElement.AppendChild(QueryColumnElement);

                    SetAttribute("QueryCode", GetStringValue(q.QueryCode), QueryColumnElement, null);
                    SetAttribute("IndexOrder", f.QueryColumnObsList.IndexOf(q).ToString(), QueryColumnElement, null);
                    SetAttribute("ObjectFieldName", GetStringValue(q.ObjectFieldName), QueryColumnElement, null);
                    SetAttribute("ColumnWidth", q.ColumnWidth.ToString(), QueryColumnElement, null);
                }
                // Query Filters Properties
                XmlElement QueryFiltersElement = doc.CreateElement("QueryFilters");
                QueryElement.AppendChild(QueryFiltersElement);
                foreach (QueryFiltersViewModel q in f.QueryFiltersObsList)
                {
                    XmlElement QueryFilterElement = doc.CreateElement("QueryFilter");
                    QueryFiltersElement.AppendChild(QueryFilterElement);

                    SetAttribute("QueryCode", GetStringValue(q.QueryCode), QueryFilterElement, null);
                    SetAttribute("PredefinedValue", GetStringValue(q.PredefinedValue), QueryFilterElement, null);
                    SetAttribute("ObjectFieldName", GetStringValue(q.ObjectFieldName), QueryFilterElement, null);
                    SetAttribute("IsPredefined", q.IsPredefined.ToString().ToLower(), QueryFilterElement, null);

                    if (!string.IsNullOrEmpty(q.PredefinedValue2))
                    {
                        SetAttribute("PredefinedValue2", GetStringValue(q.PredefinedValue2), QueryFilterElement, null);
                    }
                    if (!string.IsNullOrEmpty(q.Operator))
                    {
                        SetAttribute("Operator", GetStringValue(q.Operator), QueryFilterElement, null);
                    }

                    SetAttribute("IndexOrder", q.IndexOrder.ToString(), QueryFilterElement, null);
                    SetAttribute("CustomPredefined", q.CustomPredefined.ToString().ToLower(), QueryFilterElement, null);



                }
            }

            #endregion

            #region Screens Properties and Screen Fields Properties


            foreach (ScreensViewModel f in table.ScreensObsList)
            {
                XmlElement ScreenElement = doc.CreateElement("Screen");
                entityElement.AppendChild(ScreenElement);
                SetAttribute("Name", GetStringValue(f.Name), ScreenElement, null);
                SetAttribute("ObjectTableName", GetStringValue(f.ObjectTableName), ScreenElement, null);
                SetAttribute("IsReadOnly", f.IsReadOnly.ToString().ToLower(), ScreenElement, null);
                SetAttribute("IsHeaderScreen", f.IsHeaderScreen.ToString().ToLower(), ScreenElement, null);
                if (!string.IsNullOrEmpty(f.Code))
                {
                    SetAttribute("Code", GetStringValue(f.Code), ScreenElement, null);
                }


                SetAttribute("NumberOfColumns", "2", ScreenElement, null);
                SetAttribute("NumberOfRows", "1", ScreenElement, null);

                // Screen Fields Properties
                XmlElement ScreenFieldsElement = doc.CreateElement("ScreenFields");
                ScreenElement.AppendChild(ScreenFieldsElement);

                if ((f.ScreenFieldCol1ObsList != null && f.ScreenFieldCol1ObsList.Count > 0) || (f.ScreenFieldCol2ObsList != null && f.ScreenFieldCol2ObsList.Count > 0)
                    || (f.ScreenFieldCol3ObsList != null && f.ScreenFieldCol3ObsList.Count > 0) || (f.ScreenFieldCol4ObsList != null && f.ScreenFieldCol4ObsList.Count > 0)
                    || (f.ScreenFieldCol5ObsList != null && f.ScreenFieldCol5ObsList.Count > 0))
                {

                    int ColCount = 0;
                    int RowCount = 0;
                    if (f.ScreenFieldCol1ObsList != null && f.ScreenFieldCol1ObsList.Count > 0)
                    {
                        ColCount++;
                        RowCount = RowCount < f.ScreenFieldCol1ObsList.Count ? f.ScreenFieldCol1ObsList.Count : RowCount;
                    }
                    if (f.ScreenFieldCol2ObsList != null && f.ScreenFieldCol2ObsList.Count > 0)
                    {
                        ColCount++;
                        RowCount = RowCount < f.ScreenFieldCol2ObsList.Count ? f.ScreenFieldCol2ObsList.Count : RowCount;
                    }
                    if (f.ScreenFieldCol3ObsList != null && f.ScreenFieldCol3ObsList.Count > 0)
                    {
                        ColCount++;
                        RowCount = RowCount < f.ScreenFieldCol3ObsList.Count ? f.ScreenFieldCol3ObsList.Count : RowCount;
                    }
                    if (f.ScreenFieldCol4ObsList != null && f.ScreenFieldCol4ObsList.Count > 0)
                    {
                        ColCount++;
                        RowCount = RowCount < f.ScreenFieldCol4ObsList.Count ? f.ScreenFieldCol4ObsList.Count : RowCount;
                    }
                    if (f.ScreenFieldCol5ObsList != null && f.ScreenFieldCol5ObsList.Count > 0)
                    {
                        ColCount++;
                        RowCount = RowCount < f.ScreenFieldCol5ObsList.Count ? f.ScreenFieldCol5ObsList.Count : RowCount;
                    }
                    SetAttribute("NumberOfColumns", ColCount.ToString(), ScreenElement, null);
                    SetAttribute("NumberOfRows", RowCount.ToString(), ScreenElement, null);



                    for (int i = 0; i < ColCount; i++)
                    {
                        if (f.ScreenFieldCol1ObsList != null && f.ScreenFieldCol1ObsList.Count > 0)
                        {
                            foreach (ScreenFieldViewModel q in f.ScreenFieldCol1ObsList)
                            {
                                XmlElement ScreenFieldElement = doc.CreateElement("ScreenField");
                                ScreenFieldsElement.AppendChild(ScreenFieldElement);

                                SetAttribute("Column", i.ToString(), ScreenFieldElement, null);
                                SetAttribute("Row", f.ScreenFieldCol1ObsList.IndexOf(q).ToString(), ScreenFieldElement, null);
                                SetAttribute("ObjectFieldName", GetStringValue(q.ObjectFieldName), ScreenFieldElement, null);
                                SetAttribute("ScreenName", q.ScreenName.ToString(), ScreenFieldElement, null);
                            }
                            f.ScreenFieldCol1ObsList = null;
                        }

                        else if (f.ScreenFieldCol2ObsList != null && f.ScreenFieldCol2ObsList.Count > 0)
                        {
                            foreach (ScreenFieldViewModel q in f.ScreenFieldCol2ObsList)
                            {
                                XmlElement ScreenFieldElement = doc.CreateElement("ScreenField");
                                ScreenFieldsElement.AppendChild(ScreenFieldElement);

                                SetAttribute("Column", i.ToString(), ScreenFieldElement, null);
                                SetAttribute("Row", f.ScreenFieldCol2ObsList.IndexOf(q).ToString(), ScreenFieldElement, null);
                                SetAttribute("ObjectFieldName", GetStringValue(q.ObjectFieldName), ScreenFieldElement, null);
                                SetAttribute("ScreenName", q.ScreenName.ToString(), ScreenFieldElement, null);
                            }
                            f.ScreenFieldCol2ObsList = null;
                        }

                        else if (f.ScreenFieldCol3ObsList != null && f.ScreenFieldCol3ObsList.Count > 0)
                        {
                            foreach (ScreenFieldViewModel q in f.ScreenFieldCol3ObsList)
                            {
                                XmlElement ScreenFieldElement = doc.CreateElement("ScreenField");
                                ScreenFieldsElement.AppendChild(ScreenFieldElement);

                                SetAttribute("Column", i.ToString(), ScreenFieldElement, null);
                                SetAttribute("Row", f.ScreenFieldCol3ObsList.IndexOf(q).ToString(), ScreenFieldElement, null);
                                SetAttribute("ObjectFieldName", GetStringValue(q.ObjectFieldName), ScreenFieldElement, null);
                                SetAttribute("ScreenName", q.ScreenName.ToString(), ScreenFieldElement, null);
                            }
                            f.ScreenFieldCol3ObsList = null;
                        }

                        else if (f.ScreenFieldCol4ObsList != null && f.ScreenFieldCol4ObsList.Count > 0)
                        {
                            foreach (ScreenFieldViewModel q in f.ScreenFieldCol4ObsList)
                            {
                                XmlElement ScreenFieldElement = doc.CreateElement("ScreenField");
                                ScreenFieldsElement.AppendChild(ScreenFieldElement);

                                SetAttribute("Column", i.ToString(), ScreenFieldElement, null);
                                SetAttribute("Row", f.ScreenFieldCol4ObsList.IndexOf(q).ToString(), ScreenFieldElement, null);
                                SetAttribute("ObjectFieldName", GetStringValue(q.ObjectFieldName), ScreenFieldElement, null);
                                SetAttribute("ScreenName", q.ScreenName.ToString(), ScreenFieldElement, null);
                            }
                            f.ScreenFieldCol4ObsList = null;
                        }

                        else if (f.ScreenFieldCol5ObsList != null && f.ScreenFieldCol5ObsList.Count > 0)
                        {
                            foreach (ScreenFieldViewModel q in f.ScreenFieldCol5ObsList)
                            {
                                XmlElement ScreenFieldElement = doc.CreateElement("ScreenField");
                                ScreenFieldsElement.AppendChild(ScreenFieldElement);

                                SetAttribute("Column", i.ToString(), ScreenFieldElement, null);
                                SetAttribute("Row", f.ScreenFieldCol5ObsList.IndexOf(q).ToString(), ScreenFieldElement, null);
                                SetAttribute("ObjectFieldName", GetStringValue(q.ObjectFieldName), ScreenFieldElement, null);
                                SetAttribute("ScreenName", q.ScreenName.ToString(), ScreenFieldElement, null);
                            }
                            f.ScreenFieldCol5ObsList = null;
                        }
                    }
                }
            }

            #endregion

            #region Tabs Properties


            foreach (TabsViewModel f in table.TabsObsList)
            {
                XmlElement TabElement = doc.CreateElement("Tab");
                entityElement.AppendChild(TabElement);

                SetAttribute("TabName", GetStringValue(f.Name), TabElement, null);
                SetAttribute("TabLocalName", GetStringValue(f.LocalName), TabElement, null);
                SetAttribute("ObjectTableName", GetStringValue(f.ObjectTableName), TabElement, null);
                SetAttribute("Code", GetStringValue(f.Code), TabElement, null);
                SetAttribute("ControlPath", GetStringValue(f.ControlPath), TabElement, null);
                SetAttribute("TextCode", GetStringValue(f.TextCode), TabElement, null);
                SetAttribute("IsPackagable", f.IsPackagable.ToString().ToLower(), TabElement, null);
                if (table.TabsObsList.Count > 1 && table.TabsObsList.GroupBy(t => t.IndexOrder).Count() == 1)
                {
                    SetAttribute("IndexOrder", table.TabsObsList.IndexOf(f).ToString(), TabElement, null);
                }
                else
                {
                    SetAttribute("IndexOrder", f.IndexOrder.ToString(), TabElement, null);
                }
                SetAttribute("HtmlComponentURL", GetStringValue(f.HtmlComponentURL), TabElement, null);
                SetAttribute("HtmlComponentName", GetStringValue(f.HtmlComponentName), TabElement, null);
                if (!string.IsNullOrEmpty(f.FeatureCode))
                {
                    SetAttribute("FeatureCode", GetStringValue(f.FeatureCode), TabElement, null);
                }
                if (!string.IsNullOrEmpty(f.FeatureTextCodeCode))
                {
                    SetAttribute("FeatureTextCodeCode", GetStringValue(f.FeatureTextCodeCode), TabElement, null);
                }
                if (!string.IsNullOrEmpty(f.FeatureDefaultText))
                {
                    SetAttribute("FeatureDefaultText", GetStringValue(f.FeatureDefaultText), TabElement, null);
                }


                SetAttribute("IsPackagable", f.IsPackagable.ToString().ToLower(), TabElement, null);
                SetAttribute("IsSpellChecked", f.IsSpellChecked.ToString().ToLower(), TabElement, null);
                SetAttribute("HasGeneralFeature", f.HasGeneralFeature.ToString().ToLower(), TabElement, null);
				SetAttribute("IsLocked", f.IsLocked.ToString().ToLower(), TabElement, null);

			}

			#endregion

			#region Event Types Properties

			XmlElement TypesElement = doc.CreateElement("EventTypes");
            entityElement.AppendChild(TypesElement);

            foreach (EventTypesViewModel f in table.EventTypesObsList)
            {
                XmlElement EventElement = doc.CreateElement("EventType");
                TypesElement.AppendChild(EventElement);

                SetAttribute("Code", GetStringValue(f.Code), EventElement, null);
                SetAttribute("ObjectTableName", GetStringValue(f.ObjectTableName), EventElement, null);
                SetAttribute("EnglishName", GetStringValue(f.EnglishName), EventElement, null);
                SetAttribute("LocalName", GetStringValue(f.LocalName), EventElement, null);
                SetAttribute("IsManualEntry", f.IsManualEntry.ToString().ToLower(), EventElement, null);
                SetAttribute("ShortView", f.ShortView.ToString().ToLower(), EventElement, null);

                SetAttribute("EventTypeCategoryCode", GetStringValue(f.EventTypeCategoryCode), EventElement, null);
                SetAttribute("IsAgentView", f.IsAgentView.ToString().ToLower(), EventElement, null);
                SetAttribute("IsCustomerView", f.IsCustomerView.ToString().ToLower(), EventElement, null);
                SetAttribute("IsSharedLogisticsEnabled", f.IsSharedLogisticsEnabled.ToString().ToLower(), EventElement, null);
                SetAttribute("AllowedInAutomation", f.AllowedInAutomation.ToString().ToLower(), EventElement, null);
                SetAttribute("ManualActivatedFollowUp", f.ManualActivatedFollowUp.ToString().ToLower(), EventElement, null);
                SetAttribute("IsFollowUp", f.IsFollowUp.ToString().ToLower(), EventElement, null);
                SetAttribute("FollowUpEnglishName", GetStringValue(f.FollowUpEnglishName), EventElement, null);
                SetAttribute("FollowUpLocalName", GetStringValue(f.FollowUpLocalName), EventElement, null);
                SetAttribute("EntityStatusCode", GetStringValue(f.EntityStatusCode), EventElement, null);


            }

            #endregion

            #region Close Table Data
            if (table.rows != null)
            {
                XmlElement RecordsElement = doc.CreateElement("Records");
                entityElement.AppendChild(RecordsElement);

                foreach (Row f in table.rows)
                {
                    XmlElement RecordElement = doc.CreateElement("Record");
                    RecordsElement.AppendChild(RecordElement);
                    foreach (var item in f._data)
                    {
                        int n = 0;
                        if (item.Value != null)
                        {
                            if (item.Value.ToString() == "true" || item.Value.ToString() == "false" || int.TryParse(item.Value.ToString(), out n))
                            {
                                SetAttribute(item.Key, item.Value.ToString(), RecordElement, null);
                            }
                            else
                            {
                                if (item.Value.ToString().StartsWith("\""))
                                {
                                    SetAttribute(item.Key, item.Value.ToString(), RecordElement, null);
                                }
                                else
                                {
                                    SetAttribute(item.Key, GetStringValue(item.Value.ToString()), RecordElement, null);
                                }

                            }
                        }
                        else
                            SetAttribute(item.Key, null, RecordElement, null);

                    }
                }
            }


            #endregion

            #region Menu Buttons Properties
            if (table.MenuButtonsObsList.Count > 0)
            {
                XmlElement MenuButtonsElement = doc.CreateElement("MenuButtons");
                entityElement.AppendChild(MenuButtonsElement);
                SetAttribute("MenuButtonGroupType", GetStringValue(table.MenuButtonGroupType), MenuButtonsElement, null);
                SetAttribute("MenuButtonGroupName", GetStringValue(table.MenuButtonGroupName), MenuButtonsElement, null);

                foreach (MenuButtonViewModel f in table.MenuButtonsObsList)
                {
                    XmlElement MenuButtonElement = doc.CreateElement("MenuButton");
                    MenuButtonsElement.AppendChild(MenuButtonElement);
                    SetAttribute("EventCode", GetStringValue(f.EventCode), MenuButtonElement, null);
                    SetAttribute("DefaultText", GetStringValue(f.DefaultText), MenuButtonElement, null);
                    SetAttribute("MenuButtonType", GetStringValue(f.SelectedMenuButtonType), MenuButtonElement, null);
                    SetAttribute("Style", GetStringValue(f.Style), MenuButtonElement, null);
                    SetAttribute("IndexOrder", f.IndexOrder.ToString(), MenuButtonElement, null);
                    if (!string.IsNullOrEmpty(f.LocalDefaultText))
                    {
                        SetAttribute("LocalDefaultText", GetStringValue(f.LocalDefaultText), MenuButtonElement, null);
                    }
                    if (!string.IsNullOrEmpty(f.FeatureCode))
                    {
                        SetAttribute("FeatureCode", GetStringValue(f.FeatureCode), MenuButtonElement, null);

                    }

                    SetAttribute("IsPackagable", f.IsPackagable.ToString().ToLower(), MenuButtonElement, null);

                    if (!string.IsNullOrEmpty(f.TextCodeCode))
                    {
                        SetAttribute("TextCodeCode", GetStringValue(f.TextCodeCode), MenuButtonElement, null);
                    }
                    if (!string.IsNullOrEmpty(f.FeatureTextCodeCode))
                    {
                        SetAttribute("FeatureTextCodeCode", GetStringValue(f.FeatureTextCodeCode), MenuButtonElement, null);
                    }
                    if (!string.IsNullOrEmpty(f.FeatureDefaultText))
                    {
                        SetAttribute("FeatureDefaultText", GetStringValue(f.FeatureDefaultText), MenuButtonElement, null);
                    }
                    if (!string.IsNullOrEmpty(f.HtmlComponentPath))
                    {
                        SetAttribute("HtmlComponentPath", GetStringValue(f.HtmlComponentPath), MenuButtonElement, null);
                    }
                    if (f.Width != 0)
                    {
                        SetAttribute("Width", f.Width.ToString(), MenuButtonElement, null);
                    }
                    if (f.MenuButtonItems != null)
                    {
                        foreach (var item in f.MenuButtonItems)
                        {
                            XmlElement MenuItemElement = doc.CreateElement("MenuItem");
                            MenuButtonElement.AppendChild(MenuItemElement);
                            SetAttribute("EventCode", GetStringValue(item.EventCode), MenuItemElement, null);
                            SetAttribute("DefaultText", GetStringValue(item.DefaultText), MenuItemElement, null);
                            SetAttribute("MenuButtonType", GetStringValue(item.SelectedMenuButtonType), MenuItemElement, null);
                            SetAttribute("Style", GetStringValue(item.Style), MenuItemElement, null);
                            SetAttribute("IndexOrder", item.IndexOrder.ToString(), MenuItemElement, null);
                            if (!string.IsNullOrEmpty(item.LocalDefaultText))
                            {
                                SetAttribute("LocalDefaultText", GetStringValue(item.LocalDefaultText), MenuItemElement, null);
                            }
                            if (!string.IsNullOrEmpty(item.FeatureCode))
                            {
                                SetAttribute("FeatureCode", GetStringValue(item.FeatureCode), MenuItemElement, null);

                            }

                            SetAttribute("IsPackagable", item.IsPackagable.ToString().ToLower(), MenuItemElement, null);

                            if (!string.IsNullOrEmpty(item.TextCodeCode))
                            {
                                SetAttribute("TextCodeCode", GetStringValue(item.TextCodeCode), MenuItemElement, null);
                            }
                            if (!string.IsNullOrEmpty(item.FeatureTextCodeCode))
                            {
                                SetAttribute("FeatureTextCodeCode", GetStringValue(item.FeatureTextCodeCode), MenuItemElement, null);
                            }
                            if (!string.IsNullOrEmpty(item.FeatureDefaultText))
                            {
                                SetAttribute("FeatureDefaultText", GetStringValue(item.FeatureDefaultText), MenuItemElement, null);
                            }
                            if (!string.IsNullOrEmpty(item.HtmlComponentPath))
                            {
                                SetAttribute("HtmlComponentPath", GetStringValue(item.HtmlComponentPath), MenuItemElement, null);
                            }
                            if (item.Width != 0)
                            {
                                SetAttribute("Width", item.Width.ToString(), MenuItemElement, null);
                            }
                        }
                    }
                }
            }

            #endregion

            #region Data Contracts Properties

            XmlElement DataContractsElement = doc.CreateElement("DataContracts");
            entityElement.AppendChild(DataContractsElement);
            if (table.DataContractsObsList != null)
            {
                foreach (DataContractViewModel f in table.DataContractsObsList)
                {
                    XmlElement DataContractElement = doc.CreateElement("DataContract");
                    DataContractsElement.AppendChild(DataContractElement);

                    SetAttribute("Name", GetStringValue(f.DCName), DataContractElement, null);
                    SetAttribute("Version", GetStringValue(f.DCVersion), DataContractElement, null);
                    SetAttribute("IncludeTenant0Data", f.IncludeTenant0Data.ToString().ToLower(), DataContractElement, null);
                    SetAttribute("ComputingPartnerName", GetStringValue(f.ComputingPartnerName), DataContractElement, null);
                    if (f.DCFieldsObsList != null)
                    {
                        foreach (var item in f.DCFieldsObsList)
                        {
                            XmlElement DCFieldElement = doc.CreateElement("DCField");
                            DataContractElement.AppendChild(DCFieldElement);

                            SetAttribute("DCFieldName", GetStringValue(item.DCFieldName), DCFieldElement, null);
                            SetAttribute("DCVersion", GetStringValue(item.DCVersion), DCFieldElement, null);
                            SetAttribute("FieldName", GetStringValue(item.FieldName), DCFieldElement, null);
                            SetAttribute("IsKey", item.IsKey.ToString().ToLower(), DCFieldElement, null);
                            SetAttribute("IsManualMapping", item.IsManualMapping.ToString().ToLower(), DCFieldElement, null);
                            SetAttribute("IsMulti", item.IsMulti.ToString().ToLower(), DCFieldElement, null);
                            SetAttribute("MultiTableName", GetStringValue(item.MultiTableName), DCFieldElement, null);
                            SetAttribute("FieldsDataType", GetStringValue(item.FieldDataType), DCFieldElement, null);
                            SetAttribute("IsNullable", item.IsNullable.ToString().ToLower(), DCFieldElement, null);
                            SetAttribute("IsSpecialField", item.IsSpecialField.ToString().ToLower(), DCFieldElement, null);
                            SetAttribute("IsCustomType", item.IsCustomType.ToString().ToLower(), DCFieldElement, null);
                            SetAttribute("IsAttribute", item.IsAttribute.ToString().ToLower(), DCFieldElement, null);
                            SetAttribute("IgnoreCustomTypeCheck", item.IgnoreCustomTypeCheck.ToString().ToLower(), DCFieldElement, null);
                            SetAttribute("IsCloseField", item.IsCloseField.ToString().ToLower(), DCFieldElement, null);
                            SetAttribute("IsCompositKey", item.IsCompositKey.ToString().ToLower(), DCFieldElement, null);
                            SetAttribute("CloseTableCode", GetStringValue(item.CloseTableCode), DCFieldElement, null);
                            SetAttribute("IsUpdateAllowed", item.IsUpdateAllowed.ToString().ToLower(), DCFieldElement, null);

                        }
                    }

                }
            }
            #endregion

            #region Additional TextCodes Properties

            XmlElement AdditionalTextCodesElement = doc.CreateElement("AdditionalTextCodes");
            entityElement.AppendChild(AdditionalTextCodesElement);

            foreach (TextCodesViewModel f in table.AdditionalTextCodesList)
            {
                XmlElement TextCodeElement = doc.CreateElement("TextCode");
                AdditionalTextCodesElement.AppendChild(TextCodeElement);

                SetAttribute("Code", GetStringValue(f.Code), TextCodeElement, null);
                SetAttribute("DefaultText", GetStringValue(f.DefaultText), TextCodeElement, null);
                SetAttribute("LocalDefaultText", GetStringValue(f.LocalDefaultText), TextCodeElement, null);
                SetAttribute("TextCodeTypeCode", GetStringValue(f.TextCodeTypeCode), TextCodeElement, null);
                SetAttribute("IsSpellChecked", f.IsSpellChecked.ToString().ToLower(), TextCodeElement, null);



            }

            #endregion

            #region Additional Features Properties

            XmlElement AdditionalFeaturesElement = doc.CreateElement("AdditionalFeatures");
            entityElement.AppendChild(AdditionalFeaturesElement);

            foreach (FeaturesViewModel f in table.AdditionalFeaturesList)
            {
                XmlElement TextCodeElement = doc.CreateElement("Feature");
                AdditionalFeaturesElement.AppendChild(TextCodeElement);

                SetAttribute("Code", GetStringValue(f.Code), TextCodeElement, null);
                SetAttribute("FeatureTypeCode", GetStringValue(f.FeatureTypeCode), TextCodeElement, null);
                SetAttribute("FeatureTextCodeCode", GetStringValue(f.FeatureTextCodeCode), TextCodeElement, null);
                SetAttribute("FeatureDefaultText", GetStringValue(f.FeatureDefaultText), TextCodeElement, null);
                SetAttribute("IsPackagable", f.IsPackagable.ToString().ToLower(), TextCodeElement, null);
                SetAttribute("IsOld", f.IsOld.ToString().ToLower(), TextCodeElement, null);
                SetAttribute("IsCoreFeature", f.IsCoreFeature.ToString().ToLower(), TextCodeElement, null);
                SetAttribute("IsBusinessUnitEnabled", f.IsBusinessUnitEnabled.ToString().ToLower(), TextCodeElement, null);
                SetAttribute("ToggleCode", GetStringValue(f.ToggleCode), TextCodeElement, null);


            }

            #endregion

            string tableName = table.ObjectTableName;
            if (table.ObjectTableName.Contains("."))
            {
                tableName = table.ObjectTableName.Split('.')[1];
            }

            if (!string.IsNullOrEmpty(App.DirectOpenPath))
            {
                FileStream fileStream = new FileStream(App.DirectOpenPath, FileMode.Truncate, FileAccess.Write);
                XmlWriterSettings settings = new XmlWriterSettings() { Indent = true, NewLineOnAttributes = true, OmitXmlDeclaration = false, WriteEndDocumentOnClose = false };//, WriteEndDocumentOnClose = true, OmitXmlDeclaration = true
                XmlWriter xmlWriter = XmlWriter.Create(fileStream, settings);

                doc.Save(xmlWriter);
                xmlWriter.Close();
                xmlWriter.Dispose();
                //doc.Save(App.DirectOpenPath);
            }
            else
            {
                MessageBox.Show("file path in not valid!");
            }

        }

        #endregion

        #region GenerateDXMLFileFromTool

        public static void GenerateDXMLFileFromTool(ObjectTableViewModel table)
        {
            if (table.ObsList.Where(f => f.IsDBField).Any())
            {
                if (!string.IsNullOrEmpty(App.DirectOpenPath))
                {
                    string entityName = Path.GetFileName(App.DirectOpenPath).Replace(".lxml", string.Empty);
                    string dxmlFilePath = App.DirectOpenPath.Replace("EntityFiles", "DBTables").Replace("lxml", "dxml");
                    List<XElement> indexElements = new List<XElement>();
                    List<XElement> uniqueConstraintElements = new List<XElement>();
                    List<XElement> columnWithDefaultValueElements = new List<XElement>();
                    List<XElement> columnWithIdentityElements = new List<XElement>();
                    List<XElement> columnWithInitialValueScriptElements = new List<XElement>();

                    if (File.Exists(dxmlFilePath))
                    {
                        XDocument oldDoc = XDocument.Load(dxmlFilePath);

                        indexElements = oldDoc.Descendants("Index").ToList();
                        uniqueConstraintElements = oldDoc.Descendants("UniqueConstraint").ToList();
                        columnWithDefaultValueElements = oldDoc.Descendants("Column").Where(x => x.Attribute("DefaultValue") != null).ToList();
                        columnWithIdentityElements = oldDoc.Descendants("Column").Where(x => x.Attribute("Identity") != null).ToList();
                        columnWithInitialValueScriptElements = oldDoc.Descendants("Column").Where(x => x.Attribute("InitialValueScript") != null).ToList();
                    }

                    XmlDocument doc = new XmlDocument();
                    XmlElement tableElement = (XmlElement)doc.AppendChild(doc.CreateElement("Table"));

                    string tableName = table.DBTableName.ToLower().StartsWith("customs.") ? table.DBTableName.Split('.')[1] : table.DBTableName;

                    tableElement.SetAttribute("Name", tableName);

                    if (!string.IsNullOrEmpty(table.DBTableShortName))
                    {
                        tableElement.SetAttribute("ShortName", table.DBTableShortName);
                    }

                    string dbTableOldNames = null;

                    if (!string.IsNullOrEmpty(table.DBTableOldNames))
                    {
                        string tableOldNames = string.Join(",", table.DBTableOldNames.Split(',').Select(n => n.ToLower().StartsWith("customs.") ? n.Split('.')[1] : n).ToArray());

                        if (tableOldNames.Contains(","))
                        {
                            var oldNamesExceptName = tableOldNames.Split(',').Where(x => x?.ToLower() != tableName?.ToLower() && x?.ToLower() != table.DBTableShortName?.ToLower());

                            dbTableOldNames = oldNamesExceptName.Count() == 1 ? oldNamesExceptName.First() : string.Join(",", oldNamesExceptName.Reverse().Distinct(StringComparer.OrdinalIgnoreCase).ToArray());
                        }
                        else
                        {
                            dbTableOldNames = tableName?.ToLower() == tableOldNames?.ToLower() ? null : tableOldNames;
                        }
                    }

                    if (!string.IsNullOrEmpty(dbTableOldNames))
                    {
                        tableElement.SetAttribute("OldNames", dbTableOldNames);
                    }


                    tableElement.SetAttribute("Schema", table.DxmlDatabaseSchemaCode);
                    tableElement.SetAttribute("DBType", table.DxmlDatabaseTypeCode);

                    if (!string.IsNullOrEmpty(table.ClientModuleName))
                    {
                        tableElement.SetAttribute("Module", table.ClientModuleName);
                    }

                    foreach (ObjectFieldsViewModel field in table.ObsList.Where(f => f.IsDBField))
                    {
                        XmlElement columnElement = doc.CreateElement("Column");
                        tableElement.AppendChild(columnElement);

                        string fieldName = field.FieldName;
                        string fieldShortName = field.ShortName;
                        string fieldDataType = field.FieldDataType;
                        int fieldMaxLength = field.MaxLength;
                        bool fieldIsMaxLength = field.IsMaxLength;
                        bool fieldIsPrimaryKey = field.IsPrimaryKey;
                        bool fieldIsRequired = field.IsRequired;
                        bool fieldIsNullable = field.IsNullable;
                        bool fieldIsFixedLength = field.IsFixedLength;
                        string fieldOldNames = field.OldNames;
                        int? fieldNumberOfDigits = field.NumberOfDigits;
                        int? fieldDigitsAfterPoint = field.DigitsAfterPoint;

                        string dxmlColumnDataType = GetDataTypeForDXMLColumn(fieldDataType, fieldIsFixedLength);
                        bool dxmlColumnNullable;

                        if ((entityName.ToLower() == "Address".ToLower() && (fieldName.ToLower() == "City".ToLower() || fieldName.ToLower() == "CountryId".ToLower())))
                        {
                            dxmlColumnNullable = true;
                        }
                        else
                        {
                            dxmlColumnNullable = GetNullableForDXMLColumn(fieldIsRequired, fieldIsNullable, fieldIsPrimaryKey, dxmlColumnDataType);
                        }


                        int dxmlColumnSize = new string[] { "bit", "datetime", "datetime2", "decimal", "float", "int" }.Contains(dxmlColumnDataType) ? 0 : (fieldIsMaxLength ? -1 : fieldMaxLength);

                        columnElement.SetAttribute("Name", fieldName);

                        if (!string.IsNullOrEmpty(fieldShortName))
                        {
                            columnElement.SetAttribute("ShortName", fieldShortName);
                        }

                        string oldNames = null;

                        if (!String.IsNullOrEmpty(fieldOldNames))
                        {
                            if (fieldOldNames.Contains(","))
                            {
                                var oldNamesExceptName = fieldOldNames.Split(',').Where(x => x?.ToLower() != fieldName?.ToLower() && x?.ToLower() != fieldShortName?.ToLower());

                                oldNames = oldNamesExceptName.Count() == 1 ? oldNamesExceptName.First() : string.Join(",", oldNamesExceptName.Reverse().Distinct(StringComparer.OrdinalIgnoreCase).ToArray());
                            }
                            else
                            {
                                oldNames = (fieldName?.ToLower() == fieldOldNames?.ToLower()) ? null : fieldOldNames;
                            }
                        }

                        if (!String.IsNullOrEmpty(oldNames))
                        {
                            columnElement.SetAttribute("OldNames", oldNames);
                        }

                        columnElement.SetAttribute("Type", dxmlColumnDataType);

                        if (dxmlColumnSize != 0)
                        {
                            columnElement.SetAttribute("Size", dxmlColumnSize.ToString());
                        }

                        if (dxmlColumnDataType == "decimal" && fieldNumberOfDigits != null)
                        {
                            columnElement.SetAttribute("Precision", fieldNumberOfDigits.ToString());
                        }

                        if (dxmlColumnDataType == "decimal" && fieldDigitsAfterPoint != null)
                        {
                            columnElement.SetAttribute("Scale", fieldDigitsAfterPoint.ToString());
                        }

                        string columnNames;
                        if (!string.IsNullOrEmpty(field.OldNames))
                        {
                            columnNames = field.OldNames;
                        }
                        else
                        {
                            columnNames = field.FieldName;
                        }

                        XElement columnWithDefaultValueElement = columnWithDefaultValueElements.Where(x => (!columnNames.Contains(",") && x.Attribute("Name").Value == columnNames) || (columnNames.Contains(",") && columnNames.Split(',').Contains(x.Attribute("Name").Value))).FirstOrDefault();
                        if (columnWithDefaultValueElement != null)
                        {
                            columnElement.SetAttribute("DefaultValue", columnWithDefaultValueElement.Attribute("DefaultValue").Value);
                        }

                        XElement columnWithIdentityElement = columnWithIdentityElements.Where(x => (!columnNames.Contains(",") && x.Attribute("Name").Value == columnNames) || (columnNames.Contains(",") && columnNames.Split(',').Contains(x.Attribute("Name").Value))).FirstOrDefault();
                        if (columnWithIdentityElement != null)
                        {
                            columnElement.SetAttribute("Identity", columnWithIdentityElement.Attribute("Identity").Value);
                        }

                        XElement columnWithInitialValueScriptElement = columnWithInitialValueScriptElements.Where(x => (!columnNames.Contains(",") && x.Attribute("Name").Value == columnNames) || (columnNames.Contains(",") && columnNames.Split(',').Contains(x.Attribute("Name").Value))).FirstOrDefault();
                        if (columnWithInitialValueScriptElement != null)
                        {
                            columnElement.SetAttribute("InitialValueScript", columnWithInitialValueScriptElement.Attribute("InitialValueScript").Value);
                        }

                        XmlElement constraintsElement = doc.CreateElement("Constraints");
                        columnElement.AppendChild(constraintsElement);

                        if (fieldIsPrimaryKey)
                        {
                            constraintsElement.SetAttribute("PrimaryKey", "true");
                        }

                        constraintsElement.SetAttribute("Nullable", dxmlColumnNullable ? "true" : "false");
                    }

                    List<string> processedForeignKeyFields = new List<string>();

                    foreach (ObjectFieldsViewModel field in table.ObsList.Where(f => f.IsDBField && f.IsForeignKey))
                    {
                        if (!processedForeignKeyFields.Contains(field.FieldName))
                        {
                            XmlElement relationElement = doc.CreateElement("Relation");
                            tableElement.AppendChild(relationElement);

                            string dontBuildRelationOnDB = field.DontBuildRelationOnDB ? "true" : "false";

                            string fieldNavigationPropertyName = field.NavigationPropertyName;

                            string[] filedsWithSameNavigationPropertyName = table.ObsList.Where(f => f.IsDBField && f.IsForeignKey && f.NavigationPropertyName == fieldNavigationPropertyName).Select(f => f.FieldName).ToArray();

                            string foreignKeyColumn = filedsWithSameNavigationPropertyName.Length == 1 ? filedsWithSameNavigationPropertyName[0] : string.Join(",", filedsWithSameNavigationPropertyName);

                            relationElement.SetAttribute("ForeignKeyColumn", foreignKeyColumn);

                            ForeignEntityData foreignEntityData;

                            if (field.ForeignEntity.ToLower() == entityName.ToLower())
                            {
                                foreignEntityData = GetForeignEntityData(table);
                            }
                            else
                            {
                                foreignEntityData = GetForeignEntityData(field.ForeignEntity);
                            }

                            if (foreignEntityData != null)
                            {
                                relationElement.SetAttribute("ReferencedTable", foreignEntityData.ReferencedTable);
                                relationElement.SetAttribute("ReferencedColumn", foreignEntityData.ReferencedColumn);
                                relationElement.SetAttribute("ReferencedTableSchema", foreignEntityData.ReferencedTableSchema);
                            }

                            if (field.DontBuildRelationOnDB)
                            {
                                relationElement.SetAttribute("Ignore", dontBuildRelationOnDB);
                            }

                            foreach (string fieldName in filedsWithSameNavigationPropertyName)
                            {
                                processedForeignKeyFields.Add(fieldName);
                            }
                        }
                    }

                    foreach (var indexElement in indexElements)
                    {
                        XmlElement indexXmlElement = doc.CreateElement("Index");

                        indexElement.Attributes().ToList().ForEach(element =>
                        {
                            indexXmlElement.SetAttribute(element.Name.LocalName, element.Value);
                        });

                        tableElement.AppendChild(indexXmlElement);
                    }

                    foreach (var uniqueConstraintElement in uniqueConstraintElements)
                    {
                        XmlElement uniqueConstraintXmlElement = doc.CreateElement("UniqueConstraint");

                        uniqueConstraintElement.Attributes().ToList().ForEach(element =>
                        {
                            uniqueConstraintXmlElement.SetAttribute(element.Name.LocalName, element.Value);
                        });

                        tableElement.AppendChild(uniqueConstraintXmlElement);
                    }

                    try
                    {
                        //doc.Save(dxmlFilePath);
                        FileStream fileStream;
                        if (File.Exists(dxmlFilePath))
                        {
                            fileStream = new FileStream(dxmlFilePath, FileMode.Truncate, FileAccess.Write);
                        }
                        else
                        {
                            fileStream = new FileStream(dxmlFilePath, FileMode.CreateNew, FileAccess.Write);
                        }

                        XmlWriterSettings xmlWriterSettings = new XmlWriterSettings() { Indent = true, NewLineOnAttributes = true, OmitXmlDeclaration = false, WriteEndDocumentOnClose = false };
                        XmlWriter xmlWriter = XmlWriter.Create(fileStream, xmlWriterSettings);

                        doc.Save(xmlWriter);
                        xmlWriter.Close();
                        xmlWriter.Dispose();
                        fileStream.Close();
                    }
                    catch (Exception exception)
                    {
                        MessageBox.Show("Error: " + exception.Message);
                    }
                }
                else
                {
                    MessageBox.Show("DXML File Path Is Not Valid");
                }
            }
        }

        #endregion

        #region GetDataType


        public static string GetDataType(string type)
        {
            string result = "Text";
            type = type.ToLower();
            switch (type)
            {
                case "varchar":
                    result = "Text";
                    break;
                case "nvarchar":
                    result = "nText";
                    break;
                case "decimal":
                    result = "Decimal";
                    break;

                case "double":
                    result = "Double";
                    break;

                case "int32":
                    result = "Integer";
                    break;

                case "bool":
                    result = "Boolean";
                    break;

                case "datetime":
                    result = "DateTime";
                    break;

            }

            return result;
        }

        #endregion

        #region GetStringValue

        private static string GetStringValue(object value)
        {

            if (value != null)
            {
                //if (value.ToString().Contains("\""))
                //{
                //    value = "\"" + value.ToString().Replace("\"", @"\""") + "\"";
                //}
                //if(value.ToString().Contains("\n"))
                //{
                //    value = value.ToString().Replace("\n", "\"" + "\n" + "\"");
                //}
                return "\"" + value.ToString().Replace("\"", "\u0022") + "\"";//Char.ConvertFromUtf32(34);//

                //  return value.ToString();
            }
            else
            {
                return null;
            }
        }


        #endregion

        #region SetAttribute

        private static void SetAttribute(string atrrName, string attrValue, XmlElement fieldElement, object field)
        {
            if (attrValue != null)
            {
                if (IsHebrew(attrValue) && fieldElement.Name != "Record")
                {
                    fieldElement.SetAttribute(atrrName + "Back_up", attrValue);

                    attrValue = ConvertToBase64(attrValue);
                    attrValue = "\"" + "BS64:" + attrValue + "\"";


                }
                fieldElement.SetAttribute(atrrName, attrValue);
            }
        }

        #endregion

        #region SetAttribute

        private static void SetAttribute(string atrrName, string attrValue, XmlElement fieldElement)
        {
            if (!string.IsNullOrEmpty(attrValue))
            {
                if (IsHebrew(attrValue) && fieldElement.Name != "Record")
                {
                    fieldElement.SetAttribute(atrrName + "Back_up", attrValue);

                    attrValue = ConvertToBase64(attrValue);
                    attrValue = "\"" + "BS64:" + attrValue + "\"";
                }
                fieldElement.SetAttribute(atrrName, attrValue);
            }
        }


        private static bool IsHebrew(string text)
        {
            Regex hebrewRegex = new Regex(@"[\u0590-\u05FF]");
            return hebrewRegex.IsMatch(text);
        }

        private static string ConvertToBase64(string text)
        {
            var bytes = Encoding.UTF8.GetBytes(text);
            return Convert.ToBase64String(bytes);
        }
        #endregion

        #region RemoveAttribute

        private static void RemoveAttribute(string atrrName, XmlElement fieldElement)
        {
            fieldElement.RemoveAttribute(atrrName);

        }
        #endregion

        public static string HashString(string text)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(text);
            byte[] buffer2 = new SHA256Managed().ComputeHash(bytes);
            char[] chArray = new char[0x10];
            for (int i = 0; i < chArray.Length; i++)
            {
                chArray[i] = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ"[buffer2[i] % "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ".Length];
            }
            return new string(chArray);
        }

        private static string GetDataTypeForDXMLColumn(string type, bool isFixedLength)
        {
            switch (type)
            {
                case "Boolean":
                    return "bit";
                case "Constant":
                case "PickList":
                case "List":
                case "Byte[]":
                    return "varchar";
                case "Date":
                    return "date";
                case "DateTime":
                    return "datetime";
                case "DateTime2":
                    return "datetime2";
                case "Decimal":
                case "UnsDecimal":
                    return "decimal";
                case "Double":
                case "SigDouble":
                    return "float";
                case "Integer":
                case "UnsInteger":
                    return "int";
                case "nText":
                    return "nvarchar";
                case "LookUp":
                case "Emails":
                case "Text":
                    return isFixedLength ? "char" : "varchar";
                case "Raw":
                    return "timestamp";
                case "Binary":
                    return "varbinary";
                case "Time":
                    return "time";
                case "BigInteger":
                    return "bigint";
                default:
                    return null;
            }
        }

        private static bool GetNullableForDXMLColumn(bool fieldIsRequired, bool fieldIsNullable, bool fieldIsPrimaryKey, string dxmlColumnDataType)
        {
            if (fieldIsPrimaryKey)
            {
                return false;
            }

            if (new string[] { "bit", "datetime", "datetime2", "decimal", "float", "int" }.Contains(dxmlColumnDataType))
            {
                if (!fieldIsRequired && fieldIsNullable)
                {
                    return true;
                }

                return false;
            }

            return !fieldIsRequired;
        }

        private static ForeignEntityData GetForeignEntityData(string foreignEntity)
        {
            string foreignEntityLXMLFilePath = GetForeignEntityLXMLFilePath(foreignEntity);

            if (foreignEntityLXMLFilePath != null)
            {
                XDocument xmlDocument = new XDocument();
                try
                {
                      xmlDocument = XDocument.Load(foreignEntityLXMLFilePath);

                }
                catch (Exception)
                {
                    MessageBox.Show(foreignEntityLXMLFilePath);
                   
                }

                string referencedTable = xmlDocument.Root.Attribute("DBTableName") == null ? null : xmlDocument.Root.Attribute("DBTableName").Value.Split('"')[1].Split('"')[0];
                string referencedTableSchema = xmlDocument.Root.Attribute("DxmlDatabaseSchemaCode") == null ? null : xmlDocument.Root.Attribute("DxmlDatabaseSchemaCode").Value;

                string[] primaryKeyFields = xmlDocument.Descendants("field").Where(x => x.Attribute("HasDataBaseField").Value == "true" && x.Attribute("IsPrimaryKey") != null && x.Attribute("IsPrimaryKey").Value == "true" && x.Attribute("FieldName") != null).Select(x => x.Attribute("FieldName").Value.Split('"')[1].Split('"')[0]).ToArray();
                string referencedColumn = primaryKeyFields.Length > 0 ? string.Join(",", primaryKeyFields) : null;

                return new ForeignEntityData
                {
                    ReferencedTable = referencedTable.ToLower().StartsWith("customs.") ? referencedTable.Split('.')[1] : referencedTable,
                    ReferencedTableSchema = referencedTableSchema,
                    ReferencedColumn = referencedColumn
                };
            }
            else
            {
                string foreignEntityDXMLFilePath = GetForeignEntityDXMLFilePath(foreignEntity);

                if (foreignEntityDXMLFilePath != null)
                {
                    XDocument xmlDocument = XDocument.Load(foreignEntityDXMLFilePath);

                    string referencedTable = xmlDocument.Root.Attribute("Name") == null ? null : xmlDocument.Root.Attribute("Name").Value;
                    string referencedTableSchema = xmlDocument.Root.Attribute("Schema") == null ? null : xmlDocument.Root.Attribute("Schema").Value;

                    string[] primaryKeyFields = xmlDocument.Descendants("Column").Where(x => x.Elements("Constraints").First().Attribute("PrimaryKey") != null && x.Elements("Constraints").First().Attribute("PrimaryKey").Value == "true").Select(x => x.Attribute("Name").Value).ToArray();
                    string referencedColumn = primaryKeyFields.Length > 0 ? string.Join(",", primaryKeyFields) : null;

                    return new ForeignEntityData
                    {
                        ReferencedTable = referencedTable,
                        ReferencedTableSchema = referencedTableSchema,
                        ReferencedColumn = referencedColumn
                    };
                }
            }

            return null;
        }

        private static ForeignEntityData GetForeignEntityData(ObjectTableViewModel table)
        {
            if (table != null)
            {
                string referencedTable = table.DBTableName;
                string referencedTableSchema = table.DxmlDatabaseSchemaCode;

                string[] primaryKeyFields = table.ObsList.Where(f => f.IsDBField && f.IsPrimaryKey).Select(f => f.FieldName).ToArray();
                string referencedColumn = primaryKeyFields.Length > 0 ? string.Join(",", primaryKeyFields) : null;

                return new ForeignEntityData
                {
                    ReferencedTable = referencedTable.ToLower().StartsWith("customs.") ? referencedTable.Split('.')[1] : referencedTable,
                    ReferencedTableSchema = referencedTableSchema,
                    ReferencedColumn = referencedColumn
                };
            }

            return null;
        }

        private static string GetForeignEntityLXMLFilePath(string foreignEntity)
        {
            string foreignEntityFileName = App.GetForeignEntityFileName(foreignEntity);
            return App.LXMLFilesPaths.Where(l => Path.GetFileName(l).ToLower() == (foreignEntityFileName.ToLower() + ".lxml")).FirstOrDefault();
        }

        private static string GetForeignEntityDXMLFilePath(string foreignEntity)
        {
            string foreignEntityFileName = App.GetForeignEntityFileName(foreignEntity);
            return App.DXMLFilesPaths.Where(d => Path.GetFileName(d).ToLower() == (foreignEntityFileName.ToLower() + ".dxml")).FirstOrDefault();
        }

        private class ForeignEntityData
        {
            public string ReferencedTable { get; set; }
            public string ReferencedTableSchema { get; set; }
            public string ReferencedColumn { get; set; }
        }
    }
}