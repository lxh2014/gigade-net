var pageSize = 25;

Ext.define('GIGADE.IpoNvd', {
    extend: 'Ext.data.Model',
    fields: [
        { name: 'row_id', type: 'int' }, //工作單號
        { name: 'work_id', type: 'string' }, //工作單號
        { name: 'ipo_id', type: 'string' }, //採購單編號
        { name: 'item_id', type: 'string' }, //商品細項編號
        { name: 'loc_id', type: 'string' }, //商品主料位
        { name: 'ipo_qty', type: 'string' },//採購單驗收數量
        { name: "out_qty", type: "string" },//未收貨上架數量
        { name: "com_qty", type: "int" },//完成收貨上架數量
        { name: "cde_dt", type: 'date', dateFormat: "Y-m-d H:i:s" },//有效日期
        { name: "made_date", type: 'date', dateFormat: "Y-m-d H:i:s" },//製造日期
        { name: "work_status", type: "string" },//收貨上架狀態 
        { name: "create_username", type: 'string' },  //創建人
        { name: "create_datetime", type: 'string' },  //創建時間
        { name: "modify_username", type: 'string' },  //修改人
        { name: "modify_datetime", type: 'string' },  //修改時間

    ]
});

var IpoNvd_Store = Ext.create('Ext.data.Store', {
    model: 'GIGADE.IpoNvd',
    pageSize: pageSize,
    // autoLoad: true,//自動加載
    autoDestroy: true,
    proxy: {
        type: 'ajax',
        url: '/ReceiptShelves/GetIpoNvdList',
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
    { "txt": '全部', "value": "all_type" },
    { "txt": '採購單編號', "value": "ipo_id" },
    { "txt": '商品細項編號', "value": "item_id" },
    ]
});


