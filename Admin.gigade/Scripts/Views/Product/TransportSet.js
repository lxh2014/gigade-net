/*  
 * 
 * 文件名称：TransportSet.js 
 * 摘    要：單一商品與組合商品修改和新增 物流配送模式頁面
 * 
 */
var PRODUCT_ID = 0, OLD_PRODUCT_ID = 0;
var COMBO_TYPE = location.pathname == '/Product/TransportSet' ? 1 : 2;

Ext.onReady(function () {
    PRODUCT_ID = window.parent.GetProductId() ? window.parent.GetProductId() : 0;
    OLD_PRODUCT_ID = window.parent.GetCopyProductId() ? window.parent.GetCopyProductId() : 0;

    var frm = Ext.create('Ext.form.Panel', {
        id: 'frm',
        layout: 'anchor',
        //defaults: { anchor: "40%", msgTarget: "side", labelWidth: 130 },
        border: false,
        //plain: true,
        bodyStyle: 'padding:10px 10px 0px 10px',
        items: [{
            xtype: 'checkboxgroup',
            fieldLabel: LOGISTICS_DISTRMODE,
            id: 'transportCheck',
            columns: 2,
            width: 500,
            allowBlank: false,
            vertical: true,
            items: [{ boxLabel: DISTRMODE_1, inputValue: '1|11', name: 'transportGroup', checked: true },
                { boxLabel: DISTRMODE_2, inputValue: '2|21', name: 'transportGroup' },
                { boxLabel: DISTRMODE_3, inputValue: '3|31', name: 'transportGroup' },
                { boxLabel: DISTRMODE_4, inputValue: '2|22', name: 'transportGroup' },
                { boxLabel: DISTRMODE_5, inputValue: '1|12', name: 'transportGroup' }
            ]
        }]
    });

    Ext.create('Ext.container.Viewport', {
        layout: 'fit',
        items: [frm],
        renderTo: Ext.getBody(),
        autoScroll: true,
        //padding: PRODUCT_ID == "" ? '10 1000 0 10' : '10 500 0 10',
        listeners: {
            resize: function (o) {
                this.doLayout();
            }
        }
    });

    Ext.Ajax.request({
        url: '/Product/GetProductDeliverySet',
        method: 'post',
        async: false,
        params: {
            productId: OLD_PRODUCT_ID ? OLD_PRODUCT_ID : PRODUCT_ID,
            comboType: COMBO_TYPE,
            oldProductId: OLD_PRODUCT_ID
        },
        success: function (data) {
            var values = eval("(" + data.responseText + ")");
            if (window.parent.GetProductId())
                Ext.getCmp("transportCheck").setValue({
                    'transportGroup': values
                });

        }
    });
    //ToolAuthority();
});

function save() {
    //添加 遮罩層  避免用戶多次點擊  edit by zhuoqin0830w  2015/09/24
    var mask;
    if (!mask) {
        mask = new Ext.LoadMask(Ext.getBody(), { msg: '請稍等...' });
    }
    mask.show();
    //添加disabled屬性  避免用戶多次點擊  edit by zhuoqin0830w  2015/09/24
    window.parent.setMoveEnable(false);

    var success = false;
    var frm = Ext.getCmp('frm').getForm();
    if (frm.isValid()) {
        var itcId = Ext.getCmp("transportCheck").getChecked();
        var itcIds = "";
        for (var i = 0; i < itcId.length; i++) {
            itcIds += itcId[i].inputValue + ',';
        }
        var itcs = itcIds.slice(0, -1);
        var items = itcs.split(',');

        var proDeliverySets = new Array();
        for (var i = 0, j = items.length; i < j; i++) {
            var area_type = items[i].split('|');
            proDeliverySets.push({
                Product_id: parseInt(PRODUCT_ID),
                Freight_big_area: parseInt(area_type[0]),
                Freight_type: parseInt(area_type[1])
            });
        }
        //儲存數據
        Ext.Ajax.request({
            url: '/Product/ProductDeliverySetSave',
            method: 'POST',
            async: window.parent.GetProductId() == '' ? false : true,
            params: {
                'proDeliverySets': Ext.encode(proDeliverySets),
                'comboType': COMBO_TYPE,
                'oldProductId': OLD_PRODUCT_ID
            }, success: function (msg) {
                var resMsg = eval("(" + msg.responseText + ")");
                mask.hide();
                if (resMsg.success) {
                    success = true;
                    if (PRODUCT_ID != '') {
                        Ext.Msg.alert(INFORMATION, SAVE_SUCCESS);
                    }
                }
                else {
                    Ext.Msg.alert(INFORMATION, resMsg.msg ? resMsg.msg : FAILURE);
                }
                window.parent.setMoveEnable(true);
            },
            failure: function () {
                mask.hide();
                Ext.Msg.alert(INFORMATION, FAILURE);
                window.parent.setMoveEnable(true);
            }
        });
    } else { mask.hide(); window.parent.setMoveEnable(true); }
    //window.parent.setMoveEnable(true);
    return success;
}