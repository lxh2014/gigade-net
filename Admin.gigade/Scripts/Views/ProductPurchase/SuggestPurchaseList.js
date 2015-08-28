var CallidForm;
var pageSize = 25;
Ext.Loader.setConfig({ enabled: true });
Ext.Loader.setPath('Ext.ux', '/Scripts/Ext4.0/ux');
Ext.require([
    'Ext.form.Panel',
    'Ext.ux.form.MultiSelect',
    'Ext.ux.form.ItemSelector'
]);
var pageSize = 25;


//簡訊查詢Model
Ext.define('gigade.Product', {
    extend: 'Ext.data.Model',
    fields: [
        { name: "vendor_id", type: "int" },//供應商編號
        { name: "safe_stock_amount", type: "string" },//安全存量細數
        { name: "erp_id", type: "string" },//Erp廠商編號
        { name: "arrive_days", type: "string" },//採購調整系數
        { name: "sum_total", type: "int" },//購買總數
        { name: "product_id", type: "int" },//商品編號
        { name: "item_id", type: "string" },//商品細項編號
        { name: "product_name", type: "string" },//商品名稱
        { name: "spec_id_1", type: "string" },//規格1
        { name: "spec_title_1", type: "string" },//規格1
        { name: "spec_id_2", type: "string" },//規格2
        { name: "spec_title_2", type: "string" },//規格2
        { name: "product_mode_name", type: "string" },//出貨方式
        { name: "prepaid", type: "int" },//是否買斷
        { name: "item_stock", type: "string" },//庫存量
        { name: "item_alarm", type: "string" },//安全存量
        { name: "sum_total", type: "string" },//購買總數
        { name: "averageCount", type: "string" },//平均平均量
        { name: "suggestPurchaseCount", type: "string" },//建議採購數量
        { name: "min_purchase_amount", type: "string" },//最小採購量
        { name: "vendor_name_simple", type: "string" },//供應商名稱
        { name: "procurement_days", type: "string" },//供應商採購天數
        { name: "item_money", type: "string" },//售價(單價)
        { name: "item_cost", type: "string" },//成本(單價)
        { name: "product_status", type: "string" },//商品狀態
        { name: "sale_name", type: "string" },//販售狀態
        { name: "create_datetime", type: "string" },//下單採購時間
        { name: "NoticeGoods",type:"string" }//補貨通知人數
    ]
});


