

Ext.Loader.setConfig({ enabled: true });
Ext.Loader.setPath('Ext.ux', '/Scripts/Ext4.0/ux');
Ext.require([
    'Ext.form.Panel',
    'Ext.ux.form.MultiSelect',
    'Ext.ux.form.ItemSelector'
]);
var pageSize = 20;
//調度狀態 
var SchedulingStatuStore = Ext.create('Ext.data.Store', {
    fields: ['txt', 'value'],
    data: [
        { "txt": '全部', "value": "-1" },
        { "txt": '無', "value": "0" },
        { "txt": '有', "value": "1" }
    ]
});
var chuhuoStore = Ext.create('Ext.data.Store', {
    fields: ['txt', 'value'],
    data: [

        { "txt": '統倉出貨', "value": "1" },
        { "txt": '供應商自行出貨', "value": "2" }
    ]
});

//出貨篩選
var picichuhuoStore = Ext.create('Ext.data.Store', {
    fields: ['txt', 'value'],
    data: [
        { "txt": '全部', "value": "-1" },
        { "txt": '出貨中', "value": "0" },
        { "txt": '已出貨', "value": "1" }
    ]
});

var yunsongStore = Ext.create('Ext.data.Store', {
    fields: ['txt', 'value'],
    data: [
        { "txt": '全部', "value": "-1" },
        { "txt": '常溫', "value": "1" },
        { "txt": '冷凍', "value": "2" },
        { "txt": '冷藏', "value": "5" }
    ]
});


var lieyinStore = Ext.create('Ext.data.Store', {
    fields: ['txt', 'value'],
    data: [
        { "txt": '全部', "value": "-1" },
        { "txt": '檢視列印完成', "value": "1" },
        { "txt": '檢視未列印', "value": "2" },
        { "txt": '出貨列印完成', "value": "3" },
        { "txt": '出貨未列印', "value": "4" },
        { "txt": '貨運列印完成', "value": "5" },
        { "txt": '貨運未列印', "value": "6" }
    ]
});
//聲明grid
Ext.define('GIGADE.VendorList', {
    extend: 'Ext.data.Model',
    fields: [
         { name: "ticket_id", type: "string" },
         { name: "vendor_name_simple", type: "string" },
         { name: "delivery_store", type: "string" },//運送方式
         { name: "warehouse_status", type: "int" },//調度或者寄倉
         { name: "export_id", type: "string" },//冷凍 常溫
         { name: "ticket_status", type: "string" },//出貨狀態 出貨單狀態  0 出貨中 1 已出貨
         { name: "created", type: "string" },
         { name: "modified", type: "string" },
         { name: "seized_status", type: "int" },//撿貨單
         { name: "ship_status", type: "int" },//出貨明細
         { name: "Freight_status", type: "int" },//貨運單
         { name: "deliver_ys_type", type: "string" }
    ]
});
//獲取grid中的數據
var VendorListStore = Ext.create('Ext.data.Store', {
    pageSize: pageSize,
    model: 'GIGADE.VendorList',
    proxy: {
        type: 'ajax',
        url: '/SendProduct/Getnewticketslist',
        actionMethods: 'post',
        reader: {
            type: 'json',
            totalProperty: 'totalCount',
            root: 'data'
        }
    }
});

////運送方式Model
//Ext.define("gigade.statusModel", {
//    extend: 'Ext.data.Model',
//    fields: [
//        { name: "type_id", type: "string" },
//        { name: "type_name", type: "string" }]
//});
////運送方式Store
//var searchStatusrStore = Ext.create('Ext.data.Store', {
//    model: 'gigade.statusModel',
//    autoLoad: true,
//    data: [
//        { type_id: '-1', type_name: "所有狀態" },
//        { type_id: '1', type_name: "調度倉" },
//        { type_id: '0', type_name: "自行出貨" }
//    ]
//});
var sm = Ext.create('Ext.selection.CheckboxModel', {
    listeners: {
        selectionchange: function (sm, selections) {
            Ext.getCmp("VendorListGrid").down('#Export1').setDisabled(selections.length == 0)
            Ext.getCmp("VendorListGrid").down('#Export2').setDisabled(selections.length == 0)
            Ext.getCmp("VendorListGrid").down('#Export3').setDisabled(selections.length == 0)
            var model = Ext.getCmp("VendorListGrid").getSelectionModel();
            var row = Ext.getCmp("VendorListGrid").getSelectionModel().getSelection();
            if (row.length > 1) {
                for (i = 0; i < row.length; i++) {
                    var start = row[0].data.delivery_store;
                    if (start != row[i + 1].data.delivery_store) {
                        model.deselectAll();
                        Ext.Msg.alert("提示信息", "必須選擇同一物流");
                    }
                }
            }
        }
    }
});
//加載前先獲取ddl的值
VendorListStore.on('beforeload', function () {
    Ext.apply(VendorListStore.proxy.extraParams, {
        deliver_type: Ext.getCmp("deliver_type").getValue(),//批次出貨類別
        vendorcondition: Ext.getCmp("vendorcondition").getValue(),//出貨廠商
        shipment: Ext.getCmp("shipment").getValue(),//物流商
        scheduling: Ext.getCmp("scheduling").getValue(),//調度狀態
        screen: Ext.getCmp("screen").getValue(),//批次出貨狀態
        lytype: Ext.getCmp("lytype").getValue(),//列印處理狀態
        ystype: Ext.getCmp("ystype").getValue(),//運送方式
        search: Ext.getCmp("search").getValue()//搜索內容
    })
});

