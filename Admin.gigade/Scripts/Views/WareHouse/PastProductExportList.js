Ext.require([
    'Ext.form.*'
]);

Ext.onReady(function () {
    var DateTypeStore = Ext.create('Ext.data.Store', {
        fields: ['txt', 'value'],
        data: [
            { "txt": "即期", "value": "1" },
            { "txt": "過期", "value": "2" }
        ]
    });
    var formPanel = Ext.create('Ext.form.Panel', {
        frame: false,
        width: 980,
        bodyPadding: 30,
        border: false,
        items: [
            {
                html: '<div class="capion">提示：匯出即期品/過期品報表</div>',
                frame: false,
                border: false

            },
            {
                xtype: 'combobox',
                name: 'time_type',
                id: 'time_type',
                editable: false,
                fieldLabel: "時間類型",
                labelWidth: 63,
                store: DateTypeStore,
                queryMode: 'local',
                submitValue: true,
                displayField: 'txt',
                valueField: 'value',
                typeAhead: true,
                forceSelection: false,
                emptyText: '請選擇時間類型',
                value: 1
            },
            {
                xtype: 'fieldcontainer',
                fieldLabel: "走道範圍",
                width: 350,
                combineErrors: true,
                layout: 'hbox',
                items: [
                {
                    xtype: "textfield",
                    id: 'startIloc',
                    name: 'startIloc',
                    allowBlank: false,
                    flex: 5
                },
                {
                    xtype: 'displayfield',
                    value: "--",
                    flex: 1
                },
                {
                    xtype: "textfield",
                    id: 'endIloc',
                    name: 'endIloc',
                    flex: 5,
                    allowBlank: false
                }]                     
            },
            {
                xtype: 'fieldcontainer',
                fieldLabel: "保存期限",
                width: 350,
                combineErrors: true,
                layout: 'hbox',
                items: [
                    {
                        xtype: "numberfield",
                        id: 'startDay',
                        name: 'startDay',
                        allowBlank: false,
                        minValue: 0,
                        value:0,
                        flex: 5,
                        listeners: {
                            change:function() {
                                if (Ext.getCmp('startDay').getValue().trim() == "")
                                {
                                    Ext.getCmp('btnQuery').hide();
                                }
                                
                            }
                        }
                    },
                    {
                        xtype: 'displayfield',
                        value: "--",
                        flex: 1
                    },
                    {
                        xtype: "numberfield",
                        id: 'endDay',
                        name: 'endDay',
                        minValue:0,
                        flex: 5,
                        value: 0,
                        allowBlank: false,
                        listeners: {
                            change: function () {
                                if (Ext.getCmp('endDay').getValue().trim() == "") {
                                    Ext.getCmp('btnQuery').hide();
                                }
                            }
                        }
                    }
                ]
            },
            {//預告天數
                xtype: "numberfield",
                fieldLabel: "預告天數",
                width: 350,
                minValue: 0,
                value:15,
                id: 'yugaoDay',
                name: 'yugaoDay',
                allowBlank: false,
                listeners: {
                    change: function () {
                        if (Ext.getCmp('yugaoDay').getValue().trim() == "") {
                            Ext.getCmp('btnQuery').hide();
                        }
                    }
                }
            },
            { xtype: 'button',text: "確定匯出",id: 'btnQuery',buttonAlign: 'center',handler: ExportPastProduct }
        ],
        renderTo: Ext.getBody()
    });

    function ExportPastProduct() {
        var start = Ext.getCmp('startDay').getValue();
        var end = Ext.getCmp('endDay').getValue();
        if (start > end) {
            Ext.Msg.alert(INFORMATION, "請輸入正確的區間!");
            Ext.getCmp('startDay').setValue(0);
            Ext.getCmp('endDay').setValue(0);
        }
        else {
            window.open("/WareHouse/PastProductExportlist?time_type=" + Ext.getCmp('time_type').getValue() + "&startIloc=" + Ext.getCmp('startIloc').getValue() + "&endIloc=" + Ext.getCmp('endIloc').getValue() + "&startDay=" + Ext.getCmp('startDay').getValue() + "&endDay=" + Ext.getCmp('endDay').getValue() + "&yugaoDay=" + Ext.getCmp('yugaoDay').getValue());
        }
    }

});
