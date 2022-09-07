using System;
using System.Collections.Generic;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Diagnostics;
using System.ComponentModel;
using System.IO;
using System.Text.RegularExpressions;
using System.Collections.ObjectModel;

namespace WSLManager
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public class Dist
        {
            public int DistNo { get; set; }
            public string DistName { get; set; }
            public string DistDefault { get; set; }
            public string DistWSLVer { get; set; }
        }

        public MainWindow()
        {
            InitializeComponent();
        }

        private void InitWSL()
        {
            try
            {
                using (Process initWSLProcess = new Process())
                {

                    initWSLProcess.StartInfo.FileName = "C:\\Windows\\System32\\wsl.exe";
                    initWSLProcess.StartInfo.Arguments = "--list --verbose";

                    initWSLProcess.StartInfo.UseShellExecute = false;
                    initWSLProcess.StartInfo.CreateNoWindow = true;

                    initWSLProcess.StartInfo.RedirectStandardInput = false;
                    initWSLProcess.StartInfo.RedirectStandardOutput = true;

                    initWSLProcess.Start();
                    string result = initWSLProcess.StandardOutput.ReadToEnd();
                    string[][] DistList = ParseWSLOutput(result);

                    for (int i = 0; i < DistList.Length; i++)
                    {
                        dataGrid.ItemsSource = new ObservableCollection<Dist>
                        {
                            new Dist {DistNo=i,DistDefault="",DistName=DistList[i][1], DistWSLVer=DistList[i][14] }
                        };
                    }

                    initWSLProcess.WaitForExit();
                    
                    initWSLProcess.Close();
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                return;
            }
            return;
        }
        private static string[][] ParseWSLOutput(string text)
        {
            string[] rawStrings = text.Split(new string[] {"\r\n", "\r", "\n"}, StringSplitOptions.RemoveEmptyEntries);
            string[][] ret = new string[rawStrings.Length][];
            int i = 0;
            foreach (string s in rawStrings)
            {
                ret[i] = s.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                i++;
            }

            return ret;
        }

        private static string ConvertSJIStoUTF8(string text)
        {
            byte[] bytesData = Encoding.UTF8.GetBytes(text);
            string ret = '%' + BitConverter.ToString(bytesData).Replace('-', '%');
            return ret;
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            InitWSL();
        }
    }
}
