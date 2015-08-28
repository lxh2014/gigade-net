

Ext.onReady(function () {
    product_id = window.parent.GetProductId();
    createForm(product_id);
    var mainForm = Ext.create('Ext.tab.Panel', {
        id: 'ContentPanel',
        tabPosition: 'bottom',
        activeTab: 0,
        layout: 'fit',
        items: [tabs],
        frame: true
    })


    Ext.create('Ext.Viewport', {
        layout: 'fit',
        items: [mainForm],
        height: document.documentElement.clientHeight * 565 / 783,
        renderTo: Ext.getBody(),
        autoScroll: true,
        listeners: {
            resize: function () {
                //mainForm.height = document.documentElement.clientHeight * 565 / 783;
                this.doLayout();
            }
        }
    });

});
