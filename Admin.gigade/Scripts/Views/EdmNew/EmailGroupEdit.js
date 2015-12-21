editFunction = function (row, store) {


    var editFrm = Ext.create('Ext.form.Panel', {
        id: 'editFrm',
        frame: true,
        plain: true,
        constrain: true,
        //autoScroll: true,
        layout: 'anchor',
        labelWidth: 45,
        resizable: false,
        url: '/EdmNew/SaveEmailGroup',
        defaults: { anchor: "95%", msgTarget: "side" },
        items: [
              {
                  xtype: 'displayfield',
                  fieldLabel: '編號',
                  id: 'group_id',
                  name: 'group_id',
                  hidden: true
              },
      
   
            {
                xtype: 'textfield',
                fieldLabel: '名稱',
                id: 'group_name',
                name: 'group_name',
                allowBlank: false
            },
        ],
        buttons: [
            {
                formBind: true,
                disabled: true,
                text: '保存',
                handler: function () {
                        var form = this.up('form').getForm();
                        var myMask = new Ext.LoadMask(Ext.getBody(), { msg: "Please wait..." });
                        myMask.show();
                        if (form.isValid()) {
                            this.disable();
                            form.submit({
                                params: {
                                    group_id: Ext.htmlEncode(Ext.getCmp('group_id').getValue()),
                                    group_name: Ext.htmlEncode(Ext.getCmp('group_name').getValue()),
                                },
                                success: function (form, action) {
                                    myMask.hide();
                                    var result = Ext.decode(action.response.responseText);
                                    if (result.success) {
                                        myMask.hide();
                                        Ext.Msg.alert("提示信息", "保存成功! ");
                                        store.load();
                                        editWin.close();
                                    }
                                    else {
                                        myMask.hide();
                                        Ext.Msg.alert("提示信息", "保存失敗! ");
                                        store.load();
                                        editWin.close();
                                    }
                                },
                                failure: function () {
                                    myMask.hide();
                                    Ext.Msg.alert("提示信息", "出現異常! ");
                                }
                            });
                        }

                }
            }
        ]
    });

    var editWin = Ext.create('Ext.window.Window', {
        title: '信箱名單管理新增/編輯',
        iconCls: 'icon-user-edit',
        id: 'editWin',
        height:145,
        width: 421,
        y: 100,
        layout: 'fit',
        items: [editFrm],
        constrain: true,
        closeAction: 'destroy',
        modal: true,
        //  resizable: false,
        labelWidth: 60,
        bodyStyle: 'padding:5px 5px 5px 5px',
        closable: false,
        tools: [
         {
             type: 'close',
             qtip: '是否關閉',
             handler: function (event, toolEl, panel) {
                 Ext.MessageBox.confirm("確認", "是否確定關閉窗口?", function (btn) {
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
                if (row) {
                  
                    editFrm.getForm().loadRecord(row);
                 
                }
                else {
                  
                    editFrm.getForm().reset();
                }
            }
        }
    });
    editWin.show();
    
}