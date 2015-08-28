var channel_id

//運費Model
var currentRow = 0;
Ext.define('Shipping', {
    extend: 'Ext.data.Model',
    fields: [
        { name: "channel_id", type: "string" },
        { name: "shipping_carrior", type: "string" },
        { name: "shipping_carrior_content", type: "string" },
        { name: "shipco", type: "string" },
        { name: "shipco_content", type: "string" },
        /*
        { name: "shipping_type", type: "string" },
        { name: "shipping_type_content", type: "string" },
        */
        {name: "n_threshold", type: "string" },
        { name: "l_threshold", type: "string" },
        { name: "n_fee", type: "string" },
        { name: "l_fee", type: "string" },
        { name: "n_return_fee", type: "string" },
        { name: "l_return_fee", type: "string" },
        { name: "retrieve_mode", type: "string"}]
});

//貨運物流業者Model
Ext.define('Parameter', {
    extend: 'Ext.data.Model',
    fields: [
        { name: "rowid", type: "string" },
        { name: "parameterCode", type: "string" },
        { name: "parameterName", type: "string"}]
});

//運費Model
var ShippingStore = Ext.create('Ext.data.Store', {
    autoDestroy: true,
    model: 'Shipping',
    remoteSort: false,
    autoLoad: false,
    proxy: {
        type: 'ajax',
        url: "/Channel/QueryShipping",
        reader: {
            type: 'json',
            root: 'items'
        }
    }
});

//貨運物流業者store
var DeliverStore = Ext.create('Ext.data.Store', {
    autoLoad: false,
    model: 'Parameter',
    proxy: {
        type: 'ajax',
        url: '/Channel/QueryDeliver?paraType=Deliver_Store',
        reader: {
            type: 'json',
            root: 'items'
        },
        sorters: [{
            property: 'rowid',
            direction: 'ASC'
        }]
    }
});

//取貨方式store
var retrieveStore = Ext.create("Ext.data.Store", {
    model: 'Parameter',
    proxy: {
        type: 'ajax',
        url: '/Channel/QueryDeliver?paraType=retrieve_mode',
        reader: {
            type:'json',
            root:'items'
        },
        sorters: [{
            property: 'rowid',
            direction: 'ASC'
        }]
    }
});

ShippingStore.on('beforeload', function () {
    Ext.apply(ShippingStore.proxy.extraParams,
        {
            channel_id: 0,
            shippingcarrior:0
        });
    });

var DeliverTypeStore = Ext.create('Ext.data.Store', {
    fields: ['text', 'value'],
    data: [
        { "text": SHIPPINGTYPE_ROOM, "value": "1" },
        { "text": SHIPPINGTYPE_ICE, "value": "2" }
    ]
});

var cellEditing = Ext.create('Ext.grid.plugin.CellEditing', {
    clicksToEdit: 1
});

