Ext.Loader.setConfig({ enabled: true });
Ext.Loader.setPath('Ext.ux', '/Scripts/Ext4.0/ux');
Ext.require([
    'Ext.form.Panel',
    'Ext.ux.form.MultiSelect',
    'Ext.ux.form.ItemSelector'
]);
var pageSize = 25;
Ext.define('gigade.ManageLogin', {
    extend: 'Ext.data.Model',
    fields: [
        { name: "loginID", type: "int" },//登陸編號
        { name: "user_id", type: "int" },//登陸用戶Id
        { name: "login_ipfrom", type: "string" },//來源IP
        { name: "login_createtime", type: "string" },//記錄時間
        { name: "user_name", type: "string" }//登錄人名稱
    ]
});
var ManageLoginStore = Ext.create('Ext.data.Store', {
    autoDestroy: true,
    pageSize: pageSize,
    model: 'gigade.ManageLogin',
    proxy: {
        type: 'ajax',
        url: '/Manage/GetManageLoginList',
        reader: {
            type: 'json',
            root: 'data',
            totalProperty: 'totalCount'
        }
    }
});
ManageLoginStore.on('beforeload', function () {
    Ext.apply(ManageLoginStore.proxy.extraParams, {
        LoginSearch: Ext.getCmp('LoginSearch').getValue(),
        IPSearch: Ext.getCmp('IPSearch').getValue(),
        time_start: Ext.getCmp('time_start').getValue(),
        time_end: Ext.getCmp('time_end').getValue()
    });
});

setNextMonth = function (source, n) {
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

Ext.onReady(function () {
    var frm = Ext.create('Ext.form.Panel', {
        id: 'frm',
        layout: 'anchor',
        height: 90,
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
                        xtype: 'textfield',
                        id: 'LoginSearch',
                        margin: '0 5px',
                        name: 'message',
                        fieldLabel: '登入人名稱',
                        labelWidth: 70,
                        listeners: {
                            specialkey: function (field, e) {
                                if (e.getKey() == Ext.EventObject.ENTER) {
                                    Query();
                                }
                            }
                        }
                    },
                    {
                        xtype: 'textfield',
                        id: 'IPSearch',
                        margin: '0 10px',
                        name: 'message',
                        fieldLabel: '來源IP',
                        labelWidth: 70,
                        listeners: {
                            specialkey: function (field, e) {
                                if (e.getKey() == Ext.EventObject.ENTER) {
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
                        xtype: "datetimefield",
                        fieldLabel: "登入日期",
                        labelWidth: 70,
                        margin: '3 5',
                        id: 'time_start',
                        name: 'time_start',
                        format: 'Y-m-d H:i:s',
                        editable: false,
                        time: { hour: 00, min: 00, sec: 00 },//開始時間00：00：00
                        listeners: {
                            select: function (a, b, c) {
                                var start = Ext.getCmp("time_start");
                                var end = Ext.getCmp("time_end");
                                if (end.getValue() == null) {
                                    end.setValue(setNextMonth(start.getValue(), 1));
                                } else if (end.getValue() < start.getValue()) {
                                    Ext.Msg.alert(INFORMATION, DATA_TIP);
                                    start.setValue(setNextMonth(end.getValue(), -1));
                                }
                                else if (end.getValue() > setNextMonth(start.getValue(), 1)) {
                                    Ext.Msg.alert(INFORMATION, DATE_LIMIT);
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
                        margin: '3 0',
                        value: "~"
                    },
                    {
                        xtype: "datetimefield",
                        id: 'time_end',
                        name: 'time_end',
                        margin: '3 5',
                        format: 'Y-m-d H:i:s',
                        editable: false,
                        time: { hour: 23, min: 59, sec: 59 },//開始時間00：00：00
                        listeners: {
                            select: function (a, b, c) {
                                var start = Ext.getCmp("time_start");
                                var end = Ext.getCmp("time_end");
                                if (start.getValue() != "" && start.getValue() != null) {
                                    if (end.getValue() < start.getValue()) {
                                        Ext.Msg.alert(INFORMATION, DATA_TIP);
                                        end.setValue(setNextMonth(start.getValue(), 1));
                                    }
                                    else if (end.getValue() > setNextMonth(start.getValue(), 1)) {
                                        // Ext.Msg.alert(INFORMATION, DATE_LIMIT);
                                        start.setValue(setNextMonth(end.getValue(), -1));
                                    }
                                }
                                else {
                                    start.setValue(setNextMonth(end.getValue(), -1));
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
                        xtype: 'button',
                        margin: '3 5',
                        iconCls: 'icon-search',
                        text: "查詢",
                        handler: Query
                    },
                    {
                        xtype: 'button',
                        text: '重置',
                        margin: '3 0',
                        id: 'btn_reset',
                        iconCls: 'ui-icon ui-icon-reset',
                        listeners: {
                            click: function () {
                                this.up('form').getForm().reset();
                            }
                        }
                    }
                ]
            }
        ]
    });
    var ManageLogin = Ext.create('Ext.grid.Panel', {
        id: 'ManageLogin',
        store: ManageLoginStore,
        flex: 9.3,
        width: document.documentElement.clientWidth,
        columnLines: true,
        frame: true,
        columns: [

            { header: "登入編號", dataIndex: 'loginID', width: 150, align: 'center' },
            { header: "登入人名稱", dataIndex: 'user_name', width: 200, align: 'center' },
            { header: "登入時間", dataIndex: 'login_createtime', width: 200, align: 'center' },
            { header: "來源IP", dataIndex: 'login_ipfrom', width: 200, align: 'center' }

        ],
        tbar: [],
        bbar: Ext.create('Ext.PagingToolbar', {
            store: ManageLoginStore,
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
        },
        // selModel: sm
    });

    Ext.create('Ext.container.Viewport', {
        layout: 'vbox',
        items: [frm, ManageLogin],//
        renderTo: Ext.getBody(),
        autoScroll: true,
        listeners: {
            resize: function () {
                ManageLogin.width = document.documentElement.clientWidth;
                this.doLayout();
            }
        }
    });
    ToolAuthority();

    function Query() {
        if (Ext.getCmp('LoginSearch').getValue() != "" || Ext.getCmp('IPSearch').getValue() != "" || Ext.getCmp('time_start').getValue() != null || Ext.getCmp('time_end').getValue() != null) {
            ManageLoginStore.removeAll();
            ManageLoginStore.loadPage(1, {
                params: {
                    params: { start: 0, limit: pageSize }
                }

            });
        }
        else {
            Ext.Msg.alert(INFORMATION, SEARCH_LIMIT);
        }
    }
});

