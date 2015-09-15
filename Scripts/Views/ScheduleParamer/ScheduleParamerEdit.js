

editFunction = function (row, store) {

    var SaveFrm = Ext.create('Ext.form.Panel', {
        id: 'SaveFrm',
        frame: true,
        autoScroll: true,
        defaultType: 'textfield',
        layout: 'anchor',
        labelWidth: 45,
        url: '/ScheduleParamer/ScheduleParamerSave',
        defaults: { anchor: "95%", msgTarget: "side" },
        items: [
            {
                xtype: 'displayfield',
                id: 'para_id',
                name: 'para_id',
                fieldLabel: '參數值',
                labelWidth: 70,
                hidden:true
            },
            {
                xtype: 'textfield',
                id: 'para_value',
                name: 'para_value',
                fieldLabel: '參數值',
                labelWidth: 70,
                allowBlank:false
            },
            {
                xtype: 'textfield',
                id: 'para_name',
                name: 'para_name',
                fieldLabel: '參數名稱',
                labelWidth: 70,
                allowBlank: false
            },
            {
                xtype: 'textfield',
                fieldLabel: '明細代碼',
                id: 'schedule_code',
                labelWidth: 70,
                name: 'schedule_code',
                allowBlank: false
            }
        ],
        buttons: [{
            text: "保存",
            formBind: true,
            vtype: 'submit',
            disabled: true,
            handler: function () {
                var form = this.up('form').getForm();
                if (form.isValid()) {
                    form.submit({
                        params: {
                            //裡面的數據錯誤。會導致無法顯示各種數據
                            para_id: Ext.htmlEncode(Ext.getCmp('para_id').getValue()),
                            para_value: Ext.getCmp('para_value').getValue(),
                            para_name: Ext.htmlEncode(Ext.getCmp('para_name').getValue()),
                            schedule_code: Ext.htmlEncode(Ext.getCmp('schedule_code').getValue())
                        },
                        success: function (form, action) {
                            var result = Ext.decode(action.response.responseText);
                            Ext.Msg.alert(INFORMATION, SUCCESS);
                            if (result.success) {
                                ScheduleParameterStore.load();
                                editWin.close();
                            }
                            else {
                                Ext.Msg.alert(INFORMATION, ERRORSHOW + result.success);
                            }
                        },
                        failure: function () {
                            Ext.Msg.alert(INFORMATION, FAILURE);
                        }
                    });
                }
            }
        }]

    });
    var editWin = Ext.create('Ext.window.Window', {
        title: "新增",
        iconCls: 'icon-user-edit',
        id: 'editWin',
        width: 400,
        layout: 'fit',
        items: [SaveFrm],
        y: 200,
        closeAction: 'destroy',
        modal: true,
        constrain: true,
        resizable: false,
        labelWidth: 60,
        bodyStyle: 'padding:5px 5px 5px 5px',
        closable: false
        ,
        tools: [
         {
             type: 'close',
             qtip: "關閉",
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
                if (row != null) {
                    SaveFrm.getForm().loadRecord(row);
                }
            }
        }
    });
    editWin.show();
}
