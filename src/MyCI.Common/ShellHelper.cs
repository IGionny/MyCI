using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace MyCI.Common
{
    public static class ShellHelper
    {
        public static string Win(string cmd, string workingDirectory = null)
        {
            
            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "git",
                    Arguments = $"PULL",
                    WorkingDirectory = workingDirectory,
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };
            try
            {
                process.Start();
                var result = process.StandardOutput.ReadToEnd();
                process.WaitForExit();
                return result;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            finally
            {
                process?.Dispose();
            }
        }

        

        public static string Execute(string cmd, string workingDirectory = null)
        {
            if (System.Runtime.InteropServices.RuntimeInformation
                .IsOSPlatform(OSPlatform.Windows))
            {
                return Win(cmd,workingDirectory);
            }
            return Bash(cmd,workingDirectory);
        }

        public static string Bash(string cmd, string workingDirectory = null)
        {
            var escapedArgs = cmd.Replace("\"", "\\\"");

            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "/bin/bash",
                    Arguments = $"-c \"{escapedArgs}\"",        
                    WorkingDirectory =  workingDirectory,
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };
            process.Start();
            var result = process.StandardOutput.ReadToEnd();
            process.WaitForExit();
            process.Dispose();
            return result;
        }
    }
}