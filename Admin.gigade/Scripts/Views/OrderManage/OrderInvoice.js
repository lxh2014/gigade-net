
var pageSize = 25;
/**********************************************************************站臺管理主頁面**************************************************************************************/
//站臺管理Model
Ext.define('gigade.Status', {
    extend: 'Ext.data.Model',
    fields: [
        { name: "invoice_id", type: "int" },//發票編號
        { name: "invoice_number", type: "string" },//發票號碼
        { name: "tax_type", type: "int" },//營業稅
        { name: "free_tax", type: "string" },//免稅金額
        { name: "tax_amount", type: "string" },//營業稅
        { name: "total_amount", type: "string" },//應稅金額
        { name: "invoice_status", type: "int" },//狀態--1.存入資料庫2上傳至發票機3上傳至財政部
        { name: "invoice_attribute", type: "int" },//屬性---1，開立發票2作廢發票3折讓發票4金額異動5二聯轉三聯
        { name: "open_date", type: "string" }//開立時間
    ]
});
var InvoiceMasterStore = Ext.create('Ext.data.Store', {
    autoDestroy: true,
    pageSize: pageSize,
    model: 'gigade.Status',
    proxy: {
        type: 'ajax',
        url: '/OrderManage/GetInvoiceMasterList',
        reader: {
            type: 'json',
            root: 'data',
            totalProperty: 'totalCount'
        }
    }
    //    autoLoad: true
});
InvoiceMasterStore.on('beforeload', function () {
    Ext.apply(InvoiceMasterStore.proxy.extraParams,
        {
            Order_Id: window.parent.GetOrderId()

        });
});

Ext.onReady(function () {
    var StatusGrid = Ext.create('Ext.grid.Panel', {
        id: 'StatusGrid',
        store: InvoiceMasterStore,
        width: document.documentElement.clientWidth,
        columnLines: true,
        frame: true,
        columns: [
            { header: "發票編號", dataIndex: 'invoice_id', width: 60, align: 'center' },
            { header: "發票號碼", dataIndex: 'invoice_number', width: 200, align: 'center' },
            { header: "營業稅", dataIndex: 'tax_type', width: 60, align: 'center',renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                switch (record.data.tax_type) {
                    case 1:
                        return "應稅";
                    case 2:
                        return "零稅率";
                    case 3:
                        return "免稅";
                    default:
                        return record.date.tax_type;
                }
            }
            },
            { header: "免稅金額", dataIndex: 'free_tax', width: 120, align: 'center' },
            { header: "營業稅", dataIndex: 'tax_amount', width: 120, align: 'center' },
            { header: "應稅金額", dataIndex: 'total_amount', width: 120, align: 'center' },
            { header: "狀態", dataIndex: 'invoice_status', width: 200, align: 'center', renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                switch (record.data.invoice_status) {
                    case 1:
                        return "存入資料庫";
                    case 2:
                        return "上傳至發票機";
                    case 3:
                        return "上傳至財政部";
                    default:
                        return record.date.invoice_status;
                }
                }
            },
            {
                header: "屬性", dataIndex: 'invoice_attribute', width: 180, align: 'center', renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                    switch (record.data.invoice_attribute) {
                      
                        case 1:
                            return "開立發票";
                        case 2:
                            return "作廢發票";
                        case 3:
                            return "折讓發票";
                        case 4:
                            return "金額異動";
                        case 5:
                            return "二聯轉三聯";
                        default:
                            return record.date.invoice_attribute;
                    }
                }
            },
            {
                header: "開立時間", dataIndex: 'open_date', width: 120, align: 'center'
               
            }
          
        ],

        bbar: Ext.create('Ext.PagingToolbar', {
            store: InvoiceMasterStore,
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
        items: [StatusGrid],
        renderTo: Ext.getBody(),
        autoScroll: true,
        listeners: {
            resize: function () {
                StatusGrid.width = document.documentElement.clientWidth;
                this.doLayout();
            }
        }
    });
    ToolAuthority();
    InvoiceMasterStore.load({ params: { start: 0, limit: 25 } });
});
