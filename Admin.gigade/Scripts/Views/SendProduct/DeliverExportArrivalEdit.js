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
            editable:false,
            hidden: false,
            listeners: {               
                //change: function (ths, newValue, oldValue, eOpts) {
                //    if (oldValue)
                //    {

                //    }
                //    if (newValue != oldValue) {
                //        alert(newValue != oldValue);
                //        Ext.getCmp('flag').setValue(Ext.getCmp('flag').getValue() + 1);

                //    }                    
                //}               
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
                //change: function (ths, newValue, oldValue, eOpts) {
                //    if (newValue != oldValue) {
                //        alert(newValue != oldValue);
                //        Ext.getCmp('flag').setValue(Ext.getCmp('flag').getValue() + 1);
                //    }
                //}              
            }
        }, {
            xtype: 'textfield',
            fieldLabel: '備註',
            id: 'dcl_note',
            name: 'dcl_note',
            submitValue: true,
            hidden: false,
            listeners: {
                //change: function (ths, newValue, oldValue, eOpts) {
                //    if (newValue != oldValue) {
                //        alert(newValue != oldValue);
                //        Ext.getCmp('flag').setValue(Ext.getCmp('flag').getValue() + 1);
                //    }                  
                //}             
            }
        },      
        {
             xtype: 'fieldcontainer',
             fieldLabel: '已協調營管',             
             id: 'checkbox1',
             layout: 'hbox',
             items: [
                 {
                     xtype: 'checkbox',
                     boxLabel: '是',
                     name: 'yes',
                     inputValue: '1',
                     id: 'yes'
                 },
                 {
                     xtype: 'displayfield',
                     value: "&nbsp&nbsp&nbsp&nbsp&nbsp<font color='red'>* 與營管聯繫協調后再勾選</font>"
                 }
             ]
        },
        {
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
                var bool_1 = (Ext.htmlEncode(Ext.Date.format(new Date(Ext.getCmp('expect_arrive_date').getValue()), 'Y-m-d')) == row.data.expect_arrive_date.toString());
                var bool_1_1 = (Ext.htmlEncode(Ext.getCmp('expect_arrive_date').getValue()) == 'Mon Jan 01 0001 00:00:00 GMT+0800');          
                var bool_2 = (Ext.getCmp('expect_arrive_period').getValue() == row.data.expect_arrive_period);
                var bool_3 = (Ext.getCmp('dcl_note').getValue().trim() == '');

                var bool_4 = (Ext.htmlEncode(Ext.Date.format(new Date(Ext.getCmp('expect_arrive_date').getValue()), 'Y-m-d')) <= row.data.expect_arrive_date.toString());
                var bool_5 = Ext.getCmp('yes').getValue();
                
                var start = Ext.getCmp("expect_arrive_date").getValue();
                //alert((Ext.htmlEncode(Ext.getCmp('expect_arrive_date').getValue())));
                //alert(bool_1);
                //alert(bool_1_1);
                //alert(bool_2);
                //alert(bool_3);
                
                if (bool_1_1) {
                    Ext.Msg.alert('提示', '請選擇“期望到貨日”');
                    return false;
                    //if (bool_1_1 && bool_2 && bool_3) {
                    //    Ext.Msg.alert('提示', '請修改參數后再保存');
                    //    return false;
                    //}
                }
                else {
                    if (bool_1 && bool_2 && bool_3) {
                        Ext.Msg.alert('提示', '請修改參數后再保存');
                        return false;
                    }
                }
                //if (bool_4) {                    
                //    if(!bool_5){
                //        Ext.Msg.alert('提示', '您選擇的期望到貨日<font color="red">小於</font>修改前的日期，<br/>請與營管聯繫進行協調');                       
                //        return false;
                //    }
                //}
                if (bool_4) {
                    Ext.Msg.alert('提示', '您選擇的期望到貨日<font color="red">必須大於</font>修改前的日期！');
                    return false;
                }
                //time1 = Ext.htmlEncode(Ext.Date.format(new Date(Ext.getCmp('expect_arrive_date').getValue()), 'Y-m-d'));
                                                                        
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