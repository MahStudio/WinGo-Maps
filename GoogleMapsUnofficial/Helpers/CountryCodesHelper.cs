using System.Collections.Generic;
using System.Globalization;

namespace GoogleMapsUnofficial.Helpers
{
    class CountryCodesHelper
    {
        Dictionary<string, string> CountriesAndLanguageCodes = new Dictionary<string, string>()
        {
            { "Afghanistan - Dari",    "prs-AF" },
            {"Afghanistan - Pashto",    "ps-AF" },
            {"Albania - Albanian",    "sq-AL" },
            { "Algeria - Arabic",    "ar-DZ"},
            {"Algeria - Tamazight (Latin)",    "tzm-DZ" },
{"Argentina - Spanish",    "es-AR" },
{"Armenia - Armenian",    "hy-AM" },
{"Australia - English",    "en-AU"                            },
{"Austria - German",    "de-AT"                               },
{"Azerbaijan - Azeri (Cyrillic)",    "az-AZ"                  },
{"Azerbaijan - Azeri (Latin)",    "az-AZ"                     },
{"Bahrain - Arabic",    "ar-BH"                               },
{"Bangladesh - Bengali",    "bn-BD"                           },
{"Belarus - Belarusian",    "be-BY"                           },
{"Belgium - Dutch",    "nl-BE"                                },
{"Belgium - French",    "fr-BE"                               },
{"Belize - English",    "en-BZ"                               },
{"Bolivarian Republic of Venezuela - Spanish",    "es-VE"     },
{"Bolivia - Quechua",    "quz-BO"                             },
{"Bolivia - Spanish",    "es-BO"                              },
{"Bosnia and Herzegovina - Bosnian (Cyrillic)",    "bs-BA"    },
{"Bosnia and Herzegovina - Bosnian (Latin)",    "bs-BA"       },
{"Bosnia and Herzegovina - Croatian",    "hr-BA"              },
{"Bosnia and Herzegovina - Serbian (Cyrillic)",    "sr-BA"    },
{"Bosnia and Herzegovina - Serbian (Latin)",    "sr-BA"       },
{"Brazil - Portuguese",    "pt-BR"                            },
{"Brunei Darussalam - Malay",    "ms-BN"                      },
{"Bulgaria - Bulgarian",    "bg-BG"                           },
{"Cambodia - Khmer",    "km-KH"                               },
{"Canada - English",    "en-CA"                               },
{"Canada - French",    "fr-CA"                                },
{"Canada - Inuktitut (Latin)",    "iu-CA"                     },
{"Canada - Inuktitut (Syllabics)",    "iu-CA"                 },
{"Canada - Mohawk",    "moh-CA"                               },
{"Caribbean - English",    "en-029"                           },
{"Chile - Mapudungun",    "arn-CL"                            },
{"Chile - Spanish",    "es-CL"                                },
{"Colombia - Spanish",    "es-CO"                             },
{"Costa Rica - Spanish",    "es-CR"                           },
{"Croatia - Croatian",    "hr-HR"                             },
{"Czech Republic - Czech",    "cs-CZ"                         },
{"Denmark - Danish",    "da-DK"                               },
{"Dominican Republic - Spanish",    "es-DO"                   },
{"Ecuador - Quechua",    "quz-EC"                             },
{"Ecuador - Spanish",    "es-EC"                              },
{"Egypt - Arabic",    "ar-EG"                                 },
{"El Salvador - Spanish",    "es-SV"                          },
{"Estonia - Estonian",    "et-EE"                             },
{"Ethiopia - Amharic",    "am-ET"                             },
{"Faroe Islands - Faroese",    "fo-FO"                        },
{"Finland - Finnish",    "fi-FI"                              },
{"Finland - Sami (Inari)",    "smn-FI"                        },
{"Finland - Sami (Northern)",    "se-FI"                      },
{"Finland - Sami (Skolt)",    "sms-FI"                        },
{"Finland - Swedish",    "sv-FI"                              },
{"France - Alsatian",    "gsw-FR"                             },
{"France - Breton",    "br-FR"                                },
{"France - Corsican",    "co-FR"                              },
{"France - French",    "fr-FR"                                },
{"France - Occitan",    "oc-FR"                               },
{"Georgia - Georgian",    "ka-GE"                             },
{"Germany - German",    "de-DE"                               },
{"Germany - Lower Sorbian",    "dsb-DE"                       },
{"Germany - Upper Sorbian",    "hsb-DE"                       },
{"Greece - Greek",    "el-GR"                                 },
{"Greenland - Greenlandic",    "kl-GL"                        },
{"Guatemala - K'iche",    "qut-GT"                            },
{"Guatemala - Spanish",    "es-GT"                            },
{"Honduras - Spanish",    "es-HN"                             },
{"Hong Kong S.A.R. - Chinese (Traditional) Legacy",    "zh-HK"},
{"Hungary - Hungarian",    "hu-HU" },
{"Iceland - Icelandic",    "is-IS"},
{"India - Assamese",    "as-IN"   },
{"India - Bengali",    "bn-IN"    },
{"India - English",    "en-IN"    },
{"India - Gujarati",    "gu-IN"   },
{"India - Hindi",    "hi-IN"      },
{"India - Kannada",    "kn-IN"    },
{"India - Konkani",    "kok-IN"   },
{"India - Malayalam",    "ml-IN"  },
{"India - Marathi",    "mr-IN"    },
{"India - Oriya",    "or-IN"      },
{"India - Punjabi",    "pa-IN"    },
{"India - Sanskrit",    "sa-IN"   },
{"India - Tamil",    "ta-IN"      },
{"India - Telugu",    "te-IN"     },
{"Indonesia - Indonesian",    "id-ID" },
{"Iran - Persian",    "fa-IR" },
{"Iraq - Arabic",    "ar-IQ"},
{"Ireland - English",    "en-IE"},
{"Ireland - Irish",    "ga-IE"},
{"Islamic Republic of Pakistan - Urdu",    "ur-PK"},
{"Israel - Hebrew",    "he-IL"                    },
{"Italy - Italian",    "it-IT"                    },
{"Jamaica - English",    "en-JM"                  },
{"Japan - Japanese",    "ja-JP"                   },
{"Jordan - Arabic",    "ar-JO"                    },
{"Kazakhstan - Kazakh",    "kk-KZ"                },
{"Kenya - Kiswahili",    "sw-KE"                  },
{"Korea - Korean",    "ko-KR"                     },
{"Kuwait - Arabic",    "ar-KW"                    },
{"Kyrgyzstan - Kyrgyz",    "ky-KG"                },
{"Lao P.D.R. - Lao",    "lo-LA"                   },
{"Latvia - Latvian",    "lv-LV"                   },
{"Lebanon - Arabic",    "ar-LB"                   },
{"Libya - Arabic",    "ar-LY"                     },
{"Liechtenstein - German",    "de-LI"             },
{"Lithuania - Lithuanian",    "lt-LT"             },
{"Luxembourg - French",    "fr-LU"                },
{"Luxembourg - German",    "de-LU"                },
{"Luxembourg - Luxembourgish",    "lb-LU"         },
{"Macao S.A.R. - Chinese (Traditional) Legacy",    "zh-MO" },
{"Macedonia (FYROM) - Macedonian (FYROM)",    "mk-MK" },
{"Malaysia - English",    "en-MY" },
{"Malaysia - Malay",    "ms-MY" },
{"Maldives - Divehi",    "dv-MV" },
{"Malta - Maltese",    "mt-MT" },
{"Mexico - Spanish",    "es-MX" },
{"Mongolia - Mongolian (Cyrillic)",    "mn-MN"},
{"Montenegro - Serbian (Cyrillic)",    "sr-ME"},
{"Montenegro - Serbian (Latin)",    "sr-ME"   },
{"Morocco - Arabic",    "ar-MA"               },
{"Nepal - Nepali",    "ne-NP"                 },
{"Netherlands - Dutch",    "nl-NL"            },
{"Netherlands - Frisian",    "fy-NL"          },
{"New Zealand - English",    "en-NZ"          },
{"New Zealand - Maori",    "mi-NZ"            },
{"Nicaragua - Spanish",    "es-NI"            },
{"Nigeria - Hausa (Latin)",    "ha-NG"        },
{"Nigeria - Igbo",    "ig-NG"                 },
{"Nigeria - Yoruba",    "yo-NG"               },
{"Norway - Norwegian (Bokmal)",    "nb-NO"    },
{"Norway - Norwegian (Nynorsk)",    "nn-NO"   },
{"Norway - Sami (Lule)",    "smj-NO"          },
{"Norway - Sami (Northern)",    "se-NO"       },
{"Norway - Sami (Southern)",    "sma-NO"      },
{"Oman - Arabic",    "ar-OM"                  },
{"Panama - Spanish",    "es-PA"               },
{"Paraguay - Spanish",    "es-PY"             },
{"People's Republic of China - Chinese (Simplified) Legacy",    "zh-CN" },
{"People's Republic of China - Mongolian (Traditional Mongolian)",    "mn-CN" },
{"People's Republic of China - Tibetan",    "bo-CN"},
{"People's Republic of China - Uyghur",    "ug-CN" },
{"People's Republic of China - Yi",    "ii-CN"     },
{"Peru - Quechua",    "quz-PE"                     },
{"Peru - Spanish",    "es-PE"                      },
{"Philippines - Filipino",    "fil-PH"             },
{"Poland - Polish",    "pl-PL"                     },
{"Portugal - Portuguese",    "pt-PT"               },
{"Principality of Monaco - French",    "fr-MC"     },
{"Puerto Rico - Spanish",    "es-PR"               },
{"Qatar - Arabic",    "ar-QA"                      },
{"Republic of the Philippines - English",    "en-PH" },
{"Romania - Romanian",    "ro-RO" },
{"Russia - Bashkir",    "ba-RU" },
{"Russia - Russian",    "ru-RU"},
{"Russia - Tatar",    "tt-RU"},
{"Russia - Yakut",    "sah-RU"},
{"Rwanda - Kinyarwanda",    "rw-RW"},
{"Saudi Arabia - Arabic",    "ar-SA"},
{"Senegal - Wolof",    "wo-SN"},
{"Serbia - Serbian (Cyrillic)",    "sr-RS"},
{"Serbia - Serbian (Latin)",    "sr-RS"},
{"Serbia and Montenegro (Former) - Serbian (Cyrillic)",    "sr-CS"},
{"Serbia and Montenegro (Former) - Serbian (Latin)",    "sr-CS"},
{"Singapore - Chinese (Simplified) Legacy",    "zh-SG"},
{"Singapore - English",    "en-SG"},
{"Slovakia - Slovak",    "sk-SK"},
{"Slovenia - Slovenian",    "sl-SI"},
{"South Africa - Afrikaans",    "af-ZA"},
{"South Africa - English",    "en-ZA"},
{"South Africa - isiXhosa",    "xh-ZA"},
{"South Africa - isiZulu",    "zu-ZA"},
{"South Africa - Sesotho sa Leboa",    "nso-ZA"},
{"South Africa - Setswana",    "tn-ZA"},
{"Spain - Basque",    "eu-ES"},
{"Spain - Catalan",    "ca-ES"},
{"Spain - Galician",    "gl-ES"},
{"Spain - Spanish",    "es-ES"},
{"Sri Lanka - Sinhala",    "si-LK"},
{"Sweden - Sami (Lule)",    "smj-SE"},
{"Sweden - Sami (Northern)",    "se-SE"},
{"Sweden - Sami (Southern)",    "sma-SE"},
{"Sweden - Swedish",    "sv-SE"},
{"Switzerland - French",    "fr-CH"},
{"Switzerland - German",    "de-CH"},
{"Switzerland - Italian",    "it-CH"},
{"Switzerland - Romansh",    "rm-CH"},
{"Syria - Arabic",    "ar-SY"},
{"Syria - Syriac",    "syr-SY"},
{"Taiwan - Chinese (Traditional) Legacy",    "zh-TW"},
{"Tajikistan - Tajik (Cyrillic)",    "tg-TJ"},
{"Thailand - Thai",    "th-TH"},
{"Trinidad and Tobago - English",    "en-TT"},
{"Tunisia - Arabic",    "ar-TN"},
{"Turkey - Turkish",    "tr-TR"},
{"Turkmenistan - Turkmen",    "tk-TM"},
{"U.A.E. - Arabic",    "ar-AE"},
{"Ukraine - Ukrainian",    "uk-UA"},
{"United Kingdom - English",    "en-GB"},
{"United Kingdom - Scottish Gaelic",    "gd-GB"},
{"United Kingdom - Welsh",    "cy-GB"},
{"United States - English",    "en-US"},
{"United States - Spanish",    "es-US"},
{"Uruguay - Spanish",    "es-UY"},
{"Uzbekistan - Uzbek (Cyrillic)",    "uz-UZ"},
{"Uzbekistan - Uzbek (Latin)",    "uz-UZ"},
{"Vietnam - Vietnamese",    "vi-VN"},
{"Yemen - Arabic",    "ar-YE"},
{"Zimbabwe - English",    "en-ZW"}
        };

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

