using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoogleMapsUnofficial.Helpers
{
    class CountryCodesHelper
    {
        Dictionary<string, string> countryCodesMapping = new Dictionary<string, string>() {
   { "AFG", "AF" },    // Afghanistan
   { "ALB", "AL" },    // Albania
   { "ARE", "AE" },    // U.A.E.
   { "ARG", "AR" },    // Argentina
   { "ARM", "AM" },    // Armenia
   { "AUS", "AU" },    // Australia
   { "AUT", "AT" },    // Austria
   { "AZE", "AZ" },    // Azerbaijan
   { "BEL", "BE" },    // Belgium
   { "BGD", "BD" },    // Bangladesh
   { "BGR", "BG" },    // Bulgaria
   { "BHR", "BH" },    // Bahrain
   { "BIH", "BA" },    // Bosnia and Herzegovina
   { "BLR", "BY" },    // Belarus
   { "BLZ", "BZ" },    // Belize
   { "BOL", "BO" },    // Bolivia
   { "BRA", "BR" },    // Brazil
   { "BRN", "BN" },    // Brunei Darussalam
   { "CAN", "CA" },    // Canada
   { "CHE", "CH" },    // Switzerland
   { "CHL", "CL" },    // Chile
   { "CHN", "CN" },    // People's Republic of China
   { "COL", "CO" },    // Colombia
   { "CRI", "CR" },    // Costa Rica
   { "CZE", "CZ" },    // Czech Republic
   { "DEU", "DE" },    // Germany
   { "DNK", "DK" },    // Denmark
   { "DOM", "DO" },    // Dominican Republic
   { "DZA", "DZ" },    // Algeria
   { "ECU", "EC" },    // Ecuador
   { "EGY", "EG" },    // Egypt
   { "ESP", "ES" },    // Spain
   { "EST", "EE" },    // Estonia
   { "ETH", "ET" },    // Ethiopia
   { "FIN", "FI" },    // Finland
   { "FRA", "FR" },    // France
   { "FRO", "FO" },    // Faroe Islands
   { "GBR", "GB" },    // United Kingdom
   { "GEO", "GE" },    // Georgia
   { "GRC", "GR" },    // Greece
   { "GRL", "GL" },    // Greenland
   { "GTM", "GT" },    // Guatemala
   { "HKG", "HK" },    // Hong Kong S.A.R.
   { "HND", "HN" },    // Honduras
   { "HRV", "HR" },    // Croatia
   { "HUN", "HU" },    // Hungary
   { "IDN", "ID" },    // Indonesia
   { "IND", "IN" },    // India
   { "IRL", "IE" },    // Ireland
   { "IRN", "IR" },    // Iran
   { "IRQ", "IQ" },    // Iraq
   { "ISL", "IS" },    // Iceland
   { "ISR", "IL" },    // Israel
   { "ITA", "IT" },    // Italy
   { "JAM", "JM" },    // Jamaica
   { "JOR", "JO" },    // Jordan
   { "JPN", "JP" },    // Japan
   { "KAZ", "KZ" },    // Kazakhstan
   { "KEN", "KE" },    // Kenya
   { "KGZ", "KG" },    // Kyrgyzstan
   { "KHM", "KH" },    // Cambodia
   { "KOR", "KR" },    // Korea
   { "KWT", "KW" },    // Kuwait
   { "LAO", "LA" },    // Lao P.D.R.
   { "LBN", "LB" },    // Lebanon
   { "LBY", "LY" },    // Libya
   { "LIE", "LI" },    // Liechtenstein
   { "LKA", "LK" },    // Sri Lanka
   { "LTU", "LT" },    // Lithuania
   { "LUX", "LU" },    // Luxembourg
   { "LVA", "LV" },    // Latvia
   { "MAC", "MO" },    // Macao S.A.R.
   { "MAR", "MA" },    // Morocco
   { "MCO", "MC" },    // Principality of Monaco
   { "MDV", "MV" },    // Maldives
   { "MEX", "MX" },    // Mexico
   { "MKD", "MK" },    // Macedonia (FYROM)
   { "MLT", "MT" },    // Malta
   { "MNE", "ME" },    // Montenegro
   { "MNG", "MN" },    // Mongolia
   { "MYS", "MY" },    // Malaysia
   { "NGA", "NG" },    // Nigeria
   { "NIC", "NI" },    // Nicaragua
   { "NLD", "NL" },    // Netherlands
   { "NOR", "NO" },    // Norway
   { "NPL", "NP" },    // Nepal
   { "NZL", "NZ" },    // New Zealand
   { "OMN", "OM" },    // Oman
   { "PAK", "PK" },    // Islamic Republic of Pakistan
   { "PAN", "PA" },    // Panama
   { "PER", "PE" },    // Peru
   { "PHL", "PH" },    // Republic of the Philippines
   { "POL", "PL" },    // Poland
   { "PRI", "PR" },    // Puerto Rico
   { "PRT", "PT" },    // Portugal
   { "PRY", "PY" },    // Paraguay
   { "QAT", "QA" },    // Qatar
   { "ROU", "RO" },    // Romania
   { "RUS", "RU" },    // Russia
   { "RWA", "RW" },    // Rwanda
   { "SAU", "SA" },    // Saudi Arabia
   { "SCG", "CS" },    // Serbia and Montenegro (Former)
   { "SEN", "SN" },    // Senegal
   { "SGP", "SG" },    // Singapore
   { "SLV", "SV" },    // El Salvador
   { "SRB", "RS" },    // Serbia
   { "SVK", "SK" },    // Slovakia
   { "SVN", "SI" },    // Slovenia
   { "SWE", "SE" },    // Sweden
   { "SYR", "SY" },    // Syria
   { "TAJ", "TJ" },    // Tajikistan
   { "THA", "TH" },    // Thailand
   { "TKM", "TM" },    // Turkmenistan
   { "TTO", "TT" },    // Trinidad and Tobago
   { "TUN", "TN" },    // Tunisia
   { "TUR", "TR" },    // Turkey
   { "TWN", "TW" },    // Taiwan
   { "UKR", "UA" },    // Ukraine
   { "URY", "UY" },    // Uruguay
   { "USA", "US" },    // United States
   { "UZB", "UZ" },    // Uzbekistan
   { "VEN", "VE" },    // Bolivarian Republic of Venezuela
   { "VNM", "VN" },    // Vietnam
   { "YEM", "YE" },    // Yemen
   { "ZAF", "ZA" },    // South Africa
   { "ZWE", "ZW" },    // Zimbabwe
};
    }
}
