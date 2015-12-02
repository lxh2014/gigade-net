var pageSize = 25;

Ext.define('GIGADE.Aseld', {
    extend: 'Ext.data.Model',
    fields: [
        { name: 'seld_id', type: 'int' }, //流水号
        { name: 'ord_id', type: 'string' }, //訂單號
        { name: 'deliver_code', type: 'string' }, //出貨單號
        { name: 'cust_name', type: 'string' },//收件人
        { name: "productname", type: "string" },//品名
        { name: "out_qty", type: "int" },//撿貨數量
        { name: "sel_loc", type: "string" },//料位
        { name: "item_id", type: "string" },//細項編號
        { name: "note_order", type: 'string' },  //備註 
        { name: "description", type: 'string' },  //
        { name: "prod_sz", type: 'string' },  //

    ]
});

var Aseld_Store = Ext.create('Ext.data.Store', {
    model: 'GIGADE.Aseld',
    pageSize: pageSize,
    // autoLoad: true,//自動加載
    autoDestroy: true,
    proxy: {
        type: 'ajax',
        url: '/MarketTally/GetAllAseldList',
        actionMethods: 'post',
        reader: {
            type: 'json',
            root: 'data',
            totalProperty: 'totalCount'
        }
    }
});

var SearchTypeStore = Ext.create('Ext.data.Store', {
    fields: ['txt', 'value'],
    data: [
    { "txt": '工作代號', "value": "assg_id" },
    { "txt": '細項編號', "value": "item_id" },
    { "txt": '訂單號', "value": "ord_id" },
    { "txt": '出貨單號', "value": "deliver_code" },

    ]
});


