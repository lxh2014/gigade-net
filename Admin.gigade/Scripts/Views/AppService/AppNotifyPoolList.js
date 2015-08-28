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
    Ext.apply(gridShow.proxy.extraParams,
             {
                 timestart: Ext.getCmp('dfstart').getValue(),
                 timeend: Ext.getCmp('dfend').getValue()
             });
});

//查詢按鈕事件
function btnSearchFn() {
    gridShow.removeAll();
    //為Store數據集傳遞AJax參數
    Ext.getCmp("ShowGrid").store.loadPage(1, {
        params: {
            timestart: Ext.getCmp('dfstart').getValue(),
            timeend: Ext.getCmp('dfend').getValue()
        }
    });
}
//加載到頁面上面
Ext.onReady(function () {
    //創建人員查詢列表
    var GShow = Ext.create('Ext.grid.Panel', {
        id: 'ShowGrid',
        store: gridShow,
        width: document.documentElement.clientWidth,
        columnLines: true,
        frame: true,
        columns: [{ header: RID, dataIndex: 'id', width: 100, align: 'center', hidden: true },
             { text: XID, xtype: 'rownumberer', width: 40, align: 'center', menuDisabled: true, sortable: false },
            { text: TITLE, dataIndex: 'title', width: 100, align: 'center', menuDisabled: true, sortable: false },
            { text: ALERTTXT, dataIndex: 'alert', width: 100, align: 'center', menuDisabled: true, sortable: false },
            { text: URLTEXT, dataIndex: 'url', width: 100, align: 'center', menuDisabled: true, sortable: false },
            { text: TOTEXT, dataIndex: 'to', width: 100, align: 'center', menuDisabled: true, sortable: false },
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
        ],
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
           },
           '->',
       {
           xtype: 'datefield',
           fieldLabel: '&nbsp;&nbsp;' + VALID_START,
           name: 'dfstart',
           id: 'dfstart',
           format: 'Y-m-d',
           labelWidth: 90,
           width: 200,
           editable: false,
           dateRange: { begin: 'dfstart', end: 'dfend' },
           vtype: 'dateRange'
       }, {
           xtype: 'displayfield',
           value: '~ ',
           id: 'blp',
           disabled: true,
           margin: '0 0 0 5'
       }, {
           xtype: 'datefield',
           fieldLabel: '&nbsp;&nbsp;' + VALID_END,
           name: 'dfend',
           id: 'dfend',
           format: 'Y-m-d',
           labelWidth: 90,
           width: 200,
           editable: false,
           dateRange: { begin: 'dfstart', end: 'dfend' },
           vtype: 'dateRange'
       }, {
           xtype: "button",
           id: "btnSearch",
           margin: "0 0 0 5",
           width: 50,
           text: SEARCHBTN,
           iconCls: 'icon-search',
           handler: btnSearchFn
       }, {
           xtype: 'button',
           text: REPEATBTN,
           id: 'btn_reset',
           iconCls: 'ui-icon ui-icon-reset',
           listeners: {
               click: function () {
                   Ext.getCmp('dfstart').reset(),
                   Ext.getCmp('dfend').reset()
               }
           }
       }],
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
//自定义VTypes类型，验证日期范围  
Ext.apply(Ext.form.VTypes, {
    dateRange: function (val, field) {
        if (field.dateRange) {
            var beginId = field.dateRange.begin;
            this.beginField = Ext.getCmp(beginId);
            var endId = field.dateRange.end;
            this.endField = Ext.getCmp(endId);
            var beginDate = this.beginField.getValue();
            var endDate = this.endField.getValue();
        }
        if (beginDate != null && endDate != null) {
            if (beginDate <= endDate) {
                return true;
            } else {
                return false;
            }
        }
        else {
            return true;
        }
    },
    //验证失败信息  
    dateRangeText: REGXVALID_END
});
//添加信息
function btnAdd() {
    SaveReport(null);
}