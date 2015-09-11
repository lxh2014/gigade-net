/*
新增：【新促銷】促銷免運    add by shuangshuang0420j  20150727 15:07
修改1：
修改2：
*/
var currentRecord = { data: {} };
var pageSize = 25;

Ext.Loader.setConfig({ enabled: true });

var freightStore = Ext.create('Ext.data.Store', {
    fields: ['parameterName', 'ParameterCode'],
    data: [
           { 'parameterName': '全部', 'ParameterCode': '0' },
           { 'parameterName': '常溫', 'ParameterCode': '1' },
           { 'parameterName': '冷凍', 'ParameterCode': '2' },
    ]
});


//簡訊查詢Model
Ext.define('gigade.Ipo', {
    extend: 'Ext.data.Model',
    fields: [
    { name: "row_id", type: "int" },
    { name: "po_id", type: "string" },
    { name: "vend_id", type: "string" },
    { name: "buyer", type: "string" },
    { name: "sched_rcpt_dt", type: "string" },
    { name: "po_type", type: "string" },
    { name: "po_type_desc", type: "string" },
    { name: "cancel_dt", type: "string" },
    { name: "msg1", type: "string" },
    { name: "msg2", type: "string" },
    { name: "msg3", type: "string" },
    { name: "create_user", type: "string" },
    { name: "create_dtim", type: "string" },//user_username
    { name: "status", type: "int" },
    { name: "user_username", type: "string" }
    ]
});

var IpoStore = Ext.create('Ext.data.Store', {
    pageSize: pageSize,
    model: 'gigade.Ipo',
    proxy: {
        type: 'ajax',
        url: '/WareHouse/GetIpoList',
        reader: {
            type: 'json',
            root: 'data',
            totalProperty: 'totalCount'
        }
    }
});

Ext.define("gigade.parameter", {
    extend: 'Ext.data.Model',
    fields: [
        { name: "ParameterCode", type: "string" },
        { name: "parameterName", type: "string" }
    ]
});
var PoTypeStore = Ext.create('Ext.data.Store', {
    model: 'gigade.parameter',
    autoLoad: true,
    proxy: {
        type: 'ajax',
        url: "/WareHouse/GetPromoInvsFlgList?Type=po_type",
        //     actionMethods: 'post',
        reader: {
            type: 'json',
            root: 'data'
        }
    }
});

Ext.define('gigade.Ipod', {
    extend: 'Ext.data.Model',
    fields: [
     { name: "row_id", type: "int" },
     { name: "po_id", type: "string" },
     { name: "pod_id", type: "int" },
     { name: "ParameterCode", type: "string" },
     { name: "bkord_allow", type: "string" },
     { name: "cde_dt_incr", type: "int" },
     { name: "cde_dt_var", type: "int" },
     { name: "cde_dt_shp", type: "int" },
     { name: "pwy_dte_ctl", type: "string" },
     { name: "qty_ord", type: "int" },
     { name: "qty_damaged", type: "int" },
     { name: "qty_claimed", type: "int" },
     { name: "promo_invs_flg", type: "string" },
     { name: "req_cost", type: "string" },
     { name: "off_invoice", type: "string" },
     { name: "new_cost", type: "string" },
     { name: "freight_price", type: "int" },
     { name: "prod_id", type: "string" },
     { name: "product_id", type: "int" },
     { name: "product_name", type: "string" },
     { name: "create_user", type: "string" },
     { name: "user_username", type: "string" },
     { name: "create_dtim", type: "string" },
     { name: "change_dtim", type: "string" },
     { name: "user_username", type: "string" },
     { name: "parameterName", type: "string" },
     { name: "plst_id", type: "string" },
     { name: "spec", type: "string" },
    { name: "item_stock", type: "string" }//庫存
    ]
});

