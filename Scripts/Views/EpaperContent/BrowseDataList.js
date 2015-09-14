Ext.Loader.setConfig({ enabled: true });
Ext.Loader.setPath('Ext.ux', '/Scripts/Ext4.0/ux');
Ext.require([
    'Ext.form.Panel',
    'Ext.ux.form.MultiSelect',
    'Ext.ux.form.ItemSelector'
]);
var pageSize = 25;

//出貨查詢Model
Ext.define('GIGADE.BrowseData', {
    extend: 'Ext.data.Model',
    fields: [
        { name: 'id', type: 'int' },                //id
        { name: 'user_id', type: 'string' },        //會員ID
        { name: 'user_name', type: 'string' },      //會員姓名
        { name: 'product_id', type: 'int' },        //商品ID
        { name: 'product_name', type: 'string' },   //商品名稱
        { name: 'count', type: 'int' },             //點擊次數
        { name: 'type', type: 'int' },              //類型
        { name: 'buyCount', type: 'int' }           //購買次數(有購買算一次，退貨取消也包含)
    ]
});

//Store
var BrowseDataStore = Ext.create("Ext.data.Store", {
    autoDestroy: true,
    pageSize: pageSize,
    model: 'GIGADE.BrowseData',
    autoLoad: false,
    proxy: {
        type: 'ajax',
        url: '/EpaperContent/BrowseDataList',
        reader: {
            type: 'json',
            root: 'data',
            totalProperty: 'totalCount'
        }
    }
});
BrowseDataStore.on('beforeload', function () {
    Ext.apply(BrowseDataStore.proxy.extraParams, {
        type: Ext.getCmp("activityQuery").getValue(),
        searchContent: Ext.getCmp("searchContent").getValue(),
        isSecret:true,
    });
});

var ActivityQueryStore = Ext.create('Ext.data.Store', {
    fields: ['txt', 'value'],
    data: [
        { 'txt': '全部', 'value': '0' },
        { "txt": '瀏覽商品', "value": "1" },
        { "txt": '刪除商品', "value": "2" }
    ]
});

var idStore = Ext.create('Ext.data.Store', {
    fields: ['txtid', 'valueid'],
    data: [      
        { "txtid": '會員ID', "valueid": "1" },
        { "txtid": '商品ID', "valueid": "2" },
        { "txtid": '會員姓名', "valueid": "3" },
        { "txtid": '商品名稱', "valueid": "4" }
    ]
});

Ext.onReady(function () {
    var frm = Ext.create('Ext.form.Panel', {
        id: 'frm',
        layout: 'anchor',
        height: 130,
        border: 0,
        width: document.documentElement.clientWidth,
        items: [
            {
                xtype: 'fieldcontainer',
                combineErrors: true,
                layout: 'hbox',
                items: [
                    {
                        xtype: 'combobox',
                        id: 'activityQuery',
                        margin: '5,0,0,0',
                        fieldLabel: '類型',
                        queryMode: 'local',
                        editable: false,
                        labelWidth: 80,
                        store: ActivityQueryStore,
                        displayField: 'txt',
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
                        id: 'idQuery',
                        margin: '5,0,0,0',
                        fieldLabel: '搜尋類型',
                        queryMode: 'local',
                        editable: false,
                        labelWidth: 80,
                        store: idStore,
                        displayField: 'txtid',
                        valueField: 'valueid',
                        emptyText:'請選擇'
                    },
                    {
                        id: 'searchContent',
                        xtype: 'textfield',                    
                        width: 250,
                        margin: '5,0,0,0',
                        labelWidth: 50,
                        name: 'searchContent',
                        allowBlank: true,
                        listeners: {
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
                        xtype: 'button',
                        margin: '0 10 0 10',
                        text: "查詢",
                        handler: Query
                    },
                       {
                           xtype: 'button',
                           margin: '0 10 0 10',
                           text: "重置",
                           handler: function () {
                               Ext.getCmp('activityQuery').setValue(0);
                               Ext.getCmp('searchContent').setValue("");
                               Ext.getCmp('idQuery').reset();
                           }
                       }
                ]
            }
        ]
    });

    //頁面加載時創建grid
    var BrowseDataGrid = Ext.create('Ext.grid.Panel', {
        id: 'BrowseDataGrid',
        store: BrowseDataStore,
        //height: 720,
        flex: 1.8,
        columnLines: true,
        frame: true,
        columns: [
            { header: 'Id', dataIndex: 'id', width: 60, align: 'center' },
            { header: '會員ID', dataIndex: 'user_id', width: 100, align: 'center' },
            {
                header: '會員姓名', dataIndex: 'user_name', width: 100, align: 'center',
                renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                    return "<span onclick='SecretLogin(" + record.data.id + ")'  >" + value + "</span>";
                }
            },

            { header: '商品ID', dataIndex: 'product_id', width: 100, align: 'center' },
            { header: '商品名稱', dataIndex: 'product_name', width: 500, align: 'center' },
            { header: '點擊次數', dataIndex: 'count', width: 90, align: 'center' },
            {
                header: '類型', dataIndex: 'type', width: 90, align: 'center',
                renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                    switch (record.data.type) {
                        case 0:
                            return "";
                            break;
                        case 1:
                            return "瀏覽商品";
                            break;
                        case 2:
                            return "刪除商品";
                            break;
                        default:
                            return record.data.type;
                            break;
                    }
                }
            },
            { header: '購買次數(有購買算一次,退貨取消也包含)', dataIndex: 'buyCount', width: 250, align: 'center' }
        ],
        listeners: {
            scrollershow: function (scroller) {
                if (scroller && scroller.scrollEl) {
                    scroller.clearManagedListeners();
                    scroller.mon(scroller.scrollEl, 'scroll', scroller.onElScroll, scroller);
                }
            }
        },
        bbar: Ext.create('Ext.PagingToolbar', {
            store: BrowseDataStore,
            pageSize: pageSize,
            displayInfo: true,
            displayMsg: NOW_DISPLAY_RECORD + ': {0} - {1}' + TOTAL + ': {2}',
            emptyMsg: NOTHING_DISPLAY
        })
    });

    Ext.create('Ext.Viewport', {
        layout: 'vbox',
        items: [frm, BrowseDataGrid],
        renderTo: Ext.getBody(),
        autoScroll: true,
        listeners: {
            resize: function () {
                BrowseDataGrid.width = document.documentElement.clientWidth;
                this.doLayout();
            }
        }
    });

    ToolAuthority();
   // BrowseDataStore.load({ params: { start: 0, limit: 25 } });
});

//查询
Query = function () {
    var activityQuery = Ext.getCmp('activityQuery').getValue();
    var searchContent = Ext.getCmp('searchContent').getValue();
    var idQuery = Ext.getCmp('idQuery').getValue();
    if (activityQuery == 0 &&searchContent == "")
    {
        Ext.Msg.alert("提示信息", "請輸入查詢條件！");
        return;
    }
    if ((idQuery == "" || idQuery == null) && searchContent != "") {
        Ext.Msg.alert("提示信息", "請選擇查詢類型！");
        return;
    }
    BrowseDataStore.removeAll();
    Ext.getCmp("BrowseDataGrid").store.loadPage(1, {
        params: {
            type: Ext.getCmp("activityQuery").getValue(),
            searchContent: Ext.getCmp("searchContent").getValue(),
            isSecret: true,
            searchType: idQuery
        }
    });
}

function SecretLogin(rid) {//secretcopy
    var secret_type = "19";//參數表中的"商品點擊查詢"
    var url = "/EpaperContent/BrowseData ";
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