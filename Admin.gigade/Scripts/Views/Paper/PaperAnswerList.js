var CallidForm;
var pageSize = 25;
var p_id = document.getElementById('p_id').value;
var user_id = document.getElementById('u_id').value;
var boolPassword = true;//secretcopy
var info_type = "users";
var secret_info = "user_email;user_id";
//商品主料位管理Model
Ext.define('gigade.PaperAnswer', {
    extend: 'Ext.data.Model',
    fields: [
        { name: "answerID", type: "string" },
        { name: "paperID", type: "string" },
        { name: "paperName", type: "string" },
        { name: "userid", type: "string" },
        { name: "userMail", type: "string" },
        { name: "order_id", type: "string" },
        { name: "classID", type: "string" },
        { name: "className", type: "string" },
        { name: "answerContent", type: "string" },
        { name: "classContent", type: "string" },
        { name: "classType", type: "string" },
        { name: "answerDate", type: "string" }
    ]
});

var PaperAnswerStore = Ext.create('Ext.data.Store', {
    autoDestroy: true,
    pageSize: pageSize,
    model: 'gigade.PaperAnswer',
    proxy: {
        type: 'ajax',
        url: '/Paper/GetPaperAnswerList',
        reader: {
            type: 'json',
            root: 'data',
            totalProperty: 'totalCount'
        }
    }
    //    autoLoad: true
});
var sm = Ext.create('Ext.selection.CheckboxModel', {
    listeners: {
        selectionchange: function (sm, selections) {
            Ext.getCmp("gdPaperAnswer").down('#edit').setDisabled(selections.length == 0);
        }
    }
});

Ext.define('gigade.Paper', {
    extend: 'Ext.data.Model',
    fields: [
        { name: "paperID", type: "int" },
        { name: "paperName", type: "string" }]
});
var PaperStore = Ext.create('Ext.data.Store', {
    autoDestroy: true,
    //pageSize: pageSize,
    model: 'gigade.Paper',
    //autoLoad: true,
    proxy: {
        type: 'ajax',
        url: '/Paper/GetPaperList?isPage=false',
        actionMethods: 'post',
        reader: {
            type: 'json',
            root: 'data'
            //totalProperty: 'totalCount'
        }
    }
    //    autoLoad: true
});
PaperAnswerStore.on('beforeload', function () {
    Ext.apply(PaperAnswerStore.proxy.extraParams, {
        paper_id: Ext.getCmp('paper').getValue(),
        //paper_id:p_id,
        user_id: user_id
    });
});


