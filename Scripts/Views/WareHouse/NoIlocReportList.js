Ext.require([
    'Ext.form.*'
]);
Ext.define("gigade.Vendor", {
    extend: 'Ext.data.Model',
    fields: [
        { name: "vendor_id", type: "string" },
        { name: "vendor_name_simple", type: "string" }]
});
var VendorStore = Ext.create('Ext.data.Store', {
    model: 'gigade.Vendor',
    autoLoad: true,
    proxy: {
        type: 'ajax',
        url: "/WareHouse/GetVendorName",
        actionMethods: 'post',
        reader: {
            type: 'json',
            root: 'data'
        }
    }
});

Ext.onReady(function () {
    var formPanel = Ext.create('Ext.form.Panel', {
        frame: false,
        width: 980,
        bodyPadding: 30,
        border: false,
        items: [
        {
                html: '<div class="capion">提示：匯出無主料位商品報表</div>',
                frame: false,
                border: false

            },
           {
               xtype: 'combobox',
               allowBlank: true,
               name: 'search',
               id: 'search',
               editable: false,
               fieldLabel: SEARCH,
               labelWidth: 40,
               queryMode: 'local',
               store: VendorStore,
               submitValue: true,
               displayField: 'vendor_name_simple',
               valueField: 'vendor_id',
               forceSelection: false,
               value: '0'
             }
             ,
           {
               xtype: 'button',
                text: "確定匯出",
               id: 'btnQuery',
                buttonAlign: 'center',
                handler: function () {
                    //alert(Ext.getCmp('product_id').getValue());
                    window.open('/WareHouse/NoIlocReportList?search=' + Ext.getCmp('search').getValue());
                }
            }
      ],
        renderTo: Ext.getBody()
    });
    });
