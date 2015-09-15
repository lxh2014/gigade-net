Ext.onReady(function () {
    var exportTab = Ext.create('Ext.form.Panel', {
        layout: 'anchor',
        width: 600,
        url: '/Product/productcategoryinto',//批次商品類別匯入
        defaults: {
            labelWidth: 150,
            width: 400,
            margin: '20 10 0 20'
        },
        border: false,
        plain: true,
        id: 'InvoiceCheck',
        items: [
        {
            xtype: 'filefield',
            name: 'ImportFileMsg',
            id: 'ImportFileMsg',
            fieldLabel: PRODUCT_IN_CATEGORY_REPEATEDLY,
            msgTarget: 'side',
            buttonText: BROWSE,
            submitValue: true,
            allowBlank: false,
            fileUpload: true,
            validator:
                function (value) {
                    var type = value.split('.');
                    if (type[type.length - 1] == 'csv') {
                        return true;
                    }
                    else {
                        return IN_FILE_TYPE_WRONG;
                    }
                }
        }
        ],
        buttonAlign: 'right',
        buttons: [{
            text: AFFIRM_IN,
            formBind: true,
            disabled: true,
            handler: function () {
                var form = this.up('form').getForm();
                if (form.isValid()) {
                    form.submit({
                        waitMsg: IN_FILEING_PLEASE_WAIT,
                        waitTitle: NOTICE,
                        timeout: 9000000,
                        params: {
                            ImportFileMsg: Ext.htmlEncode(Ext.getCmp('ImportFileMsg').getValue())
                        },
                        success: function (form, action) {
                            var result = Ext.decode(action.response.responseText);
                            if (result.success) {
                               
                                var total = result.total;//匯入數據條數
                                if (parseInt(total) > 0) {//如果匯入成數量大於0
                                    if (parseInt(result.error) > 0) {//如果匯入失敗數量大於0
                                        if (parseInt(result.repeat) > 0) {//重複數據
                                            Ext.Msg.alert(ALERT_MESSAGE, MAIL_SUCCESS_MESSAGE + total + ITEM_COUNT + "<br/>" + MAIL_BUG_MESSAGE + result.error + ITEM_COUNT + "<br/>" + DATABASE_HAVE_THIS_DATA + result.repeat + ITEM_COUNT + "<br/>" + "<a href='#' onclick='updownthis()'>" + CLICK_DOWN_CANT_IN_DATA + "</a>");
                                        }
                                        else {
                                            Ext.Msg.alert(ALERT_MESSAGE, MAIL_SUCCESS_MESSAGE + total + ITEM_COUNT+"<br/>" + MAIL_BUG_MESSAGE + result.error + ITEM_COUNT+"<br/>" + "<a href='#' onclick='updownthis()'>"+CLICK_DOWN_CANT_IN_DATA+"</a>");
                                        }
                                    }
                                    else {
                                        if (parseInt(result.repeat) > 0) {
                                            Ext.Msg.alert(ALERT_MESSAGE, MAIL_SUCCESS_MESSAGE + total + ITEM_COUNT + "<br/>" + DATABASE_HAVE_THIS_DATA + result.repeat + ITEM_COUNT + "<br/>" + "<a href='#' onclick='updownthis()'>" + CLICK_DOWN_CANT_IN_DATA + "</a>");
                                        }
                                        else {
                                            Ext.Msg.alert(ALERT_MESSAGE, MAIL_SUCCESS_MESSAGE + total + ITEM_COUNT+"<br/>");
                                        }
                                    }
                                }
                                else {
                                    if (parseInt(result.error) > 0) {//如果匯入失敗數量大於0
                                        if (parseInt(result.repeat) > 0) {//重複數據
                                            Ext.Msg.alert(ALERT_MESSAGE, MAIL_BUG_MESSAGE + result.error + ITEM_COUNT + "<br/>" + DATABASE_HAVE_THIS_DATA + result.repeat + ITEM_COUNT + "<br/>" + "<a href='#' onclick='updownthis()'>" + CLICK_DOWN_CANT_IN_DATA + "</a>");
                                    }
                                    else {
                                            Ext.Msg.alert(ALERT_MESSAGE, MAIL_BUG_MESSAGE + result.error + ITEM_COUNT+"<br/>" + "<a href='#' onclick='updownthis()'>"+CLICK_DOWN_CANT_IN_DATA+"</a>");
                                    }
                                }
                                    else {
                                        if (parseInt(result.repeat) > 0) {
                                            Ext.Msg.alert(ALERT_MESSAGE, DATABASE_HAVE_THIS_DATA + result.repeat + ITEM_COUNT + "<br/>" + "<a href='#' onclick='updownthis()'>" + CLICK_DOWN_CANT_IN_DATA + "</a>");
                                }
                                else {
                                    Ext.Msg.alert(ALERT_MESSAGE, NOW_NOT_NEW_DATA_CAN_INSERT);
                                }
                            }
                                }
                            }
                            else {
                                Ext.Msg.alert(INFORMATION, FAILURE);
                            }
                        },
                        failure: function (form, action) {
                            Ext.Msg.alert(ALERT_MESSAGE, DATA_IN_FAILED);
                        }
                    });
                }
            }
        }]
    });
    var hpanel = Ext.create('Ext.form.Panel', {
        bodyStyle: "padding:5px;background:#87CEEB",
        border: false,
        html: IN_FILE_NEED_KNOW_MESSAGE
    });
    Ext.create('Ext.Viewport', {
        layout: 'anchor',
        items: [exportTab, hpanel],
        autoScroll: true
    });
});
function updownthis() {
    window.open("/Product/Updownmessage");
}
