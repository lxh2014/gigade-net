/*****************************************************************************/

//定義物流設定Model&Store
Ext.define('gigade.Transport', {
    extend: 'Ext.data.Model',
    fields: [
        { name: "Rid", type: "int" },
        { name: "Delivery_store_id", type: "int" },
        { name: "Freight_big_area", type: "int" },
        { name: "Freight_type", type: "int" },
        { name: "Delivery_freight_set", type: "int" },
        { name: "Active", type: "int" },
        { name: "Charge_type", type: "int" },
        { name: "Shipping_fee", type: "int" },
        { name: "Return_fee", type: "int" },
        { name: "Size_limitation", type: "int" },
        { name: "Length", type: "int" },
        { name: "Width", type: "int" },
        { name: "Height", type: "int" },
        { name: "Weight", type: "int" },
        { name: "Pod", type: "int" },
        { name: "Freight_type_Name", type: "string" },
        { name: "Area_name", type: "string" },
        { name: "Store_name", type: "string" },
        { name: "Note", type: "string" }]
});

var transportStore = Ext.create('Ext.data.Store', {
    autoDestroy: true,
    model: 'gigade.Transport',
    proxy: {
        type: 'ajax',
        url: '/DeliveryStore/QueryShippingCarriorAll',
        reader: {
            type: 'json',
            root: 'items'
        }
    },
    autoLoad: true
});


var sm = Ext.create('Ext.selection.CheckboxModel', {
    listeners: {
        selectionchange: function (sm, selections) {
            //Ext.getCmp("transport").down('#add').setDisabled(selections.length == 0);
            Ext.getCmp("transport").down('#edit').setDisabled(selections.length == 0);
            Ext.getCmp("transport").down('#remove').setDisabled(selections.length == 0);
        }
    }
})
//定義物流設定列表
var transport = Ext.create('Ext.grid.Panel', {
    id: 'transport',
    store: transportStore,
    width: document.documentElement.clientWidth,
    columnLines: true,
    frame: true,
    columns: [
        { header: NUM, xtype: 'rownumberer', width: 46, align: 'center' },
        //{ header: NUM, dataIndex: 'Rid', align: 'center', width: 50 },
        { header: LOGISTICS_COMPANY, dataIndex: 'Store_name', width: 100, align: 'center' },
        { header: DELIVER_REGION, dataIndex: 'Area_name', width: 100, align: 'center' },
        { header: LOGISTICS_DISTRMODE, dataIndex: 'Freight_type_Name', width: 120, align: 'center' },
        {
            header: NORMAL_LOW_TEMPERATURE, dataIndex: 'Delivery_freight_set', width: 90, align: 'center',
            renderer: function (value) {
                if (value == '1') {
                    return NORMAL_TEMPERATURE;
                } else {
                    return LOW_TEMPERATURE;
                }
            }
        }, {
            header: IF_USING, dataIndex: 'Active', width: 60, align: 'center',
            renderer: function (value) {
                if (value == '1') {
                    return YES;
                } else {
                    return NO;
                }
            }
        }, {
            header: CHARGE, dataIndex: 'Charge_type', width: 70, align: 'center',
            renderer: function (value) {
                if (value == '1') {
                    return CONSTANT;
                } else {
                    return ADD_UP;
                }
            }
        }, { header: CARRYIAGE, dataIndex: 'Shipping_fee', width: 60, align: 'center' },
        { header: RETURN_CARRYIAGE, dataIndex: 'Return_fee', width: 70, align: 'center' },
        {
            header: HANE_SIZE_RESTRICT, dataIndex: 'Size_limitation', width: 90, align: 'center',
            renderer: function (value) {
                if (value == '1') {
                    return HAVE;
                } else if (value == '2') {
                    return NOT;
                }
            }
        }, {
            header: LONG, dataIndex: 'Length', width: 60, align: 'center',
            renderer: function (value) {
                if (value == '0') {
                    return '--';
                } else return value;
            }
        }, {
            header: WIDE, dataIndex: 'Width', width: 60, align: 'center',
            renderer: function (value) {
                if (value == '0') {
                    return '--';
                } else return value;
            }
        }, {
            header: HIGH, dataIndex: 'Height', width: 60, align: 'center',
            renderer: function (value) {
                if (value == '0') {
                    return '--';
                } else return value;
            }
        }, {
            header: WEIGHT, dataIndex: 'Weight', width: 60, align: 'center',
            renderer: function (value) {
                if (value == '0') {
                    return '--';
                } else return value;
            }
        },
        { header: CASH_ON_DELIVERY, dataIndex: 'Pod', width: 100, align: 'center' },
        { header: SPECIAL_LIMTI, dataIndex: 'Note', width: 200, align: 'center' }],
    tbar: [
        //{ xtype: 'ManageButton', addtext: '添加', updatetext: '編輯', id: 'Button', addhandler: onAddClick, updatehandler: onEditClick },
        { xtype: 'button', text: ADD, id: 'add', iconCls: 'ui-icon ui-icon-add', handler: onAddClick },
        { xtype: 'button', text: EDIT, id: 'edit', iconCls: 'ui-icon ui-icon-pencil', disabled: true, handler: onEditClick },
        { xtype: 'button', text: REMOVE, id: 'remove', iconCls: 'ui-icon ui-icon-delete', disabled: true, handler: onRemoveClick },
        '->',
        { text: ' ' }
    ],
    listeners: {
        scrollershow: function (scroller) {
            if (scroller && scroller.scrollEl) {
                scroller.clearManagedListeners();
                scroller.mon(scroller.scrollEl, 'scroll', scroller.onElScroll, scroller);
            }
        }
    },
    selModel: sm
})


