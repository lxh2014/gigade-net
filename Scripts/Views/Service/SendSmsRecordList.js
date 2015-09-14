var CallidForm;
var pageSize = 25;
var SmsId = 0;
//廠商Store
var ProviderStore = Ext.create('Ext.data.Store', {
    fields: ["text", "value"],
    data: [
        { text: '全部', value: '0' },
        { text: 'FreeSMS', value: '1' },
        { text: 'Every8d', value: '2' }
    ]
});
var SuccessStore = Ext.create('Ext.data.Store', {
    fields: ["text", "value"],
    data: [
         { text: '全部', value: '-1' },
        { text: '成功', value: '1' },
        { text: '失敗', value: '0' }
    ]
});
//簡訊發送記錄Model
Ext.define('gigade.SendSmsRecord', {
    extend: 'Ext.data.Model',
    fields: [
        { name: "id", type: "int" },
        { name: "sms_id", type: "int" },
        { name: "sucess_status", type: "bool" },
        { name: "code", type: "string" },
        { name: "created", type: "string" },
        { name: "provider", type: "int" }
    ]
});

//
//簡訊發送記錄Store
var SendSmsRecordStore = Ext.create('Ext.data.Store', {
    autoDestroy: true,
    pageSize: pageSize,
    model: 'gigade.SendSmsRecord',
    proxy: {
        type: 'ajax',
        url: '/Service/SendSmsRecordList',
        reader: {
            type: 'json',
            root: 'data',
            totalProperty: 'totalCount'
        }
    }
});
var sm = Ext.create('Ext.selection.CheckboxModel', {
    listeners: {
        selectionchange: function (sm, selections) {

        }
    }
});
SendSmsRecordStore.on('beforeload', function () {
    Ext.apply(SendSmsRecordStore.proxy.extraParams, {
        provider: Ext.getCmp("providerquery").getValue(),//廠商--provider
        start_time: Ext.getCmp("start_time").getValue(),//開始時間--start_time--created
        end_time: Ext.getCmp("end_time").getValue(),//結束時間--end_time--created
        success: Ext.getCmp("successquery").getValue(),//發送狀態--success
        SmsId: document.getElementById("SMSID").value
    })
});

function Query(x) {
    SendSmsRecordStore.removeAll();
    Ext.getCmp("SendSmsView").store.loadPage(1);
}

