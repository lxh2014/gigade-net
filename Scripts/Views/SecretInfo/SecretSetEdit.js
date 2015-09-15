editFunction = function (row, store) {
    //下拉框
    Ext.define("gigade.ManageUser", {
        extend: 'Ext.data.Model',
        fields: [
            { name: "user_id", type: "string" },
            { name: "user_name", type: "string" }]
    });
    var muStore = Ext.create('Ext.data.Store', {
        model: 'gigade.ManageUser',
        autoLoad: false,
        proxy: {
            type: 'ajax',
            url: "/SecretInfo/GetManagerUser",
            actionMethods: 'post',
            reader: {
                type: 'json',
                root: 'data'
            }
        }
    });
    var issubmit = false;
    var editSecretSetFrm = Ext.create('Ext.form.Panel', {
        id: 'editSecretSetFrm',
        frame: true,
        plain: true,
        constrain: true,
        defaultType: 'textfield',
        autoScroll: true,
        layout: 'anchor',
        labelWidth: 80,
        url: '/SecretInfo/SaveSecretSet',
        defaults: { anchor: "95%", msgTarget: "side" },
        items: [
            {
                xtype: 'displayfield',
                fieldLabel: '流水號',
                id: 'id',
                name: 'id',
                hidden: true,
                submitValue: true
            },
            {
                xtype: 'combobox',
                id: 'user_id',
                name: 'user_id',
                fieldLabel: '用戶名稱',
                queryMode: 'local',
                store: muStore,
                displayField: 'user_name',
                valueField: 'user_id',
                emptyText: '請選擇',
                triggerAction: 'query',
                queryParam: 'user_name',
                typeAhead: true,
                allowBlank: false,
                forceSelection: false,
                hiddenName: 'user_id'
            },
             {
                 xtype: 'fieldcontainer',
                 combineErrors: true,
                 items: [
                {

                    xtype: 'radiogroup',
                    fieldLabel: "修改密碼",
                    labelWidth: 100,
                    id: 'is_update_pwd',
                    hidden: row == null ? true : false,
                    width: 250,
                    colName: 'is_update_pwd',
                    name: 'is_update_pwd',
                    defaults: {
                        name: 'is_update_pwd'
                    },
                    columns: 2,
                    vertical: true,
                    items: [
                               {
                                   id: 'bt1',
                                   boxLabel: YES,
                                   inputValue: "1",
                                   width: 100
                                   ,
                                   listeners: {
                                       change: function (radio, newValue, oldValue) {
                                           var osecret_password = Ext.getCmp("osecret_password");
                                           var nsecret_password = Ext.getCmp("nsecret_password");
                                           if (newValue) {
                                               osecret_password.allowBlank = false;
                                               osecret_password.setDisabled(false);
                                               nsecret_password.allowBlank = false;
                                               nsecret_password.setDisabled(false);
                                           }
                                       }
                                   }
                               },
                               {
                                   id: 'bt2',
                                   boxLabel: NO,
                                   checked: true,
                                   inputValue: "0"
                                   ,
                                   listeners: {
                                       change: function (radio, newValue, oldValue) {
                                           var osecret_password = Ext.getCmp("osecret_password");
                                           var nsecret_password = Ext.getCmp("nsecret_password");
                                           if (newValue) {
                                               osecret_password.setValue("0");
                                               osecret_password.allowBlank = true;
                                               osecret_password.setValue("");
                                               osecret_password.isValid();
                                               osecret_password.setDisabled(true);

                                               nsecret_password.setValue("0");
                                               nsecret_password.allowBlank = true;
                                               nsecret_password.setValue("");
                                               nsecret_password.isValid();
                                               nsecret_password.setDisabled(true);

                                           }
                                       }
                                   }
                               }
                    ]
                }
                 ]
             },
             {
                 xtype: 'textfield',
                 fieldLabel: '原始密碼',
                 id: 'osecret_password',
                 name: 'osecret_password',
                 inputType: "password",
                 disabled: true,
                 hidden: row == null ? true : false,
                 allowBlank: true,
                 submitValue: true
             },
             {
                 xtype: 'textfield',
                 fieldLabel: '新密碼',
                 id: 'nsecret_password',
                 name: 'nsecret_password',
                 inputType: "password",
                 minLength: 8,
                 disabled: row == null ? false : true,
                 allowBlank: row == null ? false : true,
                 submitValue: true
             },
             {
                 xtype: 'textfield',
                 fieldLabel: '綁定IP',
                 id: 'ipfrom',
                 name: 'ipfrom',
                 allowBlank: false,
                 submitValue: true,
                 regex: /^((?:(?:25[0-5]|2[0-4]\d|((1\d{2})|([1-9]?\d)))\.){3}(?:25[0-5]|2[0-4]\d|((1\d{2})|([1-9]?\d))))$/,
                 regexText: '请输入正确的IP'
             },
              {
                  xtype: "numberfield",
                  id: 'secret_limit',
                  name: 'secret_limit',
                  fieldLabel: '5分鐘內次數限制',
                  value: 1,
                  minValue: 1,
                  allowBlank: false,
                  submitValue: true
              },
             {
                 xtype: 'displayfield',
                 fieldLabel: '建立時間',
                 id: 'createdate',
                 name: 'createdate',
                 hidden: true
             },
             {
                 xtype: 'displayfield',
                 fieldLabel: '修改時間',
                 id: 'updatedate',
                 name: 'updatedate',
                 hidden: true
             }],
        buttons: [{
            formBind: true,
            disabled: true,
            text: SAVE,
            type: 'submit',
            handler: function () {
                this.disable();
                var form = this.up('form').getForm();
                var uid = Ext.getCmp('user_id').getValue();
                var ipfrom = Ext.getCmp('ipfrom').getValue();
                if (Ext.getCmp('nsecret_password').getValue() != "") {
                    Ext.Ajax.request({
                        url: "/SecretInfo/GetManagerUser?ispage=0",
                        method: 'POST',
                        async: false,
                        params: {
                            user_id: Ext.getCmp('user_id').getValue(),
                            secret_password: Ext.getCmp('nsecret_password').getValue()
                        },
                        success: function (form, action) {
                            var result = Ext.decode(form.responseText);
                            if (!result.success) {
                                issubmit = false;
                                Ext.Msg.alert(INFORMATION, "密碼不能與登入密碼一樣!");
                                Ext.getCmp('nsecret_password').setValue("");
                            }
                            else {
                                issubmit = true;
                            }
                        }
                    });
                }
                Ext.Ajax.request({
                    url: "/SecretInfo/GetSecretSetList",
                    method: 'post',
                    async: false,
                    params: {
                        id: row == null ? 0 : Ext.getCmp('id').getValue(),
                        search_content: Ext.getCmp('user_id').getValue(),
                        ipfrom: Ext.getCmp('ipfrom').getValue(),
                        ispage: "false"
                    },
                    success: function (form, action) {
                        var result = Ext.decode(form.responseText);
                        if (result.success) {
                            if (result.data.length > 0) {
                                issubmit = false;
                                Ext.Msg.alert(INFORMATION, "相同的用戶和IP不能重複添加!");
                                Ext.getCmp('ipfrom').setValue("");

                            }
                            else {
                                issubmit = true;
                            }
                        }

                    }
                });
                if (issubmit) {
                    if (form.isValid()) {
                        if (Ext.htmlEncode(Ext.getCmp('osecret_password').getValue()) == Ext.htmlEncode(Ext.getCmp('nsecret_password').getValue()) && Ext.htmlEncode(Ext.getCmp('osecret_password').getValue()) != "") {
                            Ext.Msg.alert(INFORMATION, "資安新密碼不可以等於原始密碼！");
                            Ext.getCmp('nsecret_password').setValue("");
                            return;
                        }
                        form.submit({
                            params: {
                                id: Ext.htmlEncode(Ext.getCmp('id').getValue()),
                                user_id: Ext.htmlEncode(Ext.getCmp('user_id').getValue()),
                                ipfrom: Ext.htmlEncode(Ext.getCmp('ipfrom').getValue()),
                                secret_limit: Ext.getCmp('secret_limit').getValue(),
                                osecret_password: Ext.htmlEncode(Ext.getCmp('osecret_password').getValue()),
                                nsecret_password: Ext.htmlEncode(Ext.getCmp('nsecret_password').getValue())
                            },
                            success: function (form, action) {
                                var result = Ext.JSON.decode(action.response.responseText);
                                if (result.success) {
                                    Ext.Msg.alert(INFORMATION, result.msg);
                                    store.load();
                                    Ext.getCmp('editSecretSetWin').destroy();

                                }
                                else {
                                    Ext.Msg.alert(INFORMATION, result.msg);
                                }
                            },
                            failure: function (form, action) {
                                var result = Ext.JSON.decode(action.response.responseText);
                                Ext.Msg.alert(INFORMATION, result.msg);
                            }
                        });
                    }
                }
            }
        }]
    });
    var editSecretSetWin = Ext.create('Ext.window.Window', {
        iconCls: 'icon-user-edit',
        id: 'editSecretSetWin',
        width: 400,
        layout: 'anchor',
        items: [editSecretSetFrm],
        constrain: true,
        closeAction: 'destroy',
        modal: true,
        resizable: false,
        title: row == null ? '密碼新增' : '密碼編輯',
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
                         Ext.getCmp('editSecretSetWin').destroy();
                     }
                     else {
                         return false;
                     }
                 });
             }
         }
        ],
        listeners: {
            resize: function () {
                h = Ext.getCmp('editSecretSetFrm').getHeight() + 50;
                Ext.getCmp('editSecretSetWin').setHeight(h);
                this.doLayout();
            },
            'show': function () {
                if (row != null) {
                    editSecretSetFrm.getForm().loadRecord(row);
                    //alert(muStore.getCount());
                    //  Ext.getCmp("user_id").setValue(muStore.getAt(muStore.find("user_id", row.data.user_id)).data.user_name);

                }

            }
        }
    });

    muStore.load({
        callback: function () {
            editSecretSetWin.show();
        }
    })

}





