/*
 * Copyright (c)J01 
 * 作   者：yachao1120j
 * CreateTime :2015/8/24
 * 補貨通知統計
 */
var pageSize = 25;

// 列表頁的model
Ext.define('gridlistRIS', {
    extend: 'Ext.data.Model',
    fields: [
         { name: "id", type: "int" },//商品編號
        { name: "product_id", type: "int" },//商品編號
        { name: "product_name", type: "string" },//商品名稱
        { name: "item_id", type: "int" },//商品細項編號
        { name: "product_spec", type: "string" },//商品規格
        { name: "vendor_name_full_OR_vendor_id", type: "string" },//供應商名稱或編號
        { name: "vendor_name_full", type: "string" },//供應商名稱
        { name: "vendor_id", type: "int" },//供應商編號
        { name: "ri_nums", type: "int" },//補貨通知人數
        { name: "start_time", type: "int" },//開始時間
        { name: "end_time", type: "int" },//結束時間
        { name: "spec_title_1", type: "string" },//規格1
        { name: "spec_title_2", type: "string" },//規格2
    ],
});

//列表頁的數據源
var ArrNoticeStore = Ext.create('Ext.data.Store', {//ArrNoticeStore
    pageSize: pageSize,
    autoDestroy: true,
    model: 'gridlistRIS',
    proxy: {
        type: 'ajax',
        url: '/ProductPurchase/GetArrNoticeList',
        reader: {
            type: 'json',
            root: 'data',
            totalProperty: 'totalCount'
        }
    }
});

//定義彈窗的model
Ext.define('gridlistRis', {
    extend: 'Ext.data.Model',
    fields: [
        // { name: "id", type: "int" },//編號
        { name: "user_id", type: "int" },//用戶編號
        { name: "user_name", type: "string" },
        { name: "screate_time", type: "string" },
        { name: "user_status", type: "int" },
        { name: "sstatus", type: "string" },
         { name: "item_id", type: "int" },
         { name: "muser_name", type: "string" },
         { name: "ssend_notice_time", type: "string" },
         { name: "start_time", type: "int" },//開始時間
        { name: "end_time", type: "int" },//結束時間
        { name: "source_type", type: "int" },//訊息來源 1:來自前台  2:來自後台 默認是1user_status
       { name: "user_status", type: "int" }//狀態
    ]
});
//彈窗的數據源
var ArrUserStore = Ext.create('Ext.data.Store', {
    pageSize: pageSize,
    autoDestory: true,
    model: 'gridlistRis',
    proxy: {
        type: 'ajax',
        url: '/ProductPurchase/ShowArrByUserList',
        reader: {
            type: 'json',
            root: 'data',
            //totalProperty: 'totalCount',
        }
    }
});


//彈窗的 grid
var rislist = Ext.create('Ext.grid.Panel', {
    id: 'rislist',
    store: ArrUserStore,
    columnLines: true,
    autoScroll: true,
    border: 0,
    //viewConfig: {
    //    enableTextSelection: true,
    //    stripeRows: false,
    //    getRowClass: function (record, rowIndex, rowParams, store) {
    //        return "x-selectable";
    //    }
    //},
    columns: [
         new Ext.grid.RowNumberer(),//自動顯示行號
      
        { header: "用戶編號", dataIndex: "user_id", width: 80, align: 'center' },
        { header: "姓名", dataIndex: "muser_name", width: 80, menuDisabled: true, align: 'center' },
        {
            header: "信息來源", dataIndex: "source_type", width: 60, align: 'center',
            renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                if (value == 1)
                    return "來自前臺";
                else
                    return "後臺操作";
            }
        },
        { header: "創建時間", dataIndex: "screate_time", width: 100, align: 'center' },
        {
            header: "發送補貨通知時間", dataIndex: "ssend_notice_time", width: 130, align: 'center',
            renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                if (value.substr(0, 10) == "1970-01-01")
                    return "-";
                else
                    return value;
                
            }
        },
        { header: "狀態", dataIndex: "sstatus", width: 60, align: 'center' },
        {
            header: "操作", dataIndex: "user_status",flex: 1, align: 'center',
            renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                if (value == 0) {
                    return "<a href='javascript:void(0);' onclick='onReturnClick(" + record.data.item_id+","+ record.data.user_id + ")'>取消通知</a>";
                }

            }
        }
    ],
    //viewConfig: { emptyText: '<span>暫無數據！</span>' },
    listeners: {
        scrollershow: function (scroller) {
            if (scroller && scroller.scrollEl) {
                scroller.clearManagedListeners();
                scroller.mon(scroller.scrollEl, 'scroll', scroller.onElScroll, scroller);
            }
        }
    }
});
var groupAddWin = Ext.create('Ext.window.Window', {
    title: '補貨通知詳情',
    width: 660,
    height: document.documentElement.clientHeight-200,
    layout: 'fit',
    items: [rislist],
    closeAction: 'hide',
    modal: true,
    resizable: false,
    constrain: true,
    bodyStyle: 'padding:5px 5px 5px 5px'
});