//
//採購單單身store
var IpodStore = Ext.create('Ext.data.Store', {
    autoDestroy: true,
    pageSize: pageSize,
    model: 'gigade.Ipod',
    proxy: {
        type: 'ajax',
        url: '/WareHouse/GetIpodCheck',
        reader: {
            type: 'json',
            root: 'data'
        }
    },

    listeners: {
    update: function (store, record) {
        //如果編輯的是轉移數量
        //var row_id = e.record.data.row_id;
        var qty_damaged = record.get("qty_damaged");
        var qty_claimed = record.get("qty_claimed");
        var qty_ord = record.get("qty_ord");
        if (parseInt(qty_damaged) + parseInt(qty_claimed) != parseInt(qty_ord))
        {
            Ext.Msg.alert("錯誤提示", "不允收的量 + 實際收貨量 != 下單採購量,保存失敗！");
            return false;
        }

            if (record.isModified('qty_damaged') || record.isModified('qty_claimed')) {
                Ext.Ajax.request({
                    url: '/WareHouse/UpdateIpodCheck',
                    params: {
                        row_id: record.get("row_id"),
                        qty_damaged: record.get("qty_damaged"),
                        qty_claimed: record.get("qty_claimed"),
                        item_stock: parseInt(record.get("item_stock")) + parseInt(qty_claimed),
                        
                        plst_id:"F"

                    },
                    success: function (response)
                    {
                        var res = Ext.decode(response.responseText);
                        if (res.success)
                        {
                            Ext.Msg.alert("提示信息", "驗收成功!");
                            IpodStore.load();
                        }
                        else
                        {
                            Ext.Msg.alert("提示信息", "驗收失敗!");
                            IpodStore.load();
                        }
                    },
                    failure: function () {

                        Ext.Msg.alert("提示信息", "驗收失敗!");

                    }
                });
            }
        }
    }
});


IpoStore.on("beforeload", function ()
{
    Ext.apply(IpoStore.proxy.extraParams, {
        Poid: Ext.getCmp("po_id").getValue(),
        Potype: Ext.getCmp('Poty').getValue(),
        start_time: Ext.htmlEncode(Ext.Date.format(new Date(Ext.getCmp('start_time').getValue()), 'Y-m-d 00:00:00')),
        end_time: Ext.htmlEncode(Ext.Date.format(new Date(Ext.getCmp('end_time').getValue()), 'Y-m-d 23:59:59')),
        freight:Ext.getCmp("freight").getValue()
    })
})

IpodStore.on("beforeload", function ()
{
    Ext.apply(IpodStore.proxy.extraParams, {
        ipod: Ext.getCmp("ipod") ? Ext.getCmp("ipod").getValue() : ''
    })
})

