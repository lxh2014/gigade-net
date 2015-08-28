var pageSize = 25;
Ext.define('gigade.TrialProdCateModel', {
    extend: 'Ext.data.Model',
    fields: [
    { name: "id", type: "int" },
    { name: "event_id", type: "string" },
    { name: "event_name", type: "string" },
    { name: "type", type: "int" },
    { name: "category_id", type: "int" },
    { name: "category_name", type: "string" },
    { name: "product_id", type: "int" },
    { name: "product_name", type: "string" },
    { name: "start_date", type: "datetime" },
    { name: "end_date", type: "datetime" }
    ]
}); 
var TrialProdCateStore = Ext.create('Ext.data.Store', {
    pageSize: pageSize,
    model: 'gigade.TrialProdCateModel',
    proxy: {
        type: 'ajax',
        url: '/TrialProdCate/GetTrialProdCateList',
        reader: {
            type: 'json',
            root: 'data',
            totalProperty: 'totalCount'
        }
    }
});
//頁面載入
Ext.onReady(function () {
    var gdTrialProd = Ext.create('Ext.grid.Panel', {
        id: 'gdTrialProd',
        store: TrialProdCateStore,
        width: document.documentElement.clientWidth,
        columnLines: true,
        frame: true,
        viewConfig: {
            enableTextSelection: true,
            stripeRows: false,
            getRowClass: function (record, rowIndex, rowParams, store) {
                return "x-selectable";
            }
        },
        columns: [
        { header: ID, dataIndex: 'id', width: 120, align: 'center', align: 'center' },
        { header: EVENTID, dataIndex: 'event_id', width: 120, align: 'center', align: 'center' },
        { header: EVENTNAME, dataIndex: 'event_name', width: 150, align: 'center', align: 'center' },
        { header: EVENTTYPE, dataIndex: 'type', width: 120, align: 'center', align: 'center', renderer: Type },
        { header: CATEID, dataIndex: 'category_id', width: 120, align: 'center', align: 'center' },
        { header: CATE_NAME, dataIndex: 'category_name', width: 150, align: 'center', align: 'center' },
        { header: PRODID, dataIndex: 'product_id', width: 120, align: 'center', align: 'center' },
        { header: PRODNAME, dataIndex: 'product_name', width: 200, align: 'center', align: 'center' },
        { header: STARTDATE, dataIndex: 'start_date', width: 120, align: 'center', align: 'center' },
        { header: ENDDATE, dataIndex: 'end_date', width: 200, align: 'center', align: 'center' }
        ],
        tbar: [
        { xtype: 'button', text: UPDATACATE, id: 'add', hidden: false, iconCls: 'icon-user-add', handler: UpdateTrialProdCate }
        ],
        bbar: Ext.create('Ext.PagingToolbar', {
            store: TrialProdCateStore,
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
        items: [gdTrialProd],
        renderTo: Ext.getBody(),
        autoScroll: true,
        listeners: {
            resize: function () {
                gdTrialProd.width = document.documentElement.clientWidth;
                this.doLayout();
            }
        }
    });
    ToolAuthority();
    TrialProdCateStore.load({ params: { start: 0, limit: 25 } });
});
UpdateTrialProdCate = function () {
    Ext.Msg.confirm(INFO, Ext.String.format(IS_ONFIRM), function (btn) {
        if (btn == 'yes') {
            Ext.Ajax.request({
                url: '/TrialProdCate/UpdateTrialProdCate',
                method: 'post',
                success: function (form, action) {
                    TrialProdCateStore.removeAll();
                    Ext.getCmp("gdTrialProd").store.loadPage(1, {
                        params: {
                            start: 0, limit: 25
                        }
                    });
                },
                failure: function () {
                    Ext.Msg.alert(INFORMATION, FAILURE);
                }
            })
        }
    })
}
function Type(val) {
    switch (val) {
        case 1:
            return EATTRIAL;
            break;
        case 2:
            return USETRIAL;
            break;
        default:
            return BUFEN;
            break;
    }
}