//var channelTpl2 = new Ext.XTemplate(//Gwjserch
//    '<a href="/SendProduct/--------?ticket_id={ticket_id}">' + "檢視" + '</a>'
//);
Ext.onReady(function () {
    var frm = Ext.create('Ext.form.Panel', {
        id: 'frm',
        layout: 'anchor',
        height: 130,
        border: 0,
        bodyPadding: 10,
        width: document.documentElement.clientWidth,
        items: [
                {
                    xtype: 'fieldcontainer',
                    combineErrors: true,
                    layout: 'hbox',
                    items: [
                               {
                                   xtype: 'combobox',
                                   id: 'deliver_type',
                                   margin: '0 5px',
                                   fieldLabel: '批次出貨類別',
                                   colName: 'deliver_type',
                                   queryMode: 'local',
                                   editable: false,
                                   store: chuhuoStore,
                                   displayField: 'txt',
                                   valueField: 'value',
                                   value: 1,
                                   listeners: {
                                       select: function () {
                                           if (Ext.getCmp("deliver_type").getValue() == 1) {
                                               Ext.getCmp("shipment").show();
                                               Ext.getCmp("gongyinshang").hide();
                                               Ext.getCmp("gongyinshang").setValue(0);
                                           }
                                           else if (Ext.getCmp("deliver_type").getValue() == 2) {
                                               Ext.getCmp("gongyinshang").show();
                                               Ext.getCmp("shipment").hide();
                                               Ext.getCmp("shipment").setValue(0);
                                           }
                                       }
                                   }
                               },
                               {
                                   xtype: 'combobox',
                                   id: 'vendorcondition',
                                   margin: '0 5px',
                                   fieldLabel: '出貨廠商',
                                   colName: 'vendorcondition',
                                   queryMode: 'local',
                                   editable: false,
                                   store: VendorConditionStore,
                                   displayField: 'vendor_name_simple',
                                   valueField: 'vendor_id',
                                   listeners: {
                                       beforerender: function () {
                                           VendorConditionStore.load({
                                               callback: function () {
                                                   VendorConditionStore.insert(0, { vendor_id: '0', vendor_name_simple: '全部' });
                                                   Ext.getCmp("vendorcondition").setValue(VendorConditionStore.data.items[0].data.vendor_id);
                                               }
                                           });
                                       }
                                   }
                               },
                              {
                                  xtype: 'combobox',
                                  id: 'shipment',
                                  margin: '0 5px',
                                  fieldLabel: '物流商',
                                  colName: 'shipment',
                                  queryMode: 'local',
                                  editable: false,
                                  store: DeliverStore,
                                  displayField: 'parameterName',
                                  valueField: 'parameterCode',
                                  listeners: {
                                      beforerender: function () {
                                          DeliverStore.load({
                                              callback: function () {
                                                  DeliverStore.insert(0, { parameterCode: '0', parameterName: '全部' });
                                                  Ext.getCmp("shipment").setValue(DeliverStore.data.items[0].data.parameterCode);
                                              }
                                          });
                                      }
                                  }
                              },
                                {//供應商
                                    xtype: 'combobox',
                                    id: 'gongyinshang',
                                    margin: '0 5px',
                                    fieldLabel: '供應商',
                                    colName: 'gongyinshang',
                                    queryMode: 'local',
                                    editable: false,
                                    store: CateStore,
                                    displayField: 'vendor_name_simple',
                                    valueField: 'vendor_id',
                                    hidden: true,
                                    listeners: {
                                        beforerender: function () {
                                            CateStore.load({
                                                callback: function () {
                                                    CateStore.insert(0, { vendor_id: '0', vendor_name_simple: '全部' });
                                                    Ext.getCmp("gongyinshang").setValue(CateStore.data.items[0].data.vendor_id);
                                                }
                                            });
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
                                    xtype: 'combobox',
                                    id: 'scheduling',
                                    margin: '0 5px',
                                    fieldLabel: '調度狀態',
                                    colName: 'scheduling',
                                    queryMode: 'local',
                                    editable: false,
                                    store: SchedulingStatuStore,
                                    displayField: 'txt',
                                    valueField: 'value',
                                    value: -1
                                },
                                {
                                    xtype: 'combobox',
                                    id: 'screen',
                                    margin: '0 5px',
                                    fieldLabel: '批次出貨狀態',
                                    colName: 'screen',
                                    queryMode: 'local',
                                    editable: false,
                                    store: picichuhuoStore,
                                    displayField: 'txt',
                                    valueField: 'value',
                                    value: 0
                                },
                                 {
                                     xtype: 'combobox',
                                     id: 'lytype',
                                     margin: '0 5px',
                                     fieldLabel: '列印處理狀態',
                                     colName: 'lytype',
                                     queryMode: 'local',
                                     editable: false,
                                     store: lieyinStore,
                                     displayField: 'txt',
                                     valueField: 'value',
                                     value: -1
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
                                        id: 'ystype',
                                        margin: '0 5px',
                                        fieldLabel: '運送方式',
                                        colName: 'ystype',
                                        queryMode: 'local',
                                        editable: false,
                                        store: yunsongStore,
                                        displayField: 'txt',
                                        valueField: 'value',
                                        value: -1
                                    },
                                      {
                                          xtype: 'textfield',
                                          fieldLabel: '搜索',
                                          id: 'search',
                                          colName: 'search',
                                          margin: '0 5px',
                                          //hidden: true,
                                          submitValue: false,
                                          name: 'productid'
                                      }

                        ]
                    },
                    {
                        xtype: 'fieldcontainer',
                        combineErrors: true,
                        layout: 'hbox',
                        items: [
                                   {
                                       xtype: 'displayfield',
                                       width: 115
                                   },

                                  {
                                      xtype: 'button',
                                      iconCls: 'icon-search',
                                      text: "查詢",
                                      handler: Query
                                  },
                                  {
                                      xtype: 'displayfield',
                                      width: 48
                                  },
                                   {
                                       xtype: 'button',
                                       text: '重置',
                                       id: 'btn_reset',
                                       iconCls: 'ui-icon ui-icon-reset',
                                       listeners: {
                                           click: function () {
                                               Ext.getCmp("deliver_type").setValue(1);
                                               Ext.getCmp("vendorcondition").setValue(0);
                                               Ext.getCmp("shipment").setValue(0);
                                               Ext.getCmp("scheduling").setValue(-1);
                                               Ext.getCmp("screen").setValue(-1);
                                               Ext.getCmp("lytype").setValue(-1);
                                               Ext.getCmp("ystype").setValue(-1);
                                               Ext.getCmp("search").setValue("");
                                           }
                                       }
                                   }
                        ]
                    }
        ]
    });

    //頁面加載時創建grid
    var VendorListGrid = Ext.create('Ext.grid.Panel', {
        id: 'VendorListGrid',
        store: VendorListStore,
        flex: 8.8,
        columnLines: true,
        frame: true,
        columns: [
            { header: "批次出貨單編號", dataIndex: 'ticket_id', width: 100, align: 'center' },
            { header: "出貨廠商", dataIndex: 'vendor_name_simple', width: 150, align: 'center' },
            { header: "物流業者", dataIndex: 'delivery_store', width: 150, align: 'center', hidden: true },
            { header: "物流業者", dataIndex: 'deliver_ys_type', width: 150, align: 'center' },
            {
                header: "調度", dataIndex: 'warehouse_status', width: 80, align: 'center',
                renderer: function (value) {
                    if (value == 1) {
                        return "調度";
                    } else {
                        return "";
                    }
                }
            },
            {
                header: "運送方式", dataIndex: 'export_id', width: 180, align: 'center',
                renderer: function (value) {
                    if (value == 2) {
                        return "常溫";
                    } else if (value == 92) {
                        return "冷凍";
                    }
                }
            },
            {
                header: "狀態", dataIndex: 'ticket_status', width: 100, align: 'center', renderer: function (value) {
                    if (value == 0) {
                        return "出貨中";
                    } else if (value == 1) {
                        return "已出貨";
                    }
                }
            },
            { header: "建立時間", dataIndex: 'created', width: 150, align: 'center' },
            { header: "更新時間", dataIndex: 'modified', width: 150, align: 'center' },
            {
                header: "撿貨單", dataIndex: 'seized_status', width: 100, align: 'center',
                renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                    if (value == 1) {
                        return "<img hidValue='0' id='img" + record.data.ticket_id + "' src='../../../Content/img/icons/ok.PNG'/><a href='javascript:void(0);' onclick='OnlyExportpdfjhd("+record.data.ticket_id+")'>撿貨單</a>";
                    } else {
                        return "<img hidValue='1' id='img" + record.data.ticket_id + "' src='../../../Content/img/icons/close.PNG'/><a href='javascript:void(0);' onclick='OnlyExportpdfjhd(" + record.data.ticket_id + ")'>撿貨單</a>";
                    }
                }
            },
            {
                header: "出貨明細", dataIndex: 'ship_status', width: 100, align: 'center',
                renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                    if (value == 1) {
                        return "<img hidValue='0' id='img" + record.data.ticket_id + "' src='../../../Content/img/icons/ok.PNG'/><a href='javascript:void(0);' onclick='OnlyDeliversPDF(" + record.data.ticket_id + ")'>出貨明細</a>";
                    } else {
                        return "<img hidValue='1' id='img" + record.data.ticket_id + "' src='../../../Content/img/icons/close.PNG'/><a href='javascript:void(0);' onclick='OnlyDeliversPDF(" + record.data.ticket_id + ")'>出貨明細</a>";
                    }
                }
            },
            {
                header: "貨運單", dataIndex: 'Freight_status', width: 100, align: 'center',
                renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                    if (value == 1) {
                        return "<img hidValue='0' id='img" + record.data.ticket_id + "' src='../../../Content/img/icons/ok.PNG'/><a href='javascript:void(0);' onclick='OnlyGetXls(" + record.data.ticket_id + "," + record.data.delivery_store + ")'>貨運單</a>";
                    } else {
                        return "<img hidValue='1' id='img" + record.data.ticket_id + "' src='../../../Content/img/icons/close.PNG'/><a href='javascript:void(0);' onclick='OnlyGetXls(" + record.data.ticket_id + "," + record.data.delivery_store + ")'>貨運單</a>";
                    }
                }
            },
            //{ header: "檢視", xtype: 'templatecolumn', width: 80, tpl: channelTpl2, align: 'center' },
            {
                header: "檢視", dataIndex: 'ticket_id', width: 80, align: 'center',
                renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                    return '<a href=javascript:TranToDetial("/SendProduct/SubPoenaDetails","' + value + '")>檢視</a> ';
                }
            }
        ],
        listeners: {
            scrollershow: function (scroller) {
                if (scroller && scroller.scrollEl) {
                    scroller.clearManagedListeners();
                    scroller.mon(scroller.scrollEl, 'scroll', scroller.onElScroll, scroller);
                }
            }
        },
        tbar: [
          { xtype: 'button', id: 'Export1', text: '匯出撿貨單', disabled: true, handler: SomeExportpdfjhd },
          { xtype: 'button', id: 'Export2', text: '匯出出貨明細', disabled: true, handler: DeliversPDF },
          { xtype: 'button', id: 'Export3', text: '匯出貨運單', disabled: true, handler: GetXls }, '->',
          { xtype: 'button', id: 'Export4', text: '下載印單軟體', disabled: false, handler: UpdownIt }
           //{
           //    xtype: 'displayfield',
           //    id:'xiazai',
           //    value: '<a href="http://admin.gimg.tw:8080/AdbeRdr950_zh_TW.exe">' + "下載印單軟體" + '</a>'
           //},
           // {
           //     xtype: 'displayfield',
           //     value: '    '
           // }
        ],
        bbar: Ext.create('Ext.PagingToolbar', {
            store: VendorListStore,
            pageSize: pageSize,
            displayInfo: true,
            displayMsg: NOW_DISPLAY_RECORD + ': {0} - {1}' + TOTAL + ': {2}',
            emptyMsg: NOTHING_DISPLAY
        }),
        selModel: sm
    });

    Ext.create('Ext.Viewport', {
        layout: 'vbox',
        items: [frm, VendorListGrid],
        renderTo: Ext.getBody(),
        autoScroll: true,
        listeners: {
            resize: function () {
                VendorListGrid.width = document.documentElement.clientWidth;
                this.doLayout();
            }
        }
    });

    ToolAuthority();
    VendorListStore.load({ params: { start: 0, limit: 20 } });
});
function Tomorrow() {
    var d;
    var s = "";
    d = new Date();                             // 创建 Date 对象。
    s += d.getFullYear() + "/";                     // 获取年份。
    s += (d.getMonth() + 1) + "/";              // 获取月份。
    s += d.getDate() + 1;                          // 获取日。
    return (new Date(s));                                 // 返回日期。
}