//左邊活動列表
var ipoList = Ext.create('Ext.grid.Panel', {
    id: ' ipoList',
    autoScroll: true,
    layout: 'anchor',
    height: document.documentElement.clientHeight - 12,
    border: false,
    frame: false,
    store: IpoStore,
    dockedItems: [{
        id: 'dockedItem',
        xtype: 'toolbar',
        layout: 'column',
        dock: 'top',
        items: [
        {
            xtype: 'datefield',
            margin: '5 0 0 0',
            fieldLabel: "創建時間",
            labelWidth: 70,
            id: 'start_time',
            format: 'Y-m-d',
            value: Tomorrow(1 - new Date().getDate()),
            editable: false,
            listeners: {
                select: function (a, b, c)
                {
                    var start = Ext.getCmp("start_time");
                    var end = Ext.getCmp("end_time");
                    var s_date = new Date(start.getValue());
                    end.setValue(new Date(s_date.setMonth(s_date.getMonth() + 1)));
                },
                specialkey: function (field, e)
                {
                    if (e.getKey() == Ext.EventObject.ENTER)
                    {
                        Search();
                    }
                }
            }
        },
        {
            xtype: 'displayfield',
            margin: '5 0 0 0',
            value: '~'
        },
        {
            xtype: 'datefield',
            id: 'end_time',
            format: 'Y-m-d',
            margin: '5 0 0 0',
            value: Tomorrow(0),
            editable: false,
            listeners: {
                select: function (a, b, c)
                {
                    var start = Ext.getCmp("start_time");
                    var end = Ext.getCmp("end_time");
                    var s_date = new Date(start.getValue());
                    if (end.getValue() < start.getValue())
                    {
                        Ext.Msg.alert("提示", "開始時間不能大於結束時間！");
                        end.setValue(new Date(s_date.setMonth(s_date.getMonth() + 1)));
                    }
                },
                specialkey: function (field, e)
                {
                    if (e.getKey() == Ext.EventObject.ENTER)
                    {
                        Search();
                    }
                }
            }
        },
        {
            xtype: 'textfield',
            fieldLabel: '採購單單號',
            id: 'po_id',
            name: 'po_id',
            margin: '5 0 0 0',
            width: 210,
            labelWidth: 70,
            listeners: {
                specialkey: function (field, e)
                {
                    if (e.getKey() == e.ENTER)
                    {
                        Search();
                    }
                }
            }
        },
        
        {
            xtype: 'combobox', //類型
            editable: false,
            id: 'freight',
            fieldLabel: "溫層",
            name: 'freight',
            width: 160,
            labelWidth: 35,
            margin: '5 0 5 15',
            store: freightStore,
            displayField: 'parameterName',
            valueField: 'ParameterCode',
            emptyText: "请选择溫層",
            value: '0'
        },
        {
            xtype: 'combobox', //類型
            editable: false,
            id: 'Poty',
            fieldLabel: "採購單類別",
            name: 'Poty',
            width: 210,
            labelWidth: 70,
            margin: '5 0 0 0',
            store: PoTypeStore,
            // margin: '20 0 0 0',
            lastQuery: '',
            displayField: 'parameterName',
            valueField: 'ParameterCode',
            emptyText: "请选择採購單類別",
            value: ''
        },
        {
            xtype: 'button',
            text: '查詢',
            id: 'grid_btn_search',
            iconCls: 'ui-icon ui-icon-search',
            margin: ' 5 0 10 0',
            width: 70,
            handler: Search
        },
        {
            xtype: 'button',
            text: "打印採購單",
            margin: ' 5 0 5 0',
            width: 90,
            id: 'ExportEnter',
            icon: '../../../Content/img/icons/excel.gif',
            handler: onExportEnter
        },
        ]
    }],
    columns: [
        { header: '採購單單號', dataIndex: 'po_id', align: 'center', width: 120, menuDisabled: true, sortable: false },
        { header: '採購單別描述', dataIndex: 'po_type_desc', align: 'center', width: 120, menuDisabled: true, sortable: false, flex: 1 },
        { header: "廠商代號", dataIndex: 'vend_id', width: 100, align: 'center', menuDisabled: true, sortable: false },
        { header: '創建時間', dataIndex: 'create_dtim', align: 'center', menuDisabled: true, sortable: false }

    ],

    bbar: Ext.create('Ext.PagingToolbar', {
        store: IpoStore,
        dock: 'bottom',
        pageSize: pageSize,
        displayInfo: true
    }),
    listeners: {
        itemclick: function (view, record, item, index, e)
        {
            LoadDetail(currentRecord = record);
        },
        resize: function ()
        {
            this.doLayout();
        }
    }
})

