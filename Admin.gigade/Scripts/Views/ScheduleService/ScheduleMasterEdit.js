/*************************************************************************************添加 編輯 框*************************************************************************************************/
editFunction_master = function (row, store) {
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
                
               // xtype: 'combobox',
                xtype: 'textfield',
                fieldLabel: '排程Code',
                id: 'schedule_code_master',
                name: 'schedule_code_master',
                allowBlank: false,

               // disabled: true,
               // editable: false,
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
                                schedule_code: Ext.htmlEncode(Ext.getCmp('schedule_code_master').getValue()),
                                schedule_name: Ext.htmlEncode(Ext.getCmp('schedule_name').getValue()),
                                schedule_api: Ext.htmlEncode(Ext.getCmp('schedule_api').getValue()),
                                schedule_description: Ext.htmlEncode(Ext.getCmp('schedule_description').getValue()),
                                schedule_state: Ext.htmlEncode(Ext.getCmp('schedule_state').getValue().ignore_stockVal),
                            },
                            success: function (form, action) {
                                var result = Ext.decode(action.response.responseText);
                                if (result.success) {
                                    Ext.Msg.alert(INFORMATION, "保存成功! ");
                                    ScheduleStore.load();
                                    editWin.close();
                                }
                                else
                                {
                                    if (result.msg == "3")
                                    {
                                        Ext.Msg.alert(INFORMATION, "排程Code已存在! ");
                                    }
                                    else
                                    {
                                        Ext.Msg.alert(INFORMATION, "保存失敗 ! ");
                                    }
                                    alert(result.msg);
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
        title: "排程服務",
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
                    Ext.getCmp('schedule_code_master').setDisabled(true);//排程code 新增時可以修改;編輯時不可修改
                    if (row.data.schedule_state == 0) {
                        Ext.getCmp("id1").setValue(false);
                        Ext.getCmp("id2").setValue(true);
                    } else {
                        Ext.getCmp("id1").setValue(true);
                        Ext.getCmp("id2").setValue(false);
                    }
                    editFrm.getForm().loadRecord(row);
                    Ext.getCmp('schedule_code_master').setValue(row.data.schedule_code);
                }
                else {
                    editFrm.getForm().reset();

                }
            }
        }
    });

    editWin.show();

}