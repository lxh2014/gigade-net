
var pageSize = 25;
////表名數據
//var tableNameStore = Ext.create('Ext.data.Store', {
//    fields: ['table_name'],
//    data: [{'table_name':'comment_detail'}]
//});
//表名數據
var tableNameStore = Ext.create('Ext.data.Store', {
    fields: ['table_name'],
    autoLoad: true,
    proxy: {
        type: 'ajax',
        url: "/ProductComment/GetTableName",
        noCache: false,
        actionMethods: 'post',
        reader: {
            type: 'json',
            root: 'data'
        }
    }
});

//歷史記錄列表grid
var listStore = Ext.create('Ext.data.Store', {
    fields: ['pk_id', 'user_name', 'create_time'],
    pageSize: pageSize,
    //autoLoad: true,
    proxy: {
        type: 'ajax',
        url: '/ProductComment/GetChangeLogList',
        actionMethods: 'post',
        reader: {
            type: 'json',
            root: 'data',
            totalProperty: 'totalCount'
        }
    }
});
//歷史詳情
var historyDetailStore = Ext.create('Ext.data.Store', {
    fields: ['pk_id', 'change_table', 'tclModel'], //'change_field', 'field_ch_name', 'old_value', 'new_value'],
    proxy: {
        type: 'ajax',
        url: "/ProductComment/GetChangeLogDetailList",
        noCache: false,
        actionMethods: 'post',
        reader: {
            type: 'json',
            root: 'data'
        }
    }
});

listStore.on("beforeload", function ()
{
    Ext.apply(listStore.proxy.extraParams, {
        table_name: Ext.getCmp('table_name').getValue(),
        comment_id: Ext.getCmp('comment_id').getValue(),
        start_time: Ext.getCmp('date_one').getValue(),
        end_time: Ext.getCmp('date_two').getValue()
    })
})



