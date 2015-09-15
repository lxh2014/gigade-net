editFunction = function (rowID) {
    var row = null;


    var isnewMain = true;
    Ext.define("gigade.manageUser", {
        extend: 'Ext.data.Model',
        fields: [
            { name: "user_email", type: "string" },
            { name: "user_name", type: "string" }]
    });
    //下拉框人員的數據
    var ManageUserStore = Ext.create('Ext.data.Store', {
        autoDestroy: true,
        pageSize: pageSize,
        model: 'gigade.manageUser',
        proxy: {
            type: 'ajax',
            url: '/MailGroup/ManageUser',
            reader: {
                type: 'json',
                root: 'data',
                totalProperty: 'totalCount'
            }
        }
    });
    ManageUserStore.load();
    var boolClass = true;
    var editUserFrm = Ext.create('Ext.form.Panel', {
        id: 'editUserFrm',
        frame: true,
        plain: true,
        //constrain: true,
        //defaultType: 'textfield',
        //  autoScroll: true,
        layout: 'anchor',
        labelWidth: 40,
        url: '/MailGroup/SaveMailUser',
        defaults: { anchor: "95%", msgTarget: "side" },
        items: [
                  {
                      xtype: 'displayfield',
                      fieldLabel: '用戶編號',
                      id: 'row_id',
                      name: 'row_id',
                      hidden: true,
                      submitValue: true
                  },
                  {
                      xtype: 'combobox', //banner_id
                      allowBlank: false,//是否允许为空
                      msgTarget: "side",//錯誤信息所在位置
                      fieldLabel: "用戶名",
                      editable: true,//是否可編輯

                      id: 'user_name',
                      labelWidth: 60,
                      name: 'class_name',
                      queryMode: 'local',
                      //disabled: true,
                      hiddenname: 'user_id',//隐藏字段的名字
                      store: ManageUserStore,
                      displayField: 'user_name',
                      valueField: 'user_email',
                      // forceSelection: false,//值为true时将限定选中的值为列表中的值，值为false则允许用户将任意文本设置到字段
                      typeAhead: true,//设置在输入过程中是否自动选择匹配的剩余部分文本
                      emptyText: "輸入或選擇",
                      listeners: {

                          'blur': function () {
                              //alert(editUserFrm.getForm().getValues('user_id'));
                              // alert(document.getElementsByName('class_name')[0].getRawValue);

                              //if (document.getElementsByName('class_name')[0].value != Ext.getCmp('user_name').getValue()) {
                              //    document.getElementsByName('class_name')[0].value = ManageUserStore.getAt(0).get('user_id');
                              //    Ext.getCmp("user_mail").setValue(Ext.getCmp('user_name').value);
                              //}

                          },
                          'change': function () {
                              if (!isnewMain) {
                                  Ext.getCmp("user_mail").setValue(Ext.getCmp('user_name').value);

                              } else {
                                  if (!row) {
                                      Ext.getCmp("user_mail").setValue(Ext.getCmp('user_name').value);
                                  }
                              }
                              isnewMain = false;
                          }
                      }
                  },
                  {
                      xtype: 'textfield',
                      fieldLabel: '郵箱地址',
                      id: 'user_mail',
                      labelWidth: 60,
                      vtype: 'email',
                      name: 'user_mail',
                      allowBlank: false,//是否允许为空
                      submitValue: false
                  },
                  {
                      xtype: 'textfield',
                      inputType: "password",
                      fieldLabel: '郵箱密碼',
                      id: 'user_pwd',
                      labelWidth: 60,
                      name: 'user_pwd',
                      allowBlank: true,//是否允许为空
                      submitValue: false
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
                            row_id: Ext.getCmp('row_id').getValue(),
                            user_name: Ext.getCmp('user_name').getRawValue(),
                            user_mail: Ext.getCmp('user_mail').getValue(),
                            user_pwd: Ext.getCmp('user_pwd').getValue()
                        },
                        success: function (form, action) {
                            if (action.result.success == true) {
                                if (action.result.msg > 0) {
                                    Ext.Msg.alert("提示信息", "保存成功！");
                                    editUserWin.close();
                                    UserMailStore.load();
                                }
                                if (action.result.msg == -1) {
                                    Ext.Msg.alert("提示信息", "此郵箱已存在！");
                                }
                            }
                        },
                        failure: function (form, action) {
                            Ext.Msg.alert("提示信息", "保存失敗！");
                        }
                    });
                }
            }
        }]
    });
    var editUserWin = Ext.create('Ext.window.Window', {

        id: 'editUserWin',
        width: 300,
        iconCls: 'icon-user-edit',

        layout: 'fit',
        constrain: true,
        closeAction: 'destroy',
        modal: true,
        closable: false,
        items: [editUserFrm],
        //  resizable: false,
        labelWidth: 60,
        bodyStyle: 'padding:5px 5px 5px 5px',
        tools: [
         {
             type: 'close',
             qtip: '是否關閉',
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
                }
            }
        }
    });

    if (rowID != null) {
        edit_UserMailStore.load({
            params: { relation_id: rowID },
            callback: function () {
                row = edit_UserMailStore.getAt(0);
                editUserWin.show();
            }
        });
    }
    else {
        editUserWin.show();
    }
}