onAddClick = function () {
    editFunction(null, VendorListStore);
}

onEditClick = function () {
    var row = Ext.getCmp("VendorListGrid").getSelectionModel().getSelection();
    if (row.length == 0) {
        Ext.Msg.alert(INFORMATION, NO_SELECTION);
    } else if (row.length > 1) {
        Ext.Msg.alert(INFORMATION, ONE_SELECTION);
    } else if (row.length == 1) {
        editFunction(row[0], VendorListStore);
    }
}

function ExportCSV() {
    Ext.Ajax.request({
        url: "/Vendor/VendorExportCSV",
        params: {
            dateType: Ext.getCmp('ven_type').getValue(),
            dateCon: Ext.getCmp('search_con').getValue(),
            dateStatus: Ext.getCmp('ven_status').getValue(),
            dateOne: Ext.getCmp('dateOne').getValue(),
            dateTwo: Ext.getCmp('dateTwo').getValue()
        },
        success: function (response) {
            if (response.responseText.split(',')[0] == "true") {
                window.location.href = '../../ImportUserIOExcel/' + response.responseText.split(',')[1];
            }
        }
    });
}


//查询
Query = function () {
    VendorListStore.removeAll();
    Ext.getCmp("VendorListGrid").store.loadPage(1, {
        params: {
            deliver_type: Ext.getCmp("deliver_type").getValue(),//批次出貨類別
            vendorcondition: Ext.getCmp("vendorcondition").getValue(),//出貨廠商
            shipment: Ext.getCmp("shipment").getValue(),//物流商
            scheduling: Ext.getCmp("scheduling").getValue(),//調度狀態
            screen: Ext.getCmp("screen").getValue(),//批次出貨狀態
            lytype: Ext.getCmp("lytype").getValue(),//列印處理狀態
            ystype: Ext.getCmp("ystype").getValue(),//運送方式
            search: Ext.getCmp("search").getValue()//搜索內容
        }
    });
}

