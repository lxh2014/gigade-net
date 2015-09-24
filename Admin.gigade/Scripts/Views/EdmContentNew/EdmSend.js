sendFunction = function (row, store) {

    //Ext.define('gigade.edm_template', {
    //    extend: 'Ext.data.Model',
    //    fields: [
    //        { name: 'template_id', type: 'int' },
    //        { name: 'edit_url', type: 'string' }
    //    ]
    //});
    //var EdmTemplateStore = Ext.create("Ext.data.Store", {
    //    autoLoad: true,
    //    model: 'gigade.edm_template',
    //    proxy: {
    //        type: 'ajax',
    //        url: '/EdmNew/GetEdmTemplateStore',
    //        reader: {
    //            type: 'json',
    //            root: 'data'
    //        }
    //    }
    //});

    var importanceStore = Ext.create('Ext.data.Store', {
        fields: ['txt', 'value'],
        data: [
              { 'txt': '一般', 'value': '0' },
              { 'txt': '重要', 'value': '1' },
              { 'txt': '特級', 'value': '2' },
        ]
    });
    var sendFrm = Ext.create('Ext.form.Panel', {
        id: 'sendFrm',
        frame: true,
        plain: true,
        constrain: true,
        //autoScroll: true,
        layout: 'anchor',
        labelWidth: 45,
      //  url: '/EdmNew/SaveEdmContentNew',
     //   defaults: { anchor: "95%", msgTarget: "side" },
        items: [
              {
                  xtype: 'displayfield',
                  value:'測試發送，只會發送給自己，會立即寄出',
              },
     {
         xtype: 'button',
         id: 'test_send',
         text:'測試發送',
     },
         
      
            {
                xtype: 'datetimefield',
                fieldLabel: '排程發送時間',
                id: 'schedule_date',
                name: 'schedule_date',
                allowBlank: false
            },
           {
               xtype: 'datetimefield',
               fieldLabel: '信件有效時間',
               id: 'valid_until_date',
               name: 'valid_until_date',
               allowBlank: false
           },
           {
               xtype: 'combobox',
               id: 'send_condition',
               name: 'send_condition',
               fieldLabel:'發送名單條件',
           },
           {
               xtype: 'fieldcontainer',
               layout:'hbox',
               items: [
                   {
                       xtype: 'displayfield',
                       value:'123',
                   },
                   {
                       xtype: 'textareafield',
                       id: 'extra_send',
                       height: 200,
                       name: 'extra_send',
                   },
                    {
                        xtype: 'displayfield',
                        value: '123',
                    },
                     {
                         xtype: 'textareafield',
                         id: 'extra_no_send',
                         height: 200,
                         margin:'0 0 0 20',
                         name: 'extra_no_send',
                     },
               ],
           },
        ],
        buttons: [
            {
                formBind: true,
                disabled: true,
                text: '保存',
                handler: function () {
                        var form = this.up('form').getForm();
                        if (form.isValid()) {
                            this.disable();
                            form.submit({
                                params: {
                             
                                },
                                success: function (form, action) {
                                    var result = Ext.decode(action.response.responseText);
                                    if (result.success) {
                                        Ext.Msg.alert("提示信息", "保存成功! ");
                                        store.load();
                                        editWin.close();
                                    }
                                    else {
                                        Ext.Msg.alert("提示信息", "保存失敗! ");
                                        store.load();
                                        editWin.close();
                                    }
                                }
                            });
                        }

                }
            }
        ]
    });

    var sendWin = Ext.create('Ext.window.Window', {
        title: '電子報發送',
        iconCls: 'icon-user-edit',
        id: 'sendWin',
        height: 650,
        width: 750,
        y: 100,
        layout: 'fit',
        items: [sendFrm],
        constrain: true,
        closeAction: 'destroy',
        modal: true,
        //  resizable: false,
        labelWidth: 60,
        bodyStyle: 'padding:5px 5px 5px 5px',
        closable: false,
        tools: [
         {
             type: 'close',
             qtip: '是否關閉',
             handler: function (event, toolEl, panel) {
                 Ext.MessageBox.confirm("確認", "是否確定關閉窗口?", function (btn) {
                     if (btn == "yes") {
                         Ext.getCmp('sendWin').destroy();
                     }
                     else {
                         return false;
                     }
                 });
             }
         }
        ],
    });
    sendWin.show();
}