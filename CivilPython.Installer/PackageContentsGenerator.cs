using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CivilPython.Installer
{
    internal static class PackageContentsGenerator
    {
        public static void Generate(
            string bundleDir,
            string version,
            string productCode,
            string upgradeCode,
            string appVersion = "2.3")
        {
            string bundleName = $"CivilPython{version}";

            string series = GetSeries(version);

            string templatePath = "PackageContents.template.xml";

            string outputPath = Path.Combine(bundleDir, "PackageContents.xml");

            string template = File.ReadAllText(templatePath);

            template = template.Replace("$(BundleName)", bundleName);

            template = template.Replace("$(AssemblyName)", bundleName);

            template = template.Replace("$(ProductCode)", productCode);

            template = template.Replace("$(UpgradeCode)", upgradeCode);

            template = template.Replace("$(Series)", series);

            template = template.Replace("$(AppVersion)", appVersion);

            template = template.Replace("$(Commands)", GenerateCommands());

            File.WriteAllText(outputPath, template, Encoding.UTF8);

            Console.WriteLine($"PackageContents.xml generated: {outputPath}");

        }

        private static string GenerateCommands()
        {
            var commands = new List<string>
            {
                "Python",
                "-python",
                "-replacesolid",
                "-ExportLandFeatureLinesToXml",
                "-ExportCorridorFeatureLinesToXML",
                "-ExportSubassemblyShapesToXML",
                "-ExportSubassemblyLinksToXML",
                "-ExportSurfaceToXML",
                "-ExportSurfaceTrianglesToXML",
                "-CreatePropertySetDefinition",
                "-AssignPropertySet",
                "-BindAllInsert"
            };

            var sb = new StringBuilder();

            sb.AppendLine("<Commands>");

            foreach(var cmd in commands)
            {
                sb.AppendLine(
                    $"<Command Local=\"{cmd}\" Global=\"{cmd}\" />");
            }

            sb.AppendLine("</Commands>");

            return sb.ToString();
        }

        private static string GetSeries(string version)
        {
            return version switch
            {
                "2023" => "R24.2",
                "2024" => "R24.3",
                "2025" => "R25.0",
                "2026" => "R25.1",
                "2027" => "R26.0",
                _ => throw new Exception($"Unknown version: {version}")
            };
        }
    }
}