//跳轉到出傳票明細頁
function TranToDetial(url, ticket_id) {
    var urlTran = url + '?ticket_id=' + ticket_id;
    var panel = window.parent.parent.Ext.getCmp('ContentPanel');
    var copy = panel.down('#eledetial');
    if (copy) {
        copy.close();
    }
    copy = panel.add({
        id: 'eledetial',
        title: "傳票明細",
        html: window.top.rtnFrame(urlTran),
        closable: true
    });
    panel.setActiveTab(copy);
    panel.doLayout();
}

//更改活動狀態(設置活動可用與不可用)
function UpdateActive(id, fatherId) {
    var activeValue = $("#img" + id).attr("hidValue");
    $.ajax({
        url: "/ProductCategory/UpdateActive",
        data: {
            "id": id,
            "active": activeValue
        },
        type: "POST",
        dataType: "json",
        success: function (msg) {
            ProductCategoryStore.remove();
            if (activeValue == 1) {
                $("#img" + id).attr("hidValue", 0);
                $("#img" + id).attr("src", "../../../Content/img/icons/accept.gif");
                ProductCategoryStore.load({ params: { father_id: fatherId } });
            } else {
                $("#img" + id).attr("hidValue", 1);
                $("#img" + id).attr("src", "../../../Content/img/icons/drop-no.gif");
                ProductCategoryStore.load({ params: { father_id: fatherId } });
            }
        },
        error: function (msg) {
            Ext.Msg.alert(INFORMATION, FAILURE);
        }
    });
}

