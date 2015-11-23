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


    Ext.define('gigade.email_group', {
        extend: 'Ext.data.Model',
        fields: [
            { name: 'group_id', type: 'int' },
            { name: 'group_name', type: 'string' }
        ]
    });
    var EmailGroupStore = Ext.create("Ext.data.Store", {
        autoLoad: true,
        model: 'gigade.email_group',
        proxy: {
            type: 'ajax',
            url: '/EdmNew/EmailGroupStore',
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
        layout: 'anchor',
        labelWidth: 45,
        autoScroll: true,
        items: [
                                  {
                                      xtype: 'displayfield',
                                      id: 'subject',
                                      name: 'subject',
                                      width: 390,
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
                               hidden: true,
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
                                  id: 'template_data_send',
                                  name: 'template_data_send',
                                  fieldLabel: 'template_data',
                                  hidden: true,
                              },
                             {
                                 xtype: 'textareafield',
                                 id: 'test_send_list',
                                 width: 295,
                                 height: 225,
                                 name: 'test_send_list',
                                 listeners: {
                                     'afterrender': function () {
                                         var myMask = new Ext.LoadMask(Ext.getBody(), { msg: "正在獲取測試名單..." });
                                         myMask.show();
                                         Ext.Ajax.request({
                                             url: '/EdmNew/GetTestSendList',
                                             success: function (data) {
                                                 myMask.hide();
                                                 var result = data.responseText;
                                                 Ext.getCmp('test_send_list').setValue(result);
                                             }
                                         });
                                     }
                                 }
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
                                timeout: 90000,
                                params: {
                                    testSend: 'true',
                                    content_id: Ext.getCmp('content_id').getValue(),
                                    group_id: Ext.getCmp('group_id').getValue(),
                                    sender_email: Ext.getCmp('sender_email').getValue(),
                                    sender_name: Ext.getCmp('sender_name').getValue(),
                                    subject: Ext.getCmp('subject').getValue(),
                                    body: Ext.getCmp('template_data_send').getValue(),
                                    test_send_list: Ext.getCmp('test_send_list').getValue(),

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
                width: 275,
                name: 'schedule_date',
                editable: false,
                allowBlank: false,
                allowBlank: false,
                format: 'Y-m-d H:i:s',
                value: new Date(new Date().getFullYear(), new Date().getMonth(), new Date().getDate() + 1),
                listeners: {
                    select: function () {
                        var sd = Ext.getCmp('schedule_date');
                        var ed = Ext.getCmp('expire_date');
                        var nowDate = Ext.htmlEncode(Ext.Date.format(new Date(), 'Y-m-d H:i:s'));
                        var sdTime = Ext.htmlEncode(Ext.Date.format(new Date(Ext.getCmp('schedule_date').getValue()), 'Y-m-d H:i:s'));
                        if (sdTime <= nowDate) {
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
                       var sdTime = Ext.htmlEncode(Ext.Date.format(new Date(Ext.getCmp('schedule_date').getValue()), 'Y-m-d H:i:s'));
                       var edTime = Ext.htmlEncode(Ext.Date.format(new Date(Ext.getCmp('expire_date').getValue()), 'Y-m-d 23:59:59'));
                       if (sdTime>= edTime) {
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
               store: elcmStore,
               valueField: 'elcm_id',
               id: 'elcm',
               name: 'elcm',
               fieldLabel: '自動名單條件',
               editable: false,
               allowBlank: false,
               lastQuery: '',
               value: '0',
           },
           {
               xtype: 'combobox',
               width: 275,
               store: EmailGroupStore,
               valueField: 'group_id',
               displayField: 'group_name',
               fieldLabel: '固定信箱名單',
               editable: false,
               allowBlank: false,
               id: 'email_group_id',
               name: 'email_group_id',
               lastQuery: '',
               value: '0',
           },
           {
               xtype: 'checkboxfield',
               boxLabel: '包含訂閱(會員及非會員)',
               checked: true,
               id: 'checkbox1',
               margin: '0 0 0 105',
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
                                                  value: '額外發送名單（換行輸入下一筆）',
                                                  margin: '0 0 0 30',
                                              },
                                               {
                                                   xtype: 'textareafield',
                                                   id: 'extra_send',
                                                   width: 295,
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
                                                  margin: '0 0 0 60',
                                              },
                                               {
                                                   xtype: 'textareafield',
                                                   id: 'extra_no_send',
                                                   width: 295,
                                                   height: 225,
                                                   margin: '0 0 0 30',
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
                                      var elcm = Ext.getCmp('elcm').getValue();
                                      var email_group_id = Ext.getCmp('email_group_id').getValue();
                                      var checkbox1 = Ext.getCmp('checkbox1').getValue();
                                      var extra_send = Ext.getCmp('extra_send').getValue();
                                      var extra_no_send = Ext.getCmp('extra_no_send').getValue();

                                      if (elcm == 0 && email_group_id == 0 && checkbox1 == 0 && checkbox1 == false && extra_send == "" && extra_no_send == "") {
                                          Ext.Msg.alert("提示信息", "當前所選條件未能得出有效信箱！");
                                      }
                                      else {
                                          var myMask = new Ext.LoadMask(Ext.getBody(), { msg: "Please wait..." });
                                          myMask.show();

                                          var schedule_date = Ext.getCmp('schedule_date').getValue(); //為空則null
                                          var expire_date = Ext.getCmp('expire_date').getValue();//為空則null
                                          var elcm = Ext.getCmp('elcm').getValue();//為空則null

                                          if (schedule_date == null) {
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
                                              timeout: 180000,
                                              params: {
                                                  testSend: 'false',
                                                  content_id: Ext.getCmp('content_id').getValue(),
                                                  group_id: Ext.getCmp('group_id').getValue(),
                                                  sender_email: Ext.getCmp('sender_email').getValue(),
                                                  sender_name: Ext.getCmp('sender_name').getValue(),
                                                  subject: Ext.getCmp('subject').getValue(),
                                                  body: Ext.getCmp('template_data_send').getValue(),
                                                  schedule_date: Ext.getCmp('schedule_date').getValue(),
                                                  expire_date: Ext.getCmp('expire_date').getValue(),
                                                  elcm_id: Ext.getCmp('elcm').getValue(),
                                                  extra_send: Ext.getCmp('extra_send').getValue(),
                                                  extra_no_send: Ext.getCmp('extra_no_send').getValue(),
                                                  is_outer: Ext.getCmp('checkbox1').getValue(),
                                                  email_group_id: Ext.getCmp('email_group_id').getValue(),
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
                                  }
                              },
               ],
           },
        ],
    });
    var subjectFrm = Ext.create('Ext.form.Panel', {
        id: 'subjectFrm',
        frame: true,
        plain: true,
        layout: 'anchor',
        width: 700,
        constrain: true,
        items: [
            {
                xtype: 'displayfield',
                id: 'subject',
                name: 'subject',
                width: 390,
                fieldLabel: '電子報名稱',
                margin: '0 0 0 10',

            }
        ],
    });
    var tabs1 = Ext.createWidget('tabpanel', {
      //  renderTo: 'tabPanel',
        activeTab: 0,
        width: 700,
        constrain: true,
        items: [
            {
                id: 'tab1',
                title: '測試發送',
                items: [
                      //{
                      //    xtype: 'displayfield',
                      //    id: 'subject',
                      //    name: 'subject',
                      //    width: 390,
                      //    fieldLabel: '電子報名稱',
                      //    margin:'0 0 0 10',
                      //},
                      {
                          xtype: 'displayfield',
                          margin:'0 0 10 0 ',
                      },
                      {
                          xtype: 'fieldset',
                          layout: 'anchor',
                          items: [
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
                                                     hidden: true,
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
                                                        id: 'template_data_send',
                                                        name: 'template_data_send',
                                                        fieldLabel: 'template_data',
                                                        hidden: true,
                                           },

                                             {
                                                 xtype: 'displayfield',
                                                 value: '測試名單（換行輸入下一筆）',
                                                 margin: '0 0 0 2',
                                             },


                                           {
                                                       xtype: 'textareafield',
                                                       id: 'test_send_list',
                                                       width: 295,
                                                       height: 225,
                                                       name: 'test_send_list',
                                                       listeners: {
                                                           'afterrender': function () {
                                                               var myMask = new Ext.LoadMask(Ext.getBody(), { msg: "正在獲取測試名單..." });
                                                               myMask.show();
                                                               Ext.Ajax.request({
                                                                   url: '/EdmNew/GetTestSendList',
                                                                   success: function (data) {
                                                                       myMask.hide();
                                                                       var result = data.responseText;
                                                                       Ext.getCmp('test_send_list').setValue(result);
                                                                   }
                                                               });
                                                           }
                                                       }
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
                                                      timeout: 90000,
                                                      params: {
                                                          testSend: 'true',
                                                          content_id: Ext.getCmp('content_id').getValue(),
                                                          group_id: Ext.getCmp('group_id').getValue(),
                                                          sender_email: Ext.getCmp('sender_email').getValue(),
                                                          sender_name: Ext.getCmp('sender_name').getValue(),
                                                          subject: Ext.getCmp('subject').getValue(),
                                                          body: Ext.getCmp('template_data_send').getValue(),
                                                          test_send_list: Ext.getCmp('test_send_list').getValue(),

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
                      
                ]
            },
            {
                id: 'tab2',
                title: '正式發送',
                items: [
                                 {
                                     xtype: 'datetimefield',
                                     fieldLabel: '排程發送時間',
                                     id: 'schedule_date',
                                     width: 275,
                                     name: 'schedule_date',
                                     editable: false,
                                     allowBlank: false,
                                     allowBlank: false,
                                     format: 'Y-m-d H:i:s',
                                     value: new Date(new Date().getFullYear(), new Date().getMonth(), new Date().getDate() + 1),
                                     listeners: {
                                         select: function () {
                                             var sd = Ext.getCmp('schedule_date');
                                             var ed = Ext.getCmp('expire_date');
                                             var nowDate = Ext.htmlEncode(Ext.Date.format(new Date(), 'Y-m-d H:i:s'));
                                             var sdTime = Ext.htmlEncode(Ext.Date.format(new Date(Ext.getCmp('schedule_date').getValue()), 'Y-m-d H:i:s'));
                                             if (sdTime <= nowDate) {
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
                       var sdTime = Ext.htmlEncode(Ext.Date.format(new Date(Ext.getCmp('schedule_date').getValue()), 'Y-m-d H:i:s'));
                       var edTime = Ext.htmlEncode(Ext.Date.format(new Date(Ext.getCmp('expire_date').getValue()), 'Y-m-d 23:59:59'));
                       if (sdTime >= edTime) {
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
               store: elcmStore,
               valueField: 'elcm_id',
               id: 'elcm',
               name: 'elcm',
               fieldLabel: '自動名單條件',
               editable: false,
               allowBlank: false,
               lastQuery: '',
               value: '0',
           },
           {
               xtype: 'combobox',
               width: 275,
               store: EmailGroupStore,
               valueField: 'group_id',
               displayField: 'group_name',
               fieldLabel: '固定信箱名單',
               editable: false,
               allowBlank: false,
               id: 'email_group_id',
               name: 'email_group_id',
               lastQuery: '',
               value: '0',
           },
           {
               xtype: 'checkboxfield',
               boxLabel: '包含訂閱(會員及非會員)',
               checked: true,
               id: 'checkbox1',
               margin: '0 0 0 105',
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
                                                  value: '額外發送名單（換行輸入下一筆）',
                                                  margin: '0 0 0 30',
                                              },
                                               {
                                                   xtype: 'textareafield',
                                                   id: 'extra_send',
                                                   width: 295,
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
                                                  margin: '0 0 0 60',
                                              },
                                               {
                                                   xtype: 'textareafield',
                                                   id: 'extra_no_send',
                                                   width: 295,
                                                   height: 225,
                                                   margin: '0 0 0 30',
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
                                      var elcm = Ext.getCmp('elcm').getValue();
                                      var email_group_id = Ext.getCmp('email_group_id').getValue();
                                      var checkbox1 = Ext.getCmp('checkbox1').getValue();
                                      var extra_send = Ext.getCmp('extra_send').getValue();
                                      var extra_no_send = Ext.getCmp('extra_no_send').getValue();

                                      if (elcm == 0 && email_group_id == 0 && checkbox1 == 0 && checkbox1 == false && extra_send == "" && extra_no_send == "") {
                                          Ext.Msg.alert("提示信息", "當前所選條件未能得出有效信箱！");
                                      }
                                      else {
                                          var myMask = new Ext.LoadMask(Ext.getBody(), { msg: "Please wait..." });
                                          myMask.show();

                                          var schedule_date = Ext.getCmp('schedule_date').getValue(); //為空則null
                                          var expire_date = Ext.getCmp('expire_date').getValue();//為空則null
                                          var elcm = Ext.getCmp('elcm').getValue();//為空則null

                                          if (schedule_date == null) {
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
                                              timeout: 180000,
                                              params: {
                                                  testSend: 'false',
                                                  content_id: Ext.getCmp('content_id').getValue(),
                                                  group_id: Ext.getCmp('group_id').getValue(),
                                                  sender_email: Ext.getCmp('sender_email').getValue(),
                                                  sender_name: Ext.getCmp('sender_name').getValue(),
                                                  subject: Ext.getCmp('subject').getValue(),
                                                  body: Ext.getCmp('template_data_send').getValue(),
                                                  schedule_date: Ext.getCmp('schedule_date').getValue(),
                                                  expire_date: Ext.getCmp('expire_date').getValue(),
                                                  elcm_id: Ext.getCmp('elcm').getValue(),
                                                  extra_send: Ext.getCmp('extra_send').getValue(),
                                                  extra_no_send: Ext.getCmp('extra_no_send').getValue(),
                                                  is_outer: Ext.getCmp('checkbox1').getValue(),
                                                  email_group_id: Ext.getCmp('email_group_id').getValue(),
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
                                  }
                              },
               ],
           },
                ]
            }
        ]
    });

    var sendWin = Ext.create('Ext.window.Window', {
        title: '電子報發送',
        iconCls: 'icon-user-edit',
        id: 'sendWin',
        height: 550,
        width: 700,
        //   y: 100,
        layout: 'vbox',
        items: [subjectFrm,tabs1],
        constrain: true,
        closeAction: 'destroy',
        modal: true,
        //  resizable: false,
        labelWidth: 60,
        //bodyStyle: 'padding:5px 5px 5px 5px',
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
                Ext.getCmp('template_data_send').setValue(row.data.template_data_send);
            }
        }
    });
    sendWin.show();
}