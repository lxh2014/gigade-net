
var pageSize = 25;
//列表Model
Ext.define('gigade.MailSend', {
    extend: 'Ext.data.Model',
    fields:
        [
            { name: "rowid", type: "int" },
            { name: "mailfrom", type: "string" },
            { name: "mailto", type: "string" },
            { name: "subject", type: "string" },
            { name: "mailbody", type: "string" },
            { name: "status", type: "bool" },
            { name: "kuser", type: "string" },
            { name: "kdate", type: "string" },
            { name: "senddate", type: "string" },
            { name: "source", type: "string" },
            { name: "weight", type: "int" }
        ]
});
//到Controller獲取數據
var ListStore = Ext.create('Ext.data.Store', {
    autoDestroy: true, //自動消除
    pageSize: pageSize,//每頁最大數據,傳到前臺 
    model: 'gigade.MailSend',
    proxy: {
        type: 'ajax',
        url: '/MailSend/GetList',
        reader: {
            type: 'json',
            root: 'data',
            totalProperty: 'totalCount'//總行數
        }
    }
});
ListStore.on('beforeload', function () {
    //ListStore.removeAll();
    Ext.apply(ListStore.proxy.extraParams, {
        startT: Ext.getCmp('startTime').getValue(),//時間開始點
        endT: Ext.getCmp('endTime').getValue(),//時間結束點
        search: Ext.getCmp("search").getValue()//搜索關鍵字
    });
});

///搜索-Query
function Query() {
    ListStore.removeAll();
    var startTime = Ext.getCmp('startTime').getValue();//時間開始點
    var searchKey = Ext.getCmp("search").getValue();//搜索關鍵字
    if (startTime != null || searchKey != '') {
        Ext.getCmp("ListPanl").store.load();
    }
    else {
        Ext.Msg.alert("提示信息", "請選擇搜索條件");
    }
}
Ext.onReady(function () {
    ///頁面加載的時候創建grid.Panel
    var ListPanl = Ext.create('Ext.grid.Panel', {
        //title: '郵件內容',
        id: 'ListPanl',
        autoHeight: true,
        columnLines: true,
        flex: 8.1,
        store: ListStore,
        columns: [

            { header: '流水編號', dataIndex: 'rowid', flex: 1, align: 'center' },
            { header: '發件方', dataIndex: 'mailfrom', flex: 2, align: 'center' },
            { header: '收件方', dataIndex: 'mailto', flex: 2, align: 'center' },
            { header: '郵件主旨', dataIndex: 'subject', flex: 2, align: 'center' },
            {
                header: '郵件內容', dataIndex: 'mailbody', flex: 2, align: 'center',
                renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                    return Ext.String.format("<a href='javascript:void(0);' onclick='onStatusClick()'  >" + value + "</a>");
                }
            },
            ////發送狀態 數據庫中 0:未發送,1:已發送
            {
                header: "發送狀態", dataIndex: 'status', flex: 1, align: 'center',
                renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                    switch (record.data.status) {
                        case false:
                            return "未發送";
                            break;
                        case true:
                            return "已發送";
                            break;
                        default:
                            return "未知";
                    }
                }
            },
            { header: '輸入人員', dataIndex: 'kuser', flex: 2, align: 'center' },
            { header: '輸入時間', dataIndex: 'kdate', flex: 2, align: 'center' },
            { header: '發送時間', dataIndex: 'senddate', flex: 2, align: 'center' },
            { header: '郵件來源', dataIndex: 'source', flex: 2, align: 'center' },
            { header: '優先級', dataIndex: 'weight', flex: 1, align: 'center' }
        ],
        tbar: [
                ///使查詢框處於右側
              '->',
                //----------------------时间搜索-------------------
               {
                   xtype: 'datetimefield',
                   id: 'startTime',
                   name: 'startTime',
                   fieldLabel: "輸入時間區間",
                   margin: '0 5',
                   labelWidth: 80,
                   //width: 250,
                   format: 'Y-m-d H:i:s',
                   time: { hour: 00, min: 00, sec: 00 },//標記結束時間00:00:00
                   editable: false,
                   ////無默認開始時間
                   //value: setNextMonth(Date.now(), -1),
                   listeners: {
                       select: function (a, b, c) {
                           var startTime = Ext.getCmp("startTime");
                           var endTime = Ext.getCmp("endTime");
                           if (endTime.getValue() == null) {
                               endTime.setValue(setNextMonth(startTime.getValue(), 3));
                           } else if (startTime.getValue() > endTime.getValue()) {
                               Ext.Msg.alert(INFORMATION, "開始時間不能大於結束時間");
                               endTime.setValue(setNextMonth(startTime.getValue(), 3));
                           }
                           else if (endTime.getValue() > setNextMonth(startTime.getValue(), 3)) {
                               endTime.setValue(setNextMonth(startTime.getValue(), 3));
                           }
                       },
                       specialkey: function (field, e) {
                           if (e.getKey() == e.ENTER) {
                               Query();
                           }
                       }
                   }
               },
               {
                   xtype: 'displayfield',
                   value: '~',
                   margin: '0 10'
               },
               {
                   xtype: 'datetimefield',
                   id: 'endTime',
                   name: 'endTime',
                   //width: 205,
                   margin: '0 3',
                   format: 'Y-m-d H:i:s',
                   editable: false,
                   time: { hour: 23, min: 59, sec: 59 },//標記結束時間23:59:59  
                   ////無默認時間
                   //value: setNextMonth(Date.now(), 0),
                   listeners: {
                       select: function (a, b, c) {
                           var startTime = Ext.getCmp("startTime");
                           var endTime = Ext.getCmp("endTime");
                           var s_date = new Date(startTime.getValue());
                           var now_date = new Date(endTime.getValue());
                           if (startTime.getValue() != "" && startTime.getValue() != null) {
                               if (endTime.getValue() < startTime.getValue()) {
                                   Ext.Msg.alert(INFORMATION, "結束時間不能小於開始時間");
                                   endTime.setValue(setNextMonth(startTime.getValue(), 3));
                               }
                               else if (endTime.getValue() > setNextMonth(startTime.getValue(), 3)) {
                                   startTime.setValue(setNextMonth(endTime.getValue(), -3));
                               }
                           }
                           else {
                               startTime.setValue(setNextMonth(endTime.getValue(), -3));
                           }
                       },
                       specialkey: function (field, e) {
                           if (e.getKey() == e.ENTER) {
                               Query();
                           }
                       }
                   }
               },
            //--------------------搜索關鍵字--------------------
            {
                xtype: 'textfield',
                fieldLabel: '收件方/主旨',
                labelWidth: 80,
                id: 'search',
                colName: 'search',
                submitValue: false,
                name: 'searchCode',
                margin:"0 10 0 20",
                listeners: {
                    specialkey: function (field, e) {
                        if (e.getKey() == e.ENTER) {
                            Query();
                        }
                    }
                }
            },
            { xtype: 'button', text: '查詢', iconCls: 'icon-search', handler: Query },
            {
                xtype: 'button',
                text: '重置',
                iconCls: 'ui-icon ui-icon-reset',
                handler: function () {
                    Ext.getCmp('startTime').setValue('');
                    Ext.getCmp('endTime').setValue('');
                    Ext.getCmp('search').setValue('');
                }
            },
        ],
        bbar: Ext.create('Ext.PagingToolbar', {
            store: ListStore,
            pageSize: pageSize,
            displayInfo: true,
            displayMsg: "當前顯示記錄" + ': {0} - {1}' + "總計" + ': {2}',
            emptyMsg: "沒有記錄可以顯示"
        }),
    });
    Ext.create('Ext.container.Viewport', {
        layout: 'vbox',
        items: [ListPanl],
        renderTo: Ext.getBody(),
        listeners: {
            resize: function () {
                ListPanl.width = document.documentElement.clientWidth;
                ListPanl.height = document.documentElement.clientHeight;
                this.doLayout();
            }
        }
    });
    /////頁面首次加載load數據
    //ListStore.load({ params: { start: 0, limit: pageSize } });
});

