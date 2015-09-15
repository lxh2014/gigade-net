Ext.require([
    'Ext.form.*'
]);
var yunsongtype = Ext.create('Ext.data.Store', {
    fields: ['id', 'name'],
    data: [
        { "id": "2", "name": "常溫" },
        { "id": "92", "name": "冷凍" }
        //...
    ]
});
Ext.onReady(function () {
    var formPanel = Ext.create('Ext.form.Panel', {
        frame: false,
        width: 980,
        bodyPadding: 30,
        border: false,
        items: [
             {
                 html: '<div class="capion">提示：匯出大出貨報表（熱區品項調整料位）</div>',
                 frame: false,
                 border: false

             },
              {
                  html: '<div class="capion">依據目標訂單中個別商品總的訂貨數量,由大到小進行排序,取前50名</div>',
                  frame: false,
                  border: false

              },
                {
                    xtype: 'combobox',
                    name: 'search',
                    id: 'search',
                    editable: false,
                    fieldLabel: "溫層",
                    store: yunsongtype,
                    queryMode: 'local',
                    submitValue: true,
                    displayField: 'name',
                    valueField: 'id',
                    typeAhead: true,
                    forceSelection: false,
                    value:2
                },
               {
                   fieldLabel: '請選擇匯出個數',
                   xtype: 'numberfield',
                   id: 'counts',
                   name: 'counts',
                   allowBlank: false,
                   value:50,
                   minValue: 1
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
        window.open("/WareHouse/ExportDeliveryStatement?counts=" + Ext.getCmp("counts").getValue() + "&searchtype=" + Ext.getCmp("search").getValue());
    }

});
