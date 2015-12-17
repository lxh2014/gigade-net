Ext.Loader.setConfig({ enabled: true });
Ext.Loader.setPath('Ext.ux', '/Scripts/Ext4.0/ux');
Ext.require([
    'Ext.form.Panel',
    'Ext.ux.form.MultiSelect',
    'Ext.ux.form.ItemSelector'
]);
var pageSize = 25;
var info_type = "vip_user";
var secret_info = "v_id;user_email";
/**********************************************************************群組管理主頁面**************************************************************************************/
//群組管理Model
Ext.define('gigade.Fares', {
    extend: 'Ext.data.Model',
    fields: [
        { name: "v_id", type: "int" },
        { name: "vuser_email", type: "string" },
        { name: "status", type: "int" },
        { name: "emp_id", type: "string" },
        { name: "screatedate", type: "string" }
    ]
});

var FaresStore = Ext.create('Ext.data.Store', {
    autoDestroy: true,
    pageSize: pageSize,
    model: 'gigade.Fares',
    proxy: {
        type: 'ajax',
        url: '/Member/GetVipUserList',
        reader: {
            type: 'json',
            root: 'data',
            totalProperty: 'totalCount'
        }
    }
});

var DDLStore = Ext.create('Ext.data.Store', {
    fields: ['txt', 'value'],
    data: [
        { "txt": "所有會員資料", "value": "0" },
        { "txt": "電子信箱", "value": "1" }

    ]
});

FaresStore.on('beforeload', function () {
    Ext.apply(FaresStore.proxy.extraParams,
        {
            serchs: Ext.getCmp('serchs').getValue(),
            serchcontent: Ext.getCmp('serchcontent').getValue(),
            groupid: document.getElementById("groupid").value
        });
});
function Query(x) {
    FaresStore.removeAll();
    Ext.getCmp("gdFgroup").store.loadPage(1, {
        params: {
            serchs: Ext.getCmp('serchs').getValue(),
            serchcontent: Ext.getCmp('serchcontent').getValue(),
            groupid: document.getElementById("groupid").value
        }
    });
}

Ext.onReady(function () {
    var gdFgroup = Ext.create('Ext.grid.Panel', {
        id: 'gdFgroup',
        store: FaresStore,
        width: document.documentElement.clientWidth,
        height: 800,
        columnLines: true,
        frame: true,
        columns: [
            {
                header: "編號", dataIndex: 'v_id', width: 100, align: 'center'
            },
            {
                header: "電子郵件", dataIndex: 'vuser_email', width: 400, align: 'center',
                renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                    return "<span onclick='SecretLogin(" + record.data.v_id + "," + record.data.v_id + ",\"" + info_type + "\")'  >" + value + "</span>";

                }
            },
            {
                header: "狀態", dataIndex: 'status', width: 70, align: 'center', renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                    if (value == 1) {
                        return "<img  id='img' src='../../../Content/img/icons/ok.png'/>";
                    }
                    else {
                        return "<img  id='img' src='../../../Content/img/icons/cross.gif'/>";
                    }
                }
            },
            { header: "員工編號", dataIndex: 'emp_id', width: 130, align: 'center' },
            { header: "建立時間", dataIndex: 'screatedate', width: 300, align: 'center' }
        ],
        tbar: [
           { xtype: 'combobox', editable: false, fieldLabel: "查詢條件", labelWidth: 60, id: 'serchs', store: DDLStore, displayField: 'txt', valueField: 'value', value: 0 },
           { xtype: 'textfield', fieldLabel: "查詢內容", id: 'serchcontent', labelWidth: 60 },
           {
               text: SEARCH,
               iconCls: 'icon-search',
               id: 'btnQuery',
               handler: Query
           },
           {
               text: RETURN,
               scale: 'small',
               style: { marginLeft: '20px' },
               handler: function () {
                   window.location.href = '/Member/VipUserGroupList';
               }

           }
        ],
        bbar: Ext.create('Ext.PagingToolbar', {
            store: FaresStore,
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
        items: [gdFgroup],
        renderTo: Ext.getBody(),
        autoScroll: true,
        listeners: {
            resize: function () {
                gdFgroup.width = document.documentElement.clientWidth;
                this.doLayout();
            }
        }
    });
    ToolAuthority();
    FaresStore.load({ params: { start: 0, limit: 25 } });
});
function SecretLogin(rid, info_id, info_type) {//secretcopy
    var secret_type = "15";
    var url = "/Manager/VipUserList?group_id=" + document.getElementById("groupid").value + "&vid=" + rid;
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