        Dictionary<string, string> countriesAndCodes = new Dictionary<string, string>() {
   { "Afghanistan", "AF" },    // Afghanistan
   { "Albania", "AL" },    // Albania
   { "U.A.E", "AE" },    // U.A.E.
   { "Argentina", "AR" },    // Argentina
   { "Armenia", "AM" },    // Armenia
   { "Australia", "AU" },    // Australia
   { "Austria", "AT" },    // Austria
   { "Azerbaijan", "AZ" },    // Azerbaijan
   { "Belgium", "BE" },    // Belgium
   { "Bangladesh", "BD" },    // Bangladesh
   { "Bulgaria", "BG" },    // Bulgaria
   { "Bahrain", "BH" },    // Bahrain
   { "Bosnia and Herzegovina", "BA" },    // Bosnia and Herzegovina
   { "Belarus", "BY" },    // Belarus
   { "Belize", "BZ" },    // Belize
   { "Bolivia", "BO" },    // Bolivia
   { "Brazil", "BR" },    // Brazil
   { "Brunei Darussalam", "BN" },    // Brunei Darussalam
   { "Canada", "CA" },    // Canada
   { "Switzerland", "CH" },    // Switzerland
   { "Chile", "CL" },    // Chile
   { "China, People's Republic", "CN" },    // People's Republic of China
   { "Colombia", "CO" },    // Colombia
   { "Costa Rica", "CR" },    // Costa Rica
   { "Czech Republic", "CZ" },    // Czech Republic
   { "Germany", "DE" },    // Germany
   { "Denmark", "DK" },    // Denmark
   { "Dominican Republic", "DO" },    // Dominican Republic
   { "Algeria", "DZ" },    // Algeria
   { "Ecuador", "EC" },    // Ecuador
   { "Egypt", "EG" },    // Egypt
   { "Spain", "ES" },    // Spain
   { "Estonia", "EE" },    // Estonia
   { "Ethiopia", "ET" },    // Ethiopia
   { "Finland", "FI" },    // Finland
   { "France", "FR" },    // France
   { "Faroe Islands", "FO" },    // Faroe Islands
   { "United Kingdom", "GB" },    // United Kingdom
   { "Georgia", "GE" },    // Georgia
   { "Greece", "GR" },    // Greece
   { "Greenland", "GL" },    // Greenland
   { "Guatemala", "GT" },    // Guatemala
   { "Hong Kong S.A.R", "HK" },    // Hong Kong S.A.R.
   { "Honduras", "HN" },    // Honduras
   { "Croatia", "HR" },    // Croatia
   { "Hungary", "HU" },    // Hungary
   { "Indonesia", "ID" },    // Indonesia
   { "India", "IN" },    // India
   { "Ireland", "IE" },    // Ireland
   { "Iran, Islamic Republic of", "IR" },    // Iran
   { "Iraq", "IQ" },    // Iraq
   { "Iceland", "IS" },    // Iceland
   { "Israel", "IL" },    // Israel
   { "Italy", "IT" },    // Italy
   { "Jamaica", "JM" },    // Jamaica
   { "Jordan", "JO" },    // Jordan
   { "Japan", "JP" },    // Japan
   { "Kazakhstan", "KZ" },    // Kazakhstan
   { "Kenya", "KE" },    // Kenya
   { "Kyrgyzstan", "KG" },    // Kyrgyzstan
   { "Cambodia", "KH" },    // Cambodia
   { "Korea", "KR" },    // Korea
   { "Kuwait", "KW" },    // Kuwait
   { "Lao P.D.R", "LA" },    // Lao P.D.R.
   { "Lebanon", "LB" },    // Lebanon
   { "Libya", "LY" },    // Libya
   { "Liechtenstein", "LI" },    // Liechtenstein
   { "Sri Lanka", "LK" },    // Sri Lanka
   { "Lithuania", "LT" },    // Lithuania
   { "Luxembourg", "LU" },    // Luxembourg
   { "Latvia", "LV" },    // Latvia
   { "Macao S.A.R", "MO" },    // Macao S.A.R.
   { "Morocco", "MA" },    // Morocco
   { "Principality of Monaco", "MC" },    // Principality of Monaco
   { "Maldives", "MV" },    // Maldives
   { "Mexico", "MX" },    // Mexico
   { "Macedonia (FYROM)", "MK" },    // Macedonia (FYROM)
   { "Malta", "MT" },    // Malta
   { "Montenegro", "ME" },    // Montenegro
   { "Mongolia", "MN" },    // Mongolia
   { "Malaysia", "MY" },    // Malaysia
   { "Nigeria", "NG" },    // Nigeria
   { "Nicaragua", "NI" },    // Nicaragua
   { "Netherlands", "NL" },    // Netherlands
   { "Norway", "NO" },    // Norway
   { "Nepal", "NP" },    // Nepal
   { "New Zealand", "NZ" },    // New Zealand
   { "Oman", "OM" },    // Oman
   { "Pakistan, Islamic Republic of", "PK" },    // Islamic Republic of Pakistan
   { "Panama", "PA" },    // Panama
   { "Peru", "PE" },    // Peru
   { "Philippines, Republic of", "PH" },    // Republic of the Philippines
   { "Poland", "PL" },    // Poland
   { "Puerto Rico", "PR" },    // Puerto Rico
   { "Portugal", "PT" },    // Portugal
   { "Paraguay", "PY" },    // Paraguay
   { "Qatar", "QA" },    // Qatar
   { "Romania", "RO" },    // Romania
   { "Russia", "RU" },    // Russia
   { "Rwanda", "RW" },    // Rwanda
   { "Saudi Arabia", "SA" },    // Saudi Arabia
   { "Serbia and Montenegro (Former)", "CS" },    // Serbia and Montenegro (Former)
   { "Senegal", "SN" },    // Senegal
   { "Singapore", "SG" },    // Singapore
   { "El Salvador", "SV" },    // El Salvador
   { "Serbia", "RS" },    // Serbia
   { "Slovakia", "SK" },    // Slovakia
   { "Slovenia", "SI" },    // Slovenia
   { "Sweden", "SE" },    // Sweden
   { "Syria", "SY" },    // Syria
   { "Tajikistan", "TJ" },    // Tajikistan
   { "Thailand", "TH" },    // Thailand
   { "Turkmenistan", "TM" },    // Turkmenistan
   { "Trinidad and Tobago", "TT" },    // Trinidad and Tobago
   { "Tunisia", "TN" },    // Tunisia
   { "Turkey", "TR" },    // Turkey
   { "Taiwan", "TW" },    // Taiwan
   { "Ukraine", "UA" },    // Ukraine
   { "Uruguay", "UY" },    // Uruguay
   { "United States", "US" },    // United States
   { "Uzbekistan", "UZ" },    // Uzbekistan
   { "Bolivarian Republic of Venezuela", "VE" },    // Bolivarian Republic of Venezuela
   { "Vietnam", "VN" },    // Vietnam
   { "Yemen", "YE" },    // Yemen
   { "South Africa", "ZA" },    // South Africa
   { "Zimbabwe", "ZW" },    // Zimbabwe
};
    }
}
