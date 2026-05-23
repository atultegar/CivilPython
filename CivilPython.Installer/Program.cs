using WixSharp;
namespace CivilPython.Installer
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string version = args.Length > 0 
                ? args[0] 
                : "2023";

            string bundleName = $"CivilPython{version}.bundle";

            string sourceBundle = $@"..\CivilPython\{bundleName}";

            var versionInt = Int32.Parse(version);

            var guids = GetGuids(version);

            PackageContentsGenerator.Generate(
                sourceBundle,
                version,
                guids.ProductGuid.ToString(),
                guids.UpgradeGuid.ToString(),
                appVersion: "2.3.1");

            string appDataFolder = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), 
                "Autodesk", 
                "ApplicationPlugins", 
                bundleName);

            string programDataFolder = $@"C:\ProgramData\Autodesk\ApplicationPlugins\{bundleName}";

            string installDir = versionInt >= 2026
                ? appDataFolder
                : programDataFolder;

            var project = new Project(
                $"CivilPython {version}",
                new Dir(installDir,
                    new Files($@"{sourceBundle}\*.*"),
                    new DirFiles($@"{sourceBundle}\Contents\win\*.*")
                )
            );

            project.Platform = Platform.x64;            

            project.GUID = guids.UpgradeGuid;

            project.OutFileName = $"CivilPython{version}Installer";

            var licenseFile = Path.Combine(ResolveInstallerProjectRoot(), "Assets", "LICENSE.rtf");
            if (System.IO.File.Exists(licenseFile))
            {
                project.LicenceFile = licenseFile;
            }

            project.ControlPanelInfo.Manufacturer = "Autodesk/BIMformative";

            project.Version = new Version("2.3.1");

            project.MajorUpgrade = MajorUpgrade.Default;

            project.OutDir = Path.Combine(Environment.CurrentDirectory, "output");

            Compiler.BuildMsi(project);
        }

        private static (Guid ProductGuid, Guid UpgradeGuid) GetGuids(string version)
        {
            switch (version)
            {
                case "2023":
                    return (
                        new Guid("C91B294E-FCDF-4B49-A1E6-F7A4472666EF"),
                        new Guid("F3EB1ECD-2B78-483F-8352-DAFEB0A3AFC5")
                    );

                case "2024":
                    return (
                        new Guid("2AF8D273-07FC-4319-8E3F-ABCDC9A0217E"),
                        new Guid("03B1B30F-C89C-42FC-9790-7AE427205110")
                    );

                case "2025":
                    return (
                        new Guid("251148E4-42AC-4AD5-BECC-480E68ADD43D"),
                        new Guid("E1AEE694-7A1B-4138-ADB9-B1B7E9F27004")
                    );

                case "2026":
                    return (
                        new Guid("3F02A820-2D48-4443-82F6-8D8DFE2CEF86"),
                        new Guid("1999E852-DBFB-4207-B06F-4170DD4DE8FD")
                    );

                case "2027":
                    return (
                        new Guid("6299FAF1-D1D3-47B2-AE4B-5D7FE98B4054"),
                        new Guid("2522EBAD-1540-4F0A-8810-D08FA10B2BBE")
                    );

                default:
                    return (
                        Guid.NewGuid(),
                        Guid.NewGuid()
                    );
            }
        }

        private static string ResolveInstallerProjectRoot()
        {
            var current = AppContext.BaseDirectory;
            var dir = new DirectoryInfo(current);

            while (dir != null)
            {
                var projectFile = Path.Combine(dir.FullName, "CivilPython.Installer.csproj");
                if (System.IO.File.Exists(projectFile))
                    return dir.FullName;

                dir = dir.Parent;
            }

            throw new DirectoryNotFoundException(
                "Could not locate CivilPython.Installer.csproj from " + current);
        }

    }
}
