
//庫存異動通知
var AuthView1 = Ext.create('Ext.view.View', {
    deferInitialRefresh: false,
    autoScroll: true,
    frame: true,
    plain: true,
    store: sendStore,
    tpl: Ext.create('Ext.XTemplate',
        '<div class="View">',
                '<div class="group" boder:"1px">',
                    '<dl>',
                        '<tpl for=".">',
                            '<div class="name downline">',
                                '<label style="font-weight: bold;">{user_mail}</label>&nbsp;<lable alt= "刪除" name="{row_id}" style="color:red; font-size:1.5em;font-weight: bold; cursor:pointer" onclick="pageCancel(this,stockFrm)" ">×</lable><br/>',
                            '</div>',
                        '</tpl>',
                    '</dl>',
                '</div>',
        '</div>'
    ),
    itemSelector: 'dl',
    overItemCls: 'group-hover'
});

var stockFrm = Ext.create('GIGADE.DraPanel', {
    id: 'stockFrm',
    title: STOCK_NOTICE,
    items: [{
        xtype: 'form',
        bodyPadding: 10,
        border: false,
        items: [items, {
            xtype: 'panel',
            autoScroll: true,
            frame: false,
            items: [AuthView1]
        }],
        defaults: { anchor: "95%", msgTarget: "side" },
        listeners: {
            dirtychange: function (form) {
                stockFrm.query('*[name=btnSave]')[0].setDisabled(false);
            },
            afterrender: function (form) {
                productFrm.query('*[name = Enter ]')[0];
            }
        }
    }],
    tools: [
            Ext.create('GIGADE.BtnSave', {
                handler: function () {
                    save(stockFrm);
                }
            })
    ],
    listeners: {
        beforerender: function (panel) {
            stockFrm.query('*[name=receiverComb]')[0].store.load();
            stockFrm.down().getForm().load({
                url: '/System/mailSetQuery',
                method: 'POST',
                params: {
                    paraType: 'warn_stock'
                },
                success: function (form, action) {
                    if (action.params.paraType == "warn_stock") {
                        if (action.result.data.sendTo != "") {
                            var listArray = action.result.data.sendTo.split(',');
                            for (var i = 0; i < listArray.length; i++) {
                                sendStore.insert(0, { row_id: listArray[i].split('|')[1], user_mail: listArray[i].split('|')[0], group_name: listArray[i].split('|')[2] });
                            }
                        }
                    }
                }
            })
        }
    }
});


function getStock(panel) {
    var c_switch = panel.query('*[name=switch]')[0].getValue() ? 1 : 0;
    var c_sendTo = "";
    for (var i = 0; i < sendStore.data.items.length; i++) {
        c_sendTo += sendStore.data.items[i].data.user_mail + "|" + sendStore.data.items[i].data.row_id + "|" + sendStore.data.items[i].data.group_name + ","
    }
    c_sendTo = c_sendTo.substring(0, c_sendTo.length - 1);
    var c_sendTime = Ext.htmlEncode(panel.query('*[name=sendTime]')[0].getValue());
    var result = Ext.String.format('[{ParameterType:\'{0}\',ParameterCode:\'{1}\',parameterName:\'{2}\',Rowid:{3}}', 'warn_stock', c_switch, 'switch', panel.query('*[name=switchId]')[0].getValue());
    result += Ext.String.format(',{ParameterType:\'{0}\',ParameterCode:\'{1}\',parameterName:\'{2}\',Rowid:{3}}', 'warn_stock', c_sendTo, 'sendTo', panel.query('*[name=sendToId]')[0].getValue());
    result += Ext.String.format(',{ParameterType:\'{0}\',ParameterCode:\'{1}\',parameterName:\'{2}\',Rowid:{3}}]', 'warn_stock', c_sendTime, 'sendTime', panel.query('*[name=sendTimeId]')[0].getValue());

    return result
}
