var pageSize = 25;

/**********************************************************************群組管理主頁面**************************************************************************************/
var searchStore = Ext.create('Ext.data.Store', {
    fields: ['txt', 'value'],
    data: [
          { 'txt': '所有資料', 'value': '0' },
          { 'txt': '商品名稱', 'value': '1' }
    ]
});
var dateStore = Ext.create('Ext.data.Store', {
    fields: ['txt', 'value'],
    data: [
           { 'txt': '所有日期', 'value': '0' },
            { 'txt': '轉單日期', 'value': '1' }
    ]
});
//群組管理Model
Ext.define('gigade.OrderVendorProduces', {
    extend: 'Ext.data.Model',
    fields: [
        { name: "Order_Id", type: "int" },
        { name: "Vendor_Name_Simple", type: "string" },
        { name: "Product_Name", type: "string" },
        { name: "Product_Spec_Name", type: "string" },
        { name: "Buy_Num", type: "int" },
        { name: "Order_Name", type: "string" },
        { name: "Slave_Status", type: "string" },
        { name: "Product_Mode", type: "int" },
        { name: "slave_date_delivery", type: "int" },
        { name: "Note_Order", type: "string" },
         { name: "slave_status_string", type: "string" },
          { name: "product_mode_string", type: "string" },
             { name: "spec_image", type: "string" },
          { name: "s_slave_date_delivery", type: "datetime" }
    ]
});

var OrderVendorProducesStore = Ext.create('Ext.data.Store', {
    autoDestroy: true,
    pageSize: pageSize,
    model: 'gigade.OrderVendorProduces',
    proxy: {
        type: 'ajax',
        url: '/SendProduct/OrderVendorProducesList',
        reader: {
            timeout:30000,
            type: 'json',
            root: 'data',
            totalProperty: 'totalCount'
        }
    }
});
var sm = Ext.create('Ext.selection.CheckboxModel', {
    listeners: {
        selectionchange: function (sm, selections) { }
        }
});

OrderVendorProducesStore.on('beforeload', function () {
    Ext.apply(OrderVendorProducesStore.proxy.extraParams, {
            select_type: Ext.getCmp('select_type').getValue(),
            search_con: Ext.getCmp('search_con').getValue(),
            date: Ext.getCmp('date').getValue(),
            start_time: Ext.getCmp('start_time').getValue(),
            end_time: Ext.getCmp('end_time').getValue(),
            radiostatus: Ext.getCmp('radiostatus').getValue().radiostatus,
            proModel: Ext.getCmp('proModel').getValue().proModel
        });
});

