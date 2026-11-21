using System;
namespace Skyline_Sports_Project.Engine;
public static class ClubBranding
{
 public const int GenericLogoCount=24, RplLogoOffset=24, RplLogoCount=16, NoLogoId=40, ManualGenericLogoOffset=41, FnlLogoOffset=65, FnlLogoCount=18, OtherLogoOffset=83, OtherLogoCount=7, LogoCount=90;
 private static readonly string[] Sponsors={"СеверЭнерго","ВолгаТелеком","Полюс Банк","Гранит Страхование","Вектор Моторс","Нева Финанс","АрктикТех","Старт Спорт","Маяк Мобайл","Орбита Маркет","Титан Индастри","Прайм Инвест"};
 private static readonly string[][] FnlAliases={new[]{"арсенал","тула"},new[]{"челябинск"},new[]{"камаз","набережные челны"},new[]{"ленинградец","ленинградская область"},new[]{"нефтехимик","нижнекамск"},new[]{"пари нн","нижний новгород","нижегород"},new[]{"ротор","волгоград"},new[]{"шинник","ярославль"},new[]{"ска","хабаровск"},new[]{"сочи"},new[]{"спартак кострома","кострома"},new[]{"текстильщик","иваново"},new[]{"торпедо","москва"},new[]{"уфа"},new[]{"урал","екатеринбург"},new[]{"велес","москва"},new[]{"волга","ульяновск"},new[]{"енисей","красноярск"}};
 private static readonly string[][] RplAliases={
 new[]{"зенит","zenit"},new[]{"ахмат","akhmat","грозный"},new[]{"акрон","akron","тольятти"},new[]{"балтика","baltika","калининград"},
 new[]{"цска","cska"},new[]{"динамо махачкала","махачкала","dynamo-makhachkala"},new[]{"динамо москва","dynamo-moscow","московское динамо"},
 new[]{"факел","fakel","воронеж"},new[]{"краснодар","krasnodar"},new[]{"крылья советов","krylya-sovetov","самара"},
 new[]{"локомотив","lokomotiv"},new[]{"оренбург","orenburg"},new[]{"родина","rodina"},new[]{"ростов","rostov"},new[]{"рубин","rubin","казань"},new[]{"спартак","spartak"}};
 public static int SponsorCount=>Sponsors.Length;
 public static string SponsorName(int id)=>id>=0&&id<Sponsors.Length?Sponsors[id]:"Без спонсора";
 public static int SponsorTier(int id){if(id<0||id>=Sponsors.Length)return 0;if(id<3)return 1;if(id<6)return 2;if(id<9)return 3;return 4;}
 public static int NormalizeLogo(int id)=>id>=0&&id<LogoCount?id:-1;
 public static int NormalizeSponsor(int id)=>id>=0&&id<Sponsors.Length?id:-1;
 public static int FindRplLogoId(string organizationName,string mascot){string v=((organizationName??"")+" "+(mascot??"")).ToLowerInvariant();for(int i=0;i<RplAliases.Length;i++)foreach(string a in RplAliases[i])if(v.IndexOf(a,StringComparison.OrdinalIgnoreCase)>=0)return i;return -1;}
 public static int FindFnlLogoId(string organizationName,string mascot){string v=((organizationName??"")+" "+(mascot??"")).ToLowerInvariant();for(int i=0;i<FnlAliases.Length;i++)foreach(string a in FnlAliases[i])if(v.IndexOf(a,StringComparison.OrdinalIgnoreCase)>=0)return i;return -1;}
}
