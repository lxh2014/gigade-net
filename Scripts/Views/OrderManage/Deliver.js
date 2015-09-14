
var CallidForm;
var pageSize = 25;
/**********************************************************************站臺管理主頁面**************************************************************************************/
//站臺管理Model
Ext.define('gigade.Deliver', {
    extend: 'Ext.data.Model',
    fields: [
        { name: "vendor_name_simple", type: "string" },
        { name: "deliver_id", type: "int" },
        { name: "deliver_store", type: "int" },
        { name: "deliver_code", type: "string" },
        { name: "delivertime", type: "string" },
        { name: "deliverup", type: "string" },
        { name: "deliver_note", type: "string" },
        { name: "deliver_name", type: "string" },
        { name: "Deliver_Store_Url", type: "string" }


    ]
});

var DeliverStore = Ext.create('Ext.data.Store', {
    autoDestroy: true,
    pageSize: pageSize,
    model: 'gigade.Deliver',
    proxy: {
        type: 'ajax',
        url: '/OrderManage/GetDeliver',
        reader: {
            type: 'json',
            root: 'data',
            totalProperty: 'totalCount'
        }
    }
    //    autoLoad: true
});
DeliverStore.on('beforeload', function () {
    Ext.apply(DeliverStore.proxy.extraParams,
        {
            Order_Id: window.parent.GetOrderId()

        });
});
Ext.onReady(function () {
    var DeliverGrid = Ext.create('Ext.grid.Panel', {
        id: 'DeliverGrid',
        store: DeliverStore,
        width: document.documentElement.clientWidth,
        columnLines: true,
        frame: true,
        columns: [
            { header: "出貨商", dataIndex: 'vendor_name_simple', width: 60, align: 'center' },
            { header: "出貨單編號", dataIndex: 'deliver_id', width: 200, align: 'center' },
            {
                header: "物流業者", dataIndex: 'deliver_name', width: 100, align: 'center',
                renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                    switch (record.data.deliver_store) {
                        case 1:
                        case 2:
                        case 4:
                            return Ext.String.format('<a href="{0}" target="_blank">{1}</a>', record.data.Deliver_Store_Url, record.data.deliver_name);
                            break;
                        default:
                            return record.data.deliver_name
                            break;
                    }
                    if (record.data.deliver_store == 1)
                    {
                        return Ext.String.format('<a href="{0}" target="_blank">{1}</a>', record.data.Deliver_Store_Url, record.data.deliver_name);
                    }
                }
            },
            { header: "物流單號", dataIndex: 'deliver_code', width: 100, align: 'center' },
          
            {
                header: "寄送日期", dataIndex: 'delivertime', width: 200, align: 'center',
                renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                    return record.data.delivertime.substr(0, 10);
                }
            },
            { header: "建單日期", dataIndex: 'deliverup', width: 200, align: 'center' },
            { header: "備註", dataIndex: 'deliver_note', width: 200, align: 'center' }
        ],

        bbar: Ext.create('Ext.PagingToolbar', {
            store: DeliverStore,
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
        items: [DeliverGrid],
        renderTo: Ext.getBody(),
        autoScroll: true,
        listeners: {
            resize: function () {
                DeliverGrid.width = document.documentElement.clientWidth;
                this.doLayout();
            }
        }
    });
    ToolAuthority();
    DeliverStore.load({ params: { start: 0, limit: 25 } });
});





