using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace P2PlayDesktop
{
    public partial class Form1 : Form
    {
        private Boolean connect;
        private Hotspot hotspot;
        private ConnectionManager cm;

        public Form1()
        {
            InitializeComponent();
            hotspot = new Hotspot();
        }

        delegate void SetTextCallback(string text);

        public void Log(string s)
        {
            if (this.txtConsole.InvokeRequired)
            {
                SetTextCallback d = new SetTextCallback(Log);
                this.Invoke(d, new object[] { s });
            }
            else
            {
                this.txtConsole.AppendText("\r\n" + s);   
            }
        }

        public static bool IsAdmin()
        {
            WindowsIdentity id = WindowsIdentity.GetCurrent();
            WindowsPrincipal p = new WindowsPrincipal(id);
            return p.IsInRole(WindowsBuiltInRole.Administrator);
        }

        public void RestartElevated()
        {
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.UseShellExecute = true;
            startInfo.CreateNoWindow = true;
            startInfo.WorkingDirectory = Environment.CurrentDirectory;
            startInfo.FileName = System.Windows.Forms.Application.ExecutablePath;
            startInfo.Verb = "runas";
            try
            {
                Process p = Process.Start(startInfo);
            }
            catch
            {

            }

            System.Windows.Forms.Application.Exit();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            if (!IsAdmin())
            {
                RestartElevated();
            }
        }

        private void Form1_Closing(object sender, EventArgs e)
        {
            hotspot.createHotspot(null, null, false);
            Application.Exit();
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            if (btnStart.Text == "Stop")
            {
                hotspot.createHotspot(null, null, false);
                txtSSID.Enabled = true;
                txtKey.Enabled = true;
                btnStart.Text = "Start";
                connect = false;
                cm.closeConnections();
            }
            else
            {
                string ssid = txtSSID.Text, key = txtKey.Text;
                if (!connect)
                {
                    if (ssid == null || ssid == "")
                    {
                        MessageBox.Show("SSID cannot be left blank !",
                        "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {

                        if (key == null || key == "")
                        {
                            MessageBox.Show("Key value cannot be left blank !",
                            "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        else
                        {
                            if (key.Length >= 8)
                            {
                                hotspot.createHotspot(ssid, key, true);
                                txtSSID.Enabled = false;
                                txtKey.Enabled = false;
                                btnStart.Text = "Stop";
                                connect = true;

                                cm = new ConnectionManager(this);
                            }
                            else
                            {
                                MessageBox.Show("Key should be more then or Equal to 8 Characters !",
                                "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }
                        }
                    }
                }
                else
                {
                    hotspot.createHotspot(null, null, false);
                    txtSSID.Enabled = true;
                    txtKey.Enabled = true;
                    btnStart.Text = "Start";
                    connect = false;
                    cm.closeConnections();
                }
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            cm.closeConnections();
        }
    }
}
