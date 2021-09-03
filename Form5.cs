using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace CCD上位机
{
    public partial class Form5 : Form
    {
        public Form5()
        {
            InitializeComponent();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void button9_Click(object sender, EventArgs e)
        {
            webBrowser1.Url = new System.Uri("https://sdbs.db.aist.go.jp/sdbs/cgi-bin/cre_index.cgi", System.UriKind.Absolute);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            calibration calibration = new calibration();
            calibration.ShowDialog();
        }
    }
}
