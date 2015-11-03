pageSize = 25;

// 類別下拉框store
Ext.define('gigade.ProductCategory', {
    extend: 'Ext.data.Model',
    fields: [
    { name: 'category_name', type: 'string' }
    ]
});
var chooseCategoryStore = Ext.create('Ext.data.Store', {
    model: 'gigade.ProductCategory',
    autoLoad:true,
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
    { 'txt': '訂單日期', 'value': '1' }
    ]
});

//列表頁store  GetCategorySummaryList
Ext.define('gigade.OrderDetail', {
    extend: 'Ext.data.Model',
    fields: [
    { name: 'amount', type: 'int' },
    { name: 'category_name', type: 'string' }
    ]
});
var CategorySummaryStore = Ext.create('Ext.data.Store', {
    model: 'gigade.OrderDetail',
    autoLoad: true,
    proxy: {
        type: 'ajax',
        url: '/Order/GetCategorySummaryList',
        reader: {
            type: 'json',
            root: 'data'
        }
    }
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
                id: 'chooseCategory',
                fieldLabel: '類別選擇',
                store: chooseCategoryStore,
                displayField: 'category_name',           
                width: 180,
                labelWidth: 60,
                margin: '5 10 0 5',
                lastQuery: '',
                editable: false,              
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
                id: 'receiptStatus',
                name: 'receiptStatus',
                fieldLabel: "收款狀態",
                colName: 'receiptStatus',
                labelWidth: 60,
                margin: '5 10 0 5',
                width: 300,
                columns: 2,
                items: [
                { id: 'status1', boxLabel: "未收+實收金額", inputValue: '' },
                { id: 'status2', boxLabel: "實收金額", inputValue: '0', checked: true }
                ]
            },
            {
                xtype: 'button',
                text: SEARCH,
                id: 'btnQuery',
                margin: '5 5 0 7',
                iconCls: 'ui-icon ui-icon-search-2',
                //handler: Query
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
                name: 'sum',
                id: 'sum',
            },
             {
                 xtype: 'displayfield',
                 id: 'sumValue',
                 labelWidth: 10,
                 value: '---------',
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
        { header: "類別", dataIndex: 'pb_id', flex: 1, align: 'center' },
        { header: "金額", dataIndex: 'pb_startdate', flex: 1, align: 'center' }
        ],
        bbar: Ext.create('Ext.PagingToolbar', {
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