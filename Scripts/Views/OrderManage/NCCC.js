Ext.Loader.setConfig({ enabled: true });
Ext.Loader.setPath('Ext.ux', '/Scripts/Ext4.0/ux');
Ext.require([
    'Ext.form.Panel',
    'Ext.ux.form.MultiSelect',
    'Ext.ux.form.ItemSelector'
]);
var CallidForm;
var pageSize = 25;
/**********************************************************************站臺管理主頁面**************************************************************************************/
//站臺管理Model
Ext.define('gigade.NCCC', {
    extend: 'Ext.data.Model',
    fields: [
        { name: "nccc_id", type: "int" },
        { name: "order_id", type: "int" },
        { name: "responsecode", type: "string" },
        { name: "merchantid", type: "string" },
        { name: "transdate", type: "string" },
        { name: "terminalid", type: "string" },
        { name: "transtime", type: "string" },
        { name: "pan", type: "string" },
        { name: "transamt", type: "string" },
        { name: "transcode", type: "string" },
        { name: "approvecode", type: "string" },
        { name: "responsemsg", type: "string" },
        { name: "bankname", type: "string" },
        { name: "nccc_createdates", type: "string" },
             { name: "pan_bankname", type: "string" },
        
    ]
});

var NCCCStore = Ext.create('Ext.data.Store', {
    autoDestroy: true,
    pageSize: pageSize,
    model: 'gigade.NCCC',
    proxy: {
        type: 'ajax',
        url: '/OrderManage/GetNCCC',
        reader: {
            type: 'json',
            root: 'data',
            totalProperty: 'totalCount'
        }
    }
    //    autoLoad: true
}); NCCCStore.on('beforeload', function () {
    Ext.apply(NCCCStore.proxy.extraParams,
        {
            Order_Id: window.parent.GetOrderId(),
            isSecret:'true',

        });
});
Ext.onReady(function () {
    var NCCCGrid = Ext.create('Ext.grid.Panel', {
        id: 'NCCCGrid',
        store: NCCCStore,
        width: document.documentElement.clientWidth,
        columnLines: true,
        frame: true,
        columns: [
            { header: "流水號", dataIndex: 'nccc_id', width: 80, align: 'center' },
            {
                header: "刷卡狀態", dataIndex: 'responsecode', width: 60, align: 'center',
                renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                    if (record.data.responsecode=='00'||record.data.responsecode=='08'||record.data.responsecode=='11') {
                        return "成功";
                    }
                    else {
                        return '<font color="red">失敗</font>';
                    }
                }                
            },
            { header: "特約商店代號", dataIndex: 'merchantid', width: 100, align: 'center' },
            { header: "交易日期", dataIndex: 'transdate', width: 80, align: 'center' },
            { header: "端末機代號", dataIndex: 'terminalid', width: 100, align: 'center' },
            { header: "交易時間", dataIndex: 'transtime', width: 80, align: 'center' },
            {
                header: "交易卡號", dataIndex: 'pan_bankname', width: 150, align: 'center',
                    renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                        return "<a href='#' onclick='SecretLogin(" + record.data.nccc_id + ")'  >" + value + "</a>";
                    }
            },
            { header: "交易金額", dataIndex: 'transamt', width: 80, align: 'center' },
            { header: "交易代碼", dataIndex: 'transcode', width: 100, align: 'center' },
            {
                header: "回應碼", dataIndex: 'responsecode', width: 300, align: 'center',//xtype: 'templatecolumn',
                renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                    // return Ext.String.format("<a href='javascript:void({0});'  onclick='TransToVendor()'>{1}</a>", record.data.export_id, value);
                    //  return '<a href="#"  onclick="showVendorDetail(' + record.data.export_id + ')">' + value+'</a>';
                    return value + '<a href="#"  onclick="TranToCreditCard()">【查詢代碼】</a>聯合信用卡中心:02-27151754分機:0';
                }
            },
            //{
            //    header: "回應碼", dataIndex: 'responsecode', width: 300, align: 'center', sortable: false,//xtype: 'templatecolumn',
            //     //renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
            //     //    return Ext.String.format('<a href=javascript:TranToCreditCard()  target="_blank">【查詢代碼】</a>' + "聯合信用卡中心:02-27151754分機:0");
            //     //},
            //     tpl: Ext.create('Ext.XTemplate',
            //        '<tpl >',
            //           '<div>'+ record.data.responsecode  +'</div>' + '<a href=javascript:TranToCreditCard() >【查詢代碼】</a>' + '<div>聯合信用卡中心:02-27151754分機:0</div>',
            //           // '<a href=javascript:TranToCreditCard() >【查詢代碼】</a>' + '<div>聯合信用卡中心:02-27151754分機:0</div>',
            //        '</tpl>'),
            //    menuDisabled: true  
            //},
            { header: "授權碼", dataIndex: 'approvecode', width: 100, align: 'center' },
            { header: "回應訊息", dataIndex: 'responsemsg', width: 100, align: 'center' },
            //{ header: "發卡銀行", dataIndex: 'bankname', width: 100, align: 'center' },
            { header: "時間", dataIndex: 'nccc_createdates', width: 180, align: 'center' }
           
        ],

        bbar: Ext.create('Ext.PagingToolbar', {
            store: NCCCStore,
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
        items: [NCCCGrid],
        renderTo: Ext.getBody(),
        autoScroll: true,
        listeners: {
            resize: function () {
                NCCCGrid.width = document.documentElement.clientWidth;
                this.doLayout();
            }
        }
    });
    ToolAuthority();
    NCCCStore.load({ params: { start: 0, limit: 25 } });
});

function TranToCreditCard() {
    var url = '/OrderManage/CreditCardMsg';
    var panel = window.parent.parent.Ext.getCmp('ContentPanel');
    var copy = panel.down('#credit');
    if (copy) {
        copy.close();
    }
    copy = panel.add({
        id: 'credit',
        title: '信用卡回傳信息',
        html: window.top.rtnFrame(url),
        closable: true
    });
    panel.setActiveTab(copy);
    panel.doLayout();
}


function SecretLogin(rid) {//secretcopy
    var secret_type = "22"; 
    var url = "/OrderManage/NCCC ";
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



