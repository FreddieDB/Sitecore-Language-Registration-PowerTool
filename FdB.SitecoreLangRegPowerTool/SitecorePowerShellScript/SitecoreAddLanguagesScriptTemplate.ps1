$item = New-Item "master:/sitecore/system/Languages/XX-ISO-LANG-COUNTRY" -type "/sitecore/templates/System/Language"

$item.Editing.BeginEdit()
	$item.Fields["Charset"] = "iso-8859-1"
	$item.Fields["Code page"] = "65001"
	$item.Fields["Encoding"] = "utf-8"
	$item.Fields["Fallback Language"] = "XX-FALLBACK-ISO-LANG-COUNTRY" #Leave as empty string if not applicable. Example: "en-CA"
	$item.Fields["Iso"] = "XX-ISOLang" #Language ISO code. Example: "es"
	$item.Fields["Regional Iso Code"] = "XX-ISO-LANG-COUNTRY" #ISO Language-Country code for new Language. Example: "es-MX"
$item.Editing.EndEdit()