var rowEditing = Ext.create('Ext.grid.plugin.RowEditing', {
    clicksToMoveEditor: 1,
    autoCancel: false,
    clicksToEdit: 1,
    errorSummary: false,
    listeners: {
        beforeedit: function (e, eOpts)
        {
            if (e.colIdx == 0)
            {
                e.hide();
            }
            if (e.record.data.plst_id == "已驗收")
            {
                return false;
            }

        }
    }
});
Ext.grid.RowEditor.prototype.saveBtnText = "保存";
Ext.grid.RowEditor.prototype.cancelBtnText = "取消";
//Ext.grid.RowEditor.buttonAlign = "center";
var center = Ext.create('Ext.form.Panel', {
    id: 'center',
    autoScroll: true,
    border: false,
    frame: false, 
    layout: { type: 'vbox', align: 'stretch' },
    defaults: { margin: '2 2 2 2' },
    items: [
       
        {
            id: 'ipod',
            xtype: 'textfield',
            fieldLabel: "ipod",
            maxLength:18,
            labelWidth: 70,
            //regex: /^\d+$/,
            //regexText: '请输入正確的編號,訂單編號,行動電話進行查詢',
            name: 'ipod',
            allowBlank: true,
            hidden: true,
        },   
        {

            title: '採購單內容',
            xtype: 'gridpanel',
            id: 'detailist',
            autoScroll: true,
            frame: false,
            height: document.documentElement.clientHeight,
            store: IpodStore,
            plugins: [rowEditing],
            columns: [
                {
                    header: "驗收狀態", dataIndex: 'plst_id', width: 80, align: 'center',
                    renderer: function (value, cellmeta, record, rowIndex, columnIndex, store)
                    {
                        if (value=="已驗收")
                        {
                            return '<span style="color:red">'+ value +'</span> ';
                        } 
                        else
                        {

                            return '<span style="color:green">' +value + '</span> ';
                        }
                    }
                },
                { header: "商品編號", dataIndex: 'product_id', width: 100, align: 'center' },
                { header: "商品名稱", dataIndex: 'product_name', width: 300, align: 'center' },
                { header: "商品細項編號", dataIndex: 'prod_id', width: 100, align: 'center' },
                { header: "規格", dataIndex: 'spec', width: 80, align: 'center' },
                //{ header: "收貨狀態", dataIndex: 'parameterName', width: 100, align: 'center' },
                //{ header: "收貨狀態", dataIndex: 'ParameterCode', width: 100, align: 'center', hidden: true },
                {
                    header: "下單採購量", dataIndex: 'qty_ord', width: 80, align: 'center'
                },
                {
                    header: "庫存", dataIndex: 'item_stock', width: 80, align: 'center'
                },
                //{ header: "是否允許多次收貨", dataIndex: 'bkord_allow', width: 120, align: 'center' },
                {
                    header: "不允收的量", dataIndex: 'qty_damaged', flex: 1, align: 'center', editor: { xtype: 'numberfield', allowBlank: false, minValue: 0, allowDecimals: false },
                },
                { header: "實際收貨量", dataIndex: 'qty_claimed', flex: 1, align: 'center', editor: { xtype: 'numberfield', allowBlank: false, minValue: 0, allowDecimals: false } },
                //{ header: "品項庫存用途", dataIndex: 'promo_invs_flg', flex: 1, align: 'center' },
                //{ header: "訂貨價格", dataIndex: 'new_cost', flex: 1, align: 'center' },
                //{ header: "運費", dataIndex: 'freight_price', flex: 1, align: 'center' },
                //{ header: "是否有效期控管", dataIndex: 'pwy_dte_ctl', flex: 1, align: 'center' },
                //{ header: "有效期天數", dataIndex: 'cde_dt_incr', flex: 1, align: 'center' },
                //{ header: "允收天數", dataIndex: 'cde_dt_var', flex: 1, align: 'center' },
                //{ header: "允出天數", dataIndex: 'cde_dt_shp', flex: 1, align: 'center' },
                //{ header: "創建人", dataIndex: 'user_username', flex: 1, align: 'center' },
                //{ header: "創建時間", dataIndex: 'create_dtim', flex: 1, align: 'center' },
                { header: "修改人", dataIndex: 'user_username', flex: 1, align: 'center' },
                { header: "修改時間", dataIndex: 'change_dtim', flex: 1, align: 'center' }
            ],
            tbar: [
                 //{ xtype: 'button', text: "編輯", id: 'edit', iconCls: 'icon-user-edit', disabled: true, },//handler: onEditClick 
                 // { xtype: 'button', text: "刪除", id: 'delete', iconCls: 'icon-user-remove', disabled: true,  }//handler: onDeleteClick
            ],

            listeners: {
                scrollershow: function (scroller)
                {
                    if (scroller && scroller.scrollEl)
                    {
                        scroller.clearManagedListeners();
                        scroller.mon(scroller.scrollEl, 'scroll', scroller.onElScroll, scroller);
                    }
                },
                
            }

        }
    ],
    //bbar: Ext.create('Ext.PagingToolbar', {
    //    store: IpodStore,
    //    pageSize: pageSize,
    //    displayInfo: true,
    //    displayMsg: NOW_DISPLAY_RECORD + ': {0} - {1}' + TOTAL + ': {2}',
    //    emptyMsg: NOTHING_DISPLAY
    //}),
   
})

