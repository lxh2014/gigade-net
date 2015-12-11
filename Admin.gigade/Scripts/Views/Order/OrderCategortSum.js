pageSize = 20;
// 類別下拉框store
Ext.define('gigade.ProductCategory', {
    extend: 'Ext.data.Model',
    fields: [
    { name: 'category_name', type: 'string' },
    { name: 'category_id', type: 'int' }
    ]
});
var chooseCategoryStore = Ext.create('Ext.data.Store', {
    model: 'gigade.ProductCategory',
    autoLoad: true,
    proxy: {
        type: 'ajax',
        url: '/Order/GetProductCategoryStore',
        reader: {
            type: 'json',
            root: 'data'
        }
    }
});

var dateStore = Ext.create('Ext.data.Store', {
    fields: ['txt', 'value'],
    data: [
    { 'txt': '所有日期', 'value': '0' },
    { 'txt': '購買日期', 'value': '1' }
    ]
});

//列表頁store  GetCategorySummaryList
Ext.define('gigade.OrderDetail', {
    extend: 'Ext.data.Model',
    fields: [
    { name: 'amount', type: 'int' },
    { name: 'category_id', type: 'int' },
    { name: 'category_name', type: 'string' }
    ]
});
var CategorySummaryStore = Ext.create('Ext.data.Store', {
    model: 'gigade.OrderDetail',
    autoLoad: false,
    proxy: {
        type: 'ajax',
        url: '/Order/GetCategorySummaryList',
        reader: {
            type: 'json',
            root: 'data',
            totalProperty: 'totalCount'
        }
    }
});
CategorySummaryStore.on('beforeload', function () {
    Ext.apply(CategorySummaryStore.proxy.extraParams, {
        dateCon: Ext.getCmp('dateCon').getValue(),
        date_start: Ext.getCmp('timestart').getValue(),
        date_end: Ext.getCmp('timeend').getValue(),
        chooseCategory: Ext.getCmp('chooseCategory').getValue(),
        receiptStatus: Ext.getCmp('receiptStatus').getValue().status,
        c_money: Ext.getCmp('c_money').getValue(),
        c_money1: Ext.getCmp('c_money1').getValue()
    });
});
Ext.onReady(function () {
    var searchForm = Ext.create('Ext.form.Panel', {
        id: 'searchForm',
        border: 0,
        height: 120,
        layout: 'anchor',
        width: document.documentElement.clientWidth,
        items: [
        {
            xtype: 'fieldcontainer',
            layout: 'hbox',
            items: [
            {
                xtype: 'combobox',
                name: 'chooseCategory',
                id: 'chooseCategory',
                fieldLabel: '類別選擇',
                store: chooseCategoryStore,
                labelWidth: 60,
                margin: '5 10 0 5',
                typeAhead: true,
                queryMode: 'local',
                editable: true,
                submitValue: true,
                displayField: 'category_name',
                valueField: 'category_id',
                forceSelection: false,
                lastQuery: '',
                value: '5',
                listeners: {
                    focus: function ()
                    { this.setValue(''); }
                }
            },
            {
                xtype: 'combobox',
                id: 'dateCon',
                name: 'dateCon',
                store: dateStore,
                displayField: 'txt',
                valueField: 'value',
                fieldLabel: '日期條件',
                value: '0',
                editable: false,
                labelWidth: 60,
                width: 180,
                margin: '5 10 0 5',
            },
            {
                xtype: 'datefield',
                allowBlank: true,
                id: 'timestart',
                format: 'Y-m-d',
                margin: '5 5 0 0',
                editable: false,
                labelWidth: 60,
                value: new Date(new Date().getFullYear(), new Date().getMonth() - 1, new Date().getDate()),
                listeners: {
                    select: function (a, b, c) {
                        var start = Ext.getCmp("timestart");
                        var end = Ext.getCmp("timeend");
                        if (end.getValue() == null) {
                            end.setValue(setNextMonth(start.getValue(), 1));
                        } else if (end.getValue() < start.getValue()) {
                            end.setValue(setNextMonth(start.getValue(), 1));
                        }
                    }
                }
            },
            {
                xtype: 'displayfield',
                margin: '5 0 0 0',
                value: "~"
            },
            {
                xtype: 'datefield',
                allowBlank: true,
                id: 'timeend',
                format: 'Y-m-d',
                editable: false,
                margin: '5 0 0 5',
                value: new Date(new Date().getFullYear(), new Date().getMonth(), new Date().getDate()),
                listeners: {
                    select: function (a, b, c) {
                        var start = Ext.getCmp("timestart");
                        var end = Ext.getCmp("timeend");
                        if (start.getValue() != "" && start.getValue() != null) {
                            if (end.getValue() < start.getValue()) {
                                start.setValue(setNextMonth(end.getValue(), -1));
                            }
                        }
                        else {
                            start.setValue(setNextMonth(end.getValue(), -1));
                        }
                    }
                }
            }
            ]
        },
        {
            xtype: 'fieldcontainer',
            layout: 'hbox',
            items: [
            {
                xtype: 'radiogroup',
                fieldLabel: '收款狀態',
                id: 'receiptStatus',
                name: 'receiptStatus',
                labelWidth: 60,
                margin: '5 0 0 5',
                columns: 2,
                width: 300,
                vertical: true,
                items: [
                { boxLabel: '未收+實收金額', id: 'rdo1', name: 'status', inputValue: '0' },
                { boxLabel: '實收金額', id: 'rdo2', name: 'status', inputValue: '1', checked: true }
                ]
            },
            {
                xtype: "numberfield",
                fieldLabel: '金額範圍',
                id: 'c_money',
                name: 'c_money',
                labelWidth: 60,
                margin: '5 0 0 0',
                minValue: 0,
                value: 0
            },
            {
                xtype: 'displayfield',
                margin: '5 5 0 5',
                value: "~"
            },
            {
                xtype: "numberfield",
                id: 'c_money1',
                name: 'c_money1',
                labelWidth: 60,
                margin: '5 0 0 0',
                minValue: 0,
                value: 0
            },
            {
                xtype: 'button',
                text: SEARCH,
                id: 'btnQuery',
                margin: '5 5 0 7',
                iconCls: 'ui-icon ui-icon-search-2',
                handler: Query
            },
            {
                xtype: 'button',
                text: RESET,
                id: 'btn_reset',
                iconCls: 'ui-icon ui-icon-reset',
                margin: '5 5 0 0',
                listeners: {
                    click: function () {
                        Ext.getCmp("searchForm").getForm().reset();
                        Ext.getCmp("chooseCategory").setValue('');
                        Ext.getCmp("gdList").store.removeAll();
                    }
                }
            },
            ]
        },
        {
            xtype: 'fieldcontainer',
            layout: 'hbox',
            items: [
            {
                xtype: 'displayfield',
                fieldLabel: '類別總金額',
                margin: '5 0 0 5',
                labelWidth: 80,
                name: 'sum',
                id: 'sum',
            },
            {
                xtype: 'displayfield',
                id: 'sumValue',
                labelWidth: 5,
                value: '--------',
                margin: '5 0 0 5'
            },
            ]
        }
        ]
    });
    var gdList = Ext.create('Ext.grid.Panel', {
        id: 'gdList',
        store: CategorySummaryStore,
        width: document.documentElement.clientWidth,
        columnLines: true,
        frame: true,
        flex: 8,
        columns: [
        { header: "品牌編號", dataIndex: 'amount', flex: 1, align: 'center', hidden: true },
        { header: "類別", dataIndex: 'category_name', flex: 1, align: 'center' },
        {
            header: "金額", dataIndex: 'amount', flex: 1, align: 'center',
            renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                return "<a href='javascript:void(0)' onclick='openAmountDetial(" + rowIndex + ")'>" + change(value) + "</a>";
            }
        }
        ,
        ],
        bbar: Ext.create('Ext.PagingToolbar', {
            id: "toobar",
            store: CategorySummaryStore,
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
        items: [searchForm, gdList],
        renderTo: Ext.getBody(),
        listeners: {
            resize: function () {
                gdList.width = document.documentElement.clientWidth;
                this.doLayout();
            }
        }
    });
})

Query = function () {
    var chooseCategory = Ext.getCmp('chooseCategory').getValue();
    var dateCon = Ext.getCmp('dateCon').getValue();
    var date_start = Ext.getCmp('timestart').getValue();
    var date_end = Ext.getCmp('timeend').getValue();
    CategorySummaryStore.removeAll();
    Ext.getCmp("sumValue").reset();
    
    var c_money = Ext.getCmp('c_money').getValue();
    var c_money1 = Ext.getCmp('c_money1').getValue();
    if (c_money > c_money1)
    {
        Ext.getCmp('c_money').setValue(c_money1);
        Ext.getCmp('c_money1').setValue(c_money);
    }
    if (chooseCategory == null || chooseCategory == "") {
        Ext.Msg.alert(INFORMATION, "請選擇類別");
        return;
    }
    else if (dateCon != 0 && ((date_start == null || date_start == "") || (date_end == null || date_end == ""))) {
        Ext.Msg.alert(INFORMATION, "請選擇日期");
        return;
    }
    else {
        if (Ext.getCmp("toobar").getPageData().currentPage != 1) {
            Ext.getCmp("toobar").moveFirst();
        }
        CategorySummaryStore.load({
            callback: function (records, operation, success) {
                if (success) {
                    var result = Ext.decode(operation.response.responseText)
                    Ext.getCmp("sumValue").setValue(change(result.sumAmount));
                }
            }
        })
    }
}
setNextMonth = function (source, n) {
    var s = new Date(source);
    s.setMonth(s.getMonth() + n);
    if (n < 0) {
        s.setHours(0, 0, 0);
    }
    else if (n > 0) {
        s.setHours(23, 59, 59);
    }
    return s;
}

function openAmountDetial(index) {
    var id = Ext.getCmp("gdList").store.data.items[index].data.category_id;
    var name = Ext.getCmp("gdList").store.data.items[index].data.category_name;
    var amount = Ext.getCmp("gdList").store.data.items[index].data.amount;
    var dateCon = Ext.getCmp('dateCon').getValue();
    var date_start = Ext.htmlEncode(Ext.Date.format(new Date(Ext.getCmp('timestart').getValue()), 'Y-m-d H:i:s'));
    var date_end = Ext.htmlEncode(Ext.Date.format(new Date(Ext.getCmp('timeend').getValue()), 'Y-m-d H:i:s'));
    var c_money = Ext.getCmp('c_money').getValue();
    var c_money1 = Ext.getCmp('c_money1').getValue();

    var receiptStatus = Ext.getCmp('receiptStatus').getValue().status;
    var params = id + '|' + name + '|' + amount + '|' + receiptStatus + '|' + dateCon + '|' + date_start + '|' + date_end + '|' + c_money + '|' + c_money1;
    var urlTran = '/Order/OrderAmountDetial?_parameters=' + params;
    var panel = window.parent.parent.Ext.getCmp('ContentPanel');
    var copy = panel.down('#OrderAmountDetial');
    if (copy) {
        copy.close();
    }
    copy = panel.add({
        id: 'OrderAmountDetial',
        title: '類別訂單明細',
        html: window.top.rtnFrame(urlTran),
        closable: true
    });
    panel.setActiveTab(copy);
    panel.doLayout();
}

function change(value) {
    value = value.toString();
    if (/^\d+$/.test(value)) {
        if (value.length > 9) {
            value = value.replace(/^(\d+)(\d{3})(\d{3})(\d{3})$/, "$1,$2,$3,$4");
        }
        else if (value.length > 6) {
            value = value.replace(/^(\d+)(\d{3})(\d{3})$/, "$1,$2,$3");
        }
        else {
            value = value.replace(/^(\d+)(\d{3})$/, "$1,$2");
        }
    }
    return value;
}