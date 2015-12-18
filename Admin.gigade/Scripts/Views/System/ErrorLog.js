var pageSize = 30;
var startDate = "";
var endDate = "";
var docWidth = 0;
var docHeight = 0;
Ext.define('GIGADE.ERRORLOG', {
    extend: 'Ext.data.Model',
    fields: [
       { name: 'rowid', type: 'int' },
       { name: 'log_date', type: 'string' },
       { name: 'Thread', type: 'string' },
       { name: 'Level', type: 'string' },
       { name: 'logger', type: 'string' },
       { name: 'message', type: 'string' },
       { name: 'method', type: 'string' }
    ]
});


var errorLogStore = Ext.create('Ext.data.Store', {
    model: 'GIGADE.ERRORLOG',
    pageSize: pageSize,
    proxy: {
        type: 'ajax',
        url: '/System/QueryErrorLog',
        actionMethods: 'post',
        reader: {
            type: 'json',
            root: 'data',
            totalProperty: 'totalCount'
        }
    },
    listeners: {
    }
});

//定義ComboBox模型(級別)  add by zhuoqin0830w 2015/02/04
Ext.define('gigade.LEVEL', {
    extend: 'Ext.data.Model',
    fields: [{ type: 'string', name: 'Level' }]
});

//添加級別的store  add by zhuoqin0830w 2015/02/04
var levelStore = Ext.create('Ext.data.Store', {
    model: 'gigade.LEVEL',
    proxy: {
        type: 'ajax',
        url: '/System/GetLevel',
        actionMethods: 'post',
        reader: {
            type: 'json'
        }
    },
    autoLoad: true,
    remoteSort: true
});

errorLogStore.on('beforeload', function () {
    Ext.apply(errorLogStore.proxy.extraParams,
             {
                 startDate: Ext.Date.format(new Date(Ext.htmlEncode(Ext.getCmp('dateStart').getValue())), 'Y-m-d H:i:s'),
                 endDate: Ext.Date.format(new Date(Ext.htmlEncode(Ext.getCmp('dateEnd').getValue())), 'Y-m-d H:i:s'),
                 //獲取 級別 值  add by zhuoqin0830w 2015/02/05
                 level: Ext.getCmp('level').getValue(),
                 limit: pageSize
             });
});

//查詢
query = function () {
    var startDate = Ext.getCmp('dateStart').getValue();
    var endDate = Ext.getCmp('dateEnd').getValue();
    if (endDate) {
        if (startDate > endDate) {
            Ext.getCmp('dateStart').markInvalid(START_LARGER_THAN_END);
            return;
        }
    }
    Ext.getCmp('dateStart').clearInvalid();
    Ext.getCmp('errorLogGrid').store.loadPage(1);
}

//顯示詳細信息
showDetail = function (row, obj) {
    var detailPanel = Ext.create('Ext.form.Panel', {
        id: 'detailPanel',
        plain: true,
        autoScroll: true,
        defaults: { anchor: "95%" },
        bodyPadding: '5',
        labelAlign: 'top',
        items: [{
            xtype: 'displayfield',
            id: 'dis_log_date',
            fieldLabel: LABEL_TIME,
            labelStyle: 'font-weight:bold',
            style: { marginBottom: '10px', borderBottom: '1px solid #ced9e7', paddingBottom: '10px' }
        }, {
            xtype: 'displayfield',
            fieldLabel: LABEL_THREAD,
            labelStyle: 'font-weight:bold',
            name: 'Thread',
            style: { marginBottom: '10px', borderBottom: '1px solid #ced9e7', paddingBottom: '10px' }
        }, {
            xtype: 'displayfield',
            fieldLabel: LABEL_LEVEL,
            labelStyle: 'font-weight:bold',
            name: 'Level',
            style: { marginBottom: '10px', borderBottom: '1px solid #ced9e7', paddingBottom: '10px' }
        }, {
            xtype: 'displayfield',
            fieldLabel: LABEL_METHOD,
            labelStyle: 'font-weight:bold',
            name: 'method',
            style: { marginBottom: '10px', borderBottom: '1px solid #ced9e7', paddingBottom: '10px' }
        }, {
            xtype: 'displayfield',
            fieldLabel: LABEL_LOGGER,
            labelStyle: 'font-weight:bold',
            name: 'logger',
            style: { marginBottom: '10px', borderBottom: '1px solid #ced9e7', paddingBottom: '10px' }
        }, {
            xtype: 'textarea',
            fieldLabel: LABEL_MESSAGE,
            labelStyle: 'font-weight:bold',
            name: 'message',
            height: '150px'
        }]
    });

    docWidth = document.documentElement.clientWidth;
    docHeight = document.documentElement.clientHeight;

    var detailWin = Ext.create('Ext.window.Window', {
        width: docWidth / 3.1,
        minWidth: 500,
        height: docHeight - docHeight / 3,
        modal: true,
        resizable: false,
        constrain: true,
        iconCls: 'icon-view',
        closeAction: 'destroy',
        padding: 5,
        items: [detailPanel],
        layout: 'fit',
        listeners: {
            show: function () {
                var value = row.data.log_date;
                value = value.substring(value.lastIndexOf('(') + 1, value.lastIndexOf(')'));
                value = Ext.Date.format(new Date(eval(value)), 'Y-m-d H:i');
                Ext.getCmp('dis_log_date').setValue(value);
                detailPanel.loadRecord(row);
            }
        }
    });

    detailWin.show(obj);
}