Ext.onReady(function ()
{

    Ext.QuickTips.init();
    Ext.create('Ext.Viewport', {
        id: "index",
        autoScroll: true,
        width: document.documentElement.clientWidth,
        height: document.documentElement.clientHeight,
        layout: 'border',
        items: [{
            region: 'west',//左西
            xtype: 'panel',
            autoScroll: true,
            frame: false,
            width: 420,
            margins: '5 4 5 5',
            id: 'west-region-container',
            layout: 'anchor',
            items: ipoList
        }
        ,
        {
            region: 'center',//中間
            id: 'center-region-container',
            xtype: 'panel',
            frame: false,
            layout: 'fit',
            width: 480,
            
            margins: '5 4 5 5',
            items: center
        }
        ],
        listeners: {
            resize: function ()
            {
                Ext.getCmp("index").width = document.documentElement.clientWidth;
                Ext.getCmp("index").height = document.documentElement.clientHeight;
                this.doLayout();
            },
            scrollershow: function (scroller)
            {
                if (scroller && scroller.scrollEl)
                {
                    scroller.clearManagedListeners();
                    scroller.mon(scroller.scrollEl, 'scroll', scroller.onElScroll, scroller);
                }
            }
        },
        renderTo: Ext.getBody()
    });

});
//複選框列
var cbm = Ext.create('Ext.selection.CheckboxModel', {
    listeners: {
        selectionchange: function (sm, selections)
        {
            Ext.getCmp("edit").setDisabled(selections.length == 0);
            Ext.getCmp("delete").setDisabled(selections.length == 0);
        }
    }
});


function LoadDetail(record)
{
    if (record.data.row_id == undefined || record.data.row_id == 0)
    {
        Ext.getCmp('center').getForm().reset();
        IpodStore.removeAll();
    }
    else
    {
        Ext.getCmp("ipod").setValue(record.data.po_id);
        center.getForm().loadRecord(record);
        IpodStore.load();
        //alert(Ext.getCmp("ipod").getValue());
    }
}


function Search()
{
    var potys = Ext.getCmp('Poty').getValue();
    var Poid = Ext.getCmp('po_id').getValue();
    if (potys.trim() == "" && Poid.trim() == "")
    {
        Ext.Msg.alert("提示", "請輸入採購單號或選擇查詢類別!");
        return;
    }
    IpoStore.removeAll();
    IpoStore.loadPage(1);
}

function Tomorrow(s)
{
    var d;
    d = new Date();                             // 创建 Date 对象。                               // 返回日期。
    d.setDate(d.getDate() + s);
    return d;
}

/************************匯入 匯出**************************/
function onExportEnter()
{
    var potys = Ext.getCmp('Poty').getValue();
    var Poid = Ext.getCmp('po_id').getValue();
    var start_time = Ext.htmlEncode(Ext.Date.format(new Date(Ext.getCmp('start_time').getValue()), 'Y-m-d 00:00:00'));
    var end_time = Ext.htmlEncode(Ext.Date.format(new Date(Ext.getCmp('end_time').getValue()), 'Y-m-d 23:59:59'));
    var freight = Ext.getCmp("freight").getValue();
    if (potys.trim() == "")
    {
        Ext.Msg.alert("提示", "請選擇匯出類別!");
    } else
    {
        var Potype = Ext.getCmp('Poty').getValue();
        window.open("/WareHouse/WritePdf?Poid=" + Poid + "&Potype=" + Potype + "&start_time=" + start_time + "&end_time=" + end_time + "&freight=" + freight);
    }
}