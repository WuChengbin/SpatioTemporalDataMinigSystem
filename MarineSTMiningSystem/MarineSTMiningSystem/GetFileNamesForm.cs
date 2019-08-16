using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MarineSTMiningSystem
{
    public partial class GetFileNamesForm : Form
    {
        public GetFileNamesForm()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string inFolderPath = textBox1.Text.Trim();
            string outFilePath = textBox2.Text.Trim();
            string[] filePaths= Directory.GetFiles(inFolderPath);
            StreamWriter sw = new StreamWriter(outFilePath);
            foreach(string filePath in filePaths)
            {
                string fileName = Path.GetFileNameWithoutExtension(filePath);
                sw.WriteLine(fileName);
            }
            sw.Close();
            MessageBox.Show("成功");
        }
    }
}
