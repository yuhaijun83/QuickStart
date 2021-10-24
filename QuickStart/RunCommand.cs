using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;

namespace QuickStart
{
    class RunCommand
    {
        public static string RunCmdByHide(string programName, string param)
        {
            Process process = new Process();
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.FileName = "cmd.exe";
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardError = true;
            process.StartInfo.RedirectStandardInput = true;
            process.StartInfo.RedirectStandardOutput = true;

            process.Start();
            process.StandardInput.WriteLine(programName + " " + param);
            process.StandardInput.WriteLine("exit");
            string outStr = process.StandardOutput.ReadToEnd();
            process.Close();
          
            return outStr;
        }

        public static void RunCmd(string programName, string param)
        {
            Process process = new Process();
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.FileName = "cmd.exe";
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardError = true;
            process.StartInfo.RedirectStandardInput = true;
            process.StartInfo.RedirectStandardOutput = true;

            process.Start(); 
            process.StandardInput.WriteLine("start " + programName + " " + param);
            process.Close();
        }

        public static void RunProgram(string programName, string param)
        {
            Process process = new Process();
            ProcessStartInfo startInfo = new ProcessStartInfo(programName, param);
            process.StartInfo = startInfo;
            process.Start();
        }

    }
}
