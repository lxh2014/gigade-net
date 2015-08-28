Ext.require([
    'Ext.form.*'
]);

Ext.onReady(function () {
    var Aleld_idTypeStore = Ext.create('Ext.data.Store', {
        fields: ['txt', 'value'],
        data: [
            { "txt": "寄倉", "value": "2" },
            { "txt": "調度", "value": "3" }
        ]
    });
    var formPanel = Ext.create('Ext.form.Panel', {
        frame: false,
        width: 980,
        bodyPadding: 30,
        border: false,
        items: [
             {
                 html: '<div class="capion">提示:匯出撿貨表by料位元報表</div>',
                 frame: false,
                 border: false

             },
              {
                  xtype: 'textfield',
                  name: 'job_id',
                  id: 'job_id',
                  editable: false,
                  fieldLabel: "工作編號",
                  allowBlank: false,
                  listeners: {
                      change: function () {
                          var id = Ext.getCmp('job_id').getValue();
                          if (id.trim() == "") {
                              Ext.Msg.alert("提示信息", "工作編號不能為空!");
                          }
                          else {
                              Ext.getCmp('btn_Query').setDisabled(false);
                          }
                         
                      }
                  }
              },
                {
                    xtype: 'combobox',
                    name: 'aseld_type',
                    id: 'aseld_type',
                    editable: false,
                    fieldLabel: "類型",
                    store: Aleld_idTypeStore,
                    queryMode: 'local',
                    submitValue: true,
                    displayField: 'txt',
                    valueField: 'value',
                    typeAhead: true,
                    forceSelection: false,
                    emptyText: '請選擇類型',
                    value:2
                },
              {
                  xtype: 'button',
                  text: "確定匯出",
                  id: 'btn_Query',
                  buttonAlign: 'center',
                  disabled:true,
                  handler: ExportAseld
              }

        ],
        renderTo: Ext.getBody()
    });

    function ExportAseld() {
        window.open("/WareHouse/ExportAseldMessage?job_id=" + Ext.getCmp('job_id').getValue() + "&aseld_type=" + Ext.getCmp('aseld_type').getValue());
    }

});
