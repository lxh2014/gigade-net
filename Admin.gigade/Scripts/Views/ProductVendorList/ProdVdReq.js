var pageSize = 25;
//申請處理狀態 
var reqStatusStore = Ext.create('Ext.data.Store', {
    fields: ['text', 'value'],
    data: [
        { "text": '全部', "value": "0" },
        { "text": '申請中', "value": "1" },
        { "text": '已完成', "value": "2" },
        { "text": '不更動', "value": "3" }
    ]
});
//申請類型
var reqTypeStore = Ext.create('Ext.data.Store', {
    fields: ['text', 'value'],
    data: [
        { "text": '全部', "value": "0" },
        { "text": '申請上架', "value": "1" },
        { "text": '申請下架', "value": "2" }
    ]
});
//品牌store
var brandStore = Ext.create('Ext.data.Store', {
    model: 'gigade.Brand',
    autoLoad: true,
    proxy: {
        type: 'ajax',
        url: "/Brand/QueryBrand",
        noCache: false,
        getMethod: function () { return 'get'; },
        actionMethods: 'post',
        reader: {
            type: 'json',
            root: 'item'
        }
    }
});
var apply_Accdi = Ext.create('Ext.form.FieldContainer', {
    layout: 'hbox',
    anchor: '100%',
    items: [
    {
        xtype: 'combobox',
        fieldLabel: BRAND,
        store: brandStore,
        id: 'brand_id',
        colName: 'brand_id',
        name: 'brand_id',
        displayField: 'brand_name',
        typeAhead: true,
        queryMode: 'local',
        valueField: 'brand_id'
    }, {
        xtype: 'textfield',
        fieldLabel: '商品編號',
        id: 'productid',
        colName: 'productid',
        margin: '0 20px',
        //hidden: true,
        submitValue: false,
        name: 'productid'
    },
    {
        xtype: 'combobox',
        id: 'reqstatus',
        margin: '0 20px',
        fieldLabel: '申請處理狀態',
        colName: 'reqstatus',
        queryMode: 'local',
        editable: false,
        store: reqStatusStore,
        displayField: 'text',
        valueField: 'value',
        value: 1
    }
    ]
});
var reqTime = {
    xtype: 'fieldcontainer',
    layout: 'hbox',
    anchor: '100%',
    items: [
        {
            xtype: 'combobox',
            id: 'reqtype',
            margin: '0 5px 0 0',
            fieldLabel: '申請類型',
            colName: 'reqtype',
            queryMode: 'local',
            editable: false,
            store: reqTypeStore,
            displayField: 'text',
            valueField: 'value'
        },
        {
            xtype: 'datefield',
            id: 'time_start',
            name: 'time_start',
            fieldLabel: '申請時間',
            margin: '0 5px 0 15px',
            editable: false,
            vtype: 'daterange',
            endDateField: 'time_end'
        }, {
            xtype: 'displayfield',
            value: '~'
        }, {
            xtype: 'datefield',
            id: 'time_end',
            name: 'time_end',
            margin: '0 5px',
            editable: false,
            vtype: 'daterange',
            startDateField: 'time_start'
        }
    ]
};

//供應商上下架申請model
Ext.define('GIGADE.ProdVdReqModel', {
    extend: 'Ext.data.Model',
    fields: [
        { name: 'rid', type: 'int' },
        { name: 'vendor_id', type: 'int' },
         { name: 'vendor_name', type: 'string' },
          { name: 'brand_id', type: 'int' },
        { name: 'brand_name', type: 'string' },
        { name: 'product_id', type: 'int' },
        { name: 'product_status', type: 'int' },
        { name: 'statusName', type: 'string' },
        { name: 'req_status', type: 'int' },
        { name: 'req_datatime', type: 'string' },
        { name: 'explain', type: 'string' },
        { name: 'req_type', type: 'int' },
        { name: 'user_id', type: 'int' },
        { name: 'reply_datetime', type: 'string' },
        { name: 'reply_note', type: 'string' }
    ]
});

var prodVdReqListStore = Ext.create("Ext.data.Store", {
    model: 'GIGADE.ProdVdReqModel',
    autoLoad: false,
    proxy: {
        type: 'ajax',
        url: '/ProductVendorList/ProdVdReqListQuery',
        actionMethods: 'post',
        reader: {
            type: 'json',
            root: 'data',
            totalProperty: 'totalCount'
        }
    }
});

