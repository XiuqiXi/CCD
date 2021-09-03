using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;

namespace CCD上位机
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
        }
        public bool StartProcess(string runFilePath, params string[] args)
        {
            string s = "";
            foreach (string arg in args)
            {
                s = s + arg + " ";
            }
            s = s.Trim();
            Process process = new Process();//创建进程对象    
            ProcessStartInfo startInfo = new ProcessStartInfo(runFilePath, s); // 括号里是(程序名,参数)
            process.StartInfo = startInfo;
            process.Start();
            return true;
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            Form1 form1 = new Form1();
            form1.ShowDialog();
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            //string exe_path = "E:/example.exe";  // 被调exe
            //string[] the_args = { "1", "2", "3", "4" };   // 被调exe接受的参数
            //StartProcess(exe_path, the_args);
            Form3 form3 = new Form3();
            form3.ShowDialog();
        }

        private void label8_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox4_Click(object sender, EventArgs e)
        {
            Form5 form5 = new Form5();
            form5.ShowDialog();
        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox5_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("explorer.exe", "https://xiuqixi.github.io/");
        }

        
        private void toolTip1_Popup(object sender, PopupEventArgs e)
        {

        }

        private void Form2_Load(object sender, EventArgs e)
        {
            toolTip1.IsBalloon = true;
            toolTip1.SetToolTip(this.pictureBox1, "This section is for obtaining raw data from CCD");
        }
    }
}