IpoNvd_Store.on('beforeload', function ()
{
    Ext.apply(IpoNvd_Store.proxy.extraParams,
        {
            search_type: Ext.getCmp("search_type").getValue(),
            search_con: Ext.getCmp("search_con").getValue(),
            start_time: Ext.getCmp('start_time').getValue() == null ? null : Ext.Date.format(new Date(Ext.getCmp('start_time').getValue()), 'Y-m-d H:i:s'),
            end_time: Ext.getCmp('end_time').getValue() == null ? null : Ext.Date.format(new Date(Ext.getCmp('end_time').getValue()), 'Y-m-d H:i:s'),
            work_id: 'empty', //僅允許查詢work_id為""的記錄
        });
});
//前面選擇框 選擇之後顯示快速結單
var sm = Ext.create('Ext.selection.CheckboxModel', {
    listeners: {
        selectionchange: function (sm, selections)
        {
            Ext.getCmp("IpoNvd_grid").down('#addjob').setDisabled(selections.length == 0);
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
                         value: "all_type",
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
                        fieldLabel: '創建時間',
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

    var IpoNvd_grid = Ext.create('Ext.grid.Panel', {
        id: 'IpoNvd_grid',
        store: IpoNvd_Store,
        width: document.documentElement.clientWidth,
        height: document.documentElement.clientHeight - 100,
        columnLines: true,
        frame: true,
        columns: [
            //new Ext.grid.RowNumberer(),//自動顯示行號

         { header: "工作單號", dataIndex: "work_id", width: 150, align: 'center', hidden:true },
         { header: "採購單編號", dataIndex: "ipo_id", width: 150, align: 'center' },
         { header: "商品細項編號", dataIndex: "item_id", width: 100, align: 'center' },
         { header: '商品主料位', dataIndex: 'loc_id', width: 100, align: 'center' },
         { header: "採購單驗收數量", dataIndex: "ipo_qty", width: 100, align: 'center' },
         { header: "未收貨上架數量", dataIndex: "out_qty", width: 100, align: 'center' },
         { header: "完成收貨上架數量", dataIndex: "com_qty", width: 100, align: 'center' },
         {
             header: "有效日期", dataIndex: "cde_dt", width: 150, align: 'center',
             renderer: Ext.util.Format.dateRenderer('Y-m-d')
             
         },
         {
             header: "製造日期", dataIndex: "made_date", width: 150, align: 'center',
             renderer: Ext.util.Format.dateRenderer('Y-m-d')
         },
         {
             header: "收貨上架狀態", dataIndex: "work_status", width: 100, align: 'center',
             renderer: function (value)
             {
                 if (value == "AVL")
                 {
                     return Ext.String.format('未處理');
                 }
                 else if (value == "SKP")
                 {
                     return Ext.String.format('已處理但未完成');
                 }
                 else if (value == "COM")
                 {
                     return Ext.String.format('已完成');
                 } 
             }
         },
         { header: "創建人", dataIndex: "create_username", width: 100, align: 'center' },
         { header: "創建時間", dataIndex: "create_datetime", width: 150, align: 'center' },
         { header: "修改人", dataIndex: "modify_username", width: 100, align: 'center' },
         { header: "修改時間", dataIndex: "modify_datetime", width: 150, align: 'center' },
        
        ],
        tbar: [
            {
                xtype: 'button', text: '生成收貨上架工作編號', id: 'addjob', iconCls: 'icon-user-edit', disabled: true, handler: GetJobMsg
            }
        ],
        bbar: Ext.create('Ext.PagingToolbar', {
            store: IpoNvd_Store,
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
        items: [Searchform, IpoNvd_grid],
        autoScroll: true,
        renderTo: Ext.getBody(),
        listeners: {
            resize: function ()
            {
                IpoNvd_grid.width = document.documentElement.clientWidth;
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
    IpoNvd_Store.removeAll();
    search_type = Ext.getCmp("search_type").getValue();
    search_con = Ext.getCmp("search_con").getValue();
    if (search_type == "ipo_id" && search_con.trim()=="")
    {
        Ext.Msg.alert(INFORMATION, '請輸入採購單編號');
        return false;
    }
    else if (search_type == "item_id" && search_con.trim() == "")
    {
        Ext.Msg.alert(INFORMATION, '請輸入商品細項編號');
        return false;
    }

    if (search_con.trim() == "" && Ext.getCmp('start_time').getValue() == null && Ext.getCmp('end_time').getValue() == null)
    {
        Ext.Msg.alert(INFORMATION, '請先輸入查詢條件');
        return false;
    }

    Ext.getCmp('IpoNvd_grid').store.loadPage(1, {
        params: {

        }
    });
}
function GetJobMsg()
{
    var row = Ext.getCmp("IpoNvd_grid").getSelectionModel().getSelection();
    if (row.length <= 0)
    {
        Ext.Msg.alert("未選中任何行!");
    }

    else
    {
        //  var id = Ext.getCmp('id').getValue();
        Ext.Msg.confirm('提示', Ext.String.format("確定將選中的" + row.length + "條數據生產工作單?", row.length), function (btn)
        {
            if (btn == 'yes')
            {
                var rowIDs = '';
                for (var i = 0; i < row.length; i++)
                {
                    rowIDs += row[i].data.row_id + ',';//
                }
                Ext.Ajax.request({
                    url: '/ReceiptShelves/CreateTallyList',
                    method: 'post',
                    params: { id: rowIDs },
                    success: function (form, action)
                    {
                        var result = Ext.decode(form.responseText);
                        if (result.success)
                        {
                            if (result.work_id != null && result.work_id != "")
                            {
                                Ext.Msg.alert(INFORMATION, "生產工作單成功!工作單號為:" + result.work_id);
                            }
                            IpoNvd_Store.load();
                        }
                        else
                        {
                            Ext.Msg.alert(INFORMATION, "生產工作單失敗!");
                            IpoNvd_Store.load();
                        }
                    },
                    failure: function ()
                    {
                        Ext.Msg.alert(INFORMATION, "生產工作單失敗!");
                        IpoNvd_Store.load();
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