//加載的時候得到數據 (點擊查看) 
ArrUserStore.on('beforeload', function () {
    Ext.apply(ArrUserStore.proxy.extraParams,
        {
        });
});

//加載的時候得到數據  
ArrNoticeStore.on('beforeload', function () {
    Ext.apply(ArrNoticeStore.proxy.extraParams,
        {
            vendor_name_full_OR_vendor_id: Ext.getCmp('vendor_name_full_OR_vendor_id').getValue(),
            product_id: Ext.getCmp('product_ids').getValue(),
            start_time: Ext.htmlEncode(Ext.Date.format(new Date(Ext.getCmp('start_time').getValue()), 'Y-m-d H:i:s')),
            end_time: Ext.htmlEncode(Ext.Date.format(new Date(Ext.getCmp('end_time').getValue()), 'Y-m-d H:i:s')),
        });
});

//頁面加載
Ext.onReady(function () {
    var searchForm = Ext.create('Ext.form.Panel', {
        id: 'searchForm',
        layout: 'anchor',
        border: 0,
        bodyPadding: 10,
        width: document.documentElement.clientWidth,
        items: [
               {
                   xtype: 'fieldcontainer',
                   layout: 'hbox',
                   items: [
                         {
                             xtype: 'textfield',
                             id: 'vendor_name_full_OR_vendor_id',
                             labelWidth: 100,
                             fieldLabel: '供應商編號/名稱',
                             margin: '0 0 0 10',
                             listeners: {
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
                   layout: 'hbox',
                   items: [
                    {
                        xtype: 'textfield',
                        id: 'product_ids',
                        margin: '0 0 0 10',
                        labelWidth: 60,
                        fieldLabel: '商品編號',
                        listeners: {
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
                layout: 'hbox',
                items: [
                      {
                          xtype: 'datefield',
                          margin: '0 0 0 10',
                          fieldLabel: '時間區間',
                          labelWidth: 60,
                          id: 'start_time',
                          format: 'Y-m-d',
                          value: Tomorrow(1 - new Date().getDate()),
                          editable: false,
                          listeners: {
                              select: function (a, b, c) {
                                  var start = Ext.getCmp("start_time");
                                  var end = Ext.getCmp("end_time");
                                  var s_date = new Date(start.getValue());
                                  end.setValue(new Date(s_date.setMonth(s_date.getMonth() + 1)));
                              },
                              specialkey: function (field, e) {
                                  if (e.getKey() == Ext.EventObject.ENTER) {
                                      Query();
                                  }
                              }
                          }
                      },
                      {
                          xtype: 'displayfield',
                          margin: '0 5 0 5',
                          value: '~'
                      },
                      {
                          xtype: 'datefield',
                          id: 'end_time',
                          format: 'Y-m-d',
                          value: Tomorrow(0),
                          editable: false,
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
            }
        ],

        buttonAlign: 'left',
        buttons: [
                {
                    text: '查詢',
                    // margin: '0 8 0 8',
                    margin: '0 10 0 10',
                    iconCls: 'icon-search',
                    handler: function () {
                        Query();
                    }
                },
                {
                    text: '重置',
                    iconCls: 'ui-icon ui-icon-reset',
                    handler: function () {
                        this.up('form').getForm().reset();
                    }
                }
        ]
    });
    //第二個panel
    var RISGrid = Ext.create('Ext.grid.Panel', {
        id: 'RISGrid',
        store: ArrNoticeStore,
        width: document.documentElement.clientWidth,
        columnLines: true,
        frame: true,
        flex: 9.4,

        columns: [
                { header: '商品編號', dataIndex: 'product_id' },
                { header: '商品名稱', dataIndex: 'product_name', width: 200 },
                { header: '商品細項編號', dataIndex: 'item_id' },
                { header: '商品規格', dataIndex: 'product_spec' },
                { header: '供應商編號', dataIndex: 'vendor_id' },
                { header: "供應商名稱", dataIndex: "vendor_name_full", width: 180 },
                { header: '補貨通知人數', dataIndex: 'ri_nums' },
                {
                    header: '補貨通知詳情', width: 120, align: 'center',
                    renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                        return "<a href='javascript:void(0);' onclick='onShowClick(" + record.data.item_id + ")'>點擊查看</a>";
                    }
                }
        ],
        tbar: [
           { xtype: 'button', text: "新增補貨通知", id: 'add', iconCls: 'ui-icon ui-icon-user-add', handler: onAddClick },
           { xtype: 'button', text: "取消通知", id: 'edit', iconCls: 'ui-icon ui-icon-user-edit',handler: onEditClick },
           { xtype: 'button', text: '匯出Excel', margin: '0 0 0 5', iconCls: 'icon-excel', id: 'btnExcel', handler: Export }

        ],
        bbar: Ext.create('Ext.PagingToolbar', {
            store: ArrNoticeStore,
            pageSize: pageSize,
            displayInfo: true,//是否顯示數據信息
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

        }//,
        //selModel: sm,
    });

    Ext.create('Ext.Viewport', {
        layout: 'vbox',
        items: [searchForm, RISGrid],// 包含两个控件 
        autoScroll: true,
        renderTo: Ext.getBody(),
        listeners: {
            resize: function () {
                RISGrid.width = document.documentElement.clientWidth;
                this.doLayout();
            }
        }
    });

});
//*********新增********//
onAddClick = function () {
    editFunction(null, ArrNoticeStore);
}

//*********編輯********//
onEditClick = function () {
        editFunction(1, ArrNoticeStore);
}
 function onReturnClick(item_id, user_id)
{
     Ext.MessageBox.confirm(CONFIRM, "確定要取消通知?", function (btn) {
         var startTime = Ext.getCmp('start_time').getValue();  // add 2015/8/28 yachao1120j
         var endTime = Ext.getCmp('end_time').getValue();
         if (btn == "yes") {
             Ext.Ajax.request({
                 url: '/ProductPurchase/SaveArrivaleNotice',
                 method: 'post',
                 params: {
                     item_id: item_id,
                     user_id: user_id,
                     id:3
                 },
                 success: function (form, action) {
                     Ext.Msg.alert(INFORMATION, SUCCESS);
                     ArrUserStore.load({ params: { item_id: item_id, startTime: startTime, endTime: endTime } });
                 },
                 failure: function () {
                     Ext.Msg.alert(INFORMATION, FAILURE);
                     ArrUserStore.load({ params: { item_id: item_id, startTime: startTime, endTime: endTime } });
                 }
             });
         } else {
             return false;
         }
     });
    
}

/************匯入到Exce************/
function Export() {
    var vendor_name_full_OR_vendor_id= Ext.getCmp('vendor_name_full_OR_vendor_id').getValue();
    var product_id= Ext.getCmp('product_ids').getValue();
    var start_time= Ext.htmlEncode(Ext.Date.format(new Date(Ext.getCmp('start_time').getValue()), 'Y-m-d H:i:s'));
    var end_time = Ext.htmlEncode(Ext.Date.format(new Date(Ext.getCmp('end_time').getValue()), 'Y-m-d H:i:s'));
    window.open("/ProductPurchase/ExportCSV?vendor_name_full_OR_vendor_id=" + vendor_name_full_OR_vendor_id + "&product_id=" + product_id + "&start_time=" + Ext.Date.format(new Date(Ext.getCmp('start_time').getValue()), 'Y-m-d') + "&end_time=" + Ext.Date.format(new Date(Ext.getCmp('end_time').getValue()), 'Y-m-d'));
}
/******************************************************************************************************************************************************************************************/
function Tomorrow(s) {
    var d;
    d = new Date();                             // 创建 Date 对象。                               // 返回日期。
    d.setDate(d.getDate() + s);
    return d;
}

/*************************************************************************************查询信息*************************************************************************************************/

function Query(x) {
    Ext.getCmp('RISGrid').store.loadPage(1, {
        params: {

        }
    });
}
/*************************************************************************************點擊查看*************************************************************************************************/

onShowClick = function (item_id) {
    var startTime = Ext.getCmp('start_time').getValue();  // add 2015/8/28 yachao1120j
    var endTime = Ext.getCmp('end_time').getValue();
    ArrUserStore.load({ params: { item_id: item_id, startTime: startTime, endTime: endTime } });//點擊事件 得到 item_id  startTime endTime
    groupAddWin.show();

}