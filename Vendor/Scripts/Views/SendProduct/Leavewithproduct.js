var pageSize = 25;
//聲明grid
var SearchType = Ext.create('Ext.data.Store', {
    fields: ['id', 'name'],
    data: [
        { "id": "0", "name": "所有資料" },
        { "id": "1", "name": "商品名稱" }
    ]
});
var SearchTimeType = Ext.create('Ext.data.Store', {
    fields: ['id', 'name'],
    data: [
        { "id": "0", "name": "所有日期" },
        { "id": "1", "name": "出貨日期" }
    ]
});
Ext.define('GIGADE.Leaveproduct', {
    extend: 'Ext.data.Model',
    fields: [
       { name: "order_id", type: "int" }, //廠商出貨編號
        { name: "detail_id", type: "int" },//訂單編號
        { name: "product_spec_name_new", type: "string" }, //細項編號
        { name: "state", type: "string" },//商品名稱
        { name: "number_count", type: "int" },//購買數量
        { name: "chuhuoriqi", type: "string" },//
        { name: "comboxtype", type: "string" }//組合商品名稱
    ]
});
//獲取grid中的數據
var VendorListStore = Ext.create('Ext.data.Store', {
    pageSize: pageSize,
    model: 'GIGADE.Leaveproduct',
    proxy: {
        type: 'ajax',
        url: '/SendProduct/GetLeaveproductList',
        actionMethods: 'post',
        reader: {
            type: 'json',
            totalProperty: 'totalCount',
            root: 'data'
        }
    }
});

VendorListStore.on('beforeload', function () {
    Ext.apply(VendorListStore.proxy.extraParams, {
        serch_type: Ext.getCmp('serch_type').getValue(),
        searchcomment: Ext.getCmp('searchcomment').getValue(),
        serch_time_type: Ext.getCmp('serch_time_type').getValue(),
        time_start: Ext.getCmp('time_start').getValue(),
        time_end: Ext.getCmp('time_end').getValue()
    });
});

Ext.onReady(function () {
  //頁面加載時創建grid
    var VendorListGrid = Ext.create('Ext.grid.Panel', {
        id: 'VendorListGrid',
        store: VendorListStore,
        //height: 699,
        flex:1.8,
        columnLines: true,
        frame: true,
        columns: [

            { header: "付款單號", dataIndex: 'order_id', width: 100, align: 'center'},
            { header: "購物編號", dataIndex: 'detail_id', width: 150, align: 'center' },
            { header: "商品名稱", dataIndex: 'product_spec_name_new', width: 350, align: 'center' },
            { header: "數量", dataIndex: 'number_count', width: 150, align: 'center' },
            { header: "狀態", dataIndex: 'state', width: 150, align: 'center' },
            { header: "出貨日期", dataIndex: 'chuhuoriqi', width: 150, align: 'center' },
            { header: "組合模式", dataIndex: 'comboxtype', width: 150, align: 'center' }

        ],
        tbar: [
             {
                 xtype: 'combo',
                 fieldLabel: '關鍵字查詢',
                 store:SearchType,
                 id: 'serch_type',
                 queryMode: 'local',
                 name: 'serch_type',
                 displayField: 'name',
                 valueField: 'id',
                 width: 200,
                 value:0,
                 editable: false
             },
              { xtype: 'textfield', id: 'searchcomment', name: 'searchcomment', width: 160 },
                {
                    xtype: 'combo',
                    fieldLabel: '日期條件',
                    store: SearchTimeType,
                    id: 'serch_time_type',
                    queryMode: 'local',
                    name: 'serch_time_type',
                    displayField: 'name',
                    valueField: 'id',
                    width: 200,
                    value:0,
                    editable: false
                },
                  {
                      xtype: 'datefield',
                      id: 'time_start',
                      name: 'time_start',
                      margin: '0 5px 0 5px',
                      editable: false,
                      value: Tomorrow(-3)
                  },
                    {
                        xtype: 'displayfield',
                        value: '~'
                    },
                    {
                        xtype: 'datefield',
                        id: 'time_end',
                        name: 'time_end',
                        margin: '0 5px',
                        editable: false,
                        value: Tomorrow(0)
                    },
                       {
                           xtype: 'button',
                           text: "查詢",
                           iconCls: 'icon-search',
                           id: 'btnQuery',
                           handler: Query
                       },
        ],
        listeners: {
            scrollershow: function (scroller) {
                if (scroller && scroller.scrollEl) {
                    scroller.clearManagedListeners();
                    scroller.mon(scroller.scrollEl, 'scroll', scroller.onElScroll, scroller);
                }
            }
        },
        bbar: Ext.create('Ext.PagingToolbar', {
            store: VendorListStore,
            pageSize: pageSize,
            displayInfo: true,
            displayMsg: "顯示為" + ': {0} - {1}' + "共" + ': {2}',
            emptyMsg: "沒有任何數據"
        })
    });

    Ext.create('Ext.Viewport', {
        layout: 'vbox',
        items: [VendorListGrid],
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
    VendorListStore.load({ params: { start: 0, limit: 25 } });
})
function Tomorrow(i) {
    var d;
    d = new Date();                             // 创建 Date 对象。
    d.setDate(d.getDate() + i);
    return d;
}

//查询
Query = function () {
    VendorListStore.removeAll();
    Ext.getCmp("VendorListGrid").store.loadPage(1, {
        params: {
            serch_type: Ext.getCmp('serch_type').getValue(),
            searchcomment: Ext.getCmp('searchcomment').getValue(),
            serch_time_type: Ext.getCmp('serch_time_type').getValue(),
            time_start: Ext.getCmp('time_start').getValue(),
            time_end: Ext.getCmp('time_end').getValue()
        }
    });
}