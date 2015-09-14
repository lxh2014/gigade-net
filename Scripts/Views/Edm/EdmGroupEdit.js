

editFunction = function (row, store) {
    var editFrm = Ext.create('Ext.form.Panel', {
        id: 'editFrm',
        frame: true,
        plain: true,
        layout: 'anchor',
        url: '/Edm/SaveEdmGroup',
        labelWidth: 45,
        defaults: { anchor: "95%", msgTarget: "side" },
        items: [
            {
                xtype: 'displayfield',
                fieldLabel: "群組編號",
                id: 'group_id',
                hidden: true,
            },
        {
            xtype: 'textfield',
            fieldLabel: "群組名稱",
            id: 'group_name',
            allowBlank: false,
        }, {
            xtype: 'displayfield',
            fieldLabel: "建立日期",
            id: 's_group_createdate',
            hidden: true,
        }, {
            xtype: 'displayfield',
            fieldLabel: "修改日期",
            id: 's_group_updatedate',
            hidden: true,
        }
        ],
        buttons: [{
            text: '保存',
            formBind: true,
            disabled: true,
            handler: function () {
                var myMask = new Ext.LoadMask(Ext.getBody(), { msg: "Please wait..." });
                myMask.show();
                var form = this.up('form').getForm();             
                if (form.isValid()) {
                    this.disable();
                    form.submit({
                        params: {
                            group_id: Ext.htmlEncode(Ext.getCmp('group_id').getValue()),
                            group_name: Ext.htmlEncode(Ext.getCmp('group_name').getValue()),
                        },
                        success: function (form, action) {
                            myMask.hide();
                            if (action.result.success) {
                                myMask.hide();
                                Ext.Msg.alert("提示信息", "保存成功！");
                                editWin.close();
                                store.load();
                            }
                            else {
                                myMask.hide();
                                Ext.Msg.alert("提示信息", "保存失敗！");
                                editWin.close();
                            }
                        },
                        failure: function (form, action) {
                                myMask.hide();
                                Ext.Msg.alert("提示信息", "保存失敗！");
                                editWin.close();
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
        height: 195,
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
            Ext.MessageBox.confirm("確認信息", "是否關閉窗口", function (btn) {
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
                    Ext.getCmp('group_id').show(true);
                    Ext.getCmp('s_group_createdate').show(true);
                    Ext.getCmp('s_group_updatedate').show(true);
                }
            }
        },

    });
    editWin.show();

};