//renderer: function (value) {
//    value = value.substring(value.lastIndexOf('(') + 1, value.lastIndexOf(')'));
//    return Ext.Date.format(new Date(eval(value)), 'Y-m-d H:i');
//}

Ext.onReady(function () {

    var errorLogGrid = Ext.create('Ext.grid.Panel', {
        id: 'errorLogGrid',
        store: errorLogStore,
        width: document.documentElement.clientWidth,
        columns: [{ header: NO, xtype: 'rownumberer', width: 38, align: 'center' },
                  {
                      header: DETAIL_SHOW, xtype: 'actioncolumn', width: 50, align: 'center',
                      items: [{
                          icon: '/Content/img/icons/application_view_list.png',
                          iconCls: 'icon-cursor',
                          tooltip: DETAIL_INFO,
                          handler: function (grid, rowIndex, colIndex) {
                              var rec = grid.getStore().getAt(rowIndex);
                              showDetail(rec, this);
                          }
                      }]
                  },
                  {
                      header: LOG_DATE, dataIndex: 'log_date', width: 150, align: 'center', renderer: function (value) {
                          value = value.substring(value.lastIndexOf('(') + 1, value.lastIndexOf(')'));
                          value = Ext.Date.format(new Date(eval(value)), 'Y-m-d H:i');
                          return value;
                      }
                  },
                  { header: THREAD, dataIndex: 'Thread', width: 50, align: 'center' },
                  { header: LEVEL, dataIndex: 'Level', width: 66, align: 'center' },
                  { header: METHOD, dataIndex: 'method', width: 150, align: 'center' },
                  { header: LOGGER, dataIndex: 'logger', width: 250, align: 'left' },
                  { header: MESSAGE, dataIndex: 'message', width: 300, align: 'left', flex: 1 }
        ],
        tbar: [{
            xtype: 'datetimefield',
            editable: false,
            fieldLabel: TIME_BETWEEN,
            labelWidth: 65,
            format: 'Y-m-d H:i:s',
            time: { hour: 00, min: 00, sec: 00 },
            id: 'dateStart',
            listeners: {
                select: function (a, b, c) {
                    var start = Ext.getCmp("dateStart");
                    var end = Ext.getCmp("dateEnd");
                    var start_date = start.getValue();
                    if (end.getValue() == "") {
                        Ext.getCmp('dateEnd').setValue(new Date(start_date.getFullYear(), start_date.getMonth() + 1, start_date.getDate(), 23, 59, 59));
                    }
                   else if (end.getValue() < start.getValue()) {
                        Ext.getCmp('dateEnd').setValue(new Date(start_date.getFullYear(), start_date.getMonth() + 1, start_date.getDate(), 23, 59, 59));
                    }
                }
            }
        }, {
            xtype: 'displayfield',
            value: '~'
        }, {
            xtype: 'datetimefield',
            format: 'Y-m-d H:i:s',
            time: { hour: 23, min: 59, sec: 59 },
            editable: false,
            id: 'dateEnd',
            listeners: {
                select: function (a, b, c) {
                    var start = Ext.getCmp("dateStart");
                    var end = Ext.getCmp("dateEnd");
                    var end_date = end.getValue();
                    if (start.getValue() == "") {
                        Ext.getCmp('dateStart').setValue(new Date(end_date.getFullYear(), end_date.getMonth() - 1, end_date.getDate()));
                    }
                    if (end.getValue() < start.getValue()) {
                        Ext.getCmp('dateStart').setValue(new Date(end_date.getFullYear(), end_date.getMonth() - 1, end_date.getDate()));
                    }
                }
            }
        }, {//添加級別下拉框  add by zhuoqin0830w 2015/02/04
            xtype: "combobox",
            name: 'level',
            id: 'level',
            labelWidth: 50,
            width: 150,
            fieldLabel: '&nbsp;&nbsp;' + LEVEL,
            displayField: 'Level',
            valueField: 'Level',
            store: levelStore,
            editable: true,
            listeners: {
                beforerender: function () {
                    levelStore.load({
                        callback: function () {
                            Ext.getCmp("level").setValue(levelStore.data.items[0].data.Level);
                        }
                    });
                }
            }
        }, {
            xtype: 'button',
            text: BTN_SEARCH,
            iconCls: 'ui-icon ui-icon-search-2',
            handler: query
        }],
        bbar: Ext.create('Ext.PagingToolbar', {
            store: errorLogStore,
            pageSize: pageSize,
            displayInfo: true,
            displayMsg: NOW_DISPLAY_RECORD + ': {0} - {1}' + TOTAL + ': {2}',
            emptyMsg: NOTHING_DISPLAY
        })
    });


    Ext.create('Ext.container.Viewport', {
        layout: 'fit',
        items: [errorLogGrid],
        renderTo: Ext.getBody(),
        autoScroll: true,
        listeners: {
            resize: function () {
                errorLogGrid.width = document.documentElement.clientWidth;
                this.doLayout();

            }
        }
    });

    Ext.getCmp('errorLogGrid').store.loadPage(1);



});
Today = function () {
    var d;
    var dt;
    var s = "";
    d = new Date();                             // 创建 Date 对象。
    s += d.getFullYear() + "/";                     // 获取年份。
    s += (d.getMonth() + 1) + "/";              // 获取月份。
    s += d.getDate();
    dt = new Date(s);
    dt.setDate(dt.getDate());
    dt.setHours(23, 59, 59);
    return dt;                                 // 返回日期。
}

function setNextMonth(source, n) {
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


