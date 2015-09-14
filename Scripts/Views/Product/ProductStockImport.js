Ext.Loader.setConfig({ enabled: true });
Ext.Loader.setPath('Ext.ux', '/Scripts/Ext4.0/ux');
Ext.require([
    'Ext.form.Panel',
    'Ext.ux.form.MultiSelect',
    'Ext.ux.form.ItemSelector'
]);
Ext.define('GIGADE.ProductStocks', {
    extend: 'Ext.data.Model',
    fields: [
              { name: "product_id", type: "string" }, //商品編號
              { name: "item_id", type: "string" }, //細項編號
              { name: "item_stock", type: "string" }, //庫存
              { name: "item_alarm", type: "string" },//庫存警告
              { name: "product_name", type: "string" },
              { name: "spec_name1", type: "string" },
              { name: "spec_name2", type: "string" },
              { name: "type", type: "string" },
              { name: "product_mode", type: "string" },
              { name: "prepaid", type: "string" },
              { name: "spec_status", type: "string" },
              { name: "spec_status2", type: "string" },
              { name: "remark", type: "string" }
    ]
});
//獲取grid中的數據
var ProductStockStore = Ext.create('Ext.data.Store', {
    model: 'GIGADE.ProductStocks',
    proxy: {
        type: 'ajax',
        url: '/Product/GetProductStockList',
        actionMethods: 'post',
        reader: {
            type: 'json',
            //totalProperty:10,
            root: 'data'
        }
    }
});
Ext.onReady(function () {
    var exportTab = Ext.create('Ext.form.Panel', {
        layout: 'anchor',
        renderTo: Ext.getBody(),
        width: 400,
        height: 140,
        url: '/Product/UploadExcel',
        border: false,
        plain: true,
        defaultType: 'displayfield',
        bodyPadding: 20,
        labelWidth: 45,
        id: 'ImportFile',
        defaults: { anchor: "95%", msgTarget: "side" },
        items: [
             {
                 html: SHIPMENT_SERVICE_PROMPT_MESSAGE,
                 border: 0,
                 height: 40
                 
             },
             {
                 xtype: 'filefield',
                 name: 'ImportExcelFile',
                 id: 'ImportExcelFile',
                 fieldLabel: EXCEL_IN_DATA,
                 msgTarget: 'side',
                 buttonText: BROWSE,
                 submitValue: true,
                 validator:
                 function (value) {
                     var type = value.split('.');
                     if (type[type.length - 1] == 'xls' || type[type.length - 1] == 'xlsx') {
                         return true;
                     }
                     else {
                         return IN_FILE_TYPE_WRONG;
                     }
                 },
                 width: 300,
                 allowBlank: false,
                 fileUpload: true
             }
        ],
        buttonAlign: 'right',
        buttons: [
            {
            text: UPLOAD,
            id: 'upload',
            formBind: true,
            disabled: true,
            handler: function () {
                var form = this.up('form').getForm();
                if (form.isValid()) {
                    form.submit({
                        params: {
                            file: Ext.htmlEncode(Ext.getCmp('ImportExcelFile').getValue())
                        },
                        success: function (form, action) {
                            var result = Ext.decode(action.response.responseText);
                            if (result.success) {
                                Ext.Msg.alert(INFORMATION, USUCCESS);
                                Ext.getCmp('import').setDisabled(false);
                                ProductStockStore.load();
                                Ext.getCmp('gdProductStock').show();
                            }
                            else {
                                Ext.Msg.alert(INFORMATION, UFAILURE);
                            }
                        },
                        failure: function (form, action) {
                            var result = Ext.decode(action.response.responseText);
                            Ext.Msg.alert(INFORMATION, UFAILURE);
                        }
                    });
                }
            }
        },
        {
            text: IMPORT,
            id: 'import',
            disabled: true,
            handler: function () {
                Ext.MessageBox.confirm(CONFIRM, IS_CLOSEFORM, function (btn) {
                    if (btn == "yes") {
                        Ext.MessageBox.show({
                            msg: WAIT,
                            width: 300,
                            wait: true
                        });
                        Ext.Ajax.request({
                            url: "/Product/UpdateStock",
                            method: 'post',
                            timeout: 900000,
                            success: function (response) {
                                var result = Ext.decode(response.responseText);
                                if (result.success) {
                                    Ext.Msg.alert(INFORMATION, result.msg);
                                }
                            },
                            failure: function (response) {
                                Ext.Msg.alert(INFORMATION, result.msg);
                            }
                        })
                    }
                    else {
                        return false;
                    }
                })

            }
        }]
    });

    //頁面加載時創建grid
    var gdProductStock = Ext.create('Ext.grid.Panel', {
        id: 'gdProductStock',
        store: ProductStockStore,
        height: '600',
        columnLines: true,
        frame: true,
        hidden: true,
        columns: [
            { xtype: 'rownumberer', width: 60 },
            {
                header: TYPE, dataIndex: '', width: 80, align: 'center', renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                    if (record.data.prepaid == "是") {
                        return PREPAID;
                    }
                    else {
                        return record.data.product_mode;
                    }
                }
            },
            { header: ITEMID, dataIndex: 'item_id', width: 100, align: 'center' },
            { header: PRODUCTID, dataIndex: 'product_id', width: 100, align: 'center' },
            { header: PRODUCTNAME, dataIndex: 'product_name', width: 200, align: 'center' },
            { header: SPECNAME1, dataIndex: 'spec_name1', width: 70, align: 'center' },
            { header: SPECNAME2, dataIndex: 'spec_name2', width: 70, align: 'center' },
            { header: PRODUCTSTOCK, dataIndex: 'item_stock', width: 70, align: 'center' },
            { header: PRODUCTALARM, dataIndex: 'item_alarm', width: 70, align: 'center' },
            { header: STANDARD_ONE_SHOW, dataIndex: 'spec_status', width: 100, align: 'center' },
            { header: STANDARD_TWO_SHOW, dataIndex: 'spec_status2', width: 100, align: 'center' },
            { header: REMARK, dataIndex: 'remark', width: 100, align: 'center' }
        ],
        listeners: {
            scrollershow: function (scroller) {
                if (scroller && scroller.scrollEl) {
                    scroller.clearManagedListeners();
                    scroller.mon(scroller.scrollEl, 'scroll', scroller.onElScroll, scroller);
                }
            }
        },
        viewConfig: {
            forceFit: true, getRowClass: function (record, rowIndex, rowParams, store) {
                //alert(record.data.type);
                if (record.data.type =="1") {
                    // alert(record.data.type);
                    return 'product_stock_type';
                }
            }
        }
    });

    Ext.create('Ext.container.Viewport', {
        layout: 'fit',
        items: [exportTab, gdProductStock],
        renderTo: Ext.getBody(),
        autoScroll: true,
        listeners: {
            resize: function () {
                // exportTab.width = document.documentElement.clientWidth;
                this.doLayout();
            }
        }
    });
});
