using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace P2PlayDesktop
{
    class Hotspot
    {
        private Process process;

        private void createCmd()
        {
            ProcessStartInfo processStartInfo = new ProcessStartInfo("cmd.exe");
            processStartInfo.RedirectStandardInput = true;
            processStartInfo.RedirectStandardOutput = true;
            processStartInfo.CreateNoWindow = true;
            processStartInfo.UseShellExecute = false;
            process = Process.Start(processStartInfo);
        }

        private void setUpVirtualNetwork(string ssid, string key)
        {
            process.StandardInput.WriteLine("netsh wlan set hostednetwork mode=allow ssid=" + ssid + " key=" + key);
            process.StandardInput.WriteLine("netsh wlan start hostednetwork");
            process.StandardInput.Close();
        }

        private void tearDownVirtualNetwork() 
        {
            process.StandardInput.WriteLine("netsh wlan stop hostednetwork");
            process.StandardInput.Close();
        }

        public Boolean createHotspot(string ssid, string key, bool status)
        {
            createCmd();

            if (process != null)
            {
                if (status)
                {
                    setUpVirtualNetwork(ssid, key);
                    return true;
                }
                else
                {
                    tearDownVirtualNetwork();
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
    }
}
