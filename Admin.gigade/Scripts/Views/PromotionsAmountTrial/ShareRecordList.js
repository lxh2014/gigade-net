var pageSize = 25;
var boolPassword = true;//secretcopy
var info_type = "trial_share";
var secret_info = "";
//alert(document.getElementById('eventId').value);
/******************促銷試用活動管理主頁面****************************/
//促銷試用活動管理Model
Ext.define('gigade.ShareRecordModel', {
    extend: 'Ext.data.Model',
    fields: [
        { name: "share_id", type: "int" },
        { name: "trial_id", type: "int" },
        { name: "user_id", type: "int" },
        { name: "is_show_name", type: "int" },
        { name: "user_name", type: "string" },
        { name: "user_gender", type: "int" },
        { name: "content", type: "string" },
        { name: "share_time", type: "string" },
        { name: "status", type: "int" },
        { name: "s_share_time", type: "string" },
        { name: "event_name", type: "string" },
        { name: "real_name", type: "string" },
        { name: "gender", type: "string" },
        { name: "after_name", type: "string" },



    ]
});
secret_info = "user_name;";
var ShareRecordStore = Ext.create('Ext.data.Store', {
    model: 'gigade.ShareRecordModel',
    proxy: {
        type: 'ajax',
        url: '/PromotionsAmountTrial/GetShareRecordList',
        reader: {
            type: 'json',
            root: 'data',
            totalProperty: 'totalCount'
        }
    }
});

//前面選擇框 選擇之後顯示編輯刪除
var sm = Ext.create('Ext.selection.CheckboxModel', {
    listeners: {
        selectionchange: function (sm, selections) {
            //  Ext.getCmp("shareRecord").down('#add').setDisabled(selections.length == 0);
            Ext.getCmp("shareRecord").down('#edit').setDisabled(selections.length == 0);
        }
    }
});
ShareRecordStore.on('beforeload', function () {
    Ext.apply(ShareRecordStore.proxy.extraParams,
      {
          trial_id: document.getElementById("trial_id").value,
          relation_id: "",
          isSecret: true
      });
});




var edit_ShareRecordStore = Ext.create('Ext.data.Store', {
    model: 'gigade.ShareRecordModel',
    proxy: {
        type: 'ajax',
        url: '/PromotionsAmountTrial/GetShareRecordList',
        reader: {
            type: 'json',
            root: 'data',
            totalProperty: 'totalCount'
        }
    }
});
edit_ShareRecordStore.on('beforeload', function () {
    Ext.apply(edit_ShareRecordStore.proxy.extraParams,
      {
          trial_id: document.getElementById("trial_id").value,
          relation_id: "",
          isSecret: false
      });
});
//頁面載入
Ext.onReady(function () {
    var shareRecord = Ext.create('Ext.grid.Panel', {
        id: 'shareRecord',
        store: ShareRecordStore,
        width: document.documentElement.clientWidth,
        columnLines: true,
        frame: true,
        columns: [
            { header: "編號", dataIndex: 'share_id', width: 60, align: 'center' },
            { header: "活動編號", dataIndex: 'trial_id', width: 100, align: 'center' },
            {
                header: USERNAME, dataIndex: 'user_name', width: 80, align: 'center',
                renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {//secretcopy
                    return "<span onclick='SecretLogin(" + record.data.share_id + "," + record.data.user_id + ",\"" + info_type + "\")'  >" + value + "</span>";

                }
            },
            {
                header: SHARECONTENT, dataIndex: 'content', width: 180, align: 'center'
            },
            { header: SHARETIME, dataIndex: 'share_time', width: 150, align: 'center' },
                {
                    header: "狀態", dataIndex: 'status', width: 150, align: 'center', renderer: function (value) {
                        if (value == 0) {
                            return "新建立";
                        }
                        else if (value == 1) {
                            return "顯示";
                        }
                        else if (value == 2) {
                            return "隱藏";
                        }
                        else if (value == 3) {
                            return "下檔";
                        }
                    }
                },


        ],
        tbar: [
            { xtype: 'button', text: EDIT, id: 'edit', hidden: false, iconCls: 'icon-user-edit', disabled: true, handler: onEditClick }

        ],
        bbar: Ext.create('Ext.PagingToolbar', {
            store: ShareRecordStore,
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
        items: [shareRecord],
        renderTo: Ext.getBody(),
        autoScroll: true,
        listeners: {
            resize: function () {
                shareRecord.width = document.documentElement.clientWidth;
                this.doLayout();
            }
        }
    });
    //ToolAuthority();
    QueryToolAuthorityByUrl('/PromotionsAmountTrial/ShareRecord');
    ShareRecordStore.load({ params: { start: 0, limit: 25 } });
});
function SecretLogin(rid, info_id, info_type) {//secretcopy
    var secret_type = "5";//參數表中的"試用試吃活動"
    var url = "/PromotionsAmountTrial/ShareRecord";
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
    var row = Ext.getCmp("shareRecord").getSelectionModel().getSelection();
    if (row.length == 0) {
        Ext.Msg.alert(INFORMATION, NO_SELECTION);
    } else if (row.length > 1) {
        Ext.Msg.alert(INFORMATION, ONE_SELECTION);
    } else if (row.length == 1) {
        var secret_type = "5";//參數表中的"會員查詢列表"
        var url = "/PromotionsAmountTrial/ShareRecord/Edit ";
        var ralated_id = row[0].data.share_id;
        var info_id = row[0].data.user_id;
        boolPassword = SaveSecretLog(url, secret_type, ralated_id);//判斷5分鐘之內是否有輸入密碼
        if (boolPassword != "-1") {
            if (boolPassword) {//驗證
                SecretLoginFun(secret_type, ralated_id, true, false, true, url, info_type, info_id, secret_info);//先彈出驗證框，關閉時在彈出顯示框
            } else {
                editFunction(ralated_id);
            }
        }
        //editFunction(row[0], ShareRecordStore);
    }
}


