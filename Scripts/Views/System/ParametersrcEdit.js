editFunction = function (row, store) {
    var editFrm = Ext.create('Ext.form.Panel', {
        id: 'editFrm',
        frame: true,
        plain: true,
        constrain: true,
        autoScroll: true,
        labelWidth: 45,
        url: '/System/ParametersrcSave',
        defaults: { anchor: "95%", msgTarget: "side" },
        items: [{
            xtype: 'textfield',
            fieldLabel: '參數編號',
            id: 'Rowid',
            name: 'rowid',
            submitValue: true,
            hidden: true
        },
        {
            xtype: 'textfield',
            name: 'parameterType',
            id: 'ParameterType',
            submitValue: true,
            allowBlank: true,
            fieldLabel: '參數類型'
        },
        {
            xtype: 'textfield',
            name: 'parameterProperty',
            id: 'ParameterProperty',
            submitValue: true,
            allowBlank: true,
            fieldLabel: '參數屬性'
        }, {
            xtype: 'fieldcontainer',
            layout: 'hbox',
            items: [
            {
                xtype: 'displayfield',
                fieldLabel: '參數編碼',
                width: 105
            },
            {
                xtype: 'textfield',
                name: 'parameterCode',
                id: 'ParameterCode',
                submitValue: true,
                allowBlank: false,
                // fieldLabel: '參數編碼',
                width: 190
            },
            {
                xtype: "datetimefield",
                id: 'codeTime',
                format: 'Y-m-d H:i',
                editable: false,
                width: 190,
                hidden: true,
                allowBlank: true
            },
            {
                xtype: 'checkbox',
                fieldLabel: '時間',
                id: 'chkcode',
                margin: '0 0 0 2',
                labelWidth: 30,
                handler: function () {
                    if (this.checked == true) {
                        Ext.getCmp('ParameterCode').hide();
                        Ext.getCmp('ParameterCode').allowBlank = true;
                        Ext.getCmp('ParameterCode').reset();
                        Ext.getCmp('codeTime').show();
                        Ext.getCmp('codeTime').allowBlank = false;
                    }
                    if (this.checked == false) {
                        Ext.getCmp('ParameterCode').show();
                        Ext.getCmp('ParameterCode').allowBlank = false;
                        Ext.getCmp('codeTime').hide();
                        Ext.getCmp('codeTime').allowBlank = true;
                        Ext.getCmp('codeTime').reset();
                    }
                }
            }
            ]
        },
        {
            xtype: 'textfield',
            name: 'parameterName',
            id: 'parameterName',
            submitValue: true,
            allowBlank: false,
            fieldLabel: '參數名稱'
        },
        {
            xtype: 'textfield',
            name: 'TopValue',
            id: 'TopValue',
            //allowBlank: false,
            submitValue: true,
            fieldLabel: '父節點'
        },
        {
            xtype: 'fieldcontainer',
            layout: 'hbox',
            items: [
            {
                xtype: 'displayfield',
                fieldLabel: '說明',
                width: 105
            },
            {
                xtype: 'textfield',
                name: 'remark',
                id: 'remark',
                submitValue: true,
                width: 190,
                allowBlank: false,
            },
            {
                xtype: "datetimefield",
                id: 'remarkTime',
                format: 'Y-m-d H:i',
                width: 190,
                editable:false,
                hidden: true,
                allowBlank: true
            },
            {
                xtype: 'checkbox',
                fieldLabel: '時間',
                id: 'chkremark',
                margin: '0 0 0 2',
                labelWidth: 30,
                handler: function () {
                    if (this.checked == true) {
                        Ext.getCmp('remark').hide();
                        Ext.getCmp('remark').allowBlank = true;
                        Ext.getCmp('remark').reset();
                        Ext.getCmp('remarkTime').show();
                        Ext.getCmp('remarkTime').allowBlank = false;
                    }
                    if (this.checked == false) {
                        Ext.getCmp('remark').show();
                        Ext.getCmp('remark').allowBlank = false;
                        Ext.getCmp('remarkTime').hide();
                        Ext.getCmp('remarkTime').allowBlank = true;
                        Ext.getCmp('remarkTime').reset();
                    }
                }
            }
            ]
        },
        {
            xtype: 'numberfield',
            name: 'sort',
            id: 'Sort',
            value: 0,
            minValue: 0,
            submitValue: true,
            fieldLabel: '排序'
        }

        ],
        buttons: [{
            formBind: true,
            disabled: true,
            text: '保存',
            handler: function () {
                var form = this.up('form').getForm();
                var chkcode = Ext.getCmp('chkcode').getValue();
                var chkremark = Ext.getCmp('chkremark').getValue();
                var Code = "";
                var Explain = "";
                if (chkcode == true) {
                    Code = formatDate(Ext.getCmp('codeTime').getValue());
                }
                else {
                    Code = Ext.htmlEncode(Ext.getCmp('ParameterCode').getValue());
                }
                if (chkremark == true) {
                    Explain = formatDate(Ext.getCmp('remarkTime').getValue())
                }
                else {
                    Explain = Ext.htmlEncode(Ext.getCmp('remark').getValue())
                }             
                if (form.isValid()) {
                    form.submit({
                        params: {
                            Code: Code,
                            Explain: Explain
                        },
                        success: function (form, action) {
                            var result = Ext.decode(action.response.responseText);
                            if (result.success) {
                                Ext.Msg.alert(INFORMATION, "保存成功! ");
                                editWin.close();
                                ParametersrcStore.load();

                            }
                            else {
                                Ext.Msg.alert(INFORMATION, "保存失敗! ");
                                editWin.close();
                                ParametersrcStore.load();
                            }
                        },
                        failure: function (form, action) {
                            var result = Ext.decode(action.response.responseText);
                            Ext.Msg.alert(INFORMATION, "保存失敗! ");
                            editWin.close();
                            ParametersrcStore.load();
                        }
                    });
                }
            }
        }
        ]
    });
    var editWin = Ext.create('Ext.window.Window', {
        title: '參數表新增/修改',
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
        }
        ],
        listeners: {
            'show': function () {
                if (row) {
                    editFrm.getForm().loadRecord(row);
                    initForm(row);
                }
            }
        }
    });
    editWin.show();
}
function initForm(row) {
    var remark = row.data.remark;
    var ParameterCode = row.data.ParameterCode;
    var regex = /\d{4}-\d{2}-\d{2}\s+\d{2}:\d{2}/;
    if (regex.test(ParameterCode)) {
        Ext.getCmp('ParameterCode').hide();
        Ext.getCmp('chkcode').checked = true;
        Ext.getCmp('chkcode').setDisabled(true);
        Ext.getCmp('ParameterCode').allowBlank = true;
        Ext.getCmp('codeTime').show();
        Ext.getCmp('codeTime').setRawValue(ParameterCode);
    }
    if (regex.test(remark)) {
        Ext.getCmp('remark').hide();
        Ext.getCmp('chkremark').checked = true;
        Ext.getCmp('chkremark').setDisabled(true);
        Ext.getCmp('remark').allowBlank = true;
        Ext.getCmp('remarkTime').show();
        Ext.getCmp('remarkTime').setRawValue(remark);
    }
}
function formatDate(now) {
    var year = now.getFullYear();
    var month = now.getMonth() + 1;
    if (month < 10) {
        month = "0" + month;
    }
    var date = now.getDate();
    if (date < 10) {
        date = "0" + date;
    }
    var hour = now.getHours();
    if (hour < 10) {
        hour = "0" + hour;
    }
    var minute = now.getMinutes();
    if (minute < 10) {
        minute = "0" + minute;
    }
    return year + "-" + month + "-" + date + " " + hour + ":" + minute;
};