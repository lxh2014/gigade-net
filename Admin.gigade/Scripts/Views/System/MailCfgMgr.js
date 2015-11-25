

/*********自定义Protal**********/

var panels = new Array();
panels.push({ items: [stockFrm] });
panels.push({ items: [productFrm] });
panels.push({ items: [productMapFrm] });

Ext.define('GIGADE.Portal', {
    extend: 'Ext.container.Viewport',
    uses: ['Ext.app.PortalPanel', 'Ext.app.PortalColumn', 'Ext.app.GridPortlet', 'GIGADE.DraPanel'],
    initComponent: function () {
        Ext.apply(this, {
            layout: {
                type: 'border',
                padding: '0 5 5 5'
            },
            items: [{
                xtype: 'container',
                region: 'center',
                layout: 'border',
                items: [{
                    xtype: 'portalpanel',
                    region: 'center',
                    items: [panels]
                }]
            }]
        });
        this.callParent(arguments);
    }
});

/*********保存**********/
function save(panel) {
    if (!panel.down('form').getForm().isValid()) { return; };
    var result;
    var emailRegx = /^\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$/;
    //var g_sendTo = Ext.htmlEncode(panel.query('*[name=sendTo]')[0].getValue());
    var g_sendTo = "\n";
    var id = panel.id;
    switch (id) {
        case "stockFrm":
            for (var i = 0 ; i < sendStore.data.length; i++) {
                g_sendTo += sendStore.data.items[i].data.user_mail + ",";
            }
            g_sendTo = g_sendTo.substring(0, g_sendTo.length - 1);
            break;
        case "productFrm":
            for (var i = 0 ; i < sendStore2.data.length; i++) {
                g_sendTo += sendStore2.data.items[i].data.user_mail + ",";
            }
            g_sendTo = g_sendTo.substring(0, g_sendTo.length - 1);
            break;
        case "productMapFrm":
            for (var i = 0 ; i < sendStore3.data.length; i++) {
                g_sendTo += sendStore3.data.items[i].data.user_mail + ",";
            }
            g_sendTo = g_sendTo.substring(0, g_sendTo.length - 1);
            break;
    }


    //驗證收件人郵箱格式
    var re = /\n/g;
    g_sendTo = g_sendTo.replace(re, '');

    var array = g_sendTo.split(',');
    var bool = true;
    Ext.Array.each(array, function (value) {
        if (value) {
            if (!value.match(emailRegx)) {
                bool = false;
            }
        }
    });

    if (!bool) {
        Ext.Msg.alert(INFORMATION, MAIL + FORMAT_ERROR);
        return;
    }

    var paraType = '';

    //獲取保存數據
    switch (panel) {

        case stockFrm: result = getStock(panel); paraType = 'warn_stock'; break;
        case productFrm: result = getProduct(panel); paraType = 'warn_product'; break;
        case productMapFrm: result = getProductMap(panel); paraType = 'warn_productMap'; break;
        default: break;
    }

    if (result == -1) {///add by wwei0216w 如果email加起來超過300的長度限制result就 = -1
        Ext.Msg.alert(INFORMATION, LENGTH_ERROR);
        return;
    }

    Ext.Ajax.request({
        url: '/System/mailSetSave',
        params: {
            jsonSave: result
        },
        success: function (response) {
            var result = Ext.decode(response.responseText);
            if (result.success) {
                Ext.Msg.alert(INFORMATION, SUCCESS, function () {

                    panel.down('form').getForm().load({
                        url: '/System/mailSetQuery',
                        method: 'POST',
                        params: {
                            paraType: paraType
                        },
                        success: function (form, action) {}
                    });
                });
            }
            else {
                Ext.Msg.alert(INFORMATION, FAILURE);
            }
        }
    });

}

Ext.onReady(function () {

    Ext.create('GIGADE.Portal');

});


