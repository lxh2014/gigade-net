Ext.require([
    'Ext.form.*'
]);
var sort = "0";
Ext.onReady(function () {
    var formPanel = Ext.create('Ext.form.Panel', {
        frame: false,
        //width: 430,
        bodyPadding: 30,
        border: false,
        items: [
            {
                html: '<div class="capion">提示：匯出料位盤點差異報表(OBK)</div>',
                frame: false,
                border: false
            },
            {
                xtype: 'fieldcontainer',
                fieldLabel: "走道範圍",
                id: 'Iloc_all',
                width: 350,
                combineErrors: true,
                layout: 'hbox',
                items: [
                    {
                        xtype: "textfield",
                        id: 'startIloc',
                        name: 'startIloc',
                        allowBlank: true,
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
                        allowBlank: true
                    }
                ]
            },
            {
                xtype: 'fieldcontainer',
                fieldLabel: "金額",
                id: 'cost',
                width: 350,
                combineErrors: true,
                layout: 'hbox',
                items: [
                    {
                        xtype: "textfield",
                        id: 'startcost',
                        name: 'startcost',
                        allowBlank: true,
                        flex: 5
                    },
                    {
                        xtype: 'displayfield',
                        value: "--",
                        flex: 1
                    },
                    {
                        xtype: "textfield",
                        id: 'endcost',
                        name: 'endcost',
                        flex: 5,
                        allowBlank: true
                    }
                ]
            },
            {
                xtype: 'button',
                text: "確定匯出",
                id: 'btnQuery',
                buttonAlign: 'center',
                handler: ExportCountBook
            }
        ],
        renderTo: Ext.getBody()
    });

    function ExportCountBook() {

        window.open("/WareHouse/GetCountBookOBK?startIloc=" + Ext.getCmp('startIloc').getValue() + "&endIloc=" + Ext.getCmp('endIloc').getValue() + "&startcost=" + Ext.getCmp('startcost').getValue() + "&endcost=" + Ext.getCmp('endcost').getValue());
    }

});