function DeliversPDF() {
    var ticketIds = "";
    var row = Ext.getCmp("VendorListGrid").getSelectionModel().getSelection();
    for (var i = 0; i < row.length; i++) {
        var ticket_id = row[i].get('ticket_id');
        ticketIds += ticket_id + ',';
    }
    window.open('/SendProduct/GetDeliversPDF?ticket_id=' + ticketIds);
    Ext.Ajax.request({
        url: "/SendProduct/UpdateTicketStatus",
        params: {
            tickets: ticketIds,
            type: 2
        },
        success: function (form, action) {
            var result = Ext.decode(form.responseText);
            if (result.success) {
                VendorListStore.load();
            }
        }
    });
}

function SomeExportpdfjhd() {
    var ticketIds = "";
    var row = Ext.getCmp("VendorListGrid").getSelectionModel().getSelection();
    for (var i = 0; i < row.length; i++) {
        var ticket_id = row[i].get('ticket_id');
        ticketIds += ticket_id + ',';
    }
    window.open('/SendProduct/GetPDF?ticket_id=' + ticketIds);
    Ext.Ajax.request({
        url: "/SendProduct/UpdateTicketStatus",
        params: {
            tickets: ticketIds,
            type:1
        },
        success: function (form, action) {
            var result = Ext.decode(form.responseText);
            if (result.success) {
                VendorListStore.load();
            }
        }
    });
}
function GetXls(id) {
    var ticketIds = "";
    var row = Ext.getCmp("VendorListGrid").getSelectionModel().getSelection();
    for (var i = 0; i < row.length; i++) {
        var ticket_id = row[i].get('ticket_id');
        ticketIds += ticket_id + ',';
    }
    if (row[0].get("delivery_store") == 16 || row[0].get("delivery_store") == 17) {
        window.open('/SendProduct/GetCarWaybillsPDF?ticket_id=' + ticketIds);
    } else if (row[0].get("delivery_store") == 42) {
        window.open('/SendProduct/GetShopbillsPDF?ticket_id=' + ticketIds);
    } else {
        window.open('/SendProduct/GetWaybillsXls?ticket_id=' + ticketIds);
    }
    Ext.Ajax.request({
        url: "/SendProduct/UpdateTicketStatus",
        params: {
            tickets: ticketIds,
            type: 3
        },
        success: function (form, action) {
            var result = Ext.decode(form.responseText);
            if (result.success) {
                VendorListStore.load();
            }
        }
    });
}