Ext.onReady(function () {
    if (window.parent.getChannelId() != "") {
        ShippingStore.load({
            params: {
                channel_id: window.parent.getChannelId()
            }
        });
    }
    var gdShipping = Ext.create('Ext.grid.Panel', {
        id: 'gdShipping',
        store: ShippingStore,
        plugins: [cellEditing],
        width: document.documentElement.clientWidth,
        frame: true,
        columns: [
            { header: GONGNENG, menuDisabled: true, xtype: 'actioncolumn', width: 100, align: 'center',
                items: [{
                    icon: '../../../Content/img/icons/cross.gif',
                    tooltip: DELETE,
                    handler: function (grid, rowIndex, colIndex) {
                        if (!confirm(YDELETE)) {
                            return;
                        }
                        var rec = gdShipping.getStore().getAt(rowIndex);
                        ShippingStore.removeAt(rowIndex);
                        if (window.parent.getChannelId() != "") {
                            Ext.Ajax.request({
                                url: '/Channel/DeleteShipping',
                                params: {
                                    shippingcarrior: rec.get('shipping_carrior'),
                                    channel_id: window.parent.getChannelId()
                                    //shippingtype: rec.get('shipping_type')
                                },
                                //                                success: function (response) {
                                //                                    var result = Ext.decode(response.responseText);
                                //                                    if (result.msg != '') {
                                //                                        Ext.Msg.alert(MESSAGE, result.msg);
                                //                                    }
                                //                                    ShippingStore.removeAt(rowIndex);
                                //                                    ShippingStore.load({
                                //                                        params: {
                                //                                            channel_id: window.parent.getChannelId()
                                //                                        }
                                //                                    });
                                //                                },
                                failure: function (response) {
                                    var result = Ext.decode(response.responseText);
                                    Ext.Msg.alert(MESSAGE, result.msg);
                                }
                            });
                        }
                    }
                }]
            },
            { header: 'ID', dataIndex: 'shipping_carrior', hidden: true },
            { header: SHIPPINGCAR, sortable: false, menuDisabled: true, dataIndex: 'shipping_carrior_content', width: 180, renderer: DeliverRenderer, editor: { xtype: 'combobox', id: 'cob_shippingcarrior', queryMode: 'local', displayField: 'parameterName', valueField: 'parameterCode', store: DeliverStore, editable: false, allowBlank: false }
            },
            { header: 'ID2', dataIndex: 'shipco', hidden: true },
            { header: SHIPOC, sortable: false, menuDisabled: true, dataIndex: 'shipco_content', width: 200, editor: { xtype: 'textfield', id: 'cob_shipoc', allowBlank: false }
            },
        /*{ header: SHIPPINGTYPE, dataIndex: 'shipping_type', width: 150, renderer: DeliverTypeRenderer, editor: { xtype: 'combobox', id: 'cob_shippingtype', queryMode: 'local', displayField: 'text', valueField: 'value', store: DeliverTypeStore, editable: false, allowBlank: false }
        },
        { header: THRESHOLD, dataIndex: 'threshold', width: 150, editor: { xtype: 'numberfield', value: 0, step: 1, minValue: 0, id: 'txt_threshold'} },
        { header: FEE, dataIndex: 'fee', width: 150, editor: { xtype: 'numberfield', value: 0, step: 1, minValue: 0, id: 'txt_fee'} },
        { header: RETURNFEE, dataIndex: 'return_fee', width: 150, editor: { xtype: 'numberfield', value: 0, step: 1, minValue: 0, id: 'txt_returnfee'} }*/
                {
                text: SHIPPINGTYPE_ROOM,
                menuDisabled: true,
                columns: [{ header: THRESHOLD, menuDisabled: true, dataIndex: 'n_threshold', width: 140, editor: { xtype: 'numberfield', maxValue: 999999, value: 0, step: 1, minValue: 0, id: 'txt_nthreshold'} },
                    { header: FEE, dataIndex: 'n_fee', width: 140, menuDisabled: true, editor: { xtype: 'numberfield', value: 0, step: 1, maxValue: 999999, minValue: 0, id: 'txt_nfee'} },
                    { header: RETURNFEE, dataIndex: 'n_return_fee', menuDisabled: true, width: 140, editor: { xtype: 'numberfield', value: 0, step: 1, maxValue: 999999, minValue: 0, id: 'txt_nreturnfee'}}]
            }, {
                text: SHIPPINGTYPE_ICE,
                menuDisabled: true,
                columns: [{ header: THRESHOLD, menuDisabled: true, dataIndex: 'l_threshold', width: 140, editor: { xtype: 'numberfield', value: 0, step: 1, maxValue: 999999, minValue: 0, id: 'txt_lthreshold'} },
                    { header: FEE, dataIndex: 'l_fee', menuDisabled: true, width: 140, editor: { xtype: 'numberfield', value: 0, step: 1, maxValue: 999999, minValue: 0, id: 'txt_lfee'} },
                    { header: RETURNFEE, dataIndex: 'l_return_fee', menuDisabled: true, width: 140, editor: { xtype: 'numberfield', value: 0, step: 1, maxValue: 999999, minValue: 0, id: 'txt_lreturnfee'}}]
            },
            { header: RETRIEVE_MODE, sortable: false, menuDisabled: true, dataIndex: 'retrieve_mode', width: 200, renderer: RetrieveRenderer, editor: { xtype: 'combobox', id: 'cob_retrieve', queryMode: 'local', displayField: 'parameterName', valueField: 'parameterCode', store: retrieveStore, editable: false, allowBlank: false }
            }
        ],
        tbar: [{ xtype: 'button', text: INSERTSHIPPING, iconCls: 'icon-user-add', handler: onAddClick}],
        buttonAlign: 'left',
        buttons: [
            {
                text: SAVE,
                id: 'btn_submit',
                handler: function () {
                    cellEditing.completeEdit();

                    if (window.parent.getChannelId() == "") {
                        alert(NEXT_INSERT);
                        return;
                    }

                    var gdShipping = Ext.getCmp("gdShipping").store.data.items;
                    var InsertValues = "";
                    for (var a = 0; a < gdShipping.length; a++) {
                        var shippingcarrior = gdShipping[a].get("shipping_carrior");
                        var shipping_carrior = gdShipping[a].get("shipping_carrior_content");
                        //var shipping_type = gdShipping[a].get("shipping_type");
                        var shipco = gdShipping[a].get("shipco");
                        var shipco_content = gdShipping[a].get("shipco_content");
                        //var shippingtype = gdShipping[a].get("shipping_type_content");
                        var n_threshold = gdShipping[a].get("n_threshold");
                        var n_fee = gdShipping[a].get("n_fee");
                        var n_return_fee = gdShipping[a].get("n_return_fee");
                        var l_threshold = gdShipping[a].get("l_threshold");
                        var l_fee = gdShipping[a].get("l_fee");
                        var l_return_fee = gdShipping[a].get("l_return_fee");
                        var retrieve = gdShipping[a].get("retrieve_mode");

                        if (shipping_carrior == "" || shipco_content == "") {
                            Ext.Msg.alert(MESSAGE, NOTNULL_SHIPPING_CARRIORANDTYPE);
                            return;
                        }

                        InsertValues += shippingcarrior + ',' + shipping_carrior + ',' + n_threshold + ',' + l_threshold + ',' + n_fee + ',' + l_fee + ',' + n_return_fee + ',' + l_return_fee + ',' + shipco + ',' + retrieve + ',' + shipco_content + ';';
                    }

                    Ext.Ajax.request({
                        url: '/Channel/SaveChannelShipping',
                        params: {
                            'Channel_id': window.parent.getChannelId(),
                            'InsertValues': InsertValues
                        },
                        success: function (response, opts) {
                            var result = eval("(" + response.responseText + ")");
                            Ext.Msg.alert(MESSAGE, result.msg);
                            ShippingStore.load({
                                params: {
                                    channel_id: window.parent.getChannelId()
                                }
                            });
                        },
                        failure: function (response) {
                            var result = eval("(" + response.responseText + ")");
                            Ext.Msg.alert(MESSAGE, result.msg);
                        }
                    });
                }
            }, {
                text: RETURN,
                hidden: window.parent.getChannelId() == '',
                handler: function () {
                    window.parent.location.href = 'http://' + window.parent.location.host + '/channel/channellist';
                }
            }]
    });

    Ext.create('Ext.container.Viewport', {
        layout: 'fit',
        items: [gdShipping],
        renderTo: Ext.getBody(),
        autoScroll: true,
        listeners: {
            resize: function () {
                gdShipping.width = document.documentElement.clientWidth;
                this.doLayout();
            }
        }
    });

    DeliverStore.load(function (records, operation, success) {
        channel_id = window.parent.getChannelId();
        if (channel_id != "") {
            ShippingStore.load({
                params: {
                    channel_id: channel_id
                }
            });
        }
    });
});
retrieveStore.load(function (records, operation, success) {
    channel_id = window.parent.getChannelId();
    if (channel_id != "") {
        ShippingStore.load({
            params: {
                channel_id: channel_id
            } 
        });
    }
});

