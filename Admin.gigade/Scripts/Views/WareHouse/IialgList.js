Ext.Loader.setConfig({ enabled: true });
Ext.Loader.setPath('Ext.ux', '/Scripts/Ext4.0/ux');
Ext.require([
    'Ext.form.Panel',
     'Ext.ux.form.MultiSelect',
     'Ext.ux.form.ItemSelector'
]);
var pageSize = 25;
var boolPassword = true;//secretcopy
var info_type = "manager_user";
var secret_info = "";
Ext.define('gigade.Iialg', {
    extend: 'Ext.data.Model',
    fields: [
        { name: "row_id", type: "int" },
        { name: "loc_id", type: "string" },
        { name: "item_id", type: "int" },
        { name: "iarc_id", type: "string" },
        { name: "qty_o", type: "int" },
        { name: "type", type: "int" },
        { name: "adj_qty", type: "int" },
        { name: "create_dtim", type: "string" },
        { name: "create_user", type: "string" },
        { name: "doc_no", type: "string" },
        { name: "po_id", type: "string" },
        { name: "made_dt", type: "string" },
        { name: "cde_dt", type: "string" },
        { name: "c_made_dt", type: "string" },
        { name: "c_cde_dt", type: "string" },
        { name: "remarks", type: "string" },
        { name: "product_name", type: "string" },
        { name: "prod_sz", type: "string" },
        { name: "qty", type: "string" },
        { name: "loc_R", type: "string" },
        { name: "id", type: "int" },
        { name: "name", type: "string" }
    ]
});
secret_info = "user_id;user_name;user_email";
Ext.define("gigade.kucuntiaozhengModel", {
    extend: 'Ext.data.Model',
    fields: [
        { name: 'ParameterCode', type: 'string' },
        { name: 'parameterName', type: 'string' }
    ]
});
//庫調原因
var KutiaoStore = Ext.create("Ext.data.Store", {
    model: 'gigade.kucuntiaozhengModel',
    autoLoad: true,
    proxy: {
        type: 'ajax',
        url: "/WareHouse/Getkutiaowhy",
        actionMethods: 'post',
        reader: {
            type: 'json',
            root: 'data'
        }
    }
});
var IialgStore = Ext.create('Ext.data.Store', {
    autoDestroy: true,
    pageSize: pageSize,
    model: 'gigade.Iialg',
    proxy: {
        type: 'ajax',
        url: '/WareHouse/GetIialgList',
        reader: {
            type: 'json',
            root: 'data',
            totalProperty: 'totalCount'
        }
    }
});
IialgStore.on('beforeload', function () {
    Ext.apply(IialgStore.proxy.extraParams, {
        item_id: Ext.getCmp('item_id').getValue(),
        loc_id: Ext.getCmp('loc_id').getValue(),
        po_id: Ext.getCmp('po_id').getValue(),
        //iarc_id: Ext.getCmp('iarc_id').getValue(),
        starttime: Ext.getCmp('start_time').getValue(),
        endtime: Ext.getCmp('end_time').getValue()
    })
});
function Query(x) {
    IialgStore.removeAll();
    Ext.getCmp("IialgView").store.loadPage(1, {
        params: {
            item_id: Ext.getCmp('item_id').getValue(),
            loc_id: Ext.getCmp('loc_id').getValue(),
            po_id: Ext.getCmp('po_id').getValue(),
            //iarc_id: Ext.getCmp('iarc_id').getValue(),            
            starttime: Ext.getCmp('start_time').getValue(),
            endtime: Ext.getCmp('end_time').getValue()
        }
    });
}
Ext.onReady(function () {
    var frm = Ext.create('Ext.form.Panel', {
        id: 'frm',
        layout: 'anchor',
        height: 100,
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
                        xtype: 'textfield',
                        fieldLabel: '商品細項編號',
                        id: 'item_id',
                        name: 'item_id',
                        listeners: {
                            specialkey: function (field, e) {
                                if (e.getKey() == Ext.EventObject.ENTER) {
                                    Query();
                                }
                            }
                        }
                    },
                    {
                        xtype: 'textfield',
                        fieldLabel: '調整料位',
                        id: 'loc_id',
                        name: 'loc_id',
                        listeners: {
                            specialkey: function (field, e) {
                                if (e.getKey() == Ext.EventObject.ENTER) {
                                    Query();
                                }
                            }
                        }
                    },
                    {
                        xtype: 'textfield',
                        fieldLabel: '前置單號',
                        id: 'po_id',
                        name: 'po_id',
                        margin: '0 5 0 2',
                        listeners: {
                            specialkey: function (field, e) {
                                if (e.getKey() == Ext.EventObject.ENTER) {
                                    Query();
                                }
                            }
                        }
                    },
                    {
                        xtype: 'combobox',
                        id: 'iarc_id',
                        name: 'iarc_id',
                        fieldLabel: '庫調原因',
                        displayField: 'txt',
                        valueField: 'value',
                        labelWidth: 70,
                        hidden: true,
                        editable: false,
                        store: KutiaoStore,
                        emptyText: '請選擇...',
                        value: 0
                    }
                ]
            },
            {
                xtype: 'fieldcontainer',
                combineErrors: true,
                layout: 'hbox',
                items: [
                     { xtype: 'label', text: '調整時間:' },
                {
                    xtype: "datefield",
                    editable: false,
                    margin: '0 0 0 5',
                    id: 'start_time',
                    name: 'start_time',
                    format: 'Y/m/d',
                    value: new Date(Tomorrow().setMonth(Tomorrow().getMonth() - 1)),
                    listeners: {
                        select: function (a, b, c) {
                            var start = Ext.getCmp("start_time");
                            var end = Ext.getCmp("end_time");
                            var s_date = new Date(end.getValue());
                            if (end.getValue() < start.getValue()) {
                                Ext.Msg.alert("提示", "開始時間不能大於結束時間！");
                                start.setValue(new Date(s_date.setMonth(s_date.getMonth() - 1)));
                            }
                        },
                        specialkey: function (field, e) {
                            if (e.getKey() == Ext.EventObject.ENTER) {
                                Query();
                            }
                        }
                    }
                },
                { xtype: 'displayfield', value: '~ ' },
                {
                    xtype: "datefield",
                    editable: false,
                    id: 'end_time',
                    name: 'end_time',
                    format: 'Y/m/d',
                    value: Tomorrow(),
                    listeners: {
                        select: function (a, b, c) {
                            var start = Ext.getCmp("start_time");
                            var end = Ext.getCmp("end_time");
                            var s_date = new Date(start.getValue());
                            if (end.getValue() < start.getValue()) {
                                Ext.Msg.alert("提示", "開始時間不能大於結束時間！");
                                end.setValue(new Date(s_date.setMonth(s_date.getMonth() + 1)));
                            }
                        },
                        specialkey: function (field, e) {
                            if (e.getKey() == Ext.EventObject.ENTER) {
                                Query();
                            }
                        }
                    }
                },
                ]
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
                        margin: '0 10 0 10',
                        iconCls: 'ui-icon ui-icon-reset',
                        listeners: {
                            click: function () {
                                Ext.getCmp('item_id').setValue('');
                                Ext.getCmp('loc_id').setValue('');
                                Ext.getCmp('po_id').setValue('');
                                Ext.getCmp('iarc_id').setValue(0);
                                Ext.getCmp('start_time').setValue(new Date(Tomorrow().setMonth(Tomorrow().getMonth() - 1)));
                                Ext.getCmp('end_time').setValue(Tomorrow());
                            }
                        }
                    }
                ]
            }
        ]
    });
    var IialgView = Ext.create('Ext.grid.Panel', {
        id: 'IialgView',
        store: IialgStore,
        flex: 8,
        width: document.documentElement.clientWidth,
        columnLines: true,
        frame: true,
        columns: [
            { header: "編號", dataIndex: 'id', width: 40, align: 'center' },
            { header: "庫存調整單號", dataIndex: 'row_id', width: 80, align: 'center' },
            { header: "主料位", dataIndex: 'loc_id', width: 80, align: 'center' },
            { header: "調整料位", dataIndex: 'loc_R', width: 80, align: 'center' },
            { header: "商品細項編號", dataIndex: 'item_id', width: 80, align: 'center' },
            { header: "商品名稱", dataIndex: 'product_name', width: 150, align: 'center' },
            { header: "商品規格", dataIndex: 'prod_sz', width: 50, align: 'center' },
            {
                header: "製造日期", dataIndex: 'made_dt', width: 90, align: 'center',
                renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                    if (value != '' && value.length >= 10) {
                        return value.substring(0, 10);
                    }
                }
            },
            {
                header: "有效日期", dataIndex: 'cde_dt', width: 90, align: 'center',
                renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                    if (value != '' && value.length >= 10) {
                        return value.substring(0, 10);
                    }
                }
            },
             {
                 header: "新製造日期", dataIndex: 'c_made_dt', width: 100, align: 'center',
                 renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                     if (value != '' && value.length >= 10) {
                         if (value.substring(0, 10) == "0001-01-01") {
                             return "日期無更改";
                         }
                         else {
                             return value.substring(0, 10);
                         }
                     }
                 }
             },
            {
                header: "新有效日期", dataIndex: 'c_cde_dt', width: 100, align: 'center',
                renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                    if (value != '' && value.length >= 10) {
                        if (value.substring(0, 10) == "0001-01-01") {
                            return "日期無更改";
                        }
                        else {
                            return value.substring(0, 10);
                        }
                    }
                }
            },
            { header: "調整前數量", dataIndex: 'qty_o', width: 70, align: 'center' },
            { header: "調整數量", dataIndex: 'adj_qty', width: 70, align: 'center' },
            { header: "調整后數量", dataIndex: 'qty', width: 70, align: 'center' },
            { header: "庫調原因", dataIndex: 'iarc_id', width: 70, align: 'center' },
            { header: "調整日期", dataIndex: 'create_dtim', width: 120, align: 'center' },
            { header: "調整人員", dataIndex: 'name', width: 70, align: 'center' },
            { header: "前置單號", dataIndex: 'po_id', width: 100, align: 'center' },
            { header: "備註欄", dataIndex: 'remarks', width: 100, align: 'center' }
        ],
        tbar: [
           { xtype: 'button', text: "歷史記錄匯出", id: 'exportExcel', icon: '../../../Content/img/icons/excel.gif', handler: ExportExcel }
        ],
        bbar: Ext.create('Ext.PagingToolbar', {
            store: IialgStore,
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
        layout: 'vbox',
        items: [frm, IialgView],
        renderTo: Ext.getBody(),
        autoScroll: true,
        listeners: {
            resize: function () {
                IialgView.width = document.documentElement.clientWidth;
                this.doLayout();
            }
        }
    });
    ToolAuthority();
    //IialgStore.load({ params: { start: 0, limit: 25 } });
})
//匯出報表
ExportExcel = function () {
    window.open("/WareHouse/IialgExcel?item_id=" + Ext.getCmp("item_id").getValue() + "&loc_id=" + Ext.getCmp("loc_id").getValue() + "&po_id=" + Ext.getCmp("po_id").getValue() + "&starttime=" + Ext.Date.format(new Date(Ext.getCmp('start_time').getValue()), 'Y-m-d H:i:s') + "&endtime=" + Ext.Date.format(new Date(Ext.getCmp('end_time').getValue()), 'Y-m-d H:i:s'));
}
function Tomorrow() {
    var d;
    d = new Date();
    d.setDate(d.getDate() + 1);
    return d;
}