prodVdReqListStore.on("beforeload", function () {
    Ext.apply(prodVdReqListStore.proxy.extraParams,
             {
                 brand_id: Ext.htmlEncode(Ext.getCmp("brand_id").getValue()),
                 product_id: Ext.htmlEncode(Ext.getCmp('productid').getValue()),
                 req_status: Ext.getCmp('reqstatus').getValue(),
                 time_start: Ext.getCmp("time_start").getValue(),
                 time_end: Ext.getCmp("time_end").getValue(),
                 req_type: Ext.getCmp("reqtype").getValue()
             });
});
Ext.onReady(function () {

    var query = function () {
        prodVdReqListStore.loadPage(1);
    }
    document.body.onkeydown = function () {
        if (event.keyCode == 13) {
            $("#btnSearch").click();
        }
    };
    function searchShow() {
        Ext.getCmp('prodVdReqGrid').show();
        query();
    }
    var frm = Ext.create('Ext.form.Panel', {
        layout: 'column',
        border: false,
        width: 1185,
        margin: '0 0 10 0',
        defaults: { anchor: '95%', msgTarget: "side", padding: '5 5' },
        items: [{
            xtype: 'panel',
            columnWidth: 1,
            border: 0,
            layout: 'anchor',
            items: [apply_Accdi, reqTime]
        }],
        buttonAlign: 'center',
        buttons: [{
            text: SEARCH,
            id: 'btnSearch',
            colName: 'btnSearch',
            iconCls: 'ui-icon ui-icon-search-2',
            handler: function () {
                Ext.getCmp('prodVdReqGrid').show();
                query();
            }
        }, {
            text: RESET,
            id: 'btn_reset',
            iconCls: 'ui-icon ui-icon-reset',
            listeners: {
                click: function () {
                    frm.getForm().reset();
                    Ext.getCmp("reqstatus").setValue(1);
                    Ext.getCmp("reqtype").setValue(0);
                }
            }
        }]
    });

    //列选择模式
    var sm = Ext.create('Ext.selection.CheckboxModel', {
        listeners: {
            selectionchange: function (sm, selections) {
                var rows = Ext.getCmp('prodVdReqGrid').getSelectionModel().getSelection();
                if (selections.length > 0) {
                    Ext.Array.each(selections, function (rows) {
                        if (rows.data.req_status == 1) {
                            Ext.getCmp("prodVdReqGrid").down('#btnPass').setDisabled(false);
                            Ext.getCmp("prodVdReqGrid").down('#btnBack').setDisabled(false);
                        }
                    })
                } else {
                    Ext.getCmp("prodVdReqGrid").down('#btnPass').setDisabled(true);
                    Ext.getCmp("prodVdReqGrid").down('#btnBack').setDisabled(true);
                }
            }
        }
    });



    var grid = Ext.create("Ext.grid.Panel", {
        id: 'prodVdReqGrid',
        selModel: sm,
        height: document.documentElement.clientWidth <= 1185 ? document.documentElement.clientHeight - 103 - 20 : document.documentElement.clientHeight - 103,
        columnLines: true,
        store: prodVdReqListStore,
        columns: [
            { header: 'rid', colName: 'rid', hidden: true, dataIndex: 'rid', width: 120, align: 'left' },
            { header: '申請供應商ID', colName: 'vendor_id', hidden: true, dataIndex: 'vendor_id', width: 120, align: 'left' },
            { header: '供應商', colName: 'vendor_name', hidden: true, dataIndex: 'vendor_name', width: 120, align: 'left' },
            { header: '品牌', colName: 'brand_id', hidden: true, dataIndex: 'brand_id', width: 120, align: 'left' },
            { header: '品牌名稱', colName: 'brand_name', hidden: true, dataIndex: 'brand_name', width: 120, align: 'left' },
            { header: '申請之商品編號', colName: 'product_id', hidden: true, xtype: 'templatecolumn',
                tpl: Ext.create('Ext.XTemplate',
        '<tpl if="this.canCopy(product_id)">',
            '<a href="javascript:ProductLink({product_id})">{product_id}</a>',
        '</tpl>',
         {
             canCopy: function (product_id) {
                 return product_id >= 10000;
             }
         }), width: 120, align: 'center', sortable: false, menuDisabled: true
            },
        //             { header: '申請之商品編號', colName: 'product_id', hidden: false, xtype: 'templatecolumn', tpl: '<a href=# onclick="javascript:showDetail({product_id})" >{product_id}</a>', width: 120, align: 'center', sortable: false, menuDisabled: true },
        //         {
        //         header: PREVIEW_PRODUCT, colName: 'preview_product', hidden: false, xtype: 'templatecolumn',
        //         tpl: '<a href=javascript:ProductPreview(0,{product_id})>' + PREVIEW_PRODUCT + '</a>', //0為商品預覽 调用自己写的方法 ProductPreview()
        //         width: 60, align: 'center', sortable: false, menuDisabled: true
        //     },
     {header: '商品目前狀態', colName: 'product_status', hidden: true, dataIndex: 'product_status', width: 120, align: 'left' },
     { header: '商品目前狀態', colName: 'statusName', hidden: true, dataIndex: 'statusName', width: 120, align: 'left' },
            { header: '申請處理狀態', colName: 'req_status', hidden: true, dataIndex: 'req_status', width: 120, align: 'left', renderer: ReqStatus
            },
            { header: '申請時間', colName: 'req_datatime', hidden: true, dataIndex: 'req_datatime', width: 120, align: 'left' },
            { header: '申請說明', colName: 'explain', hidden: true, dataIndex: 'explain', width: 120, align: 'left' },
            { header: '申請類型', colName: 'req_type', hidden: true, dataIndex: 'req_type', width: 120, align: 'left', renderer: ReqType },
            { header: '處理之管理人員ID', colName: 'user_id', hidden: true, dataIndex: 'user_id', width: 120, align: 'left',
                renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                    if (record.data.user_id != 0) {
                        return value;
                    } else {
                        return null;
                    }
                }
            },
            { header: '處理完成時間', colName: 'reply_datetime', hidden: true, dataIndex: 'reply_datetime', width: 120, align: 'left',
                renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                    if (record.data.reply_datetime == '0001-01-01 00:00:00') {
                        return null;
                    } else {
                        return value;
                    }
                }
            },
            { header: '處理說明', colName: 'reply_note', hidden: true, dataIndex: 'reply_note', width: 120, align: 'left' }
        ],

        listeners: {
            viewready: function (grid) {
                ToolAuthorityQuery(function () {
                    setShow(grid, 'colName');
                });
            }
        },
        tbar: [
            {
                text: VERIFY_PASS,
                id: 'btnPass',
                colName: 'btnPass',
                hidden: false,
                disabled: true,
                iconCls: 'icon-accept',
                handler: btnPassOnClick

            }, {
                text: VERIFY_BACK,
                colName: 'btnBack',
                disabled: true,
                hidden: false,
                id: 'btnBack',
                icon: '../../../Content/img/icons/drop-no.gif',
                handler: btnBackOnClick
            }
            , '->', { text: '' }
        ],
        bbar: Ext.create('Ext.PagingToolbar', {
            store: prodVdReqListStore,
            pageSize: pageSize,
            displayInfo: true,
            displayMsg: NOW_DISPLAY_RECORD + ': {0} - {1}' + TOTAL + ': {2}',
            emptyMsg: NOTHING_DISPLAY
        }
        )
    });
    Ext.EventManager.onWindowResize(function () { grid.getView().refresh(); });
    Ext.create('Ext.Viewport', {
        layout: 'anchor',
        items: [frm, grid],
        renderTo: Ext.getBody(),
        autoScroll: true,
        listeners: {
            resize: function () {
                if (document.documentElement.clientWidth <= 1185) {
                    Ext.getCmp('prodVdReqGrid').setHeight(document.documentElement.clientHeight - 103 - 20);
                    Ext.getCmp('prodVdReqGrid').setWidth(document.documentElement.clientWidth);
                }
                else {
                    Ext.getCmp('prodVdReqGrid').setHeight(document.documentElement.clientHeight - 103);
                    Ext.getCmp('prodVdReqGrid').setWidth(document.documentElement.clientWidth);
                }

                this.doLayout();
            },
            afterrender: function (view) {
                ToolAuthorityQuery(function () {
                    //alert("权限");
                    setShow(frm, 'colName');
                    window.setTimeout(searchShow, 1);
                });
            }
        }
    });
    prodVdReqListStore.loadPage(1);
});

