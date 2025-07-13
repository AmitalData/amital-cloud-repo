using MeatadataGeneratorTool.CloseTablesData;
using MeatadataGeneratorTool.DataContractsModule;
using MeatadataGeneratorTool.EventTypes;
using MeatadataGeneratorTool.Helpers;
using MeatadataGeneratorTool.MenuButtons;
using MeatadataGeneratorTool.QueryModule;
using MeatadataGeneratorTool.ScreensModule;
using MeatadataGeneratorTool.TabsModule;
using MeatadataGeneratorTool.ToolVersion;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Xml;
using System.Xml.Linq;

namespace MeatadataGeneratorTool
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static string DirectOpenPath { get; set; }
        public static string ProjectName { get; set; } = "Logitude";
		public static ObjectTableControl CurrentControl { get; set; }
        public static MainWindowControl MainControl { get; set; }

        public static List<string> LXMLFilesPaths { get; set; }
        public static List<string> DXMLFilesPaths { get; set; }
        public string CurrentVersion = "0.0";

        private string GetAssemplyVersion()
        {
            Assembly currentAssembly = Assembly.GetExecutingAssembly();
            CustomAttributeData AssemblyVersion = currentAssembly.CustomAttributes.Where(a => a.AttributeType.Name == "AssemblyFileVersionAttribute").FirstOrDefault();
            if (AssemblyVersion != null)
            {
                return (string)AssemblyVersion.ConstructorArguments[0].Value;
            }
            return "0.0";
        }
        protected override void OnStartup(StartupEventArgs e)
        {
            CurrentVersion = GetAssemplyVersion();
            if (e.Args != null && e.Args.Length > 0)
            {
                try
                {
                    string workingDirectory = Directory.GetCurrentDirectory();
                    string projectDirectory = Directory.GetParent(workingDirectory).Parent.FullName;
                    //MessageBox.Show(projectDirectory);
                    if (projectDirectory.EndsWith(@"Logitude"))
                    {
                        projectDirectory = projectDirectory + @"\MeatadataGeneratorTool\MeatadataGeneratorTool\MeatadataGeneratorTool\ToolVersion";
                    }
                    else
                    {
                        projectDirectory = projectDirectory.Replace(@"\Logitude.MetaData", "") + @"\MeatadataGeneratorTool\MeatadataGeneratorTool\MeatadataGeneratorTool\ToolVersion"; 
                    }
                    string[] DirectoryFiles = Directory.GetFiles(projectDirectory, "Version.vxml", SearchOption.AllDirectories);//, "Version.vxml", SearchOption.AllDirectories);
                    string verisonFilePath = DirectoryFiles[0];//.Where(a => a.Contains("Version.vxml")).FirstOrDefault(); 
                    string verisonFileString = File.ReadAllText(verisonFilePath);
                    VersionInfo versionInfo = verisonFileString.ParseXML<VersionInfo>();
                    if (versionInfo == null || versionInfo.VersionNo != CurrentVersion)
                    {
                        MessageBox.Show("You don't have the latest version of the tool, Please rebuild the tool to use the latest version. ( " + versionInfo.VersionNo +" )");
                        base.OnStartup(e);
                        Environment.Exit(0);
                        return;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    Environment.Exit(0);
                    return;

                }
                DirectOpenPath = e.Args[0].ToString();
                ProjectName = GetSolutionFolderName(DirectOpenPath) ?? ProjectName;


				if (!string.IsNullOrEmpty(App.DirectOpenPath))
                {
					FileStream stream = null;
                    try
                    {
                        stream = new FileStream(App.DirectOpenPath, FileMode.Open);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                        base.OnStartup(e);
                        return;
                    }

                    try
                    {

                        XmlDocument document = new XmlDocument();
                        document.Load(stream);
                        XmlParserHelper ParserHelper = new XmlParserHelper();
                        ObjectTableViewModel model = ParserHelper.LoadObjectTableData(document);
                        CurrentControl = new ObjectTableControl();
                        CurrentControl.DataContext = model;

                        //CurrentControl.WindowStyle = WindowStyle.None;
                        CurrentControl.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                        CurrentControl.WindowState = WindowState.Maximized;

                        CurrentControl.Show();




                        CurrentControl.Closed += CurrentControl_Closed;
                        stream.Close();
                        stream.Dispose();

                        LXMLFilesPaths = new List<string>();
                        DXMLFilesPaths = new List<string>();

						Thread thread = new Thread(new ThreadStart(GetLXMLAndDXMLFilesPaths));
						thread.Start();
                    }
                    catch (Exception err)
                    {
                        LXMLFilesPaths = new List<string>();
                        DXMLFilesPaths = new List<string>();

						Thread thread = new Thread(new ThreadStart(GetLXMLAndDXMLFilesPaths));
						thread.Start();
                        //MessageBox.Show(err.Message);
                        if (DirectOpenPath.Contains(".lxml"))
                        {
                            //MessageBox.Show(err.Message + Environment.NewLine + err.StackTrace);
                            var FileName = Path.GetFileName(DirectOpenPath).Replace(".lxml", "");
                            MainWindowViewModel model = new MainWindowViewModel();
                            model.ObjectTableName = FileName;
                            MainControl = new MainWindowControl();
                            MainControl.DataContext = model;
                            MainControl.Show();
                            MainControl.Closed += MainControl_Closed;
                        }
                        else
                        {
                            MessageBox.Show(err.Message);
                        }
                    };
                    //XmlElement element = new XmlElement();


                    //XElement el = XElement.Parse(fileContent);
                    // MessageBox.Show(fileContent);
                }
            }
            else
            {
                LoadFileWindow loadFileWindow = new LoadFileWindow();
                loadFileWindow.Show();
            }

            base.OnStartup(e);
        }

		static string GetSolutionFolderName(string filePath)
		{
			var directory = Path.GetDirectoryName(filePath);

			while (directory != null)
			{
				if (Directory.GetFiles(directory, "*.sln").Length > 0)
				{
					return Path.GetFileName(directory); 
				}

				directory = Directory.GetParent(directory)?.FullName;
			}

			return null; 
		}

		public void GetLXMLAndDXMLFilesPaths()
        {
            try
            {
                string projectDirectory = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName + @"\";
                if (projectDirectory.Contains(@"\" + ProjectName + @"\"))
                {
                    string logitudePath = projectDirectory.Split(new string[] { @"\" + ProjectName + @"\" }, StringSplitOptions.None)[0];
                    LXMLFilesPaths = Directory.GetFiles(logitudePath + @"\" + ProjectName + @"\", "*.lxml", SearchOption.AllDirectories).Where(l => !l.ToLower().Contains("logitudefrontend")).ToList();
                    DXMLFilesPaths = Directory.GetFiles(logitudePath + @"\" + ProjectName + @"\", "*.dxml", SearchOption.AllDirectories).ToList();
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show("Error While Loading Files: " + exception.Message);
            }
        }

        public static string GetForeignEntityFileName(string foreignEntity)
        {
            if (foreignEntity == "AutomaticExternalRconcilMthod")
            {
                return "AutomaticExternalReconcileMethod";
            }

            if (foreignEntity == "DWQuery")
            {
                return "DWQuery ";
            }

            return foreignEntity;
        }

        private void MainControl_Closed(object sender, EventArgs e)
        {
            MainControl.Close();
        }

        void CurrentControl_Closed(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }

    }
}
