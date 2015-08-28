using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Configuration;

public class Unitle
{
    public enum ImgPathType
    {
        local,
        server
    }

    public enum ImgGigade100ComType
    {
        default50Path,
        prodPath,
        prod50Path,
        prod150Path,
        prod280Path,
        specPath,
        spec100Path,
        spec280Path,
        descPath,
        desc400Path,
        prod_tagPath,
        prod_noticePath,
        prod_notice400Path,
        promoPath,  //促銷活動圖片存放路徑
        promoPairPath,  //促銷紅配綠圖片存放路徑
        healthPath,//健康2.0
        elementPath,//中信廣告圖
        ftpuser,
        ftppwd,
        descMobilePath,
        desc400MobilePath,
        prod_mobile640, //add by wwei0216w 添加640圖片路徑
        vendorPath,
        paperPath,
        NewPromoPath,//通關密語&&問卷送禮
        archives,//檔案上傳--chaojie1124j添加於2015/03/19
        coursePath,
        InspectionReportPath,
        MemberEventPath
    }

    public static string GetImgGigade100ComPath(ImgGigade100ComType pathType)
    {
        string result = string.Empty;
        switch (pathType)
        {
            case ImgGigade100ComType.default50Path:
                result = ConfigurationManager.AppSettings["default50Path"];
                break;
            case ImgGigade100ComType.prodPath:
                result = ConfigurationManager.AppSettings["prodPath"];
                break;
            case ImgGigade100ComType.prod50Path:
                result = ConfigurationManager.AppSettings["prod50Path"];
                break;
            case ImgGigade100ComType.prod150Path:
                result = ConfigurationManager.AppSettings["prod150Path"];
                break;
            case ImgGigade100ComType.prod280Path:
                result = ConfigurationManager.AppSettings["prod280Path"];
                break;
            case ImgGigade100ComType.specPath:
                result = ConfigurationManager.AppSettings["specPath"];
                break;
            case ImgGigade100ComType.spec100Path:
                result = ConfigurationManager.AppSettings["spec100Path"];
                break;
            case ImgGigade100ComType.spec280Path:
                result = ConfigurationManager.AppSettings["spec280Path"];
                break;
            case ImgGigade100ComType.descPath:
                result = ConfigurationManager.AppSettings["descPath"];
                break;
            case ImgGigade100ComType.desc400Path:
                result = ConfigurationManager.AppSettings["desc400Path"];
                break;
            case ImgGigade100ComType.prod_tagPath:
                result = ConfigurationManager.AppSettings["prod_tagPath"];
                break;
            case ImgGigade100ComType.prod_noticePath:
                result = ConfigurationManager.AppSettings["prod_noticePath"];
                break;
            case ImgGigade100ComType.prod_notice400Path:
                result = ConfigurationManager.AppSettings["prod_notice400Path"];
                break;
            case ImgGigade100ComType.promoPath:
                result = ConfigurationManager.AppSettings["promoPath"];
                break;
            case ImgGigade100ComType.promoPairPath:
                result = ConfigurationManager.AppSettings["promoPairPath"];
                break;
            case ImgGigade100ComType.healthPath:
                result = ConfigurationManager.AppSettings["healthPath"];
                break;
            case ImgGigade100ComType.elementPath:
                result = ConfigurationManager.AppSettings["elementPath"];
                break;
            case ImgGigade100ComType.ftpuser:
                result = ConfigurationManager.AppSettings["ftpuser"];
                break;
            case ImgGigade100ComType.ftppwd:
                result = ConfigurationManager.AppSettings["ftppwd"];
                break;
            case ImgGigade100ComType.descMobilePath:
                result = ConfigurationManager.AppSettings["descMobilePath"];
                break;
            case ImgGigade100ComType.desc400MobilePath:
                result = ConfigurationManager.AppSettings["desc400MobilePath"];
                break;
            case ImgGigade100ComType.prod_mobile640: //add by wwei0216w 2015/4/1
                result = ConfigurationManager.AppSettings["mobile640Path"];
                break;
            case ImgGigade100ComType.vendorPath:
                result = ConfigurationManager.AppSettings["vendorPath"];
                break;
            case ImgGigade100ComType.paperPath:
                result = ConfigurationManager.AppSettings["paperPath"];
                break;
            case ImgGigade100ComType.NewPromoPath:
                result = ConfigurationManager.AppSettings["NewPromoPath"];
                break;
            case ImgGigade100ComType.archives:
                result = ConfigurationManager.AppSettings["archivesPath"];
                break;
            case ImgGigade100ComType.coursePath:
                result = ConfigurationManager.AppSettings["coursePath"];
                break;
            case ImgGigade100ComType.InspectionReportPath:
                result = ConfigurationManager.AppSettings["InspectionReportPath"];
                break;
            case ImgGigade100ComType.MemberEventPath:
                result = ConfigurationManager.AppSettings["MemberEventPath"];
                break;
                
                
        }
        return result;
    }

    public static string GetImgGigade100ComSitePath(ImgPathType pathType)
    {
        string result = string.Empty;
        switch (pathType)
        {
            case ImgPathType.local:
                result = ConfigurationManager.AppSettings["imgLocalPath"];
                break;
            case ImgPathType.server:
                result = ConfigurationManager.AppSettings["imgServerPath"];
                break;
            default:
                break;
        }
        return result;
    }

}
