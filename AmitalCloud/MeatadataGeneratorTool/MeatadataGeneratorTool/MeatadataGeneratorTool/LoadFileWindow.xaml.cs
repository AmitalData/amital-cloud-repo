using MeatadataGeneratorTool.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Xml;
using Path = System.IO.Path;

namespace MeatadataGeneratorTool
{
    /// <summary>
    /// Interaction logic for LoadFileWindow.xaml
    /// </summary>
    public partial class LoadFileWindow : Window
    {
        public LoadFileWindow()
        {
            InitializeComponent();
        }

        private void btnLoadLxmlFile_Click(object sender, RoutedEventArgs e)
        {
            string projectPath = Path.GetDirectoryName(System.IO.Path.GetDirectoryName(System.IO.Directory.GetCurrentDirectory()));
            DirectoryInfo solutionDir = System.IO.Directory.GetParent(projectPath);
            string solutionDirectory = solutionDir.FullName;

            string dir = solutionDirectory + @"\Logitude.MetaData\EntityFiles";//.Replace(@"MeatadataGeneratorTool\MeatadataGeneratorTool", @"MetaDataGenerator\GeneratedFiles\New");
           
            // Create OpenFileDialog 
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();



            // Set filter for file extension and default file extension 
            dlg.DefaultExt = ".lxml";
            dlg.Filter = "LXML files (*.lxml)|*.lxml|All files (*.*)|*.*";
            dlg.InitialDirectory = dir;

            // Display OpenFileDialog by calling ShowDialog method 
            Nullable<bool> result = dlg.ShowDialog();


            // Get the selected file name and display in a TextBox 
            if (result == true)
            {

                FileStream  stream = new FileStream(dlg.FileName, FileMode.Open);

                // Open document 
                string filename = dlg.FileName;
                //textBox1.Text = filename;
                XmlDocument document = new XmlDocument();
                document.Load(stream);
                XmlParserHelper ParserHelper = new XmlParserHelper();
                ObjectTableViewModel model = ParserHelper.LoadObjectTableData(document);

                rtxtFileContent.AppendText(document.OuterXml);
            }
        }
    }
}
