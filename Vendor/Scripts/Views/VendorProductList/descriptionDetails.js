var descripItems = [
{//權限問題：我把 hidden: true, 改成hidden: false,
    xtype: 'displayfield',
    fieldLabel: CONTENT_1,
    hidden: false,
    id: 'page_content_1',
    name: 'page_content_1',
    colName: 'page_content_1',
    editable: false,
    style: { borderBottom: '1px solid #ced9e7', paddingTop: '5px' }
}, 
{
    xtype: 'displayfield',
    fieldLabel: CONTENT_2,
    editable: false,
    hidden: false,
    colName: 'page_content_2',
    id: 'page_content_2',
    name: 'page_content_2',
    style: { borderBottom: '1px solid #ced9e7', paddingTop: '5px' }
}, 
{
    xtype: 'displayfield',
    fieldLabel: CONTENT_3,
    editable: false,
    colName: 'page_content_3',
    hidden: false,
    id: 'page_content_3',
    name: 'page_content_3',
    style: { borderBottom: '1px solid #ced9e7', paddingTop: '5px' }
}, 
{
    xtype: 'displayfield',
    width: 240,
    fieldLabel: BUY_LIMIT,
    hidden: false,
    colName: 'product_buy_limit',
    id: 'product_buy_limit',
    name: 'product_buy_limit',
    style: { borderBottom: '1px solid #ced9e7', paddingTop: '5px' }
}, 
{
    xtype: 'displayfield',
    fieldLabel: KEYWORDS,
    hidden: false,
    colName: 'product_keywords',
    id: 'product_keywords',
    name: 'product_keywords',
    style: { borderBottom: '1px solid #ced9e7', paddingTop: '5px' }
}, 
{
    xtype: 'checkboxgroup',
    fieldLabel: TAG,
    height: 40,
    colName: 'product_tag_set',
    hidden: false,
    id: 'product_tag_set',
    name: 'product_tag_set',
    style: { borderBottom: '1px solid #ced9e7', paddingTop: '5px' }
}, 
{
    xtype: 'checkboxgroup',
    fieldLabel: NOTICE,
    colName: 'product_notice_set',
    hidden: false,
    id: 'product_notice_set',
    name: 'product_notice_set',
    style: { borderBottom: '1px solid #ced9e7', paddingTop: '5px' }
}
]
