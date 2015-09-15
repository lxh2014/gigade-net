Ext.require([
    'Ext.form.*'
]);

Ext.onReady(function () {
    var formPanel = Ext.create('Ext.form.Panel', {
        frame: false,
        width: 980,
        bodyPadding: 30,
        border: false,
        items: [
            {
                html: '<div class="capion">提示：匯出無條碼商品表</div>',
                frame: false,
                border: false

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
                    }
                ]
            },
            {
                xtype: 'button',
                text: "確定匯出",
                id: 'btnQuery',
                buttonAlign: 'center',
                handler: ExportIloc
            }
        ],
        renderTo: Ext.getBody()
    });

    function ExportIloc() {
        window.open("/WareHouse/ExportInoiupc?startIloc=" + Ext.getCmp('startIloc').getValue() + "&endIloc=" + Ext.getCmp('endIloc').getValue());
    }

});
