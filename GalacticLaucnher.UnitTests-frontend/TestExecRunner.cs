using System.Diagnostics;
using System.Runtime.InteropServices;
using GalacticLauncher.Frontend.Services.Executables;

namespace GalacticLauncher.Frontend.Tests.Services.Executables
{
    public class ExecRunnerTests : IDisposable
    {
        private readonly string _tempDirectory;
        private readonly string _dummyFilePath;

        public ExecRunnerTests()
        {
            _tempDirectory = Path.Combine(Path.GetTempPath(), $"ExecRunnerTests_{Guid.NewGuid()}");
            Directory.CreateDirectory(_tempDirectory);
            _dummyFilePath = Path.Combine(_tempDirectory, "dummy_app.exe");

            File.WriteAllText(_dummyFilePath, string.Empty);
        }

        public void Dispose()
        {
            if (Directory.Exists(_tempDirectory))
            {
                try { Directory.Delete(_tempDirectory, true); } catch { }
            }
        }

        private ExecRunner CreateRunner()
        {
            return new ExecRunner();
        }

        /// <summary>
        /// INITIALIZATION TESTS
        /// </summary>

        [Fact]
        public void RunProcess_ShouldCorrectlyInitializeProcessProperties()
        {
            var runner = CreateRunner();

            string safeExecPath;
            string cliArgs;

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                safeExecPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), "cmd.exe");
                cliArgs = "/c echo Test";
            }
            else
            {
                safeExecPath = "/bin/sh";
                cliArgs = "-c \"echo Test\"";
            }

            using Process? process = runner.RunProcess(safeExecPath, cliArgs);

            Assert.NotNull(process);

            Assert.Equal(safeExecPath, process.StartInfo.FileName);
            Assert.Equal(cliArgs, process.StartInfo.Arguments);
            Assert.False(process.StartInfo.UseShellExecute);
            Assert.Equal(Path.GetDirectoryName(safeExecPath), process.StartInfo.WorkingDirectory);

            if (!process.HasExited)
            {
                process.Kill();
            }
        }

        /// <summary>
        /// LINUX/MAC TESTS
        /// </summary>

        [Fact]
        public void RunProcess_ShouldApplyExecutePermission_WhenOnUnixAndPermissionIsMissing()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return;
            }

            var runner = CreateRunner();

            File.SetUnixFileMode(_dummyFilePath, UnixFileMode.UserRead | UnixFileMode.UserWrite);

            try
            {
                using var _ = runner.RunProcess(_dummyFilePath, "");
            }
            catch (Exception)
            {
            }

            UnixFileMode currentMode = File.GetUnixFileMode(_dummyFilePath);

            Assert.True(currentMode.HasFlag(UnixFileMode.UserExecute),
                "UserExecute flag should be added to the file automatically.");
        }
    }
}