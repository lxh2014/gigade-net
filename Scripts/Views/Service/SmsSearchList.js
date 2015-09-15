var pageSize = 25;

var boolPassword = true;//secretcopy

//簡訊查詢Model
Ext.define('gigade.Sms', {
    extend: 'Ext.data.Model',
    fields: [
    { name: "id", type: "int" },
    { name: "order_id", type: "string" },
    { name: "mobile", type: "string" },
    { name: "subject", type: "string" },
    { name: "content", type: "string" },
    { name: "send", type: "int" },
    { name: "trust_send", type: "string" },
    { name: "created", type: "string" },
    { name: "estimated_send_time", type: "string" },
    { name: "modified", type: "string" },
    { name: "modified_time", type: "string" },
    { name: "estimated_time", type: "string" },
    { name: "created_time", type: "string" }
    ]
});

//
//簡訊查詢Store
var SmsStore = Ext.create('Ext.data.Store', {
    autoDestroy: true,
    pageSize: pageSize,
    model: 'gigade.Sms',
    proxy: {
        type: 'ajax',
        url: '/Service/SmsList',
        reader: {
            type: 'json',
            root: 'data',
            totalProperty: 'totalCount'
        }
    }
});
SmsStore.on('beforeload', function () {
    Ext.apply(SmsStore.proxy.extraParams, {
        datequery: Ext.getCmp('datequery').getValue(),
        time_start: Ext.getCmp('time_start').getValue(),
        time_end: Ext.getCmp('time_end').getValue(),
        searchcontent: Ext.getCmp('searchcontent').getValue(),
        send: Ext.getCmp('send').getValue(),
        trustsend: Ext.getCmp('trustsend').getValue(),
        relation_id: "",
        isSecret: true
    })
});

var edit_SmsStore = Ext.create('Ext.data.Store', {
    autoDestroy: true,
    pageSize: pageSize,
    model: 'gigade.Sms',
    proxy: {
        type: 'ajax',
        url: '/Service/SmsList',
        reader: {
            type: 'json',
            root: 'data',
            totalProperty: 'totalCount'
        }
    }
});
edit_SmsStore.on('beforeload', function () {
    Ext.apply(edit_SmsStore.proxy.extraParams, {
        datequery: Ext.getCmp('datequery').getValue(),
        time_start: Ext.getCmp('time_start').getValue(),
        time_end: Ext.getCmp('time_end').getValue(),
        searchcontent: Ext.getCmp('searchcontent').getValue(),
        send: Ext.getCmp('send').getValue(),
        trustsend: Ext.getCmp('trustsend').getValue(),
        relation_id: "",
        isSecret: false
    })
});

var datequeryStore = Ext.create('Ext.data.Store', {
    fields: ['txt', 'value'],
    data: [
    { "txt": '所有時間', "value": "-1" },
    { "txt": '建立時間', "value": "0" },
    { "txt": '發送時間', "value": "1" }
    ]
});
var SendStore = Ext.create('Ext.data.Store', {
    fields: ['txt', 'value'],
    data: [
    { "txt": '全部狀態', "value": "-1" },
    { "txt": '未發送', "value": "0" },
    { "txt": '已发送', "value": "1" },
    { "txt": '失敗重發', "value": "2" },
    { "txt": '已取消', "value": "3" }
    ]
});
var TrustSendStore = Ext.create('Ext.data.Store', {
    fields: ['txt', 'value'],
    data: [
    { "txt": '發送中', "value": "0" },
    { "txt": '發送完成', "value": "1" }
    ]
});
function Query(x) {
    var selDate = Ext.getCmp("datequery").getValue();
    var start = Ext.getCmp("time_start").getValue();
    var end = Ext.getCmp("time_end").getValue();
    SmsStore.removeAll();
    if ((selDate == -1 || selDate == '' || selDate == null) && (start != null || end != null)) {
        Ext.Msg.alert(INFORMATION, '請先選擇日期條件');
        Ext.getCmp("time_start").reset();
        Ext.getCmp("time_end").reset();
    }
    else if ((selDate != -1 ) && (start == null || end == null)) {
        Ext.Msg.alert(INFORMATION, '請選擇日期');
        Ext.getCmp("time_start").reset();
        Ext.getCmp("time_end").reset();
    }
    else {
        Ext.getCmp("SmsView").store.loadPage(1);
    }
}

