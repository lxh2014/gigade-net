var status = [
    { boxLabel: STATUS_NORMAL, name: 'status', inputValue: '1', checked: true },
    { boxLabel: STATUS_CLOASE, name: 'status', inputValue: '2' },
    { boxLabel: STATUS_NO_DELIVERY, name: 'status', inputValue: '3' }
];

var SaveWin = function (row) {

    var DeliveryStoreForm = Ext.create('Ext.form.Panel', {
        id: 'DeliveryStoreForm',
        frame: true,
        plain: true,
        autoScroll: true,
        defaultType: 'textfield',
        layout: 'anchor',
        labelWidth: 45,
        url: '/DeliveryStore/SaveDeliveryStore',
        defaults: { anchor: "95%", msgTarget: "side" },
        items: [{
            hidden: true,
            name: 'rowid'
        }, {
            xtype: 'combobox',
            store: paraDelivery,
            displayField: 'parameterName',
            valueField: 'ParameterCode',
            queryMode: 'local',
            fieldLabel: DELIVERY_STORE_NAME,
            allowBlank: false,
            editable: false,
            name: 'delivery_store_id'
        }, {
            xtype: 'combobox',
            store: bigZip,
            displayField: 'big',
            valueField: 'bigcode',
            queryMode: 'local',
            fieldLabel: BIG_TEXT,
            editable: false,
            name: 'bigcode',
            id: 'bigcode',
            listeners: {
                select: function (combo, record) {
                    var code = combo.value;

                    var middleComboBox = Ext.getCmp("middlecode");
                    middleComboBox.clearValue();
                    middleComboBox.store.load
                ({
                    params: {
                        topValue: code
                    }
                });

                    var smallComboBox = Ext.getCmp("smallcode");
                    smallComboBox.clearValue();
                    smallComboBox.setValue("");
                    smallZip.removeAll();
                }
            }
        }, {
            xtype: 'combobox',
            store: middleZip,
            displayField: 'middle',
            valueField: 'middlecode',
            queryMode: 'local',
            fieldLabel: MIDDLE_TEXT,
            editable: false,
            name: 'middlecode',
            id: 'middlecode',
            listeners: {
                select: function (combo, record) {
                    var code = combo.value;
                    var smallComboBox = Ext.getCmp("smallcode");
                    smallComboBox.clearValue();
                    smallComboBox.store.load
                ({
                    params: {
                        topValue: code,
                        topText: combo.rawValue
                    }
                });
                },
                beforequery: function (qe) {
                    delete qe.combo.lastQuery;
                    var bigComboBox = Ext.getCmp("bigcode");
                    middleZip.load({
                        params: { topValue: bigComboBox.getValue() }
                    });
                }
            }
        }, {
            xtype: 'combobox',
            store: smallZip,
            displayField: 'small',
            valueField: 'zipcode',
            queryMode: 'local',
            fieldLabel: SMALL_TEXT,
            editable: false,
            submitValue: false,
            name: 'smallcode',
            id: 'smallcode',
            listeners: {
                beforequery: function (qe) {
                    delete qe.combo.lastQuery;
                    var middleComboBox = Ext.getCmp("middlecode");
                    smallZip.load({
                        params: { topValue: middleComboBox.getValue(), topText: middleComboBox.rawValue }
                    });
                }
            }
        }, {
            fieldLabel: STORE_ID,
            allowBlank: false,
            maxLength: 6,
            minLength: 6,
            submitValue: false,
            id: 'store_id',
            name: 'store_id'
        }, {
            fieldLabel: STORE_NAME,
            allowBlank: false,
            submitValue: false,
            id: 'store_name',
            name: 'store_name'
        }, {
            fieldLabel: STORE_ADDRESS,
            allowBlank: false,
            submitValue: false,
            id: 'address',
            name: 'address'
        }, {
            fieldLabel: STORE_PHONE,
            allowBlank: false,
            submitValue: false,
            id: 'phone',
            name: 'phone'
        }, {
            fieldLabel: STORE_STATUS,
            xtype: 'radiogroup',
            items: status,
            allowBlank: false,
            columns: 4
        }],
        buttonAlign: 'center',
        buttons: [
    {
        text: SAVE,
        formBind: true,
        disabled: true,
        handler: function () {
            var form = this.up('form').getForm();
            if (form.isValid()) {
                form.submit({
                    params: {
                        BigText: Ext.getCmp('bigcode').getRawValue(),
                        MiddleText: Ext.getCmp('middlecode').getRawValue(),
                        smallcode: Ext.getCmp('smallcode').getRawValue(),
                        store_id: Ext.htmlEncode(Ext.getCmp('store_id').getValue()),
                        store_name: Ext.htmlEncode(Ext.getCmp('store_name').getValue()),
                        address: Ext.htmlEncode(Ext.getCmp('address').getValue()),
                        phone: Ext.htmlEncode(Ext.getCmp('phone').getValue())
                    },
                    success: function (form, action) {
                        var result = Ext.decode(action.response.responseText);
                        Ext.Msg.alert(INFORMATION, SUCCESS);
                        if (result.success) {
                            Ext.getCmp('SaveWin').close();
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


    Ext.create('Ext.window.Window', {
        id: 'SaveWin',
        title: SAVETITLE,
        items: [DeliveryStoreForm],
        width: 390,
        height: document.documentElement.clientHeight * 330 / 783,
        layout: 'fit',
        labelWidth: 100,
        closeAction: 'destroy',
        resizable: false,
        modal: 'true',
        iconCls: row ? "icon-edit" : "icon-add",
        bodyStyle: 'padding:5px 5px 5px 5px',
        listeners: {
            "show": function () {
                if (row) {
                    middleZip.load({ params: { topValue: row.data.bigcode } });
                    smallZip.load({ params: { topValue: row.data.middlecode, topText: row.data.middle } });
                    DeliveryStoreForm.getForm().loadRecord(row);
                }
                else {
                    DeliveryStoreForm.getForm().reset();
                }
            },
            "close": function () {
                deliveryStore.load();
            }
        }
    }).show();
}