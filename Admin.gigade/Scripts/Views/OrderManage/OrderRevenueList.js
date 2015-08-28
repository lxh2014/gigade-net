
Ext.Loader.setConfig({ enabled: true });
Ext.Loader.setPath('Ext.ux', '/Scripts/Ext4.0/ux');
Ext.require([
'Ext.form.Panel',
'Ext.ux.form.MultiSelect',
'Ext.ux.form.ItemSelector'
]);
var pageSize = 25;
var BrandId;
var htmljosn = '暫無數據';

var sm = Ext.create('Ext.selection.CheckboxModel', {
    listeners: {
        selectionchange: function (sm, selections) {

            Ext.getCmp("OrderBrandProducesListGrid").down('#Remove').setDisabled(selections.length == 0);
            Ext.getCmp("OrderBrandProducesListtGrid").down('#Edit').setDisabled(selections.length == 0)
        }
    }
});
//查詢條件
var searchStatusrStore = Ext.create('Ext.data.Store', {
    fields: ['txt', 'value'],
    data: [
    { "txt": "所有資料", "value": "0" },
    { "txt": "商品名稱", "value": "1" },
    { "txt": "會員編號", "value": "2" },
    ]
});
/*賣場，chanel,幾家地，雅虎商城*/
Ext.define('gigade.Channel', {
    extend: 'Ext.data.Model',
    fields: [
    { name: "channel_id", type: "string" },
    { name: "channel_name_simple", type: "string" }
    ]
});
var ChannelStore = Ext.create('Ext.data.Store', {
    autoDestroy: true,
    model: 'gigade.Channel',
    proxy: {
        type: 'ajax',
        url: '/OrderManage/GetChannel',
        actionMethods: 'post',
        reader: {
            type: 'json',
            root: 'data'
        }
    }

});


