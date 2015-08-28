using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model.Custom
{
    public class ProductCategoryCustom
    {
        public int rowid { get; set; }
        //public uint id { get; set; }
        public string id { get; set; }
        //edit by wangwei0216w
        public string parameterCode { get; set; }
        public string text { get; set; }
        public bool Checked { get; set; }
        public List<ProductCategoryCustom> children { get; set; }
        public bool leaf
        {
            get { return this.children.Count == 0; }
        }
        public bool expanded
        {
            get { return this.leaf ? false : true; }
        }

        public ProductCategoryCustom()
        {
            rowid = 0;
            id = string.Empty;
            parameterCode = string.Empty;
            text = string.Empty;
            Checked = false;
            children = new List<ProductCategoryCustom>();
        }
    }
}
