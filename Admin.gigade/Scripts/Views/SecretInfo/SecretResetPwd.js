
ResetPwdFunction = function (row, store) {

    var ResetPwdFrm = Ext.create('Ext.form.Panel', {
        id: 'ResetPwdFrm',
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
                  fieldLabel: "流水號",
                  id: 'id',
                  labelWidth: 65,
                  width: 200,
                  hidden: true,
                  value: row.data.id
              },
               {
                   xtype: 'displayfield',
                   fieldLabel: "用戶編號",
                   id: 'user_id',
                   hidden: true,
                   labelWidth: 65,
                   width: 200,
                   value: row.data.user_id
               },
           {
               xtype: 'displayfield',
               fieldLabel: "用戶名稱",
               id: 'user_name',
               labelWidth: 65,
               width: 200,
               value: row.data.user_username
           }, {
               xtype: 'displayfield',
               fieldLabel: "用戶ip",
               labelWidth: 65,
               id: 'ipfrom',
               width: 200,
               value: row.data.ipfrom
           },
            {
                xtype: 'textfield',
                fieldLabel: '新密碼',
                id: 'nsecret_password',
                name: 'nsecret_password',
                inputType: "password",
                minLength: 8,
                labelWidth: 65,
                allowBlank: false,
                listeners: {
                    blur: function () {
                        var id = Ext.getCmp('id').getValue().toString();
                        var secret_password = Ext.getCmp('nsecret_password').getValue().toString();
                        if (user_id.length != 0 && secret_password.length > 0) {
                            Ext.Ajax.request({
                                url: "/SecretInfo/GetManagerUser",
                                method: 'POST',
                                params: {
                                    id: Ext.getCmp('id').getValue(),
                                    ipfrom: Ext.getCmp('ipfrom').getValue(),
                                    secret_password: secret_password
                                },
                                success: function (form, action) {
                                    var result = Ext.decode(form.responseText);
                                    if (!result.success) {
                                        Ext.Msg.alert(INFORMATION, "密碼不能與登入密碼一樣!");
                                        Ext.getCmp('nsecret_password').setValue("");
                                    }
                                }
                            });
                        }
                    }
                },
                submitValue: true
            }
        ],
        buttons: [{
            formBind: true,
            disabled: true,
            text: SAVE,
            type: 'submit',
            handler: function () {
                this.disable();
                var form = this.up('form').getForm();
                var id = Ext.getCmp('id').getValue();
                Ext.Ajax.request({
                    url: "/SecretInfo/GetManagerUser?ispage=0",
                    method: 'POST',
                    async: false,
                    params: {
                        id: Ext.getCmp('id').getValue(),
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
                if (issubmit) {
                    if (form.isValid()) {
                        form.submit({
                            params: {
                                id: Ext.htmlEncode(Ext.getCmp('id').getValue()),
                                ipfrom: Ext.getCmp('ipfrom').getValue(),
                                reset: 1,
                                nsecret_password: Ext.htmlEncode(Ext.getCmp('nsecret_password').getValue())
                            },
                            success: function (form, action) {
                                var result = Ext.JSON.decode(action.response.responseText);
                                if (result.success) {
                                    Ext.Msg.alert(INFORMATION,SUCCESS);
                                    store.load();
                                    Ext.getCmp('ResetPwdWin').destroy();

                                }
                                else {
                                    Ext.Msg.alert(INFORMATION,FAILURE);
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

    var ResetPwdWin = Ext.create('Ext.window.Window', {
        iconCls: 'icon-user-edit',
        id: 'ResetPwdWin',
        width: 400,
        layout: 'anchor',
        items: [ResetPwdFrm],
        constrain: true,
        closeAction: 'destroy',
        modal: true,
        resizable: false,
        title: '密碼編輯',
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
                         Ext.getCmp('ResetPwdWin').destroy();
                     }
                     else {
                         return false;
                     }
                 });
             }
         }
        ]
    });
    ResetPwdWin.show();
}