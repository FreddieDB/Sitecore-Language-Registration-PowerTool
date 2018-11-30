using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using CsvHelper;

namespace FdB.SitecoreLangRegPowerTool
{
    /// <summary>
    /// Console application to bulk register custom culture codes in Windows. The application expects no cmd line params.
    /// It also generates a Sitecore Powershell script based on the configuration data in the CSV file. This is used to add new Languages to Sitecore in system\languages.
    /// See https://github.com/FreddieDB/Sitecore-Language-Registration-PowerTool for source code repository and documentation.
    /// </summary>
    class Program
    {

        static void Main(string[] args)
        {
            try
            {
                var originalConsoleColor = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.DarkGreen;
                Console.WriteLine("==============================================================================================================");
                Console.WriteLine("============================= Sitecore Language Registration PowerTool v1.0 ==================================");
                Console.WriteLine("==============================================================================================================");
                Console.WriteLine("");
                Console.WriteLine("* This tool adds custom Cultures to .Net. The purpose of this is so that Sitecore can add and utilize custom languages");
                Console.WriteLine("* It uses a simple CSV File as input for the Language Registration and registers the new Cultures utilizing the .Net System.Globalization.CultureAndRegionInfoBuilder classes.");
                Console.WriteLine("* It also produces a Sitecore Powershell script based on the data configured in the CSV file that adds the custom Language to Sitecore - in sitecore/system/languages.");
                Console.WriteLine("* This application uses 2 files (provided) that must be in the same Folder as the exe:");
                Console.WriteLine("* 1: A file named 'SitecoreAddLanguagesScriptTemplate.ps1'");
                Console.WriteLine("* 2: A file named 'CustomLangRegistration.csv'");
                Console.WriteLine("* This application is Open Source. See https://github.com/FreddieDB/Sitecore-Language-Registration-PowerTool for source code and documentation.");
                Console.WriteLine("");
                Console.WriteLine("!! Configure the custom cultures in the CSV file first according to your Sitecore website multi-language requirements!!");
                Console.WriteLine("");
                Console.WriteLine("Continue? (Y/N)");

                var yn = Console.ReadKey();

                if (yn.KeyChar.ToString().ToLower().Equals("n")) return;
                Console.WriteLine("");
                Console.ForegroundColor = originalConsoleColor;

                Console.WriteLine(AppDomain.CurrentDomain.BaseDirectory);

                string pathCSVFile = AppDomain.CurrentDomain.BaseDirectory + "CustomLangRegistration.csv";
                string pathPSTemplate = AppDomain.CurrentDomain.BaseDirectory + "SitecoreAddLanguagesScriptTemplate.ps1";

                if (!File.Exists(pathCSVFile) || !File.Exists(pathPSTemplate))
                {
                    Console.WriteLine("Missing files: 'CustomLangRegistration.csv' OR 'SitecoreAddLanguagesScriptTemplate.ps1'. Please ensure that both files are present in the same directory as this exe file.'");
                    Console.WriteLine("Press any key to exit...");
                    Console.ReadLine();
                    return;
                }

                string psTemplate = "";
                StringBuilder sbPSScript = new StringBuilder();

                List<CustomCulture> cultures = new List<CustomCulture>();


                Console.WriteLine($"Opening Sitecore Powershell Template from file: '{pathPSTemplate}'...");
                using (StreamReader sr = File.OpenText(pathPSTemplate))
                {
                    psTemplate = sr.ReadToEnd();
                }

                Console.WriteLine($"Reading the Pre-Configured CSV file: '{pathCSVFile}'...");

                using (StreamReader sr = new StreamReader(pathCSVFile))
                {
                    CsvHelper.CsvReader reader = new CsvReader(sr);
                    reader.Configuration.MissingFieldFound = null;
                    var records = reader.GetRecords<CSVSModelNewCountryLangRegistration>();

                    Console.WriteLine($"Adding data to internal lists...");

                    foreach (CSVSModelNewCountryLangRegistration x in records)
                    {
                        if (x.Active.ToUpper().Trim() == "Y")
                            cultures.Add(new CustomCulture(x.BaseFromCultureCode, x.BaseFromCultureCode.Split('-')[1], x.NewLanguageCultureCode, x.NewLanguageCultureCode, x.NewEnglishCultureName, x.NewEnglishCultureName, x.NewCountryName, x.NewCountryName));

                        if (x.AddToSitecorePowerShellScript.ToUpper().Trim() == "Y")
                        {
                            string newPSAddSCLang = psTemplate;
                            newPSAddSCLang = newPSAddSCLang.Replace("XX-ISO-LANG-COUNTRY", x.NewLanguageCultureCode);
                            newPSAddSCLang = newPSAddSCLang.Replace("XX-FALLBACK-ISO-LANG-COUNTRY", x.FallbackLanguageCultureCode);
                            newPSAddSCLang = newPSAddSCLang.Replace("XX-ISOLang", x.NewLanguageCultureCode.Split('-')[0]);

                            sbPSScript.AppendLine(newPSAddSCLang);
                        }
                    }
                }


                Console.WriteLine($"Registering the new Cultures/Languages with Windows...");

                foreach (var currCulture in cultures)
                {
                    CultureAndRegionInfoBuilder crBuilder = null;

                    try
                    {
                        Console.WriteLine($"Registering {currCulture.EnglishName}...");
                        crBuilder = new CultureAndRegionInfoBuilder(currCulture.CultureName, CultureAndRegionModifiers.None);
                        crBuilder.LoadDataFromCultureInfo(new CultureInfo(currCulture.BaseFrom));
                        crBuilder.LoadDataFromRegionInfo(new RegionInfo(currCulture.BaseFromReg));
                        crBuilder.CultureEnglishName = currCulture.EnglishName;
                        crBuilder.CultureNativeName = currCulture.NativeName;
                        crBuilder.IetfLanguageTag = currCulture.CultureLangTag;
                        crBuilder.RegionEnglishName = currCulture.RegEnglishName;
                        crBuilder.RegionNativeName = currCulture.RegNativeName;
                        crBuilder.Register();
                        System.Console.WriteLine(crBuilder.CultureName + " successfully registered.");
                    }
                    catch (Exception e)
                    {
                        System.Console.WriteLine(e.Message);
                    }
                }

                Console.WriteLine("Generating Sitecore PowerShell Script to add Languages to Sitecore...");

                string scPSScriptPath = AppDomain.CurrentDomain.BaseDirectory + "SitecoreAddCustomLanguages_" + DateTime.Now.ToFileTime() + ".ps1";
                

                using (StreamWriter sw = new StreamWriter(scPSScriptPath))
                {
                    sw.Write(sbPSScript.ToString());
                }
                Console.WriteLine("Sitecore PowerShell Script created: " + scPSScriptPath);

                Console.WriteLine($"Done...");

                Console.WriteLine($"Remember to Restart Windows in order for the changes to take effect! Copy the Sitecore Powershell script into Sitecore Powershell Console or ISE and run it to create the new Sitecore Languages");

                System.Console.WriteLine("Press any key to exit");
                System.Console.ReadKey();

                return;
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred trying to serialize CSV data or reading the files: " + ex.ToString());
            }

        }

        class CustomCulture
        {
            public string BaseFrom { get; set; }
            public string BaseFromReg { get; set; }
            public string CultureName { get; set; }
            public string CultureLangTag { get; set; }
            public string EnglishName { get; set; }
            public string NativeName { get; set; }
            public string RegEnglishName { get; set; }
            public string RegNativeName { get; set; }

            public CustomCulture(string baseFrom, string baseFromReg, string cultureName, string cultureLangTag, string englishName, string nativeName, string regEnglishName, string regNativeName)
            {
                this.BaseFrom = baseFrom;
                this.BaseFromReg = baseFromReg;
                this.CultureName = cultureName;
                this.CultureLangTag = cultureLangTag;
                this.EnglishName = englishName;
                this.NativeName = nativeName;
                this.RegEnglishName = regEnglishName;
                this.RegNativeName = regNativeName;
            }

        }

    }
}
