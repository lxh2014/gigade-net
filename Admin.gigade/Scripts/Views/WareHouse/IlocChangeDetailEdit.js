editFunction = function (row, store) {
    var editFrm = Ext.create('Ext.form.Panel', {
        id: 'editFrm',
        frame: true,
        plain: true,
        constrain: true,
        autoScroll: true,
        layout: 'anchor',
        labelWidth: 50,
        url: '/WareHouse/GetIlocChangeDetailEdit',
        defaults: { anchor: "95%", msgTarget: "side" },
        items: [
            {
                xtype: 'textfield',
                fieldLabel: 'icd_id',
                id: 'icd_id',
                name: 'icd_id',
                hidden: true,
                submitValue: true

            },
               {
                   xtype: 'displayfield',
                   fieldLabel: '商品編號',
                   labelWidth: 100,
                   id: 'icd_item_id',
                   submitValue: true
               },
                {
                    xtype: 'displayfield',
                    fieldLabel: '商品名稱',
                    labelWidth: 100,
                    id: 'product_name',
                    submitValue: true
                },
            {
                xtype: 'displayfield',
                fieldLabel: '規格',
                labelWidth: 100,
                id: 'product_sz',
                submitValue: true
            },
             {
                 xtype: 'displayfield',
                 fieldLabel: '原料位編號',
                 labelWidth: 100,
                 id: 'icd_old_loc_id',
                 submitValue: true
             },
              {
                  xtype: 'displayfield',
                  fieldLabel: '新料位編號',
                  labelWidth: 100,
                  id: 'icd_new_loc_id',
                  submitValue: true
              },
               {
                   xtype: 'textfield',
                   fieldLabel: '轉移的原料位',
                   id: 'old_loc_id',
                   name: 'old_loc_id',
                   submitValue: true,
                   allowBlank: false
                  

               },
                {
                    xtype: 'textfield',
                    fieldLabel: '轉移的新料位',
                    id: 'new_loc_id',
                    name: 'new_loc_id',
                    submitValue: true,
                    allowBlank: false

                }
            //,
            //{
            //    xtype: 'textfield',
            //    name: 'loc_id',
            //    id: 'loc_id',
            //    maxLength: 25,
            //    allowBlank: false,
            //    submitValue: true,
            //    fieldLabel: '料位編號'
            //},
            //{
            //    xtype: 'numberfield',
            //    name: 'loc_stor_cse_cap',
            //    id: 'loc_stor_cse_cap',
            //    submitValue: true,
            //    allowBlank: false,
            //    value: 0,
            //    minValue: 0,
            //    fieldLabel: '料位容量'
            //}
        ],
        buttons: [
            {
                formBind: true,
                disabled: true,
                text: SAVE,
                handler: function () {
                    var form = this.up('form').getForm();
                    if (form.isValid()) {
                        var new_loc_id=Ext.htmlEncode(Ext.getCmp('icd_old_loc_id').getValue());
                        var old_loc_id=Ext.htmlEncode(Ext.getCmp('icd_new_loc_id').getValue());
                        var new_locid=Ext.htmlEncode(Ext.getCmp('old_loc_id').getValue());
                        var old_locid=Ext.htmlEncode(Ext.getCmp('new_loc_id').getValue());
                        if (new_loc_id == new_locid && old_loc_id == old_locid) {
                            form.submit({
                                params: {
                                    icd_id_In: Ext.htmlEncode(Ext.getCmp('icd_id').getValue())
                                    //item_id: Ext.htmlEncode(Ext.getCmp('item_id').getValue()),
                                    //loc_id: Ext.htmlEncode(Ext.getCmp('loc_id').getValue()),
                                    //loc_stor_cse_cap: Ext.htmlEncode(Ext.getCmp('loc_stor_cse_cap').getValue())
                                },
                                success: function (form, action) {
                                    var result = Ext.decode(action.response.responseText);  //Ext.decode 把json格式的字符串轉化為json對象
                                    if (result.success) {
                                        //if (result.msg != undefined) {
                                        //    Ext.Msg.alert(INFORMATION, result.msg);
                                        //}
                                        //else {
                                            Ext.Msg.alert(INFORMATION, "操作成功!");
                                        //}
                                        editWin.close();
                                        IlocChangeDetailsStore.load();
                                    }
                                    else {
                                        Ext.Msg.alert(INFORMATION, "操作失敗!");
                                        IlocChangeDetailsStore.load();
                                    }
                                },
                                failure: function (form, action) {
                                   
                                        Ext.Msg.alert(INFORMATION, "操作超時!");
                                 
                                }
                            });
                        }
                        else
                        {
                            Ext.Msg.alert(INFORMATION,"輸入的原料位或新料位與消息不匹配");
                        }
                    }
                }
            }
        ]
    });
    var editWin = Ext.create('Ext.window.Window', {
        title: '搬移理貨',
        iconCls: row ? 'icon-user-edit' : 'icon-user-add',
        id: 'editWin',
        width: 400,
        height: 300,
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
                   // Ext.getCmp('item_id').setDisabled(true);
                    //Ext.getCmp('loc_id').setDisabled(true);

                }
            }
        }
    });
    editWin.show();
}