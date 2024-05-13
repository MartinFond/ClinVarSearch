using System;
using System.Diagnostics;

public class PythonRunner
{
    public string RunPythonScript(string pythonInterpreter, string pythonScript, string[] arguments)
    {
        // Create a new process to execute the Python script
        ProcessStartInfo startInfo = new ProcessStartInfo();
        startInfo.FileName = pythonInterpreter;
        startInfo.Arguments = $"{pythonScript} {string.Join(" ", arguments)}"; // Pass script path and arguments
        startInfo.UseShellExecute = false;
        startInfo.RedirectStandardOutput = true;

        // Start the process
        using (Process process = Process.Start(startInfo))
        {
            // Read the output of the Python script
            string output = process.StandardOutput.ReadToEnd();

            // Wait for the process to finish
            process.WaitForExit();

            // Return the output
            return output;
        }
    }
}

