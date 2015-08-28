var baseItems = [//權限問題：我把 hidden: true, 改成hidden: false,
{
xtype: 'displayfield',
hidden: false,
name: 'brand_name',
width: 400,
colName: 'brand_name',
fieldLabel: BRAND,
style: { borderBottom: '1px solid #ced9e7' }
},
{
    xtype: 'displayfield',
    id: 'product_name',
    hidden: false,
    width: 400,
    colName: 'product_name',
    name: 'Product_Name',
    fieldLabel: PRODUCT_NAME,
    style: { borderBottom: '1px solid #ced9e7' }
},
{
    xtype: 'displayfield',
    hidden: false,
    id: 'product_sort',
    fieldLabel: PRODUCT_SORT,
    name: 'product_sort',
    colName: 'product_sort',
    width: 400,
    style: { borderBottom: '1px solid #ced9e7' }
},
{
    xtype: 'displayfield',
    hidden: false,
    id: 'product_vendor_code',
    fieldLabel: PRODUCT_VENDOR_CODE,
    width: 400,
    name: 'product_vendor_code',
    colName: 'product_vendor_code',
    style: { borderBottom: '1px solid #ced9e7' }
},
{
    xtype: 'displayfield',
    id: 'product_start',
    fieldLabel: PRODUCT_START,
    colName: 'product_start',
    name: 'product_start',
    hidden: false,
    width: 400,
    style: { borderBottom: '1px solid #ced9e7' }
},
{
    xtype: 'displayfield',
    id: 'product_end',
    fieldLabel: PRODUCT_END,
    colName: 'product_end',
    width: 400,
    name: 'product_end',
    hidden: false,
    style: { borderBottom: '1px solid #ced9e7' }
},
{
    xtype: 'displayfield',
    id: 'expect_time',
    fieldLabel: EXPECT_TIME,
    width: 400,
    hidden: false,
    name: 'expect_time',
    colName: 'product_end',
    style: { borderBottom: '1px solid #ced9e7' }
},
{
    xtype: 'displayfield',
    hidden: false,
    id: 'product_freight_set',
    fieldLabel: PRODUCT_FREIGHT_SET,
    width: 400,
    name: 'product_freight_set',
    colName: 'product_freight_set',
    style: { borderBottom: '1px solid #ced9e7' }
},
{
    xtype: 'displayfield',
    hidden: false,
    id: 'product_mode',
    name: 'product_mode',
    fieldLabel: PRODUCT_MODE,
    width: 400,
    colName: 'product_mode',
    style: { borderBottom: '1px solid #ced9e7' }
},
{
    xtype: 'displayfield',
    hidden: false,
    id: 'tax_type',
    fieldLabel: TAX_TYPE,
    width: 400,
    name: 'tax_type',
    colName: 'tax_type',
    style: { borderBottom: '1px solid #ced9e7' }
},
{
    xtype: 'displayfield',
    id: 'combination',
    name: 'combination',
    width: 400,
    fieldLabel: COMBINATION,
    hidden: false,
    colName: 'combination',
    style: { borderBottom: '1px solid #ced9e7' }
},
{
    xtype: 'displayfield',
    width: 400,
    id: 'expect_msg',
    fieldLabel: EXPECT_MSG,
    hidden: false,
    colName: 'expect_msg',
    name: 'expect_msg',
    style: { borderBottom: '1px solid #ced9e7' }
}]