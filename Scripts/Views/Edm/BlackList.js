pageSize = 25;
var boolPassword = true;//secretcopy
var info_type = "vip_user";
var secret_info = "user_id;user_email";
Ext.define('gigade.VipUser', {
    extend: 'Ext.data.Model',
    fields: [
        { name: 'v_id', type: 'int' },
        { name: 'user_email', type: 'string' },
        { name: 'user_id', type: 'int' },
		{ name: 'status', type: 'int' },
        { name: 'group_id', type: 'int' },
        { name: 'emp_id', type: 'string' },
        { name: 'createdate', type: 'int' },
        { name: 'create', type: 'string' },
        { name: 'source', type: 'int' },
        { name: 'createUsername', type: 'string' },
        { name: 'updateUsername', type: 'string' },
        { name: 'updatedate', type: 'string' }
    ]
});
var BlackListStore = Ext.create('Ext.data.Store', {
    autoDestroy: true,
    pageSize: pageSize,
    model: 'gigade.VipUser',
    proxy: {
        type: 'ajax',
        url: '/Edm/GetBlackList',
        reader: {
            type: 'json',
            root: 'data',
            totalProperty: 'totalCount'
        }
    }
});
BlackListStore.on('beforeload', function () {
    Ext.apply(BlackListStore.proxy.extraParams,
        {
            start_date: Ext.getCmp('start').getValue(),
            end_date: Ext.getCmp('end').getValue(),
            searchState: Ext.getCmp('searchState').getValue(),
            search_text: Ext.getCmp('search_text').getValue(),
            source: Ext.getCmp('source').getValue(),
            relation_id: "",
            isSecret: true
        });
});
var SearchStatus = Ext.create('Ext.data.Store', {
    fields: ['txt', 'value'],
    data: [
    { "txt": "全部", "value": "2" },
    { "txt": "解除", "value": "0" },
    { "txt": "鎖定", "value": "1" }
    ]
});
var SourceStore = Ext.create('Ext.data.Store', {
    fields: ['sou', 'sourceValue'],
    data: [
    { "sou": "全部", "sourceValue": "0" },
    { "sou": "用戶", "sourceValue": "1" },
    { "sou": "客服", "sourceValue": "2" }
    ]
});
var sm = Ext.create('Ext.selection.CheckboxModel', {
    listeners: {
        selectionchange: function (sm, selections) {
            Ext.getCmp("gdList").down('#edit').setDisabled(selections.length == 0);
            var row = Ext.getCmp("gdList").getSelectionModel().getSelection();

        }
    }
});

