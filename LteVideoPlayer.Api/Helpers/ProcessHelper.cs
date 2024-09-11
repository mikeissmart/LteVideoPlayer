using LteVideoPlayer.Api.Configs;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace LteVideoPlayer.Api.Helpers
{
    public static class ProcessHelper
    {
        public static bool RunProcess(
            string fileName,
            string arguments,
            out string output,
            out string error,
            CancellationToken cancellationToken = default)
        {
            output = "";
            error = "";

            var outputStr = new StringBuilder();
            var errorStr = new StringBuilder();

            var startInfo = new ProcessStartInfo
            {
                FileName = fileName,
                Arguments = arguments,
                CreateNoWindow = true,
                RedirectStandardError = true,
                RedirectStandardOutput = true,
                UseShellExecute = false,
            };

            using (var process = new Process())
            {
                process.StartInfo = startInfo;

                process.OutputDataReceived += (s, e) =>
                {
                    lock (outputStr)
                    {
                        outputStr.Append(e.Data);
                    }
                };
                process.ErrorDataReceived += (s, e) =>
                {
                    lock (errorStr)
                    {
                        errorStr.Append(e.Data);
                    }
                };

                process.Start();
                process.BeginOutputReadLine();
                process.BeginErrorReadLine();
                while (!process.HasExited && !cancellationToken.IsCancellationRequested)
                {
                    Thread.Sleep(5000);
                }
                if (cancellationToken.IsCancellationRequested)
                {
                    process.Kill(true);
                    return false;
                }

                output = outputStr.ToString();
                error = errorStr.ToString();

                return true;
            }
        }
    }
}