//加載的時候得到數據 (排程日誌查詢) 
Aseld_Store.on('beforeload', function ()
{
    Ext.apply(Aseld_Store.proxy.extraParams,
        {
            search_type: Ext.getCmp("search_type").getValue(),
            search_con: Ext.getCmp("search_con").getValue(),
            start_time: Ext.getCmp('start_time').getValue() == null ? null : Ext.Date.format(new Date(Ext.getCmp('start_time').getValue()), 'Y-m-d H:i:s'),
            end_time: Ext.getCmp('end_time').getValue() == null ? null : Ext.Date.format(new Date(Ext.getCmp('end_time').getValue()), 'Y-m-d H:i:s'),
        });
});
//前面選擇框 選擇之後顯示快速結單
var sm = Ext.create('Ext.selection.CheckboxModel', {
    listeners: {
        selectionchange: function (sm, selections)
        {
            Ext.getCmp("Aseld_grid").down('#automarkettally').setDisabled(selections.length == 0);
        }
    }
});
//列表頁加載
Ext.onReady(function ()
{
    var Searchform = Ext.create('Ext.form.Panel', {
        id: 'Searchform',
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
                         xtype: 'combobox',
                         fieldLabel: '查詢條件',
                         margin: '0 5 0 5',
                         labelWidth: 60,
                         width: 200,
                         id: 'search_type',
                         editable: false,
                         displayField: 'txt',
                         valueField: 'value',
                         value: "assg_id",
                         store: SearchTypeStore,
                     },
                     {
                         xtype: 'textfield',
                         //fieldLabel: KEY,
                         labelWidth: 60,
                         margin: '0 5 0 5',
                         id: 'search_con',
                         name: 'search_con',
                         value: '',
                         width: 150,
                         listeners: {
                             specialkey: function (field, e)
                             {
                                 if (e.getKey() == Ext.EventObject.ENTER)
                                 {
                                     Query();
                                 }
                             },
                             focus: function ()
                             {
                                 var search_type = Ext.getCmp("search_type").getValue();
                                 if (search_type == null || search_type == '')
                                 {
                                     Ext.Msg.alert("提示信息", "請先選則查詢類型");
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
                        xtype: 'datefield',
                        margin: '0 2 0 5',
                        labelWidth: 60,
                        width: 200,
                        id: 'start_time',
                        format: 'Y-m-d',
                        fieldLabel: '日期條件',
                        //value: Tomorrow(1 - new Date().getDate()),
                        editable: false,
                        listeners: {
                            select: function (a, b, c)
                            {
                                var start = Ext.getCmp("start_time");
                                var end = Ext.getCmp("end_time");
                                var s_date = new Date(start.getValue());
                                end.setValue(new Date(s_date.setMonth(s_date.getMonth() + 1)));
                            },
                            specialkey: function (field, e)
                            {
                                if (e.getKey() == Ext.EventObject.ENTER)
                                {
                                    Query();
                                }
                            }
                        }
                    },
                    {
                        xtype: 'displayfield',
                        margin: '0 0 0 0',
                        value: "~"
                    },
                    {
                        xtype: 'datefield',
                        id: 'end_time',
                        format: 'Y-m-d',
                        margin: '0 5 0 3',
                        //labelWidth: 15,
                        // width: 210,
                        editable: false,
                        //value: Tomorrow(0),
                        listeners: {
                            select: function (a, b, c)
                            {
                                var start = Ext.getCmp("start_time");
                                var end = Ext.getCmp("end_time");
                                var s_date = new Date(start.getValue());
                                var end_date = new Date(end.getValue());
                                if (start.getValue() == null)
                                {
                                    start.setValue(new Date(end_date.setMonth(end_date.getMonth() - 1)));
                                }
                                
                                if (end.getValue() < start.getValue())
                                {
                                    Ext.Msg.alert("提示", "開始時間不能大於結束時間！");
                                    end.setValue(new Date(s_date.setMonth(s_date.getMonth() + 1)));
                                }
                            },
                            specialkey: function (field, e)
                            {
                                if (e.getKey() == Ext.EventObject.ENTER)
                                {
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
                        margin: '3 5 0 5',
                        text: '查詢',
                        iconCls: 'icon-search',
                        handler: Query
                    },
                    {
                        xtype: 'button',
                        text: '重置',
                        margin: '3 5 0 5',
                        iconCls: 'ui-icon ui-icon-reset',
                        handler: Reset
                        
                    }
                ]
            }

        ]
    });

    var Aseld_grid = Ext.create('Ext.grid.Panel', {
        id: 'Aseld_grid',
        store: Aseld_Store,
        width: document.documentElement.clientWidth,
        height: document.documentElement.clientHeight - 100,
        columnLines: true,
        frame: true,
        columns: [
            //new Ext.grid.RowNumberer(),//自動顯示行號
         { header: "流水号", dataIndex: "seld_id", width: 80, align: 'center' },
         { header: "訂單號", dataIndex: "ord_id", width: 150, align: 'center' },
         { header: "出貨單號", dataIndex: "deliver_code", width: 150, align: 'center' },
        { header: "收件人", dataIndex: "cust_name", width: 150, align: 'center' },
        { header: "細項編號", dataIndex: "item_id", width: 150, align: 'center' },
        {
            header: "品名",dataIndex: "productname", width: 300, align: 'center',
            renderer: function (value, cellmeta, record, rowIndex, columnIndex, store)
            {
                var prod = "";
                if (record.data.prod_sz != "")
                {
                    prod = "(" + record.data.prod_sz + ")";
                }
                return record.data.description + prod;
                
            }
        },
        { header: "撿貨數量", dataIndex: "out_qty", width: 150, align: 'center' },
        { header: "料位", dataIndex: "sel_loc", width: 150, align: 'center' },
        
        { header: "備註", dataIndex: "note_order", width: 150, align: 'center' },

        ],
        tbar: [
            {
                xtype: 'button', text: '快速結單', id: 'automarkettally', iconCls: 'icon-user-edit', disabled: true, handler: onAutoMarketTally
            }
        ],
        bbar: Ext.create('Ext.PagingToolbar', {
            store: Aseld_Store,
            pageSize: pageSize,
            displayInfo: true,//是否顯示數據信息
            displayMsg: NOW_DISPLAY_RECORD + ': {0} - {1}' + TOTAL + ': {2}',
            emptyMsg: NOTHING_DISPLAY
        }),
        listeners: {
            scrollershow: function (scroller)
            {
                if (scroller && scroller.scrollEl)
                {
                    scroller.clearManagedListeners();
                    scroller.mon(scroller.scrollEl, 'scroll', scroller.onElScroll, scroller);
                }
            }
        },
        selModel: sm
    });
    Ext.create('Ext.Viewport', {
        layout: 'vbox',
        items: [Searchform,Aseld_grid],
        autoScroll: true,
        renderTo: Ext.getBody(),
        listeners: {
            resize: function ()
            {
                Aseld_grid.width = document.documentElement.clientWidth;
                this.doLayout();
            }
        }
    });
})

/*************************************************************************************重置按鈕*************************************************************************************************/
function Reset()
{
    Ext.getCmp('search_type').reset();
    Ext.getCmp('search_con').reset();
    Ext.getCmp('start_time').reset();
    Ext.getCmp('end_time').reset();
}
/*************************************************************************************查询信息*************************************************************************************************/

function Query(x)
{
    Aseld_Store.removeAll();
    search_con = Ext.getCmp("search_con").getValue();

    if (search_con.trim() == "" && Ext.getCmp('start_time').getValue() == null && Ext.getCmp('end_time').getValue() == null)
    {
        Ext.Msg.alert(INFORMATION, '請先輸入查詢條件');
        return false;
    }

    Ext.getCmp('Aseld_grid').store.loadPage(1, {
        params: {

        }
    });
}
function onAutoMarketTally()
{
    var row = Ext.getCmp("Aseld_grid").getSelectionModel().getSelection();
    if (row.length <= 0)
    {
        Ext.Msg.alert("未選中任何行!");
    }

    else
    {
        //  var id = Ext.getCmp('id').getValue();
        Ext.Msg.confirm('提示', Ext.String.format("確定將選中的" + row.length + "條數據快速結單?", row.length), function (btn)
        {
            if (btn == 'yes')
            {
                var rowIDs = '';
                for (var i = 0; i < row.length; i++)
                {

                    rowIDs += row[i].data.seld_id + ',';//可以刪除多條數據記錄

                    //  rowIDs += row[i].data.id//刪除一條數據記錄

                    //Ext.Msg.alert(rowIDs);
                }
                Ext.Ajax.request({
                    url: '/MarketTally/RFAutoMarketTally',
                    method: 'post',
                    params: { id: rowIDs },
                    success: function (form, action)
                    {
                        var result = Ext.decode(form.responseText);
                        if (result.success)
                        {
                            Ext.Msg.alert(INFORMATION, "快速結單成功!");
                            // ScheduleStore.loadPage(1);
                            Aseld_Store.load();
                        }
                        else
                        {
                            Ext.Msg.alert(INFORMATION, "快速結單失敗!");
                            //ScheduleStore.loadPage(1);
                            Aseld_Store.load();
                        }
                    },
                    failure: function ()
                    {
                        Ext.Msg.alert(INFORMATION, "快速結單失敗!");
                        Aseld_Store.load();
                    }
                });
            }
        });
    }
}
/******************************************************************************************************************************************************************************************/
function Tomorrow(s)
{
    var d;
    d = new Date();                             // 创建 Date 对象。                               // 返回日期。
    d.setDate(d.getDate() + s);
    return d;
}
