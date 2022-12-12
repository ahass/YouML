using System.Reflection;
using YouML.Parser;
using YouML.Renderer;

namespace YouML.Tests
{
    [TestClass]
    public class UnitTests
    {
        [TestMethod]
        public void TestStaticClasses()
        {
            var assembly = Assembly.GetExecutingAssembly();

            string resourceName = assembly.GetManifestResourceNames()
                .Single(str => str.EndsWith("CsExtensions.cs"));

            using Stream stream = assembly.GetManifestResourceStream(resourceName);

            using StreamReader reader = new StreamReader(stream);
            var fileCode = reader.ReadToEnd();

            if (fileCode.Equals(string.Empty)) return;

            var classCode = ClassParser.Parse(fileCode);

            var plantCode = UmlRenderer.Render(classCode);

            string exp = $"@startuml\r\nFrame YouML.Tests.Files #GreenYellow/LightGoldenRodYellow{{\r\nclass CsExtensions {{\r\n.. Methods ..\r\n+bool IsUpperAlpha()\r\n+bool IsLowerAlpha()\r\n+bool IsAlpha()\r\n+bool IsNumber()\r\n+string IfNullOrWhiteSpace()\r\n+bool IsVoid()\r\n+string Repeat()\r\n+int CountSubstring()\r\n+int IndexOfToOccurance()\r\n+string GetSubDirectory()\r\n+string GetSubDirectoryName()\r\n+string PadCenter()\r\n+string ToList()\r\n}}\r\n}}\r\n@enduml\r\n";

            Assert.AreEqual(exp, plantCode);
        }
    }
}