Ext.onReady(function () {
    var frm = Ext.create('Ext.form.Panel', {
        id: 'frm',
        layout: 'anchor',
        height: 120,
        border: 0,
        bodyPadding: 10,
        width: document.documentElement.clientWidth,
        items: [

            {
                xtype: 'fieldcontainer',
                combineErrors: true,
                margin: '0 5px',
                layout: 'hbox',
                items: [
                    {
                        xtype: 'displayfield',
                        labelWidth: 100,
                        value: '發送時間:'

                    },
                    {
                        id: 'start_time',
                        xtype: 'datefield',
                        name: 'start_time',
                        editable: false,
                        margin: '0 5px 0 27px',
                        listeners: {
                            'select': function () {
                                var start = Ext.getCmp("start_time");
                                var end = Ext.getCmp("end_time");
                                if (end.getValue() == null) {
                                    end.setValue(setNextMonth(start.getValue(), 1));
                                } else if (start.getValue() > end.getValue()) {
                                    Ext.Msg.alert(INFORMATION, "開始時間不能大於結束時間");
                                    end.setValue(setNextMonth(start.getValue(), 1));
                                }
                                else if (end.getValue() > setNextMonth(start.getValue(), 1)) {
                                    end.setValue(setNextMonth(start.getValue(), 1));
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
                        value: '~'
                    },
                    {
                        id: 'end_time',
                        xtype: 'datefield',
                        name: 'end_time',
                        editable: false,
                        margin: '0 5px',
                        listeners: {
                            'select': function () {
                                var start = Ext.getCmp("start_time");
                                var end = Ext.getCmp("end_time");
                                var s_date = new Date(start.getValue());
                                var now_date = new Date(end.getValue());
                                if (start.getValue() != "" && start.getValue() != null) {
                                    if (end.getValue() < start.getValue()) {
                                        Ext.Msg.alert(INFORMATION, "結束時間不能小於開始時間");
                                        end.setValue(setNextMonth(start.getValue(), 1));
                                    } else if (end.getValue() > setNextMonth(start.getValue(), 1)) {
                                        start.setValue(setNextMonth(end.getValue(), -1));
                                    }
                                } else {
                                    start.setValue(setNextMonth(end.getValue(), -1));
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
                    {
                        xtype: 'combobox',
                        id: 'providerquery',
                        margin: '0 3px',
                        fieldLabel: '廠商',
                        colName: 'datequery',
                        labelWidth: 100,
                        queryMode: 'local',
                        editable: false,
                        listWidth: 80,
                        store: ProviderStore,
                        displayField: 'text',
                        valueField: 'value',
                        value: 0
                    }
                ]
            },
            {
                xtype: 'fieldcontainer',
                combineErrors: true,
                layout: 'hbox',
                items: [
                    {
                        xtype: 'combobox',
                        id: 'successquery',
                        margin: '0 3px',
                        fieldLabel: '發送結果',
                        labelWidth: 100,
                        colName: 'datequery',
                        queryMode: 'local',
                        editable: false,
                        store: SuccessStore,
                        displayField: 'text',
                        valueField: 'value',
                        value: -1
                    }
                ]
            }
            ,
            {
                xtype: 'fieldcontainer',
                combineErrors: true,
                layout: 'hbox',
                items: [
                    {
                        xtype: 'button',
                        margin: '0 8 0 8',
                        iconCls: 'icon-search',
                        text: "查詢",
                        handler: Query
                    },
                    {
                        xtype: 'button',
                        text: '重置',
                        id: 'btn_reset',
                        iconCls: 'ui-icon ui-icon-reset',
                        listeners: {
                            click: function () {
                                Ext.getCmp('providerquery').setValue(0);//廠商--provider
                                Ext.getCmp('start_time').setValue("");//開始時間--start_time--created
                                Ext.getCmp('end_time').setValue("");//結束時間--end_time--created
                                Ext.getCmp('successquery').setValue(-1);//發送狀態--success
                            }
                        }
                    }
                ]
            }
        ]
    });

    var SendSmsView = Ext.create('Ext.grid.Panel', {
        id: 'SendSmsView',
        store: SendSmsRecordStore,
        flex: 8.8,
        width: document.documentElement.clientWidth,
        columnLines: true,
        frame: true,
        columns: [
            { header: "ID", dataIndex: 'id', width: 160, align: 'center' },
            {
                header: 'SMS ID', dataIndex: 'created', width: 90, align: 'center', renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                    return '<a href=javascript:TranToDetial("' + record.data.sms_id + '")>' + record.data.sms_id + '</a>';
                }
            },
            { header: "發送時間", dataIndex: 'created', width: 250, align: 'center' },
            {
                header: "廠商", dataIndex: 'provider', width: 250, align: 'center', renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                    return ProviderStore.getAt(ProviderStore.find("value", value)).data.text;
                }
            },
            {
                header: "狀態", dataIndex: 'sucess_status', width: 100, align: 'center', renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                    if (record.data.sucess_status) {
                        return "成功";
                    }
                    else {
                        return "失敗";
                    }
                }
            },
            { header: "CODE", dataIndex: 'code', width: 160, align: 'center' }
        ],
        bbar: Ext.create('Ext.PagingToolbar', {
            store: SendSmsRecordStore,
            pageSize: pageSize,
            displayInfo: true,
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
        }
    });

    Ext.create('Ext.container.Viewport', {
        layout: 'vbox',
        items: [frm, SendSmsView],
        renderTo: Ext.getBody(),
        autoScroll: true,
        listeners: {
            resize: function () {
                SendSmsView.width = document.documentElement.clientWidth;
                this.doLayout();
            }
        }
    });
    ToolAuthority();
});

//初始時間
function Tomorrow(days) {
    var d;
    var s = "";
    d = new Date();                             // 创建 Date 对象。
    s += d.getFullYear() + "/";                     // 获取年份。
    s += (d.getMonth() + 1) + "/";              // 获取月份。
    s += d.getDate() + days;                          // 获取日。
    return (new Date(s));                                 // 返回日期。
}
function TranToDetial(SMSID) {
    var url = '/Service/SmsSearchIndex?SMSID=' + SMSID;
    var panel = window.parent.parent.Ext.getCmp('ContentPanel');
    var copy = panel.down('#SendSms');
    if (copy) {
        copy.close();
    }
    copy = panel.add({
        id: 'SendSms',
        title: '簡訊查詢',
        html: window.top.rtnFrame(url),
        closable: true
    });
    panel.setActiveTab(copy);
    panel.doLayout();
}
setNextMonth = function (source, n) {
    var s = new Date(source);
    s.setMonth(s.getMonth() + n);
    if (n < 0) {
        s.setHours(0, 0, 0);
    } else if (n > 0) {
        s.setHours(23, 59, 59);
    }
    return s;
}