Ext.onReady(function () {
    var frm = Ext.create('Ext.form.Panel', {
        id: 'frm',
        layout: 'anchor',
        height: 70,
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
                        xtype: 'datetimefield',
                        fieldLabel: "列入黑名單日期",
                        id: 'start',
                        name: 'start',
                        margin: '0 5px 0 0',
                        labelWidth: 90,
                        // width: 210,
                        format: 'Y-m-d H:i:s',
                        editable: false,
                        time: { hour: 00, min: 00, sec: 00 },//開始時間00：00：00
                        listeners: {
                            select: function (a, b, c) {
                                var tstart = Ext.getCmp("start");
                                var tend = Ext.getCmp("end");
                                if (tend.getValue() == null) {
                                    tend.setValue(setNextMonth(tstart.getValue(), 1));
                                }
                                else if (tend.getValue() < tstart.getValue()) {
                                    Ext.Msg.alert(INFORMATION, "開始時間不能大於結束時間");
                                    tend.setValue(setNextMonth(tstart.getValue(), 1));
                                }
                            }
                        }
                    },
                    {
                        xtype: 'displayfield',
                        margin: '0 2 0 2',
                        value: "~"
                    },
                    {
                        xtype: 'datetimefield',
                        id: 'end',
                        name: 'end',
                        margin: '0 5px 0 0',
                        labelWidth: 15,
                        // width: 210,
                        format: 'Y-m-d H:i:s',
                        editable: false,
                        time: { hour: 23, min: 59, sec: 59 },//標記結束時間
                        listeners: {
                            select: function (a, b, c) {
                                var tstart = Ext.getCmp("start");
                                var tend = Ext.getCmp("end");
                                if (tstart.getValue() == null) {
                                    tstart.setValue(setNextMonth(tend.getValue(), -1));
                                }
                                else if (tend.getValue() < tstart.getValue()) {
                                    Ext.Msg.alert(INFORMATION, "開始時間不能大於結束時間");
                                    tstart.setValue(setNextMonth(tend.getValue(), -1));
                                }
                            }
                        }
                    }, {
                        xtype: 'textfield',
                        id: 'search_text',
                        name: 'search_text',
                        value: '',
                        labelWidth: 60,
                        // width: 190,
                        fieldLabel: '會員郵箱',
                        value: '',
                        editable: false,
                        listeners: {
                            specialkey: function (field, e) {
                                if (e.getKey() == e.ENTER) {
                                    Search(1);
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
                        xtype: 'textfield',
                        id: 'search_uid',
                        name: 'search_uid',
                        value: '',
                        labelWidth: 90,
                        width:230,
                        fieldLabel: '會員編號',
                        value: '',
                        editable: false,
                        listeners: {
                            specialkey: function (field, e) {
                                if (e.getKey() == e.ENTER) {
                                    Search(1);
                                }
                            }
                        }
                    }, {
                        xtype: 'combobox',
                        id: 'source',
                        fieldLabel: '添加來源',
                        labelWidth: 55,
                        width: 170,
                        margin: '0 5 0 10',
                        queryMode: 'local',
                        editable: false,
                        store: SourceStore,
                        displayField: 'sou',
                        valueField: 'sourceValue',
                        value: ''
                    },
                    {
                        xtype: 'combobox',
                        id: 'searchState',
                        fieldLabel: '狀態',
                        labelWidth: 60,
                        //  width: 190,
                        margin: '0 5 0 0',
                        queryMode: 'local',
                        editable: false,
                        store: SearchStatus,
                        displayField: 'txt',
                        valueField: 'value',
                        value: ''
                    },
                    {
                        xtype: 'button',
                        margin: '0 5 0 5',
                        iconCls: 'ui-icon ui-icon-search-2',
                        text: "查詢",
                        handler: Search
                    },
            {
                xtype: 'button',
                text: '重置',
                id: 'btn_reset',
                iconCls: 'ui-icon ui-icon-reset',
                listeners: {
                    click: function () {
                        Ext.getCmp('searchState').setValue('');
                        Ext.getCmp('search_text').setValue('');
                        Ext.getCmp('start').setValue(null);
                        Ext.getCmp('end').setValue(null);
                        Ext.getCmp('source').setValue('');
                        Ext.getCmp('search_uid').setValue('');
                    }
                }
            }
                ]
            }
        ]
    });
    var gdList = Ext.create('Ext.grid.Panel', {
        id: 'gdList',
        store: BlackListStore,
        width: document.documentElement.clientWidth,
        columnLines: true,
        height: document.documentElement.clientHeight - 70,
        frame: true,
        columns: [
            { header: "黑名單編號", dataIndex: 'v_id', flex: 1, align: 'center' },
            { header: "會員編號", dataIndex: 'user_id', flex: 1, align: 'center' },
            {
                header: "會員郵箱", dataIndex: 'user_email', flex: 3, align: 'center',
                renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {//secretcopy
                    return "<span onclick='SecretLogin(" + record.data.v_id + "," + record.data.v_id + ",\"" + info_type + "\")'  >" + value + "</span>";
                }
            },
            {
                header: "添加來源", dataIndex: 'source', flex: 1, align: 'center',
                renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                    if (value == 2) {
                        return '客服';
                    }
                    else {
                        return '用戶';
                    }
                }
            },
            {
                header: "狀態", dataIndex: 'status', flex: 1, align: 'center',
                renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                    if (value == true) {
                        return "<a href='javascript:void(0);' onclick='UpdateActive(" + record.data.v_id + "," + record.data.user_id + ")'><img hidValue='0' id='img" + record.data.v_id + "' src='../../../Content/img/icons/hmenu-lock.png'/></a>";
                    }
                    else {
                        return "<a href='javascript:void(0);' onclick='UpdateActive(" + record.data.v_id + "," + record.data.user_id + ")'><img hidValue='1' id='img" + record.data.v_id + "' src='../../../Content/img/icons/hmenu-unlock.png'/></a>";
                    }
                }
            },
            { header: "創建者", dataIndex: 'createUsername', flex: 1, align: 'center' },
            { header: "列入黑名單日期", dataIndex: 'create', flex: 1, align: 'center' },
            { header: "修改者", dataIndex: 'updateUsername', flex: 1, align: 'center' },
            { header: "修改時間", dataIndex: 'updatedate', flex: 1, align: 'center' }
        ],
        tbar: [
             {
                 xtype: 'button',
                 text: ADD,
                 id: 'add',
                 iconCls: 'icon-user-add',
                 handler: addClick
             },
            {
                xtype: 'button',
                id: 'exportfile',
                text: '匯出',
                iconCls: 'icon-excel',
                hidden: true,
                handler: ExportFile
            }
        ],
        bbar: Ext.create('Ext.PagingToolbar', {
            store: BlackListStore,
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
        // selModel: sm     
    });

    Ext.create('Ext.container.Viewport', {
        layout: 'vbox',
        items: [frm, gdList],
        renderTo: Ext.getBody(),
        autoScroll: true,
        listeners: {
            resize: function () {
                gdList.width = document.documentElement.clientWidth;
                this.doLayout();
            }
        }
    });
    ToolAuthority();
})
function Search() {
    BlackListStore.removeAll();   
    var uid = Ext.getCmp('search_uid').getValue();
    var state = Ext.getCmp('searchState').getValue();
    var text = Ext.getCmp('search_text').getValue();
    var start = Ext.getCmp('start').getValue();
    var end = Ext.getCmp('end').getValue();
    var sou = Ext.getCmp('source').getValue();
    if (sou == "" && state == '' && text == '' && uid=='' && (start == null || end == null)) {
        Ext.Msg.alert(INFORMATION, '請選擇查詢條件');
    }
    else {
        Ext.getCmp("gdList").store.loadPage(1, {
            params: {
                start_date: Ext.getCmp('start').getValue(),
                end_date: Ext.getCmp('end').getValue(),
                searchState: Ext.getCmp('searchState').getValue(),
                search_text: Ext.getCmp('search_text').getValue(),
                source: sou,
                uid: uid
            }
        });
    }
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
    return s;
}
var addClick = function () {
    editFunction(BlackListStore);
}
function UpdateActive(id, uid) {

    var activeValue = $("#img" + id).attr("hidValue");
    $.ajax({
        url: "/Edm/UpdateState",
        data: {
            "id": id,
            "uid": uid,
            "active": activeValue
        },
        type: "POST",
        dataType: "json",
        success: function (msg) {
            if (activeValue == 1) {
                $("#img" + id).attr("hidValue", 0);
                $("#img" + id).attr("src", "../../../Content/img/icons/accept.gif");
                // BlackListStore.load();
                Search(1);
            }
            else {
                $("#img" + id).attr("hidValue", 1);
                $("#img" + id).attr("src", "../../../Content/img/icons/drop-no.gif");
                //BlackListStore.load();
                Search(1);
            }
        },
        error: function (msg) {
            Ext.Msg.alert(INFORMATION, FAILURE);
            //BlackListStore.load();
            Search(1);
        }
    });
}
var ExportFile = function () {
    // BlackListStore.removeAll();
    var uid = Ext.getCmp('search_uid').getValue();
    var state = Ext.getCmp('searchState').getValue();
    var text = Ext.getCmp('search_text').getValue();
    var start = Ext.getCmp('start').getValue();
    var end = Ext.getCmp('end').getValue();
    var sou = Ext.getCmp('source').getValue();
    if (sou == "" && state == '' && text == '' && uid==''&&(start == null || end == null)) {
        Ext.Msg.alert(INFORMATION, '請選擇查詢條件');
    }
    else {
        Ext.MessageBox.show({
            msg: 'Loading....',
            wait: true
        });
        Ext.Ajax.request({
            url: '/Edm/ExportFile',
            timeout: 900000,
            params: {
                start_date: Ext.getCmp('start').getValue(),
                end_date: Ext.getCmp('end').getValue(),
                searchState: Ext.getCmp('searchState').getValue(),
                search_text: Ext.getCmp('search_text').getValue(),
                source: Ext.getCmp('source').getValue(),
                uid: uid
            },
            success: function (form, action) {
                var result = Ext.decode(form.responseText);
                if (result.success) {
                    window.location = '../../ImportUserIOExcel/' + result.ExcelName;
                    Ext.MessageBox.hide();
                } else {
                    Ext.MessageBox.hide();
                    Ext.Msg.alert(INFORMATION, "匯出失敗或沒有數據,請先搜索查看!");
                }
            }
        });
    }
}
function SecretLogin(rid, info_id, info_type) {//secretcopy
    var secret_type = "15";//參數表中的"訊息管理"
    var url = "/Edm/BlackList";
    var ralated_id = rid;
    //點擊機敏信息先保存記錄在驗證密碼是否需要輸入
    boolPassword = SaveSecretLog(url, secret_type, ralated_id);//判斷5分鐘之內是否有輸入密碼
    if (boolPassword != "-1") {//不准查看
        if (boolPassword) {//超過5分鐘沒有輸入密碼
            //參數1：機敏頁面代碼，2：機敏資料主鍵，3：是否彈出驗證密碼框,4：是否直接顯示機敏信息6.驗證通過后是否打開編輯窗口
            //  function SecretLoginFun(type, relatedID, isLogin, isShow, editO, isEdit) {
            SecretLoginFun(secret_type, ralated_id, true, true, false, url, info_type, info_id, secret_info);//先彈出驗證框，關閉時在彈出顯示框

        } else {
            SecretLoginFun(secret_type, ralated_id, false, true, false, url, info_type, info_id, secret_info);//直接彈出顯示框
        }
    }
}