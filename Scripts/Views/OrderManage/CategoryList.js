Ext.Loader.setConfig({ enabled: true });
Ext.Loader.setPath('Ext.ux', '/Scripts/Ext4.0/ux');
Ext.require([
    'Ext.form.Panel',
    'Ext.ux.form.MultiSelect',
    'Ext.ux.form.ItemSelector'
]);
var pageSize = 25;
var sum = 0;
/**************群組管理主頁面************************/
//群組管理Model
Ext.define('gigade.Fares', {
    extend: 'Ext.data.Model',
    fields: [
        { name: "category_id", type: "int" }, //付款單號
        { name: "category_name", type: "string" },
        { name: "amo", type: "string" },
        { name: "sum", type: "string" }
    ]
});
var CategoryStore = Ext.create('Ext.data.Store', {
    pageSize: pageSize,
    model: 'gigade.Fares',
    proxy: {
        type: 'ajax',
        url: '/OrderManage/GetCategoryList',
        reader: {
            type: 'json',
            root: 'data',
            totalProperty: 'totalCount'
        }
    }
});
var DTStore = Ext.create('Ext.data.Store', {
    fields: ['txt', 'value'],
    data: [
        { "txt": "所有日期", "value": "0" },
        { "txt": "購買日期", "value": "1" }
    ]
});
CategoryStore.on('beforeload', function () {
    Ext.apply(CategoryStore.proxy.extraParams,
        {
            serchs: Ext.getCmp('serchs').getValue(),
            seldate: Ext.getCmp('seldate').getValue(),
            starttime: Ext.getCmp('deliverstart').getValue(),
            endtime: Ext.getCmp('deliverend').getValue(),
            brand_status: Ext.htmlEncode(Ext.getCmp("brand_status").getValue().Status)
        });
});

