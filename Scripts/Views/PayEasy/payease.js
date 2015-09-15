
Ext.onReady(function () {
    var filterPanel = Ext.create('Ext.form.Panel', {
    bodyPadding: 10,  // Don't want content to crunch against the borders
    title: PAYEASY,
    items: [
        {
            html: PAYEASYWARNING,
            border: 0,
            height: 40
        },
        {
            html: PAYEASYID,
            border:0,
            height:20
        },
        {
            xtype: 'numberfield',
            id: 'product_id',
            name: 'product_id',
            minValue: 1,
            fieldlabel: PRODUCTSTART                
        },
        {
            xtype: 'button',
            text: CONFIRM,
            handler: function () {
                //alert(Ext.getCmp('product_id').getValue());
                window.open('/payeasy/outexcel?product_id=' + Ext.getCmp('product_id').getValue());
            }
        }]
       
   // renderTo: Ext.getBody()
});
    var filterPanel2 = Ext.create('Ext.form.Panel', {
        bodyPadding: 10,  // Don't want content to crunch against the borders
        title: PAYEASYEDIT,
        url: '/PayEasy/OutExcel2',
        items: [
            {
                html: PAYEASYWARN,
                border: 0,
                height:40
            },
            {   //專區Banner           
                xtype: 'filefield',
                name: 'banner_image',
                id: 'banner_image',
                fieldLabel: UPPRODUCT,
                msgTarget: 'side',
                allowBlank: false,
                buttonText: SELECTEXC,
                fileUpload: true
            },
            {
                xtype: 'button',
                text: CONFIRM,
                width: 50,
                handler: function () {            
                    var form = this.up('form').getForm();
                    if (form.isValid()) {
                        form.submit({
                    });
                }
            }
        }]
        // renderTo: Ext.getBody()
    });
    Ext.create('Ext.tab.Panel', {
   
        renderTo: Ext.getBody(),
        items: [filterPanel, filterPanel2
        ]
    });
});

PayeaseCsv = function () {
    Ext.Ajax.request({
        url: '/PayEasy/OutExcel3',
        method: 'post',
        params: {
            product_id: Ext.getCmp("product_id").getValue() 
        },
        failure: function () {
            Ext.Msg.alert("a", "操作失敗");
        }
    });
}
