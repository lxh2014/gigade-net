
var CallidForm;
var pageSize = 25;
/**********************************************************************站臺管理主頁面**************************************************************************************/
//站臺管理Model
Ext.define('gigade.Money', {
    extend: 'Ext.data.Model',
    fields: [
        { name: "money_id", type: "int" },
        { name: "moneytype", type: "string" },
        { name: "money_total", type: "int" },
        { name: "states", type: "string" },
        { name: "money_source", type: "string" },
        { name: "money_createdate", type: "string" },
        { name: "money_note", type: "string" }, 
        { name: "createdate", type: "string" },
        { name: "cs_note", type: "string" }
    ]
});

var MoneyStore = Ext.create('Ext.data.Store', {
    autoDestroy: true,
    pageSize: pageSize,
    model: 'gigade.Money',
    proxy: {
        type: 'ajax',
        url: '/OrderManage/GetMoney',
        reader: {
            type: 'json',
            root: 'data',
            totalProperty: 'totalCount'
        }
    }
    //    autoLoad: true
});
MoneyStore.on('beforeload', function () {
    Ext.apply(MoneyStore.proxy.extraParams,
        {
            Order_Id: window.parent.GetOrderId()

        });
});
Ext.onReady(function () {
    var MoneyGrid = Ext.create('Ext.grid.Panel', {
        id: 'MoneyGrid',
        store: MoneyStore,
        width: document.documentElement.clientWidth,
        columnLines: true,
        frame: true,
        columns: [
            { header: "退款單號", dataIndex: 'money_id', width: 100, align: 'center' },
            { header: "退款方式", dataIndex: 'moneytype', width: 100, align: 'center' },
            {
                header: "金額", dataIndex: 'money_total', width: 100, align: 'center',
                renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                    return change(value);
                }
            },
            { header: "退款狀態", dataIndex: 'states', width: 100, align: 'center' },
            { header: "來源", dataIndex: 'money_source', width: 100, align: 'center' },
            { header: "退款單日期", dataIndex: 'createdate', width: 100, align: 'center' },
            { header: "客服備註", dataIndex: 'cs_note', width: 100, align: 'center' },
            { header: "會計備註", dataIndex: 'money_note', width: 100, align: 'center' },
            {
                header: "功能",  width: 120, align: 'center',
                renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                    //  return "<a href='javascript:void(0);' onclick='Import("+record.data.money_id+",'"+record.data.cs_note+"')'>修改客服退款備註</a>";
                    return "<a href='javascript:void(0);' onclick='Import("+record.data.money_id+","+'"'+record.data.cs_note+'"'+")'>修改客服退款備註</a>";
                }
            }
        ],
        bbar: Ext.create('Ext.PagingToolbar', {
            store: MoneyStore,
            pageSize: pageSize,
            displayInfo: true,
            displayMsg: NOW_DISPLAY_RECORD + ': {0} - {1}' + TOTAL + ': {2}',
            emptyMsg: NOTHING_DISPLAY
        }),
        listeners: {
            scrollershow: function (scroller) {
                if (scroller && scroller.scrollEl) {
                    scroller.clearManagedListeners();
                    scroller.mon(scroller.scrollEl, 'scroll', scroller.onElScroll, scroller);
                }
            }
        }

    });

    Ext.create('Ext.container.Viewport', {
        layout: 'fit',
        items: [MoneyGrid],
        renderTo: Ext.getBody(),
        autoScroll: true,
        listeners: {
            resize: function () {
                MoneyGrid.width = document.documentElement.clientWidth;
                this.doLayout();
            }
        }
    });
    ToolAuthority();
    MoneyStore.load({ params: { start: 0, limit: 25 } });
});



////////////////////////

Import = function (money_id,cs_note) {
        var ReturnMoneyFrm = Ext.create('Ext.form.Panel', {
            id: 'ReturnMoneyFrm',
            frame: true,
            plain: true,
            constrain: true,
            defaultType: 'textfield',
            autoScroll: true,
            layout: 'anchor',
            labelWidth: 45,
            url: '/OrderManage/SaveCSNote',
            defaults: { anchor: "95%", msgTarget: "side" },
            items: [
                {
                    xtype: 'displayfield',
                    id: 'money_id',
                    name: 'money_id',
                    fieldLabel: 'm_id',
                   hidden: true,
                },
                {
                    xtype: 'textareafield',
                    id: 'cs_note',
                    name: 'cs_note',
                    fieldLabel: '退款備註',
                    maxLength: 200,
                    //listeners: {
                    //    afterrender: function () {

                    //    }
                    //}
                },
        
            ],
            buttons: [{
                text: '確定',
                formBind: true,
                disabled: true,
                handler: function () {
                    var form = this.up('form').getForm();
                    var myMask = new Ext.LoadMask(Ext.getBody(), { msg: "Please wait..." });
                    myMask.show();
                    if (form.isValid()) {
                        form.submit({
                            params: {
                                cs_note:Ext.htmlEncode(Ext.getCmp('cs_note').getValue()),
                                money_id: Ext.getCmp('money_id').getValue(),
                            },
                            success: function (form, action) {
                                myMask.hide();
                                var result = Ext.decode(action.response.responseText);
                                if (result.success) {
                                    Ext.Msg.alert("提示", "修改成功！");
                                    ReturnMoneyWin.close();
                                    MoneyStore.load();
                                }
                                else {
                                    myMask.hide();
                                    Ext.Msg.alert("提示", "修改失敗!");
                                }
                            },
                            failure: function (form, action) {
                                myMask.hide();
                                Ext.Msg.alert("提示信息", "出現異常");
                            }
                        });
                    }
                }
            }]
        });

        var ReturnMoneyWin = Ext.create('Ext.window.Window', {
            iconCls: 'icon-user-edit',
            id: 'ReturnMoneyWin',
            width: 400,
            height: 200,
            title: '退款單備註',
            y: 100,
            layout: 'fit',
            items: [ReturnMoneyFrm],
            constrain: true,
            closeAction: 'destroy',
            modal: true,
            resizable: false,
            labelWidth: 60,
            bodyStyle: 'padding:5px 5px 5px 5px',
            closable: false,
            tools: [
             {
                 type: 'close',
                 qtip: '是否關閉',
                 handler: function (event, toolEl, panel) {
                     Ext.MessageBox.confirm("提示", "確認關閉？", function (btn) {
                         if (btn == "yes") {
                             Ext.getCmp('ReturnMoneyWin').destroy();

                         }
                         else {
                             return false;
                         }
                     });
                 }
             }
            ],
            listeners: {
                'show': function () {
                    Ext.getCmp('money_id').setValue(money_id);
                    Ext.getCmp('cs_note').setValue(cs_note);
                   
                }
            }
        });
        ReturnMoneyWin.show();
}