Ext.onReady(function () {
    var frm = Ext.create('Ext.form.Panel', {
        id: 'frm',
        layout: 'anchor',
        height: 80,
        flex: 1.5,
        border: 0,
        bodyPadding: 10,
        width: document.documentElement.clientWidth,
        items: [
        {
            xtype: 'fieldcontainer',
            combineErrors: true,
            layout: 'hbox',
            margin: '10,0,0,0',
            items: [
            {
                xtype: 'combobox',
                id: 'datequery',
                fieldLabel: '日期條件',
                queryMode: 'local',
                editable: false,
                labelWidth: 60,
                store: datequeryStore,
                displayField: 'txt',
                valueField: 'value',
                value: -1
            },
            {
                xtype: 'datefield',
                id: 'time_start',
                name: 'time_start',
                margin: '0,0, 5,40',
                editable: false,
                listeners: {
                    'select': function () {
                        var start = Ext.getCmp("time_start");
                        var end = Ext.getCmp("time_end");
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
                    },
                    focus: function () {
                        var searchType = Ext.getCmp("datequery").getValue();
                        if (searchType == null || searchType == '' || searchType == '-1') {
                            Ext.Msg.alert(INFORMATION, '請先選擇日期條件');
                            this.focus = false;
                        }
                    }
                }

            },
            {
                xtype: 'displayfield',
                value: '~'
            },
            {
                xtype: 'datefield',
                id: 'time_end',
                name: 'time_end',
                margin: '0 5px',
                editable: false,
                listeners: {
                    'select': function () {
                        var start = Ext.getCmp("time_start");
                        var end = Ext.getCmp("time_end");
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
                    },
                    focus: function () {
                        var searchType = Ext.getCmp("datequery").getValue();
                        if (searchType == null || searchType == '' || searchType == '-1') {
                            Ext.Msg.alert(INFORMATION, '請先選擇日期條件');
                            this.focus = false;
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
            margin: '10,0,0,0',
            items: [
            {
                xtype: 'combobox',
                id: 'send',
                margin: '0,0,0,0',
                fieldLabel: '發送狀態',
                queryMode: 'local',
                editable: false,
                labelWidth: 60,
                store: SendStore,
                displayField: 'txt',
                valueField: 'value',
                value: -1,
                listeners: {
                    "select": function (combo, record) {
                        var send = Ext.getCmp("send");
                        var trustsend = Ext.getCmp("trustsend");
                        if (send.getValue() == 1) {
                            trustsend.setDisabled(false);
                        } else {
                            trustsend.clearValue();
                            trustsend.setDisabled(true);
                        }
                    }
                }
            },
            {
                xtype: 'combobox',
                id: 'trustsend',
                margin: '0,0, 5,40',
                fieldLabel: '電信發送狀態',
                queryMode: 'local',
                editable: false,
                disabled: true,
                labelWidth: 80,
                store: TrustSendStore,
                displayField: 'txt',
                valueField: 'value',
                value: -1
            }
            ]
        },
        {
            xtype: 'fieldcontainer',
            combineErrors: true,
            layout: 'hbox',
            items: [{
                id: 'searchcontent',
                xtype: 'textfield',
                fieldLabel: "編號/訂單編號/行動電話",
                width: 313,
                labelWidth: 140,
                regex: /^\d+$/,
                regexText: '请输入正確的編號,訂單編號,行動電話進行查詢',
                name: 'searchcontent',
                allowBlank: true,
                value: document.getElementById("SMSID").value,
                listeners: {                   
                    specialkey: function (field, e) {
                        if (e.getKey() == e.ENTER) {
                            Query();
                        }
                    }
                }
            },
            {
                xtype: 'button',
                text: SEARCH,
                margin: '0,0, 5,40',
                margin: '0 10 0 20',
                id: 'btnQuery',
                iconCls: 'icon-search',
                handler: Query
            },
            {
                xtype: 'button',
                text: RESET,
                margin: '0 0 0 0',
                id: 'btn_reset',
                iconCls: 'ui-icon ui-icon-reset',
                listeners: {
                    click: function () {
                        Ext.getCmp("datequery").setValue(-1);
                        Ext.getCmp("searchcontent").setValue('');
                        Ext.getCmp('time_start').reset();//開始時間--time_start
                        Ext.getCmp('time_end').reset();//結束時間--time_end
                        Ext.getCmp('send').setValue(-1),
                        Ext.getCmp('trustsend').clearValue();
                        Ext.getCmp('trustsend').setDisabled(true);
                    }
                }
            }
            ]
        },

        ]
    });

    var SmsView = Ext.create('Ext.grid.Panel', {
        id: 'SmsView',
        store: SmsStore,
        flex: 8,
        width: document.documentElement.clientWidth,
        columnLines: true,
        frame: true,
        columns: [
        { header: "編號", dataIndex: 'id', width: 60, align: 'center' },
        {
            header: "訂單編號", dataIndex: 'order_id', width: 100, align: 'center'

        },
        {
            header: "行動電話", dataIndex: 'mobile', width: 130, align: 'center',
            renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                return "<a  href='javascript:void(0);' onclick='SecretLogin(" + record.data.id + ")'  >" + value + "</a>"
            }
        },
        { header: "主旨", dataIndex: 'subject', width: 150, align: 'center' },
        { header: "內容", dataIndex: 'content', flex: 1, align: 'center' },
        {
            header: "發送狀態", dataIndex: 'send', width: 100, align: 'center', renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                var sendcontent = record.data.send;
                switch (record.data.send) {
                    case 0:
                        //sendcontent = "未發送";
                        sendcontent = "<a href=javascript:onEditClick(" + record.data.id + ")>未發送</a>"
                        break;
                    case 1:
                        sendcontent = "已发送";
                        break;
                    case 2:
                        sendcontent = "失敗重發";
                        break;
                    case 3:
                        sendcontent = "已取消";
                        break;
                }
                return sendcontent;
            }
        },
        {
            header: "電信發送狀態", dataIndex: 'trust_send', width: 100, align: 'center', renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                var sendcontent = record.data.trust_send;
                switch (record.data.send) {
                    case 0:
                        sendcontent = "未發送";
                        break;
                    case 1:
                        sendcontent = "發送失敗";
                        if (record.data.trust_send == 1) {
                            sendcontent = "發送完成";
                        }
                        if (record.data.trust_send == 0 || record.data.trust_send == 2) {
                            sendcontent = "發送中";
                        }
                        break;
                    case 2:
                        sendcontent = "發送失敗";
                        break;
                    case 3:
                        sendcontent = "已取消";
                        break;
                }
                return sendcontent;
            }
        },
        { header: "建立時間", dataIndex: 'created_time', width: 150, align: 'center' },
        { header: "預計發送時間", dataIndex: 'estimated_time', width: 150, align: 'center' },
        {
            header: "發送時間", dataIndex: 'modified_time', width: 150, align: 'center', renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                if (record.data.send == 0 || record.data.send == 3) {
                    return '';
                }
                else {
                    return record.data.modified_time;
                }
            }
        }
        ,
        {
            header: '檢視', dataIndex: 'modified_time', width: 60, align: 'center',
            renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                if (record.data.send == 1) {
                    return '<a href=javascript:TranToDetial("' + record.data.id + '")>' + "<img hidValue='0' id='reply_ok' src='../../../Content/img/icons/reply.png'/> " + '</a>';
                };
            }
        }
        ],
        bbar: Ext.create('Ext.PagingToolbar', {
            store: SmsStore,
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
        items: [frm, SmsView],//
        renderTo: Ext.getBody(),
        autoScroll: true,
        listeners: {
            resize: function () {
                SmsView.width = document.documentElement.clientWidth;               
                this.doLayout();
            }
        }
    });
    ToolAuthority();
});


function SecretLogin(rid) {//secretcopy
    var secret_type = "3";//參數表中的"簡訊查詢列表"
    var url = "/Service/SmsSearchIndex ";
    var ralated_id = rid;
    //點擊機敏信息先保存記錄在驗證密碼是否需要輸入
    boolPassword = SaveSecretLog(url, secret_type, ralated_id);//判斷5分鐘之內是否有輸入密碼
    if (boolPassword != "-1") {//不准查看
        if (boolPassword) {//超過5分鐘沒有輸入密碼
            //參數1：機敏頁面代碼，2：機敏資料主鍵，3：是否彈出驗證密碼框,4：是否直接顯示機敏信息6.驗證通過后是否打開編輯窗口
            //  function SecretLoginFun(type, relatedID, isLogin, isShow, editO, isEdit) {
            SecretLoginFun(secret_type, ralated_id, true, true, false, url, null, null, null);//先彈出驗證框，關閉時在彈出顯示框

        } else {
            SecretLoginFun(secret_type, ralated_id, false, true, false, url, null, null, null);//直接彈出顯示框
        }
    }
}

function TranToDetial(SMSID) {
    var url = '/Service/SendSmsRecord?SMSID=' + SMSID + '&StartTime=' + 1;
    var panel = window.parent.parent.Ext.getCmp('ContentPanel');
    var copy = panel.down('#SmsSearch');
    if (copy) {
        copy.close();
    }
    copy = panel.add({
        id: 'SmsSearch',
        title: '簡訊發送記錄',
        html: window.top.rtnFrame(url),
        closable: true
    });
    panel.setActiveTab(copy);
    panel.doLayout();
}

onEditClick = function (rID) {
    var secret_type = "3";//參數表中的"簡訊查詢查詢列表"
    var url = "/Service/SmsSearchIndex/Edit ";
    var ralated_id = rID;
    boolPassword = SaveSecretLog(url, secret_type, ralated_id);//判斷5分鐘之內是否有輸入密碼
    if (boolPassword != "-1") {
        if (boolPassword) {//驗證
            SecretLoginFun(secret_type, ralated_id, true, false, true, url, "", "", "");//先彈出驗證框，關閉時在彈出顯示框
        } else {
            editFunction(ralated_id);
        }
    }
}
function TransToUser(UserMobile) {
    var url = '/Member/UsersListIndex?UserMobile=' + UserMobile.split('*')[0];
    var panel = window.parent.parent.Ext.getCmp('ContentPanel');
    var copy = panel.down('#user_detial');
    if (copy) {
        copy.close();
    }
    copy = panel.add({
        id: 'user_detial',
        title: '會員內容',
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


