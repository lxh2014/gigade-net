using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model
{
    public class EdmTest : PageBase
    {
        public int email_id { get; set; }
        public string test_username { get; set; }
        public int test_status { get; set; }
        public int test_createdate { get; set; }
        public int test_updatedate { get; set; }
        public EdmTest()
        {
            email_id = 0;
            test_username = string.Empty;
            test_status = 0;
            test_createdate = 0;
            test_updatedate = 0;
        }
    }
}
