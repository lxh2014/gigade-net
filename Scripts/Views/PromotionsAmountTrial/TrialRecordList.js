var pageSize = 25;
var boolPassword = true;//secretcopy
var info_type = "users";
var secret_info = "";
/******************促銷試用活動管理主頁面****************************/
//促銷試用活動管理Model
Ext.define('gigade.TrialApplyModel', {
    extend: 'Ext.data.Model',
    fields: [
        { name: "record_id", type: "int" },
        { name: "trial_id", type: "int" },
        { name: "user_id", type: "int" },
        { name: "user_email", type: "string" },
        { name: "user_name", type: "string" },
        { name: "status", type: "int" },
        { name: "apply_time", type: "string" },
        { name: "name", type: "string" },
        { name: "event_type", type: "string" },
        { name: "eventId", type: "string" }, 
        { name: "paper_id", type: "int" }

    ]
});
secret_info = "user_email;user_name";
var TrialApplyStore = Ext.create('Ext.data.Store', {
    model: 'gigade.TrialApplyModel',
    proxy: {
        type: 'ajax',
        url: '/PromotionsAmountTrial/GetTrialRecordList',
        reader: {
            type: 'json',
            root: 'data',
            totalProperty: 'totalCount'
        }
    }
});

//定義ddl的數據
var DDLStore = Ext.create('Ext.data.Store', {
    fields: ['txt', 'value'],
    data: [
        { "txt": '所有狀態', "value": "0" },
        { "txt": '報名中', "value": "1" },
        { "txt": '已錄取', "value": "2" },
        { "txt": '未錄取', "value": "3" }
    ]
});
//前面選擇框 選擇之後顯示編輯刪除
var sm = Ext.create('Ext.selection.CheckboxModel', {
    listeners: {
        selectionchange: function (sm, selections) {
            Ext.getCmp("TrialApply").down('#edit').setDisabled(selections.length == 0);
        }
    }
});

//定義ddl的數據


TrialApplyStore.on('beforeload', function () {
    Ext.apply(TrialApplyStore.proxy.extraParams, {
        trial_id: document.getElementById("trial_id").value,
        luquStatus: Ext.getCmp('ccstatus').getValue(),
        relation_id: "",
        isSecret: true
    });
});


//用作編輯時獲得數據包含機敏信息
var edit_TrialApplyStore = Ext.create('Ext.data.Store', {
    model: 'gigade.TrialApplyModel',
    proxy: {
        type: 'ajax',
        url: '/PromotionsAmountTrial/GetTrialRecordList',
        reader: {
            type: 'json',
            root: 'data',
            totalProperty: 'totalCount'
        }
    }
});

edit_TrialApplyStore.on('beforeload', function () {
    Ext.apply(edit_TrialApplyStore.proxy.extraParams, {
        trial_id: document.getElementById("trial_id").value,
        luquStatus: Ext.getCmp('ccstatus').getValue(),
        relation_id: "",
        isSecret: false
    });
});


