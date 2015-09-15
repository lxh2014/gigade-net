editFunction = function (row, store) {
    var editFrm = Ext.create('Ext.form.Panel', {
        id: 'editFrm',
        frame: true,
        plain: true,
        constrain: true,
        //defaultType: 'textfield',
        autoScroll: true,
        layout: 'anchor',
        labelWidth: 50,
        url: '/WareHouse/GetIPlasEdit',
        defaults: { anchor: "95%", msgTarget: "side" },
        items: [
            {
                xtype: 'textfield',
                fieldLabel: 'plas_id',
                id: 'plas_id',
                name: 'plas_id',
                hidden: true,
                submitValue: true

            },
            {
                xtype: 'textfield',
                fieldLabel: '商品品號/條碼',
                labelWidth: 100,
                id: 'item_id',
                name: 'item_id',
                submitValue: true,
                allowBlank: false,
                listeners: {
                    blur: function () {
                        var id = Ext.getCmp('item_id').getValue();
                        Ext.Ajax.request({
                            url: "/WareHouse/Getprodbyid",
                            method: 'post',
                            type: 'text',
                            params: {
                                id: id
                            },
                            success: function (form, action) {
                                var result = Ext.decode(form.responseText);
                                if (result.success) {
                                    msg = result.msg;
                                    Ext.getCmp("product_name").setValue(msg);
                                }
                                else {
                                    Ext.getCmp("product_name").setValue("沒有該商品信息！");
                                }
                            }
                        });
                    }
                }
            },
            {
                xtype: 'displayfield',
                fieldLabel: '商品名稱',
                labelWidth: 100,
                id: 'product_name',
                submitValue:true           
            },
            {
                xtype: 'textfield',
                name: 'loc_id',
                id: 'loc_id',
                maxLength: 25,
                allowBlank: false,
                submitValue: true,
                fieldLabel: '料位編號'
            },
            {
                xtype: 'numberfield',
                name: 'loc_stor_cse_cap',
                id: 'loc_stor_cse_cap',
                submitValue: true,
                allowBlank: false,
                value: 0,
                minValue: 0,
                fieldLabel: '料位容量'
            }
        ],
        buttons: [
            {
                formBind: true,
                disabled: true,
                text: SAVE,
                handler: function () {
                    var form = this.up('form').getForm();
                    if (form.isValid()) {
                        form.submit({
                            params: {
                                plas_id: Ext.htmlEncode(Ext.getCmp('plas_id').getValue()),
                                item_id: Ext.htmlEncode(Ext.getCmp('item_id').getValue()),
                                loc_id: Ext.htmlEncode(Ext.getCmp('loc_id').getValue()),
                                loc_stor_cse_cap: Ext.htmlEncode(Ext.getCmp('loc_stor_cse_cap').getValue())
                            },
                            success: function (form, action) {
                                var result = Ext.decode(action.response.responseText);  //Ext.decode 把json格式的字符串轉化為json對象
                                if (result.success) {                                   
                                    if (result.msg != undefined) {
                                        Ext.Msg.alert(INFORMATION, result.msg);
                                    }
                                    else {
                                        Ext.Msg.alert(INFORMATION, "操作成功！");
                                    }
                                    editWin.close();
                                    IplasStore.load();
                                }
                                else {
                                    Ext.Msg.alert(INFORMATION, result.Msg);
                                    IplasStore.load();
                                }
                            },
                            failure: function (form, action) {
                                var result = Ext.decode(action.response.responseText);
                                if (result.msg != undefined) {
                                    Ext.Msg.alert(INFORMATION, result.msg);
                                }
                            }
                        });
                    }
                }
            }
        ]
    });
    var editWin = Ext.create('Ext.window.Window', {
        title: '商品主料位編輯',
        iconCls: row ? 'icon-user-edit' : 'icon-user-add',
        id: 'editWin',
        width: 400,
        height:250,
        layout: 'fit',
        items: [editFrm],
        constrain: true,
        closeAction: 'destroy',
        modal: true,
        resizable: false,
        labelWidth: 60,
        bodyStyle: 'padding:5px 5px 5px 5px',
        closable: false,
        tools: [
            {
                type: 'close',
                qtip: '是否關閉',
                handler: function (event, toolEl, panel) {
                    Ext.MessageBox.confirm(CONFIRM, IS_CLOSEFORM, function (btn) {
                        if (btn == "yes") {
                            Ext.getCmp('editWin').destroy();
                        } else {
                            return false;
                        }
                    });
                }
            }
        ],
        listeners: {
            'show': function () {
                if (row != null) {
                    editFrm.getForm().loadRecord(row);
                    Ext.getCmp('item_id').setDisabled(true);
                    //Ext.getCmp('loc_id').setDisabled(true);

                }
            }
        }
    });
    editWin.show();
}