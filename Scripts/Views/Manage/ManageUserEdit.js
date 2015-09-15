editFunction = function (rowID, store) {
    var row = null;
    if (rowID != null) {
        edit_ManageUserStore.load({
            params: { relation_id: rowID },
            callback: function () {
                row = edit_ManageUserStore.getAt(0);
                editUserWin.show();
            }
        });
    }
    else {
        editUserWin.show();
    }
    var editUserFrm = Ext.create('Ext.form.Panel', {
        id: 'editUserFrm',
        frame: true,
        plain: true,
        layout: 'anchor',
        labelWidth: 40,
        url: '/Manage/SaveManageUser',
        defaults: { anchor: "95%", msgTarget: "side" },
        items: [
            {
                xtype: "displayfield",
                fieldLabel: "賬號編號",
                id: 'user_id',
                name: 'user_id',
                hidden:true,
                allowBlank: false,
                labelWidth: 100
            },
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
                id: 'erp_id',
                name: 'erp_id',
                labelWidth: 100,
                regex: /^(\w|[\u4e00-\u9fa5])*$/
            },
             {
                 xtype: 'radiogroup',
                 hidden: false,
                 id: 'user_status',
                 name: 'user_status',
                 fieldLabel: "賬號狀態",
                 colName: 'user_status',
                 width: 600,
                 defaults: {
                     name: 'status',
                     margin: '0 8 0 0'
                 },
                 columns: 3,
                 vertical: true,
                 items: [
                 { boxLabel: "未啟用", id: 'qs1', inputValue: '0', checked: true },
                 { boxLabel: "啟用", id: 'qs2', inputValue: '1' },
                 { boxLabel: "停用", id: 'qs3', inputValue: '2' },
                 { boxLabel: "刪除(小心!刪除後,賬號即無法再做任何變更!)", id: 'qs4', inputValue: '3' }
                 ]
             },
              {
                  xtype: 'radiogroup',
                  hidden: false,
                  id: 'manage',
                  name: 'manage',
                  fieldLabel: "管理狀態",
                  colName: 'manage',
                  width: 600,
                  defaults: {
                      name: 'manage',
                      margin: '0 8 0 0'
                  },
                  columns: 3,
                  vertical: true,
                  items: [
                  { boxLabel: "是", id: 'sta1', inputValue: '1', checked: true },
                  { boxLabel: "否", id: 'sta2', inputValue: '0' }
                  ]
              },
             {
                 xtype: "displayfield",
                 fieldLabel: "錯誤次數",
                 id: 'user_login_attempts',
                 name: 'user_login_attempts',
                 labelWidth: 100
             },
              {
                  xtype: "displayfield",
                  fieldLabel: "建立時間",
                  id: 'creattime',
                  name: 'creattime',
                  labelWidth: 100
              },
               {
                   xtype: "displayfield",
                   fieldLabel: "修改時間",
                   id: 'updtime',
                   name: 'updtime',
                   labelWidth: 100
               },
                {
                    xtype: "displayfield",
                    fieldLabel: "最後登入時間",
                    id: 'lastlogin',
                    name: 'lastlogin',
                    labelWidth: 100
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
                            user_id: Ext.getCmp("user_id").getValue(),
                            user_username: Ext.getCmp('user_username').getValue(),
                            user_email: Ext.getCmp('user_email').getValue(),
                            erp_id: Ext.getCmp('erp_id').getValue(),
                            user_status: Ext.getCmp('user_status').getValue().status,
                            manage: Ext.getCmp('manage').getValue().manage
                        },
                        success: function (form, action) {
                            myMask.hide();
                            var result = Ext.decode(action.response.responseText);
                            if (result.success == true) {
                                if (result.msg == 1) {
                                    Ext.Msg.alert(INFORMATION, "修改成功!");
                                    editUserWin.close();
                                    ManageUserStore.load();
                                } else if (result.msg == 2) {
                                    Ext.Msg.alert(INFORMATION, "郵箱已存在!");
                                    Ext.getCmp('user_email').setValue("");
                                } else if (result.msg == 4) {
                                    Ext.Msg.alert(INFORMATION, "修改失敗!");
                                }
                                else {
                                    Ext.Msg.alert(INFORMATION, result.msg);
                                }
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
            }
        }]
    });


    var editUserWin = Ext.create('Ext.window.Window', {
        id: 'editUserWin',
        width: 555,
        title: "賬號編輯",
        iconCls: 'icon-user-edit',
        iconCls: row ? "icon-user-edit" : "icon-user-add",
        layout: 'fit',
        constrain: true,
        closeAction: 'destroy',
        modal: true,
        closable: false,
        items: [
                 editUserFrm
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
                         Ext.getCmp('editUserWin').destroy();
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
                    editUserFrm.getForm().loadRecord(row); //如果是編輯的話
                    switch (row.data.user_status)
                    {
                        case 0:
                            Ext.getCmp("qs1").setValue(true);
                            break;
                        case 1:
                            Ext.getCmp("qs2").setValue(true);
                            break;
                        case 2:
                            Ext.getCmp("qs3").setValue(true);
                            break;
                        case 3:
                            Ext.getCmp("qs4").setValue(true);
                            break;
                        default:
                            Ext.getCmp("qs1").setValue(true);
                            break;
                    }
                    switch (row.data.manage) {
                        case 0:
                            Ext.getCmp("sta2").setValue(true);
                            break;
                        case 1:
                            Ext.getCmp("sta1").setValue(true);
                            break;
                        default:
                            Ext.getCmp("sta2").setValue(true);
                            break;
                    }
                    if (row.data.user_createdate <= 0) {
                        Ext.getCmp("creattime").setValue("");
                    }
                    else {
                        Ext.getCmp("creattime").setValue(row.data.creattime);
                    }
                    if (row.data.user_updatedate <= 0) {
                        Ext.getCmp("updtime").setValue("");
                    }
                    else {
                        Ext.getCmp("updtime").setValue(row.data.updtime);
                    }
                    if (row.data.user_last_login<=0) {
                        Ext.getCmp("lastlogin").setValue("");
                    }
                    else {
                        Ext.getCmp("lastlogin").setValue(row.data.lastlogin);
                    }
                }
            }
        }
    });
   // editUserWin.show();
}
