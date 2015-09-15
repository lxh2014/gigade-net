
var pageSize = 31;

//表名數據
var tableNameStore = Ext.create('Ext.data.Store', {
    fields: ['table_name'],
    autoLoad: true,
    proxy: {
        type: 'ajax',
        url: "/ProductHistory/GetTableName",
        noCache: false,
        actionMethods: 'post',
        reader: {
            type: 'json'
        }
    }
});

//歷史詳情
var historyDetailStore = Ext.create('Ext.data.Store', {
    //autoLoad: true,
    fields: ['rowid', 'table_name', 'functionname', 'pk_name', 'pk_value', 'batchno', 'historyItem'],
    proxy: {
        type: 'ajax',
        url: "/ProductHistory/HistoryDetails",
        noCache: false,
        actionMethods: 'post',
        reader: {
            type: 'json'
        }
    }
});

//歷史記錄列表grid
var listStore = Ext.create('Ext.data.Store', {
    fields: ['batchno', 'user_username', 'kdate'],
    pageSize: pageSize,
    //autoLoad: true,
    proxy: {
        type: 'ajax',
        url: '/ProductHistory/GetHistory',
        actionMethods: 'post',
        reader: {
            type: 'json',
            root: 'item',
            totalProperty: 'totalCount'
        }
    }
});


listStore.on("beforeload", function () {
    Ext.apply(listStore.proxy.extraParams, {
        table_name: Ext.getCmp("table_name") ? Ext.getCmp("table_name").getValue() : '',
        pk_value: Ext.getCmp('pk_value') ? Ext.getCmp('pk_value').getValue() : ''
    })
})



Ext.onReady(function () {
    Ext.create('Ext.Viewport', {
        id: "index",
        width: document.documentElement.clientWidth,
        height: document.documentElement.clientHeight,
        layout: 'border',
        items: [{
            region: 'north',//上北
            xtype: 'panel',
            height: 30,
            split: false,
            items: searchfrm,
            margins: '3 3 0 3'
        }, {
            region: 'west',//左西
            xtype: 'panel',
            margins: '3 0 3 3',
            width: 400,
            autoScroll: true,
            frame: false,
            layout: 'anchor',
            items: gridlist
        }, {
            region: 'center',//中間
            xtype: 'panel',
            //width: 1000,
            //height: 1000,
            layout: 'fit',
            margins: '3 3 3 3',
            items: AuthView
        }],
        listeners: {
            resize: function () {
                Ext.getCmp("index").width = document.documentElement.clientWidth;
                Ext.getCmp("index").height = document.documentElement.clientHeight;
                this.doLayout();
            }
        },
        renderTo: Ext.getBody()
    });
});


var AuthView = Ext.create('Ext.view.View', {
    deferInitialRefresh: false,
    autoScroll: true,
    frame: false,
    store: historyDetailStore,
    tpl: Ext.create('Ext.XTemplate',
        '<div id="div_historyDetail" class="View">',
        '<ul class="ul-detail">',
            '<tpl for=".">',
                '<li>',
                    '<h2>' + TABLENAME + '：{table_name}   ' + PRIMARY_KEY + '：{pk_name}    ' + PRIMARY_KEY_VALUE + '：{pk_value}</h2>',
                    '<table id="tbl_{rowid}" class="tbl-cls" style="width:800px">',
                    '<tr><th style="width:100px">' + COLUMNSTEXT + '</th><th style="width:200px">' + COLUMNSTEXT_CHINA_NAME + '</th><th style="width:200px">' + EDIT_BEFOR + '</th><th style="200px">' + EDIT_AFTER + '</th><th style="40px">' + TYPE + '</th></tr>',
                        '<tpl for="historyItem">',
                            '<tr ><td>{col_name}</td><td>{col_chsname}</td><td>{old_value}</td><td>{col_value}</td><td>',
                        '<tpl if="type==0">' + HISTORY + '</tpl>',
                        '<tpl if="type==1">' + NEW + '</tpl>',
                        '</td></tr>',
                        '</tpl>',
                    '</table>',
                '</li>',
            '</tpl>',
        '</ul>',
        '</div>'
    ),
    itemSelector: 'li-detail'
});


var searchfrm = Ext.create('Ext.form.Panel', {
    id: 'searchfrm',
    layout: 'column',
    border: false,
    defaults: { margin: '2 2 2 2' },
    items: [{
        fieldLabel: TABLENAME,//表名
        id: 'table_name',
        name: 'table_name',
        labelWidth: 43,
        width: 230,
        xtype: 'combobox',
        editable: false,
        allowBlank: false,
        displayField: 'table_name',
        valueField: 'table_name',
        store: tableNameStore,
        listeners: {
            beforerender: function () {
                tableNameStore.load({
                    callback: function () {
                        Ext.getCmp("table_name").setValue(tableNameStore.data.items[0].data.table_name);
                    }
                });
            }
        }
    }, {
        xtype: 'splitter',
        width: 36
    }, {
        xtype: 'textfield',
        fieldLabel: PRODUCT_ID,//商品ID
        id: 'pk_value',
        name: 'pk_value',
        allowBlank: false,
        width: 260,
        labelWidth: 50,
        regex: /^[0-9,]+$/,
        enableKeyEvents: true,
        listeners: {
            keyup: function (e, event) {
                if (event.keyCode == 13) {
                    Search();
                }
            }
        }
    }, {
        xtype: 'splitter',
        width: 36
    }, {
        xtype: 'button',
        text: QUERY,//查 詢
        width: 60,
        border: true,
        handler: Search
    }]

})

var gridlist = Ext.create('Ext.grid.Panel', {
    id: 'gridlist',
    layout: 'anchor',
    autoScroll: true,
    border: false,
    frame: false,
    store: listStore,
    height: document.documentElement.clientHeight - 42,
    columns: [{ header: BATCH_NUMBER, dataIndex: 'batchno', align: 'left', width: 166, menuDisabled: true, sortable: false, flex: 1 },//批號
        { header: MODIFICATION_MAN, dataIndex: 'user_username', align: 'left', width: 65, menuDisabled: true, sortable: false },//修改人
        { header: TIME, dataIndex: 'kdate', align: 'left', width: 166, menuDisabled: true, sortable: false }],//時間
    bbar: Ext.create('Ext.PagingToolbar', {
        store: listStore,
        dock: 'bottom',
        pageSize: pageSize,
        displayInfo: true
    }),
    listeners: {
        itemclick: function (grid, record) {
            // Ext.get("iMain").dom.src = "/ProductHistory/HistoryDetails?batchno=" + record.data.batchno;
            
            historyDetailStore.load({
                params: {
                    batchno: record.data.batchno
                }
            })
        }

    }

})


//查詢
function Search() {
    if (!Ext.getCmp('pk_value').isValid()) return;
    historyDetailStore.removeAll();//eidt by wwei0216w 2015/6/24 清空右邊相關歷史記錄
    listStore.removeAll();
    listStore.loadPage(1);
}