Ext.onReady(function () {
    var frm = Ext.create('Ext.form.Panel', {
        id: 'frm',
        height: 150,
        border: 0,
        width: document.documentElement.clientWidth,
        items: [
        {
            xtype: 'fieldcontainer',
            layout: 'hbox',
            margin: '10 0 10 0',
            items: [
            {
                xtype: 'combobox',
                fieldLabel: "品牌列表",
                editable: false,
                hidden: false,
                id: 'Brand_Id',
                name: 'Brand_Id',
                store: VendorBrandStore,
                displayField: 'Brand_Name',
                valueField: 'Brand_Id',
                typeAhead: true,
                forceSelection: false,
                queryMode: 'local',
                multiSelect: true, //多選
                margin: '0 0 0 5',
                listeners:
                {
                    beforerender: function () {
                        VendorBrandStore.load({
                            callback: function () {
                                VendorBrandStore.insert(0, { Brand_Id: '0', Brand_Name: '全選' });
                                Ext.getCmp("Brand_Id").setValue(VendorBrandStore.data.items[0].data.Brand_Id);
                            }
                        });
                    },
                    select: function (a, b, c) {
                        if (Ext.getCmp('Brand_Id').getValue() != -1) {
                            BrandId = Ext.getCmp('Brand_Id').getValue();
                        }
                        else {
                            BrandId = "";
                        }
                    }
                }
            },
            {
                xtype: 'combobox',
                allowBlank: true,
                fieldLabel: '關鍵字查詢',
                hidden: false,
                id: 'select_type',
                name: 'select_type',
                store: searchStatusrStore,
                queryMode: 'local',
                displayField: 'txt',
                valueField: 'value',
                typeAhead: true,
                forceSelection: false,
                editable: false,
                margin: '0 0 0 5',
                value: 0
            },
            {
                xtype: 'textfield',
                fieldLabel: "查詢內容",
                margin: '0 0 0 5',
                id: 'search_con',
                labelWidth: 60,
                name: 'search_con',
                 regex: /^((?!%).)*$/,
                 regexText: "禁止輸入百分號",
                listeners: {
                    specialkey: function (field, e) {
                        if (e.getKey() == Ext.EventObject.ENTER) {
                            Query();
                        }
                    }
                }
            }]
        },
        {
            xtype: 'fieldcontainer',
            layout: 'hbox',

            items: [
            {            
                xtype: 'combobox',
                id: 'slave_status',
                name: 'slave_status',
                fieldLabel: "訂單狀態",
                store: paymentStore,
                displayField: 'remark',
                valueField: 'ParameterCode',
                editable: false,
                typeAhead: true,
                forceSelection: false,
                queryMode: 'local',
                margin: '0 0 0 5',
                emptyText: SELECT,
                listeners:
               {
                   beforerender: function () {
                       paymentStore.load({
                           callback: function () {
                               paymentStore.insert(0, { ParameterCode: '-1', remark: '所有狀態' });
                               Ext.getCmp("slave_status").setValue(paymentStore.data.items[0].data.ParameterCode);
                           }
                       });
                   }
               }
            },
            {//付款方式                    
                xtype: 'combobox',
                fieldLabel: "付款方式",
                id: 'order_payment',
                name: 'order_payment',
                store: paymentType,
                queryMode: 'local',
                margin: '0 10 0 5',
                editable: false,
                displayField: 'parameterName',
                valueField: 'ParameterCode',
                
                emptyText: "不分",
                listeners: {
                    beforerender: function () {
                        paymentType.load();
                    }
                }
            }]            
        },
        {
            xtype: 'fieldcontainer',
            layout: 'hbox',
            items: [
            {
                xtype: 'combobox',
                fieldLabel: "賣場",
                allowBlank: true,
                hidden: false,
                displayField: 'channel_name_simple',
                valueField: 'channel_id',
                id: 'Channel_Id',
                name: 'Channel_Id',
                store: ChannelStore,
                margin: '0 0 0 5',
                typeAhead: true,
                forceSelection: false,
                editable: false,
                emptyText: '請選擇'
            },
            {
                xtype: 'combobox',
                allowBlank: true,
                fieldLabel: '管理人',
                hidden: false,
                id: 'product_manage',
                name: 'product_manage',
                store: ProductManageStore,
                margin: '0 0 0 5',
                displayField: 'userName',
                valueField: 'userId',
                typeAhead: true,
                forceSelection: false,
                editable: false,
                emptyText: '所有管理人員資料'
            }
            ]
        },
        {
            xtype: 'fieldcontainer',
            layout: 'hbox',
            items: [
            {
                xtype: "datefield",
                fieldLabel: "訂單成立日期",
                margin: '0 0 0 5',
                id: 'dateOne',
                name: 'dateOne',
                format: 'Y-m-d',
                editable: false,
                allowBlank: false,
                submitValue: true,
                value: Tomorrow(),
                listeners: {
                    select: function (a, b, c) {
                        var tstart = Ext.getCmp("dateOne");
                        var tend = Ext.getCmp("dateTwo");
                        if (tend.getValue() == null) {
                            tend.setValue(setNextMonth(tstart.getValue(),1));
                        }
                        else if (tend.getValue() < tstart.getValue()) {
                            Ext.Msg.alert(INFORMATION, "開始時間不能大於結束時間");
                            tend.setValue(setNextMonth(tstart.getValue(),1));
                        }
                    }
                }
            },
            {
                xtype: 'displayfield',
                margin: '0 26 0 27',
                value: "~"
            },
            {
                xtype: "datefield",
                format: 'Y-m-d',
                id: 'dateTwo',
                name: 'dateTwo',
                editable: false,
                allowBlank: false,
                submitValue: true,
                value: setNextMonth(Tomorrow(), 1),
                listeners: {
                    select: function (a, b, c) {
                        var tstart = Ext.getCmp("dateOne");
                        var tend = Ext.getCmp("dateTwo");
                        if (tstart.getValue() == null) {
                            tstart.setValue(setNextMonth(tend.getValue(), -1));
                        }
                        else if (tend.getValue() < tstart.getValue()) {
                            Ext.Msg.alert(INFORMATION, "開始時間不能大於結束時間");
                            tstart.setValue(setNextMonth(tend.getValue(), -1));
                        }
                    }
                }
            }]
        },
        {
            xtype: 'fieldcontainer',
            combineErrors: true,
            fieldLabel: '',
            layout: 'hbox',
            items: [
            {
                xtype: 'button',
                text: '查詢',
                iconCls: 'icon-search',
                id: 'btnQuery',
                margin: '0 10 0 10',
                handler: Query
            },
            {
                xtype: 'button',
                text: '重置',
                iconCls: 'ui-icon ui-icon-reset',
                margin: '0 10 0 10',
                id: 'Res',
                handler: function () {
                    this.up('form').getForm().reset();
                    Ext.getCmp("Brand_Id").setValue(0);
                    Ext.getCmp("slave_status").setValue(-1);
                    Ext.getCmp("order_payment").setValue("");
                }
            },
            {
                xtype: 'button',
                text: '匯出',
                margin: '0 10 0 10',
                iconCls: 'icon-excel',
                disabled: true,
                id: 'btnExcel',
                handler: Export
            }]
        }
        //{
        //    xtype: 'fieldcontainer',
        //    layout: 'hbox',
        //    items: [
        //    {
        //        xtype: 'radiogroup',
        //        fieldLabel: '訂單狀態',
        //        id: 'slave_status',
        //        name: 'slave_status',
        //        colName: 'slave_status',
        //        width: 1500,
        //        defaults: {
        //                name: 'Slave_Status'
        //        },
        //        columns: 17,
        //        items: [
        //        { id: 'id1', boxLabel: "所有狀態", inputValue: '-1', checked: true },
        //        { id: 'id2', boxLabel: "等待付款", inputValue: '0' },
        //        { id: 'id3', boxLabel: "付款失敗", inputValue: '1' },
        //        { id: 'id4', boxLabel: "待出貨", inputValue: '2' },
        //        { id: 'id5', boxLabel: "出貨中", inputValue: '3' },
        //        { id: 'id6', boxLabel: "已出貨", inputValue: '4' },
        //        { id: 'id7', boxLabel: "處理中", inputValue: '5' },
        //        { id: 'id8', boxLabel: "進倉中", inputValue: '6' },
        //        { id: 'id9', boxLabel: "已進倉", inputValue: '7' },
        //        { id: 'id10', boxLabel: "已分配", inputValue: '8' },
        //        { id: 'id11', boxLabel: "等待取消", inputValue: '10' },
        //        { id: 'id12', boxLabel: "訂單異常", inputValue: '20' },
        //        { id: 'id13', boxLabel: "單一商品取消", inputValue: '89' },
        //        { id: 'id14', boxLabel: "訂單取消", inputValue: '90' },
        //        { id: 'id15', boxLabel: "訂單退貨", inputValue: '91' },
        //        { id: 'id16', boxLabel: "訂單換貨", inputValue: '92' },
        //        { id: 'id17', boxLabel: "訂單歸檔", inputValue: '99' }
        //        ]
        //    }]            
        //},
        //{
        //    xtype: 'fieldcontainer',
        //    combineErrors: true,
        //    margin: '5 0 0 5',
        //    fieldLabel: '付款方式',
        //    width: 1500,
        //    layout: 'hbox',
        //    items: [
        //    {
        //        xtype: 'radiogroup',
        //        id: 'order_payment',
        //        name: 'order_payment',
        //        colName: 'order_payment',
        //        width: 1500,
        //        defaults: {
        //            name: 'Order_Payment'
        //        },
        //        columns: 17,
        //        items: [
        //        { id: 'OP1', boxLabel: "所有狀態", inputValue: '-1', checked: true, width: 80 },
        //        { id: 'OP2', boxLabel: "聯合信用卡", inputValue: '1', width: 80 },
        //        { id: 'OP3', boxLabel: "永豐ATM", inputValue: '2', width: 80 },
        //        { id: 'OP4', boxLabel: "藍新信用卡  ", inputValue: '3', width: 80 },
        //        { id: 'OP5', boxLabel: "支付寶", inputValue: '4', width: 80 },
        //        { id: 'OP6', boxLabel: "銀聯", inputValue: '5', width: 80 },
        //        { id: 'OP7', boxLabel: "傳真刷卡", inputValue: '6', width: 80 },
        //        { id: 'OP8', boxLabel: "延遲付款", inputValue: '7', width: 80 },
        //        { id: 'OP9', boxLabel: "黑貓貨到付款", inputValue: '8', width: 90 },
        //        { id: 'OP10', boxLabel: "現金", inputValue: '9', width: 70 },
        //        { id: 'OP11', boxLabel: "中國信託信用卡", inputValue: '10', width: 80 },
        //        { id: 'OP12', boxLabel: "中國信託信用卡紅利折抵", inputValue: '11', width: 80 },
        //        { id: 'OP13', boxLabel: "中國信託信用卡紅利折抵(10%)", inputValue: '12', width: 80 },
        //        { id: 'OP14', boxLabel: "網際威信  HiTRUST 信用卡", inputValue: '13', width: 80 },
        //        { id: 'OP15', boxLabel: "中國信託信用卡紅利折抵(20%)", inputValue: '14', width: 80 },
        //        { id: 'OP16', boxLabel: "7-11貨到付款", inputValue: '15', width: 70 }
        //        ]
        //    }]            
        //},
        ]
    });
    var OrderBrandProducesListGrid = Ext.create('Ext.panel.Panel', {
        id: 'frmhtml',
        html: htmljosn,
        renderTo: Ext.getBody()
    });
    Ext.create('Ext.Viewport', {
        layout: 'anchor',
        items: [frm, OrderBrandProducesListGrid],
        renderTo: Ext.getBody(),
        autoScroll: true,
        listeners: {
            resize: function () {
                OrderBrandProducesListGrid.width = document.documentElement.clientWidth;
                this.doLayout();
            }
        }
    });
    ToolAuthority();
})
function Tomorrow() {
    var d;
    var s = "";
    d = new Date();                             // 创建 Date 对象。
    s += d.getFullYear() + "/";                     // 获取年份。
    s += (d.getMonth() + 1) + "/";              // 获取月份。
    s += d.getDate() + 1;                       // 获取日。
    return (new Date(s));                                 // 返回日期。
}
setNextMonth = function (source, n) {
    var s = new Date(source);
    s.setMonth(s.getMonth() + n);
    return s;
}
/************匯入到Excel**ATM************/
Export = function () {
    var time1 = Ext.htmlEncode(Ext.Date.format(new Date(Ext.getCmp("dateOne").getValue()), 'Y-m-d H:i:s'));
    var time2 = Ext.htmlEncode(Ext.Date.format(new Date(Ext.getCmp("dateTwo").getValue()), 'Y-m-d H:i:s'))
    var prod = Ext.getCmp('order_payment').getValue();
    window.open("/OrderManage/GetNewOrderRevenueExprot?selecttype=" + Ext.getCmp('select_type').getValue() + "&searchcon=" + Ext.getCmp('search_con').getValue() + "&Brand_Id=" + Ext.getCmp('Brand_Id').getValue() + "&order_payment=" + prod + "&slave_status=" + Ext.getCmp('slave_status').getValue() + "&product_manage=" + Ext.getCmp('product_manage').getValue() + "&Channel_Id=" + Ext.getCmp('Channel_Id').getValue() + "&dateOne=" + time1 + "&dateTwo=" + time2);
   // window.open("/OrderManage/Export");
}
//查询
Query = function () {
    var select_type = Ext.getCmp('select_type');
    var search = Ext.getCmp('search_con');
    if (!search.isValid()) {
        Ext.Msg.alert("提示", "查詢內容格式錯誤!");
        return;
    }
    if (select_type.getValue() != 0 && search.getValue() == "") {
        Ext.Msg.alert("提示信息", "請輸入查詢內容!");
        return;
    }
    var myMask = new Ext.LoadMask(Ext.getBody(), { msg: 'Loading...' });
    myMask.show();
    Ext.Ajax.request({
        url: '/OrderManage/GetNewOrderRevenueList',
        timeout: 900000,
        method: 'post',
        params: {
            selecttype: Ext.getCmp('select_type').getValue(), //選擇查詢種類，商品名稱和會員稱號
            searchcon: Ext.getCmp('search_con').getValue(), //查詢內容
            Brand_Id: Ext.getCmp('Brand_Id').getValue(),
            dateOne: Ext.getCmp('dateOne').getValue(),
            dateTwo: Ext.getCmp('dateTwo').getValue(),
            Channel_Id: Ext.getCmp('Channel_Id').getValue(), //賣場
            product_manage: Ext.getCmp('product_manage').getValue(), //管理人員
            slave_status: Ext.htmlEncode(Ext.getCmp("slave_status").getValue()), //訂單狀態
            order_payment: Ext.htmlEncode(Ext.getCmp("order_payment").getValue())//付款方式
        },
        success: function (response) {
            myMask.hide();
            var result = Ext.decode(response.responseText);
            //alert(result.msg);
            if (result.success) {
                Ext.getCmp("btnExcel").setDisabled(false);
                htmljosn = result.msg;
                Ext.getCmp('frmhtml').update(htmljosn);
            }
            else {
                alert('系統錯誤');
            }
        },
        failure: function () {
            myMask.hide();
            Ext.Msg.alert('加載出錯！');
        }
    });
}