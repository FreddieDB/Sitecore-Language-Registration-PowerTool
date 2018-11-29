using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FdB.SitecoreLangRegPowerTool
{
    /// <summary>
    /// This class represents the data found in the SourceConfig CSV file that would be used in adding the new Language registrations to Windows
    /// </summary>
    public class CSVSModelNewCountryLangRegistration
    {
        /// <summary>
        /// Name of the new country - e.g. Ukraine
        /// </summary>
        public string NewCountryName { get; set; }
        /// <summary>
        /// NewCountryCode - ISO Country code e.g. UA
        /// </summary>
        public string NewCountryCode { get; set; }
        /// <summary>
        /// NewEnglishCultureName - Langauge (CountryName) e.g. Russian (Ukraine)
        /// </summary>
        public string NewEnglishCultureName { get; set; }
        /// <summary>
        /// BaseFromCultureCode - ISO Langauge-CountryCode e.g. ru-RU. Used to base the new culture off, in this example ru-RU = Russian-Russia 
        /// </summary>
        public string BaseFromCultureCode { get; set; }
        /// <summary>
        /// NewLanguageCultureCode - ISO Language-Country code for the new culture e.g. ru-UA
        /// </summary>
        public string NewLanguageCultureCode { get; set; }
        /// <summary>
        /// Flag Y or N to indicate whether to add this language registration into a Sitecore Powershell script to add the new Language into sitecore\system\languages. This is done manually by executing the generated Powershell script from Sitecore Desktop
        /// </summary>
        public string AddToSitecorePowerShellScript { get; set; }
        /// <summary>
        /// If this Language uses Sitecore Language Fallback, this option specifies the ISO language-county code for Langauge Fallback (e.g. ru-RU). Applicable for the Sitecore Powershell script
        /// </summary>
        public string FallbackLanguageCultureCode { get; set; }
        /// <summary>
        /// Active Flag Y or N. If set to Y language registration will be done. If flag set to N, the langauge registration will be ignored, but based on the AddToSitecorePowerShellScript flag the info will still be added to the Sitecore Powershell script to add the language to sitecore\system\languages
        /// </summary>
        public string Active { get; set; }

    }
}
