Ext.define('GIGADE.SITE', {
    extend: 'Ext.data.Model',
    fields: [
        { name: 'Site_Id', type: 'string' },
        { name: 'Site_Name', type: 'string' }
    ]
});

//站臺store
var siteStore = Ext.create('Ext.data.Store', {
    model: 'GIGADE.SITE',
    autoLoad: false,
    proxy: {
        type: 'ajax',
        url: '/ProductList/GetSite',
        actionMethods: 'post',
        reader: {
            type: 'json'
        }
    }
});


var sitebox = {
    xtype: 'combobox',
    fieldLabel: SITE,
    store: siteStore,
    labelWidth: 60,
    margin: '0 0 0 50',
    id: 'site_id',
    name: 'site_id',
    displayField: 'Site_Name',
    valueField: 'Site_Id',
    queryMode: 'local',
    colName: 'site_id',
    editable: false,
    listeners: {
        beforerender: function () {
            siteStore.load({
                callback: function () {
                    //siteStore.insert(0, { Site_Id: '0', Site_Name: '請選擇' });
                    Ext.getCmp("site_id").setValue(siteStore.data.items[0].data.Site_Id);
                }
            });
        }
    }
};

var ChannelStore = Ext.create('Ext.data.Store', {
    fields: ['channel_name_full', 'channel_id'],
    proxy: {
        type: 'ajax',
        url: '/Order/GetChannel',
        actionMethods: 'post',
        reader: {
            type: 'json',
            root: 'items'
        }
    }
});

var ImprotTypeStore = Ext.create('Ext.data.Store', {
    fields: ['text', 'value'],
    data: [
        { text: IMPORT_TYPE_HOME, value: '1' },
        { text: IMPORT_TYPE_STORE, value: '2' }
    ]
});

Ext.define('GIGADE.ChannelOrder', {
    extend: 'Ext.data.Model',
    fields: [
        { name: 'dmtshxuid', type: 'string' },
        { name: 'chlitpdno', type: 'string' },
        { name: 'dsr', type: 'string' },
        { name: 'sumup', type: 'string' },
        { name: 'qty', type: 'string' },
        { name: 'shipco', type: 'string' },
        { name: 'ordpesnm', type: 'string' },
        { name: 'orddat', type: 'string' },
        { name: 'Msg', type: 'string' },
        { name: 'CanSel', type: 'string' }
    ]
});
var ChannelOrder = Ext.create('Ext.data.Store', {
    model: 'GIGADE.ChannelOrder',
    groupField: 'dmtshxuid',
    proxy: {
        type: 'ajax',
        url: '/Order/ReadExcelFile',
        actionMethods: 'post',
        timeout: 60000 * 5,
        reader: {
            type: 'json'
        }
    }
});

//列表分组
var OrderFeature = Ext.create('Ext.grid.feature.Grouping', {
    groupHeaderTpl: '<span style="color:black">' + ORDER_SERIAL_ID + '</span> <span style="color:blue">{name}</span>'
});
var sm = Ext.create('Ext.selection.CheckboxModel', {
    listeners: {
        selectionchange: function (sm, selections) {
            Ext.getCmp("OrderGrid").down('#import').setDisabled(selections.length == 0);
        }
    },
    allowDeselect: true,
    storeColNameForHide: 'CanSel'
});

var channel = '', importType = '', files = '', site = 0;

