sendFunction = function (row, store) {
    Ext.define('gigade.elcm', {
        extend: 'Ext.data.Model',
        fields: [
            { name: 'elcm_id', type: 'int' },
            { name: 'elcm_name', type: 'string' }
        ]
    });
    var elcmStore = Ext.create("Ext.data.Store", {
        autoLoad: true,
        model: 'gigade.elcm',
        proxy: {
            type: 'ajax',
            url: '/EdmS/GetConditionList',
            reader: {
                type: 'json',
                root: 'data'
            }
        }
    });

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
                                      id: 'subject',
                                      name: 'subject',
                                      width:390,
                                      fieldLabel: '電子報名稱',
                                  },
            {
                xtype: 'fieldset',
                layout: 'anchor',
                items: [
                     {
                         xtype: 'displayfield',
                         value: '測試發送，只會發送給自己，會立即寄出',
                     },

                     {
                         xtype: 'displayfield',
                         id: 'content_id',
                         name: 'content_id',
                         fieldLabel: '編號',
                         hidden: true,
                     },
                         {
                             xtype: 'displayfield',
                             id: 'group_id',
                             name: 'group_id',
                             fieldLabel: '電子報類型',
                             hidden: true,
                         },
                           {
                               xtype: 'displayfield',
                               id: 'sender_id',
                               name: 'sender_id',
                               fieldLabel: '寄件者id',
                               hidden:true,
                           },
                         
                         {
                             xtype: 'displayfield',
                             id: 'sender_email',
                             name: 'sender_email',
                             fieldLabel: '寄件者mail',
                             hidden: true,
                         },
                          {
                              xtype: 'displayfield',
                              id: 'sender_name',
                              name: 'sender_name',
                              fieldLabel: '寄件者name',
                              hidden: true,
                          },
                         
                              {
                                  xtype: 'displayfield',
                                  id: 'template_data',
                                  name: 'template_data',
                                  fieldLabel: 'template_data',
                                  hidden: true,
                              },

                    {
                        xtype: 'button',
                        id: 'test_send',
                        text: '測試發送',
                        handler: function () {
                            var myMask = new Ext.LoadMask(Ext.getBody(), { msg: "Please wait..." });
                            myMask.show();
                            Ext.Ajax.request({
                                url: '/EdmNew/SendEdm',
                                timeout:90000,
                                params: {
                                    testSend: 'true',
                                    content_id: Ext.getCmp('content_id').getValue(),
                                    group_id: Ext.getCmp('group_id').getValue(),
                                    sender_email: Ext.getCmp('sender_email').getValue(),
                                    sender_name: Ext.getCmp('sender_name').getValue(),
                                    subject: Ext.getCmp('subject').getValue(),
                                    body: Ext.getCmp('template_data').getValue(),

                                },
                                success: function (form, action) {
                                    myMask.hide();
                                    var result = Ext.decode(form.responseText);
                                    if (result.success) {
                                        Ext.Msg.alert("提示信息", "測試電子報發送成功，請注意查收！");
                                    }
                                    else {
                                        Ext.Msg.alert("提示信息", "測試電子報發送失敗！");
                                    }
                                },
                                failure: function () {
                                    myMask.hide();
                                    Ext.Msg.alert("提示信息", "出現異常！");
                                }
                            });
                            
                        }
                     },
                ]
            },
            {
                xtype: 'datetimefield',
                fieldLabel: '排程發送時間',
                id: 'schedule_date',
                width:275,
                name: 'schedule_date',
                editable: false,
                allowBlank: false,
                allowBlank: false,
                format: 'Y-m-d H:i:s',
                value: new Date( new Date().getFullYear(),new Date().getMonth(),new Date().getDate()+1 ),
                listeners: {
                    select: function () {
                        var sd = Ext.getCmp('schedule_date');
                        var ed = Ext.getCmp('expire_date');
                        var nowDate = new Date(new Date().getFullYear(), new Date().getMonth(), new Date().getDate(), new Date().getHours(),new Date().getMinutes(),new Date().getMilliseconds());
                        if (sd.getValue() <= nowDate)
                        {
                            var new_time = new Date(new Date().getFullYear(), new Date().getMonth(), new Date().getDate() + 1, new Date().getHours(), new Date().getMinutes(), new Date().getMilliseconds());
                            sd.setValue(new_time);
                            if (sd.getValue() >= ed.getValue()) {
                                var new_time2 = new Date(sd.getValue().getFullYear(), sd.getValue().getMonth(), sd.getValue().getDate() + 1);
                                ed.setValue(new_time2);
                            }
                            Ext.Msg.alert("提示信息", "排程發送時間不能小於當前時間！");
                        }
                        if (sd.getValue() >= ed.getValue()) {
                            var new_time2 = new Date(sd.getValue().getFullYear(), sd.getValue().getMonth(), sd.getValue().getDate() + 1);
                            ed.setValue(new_time2);
                        }
                    }
                },
            },
           {
               xtype: 'datefield',
               fieldLabel: '信件有效時間',
               width: 275,
               id: 'expire_date',
               name: 'expire_date',
               format: 'Y-m-d 23:59:59',
               editable: false,
               allowBlank: false,
               value: new Date(new Date().getFullYear(), new Date().getMonth(), new Date().getDate() + 2),
               listeners: {
                   select: function () {
                       var sd = Ext.getCmp('schedule_date');//排程發送時間
                       var ed = Ext.getCmp('expire_date');//信件有效時間
                       var nowDate = new Date(new Date().getFullYear(), new Date().getMonth(), new Date().getDate());
                       if (ed.getValue() <= sd.getValue()) {
                           var new_time = new Date(sd.getValue().getFullYear(), sd.getValue().getMonth(), sd.getValue().getDate() + 1);
                           ed.setValue(new_time);
                           Ext.Msg.alert("提示信息", "信件有效時間須大於排程發送時間！");
                       }
                   }
               },
           },
           {
               xtype: 'combobox',
               displayField: 'elcm_name',
               width: 275,
               store:elcmStore,
               valueField: 'elcm_id',
               id: 'elcm',
               name: 'elcm',
               fieldLabel: '發送名單條件',
               editable: false,
               allowBlank: false,
               lastQuery:'',
               value:'0',
           },
           {
               xtype: 'checkboxfield',
               boxLabel: '包含訂閱(會員及非會員)',
               checked: true,
               id: 'checkbox1',
               margin:'0 0 0 105',
           },
           {
               xtype: 'fieldset',
               layout: 'anchor',
               items: [
                              {
                                  xtype: 'fieldcontainer',
                                  layout: 'hbox',
                                  items: [
                                      {
                                          xtype: 'fieldcontainer',
                                          items: [
                                              {
                                                  xtype: 'displayfield',
                                                  value:'額外發送名單（換行輸入下一筆）',
                                              },
                                               {
                                                   xtype: 'textareafield',
                                                   id: 'extra_send',
                                                   width: 180,
                                                   height: 225,
                                                   name: 'extra_send',
                                               },
                                          ]
                                      },
                                      {
                                        xtype: 'fieldcontainer',
                                        items: [
                                            {
                                                xtype: 'displayfield',
                                                value: '額外排除名單（換行輸入下一筆）',
                                                margin: '0 0 0 30',
                                            },
                                             {
                                                 xtype: 'textareafield',
                                                 id: 'extra_no_send',
                                                 width:180,
                                                 height: 225,
                                                 margin:'0 0 0 30',
                                                 name: 'extra_no_send',
                                             },
                                        ]
                                    },
                                  ],
                              },
                              {
                                  xtype: 'button',
                                  text: '正式發送',
                                  handler: function () {
                                      var myMask = new Ext.LoadMask(Ext.getBody(), { msg: "Please wait..." });
                                      myMask.show();

                                      var schedule_date = Ext.getCmp('schedule_date').getValue(); //為空則null
                                      var expire_date = Ext.getCmp('expire_date').getValue();//為空則null
                                      var elcm = Ext.getCmp('elcm').getValue();//為空則null
                                     
                                      if (schedule_date == null)
                                      {
                                          Ext.Msg.alert("提示信息", "排程發送時間未填寫！");
                                          myMask.hide();
                                          return;
                                      } if (expire_date == null) {
                                          Ext.Msg.alert("提示信息", "信件有效時間未填寫！");
                                          myMask.hide();
                                          return;
                                      }
                                      if (elcm == null) {
                                          Ext.Msg.alert("提示信息", "發送名單未選擇！");
                                          myMask.hide();
                                          return;
                                      }
                                      Ext.Ajax.request({
                                          url: '/EdmNew/SendEdm',
                                          params: {
                                              testSend: 'false',
                                              content_id: Ext.getCmp('content_id').getValue(),
                                              group_id: Ext.getCmp('group_id').getValue(),
                                              sender_email: Ext.getCmp('sender_email').getValue(),
                                              sender_name: Ext.getCmp('sender_name').getValue(),
                                              subject: Ext.getCmp('subject').getValue(),
                                              body: Ext.getCmp('template_data').getValue(),
                                              schedule_date: Ext.getCmp('schedule_date').getValue(),
                                              expire_date: Ext.getCmp('expire_date').getValue(),
                                              elcm_id: Ext.getCmp('elcm').getValue(),
                                              extra_send: Ext.getCmp('extra_send').getValue(),
                                              extra_no_send: Ext.getCmp('extra_no_send').getValue(),
                                              is_outer: Ext.getCmp('checkbox1').getValue(),
                                          },
                                          success: function (form, action) {
                                              myMask.hide();
                                              var result = Ext.decode(form.responseText);
                                              if (result.success) {
                                                  Ext.Msg.alert("提示信息", "電子報發送成功，請注意查收！");
                                                  sendWin.close();
                                                  store.load();
                                              }
                                              else {
                                                  Ext.Msg.alert("提示信息", "電子報發送失敗！");
                                              }
                                          },
                                          failure: function () {
                                              myMask.hide();
                                              Ext.Msg.alert("提示信息", "出現異常！");
                                          }
                                      });
                                  }
                              },
               ],
           },
        ],
    });

    var sendWin = Ext.create('Ext.window.Window', {
        title: '電子報發送',
        iconCls: 'icon-user-edit',
        id: 'sendWin',
        height: 542,
        width: 470,
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
        listeners: {
            'show': function () {
                Ext.getCmp('content_id').setValue(row.data.content_id);
                Ext.getCmp('sender_id').setValue(row.data.sender_id);
                Ext.getCmp('sender_email').setValue(row.data.sender_email);
                Ext.getCmp('sender_name').setValue(row.data.sender_name);
                Ext.getCmp('group_id').setValue(row.data.group_id);
                Ext.getCmp('subject').setValue(row.data.subject);
                Ext.getCmp('template_data').setValue(row.data.template_data);
            }
        }
    });
    sendWin.show();
}