function OnlyExportpdfjhd(ticketIds) {
    //var ticketIds = "";
    //var row = Ext.getCmp("VendorListGrid").getSelectionModel().getSelection();
    //for (var i = 0; i < row.length; i++) {
    //    var ticket_id = row[i].get('ticket_id');
    //    ticketIds += ticket_id + ',';
    //}
    ticketIds = ticketIds + ',';
    window.open('/SendProduct/GetPDF?ticket_id=' + ticketIds);
}
function OnlyDeliversPDF(ticketIds) {
    //var ticketIds = "";
    //var row = Ext.getCmp("VendorListGrid").getSelectionModel().getSelection();
    //for (var i = 0; i < row.length; i++) {
    //    var ticket_id = row[i].get('ticket_id');
    //    ticketIds += ticket_id + ',';
    //}
    ticketIds = ticketIds + ',';
    window.open('/SendProduct/GetDeliversPDF?ticket_id=' + ticketIds);
}
function OnlyGetXls(ticketIds,DeliveryStore) {
    //var ticketIds = "";
    //var row = Ext.getCmp("VendorListGrid").getSelectionModel().getSelection();
    //for (var i = 0; i < row.length; i++) {
    //    var ticket_id = row[i].get('ticket_id');
    //    ticketIds += ticket_id + ',';
    //}
    ticketIds = ticketIds + ',';
    if (DeliveryStore == 16 || DeliveryStore == 17) {
        window.open('/SendProduct/GetCarWaybillsPDF?ticket_id=' + ticketIds);
    } else if (DeliveryStore == 42) {
        window.open('/SendProduct/GetShopbillsPDF?ticket_id=' + ticketIds);
    } else {
        window.open('/SendProduct/GetWaybillsXls?ticket_id=' + ticketIds);
    }
}
function UpdownIt() {
    window.open("http://admin.gimg.tw:8080/AdbeRdr950_zh_TW.exe");
}