//查詢Store
var SuggestPurchaseStore = Ext.create('Ext.data.Store', {
    autoDestroy: true,
    pageSize: pageSize,
    model: 'gigade.Product',
    proxy: {
        type: 'ajax',
        url: '/ProductPurchase/GetSuggestPurchase',
        reader: {
            type: 'json',
            root: 'data',
            totalProperty: 'totalCount'
        }
    }
});
SuggestPurchaseStore.on('beforeload', function () {

    Ext.apply(SuggestPurchaseStore.proxy.extraParams, {
        stockScope: Ext.getCmp('stockScope').getValue().scopeValue,
        sumDays: Ext.getCmp('txtSumDays').getValue(),
        perpaid: Ext.getCmp('perpaid').getValue().perpaidValue,/*是否已下單採購*/
        Is_pod: Ext.getCmp('Is_pod').getValue().Is_podValue,/*是否 買斷*/
        vendor_name:Ext.getCmp('vendor_name').getValue(),/*供應商名稱*/
        periodDays: Ext.getCmp('txtPeriodDays').getValue()
    });
});
var sm = Ext.create('Ext.selection.CheckboxModel', {
    listeners: {
        selectionchange: function (sm, selections) {
            Ext.getCmp("SuggestPurchaseView").down('#Pod').setDisabled(selections.length == 0);
           
            // Ext.getCmp("IpoView").down('#ExportEnter').setDisabled(selections.length == 0);
        }
    }
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
    //
    if ((Ext.getCmp("txtSumDays").getValue() != "" && Ext.getCmp("txtSumDays").getValue() != null) ||
        (Ext.getCmp("txtPeriodDays").getValue() != "" && Ext.getCmp("txtPeriodDays").getValue() != null)) {
        SuggestPurchaseStore.removeAll();
        Ext.getCmp("SuggestPurchaseView").store.loadPage(1);
    } else {
        Ext.Msg.alert(INFORMATION, SEARCH_LIMIT);
    }
}
function PodQuery(x)
{
   
    var row = Ext.getCmp("SuggestPurchaseView").getSelectionModel().getSelection();
        if (row.length <= 0) {
            Ext.Msg.alert(INFORMATION, NO_SELECTION);
        } else {
            Ext.Msg.confirm(CONFIRM, Ext.String.format("確定要下採購單?", row.length), function (btn) {
                if (btn == 'yes') {
                    var rowIDs = '';
                    for (var i = 0; i < row.length; i++) {
                        if (row[i].data.create_datetime.trim() == "" ) {
                            if( row[i].data.suggestPurchaseCount != "暫不需採購"){
                            rowIDs += row[i].data.item_id + ',';
                            }
                        } else
                        {
                            Ext.Msg.alert("提示", "所選的商品有已下採購單的!");
                            return;
                        }
                    }
                    if (rowIDs.trim() == "")
                    {
                        Ext.Msg.alert("提示", "所選的商品均不需採購!");
                        return;
                    }
                    Ext.Ajax.request({
                        url: '/ProductPurchase/AddItemIpo',
                        method: 'post',
                        params: {
                            Items: rowIDs
                        },
                        success: function (form, action) {
                            Ext.Msg.alert(INFORMATION, SUCCESS);
                            SuggestPurchaseStore.load();

                        },
                        failure: function () {
                            Ext.Msg.alert(INFORMATION, FAILURE);
                            SuggestPurchaseStore.load();
                        }
                    });
                }
            });
        }
  
}

Ext.onReady(function () {
    var frm = Ext.create('Ext.form.Panel', {
        id: 'frm',
        layout: 'anchor',
        height: 120,
        flex: 2,
        border: 0,
        bodyPadding: 10,
        width: document.documentElement.clientWidth,
        items: [
            {
                xtype: 'fieldcontainer',
                combineErrors: true,
                layout: 'hbox',
               // margin: '10,0,0,0',
                items: [
                   
                    //{
                    //    xtype: 'checkboxfield',
                    //    id: 'chbAllStock',
                    //    name: 'chbAllStock',
                    //    boxLabel: '所有庫存',
                    //    width: 120,
                    //    value: -1
                    //}
                    {
                        xtype: 'radiogroup',
                        id: 'stockScope',
                        name: 'stockScope',
                        fieldLabel: "查詢條件",
                        colName: 'stockScope',
                        width: 300,
                        labelWidth: 60,
                        defaults: {
                            name: 'scopeValue'
                        },
                        columns: 3,
                        //                vertical: true,
                        items: [
                            { id: 'scope1', boxLabel: "所有庫存", inputValue: '0', checked: true, width: 65 },
                            { id: 'scope2', boxLabel: "庫存<=0", inputValue: '1' },
                            { id: 'scope3', boxLabel: "需採購", inputValue: '2' }
                            //,
                            //{ id: 'scope4', boxLabel: "大於5小於等於10", inputValue: '3' },
                            //{ id: 'scope5', boxLabel: "需採購", inputValue: '4' }
                        ]
                    },
                     {
                         xtype: 'radiogroup',
                         id: 'perpaid',
                         name: 'perpaid',
                         fieldLabel: "是否買斷",
                         colName: 'perpaid',
                         labelWidth: 60,
                         margin:'0 0 0 20',
                         width: 250,
                         defaults: {
                             name: 'perpaidValue'
                         },
                         columns: 3,
                         //                vertical: true,
                         items: [
                             { id: 'perpaid1', boxLabel: "全部", inputValue: '-1', checked: true, width: 65 },
                             { id: 'perpaid2', boxLabel: "是", inputValue: '1' },
                             { id: 'perpaid3', boxLabel: "否", inputValue: '0' }
                         ]
                     }

                ]
            },
             {
                 xtype: 'fieldcontainer',
                 combineErrors: true,
                 layout: 'hbox',
                 // margin: '10,0,0,0',
                 items: [
                         {
                             xtype: 'radiogroup',
                             id: 'Is_pod',
                             name: 'Is_pod',
                             fieldLabel: "是否已下單採購",
                             colName: 'Is_pod',
                             labelWidth: 100,
                             width: 290,
                             defaults: {
                                 name: 'Is_podValue'
                             },
                             columns: 3,
                             //                vertical: true,
                             items: [
                                 { id: 'Is_pod1', boxLabel: "全部", inputValue: '0', checked: true, width: 65 },
                                 { id: 'Is_pod2', boxLabel: "是", inputValue: '1' },
                                 { id: 'Is_pod3', boxLabel: "否", inputValue: '2' }
                             ]
                         },
                         {
                             xtype: 'textfield',
                             fieldLabel: "供應商名稱",
                             margin: '0 0 0 25',
                             id: 'vendor_name',
                             labelWidth: 80,
                             name: 'vendor_name',
                             listeners: {
                                 specialkey: function (field, e) {
                                     if (e.getKey() == Ext.EventObject.ENTER) {
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
                //margin: '10,0,0,0',
                items: [
                    {
                        xtype: 'numberfield',
                        fieldLabel: "總天數",
                        id: 'txtSumDays',
                        labelWidth: 60,
                        hideTrigger: true,//隱藏箭頭
                        mouseWheelEnabled: true,//鼠標滾動加減數字
                        allowDecimals: false,//小數
                        allowNegative: true,//是否為負
                        minValue: 1,
                        value: 90,
                        listeners: {
                            specialkey: function (field, e) {
                                if (e.getKey() == e.ENTER) {
                                    Query(1);
                                }
                            }
                        }
                    },
                    {
                        xtype: 'numberfield',
                        fieldLabel: "週期天數",
                        id: 'txtPeriodDays',
                        hideTrigger: true,
                        mouseWheelEnabled: true,
                        allowDecimals: false,
                        allowNegative: true,
                        margin: '0 0 0 20',
                        labelWidth: 60,
                        minValue: 1,
                        value: 7,
                        listeners: {
                            specialkey: function (field, e) {
                                if (e.getKey() == e.ENTER) {
                                    Query(1);
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
                        xtype: 'button',
                        text: SEARCH,
                        margin: '0,0, 5,40',
                        //iconCls: 'icon-search',
                        margin: '0 10 0 20',
                        id: 'btnQuery',
                        iconCls: 'icon-search',
                        handler: Query
                    },
                     {
                         xtype: 'button',
                         text: "重置",
                         margin: '0,0, 5,40',
                         //iconCls: 'icon-search',
                         margin: '0 10 0 20',
                         id: 'btnresult',
                         iconCls: 'ui-icon ui-icon-reset',
                         handler: function () {
                             this.up('form').getForm().reset();
                         }
                     }
                ]
            }
        ]
    });

    var SuggestPurchaseView = Ext.create('Ext.grid.Panel', {
        id: 'SuggestPurchaseView',
        store: SuggestPurchaseStore,
        flex: 8,
        width: document.documentElement.clientWidth,
        columnLines: true,
        frame: true,
        columns: [
            { header: "供應商編號", dataIndex: 'vendor_id', width: 100, align: 'center' },
            { header: "供應商簡稱", dataIndex: 'vendor_name_simple', width: 80, align: 'center' },
            { header: "商品編號", dataIndex: 'product_id', width: 60, align: 'center' },
            { header: "商品細項編號", dataIndex: 'item_id', width: 80, align: 'center' },//product_name
            { header: "商品名稱", dataIndex: 'product_name', width: 150, align: 'center' },
            {
                header: "規格1",
                dataIndex: 'spec_title_1',
                width: 100,
                align: 'center',
                renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                    return record.data.spec_title_1 + record.data.spec_id_1;
                }
            },
            {
                header: "規格2",
                dataIndex: 'spec_title_2',
                width: 100,
                align: 'center',
                renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                    return record.data.spec_title_2 + record.data.spec_id_2
                }
            },
            {
                header: "出貨方式", dataIndex: 'product_mode_name', width: 80, align: 'center'
                //,
                //renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                //    if (value == 1) {
                //        return "自出";
                //    }
                //    if (value == 2) {
                //        return "寄倉";
                //    }
                //    if (value == 3) {
                //        return "調度";
                //    } else {
                //        return value;
                //    }
                //}
            },
            {
                header: "是否買斷", dataIndex: 'prepaid', width: 80, align: 'center',
                renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                    if (value == 0) {
                        return "否";
                    }
                    if (value == 1) {
                        return "是";
                    }
                    else {
                        return value;
                    }
                }
            },
            { header: "庫存量", dataIndex: 'item_stock', width: 80, align: 'center' },
            { header: "安全存量", dataIndex: 'item_alarm', width: 80, align: 'center' },
            { header: "購買總數", dataIndex: 'sum_total', width: 80, align: 'center' },
            { header: "週期平均量", dataIndex: 'averageCount', width: 80, align: 'center' },//平均平均量
            { header: "建議採購量", dataIndex: 'suggestPurchaseCount', width: 80, align: 'center' },//建議採購量
            { header: "最小採購量", dataIndex: 'min_purchase_amount', width: 80, align: 'center' },
           
            { header: "供應商採購天數", dataIndex: 'procurement_days', width: 90, align: 'center' },
            { header: "補貨通知人數", dataIndex: 'NoticeGoods', width: 90, align: 'center' },
            { header: "售價(單價)", dataIndex: 'item_money', width: 80, align: 'center' },
            { header: "成本(單價)", dataIndex: 'item_cost', width: 80, align: 'center' },
            { header: "販售狀態", dataIndex: 'sale_name', width: 80, align: 'center' },
            { header: "下單採購時間", dataIndex: 'create_datetime', width: 80, align: 'center' }
        ],
        tbar: [
            { xtype: 'button', text: "匯出Excel", id: 'exportExcel', icon: '../../../Content/img/icons/excel.gif', handler: ExportExcel },
            { xtype: 'button', text: "下單採購", id: 'Pod', disabled: true, handler: PodQuery }
    
        ],
        bbar: Ext.create('Ext.PagingToolbar', {
            store: SuggestPurchaseStore,
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
        , selModel: sm
    });

    Ext.create('Ext.container.Viewport', {
        layout: 'vbox',
        items: [frm, SuggestPurchaseView],//
        renderTo: Ext.getBody(),
        autoScroll: true,
        listeners: {
            resize: function () {
                SuggestPurchaseView.width = document.documentElement.clientWidth;
                this.doLayout();
            }
        }
    });
    ToolAuthority();
    // SuggestPurchaseStore.load({ params: { start: 0, limit: 25 } });
});


function Tomorrow(days) {
    var d;
    var s = "";
    d = new Date();                             // 创建 Date 对象。
    s += d.getFullYear() + "/";                     // 获取年份。
    s += (d.getMonth() + 1) + "/";              // 获取月份。
    s += d.getDate() + days;                          // 获取日。
    return (new Date(s));                                 // 返回日期。
}
///匯出EXcel
ExportExcel = function () {
    var stockScope= Ext.getCmp('stockScope').getValue().scopeValue;
    var sumDays=Ext.getCmp('txtSumDays').getValue();
    var perpaid= Ext.getCmp('perpaid').getValue().perpaidValue;/*是否已下單採購*/
    var Is_pod= Ext.getCmp('Is_pod').getValue().Is_podValue;/*是否 買斷*/
    var vendor_name = Ext.getCmp('vendor_name').getValue();/*供應商名稱*/
    var periodDays = Ext.getCmp('txtPeriodDays').getValue();
    window.open("/ProductPurchase/ReportSuggestPurchaseExcel?stockScope=" + stockScope + "&sumDays=" + sumDays + "&periodDays=" + periodDays + "&perpaid=" + perpaid + "&Is_pod=" + Is_pod + "&vendor_name=" + vendor_name);
}