using LteVideoPlayer.Api.Configs;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace LteVideoPlayer.Api.Helpers
{
    public static class ProcessHelper
    {
        public static ProcessResult RunProcess(
            string fileName,
            string arguments,
            CancellationToken cancellationToken = default)
        {
            var task = RunProcessAsync(fileName, arguments, cancellationToken);
            task.Wait(cancellationToken);
            return task.Result;
        }

        public static async Task<ProcessResult> RunProcessAsync(
            string fileName,
            string arguments,
            CancellationToken cancellationToken = default)
        {
            var result = new ProcessResult();
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
                await process.WaitForExitAsync(cancellationToken);
                if (cancellationToken.IsCancellationRequested)
                {
                    process.Kill(true);
                    result.IsCompleted = false;
                }
                else
                    result.IsCompleted = true;

                result.Output = outputStr.ToString();
                result.Error = errorStr.ToString();

                return result;
            }
        }
    }

    public class ProcessResult
    {
        public bool IsCompleted { get; set; }
        public string Output { get; set; }
        public string Error { get; set; }
    }
}