function ReqStatus(val) {
    switch (val) {
        case 1:
            return '申請中';
            break;
        case 2:
            return '已完成';
            break;
        case 3:
            return '不更動';
            break;
    }
}
function ReqType(val) {
    switch (val) {
        case 1:
            return '申請上架';
            break;
        case 2:
            return '申請下架';
            break;
    }
}
function ProductLink(product_id) {
    var url;
    url = '/ProductList?product_id=' + product_id;
    var panel = window.parent.parent.Ext.getCmp('ContentPanel');
    var vendorProduct = panel.down('#308');
    if (vendorProduct) {
        vendorProduct.close();
    }
    vendorProduct = panel.add({
        id: '308',
        title: '商品列表',
        html: window.top.rtnFrame(url),
        closable: true
    });
    panel.setActiveTab(vendorProduct);
    panel.doLayout();
    //window.location.href = 'http://' + window.location.host + url;
}
var back = function (row, reason, functionid) {
    var result = '';
    for (var i = 0, j = row.length; i < j; i++) {
        if (i > 0) {
            result += ';';
        }
        result += row[i].data.rid + "," + row[i].data.product_id;
    }

    Ext.Ajax.request({
        url: '/ProductVendorList/ProdVdReqBack',
        params: {
            rIdAndProdId: result,
            backReason: reason
        },
        success: function (response) {
            var result = Ext.decode(response.responseText);
            if (result.success) {
                Ext.Msg.alert(INFORMATION, SUCCESS, function () {
                    prodVdReqListStore.loadPage(1);
                });
            }
            else {
                Ext.Msg.alert(INFORMATION, FAILURE);
            }
        }
    });
}