Ext.onReady(function () {


    var excels = document.getElementById('ExcelEx').value.split(','); //['xls', 'xlsx'];
    var pdfs = document.getElementById('PDFEx').value.split(','); //['pdf'];

    Ext.apply(Ext.form.field.VTypes, {
        excelFilter: function (val, field) {
            var type = val.split('.')[val.split('.').length - 1].toLocaleLowerCase();
            for (var i = 0; i < excels.length; i++) {
                if (excels[i] == type) {
                    return true;
                }
            }
            return false;
        },
        excelFilterText: FILE_TYPE_WRONG,

        pdfFilter: function (val, field) {
            var type = val.split('.')[val.split('.').length - 1].toLocaleLowerCase();
            for (var i = 0; i < pdfs.length; i++) {
                if (pdfs[i] == type) {
                    return true;
                }
            }
            return false;
        },
        pdfFilterText: FILE_TYPE_WRONG
    });

    var frm = Ext.create('Ext.form.Panel', {
        id: 'frm',
        layout: 'anchor',
        height: 190,
        width: 900,
        defaults: { anchor: "95%" },
        border: false,
        plain: true,
        url: '/Order/SaveImportFile',
        items: [{
            border: 0,
            layout: 'column',
            border: 0,
            padding: '5 5 0 5',
            items: [{
                fieldLabel: CHANNEL_NAME,
                labelWidth: 140,
                id: 'channel',
                xtype: 'combobox',
                submitValue: false,
                editable: false,
                allowBlank: false,
                store: ChannelStore,
                displayField: 'channel_name_full',
                valueField: 'channel_id'
            }, sitebox]
        }, {
            border: 0,
            layout: 'column',
            hidden: true,
            items: [{
                border: 0,
                bodyStyle: 'padding:5px 5px 0px 5px',
                items: [{
                    fieldLabel: IMPORT_TYPE,
                    labelWidth: 140,
                    name: 'importType',
                    id: 'importType',
                    xtype: 'combobox',
                    editable: false,
                    allowBlank: false,
                    store: ImprotTypeStore,
                    displayField: 'text',
                    valueField: 'value',
                    value: '1',
                    listeners: {
                        select: function (combo, record) {
                            var code = combo.rawValue;
                            var dispatch = Ext.getCmp('dispatch');
                            if (code == IMPORT_TYPE_STORE) {
                                if (!dispatch) {
                                    Ext.getCmp('dispatch_file').add({
                                        labelWidth: 140,
                                        fieldLabel: STORE_DISPATCH_FILE,
                                        name: 'dispatch',
                                        id: 'dispatch',
                                        xtype: 'filefield',
                                        buttonText: CHOOSE_STORE_DISPATCH_FILE,
                                        width: 500,
                                        allowBlank: false,
                                        vtype: 'pdfFilter'
                                    });
                                }
                            }
                            else {
                                if (dispatch) {
                                    Ext.getCmp('dispatch_file').remove(dispatch);
                                }
                            }
                        }
                    }
                }]
            }]
        }, {
            border: 0,
            layout: 'column',
            items: [{
                border: 0,
                bodyStyle: 'padding:5px 5px 5px 5px',
                items: [{
                    labelWidth: 140,
                    fieldLabel: IMPORT_FILE,
                    name: 'channelFile',
                    xtype: 'filefield',
                    buttonText: CHOOSE_IMPORT_FILE,
                    width: 500,
                    allowBlank: false,
                    vtype: 'excelFilter'
                }]
            }, {
                border: 0,
                bodyStyle: 'padding:5px 5px 5px 5px',
                items: [{
                    xtype: 'displayfield',
                    value: '<a href="/Order/OrderTemplate">' + TEMPLATE_DOWNLOAD + '</a>'
                }]
            }]
        }, {
            border: 0,
            layout: 'column',
            items: [{
                border: 0,
                bodyStyle: 'padding:0px 5px 5px 5px',
                id: 'dispatch_file',
                items: []
            }]
        }],
        buttonAlign: 'right',
        buttons: [{
            text: SURE,
            handler: function () {
                var form = this.up('form').getForm();
                ChannelOrder.removeAll();
                if (form.isValid()) {
                    form.submit({
                        waitMsg: FILE_UPLOADING,
                        waitTitle: WAIT_TITLE,
                        success: function (form, action) {
                            var result = Ext.decode(action.response.responseText);
                            if (result.success) {
                                channel = Ext.getCmp('channel').getValue();
                                importType = Ext.getCmp('importType').getValue();
                                files = result.msg;
                                Ext.getCmp('frm').hide();
                                Ext.getCmp('OrderGrid').show();
                                ChannelOrder.load({
                                    params: {
                                        Channel: channel,
                                        ImportType: importType,
                                        Files: files
                                    },
                                    callback: function () {
                                        if (ChannelOrder.data.length == 0) {
                                            Ext.Msg.alert(INFORMATION, PLEASE_DOWN_MODEL_ESSAY_COMPARE, function () {
                                                Cancle();
                                            });
                                        }
                                    }
                                });
                            }
                        },
                        failure: function () {
                            Ext.Msg.alert(INFORMATION, FAILURE);
                        }
                    });
                }
            }
        }]
    });

    var OrderGrid = Ext.create('Ext.grid.Panel', {
        id: 'OrderGrid',
        store: ChannelOrder,
        height: document.documentElement.clientHeight,
        columnLines: true,
        autoScroll: true,
        frame: true,
        hidden: true,
        columns: [
            { header: NUMBER, xtype: 'rownumberer', width: 100, align: 'center' },
            { header: ERROR_INFO, dataIndex: 'Msg', width: 180, align: 'center', renderer: function (val) { if (val != "警告") return '<span style="color:red">' + val + '</span>' } },
            { header: SERIAL_NUM, dataIndex: 'dmtshxuid', width: 180, align: 'center', hidden: true },
            { header: PRODUCT_ID, dataIndex: 'chlitpdno', width: 180, align: 'center' },
            { header: PRODUCT_NAME, dataIndex: 'dsr', width: 300 },
            { header: SUM_PRICE, dataIndex: 'sumup', width: 120, align: 'center' },
            { header: BUY_COUNT, dataIndex: 'qty', width: 120, align: 'center' },
            { header: DELIVERY, dataIndex: 'shipco', width: 120, align: 'center' },
            { header: ORDER_NAME, dataIndex: 'ordpesnm', width: 120, align: 'center' },
            { header: TRANS_DATE, dataIndex: 'orddat', width: 120, align: 'center' }
        ],
        tbar: [
            { xtype: 'button', id: 'import', text: IMPORT, iconCls: 'icon-upload-local', disabled: true, handler: Import },
            { xtype: 'button', text: FILE_REFRESH, iconCls: 'icon-refresh', handler: Cancle }
        ],
        listeners: {
            scrollershow: function (scroller) {
                if (scroller && scroller.scrollEl) {
                    scroller.clearManagedListeners();
                    scroller.mon(scroller.scrollEl, 'scroll', scroller.onElScroll, scroller);
                }
            }
        },
        features: [OrderFeature],
        selModel: sm
    });

    Ext.create('Ext.Viewport', {
        layout: 'anchor',
        items: [frm, OrderGrid],
        autoScroll: true,
        listeners: {
            resize: function () {
                OrderGrid.height = document.documentElement.clientHeight;
                this.doLayout();
            }
        }
    });
});