//頁面載入
Ext.onReady(function () {
    var TrialApply = Ext.create('Ext.grid.Panel', {
        id: 'TrialApply',
        store: TrialApplyStore,
        width: document.documentElement.clientWidth,
        columnLines: true,
        frame: true,
        columns: [
            { header: RECORDID, dataIndex: 'record_id', width: 60, align: 'center' },
            { header: HDID, dataIndex: 'trial_id', width: 100, align: 'center' },
            { header: '活動名稱', dataIndex: 'name', width: 100, align: 'center' },
            {
                header: '活動類型', dataIndex: 'event_type', width: 100, align: 'center',
                renderer: function (val) {
                    if (val == "T1") {
                        return Ext.String.format('試吃');
                    }
                    else if (val == 'T2') {
                        return Ext.String.format("試用");
                    }
                }
            },
            { header: '用戶編號', dataIndex: 'user_id', width: 100, align: 'center' },
            {
                header: USEREMAIL, dataIndex: 'user_email', width: 100, align: 'center',
                renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {//secretcopy
                    return "<span onclick='SecretLogin(" + record.data.record_id + "," + record.data.user_id + ",\"" + info_type + "\")'  >" + value + "</span>";
                }
            },
            {
                header: USERNAME, dataIndex: 'user_name', width: 60, align: 'center',
                renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {//secretcopy
                    return "<span onclick='SecretLogin(" + record.data.record_id + "," + record.data.user_id + ",\"" + info_type + "\")'  >" + value + "</span>";
                }
            },
            {
                header: STATUS, dataIndex: 'status', width: 200, align: 'center',
                renderer: function (val) {
                    if (val == 1) {
                        return Ext.String.format(APPLYSTATUS1);
                    }
                    else if (val == 2) {
                        return Ext.String.format(APPLYSTATUS);
                    }
                    else if (val == 3) {
                        return Ext.String.format(APPLYSTATUS3);
                    }
                }
            },
            { header: APPLYTIME, dataIndex: 'apply_time', width: 150, align: 'center' },
            {
                header: '問卷調查編號', dataIndex: 'paper_id', width: 100, align: 'center',
                renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                    if (value > 0) {
                        return '<a href=javascript:TranToPaper("/Paper/PaperAnswerList","' + value + '","' + record.data.user_id + '")>' + "問卷詳情" + '</a> ';
                    }
                }
            }

        ],
        tbar: [
            //{ xtype: 'button', text: ADD, id: 'add', hidden: false, iconCls: 'icon-user-add', handler: onAddClick },
            { xtype: 'button', text: EDIT, id: 'edit', hidden: false, iconCls: 'icon-user-edit', disabled: true, handler: onEditClick },
            "->",
            {
                xtype: 'combobox',
                editable: false,
                fieldLabel: '錄取狀態',
                labelWidth: 80,
                name: 'ccstatus',
                id: 'ccstatus',
                store: DDLStore,
                displayField: 'txt',
                valueField: 'value',
                value: '0'
                //,
                //listeners: {
                //    "select": function (combo, record) {
                //        TrialApplyStore.removeAll();
                //        Ext.getCmp("TrialApply").store.loadPage(1, {
                //            params: {
                //                luquStatus: Ext.getCmp('ccstatus').getValue()
                //            }
                //        });
                //    }
                //}
            },
             {
                 text: SEARCH,
                 iconCls: 'icon-search',
                 id: 'btnQuery',
                 handler: Query
             }



        ],
        bbar: Ext.create('Ext.PagingToolbar', {
            store: TrialApplyStore,
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
        selModel: sm
    });

    Ext.create('Ext.container.Viewport', {
        layout: 'fit',
        items: [TrialApply],
        renderTo: Ext.getBody(),
        autoScroll: true,
        listeners: {
            resize: function () {
                TrialApply.width = document.documentElement.clientWidth;
                this.doLayout();
            }
        }
    });
    //ToolAuthority();
    QueryToolAuthorityByUrl('/PromotionsAmountTrial/TrialRecord');
    TrialApplyStore.load({ params: { start: 0, limit: 25 } });
});
function Query(x) {
    TrialApplyStore.removeAll();
    Ext.getCmp("TrialApply").store.loadPage(1, {
        params: {
            luquStatus: Ext.getCmp('ccstatus').getValue()
        }
    });

}

function SecretLogin(rid, info_id, info_type) {//secretcopy
    var secret_type = "5";//參數表中的"會員查詢列表"
    var url = "/PromotionsAmountTrial/TrialRecord";
    var ralated_id = rid;
    //點擊機敏信息先保存記錄在驗證密碼是否需要輸入
    boolPassword = SaveSecretLog(url, secret_type, ralated_id);//判斷5分鐘之內是否有輸入密碼
    if (boolPassword != "-1") {//不准查看
        if (boolPassword) {//超過5分鐘沒有輸入密碼
            //參數1：機敏頁面代碼，2：機敏資料主鍵，3：是否彈出驗證密碼框,4：是否直接顯示機敏信息6.驗證通過后是否打開編輯窗口 7:客戶信息類型user:會員 order：訂單 vendor：供應商 8：客戶id9：要顯示的客戶信息
            //  function SecretLoginFun(type, relatedID, isLogin, isShow, editO, isEdit) {
            SecretLoginFun(secret_type, ralated_id, true, true, false, url, info_type, info_id, secret_info);//先彈出驗證框，關閉時在彈出顯示框

        } else {
            SecretLoginFun(secret_type, ralated_id, false, true, false, url, info_type, info_id, secret_info);//先彈出驗證框，關閉時在彈出顯示框
        }
    }
}


//修改
onEditClick = function () {
    var row = Ext.getCmp("TrialApply").getSelectionModel().getSelection();
    if (row.length == 0) {
        Ext.Msg.alert(INFORMATION, NO_SELECTION);
    } else if (row.length > 1) {
        Ext.Msg.alert(INFORMATION, ONE_SELECTION);
    } else if (row.length == 1) {
        var secret_type = "5";//參數表中的"會員查詢列表"
        var url = "/PromotionsAmountTrial/TrialRecord/Edit ";
        var ralated_id = row[0].data.record_id;
        var info_id = row[0].data.user_id;
        boolPassword = SaveSecretLog(url, secret_type, ralated_id);//判斷5分鐘之內是否有輸入密碼
        if (boolPassword != "-1") {
            if (boolPassword) {//驗證
                SecretLoginFun(secret_type, ralated_id, true, false, true, url, info_type, info_id, secret_info);//先彈出驗證框，關閉時在彈出顯示框
            } else {
                editFunction(ralated_id);
            }
        }
        //editFunction(row[0], TrialApplyStore);
    }
}
function TranToPaper(url, paper_id, user_id) {
    var copyTitle = "問卷答案";
    var urlTran = url + '?paper_id=' + paper_id + "&user_id=" + user_id;
    var panel = window.parent.parent.Ext.getCmp('ContentPanel');
    var copy = panel.down('#paperAnswer');
    if (copy) {
        copy.close();
    }
    copy = panel.add({
        id: 'paperAnswer',
        title: copyTitle,
        html: window.top.rtnFrame(urlTran),
        closable: true
    });
    panel.setActiveTab(copy);
    panel.doLayout();

}