function setDisable(flag) {
    Ext.getCmp("btn_submit").setDisabled(flag);
}

function onAddClick() {    
    var r = Ext.create('Shipping', {
        shipping_carrior: '',
        shipping_carrior_content: '',
        n_threshold: '0',
        l_threshold: '0',
        n_fee: '0',
        l_fee: '0',
        n_return_fee: '0',
        l_return_fee: '0',
        retrieve_mode:'0'

    });
    ShippingStore.insert(0, r);
    cellEditing.startEditByPosition({ row: 0, column: 2 });
}

function DeliverRenderer(value,m,r,row,column){
    var index = DeliverStore.find("parameterCode", value);
    var recode = DeliverStore.getAt(index);
    currentRow = row;
    if (recode) {
        return recode.get("parameterName");
    } else {
        return value;
    }
}

function RetrieveRenderer(value, m, r, row, column) {
    var index = retrieveStore.find("parameterCode", value);
    var recode = retrieveStore.getAt(index);
    currentRow = row;
    if (recode) {
        return recode.get("parameterName");
    } else {
        return value;
    }
}

function DeliverTypeRenderer(value) {
    var index = DeliverTypeStore.find("value", value);
    var recode = DeliverTypeStore.getAt(index);
    if (recode) {
        return recode.get("text");
    } else {
        return value;
    }
}