//選中資料導入至數據庫
function Import() {
    site = Ext.getCmp('site_id').getValue();
    Ext.getCmp('import').setDisabled(true);
    var row = Ext.getCmp("OrderGrid").getSelectionModel().getSelection();
    if (row.length > 0) {
        var chkOrder = '';
        for (var i = 0; i < row.length; i++) {
            chkOrder += row[i].data.dmtshxuid + '+' + row[i].data.chlitpdno + '|';
        }
        Ext.Ajax.request({
            url: '/Order/Import',
            method: 'post',
            waitMsg: FILE_UPLOADING,
            waitTitle: WAIT_TITLE,
            params: {
                Orders: chkOrder,
                Channel: channel,
                Site: site,
                ImportType: importType,
                Files: files
            },
            success: function (form, action) {
                var result = Ext.decode(form.responseText);
                if (result.Orders.length > 0) {
                    ChannelOrder.loadData(result.Orders);
                }
                Ext.Msg.alert(INFORMATION, Ext.String.format(IMPORT_SUCCESS_INFO, result.Total, result.SucccessCount));
            }
        });
    }
    else {
        Ext.Msg.alert(INFORMATION, NO_SELECTION);
    }
}

function Cancle() {
    channel = '', importType = '', files = '';
    Ext.getCmp('frm').show();
    Ext.getCmp('OrderGrid').hide();
    ChannelOrder.removeAll();
    Ext.getCmp("OrderGrid").getSelectionModel().deselectAll();
}