Ext.onReady(function () {
    var gdPaperAnswer = Ext.create('Ext.grid.Panel', {
        id: 'gdPaperAnswer',
        title: '問卷內容',
        store: PaperAnswerStore,
        width: document.documentElement.clientWidth,
        columnLines: true,
        frame: true,
        columns: [
            { header: "流水號", dataIndex: 'answerID', width: 80, align: 'center' },
            //{ header: "問卷編號", dataIndex: 'paperID', width: 80, align: 'center' },
            { header: "問卷名稱", dataIndex: 'paperName', width: 150, align: 'center' },
            { header: "用戶編號", dataIndex: 'userid', width: 80, align: 'center' },
            {
                header: "用戶郵箱", dataIndex: 'userMail', width: 150, align: 'center',
                renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {//secretcopy
                    return "<span onclick='SecretLogin(" + record.data.answerID + ")'  >" + value + "</span>";

                }
            },
            //{ header: "題目編號", dataIndex: 'classID', width: 80, align: 'center' },
            { header: "訂單編號", dataIndex: 'order_id', width: 80, align: 'center' },
            { header: "題目名稱", dataIndex: 'className', width: 150, align: 'center' },
            {
                header: "答案", dataIndex: '', width: 200, align: 'center',
                renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                    switch (record.data.classType) {
                        case "SC":
                        case "MC":
                            return record.data.classContent;
                        case "SL":
                        case "ML":
                            return record.data.answerContent;
                    }
                }
            },
            {
                header: "題目類型", dataIndex: 'classType', width: 80, align: 'center',
                renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                    switch (value) {
                        case "SC":
                            return "單選";
                        case "MC":
                            return "多選";
                        case "SL":
                            return "單行";
                        case "ML":
                            return "多行";
                    }
                }
            },
            { header: "作答時間", dataIndex: 'answerDate', width: 150, align: 'center' }
        ],
        tbar: [
            {
                xtype: 'button', text: '匯出Excel', id: 'outexcel', hidden: false, handler: function () {
                    var paper_id = Ext.getCmp('paper').getValue();
                    window.open('/Paper/OutExcel?paper_id=' + paper_id + '&user_id=' + user_id);
                }
            },
         '->',
         {
             xtype: 'combobox', editable: false, fieldLabel: "問卷名稱", labelWidth: 60, id: 'paper', store: PaperStore, queryMode: 'remote', lastQuery: '', displayField: 'paperName', valueField: 'paperID'
         },
         //{ xtype: 'combobox', editable: false, fieldLabel: "題目", labelWidth: 55, id: 'title', store: PaperStore, displayField: 'paperName', valueField: 'paperID', value: 0 },
         {
             text: SEARCH,
             iconCls: 'icon-search',
             id: 'btnQuery',
             handler: Query
         }
        ],
        bbar: Ext.create('Ext.PagingToolbar', {
            store: PaperAnswerStore,
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
        layout: 'fit',
        items: [gdPaperAnswer],
        renderTo: Ext.getBody(),
        autoScroll: true,
        listeners: {
            resize: function () {
                gdPaperAnswer.width = document.documentElement.clientWidth;
                this.doLayout();
            }
        }
    });
    ToolAuthority();

    PaperStore.load({
        callback: function () {
            PaperStore.insert(0, { paperID: '0', paperName: '請選擇' });
            if (p_id != '') {
                Ext.getCmp('paper').setValue(p_id);
            }
            else {
                Ext.getCmp("paper").setValue(0);
            }
            //PaperAnswerStore.load({ params: { start: 0, limit: 25, paper_id: Ext.getCmp('paper').getValue(), user_id: user_id } });
        }
    });
    //var p = Ext.getCmp('paper').getValue();

});
function SecretLogin(rid) {//secretcopy
    var secret_type = "10";//參數表中的"試用試吃活動"
    var url = "/Paper/PaperAnswerList";
    var ralated_id = rid;
    //點擊機敏信息先保存記錄在驗證密碼是否需要輸入
    boolPassword = SaveSecretLog(url, secret_type, ralated_id);//判斷5分鐘之內是否有輸入密碼
    if (boolPassword != "-1") {//不准查看
        if (boolPassword) {//超過5分鐘沒有輸入密碼
            //參數1：機敏頁面代碼，2：機敏資料主鍵，3：是否彈出驗證密碼框,4：是否直接顯示機敏信息6.驗證通過后是否打開編輯窗口 7:客戶信息類型user:會員 order：訂單 vendor：供應商 8：客戶id9：要顯示的客戶信息
            //  function SecretLoginFun(type, relatedID, isLogin, isShow, editO, isEdit) {
            SecretLoginFun(secret_type, ralated_id, true, true, false, url, null, null, null);//先彈出驗證框，關閉時在彈出顯示框

        } else {
            SecretLoginFun(secret_type, ralated_id, false, true, false, url, null, null, null);//先彈出驗證框，關閉時在彈出顯示框
        }
    }
}
/*************************************************************************************查詢*************************************************************************************************/
function Query(x) {
    if (Ext.getCmp('paper').getValue() != '0') {
        PaperAnswerStore.removeAll();
        Ext.getCmp("gdPaperAnswer").store.loadPage(1, {
            params: {
                paper_id: Ext.getCmp('paper').getValue(),
                user_id: user_id

            }
        });
    }
    else {
        Ext.Msg.alert(INFORMATION,"請選擇搜索條件!");
    }
}

/*************************************************************************************新增*************************************************************************************************/
onAddClick = function () {
    //addWin.show();
    editPaperFunction(null, PaperAnswerStore);
}

/*************************************************************************************編輯*************************************************************************************************/
onEditClick = function () {
    var row = Ext.getCmp("gdPaperAnswer").getSelectionModel().getSelection();
    //alert(row[0]);
    if (row.length == 0) {
        Ext.Msg.alert(INFORMATION, NO_SELECTION);
    } else if (row.length > 1) {
        Ext.Msg.alert(INFORMATION, ONE_SELECTION);
    } else if (row.length == 1) {
        editPaperFunction(row[0], PaperAnswerStore);
    }
}







