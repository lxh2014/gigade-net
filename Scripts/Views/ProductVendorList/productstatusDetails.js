

var productStatusPanel;

var currentProductstatusPanel;
var historyStore;

function initProductStatusPanel(product_id, product_status) {


    Ext.define('GIGADE.PRODUCTSTATUS', {
        extend: 'Ext.data.Model',
        fields: [
       { name: 'product_id', type: 'int' },
       { name: 'user_username', type: 'string' },
       { name: 'create_time', type: 'string' },
       { name: 'type', type: 'string' },
       { name: 'product_status', type: 'string' },
       { name: 'current_status', type: 'string' },
       { name: 'remark', type: 'string'}]
    });


    historyStore = Ext.create('Ext.data.Store', {
        model: 'GIGADE.PRODUCTSTATUS',
        id: 'historyStore',
        proxy: {
            type: 'ajax',
            url: '/ProductVendorList/ProductStatusHistoryQuery',
            actionMethods: 'post',
            reader: {
                type: 'json',
                root: 'data'
            }
        }
    });

    currentProductstatusPanel = Ext.create('Ext.panel.Panel', {
        title: '',
        border: false,
        padding: '10 0',
        items: [{
            id: 'currentProductstatusPanel',
            xtype: 'displayfield',
            fieldLabel: PRODUCT_STATUS,
            value: product_status,
            width: 200
        }]
    });

    productStatusPanel = Ext.create('Ext.grid.Panel', {
        id: 'productStatusPanel',
        title: PRODUCT_STATUS_HISTORY,
        store: historyStore,
        columns: [{ header: CREATE_TIME, dataIndex: 'create_time', width: 100, align: 'center', renderer: function (value) {
            value = value.substring(value.lastIndexOf('(') + 1, value.lastIndexOf(')'));
            value = Ext.Date.format(new Date(eval(value)), 'Y-m-d H:i');
            return value;
        }
        },
                  { header: USER_NAME, dataIndex: 'user_username', width: 100, align: 'center' },
                  { header: TYPE, dataIndex: 'type', width: 100, align: 'center' },
                  { header: PRODUCT_STATUS_AFTER, dataIndex: 'product_status', width: 250, align: 'center' },
                  { header: REMARK, dataIndex: 'remark', width: 300, align: 'center'}],
        listeners: {
            beforerender: function (grid) {
            }
        }
    });

    productStatusPanel.store.load({
        params: { ProductId: product_id },
        callback: function (records, operation, success) {
            if (records && records.data) {
                //                Ext.getCmp('currentProductstatusPanel').setValue(records[0].data.current_status);
            }
        }
    });
}