Ext.onReady(function () {
    Ext.create('Ext.container.Viewport', {
        layout: 'fit',
        items: [transport],
        renderTo: Ext.getBody(),
        autoScroll: true,
        listeners: {
            resize: function () {
                transport.width = document.documentElement.clientWidth;
                this.doLayout();
            }
        }
    });
    ToolAuthority();
});


//添加按鈕
var record;
function onAddClick() {
    record = null;
    editWin.show();
}


//編輯按鈕
function onEditClick() {
    var row = Ext.getCmp("transport").getSelectionModel().getSelection();
    if (row.length == 0) {
        Ext.Msg.alert(INFORMATION, NO_SELECTION);
    }
    else if (row.length > 1) {
        Ext.Msg.alert(INFORMATION, ONE_SELECTION);
    } else if (row.length == 1) {
        record = row[0];
        editWin.show();
    }
}




//刪除按鈕
function onRemoveClick() {
    var row = Ext.getCmp("transport").getSelectionModel().getSelection();
    if (row.length < 0) {
        Ext.Msg.alert(INFORMATION, NO_SELECTION);
    }
    else {
        Ext.Msg.confirm(CONFIRM, Ext.String.format(DELETE_INFO, row.length), function (btn) {
            if (btn == 'yes') {
                var rowIDs = '';
                for (var i = 0; i < row.length; i++) {
                    rowIDs += row[i].data.Rid + ',';
                }
                rowIDs = rowIDs.slice(0, -1);
                Ext.Ajax.request({
                    url: '/DeliveryStore/DeleteShippingCarrior',
                    method: 'post',
                    params: { rids: rowIDs },
                    success: function (form, action) {
                        var result = Ext.decode(form.responseText);
                        Ext.Msg.alert(INFORMATION, DELETE_SUCCESS);//刪除成功
                        if (result.success) {
                            transportStore.load();
                        }
                    },
                    failure: function () {
                    }
                });
            }
        });
    }
}

