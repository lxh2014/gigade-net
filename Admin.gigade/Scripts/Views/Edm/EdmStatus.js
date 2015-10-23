statusFunction = function (row, store) {
    var editFrm = Ext.create('Ext.form.Panel', {
        id: 'editFrm',
        frame: true,
        plain: true,
        constrain: true,
        autoScroll: true,
        layout: 'anchor',
        labelWidth: 45,
        url: '/Edm/EditStatus',   
        defaults: { anchor: "95%", msgTarget: "side" },
        items: [
        {
            xtype: 'displayfield',
            fieldLabel: '電子報編號',
            id: 'content_id',
            name: 'content_id'
        },
        {
            xtype: 'displayfield',
            fieldLabel: '郵件主旨',
            id: 'content_title',
            name: 'content_title'
        },
        {
            xtype: 'displayfield',
            fieldLabel: '郵件重要度',
            id: 'content_priority',
            name: 'content_priority',
        },
        {
            xtype: 'displayfield',
            fieldLabel: '寄件者',
            id: 'content_from_name',
            name: 'content_from_name'
        },
        {
            xtype: 'displayfield',
            fieldLabel: '回覆信箱',
            id: 'content_reply_email',
            name: 'content_reply_email'
        },
        {
            xtype: 'displayfield',
            fieldLabel: '發送時間',
            id: 's_content_start',
            name: 's_content_start'
        },
        {
            xtype: 'radiogroup',
            fieldLabel: '審核',
            id: 'content_status',
            name: 'content_status',
            columns: 1,
            vertical: true,
            items: [
            { boxLabel: '寄送測試信件', id: 'rdo1', name: 'status', inputValue: '1' },
            { boxLabel: '電子報測試正常／待發送正式電子報', id: 'rdo2', name: 'status', inputValue: '3' }
            ]
        }
        ],
        buttons: [
        {
            formBind: true,
            disabled: true,
            text: '保存',
            handler: function () {            
                var form = this.up('form').getForm();
                this.disable();
                if (form.isValid()) {
                    this.disable();
                    form.submit({
                        params: {
                            content_id: Ext.htmlEncode(Ext.getCmp("content_id").getValue()),
                            status: Ext.htmlEncode(Ext.getCmp('content_status').getValue().status)
                        },
                        success: function (form, action) {
                            var result = Ext.decode(action.response.responseText);
                            if (result.success) {                              
                                Ext.Msg.alert(INFORMATION, "保存成功! ");
                                store.load();
                                editWin.close();
                            }
                            else {
                                Ext.Msg.alert(INFORMATION, "保存失敗! ");
                                store.load();
                                editWin.close();
                            }
                        }

                    });
                }
            }
        }

        ]
    })

    var editWin = Ext.create('Ext.window.Window', {
        title: '電子報狀態',
        iconCls: 'icon-user-edit',
        id: 'editWin',
        width: 400,
        height: 400,
        layout: 'fit',
        items: [editFrm],
        constrain: true,
        closeAction: 'destroy',
        modal: true,
        //resizable: false,
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
        }
        ],
        listeners: {
            'show': function () {
                editFrm.getForm().loadRecord(row);
                initRow(row);
            }

        }
    });
    editWin.show();

    function initRow(row) {
        var value = row.data.content_priority;
        if (value == 1) {
            Ext.getCmp("content_priority").setValue("一般");
        }
        else if (value == 2) {
            Ext.getCmp("content_priority").setValue("重要");
        }
        else if (value == 3) {
            Ext.getCmp("content_priority").setValue("不重要");
        }
        var status = row.data.content_status.toString();
        if (status == '1') {
            Ext.getCmp('rdo1').setValue(true);
        }
    }
};
