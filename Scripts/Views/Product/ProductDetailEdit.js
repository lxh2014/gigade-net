editProductDetailFunction = function (row, store) {
    var editProductDetailFrm = Ext.create('Ext.form.Panel', {
        id: 'editProductDetailFrm',
        frame: true,
        plain: true,
        constrain: true,
        defaultType: 'textfield',
        autoScroll: true,
        layout: 'anchor',
        labelWidth: 45,
        url: '/Product/UpdateProductDetail',
        defaults: { anchor: "99%" },
        items: [
        {
            xtype: 'fieldcontainer',
            layout: 'column',
            items: [
            {
                xtype: 'displayfield',
                fieldLabel: PRODUCTID,
                id: 'product_id',
                name: 'product_id',
                labelWidth: 65,
                submitValue: true,
                width: 120
            },
            {
                xtype: 'displayfield',
                fieldLabel: '| ' + PRODUCT_NAME,
                id: 'product_name',
                name: 'product_name',
                labelWidth: 75,
                submitValue: true,
                width: 260
            }]
        },
        {
            xtype: 'fieldcontainer',
            layout: 'column',
            items: [
            {
                xtype: 'checkbox',
                id: 'check',
                name: 'check',
                fieldLabel: BE_RELIEVED_MESSAGE,
                labelWidth: 65,
                margin: '0 5 0 0',
                handler: checked
            },
            {
                xtype: 'textfield',
                id: 'product_IDS',
                margin: '0  0 0 5',
                labelWidth: 95,
                width:225,
                name: 'product_IDS',
                fieldLabel: '| ' + PRODUCT_SON_ID
            },
            {
                xtype: 'button',
                margin: '0 0 0 5',
                text: AFFIRM,
                handler: function () {
                    if (Ext.getCmp('product_IDS').getValue() != "") {
                        search();
                    } else {
                        Ext.Msg.alert(INFORMATION, PRODUCT_INPUT_SON_ID);
                    }
                }
            }
            ]
        },
        {
            xtype: 'displayfield',
            fieldLabel: PRODUCT_DETAILS_TEXT
        },
        {
            xtype: 'textareafield',
            id: 'product_detail_text',
            name: 'product_detail_text',
            autoScroll: true,
            height: 250,
            width: 390,
            allowBlank: false,
            submitValue: true
        }
        ],
        buttons: [
        {
            text: THIS_RESET,
            id: 'reset',
            iconCls: 'ui-icon ui-icon-reset',
            align: 'center',
            listeners: {
                click: function () {
                    Ext.getCmp('product_IDS').setValue('');
                    Ext.getCmp('check').setValue('');
                    Ext.getCmp('product_detail_text').reset();
                    Ext.getCmp('check').setDisabled(false);
                }
            }
        },
        {
            xtype: 'button',
            vtype: 'submit',
            formBind: true,
            disabled: true,
            text: THIS_SAVE,
            handler: function () {
                var form = this.up('form').getForm();
                var text = Ext.getCmp('product_detail_text').getValue().toString();
                var str = text.replace(/ /g, '')
                if (str.length == 0) {
                    Ext.Msg.alert(INFORMATION, CAN_NOT_BE_NULL_STRING_MESSAGE);
                    return;
                }
                if (form.isValid()) {
                    form.submit({
                        params: {
                            product_id: Ext.getCmp('product_id').getValue(),
                            product_detail_text: Ext.getCmp('product_detail_text').getValue(),
                            product_IDS: Ext.getCmp('product_IDS').getValue()
                        },
                        success: function (form, action) {
                            var result = Ext.decode(action.response.responseText);
                            if (result.success) {
                                Ext.Msg.alert(INFORMATION, result.msg);
                                store.load();
                                editProductDetailWin.close();
                            }
                            else {
                                Ext.Msg.alert(INFORMATION, result.msg);
                                store.load();
                                editProductDetailWin.close();
                            }
                        },
                        failure: function (form, action) {
                            var result = Ext.decode(action.response.responseText);
                            Ext.Msg.alert(INFORMATION, result.msg);
                            store.load();
                            editProductDetailWin.close();
                        }
                    });

                }

            }
        }]
    });
    function search() {
        var ids = Ext.getCmp('product_IDS').getValue();
        ids = ids.replace(/[，]/g, ",");
        ids = ids.replace(/[ ]/g, "");
        var id = ids.split(',');
        var reg = /^[0-9]+$/;
        for (var i = 0; i < id.length; i++) {
            if (reg.test(id[i]) && id[i].length <= 9) {
                if (id[i] <= 10000) {
                    Ext.Msg.alert(INFORMATION, PRODUCT_CODE_INPUT_IS_NOT_LEGAL_MESSAGE);
                    return;
                }
            } else {
                Ext.Msg.alert(INFORMATION, PRODUCT_CODE_INPUT_IS_NOT_LEGAL_MESSAGE);
                return;
            }
        }
        Ext.Ajax.request({
            url: '/Product/GetProductDetialText',
            method: 'post',
            params: {
                product_IDS: ids
            },
            success: function (msg) {
                var result = eval("(" + msg.responseText + ")");
                var stories = result.data;
                if (result.success) {
                    var text = Ext.getCmp('product_detail_text').getValue();
                    if (text != "") {
                        for (var a = 0; a < stories.length; a++) {
                            text += '\n' + stories[a].product_detail_text;
                        }
                    } else {
                        for (var b = 0; b < stories.length; b++) {
                            if (b < stories.length - 1) {
                                text += stories[b].product_detail_text + '\n';
                            } else {
                                text += stories[b].product_detail_text;
                            }
                        }

                    }
                    Ext.getCmp('product_detail_text').setValue(text);
                    Ext.getCmp('product_IDS').setValue('');
                } else {
                    Ext.Msg.alert(INFORMATION, result.data + PRODUCT_DETAILS_TEXT_NOT_EDIT_MESSAGE);
                }
            },
            failure: function () {
                Ext.Msg.alert(INFORMATION, FAILURE);
            }
        });
    }
    function checked() {
        if (Ext.getCmp('check').checked) {
            Ext.Ajax.request({
                url: '/Product/GetProductDetialSafe',
                method: 'post',
                success: function (msg) {
                    var result = eval("(" + msg.responseText + ")");
                    var stories = result.data;
                    if (result.success) {
                        var text = stories + '\n';
                        text += Ext.getCmp('product_detail_text').getValue();
                        Ext.getCmp('product_detail_text').setValue(text);
                    }
                },
                failure: function () {
                    Ext.Msg.alert(INFORMATION, FAILURE);
                }
            });
            Ext.getCmp('check').setDisabled(true);
        }
    }
    var editProductDetailWin = Ext.create('Ext.window.Window', {
        title: PRODUCT_DETAILS_CONTENT,
        iconCls: 'icon-user-edit',
        id: 'editProductDetailWin',
        width: 430,
        height: 430,
        layout: 'fit',
        items: [editProductDetailFrm],
        constrain: true,
        closeAction: 'destroy',
        modal: true,
        labelWidth: 60,
        bodyStyle: 'padding:5px 5px 5px 5px',
        closable: false,
        tools: [
        {
            type: 'close',
            qtip: AFFIRM_CLOSE,
            handler: function (event, toolEl, panel) {
                Ext.MessageBox.confirm(CONFIRM, IS_CLOSEFORM, function (btn) {
                    if (btn == "yes") {
                        Ext.getCmp('editProductDetailWin').destroy();
                    }
                    else {
                        return false;
                    }
                });
            }
        }],
        listeners: {
            'show': function () {
                editProductDetailFrm.getForm().loadRecord(row);
            }
        }
    });
    editProductDetailWin.show();
}

