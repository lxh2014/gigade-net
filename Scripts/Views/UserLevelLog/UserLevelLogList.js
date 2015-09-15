var pageSize = 25;
Ext.define('gigade.UserLevelLog', {
    extend: 'Ext.data.Model',
    fields: [
        { name: "rowID", type: "int" },
        { name: "user_id", type: "int" },
        { name: "user_name", type: "string" },
        { name: "user_email", type: "string" },
        { name: "user_order_amount", type: "string" },
        { name: "ml_code_old", type: "string" },
        { name: "ml_code_new", type: "string" },
        { name: "ml_code_change_type", type: "string" },
        { name: "create_date_time", type: "string" },
        { name: "year", type: "int" },
        { name: "month", type: "int" },
        { name: "ml_code_change_type", type: "string" },

    ]
});
var UserLevelLogStore = Ext.create('Ext.data.Store', {
    model: 'gigade.UserLevelLog',
    autoLoad: false,
    proxy: {
        type: 'ajax',
        url: '/UserLevelLog/GetUserLevelLogList',
        reader: {
            type: 'json',
            root: 'data',
            totalProperty: 'totalCount'
        }
    }
});
Ext.define("gigade.VipLevel", {
    extend: 'Ext.data.Model',
    fields: [
        { name: "ml_code", type: "string" },
        { name: "ml_name", type: "string" }]
});
var VipLevelStore = Ext.create('Ext.data.Store', {
    model: 'gigade.VipLevel',
    autoLoad: true,
    proxy: {
        type: 'ajax',
        url: "/UserLevelLog/GetVipLevel",
        actionMethods: 'post',
        reader: {
            type: 'json',
            root: 'data'
        }

    }
});
UserLevelLogStore.on('beforeload', function () {
    Ext.apply(UserLevelLogStore.proxy.extraParams,
        {
            key: Ext.getCmp('keyword').getValue(),
            searchStatus: Ext.getCmp('searchStatus').getValue(),
            leveltype: Ext.getCmp('oldnewvip').getValue(),
            levelid: Ext.getCmp('viplevel').getValue()
        });
});
var searchStatusStore = Ext.create('Ext.data.Store', {
    fields: ['StatusText', 'StatusValue'],
    data: [
    { "StatusText": "會員編號", "StatusValue": "1" },
    { "StatusText": "會員姓名", "StatusValue": "2" },
    { "StatusText": "會員郵箱", "StatusValue": "3" }
    ]
});
var UserLevel = Ext.create('Ext.data.Store', {
    fields: ['levelname', 'levelid'],
    data: [
    { "levelname": "原會員等級", "levelid": "1" },
    { "levelname": "新會員等級", "levelid": "2" },
    ]
});
Ext.onReady(function () {
    var UserLevelLog = Ext.create('Ext.grid.Panel', {
        id: 'UserLevelLog',
        store: UserLevelLogStore,
        width: document.documentElement.clientWidth,
        columnLines: true,
        frame: true,
        columns: [
         // { header: "流水號", dataIndex: 'rowID', width: 60, align: 'center' },
            { header: "會員編號", dataIndex: 'user_id', width: 80, align: 'center' },
            {
                header: "會員姓名", dataIndex: 'user_name', width: 120, align: 'center',
                renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                    return "<span onclick='SecretLogin(" + record.data.user_id + ")'  >" + value + "</span>";
                }
            },
            {
                header: "會員郵箱", dataIndex: 'user_email', width: 180, align: 'center',
                renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                    return "<span onclick='SecretLogin(" + record.data.user_id + ")'  >" + value + "</span>";
                }
            },
            { header: "原會員等級", dataIndex: 'ml_code_old', width: 80, align: 'center' },
            { header: "新會員等級", dataIndex: 'ml_code_new', width: 80, align: 'center' },
            { header: "購買金額", dataIndex: 'user_order_amount', width: 120, align: 'center' },
            { header: "等級變化類型", dataIndex: 'ml_code_change_type', width: 80, align: 'center' },
            { header: "年", dataIndex: 'year', width: 80, align: 'center' },
            { header: "月", dataIndex: 'month', width: 80, align: 'center' },
            { header: "創建日期", dataIndex: 'create_date_time', width: 120, align: 'center' }
        ],
        tbar: [
            '->',
            {
                xtype: 'combobox',
                store: searchStatusStore,
                id: 'searchStatus',
                fieldLabel: '查詢條件',
                displayField: 'StatusText',
                valueField: 'StatusValue',
                labelWidth: 65,
                margin: '0 5 0 0',
                forceSelection: false,
                editable: false,
                emptyText: '請選擇...'
            },
            {
                xtype: 'textfield',
                id: 'keyword',
                labelWidth: 150,
                listeners: {
                    focus: function () {
                        var searchType = Ext.getCmp("searchStatus").getValue();
                        if (searchType == null || searchType == '' || searchType == '0') {
                            Ext.Msg.alert("提示信息", "請先選則搜索類型");
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
                 xtype: 'combobox',
                 store: UserLevel,
                 id: 'oldnewvip',
                 fieldLabel: '會員等級類型',
                 displayField: 'levelname',
                 valueField: 'levelid',
                 labelWidth: 80,
                 margin: '0 5 0 0',
                 forceSelection: false,
                 editable: false,
                 value: 1

             },
            {
                xtype: 'combobox',
                store: VipLevelStore,
                id: 'viplevel',
                fieldLabel: '會員等級',
                displayField: 'ml_name',
                valueField: 'ml_code',
                labelWidth: 65,
                margin: '0 5 0 0',
                forceSelection: false,
                editable: false,
                emptyText: '請選擇...'

            },
            {
                xtype: 'button',
                text: '查詢',
                iconCls: 'icon-search',
                handler: function () {
                    Query();
                }
            },
            {
                xtype: 'button',
                text: '重置',
                iconCls: 'ui-icon ui-icon-reset',
                handler: function () {
                    Ext.getCmp('keyword').setValue();
                    Ext.getCmp('searchStatus').setValue();
                    Ext.getCmp('oldnewvip').setValue(1),
                    Ext.getCmp('viplevel').setValue()
                }
            }
        ],
        bbar: Ext.create('Ext.PagingToolbar', {
            store: UserLevelLogStore,
            pageSize: pageSize,
            displayInfo: true,
            displayMsg: "當前顯示記錄" + ': {0} - {1}' + "總計" + ': {2}',
            emptyMsg: "沒有記錄可以顯示"
        }),
        listeners: {
            scrollershow: function (scroller) {
                if (scroller && scroller.scrollEl) {
                    scroller.clearManagedListeners();
                    scroller.mon(scroller.scrollEl, 'scroll', scroller.onElScroll, scroller);
                }
            }
        },
    });
    Ext.create('Ext.container.Viewport', {
        layout: 'fit',
        items: [UserLevelLog],
        renderTo: Ext.getBody(),
        autoScroll: true,
        listeners: {
            resize: function () {
                UserLevelLog.width = document.documentElement.clientWidth;
                this.doLayout();
            }
        }
    });
    ToolAuthority();
});
function Query() {
    if ((Ext.getCmp('keyword').getValue() == "" || Ext.getCmp('searchStatus').getValue() == null) && (Ext.getCmp('viplevel').getValue() == null||Ext.getCmp('viplevel').getValue()==undefined)) {
        Ext.Msg.alert("提示信息", "請輸入查詢條件");
        return;
    }
    Ext.getCmp('UserLevelLog').store.loadPage(1)
}
function SecretLogin(rid) {//secretcopy
    var secret_type = "14";//參數表中的"會員等級歷程"
    var url = "/UserLevelLog/Index ";
    var ralated_id = rid;
    //點擊機敏信息先保存記錄在驗證密碼是否需要輸入
    boolPassword = SaveSecretLog(url, secret_type, ralated_id);//判斷5分鐘之內是否有輸入密碼
    if (boolPassword != "-1") {//不准查看
        if (boolPassword) {//超過5分鐘沒有輸入密碼
            //參數1：機敏頁面代碼，2：機敏資料主鍵，3：是否彈出驗證密碼框,4：是否直接顯示機敏信息6.驗證通過后是否打開編輯窗口
            //  function SecretLoginFun(type, relatedID, isLogin, isShow, editO, isEdit) {
            SecretLoginFun(secret_type, ralated_id, true, true, false, url);//先彈出驗證框，關閉時在彈出顯示框

        } else {
            SecretLoginFun(secret_type, ralated_id, false, true, false, url);//先彈出驗證框，關閉時在彈出顯示框
        }
    }
}