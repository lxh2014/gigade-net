Ext.require([
    'Ext.form.*'
]);

Ext.onReady(function () {
    var Iloc_idTypeStore = Ext.create('Ext.data.Store', {
        fields: ['txt', 'value'],
        data: [
            { "txt": "所有料位", "value": "0" },
            { "txt": "主料位", "value": "S" },
            { "txt": "副料位", "value": "R" }
        ]
    });
    var formPanel = Ext.create('Ext.form.Panel', {
        frame: false,
        width: 980,
        bodyPadding: 30,
        border: false,
        items: [
             {
                 html: '<div class="capion">提示:匯出可用副料位／主料位報表</div>',
                 frame: false,
                 border: false

             },
              {
                  xtype: 'combobox',
                  name: 'Ilocid_type',
                  id: 'Ilocid_type',
                  editable: false,
                  fieldLabel: "料位類型",
                  labelWidth: 63,
                  store: Iloc_idTypeStore,
                  queryMode: 'local',
                  submitValue: true,
                  displayField: 'txt',
                  valueField: 'value',
                  typeAhead: true,
                  forceSelection: false,
                  emptyText: '請選擇料位類型',
                  value: 0
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
                             flex:5
                         },
                         {
                             xtype: 'displayfield',
                             value: "--",
                             flex:1
                         },
                         {
                             xtype: "textfield",
                             id: 'endIloc',
                             name: 'endIloc',
                             flex:5,
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
        window.open("/WareHouse/ExportIlocList?Ilocid_type=" + Ext.getCmp('Ilocid_type').getValue() + "&startIloc=" + Ext.getCmp('startIloc').getValue() + "&endIloc=" + Ext.getCmp('endIloc').getValue());
    }

});