//添加window
var editFrm = Ext.create('Ext.form.Panel', {
    id: 'editFrm',
    frame: true,
    plain: true,
    defaultType: 'textfield',
    layout: 'anchor',
    labelWidth: 45,
    url: '/DeliveryStore/SaveShippingCarrior',
    defaults: { anchor: "95%", msgTarget: "side" },
    items: [{
        fieldLabel: 'ID',
        id: 'rid',
        name: 'Rid',
        hidden: true
    },
    delieverStore,
    delieverRange,
    freightType,
    {
        xtype: 'radiogroup',
        fieldLabel: NORMAL_LOW_TEMPERATURE,
        columns: 2,
        vertical: true,
        id: 'delivery_freight_set',
        name: 'Delivery_freight_set',
        defaults: {
            name: 'Delivery_freight_set'
        },
        items: [
        { boxLabel: NORMAL_TEMPERATURE, inputValue: '1', checked: true },
        { boxLabel: LOW_TEMPERATURE, inputValue: '2' }
        ]
    }, {
        xtype: 'radiogroup',
        fieldLabel: IF_USING,
        allowBlank: false,
        columns: 2,
        vertical: true,
        id: 'active',
        name: 'Active',
        defaults: {
            name: 'Active'
        },
        items: [
        { boxLabel: YES, inputValue: '1', checked: true },
        { boxLabel: NO, inputValue: '2' }
        ]
    }, {
        xtype: 'radiogroup',
        fieldLabel: CHARGE,
        allowBlank: false,
        columns: 2,
        vertical: true,
        id: 'charge_type',
        name: 'Charge_type',
        defaults: {
            name: 'Charge_type'
        },
        items: [
        { boxLabel: CONSTANT, inputValue: '1', checked: true },
        { boxLabel: ADD_UP, inputValue: '2' }
        ]
    }, {
        xtype: 'numberfield',
        decimalPrecision: 0,
        minValue: 0,
        fieldLabel: CARRYIAGE,
        id: 'shipping_fee',
        name: 'Shipping_fee',
        allowBlank: false
    }, {
        xtype: 'numberfield',
        decimalPrecision: 0,
        minValue: 0,
        fieldLabel: RETURN_CARRYIAGE,
        id: 'return_fee',
        name: 'Return_fee',
        allowBlank: false
    }, {
        xtype: 'radiogroup',
        fieldLabel: HANE_SIZE_RESTRICT,
        columns: 2,
        vertical: true,
        name: 'Size_limitation',
        id: 'size_limitation',
        defaults: {
            name: 'Size_limitation'
        },
        items: [
        {
            boxLabel: HAVE, name: 'Size_limitation', inputValue: '1', checked: true
        },
        { boxLabel: NOT, name: 'Size_limitation', inputValue: '2' }
        ]
        ,
        listeners: {
            change: function (radio, newValue, oldValue) {
                if (newValue.Size_limitation == 2) {
                    Ext.getCmp("length").setDisabled(true);
                    Ext.getCmp("width").setDisabled(true);
                    Ext.getCmp("height").setDisabled(true);
                    Ext.getCmp("length").setValue(0);
                    Ext.getCmp("width").setValue(0);
                    Ext.getCmp("height").setValue(0);
                } else {
                    Ext.getCmp("length").setDisabled(false);
                    Ext.getCmp("width").setDisabled(false);
                    Ext.getCmp("height").setDisabled(false);
                }
            }
        }
    }, {
        xtype: 'numberfield',
        decimalPrecision: 0,
        minValue: 0,
        fieldLabel: LONG,
        id: 'length',
        name: 'Length'
        //allowBlank: false
    }, {
        xtype: 'numberfield',
        decimalPrecision: 0,
        minValue: 0,
        fieldLabel: WIDE,
        id: 'width',
        name: 'Width'
        //allowBlank: false
    }, {
        xtype: 'numberfield',
        decimalPrecision: 0,
        minValue: 0,
        fieldLabel: HIGH,
        id: 'height',
        name: 'Height'
        //allowBlank: false
    }, {
        xtype: 'numberfield',
        decimalPrecision: 0,
        minValue: 0,
        fieldLabel: WEIGHT,
        id: 'weight',
        name: 'Weight',
        allowBlank: false
    }, {
        xtype: 'numberfield',
        decimalPrecision: 0,
        minValue: 0,
        fieldLabel: CASH_ON_DELIVERY,
        id: 'pod',
        name: 'Pod',
        allowBlank: false
    }, {
        fieldLabel: SPECIAL_LIMTI,
        id: 'note',
        name: 'Note',
        allowBlank: false
    }],
    buttons: [{
        text: SAVE,
        formBind: true,
        //disabled: true,
        handler: function () {
            var form = this.up('form').getForm();
            if (form.isValid()) {
                form.submit({
                    success: function (form, action) {
                        var result = Ext.decode(action.response.responseText);
                        Ext.Msg.alert(INFORMATION, SAVE_SUCCESS);
                        if (result.success) {
                            transportStore.load();
                            editWin.hide();
                        }
                    },
                    failure: function () {
                        Ext.Msg.alert(INFORMATION, SAVE_FAIL);
                    }
                });
            }
        }
    }]
});




var editWin = Ext.create('Ext.window.Window', {
    title: EDIT_DELIVERY_COMPANY,//編輯物流公司
    iconCls: 'ui-icon ui-icon-add',
    width: 360,
    height: document.documentElement.clientHeight * 490 / 783,
    layout: 'fit',
    items: [editFrm],
    closeAction: 'hide',
    modal: true,
    resizable: false,
    labelWidth: 60,
    labelAlign: 'right',
    bodyStyle: 'padding:5px 5px 5px 5px',
    listeners: {
        show: function () {
            if (record) {
                Ext.getCmp("delivery_freight_set").setValue({ 'Delivery_freight_set': record.data.Delivery_freight_set });//常低溫  
                Ext.getCmp("active").setValue({ 'Active': record.data.Active });//啟用     
                Ext.getCmp("charge_type").setValue({ 'Charge_type': record.data.Charge_type });//收費
                Ext.getCmp("size_limitation").setValue({ 'Size_limitation': record.data.Size_limitation });//有size限制

                Ext.getCmp("delivery_store_id").setValue(record.data.Delivery_store_id);
                Ext.getCmp("freight_big_area").setValue(record.data.Freight_big_area);
                FreightTypeStore.load({
                    params: { 'rangeid': record.data.Freight_big_area },
                    callback: function (records, operation, success) {
                        Ext.getCmp("freight_type").setValue(records[0].data.ParameterCode);
                    }
                });
                editFrm.loadRecord(record);
            }
            else {
                editFrm.getForm().reset();
            }
        }
    }
});




