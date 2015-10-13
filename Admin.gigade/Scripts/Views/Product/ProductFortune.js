/*  
 * 
 * 文件名称：ProductFortune.js 
 * 摘    要：單一商品修改和新增 抽獎頁面
 * 
 */
var PRODUCT_ID = '', OLD_PRODUCT_ID = '';

function saveTemp() {
    var mask;
    if (!mask) {
        mask = new Ext.LoadMask(Ext.getBody(), { msg: WAIT });
    }
    mask.show();

    if (!save()) {
        return;
    }
    if (PRODUCT_ID == '') {
        Ext.Ajax.request({
            url: '/Product/Temp2Pro',
            method: 'POST',
            params: {
                OldProductId: OLD_PRODUCT_ID
            },
            success: function (response) {
                if (response.responseText != null) {
                    var data = eval("(" + response.responseText + ")");
                    mask.hide();
                    if (data.success) {
                        Ext.Msg.alert(INFORMATION, data.msg, function () {
                            if (OLD_PRODUCT_ID != '') {
                                //window.parent.history.go(-1);
                                window.parent.parent.Ext.getCmp('ContentPanel').activeTab.close();
                            } else {
                                window.parent.parent.Ext.getCmp('ContentPanel').activeTab.update(window.top.rtnFrame('/Product/ProductSave'));
                            }
                        });
                    }
                    else {
                        Ext.Msg.alert(INFORMATION, data.msg);
                    }
                }
            }
        });
    }
}

function save() {
    //添加 遮罩層  避免用戶多次點擊  edit by zhuoqin0830w  2015/09/24
    var mask;
    if (!mask) {
        mask = new Ext.LoadMask(Ext.getBody(), { msg: WAIT });
    }
    mask.show();
    //添加disabled屬性  避免用戶多次點擊  edit by zhuoqin0830w  2015/09/24
    window.parent.setMoveEnable(false);

    var form = Ext.getCmp('fortunePanel').getForm();
    var result = true;
    if (form.isValid()) {
        Ext.Ajax.request({
            url: '/Product/fortuneSave',
            method: 'POST',
            async: window.parent.GetProductId() == '' ? false : true,
            params: {
                ProductId: PRODUCT_ID,
                OldProductId: OLD_PRODUCT_ID,
                Fortune_Quota: Ext.getCmp('numQuota').getValue(),
                Fortune_Freight: Ext.getCmp('numFreight').getValue()
            },
            success: function (response, opts) {
                mask.hide();
                var resText = eval("(" + response.responseText + ")");
                if (PRODUCT_ID != '') {
                    if (resText.success) {
                        Ext.Msg.alert(NOTICE, SAVE_SUCCESS);
                    }
                    else {
                        Ext.Msg.alert(NOTICE, SAVE_FAIL);
                    }
                }
                else {
                    if (!resText.success) {
                        Ext.Msg.alert(NOTICE, SAVE_FAIL);
                        result = false;
                    }
                }
                window.parent.setMoveEnable(true);
            },
            failure: function (response, opts) {
                mask.hide();
                Ext.Msg.alert(NOTICE, SAVE_FAIL);
                window.parent.setMoveEnable(true);
                result = false;
            }
        });
    } else {
        window.parent.setMoveEnable(true);
        result = false;
    }
    return result;
}

var fortunePanel = Ext.create('Ext.form.Panel', {
    id: 'fortunePanel',
    width: 300,
    defaults: { anchor: "95%", msgTarget: "side" },
    border: false,
    padding: '20 0 0 20',
    items: [{
        xtype: 'numberfield',
        decimalPrecision: 0,
        id: 'numQuota',
        colName: 'numQuota',
        hidden: true,
        name: 'Fortune_Quota',
        margin: '0 0 20 0',
        value: 0,
        fieldLabel: FORTUNE_NUM,
        minValue: 0
    }, {
        xtype: 'numberfield',
        decimalPrecision: 0,
        id: 'numFreight',
        colName: 'numFreight',
        hidden: true,
        name: 'Fortune_Freight',
        value: 0,
        fieldLabel: FORTUNE_FREIGHT,
        minValue: 0
    }],
    listeners: {
        beforerender: function () {
            fortunePanel.getForm().load({
                url: '/Product/fortuneQuery',
                method: 'POST',
                params: {
                    ProductId: PRODUCT_ID,
                    OldProductId: OLD_PRODUCT_ID
                },
                success: function (form, action) { }

            });
        }
    }
});

Ext.onReady(function () {
    PRODUCT_ID = window.parent.GetProductId();
    OLD_PRODUCT_ID = window.parent.GetCopyProductId();

    Ext.create('Ext.container.Viewport', {
        items: [fortunePanel],
        renderTo: Ext.getBody(),
        autoScroll: true,
        listeners: {
            resize: function () {
                this.doLayout();
            },
            beforerender: function (view) {
                window.parent.updateAuth(view, 'colName');
            }
        }
    });
});