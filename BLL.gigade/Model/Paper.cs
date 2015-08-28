using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model
{
    public class Paper:PageBase
    {
        public int paperID { get; set; }
        public string paperName { get; set; }
        public string paperMemo{ get; set; }
        public string paperBanner { get; set; }
        public string bannerUrl { get; set; }
        //public int isPromotion { get; set; }
        //public string promotionUrl { get; set; }
        //public int isGiveBonus { get; set; }
        //public int bonusNum { get; set; }
        //public int isGiveProduct { get; set; }
        //public int productID { get; set; }
        public int isRepeatWrite { get; set; }
        public int isRepeatGift { get; set; }
        public string event_ID { get; set; }
        public int isNewMember { get; set; }
        public DateTime paperStart { get; set; }
        public DateTime paperEnd { get; set; }
        public int status { get; set; }
        public int creator { get; set; }
        public DateTime created { get; set; }
        public int modifier { get; set; }
        public DateTime modified { get; set; }
        public string ipfrom { get; set; }
        public Paper()
    {
            paperID = 0;
            paperName = string.Empty;
            paperMemo = string.Empty;
            paperBanner = string.Empty;
            bannerUrl = string.Empty;
            //isPromotion = 0;
            //promotionUrl = string.Empty;
            //isGiveBonus = 0;
            //bonusNum = 0;
            //isGiveProduct = 0;
            //productID = 0;
            isRepeatWrite = 0;
            isRepeatGift = 0;
            event_ID = string.Empty;
            isNewMember = 0;
            paperStart = DateTime.Now;
            paperEnd = DateTime.Now;
            status = 0;
            creator = 0;
            created = DateTime.Now;
            modifier = 0;
            modified = DateTime.Now;
            ipfrom = string.Empty;
        }
    }
}
