
using BLL.gigade.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace BLL.gigade.Model
{
    //trial_picture
    public class TrialPicture : PageBase
    {

        public int share_id { get; set; }
        public string image_filename { get; set; }
        public uint image_sort { get; set; }
        public uint image_state { get; set; }
        public uint image_createdate { get; set; }
       
        public TrialPicture()
        {
            share_id = 0;
            image_filename = string.Empty;
            image_sort = 0;
            image_state = 0;
            image_createdate = 0;

        }

    }
}

