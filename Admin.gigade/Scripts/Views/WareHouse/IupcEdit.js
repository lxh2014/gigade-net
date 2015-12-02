

editFunction = function (row, store) {
    



    var editFrm = Ext.create('Ext.form.Panel', {
        id: 'editFrm',
        frame: true,
        plain: true,
        constrain: true,
        defaultType: 'textfield',
        autoScroll: true,
        layout: 'anchor',
        labelWidth: 45,
        url: '/WareHouse/SaveIupc',
        defaults: { anchor: "95%", msgTarget: "side" },
        items: [
            {
                xtype: 'textfield',
                fieldLabel: ROWID,
                id: 'row_id',
                name: 'row_id',
                submitValue: true,
                hidden: true
            },
            {
                xtype: 'textfield',
                fieldLabel: PRODID,
                id: 'item_id',
                name: 'item_id',
                allowBlank: false,
                submitValue: true,
                listeners: { 
                    blur: function () {
                        var id = Ext.getCmp('item_id').getValue();
                        //Ext.Ajax.request({
                        //    url: "/WareHouse/GetProdInfo",
                        //    method: 'post',
                        //    type: 'text',
                        //    params: {
                        //        id: id
                        //    },
                        //    success: function (form, action) {
                        //        var result = Ext.decode(form.responseText);
                        //        if (result.success) {
                        //            msg = result.msg;
                        //            Ext.getCmp("product_name").setValue(msg);
                        //        }
                        //        else {
                        //            Ext.getCmp("product_name").setValue("沒有該商品信息！");
                        //        }
                        //    }
                        //});
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
                fieldLabel: PRODNAME,
                id: 'product_name',
                name: 'product_name',
                allowBlank: false,
                submitValue: true
            },
        {
            xtype: 'textfield',
            name: 'upc_id',
            id: 'upc_id',
            submitValue: true,
            allowBlank: false,
            maxLength: 25,
            //minLength: 8,
            //regex: /^[a-zA-Z0-9]+$/,
            fieldLabel: UPCID
        },
        
        {
            xtype: 'combobox', //類型
            editable: false,
            id: 'upc_type_flg',
            fieldLabel: "條碼類型",
            name: 'upc_type_flg',
            store: IupcTypeStore,
            margin: '20 0 0 0',
            lastQuery: '',
            displayField: 'parameterName',
            valueField: 'ParameterCode',
            allowBlank: false,
            emptyText: "請選擇條碼類型",
        }
        ],
        buttons: [{
            formBind: true,
            disabled: true,
            text: SAVE,
            handler: function () {
                var form = this.up('form').getForm();
                if (form.isValid()) {
                    form.submit({
                        params: {
                            row_id: Ext.htmlEncode(Ext.getCmp('row_id').getValue().trim()),
                            item_id: Ext.htmlEncode(Ext.getCmp('item_id').getValue().trim()),
                            upc_id: Ext.htmlEncode(Ext.getCmp('upc_id').getValue().trim()),
                            upc_type_flg: Ext.htmlEncode(Ext.getCmp('upc_type_flg').getValue())
                        },
                        success: function (form, action) {
                            var result = Ext.decode(action.response.responseText);
                            if (result.success) {
                                Ext.Msg.alert(INFORMATION, result.msg);
                                IupcStore.load();
                                editWin.close();

                            }
                            else {
                                if (result.msg == "1") {
                                    Ext.Msg.alert(INFORMATION, "商品品號不存在，請重新輸入！");
                                }
                                if (result.msg == "2") {
                                    Ext.Msg.alert(INFORMATION, "條碼編號已存在，請重新輸入！");
                                }
                                if (result.msg == "3") {
                                    Ext.Msg.alert(INFORMATION, "一個商品只能有一個國際碼，請重新輸入！");
                                }
                                IupcStore.load();
                                editWin.close();

                            }
                        },
                        failure: function (form, action) {
                            var result = Ext.decode(action.response.responseText);
                            if (result.msg == "1") {
                                Ext.Msg.alert(INFORMATION, "商品品號不存在，請重新輸入！");
                            }
                            if (result.msg == "2") {
                                Ext.Msg.alert(INFORMATION, "條碼編號已存在，請重新輸入！");
                            }
                            if (result.msg == "3") {
                                Ext.Msg.alert(INFORMATION, "一個商品只能有一個國際碼，請重新輸入！");
                            }
                            IupcStore.load();
                            editWin.close();

                        }
                    });
                }
            }
        }]
    });
    var editWin = Ext.create('Ext.window.Window', {
        iconCls: 'icon-user-edit',
        id: 'editWin',
        width: 400,
        height:250,
        y: 100,
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
                     }
                     else {
                         return false;
                     }
                 });
             }
         }
        ],
        listeners: {
            'show': function () {
                //alert("123");
                //alert(row.data.upc_type_flg + "   " + row.data.upc_type_flg_string);
                editFrm.getForm().loadRecord(row);

             

               
            }
        }
    });
    editWin.show();
}
