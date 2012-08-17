using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ErrorLogging
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void buttonTrace_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Trace.TraceInformation(textBoxInfo.Text);
        }
    }
}