Ext.onReady(function ()
{
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
            resize: function ()
            {
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
                    '<h2>主鍵值：{pk_id}   功能：{change_table}    </h2>',
                    '<table class="tbl-cls" style="width:800px">',
                    '<tr><th style="width:200px">欄位</th><th style="width:200px">欄位中文名稱</th><th style="width:200px">修改前</th><th style="200px">修改后</th></tr>',
                        '<tpl for="tclModel">',
                         '<tr ><td>{change_field}</td><td>{field_ch_name}</td><td>{old_value}</td><td>{new_value}</td>',
                        '</tr>',
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
    defaults: { margin: '5 5 5 5' },
    items: [{
        fieldLabel: '功能',
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
            beforerender: function ()
            {
                tableNameStore.load({
                    callback: function ()
                    {
                        Ext.getCmp("table_name").setValue(tableNameStore.data.items[0].data.table_name);
                    }
                });
            }
        }
    },
    {
        xtype: 'numberfield',
        allowBlank: true,
        id: 'comment_id',        
        name: 'comment_id',
        fieldLabel: '評價編號',
        labelWidth: 60,
        minValue: 1,
        listeners: {
                    specialkey: function (field, e) {
                        if (e.getKey() == Ext.EventObject.ENTER) {
                            Search();
                        }
                    }
                }
    },
        {
            xtype: 'displayfield',
            margin: '5 0 5 30',
            value: "創建時間："
        },
        {
            xtype: "datetimefield",
            width: 150,
            margin: '5 0 0 0',
            id: 'date_one',
            name: 'date_one',
            editable: false,
            format: 'Y-m-d H:i:s',
            time: { hour: 00, min: 00, sec: 00 },//開始時間00：00：00
            submitValue: true,
            listeners: {
                select: function (a, b, c) {
                    var start = Ext.getCmp("date_one");
                    var end = Ext.getCmp("date_two");
                    if (end.getValue() == null) {
                        end.setValue(setNextMonth(start.getValue(), 1));
                    } else if (end.getValue() < start.getValue()) {
                        Ext.Msg.alert(INFORMATION, DATA_TIP);
                        start.setValue(setNextMonth(end.getValue(), -1));
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
         xtype: "datetimefield",
         format: 'Y-m-d H:i:s',
         editable: false,
         time: { hour: 23, min: 59, sec: 59 },//開始時間00：00：00
         id: 'date_two',
         name: 'date_two',
         margin: '5 0 0 0',
         width: 150,
         listeners: {
             select: function (a, b, c) {
                 var start = Ext.getCmp("date_one");
                 var end = Ext.getCmp("date_two");
                 if (start.getValue() != "" && start.getValue() != null) {
                     if (end.getValue() < start.getValue()) {
                         Ext.Msg.alert(INFORMATION, DATA_TIP);
                         end.setValue(setNextMonth(start.getValue(), 1));
                     }
                 }
                 else {
                     start.setValue(setNextMonth(end.getValue(), -1));
                 }
             }
         }
    }, 
    {
        xtype: 'splitter',
        width: 36
    },
    {
        xtype: 'button',
        text: '查 詢',
        iconCls: 'icon-search',
        width: 60,
        border: true,
        handler: Search
    },
    {
        xtype: 'button',
        text: '重置',
        iconCls: 'ui-icon ui-icon-reset',
        width: 60,
        border: true,
        handler: function ()
        {
            Ext.getCmp('comment_id').setValue(null);
            Ext.getCmp('date_one').setValue(null);
            Ext.getCmp('date_two').setValue(null);
            
        }
    },
    {
        icon: '../../../Content/img/icons/excel.gif',
        xtype: 'button',
        text: '匯出',
        width: 60,
        border: true,
        handler: ExportCSV
    },
    ]

})

var gridlist = Ext.create('Ext.grid.Panel', {
    id: 'gridlist',
    layout: 'anchor',
    autoScroll: true,
    border: false,
    frame: false,
    store: listStore,
    height: document.documentElement.clientHeight - 42,
    columns: [{ header: '主鍵值', dataIndex: 'pk_id', align: 'left', width: 166, menuDisabled: true, sortable: false, flex: 1 },
        { header: '創建人', dataIndex: 'user_name', align: 'left', width: 65, menuDisabled: true, sortable: false },
        { header: '創建時間', dataIndex: 'create_time', align: 'left', width: 166, menuDisabled: true, sortable: false }],
    bbar: Ext.create('Ext.PagingToolbar', {
        store: listStore,
        dock: 'bottom',
        pageSize: pageSize,
        displayInfo: true
    }),
    listeners: {
        itemclick: function (grid, record)
        {
             historyDetailStore.removeAll();
             historyDetailStore.load({
                params: {
                    pk_id: record.data.pk_id,
                    create_time: record.data.create_time
                }
            })
        }

    }

})


//查詢
function Search()
{
    comment_id=Ext.getCmp('comment_id').getValue(),
    start_time=Ext.getCmp('date_one').getValue(),
    end_time=Ext.getCmp('date_two').getValue()
    if (comment_id == ("" || null) && start_time == ("" || null) && end_time == ("" || null))
    {
        Ext.Msg.alert(INFORMATION, "請輸入查詢條件");
        return;
    }

    Ext.getCmp("gridlist").store.loadPage(1, {
        params: {
            table_name: Ext.getCmp('table_name').getValue(),
            comment_id: Ext.getCmp('comment_id').getValue(),
            start_time: Ext.getCmp('date_one').getValue(),
            end_time: Ext.getCmp('date_two').getValue()
        }
    });
}
//匯出
function ExportCSV()
{
    comment_id = Ext.getCmp('comment_id').getValue(),
    start_time = Ext.getCmp('date_one').getValue(),
    end_time = Ext.getCmp('date_two').getValue()
    if (comment_id == ("" || null) && start_time == ("" || null) && end_time == ("" || null))
    {
        Ext.Msg.alert(INFORMATION, "請輸入查詢條件");
        return;
    }
    Ext.Ajax.request({
        url: "/ProductComment/ProductCommentLogExport",
        params: {
            table_name: Ext.getCmp('table_name').getValue(),
            comment_id: Ext.getCmp('comment_id').getValue(),
            start_time: Ext.getCmp('date_one').getValue(),
            end_time: Ext.getCmp('date_two').getValue()
        },
        success: function (response)
        {
            if (response.responseText.split(',')[0] == "true")
            {
                window.location.href = '../..' + response.responseText.split(',')[2] + response.responseText.split(',')[1];
            }
        }
    });
}
setNextMonth = function (source, n)
{
    var s = new Date(source);
    s.setMonth(s.getMonth() + n);
    if (n < 0)
    {
        s.setHours(0, 0, 0);
    }
    else if (n > 0)
    {
        s.setHours(23, 59, 59);
    }
    return s;
}


