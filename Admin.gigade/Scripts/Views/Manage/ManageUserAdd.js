AddFunction = function (row, store) {

    var AddUserFrm = Ext.create('Ext.form.Panel', {
        id: 'AddUserFrm',
        frame: true,
        plain: true,
        layout: 'anchor',
        labelWidth: 40,
        url: '/Manage/SaveManageUser',
        defaults: { anchor: "95%", msgTarget: "side" },
        items: [
            {
                xtype: "textfield",
                fieldLabel: "賬號名稱",
                id: 'user_username',
                name: 'user_username',
                allowBlank: false,
                labelWidth: 100,
                regex: /^(\w|[\u4e00-\u9fa5])*$/
            },
            {
                xtype: "textfield",
                fieldLabel: "登入郵箱",
                id: 'user_email',
                name: 'user_email',
                allowBlank: false,
                vtype: 'email',
                labelWidth: 100
            },
            {
                xtype: "textfield",
                fieldLabel: "ERP人員代碼",
                id: 'userecode',
                name: 'userecode',
                labelWidth: 100,
                regex: /^(\w|[\u4e00-\u9fa5])*$/
            }
        ],
        buttons: [{
            formBind: true,
            disabled: true,
            text: SAVE,
            handler: function () {
                var form = this.up('form').getForm();
                var myMask = new Ext.LoadMask(Ext.getBody(), { msg: "loading..." });
                myMask.show();
                if (form.isValid()) {
                    form.submit({
                        params: {
                            user_username: Ext.getCmp('user_username').getValue(),
                            user_email: Ext.getCmp('user_email').getValue(),
                            erp_id: Ext.getCmp('userecode').getValue()
                        },
                        success: function (form, action) {
                            myMask.hide();
                            var result = Ext.decode(action.response.responseText);
                            if (result.success == true) {
                                if (result.msg == 1) {
                                    Ext.Msg.alert(INFORMATION, "新增成功!");
                                    AddUserWin.close();
                                    ManageUserStore.load();
                                }
                                else if (result.msg == 2) {
                                    Ext.Msg.alert(INFORMATION, "郵箱已存在!");
                                    Ext.getCmp('user_email').setValue("");
                                }
                                else if (result.msg == 3) {
                                    Ext.Msg.alert(INFORMATION, "新增成功,email發送失敗!");
                                }
                                else if (result.msg == 4) {
                                    Ext.Msg.alert(INFORMATION, "新增失敗!");
                                }
                                else {
                                    Ext.Msg.alert(INFORMATION, result.msg);
                                }
                            }
                           else{
                                Ext.Msg.alert(INFORMATION, result.msg);
                            }
                        },
                        failure: function (form, action) {
                            myMask.hide();
                            Ext.Msg.alert(INFORMATION, PROMODISCOUNTTIP);
                        }
                    });
                }
            }
        }]
    });
    var AddUserWin = Ext.create('Ext.window.Window', {
        id: 'AddUserWin',
        width: 400,
        title: "賬號新增",
        iconCls: 'icon-user-edit',
        iconCls: row ? "icon-user-edit" : "icon-user-add",
        layout: 'fit',
        constrain: true,
        closeAction: 'destroy',
        modal: true,
        closable: false,
        items: [
                 AddUserFrm
        ],
        labelWidth: 60,
        bodyStyle: 'padding:5px 5px 5px 5px',
        tools: [
         {
             type: 'close',
             qtip: CLOSE,
             handler: function (event, toolEl, panel) {
                 Ext.MessageBox.confirm(CONFIRM, IS_CLOSEFORM, function (btn) {
                     if (btn == "yes") {
                         Ext.getCmp('AddUserWin').destroy();
                         ManageUserStore.destroy();
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
                    //AddUserFrm.getForm().reset(); //如果是新增的話
            }
        }
    });
    AddUserWin.show();
}


ChangePwdFunction = function (rowID) {
    var row = null;
    if (rowID != null) {
        edit_ManageUserStore.load({
            params: { relation_id: rowID },
            callback: function () {
                row = edit_ManageUserStore.getAt(0);
                UserPwdChangeWin.show();
            }
        });
    }
    else {
        UserPwdChangeWin.show();
    }
    var UserPwdChangeFrm = Ext.create('Ext.form.Panel', {
        id: 'UserPwdChangeFrm',
        frame: true,
        plain: true,
        layout: 'anchor',
        labelWidth: 40,
        url: '/Manage/Updatepassword',
        defaults: { anchor: "95%", msgTarget: "side" },
        items: [
            {
                xtype: 'displayfield',
                fieldLabel: '用戶編號',
                hidden: true,
                id: 'user_id',
                name: 'user_id',
                labelWidth: 100
        },
            {
                xtype: 'displayfield',
                fieldLabel: '使用者名稱',
                id: 'user_username',
                name: 'user_username',
                labelWidth: 100
            },
            {
                xtype: 'displayfield',
                fieldLabel: "登入郵箱",
                id: 'user_email',
                name: 'user_email',
                vtype: 'email',
                labelWidth: 100
            },
            {
                xtype: "textfield",
                fieldLabel: "變更密碼",
                id: 'user_pwd',
                inputType:'password',
                name: 'user_pwd',
                allowBlank: false,
                labelWidth: 100
            },
            {
                xtype: "textfield",
                fieldLabel: "確認密碼",
                id: 'verify_pwd',
                inputType: 'password',
                name: 'verify_pwd',
                allowBlank: false,
                labelWidth: 100
            }
        ],
        buttons: [{
            formBind: true,
            disabled: true,
            text: SAVE,
            handler: function () {
                var form = this.up('form').getForm();
                var pwd1 = Ext.getCmp('user_pwd').getValue();
                var pwd2 = Ext.getCmp('verify_pwd').getValue();
                if (pwd1 == pwd2) {
                    var myMask = new Ext.LoadMask(Ext.getBody(), { msg: "loading..." });
                    myMask.show();
                    if (form.isValid()) {
                        form.submit({
                            params: {
                                id: Ext.getCmp('user_id').getValue(),
                                //username: Ext.getCmp('user_username').getValue(),
                                //useremail: Ext.getCmp('user_email').getValue(),
                                user_password: Ext.getCmp('user_pwd').getValue()
                            },
                            success: function (form, action) {
                                myMask.hide();
                                var result = Ext.decode(action.response.responseText);
                                if (result.success == true) {
                                    Ext.Msg.alert(INFORMATION, result.msg);
                                    UserPwdChangeWin.close();
                                    ManageUserStore.load();

                                }
                                else {
                                    Ext.Msg.alert(INFORMATION, result.msg);
                                }
                            },
                            failure: function (form, action) {
                                myMask.hide();
                                Ext.Msg.alert(INFORMATION, PROMODISCOUNTTIP);
                            }
                        });
                    }
                } else {
                    Ext.Msg.alert(INFORMATION, "兩次輸入的密碼不一致!");
                    Ext.getCmp('user_pwd').setValue("");
                    Ext.getCmp('verify_pwd').setValue("");
                }
            }
        }]
    });


    var UserPwdChangeWin = Ext.create('Ext.window.Window', {
        id: 'UserPwdChangeWin',
        width: 400,
        title: "修改密碼",
        iconCls: 'icon-user-edit',
        iconCls: row ? "icon-user-edit" : "icon-user-add",
        layout: 'fit',
        constrain: true,
        closeAction: 'destroy',
        modal: true,
        closable: false,
        items: [
                 UserPwdChangeFrm
        ],
        labelWidth: 60,
        bodyStyle: 'padding:5px 5px 5px 5px',
        tools: [
         {
             type: 'close',
             qtip: CLOSE,
             handler: function (event, toolEl, panel) {
                 Ext.MessageBox.confirm(CONFIRM, IS_CLOSEFORM, function (btn) {
                     if (btn == "yes") {
                         Ext.getCmp('UserPwdChangeWin').destroy();
                         ManageUserStore.destroy();
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
                if (row) {
                    UserPwdChangeFrm.getForm().loadRecord(row); //如果是編輯的話

                }
                else {
                    UserPwdChangeFrm.getForm().reset(); //如果是新增的話
                }
            }
        }
    });
  //  UserPwdChangeWin.show();
}
