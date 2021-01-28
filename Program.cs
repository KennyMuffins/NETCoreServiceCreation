using NSwag;
using NSwag.CodeGeneration.CSharp;
using System;
using System.IO;
using System.Text.RegularExpressions;

namespace CSharpClientGeneratorNamespace
{
    class Program
    {
        static string baseLocation = @"C:\Users\dfuser\Documents\Projects\GeneratedCode\";

        static void Main(string[] args)
        {
            var serviceSettings = new CSharpClientGeneratorSettings
            {
                ClassName = "ContentAppService",
                ClientBaseClass = "ContentManagementAppService",
                ClientBaseInterface = "IApplicationService",
                GenerateClientClasses = true,
                GenerateClientInterfaces = true,
                GenerateDtoTypes = true,
                CSharpGeneratorSettings =
                {
                    Namespace = "ContentManagement.Content"
                }
            };
            GenerateCodeInterfaceAndService(serviceSettings, "ContentAppService.cs");


            //var serviceSettingsDtos = new CSharpClientGeneratorSettings
            //{
            //    ClassName = "ContentAppServiceDto",
            //    ClientBaseClass = "",
            //    ClientBaseInterface = "",
            //    GenerateClientClasses = false,
            //    GenerateClientInterfaces = false,
            //    GenerateDtoTypes = true,
            //    CSharpGeneratorSettings =
            //    {
            //        Namespace = "ContentManagement.Content"
            //    }
            //};
            //GenerateCode(serviceSettingsDtos, "ContentAppServiceDto.cs");
        }

        public static async void GenerateCodeInterfaceAndService(CSharpClientGeneratorSettings settings, string serviceName)
        {
            System.Net.WebClient wclient = new System.Net.WebClient();

            string url = @"https://cloud.squidex.io/api/content/ilearn/swagger/v1/swagger.json";

            var document = await OpenApiDocument.FromJsonAsync(wclient.DownloadString(url));

            wclient.Dispose();

            var generator = new CSharpClientGenerator(document, settings);
            string allCode = generator.GenerateFile();

            int indexSplitInterfaceAndApp = IndexOfOccurence(allCode, "[System.CodeDom.Compiler.GeneratedCode(\"NSwag\", \"13.10.1.0 (NJsonSchema v10.3.3.0 (Newtonsoft.Json v12.0.0.0))\")]", 2);
            int indexSplitDto = IndexOfOccurence(allCode, "[System.CodeDom.Compiler.GeneratedCode(\"NJsonSchema\"", 1);

            string nameSpace = "namespace ContentManagement.Content {\n";

            //Interface Service
            string interfaceCode = allCode.Remove(indexSplitInterfaceAndApp) + "}";
            File.WriteAllText(baseLocation + "I" + serviceName, interfaceCode);

            //ServiceCode
            int length = indexSplitDto - indexSplitInterfaceAndApp;
            string serviceCode = nameSpace + allCode.Substring(indexSplitInterfaceAndApp, length) + "}";
            File.WriteAllText(baseLocation + serviceName, serviceCode);

            string dtoCode = nameSpace +  allCode.Substring(indexSplitDto);
            File.WriteAllText(baseLocation + "Dto" + serviceName, dtoCode);

        }


        public static async void GenerateCode(CSharpClientGeneratorSettings settings, string serviceName)
        {
            System.Net.WebClient wclient = new System.Net.WebClient();

            string url = @"https://cloud.squidex.io/api/content/ilearn/swagger/v1/swagger.json";

            var document = await OpenApiDocument.FromJsonAsync(wclient.DownloadString(url));

            wclient.Dispose();

            var generator = new CSharpClientGenerator(document, settings);
            string allCode = generator.GenerateFile();

            File.WriteAllText(baseLocation + serviceName, allCode);

        }

        private static int IndexOfOccurence(string s, string match, int occurence)
        {
            int i = 1;
            int index = 0;

            while (i <= occurence && (index = s.IndexOf(match, index + 1)) != -1)
            {
                if (i == occurence)
                    return index;

                i++;
            }

            return -1;
        }
    }
}
