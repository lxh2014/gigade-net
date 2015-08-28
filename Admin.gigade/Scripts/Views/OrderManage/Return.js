
var CallidForm;
var pageSize = 25;
/**********************************************************************站臺管理主頁面**************************************************************************************/
//站臺管理Model
Ext.define('gigade.Return', {
    extend: 'Ext.data.Model',
    fields: [
        { name: "return_id", type: "int" },
        { name: "return_status_string", type: "string" },
        { name: "vendor_name_simple", type: "string" },
        { name: "company_phone", type: "string" },
        { name: "company_zip", type: "int" },
        { name: "company_address", type: "string" },
        { name: "createdate", type: "string" },
        { name: "return_note", type: "string" },
        { name: "item_id", type: "int" },
        { name: "product_freight_set_string", type: "string" },
        { name: "product_name", type: "string" },
        { name: "buy_num", type: "int" },
        { name: "single_money", type: "int" },
        { name: "subtotal", type: "int" },
        { name: "deduct_bonus", type: "int" }
    ]
});

var ReturnStore = Ext.create('Ext.data.Store', {
    autoDestroy: true,
    pageSize: pageSize,
    model: 'gigade.Return',
    proxy: {
        type: 'ajax',
        url: '/OrderManage/GetReturn',
        reader: {
            type: 'json',
            root: 'data',
            totalProperty: 'totalCount'
        }
    }
    //    autoLoad: true
});
Ext.onReady(function () {
    var ReturnGrid = Ext.create('Ext.grid.Panel', {
        id: 'ReturnGrid',
        store: ReturnStore,
        width: document.documentElement.clientWidth,
        columnLines: true,
        frame: true,
        columns: [
            { header: "退貨單號", dataIndex: 'return_id', width: 100, align: 'center' },
            { header: "退貨單狀態", dataIndex: 'return_status_string', width: 100, align: 'center' },
            { header: "出貨商", dataIndex: 'vendor_name_simple', width: 100, align: 'center' },
            { header: "公司電話", dataIndex: 'company_phone', width: 100, align: 'center' },
            { header: "地址", dataIndex: 'company_address', width: 100, align: 'center' },
            { header: "退貨單建立時間", dataIndex: 'createdate', width: 100, align: 'center' },
            { header: "備註", dataIndex: 'return_note', width: 100, align: 'center' },
            { header: "商品編號", dataIndex: 'item_id', width: 100, align: 'center' },
            { header: "商品名稱", dataIndex: 'product_name', width: 200, align: 'center' },
            { header: "託運單屬性", dataIndex: 'product_freight_set_string', width: 200, align: 'center' },
            { header: "數量", dataIndex: 'buy_num', width: 100, align: 'center' },
            { header: "折扣價", dataIndex: 'single_money', width: 100, align: 'center' },
            { header: "小計", dataIndex: 'subtotal', width: 100, align: 'center' },
            { header: "購物金", dataIndex: 'deduct_bonus', width: 100, align: 'center' }
        ],

        bbar: Ext.create('Ext.PagingToolbar', {
            store: ReturnStore,
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
        items: [ReturnGrid],
        renderTo: Ext.getBody(),
        autoScroll: true,
        listeners: {
            resize: function () {
                ReturnGrid.width = document.documentElement.clientWidth;
                this.doLayout();
            }
        }
    });
    ToolAuthority();
    ReturnStore.load({ params: { start: 0, limit: 25 } });
});





