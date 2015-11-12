editFunction = function (row, DeliverExpectArrivalStore) {
    var ExpectArrivePeriodStore = Ext.create('Ext.data.Store', {
        fields: ['txt', 'value'],
        data: [
            { "txt": '不限制', "value": "0" },
            { "txt": '12:00以前', "value": "1" },
            { "txt": '12:00-17:00', "value": "2" },
            { "txt": '17:00-20:00', "value": "3" }
        ]
    });
    var editFrm = Ext.create('Ext.form.Panel', {
        id: 'editFrm',
        frame: true,
        plain: true,
        constrain: true,
        defaultType: 'textfield',
        autoScroll: true,
        layout: 'anchor',
        labelWidth: 45,
        url: '/SendProduct/SaveDeliverExportArrivalInfo',
        defaults: { anchor: "95%", msgTarget: "side" },
        items: [          
        {
            xtype: 'textfield',
            fieldLabel: '出貨單編號',
            id: 'deliver_id',
            name: 'deliver_id',
            submitValue: true,
            readOnly: true,
            hidden: false
        }, {
            xtype: 'datefield',
            fieldLabel: '期望到貨日',
            format:'Y-m-d',
            id: 'expect_arrive_date',
            name: 'expect_arrive_date',
            submitValue: true,
            editable: true,
            hidden: false,
            listeners: {               
                change: function (ths, newValue, oldValue, eOpts) {
                    if (newValue) {
                        Ext.getCmp('flag').setValue(Ext.getCmp('flag').getValue() + 1);
                    }                    
                }               
            }
        }, {          
            xtype: 'combobox',
            fieldLabel: '期望到貨時段',
            store: ExpectArrivePeriodStore,
            queryMode: 'local',
            editable: false,
            id: 'expect_arrive_period',
            name: 'expect_arrive_period',
            displayField: 'txt',
            valueField: 'value',
            submitValue: true,
            hidden: false,
            value: -1
            ,
            listeners: {                
                change: function (ths, newValue, oldValue, eOpts) {
                    if (newValue){
                        Ext.getCmp('flag').setValue(Ext.getCmp('flag').getValue() + 1);
                    }
                }              
            }
        }, {
            xtype: 'textfield',
            fieldLabel: '備註',
            id: 'dcl_note',
            name: 'dcl_note',
            submitValue: true,
            hidden: false,
            listeners: {
                change: function (ths, newValue, oldValue, eOpts) {
                    Ext.getCmp('flag').setValue(Ext.getCmp('flag').getValue() + 1);
                }             
            }
        }, {
            xtype: 'textfield',
            fieldLabel: '標記',
            id: 'flag',
            name: 'flag',
            submitValue: false,           
            hidden: true,
            value:1
        }],
        buttons: [{
            formBind: true,
            //disabled: true,
            id: 'saveBtn',
            text: '保存',
            handler: function () {              
                if (Ext.getCmp('flag').getValue() == 111) {
                    Ext.Msg.alert('提示', '請修改參數后再保存');
                    return false;
                }                
                var form = this.up('form').getForm();
                form.submit({
                    params: {
                        deliver_id: Ext.htmlEncode(Ext.getCmp('deliver_id').getValue().trim()),
                        expect_arrive_date: Ext.getCmp('expect_arrive_date').getValue(),
                        expect_arrive_period: Ext.getCmp('expect_arrive_period').getValue(),
                        dcl_note: Ext.htmlEncode(Ext.getCmp('dcl_note').getValue().trim())
                    },
                    success: function (form, action) {
                        var result = Ext.decode(action.response.responseText);
                        if (result.success) {
                            Ext.Msg.alert("提示信息", result.msg);
                            DeliverExpectArrivalStore.load();
                            editWin.close();
                        }
                        else {
                            Ext.Msg.alert("提示信息", result.msg);
                            DeliverExpectArrivalStore.load();
                            editWin.close();
                        }
                    },
                    failure: function (form, action) {
                        var result = Ext.decode(action.response.responseText);
                        Ext.Msg.alert("提示信息", result.msg);
                        DeliverExpectArrivalStore.load();
                        editWin.close();
                    }
                });
            }
        }]
    });


    var editWin = Ext.create('Ext.window.Window', {
        title: '期望到貨日編輯窗口',
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
                 Ext.MessageBox.confirm("確認信息", "要關閉此對話框嗎", function (btn) {
                     if (btn == "yes") {
                         Ext.getCmp('editWin').destroy();
                     }
                     else {
                         return false;
                     }
                 });
             }
         }],
        listeners: {
            'show': function () {
                if (row == null) {
                    
                }
                else {
                    editFrm.getForm().loadRecord(row);
                                              
                }
            }
        }
    });
    editWin.show();
}