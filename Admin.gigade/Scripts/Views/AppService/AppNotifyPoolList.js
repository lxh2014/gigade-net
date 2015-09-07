/*
* 文件名稱 :AppNotifyPoolList.js
* 文件功能描述 :推播設定列表JS
* 版權宣告 :
* 開發人員 : 肖國棟
* 版本資訊 : 1.0
* 日期 : 2015.8.24
* 修改人員 :
* 版本資訊 : 
* 日期 : 
* 修改備註 : 
*/

var pagesize = 25

//定義列表的數據模型
Ext.define('gigade.AppNotifyPoolModel', {
    extend: 'Ext.data.Model',
    fields: [{ type: 'int', name: 'id' },
                { type: 'string', name: 'title' },
                { type: 'string', name: 'alert' },
                { type: 'string', name: 'url' },
                { type: 'string', name: 'to' },
                { type: 'string', name: 'endtime' },
                { type: 'string', name: 'starttime' },
                { type: 'int', name: 'notified' },
                { type: 'string', name: 'notifytime' }]
});




//加載Grid數據源
var gridShow = Ext.create('Ext.data.Store', {
    model: 'gigade.AppNotifyPoolModel',
    //分頁大小
    pageSize: pagesize,
    proxy: {
        type: 'ajax',
        url: '/AppService/GetAppNotifyPoolInfo',
        actionMethods: 'post',
        reader: { type: 'json', root: 'data', totalProperty: 'totalCount' }
    }
});
//數據源BeforeLoad
gridShow.on("beforeload", function () {
    gridShow.removeAll();
    //驗證查詢條件
    var timestartvalue = Ext.getCmp('dfstart').getValue();
    var timestartendvalue = Ext.getCmp('dfstartend').getValue();
    var timeendstartvalue = Ext.getCmp('dfendstart').getValue();
    var timeendendvalue = Ext.getCmp('dfendend').getValue();
    if (timestartvalue == null && timestartendvalue == null && timeendstartvalue == null && timeendendvalue == null) {
        Ext.Msg.alert(INFORMATION, SEARCHNULLTEXT);
        return false;
    }
    Ext.apply(gridShow.proxy.extraParams,
             {
                 timestart: timestartvalue,
                 timestartend: timestartendvalue,
                 timeendstart: timeendstartvalue,
                 timeendend: timeendendvalue
             });
});