function Query(x) {
    CategoryStore.removeAll();
    Ext.getCmp("sum").setValue("類別總金額:0");
    Ext.getCmp("gdFgroup").store.loadPage(1, {
        params: {
            serchs: Ext.getCmp('serchs').getValue(),
            seldate: Ext.getCmp('seldate').getValue(),
            starttime: Ext.getCmp('deliverstart').getValue(),
            endtime: Ext.getCmp('deliverend').getValue(),
            brand_status: Ext.htmlEncode(Ext.getCmp("brand_status").getValue().Status)
        },
        callback: function (records, options, success) {
            if (records[0].data.sum.trim() == "") {
                sum = "類別總金額:" + 0;
                Ext.getCmp("sum").setValue(sum);
            } else {
                sum = "類別總金額:" + records[0].data.sum;
                Ext.getCmp("sum").setValue(sum);
            }
        }

    });
}
Ext.onReady(function () {

    var frm = Ext.create('Ext.form.Panel', {
        id: 'frm',
        height: 105,
        border: 0,
        bodyPadding: 10,
        width: document.documentElement.clientWidth,
        items: [
            {
                xtype: 'fieldcontainer',
                combineErrors: true,
                layout: 'hbox',
                items: [
                        {
                            xtype: 'combobox',
                            id: 'serchs',
                            editable: false,
                            fieldLabel: '類別選擇',
                            labelWidth: 60,
                            store: CateStore,
                            queryMode: 'local',
                            displayField: 'category_name',
                            valueField: 'category_id',
                            emptyText: 'SELECT',
                            emptyText: '所有活動',
                            value: 5,
                            listeners: {
                                beforerender: function () {
                                    CateStore.load();
                                }
                            }
                        },
                        {
                            xtype: 'combobox',
                            id: 'seldate',
                            editable: false,
                            fieldLabel: "日期條件",
                            labelWidth: 60,
                            store: DTStore,
                            displayField: 'txt',
                            valueField: 'value',
                            margin: '0 0 0 10',
                            value: 0
                        },
                        {
                            xtype: "datefield",
                            id: 'deliverstart',
                            name: 'deliverstart',
                            labelWidth: 40,
                            format: 'Y-m-d',
                            margin: '0 0 0 10',
                            submitValue: true,
                            editable: false,
                            value: Tomorrow(),
                            value: new Date(Tomorrow().setMonth(Tomorrow().getMonth())),
                            listeners: {
                                select: function (a, b, c) {
                                    var start = Ext.getCmp("deliverstart");
                                    var end = Ext.getCmp("deliverend");
                                    var s_date = new Date(end.getValue());
                                    if (end.getValue() < start.getValue()) {
                                        Ext.Msg.alert("提示", "開始時間不能大於結束時間！");
                                        start.setValue(new Date(s_date.setMonth(s_date.getMonth()-1)));
                                    }
                                }
                            }
                        },
                        { xtype: 'displayfield', value: "~" },
                        {
                            xtype: "datefield",
                            id: 'deliverend',
                            name: 'deliverend',
                            format: 'Y-m-d',
                            editable: false,
                            value: new Date(),
                            listeners: {
                                select: function (a, b, c) {
                                    var start = Ext.getCmp("deliverstart");
                                    var end = Ext.getCmp("deliverend");
                                    var s_date = new Date(start.getValue());
                                    if (end.getValue() < start.getValue()) {
                                        Ext.Msg.alert("提示", "開始時間不能大於結束時間！");
                                        end.setValue(new Date(s_date.setMonth(s_date.getMonth() + 1)));
                                    }
                                }
                            }
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
                        fieldLabel: '收款狀態',
                        id: 'brand_status',
                        labelWidth: 60,
                        name: 'brand_status',
                        colName: 'brand_status',
                        width: 350,
                        defaults: { name: 'Status' },
                        vertical: true,
                        items: [
                            {
                                boxLabel: '未收 + 實收金額',
                                id: 'a',
                                inputValue: '1',
                                hidden: false
                            },
                            {
                                boxLabel: '實收金額',
                                id: 'b',
                                inputValue: '2',
                                checked: true,
                                hidden: false
                            }
                        ]
                    },
                    {
                        xtype: 'displayfield',
                        value: "類別總金額:無",
                        id: 'sum',
                        width: 150
                    }
                ]
            },
            {
                xtype: 'fieldcontainer',
                combineErrors: true,
                layout: 'hbox',
                items: [
                    {
                        xtype: 'button',
                        text: SEARCH,
                        iconCls: 'icon-search',
                        margin: '0 10 0 10',
                        id: 'btnQuery',
                        handler: Query
                    },
                    {
                        xtype: 'button',
                        text: '重置',
                        iconCls: 'ui-icon ui-icon-reset',
                        margin: '0 10 0 0',
                        handler: function () {
                            this.up('form').getForm().reset();
                        }
                    }
                ]
            }
        ]
    });
    var gdFgroup = Ext.create('Ext.grid.Panel', {
        id: 'gdFgroup',
        store: CategoryStore,
        flex: 9.4,
        width: document.documentElement.clientWidth,
        columnLines: true,
        frame: true,
        columns: [
            { header: "類別", dataIndex: 'category_name', width: 200, align: 'center' },
            {
                header: "金額", dataIndex: 'amo', width: 200, align: 'center',
                renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                    if (value.trim() == "") {
                        return "0";
                    } else {
                        return value;
                    }
                }
            }
        ],
        bbar: Ext.create('Ext.PagingToolbar', {
            store: CategoryStore,
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
    ToolAuthority();
    //CategoryStore.load(
    //    {
    //        params: { start: 0, limit: 25, serchs: 5, brand_status: 0 },
    //        callback: function (records, options, success) {
    //            sum = "類別總金額:" + records[0].data.sum;
    //            Ext.getCmp("sum").setValue(sum);
    //        }
    //    });
    Ext.create('Ext.container.Viewport', {
        layout: 'vbox',
        items: [frm,gdFgroup],
        renderTo: Ext.getBody(),
        autoScroll: true,
        listeners: {
            resize: function () {
                gdFgroup.width = document.documentElement.clientWidth;
                this.doLayout();
            }
        }
    });
  
});
