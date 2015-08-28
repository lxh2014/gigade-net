var pageSize = 25;
var vendor = document.getElementById("vendor_id").value;

var ProductTypeMessage = Ext.create('Ext.data.Store', {
    fields: ['txt', 'value'],
    data: [
        { "txt": '全部狀態', "value": "-1" },
        { "txt": '新建立商品', "value": "0" },
        { "txt": '申請審核', "value": "1" },
        { "txt": '上架', "value": "5" },
        { "txt": '下架', "value": "6" },
        { "txt": '供應商新建商品', "value": "20" }
    ]
});

Ext.define('gigade.NewPromoPresent', {
    extend: 'Ext.data.Model',
    fields: [
        { name: "vendor_id", type: "int" },
        { name: "product_id", type: "int" },
        { name: "product_name", type: "string" },
        { name: "vendor_name_simple", type: "string" },
        { name: "brand_name", type: "string" },
        { name: "nProductStatus", type: "string" },
        { name: "shortage", type: "int" }
    ]
});

var VendorProductStore = Ext.create('Ext.data.Store', {
    autoDestroy: true,
    pageSize: pageSize,
    model: 'gigade.NewPromoPresent',
    proxy: {
        type: 'ajax',
        url: '/VendorStock/GetVendorProductList',
        reader: {
            type: 'json',
            root: 'data',
            totalProperty: 'totalCount'
        }
    }
});

VendorProductStore.on('beforeload', function () {
    Ext.apply(VendorProductStore.proxy.extraParams, {
        vendor_id: vendor,
        searchcontent: Ext.getCmp('searchcontent').getValue(),
        product_state: Ext.getCmp('product_state').getValue()
    });
});
Ext.onReady(function () {
    var gbVendorProduct = Ext.create('Ext.grid.Panel', {
        id: 'gbVendorProduct',
        store: VendorProductStore,
        width: document.documentElement.clientWidth,
        columnLines: true,
        frame: true,
        columns: [
            { header: PRODUCTID, dataIndex: 'product_id', width: 120, align: 'center' },
            { header: PRODUCTNAME, dataIndex: 'product_name', width: 450, align: 'center' },
            { header: VENDOR, dataIndex: 'vendor_name_simple', width: 120, align: 'center' },
            { header: BRAND, dataIndex: 'brand_name', width: 150, align: 'center' },
            { header: PRODUCTSTATE, dataIndex: 'nProductStatus', width: 120, align: 'center' },
            {
                header: PURCHASESTOPSALE,
                dataIndex: 'shortage',
                id: 'controlshortage',
                //hidden: true,
                align: 'center',
                renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                    if (value == 1) {
                        return "<a href='javascript:void(0);' onclick='UpdateShortage(" + record.data.product_id + ")'><img hidValue='0' id='img" + record.data.product_id + "' src='../../../Content/img/icons/accept.gif'/></a>";
                    } else {
                        return "<a href='javascript:void(0);' onclick='UpdateShortage(" + record.data.product_id + ")'><img hidValue='1' id='img" + record.data.product_id + "' src='../../../Content/img/icons/drop-no.gif'/></a>";
                    }
                }
            },
            {
                header: FUNCTION,
                dataIndex: 'product_id',
                align: 'center',
                width: 120,
                renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                    return "<a href=javascript:onSpecEditClick(\"" + record.data.vendor_id + "\",\"" + record.data.product_id + "\")>" + STOTKCHANGE + "</a>";
                }
            }
        ],
        tbar: [
            '->',
           {
               xtype: 'combobox',
               name: 'product_state',
               id: 'product_state',
               editable: false,
               fieldLabel: "商品狀態",
               labelWidth: 60,
               store: ProductTypeMessage,
               queryMode: 'local',
               submitValue: true,
               displayField: 'txt',
               valueField: 'value',
               typeAhead: true,
               forceSelection: false,
               value: -1
           },
           {
               xtype: 'textfield', allowBlank: true, fieldLabel: "商品編號(編號之間可以,，|分割)",
               regex: /^(\d+)([,，|]{1}\d+)*(\d+)*$/,
               regexText: '輸入格式不符合要求',
               id: 'searchcontent',
               name: 'searchcontent', labelWidth: 200,
               listeners: {
                   specialkey: function (field, e) {
                       if (e.getKey() == Ext.EventObject.ENTER) {
                           Query();
                       }
                   }
               }
           },
           {
               xtype: 'button',
               text: SEARCH,
               iconCls: 'icon-search',
               id: 'btnQuery',
               handler: Query
           },
           {
               xtype: 'button',
               text: RESET,
               id: 'btn_reset',
               listeners: {
                   click: function () {
                       Ext.getCmp("searchcontent").setValue("");
                       Ext.getCmp('product_state').setValue(-1);
                   }
               }
           }
        ],
        bbar: Ext.create('Ext.PagingToolbar', {
            store: VendorProductStore,
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
        items: [gbVendorProduct],
        renderTo: Ext.getBody(),
        autoScroll: true,
        listeners: {
            resize: function () {
                gbVendorProduct.width = document.documentElement.clientWidth;
                this.doLayout();
            }
        }
    });
    VendorProductStore.load({ params: { start: 0, limit: 25 } });
});
function Query(x) {
    VendorProductStore.removeAll();
    Ext.getCmp("gbVendorProduct").store.loadPage(1, {
        params: {
            vendor_id: vendor,
            searchcontent: Ext.getCmp('searchcontent').getValue(),
            product_state: Ext.getCmp('product_state').getValue()
        }
    });
}

onSpecEditClick = function (vendor, product) {
    editSpecFunction(vendor, product);
}

//根據商品編號更新補貨中停止販售狀態
function UpdateShortage(product_id) {
    var activeValue = $("#img" + product_id).attr("hidValue");
    $.ajax({
        url: "/VendorStock/UpdateShortage",
        data: {
            "product_id": product_id,
            "shortage": activeValue
        },
        type: "POST",
        dataType: "json",
        success: function (msg) {
            VendorProductStore.load();
        },
        error: function (msg) {
            Ext.Msg.alert(INFORMATION, FAILURE);
        }
    });
}