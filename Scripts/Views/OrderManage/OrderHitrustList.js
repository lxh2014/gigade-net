
var pageSize = 25;
var info_type = "order_payment_hitrust";
var secret_info = "id;pan;bankname";
/**********************************************************************網際威信銀行使用信息**************************************************************************************/
//站臺管理Model
Ext.define('gigade.Status', {
    extend: 'Ext.data.Model',
    fields: [
                { name: "id", type: "int" },//流水號
                { name: "order_id", type: "string" },
                { name: "retcode", type: "string" },
                { name: "retcodename", type: "string" },//刷卡狀態
                { name: "rettype", type: "string" },//交易類別
                { name: "depositamount", type: "int" },//請款金額
                { name: "approveamount", type: "int" },//核准金額
                { name: "orderstatus", type: "int" },//請款狀態 retcode==00&&orderstatus=3
                { name: "authRRN", type: "int" },//銀行調單編號
                { name: "capDate", type: "string" },//請款日期
                { name: "redem_discount_point", type: "string" },//本次折抵點數
                { name: "redem_discount_amount", type: "string" },//本次折抵金額
                { name: "createdate", type: "string" },//建立時間
                { name: "card_number", type: "string" }//卡號
                
    ]
});
var OrderHitrustStore = Ext.create('Ext.data.Store', {
    autoDestroy: true,
    pageSize: pageSize,
    model: 'gigade.Status',
    proxy: {
        type: 'ajax',
        url: '/OrderManage/GetOrderHitrustList',
        reader: {
            type: 'json',
            root: 'data',
            totalProperty: 'totalCount'
        }
    }
    //    autoLoad: true
});
OrderHitrustStore.on('beforeload', function () {
    Ext.apply(OrderHitrustStore.proxy.extraParams,
        {
            Order_Id: window.parent.GetOrderId()

        });
});

Ext.onReady(function () {
    var StatusGrid = Ext.create('Ext.grid.Panel', {
        id: 'StatusGrid',
        store: OrderHitrustStore,
        width: document.documentElement.clientWidth,
        columnLines: true,
        frame: true,
        columns: [
            { header: "流水號", dataIndex: 'id', width: 60, align: 'center' },
            { header: "號碼", dataIndex: 'retcode', width: 100, align: 'center', hidden: true },
            {
                header: "刷卡狀態", dataIndex: 'retcodename', width: 150, align: 'center', renderer:
                  function (value, cellmeta, record, rowIndex, columnIndex, store) {
                return '<font color="red">' + record.data.retcodename+ '(' + record.data.retcode + ')</font>';
                }
            },
            { header: "交易類別", dataIndex: 'rettype', width: 100, align: 'center' },
            {
                header: "請款金額", dataIndex: 'depositamount', width: 60, align: 'center', renderer: function (value) {
                    if (value == 0){
                        return "";
                    }
                    else {
                        return value;
                    }
                }
            },
            {
                header: "核准金額", dataIndex: 'approveamount', width: 60, align: 'center', renderer: function (value) {
                    if (value == 0) {
                        return "";
                    }
                    else {
                        return value;
                    }
                }
            },
            { header: "請款狀態", dataIndex: 'orderstatus', width: 60, align: 'center',renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                if (record.data.retcode == "00" && record.data.orderstatus==3) {
                    return "已請款";
                }
                else {
                    return "未請款";
                }
            } },
            {
                header: "銀行調單編號", dataIndex: 'authRRN', width: 200, align: 'center', renderer: function (value) {
                    if (value == 0) {
                        return "";
                    }
                    else {
                        return value;
                    }
                }
            },
            { header: "請款日期", dataIndex: 'capDate', width: 200, align: 'center' },
            { header: "本次折抵點數", dataIndex: 'redem_discount_point', width: 100, align: 'center' },
            { header: "本次折抵金額", dataIndex: 'redem_discount_amount', width: 100, align: 'center' },
            {
                header: '卡號', dataIndex: 'card_number', width: 100, align: 'center',
                renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                    return '<a href="#"  onclick="SecretLogin(' + record.data.order_id + ',' + 0 + ',\'' + info_type + '\')" >***(***)</a>';
                }
            },
            { header: "建立時間", dataIndex: 'createdate', width: 150, align: 'center' }

        ],

        bbar: Ext.create('Ext.PagingToolbar', {
            store: OrderHitrustStore,
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
        items: [StatusGrid],
        renderTo: Ext.getBody(),
        autoScroll: true,
        listeners: {
            resize: function () {
                StatusGrid.width = document.documentElement.clientWidth;
                this.doLayout();
            }
        }
    });
    ToolAuthority();
    OrderHitrustStore.load({ params: { start: 0, limit: 25 } });
});

