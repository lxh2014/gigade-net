editFunction = function (row, store) {

    VipGroupStore.load();
    userLevelStore.load();
    var UserConditionFrm = Ext.create('Ext.form.Panel', {
        id: 'UserConditionFrm',
        frame: true,
        plain: true,
        constrain: true,
        defaultType: 'textfield',
        autoScroll: true,
        layout: 'anchor',
        labelWidth: 45,
        url: '/EventPromo/UserConditonAddOrEdit',
        defaults: { anchor: "99%" },
        items: [
        {
            xtype: 'textfield',
            fieldLabel: '編號',
            id: 'condition_id',
            name: 'condition_id',
            hidden: true
        },
        {
            xtype: 'textfield',
            id: 'condition_name',
            name: 'condition_name',
            fieldLabel: "會員條件名稱",
            allowBlank: false
        },
          {
              xtype: 'combobox',
              fieldLabel: "會員等級",
              editable: false,
              allowBlank: false,
              id: 'level_id',
              name: 'level_id',
              width: 400,
              store: userLevelStore,
              emptyText: '請選擇',
              displayField: 'ml_name',
              valueField: 'ml_code',
              typeAhead: true,
              lastQuery: '',
              forceSelection: false
          },
         {
             xtype: 'combobox', //會員群組
             fieldLabel: "會員群組",
             editable: false,
             allowBlank: false,
             id: 'group_id',
             name: 'group_id',
             hiddenName: 'group_id',
             store: VipGroupStore,
             displayField: 'group_name',
             valueField: 'group_id',
             typeAhead: true,
             lastQuery: '',
             forceSelection: false,
             value: "0"
         },
          {
              xtype: 'datetimefield',
              fieldLabel: "首次購買時間",
              // editable: false,
              format: 'Y-m-d H:i:s',
              time: { hour: 00, min: 00, sec: 00 },//開始時間00：00：00
              id: 'first_buy_time',
              name: 'first_buy_time'
          },
           {
               xtype: 'datetimefield',
               fieldLabel: "會員註冊日起",
               // editable: false,
               format: 'Y-m-d H:i:s',
               time: { hour: 00, min: 00, sec: 00 },//開始時間00：00：00
               id: 'reg_start',
               name: 'reg_time',
               listeners: {
                   select: function (a, b, c) {
                       var start = Ext.getCmp("reg_start");
                       var end = Ext.getCmp("reg_end");
                       if (end.getValue() == null) {
                           end.setValue(setNextMonth(start.getValue(), 1));
                       } else if (end.getValue() < start.getValue()) {
                           Ext.Msg.alert(INFORMATION, "日迄時間不能小於日起時間");
                           start.setValue(setNextMonth(end.getValue(), -1));
                       }
                   }
               }
           },
           {
               xtype: 'datetimefield',
               fieldLabel: "會員註冊日迄",
               format: 'Y-m-d H:i:s',
               time: { hour: 23, min: 59, sec: 59 },//標記結束時間23:59:59
               // editable: false,
               id: 'reg_end',
               name: 'reg_time',
               listeners: {
                   select: function (a, b, c) {
                       var start = Ext.getCmp("reg_start");
                       var end = Ext.getCmp("reg_end");
                       if (start.getValue() != "" && start.getValue() != null) {
                           if (end.getValue() < start.getValue()) {
                               Ext.Msg.alert(INFORMATION, "日迄時間不能小於日起時間");
                               end.setValue(setNextMonth(start.getValue(), 1));
                           }
                       }
                       else {
                           start.setValue(setNextMonth(end.getValue(), -1));
                       }
                   }
               }
           },
            {
                xtype: 'numberfield',
                fieldLabel: "最少消費次數",
                name: 'buy_times_min',
                id: 'buy_times_min',
                allowDecimals: false,
                minValue: 0,
                value: 0,
                listeners: {
                    'blur': function () {
                        var bmax = Ext.getCmp("buy_times_max").getValue();
                        var bmin = Ext.getCmp("buy_times_min").getValue();
                        if (bmax != 0) {
                            if (bmax <= bmin) {
                                Ext.Msg.alert(INFORMATION, "最多消費次數不能小於最少消費次數！");
                                Ext.getCmp("buy_times_max").setValue(0);
                            }
                        }
                    }
                }
            },
             {
                 xtype: 'numberfield',
                 fieldLabel: "最多消費次數",
                 name: 'buy_times_max',
                 id: 'buy_times_max',
                 allowDecimals: false,
                 minValue: 0,
                 value: 0,
                 listeners: {
                     'blur': function () {
                         var bmax = Ext.getCmp("buy_times_max").getValue();
                         var bmin = Ext.getCmp("buy_times_min").getValue();
                         if (bmax != 0) {
                             if (bmax <= bmin) {
                                 Ext.Msg.alert(INFORMATION, "最多消費次數不能小於最少消費次數！");
                                 Ext.getCmp("buy_times_max").setValue(0);
                             }
                         }
                     }
                 }
             },
              {
                  xtype: 'numberfield',
                  fieldLabel: "最低消費總金額",
                  name: 'buy_amount_min',
                  id: 'buy_amount_min',
                  allowDecimals: false,
                  minValue: 0,
                  value: 0,
                  listeners: {
                      'blur': function () {
                          var bmax = Ext.getCmp("buy_amount_max").getValue();
                          var bmin = Ext.getCmp("buy_amount_min").getValue();
                          if (bmax != 0) {
                              if (bmax <= bmin) {
                                  Ext.Msg.alert(INFORMATION, "最高消費總金額不能少於最低消費總金額！");
                                  Ext.getCmp("buy_amount_max").setValue(0);
                              }
                          }
                      }
                  }
              },
               {
                   xtype: 'numberfield',
                   fieldLabel: "最高消費總金額",
                   name: 'buy_amount_max',
                   id: 'buy_amount_max',
                   allowDecimals: false,
                   minValue: 0,
                   value: 0,
                   listeners: {
                       'blur': function () {
                           var bmax = Ext.getCmp("buy_amount_max").getValue();
                           var bmin = Ext.getCmp("buy_amount_min").getValue();
                           if (bmax != 0) {
                               if (bmax <= bmin) {
                                   Ext.Msg.alert(INFORMATION, "最高消費總金額不能少於最低消費總金額！");
                                   Ext.getCmp("buy_amount_max").setValue(0);
                               }
                           }
                       }
                   }
               }
        ],
        buttons: [
        {
            formBind: true,
            text: '保存',
            handler: function () {
                var form = this.up('form').getForm();
                if (form.isValid()) {
                    this.disable();
                    form.submit({
                        params: {
                            condition_ids: Ext.htmlEncode(Ext.getCmp('condition_id').getValue()),
                            condition_name: Ext.htmlEncode(Ext.getCmp('condition_name').getValue()),
                            level_id: Ext.htmlEncode(Ext.getCmp('level_id').getValue()),
                            group_id: Ext.htmlEncode(Ext.getCmp('group_id').getValue()),
                            first_buy_time: Ext.htmlEncode(Ext.getCmp('first_buy_time').getValue()),
                            reg_start: Ext.htmlEncode(Ext.Date.format(new Date(Ext.getCmp('reg_start').getValue()), 'Y-m-d H:i:s')),
                            reg_end: Ext.htmlEncode(Ext.Date.format(new Date(Ext.getCmp('reg_end').getValue()), 'Y-m-d H:i:s')),
                            buy_times_min: Ext.htmlEncode(Ext.getCmp('buy_times_min').getValue()),
                            buy_times_max: Ext.htmlEncode(Ext.getCmp('buy_times_max').getValue()),
                            buy_amount_min: Ext.htmlEncode(Ext.getCmp('buy_amount_min').getValue()),
                            buy_amount_max: Ext.htmlEncode(Ext.getCmp('buy_amount_max').getValue())
                        },
                        success: function (form, action) {
                            var result = Ext.decode(action.response.responseText);
                            if (result.success) {

                                Ext.Msg.alert(INFORMATION, SUCCESS);

                                UserConditionStore.load();
                                editUserConditionWin.close();
                            }
                            else {
                                Ext.Msg.alert(INFORMATION, FAILURE);
                            }
                        },
                        failure: function () {

                            Ext.Msg.alert(INFORMATION, FAILURE);
                        }
                    });
                }
            }
        }
        ]
    });
    var editUserConditionWin = Ext.create('Ext.window.Window', {
        title: '會員條件新增/編輯',
        iconCls: 'icon-user-edit',
        id: 'editUserConditionWin',
        width: 430,
        height: 430,
        layout: 'fit',
        items: [UserConditionFrm],
        constrain: true,
        closeAction: 'destroy',
        modal: true,
        labelWidth: 60,
        bodyStyle: 'padding:5px 5px 5px 5px',
        closable: false,
        tools: [
        {
            type: 'close',
            qtip: '是否關閉',
            handler: function (event, toolEl, panel) {
                Ext.MessageBox.confirm(CONFIRM, IS_CLOSEFORM, function (btn) {
                    if (btn == "yes") {
                        Ext.getCmp('editUserConditionWin').destroy();
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
                    UserConditionFrm.getForm().loadRecord(row);
                    if (row.data.reg_start == "1970-01-01 08:00:00")
                    {
                        Ext.getCmp("reg_start").setValue("");
                    }
                    if (row.data.reg_end == "1970-01-01 08:00:00") {
                        Ext.getCmp("reg_end").setValue("");
                    }
                    if (row.data.first_buy_time == "1970-01-01 08:00:00") {
                        Ext.getCmp("first_buy_time").setValue("");
                    }
                   
                }
                else {
                    UserConditionFrm.getForm().reset();
                }
            }
        }
    });
    editUserConditionWin.show();
}