//核可
var btnPassOnClick = function () {

    var rows = Ext.getCmp("prodVdReqGrid").getSelectionModel().getSelection();
    if (rows.length == 0) {
        Ext.Msg.alert(INFORMATION, NO_SELECTION);
        return false;
    }

    var result = '';
    for (var i = 0, j = rows.length; i < j; i++) {
        if (i > 0) {
            result += ';';
        }
        result += rows[i].data.rid + "," + rows[i].data.product_id;
    }

    Ext.Ajax.request({
        url: '/ProductVendorList/ProdVdReqPass',
        params: {
            rIdAndProdId: result
        },
        success: function (response) {
            var result = Ext.decode(response.responseText);
            if (result.success) {
                Ext.Msg.alert(INFORMATION, SUCCESS);
                prodVdReqListStore.loadPage(1);
            }
            else {
                Ext.Msg.alert(INFORMATION, result.msg);
            }
        }
    });
}

//駁回
var btnBackOnClick = function () {
    var row = Ext.getCmp("prodVdReqGrid").getSelectionModel().getSelection();
    if (row.length == 0) {
        Ext.Msg.alert(INFORMATION, NO_SELECTION);
        return false;
    }

    Ext.MessageBox.show({
        title: BACK_REASON,
        id: 'txtReason',
        msg: INPUT_BACK_REASON,
        width: 300,
        buttons: Ext.MessageBox.OKCANCEL,
        multiline: true,
        fn: function (btn, text) {
            if (btn == "cancel") {
                return false;
            }
            back(row, text, 'btnBack');
        },
        animateTarget: 'btnBack'
    });
}

////預覽信息(商品)
////没有什么逻辑的地方
////基本都为Ext的创建方法 
//function ProductPreview(type, product_id) {
//    Ext.Ajax.request({
//        url: '/ProductVendorList/ProductPreview',
//        params: { Product_Id: product_id, Type: type },
//        success: function (form, action) {
//            var result = form.responseText;
//            var htmlval = "<a href=" + result + " target='new'>" + result + "</a>";
//            Ext.create('Ext.window.Window', {
//                title: PREVIEW_PRODUCT,
//                modal: true,
//                height: 110,
//                width: 600,
//                constrain: true,
//                layout: 'fit',

//                items: [{
//                    xtype: 'panel',
//                    border: false,
//                    layout: 'hbox',
//                    autoScroll: true,
//                    bodyStyle: {
//                        padding: '20px 10px'
//                    },
//                    items: [{
//                        xtype: 'displayfield',
//                        labelWidth: 55,
//                        labelAlign: 'top',
//                        fieldLabel: PREVIEW_PRODUCT,
//                        value: htmlval
//                    }]
//                }]
//            }).show();
//        }
//    })
//}
