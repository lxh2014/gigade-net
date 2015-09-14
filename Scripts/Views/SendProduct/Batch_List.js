Ext.Loader.setConfig({ enabled: true });
Ext.Loader.setPath('Ext.ux', '/Scripts/Ext4.0/ux');
Ext.require([
    'Ext.form.Panel',
    'Ext.ux.form.MultiSelect',
    'Ext.ux.form.ItemSelector'
]);
var pageSize = 25;

Ext.define('GIGADE.BatchList', {
    extend: 'Ext.data.Model',
    fields: [
        { name: 'slave_master_id', type: 'int' },
        { name: 'code_num', type: 'string' },
        { name: 'paper', type: 'int' },
        { name: 'order_freight_normal', type: 'int' },
        { name: 'order_freight_low', type: 'int' },
        { name: 'normal_subtotal', type: 'int' },
        { name: 'hypothermia_subtotal', type: 'int' },
        { name: 'deliver_store', type: 'int' },
         { name: 'deliver_name', type: 'string' },
        { name: 'deliver_code', type: 'string' },
        { name: 'deliver_date', type: 'string' },
        { name: 'deliver_note', type: 'string' },
        { name: 'create_date', type: 'string' },
        { name: 'on_check', type: 'int' },
        { name: 'vendor_name_simple', type: 'string' }

    ]
});
//出貨查詢列表Store
var BatchStore = Ext.create("Ext.data.Store", {
    autoDestroy: true,
    pageSize: pageSize,
    model: 'GIGADE.BatchList',
    proxy: {
        type: 'ajax',
        url: '/SendProduct/GetBatchList',
        timeout: 360000,
        reader: {
            type: 'json',
            root: 'data',
            totalProperty: 'totalCount'
        }
    }
});
//加載前
BatchStore.on('beforeload', function () {
    Ext.apply(BatchStore.proxy.extraParams, {
        searchType: Ext.getCmp('searchType').getValue(),
        searchContent: Ext.getCmp('searchContent').getValue(),
        status: Ext.getCmp('status').getValue(),
        dateType: Ext.getCmp('dateType').getValue(),
        dateStart: Ext.getCmp('dateStart').getValue(),
        dateEnd: Ext.getCmp('dateEnd').getValue()
    });
});

//出貨查詢列表Store

Ext.define('GIGADE.BatchDetailList', {
    extend: 'Ext.data.Model',
    fields: [
        { name: 'order_id', type: 'int' },
        { name: 'slave_id', type: 'string' },
        { name: 'item_id', type: 'int' },
        { name: 'product_name', type: 'string' },
        { name: 'product_spec_name', type: 'string' },
        { name: 'buy_num', type: 'int' }

    ]
});


