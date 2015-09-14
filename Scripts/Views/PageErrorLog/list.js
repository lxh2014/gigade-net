

var PageSize = 25;
/*********參數表model***********/
Ext.define("gigade.paraModel", {
    extend: 'Ext.data.Model',
    fields: [
        { name: 'parameterName', type: 'string' },
        { name: 'parameterCode', type: 'string' }
    ]
});

//類型
var errorStore = Ext.create("Ext.data.Store", {
    model: 'gigade.paraModel',
    autoLoad: true,
    proxy: {
        type: 'ajax',
        url: '/PageErrorLog/QueryPara?paraType=page_error_type',
        noCache: false,
        getMethod: function () { return 'get'; },
        actionMethods: 'post',
        reader: {
            type: 'json',
            root: 'items'
        }
    }
});


//列表Model
Ext.define('gigade.PageError', {
    extend: 'Ext.data.Model',
    fields:
        [
            { name: "rowID", type: "int" },
            { name: "error_page_url", type: "string" },
            { name: "errorName", type: "string" },
            { name: "create_date", type: "string" },
            { name: "create_ip", type: "string" }
        ]
});
//到Controller獲取數據
var Liststore = Ext.create('Ext.data.Store', {
    autoDestroy: true, //自動消除
    pageSize:PageSize,//每頁最大數據,傳到前臺 
    model: 'gigade.PageError',
    proxy: {
        type: 'ajax',
        url: '/PageErrorLog/GetList',
        reader: {
            type: 'json',
            root: 'data',
            totalProperty: 'totalCount'//總行數
        }
    }
});
Ext.onReady(function () {

    ///創建form.Panel;放置搜索條件
    var formPanel=  Ext.create('Ext.form.Panel', {
        id: 'frm',
        layout: 'anchor',
        height: 110,
        border: 0,
        bodyPadding: 10,
        width: document.documentElement.clientWidth-50,
        items:
            [
               //----------------------时间搜索-------------------
              {
                  xtype: 'fieldcontainer',
                  combineErrors: true,
                  layout: 'hbox',
                  items:
                      [
                         {
                             xtype: 'datetimefield',
                             id: 'startTime',
                             name: 'startTime',
                             fieldLabel: "時間範圍",
                             margin: '0 5',
                             labelWidth: 60,
                             width: 250,
                             format: 'Y-m-d H:i:s',
                             time: { hour: 00, min: 00, sec: 00 },//標記結束時間00:00:00
                             editable: false,
                             value: setNextMonth(Date.now(), -1),
                             listeners: {
                                 select: function (a, b, c) {
                                     var startTime = Ext.getCmp("startTime");
                                     var endTime = Ext.getCmp("endTime");
                                     if (endTime.getValue() == null) {
                                         endTime.setValue(setNextMonth(startTime.getValue(), 1));
                                     } else if (startTime.getValue() > endTime.getValue()) {
                                         Ext.Msg.alert(INFORMATION, "開始時間不能大於結束時間");
                                         endTime.setValue(setNextMonth(startTime.getValue(), 1));
                                     }
                                     else if (endTime.getValue() > setNextMonth(startTime.getValue(), 1)) {
                                         endTime.setValue(setNextMonth(startTime.getValue(), 1));
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
                            width: 205,
                            margin: '0 3',
                            format: 'Y-m-d H:i:s',
                            editable: false,
                            time: { hour: 23, min: 59, sec: 59 },//標記結束時間23:59:59    
                            value: setNextMonth(Date.now(), 0),
                            listeners: {
                                select: function (a, b, c) {
                                    var startTime = Ext.getCmp("startTime");
                                    var endTime = Ext.getCmp("endTime");
                                    var s_date = new Date(startTime.getValue());
                                    var now_date = new Date(endTime.getValue());
                                    if (startTime.getValue() != "" && startTime.getValue() != null) {
                                        if (endTime.getValue() < startTime.getValue()) {
                                            Ext.Msg.alert(INFORMATION, "結束時間不能小於開始時間");
                                            endTime.setValue(setNextMonth(startTime.getValue(), 1));
                                        } else if (endTime.getValue() > setNextMonth(startTime.getValue(), 1)) {
                                            startTime.setValue(setNextMonth(endTime.getValue(), -1));
                                        }

                                    } else {
                                        startTime.setValue(setNextMonth(endTime.getValue(), -1));
                                    }
                                },
                                specialkey: function (field, e) {
                                    if (e.getKey() == e.ENTER) {
                                        Query();
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
                         //-----------------------搜索類型------------------
                           {
                               xtype: 'combobox',
                               id: 'error_type',
                               margin: '0 5px',
                               fieldLabel: '錯誤類型',
                               labelWidth: 60,
                               width: 250,
                               colName: 'deliver_type',
                               queryMode: 'local',
                               editable: false,
                               lastQuery: '',
                               store: errorStore,
                               displayField: 'parameterName',
                               valueField: 'parameterCode',
                               value: "0",
                               listeners: {
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
                                    fieldLabel: '關鍵字',
                                    labelWidth: 40,
                                    width: 250,
                                    id: 'search',
                                    colName: 'search',
                                    margin: '0 5px',
                                    submitValue: false,
                                    name: 'searchCode',
                                    listeners: {
                                        specialkey: function (field, e) {
                                            if (e.getKey() == e.ENTER) {
                                                Query();
                                            }
                                        }
                                    }
                                },
                        ]
              },
                 //--------------------查詢------重置------------------
                  {
                      xtype: 'fieldcontainer',
                      combineErrors: true,
                      layout: 'hbox',
                      align: 'center',
                      items: [
                                //--------------------查詢-----------------------------------
                                {
                                    xtype: 'button',
                                    iconCls: 'icon-search',
                                    text: "查詢",
                                    margin: '0 8 0 105',
                                    handler: Query
                                },
                                 {
                                     xtype: 'displayfield',
                                     width: 48
                                 },
                                 //----------------------重置-------------------------------
                                 {
                                     xtype: 'button',
                                     text: '重置',
                                     id: 'btn_reset',
                                     iconCls: 'ui-icon ui-icon-reset',
                                     listeners: {
                                         click: function () {
                                             Ext.getCmp("error_type").setValue(0);
                                             Ext.getCmp("search").setValue("");
                                             Ext.getCmp("startTime").setValue(setNextMonth(Date.now(), -1));
                                             Ext.getCmp("endTime").setValue(setNextMonth(Date.now(), 0));
                                         }
                                     }
                                 }
                               ]
                  }
      ]
           
     });

    ///頁面加載的時候創建grid.Panel
    var ListPanl = Ext.create('Ext.grid.Panel', {
        title: '錯誤列表',
        id:'ListPanl',
        autoHeight: true,
        //autoWidth: true,
        columnLines: true,
        flex:8.1,
        store: Liststore,
        columns: [
            { header: '流水編號', dataIndex: 'rowID', flex: 1, align: 'center' },
            {
                header: '錯誤頁面地址',
                dataIndex: 'error_page_url',
                flex:1,
                align: 'center',
            },
            { header: '錯誤類型', dataIndex: 'errorName', flex:2, align: 'center' },
            { header: '訪問時間', dataIndex: 'create_date', flex: 1, align: 'center' },
            { header: '訪問IP', dataIndex: 'create_ip', flex: 1, align: 'center' },
       
        ],
        bbar: Ext.create('Ext.PagingToolbar', {
            store: Liststore,
            pageSize: PageSize,
            displayInfo: true,
            displayMsg: NOW_DISPLAY_RECORD + ': {0} - {1}' + TOTAL + ': {2}',
            emptyMsg: NOTHING_DISPLAY
        }),
    });
    Ext.create('Ext.container.Viewport', {
        layout: 'vbox',
        items: [formPanel,ListPanl],
        renderTo: Ext.getBody(),
        listeners: {
            resize: function () {
                ListPanl.width = document.documentElement.clientWidth;
                ListPanl.height = document.documentElement.clientHeight - 110;
                this.doLayout();
            }
        }
    });
});

///解決EXT下一頁的時候傳參失敗的問題
Liststore.on('beforeload', function () {
    Ext.apply(Liststore.proxy.extraParams, {
        errorType: Ext.getCmp("error_type").getValue(),//錯誤類型
        searchKey: Ext.getCmp("search").getValue(),//搜索關鍵字
        startT: Ext.getCmp("startTime").getValue(),//開始時間
        endT: Ext.getCmp("endTime").getValue()//結束時間
    });
});
///搜索-Query
function Query() {
    Liststore.removeAll();
    Ext.getCmp("ListPanl").store.loadPage(1, {
        params: {
            errorType: Ext.getCmp("error_type").getValue(),//錯誤類型
            searchKey: Ext.getCmp("search").getValue(),//搜索關鍵字
            startT: Ext.getCmp("startTime").getValue(),//開始時間
            endT: Ext.getCmp("endTime").getValue()//結束時間
        }
    });
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
    else {
        s.setHours(23, 59, 59);
    }
    return s;
}
