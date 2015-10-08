/*************************************************************************************添加 編輯 框*************************************************************************************************/
editFunction = function (row, store) {
    var editFrm = Ext.create('Ext.form.Panel', {
        id: 'editFrm',
        frame: true,
        plain: true,
        constrain: true,
        autoScroll: true,
        layout: 'anchor',
        labelWidth: 45,
        url: '/ScheduleService/SaveScheduleMasterInfo',
        defaults: { anchor: "95%", msgTarget: "side" },
        items: [
            {
                xtype: 'displayfield',
                fieldLabel: 'rowid',
                id: 'rowid',
                name: 'rowid',
                hidden: true
            },
            {
                xtype: 'textfield',
                fieldLabel: '排程Code',
                id: 'schedule_code',
                name: 'schedule_code',
                allowBlank: false,
            },
            {
                xtype: 'textfield',
                fieldLabel: '排程名稱',
                id: 'schedule_name',
                name: 'schedule_name',
                allowBlank: false,
            },
            {
                xtype: 'textfield',
                fieldLabel: 'contriller/action',
                id: 'schedule_api',
                name: 'schedule_api',
                allowBlank: false,
            },
             {
                 xtype: 'textfield',
                 fieldLabel: '排程描述',
                 id: 'schedule_description',
                 name: 'schedule_description',
                 allowBlank: false,
             },
              //{
              //    xtype: 'textfield',
              //    fieldLabel: '排程狀態',
              //    id: 'schedule_state',
              //    name: 'schedule_state'
              //},
                //{
                //    xtype: 'textfield',
                //    fieldLabel: '下次執行的記錄',
                //    id: 'schedule_period_id',
                //    name: 'schedule_period_id',
                //    allowBlank: false,
                //},
              {
                  xtype: 'fieldcontainer',
                  combineErrors: true,
                  layout: 'hbox',
                  items: [
                  {
                      fieldLabel: '排程狀態(是否啟用)',
                      xtype: 'radiogroup',
                      id: 'schedule_state',
                      labelWidth: 130,
                      width: 260,
                      defaults: {
                          name: 'ignore_stockVal'
                      },
                      columns: 2,
                      items: [
                          { id: 'id1', boxLabel: "是", inputValue: '1', checked: true },
                      //{ id: 'id1', boxLabel: "是", inputValue: '1' },
                      { id: 'id2', boxLabel: "否", inputValue: '0' }
                      ]
                  }
                  ]
              },

               //{
               //    xtype: 'textfield',
               //    fieldLabel: '創建人',
               //    id: 'create_user',
               //    name: 'create_user'
               //},
               // {
               //     xtype: 'textfield',
               //     fieldLabel: '修改人',
               //     id: 'change_user',
               //     name: 'change_user'
               // },

                  //{
                  //    xtype: 'fieldcontainer',
                  //    layout: 'hbox',
                  //    fieldLabel: '上次執行時間',
                  //    items: [
                  //        {
                  //            xtype: 'datefield',
                  //            id: 'previous_execute_time',
                  //            name: 'previous_execute_time',
                  //            format: 'Y-m-d',
                  //            editable: false,
                  //            allowBlank: false,
                  //        },
                  //        {
                  //            xtype: 'displayfield',
                  //            value: '<span style="color:red">※上次執行時間</span>'
                  //        }
                  //    ]
                  //},
                  //   {
                  //       xtype: 'fieldcontainer',
                  //       layout: 'hbox',
                  //       fieldLabel: '下次執行時間',
                  //       items: [
                  //           {
                  //               xtype: 'datefield',
                  //               id: 'next_execute_time',
                  //               name: 'next_execute_time',
                  //               format: 'Y-m-d',
                  //               editable: false,
                  //               allowBlank: false,
                  //           },
                  //           {
                  //               xtype: 'displayfield',
                  //               value: '<span style="color:red">※下次執行時間</span>'
                  //           }
                  //       ]
                  //   },
                  //   {
                  //       xtype: 'fieldcontainer',
                  //       layout: 'hbox',
                  //       fieldLabel: '創建日期',
                  //       items: [
                  //           {
                  //               xtype: 'datefield',
                  //               id: 'create_time',
                  //               name: 'create_time',
                  //               format: 'Y-m-d',
                  //               editable: false,
                  //               allowBlank: false,
                  //           },
                  //           {
                  //               xtype: 'displayfield',
                  //               value: '<span style="color:red">※創建日期</span>'
                  //           }
                  //       ]
                  //   },
                  //     {
                  //         xtype: 'fieldcontainer',
                  //         layout: 'hbox',
                  //         fieldLabel: '更改日期',
                  //         items: [
                  //             {
                  //                 xtype: 'datefield',
                  //                 id: 'change_time',
                  //                 name: 'change_time',
                  //                 format: 'Y-m-d',
                  //                 editable: false,
                  //                 allowBlank: false,
                  //             },
                  //             {
                  //                 xtype: 'displayfield',
                  //                 value: '<span style="color:red">※更改日期</span>'
                  //             }
                  //         ]
                  //     },
        ],

        // 点击保存按钮后  提示信息 
        buttons: [
            {
                formBind: true,
                disabled: true,
                text: '保存',
                handler: function () {
                    var form = this.up('form').getForm();
                    if (form.isValid()) {
                        form.submit({
                            params: {
                                rowid: Ext.htmlEncode(Ext.getCmp('rowid').getValue()),
                                schedule_code: Ext.htmlEncode(Ext.getCmp('schedule_code').getValue()),
                                schedule_name: Ext.htmlEncode(Ext.getCmp('schedule_name').getValue()),
                                schedule_api: Ext.htmlEncode(Ext.getCmp('schedule_api').getValue()),
                                schedule_description: Ext.htmlEncode(Ext.getCmp('schedule_description').getValue()),
                                schedule_state: Ext.htmlEncode(Ext.getCmp('schedule_state').getValue().ignore_stockVal),
                                //create_user: Ext.htmlEncode(Ext.getCmp('create_user').getValue()),
                                //change_user: Ext.htmlEncode(Ext.getCmp('change_user').getValue()),
                               // schedule_period_id: Ext.htmlEncode(Ext.getCmp('schedule_period_id').getValue()),
                                //previous_execute_time: Ext.htmlEncode(Ext.Date.format(new Date(Ext.getCmp('previous_execute_time').getValue()), 'Y-m-d H:i:s')),
                                //next_execute_time: Ext.htmlEncode(Ext.Date.format(new Date(Ext.getCmp('next_execute_time').getValue()), 'Y-m-d H:i:s')),
                                //create_time: Ext.htmlEncode(Ext.Date.format(new Date(Ext.getCmp('create_time').getValue()), 'Y-m-d H:i:s')),
                                //change_time: Ext.htmlEncode(Ext.Date.format(new Date(Ext.getCmp('change_time').getValue()), 'Y-m-d H:i:s')),
                            },
                            success: function (form, action) {
                                var result = Ext.decode(action.response.responseText);
                                if (result.success) {
                                    Ext.Msg.alert(INFORMATION, "保存成功! ");
                                    //store.load();
                                    ScheduleStore.load();
                                    editWin.close();
                                }
                                else {
                                    Ext.Msg.alert(INFORMATION, "保存失敗! ");
                                    editWin.close();
                                }
                            },
                            failure: function () {
                                Ext.Msg.alert(INFORMATION, "保存失敗! ");
                                editWin.close();
                            }
                        });
                    }
                }
            },
        ]
    });


    //点击关闭按钮后  提示信息
    //一个指定的打算作为一个应用程序窗口的面板。
    var editWin = Ext.create('Ext.window.Window', {
        title: "新增信息",
        id: 'editWin',
        iconCls: "icon-user-add",
        width: 460,
        height: 260,
        layout: 'fit',//布局样式
        items: [editFrm],
        constrain: true, //束縛窗口在框架內
        closeAction: 'destroy',
        modal: true,
        resizable: false,//false 禁止調整windows窗體的大小
        // reaizable:true,// true  允許調整windows窗體的大小
        labelWidth: 60,
        bodyStyle: 'padding:5px 5px 5px 5px',
        closable: false,
        tools: [
         {
             type: 'close',
             qtip: "關閉窗口",
             handler: function (event, toolEl, panel) {
                 Ext.MessageBox.confirm("提示信息", "是否關閉窗口", function (btn) {
                     if (btn == "yes") {
                         Ext.getCmp('editWin').destroy();
                     }
                     else {
                         return false;
                     }
                 });
             }
         }],
        listeners: {
            'show': function () {
                if (row) {
                    if (row.data.schedule_state == 0) {
                        Ext.getCmp("id1").setValue(false);
                        Ext.getCmp("id2").setValue(true);
                    } else {
                        Ext.getCmp("id1").setValue(true);
                        Ext.getCmp("id2").setValue(false);
                    }
                    editFrm.getForm().loadRecord(row);

                }
                else {

                    editFrm.getForm().reset();

                }
            }
        }
    });

    editWin.show();

}