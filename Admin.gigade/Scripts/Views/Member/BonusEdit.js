
editFunction = function (row, store) {
    var bonus_type;
    if (row.data.bonus_type == 1) {
        bonus_type = "購物金";
    }
    else {
        bonus_type = "抵用劵";
    }
    var editFrm = Ext.create('Ext.form.Panel', {
        id: 'editFrm',
        frame: true,
        plain: true,
        defaultType: 'textfield',
        layout: 'anchor',
        labelWidth: 45,
        url: '/Member/updateuser_master',
        defaults: { anchor: "100%", msgTarget: "side" },
        items: [
            {
                xtype: 'displayfield',
                fieldLabel: '會員編號',
                id: 'user_id',
                name: 'user_id'
            },
            {
                xtype: 'displayfield',
                fieldLabel: "會員姓名",
                id: 'user_name',
                name: 'user_name',
                allowBlank: false
            },
            {
                xtype: 'displayfield',
                fieldLabel: bonus_type+"編號",
                id: 'master_id',
                name: 'master_id',
                allowBlank: false
            },
            {
                xtype: 'displayfield',
                fieldLabel: "已使用" + bonus_type,
                id: 'already_use_bonus',
                name: 'already_use_bonus'
            },
            {
                xtype: 'textfield',
                fieldLabel: bonus_type + "總金額",
                id: 'master_total',
                name: 'master_total',
                allowBlank: true
            },
            {
                xtype: "datetimefield",
                fieldLabel: "開始日",
                id: 'smaster_start',
                name: 'smaster_start',
                allowBlank: false,
                format: 'Y-m-d H:i:s'
            },
            {
                xtype: 'datetimefield',
                fieldLabel: "結束日",
                id: 'smaster_end',
                name: 'smaster_end',
                allowBlank: false,
                format: 'Y-m-d H:i:s'
            },
            {
                xtype: 'textfield',
                fieldLabel: "備註",
                id: 'master_note',
                name: 'master_note',
                allowBlank: true
            }
        ],
        buttons: [
            {
                text: SAVE,
                formBind: true,
                handler: function () {
                    var form = this.up('form').getForm();
                    //                if (Ext.getCmp('status').getValue() == "已用完" || Ext.getCmp('status').getValue() == "尚餘點數" || Ext.getCmp('status').getValue() == "已過期") {
                    if (Ext.getCmp('smaster_start').getValue() > Ext.getCmp('smaster_end').getValue()) {
                        Ext.Msg.alert(INFORMATION, "開始日期不可以大於結束日期");
                        return;
                    }
                    if (parseInt(Ext.getCmp('master_total').getValue()) <= 0) {
                        Ext.Msg.alert(INFORMATION, bonus_type+"金額錯誤");
                        return;
                    }
                    if (parseInt(Ext.getCmp('master_total').getValue()) > 9999) {
                        Ext.Msg.alert(INFORMATION, bonus_type + "金額不得大於五位數");
                        return;
                    }
                    if (parseInt(Ext.getCmp('master_total').getValue()) < parseInt(Ext.getCmp('already_use_bonus').getValue())) {
                        Ext.Msg.alert(INFORMATION, "總額必須大於已使用" + bonus_type + "金額");
                        return;
                    }

                    if (form.isValid()) {
                        Ext.MessageBox.confirm(CONFIRM, "確定要修改" + bonus_type + "嗎?", function (btn) {
                            if (btn == "yes") {
                                form.submit({
                                    params: {
                                        master_id: Ext.htmlEncode(Ext.getCmp('master_id').getValue()),
                                        user_id: Ext.htmlEncode(Ext.getCmp('user_id').getValue()),
                                        master_note: Ext.htmlEncode(Ext.getCmp('master_note').getValue()),
                                        master_total: Ext.htmlEncode(Ext.getCmp('master_total').getValue()),
                                        already_use_bonus: Ext.htmlEncode(Ext.getCmp('already_use_bonus').getValue())
                                        //                                 ,
                                        //                                 timestart: Ext.htmlEncode(Ext.getCmp('smaster_start').getValue()),
                                        //                                 smaster_end: Ext.htmlEncode(Ext.getCmp('smaster_end').getValue())
                                    },
                                    success: function (form, action) {
                                        var result = Ext.decode(action.response.responseText);
                                        if (result.success) {
                                            Ext.Msg.alert(INFORMATION, "保存成功！");
                                            var index = (Ext.getCmp('PagingToolbar').el.dom.getElementsByTagName("input")[0].value - 1) * pageSize;
                                            BonusStore.load({ params: { start: index, limit: 25 } });
                                            editWin.close();
                                        } else {
                                            Ext.Msg.alert(INFORMATION, "保存失敗！");
                                        }
                                    },
                                    failure: function () {
                                        Ext.Msg.alert(INFORMATION, FAILURE);
                                    }
                                });
                            } else {
                                return false;
                            }
                        });
                    }
                }
            }
        ]
    });

    var editWin = Ext.create('Ext.window.Window', {
        title: row.data.bonus_type == "1" ? "修改購物金" : "修改抵用劵",
        id: 'editWin',
        iconCls: 'icon-user-edit',
        width: 400,
        //        height: document.documentElement.clientHeight * 260 / 783,
        layout: 'fit',
        items: [editFrm],
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
            }
        ],
        listeners: {
            'show': function () {
                if (row == null) {
                    editFrm.getForm().reset(); //如果是添加的話
                } else {
                    editFrm.getForm().loadRecord(row); //如果是編輯的話
                    var already_use_bonus = row.data.master_total - row.data.master_balance;
                    Ext.getCmp('already_use_bonus').setValue(already_use_bonus);
                    //                    if (row.data.now_time < row.data.master_start) {
                    //                        Ext.getCmp('status').setValue("尚未開通");
                    //                    }
                    //                    else if (row.data.now_time > row.data.master_end) {
                    //                        Ext.getCmp('status').setValue("已過期");
                    //                    }
                    //                    else if (row.data.master_total <= row.data.master_balance) {
                    //                        Ext.getCmp('status').setValue("未使用");
                    //                    }
                    //                    else if (row.data.master_balance > 0) {
                    //                        Ext.getCmp('status').setValue("尚餘點數");
                    //                    }
                    //                    else {
                    //                        Ext.getCmp('status').setValue("已用完");
                    //                    }

                }
            }
        }
    });
    editWin.show();
}