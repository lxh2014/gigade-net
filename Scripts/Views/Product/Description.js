var PRODUCT_ID = '', OLD_PRODUCT_ID = '';

Ext.onReady(function () {
    var frm = Ext.create('Ext.form.Panel', {
        id: 'frm',
        layout: 'anchor',
        defaults: { anchor: "95%", msgTarget: "side", labelWidth: 130 },
        border: false,
        plain: true,
        bodyStyle: 'padding:5px 5px 0px 5px',
        buttonAlign: 'center',
        buttons: [{
            text: SAVE,
            hidden: true,
            handler: function () {
                Save();
            }
        }],
        width: 900,
        items: [{
            xtype: 'textarea',
            fieldLabel: CONTENT_1,
            hidden: true,
            colName: 'page_content_1',
            id: 'page_content_1',
            name: 'page_content_1'
        }, {
            xtype: 'textarea',
            fieldLabel: CONTENT_2,
            hidden: true,
            allowBlank: false,
            colName: 'page_content_2',
            id: 'page_content_2',
            name: 'page_content_2'
        }, {
            xtype: 'textarea',
            fieldLabel: CONTENT_3,
            hidden: true,
            colName: 'page_content_3',
            id: 'page_content_3',
            name: 'page_content_3'
        }, {
            border: false,
            defaults: { labelWidth: 130 },
            items: [{
                xtype: 'numberfield',
                decimalPrecision: 0,
                minValue: 1,
                value: 20,
                width: 240,
                fieldLabel: BUY_LIMIT,
                hidden: true,
                colName: 'product_buy_limit',
                id: 'product_buy_limit',
                name: 'product_buy_limit'
            }]
        }, {
            xtype: 'textfield',
            fieldLabel: KEYWORDS,
            hidden: true,
            colName: 'product_keywords',
            id: 'product_keywords',
            name: 'product_keywords',
            listeners: {
                blur: function () {
                    var str = this.getValue().replace(/\s+[ ]{0,}/g, ',');
                    str = str.replace(/[,]{1,}/g, ',');
                    str = str.replace(/[，]{1,}/g, ',');
                    var last = str.substring(str.length - 1, str.length);
                    if (last == ",") {
                        str = str.substring(0, str.length - 1);
                    }
                    this.setValue(str);
                }
            }
        }, {
            xtype: 'checkboxgroup',
            fieldLabel: TAG,
            height: 40,
            hidden: true,
            colName: 'product_tag_set',
            id: 'product_tag_set',
            name: 'product_tag_set'
        }, {
            xtype: 'checkboxgroup',
            fieldLabel: NOTICE,
            hidden: true,
            colName: 'product_notice_set',
            id: 'product_notice_set',
            name: 'product_notice_set'
        }]
    });

    Ext.create('Ext.Viewport', {
        layout: 'anchor',
        items: [frm],
        renderTo: Ext.getBody(),
        autoScroll: true,
        listeners: {
            resize: function () {
                this.doLayout();
            }
        }
    });

    PRODUCT_ID = window.parent.GetProductId();
    OLD_PRODUCT_ID = window.parent.GetCopyProductId();
    requestTags(PRODUCT_ID, OLD_PRODUCT_ID);
    requestNotices(PRODUCT_ID, OLD_PRODUCT_ID);
    window.parent.GetProduct(this);
    window.parent.updateAuth(frm, 'colName');
});
function setForm(result) {
    Ext.getCmp('frm').down('#page_content_1').setValue(Ext.htmlDecode(result.Page_Content_1))
    Ext.getCmp('frm').down('#page_content_2').setValue(Ext.htmlDecode(result.Page_Content_2));
    Ext.getCmp('frm').down('#page_content_3').setValue(Ext.htmlDecode(result.Page_Content_3));
    if (window.parent.GetProductId() != ""&&result.Product_Buy_Limit != 0) {//edit by xiangwang0413w 2014/10/15 修改商品時,當Product_Buy_Limit為0時應顯示為0
        Ext.getCmp('frm').down('#product_buy_limit').setValue(result.Product_Buy_Limit);
    }
    Ext.getCmp('frm').down('#product_keywords').setValue(Ext.htmlDecode(result.Product_Keywords));
}
function requestTags(productId,oldproductId) {
    Ext.Ajax.request({
        url: '/Product/GetProTag',
        method: 'post',
        params: {
            ProductId: productId,
            OldProductId:oldproductId
        },
        success: function (form, action) {
            Ext.getCmp('product_tag_set').update(form.responseText);
        },
        failure: function () {
            Ext.Msg.alert(INFORMATION, DATA_LOAD_FAILURE);
            return false;
        }
    })
}
function requestNotices(productId,oldproductId) {
    Ext.Ajax.request({
        url: '/Product/GetProNotice',
        method: 'post',
        params: {
            ProductId: productId,
            OldProductId:oldproductId
        },
        success: function (form, action) {
            Ext.getCmp('product_notice_set').update(form.responseText);
        },
        failure: function () {
            Ext.Msg.alert(INFORMATION, DATA_LOAD_FAILURE);
            return false;
        }
    })
}

function save(functionid) {
    var success = false;
    var frm = Ext.getCmp('frm').getForm();
    if (frm.isValid()) {

        var notice_checked = '[', tag_checked = '[';
        $('input[name="notices"]:checked').each(function (idx) {
            notice_checked += '{notice_id:' + $(this).val()+"}";
        });
        $('input[name="tags"]:checked').each(function (idx) {
            tag_checked += '{tag_id:' + $(this).val()+"}";
        });
        notice_checked += "]"; tag_checked += "]";
        notice_checked = notice_checked.replace(/}{/g, '},{');
        tag_checked = tag_checked.replace(/}{/g, '},{');

        if (!functionid) {
            functionid = '';
        }

        Ext.Ajax.request({
            url: '/Product/SaveDescription',
            method: 'post',
            async: false,
            params: {
                page_content_1: Ext.htmlEncode(Ext.getCmp('page_content_1').getValue()),
                page_content_2: Ext.htmlEncode(Ext.getCmp('page_content_2').getValue()),
                page_content_3: Ext.htmlEncode(Ext.getCmp('page_content_3').getValue()),
                product_keywords: Ext.htmlEncode(Ext.getCmp('product_keywords').getValue()),
                product_buy_limit: Ext.getCmp('product_buy_limit').getValue(),
                Tags: tag_checked,
                Notice: notice_checked,
                ProductId: PRODUCT_ID,
                OldProductId: OLD_PRODUCT_ID,
                "function": functionid,
                "batch": window.parent.GetBatchNo()
            },
            success: function (form, action) {
                var result = Ext.decode(form.responseText);
                if (result.success) {
                    success = true;
                    if (PRODUCT_ID != '') {
                        Ext.Msg.alert(INFORMATION, SUCCESS);
                    }
                }
                else {
                    Ext.Msg.alert(INFORMATION, FAILURE);
                    window.parent.setMoveEnable(true);
                }
            },
            failure: function () {
                Ext.Msg.alert(INFORMATION, FAILURE);
                window.parent.setMoveEnable(true);
            }
        })
    }
    window.parent.setMoveEnable(true);
    return success;
}