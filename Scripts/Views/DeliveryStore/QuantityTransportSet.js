var winDetail;
var productId;
var resultPath;

var transports = Ext.create('Ext.data.Store', {
    fields: ['parameterCode', 'parameterName'],
    data: [
        { "parameterCode": "1|11", "parameterName": DISTRMODE_1 },//本島宅配
        { "parameterCode": "1|12", "parameterName": DISTRMODE_5 },//本島店配
        { "parameterCode": "2|21", "parameterName": DISTRMODE_2 },//離島宅配
        { "parameterCode": "2|22", "parameterName": DISTRMODE_4 },//離島店配
        { "parameterCode": "3|31", "parameterName": DISTRMODE_3 }//國際宅配
    ]
});

var feedbackStore = Ext.create('Ext.data.Store', {
    fields: ['ProductId', 'ProductName', 'BrandName', 'Msg']
});


Ext.onReady(function () {

    var importfrm = Ext.create('Ext.form.Panel', {
        id: 'importfrm',
        frame: true,
        layout: 'column',
        url: '/DeliveryStore/QuantityTransportSet',
        items: [{
            fieldLabel: STORE_MODE,//物流模式
            id: 'comb_transport',
            labelWidth: 80,
            width: 200,
            store: transports,
            xtype: 'combobox',
            editable: false,
            allowBlank: false,
            displayField: 'parameterName',
            valueField: 'parameterCode',
            listeners: {
                beforerender: function () {
                    transports.load({
                        callback: function () {
                            Ext.getCmp("comb_transport").setValue(transports.data.items[1].data.parameterCode);
                        }
                    });
                },
                select: function (combo, records) {
                        Ext.getCmp("submit").setDisabled(records[0].data.parameterCode != "1|12");
                }

            }
        }, {
            xtype: 'splitter',
            width: 36
        }, {
            labelWidth: 80,
            name: 'file',
            width: 300,
            fieldLabel: REQUEST_CHOOSE,//請選擇excel
            xtype: 'filefield',
            buttonText: CHOOSE,//選擇
            allowBlank: false
        }, {
            margin: '0 0 0 10',
            xtype: 'displayfield',
            value: '<a target="_blank" href="/DeliveryStore/DeliverySetTemplate">' + TEMPLATE_DOWN + '</a>'//範本下載
        }/*, {
            xtype: 'splitter',
            width: 50
        }*/, {
            margin: '0 0 0 50',
            xtype: 'button',
            text: QUANTITY_INSERT,//批量新增
            id: 'submit',
            border: true,
            handler: function () { onSubmit(1); }//上傳
        }/*, {
            xtype: 'splitter',
            width: 20
        }*/, {
            margin: '0 0 0 20',
            xtype: 'button',
            text: OUTPUT_RESULT,//導出結果
            border: true,
            handler: function () { if (resultPath) location = resultPath; }
        }/*, {
            xtype: 'splitter',
            width: 20
        }*/, {
            margin: '0 0 0 20',
            xtype: 'button',
            text: QUANTITY_DELETE,//批量刪除
            border: true,
            handler: function () { onSubmit(2); }//刪除
        }/*,{
            xtype: 'splitter',
            width: 20
        }*/, {
            margin: '0 0 0 20',
            xtype: 'button',
            text: DOWNLOAD,//下載
            border: true,
            handler: onDownload
        }]
    })



    var rebackgrid = Ext.create('Ext.grid.Panel', {
        id: 'rebackgrid',
        store: feedbackStore,
        //dockedItems: importfrm,
        columnLines: true,
        height: document.documentElement.clientHeight - 37,
        autoScroll: true,
        columns: [
            { header: NUM, xtype: 'rownumberer', width: 46, align: 'center' },
            {
                header: PRODUCT_ID, dataIndex: 'ProductId', align: 'left', width: 100, menuDisabled: true, sortable: true, renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                if (record.data.create_channel == '2') {
                    return '<a href="#"  style="color:Green" onclick="javascript:showDetail(' + record.data.ProductId + ')">' + record.data.ProductId + '</a>'
                } else {
                    return '<a href="#"  style="color:Black" onclick="javascript:showDetail(' + record.data.ProductId + ')">' + record.data.ProductId + '</a>'
                }
            } },
            { header: BRAND, dataIndex: 'BrandName', align: 'left', width: 300, menuDisabled: true, sortable: true },
            { header: PRODUCT_NAME, dataIndex: 'ProductName', align: 'left', width: 400, menuDisabled: true, sortable: true },
            { header: RESULT, dataIndex: 'Msg', align: 'left', fix: 1, width: 400, menuDisabled: true }]
    })


    Ext.create('Ext.Viewport', {
        //layout: 'anchor',
        items: [importfrm, rebackgrid],
        autoScroll: true,
        listeners: {
            resize: function () {
                rebackgrid.height = document.documentElement.clientHeight - 37;
                this.doLayout();
            }
        }
    });
})

function onSubmit(flag) {//flag:1為上傳，2為刪除
    var form = Ext.getCmp('importfrm').getForm();
    if (form.isValid()) {
        form.submit({
            waitMsg: FILE_UPLOADING,//'文件上傳中...',
            waitTitle: WAIT_TITLE,//請稍等
            params: {
                transport: Ext.getCmp("comb_transport").getValue(),
                flag:flag
            },
            success: function (form, action) {
                var result = Ext.decode(action.response.responseText);
                switch (flag) {
                    case 1://上傳
                            feedbackStore.removeAll();
                            feedbackStore.loadRawData(result.results);
                            resultPath = result.resultPath;
                        break;
                    case 2://刪除
                        Ext.Msg.alert(ALERT_MESSAGE, DELETE_SUCCESS + "~");//刪除成功
                        break;
                }
            },
            failure: function (form, action) {
                var result = Ext.decode(action.response.responseText);
                if (result.msg) {
                    Ext.Msg.alert(ALERT_MESSAGE, result.msg);
                }
                else {
                    Ext.Msg.alert(ALERT_MESSAGE, OVERTIME_PLEASE_RETRY + "~");//超時，請重試
                }
            }
        });
    }
}

function GetProductId() {
    return productId;
}
function onDownload() {
    var comb_transport = Ext.getCmp("comb_transport").getValue().split('|');
    window.open("/DeliveryStore/DownloadProductDeliverySet?" + Ext.Object.toQueryString({ Freight_big_area: comb_transport[0], Freight_type: comb_transport[1] }));
}


function showDetail(product_id) {
    productId = product_id;
    if (winDetail == undefined) {
        winDetail = Ext.create('Ext.window.Window', {
            title: PRODUCT_DETAILS_DATA,//商品詳細資料
            constrain: true,
            modal: true,
            resizable: false,
            height: document.documentElement.clientHeight * 565 / 783,
            width: 1000,
            autoScroll: false,
            layout: 'fit',
            html: "<iframe scrolling='no' frameborder=0 width=100% height=100% src='/ProductList/ProductDetails'></iframe>",
            listeners: {
                close: function (e) {
                    winDetail = undefined;
                    tabs = new Array();
                }
            }
        }).show();
    }

}