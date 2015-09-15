
var fortunePanel;

function initFortunePanel(product_id) {
    fortunePanel = Ext.create('Ext.form.Panel', {
        id: 'fortunePanel',
        width: 300,
        defaults: { anchor: "95%", msgTarget: "side" },
        border: false,
        padding: '20 0 0 20',
        items: [{
            xtype: 'displayfield',
            id: 'numQuota',
            hidden: true,
            colName: 'Fortune_Quota',
            name: 'Fortune_Quota',
            margin: '0 0 20 0',
            fieldLabel: FORTUNE_NUM
        }, {
            xtype: 'displayfield',
            id: 'numFreight',
            hidden: true,
            colName: 'Fortune_Freight',
            name: 'Fortune_Freight',
            fieldLabel: FORTUNE_FREIGHT
        }],
        listeners: {
            beforerender: function () {
                fortunePanel.getForm().load({
                    url: '/Product/fortuneQuery',
                    method: 'POST',
                    params: {
                        ProductId: product_id
                    },
                    success: function (form, action) { }

                });
            }
        }
    });

}

 

