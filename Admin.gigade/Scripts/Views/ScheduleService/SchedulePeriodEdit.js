/*************************************************************************************添加 編輯 框*************************************************************************************************/
editFunction_period= function (row, store) {
    var editFrm = Ext.create('Ext.form.Panel', {
        id: 'editFrm',
        frame: true,
        plain: true,
        constrain: true,
        autoScroll: true,
        layout: 'anchor',
        labelWidth: 45,
        url: '/ScheduleService/SaveSchedulePeriodInfo',
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
                fieldLabel: '執行頻率方式',
                id: 'period_type',
                name: 'period_type',
                allowBlank: false,
            },
            {
                xtype: 'textfield',
                fieldLabel: '執行頻率的倍數',
                id: 'period_nums',
                name: 'period_nums',
                allowBlank: false,
            },
             {
                 xtype: 'textfield',
                 fieldLabel: '當前已執行次數',
                 id: 'current_nums',
                 name: 'current_nums',
                 allowBlank: false,
             },
              {
                  xtype: 'textfield',
                  fieldLabel: '次數限制',
                  id: 'limit_nums',
                  name: 'limit_nums',
                  allowBlank: false,
              },
               //{
               //    xtype: 'fieldcontainer',
               //    layout: 'hbox',
               //    fieldLabel: '啟用時間',
               //    items: [
               //        {
               //            xtype: 'datefield',
               //            id: 'begin_datetime',
               //            name: 'begin_datetime',
               //            format: 'Y-m-d',
               //            editable: false,
               //            allowBlank: false,
               //        },
               //        {
               //            xtype: 'displayfield',
               //            value: '<span style="color:red">※此為啟用時間</span>'
               //        }
               //    ]
               //},
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
                                period_type: Ext.htmlEncode(Ext.getCmp('period_type').getValue()),
                                period_nums: Ext.htmlEncode(Ext.getCmp('period_nums').getValue()),
                                current_nums: Ext.htmlEncode(Ext.getCmp('current_nums').getValue()),
                                limit_nums: Ext.htmlEncode(Ext.getCmp('limit_nums').getValue()),
                                //begin_datetime: Ext.htmlEncode(Ext.Date.format(new Date(Ext.getCmp('begin_datetime').getValue()), 'Y-m-d H:i:s')),
                            },
                            success: function (form, action) {
                                var result = Ext.decode(action.response.responseText);
                                if (result.success) {
                                    Ext.Msg.alert(INFORMATION, "保存成功! ");
                                   // store.load();
                                     Schedule_Period_Store.load();
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
                    editFrm.getForm().loadRecord(row);
                    //initRow(row);
                }
                else {
                    editFrm.getForm().reset();
                }
            }
        }
    });

    editWin.show();

}