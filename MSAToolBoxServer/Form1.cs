using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.ServiceModel;
using MSAToolBoxServices;

namespace MSAToolBoxServer
{
    public partial class Form1 : Form
    {
        ServiceHost host;
        public Form1()
        {
            InitializeComponent();
        }

        private void btnStartRegService_Click(object sender, EventArgs e)
        {
            host = new ServiceHost(typeof(RegService));
            host.Open();
            status.Text = "服务已启动。";
            btnStartRegService.Enabled = false;
            button1.Enabled = true;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (host != null)
                host.Close();
            status.Text = "服务已停止。";
            btnStartRegService.Enabled = true;
            button1.Enabled = false;
        }
    }
}
