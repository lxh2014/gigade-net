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
        width: 430,
        bodyPadding: 30,
        border: false,
        items: [
             {
                 html: '<div class="capion">提示：匯出庫存鎖報表</div>',
                 frame: false,
                 border: false
             },
                  {
                      xtype: 'fieldcontainer',
                      fieldLabel: "商品細項編號/條碼",
                      combineErrors: true,
                      margins: '0 200 0 0',
                      layout: 'hbox',
                      defaults: {
                          flex: 1,
                          hideLabel: true
                      },
                      items: [
                          {
                              xtype: 'radiofield',//同一個頁面中name相同,則只能選擇一個
                              name: 'us',
                              inputValue: "1",
                              id: "us1",
                              checked: true,
                              listeners: {
                                  change: function (radio, newValue, oldValue) {
                                      var i = Ext.getCmp('item_id');//製造日期
                                      var j = Ext.getCmp('startIloc');
                                      var z = Ext.getCmp('endIloc');
                                      if (newValue) {
                                          j.allowBlank = true;
                                          i.setDisabled(false);
                                          j.setDisabled(true);
                                          j.setValue("");
                                          i.allowBlank = false;
                                          z.allowBlank = true;
                                          z.setDisabled(true);
                                          z.setValue("");
                                      }
                                  }
                              }
                          },
                          {
                              xtype: "textfield",
                              fieldLabel: "",
                              id: 'item_id',
                              name: 'item_id',
                              minValue:0
                          }
                      ]
                  },
            {
                xtype: 'fieldcontainer',
                fieldLabel: "走道範圍",
                combineErrors: true,
                layout: 'hbox',
                margins: '0 200 0 0',
                defaults: {
                    flex: 1,
                    hideLabel: true
                },
                items: [
                    {
                        xtype: 'radiofield',
                        name: 'us',
                        inputValue: "2",
                        id: "us2",
                        listeners: {
                            change: function (radio, newValue, oldValue) {
                                var i = Ext.getCmp('item_id');//製造日期
                                var j = Ext.getCmp('startIloc');
                                var z = Ext.getCmp('endIloc');
                                if (newValue) {
                                    i.setDisabled(true);
                                    j.setDisabled(false);
                                    i.allowBlank = true;
                                    i.setValue("");
                                    j.allowBlank = false;
                                    z.allowBlank = false;
                                    z.setDisabled(false);
                                }
                            }
                        }
                    },
                      {
                     xtype: 'fieldcontainer',
                     fieldLabel: "走道範圍",
                     id:'Iloc_all',
                     width: 350,
                     combineErrors: true,
                     layout: 'hbox',
                     items: [
                         {
                             xtype: "textfield",
                             id: 'startIloc',
                             name: 'startIloc',
                             allowBlank: false,
                             disabled:true,
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
                             disabled:true,
                             flex: 5,
                             allowBlank: false
                         }
                     ]
                 },
                ]
            },
              {
                  xtype: 'button',
                  text: "確定匯出",
                  id: 'btnQuery',
                  buttonAlign: 'center',
                  handler: ExportKucunLock
              }

        ],
        renderTo: Ext.getBody()
    });

    function ExportKucunLock() {
        window.open("/WareHouse/ExportKucunLockList?item_id=" + Ext.getCmp('item_id').getValue() + "&startIloc=" + Ext.getCmp('startIloc').getValue() + "&endIloc=" + Ext.getCmp('endIloc').getValue());
    }

});
