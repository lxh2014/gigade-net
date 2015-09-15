

editFunction = function (row, store) {
    var editFrm = Ext.create('Ext.form.Panel', {
        id: 'editFrm',
        frame: true,
        plain: true,
        layout: 'anchor',
        url: '/MailGroup/SaveMailGroup',
        labelWidth: 45,
        defaults: { anchor: "95%", msgTarget: "side" },
        items: [
            {
                xtype: 'displayfield',
                fieldLabel: "流水號",
                id: 'row_id',
                hidden:true,
            },
        {
            xtype:'textfield',
            fieldLabel: "群組名稱",
            id: 'group_name',
            allowBlank: false,
        }, {
            xtype: 'textfield',
            fieldLabel: "群組編碼",
            id: 'group_code',
            allowBlank: false,
        }, {
            xtype: 'textfield',
            fieldLabel: "備註",
            allowBlank: false,
            id: 'remark'
        }
        ],
        buttons: [{
            text: '保存',
            formBind: true,
            disabled: true,
            handler: function () {
                var form = this.up('form').getForm();
                if (form.isValid()) {
                    form.submit({
                        params: {
                            row_id: Ext.htmlEncode(Ext.getCmp('row_id').getValue()),
                            groupName: Ext.htmlEncode(Ext.getCmp('group_name').getValue()),
                            groupCode: Ext.htmlEncode(Ext.getCmp('group_code').getValue()),
                            remark: Ext.htmlEncode(Ext.getCmp('remark').getValue())
                        },
                        success: function (form, action) {
                            Ext.Msg.alert("提示信息", "保存成功！");
                            editWin.close();
                            store.load();
                          
                        },
                        failure: function (form, action) {
                            if (action.result.msg == 0) {
                                Ext.Msg.alert("提示信息", "保存失敗！");
                                editWin.close();
                            }
                            else if (action.result.msg == -1) {
                                Ext.Msg.alert("提示信息", "群組名稱或群組編碼重複！");
                            }
                        }
                    });
                }
            }

        }]
    });

    var editWin = Ext.create('Ext.window.Window', {
        id: 'editWin',
        title: '保存功能模組',
        iconCls: 'icon-user-add',
        width: 400,
        //  height: document.documentElement.clientHeight * 260 / 783,
        height:195,
        layout: 'fit',
        constrain: true,
        closeAction: 'destroy',
        modal: true,
        //labelWidth: 60,
        closable: false,
        items: [editFrm],
        tools: [
    {
        type: 'close',
        qtip: '是否關閉',
        handler: function (event, toolEl, panel) {
            Ext.MessageBox.confirm("確認信息","是否關閉窗口", function (btn) {
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
            }

        },

    });
    editWin.show();

};