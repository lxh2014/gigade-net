Ext.Loader.setConfig({ enabled: true });
Ext.Loader.setPath('Ext.ux', '/Scripts/Ext4.0/ux');
Ext.require([
    'Ext.form.Panel',
    'Ext.ux.form.MultiSelect',
    'Ext.ux.form.ItemSelector'
]);

Ext.onReady(function () {
    var exportExl = Ext.create('Ext.form.Panel', {
        frame: false,
        width:500,
        bodyPadding: 30,
        border: false,
        id: 'Import',
        items: [
         {
             html: '<div class="capion">提示：匯出缺貨明細～未完成理貨工作報表</div>',
             frame: false,
             border: false
         },
        {
            xtype: 'fieldcontainer',
            fieldLabel: '選擇',
            id: 'validate',
            width:800,
            defaultType: 'radiofield',
            submitValue: true,
            defaults: {
                flex: 1
            },
            layout: 'hbox',
            items: [{
                boxLabel: '製作總表',
                name: 'check',
                inputValue: '0',
                id: 'radio1',
                checked: true,
                listeners: {
                    change: function (radio, newValue, oldValue) {
                        var assg_id = Ext.getCmp("assg_id");
                        if (newValue) {

                            assg_id.setValue("");
                            assg_id.setDisabled(true);

                        }
                    }
                }
            },
  {
      xtype: 'fieldcontainer',
      combineErrors: true,
      layout: 'hbox',
      margins: '0 10 0 0',
      items: [
      {
          boxLabel: '產生明細（缺貨報表）',
          xtype: 'radiofield',
          name: 'check',
          inputValue: '1',
          id: "radio2",
          listeners: {
              change: function (radio, newValue, oldValue) {
                  var assg_id = Ext.getCmp("assg_id");
                  if (newValue) {
                      assg_id.setDisabled(false);
                     
                  }
              }
          }
      },
    
    {
        xtype: "textfield",
        id: 'assg_id',
        labelWidth: 150,
        disabled: true,
        width: 200,
        name: 'assg_id'

    }
      ]

  }
            ]
        },
     {
         xtype: 'button',
         text: '確定',
         width: 70,
         //style: {
         //    marginLeft: '50px'
         //},
         id: 'export', 
         handler: function () {
             window.open('/WareHouse/OutUndoneJobExl?radio1=' + Ext.getCmp('radio1').getValue() + '&radio2=' + Ext.getCmp('radio2').getValue() + '&assg_id=' + Ext.getCmp("assg_id").getValue());
            
         }
     }]

    });

    Ext.create('Ext.container.Viewport', {
        layout: 'fit',
        items: [exportExl],
        renderTo: Ext.getBody(),
        autoScroll: true,
        listeners: {
            resize: function () {
                // exportTab.width = document.documentElement.clientWidth;
                this.doLayout();
            }
        }
    });
});
