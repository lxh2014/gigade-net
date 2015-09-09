var CallidForm;
var pageSize = 25;

//密碼Model
Ext.define('gigade.ProductClick', {
    extend: 'Ext.data.Model',
    fields: [
        { name: "product_id", type: "string" },
        { name: "product_name", type: "string" },
        { name: "brand_name", type: "string" },
        { name: "prod_classify", type: "string" },
        { name: "click_year", type: "string" },
        { name: "click_month", type: "string" },
        { name: "click_day", type: "string" },
        { name: "click_total", type: "string" }
    ]
});
var ProductClickStore = Ext.create('Ext.data.Store', {
    autoDestroy: true,
    pageSize: pageSize,
    model: 'gigade.ProductClick',
    proxy: {
        type: 'ajax',
        url: '/ProductClick/GetProductClickList',
        reader: {
            type: 'json',
            root: 'data',
            totalProperty: 'totalCount'
        }
    }
});

ProductClickStore.on('beforeload', function () {
    Ext.apply(ProductClickStore.proxy.extraParams, {
        product_status: Ext.getCmp('product_status').getValue(),
        brand_id: Ext.getCmp('brand_id').getValue(),
        prod_classify: Ext.getCmp('prod_classify').getValue(),
        product_id: Ext.getCmp('product_id').getValue(),
        type: Ext.getCmp('type').getValue(),
        startdate: Ext.getCmp('startdate').getValue(),
        enddate: Ext.getCmp('enddate').getValue()
    });
});
Ext.onReady(function () {
    Ext.tip.QuickTipManager.init();
    var gdProductClick = Ext.create('Ext.grid.Panel', {
        id: 'gdProductClick',
        store: ProductClickStore,
        flex: 8.2,
        width: document.documentElement.clientWidth,
        columnLines: true,
        frame: true,
        columns: [
            { header: "流水號", xtype: 'rownumberer', flex: 0.5, align: 'center' },
            { header: "商品編號", dataIndex: 'product_id', flex: 0.5, align: 'center' },
            {
                header: "商品名稱", dataIndex: 'product_name', flex: 2, align: 'center',
                renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                    cellmeta.style = 'overflow:visible;padding:3px 3px 3px 5px;white-space:normal';
                    return value;
                }
            },
            { header: "品牌名稱", dataIndex: 'brand_name', flex: 2, align: 'center' },
            {
                header: "商品館別", dataIndex: 'prod_classify', flex: 1, align: 'center',
                renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                    switch (value) {
                        case "0":
                            break;
                        case "10":
                            return "食品館";
                            break;
                        case "20":
                            return "用品館";
                            break;
                    }
                }
            },
            { header: "年", dataIndex: 'click_year', flex: 1, id: 'year', align: 'center' },
            { header: "月", dataIndex: 'click_month', flex: 1, id: 'month', align: 'center' },
            { header: "日", dataIndex: 'click_day', flex: 1, id: 'day', align: 'center' },
            { header: "點擊次數", dataIndex: 'click_total', flex: 1, align: 'center' }
        ],
        tbar: [{ xtype: 'button', id: 'excel', hidden: false, icon: '../../../Content/img/icons/excel.gif', handler: OutExcel }],
        bbar: Ext.create('Ext.PagingToolbar', {
            store: ProductClickStore,
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
        items: [frm, gdProductClick],
        renderTo: Ext.getBody(),
        autoScroll: true,
        listeners: {
            resize: function () {
                gdProductClick.width = document.documentElement.clientWidth;
                this.doLayout();
            }
        }
    });
    ToolAuthority();
    //  ProductClickStore.load({ params: { start: 0, limit: 25 } });
});
OutExcel = function () {
    var paras = 'product_status=' + Ext.getCmp('product_status').getValue() + '&brand_id=' + Ext.getCmp('brand_id').getValue()
        + '&prod_classify=' + Ext.getCmp('prod_classify').getValue()
        + '&product_id=' + Ext.getCmp('product_id').getValue() + '&type=' + Ext.getCmp('type').getValue().type
        + '&startdate=' + Ext.Date.format(new Date(Ext.getCmp('startdate').getValue()), 'Y-m-d H:i:s') +
       '&enddate=' + Ext.Date.format(new Date(Ext.getCmp('enddate').getValue()), 'Y-m-d H:i:s');
    window.open('/ProductClick/OutProductClickExcel?' + paras);

}