Ext.onReady(function () {
    searchStore.load();//加載查詢條件
    dateTypeStore.load();//加載日期條件
    //前面選擇框 選擇之後顯示編輯刪除
    //var sm = Ext.create('Ext.selection.CheckboxModel', {
    //    listeners: {
    //        selectionchange: function (sm, selections) {
    //            Ext.getCmp("gdBatchList").down('#sure').setDisabled(selections.length == 0);
    //        }
    //    }
    //});
    var gdBatchList = Ext.create('Ext.grid.Panel', {
        id: 'gdBatchList',
        store: BatchStore,
        width: document.documentElement.clientWidth - 6,
        height: document.documentElement.clientHeight - 123,
        columnLines: true,
        frame: true,
        viewConfig: {
            enableTextSelection: true,
            stripeRows: false,
            getRowClass: function (record, rowIndex, rowParams, store) {
                return "x-selectable";
            }
        },
        columns: [
            { header: "id", dataIndex: 'slave_master_id', width: 60, hidden: true, align: 'center' },
            {
                header: "批次出貨單號", dataIndex: 'code_num', width: 150, align: 'center',
                renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                    return "<a href='javascript:void(0);' onclick='TranToDetail(" + record.data.slave_master_id + ")'>" + value + '_' + record.data.paper + "</a>";
                }
            },
            {
                header: "檢查", dataIndex: 'on_check', width: 70, align: 'center',
                renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                    if (value == 1) {
                        return "是";
                    }
                    else {
                        return "否";
                    }
                }
            },
            { header: "供應商", dataIndex: 'vendor_name_simple', width: 150, align: 'center' },
            { header: "常溫貨物總金額", dataIndex: 'normal_subtotal', width: 120, align: 'center' },
            { header: "低溫貨物總金額", dataIndex: 'hypothermia_subtotal', width: 120, align: 'center' },
            { header: "常溫補貼運費", dataIndex: 'order_freight_normal', width: 120, align: 'center' },
            { header: "低溫補貼運費", dataIndex: 'order_freight_low', width: 120, align: 'center' },
            { header: "物流業者", dataIndex: 'deliver_name', width: 130, align: 'center' },//根據deliver_store從t_parametersrc參數表中查得
            { header: "貨運單號", dataIndex: 'deliver_code', width: 130, align: 'center' },
            { header: "出貨時間", dataIndex: 'deliver_date', width: 130, align: 'center' },
            { header: "建立時間", dataIndex: 'create_date', width: 130, align: 'center' },
            { header: "備註", dataIndex: 'deliver_note', width: 200, align: 'center' }
        ],
        //tbar: [//到貨確認功能棄用
        //    { xtype: 'button', text: "到貨確認", id: 'sure', disabled: true, iconCls: 'icon-accept', handler: makeSureClike }
        //],
        bbar: Ext.create('Ext.PagingToolbar', {
            store: BatchStore,
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

    var frm = Ext.create('Ext.form.Panel', {
        id: 'frm',
        layout: 'anchor',
        width: document.documentElement.clientWidth,
        height: document.documentElement.clientHeight,
        autoScroll: true,
        items: [
            {
                xtype: 'fieldcontainer',
                combineErrors: true,
                layout: 'hbox',
                margin: '10px 0 10px 0 ',
                items: [
                    {
                        xtype: 'combobox',
                        id: 'searchType',
                        margin: '0 5px',
                        fieldLabel: '查詢條件',
                        queryMode: 'local',
                        editable: false,
                        store: searchStore,
                        displayField: 'text',
                        valueField: 'value',
                        value: "0"
                    },
                    {
                        xtype: 'textfield',
                        id: 'searchContent',
                        name: 'searchContent',
                        margin: '0 5px',
                        fieldLabel: '查詢內容'
                    }
                ]
            },
            {
                xtype: 'fieldcontainer',
                combineErrors: true,
                layout: 'hbox',
                items: [
                    {
                        xtype: 'combobox',
                        id: 'dateType',
                        margin: '0 5px',
                        fieldLabel: '日期條件',
                        queryMode: 'local',
                        editable: false,
                        store: dateTypeStore,
                        displayField: 'text',
                        valueField: 'value',
                        value: "0"
                    },
                    {
                        xtype: 'datefield',
                        id: 'dateStart',
                        name: 'dateStart',
                        margin: '0 5px 0 5px',
                        editable: false,
                        value: Tomorrow(1 - new Date().getDate())
                    },
                    {
                        xtype: 'displayfield',
                        value: '~'
                    },
                    {
                        xtype: 'datefield',
                        id: 'dateEnd',
                        name: 'dateEnd',
                        margin: '0 5px',
                        editable: false,
                        value: Tomorrow(0)
                    }
                ]
            },
              {
                  xtype: 'fieldcontainer',
                  combineErrors: true,
                  layout: 'hbox',
                  items: [
                {
                    xtype: 'radiogroup',
                    id: 'status',
                    fieldLabel: "狀態",
                    margin: '0 5px',
                    width: 400,
                    defaults: {
                        name: 'status',
                        margin: '0 8 0 0'
                    },
                    columns: 3,
                    vertical: true,
                    items: [
             { boxLabel: "所有狀態", id: 's1', inputValue: '-1', checked: true },
             { boxLabel: "未檢查", id: 's2', inputValue: '0' },
             { boxLabel: "已檢查", id: 's3', inputValue: '1' }
                    ]
                }]
              },
            {
                xtype: 'fieldcontainer',
                combineErrors: true,
                layout: 'hbox',
                items: [
                    {
                        xtype: 'button',
                        margin: '0 10 0 10',
                        iconCls: 'icon-search',
                        text: "查詢",
                        handler: Query
                    },
                    {
                        xtype: 'button',
                        text: '重置',
                        id: 'btn_reset',
                        iconCls: 'ui-icon ui-icon-reset',
                        listeners: {
                            click: function () {
                                Ext.getCmp('searchType').setValue(0);
                                Ext.getCmp('searchContent').setValue("");
                                Ext.getCmp('dateType').setValue(0);
                                Ext.getCmp('deliverystore').setValue(0);
                                Ext.getCmp('s1').checked = true;
                                Ext.getCmp('dateStart').setValue(Tomorrow(1 - new Date().getDate()));
                                Ext.getCmp('dateEnd').setValue(Tomorrow(0));
                            }
                        }
                    }
                ]
            },
            gdBatchList
        ]
    });

    Ext.create('Ext.Viewport', {
        layout: 'vbox',
        items: [frm],
        renderTo: Ext.getBody(),

        listeners: {
            resize: function () {
                //  gdChannelOrder.width = document.documentElement.clientWidth;
                this.doLayout();
            }
        }
    });

    ToolAuthority();
    BatchStore.load({ params: { start: 0, limit: 25 } });
})
function Tomorrow(days) {
    var d;
    var s = "";
    d = new Date();                             // 创建 Date 对象。
    s += d.getFullYear() + "/";                     // 获取年份。
    s += (d.getMonth() + 1) + "/";              // 获取月份。
    s += d.getDate() + days;                          // 获取日。
    return (new Date(s));                                 // 返回日期。
}

//查询
Query = function () {
    BatchStore.removeAll();
    BatchStore.load();
}
//到貨確認功能棄用
//makeSureClike = function () {
//    var row = Ext.getCmp("gdBatchList").getSelectionModel().getSelection();
//    if (row.length == 0) {
//        Ext.Msg.alert(INFORMATION, NO_SELECTION);
//    } else if (row.length > 0) {
//        var smds = null;
//        for (var i = 0; i < row.length; i++) {
//            smds += row[i].data.slave_master_id;
//            if (i != row.length - 1) {
//                smds += ",";
//            }
//        }
//        Ext.Ajax.request({
//            url: '/SendProduct/BatchSendProd',
//            params: {
//                slaveMasterIds: smds
//            },
//            success: function (form, action) {
//                var result = Ext.decode(form.responseText);
//                if (result.success) {
//                    Ext.alert("確認完成！");
//                }
//            }
//        });

//    }
//}


function TranToDetail(slave_master_id) {


    var BatchDetailStore = Ext.create("Ext.data.Store", {
        autoDestroy: true,
        model: 'GIGADE.BatchDetailList',
        proxy: {
            type: 'ajax',
            url: '/SendProduct/GetBatchDetailList',
            timeout: 360000,
            reader: {
                type: 'json',
                root: 'data'
            }
        }
    });
    BatchDetailStore.load({
        params: { slave_master_id: slave_master_id },
        callback: function () {
            if (BatchDetailStore.getCount() > 0) {
                TranToDetailBySMD(BatchDetailStore);
            }
            else {
                Ext.Msg.alert(INFORMATION, "沒有數據！");
            }
        }
    })

}