//查詢按鈕事件
function btnSearchFn() {
    gridShow.removeAll();
    //驗證查詢條件
    var timestartvalue = Ext.getCmp('dfstart').getValue();
    var timestartendvalue = Ext.getCmp('dfstartend').getValue();
    var timeendstartvalue = Ext.getCmp('dfendstart').getValue();
    var timeendendvalue = Ext.getCmp('dfendend').getValue();
    if (timestartvalue == null && timestartendvalue == null && timeendstartvalue == null && timeendendvalue == null) {
        Ext.Msg.alert(INFORMATION, SEARCHNULLTEXT);
        return false;
    }
    //為Store數據集傳遞AJax參數
    Ext.getCmp("ShowGrid").store.loadPage(1, {
        params: {
            timestart: timestartvalue,
            timestartend: timestartendvalue,
            timeendstart: timeendstartvalue,
            timeendend: timeendendvalue
        }
    });
}
//定義開始時間container
var starttimecon = Ext.create('Ext.container.Container', {
    layout: {
        type: 'hbox',
        padding: '5 0 0 0'
    },
    width: 350,
    items: [{
        xtype: 'datefield',
        fieldLabel: VALID_START,
        name: 'dfstart',
        id: 'dfstart',
        format: 'Y-m-d',
        labelWidth: 90,
        width: 200,
        editable: false,
        listeners: {
            change: function () {
                Ext.getCmp("dfstartend").setMinValue(this.getValue());
                Ext.getCmp("dfendend").setMinValue(this.getValue());
                Ext.getCmp("dfendstart").setMinValue(this.getValue());
            }
        }
    }, {
        xtype: 'displayfield',
        value: '~ ',
        id: 'blp',
        disabled: true
    }, {
        xtype: 'datefield',
        name: 'dfstartend',
        id: 'dfstartend',
        format: 'Y-m-d',
        width: 120,
        editable: false,
        listeners: {
            change: function () {
                Ext.getCmp("dfstart").setMaxValue(this.getValue());
            }
        }
    }]
});
//定義結束時間container
var endtimecon = Ext.create('Ext.container.Container', {
    layout: {
        type: 'hbox',
        padding: '5 0 0 0'
    },
    width: 350,
    items: [{
        xtype: 'datefield',
        fieldLabel: VALID_END,
        name: 'dfendstart',
        id: 'dfendstart',
        format: 'Y-m-d',
        labelWidth: 90,
        width: 200,
        editable: false,
        listeners: {
            change: function () {
                Ext.getCmp("dfendend").setMinValue(this.getValue());
            }
        }
    }, {
        xtype: 'displayfield',
        value: '~ ',
        id: 'blp',
        disabled: true
    }, {
        xtype: 'datefield',
        name: 'dfendend',
        id: 'dfendend',
        format: 'Y-m-d',
        width: 120,
        editable: false,
        listeners: {
            change: function () {
                Ext.getCmp("dfstart").setMaxValue(this.getValue());
                Ext.getCmp("dfstartend").setMaxValue(this.getValue());
                Ext.getCmp("dfendstart").setMaxValue(this.getValue());
            }
        }
    }]
});
//加載到頁面上面
Ext.onReady(function () {
    //回撤鍵查詢
    document.body.onkeydown = function () {
        if (event.keyCode == 13) {
            $("#btnSearch").click();
        }
    };
    //創建人員查詢列表
    var GShow = Ext.create('Ext.grid.Panel', {
        id: 'ShowGrid',
        store: gridShow,
        width: document.documentElement.clientWidth,
        columnLines: true,
        frame: true,
        columns: [{ header: RID, dataIndex: 'id', width: 100, align: 'center', hidden: true },
             { text: XID, xtype: 'rownumberer', width: 40, align: 'center', menuDisabled: true, sortable: false },
            { text: TITLE, dataIndex: 'title', width: 150, align: 'center', menuDisabled: true, sortable: false },
            { text: ALERTTXT, dataIndex: 'alert', width: 150, align: 'center', menuDisabled: true, sortable: false },
            { text: URLTEXT, dataIndex: 'url', width: 150, align: 'center', menuDisabled: true, sortable: false },
            { text: TOTEXT, dataIndex: 'to', width: 150, align: 'center', menuDisabled: true, sortable: false },
            {
                text: VALID_START, dataIndex: 'starttime', width: 100, align: 'center', menuDisabled: true, sortable: false
            },
            {
                text: VALID_END, dataIndex: 'endtime', width: 100, align: 'center', menuDisabled: true, sortable: false
            },
             {
                 text: NOW_STATE, dataIndex: 'notified', width: 100, align: 'center', menuDisabled: true, sortable: false,
                 renderer: function (val) { return val == '0' ? BOXFOU : BOXSHI; }
             },
              {
                  text: NOTIFY_TIME, dataIndex: 'notifytime', width: 100, align: 'center', menuDisabled: true, sortable: false,
                  renderer: function (val) { return val == '1970-01-01' ? "" : val; }
              }
        ], dockedItems: [{
            dock: 'top',
            xtype: 'toolbar',
            items: [
              starttimecon,
              endtimecon,
             {
                 xtype: "button",
                 id: "btnSearch",
                 margin: "0 0 0 0",
                 width: 50,
                 text: SEARCHBTN,
                 iconCls: 'ui-icon ui-icon-search-2',
                 handler: btnSearchFn
             }, {
                 xtype: 'button',
                 text: REPEATBTN,
                 id: 'btn_reset',
                 iconCls: 'ui-icon ui-icon-reset',
                 listeners: {
                     click: function () {
                         Ext.getCmp('dfstart').reset(),
                          Ext.getCmp('dfstartend').reset(),
                          Ext.getCmp('dfendstart').reset(),
                         Ext.getCmp('dfendend').reset()
                     }
                 }
             }
            ]
        }],
        bbar: {
            dock: 'bottom',
            xtype: 'pagingtoolbar',
            store: gridShow,
            pageSize: 5,
            displayInfo: true,
            displayMsg: NOW_DISPLAY_RECORD + ': {0} - {1}' + TOTAL + ': {2}',
            emptyMsg: NOTHING_DISPLAY
        },
        tbar: [
           {
               xtype: 'button',
               labelWidth: 50,
               text: ADDBTN,
               iconCls: 'ui-icon ui-icon-user-add',
               xtype: 'button',
               handler: btnAdd
           }
        ],
        listeners: {
            scrollershow: function (scroller) {
                if (scroller && scroller.scrollEl) {
                    scroller.clearManagedListeners();
                    scroller.mon(scroller.scrollEl, 'scroll', scroller.onElScroll, scroller);
                }
            }
        }
    })
    Ext.create('Ext.container.Viewport', {
        layout: 'fit',
        items: [GShow],
        renderTo: Ext.getBody(),
        autoScroll: true,
        listeners: {
            resize: function () {
                GShow.width = document.documentElement.clientWidth;
                this.doLayout();
            }
        }
    });
    ToolAuthority();
});
//添加信息
function btnAdd() {
    SaveReport(null);
}