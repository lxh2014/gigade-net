addFunction = function (rows, VipUserStore) {
   // var rowsid = rows.data.group_id;
    //alert(rows);
    var editFrm = Ext.create('Ext.form.Panel', {
        id: 'editFrm',
        frame: true,
        plain: true,
        constrain: true,
        defaultType: 'textfield',
        autoScroll: true,
        layout: 'anchor',
        labelWidth: 45,
        url: '/Member/SaveVipUser',
        defaults: { anchor: "95%", msgTarget: "side" },
        items: [
               {
                   //xtype: 'textfield',
                   vtype:'email',
                   fieldLabel: '會員郵箱',
                   id: 'user_Mail',
                   name: 'user_Mail',
                   submitValue: true,
                   regex: /^\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*/,
                   listeners: {
                       change: function () {////'blur'
                           var mail = Ext.getCmp("user_Mail");
                           if (mail.isValid()==true)
                           {
                              // Ext.Msg.alert("提示", row.data.group_id)
                            
                
                               var myMask = new Ext.LoadMask(Ext.getBody(), { msg: "請稍等......", removeMask: true });
                               Ext.Ajax.request({
                                   url: "/Member/GetUserName",
                                   params: {
                                       Email: Ext.getCmp("user_Mail").getValue(),
                                       group_id: rows
                                   },
                                   success: function (response) {
                                       myMask.hide();
                                       var result = Ext.decode(response.responseText);
                                       if (result.msg == "99") {
                                           Ext.getCmp("user_name").setValue('<font style="color:red">該用戶已在此群組中!</font>');
                                           Ext.getCmp("userid").setValue(0);
                                       }
                                       else if (result.msg == "98") {
                                           Ext.getCmp("user_name").setValue('<font style="color:red">此用戶不存在!</font>');
                                           Ext.getCmp("userid").setValue(0);
                                       }
                                       else if (result.msg == "100")
                                       {
                                           Ext.getCmp("user_name").setValue(result.user_name);
                                           Ext.getCmp("user_id").setValue(result.user_id);
                                           Ext.getCmp("userid").setValue(result.user_id);
                                       }
                                       else {
                                           Ext.Msg.alert("提示", "等待超時");
                                           Ext.getCmp("userid").setValue(0);
                                       }
                                   },
                                   failure: function (form, action) {
                                       myMask.hide();
                                       Ext.Msg.alert(INFORMATION, "系統出現錯誤!");
                                   }
                               });
                           } else {
                               Ext.getCmp("user_name").setValue('<font style="color:red">沒有此會員信息</font>');
                               Ext.getCmp("userid").setValue(0);
                           }

                       }
                   }
               },
               {
                   xtype: 'displayfield',
                   name: 'user_id',
                   id: 'user_id',
                   submitValue: true,
                   allowBlank: false,
                   maxLength: 25,
                   fieldLabel: '會員編號'
               },
                {
                    xtype: 'displayfield',
                    name: 'userid',
                    id: 'userid',
                    submitValue: true,
                    allowBlank: false,
                    maxLength: 25,
                    fieldLabel: '會員編號',
                    hidden: true
                },
               {
                   xtype: 'displayfield',
                   name: 'user_name',
                   id: 'user_name',
                   submitValue: true,
                   allowBlank: false,
                   maxLength: 25,
                   fieldLabel: '會員名稱'
               }
        ],
        buttons: [{
            formBind: true,
            disabled: true,
            text: '保存',
            handler: function () {
                var form = this.up('form').getForm();
                
                if (form.isValid()) {
                    var userID = Ext.getCmp('userid').getValue();
                    if (userID == "0") {
                        return false;
                    }
                    else {
                        form.submit({
                            params: {
                                group_id: rows,
                                user_id: Ext.htmlEncode(Ext.getCmp('userid').getValue()),
                                usermail: Ext.htmlEncode(Ext.getCmp('user_Mail').getValue())
                            },
                            success: function (form, action) {
                                var result = Ext.decode(action.response.responseText);
                                if (result.success) {
                                    Ext.Msg.alert(INFORMATION, "保存成功! ");
                                    VipUserStore.load();
                                    editWin.close();
                                }
                                else {
                                    Ext.Msg.alert(INFORMATION, "保存失敗! ");
                                    VipUserStore.load();
                                    editWin.close();
                                }
                            },
                            failure: function (form, action) {
                                var result = Ext.decode(action.response.responseText);
                                Ext.Msg.alert(INFORMATION, "保存失敗! " + result.msg);
                                VipUserStore.load();
                                editWin.close();
                            }
                        });

                    }
                }
            }
        }]
    });
    var editWin = Ext.create('Ext.window.Window', {
        title: '會員新增',
        iconCls: 'icon-user-edit',
        id: 'editWin',
        width: 400,
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
         }],
        //listeners: {
        //    'show': function () {
        //        if (row == null) {
        //            if (document.getElementById("modify_only").value == 1) {
        //                Ext.getCmp('bonus_rate').show();
        //                Ext.getCmp('bonus_expire_day').show();
        //            }
        //            if (document.getElementById("valet_service").value == 1) {
        //                Ext.getCmp('gift_bonus').setReadOnly(false);
        //            }
        //        }
        //        else {
        //            editFrm.getForm().loadRecord(row);
        //            if (document.getElementById("modify_only").value == 1) {
        //                Ext.getCmp('bonus_rate').show();
        //                Ext.getCmp('bonus_expire_day').show();
        //            }
        //            if (document.getElementById("valet_service").value == 1) {
        //                Ext.getCmp('gift_bonus').setReadOnly(false);
        //            }
        //        }

        //    }
        //}
    });
    editWin.show();
   // initForm(rows);
}
//function initForm(row) {
//    rowsid = row.data.group_id;
//    alert(rowsid);
//    var img = row.data.image_name.toString();
//    var imgUrl = img.substring(img.lastIndexOf("\/") + 1);
//    Ext.getCmp('image_name').setRawValue(imgUrl);
//}