Ext.onReady(function () {
    var OrderVendorProducesPanel = Ext.create('Ext.form.Panel', {
      id: 'OrderVendorProducesPanel',
      height: 120,
      bodyPadding: 15,
      border: 0,
      width: document.documentElement.clientWidth,
        items: [
            {
                xtype: 'fieldcontainer',
                layout: 'hbox',
                items: [
                    {
                        xtype: 'combobox',
                        margin:"0 0 0 10",
                        fieldLabel: "關鍵字查詢",
                        id: 'select_type',
                        name: 'select_type',
                        store: searchStore,
                        displayField: 'txt',
                        valueField: 'value',
                        editable: false,
                        value: 0
                    },
                    {
                        xtype: 'textfield',
                        id: 'search_con',
                        name: 'search_con'
                    },
                    {
                            xtype: 'combobox',
                            fieldLabel: '日期條件',
                            margin: "0 0 0 18",
                            labelWidth: 70,
                            id: 'date',
                            name: 'date',
                            store: dateStore,
                            displayField: 'txt',
                            valueField: 'value',
                            value: '0'
                    },
                    {
                            xtype: "datetimefield",
                            id: 'start_time',
                            name: 'start_time',
                            format: 'Y-m-d',
                            editable: false,
                            value: Start()
                    },
                                  {
                                      xtype: 'displayfield',
                                      margin: '0 0 0 0',
                                      value: "~"
                                  },
                    {
                                   xtype: "datetimefield",
                                   id: 'end_time',
                                   name: 'end_time',
                                   editable: false,
                                   format: 'Y-m-d',
                                   value: End()
                    }
                ]
            },
            {
                xtype: 'fieldcontainer',
                layout: 'hbox',
                items: [
                    {
                        xtype: 'radiogroup',
                        fieldLabel: '訂單狀態',
                        layout: 'hbox',
                        margin: "0 0 0 10",
                        id: 'radiostatus',
                        width:425,
                        name: 'radiostatus',
                        defaults: {
                            flex: 1
                        },
                        columns: 5,
                        items: [
                            { id: 'id1', name: 'radiostatus', boxLabel: "所有狀態", inputValue: '-1', checked: true },
                            { id: 'id2', name: 'radiostatus', boxLabel: "待出貨 ", inputValue: '2' },
                            { id: 'id3', name: 'radiostatus', boxLabel: "已出貨", inputValue: '4' },
                            { id: 'id4', name: 'radiostatus', boxLabel: "訂單歸檔", inputValue: '99' }
                        ]
                    },
                    {
                        xtype: 'radiogroup',
                        layout: 'hbox',
                        fieldLabel: '出貨模式',
                        margin: "0 0 0 10",
                        layout: 'hbox',
                        id: 'proModel',
                        name: 'proModel',
                        width: 200,
                        columns: 2,
                        defaults: {
                            flex: 1
                        },
                        items: [
                            { id: 'model1', name: 'proModel', boxLabel: "自出", inputValue: '1', checked: true },
                            { id: 'model2', name: 'proModel', boxLabel: "調度", inputValue: '2' }
                        ]
                    }
                ]
            }
        ],
        buttonAlign: 'center',
        buttons: [
            {
                text: '查詢',
                //iconCls: 'icon-excel',
                handler: Query
            },
            {
                text: '重置',
                iconCls: 'ui-icon-reset',
                handler: function () {
                   this.up('form').getForm().reset();
                }
            },
            {
                text: '撿貨單',
                handler: function () {
                    var myMask = new Ext.LoadMask("OrderVendorProducesGrid", { msg: "正在匯出中..." });
                    myMask.show();
                    Ext.Ajax.request({
                        url: '/SendProduct/ProduceGroupCsv',
                        timeout: 30000,
                        params: {
                            select_type: Ext.getCmp('select_type').getValue(),
                            search_con: Ext.getCmp('search_con').getValue(),
                            date: Ext.getCmp('date').getValue(),
                            start_time: Ext.getCmp('start_time').getValue(),
                            end_time: Ext.getCmp('end_time').getValue(),
                            radiostatus: Ext.getCmp('radiostatus').getValue().radiostatus,
                            proModel: Ext.getCmp('proModel').getValue().proModel
                        },
                        success: function (response) {
                            var result = Ext.decode(response.responseText);
                            if (result.success) {
                                myMask.hide();
                                var url = Ext.String.format('<a href=../..{0}{1}>點此下載</a>', document.getElementById('Path').value, result.filename);
                                Ext.MessageBox.alert("下載提示", "" + url + "");
                            } else {
                                myMask.hide();
                                Ext.Msg.alert("提示信息", "下載出錯");
                            }
                        }
                    });
                }
            },
            {
                   text: '報表',
                   handler: function () {
                       var myMask = new Ext.LoadMask("OrderVendorProducesGrid", { msg: "正在匯出中..." });
                       myMask.show();
                       Ext.Ajax.request({
                           url: '/SendProduct/ProduceGroupExcel',
                        timeout: 30000,
                           params: {
                               select_type: Ext.getCmp('select_type').getValue(),
                               search_con: Ext.getCmp('search_con').getValue(),
                               date: Ext.getCmp('date').getValue(),
                               start_time: Ext.getCmp('start_time').getValue(),
                               end_time: Ext.getCmp('end_time').getValue(),
                               radiostatus: Ext.getCmp('radiostatus').getValue().radiostatus,
                               proModel: Ext.getCmp('proModel').getValue().proModel
                           },
                           success: function (response) {
                               myMask.hide();
                               var result = Ext.decode(response.responseText);
                               if (result.success) {
                                 var url = Ext.String.format('<a href=../..{0}{1}>點此下載</a>', document.getElementById('Path').value, result.filename);
                                   Ext.MessageBox.alert("下載提示", "" + url + "");
                            } else {
                                   Ext.Msg.alert("提示信息", "下載出錯");
                               }
                           }
                       });
                   }
               }
        ]
    });

    var OrderVendorProducesGrid = Ext.create('Ext.grid.Panel', {
        id: 'OrderVendorProducesGrid',
        flex: 8.8,
      store: OrderVendorProducesStore,
        width: document.documentElement.clientWidth,
        columnLines: true,
        frame: true,
        columns: [
            {
                header: "付款單號", dataIndex: 'Order_Id', width: 90, align: 'center'
            },
            {
                header: "供應商", dataIndex: 'Vendor_Name_Simple', width: 85, align: 'center'
            },
            {
                header: "商品名稱/圖示", dataIndex: 'Product_Name', width: 435, align: 'center',
                renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                    if (record.data.spec_image != "") {
                        return "<a href=" + record.data.spec_image + "><font color='#00F'>" + record.data.Product_Name + "</font></a>";
                    } else {
                        return record.data.Product_Name;
                    }
                }
            },
            { header: "規格", dataIndex: 'Product_Spec_Name', width: 400, align: 'center' },
                { header: "數量", dataIndex: 'Buy_Num', width: 60, align: 'center' },
                    { header: "訂購姓名", dataIndex: 'Order_Name', width: 90, align: 'center' },
                        {
                            header: "狀態", dataIndex: 'slave_status_string', width: 90, align: 'center', renderer: function (value) {
                                if (value == "待出貨") {
                                    return "<span style= 'color:green'>待出貨</span>";
                    } else if (value == "進倉中" || value == "已進倉") {
                                    return "已出貨";
                    } else {
                                    return value;
                                }
                            }
                        },
                         { header: "出貨模式", dataIndex: 'product_mode_string', width: 80, align: 'center' },
        { header: "出貨日期", dataIndex: 's_slave_date_delivery', width: 135, align: 'center' },
            { header: "備註", dataIndex: 'Note_Order', width: 150, align: 'center' }
        ],
        bbar: Ext.create('Ext.PagingToolbar', {
            store: OrderVendorProducesStore,
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
        layout: 'vbox',
        items: [OrderVendorProducesPanel, OrderVendorProducesGrid],
        renderTo: Ext.getBody(),
        autoScroll: true,
        listeners: {
            resize: function () {
                OrderVendorProducesGrid.width = document.documentElement.clientWidth;
                this.doLayout();
            }
        }
    });
    ToolAuthority();
    OrderVendorProducesStore.load({ params: { start: 0, limit: 25 } });
});

function Query() {
    Ext.getCmp('OrderVendorProducesGrid').store.loadPage(1, {
        params: {
            select_type: Ext.getCmp('select_type').getValue(),
            search_con: Ext.getCmp('search_con').getValue(),
            date: Ext.getCmp('date').getValue(),
            start_time: Ext.getCmp('start_time').getValue(),
            end_time: Ext.getCmp('end_time').getValue(),
            radiostatus: Ext.getCmp('radiostatus').getValue().radiostatus,
            proModel: Ext.getCmp('proModel').getValue().proModel
        }
    });
}

function End() {
    var d;
    d = new Date();                             // 创建 Date 对象。
    //d.setDate(d.getDate());
    return d;
}
function Start() {
    var d;
    d = new Date();                             // 创建 Date 对象。
    d.setDate(d.getDate()-14);
    return d;
}