///點擊事件
onStatusClick = function () {
    ///返回ListPanel的模型的當前被選中的記錄的數組
    var row = Ext.getCmp("ListPanl").getSelectionModel().getSelection();
    statusFunction(row[0], ListStore);
}
statusFunction = function (row, store) {
    var editFrm = Ext.create('Ext.form.Panel', {
        id: 'editFrm',
        frame: true,
        plain: true,
        constrain: true,
        autoScroll: true,
        layout: 'anchor',
        labelWidth: 45,
        defaults: { anchor: "95%", msgTarget: "side" },
        items: [
          {
              xtype: 'displayfield',
              //fieldLabel: '郵件內容',
              id: 'mailbody',
              name: 'mailbody'
          },
        ]
    })
    var editWin = Ext.create('Ext.window.Window', {
        iconCls: 'icon-user-edit',
        id: 'editWin',
        width: 600,
        height: 500,
        //maxWidth : 800,
        //maxHeight: 600,
        layout: 'fit',
        items: [editFrm],
        constrain: true,
        closeAction: 'destroy',
        modal: true,
        resizable: false,
        labelWidth: 60,
        bodyStyle: 'padding:5px 5px 5px 5px',
        closable: false,
        tools: [
        {
            type: 'close',
            qtip: '是否關閉',
            handler: function (event, toolEl, panel) {
                Ext.MessageBox.confirm(CONFIRM, IS_CLOSEFORM, function (btn) {
                    if (btn == "yes") {
                        Ext.getCmp('editWin').destroy();
                    }
                    else {
                        return false;
                    }
                });
            }
        }
        ],
        listeners: {
            'show': function () {
                editFrm.getForm().loadRecord(row);
            }
        }
    });
    editWin.show();
};
///解決EXT下一頁的時候傳參失敗的問題


setNextMonth = function (source, n) {
    var s = new Date(source);
    s.setMonth(s.getMonth() + n);
    if (n < 0) {
        s.setHours(0, 0, 0);
    }
    else if (n > 0) {
        s.setHours(23, 59, 59);
    }
    else {
        s.setHours(23, 59, 59);
    }
    return s;
}