Bank = function Bank(a, b, c) {
    var BankFrm = Ext.create('Ext.form.Panel', {
        constrain: true,
        items: [
            {
                xtype: 'displayfield',
                fieldLabel: '查詢ID',
                id: 'id',
                name: 'id'
            },
            {
                xtype: 'displayfield',
                fieldLabel: '交易卡號',
                id: 'pan',
                name: 'pan'
            },
            {
                xtype: 'displayfield',
                fieldLabel: '髮卡銀行',
                id: 'bankname',
                name: 'bankname'
            }
        ]
    });
    var BankWin = Ext.create('Ext.window.Window', {
        title: '卡號詳情',
        iconCls: 'icon-user-edit',
        width: 450,
        height: 300,
        layout: 'fit',
        items: [BankFrm],
        constrain: true,
        closeAction: 'destroy',
        modal: true,
        resizable: false,
        labelWidth: 60,
        bodyStyle: 'padding:5px 5px 5px 5px',
        listeners: {
            'beforerender': function () {
                Ext.getCmp('id').setValue(a);
                Ext.getCmp('pan').setValue(b);
                Ext.getCmp('bankname').setValue(c);
                //Ext.Ajax.request({
                //    url: '/OrderManage/GetData',
                //    params: {
                //        order_id: document.getElementById('OrderId').value
                //    },
                //    success: function (form, action) {
                //        var result = Ext.decode(form.responseText);
                //        // alert(result);
                //        if (result.success) {
                //            Ext.getCmp('order_id').setValue(result.data.order_id);
                //            Ext.getCmp('note_order').setValue(result.data.note_order);
                //            Ext.getCmp('user_username').setValue(result.data.manager_name);
                //            Ext.getCmp('note_order_modify_time').setValue(result.data.NoteOrderModifyTime);
                //        }
                //    }
                //});
            }
        },
        closable: false,
        tools: [
        {
            type: 'close',
            handler: function (event, toolEl, panel) {
                Ext.MessageBox.confirm('提示信息', '是否關閉窗口', function (btn) {
                    if (btn == "yes") {
                        BankWin.destroy();
                    }
                    else {
                        return false;
                    }
                });
            }
        }
        ]
    });
    BankWin.show();
}
function SecretLogin(rid, info_id, info_type) {//secretcopy
    var secret_type = "21";//參數表中的"訊息管理"可根據需要修改
    var url = "/OrderManage/GetOrderHitrustList";//這個可能在後面的SecretController中用到
    var ralated_id = rid;
    //點擊機敏信息先保存記錄在驗證密碼是否需要輸入
    boolPassword = SaveSecretLog(url, secret_type, ralated_id);//判斷5分鐘之內是否有輸入密碼
    if (boolPassword != "-1") {//不准查看
        if (boolPassword) {//超過5分鐘沒有輸入密碼      
            SecretLoginFun(secret_type, ralated_id, true, true, false, url, info_type, info_id, secret_info);//先彈出驗證框，關閉時在彈出顯示框

        } else {
            SecretLoginFun(secret_type, ralated_id, false, true, false, url, info_type, info_id, secret_info);//直接彈出顯示框
        }
    }
}