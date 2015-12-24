Ext.Loader.setConfig({ enabled: true });
Ext.Loader.setPath('Ext.ux', '/Scripts/Ext4.0/ux');
Ext.require([
    'Ext.form.Panel',
    'Ext.ux.form.MultiSelect',
    'Ext.ux.form.ItemSelector'
]);
//if (!Ext.grid.GridView.prototype.templates) {
//    Ext.grid.GridView.prototype.templates = {};
//}
//Ext.grid.GridView.prototype.templates.cell = new Ext.Template(
// '<td class="x-grid3-col x-grid3-cell x-grid3-td-{id} x-selectable {css}" style="{style}" tabIndex="0" {cellAttr}>',
//'<div class="x-grid3-cell-inner x-grid3-col-{id}" {attr}>{value}</div>',
// '</td>'
//);

var CallidForm;
var pageSize = 25;
var boolPassword = true;//secretcopy
var info_type = "users";
var secret_info = "user_id;user_email";
/**********************************************************************群組管理主頁面**************************************************************************************/
//群組管理Model
Ext.define('gigade.Serial', {
    extend: 'Ext.data.Model',
    fields: [
        { name: "id", type: "int" },
        { name: "serial", type: "string" },
        { name: "user_id", type: "int" },
        { name: "user_email", type: "string" },
        { name: "created", type: "string" }
    ]
});

var FaresStore = Ext.create('Ext.data.Store', {
    autoDestroy: true
    ,
    model: 'gigade.Serial',
    proxy: {
        type: 'ajax',
        url: '/PromotionsBonus/PromotionsBonusSerialHistoryLists2',
        reader: {
            type: 'json',
            totalProperty: 'totalCount',
           // root: 'items'
            root: 'data'

        }
    }
});

Ext.onReady(function () {
    FaresStore.loadPage(1, {
        params: {
            ids: GetSerialID()
        }
    });
    FaresStore.on('beforeload', function () {
        Ext.apply(FaresStore.proxy.extraParams,
            {
                ids: GetSerialID()
            });
    });
    var gdFgroup = Ext.create('Ext.grid.Panel', {
        id: 'gdFgroup',
        store: FaresStore,
        width: document.documentElement.clientWidth,
        columnLines: true,
        frame: true,
        viewConfig: {
            enableTextSelection: true,
            stripeRows: false,
            getRowClass: function (record, rowIndex, rowParams, store) {
                return "x-selectable";
            }
        },
        columns: [
            { header: XVHAO, dataIndex: 'serial', width:100, align: 'center', align: 'center' },
            {
                header: "兌換人員", dataIndex: 'user_email', width: 100, align: 'center',
                renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {//secretcopy
                    return "<span onclick='SecretLogin(" + record.data.id + "," + record.data.user_id + ",\"" + info_type + "\")'  >" + value + "</span>";
                }
            },
            { header: '兌換時間', dataIndex: 'created', width: 100, align: 'center' }
        ],
        tbar: [
            {
                xtype: 'label',
                text: '檢視記錄',
                margins: '0 0 0 10'
            },
            '->',
              {
                  xtype: "button", text: "返回序號兌換列表", id: "goback", handler: function () {
                      window.location.href = "/PromotionsBonus/index";
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

    })

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
});

function GetSerialID() {
    return document.getElementById('SerialId').value;
}
function SecretLogin(rid, info_id, info_type) {//secretcopy
    var secret_type = "5";//參數表中的"試用試吃活動"
    var url = "/PromotionsBonus/PromotionsBonusSerialHistoryList";
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